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

        private IceChatOptions iceChatOptions;
        private IceChatColors iceChatColors;
        private IceChatMessageFormat iceChatMessages;
        private IceChatFontSetting iceChatFonts;
        private IceChatAliases iceChatAliases;
        private IceChatPopupMenus iceChatPopups;
        private IceChatEmoticon iceChatEmoticons;

        private ArrayList loadedPlugins;

        private IdentServer identServer;

        private delegate void AddWindowDelegate(IRCConnection connection, string windowName, TabWindow.WindowType type);
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
       
            //load all files from the Documents folder, unless it exist in the current folder
            if (File.Exists(currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatServer.xml"))
            {
                serversFile = currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatServer.xml";
                logsFolder = currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatLogs";
            }
            else
            {
                serversFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + System.IO.Path.DirectorySeparatorChar + "IceChatServer.xml";
                logsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + System.IO.Path.DirectorySeparatorChar + "IceChatLogs";
            }
            
            if (File.Exists(currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatOptions.xml"))
                optionsFile = currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatOptions.xml";
            else
                optionsFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + System.IO.Path.DirectorySeparatorChar + "IceChatOptions.xml";

            if (File.Exists(currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatMessages.xml"))
                messagesFile = currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatMessages.xml";
            else
                messagesFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + System.IO.Path.DirectorySeparatorChar + "IceChatMessages.xml";

            if (File.Exists(currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatFonts.xml"))
                fontsFile = currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatFonts.xml";
            else
                fontsFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + System.IO.Path.DirectorySeparatorChar + "IceChatFonts.xml";

            if (File.Exists(currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatColors.xml"))
                colorsFile = currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatColors.xml";
            else
                colorsFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + System.IO.Path.DirectorySeparatorChar + "IceChatColors.xml";

            if (File.Exists(currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatChannels.xml"))
                favoriteChannelsFile = currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatChannels.xml";
            else
                favoriteChannelsFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + System.IO.Path.DirectorySeparatorChar + "IceChatChannels.xml";

            if (File.Exists(currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatAliases.xml"))
                aliasesFile = currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatAliases.xml";
            else
                aliasesFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + System.IO.Path.DirectorySeparatorChar + "IceChatAliases.xml";

            if (File.Exists(currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatPopups.xml"))
                popupsFile = currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatPopups.xml";
            else
                popupsFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + System.IO.Path.DirectorySeparatorChar + "IceChatPopups.xml";

            if (File.Exists(currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatHighLites.xml"))
                highlitesFile = currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatHighLites.xml";
            else
                highlitesFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + System.IO.Path.DirectorySeparatorChar + "IceChatHighLites.xml";

            if (File.Exists(currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatEmoticons" + System.IO.Path.DirectorySeparatorChar + "IceChatEmoticons.xml"))
                emoticonsFile = currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatEmoticons" + System.IO.Path.DirectorySeparatorChar + "IceChatEmoticons.xml";
            else
                emoticonsFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + System.IO.Path.DirectorySeparatorChar + "IceChatEmoticons" + System.IO.Path.DirectorySeparatorChar + "IceChatEmoticons.xml";

            #endregion

            LoadOptions();
            LoadColors();

            InitializeComponent();

            serverTree = new ServerTree();
            
            panelLeft.Controls.Add(serverTree);

            this.Text = IceChat.Properties.Settings.Default.ProgramID + " " + IceChat.Properties.Settings.Default.Version + " - January 23 2010";

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

            this.tabMain.AllowDrop = true;
            this.tabMain.ImageList = this.tabPageImages;
            this.tabMain.Multiline = true;

            tabMain.SelectedIndexChanged += new EventHandler(TabMainSelectedIndexChanged);
            //tabMain.MouseClick += new MouseEventHandler(TabMainMouseClick);
            tabMain.CloseTab += new IceTabControl.CloseTabDelegate(TabMainCloseTab);
            tabMain.Font = new Font("Verdana", 10);
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

        #region Settings Loading and Saving

        private void LoadDefaultMessageSettings()
        {
            IceChatMessageFormat oldMessage = new IceChatMessageFormat();
            oldMessage.MessageSettings = new ServerMessageFormatItem[44];

            if (iceChatMessages.MessageSettings != null)
                iceChatMessages.MessageSettings.CopyTo(oldMessage.MessageSettings, 0);
            
            iceChatMessages.MessageSettings = new ServerMessageFormatItem[44];

            if (oldMessage.MessageSettings[0] == null || oldMessage.MessageSettings[0].FormattedMessage.Length == 0)
            {
                iceChatMessages.MessageSettings[0] = NewMessageFormat("Server Connect", "*** Attempting to connect to $server ($serverip) on port $port");
            }
            else
                iceChatMessages.MessageSettings[0] = oldMessage.MessageSettings[0];

            if (oldMessage.MessageSettings[1] == null || oldMessage.MessageSettings[1].FormattedMessage.Length == 0)
                iceChatMessages.MessageSettings[1] = NewMessageFormat("Server Disconnect", "*** Server disconnected on $server");
            else
                iceChatMessages.MessageSettings[1] = oldMessage.MessageSettings[1];

            if (oldMessage.MessageSettings[2] == null || oldMessage.MessageSettings[2].FormattedMessage.Length == 0)
                iceChatMessages.MessageSettings[2] = NewMessageFormat("Server Reconnect", "*** Attempting to re-connect to $server");
            else
                iceChatMessages.MessageSettings[2] = oldMessage.MessageSettings[2];

            if (oldMessage.MessageSettings[3] == null || oldMessage.MessageSettings[3].FormattedMessage.Length == 0)            
                iceChatMessages.MessageSettings[3] = NewMessageFormat("Channel Invite", "* $nick invites you to $channel");
            else
                iceChatMessages.MessageSettings[3] = oldMessage.MessageSettings[3];

            if (oldMessage.MessageSettings[7] == null || oldMessage.MessageSettings[7].FormattedMessage.Length == 0)            
                iceChatMessages.MessageSettings[7] = NewMessageFormat("Channel Mode", "&#x3;9* $nick sets mode $mode $modeparam for $channel");
            else
                iceChatMessages.MessageSettings[7] = oldMessage.MessageSettings[7];

            if (oldMessage.MessageSettings[8] == null || oldMessage.MessageSettings[8].FormattedMessage.Length == 0)
                iceChatMessages.MessageSettings[8] = NewMessageFormat("Server Mode", "&#x3;6* Your mode is now $mode");
            else
                iceChatMessages.MessageSettings[8] = oldMessage.MessageSettings[8];

            if (oldMessage.MessageSettings[9] == null || oldMessage.MessageSettings[9].FormattedMessage.Length == 0)
                iceChatMessages.MessageSettings[9] = NewMessageFormat("Server Notice", "&#x3;4*** $server $message");
            else
                iceChatMessages.MessageSettings[9] = oldMessage.MessageSettings[9];

            if (oldMessage.MessageSettings[10] == null || oldMessage.MessageSettings[10].FormattedMessage.Length == 0)
                iceChatMessages.MessageSettings[10] = NewMessageFormat("Server Message", "&#x3;4-$server- $message");
            else
                iceChatMessages.MessageSettings[10] = oldMessage.MessageSettings[10];

            if (oldMessage.MessageSettings[11] == null || oldMessage.MessageSettings[11].FormattedMessage.Length == 0)
                iceChatMessages.MessageSettings[11] = NewMessageFormat("User Notice", "&#x3;4--$nick-- $message");
            else
                iceChatMessages.MessageSettings[11] = oldMessage.MessageSettings[11];

            if (oldMessage.MessageSettings[12] == null || oldMessage.MessageSettings[12].FormattedMessage.Length == 0)
                iceChatMessages.MessageSettings[12] = NewMessageFormat("Channel Message", "&#x3;1<$color$nick&#x3;> $message");
            else
                iceChatMessages.MessageSettings[12] = oldMessage.MessageSettings[12];

            if (oldMessage.MessageSettings[13] == null || oldMessage.MessageSettings[13].FormattedMessage.Length == 0)
                iceChatMessages.MessageSettings[13] = NewMessageFormat("Self Channel Message", "&#x3;1<$nick> $message");
            else
                iceChatMessages.MessageSettings[13] = oldMessage.MessageSettings[13];

            if (oldMessage.MessageSettings[14] == null || oldMessage.MessageSettings[14].FormattedMessage.Length == 0)            
                iceChatMessages.MessageSettings[14] = NewMessageFormat("Channel Action", "&#x3;5* $nick $message");
            else
                iceChatMessages.MessageSettings[14] = oldMessage.MessageSettings[14];

            if (oldMessage.MessageSettings[15] == null || oldMessage.MessageSettings[15].FormattedMessage.Length == 0)            
                iceChatMessages.MessageSettings[15] = NewMessageFormat("Self Channel Action", "&#x3;4* $nick $message");
            else
                iceChatMessages.MessageSettings[15] = oldMessage.MessageSettings[15];

            if (oldMessage.MessageSettings[16] == null || oldMessage.MessageSettings[16].FormattedMessage.Length == 0)            
                iceChatMessages.MessageSettings[16] = NewMessageFormat("Channel Join", "&#x3;7* $nick ($host) has joined channel $channel");
            else
                iceChatMessages.MessageSettings[16] = oldMessage.MessageSettings[16];

            if (oldMessage.MessageSettings[17] == null || oldMessage.MessageSettings[17].FormattedMessage.Length == 0)            
                iceChatMessages.MessageSettings[17] = NewMessageFormat("Self Channel Join", "&#x3;4* You have joined $channel");
            else
                iceChatMessages.MessageSettings[17] = oldMessage.MessageSettings[17];

            if (oldMessage.MessageSettings[18] == null || oldMessage.MessageSettings[18].FormattedMessage.Length == 0)
                iceChatMessages.MessageSettings[18] = NewMessageFormat("Channel Part", "&#x3;3* $nick ($host) has left $channel ($reason)");
            else
                iceChatMessages.MessageSettings[18] = oldMessage.MessageSettings[18];

            if (oldMessage.MessageSettings[19] == null || oldMessage.MessageSettings[19].FormattedMessage.Length == 0)            
                iceChatMessages.MessageSettings[19] = NewMessageFormat("Self Channel Part", "&#x3;4* You have left $channel - You will be missed &#x3;10($reason)");
            else
                iceChatMessages.MessageSettings[19] = oldMessage.MessageSettings[19];

            if (oldMessage.MessageSettings[20] == null || oldMessage.MessageSettings[20].FormattedMessage.Length == 0)            
                iceChatMessages.MessageSettings[20] = NewMessageFormat("Server Quit", "&#x3;2* $nick ($host) Quit ($reason)");
            else
                iceChatMessages.MessageSettings[20] = oldMessage.MessageSettings[20];

            if (oldMessage.MessageSettings[21] == null || oldMessage.MessageSettings[21].FormattedMessage.Length == 0)            
                iceChatMessages.MessageSettings[21] = NewMessageFormat("Channel Nick Change", "&#x3;3* $nick is now known as $newnick");
            else
                iceChatMessages.MessageSettings[21] = oldMessage.MessageSettings[21];

            if (oldMessage.MessageSettings[22] == null || oldMessage.MessageSettings[22].FormattedMessage.Length == 0)            
                iceChatMessages.MessageSettings[22] = NewMessageFormat("Self Nick Change", "&#x3;4* You are now known as $newnick");
            else
                iceChatMessages.MessageSettings[22] = oldMessage.MessageSettings[22];

            if (oldMessage.MessageSettings[23] == null || oldMessage.MessageSettings[23].FormattedMessage.Length == 0)
                iceChatMessages.MessageSettings[23] = NewMessageFormat("Channel Kick", "&#x3;4* $kickee was kicked by $nick($host) &#x3;3 - Reason ($reason)");
            else
                iceChatMessages.MessageSettings[23] = oldMessage.MessageSettings[23];

            if (oldMessage.MessageSettings[24] == null || oldMessage.MessageSettings[24].FormattedMessage.Length == 0)                        
                iceChatMessages.MessageSettings[24] = NewMessageFormat("Self Channel Kick", "&#x3;4* You were kicked from $channel by $kicker (&#x3;3$reason)");
            else
                iceChatMessages.MessageSettings[24] = oldMessage.MessageSettings[24];

            if (oldMessage.MessageSettings[25] == null || oldMessage.MessageSettings[25].FormattedMessage.Length == 0)
                iceChatMessages.MessageSettings[25] = NewMessageFormat("Private Message", "&#x3;1<$nick> $message");
            else
                iceChatMessages.MessageSettings[25] = oldMessage.MessageSettings[25];

            if (oldMessage.MessageSettings[26] == null || oldMessage.MessageSettings[26].FormattedMessage.Length == 0)            
                iceChatMessages.MessageSettings[26] = NewMessageFormat("Self Private Message", "&#x3;4<$nick>&#x3;1 $message");
            else
                iceChatMessages.MessageSettings[26] = oldMessage.MessageSettings[26];

            if (oldMessage.MessageSettings[27] == null || oldMessage.MessageSettings[27].FormattedMessage.Length == 0)            
                iceChatMessages.MessageSettings[27] = NewMessageFormat("Private Action", "&#x3;13* $nick $message");
            else
                iceChatMessages.MessageSettings[27] = oldMessage.MessageSettings[27];

            if (oldMessage.MessageSettings[28] == null || oldMessage.MessageSettings[28].FormattedMessage.Length == 0)            
                iceChatMessages.MessageSettings[28] = NewMessageFormat("Self Private Action", "&#x3;12* $nick $message");
            else
                iceChatMessages.MessageSettings[28] = oldMessage.MessageSettings[28];

            if (oldMessage.MessageSettings[35] == null || oldMessage.MessageSettings[35].FormattedMessage.Length == 0)
                iceChatMessages.MessageSettings[35] = NewMessageFormat("Channel Topic Change", "&#x3;3* Topic is: $topic");
            else
                iceChatMessages.MessageSettings[35] = oldMessage.MessageSettings[35];

            if (oldMessage.MessageSettings[36] == null || oldMessage.MessageSettings[36].FormattedMessage.Length == 0)
                iceChatMessages.MessageSettings[36] = NewMessageFormat("Channel Topic Text", "&#x3;3Topic: $topic");
            else
                iceChatMessages.MessageSettings[36] = oldMessage.MessageSettings[36];

            if (oldMessage.MessageSettings[37] == null || oldMessage.MessageSettings[37].FormattedMessage.Length == 0)
                iceChatMessages.MessageSettings[37] = NewMessageFormat("Server MOTD", "&#x3;3$message");
            else
                iceChatMessages.MessageSettings[37] = oldMessage.MessageSettings[37];

            if (oldMessage.MessageSettings[38] == null || oldMessage.MessageSettings[38].FormattedMessage.Length == 0)            
                iceChatMessages.MessageSettings[38] = NewMessageFormat("Channel Notice", "&#x3;5-$nick:$status$channel- $message");
            else
                iceChatMessages.MessageSettings[38] = oldMessage.MessageSettings[38];

            if (oldMessage.MessageSettings[39] == null || oldMessage.MessageSettings[39].FormattedMessage.Length == 0)
                iceChatMessages.MessageSettings[39] = NewMessageFormat("Channel Other", "&#x3;1$message");
            else
                iceChatMessages.MessageSettings[39] = oldMessage.MessageSettings[39];

            if (oldMessage.MessageSettings[40] == null || oldMessage.MessageSettings[40].FormattedMessage.Length == 0)
                iceChatMessages.MessageSettings[40] = NewMessageFormat("User Echo", "&#x3;7$message");
            else
                iceChatMessages.MessageSettings[40] = oldMessage.MessageSettings[40];

            if (oldMessage.MessageSettings[41] == null || oldMessage.MessageSettings[41].FormattedMessage.Length == 0)
                iceChatMessages.MessageSettings[41] = NewMessageFormat("Server Error", "&#x3;4ERROR: $message");
            else
                iceChatMessages.MessageSettings[41] = oldMessage.MessageSettings[41];

            if (oldMessage.MessageSettings[42] == null || oldMessage.MessageSettings[42].FormattedMessage.Length == 0)
                iceChatMessages.MessageSettings[42] = NewMessageFormat("User Whois", "&#x3;12->> $nick $data");
            else
                iceChatMessages.MessageSettings[42] = oldMessage.MessageSettings[42];

            if (oldMessage.MessageSettings[43] == null || oldMessage.MessageSettings[43].FormattedMessage.Length == 0)
                iceChatMessages.MessageSettings[43] = NewMessageFormat("User Error", "&#x3;4ERROR: $message");
            else
                iceChatMessages.MessageSettings[43] = oldMessage.MessageSettings[43];

            //still do customize these messages
            iceChatMessages.MessageSettings[4] = NewMessageFormat("Ctcp Reply", "&#x3;12[$nick $ctcp Reply] : $reply");
            iceChatMessages.MessageSettings[5] = NewMessageFormat("Ctcp Send", "&#x3;10--> [$nick] $ctcp");
            iceChatMessages.MessageSettings[6] = NewMessageFormat("Ctcp Request", "&#x3;7[$nick] $ctcp");
            iceChatMessages.MessageSettings[29] = NewMessageFormat("DCC Chat Action", "&#x3;5* $nick $message");
            iceChatMessages.MessageSettings[30] = NewMessageFormat("Self DCC Chat Action", "&#x3;5* $nick $message");
            iceChatMessages.MessageSettings[31] = NewMessageFormat("DCC Chat Message", "&#x3;1<$nick> $message");
            iceChatMessages.MessageSettings[32] = NewMessageFormat("Self DCC Chat Message", "&#x3;4<$nick> $message");
            iceChatMessages.MessageSettings[33] = NewMessageFormat("DCC Chat Request", "&#x3;4* $nick ($host) is requesting a DCC Chat");
            iceChatMessages.MessageSettings[34] = NewMessageFormat("DCC File Send", "&#x3;4* $nick ($host) is trying to send you a file ($file) [$filesize bytes]");

            SaveMessageFormat();

        }

        private void LoadDefaultFontSettings()
        {
            IceChatFontSetting oldFonts = new IceChatFontSetting(); ;
            oldFonts.FontSettings = new FontSettingItem[7];

            if (iceChatFonts.FontSettings != null)
                iceChatFonts.FontSettings.CopyTo(oldFonts.FontSettings, 0);

            iceChatFonts.FontSettings = new FontSettingItem[7];

            if (oldFonts.FontSettings[0] == null || iceChatFonts.FontSettings[0].FontName.Length == 0)
                iceChatFonts.FontSettings[0] = NewFontSetting("Console", "Verdana", 10);
            else
                iceChatFonts.FontSettings[0] = oldFonts.FontSettings[0];

            if (oldFonts.FontSettings[1] == null || iceChatFonts.FontSettings[1].FontName.Length == 0)
                iceChatFonts.FontSettings[1] = NewFontSetting("Channel", "Verdana", 10);
            else
                iceChatFonts.FontSettings[1] = oldFonts.FontSettings[1];

            if (oldFonts.FontSettings[2] == null || iceChatFonts.FontSettings[2].FontName.Length == 0)
                iceChatFonts.FontSettings[2] = NewFontSetting("Query", "Verdana", 10);
            else
                iceChatFonts.FontSettings[2] = oldFonts.FontSettings[2];

            if (oldFonts.FontSettings[3] == null || iceChatFonts.FontSettings[3].FontName.Length == 0)
                iceChatFonts.FontSettings[3] = NewFontSetting("Nicklist", "Verdana", 10);
            else
                iceChatFonts.FontSettings[3] = oldFonts.FontSettings[3];

            if (oldFonts.FontSettings[4] == null || iceChatFonts.FontSettings[4].FontName.Length == 0)
                iceChatFonts.FontSettings[4] = NewFontSetting("Serverlist", "Verdana", 10);
            else
                iceChatFonts.FontSettings[4] = oldFonts.FontSettings[4];

            if (oldFonts.FontSettings[5] == null || iceChatFonts.FontSettings[5].FontName.Length == 0)
                iceChatFonts.FontSettings[5] = NewFontSetting("InputBox", "Verdana", 10);
            else
                iceChatFonts.FontSettings[5] = oldFonts.FontSettings[5];

            if (oldFonts.FontSettings[6] == null || iceChatFonts.FontSettings[6].FontName.Length == 0)
                iceChatFonts.FontSettings[6] = NewFontSetting("ChannelBar", "Verdana", 10);
            else
                iceChatFonts.FontSettings[6] = oldFonts.FontSettings[6];


            oldFonts = null;
            
            SaveFonts();
        }

        private ServerMessageFormatItem NewMessageFormat(string messageName, string message)
        {
            ServerMessageFormatItem m = new ServerMessageFormatItem();
            m.MessageName = messageName;
            m.FormattedMessage = message;
            return m;            
        }

        private FontSettingItem NewFontSetting(string windowType, string fontName, int fontSize)
        {
            FontSettingItem f = new FontSettingItem();
            f.WindowType = windowType;
            f.FontName = fontName;
            f.FontSize = fontSize;
            return f;
        }

        private void LoadOptions()
        {
            if (File.Exists(optionsFile))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(IceChatOptions));
                TextReader textReader = new StreamReader(optionsFile);
                iceChatOptions = (IceChatOptions)deserializer.Deserialize(textReader);
                textReader.Close();
                textReader.Dispose();
            }
            else
            {
                //create default settings
                iceChatOptions = new IceChatOptions();
                SaveOptions();
            }
        }

        private void SaveOptions()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(IceChatOptions));
            TextWriter textWriter = new StreamWriter(optionsFile);
            serializer.Serialize(textWriter, iceChatOptions);
            textWriter.Close();
            textWriter.Dispose();
        }

        private void LoadMessageFormat()
        {
            if (File.Exists(messagesFile))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(IceChatMessageFormat));
                TextReader textReader = new StreamReader(messagesFile);
                iceChatMessages = (IceChatMessageFormat)deserializer.Deserialize(textReader);
                textReader.Close();
                textReader.Dispose();
                if (iceChatMessages.MessageSettings.Length != 44)
                    LoadDefaultMessageSettings();
            }
            else
            {
                iceChatMessages = new IceChatMessageFormat();
                LoadDefaultMessageSettings();
            }
        }

        private void SaveMessageFormat()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(IceChatMessageFormat));
            TextWriter textWriter = new StreamWriter(messagesFile);
            serializer.Serialize(textWriter, iceChatMessages);
            textWriter.Close();
            textWriter.Dispose();
        }

        private void LoadAliases()
        {
            if (File.Exists(aliasesFile))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(IceChatAliases));
                TextReader textReader = new StreamReader(aliasesFile);
                iceChatAliases = (IceChatAliases)deserializer.Deserialize(textReader);
                textReader.Close();
                textReader.Dispose();
            }
            else
            {
                iceChatAliases = new IceChatAliases();
                SaveAliases();
            }
        }

        private void SaveAliases()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(IceChatAliases));
            TextWriter textWriter = new StreamWriter(aliasesFile);
            serializer.Serialize(textWriter, iceChatAliases);
            textWriter.Close();
            textWriter.Dispose();
        }

        private void LoadEmoticons()
        {
            if (File.Exists(emoticonsFile))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(IceChatEmoticon));
                TextReader textReader = new StreamReader(emoticonsFile);
                iceChatEmoticons = (IceChatEmoticon)deserializer.Deserialize(textReader);
                textReader.Close();
                textReader.Dispose();
            }
            else
            {
                iceChatEmoticons = new IceChatEmoticon();
                SaveEmoticons();
            }
        }

        private void SaveEmoticons()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(IceChatEmoticon));
            //check if emoticons Folder Exists
            if (!System.IO.File.Exists(EmoticonsFolder))
                System.IO.Directory.CreateDirectory(EmoticonsFolder);

            TextWriter textWriter = new StreamWriter(emoticonsFile);
            serializer.Serialize(textWriter, iceChatEmoticons);
            textWriter.Close();
            textWriter.Dispose();
        }

        private void LoadPopups()
        {
            if (File.Exists(popupsFile))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(IceChatPopupMenus));
                TextReader textReader = new StreamReader(popupsFile);
                iceChatPopups = (IceChatPopupMenus)deserializer.Deserialize(textReader);
                textReader.Close();
                textReader.Dispose();
            }
            else
                iceChatPopups = new IceChatPopupMenus();
                
        }

        private void SavePopups()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(IceChatPopupMenus));
            TextWriter textWriter = new StreamWriter(popupsFile);
            serializer.Serialize(textWriter, iceChatPopups);
            textWriter.Close();
            textWriter.Dispose();
        }

        private void LoadFonts()
        {
            if (File.Exists(fontsFile))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(IceChatFontSetting));
                TextReader textReader = new StreamReader(fontsFile);
                iceChatFonts = (IceChatFontSetting)deserializer.Deserialize(textReader);
                textReader.Close();
                textReader.Dispose();
                if (iceChatFonts.FontSettings.Length < 7)
                    LoadDefaultFontSettings();
            }
            else
            {
                iceChatFonts = new IceChatFontSetting();
                LoadDefaultFontSettings();
            }
        }

        private void SaveFonts()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(IceChatFontSetting));
            TextWriter textWriter = new StreamWriter(fontsFile);
            serializer.Serialize(textWriter,iceChatFonts);
            textWriter.Close();
            textWriter.Dispose();
        }

        private void LoadColors()
        {
            if (File.Exists(colorsFile))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(IceChatColors));
                TextReader textReader = new StreamReader(colorsFile);
                iceChatColors = (IceChatColors)deserializer.Deserialize(textReader);
                textReader.Close();
                textReader.Dispose();
            }
            else
                iceChatColors = new IceChatColors();                
        }

        private void SaveColors()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(IceChatColors));
            TextWriter textWriter = new StreamWriter(colorsFile);
            serializer.Serialize(textWriter, iceChatColors);
            textWriter.Close();
            textWriter.Dispose();
        }
        #endregion

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
        }

        /// <summary>
        /// Create a Default Tab for showing Welcome Information
        /// </summary>
        private void CreateDefaultConsoleWindow()
        {
            ConsoleTabWindow t = new ConsoleTabWindow();
            t.AddConsoleTab("Welcome");
            t.Name = "Console";           
            
            tabMain.TabPages.Add(t);
            tabMain.SelectedTab = t;

            WindowMessage(null, "Console", "\x00034Welcome to " + Settings.Default.ProgramID + " " + Settings.Default.Version, 1, false);
            WindowMessage(null, "Console", "\x00034** This is an Alpha version, not fully functional **", 1, false);
            WindowMessage(null, "Console", "\x00033If you want a fully working version of \x0002IceChat\x0002, visit http://www.icechat.net and download IceChat 7.63", 1, false);
            WindowMessage(null, "Console", "\x00034Please visit \x00030,4#icechat2009\x0003 on \x00030,2irc://irc.quakenet.org\x0003 if you wish to help with this project", 1, true);

            StatusText("Welcome to " + Settings.Default.ProgramID + " " + Settings.Default.Version);

        }

        internal void ReportError(string Message, string Source, string FunctionName)
        {
            System.Diagnostics.Debug.WriteLine(FunctionName +  " Error:" + Message + " :: " + Source);
            //write it out to an error window of sorts...

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
            get { return tabMain; }
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
                    ((ConsoleTabWindow)tabMain.TabPages[0]).AddText(connection, data, color, scrollToBottom);
                }
                else
                {
                    foreach (TabWindow t in tabMain.WindowTabs)
                    {
                        if (t.WindowName == name)
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
                if (CurrentWindowType != TabWindow.WindowType.Console)
                {
                    TabWindow t = (TabWindow)tabMain.SelectedTab;
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
                    ((ConsoleTabWindow)tabMain.TabPages[0]).AddText(connection, data, color, scrollToBottom);
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
        internal TabWindow GetWindow(IRCConnection connection, string name, TabWindow.WindowType windowType)
        {
            foreach (TabWindow t in tabMain.WindowTabs)
            {
                if (t.WindowName.ToLower() == name.ToLower() && t.WindowStyle == windowType)
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
        internal TabWindow CurrentWindow
        {
            get
            {
                if (tabMain.SelectedWindowTabIndex() != 0)
                    return (TabWindow)tabMain.TabPages[tabMain.SelectedWindowTabIndex()];
                else
                    return null;
            }
        }
        /// <summary>
        /// Get the Current Window Type
        /// </summary>
        internal TabWindow.WindowType CurrentWindowType
        {
            get
            {
                if (tabMain.SelectedWindowTabIndex() == 0)
                    return TabWindow.WindowType.Console;
                else
                    return ((TabWindow)tabMain.TabPages[tabMain.SelectedWindowTabIndex()]).WindowStyle;
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
                    if (CurrentWindowType == TabWindow.WindowType.Console)
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

        #region IRCConnection Events

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
            
            tabMain.SelectedIndex = 0;

            inputPanel.CurrentConnection = c;
            serverTree.AddConnection(c);
            
            c.ConnectSocket();

        }

        /// <summary>
        /// Received a CTCP Message
        /// </summary>
        /// <param name="connection">Which Connection it came from</param>
        /// <param name="nick">The Nick who sent the CTCP Message</param>
        /// <param name="ctcp">The CTCP Message</param>
        private void OnCtcpMessage(IRCConnection connection, string nick, string ctcp)
        {
            //check if CTCP's are enabled
            if (connection.ServerSetting.DisableCTCP)
                return;

            string msg = GetMessageFormat("Ctcp Request");
            msg = msg.Replace("$nick", nick);
            msg = msg.Replace("$ctcp", ctcp);
            CurrentWindowMessage(connection, msg, 7, false);
            
            msg = GetMessageFormat("Ctcp Reply");
            msg = msg.Replace("$nick", nick);
            msg = msg.Replace("$ctcp", ctcp);
            
            switch (ctcp)
            {
                case "VERSION":
                    SendData(connection, "NOTICE " + nick + " :" + ((char)1).ToString() + "VERSION " + Settings.Default.ProgramID + " " + Settings.Default.Version + ((char)1).ToString());
                    msg = msg.Replace("$reply", Settings.Default.ProgramID + " " + Settings.Default.Version);
                    CurrentWindowMessage(connection, msg, 7, false);                    
                    break;
                case "PING":
                    SendData(connection, "NOTICE " + nick + " :" + ((char)1).ToString() + "PING " + System.Environment.TickCount.ToString() + ((char)1).ToString());
                    msg = msg.Replace("$reply", System.Environment.TickCount.ToString());
                    CurrentWindowMessage(connection, msg, 7, false);                                        
                    break;
                case "TIME":
                    SendData(connection, "NOTICE " + nick + " :" + ((char)1).ToString() + "TIME " + System.DateTime.Now.ToString() + ((char)1).ToString());
                    msg = msg.Replace("$reply", System.DateTime.Now.ToString());
                    CurrentWindowMessage(connection, msg, 7, false);
                    break;
                case "USERINFO":
                    SendData(connection, "NOTICE " + nick + " :" + ((char)1).ToString() + "USERINFO IceChat IRC Client : Download at http://www.icechat.net" + ((char)1).ToString());
                    msg = msg.Replace("$reply", "IceChat IRC Client : Download at http://www.icechat.net");
                    CurrentWindowMessage(connection, msg, 7, false);
                    break;
                case "CLIENTINFO":
                    SendData(connection, "NOTICE " + nick + " :" + ((char)1).ToString() + "CLIENTINFO This client supports: UserInfo, Finger, Version, Source, Ping, Time and ClientInfo" + ((char)1).ToString());
                    msg = msg.Replace("$reply", "This client supports: UserInfo, Finger, Version, Source, Ping, Time and ClientInfo");
                    CurrentWindowMessage(connection, msg, 7, false);                    
                    break;
                case "SOURCE":
                    SendData(connection, "NOTICE " + nick + " :" + ((char)1).ToString() + "SOURCE " + Settings.Default.ProgramID + " " + Settings.Default.Version + " http://www.icechat.net" + ((char)1).ToString());
                    msg = msg.Replace("$reply", Settings.Default.ProgramID + " " + Settings.Default.Version + " http://www.icechat.net");
                    CurrentWindowMessage(connection, msg, 7, false);
                    break;
                case "FINGER":
                    SendData(connection, "NOTICE " + nick + " :" + ((char)1).ToString() + "FINGER Stop fingering me" + ((char)1).ToString());
                    msg = msg.Replace("$reply","Stop fingering me");
                    CurrentWindowMessage(connection, msg, 7, false);
                    break;
                
            }
        }

        /// <summary>
        /// Received a User Notice
        /// </summary>
        /// <param name="connection">Which Connection it came from</param>
        /// <param name="nick">The Nick who sent the Notice</param>
        /// <param name="message">The Notice message</param>
        private void OnUserNotice(IRCConnection connection, string nick, string message)
        {
            string msg = GetMessageFormat("User Notice");
            msg = msg.Replace("$server", connection.ServerSetting.ServerName);
            msg = msg.Replace("$nick", nick);
            msg = msg.Replace("$message", message);
            CurrentWindowMessage(connection, msg, 1, false);
        }
        /// <summary>
        /// Received the full host for a userreply
        /// </summary>
        /// <param name="connection">Which Connection it came from</param>
        /// <param name="fullhost">The full user host Nick+=Ident@Host</param>
        private void OnUserHostReply(IRCConnection connection, string fullhost)
        {
            string host = fullhost.Substring(fullhost.IndexOf('@') + 1);
            string nick = "";
            if (fullhost.IndexOf('*') > -1)
                nick = fullhost.Substring(0, fullhost.IndexOf('*'));
            else
                nick = fullhost.Substring(0, fullhost.IndexOf('='));
            
            //update the internal addresslist and check for user in all channels
            InternalAddressList ial = new InternalAddressList(nick, host, "");

            if (!connection.ServerSetting.IAL.ContainsKey(nick))
                connection.ServerSetting.IAL.Add(nick, ial);
            else
                ((InternalAddressList)connection.ServerSetting.IAL[nick]).Host = host;

        }

        /// <summary>
        /// Received a Server Notice 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="message"></param>
        private void OnServerNotice(IRCConnection connection, string message)
        {
            string msg = GetMessageFormat("Server Notice");
            msg = msg.Replace("$server", connection.ServerSetting.ServerName);
            msg = msg.Replace("$message", message);
            ((ConsoleTabWindow)tabMain.TabPages[0]).AddText(connection, msg, 1, false);
        }

        /// <summary>
        /// Send out a message to be parsed
        /// </summary>
        /// <param name="connection">Which Connection it came from</param>
        /// <param name="data">The message to be parsed</param>
        private void OutGoingCommand(IRCConnection connection, string data)
        {
            ParseOutGoingCommand(connection, data);
        }

        /// <summary>
        /// Recieved a Standard Server Message
        /// </summary>
        /// <param name="connection">Which Connection it came from</param>
        /// <param name="message">The Server message</param>
        private void OnServerMessage(IRCConnection connection, string message)
        {
            //goes to the console
            string msg = GetMessageFormat("Server Message");
            msg = msg.Replace("$server", connection.ServerSetting.ServerName);
            msg = msg.Replace("$message", message);
            ((ConsoleTabWindow)tabMain.TabPages[0]).AddText(connection,msg, 1, false);            
            ((ConsoleTabWindow)tabMain.TabPages[0]).LastMessageType = ServerMessageType.ServerMessage;

        }

        /// <summary>
        /// Received Server Message of the Day
        /// </summary>
        /// <param name="connection">Which Connection it came from</param>
        /// <param name="message">Message</param>
        private void OnServerMOTD(IRCConnection connection, string message)
        {
            string msg = GetMessageFormat("Server MOTD");
            msg = msg.Replace("$server", connection.ServerSetting.ServerName);
            msg = msg.Replace("$message", message);
            ((ConsoleTabWindow)tabMain.TabPages[0]).AddText(connection,msg, 1, false);
            ((ConsoleTabWindow)tabMain.TabPages[0]).LastMessageType = ServerMessageType.ServerMessage;

        }

        /// <summary>
        /// Received a Channel for the Server Channel List
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="channel">Channel Name</param>
        /// <param name="users">Total Users in Channel</param>
        /// <param name="topic">Channel Topic</param>
        private void OnChannelList(IRCConnection connection, string channel, string users, string topic)
        {
            //will make a seperate window for this eventually
            ((ConsoleTabWindow)tabMain.TabPages[0]).AddText(connection,channel + " : " + users + " : " + topic, 7, false);
        }


        /// <summary>
        /// Received a Server/Connection Error
        /// </summary>
        /// <param name="connection">Which Connection it came from</param>
        /// <param name="message">Error Message</param>
        private void OnServerError(IRCConnection connection, string message)
        {
            string[] msgs = message.Split('\n');
            foreach (string msg in msgs)
            {
                if (msg.Length > 0)
                {
                    //goes to the console                        
                    string error = GetMessageFormat("Server Error");
                    error = error.Replace("$server", connection.ServerSetting.ServerName);
                    error = error.Replace("$message", msg);

                    ((ConsoleTabWindow)tabMain.TabPages[0]).AddText(connection, error, 4, false);

                    //send it to the current window as well
                    if (CurrentWindowType != TabWindow.WindowType.Console)
                        CurrentWindowMessage(connection, error, 4, false);

                    ((ConsoleTabWindow)tabMain.TabPages[0]).LastMessageType = ServerMessageType.ServerMessage;
                }
            }

        }

        /// <summary>
        /// Received Whois Data on a Nick
        /// </summary>
        /// <param name="connection">Which Connection it came from</param>
        /// <param name="nick">The nick whois data is from</param>
        /// <param name="data">The Whois data</param>
        private void OnWhoisData(IRCConnection connection, string nick, string data)
        {
            string msg = GetMessageFormat("User Whois");
            msg = msg.Replace("$nick", nick);
            msg = msg.Replace("$data", data);
            
            //check if there is a query window open
            TabWindow t = GetWindow(connection, nick, TabWindow.WindowType.Query);
            if (t != null)
            {                
                t.TextWindow.AppendText(msg, 1);
                t.LastMessageType = ServerMessageType.Message;
            }
            else
            {
                //send whois data to the current window instead
                CurrentWindowMessage(connection, msg, 1, false);
            }
        }

        /// <summary>
        /// Received a Query/Private Message action
        /// </summary>
        /// <param name="connection">Which Connection it came from</param>
        /// <param name="nick">Nick who sent the action</param>
        /// <param name="message">Query Action Message</param>
        private void OnQueryAction(IRCConnection connection, string nick, string host, string message)
        {
            if (!tabMain.WindowExists(connection, nick, TabWindow.WindowType.Query) && iceChatOptions.DisableQueries)
                return;

            if (!tabMain.WindowExists(connection, nick, TabWindow.WindowType.Query))
                AddWindow(connection, nick, TabWindow.WindowType.Query);

            TabWindow t = GetWindow(connection, nick, TabWindow.WindowType.Query);
            if (t != null)
            {
                string msg = GetMessageFormat("Private Action");
                msg = msg.Replace("$nick", nick).Replace("$host", host);
                msg = msg.Replace("$message", message);

                bool ishandled = false;
                PluginArgs args = new PluginArgs(t.TextWindow, "", nick, host, msg);
                args.Connection = connection;

                foreach (IPluginIceChat ipc in loadedPlugins)
                {
                    if (ipc.QueryAction(args) == true)
                        ishandled = true;
                }

                if (!ishandled)
                    t.TextWindow.AppendText(msg, 1);

                t.LastMessageType = ServerMessageType.Action;
            }
        }
        /// <summary>
        /// Received a Query/Private Message 
        /// </summary>
        /// <param name="connection">Which Connection it came from</param>
        /// <param name="nick">Nick who sent the message</param>
        /// <param name="message">Query Message</param>
        private void OnQueryMessage(IRCConnection connection, string nick, string host, string message)
        {
            if (!tabMain.WindowExists(connection, nick, TabWindow.WindowType.Query) && iceChatOptions.DisableQueries)
                return;
            
            if (!tabMain.WindowExists(connection,nick, TabWindow.WindowType.Query))
                AddWindow(connection, nick, TabWindow.WindowType.Query);

            TabWindow t = GetWindow(connection, nick, TabWindow.WindowType.Query);
            if (t != null)
            {
                string msg = GetMessageFormat("Private Message");
                msg = msg.Replace("$nick", nick).Replace("$host", host);
                msg = msg.Replace("$message", message);

                bool ishandled = false;
                PluginArgs args = new PluginArgs(t.TextWindow, "", nick, host, msg);
                args.Connection = connection;

                foreach (IPluginIceChat ipc in loadedPlugins)
                {
                    if (ipc.QueryMessage(args) == true)
                        ishandled = true;
                }

                if (!ishandled)
                    t.TextWindow.AppendText(msg, 1);
                
                t.LastMessageType = ServerMessageType.Message;                
            }
        }

        /// <summary>
        /// Received a Channel Action
        /// </summary>
        /// <param name="connection">Which Connection it came from</param>
        /// <param name="channel">Which Channel it is from</param>
        /// <param name="nick">Nick who sent the action</param>
        /// <param name="message">Channel action</param>
        private void OnChannelAction(IRCConnection connection, string channel, string nick, string host, string message)        
        {
            TabWindow t = GetWindow(connection, channel, TabWindow.WindowType.Channel);
            if (t != null)
            {
                string msg = GetMessageFormat("Channel Action");
                msg = msg.Replace("$nick", nick).Replace("$channel", channel);
                msg = msg.Replace("$color", "");
                msg = msg.Replace("$message", message);
                
                bool ishandled = false;
                PluginArgs args = new PluginArgs(t.TextWindow, channel, nick, host, msg);
                args.Connection = connection;
               
                foreach (IPluginIceChat ipc in loadedPlugins)
                {
                    if (ipc.ChannelMessage(args) == true)
                        ishandled = true;
                }

                if (!ishandled)
                    t.TextWindow.AppendText(msg, 1);
                
                t.LastMessageType = ServerMessageType.Action;                
            }    
        }

        /// <summary>
        /// Received a Channel Message
        /// </summary>
        /// <param name="connection">Which Connection it came from</param>
        /// <param name="channel">Which Channel it is from</param>
        /// <param name="nick">Nick who sent the message</param>
        /// <param name="message">Channel Message</param>
        private void OnChannelMessage(IRCConnection connection, string channel, string nick, string host, string message)
        {
            TabWindow t = GetWindow(connection, channel, TabWindow.WindowType.Channel);
            if (t != null)
            {
                string msg = GetMessageFormat("Channel Message");
                msg = msg.Replace("$nick", nick).Replace("$channel", channel).Replace("$host", host);

                //assign $color to the nickname color
                //get the user mode for the nickname                
                if (msg.Contains("$color") && t.NickExists(nick))
                {
                    User u = t.GetNick(nick);
                    
                    for (int i = 0; i < u.Level.Length; i++)
                    {
                        if (u.Level[i])
                        {
                            if (connection.ServerSetting.StatusModes[0][i] == 'q')
                                msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelOwnerColor);
                            else if (connection.ServerSetting.StatusModes[0][i] == 'a')
                                msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelAdminColor);
                            else if (connection.ServerSetting.StatusModes[0][i] == 'o')
                                msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelOpColor);
                            else if (connection.ServerSetting.StatusModes[0][i] == 'h')
                                msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelHalfOpColor);
                            else if (connection.ServerSetting.StatusModes[0][i] == 'v')
                                msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelVoiceColor);
                            else
                                msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelOwnerColor);
                            
                            break;
                        }
                    } 
                    
                    if (msg.Contains("$color"))
                        msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelRegularColor);

                }

                //check if the nickname exists
                if (t.NickExists(nick))
                    msg = msg.Replace("$status", t.GetNick(nick).ToString().Replace(nick, ""));
                else
                    msg = msg.Replace("$status", "");

                msg = msg.Replace("$message", message);

                bool ishandled = false;
                PluginArgs args = new PluginArgs(t.TextWindow, channel, nick, host, msg);
                args.Connection = connection;

                foreach (IPluginIceChat ipc in loadedPlugins)
                {
                    if (ipc.ChannelMessage(args) == true)
                        ishandled = true;
                }              
  
                if (!ishandled)
                    t.TextWindow.AppendText(msg, 1);
                
                t.LastMessageType = ServerMessageType.Message;
            }
        }

        /// <summary>
        /// Received a Standard/generic Channel Message
        /// </summary>
        /// <param name="connection">Which Connection it came from</param>
        /// <param name="channel">Which Channel it is from</param>
        /// <param name="message">Channel Message</param>
        private void OnGenericChannelMessage(IRCConnection connection, string channel, string message)
        {
            TabWindow t = GetWindow(connection, channel, TabWindow.WindowType.Channel);
            if (t != null)
            {
                string msg = GetMessageFormat("Channel Other");
                msg = msg.Replace("$message", message);

                t.TextWindow.AppendText(msg, 1);
                t.LastMessageType = ServerMessageType.Other;                
            }
        }

        /// <summary>
        /// Clear the NickName List for Specified Channel
        /// </summary>
        /// <param name="connection">Which Connection it came from</param>
        /// <param name="channel">Which Channel to Clear</param>
        /*
		private void OnClearNicks(IRCConnection connection, string channel)
        {
            TabWindow t = GetWindow(connection, channel, TabWindow.WindowType.Channel);
            if (t != null)
            {
                t.Nicks.Clear();
                if (nickList.CurrentWindow == t)
                    nickList.RefreshList(t);
            }            
        }
        */

        /// <summary>
        /// A User Quit the Server
        /// </summary>
        /// <param name="connection">Which Connection it came from</param>
        /// <param name="user">Which Nick quit the Server</param>
        /// <param name="reason">Quit Reason</param>
        private void OnServerQuit(IRCConnection connection, string nick, string host, string reason)
        {
            foreach (TabWindow t in tabMain.WindowTabs)
            {
                if (t.Connection == connection)
                {
                    string msg = GetMessageFormat("Server Quit");
                    msg = msg.Replace("$nick", nick);
                    msg = msg.Replace("$host", host);
                    msg = msg.Replace("$reason", reason);

                    bool ishandled = false;
                    PluginArgs args = new PluginArgs(t.TextWindow, "", nick, host, msg);
                    args.Extra = reason;
                    
                    args.Connection = connection;
                    
                    if (t.WindowStyle == TabWindow.WindowType.Channel)
                    {
                        if (t.NickExists(nick) == true)
                        {
                            foreach (IPluginIceChat ipc in loadedPlugins)
                            {
                                if (ipc.ServerQuit(args) == true)
                                    ishandled = true;
                            }

                            if (!ishandled)
                                t.TextWindow.AppendText(msg, 1);

                            t.LastMessageType = ServerMessageType.QuitServer;                            
                            t.RemoveNick(nick);
                        }
                    }
                    if (t.WindowStyle == TabWindow.WindowType.Query)
                    {
                        if (t.WindowName == nick)
                        {
                            foreach (IPluginIceChat ipc in loadedPlugins)
                            {
                                if (ipc.ServerQuit(args) == true)
                                    ishandled = true;
                            }

                            if (!ishandled)
                                t.TextWindow.AppendText(msg, 1);

                            t.LastMessageType = ServerMessageType.QuitServer;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// A User Joined a Channel
        /// </summary>
        /// <param name="connection">Which Connection it came from</param>
        /// <param name="channel">Which Channel was Joined</param>
        /// <param name="user">Full User Host of who Joined</param>
        /// <param name="refresh">Whether to Refresh the Nick List</param>
        private void OnChannelJoin(IRCConnection connection, string channel, string nick, string host, bool refresh)
        {
            TabWindow t = GetWindow(connection, channel, TabWindow.WindowType.Channel);
            if (t != null)
            {
                if (refresh)
                {
                    string msg = GetMessageFormat("Channel Join");
                    msg = msg.Replace("$nick", nick).Replace("$channel", channel).Replace("$host", host);

                    bool ishandled = false;
                    PluginArgs args = new PluginArgs(t.TextWindow, channel, nick, host, msg);
                    args.Connection = connection;

                    foreach (IPluginIceChat ipc in loadedPlugins)
                    {
                        if (ipc.ChannelJoin(args) == true)
                            ishandled = true;
                    }

                    if (!ishandled)
                        t.TextWindow.AppendText(msg, 1);
                }
                
                t.AddNick(nick, refresh);                
                t.LastMessageType = ServerMessageType.JoinChannel;
            }
        }
        
        /// <summary>
        /// A User Parted/Left a Channel
        /// </summary>
        /// <param name="connection">Which Connection it came from</param>
        /// <param name="channel">Which Channel was Parted</param>
        /// <param name="user">Full User Host of who Parted</param>
        /// <param name="reason">Part Reason (if any)</param>
        private void OnChannelPart(IRCConnection connection, string channel, string nick, string host, string reason)
        {
            TabWindow t = GetWindow(connection,channel, TabWindow.WindowType.Channel);
            if (t != null)
            {
                string msg = GetMessageFormat("Channel Part");
                msg = msg.Replace("$channel", channel).Replace("$nick", nick).Replace("$host", host).Replace("$reason", reason);

                bool ishandled = false;
                PluginArgs args = new PluginArgs(t.TextWindow, channel, nick, host, msg);
                args.Connection = connection;

                foreach (IPluginIceChat ipc in loadedPlugins)
                {
                    if (ipc.ChannelPart(args) == true)
                        ishandled = true;
                }

                if (!ishandled)
                    t.TextWindow.AppendText(msg, 1);

                t.RemoveNick(nick);
                t.LastMessageType = ServerMessageType.PartChannel;
            }
        }

        /// <summary>
        /// A User was kicked from a Channel
        /// </summary>
        /// <param name="connection">Which Connection it came from</param>
        /// <param name="channel">Which Channel the User was Kicked from</param>
        /// <param name="nick">Nickname of who was Kicked</param>
        /// <param name="reason">Kick Reason</param>
        /// <param name="kickUser">Full User Host of Who kicked the User</param>
        private void OnKickNick(IRCConnection connection, string channel, string nick, string reason, string kickUser)
        {
            TabWindow t = GetWindow(connection,channel, TabWindow.WindowType.Channel);
            if (t != null)
            {                
                string kickNick = NickFromFullHost(kickUser);
                string kickHost = HostFromFullHost(kickUser);

                string msg = GetMessageFormat("Channel Kick");
                msg = msg.Replace("$nick", kickNick);
                msg = msg.Replace("$host", kickHost);
                msg = msg.Replace("$kickee", nick);
                msg = msg.Replace("$channel", channel);
                msg = msg.Replace("$reason", reason);
                
                t.TextWindow.AppendText(msg, 1);
                t.RemoveNick(nick);
                t.LastMessageType = ServerMessageType.Other;
            }
        }

        /// <summary>
        /// You have Joined a Channel
        /// </summary>
        /// <param name="connection">Which Connection it came from</param>
        /// <param name="channel">Which Channel you joined</param>
        private void OnChannelJoinSelf(IRCConnection connection, string channel)
        {
            //check if channel window already exists
            TabWindow t = GetWindow(connection, channel, TabWindow.WindowType.Channel);
            if (t == null)
                AddWindow(connection, channel, TabWindow.WindowType.Channel);
        }

        /// <summary>
        /// You have Parted/Left a Channel
        /// </summary>
        /// <param name="connection">Which Connection it came from</param>
        /// <param name="channel">Which Channel you parted</param>
        private void OnChannelPartSelf(IRCConnection connection, string channel)
        {
            string reason = "";
            string msg = GetMessageFormat("Self Channel Part");
            msg = msg.Replace("$nick", connection.ServerSetting.NickName).Replace("$channel", channel);
            msg = msg.Replace("$reason", reason);
            
            TabWindow t = GetWindow(connection, channel, TabWindow.WindowType.Channel);
            if (t != null)
            {
                t.IsFullyJoined = false;
                t.ClearNicks();
            }
            
            ((ConsoleTabWindow)tabMain.TabPages[0]).AddText(connection, msg, 1, false);
        }

        /// <summary>
        /// You where Kicked from a Channel
        /// </summary>
        /// <param name="connection">Which Connection it came from</param>
        /// <param name="channel">Which Channel you were kicked from</param>
        /// <param name="reason">Kick Reason</param>
        /// <param name="kickUser">Full User Host of who kicked you</param>
        private void OnKickSelf(IRCConnection connection, string channel, string reason, string kickUser)
        {
            try
            {
                RemoveWindow(connection, channel);

                string nick = NickFromFullHost(kickUser);
                string host = HostFromFullHost(kickUser);

                string msg = GetMessageFormat("Self Channel Kick");
                msg = msg.Replace("$nick", connection.ServerSetting.NickName);
                msg = msg.Replace("$kicker", nick);
                msg = msg.Replace("$host", host);
                msg = msg.Replace("$channel", channel);
                msg = msg.Replace("$reason", reason);

                ((ConsoleTabWindow)tabMain.TabPages[0]).AddText(connection, msg, 1, false);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("OnKickSelf Error:" + e.Message + " :: " + e.StackTrace);
            }
        }

        /// <summary>
        /// A User changed their Nick Name
        /// </summary>
        /// <param name="connection">Which Connection it came from</param>
        /// <param name="oldnick">Original Nick Name</param>
        /// <param name="newnick">New Nick Name</param>
        private void OnChangeNick(IRCConnection connection, string oldnick, string newnick, string host)
        {
            try
            {
                if (CurrentWindowType == TabWindow.WindowType.Console)
                {
                    if (inputPanel.CurrentConnection == connection)
                    {
                        StatusText(inputPanel.CurrentConnection.ServerSetting.NickName + " connected to " + inputPanel.CurrentConnection.ServerSetting.RealServerName);

                        if (connection.ServerSetting.NickName == newnick)
                        {
                            string msg = GetMessageFormat("Self Nick Change");
                            msg = msg.Replace("$nick", oldnick).Replace("$newnick", newnick).Replace("$host", host); ;

                            ((ConsoleTabWindow)tabMain.TabPages[0]).AddText(connection, msg, 1, false);
                        }
                    }
                }

                foreach (TabWindow t in tabMain.WindowTabs)
                {
                    if (t.Connection == connection)
                    {
                        if (t.WindowStyle == TabWindow.WindowType.Channel)
                        {
                            if (t.NickExists(oldnick))
                            {
                                if (connection.ServerSetting.NickName == newnick)
                                {
                                    string msg = GetMessageFormat("Self Nick Change");
                                    msg = msg.Replace("$nick", oldnick).Replace("$newnick", newnick).Replace("$host", host); ;

                                    t.TextWindow.AppendText(msg, 1);
                                    //update status bar as well if current channel
                                    if ((inputPanel.CurrentConnection == connection) && (CurrentWindowType == TabWindow.WindowType.Channel))
                                    {
                                        if (CurrentWindow == t)
                                            StatusText(t.Connection.ServerSetting.NickName + " in " + t.WindowName + " [" + t.ChannelModes + "] {" + t.Connection.ServerSetting.RealServerName + "}");
                                    }
                                }
                                else
                                {
                                    string msg = GetMessageFormat("Channel Nick Change");
                                    msg = msg.Replace("$nick", oldnick).Replace("$newnick", newnick).Replace("$host", host);

                                    t.TextWindow.AppendText(msg, 1);
                                }

                                User u = t.GetNick(oldnick);
                                string nick = newnick;
                                if (u != null)
                                {
                                    for (int i = 0; i < u.Level.Length; i++)
                                    {
                                        if (u.Level[i])
                                        {
                                            if (!nick.StartsWith(connection.ServerSetting.StatusModes[1][i].ToString()))
                                                nick = connection.ServerSetting.StatusModes[1][i] + nick;
                                            break;
                                        }
                                    }

                                    t.RemoveNick(oldnick);
                                    t.AddNick(nick, true);
                                    t.LastMessageType = ServerMessageType.Other;
                                }
                                
                                if ((nickList.CurrentWindow.WindowName == t.WindowName) && (nickList.CurrentWindow.Connection == t.Connection))
                                    nickList.RefreshList(t);

                            }
                        }
                        else if (t.WindowStyle == TabWindow.WindowType.Query)
                        {

                            if (t.WindowName == newnick)
                            {
                                string msg = GetMessageFormat("Nick Change");
                                msg = msg.Replace("$nick", oldnick);
                                msg = msg.Replace("$newnick", newnick);
                                msg = msg.Replace("$host", host);

                                if (connection.ServerSetting.NickName == newnick)
                                {
                                    t.TextWindow.AppendText(msg, 1);
                                    if ((inputPanel.CurrentConnection == connection) && (CurrentWindowType == TabWindow.WindowType.Query))
                                    {
                                        if (CurrentWindow == t)
                                            StatusText(t.Connection.ServerSetting.NickName + " in " + t.WindowName + " [" + t.ChannelModes + "] {" + t.Connection.ServerSetting.RealServerName + "}");
                                    }
                                }
                                else
                                    t.TextWindow.AppendText(msg, 1);

                                if ((nickList.CurrentWindow.WindowName == newnick) && (nickList.CurrentWindow.Connection == t.Connection))
                                    nickList.RefreshList(t);

                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("OnChangeNick Error:" + e.Message + " :: " + e.StackTrace);
            }
        }

        /// <summary>
        /// Channel Topic Changed
        /// </summary>
        /// <param name="connection">Which Connection it came from</param>
        /// <param name="channel">Which Channel the Topic changed for</param>
        /// <param name="nick">Nick who changed the Topic</param>
        /// <param name="topic">New Channel Topic</param>
        private void OnChannelTopic(IRCConnection connection, string channel, string nick, string host, string topic)
        {
            TabWindow t = GetWindow(connection, channel, TabWindow.WindowType.Channel);
            if (t != null)
            {
                
                //t.ChannelTopic = topic;
                string msgt = GetMessageFormat("Channel Topic Text");
                msgt = msgt.Replace("$channel", channel);
                msgt = msgt.Replace("$topic", topic);
                t.ChannelTopic = msgt;

                if (nick.Length > 0)
                {
                    string msg = GetMessageFormat("Channel Topic Change");
                    msg = msg.Replace("$nick", nick);
                    msg = msg.Replace("$host", host);                    
                    msg = msg.Replace("$channel", channel);
                    msg = msg.Replace("$topic", topic);
                    t.TextWindow.AppendText(msg, 1);
                }
                else
                {
                    t.TextWindow.AppendText(msgt, 1);
                }
                
                t.LastMessageType = ServerMessageType.Other;                                
            }
        }

        /// <summary>
        /// Your User Mode for the Server has Changed
        /// </summary>
        /// <param name="connection">Which Connection it came from</param>
        /// <param name="nick">Your Nick Name</param>
        /// <param name="mode">New User Mode(s)</param>
        private void OnUserMode(IRCConnection connection, string nick, string mode)
        {
            string msg = GetMessageFormat("Server Mode");
            msg = msg.Replace("$server", connection.ServerSetting.ServerName);
            msg = msg.Replace("$mode", mode);
            msg = msg.Replace("$nick", nick);
            ((ConsoleTabWindow)tabMain.TabPages[0]).AddText(connection, msg, 1, false);

            //parse out the user modes
            //set the mode in Server Setting

        }

        /// <summary>
        /// Channel Mode Changed
        /// </summary>
        /// <param name="connection">Which Connection it came from</param>
        /// <param name="modeSetter">Who set the mode(s)</param>
        /// <param name="channel">Channel which mode change is for</param>
        /// <param name="fullmode">All the modes and parameters</param>
        private void OnChannelMode(IRCConnection connection, string modeSetter, string modeSetterHost, string channel, string fullmode)
        {
            try
            {
                string mode = "";
                string parameter = "";

                if (fullmode.IndexOf(' ') == -1)
                {
                    mode = fullmode;
                }
                else
                {
                    mode = fullmode.Substring(0, fullmode.IndexOf(' '));
                    parameter = fullmode.Substring(fullmode.IndexOf(' ') + 1);
                }

                string msg = GetMessageFormat("Channel Mode");
                msg = msg.Replace("$modeparam", parameter);
                msg = msg.Replace("$mode", mode);
                msg = msg.Replace("$nick", modeSetter);
                msg = msg.Replace("$host", modeSetterHost);
                msg = msg.Replace("$channel", channel);

                TabWindow chan = GetWindow(connection, channel, TabWindow.WindowType.Channel);
                if (chan != null)
                {
                    //chan.UpdateNick(modeSetter, modeSetterHost);

                    if (modeSetter != channel)
                    {
                        //System.Diagnostics.Debug.WriteLine(msg);
                        WindowMessage(connection, channel, msg, 1, false);
                        //now update the modes accordingly

                    }
                    else
                    {
                        chan.ChannelModes = fullmode.Trim();
                        
                    }


                    string[] parameters = parameter.Split(new char[] { ' ' });

                    bool addMode = false;
                    int modelength = mode.Length;
                    string temp;

                    IEnumerator parametersEnumerator = parameters.GetEnumerator();
                    parametersEnumerator.MoveNext();
                    for (int i = 0; i < modelength; i++)
                    {
                        switch (mode[i])
                        {
                            case '-':
                                addMode = false;
                                break;
                            case '+':
                                addMode = true;
                                break;
                            case 'b':
                                //handle bans seperately
                                temp = (string)parametersEnumerator.Current;
                                parametersEnumerator.MoveNext();
                                break;
                            default:
                                //check if it's a status mode which can vary by server
                                temp = (string)parametersEnumerator.Current;
                                for (int j = 0; j < connection.ServerSetting.StatusModes[0].Length; j++)
                                {
                                    if (mode[i] == connection.ServerSetting.StatusModes[0][j])
                                    {
                                        chan.UpdateNick(temp, connection.ServerSetting.StatusModes[1][j].ToString(), addMode);
                                        break;
                                    }
                                }

                                //check if the mode has a parameter (CHANMODES= from 005)
                                //System.Diagnostics.Debug.WriteLine(connection.ServerSetting.ChannelModeParams.ToString() + ":" + mode[i]);
                                if (connection.ServerSetting.ChannelModeParams.Contains(mode[i].ToString()))
                                {
                                    //mode has parameter
                                    temp = (string)parametersEnumerator.Current;
                                    parametersEnumerator.MoveNext();
                                    chan.UpdateChannelMode(mode[i], temp, addMode);
                                }
                                else
                                    //check if it is an actual channel mode, and not a user mode
                                    if (connection.ServerSetting.ChannelModeNoParams.Contains(mode[i].ToString()))
                                        chan.UpdateChannelMode(mode[i], addMode);
                                break;

                        }
                    }
                    if (inputPanel.CurrentConnection == connection)
                    {
                        if (CurrentWindowType == TabWindow.WindowType.Channel)
                            if (CurrentWindow == chan)
                                StatusText(connection.ServerSetting.NickName + " in " + chan.WindowName + " [" + chan.ChannelModes + "] {" + chan.Connection.ServerSetting.RealServerName + "}");
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("OnChannelMode:" + e.Message + " :: " + e.StackTrace);
            }
        }

        /// <summary>
        /// When a User Invites you to a Channel
        /// </summary>
        /// <param name="connection">Which connection it came from</param>
        /// <param name="channel">The channel you are being invited to</param>
        /// <param name="nick">The nick who invited you</param>
        /// <param name="host">The host of the nick who invited you</param>
        private void OnChannelInvite(IRCConnection connection, string channel, string nick, string host)
        {
            string msg = GetMessageFormat("Channel Invite");
            msg = msg.Replace("$channel", channel).Replace("$nick", nick).Replace("$host", host);
            ((ConsoleTabWindow)tabMain.TabPages[0]).AddText(connection, msg, 1, false);
        }

        /// <summary>
        /// Received a Channel Notice
        /// </summary>
        /// <param name="connection">The connection the notice was received on</param>
        /// <param name="nick">The nick who sent the notice</param>
        /// <param name="host">The host of the nick who sent the notice</param>
        /// <param name="status">The status char that the notice was sent to</param>
        /// <param name="channel">The channel the notice was sent to</param>
        /// <param name="notice">The notice message</param>
        private void OnChannelNotice(IRCConnection connection, string nick, string host, char status, string channel, string message)
        {
            TabWindow t = GetWindow(connection, channel, TabWindow.WindowType.Channel);
            if (t != null)
            {
                string msg = GetMessageFormat("Channel Notice");
                msg = msg.Replace("$nick", nick);
                msg = msg.Replace("$host", host);
                if (status == '0')
                    msg = msg.Replace("$status", "");
                else
                    msg = msg.Replace("$status", status.ToString());
                
                msg = msg.Replace("$channel", channel);
                msg = msg.Replace("$message", message);
                t.TextWindow.AppendText(msg, 1);
                
                t.LastMessageType = ServerMessageType.Message;
            }
        }

        /// <summary>
        /// Shows raw Server Data in a Debug Window
        /// </summary>
        /// <param name="connection">The connection the notice was received on</param>
        /// <param name="data">The Raw Server Data</param>
        private void OnRawServerData(IRCConnection connection, string data)
        {
            //check if a Debug Window is open
            TabWindow t = GetWindow(null, "Debug", TabWindow.WindowType.Debug);
            if (t != null)
                t.TextWindow.AppendText(connection.ServerSetting.ID + ":" + data, 1);

            PluginArgs args = new PluginArgs(connection);
            args.Message = data;
            foreach (IPluginIceChat ipc in loadedPlugins)
            {
                ipc.ServerRaw(args);
                if (args.Command != null)
                {
                    if (args.Command.Length > 0)
                        ParseOutGoingCommand(connection, args.Command);
                }
            }

        }

        private void OnRawServerOutgoingData(IRCConnection connection, string data)
        {
            //check if a Debug Window is open
            TabWindow t = GetWindow(null, "Debug", TabWindow.WindowType.Debug);
            if (t != null)
                t.TextWindow.AppendText("-" + connection.ServerSetting.ID + ":" + data, 1);
        }


        private void OnIALUserData(IRCConnection connection, string nick, string host, string channel)
        {
            //internal addresslist userdata            
            for (int i = 0; i < connection.ServerSetting.StatusModes[1].Length; i++)
                nick = nick.Replace(connection.ServerSetting.StatusModes[1][i].ToString(), string.Empty);

            InternalAddressList ial = new InternalAddressList(nick, host, channel);
            
            if (!connection.ServerSetting.IAL.ContainsKey(nick))
                connection.ServerSetting.IAL.Add(nick, ial);
            else
            {
                ((InternalAddressList)connection.ServerSetting.IAL[nick]).AddChannel(channel);
                ((InternalAddressList)connection.ServerSetting.IAL[nick]).Host = host;
            }
            
        }

        private void OnIALUserChange(IRCConnection connection, string oldnick, string newnick)
        {
            //change a nickname in the IAL list
            if (connection.ServerSetting.IAL.ContainsKey(oldnick))
            {
                InternalAddressList ial = (InternalAddressList)connection.ServerSetting.IAL[oldnick];
                connection.ServerSetting.IAL.Remove(oldnick);
                ial.Nick = newnick;
                connection.ServerSetting.IAL.Add(newnick, ial);
            }
        }

        private void OnIALUserQuit(IRCConnection connection, string nick)
        {
            //user has quit, remove from IAL
            if (connection.ServerSetting.IAL.ContainsKey(nick))
                connection.ServerSetting.IAL.Remove(nick);

        }
        private void OnIALUserPart(IRCConnection connection, string nick, string channel)
        {
            //user left a channel, remove from channel list
            InternalAddressList ial = (InternalAddressList)connection.ServerSetting.IAL[nick];
            if (ial != null)
            {
                ial.RemoveChannel(channel);
                //if channels count is 0, remove the nick from the ial
                if (ial.Channels.Count == 0)
                    connection.ServerSetting.IAL.Remove(nick);
            }
        }



        #endregion

        #region Tab Events and Methods

        /// <summary>
        /// Add a new Connection Tab to the Console
        /// </summary>
        /// <param name="connection">Which Connection to add</param>
        private void OnAddConsoleTab(IRCConnection connection)
        {
            ((ConsoleTabWindow)tabMain.TabPages[0]).AddConsoleTab(connection);
        }

        /// <summary>
        /// Add a new Tab Window to the Main Tab Control
        /// </summary>
        /// <param name="connection">Which Connection it came from</param>
        /// <param name="windowName">Window Name of the New Tab</param>
        /// <param name="windowType">Window Type of the New Tab</param>
        internal void AddWindow(IRCConnection connection, string windowName, TabWindow.WindowType windowType)
        {
            if (this.InvokeRequired)
            {
                AddWindowDelegate a = new AddWindowDelegate(AddWindow);
                this.Invoke(a, new object[] { connection, windowName, windowType });
            }
            else
            {                
                TabWindow t = new TabWindow(windowName);
                t.WindowStyle = windowType;                
                t.Connection = connection;

                if (t.WindowStyle == TabWindow.WindowType.Channel)
                {
                    t.TextWindow.Font = new Font(iceChatFonts.FontSettings[1].FontName, iceChatFonts.FontSettings[1].FontSize);
                    t.TopicWindow.Font = new Font(iceChatFonts.FontSettings[1].FontName, iceChatFonts.FontSettings[1].FontSize);
                
                    //send the message
                    string msg = GetMessageFormat("Self Channel Join");
                    msg = msg.Replace("$nick", connection.ServerSetting.NickName).Replace("$channel", windowName);

                    if (FormMain.Instance.IceChatOptions.LogChannel)
                        t.TextWindow.SetLogFile(FormMain.Instance.LogsFolder + System.IO.Path.DirectorySeparatorChar + connection.ServerSetting.ServerName);

                    t.TextWindow.AppendText(msg, 1);
                }
                else if (t.WindowStyle == TabWindow.WindowType.Query)
                {
                    t.TextWindow.Font = new Font(iceChatFonts.FontSettings[2].FontName, iceChatFonts.FontSettings[2].FontSize);
                    if (FormMain.Instance.IceChatOptions.LogQuery)
                        t.TextWindow.SetLogFile(FormMain.Instance.LogsFolder + System.IO.Path.DirectorySeparatorChar + connection.ServerSetting.ServerName);
                }
                else if (t.WindowStyle == TabWindow.WindowType.Debug)
                {
                    t.TextWindow.NoColorMode = true;
                    t.TextWindow.Font = new Font("Verdana", 10);
                    t.TextWindow.SetLogFile(FormMain.Instance.LogsFolder);
                }
                
                //find the last window index for this connection
                int index = 0;
                if (t.WindowStyle == TabWindow.WindowType.Channel || t.WindowStyle == TabWindow.WindowType.Query)
                {
                    for (int i = 1; i < tabMain.TabPages.Count; i++)
                    {
                        if (((TabWindow)tabMain.TabPages[i]).Connection == connection)
                            index = i + 1;
                    }
                }
                
                if (index == 0)
                    tabMain.TabPages.Add(t);
                else
                    tabMain.TabPages.Insert(index, t);

                if (t.WindowStyle == TabWindow.WindowType.Query && !iceChatOptions.NewQueryForegound)
                    tabMain.SelectedTab = tabMain.SelectedTab;
                else
                {
                    tabMain.SelectedTab = t;
                    nickList.CurrentWindow = t;
                }
                serverTree.Invalidate();

                if (t.WindowStyle == TabWindow.WindowType.Query && iceChatOptions.WhoisNewQuery)
                    ParseOutGoingCommand(t.Connection, "/whois " + t.WindowName);
                        
            }
        }
        /// <summary>
        /// Remove a Tab Window from the Main Tab Control
        /// </summary>
        /// <param name="connection">Which Connection it is for</param>
        /// <param name="channel">The Channel/Query Window Name</param>
        internal void RemoveWindow(IRCConnection connection, string channel)
        {
            if (tabMain.InvokeRequired)
            {
                RemoveTabDelegate r = new RemoveTabDelegate(RemoveWindow);
                tabMain.Invoke(r, new object[] { connection, channel } );
            }
            else
            {
				TabWindow t = GetWindow(connection, channel, TabWindow.WindowType.Channel);
				if (t != null)
                {
					tabMain.TabPages.RemoveAt(tabMain.TabPages.IndexOf(t));
					return;
                }

                TabWindow c = GetWindow(connection, channel, TabWindow.WindowType.Query);
                if (c != null)
                    tabMain.TabPages.Remove(c);
            }
        }

        /// <summary>
        /// Close All Channels/Query Tabs for specified Connection
        /// </summary>
        /// <param name="connection">Which Connection it is for</param>
        internal void CloseAllWindows(IRCConnection connection)
        {
            for (int i = tabMain.TabPages.Count; i > 1; i--)
            {
                if (((TabWindow)tabMain.TabPages[i - 1]).Connection == connection)
                    tabMain.TabPages.RemoveAt(i-1);
            }

        }

        private string GetMessageFormat(string MessageName)
        {
            foreach (ServerMessageFormatItem msg in iceChatMessages.MessageSettings)
            {
                if (msg.MessageName.ToLower() == MessageName.ToLower())
                {
                    return msg.FormattedMessage;
                }
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
        private void TabMainSelectedIndexChanged(object sender, EventArgs e)
        {
            //update the nicklist accordingly
            if (CurrentWindowType != TabWindow.WindowType.Console)
            {
                if (CurrentWindow != null)
                {
                    TabWindow t = CurrentWindow;
                    nickList.RefreshList(t);
                    inputPanel.CurrentConnection = t.Connection;
                    
                    if (CurrentWindowType == TabWindow.WindowType.Channel)
                        StatusText(t.Connection.ServerSetting.NickName + " in channel " + t.WindowName + " [" + t.ChannelModes + "] {" + t.Connection.ServerSetting.RealServerName + "}");
                    else if (CurrentWindowType == TabWindow.WindowType.Query)
                        StatusText(t.Connection.ServerSetting.NickName + " in private chat with " + t.WindowName + " {" + t.Connection.ServerSetting.RealServerName + "}");
                    
                    CurrentWindow.LastMessageType = ServerMessageType.Default;
                    t = null;
                }
            }
            else
            {
                //make sure the 1st tab is not selected
                nickList.RefreshList();
                nickList.Header = "Console";
                if (((ConsoleTabWindow)tabMain.TabPages[0]).ConsoleTab.SelectedIndex != 0)
                {
                    inputPanel.CurrentConnection = ((ConsoleTabWindow)tabMain.TabPages[0]).CurrentConnection;
                    if (inputPanel.CurrentConnection.IsConnected)
                        StatusText(inputPanel.CurrentConnection.ServerSetting.NickName + " connected to " + inputPanel.CurrentConnection.ServerSetting.RealServerName);
                    else
                        StatusText(inputPanel.CurrentConnection.ServerSetting.NickName + " disconnected (" + inputPanel.CurrentConnection.ServerSetting.ServerName + ")");
                }
                else
                {
                    inputPanel.CurrentConnection = null;
                    StatusText("Welcome to IceChat 2009");
                }
            }
            
            inputPanel.FocusTextBox();

        }
        /// <summary>
        /// Closes the Tab selected
        /// </summary>
        /// <param name="tab">Which tab to Close</param>        
        private void TabMainCloseTab(int tab)
        {
            if (((TabWindow)tabMain.TabPages[tab]).WindowStyle == TabWindow.WindowType.Channel)
            {
                foreach (IRCConnection c in serverTree.ServerConnections.Values)
                {
                    if (c == ((TabWindow)tabMain.TabPages[tab]).Connection)
                    {
                        //check if connected
                        if (c.IsConnected)
                            ParseOutGoingCommand(c, "/part " + ((TabWindow)tabMain.TabPages[tab]).WindowName);
                        else
                            RemoveWindow(c, ((TabWindow)tabMain.TabPages[tab]).WindowName);

                        return;
                    }
                }
            }
            else if (((TabWindow)tabMain.TabPages[tab]).WindowStyle == TabWindow.WindowType.Query)
            {
                tabMain.TabPages.Remove(tabMain.TabPages[tab]);
                return;
            }
            else if (((TabWindow)tabMain.TabPages[tab]).WindowStyle == TabWindow.WindowType.Debug)
            {
                tabMain.TabPages.RemoveAt(tabMain.TabPages.IndexOf(tabMain.TabPages[tab]));
                return;
            }

        }


        /// <summary>
        /// Checks for a Middle Mouse Click to Close a Tab Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabMainMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                //make sure its not the Console window
                for (int i = 1; i < tabMain.TabPages.Count; i++)
                {
                    if (tabMain.GetTabRect(i).Contains(e.Location))
                    {                        
                        if (((TabWindow)tabMain.TabPages[i]).WindowStyle == TabWindow.WindowType.Channel)
                        {
                            foreach (IRCConnection c in serverTree.ServerConnections.Values)
                            {
                                if (c == ((TabWindow)tabMain.TabPages[i]).Connection)
                                {
                                    //check if connected
                                    if (c.IsConnected)
                                        ParseOutGoingCommand(c, "/part " + ((TabWindow)tabMain.TabPages[i]).WindowName);
                                    else
                                        RemoveWindow(c, ((TabWindow)tabMain.TabPages[i]).WindowName);
                                    
                                    return;
                                }
                            }
                        }
                        else if (((TabWindow)tabMain.TabPages[i]).WindowStyle == TabWindow.WindowType.Query)
                        {
                            tabMain.TabPages.Remove(tabMain.TabPages[i]);                             
                            return;
                        }
                        else if (((TabWindow)tabMain.TabPages[i]).WindowStyle == TabWindow.WindowType.Debug)
                        {
					        tabMain.TabPages.RemoveAt(tabMain.TabPages.IndexOf(tabMain.TabPages[i]));
                            return;
                        }
                    }
                }
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
            if (data.StartsWith("//"))
            {
                //parse out identifiers
                ParseOutGoingCommand(connection, ParseIdentifiers(connection, data, data));
                return;
            }
            
            if (data.StartsWith("/"))
            {
                //System.Diagnostics.Debug.WriteLine(data);
                
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
                            ParseOutGoingCommand(connection, ParseIdentifierValue(a.Command[0], data));
                        else
                        {
                            //it is a multulined alias, run multiple commands
                            foreach (string c in a.Command)
                            {
                                ParseOutGoingCommand(connection, ParseIdentifierValue(c, data));
                            }
                        }
                        return;
                    }
                }

                switch (command)
                {
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
                                TabWindow t = GetWindow(connection, data, TabWindow.WindowType.Channel);
                                if (t != null)
                                {
                                    FormChannelInfo fci = new FormChannelInfo(t);
                                    SendData(connection, "MODE " + t.WindowName + " +b");
                                    //check if mode (e) exists for Exception List
                                    if (connection.ServerSetting.ChannelModeParams.Contains("e"))
                                        SendData(connection, "MODE " + t.WindowName + " +e");
                                    fci.ShowDialog(this);
                                }
                            }
                            else
                            {
                                //check if current window is channel
                                if (CurrentWindowType == TabWindow.WindowType.Channel)
                                {
                                    FormChannelInfo fci = new FormChannelInfo(CurrentWindow);
                                    SendData(connection, "MODE " + CurrentWindow.WindowName + " +b");
                                    //check if mode (e) exists for Exception List
                                    if (connection.ServerSetting.ChannelModeParams.Contains("e"))
                                        SendData(connection, "MODE " + CurrentWindow.WindowName + " +e");
                                    fci.ShowDialog(this);
                                }
                            }
                        }
                        break;                    
                    case "/ame":    //me command for all channels
                        if (connection != null && data.Length > 0)
                        {
                            foreach (TabWindow t in FormMain.Instance.TabMain.WindowTabs)
                            {
                                if (t.WindowStyle == TabWindow.WindowType.Channel)
                                {
                                    if (t.Connection == connection)
                                    {
                                        SendData(connection, "PRIVMSG " + t.WindowName + " :ACTION " + data + "");
                                        string msg = GetMessageFormat("Self Channel Action");
                                        msg = msg.Replace("$nick", t.Connection.ServerSetting.NickName).Replace("$channel", t.WindowName);
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
                        foreach (string chan in connection.ServerSetting.AutoJoinChannels)
                        {
                            if (!chan.StartsWith(";"))
                                SendData(connection, "JOIN " + chan);
                        }

                        break;
                    case "/autoperform":
                        foreach (string ap in connection.ServerSetting.AutoPerform)
                        {
                            string autoCommand = ap.Replace("\r", String.Empty);
                            if (!autoCommand.StartsWith(";"))
                                ParseOutGoingCommand(connection, autoCommand);
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
                            if (CurrentWindowType != TabWindow.WindowType.Console)
                                CurrentWindow.TextWindow.ClearTextWindow();
                            else
                            {
                                //find the current console tab window
                                ((ConsoleTabWindow)TabMain.TabPages[0]).CurrentWindow().ClearTextWindow();
                            }
                        }
                        else
                        {
                            //find a match
                            if (data == "Console")
                            {
                                ((ConsoleTabWindow)TabMain.TabPages[0]).CurrentWindow().ClearTextWindow();
                                return;
                            }
                            else if (data.ToLower() == "all console")
                            {
                                //clear all the console windows and channel/queries
                                foreach (ConsoleTab c in ((ConsoleTabWindow)TabMain.TabPages[0]).ConsoleTab.TabPages)
                                    ((TextWindow)c.Controls[0]).ClearTextWindow();

                            }
                            TabWindow t = GetWindow(connection, data, TabWindow.WindowType.Channel);
                            if (t != null)
                                t.TextWindow.ClearTextWindow();
                            else
                            {
                                TabWindow q = GetWindow(connection, data, TabWindow.WindowType.Query);
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
                            TabWindow t = GetWindow(connection, channel, TabWindow.WindowType.Channel);
                            if (t != null)
                            {
                                SendData(connection, "PRIVMSG " + t.WindowName + " :ACTION " + message + "");
                                string msg = GetMessageFormat("Self Channel Action");
                                msg = msg.Replace("$nick", inputPanel.CurrentConnection.ServerSetting.NickName).Replace("$channel", t.WindowName);
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
                                catch(Exception)
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
                            if (CurrentWindowType == TabWindow.WindowType.Channel || CurrentWindowType == TabWindow.WindowType.Query)
                            {
                                string msg = GetMessageFormat("User Echo");
                                msg = msg.Replace("$message", data);

                                CurrentWindow.TextWindow.AppendText(msg, 1);
                                CurrentWindow.LastMessageType = ServerMessageType.Other;
                            }
                            else if (CurrentWindowType == TabWindow.WindowType.Console)
                            {
                                string msg = GetMessageFormat("User Echo");
                                msg = msg.Replace("$message", data);

                                ((ConsoleTabWindow)TabMain.TabPages[0]).CurrentWindow().AppendText(msg, 1);
                            }
                        }
                        break;
                    case"/google":
                        if (data.Length > 0)
                            System.Diagnostics.Process.Start("http://www.google.com/search?q=" + data);
                        else
                            System.Diagnostics.Process.Start("http://www.google.com");
                        break;
                    case "/hop":
                        if (connection != null && data.Length == 0)
                        {
                            if (CurrentWindowType == TabWindow.WindowType.Channel)
                            {
                                temp = CurrentWindow.WindowName;
                                SendData(connection, "PART " + temp);
                                SendData(connection, "JOIN " + temp);
                            }
                        }
                        else
                        {
                            TabWindow t = GetWindow(connection, data, TabWindow.WindowType.Channel);
                            if (t != null)
                            {
                                SendData(connection, "PART " + t.WindowName);
                                SendData(connection, "JOIN " + t.WindowName);
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
                                    SendData(connection, "KICK " + temp + " " + data + " :"  + msg);
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
                            if (CurrentWindowType == TabWindow.WindowType.Channel || CurrentWindowType == TabWindow.WindowType.Query)
                            {
                                SendData(connection, "PRIVMSG " + CurrentWindow.WindowName + " :ACTION " + data + "");
                                string msg = GetMessageFormat("Self Channel Action");
                                msg = msg.Replace("$nick", inputPanel.CurrentConnection.ServerSetting.NickName).Replace("$channel", CurrentWindow.WindowName);                                
                                msg = msg.Replace("$message", data);

                                CurrentWindow.TextWindow.AppendText(msg, 1);
                                CurrentWindow.TextWindow.ScrollToBottom();
                                CurrentWindow.LastMessageType = ServerMessageType.Action;
                            }
                        }
                        break;
                    case "/mode":
                        if (connection!= null && data.Length > 0)
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
                                string color = msg.Substring(0,6);
                                int result;
                                if (Int32.TryParse(msg.Substring(6, 1), out result))
                                    color += msg.Substring(6,1);
                                
                                msg = color + "*" + nick + "* " + data.Substring(data.IndexOf(' ') + 1); ;
                            }
                            
                            if (CurrentWindowType == TabWindow.WindowType.Channel || CurrentWindowType == TabWindow.WindowType.Query)
                                CurrentWindow.TextWindow.AppendText(msg, 1);
                            else if (CurrentWindowType == TabWindow.WindowType.Console)
                                ((ConsoleTabWindow)TabMain.TabPages[0]).CurrentWindow().AppendText(msg, 1);

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
                            TabWindow q = GetWindow(connection, data, TabWindow.WindowType.Query);
                            if (q != null)
                            {
                                RemoveWindow(connection, q.WindowName);
                                return;
                            }
                            else if (CurrentWindowType == TabWindow.WindowType.Query)
                            {
                                RemoveWindow(connection, CurrentWindow.WindowName);
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
                                    if (CurrentWindowType == TabWindow.WindowType.Channel)
                                    {
                                        SendData(connection, "PART " + CurrentWindow.WindowName + " :" + data);
                                        RemoveWindow(connection, CurrentWindow.WindowName);
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
                                    if (CurrentWindowType == TabWindow.WindowType.Channel)
                                    {
                                        SendData(connection, "PART " + CurrentWindow.WindowName + " :" + data);
                                        RemoveWindow(connection, CurrentWindow.WindowName);
                                    }
                                }                                   
                            }
                        }
                        else if (connection != null)
                        {
                            //check if current window is channel
                            if (CurrentWindowType == TabWindow.WindowType.Channel)
                            {
                                SendData(connection, "PART " + CurrentWindow.WindowName);
                                RemoveWindow(connection, CurrentWindow.WindowName);
                            }
                            else if (CurrentWindowType == TabWindow.WindowType.Query)
                            {
                                RemoveWindow(connection, CurrentWindow.WindowName);
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
                            if (CurrentWindowType == TabWindow.WindowType.Channel)
                            {
                                SendData(connection, "PRIVMSG " + CurrentWindow.WindowName + " :" + data);

                                string msg = GetMessageFormat("Self Channel Message");
                                string nick = inputPanel.CurrentConnection.ServerSetting.NickName;

                                msg = msg.Replace("$nick", nick).Replace("$channel", CurrentWindow.WindowName);

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
                            else if (CurrentWindowType == TabWindow.WindowType.Query)
                            {
                                SendData(connection, "PRIVMSG " + CurrentWindow.WindowName + " :" + data);

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
                                s.ID = r.Next(10000,99999);
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
                                if (CurrentWindowType == TabWindow.WindowType.Channel)
                                    SendData(connection, "TOPIC :" + CurrentWindow.WindowName);
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
                                    TabWindow t = GetWindow(connection, word, TabWindow.WindowType.Channel);
                                    if (t != null)
                                    {
                                        if (data.IndexOf(' ') > -1)
                                            SendData(connection, "TOPIC " + t.WindowName + " :" + data.Substring(data.IndexOf(' ') + 1));
                                        else
                                            SendData(connection, "TOPIC :" + t.WindowName);
                                    }
                                }
                                else
                                {
                                    if (CurrentWindowType == TabWindow.WindowType.Channel)
                                        SendData(connection, "TOPIC " + CurrentWindow.WindowName + " :" + data);
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
                            SendData(connection,"WHOIS " + data);
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
                        if (CurrentWindowType == TabWindow.WindowType.Channel)
                        {
                            SendData(connection, "PRIVMSG " + CurrentWindow.WindowName + " :" + data);
                            string msg = GetMessageFormat("Self Channel Message");
                            string nick = inputPanel.CurrentConnection.ServerSetting.NickName;
                            msg = msg.Replace("$nick", nick).Replace("$channel", CurrentWindow.WindowName);

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
                        else if (CurrentWindowType == TabWindow.WindowType.Query)
                        {
                            SendData(connection, "PRIVMSG " + CurrentWindow.WindowName + " :" + data);

                            string msg = GetMessageFormat("Self Private Message");
                            msg = msg.Replace("$nick", inputPanel.CurrentConnection.ServerSetting.NickName).Replace("$message", data);

                            CurrentWindow.TextWindow.AppendText(msg, 1);
                            CurrentWindow.LastMessageType = ServerMessageType.Message;

                        }
                        else if (CurrentWindowType == TabWindow.WindowType.Console)
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
                    case "$uptime":
                        int systemUpTime = System.Environment.TickCount / 1000;
                        TimeSpan ts = TimeSpan.FromSeconds(systemUpTime);
                        string d = ts.Hours + " hours " + ts.Minutes + " minutes " + ts.Seconds + " seconds";
                        if (ts.Days > 0)
                            d = ts.Days + " days " + d;
                        data = data.Replace(m.Value, d);
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
            //parse the initial identifiers
            data = ParseIdentifier(connection, data);
            
            //parse out the $1,$2.. identifiers
            data = ParseIdentifierValue(data, dataPassed);

            //split up the data into words
            string[] parsedData = data.Split(' ');
            
            //the data that was passed for parsing identifiers
            string[] passedData = dataPassed.Split(' ');
            
            //will hold the updates message/data after identifiers are parsed
            string[] changedData = data.Split(' ');
            
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
                                        TabWindow t = GetWindow(connection, channel, TabWindow.WindowType.Channel);
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
                                                switch (prop)
                                                {
                                                    case "host":
                                                        //if (u.Host.IndexOf('@') > -1)
                                                        //    changedData[count] = u.Host.Substring(u.Host.IndexOf('@'));
                                                        break;
                                                    case "ident":
                                                        //if (u.Host.IndexOf('@') > -1)
                                                        //    changedData[count] = u.Host.Substring(0,u.Host.IndexOf('@'));
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
                                    TabWindow t = GetWindow(connection, channel, TabWindow.WindowType.Channel);
                                    if (t != null)
                                    {
                                        if (prop.Length == 0)
                                        {
                                            //replace with channel name
                                            changedData[count] = t.WindowName;
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
            return String.Join(" ",changedData);
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
            foreach (ConsoleTab c in ((ConsoleTabWindow)tabMain.TabPages[0]).ConsoleTab.TabPages)
            {
                ((TextWindow)c.Controls[0]).Font = new Font(iceChatFonts.FontSettings[0].FontName, iceChatFonts.FontSettings[0].FontSize);
            }
            
            //do all the Channel and Query Tabs Windows
            foreach (TabWindow t in tabMain.WindowTabs)
            {
                if (t.WindowStyle == TabWindow.WindowType.Channel)
                    t.TextWindow.Font = new Font(iceChatFonts.FontSettings[1].FontName, iceChatFonts.FontSettings[1].FontSize);

                if (t.WindowStyle == TabWindow.WindowType.Query)
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

            this.nickList.Invalidate();
            this.serverTree.Invalidate();
            this.tabMain.Invalidate();

            //update all the console windows
            foreach (ConsoleTab c in ((ConsoleTabWindow)tabMain.TabPages[0]).ConsoleTab.TabPages)
            {
                ((TextWindow)c.Controls[0]).IRCBackColor = iceChatColors.ConsoleBackColor;
            }

            //update all the Channel and Query Tabs Windows
            foreach (TabWindow t in tabMain.WindowTabs)
            {
                if (t.WindowStyle == TabWindow.WindowType.Channel)
                    t.TextWindow.IRCBackColor = iceChatColors.ChannelBackColor;

                if (t.WindowStyle == TabWindow.WindowType.Query)
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
                ParseOutGoingCommand(FormMain.Instance.InputPanel.CurrentConnection, "/away");
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
            if (GetWindow(null, "Debug", TabWindow.WindowType.Debug) == null)
                AddWindow(null, "Debug", TabWindow.WindowType.Debug);
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
                        System.Diagnostics.Debug.WriteLine(ex.Message);
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
                        System.Diagnostics.Debug.WriteLine(ex.Message);
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


    }
}