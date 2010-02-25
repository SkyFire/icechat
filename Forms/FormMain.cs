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
using System.Data;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;

using IceChat.Properties;
using IceChatPlugin;

namespace IceChat
{
    public partial class FormMain : Form
    {
        internal static FormMain Instance;

        private string optionsFile;
        private string messagesFile;
        private string fontsFile;
        private string colorsFile;
        private string favoriteChannelsFile;
        private string serversFile;
        private string aliasesFile;
        private string popupsFile;
        private string highlitesFile;
        private string currentFolder;
        private string logsFolder;
        private string emoticonsFile;

        private StreamWriter errorFile;

        private IceChatOptions iceChatOptions;
        private IceChatColors iceChatColors;
        private IceChatMessageFormat iceChatMessages;
        private IceChatFontSetting iceChatFonts;
        private IceChatAliases iceChatAliases;
        private IceChatPopupMenus iceChatPopups;
        private IceChatEmoticon iceChatEmoticons;

        private ArrayList loadedPlugins;

        private IdentServer identServer;

        private delegate void AddWindowDelegate(IRCConnection connection, string windowName, IceTabPage.WindowType windowType);
        private delegate void RemoveTabDelegate(IRCConnection connection, string channel);
        private delegate int GetSelectedTabDelegate();

        private delegate void StatusTextDelegate(string data);
        private delegate void CurrentWindowDelegate(string data, int color);
        private delegate void WindowMessageDelegate(IRCConnection connection, string name, string data, int color, bool scrollToBottom);
        private delegate void CurrentWindowMessageDelegate(IRCConnection connection, string data, int color, bool scrollToBottom);

        /// <summary>
        /// All the Window Message Types used for Coloring the Tab Text for Different Events
        /// </summary>
        internal enum ServerMessageType
        {
            Default = 0,
            Message = 1,            
            Action = 2,
            JoinChannel = 3,
            PartChannel = 4,
            QuitServer = 5,
            ServerMessage = 6,
            Other = 7
        }

        public FormMain()
        {
            FormMain.Instance = this;

            #region Settings Files 
            
            //check if the xml settings files exist in current folder
            currentFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            if (!File.Exists(currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatServer.xml"))
            {
                if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar + "IceChat Networks" + Path.DirectorySeparatorChar + "IceChat"))
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar + "IceChat Networks" + Path.DirectorySeparatorChar + "IceChat");

                currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar + "IceChat Networks" + Path.DirectorySeparatorChar + "IceChat";
            }
            
            //load all files from the Local AppData folder, unless it exist in the current folder
            serversFile = currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatServer.xml";
            optionsFile = currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatOptions.xml";
            messagesFile = currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatMessages.xml";
            fontsFile = currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatFonts.xml";
            colorsFile = currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatColors.xml";
            favoriteChannelsFile = currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatChannels.xml";
            aliasesFile = currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatAliases.xml";
            popupsFile = currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatPopups.xml";
            highlitesFile = currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatHighLites.xml";
            emoticonsFile = currentFolder + System.IO.Path.DirectorySeparatorChar + "Emoticons" + System.IO.Path.DirectorySeparatorChar + "IceChatEmoticons.xml";

            logsFolder = currentFolder + System.IO.Path.DirectorySeparatorChar + "Logs";

            #endregion


            LoadOptions();
            LoadColors();

            InitializeComponent();

            serverTree = new ServerTree();
            
            panelLeft.Controls.Add(serverTree);

            this.Text = IceChat.Properties.Settings.Default.ProgramID + " " + IceChat.Properties.Settings.Default.Version + " - February 24 2010";

            if (!Directory.Exists(logsFolder))
                Directory.CreateDirectory(logsFolder);
            try
            {
                errorFile = new StreamWriter(logsFolder + System.IO.Path.DirectorySeparatorChar + "errors.log");
            }
            catch (IOException)
            {
            }

            if (!iceChatOptions.TimeStamp.EndsWith(" "))
                iceChatOptions.TimeStamp += " ";

            if (iceChatOptions.SaveWindowPosition)
            {                
                if (iceChatOptions.WindowSize != null)
                {
                    if (iceChatOptions.WindowSize.Width != 0)
                        this.Size = iceChatOptions.WindowSize;
                }
                if (iceChatOptions.WindowLocation != null)
                    this.Location = iceChatOptions.WindowLocation;
            }

            statusStripMain.Visible = iceChatOptions.ShowStatusBar;
            inputPanel.ShowColorPicker = iceChatOptions.ShowColorPicker;
            inputPanel.ShowEmoticonPicker = iceChatOptions.ShowEmoticonPicker;

            LoadAliases();
            LoadPopups();
            LoadEmoticons();
            LoadMessageFormat();
            LoadFonts();

            channelList = new ChannelList();            
            channelList.Dock = DockStyle.Fill;
            channelList.Visible = false;

            toolStripMain.BackColor = IrcColor.colors[iceChatColors.ToolbarBackColor];
            menuMainStrip.BackColor = IrcColor.colors[iceChatColors.MenubarBackColor];
            statusStripMain.BackColor = IrcColor.colors[iceChatColors.StatusbarBackColor];
            toolStripStatus.ForeColor = IrcColor.colors[iceChatColors.StatusbarForeColor];
            inputPanel.SetInputBoxColors();
            channelList.SetListColors();

            nickList = new NickList();
            nickList.Header = "Console";
            nickList.Dock = DockStyle.Fill;
            nickList.Font = new Font(iceChatFonts.FontSettings[3].FontName,iceChatFonts.FontSettings[3].FontSize);

            panelRight.Controls.Add(channelList);
            panelRight.Controls.Add(nickList);
            panelRight.Controls.Add(panelRightBottom);

            serverTree.Dock = DockStyle.Fill;
            serverTree.Font = new Font(iceChatFonts.FontSettings[4].FontName, iceChatFonts.FontSettings[4].FontSize);

            panelLeft.Width = iceChatOptions.LeftPanelWidth;
            panelRight.Width = iceChatOptions.RightPanelWidth;

            inputPanel.OnCommand +=new InputPanel.OnCommandDelegate(inputPanel_OnCommand);
            inputPanel.InputBoxFont = new Font(iceChatFonts.FontSettings[5].FontName, iceChatFonts.FontSettings[5].FontSize);

            mainTabControl.SelectedIndexChanged += new EventHandler(TabSelectedIndexChanged);
            mainTabControl.OnTabClosed += new IceTabControl.TabClosedDelegate(mainTabControl_OnTabClosed);
            
            serverTree.NewServerConnection += new NewServerConnectionDelegate(NewServerConnection);
            
            CreateDefaultConsoleWindow();

            this.FormClosing += new FormClosingEventHandler(FormMainClosing);

            tabPanelRight.SelectedIndexChanged += new EventHandler(tabPanelRight_SelectedIndexChanged);

            if (iceChatOptions.IdentServer)
                identServer = new IdentServer();

            loadedPlugins = new ArrayList();

            //load any plugin addons
            LoadPlugins();

            //fire the event that the program has fully loaded
            foreach (IPluginIceChat ipc in FormMain.Instance.IceChatPlugins)
            {
                ipc.MainProgramLoaded();
            }
        }

        private void FormMainClosing(object sender, FormClosingEventArgs e)
        {
            if (identServer != null)
            {
                identServer.Stop();
                identServer = null;
            }

            //disconnect all the servers
            foreach (IRCConnection c in serverTree.ServerConnections.Values)
            {
                if (c.IsConnected)
                {
                    c.AttemptReconnect = false;
                    ParseOutGoingCommand(c, "//quit " + c.ServerSetting.QuitMessage);
                }
            }

            if (iceChatOptions.SaveWindowPosition)
            {
                //save the window position , as long as its not minimized
                if (this.WindowState != FormWindowState.Minimized)
                {
                    iceChatOptions.WindowLocation = this.Location;
                    iceChatOptions.WindowSize = this.Size;
                    iceChatOptions.RightPanelWidth = panelRight.Width;
                    iceChatOptions.LeftPanelWidth = panelLeft.Width;
                }

                SaveOptions();
            }
            
            if (errorFile != null)
            {
                errorFile.Close();
                errorFile.Dispose();
            }
        }

        /// <summary>
        /// Create a Default Tab for showing Welcome Information
        /// </summary>
        private void CreateDefaultConsoleWindow()
        {
            IceTabPage p = new IceTabPage(IceTabPage.WindowType.Console, "Console");
            p.AddConsoleTab("Welcome");
            //mainTabControl.Controls.Add(p);
            mainTabControl.TabPages.Add(p);

            WindowMessage(null, "Console", "\x00034Welcome to " + Settings.Default.ProgramID + " " + Settings.Default.Version, 1, false);
            WindowMessage(null, "Console", "\x00034** This is an Alpha version, not fully functional **", 1, false);
            WindowMessage(null, "Console", "\x00033If you want a fully working version of \x0002IceChat\x0002, visit http://www.icechat.net and download IceChat 7.63", 1, false);
            WindowMessage(null, "Console", "\x00034Please visit \x00030,4#icechat2009\x0003 on \x00030,2irc://irc.quakenet.org\x0003 if you wish to help with this project", 1, true);

            StatusText("Welcome to " + Settings.Default.ProgramID + " " + Settings.Default.Version);
        }

        #region Internal Properties

        /// <summary>
        /// Gets the instance of the Nick List
        /// </summary>
        internal NickList NickList
        {
            get { return nickList; } 
        }
        /// <summary>
        /// Gets the instance of the Server Tree
        /// </summary>
        internal ServerTree ServerTree
        {
            get { return serverTree; }
        }
        /// <summary>
        /// Gets the instance of the Main Tab Control
        /// </summary>
        internal IceTabControl TabMain
        {
            get { return mainTabControl; }
        }

        /// <summary>
        /// Gets the instance of the InputPanel
        /// </summary>
        internal InputPanel InputPanel
        {
            get
            {
                return this.inputPanel;
            }
        }

        internal IceChatOptions IceChatOptions
        {
            get
            {
                return this.iceChatOptions;
            }
        }

        internal IceChatMessageFormat MessageFormats
        {
            get
            {
                return this.iceChatMessages;
            }
        }

        internal IceChatFontSetting IceChatFonts
        {
            get
            {
                return this.iceChatFonts;
            }
        }
        
        internal IceChatColors IceChatColors
        {
            get
            {
                return this.iceChatColors;
            }
        }

        internal IceChatAliases IceChatAliases
        {
            get
            {
                return iceChatAliases;
            }
            set
            {
                iceChatAliases = value;
                //save the aliases
                SaveAliases();
            }
        }

