/******************************************************************************\
 * IceChat 2009 Internet Relay Chat Client
 *
 * Copyright (C) 2010 Paul Vanderzee <snerf@icechat.net>
 *                                    <www.icechat.net> 
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
 *
 * Please consult the LICENSE.txt file included with this project for
 * more details
 *
\******************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Net.Security;
using System.Text.RegularExpressions;

namespace IceChat
{
    public partial class IRCConnection
    {
        private Socket serverSocket = null;
        
        private string dataBuffer;

        private bool disconnectError = false;
        private bool attemptReconnect = true;

        private System.Timers.Timer reconnectTimer;
        private System.Timers.Timer buddyListTimer;
        private int buddiesIsOnSent = 0;

        private ServerSetting serverSetting;
        private bool fullyConnected = false;
        private ArrayList commandQueue;
        private ArrayList ircTimers;

        private System.Timers.Timer pongTimer;
        
        private int whichAddressinList = 1;
        private int whichAddressCurrent = 1;
        private int totalAddressinDNS = 0;
        //private const int bytesperlong = 4; // 32 / 8
        //private const int bitsperbyte = 8;
        
        //private SslStream sslStream;
        //private NetworkStream socketStream;
        private bool proxyAuthed;

        public IRCConnection(ServerSetting ss)
        {
            dataBuffer = "";
            commandQueue = new ArrayList();
            serverSetting = ss;
            
            reconnectTimer = new System.Timers.Timer(30000);            
            reconnectTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnReconnectTimerElapsed);

            buddyListTimer = new System.Timers.Timer(60000);
            buddyListTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnBuddyListTimerElapsed);            

            pongTimer = new System.Timers.Timer(60000 * serverSetting.PongTimerMinutes);    //15 minutes
            pongTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnPongTimerElapsed);
            
            ircTimers = new ArrayList();
        }

        public void Dispose()
        {
            try
            {
                if (serverSocket != null)
                    serverSocket.Disconnect(false);

            }
            catch
            {
            }

            reconnectTimer.Stop();
            reconnectTimer.Dispose();
        }

        private void BuddyListCheck()
        {
            if (serverSetting.BuddyListEnable)
            {
                string ison = string.Empty;

                foreach (BuddyListItem buddy in serverSetting.BuddyList)
                {
                    if (ison.Length > 200)
                        break;
                    else if (!buddy.IsOnSent)
                    {
                        if (!buddy.Nick.StartsWith(";"))
                        {
                            ison += " " + buddy.Nick;
                            buddy.IsOnSent = true;
                        }
                        buddiesIsOnSent++;
                    }
                }

                if (ison != null)
                {
                    ison = "ISON" + ison;
                    SendData(ison);
                }

                buddyListTimer.Stop();
            }
            else
                FormMain.Instance.BuddyList.ClearBuddyList(this);

        }

        private void OnBuddyListTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            BuddyListCheck();
        }

        private void OnReconnectTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (attemptReconnect)
            {
                whichAddressinList++;

                //reconnect
                if (ServerMessage != null)
                {
                    string msg = FormMain.Instance.GetMessageFormat("Server Reconnect");
                    msg = msg.Replace("$serverip", serverSetting.ServerIP).Replace("$server", serverSetting.ServerName).Replace("$port", serverSetting.ServerPort);
                    ServerMessage(this, msg);
                }

                ConnectSocket();
            }
        }

        private void OnPongTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                //pong has not received, re-connect server
                //disable for the time being
                //ServerError(this, "No Pong Message Received in " + serverSetting.PongTimerMinutes + " Minutes, Reconnecting");
                //send a ping to the server
                SendData("PING :" + this.serverSetting.RealServerName);
                //ForceDisconnect();
                //attemptReconnect = true;
                //pongTimer.Stop();
                //reconnectTimer.Start();            
            
            }
            catch(SocketException ee)
            {
                System.Diagnostics.Debug.WriteLine(ee.Message + ":" + ee.StackTrace);
            }
        }


        #region Public Properties and Methods

        internal void AddToCommandQueue(string command)
        {
            commandQueue.Add(command);
        }

        internal ServerSetting ServerSetting
        {
            get
            {
                return serverSetting;
            }
            set
            {
                serverSetting = value;
            }
        }

        internal bool IsConnected
        {
            get
            {
                if (serverSocket == null)
                    return false;

                if (serverSocket.Connected)
                    return true;
                else
                    return false;
            }

        }

        internal bool DisconnectError
        {
            get
            {
                return disconnectError;
            }
        }

        internal bool IsFullyConnected
        {
            get
            {
                return fullyConnected;
            }
        }
        
        internal bool AttemptReconnect
        {
            get { return attemptReconnect; }
            set { attemptReconnect = value; }
        }

        #endregion

        #region Socket Events and Methods

        /// <summary>
        /// Event for Server Disconnection
        /// </summary>
        /// <param name="ar"></param>
        private void OnDisconnect(IAsyncResult ar)
        {
            Socket client = (Socket)ar.AsyncState;
            client.EndDisconnect(ar);

            string msg = FormMain.Instance.GetMessageFormat("Server Disconnect");
            msg = msg.Replace("$serverip", serverSetting.ServerIP);
            msg = msg.Replace("$port", serverSetting.ServerPort);
            if (serverSetting.RealServerName.Length > 0)
                msg = msg.Replace("$server", serverSetting.RealServerName);
            else
                msg = msg.Replace("$server", serverSetting.ServerName);

            foreach (IceTabPage t in FormMain.Instance.TabMain.TabPages)
            {
                if (t.WindowStyle == IceTabPage.WindowType.Channel || t.WindowStyle == IceTabPage.WindowType.Query)
                {
                    if (t.Connection == this)
                    {
                        t.ClearNicks();
                        t.IsFullyJoined = false;

                        t.TextWindow.AppendText(msg, 1);
                        t.LastMessageType = FormMain.ServerMessageType.ServerMessage;

                        if (FormMain.Instance.CurrentWindow == t)
                            FormMain.Instance.NickList.Header = t.TabCaption + ":0";                            
                    }
                }
            }
            ServerMessage(this, msg);

            FormMain.Instance.ServerTree.Invalidate();

            if (FormMain.Instance.CurrentWindowType == IceTabPage.WindowType.Console)
                if (FormMain.Instance.InputPanel.CurrentConnection == this)
                    FormMain.Instance.StatusText(serverSetting.NickName + " disconnected (" + serverSetting.ServerName + ")");

            serverSocket = null;
            serverSetting.ConnectedTime = DateTime.Now;

            commandQueue.Clear();
            
            buddyListTimer.Stop();
            FormMain.Instance.BuddyList.ClearBuddyList(this);
            if (serverSetting.BuddyList != null)
            {
                foreach (BuddyListItem buddy in serverSetting.BuddyList)
                {
                    buddy.Connected = false;
                    buddy.PreviousState = false;
                    buddy.IsOnSent = false;
                    buddy.IsOnReceived = false;
                }
            }
            FormMain.Instance.PlaySoundFile("dropped");

            initialLogon = false;
            triedAltNickName = false;
            fullyConnected = false;

            serverSetting.IAL.Clear();
            serverSetting.Away = false;
            serverSetting.RealServerName = "";

            pongTimer.Stop();

            //disable and remove all timers
            foreach (object key in ircTimers)
            {
                ((IrcTimer)key).DisableTimer();
            }
            ircTimers.Clear();

            if (disconnectError && attemptReconnect && FormMain.Instance.IceChatOptions.ReconnectServer)
            {
                //reconnect
                if (ServerMessage != null)
                    ServerMessage(this, "Waiting 30 seconds to Reconnect to (" + serverSetting.ServerName + ")");
                disconnectError = false;
                reconnectTimer.Start();
            }

        }

        /// <summary>
        /// Event for Server Connection
        /// </summary>
        /// <param name="ar"></param>
        private void OnConnectionReady(IAsyncResult ar)
        {
            reconnectTimer.Stop();
            attemptReconnect = true;

            if (serverSocket == null)
            {
                if (ServerError != null)
                    ServerError(this, "Null Socket - Can not Connect");
                return;
            }
            
            try
            {
                serverSocket.EndConnect(ar);
                //serverSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, 1);
                //SetKeepAlive(serverSocket, 180 * 1000, 540 * 1000); //check every 5 minutes, max of 15 mins inactivity
            }            
            catch (Exception e)
            {
                if (ServerError != null)
                    ServerError(this, "Socket Exception Error:" + e.Message.ToString());

                disconnectError = true;
                ForceDisconnect();
                return;
            }

            SocketPacket thisSocket = new SocketPacket(serverSocket);

            try
            {
                thisSocket.workSocket.BeginReceive(thisSocket.dataBuffer, 0, thisSocket.dataBuffer.Length, 0, new AsyncCallback(OnReceivedData), thisSocket);

                this.serverSetting.ConnectedTime = DateTime.Now;

                FormMain.Instance.ServerTree.Invalidate();

                if (serverSetting.UseProxy)
                {
                    byte[] d = new byte[256];
                    ushort nIndex = 0;
                    d[nIndex++] = 0x05;

                    if (serverSetting.ProxyUser.Length > 0)
                    {
                        d[nIndex++] = 0x02;
                        d[nIndex++] = 0x00;
                        d[nIndex++] = 0x02;
                    }
                    else
                    {
                        d[nIndex++] = 0x01;
                        d[nIndex++] = 0x00;
                    }

                    try
                    {
                        serverSocket.BeginSend(d, 0, nIndex, SocketFlags.None, new AsyncCallback(OnSendData), serverSocket);

                        if (ServerMessage != null)
                            ServerMessage(this, "Socks 5 Connection Established with " + serverSetting.ProxyIP);
                    }
                    catch (SocketException)
                    {
                        System.Diagnostics.Debug.WriteLine("Error Sending Proxy Data");
                    }
                    catch (Exception)
                    {
                        System.Diagnostics.Debug.WriteLine("proxy exception");
                    }
                }
                else
                {
                    if (serverSetting.Password != null && serverSetting.Password.Length > 0)
                        SendData("PASS " + serverSetting.Password);

                    //send the USER / NICK stuff
                    SendData("NICK " + serverSetting.NickName);
                    SendData("USER " + serverSetting.IdentName + " \"localhost\" \"" + serverSetting.ServerName + "\" :" + serverSetting.FullName);

                    whichAddressinList = whichAddressCurrent;

                    if (ServerMessage != null)
                        ServerMessage(this, "Sending User Registration Information");
                    
                    this.pongTimer.Start();
                }


            }
            catch (SocketException se)
            {
                if (ServerError != null)
                    ServerError(this, "Socket Exception Error OnConnectionReady:" + se.Message.ToString() + ":" + se.ErrorCode);

                disconnectError = true;
                ForceDisconnect();
            }
            catch (Exception e)
            {
                if (ServerError != null)
                    ServerError(this, "Exception Error OnConnectionReady:" + e.Message.ToString());

                disconnectError = true;
                ForceDisconnect();
            }
        }
        /*
        private bool SetKeepAlive(Socket sock, ulong time, ulong interval)
        {
            try
            {
                // resulting structure
                byte[] SIO_KEEPALIVE_VALS = new byte[3 * bytesperlong];

                // array to hold input values
                ulong[] input = new ulong[3];

                // put input arguments in input array
                if (time == 0 || interval == 0) // enable disable keep-alive
                    input[0] = (0UL); // off
                else
                    input[0] = (1UL); // on

                input[1] = (time); // time millis
                input[2] = (interval); // interval millis

                // pack input into byte struct
                for (int i = 0; i < input.Length; i++)
                {
                    SIO_KEEPALIVE_VALS[i * bytesperlong + 3] = (byte)(input[i] >> ((bytesperlong - 1) * bitsperbyte) & 0xff);
                    SIO_KEEPALIVE_VALS[i * bytesperlong + 2] = (byte)(input[i] >> ((bytesperlong - 2) * bitsperbyte) & 0xff);
                    SIO_KEEPALIVE_VALS[i * bytesperlong + 1] = (byte)(input[i] >> ((bytesperlong - 3) * bitsperbyte) & 0xff);
                    SIO_KEEPALIVE_VALS[i * bytesperlong + 0] = (byte)(input[i] >> ((bytesperlong - 4) * bitsperbyte) & 0xff);
                }
                // create bytestruct for result (bytes pending on server socket)
                byte[] result = BitConverter.GetBytes(0);
                // write SIO_VALS to Socket IOControl
                sock.IOControl(IOControlCode.KeepAliveValues, SIO_KEEPALIVE_VALS, result);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        */
        /// <summary>
        /// Function for sending RAW IRC Data to the Server Connection
        /// </summary>
        /// <param name="data"></param>
        internal void SendData(string data)
        {
            //check if the socket is still connected
            if (serverSocket == null)
            {
                if (ServerError != null)
                    ServerError(this, "Error: You are not Connected (Socket not created) - Can not send");
                return;
            }   
            
            //get the proper encoding            
            byte[] bytData = Encoding.GetEncoding(serverSetting.Encoding).GetBytes(data + "\r\n");
            if (bytData.Length > 0)
            {                
                if (serverSocket.Connected)
                {
                    try
                    {
                        serverSocket.BeginSend(bytData, 0, bytData.Length, SocketFlags.None, new AsyncCallback(OnSendData), serverSocket);
                        //raise an event for the debug window
                        if (RawServerOutgoingData != null)
                            RawServerOutgoingData(this, data);
                    }
                    catch (SocketException se)
                    {
                        //some kind of a socket error
                        if (ServerError != null)
                            ServerError(this, "You are not Connected - Can not send:" + se.Message);

                        disconnectError = true;
                        ForceDisconnect();
                    }
                }
                else
                {
                    if (ServerError != null)
                        ServerError(this, "You are not Connected (Socket Disconnected) - Can not send:" + data);

                    disconnectError = true;
                    ForceDisconnect();
                }
            }
        }
       
        /// <summary>
        /// Event fire when Data needs to be sent to the Server Connection
        /// </summary>
        private void OnSendData(IAsyncResult ar)
        {
            Socket handler = (Socket)ar.AsyncState;
            
            try
            {
                int bytesSent = handler.EndSend(ar);
            }
            catch (Exception e)
            {
                if (ServerError != null)
                    ServerError(this, "SendData Error:" + e.Message.ToString());

                disconnectError = true;
                ForceDisconnect();
            }
        }

        /// <summary>
        /// Event fired when data is received from the Server Connection
        /// </summary>
        /// <param name="ar"></param>
        private void OnReceivedData(IAsyncResult ar)
        {
            SocketPacket handler = (SocketPacket)ar.AsyncState;
            
            try
            {
                int size = handler.workSocket.EndReceive(ar);
                if (serverSetting.UseProxy && !proxyAuthed)
                {
                    System.Diagnostics.Debug.WriteLine("recv:" + size);

                    Decoder d = Encoding.GetEncoding(serverSetting.Encoding).GetDecoder();
                    char[] chars = new char[size];
                    int charLen = d.GetChars(handler.dataBuffer, 0, size, chars, 0);
                    
                    if (size == 2)
                    {
                        System.Diagnostics.Debug.WriteLine("got:" + (int)chars[0] + ":" + (int)chars[1]);
                        
                        if (chars[1] == 0xFF)
                        {
                            if (ServerError != null)
                                ServerError(this, "Proxy Server Error: None of the authentication method was accepted by proxy server.");
                            ForceDisconnect();
                        }
                        else if (chars[1] == 0x00)  //send proxy information
                        {
                            byte[] proxyData = new byte[7 + serverSetting.ServerName.Length];
                            proxyData[0] = 0x05;
                            proxyData[1] = 0x01;
                            proxyData[2] = 0x00;
                            proxyData[3] = 0x03;
                            proxyData[4] = Convert.ToByte(serverSetting.ServerName.Length);
                            byte[] rawBytes = new byte[serverSetting.ServerName.Length];
                            rawBytes = Encoding.Default.GetBytes(serverSetting.ServerName);
                            rawBytes.CopyTo(proxyData, 5);
                            proxyData[proxyData.Length -2] = (byte)((Convert.ToInt32(serverSetting.ServerPort) & 0xFF00) >> 8);
                            proxyData[proxyData.Length -1] = (byte)(Convert.ToInt32(serverSetting.ServerPort) & 0xFF);
                            ServerMessage(this, "Sending Proxy Verification");
                            serverSocket.BeginSend(proxyData, 0, proxyData.Length, SocketFlags.None , new AsyncCallback(OnSendData), serverSocket);                            
                            handler.workSocket.BeginReceive(handler.dataBuffer, 0, handler.dataBuffer.Length, 0, new AsyncCallback(OnReceivedData), handler);                            
                        }
                        else if (chars[1] == 0x02)  //send proxy information with user/pass
                        {
                            ushort nIndex = 0;

                            byte[] proxyData = new byte[256];
                            proxyData[nIndex++] = 0x05;
                            proxyData[nIndex++] = 0x01;
                            proxyData[nIndex++] = 0x00;
                            proxyData[nIndex++] = 0x03;
                            proxyData[nIndex++] = Convert.ToByte(serverSetting.ServerName.Length);

                            byte[] rawBytes = new byte[256];
                            rawBytes = Encoding.ASCII.GetBytes(serverSetting.ServerName);
                            rawBytes.CopyTo(proxyData, nIndex);
                            nIndex += (ushort)rawBytes.Length;

                            proxyData[proxyData.Length - 2] = (byte)((Convert.ToInt32(serverSetting.ServerPort) & 0xFF00) >> 8);
                            proxyData[proxyData.Length - 1] = (byte)(Convert.ToInt32(serverSetting.ServerPort) & 0xFF);
                            ServerMessage(this, "Sending Proxy Verification (user/pass)");
                            serverSocket.BeginSend(proxyData, 0, nIndex, 0, new AsyncCallback(OnSendData), serverSocket);
                            
                            handler.workSocket.BeginReceive(handler.dataBuffer, 0, handler.dataBuffer.Length, 0, new AsyncCallback(OnReceivedData), handler);
                        }
                    }
                    else if (size == 10)
                    {
                        System.Diagnostics.Debug.WriteLine("got10:" + (int)chars[0] + ":" + (int)chars[1]);
                        ServerMessage(this, "Socks 5 Connection Successfull");
                        SendData("NICK " + serverSetting.NickName);
                        SendData("USER " + serverSetting.IdentName + " \"localhost\" \"" + serverSetting.ServerName + "\" :" + serverSetting.FullName);
                        ServerMessage(this, "Sending User Registration Information");
                        //serverSetting.UseProxy = false;
                        proxyAuthed = true;
                        handler.workSocket.BeginReceive(handler.dataBuffer, 0, handler.dataBuffer.Length, 0, new AsyncCallback(OnReceivedData), handler);
                    }
                }
                else
                {
                    if (size > 0)
                    {
                        Decoder d = Encoding.GetEncoding(serverSetting.Encoding).GetDecoder();
                        char[] chars = new char[size];
                        int charLen = d.GetChars(handler.dataBuffer, 0, size, chars, 0);
                        System.String strData = new System.String(chars);

                        if (strData.Length != charLen)  //removes any trailing null characters
                            strData = strData.Substring(0, charLen);

                        strData = strData.Replace("\r", string.Empty);

                        if (!strData.EndsWith("\n"))
                        {
                            //create a buffer
                            dataBuffer += strData;
                            handler.workSocket.BeginReceive(handler.dataBuffer, 0, handler.dataBuffer.Length, 0, new AsyncCallback(OnReceivedData), handler);
                            return;
                        }

                        if (dataBuffer.Length > 0)
                        {
                            strData = dataBuffer + strData;
                            dataBuffer = string.Empty;
                        }

                        //split into lines and stuff
                        if (strData.IndexOf('\n') > -1)
                        {
                            string[] Data = strData.Split('\n');
                            foreach (string Line in Data)
                            {
                                if (Line.Length > 0)
                                    ParseData(Line);
                            }
                        }
                        else
                        {
                            if (strData.Length > 0)
                                ParseData(strData);
                        }

                        handler.workSocket.BeginReceive(handler.dataBuffer, 0, handler.dataBuffer.Length, 0, new AsyncCallback(OnReceivedData), handler);
                    }
                    else
                    {
                        //connection lost                    
                        if (ServerError != null)
                            ServerError(this, "Connection Lost");

                        disconnectError = true;
                        ForceDisconnect();
                    }
                }
            }                        
            catch (SocketException se)
            {
                ServerError(this, "Socket Exception OnReceiveData Error:" + se.Source + ":" + se.Message.ToString());
                disconnectError = true;
                ForceDisconnect();
            }
            catch (Exception e)
            {
                ServerError(this, "Exception OnReceiveData Error:" + e.Source + ":" + e.Message.ToString() + ":" + e.StackTrace);
                disconnectError = true;
                ForceDisconnect();
            }                 
        }
        
        /// <summary>
        /// Method for starting a Server Connection
        /// </summary>
        internal void ConnectSocket()
        {
            disconnectError = false;

            IPHostEntry hostEntry = null;
            
            try
            {
                // Get host related information.
                if (serverSetting.UseProxy)
                {
                    whichAddressCurrent = 1;
                    totalAddressinDNS = 1;
                    IPAddress proxyIP = null;

                    try
                    {
                        proxyIP = IPAddress.Parse(serverSetting.ProxyIP);
                    }
                    catch (FormatException)
                    {
                        //proxyIP = Dns.GetHostByAddress(serverSetting.ProxyIP).AddressList[0];
                    }

                    try
                    {
                        IPEndPoint proxyEndPoint = new IPEndPoint(proxyIP, Convert.ToInt32(serverSetting.ProxyPort));
                        Socket proxySocket = new Socket(proxyEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                        if (ServerMessage != null)
                        {
                            string msg = FormMain.Instance.GetMessageFormat("Server Connect");
                            msg = msg.Replace("$serverip", proxyIP.ToString()).Replace("$server", serverSetting.ProxyIP).Replace("$port", serverSetting.ProxyPort);
                            ServerMessage(this, msg);
                        }

                        serverSocket = proxySocket;
                        proxySocket.BeginConnect(proxyEndPoint, new AsyncCallback(OnConnectionReady), null);
                    }
                    catch (SocketException)
                    {
                        System.Diagnostics.Debug.WriteLine("Socket Exception Proxy Connect");
                    }
                    catch (Exception)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception Proxy Connect");
                    }
                }
                else
                {
                    hostEntry = Dns.GetHostEntry(serverSetting.ServerName);

                    whichAddressCurrent = 1;
                    totalAddressinDNS = hostEntry.AddressList.Length;

                    if (whichAddressinList > totalAddressinDNS)
                        whichAddressinList = 1;

                    IPAddress ipAddress = null;
                    if (IPAddress.TryParse(serverSetting.ServerName, out ipAddress))
                    {
                        IPEndPoint ipe = new IPEndPoint(ipAddress, Convert.ToInt32(serverSetting.ServerPort));
                        Socket tempSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                        if (ServerMessage != null)
                        {
                            string msg = FormMain.Instance.GetMessageFormat("Server Connect");
                            msg = msg.Replace("$serverip", ipAddress.ToString()).Replace("$server", serverSetting.ServerName).Replace("$port", serverSetting.ServerPort);
                            serverSetting.ServerIP = ipAddress.ToString();
                            ServerMessage(this, msg + " (" + whichAddressCurrent + "/" + hostEntry.AddressList.Length + ")");
                        }

                        serverSocket = tempSocket;
                        tempSocket.BeginConnect(ipe, new AsyncCallback(OnConnectionReady), null);
                        return;
                    }
                    
                    
                    // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
                    // an exception that occurs when the host IP Address is not compatible with the address family
                    // (typical in the IPv6 case).
                    foreach (IPAddress address in hostEntry.AddressList)
                    {
                        try
                        {
                            if (whichAddressCurrent == whichAddressinList)
                            {
                                IPEndPoint ipe = new IPEndPoint(address, Convert.ToInt32(serverSetting.ServerPort));
                                Socket tempSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                                if (ServerMessage != null)
                                {
                                    string msg = FormMain.Instance.GetMessageFormat("Server Connect");
                                    msg = msg.Replace("$serverip", address.ToString()).Replace("$server", serverSetting.ServerName).Replace("$port", serverSetting.ServerPort);
                                    serverSetting.ServerIP = address.ToString();
                                    ServerMessage(this, msg + " (" + whichAddressCurrent + "/" + hostEntry.AddressList.Length + ")");
                                }

                                serverSocket = tempSocket;
                                tempSocket.BeginConnect(ipe, new AsyncCallback(OnConnectionReady), null);

                                break;
                            }
                            whichAddressCurrent++;
                            if (whichAddressCurrent > hostEntry.AddressList.Length)
                                whichAddressCurrent = 1;
                        }
                        catch (Exception e)
                        {
                            if (ServerError != null)
                                ServerError(this, "Connect - Exception Error:" + e.Message.ToString());

                            whichAddressCurrent++;
                            if (whichAddressCurrent > hostEntry.AddressList.Length)
                                whichAddressCurrent = 1;

                            disconnectError = true;
                            ForceDisconnect();
                        }
                        finally
                        {
                            if (whichAddressCurrent > hostEntry.AddressList.Length)
                                whichAddressCurrent = 1;
                        }
                    }
                }
            }
            catch (SocketException se)
            {
                if (ServerError != null)
                    ServerError(this, "Socket Exception Error:" + se.Message);

                disconnectError = true;
                ForceDisconnect();
            }
        }

        internal void ForceDisconnect()
        {
            try
            {
                if (serverSocket != null)
                {
                    serverSocket.Shutdown(SocketShutdown.Both);
                    serverSocket.BeginDisconnect(false, new AsyncCallback(OnDisconnect), serverSocket);
                }
            }
            catch
            {
            }

            if (FormMain.Instance.IceChatOptions.ReconnectServer)
            {
                if (ServerMessage != null)
                    ServerMessage(this, "Waiting 30 seconds to Reconnect to (" + serverSetting.ServerName + ")");
                try
                {
                    if (reconnectTimer != null)
                        reconnectTimer.Start();
                }
                catch (Exception)
                {
                    //do nada
                }
            }

        }
        
        #endregion


        #region Timer Events

        internal void CreateTimer(string id, double interval, int reps, string command)
        {
            IrcTimer timer = new IrcTimer(id, interval * 1000, reps, command);
            timer.OnTimerElapsed += new IrcTimer.TimerElapsed(OnTimerElapsed);
            ircTimers.Add(timer);
            timer.Start();
        }

        internal void DestroyTimer(string id)
        {
            object remove = null;
            foreach (object key in ircTimers)
            {
                if (((IrcTimer)key).TimerID == id)
                {
                    ((IrcTimer)key).DisableTimer();
                    remove = key;
                }
            }
            if (remove != null)
                ircTimers.Remove(((IrcTimer)remove));

        }

        private void OnTimerElapsed(string command)
        {
            //System.Diagnostics.Debug.WriteLine("Timer Elapsed:" + command);
            FormMain.Instance.ParseOutGoingCommand(this, command);
        }
        
        #endregion
    }

    #region Socket Packet Class

    public class StateObject
    {
        public bool connected = false;	// ID received flag
        public Socket workSocket = null;	// Client socket.
        public Socket partnerSocket = null;	// Partner socket.
        public const int BufferSize = 1024;	// Size of receive buffer.
        public byte[] buffer = new byte[BufferSize];// Receive buffer.
        public StringBuilder sb = new StringBuilder();//Received data String.
        public string id = String.Empty;	// Host or conversation ID
        public DateTime TimeStamp;
    }

    public class SocketPacket
    {
        public SocketPacket(Socket s)
        {
            workSocket = s;
        }
        public SocketPacket()
        {
            //
        }
        public System.Net.Sockets.Socket workSocket;
        public byte[] dataBuffer = new byte[1024];
    }

    #endregion

}
