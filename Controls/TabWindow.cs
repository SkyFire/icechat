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

namespace IceChat2009
{
    public class TabWindow : System.Windows.Forms.TabPage
    {    
        private IRCConnection connection;

        private Hashtable nicks;        
        private string windowName;

        //channel settings
        private string channelTopic = "";
        private string channelModes = "";
        private string channelModeParams = "";

        private bool isFullyJoined = false;

        private delegate void ChangeTopicDelegate(string topic);
        private delegate void ChangeTextDelegate(string text);

        private System.Windows.Forms.Panel panelTopic;

        private TextWindow textWindow;
        private TextWindow textTopic;
        private WindowType windowType;

        private FormMain.ServerMessageType lastMessageType;

        public enum WindowType
        {
            Console = 1,
            Channel = 2,
            Query = 3,
            Debug = 99
        }
        

        public TabWindow(string title) : base()
        {
            InitializeComponent();
            
            nicks = new Hashtable();
            
            windowName = title;

            this.Text = title;

            lastMessageType = FormMain.ServerMessageType.Default;


        }

        internal bool NickExists(string nick)
        {
            return nicks.ContainsKey(nick);
        }
        
        internal void UpdateChannelMode(string mode, bool addMode)
        {
            //update the channelModes value
            if (channelModes.Contains(mode) && !addMode)
            {
                channelModes = channelModes.Replace(mode, "");
            }
            else if (!channelModes.Contains(mode) && addMode)
            {
                channelModes += mode;    
            }
        }

        internal void UpdateChannelMode(string mode, string param, bool addMode)
        {
            try
            {
                //update the channelModes value
                if (channelModes.Contains(mode) && !addMode)
                {
                    channelModes = channelModes.Replace(mode, "");
                }
                else if (!channelModes.Contains(mode) && addMode)
                {
                    channelModes += mode;
                }
                //update channelMode Parameters
                if (param.Length > 0)
                {
                    if (channelModeParams.Contains(param) && !addMode)
                    {
                        channelModeParams = channelModeParams.Replace(param, "");
                    }
                    else if (!channelModeParams.Contains(param) && addMode)
                    {
                        channelModeParams += " " + param;
                    }
                }
                
                if (channelModeParams == " ")
                    channelModeParams = "";

                System.Diagnostics.Debug.WriteLine(channelModes + "::" + channelModeParams);

            }
            catch (Exception e)
            {
                FormMain.Instance.ReportError(e.Message, e.StackTrace, "TabWindow::UpdateMode:" + mode + ":" + param + ":" + addMode);
            }
        }

        internal void UpdateNick(string nick, string host)
        {            
            if (host.Length > 0)
            {
                string justNick = nick;
                for (int i = 0; i < connection.ServerSetting.StatusModes[1].Length; i++)
                    justNick = justNick.Replace(connection.ServerSetting.StatusModes[1][i].ToString(), string.Empty);

                if (nicks.ContainsKey(justNick))
                {
                    User u = (User)nicks[justNick];

                    u.Host = host;
                    if (FormMain.Instance.CurrentWindowType != WindowType.Console)
                        if (FormMain.Instance.CurrentWindow.WindowName == this.windowName)
                            if (FormMain.Instance.CurrentWindow.Connection == this.connection)
                                FormMain.Instance.NickList.RefreshList(this);
                }

            }
        }
        
        internal void UpdateNick(string nick, string host, string mode, bool addMode)
        {
            string justNick = nick;
            
            for (int i = 0; i < connection.ServerSetting.StatusModes[1].Length; i++)
                justNick = justNick.Replace(connection.ServerSetting.StatusModes[1][i].ToString(), string.Empty);

            if (nicks.ContainsKey(justNick))
            {
                User u = (User)nicks[justNick];

                if (host.Length > 0)
                    u.Host = host;

                for (int i = 0; i < connection.ServerSetting.StatusModes[1].Length; i++)
                    if (mode == connection.ServerSetting.StatusModes[1][i].ToString())
                        u.Level[i] = addMode;

                if (FormMain.Instance.CurrentWindowType != WindowType.Console)
                    if (FormMain.Instance.CurrentWindow.WindowName == this.windowName)
                        if (FormMain.Instance.CurrentWindow.Connection == this.connection)
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
                if (FormMain.Instance.CurrentWindowType != WindowType.Console)
                    if (FormMain.Instance.CurrentWindow.WindowName == this.windowName)
                        if (FormMain.Instance.CurrentWindow.Connection == this.connection)
                            FormMain.Instance.NickList.RefreshList(this);
            }
        }

        internal void AddNick(string nick, string host, bool refresh)
        {
            //replace any user modes from the nick
            string justNick = nick;
            for (int i = 0; i < connection.ServerSetting.StatusModes[1].Length; i++)
                justNick = justNick.Replace(connection.ServerSetting.StatusModes[1][i].ToString(), string.Empty);

            if (nicks.ContainsKey(justNick))
                return;

            User u = new User(nick, host, this.connection);

            nicks.Add(justNick, u);

            if (refresh)
            {
                if (FormMain.Instance.CurrentWindowType != WindowType.Console)
                {
                    if (FormMain.Instance.CurrentWindow.WindowName == this.windowName)
                    {
                        if (FormMain.Instance.CurrentWindow.Connection == this.connection)
                            FormMain.Instance.NickList.RefreshList(this);
                    }
                }
            }
        }

        internal void RemoveNick(string nick)
        {
            nicks.Remove(nick);

            if (FormMain.Instance.CurrentWindowType != WindowType.Console)
            {
                if (FormMain.Instance.CurrentWindow.WindowName == this.windowName)
                {
                    if (FormMain.Instance.CurrentWindow.Connection == this.connection)
                        FormMain.Instance.NickList.RefreshList(this);
                }
            }
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

        internal string WindowName
        {
            get
            {
                return windowName;
            }
            set
            {
                windowName = value;
                UpdateText(value);
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
                    this.ImageIndex = 0;
                }
                else if (windowType == WindowType.Channel)
                {
                    panelTopic.Visible = true;
                    this.ImageIndex = 1;
                    textWindow.IRCBackColor = FormMain.Instance.IceChatColors.ChannelBackColor;
                    textTopic.IRCBackColor = FormMain.Instance.IceChatColors.ChannelBackColor;
                }
                else if (windowType == WindowType.Query)
                {
                    this.ImageIndex = 2;
                    textWindow.IRCBackColor = FormMain.Instance.IceChatColors.QueryBackColor;
                }
                else if (windowType == WindowType.Debug)
                {
                    this.ImageIndex = 4;
                }
                
            }
        }

        internal string ChannelModes
        {
            get
            {
                return channelModes + channelModeParams;
            }
            set
            {
                System.Diagnostics.Debug.WriteLine(value);
                channelModes = value;
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
                    if (FormMain.Instance.CurrentWindowType != WindowType.Console)
                    {
                        if (this == FormMain.Instance.CurrentWindow)
                        {
                            lastMessageType = FormMain.ServerMessageType.Default;
                            return;
                        }                        
                    }

                    lastMessageType = value;
                    ((IceTabControl)this.Parent).RefreshTabs();
                    FormMain.Instance.ServerTree.Invalidate();
                }
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
        
        #region InitializeComponent

        private void InitializeComponent()
        {
            this.panelTopic = new System.Windows.Forms.Panel();
            this.textTopic = new IceChat2009.TextWindow();
            this.textWindow = new IceChat2009.TextWindow();
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
        #endregion

    }


}