        internal IceChatPopupMenus IceChatPopupMenus
        {
            get
            {
                return iceChatPopups;
            }
            set
            {
                iceChatPopups = value;
                //save the popups
                SavePopups();
            }

        }

        internal IceChatEmoticon IceChatEmoticons
        {
            get
            {
                return iceChatEmoticons;
            }
            set
            {
                iceChatEmoticons = value;
                //save the Emoticons
                SaveEmoticons();
            }
        }

        internal string FavoriteChannelsFile
        {
            get
            {
                return favoriteChannelsFile;
            }
        }

        internal string ServersFile
        {
            get
            {
                return serversFile;
            }
        }

        internal string AliasesFile
        {
            get
            {
                return aliasesFile;
            }
        }

        internal ArrayList IceChatPlugins
        {
            get
            {
                return loadedPlugins;
            }
        }

        internal string LogsFolder
        {
            get
            {
                return logsFolder;
            }
        }

        internal string CurrentFolder
        {
            get
            {
                return currentFolder;
            }
        }

        internal string EmoticonsFolder
        {
            get
            {
                return System.IO.Path.GetDirectoryName(emoticonsFile);
            }
        }

        internal void StatusText(string data)
        {
            if (this.InvokeRequired)
            {
                StatusTextDelegate s = new StatusTextDelegate(StatusText);
                this.Invoke(s, new object[] { data });
            }
            else
            {
                if (inputPanel.CurrentConnection != null)
                    toolStripStatus.Text = inputPanel.CurrentConnection.ServerSetting.ID + ":Status: " + data;
                else
                    toolStripStatus.Text = "Status: " + data;
            }
        }

        #endregion

        #region Private Properties
        /// <summary>
        /// Set focus to the Input Panel
        /// </summary>
        internal void FocusInputBox()
        {
            inputPanel.FocusTextBox();
        }

        /// <summary>
        /// Sends a Message to a Named Window
        /// </summary>
        /// <param name="connection">Which Connection to use</param>
        /// <param name="name">Name of the Window</param>
        /// <param name="data">Message to send</param>
        /// <param name="color">Color number of the message</param>
        internal void WindowMessage(IRCConnection connection, string name, string data, int color, bool scrollToBottom)
        {
            if (this.InvokeRequired)
            {
                WindowMessageDelegate w = new WindowMessageDelegate(WindowMessage);
                this.Invoke(w, new object[] { connection, name, data, color, scrollToBottom} );
            }
            else
            {
                if (name == "Console")
                {
                    mainTabControl.GetTabPage("Console").AddText(connection, data, color, scrollToBottom);                    
                }
                else
                {
                    foreach (IceTabPage t in mainTabControl.TabPages)
                    {
                        if (t.TabCaption == name)
                        {
                            if (t.Connection == connection)
                            {
                                t.TextWindow.AppendText(data, color);
                                if (scrollToBottom)
                                    t.TextWindow.ScrollToBottom();
                                return;
                            }
                        }
                    }
                    
                    WindowMessage(connection, "Console", data, color, scrollToBottom);
                }
            }
        }
        /// <summary>
        /// Send a Message to the Current Window
        /// </summary>
        /// <param name="connection">Which Connection to use</param>
        /// <param name="data">Message to send</param>
        /// <param name="color">Color number of the message</param>
        internal void CurrentWindowMessage(IRCConnection connection, string data, int color, bool scrollToBottom)
        {
            if (this.InvokeRequired)
            {
                CurrentWindowMessageDelegate w = new CurrentWindowMessageDelegate(CurrentWindowMessage);
                this.Invoke(w, new object[] { connection, data, color, scrollToBottom });
            }
            else
            {
                //check what type the current window is
                if (CurrentWindowType != IceTabPage.WindowType.Console)
                {
                    IceTabPage t = mainTabControl.CurrentTab;
                    if (t != null)
                    {
                        if (t.Connection == connection)
                        {
                            t.TextWindow.AppendText(data, color);
                        }
                        else
                        {
                            WindowMessage(connection, "Console", data, color, scrollToBottom);
                        }
                    }
                }
                else
                {
                    //console window is current window
                    mainTabControl.GetTabPage("Console").AddText(connection, data, color, false);

                }
            }
        }
        /// <summary>
        /// Gets a Tab Window
        /// </summary>
        /// <param name="connection">Which Connection to use</param>
        /// <param name="name">Name of the Window</param>
        /// <param name="windowType">The Window Type</param>
        /// <returns></returns>
        internal IceTabPage GetWindow(IRCConnection connection, string sCaption, IceTabPage.WindowType windowType)
        {
            foreach (IceTabPage t in mainTabControl.TabPages)
            {
                if (t.TabCaption.ToLower() == sCaption.ToLower() && t.WindowStyle == windowType)
                {
                    if (t.Connection == connection)
                        return t;
                }
            }
            return null;
        }
        
        /// <summary>
        /// Get the Current Tab Window
        /// </summary>
        internal IceTabPage CurrentWindow
        {
            get
            {
                return mainTabControl.CurrentTab;
            }
        }
        
        /// <summary>
        /// Get the Current Window Type
        /// </summary>
        internal IceTabPage.WindowType CurrentWindowType
        {
            get
            {
                if (mainTabControl.CurrentTab != null)
                    return mainTabControl.CurrentTab.WindowStyle;
                else
                {
                    System.Diagnostics.Debug.WriteLine("CurrentWindowType Null:" + mainTabControl.TabPages.Count);
                    return IceTabPage.WindowType.Console;
                }
            }
        }

        #endregion

        #region Private Methods
        
