/******************************************************************************\
 * IceChat 9 Internet Relay Chat Client
 *
 * Copyright (C) 2011 Paul Vanderzee <snerf@icechat.net>
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
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace IceChat
{
    public partial class IRCConnection
    {
        private Socket serverSocket = null;
        private NetworkStream socketStream = null;
        private SslStream sslStream = null;

        private string dataBuffer = "";
        private Queue<string> sendBuffer;

        private bool disconnectError = false;
        private bool attemptReconnect = true;

        private System.Timers.Timer reconnectTimer;
        private System.Timers.Timer buddyListTimer;
        public int buddiesIsOnSent = 0;

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
        private byte[] readBuffer;
        private const int BUFFER_SIZE = 1024;

        public IRCConnection(ServerSetting ss)
        {
            commandQueue = new ArrayList();
            sendBuffer = new Queue<string>();
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

        public void BuddyListCheck()
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

                if (ison != string.Empty)
                {
                    ison = "ISON" + ison;
                    SendData(ison);
                }

                buddyListTimer.Stop();
            }
            else
                BuddyListClear(this);

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
                ServerReconnect(this);
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
            catch (SocketException se)
            {
                System.Diagnostics.Debug.WriteLine(se.Message);
            }
        }


        #region Public Properties and Methods

        public void AddToCommandQueue(string command)
        {
            commandQueue.Add(command);
        }

        public ServerSetting ServerSetting
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

        public System.Timers.Timer ReconnectTimer
        {
            get { return this.reconnectTimer; }
        }

        public bool IsConnected
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

        public bool DisconnectError
        {
            get
            {
                return disconnectError;
            }
            set
            {
                this.disconnectError = value;
            }
        }

        public bool IsFullyConnected
        {
            get
            {
                return fullyConnected;
            }
        }

        public bool AttemptReconnect
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

            ServerDisconnect(this);

            RefreshServerTree(this);
            StatusText(this, serverSetting.NickName + " disconnected (" + serverSetting.ServerName + ")");

            serverSocket = null;
            serverSetting.ConnectedTime = DateTime.Now;

            commandQueue.Clear();

            buddyListTimer.Stop();
            BuddyListClear(this);
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

            initialLogon = false;
            triedAltNickName = false;
            fullyConnected = false;

            if (serverSetting.IAL != null)
                serverSetting.IAL.Clear();
            
            serverSetting.Away = false;
            serverSetting.RealServerName = "";

            pongTimer.Stop();
            if (serverSetting.UseProxy)
                proxyAuthed = false;

            //disable and remove all timers
            foreach (object key in ircTimers)
            {
                ((IrcTimer)key).DisableTimer();
            }
            ircTimers.Clear();

            ServerForceDisconnect(this);

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
                    ServerError(this, "Null Socket - Can not Connect", false);
                return;
            }

            try
            {
                serverSocket.EndConnect(ar);
            }
            catch (SocketException se)
            {
                if (ServerError != null)
                    ServerError(this, "Socket Exception Error " + se.ErrorCode + ":" + se.Message, false);

                disconnectError = true;
                ForceDisconnect();
                return;
            }

            socketStream = new NetworkStream(serverSocket, true);
            if (serverSetting.UseSSL)
            {
                try
                {
                    sslStream = new SslStream(socketStream, true, this.RemoteCertificateValidationCallback);
                    sslStream.AuthenticateAsClient(serverSetting.ServerName);
                    ServerMessage(this, "*** You are connected to this server with " + sslStream.SslProtocol.ToString().ToUpper() + "-" + sslStream.CipherAlgorithm.ToString().ToUpper() + sslStream.CipherStrength + "-" + sslStream.HashAlgorithm.ToString().ToUpper() + "-" + sslStream.HashStrength + "bits");
                }
                catch (System.Security.Authentication.AuthenticationException ae)
                {
                    if (ServerError != null)
                        ServerError(this, "Authentication Error :" + ae.Message.ToString(), false);
                }
                catch (Exception e)
                {
                    if (ServerError != null)
                        ServerError(this, "Exception Error :" + e.Message.ToString(), false);
                }
            }

            try
            {
                if (serverSetting.UseSSL)
                {
                    if (sslStream != null && sslStream.CanRead)
                    {
                        readBuffer = new byte[BUFFER_SIZE];
                        sslStream.BeginRead(readBuffer, 0, readBuffer.Length, new AsyncCallback(OnReceivedData), socketStream);
                    }
                }
                else
                {
                    if (socketStream != null && socketStream.CanRead)
                    {
                        readBuffer = new byte[BUFFER_SIZE];
                        socketStream.BeginRead(readBuffer, 0, readBuffer.Length, new AsyncCallback(OnReceivedData), socketStream);
                    }
                }
                this.serverSetting.ConnectedTime = DateTime.Now;

                RefreshServerTree(this);

                if (serverSetting.UseProxy)
                {

                    //socks v5 code
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
                        socketStream.BeginWrite(d, 0, nIndex, new AsyncCallback(OnSendData), socketStream);

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

                    this.pongTimer.Start();
                }
            }

            catch (SocketException se)
            {
                System.Diagnostics.Debug.WriteLine("CODE:" + se.SocketErrorCode);
                System.Diagnostics.Debug.WriteLine("ST:"+ se.StackTrace);

                if (ServerError != null)
                    ServerError(this, "Socket Exception Error:" + se.Message.ToString() + ":" + se.ErrorCode, false);


                disconnectError = true;
                ForceDisconnect();
            }
            catch (Exception e)
            {
                if (ServerError != null)
                    ServerError(this, "Exception Error:" + e.Message.ToString(), false);

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
        public void SendData(string data)
        {
            //check if the socket is still connected
            if (serverSocket == null)
            {
                if (ServerError != null)
                    ServerError(this, "Error: You are not Connected (Socket not created) - Can not send", false);
                return;
            }

            if (socketStream == null)
            {
                System.Diagnostics.Debug.WriteLine("senddata null stream");
                return;
            }

            //get the proper encoding            
            byte[] bytData = Encoding.GetEncoding(serverSetting.Encoding).GetBytes(data + "\r\n");
            if (bytData.Length > 0)
            {
                if (socketStream.CanWrite)
                {
                    try
                    {
                        if (serverSetting.UseSSL)
                        {
                            sslStream.BeginWrite(bytData, 0, bytData.Length, new AsyncCallback(OnSendData), sslStream);
                        }
                        else
                            socketStream.BeginWrite(bytData, 0, bytData.Length, new AsyncCallback(OnSendData), socketStream);

                        //raise an event for the debug window
                        if (RawServerOutgoingData != null)
                            RawServerOutgoingData(this, data);
                    }
                    catch (SocketException se)
                    {
                        //some kind of a socket error
                        if (ServerError != null)
                            ServerError(this, "You are not Connected - Can not send:" + se.Message, false);

                        disconnectError = true;
                        ForceDisconnect();
                    }
                    catch (NotSupportedException)
                    {
                        //BeginWrite failed because of already trying to send, so add to the sendBuffer Queue
                        sendBuffer.Enqueue(data);
                    }
                }
                else
                {
                    if (ServerError != null)
                        ServerError(this, "You are not Connected (Socket Disconnected) - Can not send:" + data, false);

                    disconnectError = true;
                    ForceDisconnect();
                }
            }
        }

        public void SendData(byte[] bytData)
        {
            //check if the socket is still connected
            if (serverSocket == null)
            {
                if (ServerError != null)
                    ServerError(this, "Error: You are not Connected (Socket not created) - Can not send", false);
                return;
            }

            if (socketStream == null)
            {
                System.Diagnostics.Debug.WriteLine("senddata null stream");
                return;
            }

            //get the proper encoding            
            //byte[] bytData = Encoding.GetEncoding(serverSetting.Encoding).GetBytes(data + "\r\n");
            if (bytData.Length > 0)
            {
                if (socketStream.CanWrite)
                {
                    try
                    {
                        if (serverSetting.UseSSL)
                        {
                            sslStream.BeginWrite(bytData, 0, bytData.Length, new AsyncCallback(OnSendData), sslStream);
                        }
                        else
                            socketStream.BeginWrite(bytData, 0, bytData.Length, new AsyncCallback(OnSendData), socketStream);

                        //raise an event for the debug window
                        string strData = Encoding.GetEncoding("Windows-1252").GetString(readBuffer);
                        if (RawServerOutgoingData != null)
                            RawServerOutgoingData(this, strData);
                    }
                    catch (SocketException se)
                    {
                        //some kind of a socket error
                        if (ServerError != null)
                            ServerError(this, "You are not Connected - Can not send:" + se.Message, false);

                        disconnectError = true;
                        ForceDisconnect();
                    }
                    catch (NotSupportedException)
                    {
                        //BeginWrite failed because of already trying to send, so add to the sendBuffer Queue
                        //sendBuffer.Enqueue(data);
                    }
                }
                else
                {
                    if (ServerError != null)
                        ServerError(this, "You are not Connected (Socket Disconnected) - Can not send:" + bytData.ToString(), false);

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
            SslStream sl = null;
            NetworkStream ns = null;

            if (serverSetting.UseSSL)
                sl = (SslStream)ar.AsyncState;
            else
                ns = (NetworkStream)ar.AsyncState;

            try
            {
                //int bytesSent = handler.EndSend(ar);
                if (serverSetting.UseSSL)
                    sl.EndWrite(ar);
                else
                    ns.EndWrite(ar);

                //Check if anything in the sendBuffer Queue, if so, send it
                if (sendBuffer.Count > 0)
                    SendData(sendBuffer.Dequeue());

            }
            catch (SocketException se)
            {
                if (ServerError != null)
                    ServerError(this, "SendData Socket Error:" + se.Message.ToString(), false);
            }
            catch (Exception e)
            {
                if (ServerError != null)
                    ServerError(this, "SendData Error:" + e.Message.ToString(), false);

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
            int bytesRead;

            try
            {
                if (serverSetting.UseSSL)
                    bytesRead = sslStream.EndRead(ar);
                else
                    bytesRead = socketStream.EndRead(ar);

                if (serverSetting.UseProxy && !proxyAuthed)
                {
                    string strData = Encoding.GetEncoding(serverSetting.Encoding).GetString(readBuffer);
                    //System.Diagnostics.Debug.WriteLine("proxy data:" + bytesRead);
                    if (bytesRead == 2)
                    {
                        //System.Diagnostics.Debug.WriteLine("got:" + (int)strData[0] + ":" + (int)strData[1]);

                        if (strData[1] == 0xFF)
                        {
                            if (ServerError != null)
                                ServerError(this, "Proxy Server Error: None of the authentication method was accepted by proxy server.", false);
                            ForceDisconnect();
                        }
                        else if (strData[1] == 0x00)  //send proxy information
                        {
                            byte[] proxyData = new byte[7 + serverSetting.ServerName.Length];
                            proxyData[0] = 0x05;
                            proxyData[1] = 0x01;
                            proxyData[2] = 0x00;
                            proxyData[3] = 0x03;    //0x03 for a domain name
                            proxyData[4] = Convert.ToByte(serverSetting.ServerName.Length);
                            byte[] rawBytes = new byte[serverSetting.ServerName.Length];
                            rawBytes = Encoding.Default.GetBytes(serverSetting.ServerName);
                            //System.Diagnostics.Debug.WriteLine(rawBytes.Length + ":" + serverSetting.ServerName.Length);
                            rawBytes.CopyTo(proxyData, 5);
                            proxyData[proxyData.Length - 2] = (byte)((Convert.ToInt32(serverSetting.ServerPort) & 0xFF00) >> 8);
                            proxyData[proxyData.Length - 1] = (byte)(Convert.ToInt32(serverSetting.ServerPort) & 0xFF);
                            ServerMessage(this, "Sending Proxy Verification");
                            //System.Diagnostics.Debug.WriteLine(Convert.ToInt16(proxyData[proxyData.Length - 2]));
                            SendData(proxyData);

                            socketStream.BeginRead(readBuffer, 0, readBuffer.Length, new AsyncCallback(OnReceivedData), socketStream);
                        }
                        else if (strData[1] == 0x02)  //send proxy information with user/pass
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
                            SendData(proxyData);

                            socketStream.BeginRead(readBuffer, 0, readBuffer.Length, new AsyncCallback(OnReceivedData), socketStream);
                        }
                    }
                    else if (bytesRead == 10)
                    {
                        //System.Diagnostics.Debug.WriteLine("got10:" + (int)strData[0] + ":" + (int)strData[1]);
                        if (strData[1] == 0x00)
                        {
                            proxyAuthed = true;

                            ServerMessage(this, "Socks 5 Connection Successfull");

                            if (serverSetting.Password != null && serverSetting.Password.Length > 0)
                                SendData("PASS " + serverSetting.Password);

                            //send the USER / NICK stuff
                            SendData("NICK " + serverSetting.NickName);
                            SendData("USER " + serverSetting.IdentName + " \"localhost\" \"" + serverSetting.ServerName + "\" :" + serverSetting.FullName);

                            whichAddressinList = whichAddressCurrent;


                            ServerMessage(this, "Sending User Registration Information");

                            whichAddressinList = whichAddressCurrent;

                            readBuffer = new byte[BUFFER_SIZE];
                            socketStream.BeginRead(readBuffer, 0, readBuffer.Length, new AsyncCallback(OnReceivedData), socketStream);
                        }
                        else
                        {
                            ServerMessage(this, "Socks 5 Connection Error : " + strData[1]);
                        }
                    }
                }
                else
                {
                    if (bytesRead > 0)
                    {
                        //check for UTF8 coding here

                        string strData = "";
                        //check if AutoDetect of Encoding is enabled
                        if (this.serverSetting.AutoDecode)
                        {
                            UTF8Encoding enc = new UTF8Encoding(false, true);
                            try
                            {
                                strData = enc.GetString(readBuffer);
                            }
                            catch (ArgumentException)
                            {
                                strData = Encoding.GetEncoding("Windows-1252").GetString(readBuffer);
                            }
                        }
                        else
                        {
                            strData = Encoding.GetEncoding(serverSetting.Encoding).GetString(readBuffer);
                        }

                        while (strData.EndsWith(Convert.ToChar(0x0).ToString()))
                            strData = strData.Substring(0, strData.Length - 1);


                        //System.Diagnostics.Debug.WriteLine("data:" + strData.Length + ":" + strData);
                        strData = strData.Replace("\r", string.Empty);

                        if (!strData.EndsWith("\n"))
                        {
                            //create a buffer
                            dataBuffer += strData;
                            readBuffer = new byte[BUFFER_SIZE];
                            if (serverSetting.UseSSL)
                                sslStream.BeginRead(readBuffer, 0, readBuffer.Length, new AsyncCallback(OnReceivedData), sslStream);
                            else
                                socketStream.BeginRead(readBuffer, 0, readBuffer.Length, new AsyncCallback(OnReceivedData), socketStream);
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
                                {
                                    string strLine = Line.Replace("\0", string.Empty);
                                    ParseData(strLine);
                                }
                            }
                        }
                        else
                        {
                            if (strData.Length > 0)
                            {
                                //strip out NULL chars
                                strData = strData.Replace("\0", string.Empty);
                                ParseData(strData);
                            }
                        }

                        readBuffer = new byte[BUFFER_SIZE];
                        if (serverSetting.UseSSL)
                            sslStream.BeginRead(readBuffer, 0, readBuffer.Length, new AsyncCallback(OnReceivedData), sslStream);
                        else
                            socketStream.BeginRead(readBuffer, 0, readBuffer.Length, new AsyncCallback(OnReceivedData), socketStream);
                    }
                    else
                    {
                        //connection lost                    
                        disconnectError = true;
                        ForceDisconnect();

                        if (ServerError != null)
                            ServerError(this, "Connection Lost", false);

                    }
                }
            }
            catch (SocketException se)
            {
                ServerError(this, "Socket Exception OnReceiveData Error:" + se.Message.ToString(), false);
                disconnectError = true;
                attemptReconnect = true;
                
                ForceDisconnect();
            }
            catch (Exception e)
            {
                ServerError(this, "Exception OnReceiveData Error:" + e.Message.ToString(), false);
                disconnectError = true;
                attemptReconnect = true;

                ForceDisconnect();
            }
        }

        /// <summary>
        /// Method for starting a Server Connection
        /// </summary>
        public void ConnectSocket()
        {
            disconnectError = false;

            IPHostEntry hostEntry = null;
            //serverSetting.UseIPv6 = true;
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

                        ServerConnect(this, proxyIP.ToString());

                        //string msg = FormMain.Instance.GetMessageFormat("Server Connect");
                        //msg = msg.Replace("$serverip", proxyIP.ToString()).Replace("$server", serverSetting.ProxyIP).Replace("$port", serverSetting.ProxyPort);
                        //ServerMessage(this, msg);

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
                else if (serverSetting.UseIPv6)
                {
                    //connect to an IPv6 Server
                    IPAddress ipAddress = null;
                    if (IPAddress.TryParse(serverSetting.ServerName, out ipAddress))
                    {
                        /*
                        IPHostEntry ipHostInfo = Dns.GetHostEntry(serverSetting.ServerName);
                        System.Diagnostics.Debug.WriteLine(ipHostInfo.AddressList.Length + ":" + Socket.OSSupportsIPv6);

                        if (IPAddress.TryParse(serverSetting.ServerName, out ipAddress))
                        {
                            System.Diagnostics.Debug.WriteLine("parsed ipv6:" + ipAddress.AddressFamily);
                        }

                        foreach (IPAddress ip in ipHostInfo.AddressList)
                        {
                            System.Diagnostics.Debug.WriteLine(ip.Address.ToString() + ":" + ip.AddressFamily);
                        }
                        */
                        //ipAddress = ipHostInfo.AddressList[0];// IPAddress.Parse(address);
                        
                        if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            System.Diagnostics.Debug.WriteLine("success parse as ipv6 address");
                            
                            IPEndPoint ipe = new IPEndPoint(ipAddress, Convert.ToInt32(serverSetting.ServerPort));
                            serverSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                            serverSocket.SetSocketOption(SocketOptionLevel.IPv6, (SocketOptionName)27, 0);
                            System.Diagnostics.Debug.WriteLine("ipv6 connect:" + ipe.AddressFamily.ToString() + ":" + ipAddress.ToString());

                            ServerConnect(this, ipAddress.ToString());
                            //string msg = FormMain.Instance.GetMessageFormat("Server Connect");
                            //msg = msg.Replace("$serverip", ipAddress.ToString()).Replace("$server", serverSetting.ServerName).Replace("$port", serverSetting.ServerPort);
                            //ServerMessage(this, msg + " (IPv6)");

                            serverSetting.ServerIP = ipAddress.ToString();

                            System.Diagnostics.Debug.WriteLine("start ipv6 connect here");
                            //serverSocket.BeginConnect(ipe, new AsyncCallback(OnConnectionReady), serverSocket);
                            serverSocket.BeginConnect(ipe, new AsyncCallback(OnConnectionReady),  null);
                        }
                        return;
                    }
                    //else
                    {
                        //System.Diagnostics.Debug.WriteLine("can not get ip of ipv6 address");
                    }
                }
                else
                {
                    //this will fail on an IPv6 address
                    hostEntry = Dns.GetHostEntry(serverSetting.ServerName);

                    whichAddressCurrent = 1;
                    totalAddressinDNS = hostEntry.AddressList.Length;

                    if (whichAddressinList > totalAddressinDNS)
                        whichAddressinList = 1;

                    IPAddress ipAddress = null;
                    if (IPAddress.TryParse(serverSetting.ServerName, out ipAddress))
                    {
                        IPEndPoint ipe = new IPEndPoint(ipAddress, Convert.ToInt32(serverSetting.ServerPort));
                        System.Diagnostics.Debug.WriteLine("try ipv6 1:" + ipe.AddressFamily.ToString() + ":" + ipAddress.ToString());
                        serverSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                        serverSetting.ServerIP = ipAddress.ToString();
                        ServerConnect(this, ipAddress.ToString());

                        serverSocket.BeginConnect(ipe, new AsyncCallback(OnConnectionReady), null);
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
                                serverSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                                serverSetting.ServerIP = address.ToString();
                                ServerConnect(this, address.ToString());

                                serverSocket.BeginConnect(ipe, new AsyncCallback(OnConnectionReady), null);
                                break;
                            }
                            whichAddressCurrent++;
                            if (whichAddressCurrent > hostEntry.AddressList.Length)
                                whichAddressCurrent = 1;
                        }
                        catch (Exception e)
                        {
                            if (ServerError != null)
                                ServerError(this, "Connect - Exception Error:" + e.Message.ToString(), false);

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
                    ServerError(this, "Socket Error " + se.ErrorCode + " :" + se.Message, false);

                System.Diagnostics.Debug.WriteLine(se.StackTrace);
                System.Diagnostics.Debug.WriteLine(se.NativeErrorCode);
                System.Diagnostics.Debug.WriteLine(se.SocketErrorCode);
                System.Diagnostics.Debug.WriteLine(se.ErrorCode);
                disconnectError = true;
                ForceDisconnect();
            }
        }

        private bool RemoteCertificateValidationCallback(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None || sslPolicyErrors == SslPolicyErrors.RemoteCertificateNameMismatch)
            {
                return true;
            }

            //check if you allow it to accept connections with invalid certificates
            if (serverSetting.SSLAcceptInvalidCertificate)
                return true;

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }


        public void ForceDisconnect()
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

            ServerForceDisconnect(this);

        }

        #endregion


        #region Timer Events

        public void CreateTimer(string id, double interval, int reps, string command)
        {
            IrcTimer timer = new IrcTimer(id, interval * 1000, reps, command);
            timer.OnTimerElapsed += new IrcTimer.TimerElapsed(OnTimerElapsed);
            ircTimers.Add(timer);
            timer.Start();
        }

        public void DestroyTimer(string id)
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
            OutGoingCommand(this, command);
        }

        #endregion
    }

}
