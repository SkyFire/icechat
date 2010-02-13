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

        private System.Windows.Forms.Panel panelTopic;
        //private int ImageIndex;

        private TextWindow textWindow;
        private TextWindow textTopic;
        private WindowType windowType;

        private TabControl consoleTab;

        private FormMain.ServerMessageType lastMessageType;

        private bool DisableConsoleSelectChangedEvent = false;

        public enum WindowType
        {
            Console = 1,
            Channel = 2,
            Query = 3,
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
                //this.textWindow.AppendText(sCaption, 4);

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
            if (this.windowType != WindowType.Console)
                textWindow.Dispose();
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
                FormMain.Instance.WriteErrorFile("IceTabPage UpdateChannelMode Error:" + e.Message, e.StackTrace);
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
                FormMain.Instance.WriteErrorFile("IceTabPage UpdateChannelMode2 Error:" + e.Message, e.StackTrace);
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
                System.Diagnostics.Debug.WriteLine(i + ":" + nickNumber + ":" + nicks.Count);
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
                    //this.ImageIndex = 0;
                }
                else if (windowType == WindowType.Channel)
                {
                    panelTopic.Visible = true;
                    //this.ImageIndex = 1;
                    textWindow.IRCBackColor = FormMain.Instance.IceChatColors.ChannelBackColor;
                    textTopic.IRCBackColor = FormMain.Instance.IceChatColors.ChannelBackColor;
                }
                else if (windowType == WindowType.Query)
                {
                    //this.ImageIndex = 2;
                    textWindow.IRCBackColor = FormMain.Instance.IceChatColors.QueryBackColor;
                }
                else if (windowType == WindowType.Debug)
                {
                    //this.ImageIndex = 4;
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

                    lastMessageType = value;
                    FormMain.Instance.TabMain.Invalidate();
                    FormMain.Instance.ServerTree.Invalidate();
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
                //get the topic color                
                textTopic.AppendText(topic, 1);
            }
        }

        private void OnControlAdded(object sender, ControlEventArgs e)
        {
            e.Control.Dock = DockStyle.Fill;
        }

        public string TabCaption
        {
            get { return _tabCaption; }
        }

        internal void SelectConsoleTab(ConsoleTab c)
        {
            DisableConsoleSelectChangedEvent = true;
            consoleTab.SelectedTab = c;
            DisableConsoleSelectChangedEvent = false;
        }

        private void OnTabConsoleSelectedIndexChanged(object sender, EventArgs e)
        {
            if (consoleTab.TabPages.IndexOf(consoleTab.SelectedTab) != 0 && !DisableConsoleSelectChangedEvent)
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
                System.Drawing.Image icon = new Bitmap(Properties.Resources.CloseButton);
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

}
