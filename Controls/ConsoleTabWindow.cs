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
using System.Text;
using System.Windows.Forms;
using System.Collections;


namespace IceChat2009
{
    public class ConsoleTabWindow : System.Windows.Forms.TabPage
    {
        private TabControl consoleTab;
        private FormMain.ServerMessageType lastMessageType;

        public ConsoleTabWindow() : base()
        {
            InitializeComponent();

            consoleTab.SelectedIndexChanged += new EventHandler(OnSelectedIndexChanged);
            consoleTab.MouseUp += new MouseEventHandler(OnMouseUp);
            consoleTab.MouseDown += new MouseEventHandler(OnMouseDown);
            
        }

        /// <summary>
        /// Add a message to the Text Window for Selected Console Tab Connection
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="data"></param>
        /// <param name="color"></param>
        internal void AddText(IRCConnection connection, string data, int color)
        {
            foreach (ConsoleTab t in consoleTab.TabPages)
            {
                if (t.Connection == connection)
                {
                    ((TextWindow)t.Controls[0]).AppendText(data, color);
                    return;
                }
            }
        }
        
        /// <summary>
        /// Return the Console Tab Control
        /// </summary>
        internal TabControl ConsoleTab
        {
            get { return consoleTab; }
        }
        
        /// <summary>
        /// Return the Text Window for the Current Selected Tab in the Console Tab Control
        /// </summary>
        internal TextWindow CurrentWindow
        {
            get
            {
                return (TextWindow)consoleTab.SelectedTab.Controls[0];
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
        
        /// <summary>
        /// Get/Set the last message type
        /// </summary>
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
                    lastMessageType = value;
                    //repaint the tab
                    ((IceTabControl)this.Parent).RefreshTabs();

                    FormMain.Instance.ServerTree.Invalidate();
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

            t.Controls.Add(w);
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

        /// <summary>
        /// Console Tab Has New Tab Selected
        /// Update the Status Text accordingly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
			if (consoleTab.TabPages.IndexOf(consoleTab.SelectedTab) != 0)
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
                {
                    FormMain.Instance.StatusText(((ConsoleTab)consoleTab.SelectedTab).Connection.ServerSetting.NickName + " disconnected (" + ((ConsoleTab)consoleTab.SelectedTab).Connection.ServerSetting.ServerName + ")");
                }

                //highlite the proper item in the server tree
                FormMain.Instance.ServerTree.SelectTab(((ConsoleTab)consoleTab.SelectedTab).Connection.ServerSetting);
            }
            else
            {
                FormMain.Instance.ServerTree.SelectTab(this);
                FormMain.Instance.InputPanel.CurrentConnection = null;
                FormMain.Instance.StatusText("Welcome to IceChat 2009");
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            FormMain.Instance.FocusInputBox();
        }

        /// <summary>
        /// Checks if Middle Mouse Button is Pressed
        /// Quits Server if Server is Connected
        /// Closes Server Tab if Server is Disconnected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            //get the current tab
            if (e.Button == MouseButtons.Middle)
            {                
                for (int i = consoleTab.TabPages.Count - 1; i >=0; i--)
                {                    
					if (consoleTab.GetTabRect(i).Contains(e.Location))
                    {
                        if (((ConsoleTab)consoleTab.TabPages[i]).Connection != null)
                        {
                            //check if connected or not                        
                            if (((ConsoleTab)consoleTab.TabPages[i]).Connection.IsConnected)
                            {
                                if (((ConsoleTab)consoleTab.TabPages[i]).Connection.IsFullyConnected)
                                {
                                    ((ConsoleTab)consoleTab.TabPages[i]).Connection.SendData("QUIT :" + ((ConsoleTab)consoleTab.TabPages[i]).Connection.ServerSetting.QuitMessage);
                                    return;
                                }
                            }

                            //close all the windows related to this tab
                            FormMain.Instance.CloseAllWindows(((ConsoleTab)consoleTab.TabPages[i]).Connection);
                            //remove the server connection from the collection
                            ((ConsoleTab)consoleTab.TabPages[i]).Connection.Dispose();
                            FormMain.Instance.ServerTree.ServerConnections.Remove(((ConsoleTab)consoleTab.TabPages[i]).Connection.ServerSetting.ID);
							consoleTab.TabPages.RemoveAt(i);
                            return;
                        }
                    }
                }
            }
        }
		
        private void InitializeComponent()
        {
            this.consoleTab = new TabControl();
            this.consoleTab.Font = new System.Drawing.Font("Verdana", 10);
            this.consoleTab.Dock = DockStyle.Fill;
            this.ImageIndex = 0;
            this.Text = "Console";

            this.Controls.Add(consoleTab);
        }
    
    }

    public class ConsoleTab : System.Windows.Forms.TabPage
    {
        public IRCConnection Connection;

        public ConsoleTab(string serverName) : base()         
        {
            base.Text = serverName;
        }
    }
}