        /// <summary>
        /// Send a Message through the IRC Connection to the Server
        /// </summary>
        /// <param name="connection">Which Connection to use</param>
        /// <param name="data">RAW IRC Message to send</param>
        private void SendData(IRCConnection connection, string data)
        {
            if (connection != null)
            {
                if (connection.IsConnected)
                {
                    if (connection.IsFullyConnected)
                        connection.SendData(data);
                    else
                        //add to a command queue, which gets run once fully connected, after autoperform/autojoin
                        connection.AddToCommandQueue(data);
                }
                else
                {
                    if (CurrentWindowType == IceTabPage.WindowType.Console)
                        WindowMessage(connection, "Console", "Error: Not Connected (" + data + ")", 4, true);
                    else
                    {
                        CurrentWindow.TextWindow.AppendText("Error: Not Connected (" + data + ")", 4);
                        CurrentWindow.TextWindow.ScrollToBottom();
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Create a new Server Connection
        /// </summary>
        /// <param name="serverSetting">Which ServerSetting to use</param>
        private void NewServerConnection(ServerSetting serverSetting)
        {
            IRCConnection c = new IRCConnection(serverSetting);

            c.ChannelMessage += new ChannelMessageDelegate(OnChannelMessage);
            c.ChannelAction += new ChannelActionDelegate(OnChannelAction);
            c.QueryMessage += new QueryMessageDelegate(OnQueryMessage);
            c.QueryAction += new QueryActionDelegate(OnQueryAction);
            c.ChannelNotice += new ChannelNoticeDelegate(OnChannelNotice);

            c.ChangeNick += new ChangeNickDelegate(OnChangeNick);
            c.KickNick += new KickNickDelegate(OnKickNick);

            c.OutGoingCommand += new OutGoingCommandDelegate(OutGoingCommand);
            c.JoinChannel += new JoinChannelDelegate(OnChannelJoin);
            c.PartChannel += new PartChannelDelegate(OnChannelPart);
            c.QuitServer += new QuitServerDelegate(OnServerQuit);

            c.JoinChannelMyself += new JoinChannelMyselfDelegate(OnChannelJoinSelf);
            c.PartChannelMyself += new PartChannelMyselfDelegate(OnChannelPartSelf);
            c.KickMyself += new KickMyselfDelegate(OnKickSelf);

            c.ChannelTopic += new ChannelTopicDelegate(OnChannelTopic);
            c.ChannelMode += new ChannelModeChangeDelegate(OnChannelMode);
            c.UserMode += new UserModeChangeDelegate(OnUserMode);
            c.ChannelInvite += new ChannelInviteDelegate(OnChannelInvite);

            c.ServerMessage += new ServerMessageDelegate(OnServerMessage);
            c.ServerError += new ServerErrorDelegate(OnServerError);
            c.ServerMOTD += new ServerMOTDDelegate(OnServerMOTD);
            c.WhoisData += new WhoisDataDelegate(OnWhoisData);
            c.UserNotice += new UserNoticeDelegate(OnUserNotice);
            c.CtcpMessage += new CtcpMessageDelegate(OnCtcpMessage);
            c.GenericChannelMessage += new GenericChannelMessageDelegate(OnGenericChannelMessage);
            c.ServerNotice += new ServerNoticeDelegate(OnServerNotice);
            c.ChannelList += new ChannelListDelegate(OnChannelList);

            c.UserHostReply += new UserHostReplyDelegate(OnUserHostReply);
            c.IALUserData += new IALUserDataDelegate(OnIALUserData);
            c.IALUserChange += new IALUserChangeDelegate(OnIALUserChange);
            c.IALUserPart += new IALUserPartDelegate(OnIALUserPart);
            c.IALUserQuit += new IALUserQuitDelegate(OnIALUserQuit);

            c.RawServerIncomingData += new RawServerIncomingDataDelegate(OnRawServerData);
            c.RawServerOutgoingData += new RawServerOutgoingDataDelegate(OnRawServerOutgoingData);
            
            OnAddConsoleTab(c);

            mainTabControl.SelectTab(mainTabControl.GetTabPage("Console"));

            inputPanel.CurrentConnection = c;
            serverTree.AddConnection(c);
            
            c.ConnectSocket();

        }

        #region Tab Events and Methods

        /// <summary>
        /// Add a new Connection Tab to the Console
        /// </summary>
        /// <param name="connection">Which Connection to add</param>
        private void OnAddConsoleTab(IRCConnection connection)
        {            
            mainTabControl.GetTabPage("Console").AddConsoleTab(connection);
        }

        /// <summary>
        /// Add a new Tab Window to the Main Tab Control
        /// </summary>
        /// <param name="connection">Which Connection it came from</param>
        /// <param name="windowName">Window Name of the New Tab</param>
        /// <param name="windowType">Window Type of the New Tab</param>
        internal void AddWindow(IRCConnection connection, string windowName, IceTabPage.WindowType windowType)
        {
            if (this.InvokeRequired)
            {
                AddWindowDelegate a = new AddWindowDelegate(AddWindow);
                this.Invoke(a, new object[] { connection, windowName, windowType });
            }
            else
            {
                IceTabPage page = new IceTabPage(windowType, windowName);
                page.Connection = connection;

                if (page.WindowStyle == IceTabPage.WindowType.Channel)
                {
                    page.TextWindow.Font = new Font(iceChatFonts.FontSettings[1].FontName, iceChatFonts.FontSettings[1].FontSize);
                    page.TopicWindow.Font = new Font(iceChatFonts.FontSettings[1].FontName, iceChatFonts.FontSettings[1].FontSize);

                    //send the message
                    string msg = GetMessageFormat("Self Channel Join");
                    msg = msg.Replace("$nick", connection.ServerSetting.NickName).Replace("$channel", windowName);

                    if (FormMain.Instance.IceChatOptions.LogChannel)
                        page.TextWindow.SetLogFile();

                    page.TextWindow.AppendText(msg, 1);
                }
                else if (page.WindowStyle == IceTabPage.WindowType.Query)
                {
                    page.TextWindow.Font = new Font(iceChatFonts.FontSettings[2].FontName, iceChatFonts.FontSettings[2].FontSize);
                    if (FormMain.Instance.IceChatOptions.LogQuery)
                        page.TextWindow.SetLogFile();
                }
                else if (page.WindowStyle == IceTabPage.WindowType.Debug)
                {
                    page.TextWindow.NoColorMode = true;
                    page.TextWindow.Font = new Font("Verdana", 10);
                    page.TextWindow.SetLogFile();
                }
                
                //find the last window index for this connection
                int index = 0;
                if (page.WindowStyle == IceTabPage.WindowType.Channel || page.WindowStyle == IceTabPage.WindowType.Query)
                {
                    for (int i = 1; i < mainTabControl.TabPages.Count; i++)
                    {
                        if (mainTabControl.TabPages[i].Connection == connection)
                            index = i + 1;
                    }
                }

                if (index == 0)
                    mainTabControl.TabPages.Add(page);
                else
                    mainTabControl.TabPages.Insert(index, page);

                if (page.WindowStyle == IceTabPage.WindowType.Query && !iceChatOptions.NewQueryForegound)
                    mainTabControl.SelectTab(mainTabControl.CurrentTab);
                else
                {
                    mainTabControl.SelectTab(page);
                    nickList.CurrentWindow = page;
                }
                
                serverTree.Invalidate();

                if (page.WindowStyle == IceTabPage.WindowType.Query && iceChatOptions.WhoisNewQuery)
                    ParseOutGoingCommand(page.Connection, "/whois " + page.TabCaption);
               
            }
        }
        /// <summary>
        /// Remove a Tab Window from the Main Tab Control
        /// </summary>
        /// <param name="connection">Which Connection it is for</param>
        /// <param name="channel">The Channel/Query Window Name</param>
        internal void RemoveWindow(IRCConnection connection, string channel)
        {
            if (mainTabControl.InvokeRequired)
            {
                RemoveTabDelegate r = new RemoveTabDelegate(RemoveWindow);
                mainTabControl.Invoke(r, new object[] { connection, channel } );
            }
            else
            {
				IceTabPage t = GetWindow(connection, channel, IceTabPage.WindowType.Channel);
				if (t != null)
                {
                    mainTabControl.Controls.Remove(t);
                    return;
                }

                IceTabPage c = GetWindow(connection, channel, IceTabPage.WindowType.Query);
                if (c != null)
                    mainTabControl.Controls.Remove(c);
            }
        }

        /// <summary>
        /// Close All Channels/Query Tabs for specified Connection
        /// </summary>
        /// <param name="connection">Which Connection it is for</param>
        internal void CloseAllWindows(IRCConnection connection)
        {
            for (int i = mainTabControl.TabPages.Count - 1; i > 0; i--)
            {
                if (mainTabControl.TabPages[i].Connection == connection)
                    mainTabControl.TabPages.Remove(mainTabControl.TabPages[i]);

            }
            mainTabControl.Invalidate();
        }

        private string GetMessageFormat(string MessageName)
        {
            foreach (ServerMessageFormatItem msg in iceChatMessages.MessageSettings)
            {
                if (msg.MessageName.ToLower() == MessageName.ToLower())
                    return msg.FormattedMessage;
            }
            return null;
        }
        
        
        /// <summary>
        /// A New Tab was Selected for the Main Tab Control
        /// Update the Input Panel with the Current Connection
        /// Change the Status text for the Status Bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabSelectedIndexChanged(object sender, EventArgs e)
        {
            if (mainTabControl.CurrentTab.WindowStyle != IceTabPage.WindowType.Console)
            {
                //System.Diagnostics.Debug.WriteLine("TabSelected:" + mainTabControl.CurrentTab.TabCaption);
                if (mainTabControl.CurrentTab != null)
                {
                    IceTabPage t = mainTabControl.CurrentTab;
                    nickList.RefreshList(t);
                    inputPanel.CurrentConnection = t.Connection;

                    if (CurrentWindowType == IceTabPage.WindowType.Channel)
                        StatusText(t.Connection.ServerSetting.NickName + " in channel " + t.TabCaption + " [" + t.ChannelModes + "] {" + t.Connection.ServerSetting.RealServerName + "}");
                    else if (CurrentWindowType == IceTabPage.WindowType.Query)
                        StatusText(t.Connection.ServerSetting.NickName + " in private chat with " + t.TabCaption + " {" + t.Connection.ServerSetting.RealServerName + "}");

                    CurrentWindow.LastMessageType = ServerMessageType.Default;
                    t = null;

                    //serverTree.SelectTab(mainTabControl.CurrentTab, false);

                }
            }
            else
            {
                //make sure the 1st tab is not selected
                nickList.RefreshList();
                nickList.Header = "Console";
                if (mainTabControl.GetTabPage("Console").ConsoleTab.SelectedIndex != 0)
                {
                    inputPanel.CurrentConnection = mainTabControl.GetTabPage("Console").CurrentConnection;
                    if (inputPanel.CurrentConnection.IsConnected)
                        StatusText(inputPanel.CurrentConnection.ServerSetting.NickName + " connected to " + inputPanel.CurrentConnection.ServerSetting.RealServerName);
                    else
                        StatusText(inputPanel.CurrentConnection.ServerSetting.NickName + " disconnected (" + inputPanel.CurrentConnection.ServerSetting.ServerName + ")");

                    //serverTree.SelectTab(mainTabControl.GetTabPage("Console").CurrentConnection.ServerSetting , false);
                }
                else
                {
                    inputPanel.CurrentConnection = null;
                    //serverTree.SelectTab(mainTabControl.GetTabPage("Console").CurrentConnection.ServerSetting, false);
                    StatusText("Welcome to IceChat 2009");
                }
            }

            inputPanel.FocusTextBox();
        }


       
        /// <summary>
        /// Closes the Tab selected
        /// </summary>
        /// <param name="tab">Which tab to Close</param>        
        private void mainTabControl_OnTabClosed(int nIndex)
        {
            if (mainTabControl.GetTabPage(nIndex).WindowStyle == IceTabPage.WindowType.Channel)
            {
                foreach (IRCConnection c in serverTree.ServerConnections.Values)
                {
                    if (c == mainTabControl.GetTabPage(nIndex).Connection)
                    {
                        //check if connected
                        if (c.IsConnected)
                            ParseOutGoingCommand(c, "/part " + mainTabControl.GetTabPage(nIndex).TabCaption);
                        else
                            RemoveWindow(c, mainTabControl.GetTabPage(nIndex).TabCaption);

                        return;
                    }
                }
            }
            else if (mainTabControl.GetTabPage(nIndex).WindowStyle == IceTabPage.WindowType.Query)
            {
                mainTabControl.Controls.Remove(mainTabControl.GetTabPage(nIndex));
                return;
            }
            else if (mainTabControl.GetTabPage(nIndex).WindowStyle == IceTabPage.WindowType.Debug)
            {
                mainTabControl.Controls.Remove(mainTabControl.GetTabPage(nIndex));
            }
        }

        private void tabPanelRight_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabPanelRight.SelectedTab.Text == "Nick List")
            {
                nickList.Visible = true;
                channelList.Visible = false;
            }
            else
            {
                nickList.Visible = false;
                channelList.Visible = true;
            }
        }
        #endregion
        
        #region InputPanel Events

        /// <summary>
        /// Parse out command written in Input Box or sent from Script
        /// </summary>
        /// <param name="connection">Which Connection it is for</param>
        /// <param name="data">The Message to Parse</param>
        internal void ParseOutGoingCommand(IRCConnection connection, string data)
        {
            try
            {
                if (data.StartsWith("//"))
                {
                    //parse out identifiers
                    ParseOutGoingCommand(connection, ParseIdentifiers(connection, data, data));
                    return;
                }

                if (data.StartsWith("/"))
                {
                    int indexOfSpace = data.IndexOf(" ");
                    string command = "";
                    string temp = "";

                    if (indexOfSpace > 0)
                    {
                        command = data.Substring(0, indexOfSpace);
                        data = data.Substring(command.Length + 1);
                    }
                    else
                    {
                        command = data;
                        data = "";
                    }

                    //check for aliases
                    foreach (AliasItem a in iceChatAliases.listAliases)
                    {
                        if (a.AliasName == command)
                        {
                            if (a.Command.Length == 1)
                            {
                                data = ParseIdentifierValue(a.Command[0], data);
                                ParseOutGoingCommand(connection, ParseIdentifiers(connection, data, data));
                            }
                            else
                            {
                                //it is a multulined alias, run multiple commands
                                foreach (string c in a.Command)
                                {
                                    data = ParseIdentifierValue(c, data);
                                    ParseOutGoingCommand(connection, ParseIdentifiers(connection, data, data));
                                }
                            }
                            return;
                        }
                    }

                    switch (command)
                    {
                        case "/makeexception":
                            throw new Exception("IceChat 2009 Test Exception Error");
                        case "/userinfo":
                            if (connection != null && data.Length > 0)
                            {
                                FormUserInfo fui = new FormUserInfo(connection);
                                //find the user
                                fui.NickName(data);
                                fui.ShowDialog(this);
                            }
                            break;

                        case "/channelinfo":
                            if (connection != null)
                            {
                                if (data.Length > 0)
                                {
                                    IceTabPage t = GetWindow(connection, data, IceTabPage.WindowType.Channel);
                                    if (t != null)
                                    {
                                        FormChannelInfo fci = new FormChannelInfo(t);
                                        SendData(connection, "MODE " + t.TabCaption + " +b");
                                        //check if mode (e) exists for Exception List
                                        if (connection.ServerSetting.ChannelModeParams.Contains("e"))
                                            SendData(connection, "MODE " + t.TabCaption + " +e");
                                        fci.ShowDialog(this);
                                    }
                                }
                                else
                                {
                                    //check if current window is channel
                                    if (CurrentWindowType == IceTabPage.WindowType.Channel)
                                    {
                                        FormChannelInfo fci = new FormChannelInfo(CurrentWindow);
                                        SendData(connection, "MODE " + CurrentWindow.TabCaption + " +b");
                                        //check if mode (e) exists for Exception List
                                        if (connection.ServerSetting.ChannelModeParams.Contains("e"))
                                            SendData(connection, "MODE " + CurrentWindow.TabCaption + " +e");
                                        fci.ShowDialog(this);
                                    }
                                }
                            }
                            break;
                        case "/ame":    //me command for all channels
                            if (connection != null && data.Length > 0)
                            {
                                foreach (IceTabPage t in FormMain.Instance.TabMain.TabPages)
                                {
                                    if (t.WindowStyle == IceTabPage.WindowType.Channel)
                                    {
                                        if (t.Connection == connection)
                                        {
                                            SendData(connection, "PRIVMSG " + t.TabCaption + " :ACTION " + data + "");
                                            string msg = GetMessageFormat("Self Channel Action");
                                            msg = msg.Replace("$nick", t.Connection.ServerSetting.NickName).Replace("$channel", t.TabCaption);
                                            msg = msg.Replace("$message", data);

                                            t.TextWindow.AppendText(msg, 1);
                                            t.TextWindow.ScrollToBottom();
                                            t.LastMessageType = ServerMessageType.Action;
                                        }
                                    }
                                }
                            }
                            break;
                        case "/autojoin":
                            if (connection != null)
                            {
                                foreach (string chan in connection.ServerSetting.AutoJoinChannels)
                                {
                                    if (!chan.StartsWith(";"))
                                        SendData(connection, "JOIN " + chan);
                                }
                            }
                            break;
                        case "/autoperform":
                            if (connection != null)
                            {
                                foreach (string ap in connection.ServerSetting.AutoPerform)
                                {
                                    string autoCommand = ap.Replace("\r", String.Empty);
                                    if (!autoCommand.StartsWith(";"))
                                        ParseOutGoingCommand(connection, autoCommand);
                                }
                            }
                            break;
                        case "/away":
                            if (connection != null)
                            {
                                if (connection.ServerSetting.Away)
                                {
                                    connection.ServerSetting.Away = false;
                                    ParseOutGoingCommand(connection, "/nick " + connection.ServerSetting.DefaultNick);
                                    TimeSpan t = DateTime.Now.Subtract(connection.ServerSetting.AwayStart);

                                    string s = t.Seconds.ToString() + " secs";
                                    if (t.Minutes > 0)
                                        s = t.Minutes.ToString() + " mins " + s;
                                    if (t.Hours > 0)
                                        s = t.Hours.ToString() + " hrs " + s;
                                    if (t.Days > 0)
                                        s = t.Days.ToString() + " days " + s;

                                    ParseOutGoingCommand(connection, "/ame is no longer away : Gone for " + s);
                                }
                                else
                                {
                                    connection.ServerSetting.Away = true;
                                    connection.ServerSetting.DefaultNick = connection.ServerSetting.NickName;
                                    connection.ServerSetting.AwayStart = System.DateTime.Now;
                                    ParseOutGoingCommand(connection, "/nick " + connection.ServerSetting.AwayNickName);
                                    if (data.Length == 0)
                                        ParseOutGoingCommand(connection, "/ame is set as away");
                                    else
                                        ParseOutGoingCommand(connection, "/ame is set as away : Reason(" + data + ")");
                                }
                            }
                            break;
                        case "/ban":  // /ban #channel nick|address   /mode #channel +b host
                            if (connection != null && data.IndexOf(' ') > 0)
                            {
                                string channel = data.Split(' ')[0];
                                string host = data.Split(' ')[1];
                                ParseOutGoingCommand(connection, "/mode " + channel + " +b " + host);
                            }
                            break;
                        case "/clear":
                            if (data.Length == 0)
                            {
                                if (CurrentWindowType != IceTabPage.WindowType.Console)
                                    CurrentWindow.TextWindow.ClearTextWindow();
                                else
                                {
                                    //find the current console tab window
                                    mainTabControl.GetTabPage("Console").CurrentConsoleWindow().ClearTextWindow();

                                }
                            }
                            else
                            {
                                //find a match
                                if (data == "Console")
                                {
                                    mainTabControl.GetTabPage("Console").CurrentConsoleWindow().ClearTextWindow();
                                    return;
                                }
                                else if (data.ToLower() == "all console")
                                {
                                    //clear all the console windows and channel/queries
                                    foreach (ConsoleTab c in mainTabControl.GetTabPage("Console").ConsoleTab.TabPages)
                                        ((TextWindow)c.Controls[0]).ClearTextWindow();


                                }
                                IceTabPage t = GetWindow(connection, data, IceTabPage.WindowType.Channel);
                                if (t != null)
                                    t.TextWindow.ClearTextWindow();
                                else
                                {
                                    IceTabPage q = GetWindow(connection, data, IceTabPage.WindowType.Query);
                                    if (q != null)
                                        q.TextWindow.ClearTextWindow();
                                }
                            }
                            break;
                        case "/ctcp":
                            if (connection != null && data.IndexOf(' ') > 0)
                            {
                                //ctcp nick ctcptype
                                string nick = data.Substring(0, data.IndexOf(' '));
                                //get the message
                                string ctcp = data.Substring(data.IndexOf(' ') + 1);

                                string msg = GetMessageFormat("Ctcp Send");
                                msg = msg.Replace("$nick", nick); ;
                                msg = msg.Replace("$ctcp", ctcp.ToUpper());
                                CurrentWindowMessage(connection, msg, 7, true);
                                SendData(connection, "PRIVMSG " + nick + " " + ctcp.ToUpper() + "");
                            }
                            break;
                        case "/describe":   //me command for a specific channel
                            if (connection != null && data.IndexOf(' ') > 0)
                            {
                                //get the channel name
                                string channel = data.Substring(0, data.IndexOf(' '));
                                //get the message
                                string message = data.Substring(data.IndexOf(' ') + 1);
                                //check for the channel
                                IceTabPage t = GetWindow(connection, channel, IceTabPage.WindowType.Channel);
                                if (t != null)
                                {
                                    SendData(connection, "PRIVMSG " + t.TabCaption + " :ACTION " + message + "");
                                    string msg = GetMessageFormat("Self Channel Action");
                                    msg = msg.Replace("$nick", inputPanel.CurrentConnection.ServerSetting.NickName).Replace("$channel", t.TabCaption);
                                    msg = msg.Replace("$message", message);
                                    t.TextWindow.AppendText(msg, 1);
                                    t.TextWindow.ScrollToBottom();
                                    t.LastMessageType = ServerMessageType.Action;
                                }
                            }
                            break;
                        case "/dns":
                            if (data.Length > 0)
                            {
                                if (data.IndexOf(".") > 0)
                                {
                                    //dns a host
                                    try
                                    {
                                        System.Net.IPAddress[] addresslist = System.Net.Dns.GetHostAddresses(data);
                                        ParseOutGoingCommand(connection, "/echo " + data + " resolved to " + addresslist.Length + " address(es)");
                                        foreach (System.Net.IPAddress address in addresslist)
                                            ParseOutGoingCommand(connection, "/echo -> " + address.ToString());
                                    }
                                    catch (Exception)
                                    {
                                        ParseOutGoingCommand(connection, "/echo " + data + " does not resolve (unknown address)");
                                    }
                                }
                                else
                                {
                                    //dns a nickname (send a userhost)
                                    SendData(connection, "USERHOST " + data);
                                }
                            }
                            break;
                        case "/echo":
                            //currently just echo to the current window
                            if (data.Length > 0)
                            {
                                if (CurrentWindowType == IceTabPage.WindowType.Channel || CurrentWindowType == IceTabPage.WindowType.Query)
                                {
                                    string msg = GetMessageFormat("User Echo");
                                    msg = msg.Replace("$message", data);

                                    CurrentWindow.TextWindow.AppendText(msg, 1);
                                    CurrentWindow.LastMessageType = ServerMessageType.Other;
                                }
                                else if (CurrentWindowType == IceTabPage.WindowType.Console)
                                {
                                    string msg = GetMessageFormat("User Echo");
                                    msg = msg.Replace("$message", data);

                                    mainTabControl.GetTabPage("Console").CurrentConsoleWindow().AppendText(msg, 1);
                                }
                            }
                            break;
                        case "/google":
                            if (data.Length > 0)
                                System.Diagnostics.Process.Start("http://www.google.com/search?q=" + data);
                            else
                                System.Diagnostics.Process.Start("http://www.google.com");
                            break;
                        case "/hop":
                            if (connection != null && data.Length == 0)
                            {
                                if (CurrentWindowType == IceTabPage.WindowType.Channel)
                                {
                                    temp = CurrentWindow.TabCaption;
                                    SendData(connection, "PART " + temp);
                                    SendData(connection, "JOIN " + temp);
                                }
                            }
                            else
                            {
                                IceTabPage t = GetWindow(connection, data, IceTabPage.WindowType.Channel);
                                if (t != null)
                                {
                                    SendData(connection, "PART " + t.TabCaption);
                                    SendData(connection, "JOIN " + t.TabCaption);
                                }
                            }
                            break;
                        case "/icechat":
                            if (connection != null)
                                ParseOutGoingCommand(connection, "/me is using " + Settings.Default.ProgramID + " " + Settings.Default.Version);
                            break;
                        case "/icepath":
                            //To get current Folder and paste it into /me
                            if (connection != null)
                                ParseOutGoingCommand(connection, "/me Build Path = " + Directory.GetCurrentDirectory());
                            break;
                        case "/join":
                            if (connection != null && data.Length > 0)
                                SendData(connection, "JOIN " + data);
                            break;
                        case "/kick":
                            if (connection != null && data.Length > 0)
                            {
                                //kick #channel nick reason
                                if (data.IndexOf(' ') > 0)
                                {
                                    //get the channel
                                    temp = data.Substring(0, data.IndexOf(' '));
                                    data = data.Substring(temp.Length + 1);

                                    if (data.IndexOf(' ') > 0)
                                    {
                                        //there is a kick reason
                                        string msg = data.Substring(data.IndexOf(' ') + 1);
                                        data = data.Substring(0, data.IndexOf(' '));
                                        SendData(connection, "KICK " + temp + " " + data + " :" + msg);
                                    }
                                    else
                                    {
                                        SendData(connection, "KICK " + temp + " " + data);
                                    }
                                }
                            }
                            break;
                        case "/me":
                            //check if in channel, query, etc
                            if (connection != null && data.Length > 0)
                            {
                                if (CurrentWindowType == IceTabPage.WindowType.Channel || CurrentWindowType == IceTabPage.WindowType.Query)
                                {
                                    SendData(connection, "PRIVMSG " + CurrentWindow.TabCaption + " :ACTION " + data + "");
                                    string msg = GetMessageFormat("Self Channel Action");
                                    msg = msg.Replace("$nick", inputPanel.CurrentConnection.ServerSetting.NickName).Replace("$channel", CurrentWindow.TabCaption);
                                    msg = msg.Replace("$message", data);

                                    CurrentWindow.TextWindow.AppendText(msg, 1);
                                    CurrentWindow.TextWindow.ScrollToBottom();
                                    CurrentWindow.LastMessageType = ServerMessageType.Action;
                                }
                            }
                            break;
                        case "/mode":
                            if (connection != null && data.Length > 0)
                                SendData(connection, "MODE " + data);
                            break;
                        case "/modex":
                            if (connection != null)
                                SendData(connection, "MODE " + connection.ServerSetting.NickName + " +x");
                            break;
                        case "/motd":
                            if (connection != null)
                            {
                                connection.ServerSetting.ForceMOTD = true;
                                SendData(connection, "MOTD");
                            }
                            break;
                        case "/msg":
                            if (connection != null && data.Length > 0 && data.IndexOf(' ') > -1)
                            {
                                string nick = data.Substring(0, data.IndexOf(' '));
                                string msg = data.Substring(data.IndexOf(' ') + 1);
                                SendData(connection, "PRIVMSG " + nick + " :" + msg);

                                //get the color for the private message
                                msg = GetMessageFormat("Self Private Message");

                                if (msg.StartsWith("&#x3;"))
                                {
                                    //get the color
                                    string color = msg.Substring(0, 6);
                                    int result;
                                    if (Int32.TryParse(msg.Substring(6, 1), out result))
                                        color += msg.Substring(6, 1);

                                    msg = color + "*" + nick + "* " + data.Substring(data.IndexOf(' ') + 1); ;
                                }

                                if (CurrentWindowType == IceTabPage.WindowType.Channel || CurrentWindowType == IceTabPage.WindowType.Query)
                                    CurrentWindow.TextWindow.AppendText(msg, 1);
                                else if (CurrentWindowType == IceTabPage.WindowType.Console)
                                {
                                    mainTabControl.GetTabPage("Console").CurrentConsoleWindow().AppendText(msg, 1);
                                }

                            }
                            break;
                        case "/nick":
                            if (connection != null && data.Length > 0)
                                SendData(connection, "NICK " + data);
                            break;
                        case "/notice":
                            if (connection != null && data.Length > 0 && data.IndexOf(' ') > -1)
                            {
                                string nick = data.Substring(0, data.IndexOf(' '));
                                string msg = data.Substring(data.IndexOf(' ') + 1);
                                SendData(connection, "NOTICE " + nick + " :" + msg);
                            }
                            break;
                        case "/part":
                            if (connection != null && data.Length > 0)
                            {
                                //check if it is a query window
                                IceTabPage q = GetWindow(connection, data, IceTabPage.WindowType.Query);
                                if (q != null)
                                {
                                    RemoveWindow(connection, q.TabCaption);
                                    return;
                                }
                                else if (CurrentWindowType == IceTabPage.WindowType.Query)
                                {
                                    RemoveWindow(connection, CurrentWindow.TabCaption);
                                    return;
                                }

                                //is there a part message
                                if (data.IndexOf(' ') > -1)
                                {
                                    //check if channel is a valid channel
                                    if (Array.IndexOf(connection.ServerSetting.ChannelTypes, data[0]) != -1)
                                    {
                                        SendData(connection, "PART " + data.Substring(0, data.IndexOf(' ')) + " :" + data.Substring(data.IndexOf(' ') + 1));
                                        RemoveWindow(connection, data.Substring(0, data.IndexOf(' ')));
                                    }
                                    else
                                    {
                                        //not a valid channel, use the current window
                                        if (CurrentWindowType == IceTabPage.WindowType.Channel)
                                        {
                                            SendData(connection, "PART " + CurrentWindow.TabCaption + " :" + data);
                                            RemoveWindow(connection, CurrentWindow.TabCaption);
                                        }
                                    }
                                }
                                else
                                {
                                    //see if data is a valid channel;
                                    if (Array.IndexOf(connection.ServerSetting.ChannelTypes, data[0]) != -1)
                                    {
                                        SendData(connection, "PART " + data);
                                        RemoveWindow(connection, data);
                                    }
                                    else
                                    {
                                        if (CurrentWindowType == IceTabPage.WindowType.Channel)
                                        {
                                            SendData(connection, "PART " + CurrentWindow.TabCaption + " :" + data);
                                            RemoveWindow(connection, CurrentWindow.TabCaption);
                                        }
                                    }
                                }
                            }
                            else if (connection != null)
                            {
                                //check if current window is channel
                                if (CurrentWindowType == IceTabPage.WindowType.Channel)
                                {
                                    SendData(connection, "PART " + CurrentWindow.TabCaption);
                                    RemoveWindow(connection, CurrentWindow.TabCaption);
                                }
                                else if (CurrentWindowType == IceTabPage.WindowType.Query)
                                {
                                    RemoveWindow(connection, CurrentWindow.TabCaption);
                                }
                            }
                            break;
                        case "/run":
                            if (data.Length > 0)
                                System.Diagnostics.Process.Start(data);
                            break;
                        case "/quit":
                            if (connection != null)
                            {
                                connection.AttemptReconnect = false;

                                if (data.Length > 0)
                                    SendData(connection, "QUIT :" + data);
                                else
                                    SendData(connection, "QUIT :" + ParseIdentifiers(connection, connection.ServerSetting.QuitMessage, ""));
                            }
                            break;
                        case "/quitall":
                            foreach (IRCConnection c in serverTree.ServerConnections.Values)
                            {
                                if (c.IsConnected)
                                {
                                    c.AttemptReconnect = false;

                                    if (data.Length > 0)
                                        SendData(c, "QUIT :" + data);
                                    else
                                        SendData(c, "QUIT :" + ParseIdentifiers(connection, c.ServerSetting.QuitMessage, ""));
                                }
                            }
                            break;
                        case "/say":
                            if (connection != null && data.Length > 0)
                            {
                                if (CurrentWindowType == IceTabPage.WindowType.Channel)
                                {
                                    SendData(connection, "PRIVMSG " + CurrentWindow.TabCaption + " :" + data);

                                    string msg = GetMessageFormat("Self Channel Message");
                                    string nick = inputPanel.CurrentConnection.ServerSetting.NickName;

                                    msg = msg.Replace("$nick", nick).Replace("$channel", CurrentWindow.TabCaption);

                                    //assign $color to the nickname 
                                    if (msg.Contains("$color"))
                                    {
                                        User u = CurrentWindow.GetNick(nick);
                                        for (int i = 0; i < u.Level.Length; i++)
                                        {
                                            if (u.Level[i])
                                            {
                                                if (connection.ServerSetting.StatusModes[0][i] == 'v')
                                                    msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelVoiceColor);
                                                else if (connection.ServerSetting.StatusModes[0][i] == 'h')
                                                    msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelHalfOpColor);
                                                else if (connection.ServerSetting.StatusModes[0][i] == 'o')
                                                    msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelOpColor);
                                                else if (connection.ServerSetting.StatusModes[0][i] == 'a')
                                                    msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelAdminColor);
                                                else if (connection.ServerSetting.StatusModes[0][i] == 'q')
                                                    msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelOwnerColor);
                                                else
                                                    msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelOwnerColor);

                                                break;
                                            }
                                        }
                                        if (msg.Contains("$color"))
                                            msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelRegularColor);

                                    }

                                    msg = msg.Replace("$status", CurrentWindow.GetNick(nick).ToString().Replace(nick, ""));
                                    msg = msg.Replace("$message", data);

                                    CurrentWindow.TextWindow.AppendText(msg, 1);
                                    CurrentWindow.LastMessageType = ServerMessageType.Message;
                                }
                                else if (CurrentWindowType == IceTabPage.WindowType.Query)
                                {
                                    SendData(connection, "PRIVMSG " + CurrentWindow.TabCaption + " :" + data);

                                    string msg = GetMessageFormat("Self Private Message");
                                    msg = msg.Replace("$nick", inputPanel.CurrentConnection.ServerSetting.NickName).Replace("$message", data);

                                    CurrentWindow.TextWindow.AppendText(msg, 1);
                                    CurrentWindow.LastMessageType = ServerMessageType.Message;
                                }
                            }
                            break;
                        case "/server":
                            if (data.Length > 0)
                            {
                                //check if default nick name has been set
                                if (iceChatOptions.DefaultNick.Length == 0)
                                {
                                    CurrentWindowMessage(connection, "No Default Nick Name Assigned. Go to IceChat Settings and set one under the Default Server Section.", 1, false);
                                }
                                else
                                {
                                    ServerSetting s = new ServerSetting();
                                    //get the server name
                                    //if there is a port name. extract it
                                    if (data.Contains(" "))
                                    {
                                        s.ServerName = data.Substring(0, data.IndexOf(' '));
                                        s.ServerPort = data.Substring(data.IndexOf(' ') + 1);
                                        if (s.ServerPort.IndexOf(' ') > 0)
                                        {
                                            s.ServerPort = s.ServerPort.Substring(0, s.ServerPort.IndexOf(' '));
                                        }
                                    }
                                    else if (data.Contains(":"))
                                    {
                                        s.ServerName = data.Substring(0, data.IndexOf(':'));
                                        s.ServerPort = data.Substring(data.IndexOf(':') + 1);
                                        if (s.ServerPort.IndexOf(' ') > 0)
                                        {
                                            s.ServerPort = s.ServerPort.Substring(0, s.ServerPort.IndexOf(' '));
                                        }
                                    }
                                    else
                                    {
                                        s.ServerName = data;
                                        s.ServerPort = "6667";
                                    }

                                    s.NickName = iceChatOptions.DefaultNick;
                                    s.AltNickName = iceChatOptions.DefaultNick + "_";
                                    s.AwayNickName = iceChatOptions.DefaultNick + "[A]";
                                    s.IAL = new Hashtable();

                                    Random r = new Random();
                                    s.ID = r.Next(10000, 99999);
                                    NewServerConnection(s);
                                }
                            }
                            break;
                        case "/timer":
                            if (connection != null)
                            {
                                if (data.Length != 0)
                                {
                                    string[] param = data.Split(' ');
                                    if (param.Length == 2)
                                    {
                                        //check for /timer ID off
                                        if (param[1].ToLower() == "off")
                                        {
                                            connection.DestroyTimer(param[0]);
                                            break;
                                        }
                                    }
                                    else if (param.Length > 3)
                                    {
                                        // param[0] = TimerID
                                        // param[1] = Repetitions
                                        // param[2] = Interval
                                        // param[3+] = Command

                                        string timerCommand = String.Join(" ", param, 3, param.GetUpperBound(0) - 2);
                                        connection.CreateTimer(param[0], Convert.ToDouble(param[1]), Convert.ToInt32(param[2]), timerCommand);
                                    }
                                    else
                                    {
                                        string msg = GetMessageFormat("User Error");
                                        msg = msg.Replace("$message", "/timer [ID] [INTERVAL] [REPS] [COMMAND]");
                                        CurrentWindowMessage(connection, msg, 4, true);
                                    }
                                }
                                else
                                {
                                    string msg = GetMessageFormat("User Error");
                                    msg = msg.Replace("$message", "/timer [ID] [INTERVAL] [REPS] [COMMAND]");
                                    CurrentWindowMessage(connection, msg, 4, true);
                                }
                            }
                            break;

                        case "/topic":
                            if (connection != null)
                            {
                                if (data.Length == 0)
                                {
                                    if (CurrentWindowType == IceTabPage.WindowType.Channel)
                                        SendData(connection, "TOPIC :" + CurrentWindow.TabCaption);
                                }
                                else
                                {
                                    //check if a channel name was passed                            
                                    string word = "";

                                    if (data.IndexOf(' ') > -1)
                                        word = data.Substring(0, data.IndexOf(' '));
                                    else
                                        word = data;

                                    if (Array.IndexOf(connection.ServerSetting.ChannelTypes, word[0]) != -1)
                                    {
                                        IceTabPage t = GetWindow(connection, word, IceTabPage.WindowType.Channel);
                                        if (t != null)
                                        {
                                            if (data.IndexOf(' ') > -1)
                                                SendData(connection, "TOPIC " + t.TabCaption + " :" + data.Substring(data.IndexOf(' ') + 1));
                                            else
                                                SendData(connection, "TOPIC :" + t.TabCaption);
                                        }
                                    }
                                    else
                                    {
                                        if (CurrentWindowType == IceTabPage.WindowType.Channel)
                                            SendData(connection, "TOPIC " + CurrentWindow.TabCaption + " :" + data);
                                    }
                                }
                            }
                            break;
                        case "/version":
                            if (connection != null && data.Length > 0)
                            {
                                string msg = GetMessageFormat("Ctcp Send");
                                msg = msg.Replace("$nick", data); ;
                                msg = msg.Replace("$ctcp", "VERSION");
                                CurrentWindowMessage(connection, msg, 7, true);
                                SendData(connection, "PRIVMSG " + data + " VERSION");
                            }
                            else
                                SendData(connection, "VERSION");
                            break;
                        case "/whois":
                            if (connection != null && data.Length > 0)
                                SendData(connection, "WHOIS " + data);
                            break;
                        case "/quote":
                        case "/raw":
                            if (connection != null && connection.IsConnected)
                                connection.SendData(data);
                            break;
                        default:
                            //parse the outgoing data
                            if (connection != null)
                                SendData(connection, command.Substring(1) + " " + data);
                            break;
                    }
                }
                else
                {
                    //sends a message to the channel
                    if (inputPanel.CurrentConnection != null)
                    {
                        if (inputPanel.CurrentConnection.IsConnected)
                        {
                            //check if the current window is a Channel/Query, etc
                            if (CurrentWindowType == IceTabPage.WindowType.Channel)
                            {
                                SendData(connection, "PRIVMSG " + CurrentWindow.TabCaption + " :" + data);
                                string msg = GetMessageFormat("Self Channel Message");
                                string nick = inputPanel.CurrentConnection.ServerSetting.NickName;
                                msg = msg.Replace("$nick", nick).Replace("$channel", CurrentWindow.TabCaption);

                                //assign $color to the nickname 
                                if (msg.Contains("$color"))
                                {
                                    User u = CurrentWindow.GetNick(nick);
                                    for (int i = 0; i < u.Level.Length; i++)
                                    {
                                        if (u.Level[i])
                                        {
                                            if (connection.ServerSetting.StatusModes[0][i] == 'v')
                                                msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelVoiceColor);
                                            else if (connection.ServerSetting.StatusModes[0][i] == 'h')
                                                msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelHalfOpColor);
                                            else if (connection.ServerSetting.StatusModes[0][i] == 'o')
                                                msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelOpColor);
                                            else if (connection.ServerSetting.StatusModes[0][i] == 'a')
                                                msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelAdminColor);
                                            else if (connection.ServerSetting.StatusModes[0][i] == 'q')
                                                msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelOwnerColor);
                                            else
                                                msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelOwnerColor);

                                            break;
                                        }
                                    }

                                    if (msg.Contains("$color"))
                                        msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelRegularColor);
                                }

                                msg = msg.Replace("$status", CurrentWindow.GetNick(nick).ToString().Replace(nick, ""));
                                msg = msg.Replace("$message", data);

                                CurrentWindow.TextWindow.AppendText(msg, 1);
                                CurrentWindow.LastMessageType = ServerMessageType.Message;
                            }
                            else if (CurrentWindowType == IceTabPage.WindowType.Query)
                            {
                                SendData(connection, "PRIVMSG " + CurrentWindow.TabCaption + " :" + data);

                                string msg = GetMessageFormat("Self Private Message");
                                msg = msg.Replace("$nick", inputPanel.CurrentConnection.ServerSetting.NickName).Replace("$message", data);

                                CurrentWindow.TextWindow.AppendText(msg, 1);
                                CurrentWindow.LastMessageType = ServerMessageType.Message;

                            }
                            else if (CurrentWindowType == IceTabPage.WindowType.Console)
                            {
                                WindowMessage(connection, "Console", data, 4, true);
                            }
                        }
                        else
                        {
                            WindowMessage(connection, "Console", "Error: Not Connected", 4, true);
                            WindowMessage(connection, "Console", data, 4, true);
                        }
                    }
                    else
                    {
                        WindowMessage(null, "Console", data, 4, true);
                    }
                }
            }
            catch (Exception e)
            {
                WriteErrorFile("ParseOutGoingCommand:" + e.Message + ":" + data, e.StackTrace);
            }
        }
        /// <summary>
        /// Input Panel Text Box had Entered Key Pressed or Send Button Pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void inputPanel_OnCommand(object sender, string data)
        {
            bool ishandled = false;
            PluginArgs args = new PluginArgs(inputPanel.CurrentConnection);
            args.Extra = data;
            
            foreach (IPluginIceChat ipc in loadedPlugins)
            {
                if (ipc.InputText(args) == true)
                    ishandled = true;
            }              
                        
            if (!ishandled)
                ParseOutGoingCommand(inputPanel.CurrentConnection, data);
        }

        #endregion

        /// <summary>
        /// Get the Host from the Full User Host, including Ident
        /// </summary>
        /// <param name="host">Full User Host (user!ident@host)</param>
        /// <returns></returns>
        private string HostFromFullHost(string host)
        {
            if (host.IndexOf("!") > -1)
                return host.Substring(host.LastIndexOf("!") + 1);
            else
                return host;
        }

        /// <summary>
        /// Return the Nick Name from the Full User Host
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        private string NickFromFullHost(string host)
        {
            if (host.StartsWith(":"))
                host = host.Substring(1);

            if (host.IndexOf("!") > 0)
                return host.Substring(0, host.LastIndexOf("!"));
            else
                return host;
        }
        
        private string ParseIdentifier(IRCConnection connection, string data)
        {
            //match all words starting with a $
            string identMatch = "\\$\\b[a-zA-Z_0-9.]+\\b";
            Regex ParseIdent = new Regex(identMatch);
            Match m = ParseIdent.Match(data);
            while (m.Success)
            {
                switch (m.Value)
                {
                    case "$me":
                        if (connection != null)
                            data = data.Replace(m.Value, connection.ServerSetting.NickName);
                        else
                            data = data.Replace(m.Value, "$null");
                        break;
                    case "$ident":
                        if (connection != null)
                            data = data.Replace(m.Value, connection.ServerSetting.IdentName);
                        else
                            data = data.Replace(m.Value, "$null");
                        break;
                    case "$fullname":
                        if (connection != null)
                            data = data.Replace(m.Value, connection.ServerSetting.FullName);
                        else
                            data = data.Replace(m.Value, "$null");
                        break;
                    case "$network":
                        if (connection != null)
                            data = data.Replace(m.Value, connection.ServerSetting.NetworkName);
                        else
                            data = data.Replace(m.Value, "$null");
                        break;
                    case "$port":
                        if (connection != null)
                            data = data.Replace(m.Value, connection.ServerSetting.ServerPort);
                        else
                            data = data.Replace(m.Value, "$null");
                        break;
                    case "$servermode":
                        break;
                    case "$server":
                        if (connection != null)
                        {
                            if (connection.ServerSetting.RealServerName.Length > 0)
                                data = data.Replace(m.Value, connection.ServerSetting.RealServerName);
                            else
                                data = data.Replace(m.Value, connection.ServerSetting.ServerName);
                        }
                        else
                            data = data.Replace(m.Value, "$null");
                        break;
                    case "$online":
                        if (connection != null)
                        {
                            //check the datediff
                            TimeSpan online = DateTime.Now.Subtract(connection.ServerSetting.ConnectedTime);
                            data = data.Replace(m.Value, GetDuration((int)online.TotalSeconds));                            
                        }
                        else
                            data = data.Replace(m.Value, "$null");
                        break;
                    case "$serverip":
                        if (connection != null)
                            data = data.Replace(m.Value, connection.ServerSetting.ServerIP);
                        else
                            data = data.Replace(m.Value, "$null");
                        break;

                    //identifiers that do not require a connection                                
                    case "$sp":
                        data = data.Replace(m.Value, Environment.OSVersion.ServicePack.ToString());
                        break;
                    case "$os":
                        data = data.Replace(m.Value, Environment.OSVersion.ToString());
                        break;
                    case "$icepath":
                    case "$icechatdir":
                        data = data.Replace(m.Value, Directory.GetCurrentDirectory());
                        break;
                    case "$aliasfile":
                        data = data.Replace(m.Value, aliasesFile);
                        break;
                    case "$serverfile":
                        data = data.Replace(m.Value, serversFile);
                        break;
                    case "$popupfile":
                        data = data.Replace(m.Value, popupsFile);
                        break;
                    case "$icechatver":
                        data = data.Replace(m.Value, Settings.Default.Version);
                        break;
                    case "$icechat":
                        data = data.Replace(m.Value, Settings.Default.ProgramID + " " + Settings.Default.Version);
                        break;
                    case "$logpath":
                        data = data.Replace(m.Value, logsFolder);
                        break;
                    case "$randquit":
                        Random rand = new Random();
                        int rq = rand.Next(0, QuitMessages.RandomQuitMessages.Length);
                        data = data.Replace(m.Value, QuitMessages.RandomQuitMessages[rq]);
                        break;
                    case "$randcolor":
                        Random randcolor = new Random();
                        int rc = randcolor.Next(0, 31);
                        data = data.Replace(m.Value, rc.ToString());
                        break;
                    case "$tickcount":
                        data = data.Replace(m.Value, System.Environment.TickCount.ToString());
                        break;
                    case "$uptime":
                        System.Diagnostics.PerformanceCounter pc = new System.Diagnostics.PerformanceCounter("System", "System Up Time");
                        pc.NextValue();
                        TimeSpan ts2 = TimeSpan.FromSeconds(pc.NextValue());
                        data = data.Replace(m.Value, GetDuration(ts2.Seconds));
                        break;
                    case "$uptime2":
                        int systemUpTime = System.Environment.TickCount / 1000;
                        TimeSpan ts = TimeSpan.FromSeconds(systemUpTime);
                        data = data.Replace(m.Value, GetDuration(ts.Seconds));
                        break;
                }
                m = m.NextMatch();
            }
            
            return data;
        }

        private string ParseIdentifierValue(string data, string dataPassed)
        {
            //split up the data into words
            string[] parsedData = data.Split(' ');

            //the data that was passed for parsing identifiers
            string[] passedData = dataPassed.Split(' ');

            //will hold the updates message/data after identifiers are parsed
            string[] changedData = data.Split(' ');
        
            int count = -1;

            //search for identifiers that are numbers
            //used for replacing values passed to the function
            foreach (string word in parsedData)
            {
                count++;

                if (word.StartsWith("//") && count == 0)
                    changedData[count] = word.Substring(1);

                //parse out identifiers (start with a $)
                if (word.StartsWith("$"))
                {
                     switch (word)
                     {

                         default:
                             //search for identifiers that are numbers
                             //used for replacing values passed to the function
                             int result;
                             int z = 1;

                             while (z < word.Length)
                             {
                                 if (Int32.TryParse(word.Substring(z, 1), out result))
                                     z++;
                                 else
                                     break;
                             }

                             //check for - after numbered identifier
                             if (z > 1)
                             {
                                 //get the numbered identifier value
                                 int identVal = Int32.Parse(word.Substring(1, z - 1));

                                 if (identVal <= passedData.Length)
                                 {
                                     //if (word.Substring(z, 1) == "-")
                                     //    changedData[count] = String.Join(" ", passedData, identVal-1, passedData.GetUpperBound(0));
                                     //else    
                                     changedData[count] = passedData[identVal - 1];
                                 }
                                 else
                                     changedData[count] = "";
                             }
                             break;
                     }
                 }
             }
            return String.Join(" ",changedData);
        } 
        /// <summary>
        /// Parse out $identifiers for outgoing commands
        /// </summary>
        /// <param name="connection">Which Connection it is for</param>
        /// <param name="data">The data to be parsed</param>
        /// <returns></returns>
        private string ParseIdentifiers(IRCConnection connection, string data, string dataPassed)
        {
            string[] changedData = null;
            
            try
            {

                //parse the initial identifiers
                data = ParseIdentifier(connection, data);

                //parse out the $1,$2.. identifiers
                data = ParseIdentifierValue(data, dataPassed);

                //split up the data into words
                string[] parsedData = data.Split(' ');

                //the data that was passed for parsing identifiers
                string[] passedData = dataPassed.Split(' ');

                //will hold the updates message/data after identifiers are parsed
                changedData = data.Split(' ');

                int count = -1;

                foreach (string word in parsedData)
                {
                    count++;

                    if (word.StartsWith("//") && count == 0)
                        changedData[count] = word.Substring(1);

                    //parse out identifiers (start with a $)
                    if (word.StartsWith("$"))
                    {
                        switch (word)
                        {

                            default:
                                int result;
                                if (connection != null)
                                {
                                    if (word.StartsWith("$ial(") && word.IndexOf(')') > word.IndexOf('('))
                                    {
                                        string nick = ReturnBrackValue(word);
                                        string prop = ReturnPropertyValue(word);

                                        InternalAddressList ial = (InternalAddressList)connection.ServerSetting.IAL[nick];
                                        if (ial != null)
                                        {
                                            if (prop.Length == 0)
                                                changedData[count] = ial.Nick;
                                            else
                                            {
                                                switch (prop)
                                                {
                                                    case "nick":
                                                        changedData[count] = ial.Nick;
                                                        break;
                                                    case "host":
                                                        changedData[count] = ial.Host;
                                                        break;
                                                }
                                            }
                                        }
                                        else
                                            changedData[count] = "$null";
                                    }

                                    if (word.StartsWith("$nick(") && word.IndexOf(')') > word.IndexOf('('))
                                    {
                                        //get the value between and after the brackets
                                        string values = ReturnBrackValue(word);
                                        if (values.Split(',').Length == 2)
                                        {
                                            string channel = values.Split(',')[0];
                                            string nickvalue = values.Split(',')[1];

                                            string prop = ReturnPropertyValue(word);

                                            // $nick(#,N)     
                                            //find then channel
                                            IceTabPage t = GetWindow(connection, channel, IceTabPage.WindowType.Channel);
                                            if (t != null)
                                            {
                                                User u = null;
                                                if (Int32.TryParse(nickvalue, out result))
                                                {
                                                    if (Convert.ToInt32(nickvalue) == 0)
                                                        changedData[count] = t.Nicks.Count.ToString();
                                                    else
                                                        u = t.GetNick(Convert.ToInt32(nickvalue));
                                                }
                                                else
                                                {
                                                    u = t.GetNick(nickvalue);
                                                }

                                                if (prop.Length == 0 && u != null)
                                                {
                                                    changedData[count] = u.NickName;
                                                }
                                                else if (u != null)
                                                {
                                                    //$nick(#channel,1).op , .voice, .halfop, .admin,.owner. 
                                                    //.mode, .host, .nick,.ident
                                                    InternalAddressList ial = (InternalAddressList)connection.ServerSetting.IAL[u.NickName];
                                                    switch (prop)
                                                    {
                                                        case "host":
                                                            if (ial != null && ial.Host != null && ial.Host.Length > 0)
                                                                changedData[count] = ial.Host.Substring(ial.Host.IndexOf('@') + 1);
                                                            break;
                                                        case "ident":
                                                            if (ial != null && ial.Host != null && ial.Host.Length > 0)
                                                                changedData[count] = ial.Host.Substring(0,ial.Host.IndexOf('@'));
                                                            break;
                                                        case "nick":
                                                            changedData[count] = u.NickName;
                                                            break;
                                                        case "mode":
                                                            changedData[count] = u.ToString().Replace(u.NickName, "");
                                                            break;
                                                        case "op":
                                                            for (int i = 0; i < u.Level.Length; i++)
                                                            {
                                                                if (connection.ServerSetting.StatusModes[0][i] == 'o')
                                                                {
                                                                    if (u.Level[i] == true)
                                                                        changedData[count] = "$true";
                                                                    else
                                                                        changedData[count] = "$false";
                                                                }
                                                            }
                                                            break;
                                                        case "voice":
                                                            for (int i = 0; i < u.Level.Length; i++)
                                                            {
                                                                if (connection.ServerSetting.StatusModes[0][i] == 'v')
                                                                {
                                                                    if (u.Level[i] == true)
                                                                        changedData[count] = "$true";
                                                                    else
                                                                        changedData[count] = "$false";
                                                                }
                                                            }
                                                            break;
                                                    }
                                                    ial = null;
                                                }
                                            }
                                        }
                                    }

                                    if (word.StartsWith("$chan(") && word.IndexOf(')') > word.IndexOf('('))
                                    {
                                        //get the value between and after the brackets
                                        string channel = ReturnBrackValue(word);
                                        string prop = ReturnPropertyValue(word);

                                        //find then channel
                                        IceTabPage t = GetWindow(connection, channel, IceTabPage.WindowType.Channel);
                                        if (t != null)
                                        {
                                            if (prop.Length == 0)
                                            {
                                                //replace with channel name
                                                changedData[count] = t.TabCaption;
                                            }
                                            else
                                            {
                                                switch (prop)
                                                {
                                                    case "mode":
                                                        changedData[count] = t.ChannelModes;
                                                        break;
                                                    case "count":
                                                        changedData[count] = t.Nicks.Count.ToString();
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                        }


                    }

                }
            }
            catch (Exception e)
            {
                WriteErrorFile("ParseIdentifiers Error:" + e.Message + ":" + data, e.StackTrace);
            }
            return String.Join(" ", changedData);
        }
        private string ReturnBrackValue(string data)
        {
            //return what is between ( ) brackets
            string d = data.Substring(data.IndexOf('(') + 1);
            return d.Substring(0,d.IndexOf(')'));
        }
        
        private string ReturnPropertyValue(string data)
        {
            if (data.IndexOf('.') == -1)
                return "";
            else
                return data.Substring(data.LastIndexOf('.') + 1);
        }

        private string GetDuration(int seconds)
        {
            TimeSpan t = new TimeSpan(0, 0, seconds);

            string s = t.Seconds.ToString() + " secs";
            if (t.Minutes > 0)
                s = t.Minutes.ToString() + " mins " + s;
            if (t.Hours > 0)
                s = t.Hours.ToString() + " hrs " + s;
            if (t.Days > 0)
                s = t.Days.ToString() + " days " + s;

            return s;
        }

        #region Menu and ToolStrip Items

        /// <summary>
        /// Close the Application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //show the about box
            FormAbout fa = new FormAbout();
            fa.ShowDialog(this);
        }

        private void minimizeToTrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Hide();
            notifyIcon.Visible = true;
        }

        private void NotifyIconMouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            notifyIcon.Visible = false;
        }

        private void iceChatSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //bring up a very basic settings form
            FormSettings fs = new FormSettings(iceChatOptions, iceChatFonts, iceChatEmoticons);
            fs.SaveOptions += new FormSettings.SaveOptionsDelegate(fs_SaveOptions);
            
            fs.ShowDialog(this);
        }

        private void fs_SaveOptions()
        {
            SaveOptions();           
            SaveFonts();
            //implement the new Font Settings
            
            //do all the Console Tabs Windows
            foreach (ConsoleTab c in mainTabControl.GetTabPage("Console").ConsoleTab.TabPages)
            {
                ((TextWindow)c.Controls[0]).Font = new Font(iceChatFonts.FontSettings[0].FontName, iceChatFonts.FontSettings[0].FontSize);
            }
            
            //do all the Channel and Query Tabs Windows
            foreach (IceTabPage t in mainTabControl.TabPages)
            {
                if (t.WindowStyle == IceTabPage.WindowType.Channel)
                    t.TextWindow.Font = new Font(iceChatFonts.FontSettings[1].FontName, iceChatFonts.FontSettings[1].FontSize);

                if (t.WindowStyle == IceTabPage.WindowType.Query)
                    t.TextWindow.Font = new Font(iceChatFonts.FontSettings[2].FontName, iceChatFonts.FontSettings[2].FontSize);
            }
            
            //change the server list
            serverTree.Font = new Font(iceChatFonts.FontSettings[4].FontName, iceChatFonts.FontSettings[4].FontSize);

            //change the nick list
            nickList.Font = new Font(iceChatFonts.FontSettings[3].FontName, iceChatFonts.FontSettings[3].FontSize);

            //change the inputbox font
            inputPanel.InputBoxFont = new Font(iceChatFonts.FontSettings[5].FontName, iceChatFonts.FontSettings[5].FontSize);

            //set if Emoticon Picker/Color Picker is Visible
            inputPanel.ShowEmoticonPicker = iceChatOptions.ShowEmoticonPicker;
            inputPanel.ShowColorPicker = iceChatOptions.ShowColorPicker;

            //set if Status Bar is Visible
            statusStripMain.Visible = iceChatOptions.ShowStatusBar;

        }

        private void serverListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelLeft.Visible = serverListToolStripMenuItem.Checked;
        }

        private void nickListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelRight.Visible = nickListToolStripMenuItem.Checked;
        }

        private void statusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStripMain.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void toolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripMain.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void codePlexPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://icechat.codeplex.com/");
        }

        private void forumsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.icechat.net/forums");
        }

        private void iceChatHomePageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.icechat.net/");
        }

        private void iceChatColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //bring up a very basic settings form
            FormColors fc = new FormColors(iceChatMessages, iceChatColors);
            fc.SaveColors += new FormColors.SaveColorsDelegate(fc_SaveColors);
            
            fc.ShowDialog(this);

            foreach (IPluginIceChat ipc in loadedPlugins)
            {
                //
                //ipc.LoadOptionsForm(fc.Ta
            }

        }

        private void fc_SaveColors()
        {
            SaveMessageFormat();            
            SaveColors();

            toolStripMain.BackColor = IrcColor.colors[iceChatColors.ToolbarBackColor];
            menuMainStrip.BackColor = IrcColor.colors[iceChatColors.MenubarBackColor];
            statusStripMain.BackColor = IrcColor.colors[iceChatColors.StatusbarBackColor];
            toolStripStatus.ForeColor = IrcColor.colors[iceChatColors.StatusbarForeColor];
            inputPanel.SetInputBoxColors();
            channelList.SetListColors();
            nickList.Invalidate();
            serverTree.Invalidate();
            mainTabControl.Invalidate();

            //update all the console windows
            foreach (ConsoleTab c in mainTabControl.GetTabPage("Console").ConsoleTab.TabPages)
            {
                ((TextWindow)c.Controls[0]).IRCBackColor = iceChatColors.ConsoleBackColor;
            }

            //update all the Channel and Query Tabs Windows
            foreach (IceTabPage t in mainTabControl.TabPages)
            {
                if (t.WindowStyle == IceTabPage.WindowType.Channel)
                    t.TextWindow.IRCBackColor = iceChatColors.ChannelBackColor;

                if (t.WindowStyle == IceTabPage.WindowType.Query)
                    t.TextWindow.IRCBackColor = iceChatColors.QueryBackColor;
            }


        }

        private void toolStripSettings_Click(object sender, EventArgs e)
        {
            iceChatSettingsToolStripMenuItem.PerformClick();
        }

        private void toolStripColors_Click(object sender, EventArgs e)
        {
            iceChatColorsToolStripMenuItem.PerformClick();
        }

        private void toolStripQuickConnect_Click(object sender, EventArgs e)
        {
            //popup a small dialog asking for basic server settings
            QuickConnect qc = new QuickConnect();
            qc.QuickConnectServer += new QuickConnect.QuickConnectServerDelegate(OnQuickConnectServer);
            qc.ShowDialog(this);
        }

        private void toolStripAway_Click(object sender, EventArgs e)
        {
            //check if away or not
            if (FormMain.Instance.InputPanel.CurrentConnection != null)
            {
                if (inputPanel.CurrentConnection.ServerSetting.Away)
                {
                    ParseOutGoingCommand(inputPanel.CurrentConnection, "/away");
                }
                else
                {
                    //ask for an away reason
                    InputBoxDialog i = new InputBoxDialog();
                    i.FormCaption = "Enter your away Reason";
                    i.FormPrompt = "Away Reason";

                    i.ShowDialog();
                    if (i.InputResponse.Length > 0)
                        ParseOutGoingCommand(inputPanel.CurrentConnection, "/away " + i.InputResponse);
                    
                    i.Dispose();
                }
            }
        }

        private void OnQuickConnectServer(ServerSetting s)
        {
            NewServerConnection(s);
        }

        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripMain.Visible = false;
        }

        private void iceChatEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormEditor fe = new FormEditor();
            fe.ShowDialog(this);
        }

        private void toolStripSystemTray_Click(object sender, EventArgs e)
        {
            minimizeToTrayToolStripMenuItem.PerformClick();
        }

        private void toolStripEditor_Click(object sender, EventArgs e)
        {
            iceChatEditorToolStripMenuItem.PerformClick();
        }

        private void debugWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //add the debug window, if it does not exist
            if (GetWindow(null, "Debug", IceTabPage.WindowType.Debug) == null)
                AddWindow(null, "Debug", IceTabPage.WindowType.Debug);
        }

        private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.notifyIcon.Visible = false;
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.exitToolStripMenuItem.PerformClick();
        }

        private void OnPluginMenuItemClick(object sender, EventArgs e)
        {
            //show plugin information
            FormPluginInfo pi = new FormPluginInfo((IPluginIceChat)((ToolStripMenuItem)sender).Tag, (ToolStripMenuItem)sender);
            pi.ShowDialog(this);
        }
        
        #endregion
        
        private void LoadPlugins()
        {
            string[] pluginFiles = Directory.GetFiles(currentFolder, "*.DLL");

            for (int i = 0; i < pluginFiles.Length; i++)
            {
                //System.Diagnostics.Debug.WriteLine("checking:" + pluginFiles[i]);

                string args = pluginFiles[i].Substring(pluginFiles[i].LastIndexOf("\\") + 1);
                args = args.Substring(0, args.Length - 4);
                
                //System.Diagnostics.Debug.WriteLine(args);
                if (args.ToUpper().Substring(0,7) != "IPLUGIN")
                {
                    Type ObjType = null;
                    try
                    {
                        // load it
                        Assembly ass = null;
                        ass = Assembly.LoadFile(pluginFiles[i]);
                        //System.Diagnostics.Debug.WriteLine(ass.ToString());
                        if (ass != null)
                        {
                            ObjType = ass.GetType("IceChatPlugin.Plugin");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("assembly is null");
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteErrorFile("LoadPlugins Cast:" + ex.Message, ex.StackTrace);
                    }
                    try
                    {
                        // OK Lets create the object as we have the Report Type
                        if (ObjType != null)
                        {
                            //System.Diagnostics.Debug.WriteLine("create instance of " + args);

                            IPluginIceChat ipi = (IPluginIceChat)Activator.CreateInstance(ObjType);
                            
                            ipi.MainForm = this;
                            ipi.MainMenuStrip = this.MainMenuStrip;

                            WindowMessage(null, "Console", "Loaded Plugin - " + ipi.Name + " v" + ipi.Version + " by " + ipi.Author, 4, true);
                            
                            //add the menu items
                            ToolStripMenuItem t = new ToolStripMenuItem(ipi.Name);
                            t.Tag = ipi;
                            t.Click += new EventHandler(OnPluginMenuItemClick);
                            pluginsToolStripMenuItem.DropDownItems.Add(t);

                            //declare all the events                            
                            ipi.OnChannelMessage += new ChannelMessageHandler(Plugin_OnChangedMessage);
                            ipi.OnChannelAction += new ChannelActionHandler(Plugin_OnChangedMessage);
                            ipi.OnQueryMessage += new QueryMessageHandler(Plugin_OnChangedMessage);
                            ipi.OnQueryAction += new QueryActionHandler(Plugin_OnChangedMessage);

                            ipi.OnChannelJoin += new ChannelJoinHandler(Plugin_OnChangedMessage);
                            ipi.OnChannelPart += new ChannelPartHandler(Plugin_OnChangedMessage);
                            ipi.OnServerQuit += new ServerQuitHandler(Plugin_OnChangedMessage);

                            ipi.OnInputText += new InputTextHandler(Plugin_OnInputText);

                            loadedPlugins.Add(ipi);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("obj type is null:" + args);
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteErrorFile("LoadPlugins Assign:" + ex.Message, ex.StackTrace);
                    }
                }
            }
        }

        internal void UnloadPlugin(IPluginIceChat plugin, ToolStripMenuItem menuItem)
        {
            //unload specified plugin
            loadedPlugins.Remove(plugin);
            pluginsToolStripMenuItem.DropDownItems.Remove(menuItem);
            WindowMessage(null, "Console", "Unloaded Plugin - " + plugin.Name, 4, true);

        }

        private void Plugin_OnInputText(object sender, PluginArgs e)
        {
            ParseOutGoingCommand((IRCConnection)e.Connection, e.Extra);
        }

        //handles onChannelMessage, OnChannelAction, OnQueryMessage, OnQueryAction
        //OnChannelJoin, OnChannelPart, OnServerQuit
        private void Plugin_OnChangedMessage(object sender, PluginArgs e)
        {
            ((TextWindow)e.TextWindow).AppendText(e.Message, 1);
            if (e.Command != null)
            {
                if (e.Connection != null)
                    ParseOutGoingCommand((IRCConnection)e.Connection, e.Command);
            }
        }

        internal void WriteErrorFile(string message, string stackTrace)
        {
            System.Diagnostics.Debug.WriteLine(message + ":" + stackTrace);
            if (errorFile != null)
            {
                errorFile.WriteLine(DateTime.Now.ToString("G") + ":" + message + ":" + stackTrace);
                errorFile.Flush();
            }
        }

    }
}