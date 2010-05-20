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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace IceChat
{
    public class IceTabPage : Panel, IDisposable
    {
        private IRCConnection connection;

        private Hashtable nicks;

        //channel settings
        private string channelTopic = "";
        private string fullChannelMode = "";

        internal struct channelMode
        {
            public char mode;
            public bool set;
            public string param;
        }
        private Hashtable channelModes;

        private bool isFullyJoined = false;
        private bool hasChannelInfo = false;
        private FormChannelInfo channelInfoForm = null;

        private delegate void ChangeTopicDelegate(string topic);
        private delegate void ChangeTextDelegate(string text);
        private delegate TextWindow CurrentWindowDelegate();
        private delegate void AddChannelListDelegate(string channel, int users, string topic);
        private delegate void AddDccChatDelegate(string message);
        private delegate void ClearChannelListDelegate();

        private Panel panelTopic;
        
        private TextWindow textWindow;
        private TextWindow textTopic;
        private WindowType windowType;
        private ChannelListView channelList;

        private TabControl consoleTab;

        private FormMain.ServerMessageType lastMessageType;

        private bool _disableConsoleSelectChangedEvent = false;
        private bool _disableSounds = false;

        private TcpClient dccChatSocket;
        private TcpListener dccChatSocketListener;
        private Thread dccThread;
        private Thread listenThread;
        private System.Timers.Timer dccTimeOutTimer;

        public enum WindowType
        {
            Console = 1,
            Channel = 2,
            Query = 3,
            ChannelList = 4,
            DCCChat = 5,
            DCCFile = 6,
            Window = 7,
            Debug = 99
        }

        //private Image imgIcon;
        //private int iTabIndex;
        private string _tabCaption;

        public IceTabPage(WindowType windowType, string sCaption) 
        {
            if (windowType == WindowType.Channel)
            {
                InitializeChannel();
                textTopic.NoEmoticons = true;
                textTopic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                this.textTopic.AppendText("Topic:", 3);                
            }
            else if (windowType == WindowType.Query)
            {
                InitializeChannel();
                panelTopic.Visible = false;
            }
            else if (windowType == WindowType.Console)
            {
                InitializeConsole();
            }
            else if (windowType == WindowType.ChannelList)
            {
                InitializeChannelList();
            }
            else if (windowType == WindowType.DCCChat)
            {
                InitializeChannel();
                panelTopic.Visible = false;
            }
            else if (windowType == WindowType.Window)
            {
                InitializeChannel();
                panelTopic.Visible = false;
                textWindow.NoEmoticons = true;
            }
            else if (windowType == WindowType.Debug)
            {
                InitializeChannel();
                panelTopic.Visible = false;
                textWindow.NoEmoticons = true;
            }

            _tabCaption = sCaption;
            this.WindowStyle = windowType;

            nicks = new Hashtable();
            channelModes = new Hashtable();
            
            lastMessageType = FormMain.ServerMessageType.Default;
        }

        protected override void Dispose(bool disposing)
        {
            //this will dispose the TextWindow, making it close the log file
            if (this.windowType == WindowType.Channel || this.windowType == WindowType.Query)
                textWindow.Dispose();

            if (this.windowType == WindowType.DCCChat)
            {
                if (dccChatSocket != null)
                    dccChatSocket.Close();
                
                if (dccThread != null)
                    dccThread.Abort();

                if (listenThread != null)
                    listenThread.Abort();

                if (dccChatSocketListener != null)
                    dccChatSocketListener.Stop();
            }
        }

        /// <summary>
        /// Add a message to the Text Window for Selected Console Tab Connection
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="data"></param>
        /// <param name="color"></param>
        internal void AddText(IRCConnection connection, string data, int color, bool scrollToBottom)
        {
            foreach (ConsoleTab t in consoleTab.TabPages)
            {
                if (t.Connection == connection)
                {
                    ((TextWindow)t.Controls[0]).AppendText(data, color);
                    if (scrollToBottom)
                        ((TextWindow)t.Controls[0]).ScrollToBottom();
                    return;
                }
            }
        }

        /// <summary>
        /// Add a new Tab/Connection to the Console Tab Control
        /// </summary>
        /// <param name="connection"></param>
        internal void AddConsoleTab(IRCConnection connection)
        {
            ConsoleTab t = new ConsoleTab(connection.ServerSetting.ServerName);
            t.Connection = connection;

            TextWindow w = new TextWindow();
            w.Dock = DockStyle.Fill;
            w.Font = new System.Drawing.Font(FormMain.Instance.IceChatFonts.FontSettings[0].FontName, FormMain.Instance.IceChatFonts.FontSettings[0].FontSize);
            w.IRCBackColor = FormMain.Instance.IceChatColors.ConsoleBackColor;
            w.NoEmoticons = true;

            t.Controls.Add(w);
            if (FormMain.Instance.IceChatOptions.LogConsole)
                w.SetLogFile();

            consoleTab.TabPages.Add(t);
            consoleTab.SelectedTab = t;

        }
        /// <summary>
        /// Temporary Method to create a NULL Connection for the Welcome Tab in the Console
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="serverName"></param>
        internal void AddConsoleTab(string serverName)
        {
            //this is only used for now, to show the "Welcome" Tab
            ConsoleTab t = new ConsoleTab(serverName);

            TextWindow w = new TextWindow();
            w.Dock = DockStyle.Fill;
            w.Font = new System.Drawing.Font(FormMain.Instance.IceChatFonts.FontSettings[0].FontName, FormMain.Instance.IceChatFonts.FontSettings[0].FontSize);
            w.IRCBackColor = FormMain.Instance.IceChatColors.ConsoleBackColor;

            t.Controls.Add(w);
            consoleTab.TabPages.Add(t);
            consoleTab.SelectedTab = t;

        }

        internal bool NickExists(string nick)
        {
            return nicks.ContainsKey(nick);
        }

        internal void UpdateChannelMode(char mode, bool addMode)
        {
            try
            {
                channelMode c = new channelMode();
                c.mode = mode;
                c.set = addMode;
                c.param = null;

                if (channelModes.Contains(mode))
                {
                    if (addMode)
                        channelModes[mode] = c;
                    else
                        channelModes.Remove(mode);
                }
                else
                {
                    channelModes.Add(mode, c);
                }

                string modes = "";
                string prms = " ";
                foreach (channelMode cm in channelModes.Values)
                {
                    modes += cm.mode.ToString();
                    if (cm.param != null)
                        prms += cm.param + " ";
                }

                if (modes.Trim().Length > 0)
                    ChannelModes = "+" + modes.Trim() + prms.TrimEnd();
                else
                    ChannelModes = "";
            }
            catch (Exception e)
            {
                FormMain.Instance.WriteErrorFile(FormMain.Instance.InputPanel.CurrentConnection,"IceTabPage UpdateChannelMode", e);
            }
        }

        internal void UpdateChannelMode(char mode, string param, bool addMode)
        {
            try
            {
                channelMode c = new channelMode();
                c.mode = mode;
                c.set = addMode;
                c.param = param;

                if (channelModes.Contains(mode))
                {
                    if (addMode)
                        channelModes[mode] = c;
                    else
                        channelModes.Remove(mode);
                }
                else
                {
                    if (addMode)
                        channelModes.Add(mode, c);
                }

                string modes = "";
                string prms = " ";
                foreach (channelMode cm in channelModes.Values)
                {
                    modes += cm.mode.ToString();
                    if (cm.param != null)
                        prms += cm.param + " ";
                }
                if (modes.Trim().Length > 0)
                    ChannelModes = "+" + modes.Trim() + prms.TrimEnd();
                else
                    ChannelModes = "";
            }
            catch (Exception e)
            {
                FormMain.Instance.WriteErrorFile(FormMain.Instance.InputPanel.CurrentConnection,"IceTabPage UpdateChannelMode2", e);
            }
        }

        internal void UpdateNick(string nick, string mode, bool addMode)
        {
            string justNick = nick;

            for (int i = 0; i < connection.ServerSetting.StatusModes[1].Length; i++)
                justNick = justNick.Replace(connection.ServerSetting.StatusModes[1][i].ToString(), string.Empty);

            if (nicks.ContainsKey(justNick))
            {
                User u = (User)nicks[justNick];

                for (int i = 0; i < connection.ServerSetting.StatusModes[1].Length; i++)
                    if (mode == connection.ServerSetting.StatusModes[1][i].ToString())
                        u.Level[i] = addMode;

                if (FormMain.Instance.CurrentWindow == this)
                    FormMain.Instance.NickList.RefreshList(this);
            }
        }


        internal User GetNick(string nick)
        {
            for (int i = 0; i < connection.ServerSetting.StatusModes[1].Length; i++)
                nick = nick.Replace(connection.ServerSetting.StatusModes[1][i].ToString(), string.Empty);

            if (nicks.ContainsKey(nick))
                return (User)nicks[nick];

            return null;
        }

        internal User GetNick(int nickNumber)
        {
            if (nickNumber <= nicks.Count)
            {
                int i = 1;
                foreach (User u in nicks.Values)
                {
                    if (nickNumber == i)
                        return u;
                    i++;
                }
            }
            return null;
        }

        internal void AddNick(string nick, bool refresh)
        {
            //replace any user modes from the nick
            string justNick = nick;
            for (int i = 0; i < connection.ServerSetting.StatusModes[1].Length; i++)
                justNick = justNick.Replace(connection.ServerSetting.StatusModes[1][i].ToString(), string.Empty);

            if (nicks.ContainsKey(justNick))
                return;

            User u = new User(nick, this.connection);

            nicks.Add(justNick, u);
            if (refresh)
            {
                if (FormMain.Instance.CurrentWindow == this)
                    FormMain.Instance.NickList.RefreshList(this);
            }
            
        }

        internal void RemoveNick(string nick)
        {
            nicks.Remove(nick);
            if (FormMain.Instance.CurrentWindow == this)
                FormMain.Instance.NickList.RefreshList(this);
            
        }

        internal void ClearNicks()
        {
            nicks.Clear();
        }

        internal Hashtable Nicks
        {
            get { return nicks; }
        }

        internal TextWindow TextWindow
        {
            get
            {
                return this.textWindow;
            }
        }

        internal TextWindow TopicWindow
        {
            get
            {
                return this.textTopic;
            }
        }

        internal void RequestDCCChat()
        {
            //send out a dcc chat request
            string localIP = IPAddressToLong(this.connection.ServerSetting.LocalIP);
            Random port = new Random();
            int p = port.Next(FormMain.Instance.IceChatOptions.DCCPortLower, FormMain.Instance.IceChatOptions.DCCPortUpper);

            dccChatSocketListener = new TcpListener(new IPEndPoint(IPAddress.Any, Convert.ToInt32(p)));
            listenThread = new Thread(new ThreadStart(ListenForConnection));
            listenThread.Start();

            //connection.SendData("NOTICE " + _tabCaption + " :DCC CHAT (" + this.connection.ServerSetting.LocalIP.ToString() + ")");            
            connection.SendData("PRIVMSG " + _tabCaption + " :DCC CHAT chat " + localIP + " " + p.ToString() + "");
            dccTimeOutTimer = new System.Timers.Timer();
            dccTimeOutTimer.Interval = 1000 * FormMain.Instance.IceChatOptions.DCCChatTimeOut;
            dccTimeOutTimer.Elapsed += new System.Timers.ElapsedEventHandler(dccTimeOutTimer_Elapsed);
            dccTimeOutTimer.Start();
        }

        private void dccTimeOutTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            string msg = FormMain.Instance.GetMessageFormat("DCC Chat Timeout");
            msg = msg.Replace("$nick", _tabCaption);
            textWindow.AppendText(msg, 1);

            dccTimeOutTimer.Stop();
            dccChatSocketListener.Stop();
            listenThread.Abort();
        }

        private void ListenForConnection()
        {
            this.dccChatSocketListener.Start();
            bool keepListening = true;
            
            while (keepListening)
            {
                dccChatSocket = dccChatSocketListener.AcceptTcpClient();
                dccChatSocketListener.Stop();
                
                string msg = FormMain.Instance.GetMessageFormat("DCC Chat Connect");
                msg = msg.Replace("$nick", _tabCaption);
                textWindow.AppendText(msg, 1);

                dccThread = new Thread(new ThreadStart(GetDCCChatData));
                dccThread.Start();
                keepListening = false;
            }
        }

        internal void StartDCCChat(string nick, string ip, string port)
        {
            dccChatSocket = new TcpClient();
            IPAddress ipAddr = LongToIPAddress(ip);
            IPEndPoint ep = new IPEndPoint(ipAddr, Convert.ToInt32(port));
            try
            {
                dccChatSocket.Connect(ep);
                if (dccChatSocket.Connected)
                {
                    string msg = FormMain.Instance.GetMessageFormat("DCC Chat Connect");
                    msg = msg.Replace("$nick", nick).Replace("$ip", ip).Replace("$port", port);
                    textWindow.AppendText(msg, 1);

                    dccThread = new Thread(new ThreadStart(GetDCCChatData));
                    dccThread.Start();
                }
            }
            catch (SocketException se)
            {
                textWindow.AppendText(se.Message, 4);
            }
        }

        internal void SendDCCChatData(string message)
        {
            if (dccChatSocket != null)
            {
                if (dccChatSocket.Connected)
                {
                    NetworkStream ns = dccChatSocket.GetStream();
                    ASCIIEncoding encoder = new ASCIIEncoding();
                    byte[] buffer = encoder.GetBytes(message + "\n");
                    ns.Write(buffer, 0, buffer.Length);
                    ns.Flush();
                }
            }
        }

        private void GetDCCChatData()
        {
            while (true)
            {
                int buffSize = 0;
                byte[] buffer = new byte[8192];
                NetworkStream ns = dccChatSocket.GetStream();
                buffSize = dccChatSocket.ReceiveBufferSize;
                int bytesRead = ns.Read(buffer, 0, buffSize);
                Decoder d = Encoding.GetEncoding(this.connection.ServerSetting.Encoding).GetDecoder();
                char[] chars = new char[buffSize];
                int charLen = d.GetChars(buffer, 0, buffSize, chars, 0);
                System.String strData = new System.String(chars);
                if (bytesRead == 0)
                {
                    //we have a disconnection
                    break;
                }
                AddDccMessage(strData);
            }
            
            string msg = FormMain.Instance.GetMessageFormat("DCC Chat Disconnect");
            msg = msg.Replace("$nick", _tabCaption);
            textWindow.AppendText(msg, 1);
            dccChatSocket.Close();
            dccThread.Abort();
        }

        private void AddDccMessage(string message)
        {
            if (this.InvokeRequired)
            {
                AddDccChatDelegate a = new AddDccChatDelegate(AddDccMessage);
                this.Invoke(a, new object[] { message });
            }
            else
            {
                string[] lines = message.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    if (line[0] != (char)0)
                    {
                        string msg = FormMain.Instance.GetMessageFormat("DCC Chat Message");
                        msg = msg.Replace("$nick", _tabCaption);
                        msg = msg.Replace("$message", line);
                        textWindow.AppendText(msg, 1);
                    }
                }
            }
        }

        private string IPAddressToLong(IPAddress ip)
        {
            return NetworkUnsignedLong(ip.Address).ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        private IPAddress LongToIPAddress(string longIP)
        {
            byte[] quads = BitConverter.GetBytes(long.Parse(longIP, System.Globalization.CultureInfo.InvariantCulture));
            return IPAddress.Parse(quads[3] + "." + quads[2] + "." + quads[1] + "." + quads[0]);
        }

        private long NetworkUnsignedLong(long hostOrderLong)
        {
            long networkLong = IPAddress.HostToNetworkOrder(hostOrderLong);
            //Network order has the octets in reverse order starting with byte 7
            //To get the correct string simply shift them down 4 bytes
            //and zero out the first 4 bytes.
            return (networkLong >> 32) & 0x00000000ffffffff;
        }

        /// <summary>
        /// Return the Connection for the Current Selected in the Console Tab Control
        /// </summary>
        internal IRCConnection CurrentConnection
        {
            get
            {
                return ((ConsoleTab)consoleTab.SelectedTab).Connection;
            }
        }

        internal IRCConnection Connection
        {
            get
            {
                return this.connection;
            }
            set
            {
                this.connection = value;
            }
        }

        internal WindowType WindowStyle
        {
            get
            {
                return windowType;
            }
            set
            {
                windowType = value;
                if (windowType == WindowType.Console)
                {
                    //nada
                }
                else if (windowType == WindowType.Channel)
                {
                    panelTopic.Visible = true;
                    textWindow.IRCBackColor = FormMain.Instance.IceChatColors.ChannelBackColor;
                    textTopic.IRCBackColor = FormMain.Instance.IceChatColors.ChannelBackColor;
                }
                else if (windowType == WindowType.Query)
                {
                    textWindow.IRCBackColor = FormMain.Instance.IceChatColors.QueryBackColor;
                }
                else if (windowType == WindowType.ChannelList)
                {
                    //nada
                }
                else if (windowType == WindowType.DCCChat)
                {
                    textWindow.IRCBackColor = FormMain.Instance.IceChatColors.QueryBackColor;
                }
                else if (windowType == WindowType.Window)
                {
                    //nada
                }
                else if (windowType == WindowType.Debug)
                {
                    //nada
                }

            }
        }

        internal string ChannelModes
        {
            get
            {
                return fullChannelMode;
            }
            set
            {
                fullChannelMode = value;
            }
        }

        internal Hashtable ChannelModesHash
        {
            get
            {
                return channelModes;
            }
        }

        internal string ChannelTopic
        {
            get
            {
                return channelTopic;
            }
            set
            {
                channelTopic = value;
                UpdateTopic(value);
            }
        }

        internal bool IsFullyJoined
        {
            get
            {
                return isFullyJoined;
            }
            set
            {
                isFullyJoined = value;
            }
        }

        internal bool HasChannelInfo
        {
            get
            {
                return hasChannelInfo;
            }
            set
            {
                hasChannelInfo = value;
            }
        }

        internal FormChannelInfo ChannelInfoForm
        {
            get
            {
                return channelInfoForm;
            }
            set
            {
                channelInfoForm = value;
            }
        }
        
        //whether to play sound events for this window
        internal bool DisableSounds
        {
            get { return _disableSounds; }
            set { _disableSounds = value; }
        }

        internal TabControl ConsoleTab
        {
            get { return consoleTab; }
        }

        internal FormMain.ServerMessageType LastMessageType
        {
            get
            {
                return lastMessageType;
            }
            set
            {
                if (lastMessageType != value)
                {
                    //check if we are the current window or not
                    if (this == FormMain.Instance.CurrentWindow)
                    {
                        lastMessageType = FormMain.ServerMessageType.Default;
                        return;
                    }
                    
                    // do not change if already a New Message
                    if (lastMessageType != FormMain.ServerMessageType.Message)
                    {
                        lastMessageType = value;
                        FormMain.Instance.TabMain.Invalidate();
                        FormMain.Instance.ServerTree.Invalidate();
                    }
                }
            }
        }

        internal TextWindow CurrentConsoleWindow()
        {
            if (this.InvokeRequired)
            {
                CurrentWindowDelegate cwd = new CurrentWindowDelegate(CurrentConsoleWindow);
                return (TextWindow)this.Invoke(cwd, new object[] { });
            }
            else
            {
                return (TextWindow)consoleTab.SelectedTab.Controls[0];
            }
        }

        internal int TotalChannels
        {
            get { return this.channelList.Items.Count; }
        }

        internal void ClearChannelList()
        {
            if (this.InvokeRequired)
            {
                ClearChannelListDelegate c = new ClearChannelListDelegate(ClearChannelList);
                this.Invoke(c, new object[] { });
            } 
            else
                this.channelList.Items.Clear();
        }

        private void UpdateText(string text)
        {
            if (this.InvokeRequired)
            {
                ChangeTextDelegate c = new ChangeTextDelegate(UpdateText);
                this.Invoke(c, new object[] { text });
            }
            else
            {
                this.Text = text;
                this.Update();
            }
        }

        private void UpdateTopic(string topic)
        {
            if (this.InvokeRequired)
            {
                ChangeTopicDelegate c = new ChangeTopicDelegate(UpdateTopic);
                this.Invoke(c, new object[] { topic });
            }
            else
            {
                channelTopic = topic;
                textTopic.ClearTextWindow();
                string msgt = FormMain.Instance.GetMessageFormat("Channel Topic Text");
                msgt = msgt.Replace("$channel", this.TabCaption);
                msgt = msgt.Replace("$topic", topic);
                textTopic.AppendText(msgt, 1);
            }
        }

        public void AddChannelList(string channel, int users, string topic)
        {
            if (this.InvokeRequired)
            {
                AddChannelListDelegate a = new AddChannelListDelegate(AddChannelList);
                this.Invoke(a, new object[] { channel, users, topic });
            }
            else
            {
                ListViewItem lvi = new ListViewItem(channel);
                lvi.SubItems.Add(users.ToString());
                lvi.SubItems.Add(topic);
                channelList.Items.Add(lvi);
            }
        }

        private void OnControlAdded(object sender, ControlEventArgs e)
        {
            e.Control.Dock = DockStyle.Fill;
        }

        public string TabCaption
        {
            get { return _tabCaption; }
            set { this._tabCaption = value; }
        }

        internal void SelectConsoleTab(ConsoleTab c)
        {
            _disableConsoleSelectChangedEvent = true;
            consoleTab.SelectedTab = c;
            _disableConsoleSelectChangedEvent = false;
        }

        private void OnTabConsoleSelectedIndexChanged(object sender, EventArgs e)
        {
            ((TextWindow)(consoleTab.SelectedTab.Controls[0])).resetUnreadMarker(); 
            
            if (consoleTab.TabPages.IndexOf(consoleTab.SelectedTab) != 0 && !_disableConsoleSelectChangedEvent)
            {
                FormMain.Instance.InputPanel.CurrentConnection = ((ConsoleTab)consoleTab.SelectedTab).Connection;

                if (((ConsoleTab)consoleTab.SelectedTab).Connection.IsConnected)
                {
                    if (((ConsoleTab)consoleTab.SelectedTab).Connection.ServerSetting.RealServerName != null)
                        FormMain.Instance.StatusText(((ConsoleTab)consoleTab.SelectedTab).Connection.ServerSetting.NickName + " connected to " + ((ConsoleTab)consoleTab.SelectedTab).Connection.ServerSetting.RealServerName);
                    else
                        FormMain.Instance.StatusText(((ConsoleTab)consoleTab.SelectedTab).Connection.ServerSetting.NickName + " connected to " + ((ConsoleTab)consoleTab.SelectedTab).Connection.ServerSetting.ServerName);
                }
                else
                    FormMain.Instance.StatusText(((ConsoleTab)consoleTab.SelectedTab).Connection.ServerSetting.NickName + " disconnected (" + ((ConsoleTab)consoleTab.SelectedTab).Connection.ServerSetting.ServerName + ")");
                
                //highlite the proper item in the server tree
                FormMain.Instance.ServerTree.SelectTab(((ConsoleTab)consoleTab.SelectedTab).Connection.ServerSetting, false);
                
            }
            else
            {
                //FormMain.Instance.ServerTree.SelectTab(this);
                FormMain.Instance.InputPanel.CurrentConnection = null;
                FormMain.Instance.StatusText("Welcome to IceChat 2009");
            }
            
        }

        private void OnTabConsoleMouseUp(object sender, MouseEventArgs e)
        {
            FormMain.Instance.FocusInputBox();
        }

        /// <summary>
        /// Checks if Left Mouse Button is Pressed by the "X" button
        /// Quits Server if Server is Connected
        /// Closes Server Tab if Server is Disconnected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTabConsoleMouseDown(object sender, MouseEventArgs e)
        {            
            
            if (e.Button == MouseButtons.Left)
            {
                for (int i = consoleTab.TabPages.Count - 1; i >= 0; i--)
                {
                    if (consoleTab.GetTabRect(i).Contains(e.Location) && i == consoleTab.SelectedIndex)
                    {
                        if (((ConsoleTab)consoleTab.TabPages[i]).Connection != null)
                        {
                            if (e.Location.X > consoleTab.GetTabRect(i).Right - 14)
                            {
                                if (((ConsoleTab)consoleTab.TabPages[i]).Connection.IsConnected)
                                {
                                    if (((ConsoleTab)consoleTab.TabPages[i]).Connection.IsFullyConnected)
                                    {
                                        ((ConsoleTab)consoleTab.TabPages[i]).Connection.AttemptReconnect = false;
                                        ((ConsoleTab)consoleTab.TabPages[i]).Connection.SendData("QUIT :" + ((ConsoleTab)consoleTab.TabPages[i]).Connection.ServerSetting.QuitMessage);
                                        return;
                                    }
                                }
                                //close all the windows related to this tab
                                FormMain.Instance.CloseAllWindows(((ConsoleTab)consoleTab.TabPages[i]).Connection);
                                //remove the server connection from the collection
                                ((ConsoleTab)consoleTab.TabPages[i]).Connection.Dispose();
                                FormMain.Instance.ServerTree.ServerConnections.Remove(((ConsoleTab)consoleTab.TabPages[i]).Connection.ServerSetting.ID);
                                consoleTab.TabPages.RemoveAt(consoleTab.TabPages.IndexOf(consoleTab.TabPages[i]));
                                return;
                            }
                        }
                    }
                }
            }
        }

        private void OnControlRemoved(object sender, ControlEventArgs e)
        {
            //this will close the log file for the particular server tab closed
            if (e.Control.GetType() == typeof(ConsoleTab))
                ((TextWindow)((ConsoleTab)e.Control).Controls[0]).Dispose();
        }

        private void OnTabConsoleDrawItem(object sender, DrawItemEventArgs e)
        {
            string name = consoleTab.TabPages[e.Index].Text;
            Rectangle bounds = e.Bounds;
            e.Graphics.FillRectangle(new SolidBrush(Color.White), bounds);

            if (e.Index == consoleTab.SelectedIndex)
            {
                bounds.Offset(4, 2);
                e.Graphics.DrawString(name, this.Font, new SolidBrush(Color.Red), bounds);
                bounds.Offset(0, -1);
            }
            else
            {
                bounds.Offset(2, 3);
                e.Graphics.DrawString(name, this.Font, new SolidBrush(Color.Black), bounds);
                bounds.Offset(4, -2);
            }
            if (e.Index != 0 && e.Index == consoleTab.SelectedIndex)
            {
                System.Drawing.Image icon = StaticMethods.LoadResourceImage("CloseButton.png");
                e.Graphics.DrawImage(icon, bounds.Right - 20, bounds.Top + 4, 12, 12);
                icon.Dispose();
            }

        }

        private void OnTabConsoleSelectingTab(object sender, TabControlCancelEventArgs e)
        {
            if (consoleTab.GetTabRect(e.TabPageIndex).Contains(consoleTab.PointToClient(Cursor.Position)) && e.TabPageIndex != 0)
            {
                if (this.PointToClient(Cursor.Position).X > consoleTab.GetTabRect(e.TabPageIndex).Right - 14)
                    e.Cancel = true;
            }
        }

        private void InitializeConsole()
        {
            this.SuspendLayout();
            // 
            // consoleTab
            // 
            this.consoleTab = new TabControl();
            this.consoleTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.consoleTab.Font = new System.Drawing.Font("Verdana", 10F);
            this.consoleTab.Location = new System.Drawing.Point(0, 0);
            this.consoleTab.Name = "consoleTab";
            this.consoleTab.SelectedIndex = 0;
            this.consoleTab.Size = new System.Drawing.Size(200, 100);
            this.consoleTab.TabIndex = 0;
            // 
            // ConsoleTabWindow
            // 
            this.Controls.Add(this.consoleTab);
            //this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //this.ImageIndex = 0;
            this.ResumeLayout(false);
            
            consoleTab.DrawMode = TabDrawMode.OwnerDrawFixed;
            consoleTab.SizeMode = TabSizeMode.Normal;
            consoleTab.SelectedIndexChanged += new EventHandler(OnTabConsoleSelectedIndexChanged);
            consoleTab.MouseUp += new MouseEventHandler(OnTabConsoleMouseUp);
            consoleTab.MouseDown += new MouseEventHandler(OnTabConsoleMouseDown);
            consoleTab.DrawItem += new DrawItemEventHandler(OnTabConsoleDrawItem);
            consoleTab.Selecting += new TabControlCancelEventHandler(OnTabConsoleSelectingTab);

            consoleTab.ControlRemoved += new ControlEventHandler(OnControlRemoved);

        }
        
        private void InitializeChannelList()
        {
            this.channelList = new ChannelListView();
            this.channelList.SuspendLayout();
            this.SuspendLayout();

            this.channelList.Dock = DockStyle.Fill;
            this.channelList.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.channelList.Location = new System.Drawing.Point(0, 0);
            this.channelList.Name = "channelList";
            this.channelList.DoubleClick += new EventHandler(channelList_DoubleClick);
            this.channelList.ColumnClick += new ColumnClickEventHandler(channelList_ColumnClick);
            
            ColumnHeader c = new ColumnHeader();
            c.Text = "Channel";
            c.Width = 200;
            this.channelList.Columns.Add(c);
            
            this.channelList.Columns.Add("Users");
            ColumnHeader t = new ColumnHeader();
            t.Text = "Topic";
            t.Width = 2000;
            this.channelList.Columns.Add(t);

            this.channelList.View = View.Details;
            this.channelList.MultiSelect = false;
            this.channelList.FullRowSelect = true;
            

            this.Controls.Add(channelList);
            this.channelList.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void channelList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewSorter Sorter = new ListViewSorter();
            channelList.ListViewItemSorter = Sorter;
            if (!(channelList.ListViewItemSorter is ListViewSorter))
                return;

            Sorter = (ListViewSorter)channelList.ListViewItemSorter;
            if (Sorter.LastSort == e.Column)
            {
                if (channelList.Sorting == SortOrder.Descending)
                    channelList.Sorting = SortOrder.Ascending;
                else
                    channelList.Sorting = SortOrder.Descending;
            }
            else
            {
                channelList.Sorting = SortOrder.Ascending;
            }
            Sorter.ByColumn = e.Column;
            
            channelList.Sort();
        }

        private void channelList_DoubleClick(object sender, EventArgs e)
        {
            foreach (ListViewItem eachItem in channelList.SelectedItems)
                FormMain.Instance.ParseOutGoingCommand(this.connection, "/join " + eachItem.Text);
        }

        private void InitializeChannel()
        {
            this.panelTopic = new System.Windows.Forms.Panel();
            this.textTopic = new IceChat.TextWindow();
            this.textWindow = new IceChat.TextWindow();
            this.panelTopic.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTopic
            // 
            this.panelTopic.Controls.Add(this.textTopic);
            this.panelTopic.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTopic.Location = new System.Drawing.Point(0, 0);
            this.panelTopic.Name = "panelTopic";
            this.panelTopic.Size = new System.Drawing.Size(304, 22);
            this.panelTopic.TabIndex = 1;
            this.panelTopic.Visible = false;
            // 
            // textTopic
            // 
            this.textTopic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textTopic.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textTopic.IRCBackColor = 0;
            this.textTopic.IRCForeColor = 1;
            this.textTopic.Location = new System.Drawing.Point(0, 0);
            this.textTopic.Name = "textTopic";
            this.textTopic.NoColorMode = false;
            this.textTopic.ShowTimeStamp = true;
            this.textTopic.SingleLine = true;
            this.textTopic.Size = new System.Drawing.Size(304, 22);
            this.textTopic.TabIndex = 0;
            // 
            // textWindow
            // 
            this.textWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textWindow.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textWindow.IRCBackColor = 0;
            this.textWindow.IRCForeColor = 0;
            this.textWindow.Location = new System.Drawing.Point(0, 22);
            this.textWindow.Name = "textWindow";
            this.textWindow.NoColorMode = false;
            this.textWindow.ShowTimeStamp = true;
            this.textWindow.SingleLine = false;
            this.textWindow.Size = new System.Drawing.Size(304, 166);
            this.textWindow.TabIndex = 0;
            // 
            // TabWindow
            // 
            this.Controls.Add(this.textWindow);
            this.Controls.Add(this.panelTopic);
            this.Size = new System.Drawing.Size(304, 188);
            this.panelTopic.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }

    public class ConsoleTab : TabPage
    {
        public IRCConnection Connection;

        public ConsoleTab(string serverName)
        {
            base.Text = serverName;
        }
    }
    
    //flicker free listview for channel list
    public class ChannelListView : ListView
    {
        public ChannelListView()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.EnableNotifyMessage, true);
        }

        protected override void OnNotifyMessage(Message m)
        {
            // filter WM_ERASEBKGND
            if (m.Msg != 0x14)
            {
                base.OnNotifyMessage(m);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pea)
        {
            // do nothing here since this event is now handled by OnPaint
        }

        protected override void OnPaint(PaintEventArgs pea)
        {
            base.OnPaint(pea);
        }


    }

    public class ListViewSorter : System.Collections.IComparer
    {
        public int Compare(object o1, object o2)
        {
            if (!(o1 is ListViewItem))
                return (0);
            if (!(o2 is ListViewItem))
                return (0);

            ListViewItem lvi1 = (ListViewItem)o2;
            string str1 = lvi1.SubItems[ByColumn].Text;
            ListViewItem lvi2 = (ListViewItem)o1;
            string str2 = lvi2.SubItems[ByColumn].Text;

            int result;
            if (lvi1.ListView.Sorting == SortOrder.Ascending)
            {
                int r1;
                int r2;
                if (int.TryParse(str1, out r1) && int.TryParse(str2, out r2))
                {
                    //check if numeric
                    //System.Diagnostics.Debug.WriteLine("check str1 str2:" + str1 + ":" + str2);
                    if (Convert.ToInt32(str1) > Convert.ToInt32(str2))
                        result = 1;
                    else
                        result = -1;
                }
                else
                    result = String.Compare(str1, str2);
            }
            else
            {
                int r3;
                int r4;
                if (int.TryParse(str1, out r3) && int.TryParse(str2, out r4))
                {
                    //check if numeric
                    //System.Diagnostics.Debug.WriteLine("check str1 str2:" + str1 + ":" + str2);
                    if (Convert.ToInt32(str1) < Convert.ToInt32(str2))
                        result = 1;
                    else
                        result = -1;
                }
                else
                    result = String.Compare(str2, str1);
                //result = String.Compare(str2, str1);
            }
            LastSort = ByColumn;

            return (result);
        }


        public int ByColumn
        {
            get { return Column; }
            set { Column = value; }
        }
        int Column = 0;

        public int LastSort
        {
            get { return LastColumn; }
            set { LastColumn = value; }
        }
        int LastColumn = 0;
    }   

}
