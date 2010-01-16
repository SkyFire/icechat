/******************************************************************************\
 * IceChat 2009 Internet Relay Chat Client
 *
 * Copyright (C) 2009 Paul Vanderzee <snerf@icechat.net>
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

        private ServerSetting serverSetting;
        private bool fullyConnected = false;
        private ArrayList commandQueue;

        public IRCConnection(ServerSetting ss)
        {
            dataBuffer = "";
            commandQueue = new ArrayList();
            serverSetting = ss;
            reconnectTimer = new System.Timers.Timer(30000);
            reconnectTimer.Enabled = true;
            reconnectTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimerElapsed);
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

            reconnectTimer.Enabled = false;
            reconnectTimer.Dispose();
        }

        private void OnTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ConnectSocket();
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
            
            string msg = GetMessageFormat("Server Disconnect");
            msg = msg.Replace("$serverip", serverSetting.ServerIP).Replace("$server", serverSetting.ServerName).Replace("$port", serverSetting.ServerPort);
            
            foreach (TabWindow t in FormMain.Instance.TabMain.WindowTabs)
            {
                if (t.WindowStyle == TabWindow.WindowType.Channel || t.WindowStyle == TabWindow.WindowType.Query)
                {
                    if (t.Connection == this)
                    {
                        t.ClearNicks();
                        t.IsFullyJoined = false;

                        t.TextWindow.AppendText(msg, 1);
                        t.LastMessageType = FormMain.ServerMessageType.ServerMessage;

                        if (FormMain.Instance.CurrentWindow == t)
                            FormMain.Instance.NickList.Header = t.WindowName + ":0";                            
                    }
                }
            }
            ServerMessage(this, msg);

            FormMain.Instance.ServerTree.Invalidate();

            if (FormMain.Instance.CurrentWindowType == TabWindow.WindowType.Console)
                if (FormMain.Instance.InputPanel.CurrentConnection == this)
                    FormMain.Instance.StatusText(serverSetting.NickName + " disconnected (" + serverSetting.ServerName + ")");

            serverSocket = null;
            
            commandQueue.Clear();
            initialLogon = false;
            triedAltNickName = false;
            serverSetting.IAL.Clear();

            if (disconnectError && attemptReconnect && FormMain.Instance.IceChatOptions.ReconnectServer)
            {
                //reconnect
                if (ServerMessage != null)
                    ServerMessage(this, "Waiting 30 seconds to Reconnect to (" + serverSetting.ServerName + ")");
                disconnectError = false;
                try
                {
                    if (reconnectTimer != null)
                        reconnectTimer.Start();
                }
                catch
                {
                }
            }

        }

        /// <summary>
        /// Event for Server Connection
        /// </summary>
        /// <param name="ar"></param>
        private void OnConnectionReady(IAsyncResult ar)
        {
            reconnectTimer.Stop();

            if (serverSocket == null)
            {
                if (ServerError != null)
                    ServerError(this, "Null Socket - Can not Connect");
                return;
            }
            
            try
            {
                serverSocket.EndConnect(ar);
            }            
            catch (Exception e)
            {
                if (ServerError != null)
                    ServerError(this, "Socket Error:" + e.Message.ToString());

                disconnectError = true;
                if (serverSocket.Connected)
                {
                    serverSocket.Shutdown(SocketShutdown.Both);
                    serverSocket.BeginDisconnect(false, new AsyncCallback(OnDisconnect), serverSocket);
                }
                return;
            }

            SocketPacket thisSocket = new SocketPacket(serverSocket);

            try
            {
                thisSocket.workSocket.BeginReceive(thisSocket.dataBuffer, 0, thisSocket.dataBuffer.Length, 0, new AsyncCallback(OnReceivedData), thisSocket);

                FormMain.Instance.ServerTree.Invalidate();

                //send the USER / NICK stuff
                SendData("NICK " + serverSetting.NickName);

                //string ServerName = IPAddress.Parse(((IPEndPoint)serverSocket.RemoteEndPoint).Address.ToString()).ToString();
                SendData("USER " + serverSetting.IdentName + " \"localhost\" \"" + serverSetting.ServerName + "\" :" + serverSetting.FullName);

                if (ServerMessage != null)
                    ServerMessage(this, "Sending User Registration Information");

            }
            catch (SocketException se)
            {
                if (ServerError != null)
                    ServerError(this, "Socket Exception Error OnConnectionReady:" + se.Message.ToString() + ":" + se.ErrorCode);

                disconnectError = true;
                serverSocket.Shutdown(SocketShutdown.Both);
                serverSocket.BeginDisconnect(false, new AsyncCallback(OnDisconnect), serverSocket);


            }
            catch (Exception e)
            {
                if (ServerError != null)
                    ServerError(this, "Exception Error OnConnectionReady:" + e.Message.ToString());

                disconnectError = true;
                serverSocket.Shutdown(SocketShutdown.Both);
                serverSocket.BeginDisconnect(false, new AsyncCallback(OnDisconnect), serverSocket);
            }
        }

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
            byte[] bytData = Encoding.GetEncoding(serverSetting.Encoding).GetBytes(data + "\n"); ; 
            if (bytData.Length > 0)
            {                
                if (serverSocket.Connected)
                {
                    try
                    {
                        serverSocket.BeginSend(bytData, 0, bytData.Length, 0, new AsyncCallback(OnSendData), serverSocket);
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
                    }
                }
                else
                {
                    if (ServerError != null)
                        ServerError(this, "You are not Connected - Can not send:" + data);

                    disconnectError = true;
                    serverSocket.Shutdown(SocketShutdown.Both);
                    serverSocket.BeginDisconnect(false, new AsyncCallback(OnDisconnect), serverSocket);
                }
            }
        }
       
        /// <summary>
        /// Event fire when Data needs to be sent to the Server Connection
        /// </summary>
        /// <param name="ar"></param>
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
                serverSocket.Shutdown(SocketShutdown.Both);
                serverSocket.BeginDisconnect(false, new AsyncCallback(OnDisconnect), serverSocket);

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

                if (size > 0)
                {
                    Decoder d = Encoding.GetEncoding(serverSetting.Encoding).GetDecoder();
                    char[] chars = new char[size];
                    int charLen = d.GetChars(handler.dataBuffer, 0, size, chars, 0);
                    System.String strData = new System.String(chars);
                    strData = strData.Replace("\r", "");

                    if (!strData.EndsWith("\n") && strData[strData.Length - 1] != (char)0)
                    {
                        //create a buffer
                        dataBuffer += strData;
                        handler.workSocket.BeginReceive(handler.dataBuffer, 0, handler.dataBuffer.Length, 0, new AsyncCallback(OnReceivedData), handler);
                        return;
                    }

                    if (dataBuffer.Length > 0)
                    {
                        strData = dataBuffer + strData;
                        dataBuffer = "";
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
                    serverSocket.Shutdown(SocketShutdown.Both);
                    serverSocket.BeginDisconnect(false, new AsyncCallback(OnDisconnect), serverSocket);

                }
            }            
            
            catch (SocketException se)
            {
                ServerError(this, "Socket Exception OnReceiveData Error:" + se.Source + ":" + se.Message.ToString());

                disconnectError = true;
                serverSocket.Shutdown(SocketShutdown.Both);
                serverSocket.BeginDisconnect(false, new AsyncCallback(OnDisconnect), serverSocket);
            }
            catch (Exception e)
            {
                ServerError(this, "Exception OnReceiveData Error:" + e.Source + ":" + e.Message.ToString() + ":" + e.StackTrace);

                disconnectError = true;
                serverSocket.Shutdown(SocketShutdown.Both);
                serverSocket.BeginDisconnect(false, new AsyncCallback(OnDisconnect), serverSocket);
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
                hostEntry = Dns.GetHostEntry(serverSetting.ServerName);

                // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
                // an exception that occurs when the host IP Address is not compatible with the address family
                // (typical in the IPv6 case).
                foreach (IPAddress address in hostEntry.AddressList)
                {
                    try
                    {
                        IPEndPoint ipe = new IPEndPoint(address, Convert.ToInt32(serverSetting.ServerPort));
                        Socket tempSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                        if (ServerMessage != null)
                        {
                            string msg = GetMessageFormat("Server Connect");
                            msg = msg.Replace("$serverip", address.ToString()).Replace("$server", serverSetting.ServerName).Replace("$port", serverSetting.ServerPort);
                            serverSetting.ServerIP = address.ToString();
                            ServerMessage(this, msg);
                        }

                        tempSocket.BeginConnect(ipe, new AsyncCallback(OnConnectionReady), null);
                        serverSocket = tempSocket;
                        
                        break;
                    }
                    catch (Exception e)
                    {
                        if (ServerError != null)
                            ServerError(this, "Connect - Exception Error:" + e.Message.ToString());

                        disconnectError = true;
                        if (serverSocket != null)
                        {
                            serverSocket.Shutdown(SocketShutdown.Both);
                            serverSocket.BeginDisconnect(false, new AsyncCallback(OnDisconnect), serverSocket);
                        }

                    }
                }
            }
            catch (SocketException se)
            {
                if (ServerError != null)
                    ServerError(this, "Socket Exception Error:" + se.Message);

                disconnectError = true;

                //reconnect
                if (ServerMessage != null)
                {
                    string msg = GetMessageFormat("Server Reconnect");
                    msg = msg.Replace("$serverip", serverSetting.ServerIP).Replace("$server", serverSetting.ServerName).Replace("$port", serverSetting.ServerPort);
                    ServerMessage(this, msg);
                }
                reconnectTimer.Start();            
            }
        }
        #endregion

        private string GetMessageFormat(string MessageName)
        {
            foreach (ServerMessageFormatItem msg in FormMain.Instance.MessageFormats.MessageSettings)
            {
                if (msg.MessageName.ToLower() == MessageName.ToLower())
                {
                    return msg.FormattedMessage;
                }
            }
            return null;
        }

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
