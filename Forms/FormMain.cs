/******************************************************************************\
 * IceChat 2009 Internet Relay Chat Client
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
        public static FormMain Instance;

        private string optionsFile;
        private string messagesFile;
        private string fontsFile;
        private string colorsFile;
        private string soundsFile; 
        private string favoriteChannelsFile;
        private string serversFile;
        private string aliasesFile;
        private string popupsFile;
        private string highlitesFile;

        private string currentFolder;
        private string logsFolder;
        private string pluginsFolder;
        private string emoticonsFile;
        private string scriptsFolder;
        private string soundsFolder;
        private string picturesFolder;

        private List<LanguageItem> languageFiles;
        private LanguageItem currentLanguageFile;

        private StreamWriter errorFile;

        private IceChatOptions iceChatOptions;
        private IceChatColors iceChatColors;
        private IceChatMessageFormat iceChatMessages;
        private IceChatFontSetting iceChatFonts;
        private IceChatAliases iceChatAliases;
        private IceChatPopupMenus iceChatPopups;
        private IceChatEmoticon iceChatEmoticons;
        private IceChatLanguage iceChatLanguage;
        private IceChatSounds iceChatSounds;
        //private System.Threading.Mutex mutex;

        private ArrayList loadedPlugins;
        private ArrayList loadedScripts;

        private IdentServer identServer;
        
        private TabPage nickListTab;
        private TabPage serverListTab;
        private TabPage channelListTab;
        private TabPage buddyListTab;

        private delegate void AddWindowDelegate(IRCConnection connection, string windowName, IceTabPage.WindowType windowType);
        private delegate void RemoveTabDelegate(IRCConnection connection, string channel, IceTabPage.WindowType windowType);
        private delegate int GetSelectedTabDelegate();

        private delegate void StatusTextDelegate(string data);
        private delegate void CurrentWindowDelegate(string data, int color);
        private delegate void WindowMessageDelegate(IRCConnection connection, string name, string data, int color, bool scrollToBottom);
        private delegate void CurrentWindowMessageDelegate(IRCConnection connection, string data, int color, bool scrollToBottom);

        private System.Media.SoundPlayer player;

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

        public FormMain(string[] args)
        {
            FormMain.Instance = this;

            player = new System.Media.SoundPlayer();

            bool forceCurrentFolder = false;

            if (args.Length > 0)
            {
                string prevArg = "";
                foreach (string arg in args)
                {
                    if (prevArg.Length == 0)
                        prevArg = arg;
                    else
                    {
                        switch (prevArg.ToLower())
                        {
                            case "-profile":
                                currentFolder = arg;
                                //check if the folder exists, ir not, create it
                                if (!Directory.Exists(currentFolder))
                                    Directory.CreateDirectory(currentFolder);
                                forceCurrentFolder = true;
                                break;
                        }
                        
                        prevArg = "";
                    }
                }
            }
            
            //mutex = new System.Threading.Mutex(true, "IceChatMutex");

            #region Settings Files 
            
            //check if the xml settings files exist in current folder
            if (currentFolder == null)
                currentFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            if (!File.Exists(currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatServer.xml") && !forceCurrentFolder)
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
            soundsFile = currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatSounds.xml";
            favoriteChannelsFile = currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatChannels.xml";
            aliasesFile = currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatAliases.xml";
            popupsFile = currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatPopups.xml";
            highlitesFile = currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatHighLites.xml";
            emoticonsFile = currentFolder + System.IO.Path.DirectorySeparatorChar + "Emoticons" + System.IO.Path.DirectorySeparatorChar + "IceChatEmoticons.xml";

            logsFolder = currentFolder + System.IO.Path.DirectorySeparatorChar + "Logs";
            pluginsFolder = currentFolder + System.IO.Path.DirectorySeparatorChar + "Plugins";
            scriptsFolder = currentFolder + System.IO.Path.DirectorySeparatorChar + "Scripts";
            soundsFolder = currentFolder + System.IO.Path.DirectorySeparatorChar + "Sounds";
            picturesFolder = currentFolder + System.IO.Path.DirectorySeparatorChar + "Pictures";

            if (!Directory.Exists(pluginsFolder))
                Directory.CreateDirectory(pluginsFolder);

            if (!Directory.Exists(scriptsFolder))
                Directory.CreateDirectory(scriptsFolder);

            if (!Directory.Exists(soundsFolder))
                Directory.CreateDirectory(soundsFolder);

            if (!Directory.Exists(picturesFolder))
                Directory.CreateDirectory(picturesFolder);


            #endregion

            languageFiles = new List<LanguageItem>();
            currentLanguageFile = new LanguageItem();
            languageFiles.Add(currentLanguageFile);     // default language English

            DirectoryInfo languageDirectory = null;

            languageDirectory = new DirectoryInfo(currentFolder + System.IO.Path.DirectorySeparatorChar + "Languages");
            if (!Directory.Exists(currentFolder + System.IO.Path.DirectorySeparatorChar + "Languages"))
                Directory.CreateDirectory(currentFolder + System.IO.Path.DirectorySeparatorChar + "Languages");
            
            if (languageDirectory != null)
            {
                // scan the language directory for xml files and make LanguageItems for each file
                FileInfo[] langFiles = languageDirectory.GetFiles("*.xml");
                foreach (FileInfo fi in langFiles)
                {
                    string langFile = languageDirectory.FullName + System.IO.Path.DirectorySeparatorChar + fi.Name;
                    LanguageItem languageItem = LoadLanguageItem(langFile);
                    if (languageItem != null) languageFiles.Add(languageItem);
                }
            }

            LoadOptions();
            LoadColors();
            LoadSounds();

            // use the language saved in options if availlable,
            // if not (e.g. user deleted xml file) default is used
            foreach (LanguageItem li in languageFiles)
            {
                if (li.LanguageName == iceChatOptions.Language)
                {
                    currentLanguageFile = li;
                    break;
                }
            }
            LoadLanguage(); // The language class MUST be loaded before any GUI component is created

            //check if we have any servers/settings saved, if not, load firstrun
            if (!File.Exists(serversFile))
            {
                FormFirstRun firstRun = new FormFirstRun(currentFolder);
                firstRun.ShowDialog(this);
            }
            
            InitializeComponent();
            //load icons from Embedded Resources
            this.toolStripQuickConnect.Image = StaticMethods.LoadResourceImage("quick.png");
            this.toolStripSettings.Image = StaticMethods.LoadResourceImage("settings.png");
            this.toolStripColors.Image = StaticMethods.LoadResourceImage("colors.png");
            this.toolStripEditor.Image = StaticMethods.LoadResourceImage("editor.png");
            this.toolStripAway.Image = StaticMethods.LoadResourceImage("away.png");
            this.toolStripSystemTray.Image = StaticMethods.LoadResourceImage("system-tray.png");
            
            this.minimizeToTrayToolStripMenuItem.Image = StaticMethods.LoadResourceImage("icechat2009.ico");
            this.debugWindowToolStripMenuItem.Image = StaticMethods.LoadResourceImage("window.png");
            this.exitToolStripMenuItem.Image = StaticMethods.LoadResourceImage("disconected.png");
            this.iceChatSettingsToolStripMenuItem.Image = StaticMethods.LoadResourceImage("settings.png");
            this.iceChatColorsToolStripMenuItem.Image = StaticMethods.LoadResourceImage("colors.png");
            this.iceChatEditorToolStripMenuItem.Image = StaticMethods.LoadResourceImage("editor.png");
            this.codePlexPageToolStripMenuItem.Image = StaticMethods.LoadResourceImage("codeplex.ico");
            this.forumsToolStripMenuItem.Image = StaticMethods.LoadResourceImage("smf.ico");
            this.facebookFanPageToolStripMenuItem.Image = StaticMethods.LoadResourceImage("facebook.png");

            this.notifyIcon.Icon = System.Drawing.Icon.FromHandle(StaticMethods.LoadResourceImage("icechat2009.ico").GetHicon());
            this.Icon = System.Drawing.Icon.FromHandle(StaticMethods.LoadResourceImage("icechat2009.ico").GetHicon());

            serverListToolStripMenuItem.Checked = iceChatOptions.ShowServerTree;
            panelDockLeft.Visible = serverListToolStripMenuItem.Checked;
            splitterLeft.Visible = serverListToolStripMenuItem.Checked;

            nickListToolStripMenuItem.Checked = iceChatOptions.ShowNickList;
            panelDockRight.Visible = nickListToolStripMenuItem.Checked;
            splitterRight.Visible = nickListToolStripMenuItem.Checked;

            statusBarToolStripMenuItem.Checked = iceChatOptions.ShowStatusBar;
            statusStripMain.Visible = statusBarToolStripMenuItem.Checked;

            toolBarToolStripMenuItem.Checked = iceChatOptions.ShowToolBar;
            toolStripMain.Visible = toolBarToolStripMenuItem.Checked;

            channelBarToolStripMenuItem.Checked = iceChatOptions.ShowTabBar;
            mainTabControl.ShowTabs = iceChatOptions.ShowTabBar;

            serverTree = new ServerTree();
            serverTree.Dock = DockStyle.Fill;
            
            this.Text = IceChat.Properties.Settings.Default.ProgramID + " " + IceChat.Properties.Settings.Default.Version + " - January 2 2011";
            
            if (!Directory.Exists(logsFolder))
                Directory.CreateDirectory(logsFolder);

            try
            {
                errorFile = new StreamWriter(logsFolder + System.IO.Path.DirectorySeparatorChar + "errors.log");
            }
            catch (IOException io)
            {
                System.Diagnostics.Debug.WriteLine("Can not create errors.log:" + io.Message);
            }

            if (!iceChatOptions.TimeStamp.EndsWith(" "))
                iceChatOptions.TimeStamp += " ";

            if (iceChatOptions.WindowSize != null)
            {
                if (iceChatOptions.WindowSize.Width != 0)
                    this.Size = iceChatOptions.WindowSize;
                else
                {
                    Rectangle r = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
                    this.Width = r.Width;
                    this.Height = r.Height;
                }
            }
            
            if (iceChatOptions.WindowLocation != null)
                this.Location = iceChatOptions.WindowLocation;

            statusStripMain.Visible = iceChatOptions.ShowStatusBar;
            inputPanel.ShowColorPicker = iceChatOptions.ShowColorPicker;
            inputPanel.ShowEmoticonPicker = iceChatOptions.ShowEmoticonPicker;
            inputPanel.ShowSearchPanel = false;

            LoadAliases();
            LoadPopups();
            LoadEmoticons();
            LoadMessageFormat();
            LoadFonts();

            channelList = new ChannelList();            
            channelList.Dock = DockStyle.Fill;
            buddyList = new BuddyList();
            buddyList.Dock = DockStyle.Fill;

            toolStripMain.BackColor = IrcColor.colors[iceChatColors.ToolbarBackColor];
            menuMainStrip.BackColor = IrcColor.colors[iceChatColors.MenubarBackColor];
            statusStripMain.BackColor = IrcColor.colors[iceChatColors.StatusbarBackColor];
            toolStripStatus.ForeColor = IrcColor.colors[iceChatColors.StatusbarForeColor];
            inputPanel.SetInputBoxColors();
            channelList.SetListColors();
            buddyList.SetListColors();

            nickList = new NickList();            
            nickList.Header = iceChatLanguage.consoleTabTitle;
            nickList.Dock = DockStyle.Fill;

            serverListTab = new TabPage("Favorite Servers");
            Panel serverPanel = new Panel();
            serverPanel.Dock = DockStyle.Fill;
            serverPanel.Controls.Add(serverTree);
            serverListTab.Controls.Add(serverPanel);            
            this.panelDockLeft.TabControl.TabPages.Add(serverListTab);

            nickListTab = new TabPage("Nick List");
            Panel nickPanel = new Panel();
            nickPanel.Dock = DockStyle.Fill;
            nickPanel.Controls.Add(nickList);            
            nickListTab.Controls.Add(nickPanel);
            this.panelDockRight.TabControl.TabPages.Add(nickListTab);

            channelListTab = new TabPage("Favorite Channels");
            Panel channelPanel = new Panel();
            channelPanel.Dock = DockStyle.Fill;
            channelPanel.Controls.Add(channelList);
            channelListTab.Controls.Add(channelPanel);
            this.panelDockRight.TabControl.TabPages.Add(channelListTab);

            buddyListTab = new TabPage("Buddy List");
            Panel buddyPanel = new Panel();
            buddyPanel.Dock = DockStyle.Fill;
            buddyPanel.Controls.Add(buddyList);
            buddyListTab.Controls.Add(buddyPanel);
            this.panelDockRight.TabControl.TabPages.Add(buddyListTab);

            panelDockLeft.Width = iceChatOptions.LeftPanelWidth;
            panelDockLeft.TabControl.Alignment = TabAlignment.Left;
            panelDockRight.Width = iceChatOptions.RightPanelWidth;
            panelDockRight.TabControl.Alignment = TabAlignment.Right;

            nickList.Font = new Font(iceChatFonts.FontSettings[3].FontName, iceChatFonts.FontSettings[3].FontSize);
            serverTree.Font = new Font(iceChatFonts.FontSettings[4].FontName, iceChatFonts.FontSettings[4].FontSize);

            inputPanel.OnCommand +=new InputPanel.OnCommandDelegate(inputPanel_OnCommand);
            inputPanel.InputBoxFont = new Font(iceChatFonts.FontSettings[5].FontName, iceChatFonts.FontSettings[5].FontSize);

            mainTabControl.SelectedIndexChanged += new IceTabControl.TabEventHandler(TabSelectedIndexChanged);
            mainTabControl.OnTabClosed += new IceTabControl.TabClosedDelegate(mainTabControl_OnTabClosed);

            panelDockLeft.Initialize();
            panelDockRight.Initialize();

            menuMainStrip.Font = new Font(iceChatFonts.FontSettings[7].FontName, iceChatFonts.FontSettings[7].FontSize);

            serverTree.NewServerConnection += new NewServerConnectionDelegate(NewServerConnection);
            serverTree.SaveDefault += new ServerTree.SaveDefaultDelegate(OnDefaultServerSettings);
            CreateDefaultConsoleWindow();

            this.FormClosing += new FormClosingEventHandler(FormMainClosing);

            if (iceChatOptions.IdentServer)
                identServer = new IdentServer();

            loadedPlugins = new ArrayList();
            loadedScripts = new ArrayList();

            //load any plugin addons
            LoadPlugins();
            LoadScripts();

            //fire the event that the program has fully loaded
            foreach (IPluginIceChat ipc in FormMain.Instance.IceChatPlugins)
            {
                ipc.MainProgramLoaded();
            }

            if (iceChatLanguage.LanguageName != "English") ApplyLanguage(); // ApplyLanguage can first be called after all child controls are created
        
            //auto start any Auto Connect Servers
            foreach (ServerSetting s in serverTree.ServersCollection.listServers)
            {
                if (s.AutoStart)
                    NewServerConnection(s);
            }
            
            WindowMessage(null, "Console", "Current Data Folder: " + currentFolder, 4, true);
        }

        
        /// <summary>
        /// Save Default Server Settings
        /// </summary>
        private void OnDefaultServerSettings()
        {
            SaveOptions();
        }

        #region Load Language File

        private LanguageItem LoadLanguageItem(string languageFileName)
        {
            if (File.Exists(languageFileName))
            {
                LanguageItem languageItem = null;
                XmlSerializer deserializer = new XmlSerializer(typeof(LanguageItem));
                TextReader textReader = new StreamReader(languageFileName);
                try
                {
                    languageItem = (LanguageItem)deserializer.Deserialize(textReader);
                    languageItem.LanguageFile = languageFileName;
               }
                catch
                {
                    languageItem = null;
                }
                finally
                {
                    textReader.Close();
                    textReader.Dispose();
                }
                return languageItem;
            }
            else
            {
                return null;
            }
        }

        private void LoadLanguage()
        {
            if (File.Exists(currentLanguageFile.LanguageFile))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(IceChatLanguage));
                TextReader textReader = new StreamReader(currentLanguageFile.LanguageFile);
                iceChatLanguage = (IceChatLanguage)deserializer.Deserialize(textReader);
                textReader.Close();
                textReader.Dispose();
            }
            else
            {
                iceChatLanguage = new IceChatLanguage();
            }
        }

        private void ApplyLanguage()
        {
            mainToolStripMenuItem.Text = iceChatLanguage.mainToolStripMenuItem;
            minimizeToTrayToolStripMenuItem.Text = iceChatLanguage.minimizeToTrayToolStripMenuItem;
            debugWindowToolStripMenuItem.Text = iceChatLanguage.debugWindowToolStripMenuItem;
            exitToolStripMenuItem.Text = iceChatLanguage.exitToolStripMenuItem;
            optionsToolStripMenuItem.Text = iceChatLanguage.optionsToolStripMenuItem;
            iceChatSettingsToolStripMenuItem.Text = iceChatLanguage.iceChatSettingsToolStripMenuItem;
            iceChatColorsToolStripMenuItem.Text = iceChatLanguage.iceChatColorsToolStripMenuItem;
            iceChatEditorToolStripMenuItem.Text = iceChatLanguage.iceChatEditorToolStripMenuItem;
            pluginsToolStripMenuItem.Text = iceChatLanguage.pluginsToolStripMenuItem;
            viewToolStripMenuItem.Text = iceChatLanguage.viewToolStripMenuItem;
            serverListToolStripMenuItem.Text = iceChatLanguage.serverListToolStripMenuItem;
            nickListToolStripMenuItem.Text = iceChatLanguage.nickListToolStripMenuItem;
            statusBarToolStripMenuItem.Text = iceChatLanguage.statusBarToolStripMenuItem;
            toolBarToolStripMenuItem.Text = iceChatLanguage.toolBarToolStripMenuItem;
            helpToolStripMenuItem.Text = iceChatLanguage.helpToolStripMenuItem;
            codePlexPageToolStripMenuItem.Text = iceChatLanguage.codePlexPageToolStripMenuItem;
            iceChatHomePageToolStripMenuItem.Text = iceChatLanguage.iceChatHomePageToolStripMenuItem;
            forumsToolStripMenuItem.Text = iceChatLanguage.forumsToolStripMenuItem;
            aboutToolStripMenuItem.Text = iceChatLanguage.aboutToolStripMenuItem;
            toolStripQuickConnect.Text = iceChatLanguage.toolStripQuickConnect;
            toolStripSettings.Text = iceChatLanguage.toolStripSettings;
            toolStripColors.Text = iceChatLanguage.toolStripColors;
            toolStripEditor.Text = iceChatLanguage.toolStripEditor;
            toolStripAway.Text = iceChatLanguage.toolStripAway;
            toolStripSystemTray.Text = iceChatLanguage.toolStripSystemTray;
            toolStripStatus.Text = iceChatLanguage.toolStripStatus;
            
            channelListTab.Text = iceChatLanguage.tabPageFaveChannels;
            nickListTab.Text = iceChatLanguage.tabPageNicks;
            serverListTab.Text = iceChatLanguage.serverTreeHeader;

            channelList.ApplyLanguage();
            nickList.ApplyLanguage();
            serverTree.ApplyLanguage();
            inputPanel.ApplyLanguage();

            mainTabControl.Invalidate(); // repaint tabs to apply changes to user drawn tabs
        }

        #endregion

        private void FormMainClosing(object sender, FormClosingEventArgs e)
        {
            //check if there are any connections open
            foreach (IRCConnection c in serverTree.ServerConnections.Values)
            {
                if (c.IsConnected)
                {
                    DialogResult dr = MessageBox.Show("You are connected to a Server(s), are you sure you want to close IceChat?", "Close IceChat", MessageBoxButtons.YesNo);
                    if (e.CloseReason == CloseReason.UserClosing && dr == DialogResult.No)
                    {
                        e.Cancel = true;
                        return;
                    }
                    else
                        break;
                }
            }
            
            if (identServer != null)
            {
                identServer.Stop();
                identServer = null;
            }
            
            for (int i = 0; i < loadedPlugins.Count; i++)
                loadedPlugins.RemoveAt(i);

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
                    if (!panelDockRight.IsDocked)
                        iceChatOptions.RightPanelWidth = panelDockRight.Width;
                    if (!panelDockLeft.IsDocked)
                        iceChatOptions.LeftPanelWidth = panelDockLeft.Width;
                }

                SaveOptions();
            }

            //mutex.ReleaseMutex();

            if (errorFile != null)
            {
                errorFile.Close();
                errorFile.Dispose();
            }
        }
        
        /// <summary>
        /// Play the specified sound file (currently only supports WAV files)
        /// </summary>
        /// <param name="sound"></param>
        internal void PlaySoundFile(string key)
        {
            IceChatSounds.soundEntry sound = IceChatSounds.getSound(key);
            if (sound != null)
            {
                string file = sound.getSoundFile();
                if (file != null && file.Length > 0)
                {
                    try
                    {
                        player.SoundLocation = @file;
                        player.Play();
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Create a Default Tab for showing Welcome Information
        /// </summary>
        private void CreateDefaultConsoleWindow()
        {
            IceTabPage p = new IceTabPage(IceTabPage.WindowType.Console, "Console");
            p.AddConsoleTab(iceChatLanguage.consoleTabWelcome);
            mainTabControl.TabPages.Add(p);

            WindowMessage(null, "Console", "\x00034Welcome to " + Settings.Default.ProgramID + " " + Settings.Default.Version, 1, false);
            WindowMessage(null, "Console", "\x00034** This is a Beta version, not fully functional, not all options are added **", 1, false);
            WindowMessage(null, "Console", "\x00033If you want a fully working version of \x0002IceChat\x0002, visit http://www.icechat.net and download IceChat 7.70", 1, false);
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

        internal IceChatSounds IceChatSounds
        {
            get
            {
                return this.iceChatSounds;
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

        internal IceChatLanguage IceChatLanguage
        {
            get
            {
                return iceChatLanguage;
            }
        }

        internal List<LanguageItem> IceChatLanguageFiles
        {
            get
            {
                return languageFiles;
            }
        }

        internal LanguageItem IceChatCurrentLanguageFile
        {
            get
            {
                return currentLanguageFile;
            }
            set
            {
                if (currentLanguageFile != value)
                {
                    currentLanguageFile = value;
                    LoadLanguage();
                    ApplyLanguage();
                }
           }
        }


        internal string FavoriteChannelsFile
        {
            get
            {
                return favoriteChannelsFile;
            }
        }

        internal BuddyList BuddyList
        {
            get
            {
                return this.buddyList;
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

        internal string PopupsFile
        {
            get
            {
                return popupsFile;
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
                    if (connection != null)
                        if (connection.IsFullyConnected)
                            if (!connection.ServerSetting.DisableSounds)
                                PlaySoundFile("conmsg");
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
                if (CurrentWindowType == IceTabPage.WindowType.ChannelList)
                {
                    //do nothing, send it to the console
                    mainTabControl.GetTabPage("Console").AddText(connection, data, color, false);
                }
                else if (CurrentWindowType != IceTabPage.WindowType.Console)
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
                    if (t.Connection == null && windowType == IceTabPage.WindowType.DCCFile)
                        return t;
                    else if (t.Connection == null && windowType == IceTabPage.WindowType.Debug)
                        return t;
                    else if (t.Connection == connection)
                        return t;
                }
                else if (t.WindowStyle == windowType && windowType == IceTabPage.WindowType.ChannelList)
                {
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
                    else if (CurrentWindow.WindowStyle != IceTabPage.WindowType.ChannelList && CurrentWindow.WindowStyle != IceTabPage.WindowType.DCCFile)
                    {
                        CurrentWindow.TextWindow.AppendText("Error: Not Connected (" + data + ")", 4);
                        CurrentWindow.TextWindow.ScrollToBottom();
                    }
                    else
                    {
                        WindowMessage(connection, "Console", "Error: Not Connected (" + data + ")", 4, true);
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
            c.CtcpReply += new CtcpReplyDelegate(OnCtcpReply);
            c.GenericChannelMessage += new GenericChannelMessageDelegate(OnGenericChannelMessage);
            c.ServerNotice += new ServerNoticeDelegate(OnServerNotice);
            c.ChannelListStart += new ChannelListStartDelegate(OnChannelListStart);
            c.ChannelList += new ChannelListDelegate(OnChannelList);
            c.DCCChat += new DCCChatDelegate(OnDCCChat);
            c.DCCFile += new DCCFileDelegate(OnDCCFile);
            c.DCCPassive += new DCCPassiveDelegate(OnDCCPassive);
            c.UserHostReply += new UserHostReplyDelegate(OnUserHostReply);
            c.IALUserData += new IALUserDataDelegate(OnIALUserData);
            c.IALUserChange += new IALUserChangeDelegate(OnIALUserChange);
            c.IALUserPart += new IALUserPartDelegate(OnIALUserPart);
            c.IALUserQuit += new IALUserQuitDelegate(OnIALUserQuit);

            c.BuddyListRefresh += new BuddyListRefreshDelegate(OnBuddyListRefresh);

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
                IceTabPage page;
                if (windowType == IceTabPage.WindowType.DCCFile)
                    page = new IceTabPageDCCFile(IceTabPage.WindowType.DCCFile, windowName);
                else
                {
                    page = new IceTabPage(windowType, windowName);
                    page.Connection = connection;
                }

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
                if (page.WindowStyle == IceTabPage.WindowType.Channel || page.WindowStyle == IceTabPage.WindowType.Query || page.WindowStyle == IceTabPage.WindowType.DCCChat || page.WindowStyle == IceTabPage.WindowType.DCCFile)
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
                {
                    mainTabControl.SelectTab(mainTabControl.CurrentTab);
                    serverTree.SelectTab(mainTabControl.CurrentTab, false);
                }
                else
                {
                    mainTabControl.SelectTab(page);
                    nickList.CurrentWindow = page;
                    serverTree.SelectTab(page, false);
                }

                if (page.WindowStyle == IceTabPage.WindowType.Query && iceChatOptions.WhoisNewQuery)
                    ParseOutGoingCommand(page.Connection, "/whois " + page.TabCaption);
               
            }
        }
        /// <summary>
        /// Remove a Tab Window from the Main Tab Control
        /// </summary>
        /// <param name="connection">Which Connection it is for</param>
        /// <param name="channel">The Channel/Query Window Name</param>
        internal void RemoveWindow(IRCConnection connection, string windowCaption, IceTabPage.WindowType windowType)
        {
            if (mainTabControl.InvokeRequired)
            {
                RemoveTabDelegate r = new RemoveTabDelegate(RemoveWindow);
                mainTabControl.Invoke(r, new object[] { connection, windowCaption, windowType });
            }
            else
            {
                IceTabPage t = GetWindow(connection, windowCaption, IceTabPage.WindowType.Channel);
				if (t != null)
                {
                    mainTabControl.Controls.Remove(t);
                    return;
                }

                IceTabPage c = GetWindow(connection, windowCaption, IceTabPage.WindowType.Query);
                if (c != null)
                {
                    mainTabControl.Controls.Remove(c);
                    return;
                }

                IceTabPage cl = GetWindow(connection, "", IceTabPage.WindowType.ChannelList);
                if (cl != null)
                {
                    mainTabControl.Controls.Remove(cl);
                    return;
                }
                
                IceTabPage de = GetWindow(null, "Debug", IceTabPage.WindowType.Debug);
                if (de != null)
                    mainTabControl.Controls.Remove(de);

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

        internal string GetMessageFormat(string MessageName)
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
        private void TabSelectedIndexChanged(object sender, TabEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("tab select:");
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
                    else if (CurrentWindowType == IceTabPage.WindowType.DCCChat)
                        StatusText(t.Connection.ServerSetting.NickName + " in DCC chat with " + t.TabCaption + " {" + t.Connection.ServerSetting.RealServerName + "}");
                    
                    CurrentWindow.LastMessageType = ServerMessageType.Default;
                    t = null;
                    
                    if (!e.IsHandled)
                        serverTree.SelectTab(mainTabControl.CurrentTab, false);

                }
            }
            else
            {
                //make sure the 1st tab is not selected
                nickList.RefreshList();
                nickList.Header = iceChatLanguage.consoleTabTitle; 
                if (mainTabControl.GetTabPage("Console").ConsoleTab.SelectedIndex != 0)
                {
                    inputPanel.CurrentConnection = mainTabControl.GetTabPage("Console").CurrentConnection;
                    if (inputPanel.CurrentConnection.IsConnected)
                        StatusText(inputPanel.CurrentConnection.ServerSetting.NickName + " connected to " + inputPanel.CurrentConnection.ServerSetting.RealServerName);
                    else
                        StatusText(inputPanel.CurrentConnection.ServerSetting.NickName + " disconnected (" + inputPanel.CurrentConnection.ServerSetting.ServerName + ")");
                    if (!e.IsHandled)
                        serverTree.SelectTab(mainTabControl.GetTabPage("Console").CurrentConnection.ServerSetting, false);
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
                            RemoveWindow(c, mainTabControl.GetTabPage(nIndex).TabCaption, mainTabControl.GetTabPage(nIndex).WindowStyle);

                        return;
                    }
                }
            }
            else if (mainTabControl.GetTabPage(nIndex).WindowStyle == IceTabPage.WindowType.Query)
            {
                mainTabControl.Controls.Remove(mainTabControl.GetTabPage(nIndex));
            }
            else if (mainTabControl.GetTabPage(nIndex).WindowStyle == IceTabPage.WindowType.ChannelList)
            {
                mainTabControl.Controls.Remove(mainTabControl.GetTabPage(nIndex));
            }
            else if (mainTabControl.GetTabPage(nIndex).WindowStyle == IceTabPage.WindowType.DCCChat)
            {
                mainTabControl.Controls.Remove(mainTabControl.GetTabPage(nIndex));
            }
            else if (mainTabControl.GetTabPage(nIndex).WindowStyle == IceTabPage.WindowType.DCCFile)
            {
                mainTabControl.Controls.Remove(mainTabControl.GetTabPage(nIndex));
            }
            else if (mainTabControl.GetTabPage(nIndex).WindowStyle == IceTabPage.WindowType.Window)
            {
                mainTabControl.Controls.Remove(mainTabControl.GetTabPage(nIndex));
            }
            else if (mainTabControl.GetTabPage(nIndex).WindowStyle == IceTabPage.WindowType.Debug)
            {
                mainTabControl.Controls.Remove(mainTabControl.GetTabPage(nIndex));
            }
        }

        #endregion
        
        #region InputPanel Events

        /// <summary>
        /// Parse out command written in Input Box or sent from Script
        /// </summary>
        /// <param name="connection">Which Connection it is for</param>
        /// <param name="data">The Message to Parse</param>
        public void ParseOutGoingCommand(IRCConnection connection, string data)
        {
            try
            {
                data = data.Replace("&#x3;", ((char)3).ToString());
                
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

                    switch (command.ToLower())
                    {
                        case "/makeexception":
                            throw new Exception("IceChat 2009 Test Exception Error");
                        
                        case "/bg": //change background image for a window(s)
                            if (data.Length > 0)
                            {
                                //bg windowtype imagefile
                                //if imagefile is blank, erase background image
                                string window = data.Split(' ')[0];
                                string file = "";
                                if (data.IndexOf(' ') > -1)
                                    file = data.Substring(window.Length + 1);

                                switch (window.ToLower())
                                {
                                    case "console":
                                        if (CurrentWindowType == IceTabPage.WindowType.Console)
                                        {
                                            if (file.IndexOf(System.IO.Path.DirectorySeparatorChar) > -1)
                                                mainTabControl.GetTabPage("Console").CurrentConsoleWindow().BackGroundImage = file;
                                            else
                                            {
                                                if (File.Exists(picturesFolder + System.IO.Path.DirectorySeparatorChar + file))
                                                    mainTabControl.GetTabPage("Console").CurrentConsoleWindow().BackGroundImage = (picturesFolder + System.IO.Path.DirectorySeparatorChar + file);
                                                else
                                                    mainTabControl.GetTabPage("Console").CurrentConsoleWindow().BackGroundImage = "";
                                            }
                                        }
                                        break;
                                }
                            }
                            break;
                        
                        case "/unloadplugin":
                            if (data.Length > 0)
                            {
                                //get the plugin name, and look for it in the menu items
                                ToolStripMenuItem menuItem = null;
                                foreach (ToolStripMenuItem t in pluginsToolStripMenuItem.DropDownItems)
                                    if (t.ToolTipText.ToLower() == data.ToLower())
                                        menuItem = t;
                                
                                if (menuItem != null)
                                {
                                    //match
                                    IPluginIceChat plugin = (IPluginIceChat)menuItem.Tag;

                                    plugin.OnCommand -= new OutGoingCommandHandler(Plugin_OnCommand);
                                    loadedPlugins.Remove(plugin);
                                    menuItem.Click -= new EventHandler(OnPluginMenuItemClick);
                                    pluginsToolStripMenuItem.DropDownItems.Remove(menuItem);
                                    WindowMessage(null, "Console", "Unloaded Plugin - " + plugin.Name, 4, true);
                                    plugin.Dispose();
                                }
                            }
                            break;                        
                        case "/loadplugin":
                            if (data.Length > 0)
                            {
                                Type ObjType = null;
                                Assembly ass = null;

                                try
                                {
                                    //reload the plugin
                                    ass = Assembly.LoadFile(pluginsFolder + System.IO.Path.DirectorySeparatorChar + data);

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
                                    WriteErrorFile(connection, "ReLoadPlugins Cast:",ex);
                                }
                                try
                                {
                                    // OK Lets create the object as we have the Report Type
                                    if (ObjType != null)
                                    {
                                        IPluginIceChat ipi = (IPluginIceChat)Activator.CreateInstance(ObjType);

                                        ipi.MainForm = this;
                                        ipi.MainMenuStrip = this.MainMenuStrip;
                                        ipi.CurrentFolder = currentFolder;

                                        WindowMessage(null, "Console", "Loaded Plugin - " + ipi.Name + " v" + ipi.Version + " by " + ipi.Author, 4, true);

                                        ToolStripMenuItem t = new ToolStripMenuItem(ipi.Name);
                                        t.Tag = ipi;
                                        t.ToolTipText = data;
                                        t.Click += new EventHandler(OnPluginMenuItemClick);
                                        pluginsToolStripMenuItem.DropDownItems.Add(t);

                                        ipi.OnCommand += new OutGoingCommandHandler(Plugin_OnCommand);
                                        ipi.Initialize();
                                        loadedPlugins.Add(ipi);
                                    }
                                    else
                                    {
                                        System.Diagnostics.Debug.WriteLine("obj type is null:" + data);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    WriteErrorFile(connection, "LoadPlugins",ex);
                                }
                            }                            
                            break;
                        case "/reload":
                            if (data.Length > 0)
                            {
                                switch (data)
                                {
                                    case "alias":
                                    case "aliases":
                                        CurrentWindowMessage(connection, "Aliases file reloaded", 4, true);
                                        LoadAliases();
                                        break;
                                    case "popup":
                                    case "popups":
                                        CurrentWindowMessage(connection, "Popups file reloaded", 4, true);
                                        LoadPopups();
                                        break;
                                    case "emoticon":
                                    case "emoticons":
                                        CurrentWindowMessage(connection, "Emoticons file reloaded", 4, true);
                                        LoadEmoticons();
                                        break;
                                    case "sound":
                                    case "sounds":
                                        CurrentWindowMessage(connection, "Sounds file reloaded", 4, true);
                                        LoadSounds();
                                        break;
                                    case "color":
                                    case "colors":
                                        CurrentWindowMessage(connection, "Colors file reloaded", 4, true);
                                        LoadColors();
                                        toolStripMain.BackColor = IrcColor.colors[iceChatColors.ToolbarBackColor];
                                        menuMainStrip.BackColor = IrcColor.colors[iceChatColors.MenubarBackColor];
                                        statusStripMain.BackColor = IrcColor.colors[iceChatColors.StatusbarBackColor];
                                        toolStripStatus.ForeColor = IrcColor.colors[iceChatColors.StatusbarForeColor];
                                        inputPanel.SetInputBoxColors();
                                        channelList.SetListColors();
                                        buddyList.SetListColors();
                                        break;
                                    case "font":
                                    case "fonts":
                                        CurrentWindowMessage(connection, "Fonts file reloaded", 4, true);
                                        LoadFonts();
                                        nickList.Font = new Font(iceChatFonts.FontSettings[3].FontName, iceChatFonts.FontSettings[3].FontSize);
                                        serverTree.Font = new Font(iceChatFonts.FontSettings[4].FontName, iceChatFonts.FontSettings[4].FontSize);
                                        menuMainStrip.Font = new Font(iceChatFonts.FontSettings[7].FontName, iceChatFonts.FontSettings[7].FontSize);
                                        break;
                                }
                            }                            
                            break;                        
                        case "/reloadplugin":
                            if (data.Length > 0)
                            {
                                //get the plugin name, and look for it in the menu items
                                ToolStripMenuItem menuItem = null;
                                foreach (ToolStripMenuItem t in pluginsToolStripMenuItem.DropDownItems)
                                    if (t.ToolTipText.ToLower() == data.ToLower())
                                        menuItem = t;

                                if (menuItem != null)
                                {
                                    //match
                                                                       
                                    IPluginIceChat plugin = (IPluginIceChat)menuItem.Tag;

                                    plugin.OnCommand -= new OutGoingCommandHandler(Plugin_OnCommand);
                                    loadedPlugins.Remove(plugin);
                                    plugin.Dispose();

                                    Type ObjType = null;
                                    Assembly ass = null;

                                    try
                                    {
                                        //reload the plugin
                                        ass = Assembly.LoadFile(pluginsFolder + System.IO.Path.DirectorySeparatorChar + menuItem.ToolTipText);

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
                                        WriteErrorFile(connection, "ReLoadPlugins Cast:", ex);
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
                                            ipi.CurrentFolder = currentFolder;

                                            WindowMessage(null, "Console", "Re-Loaded Plugin - " + ipi.Name + " v" + ipi.Version + " by " + ipi.Author, 4, true);

                                            menuItem.Tag = ipi;

                                            ipi.OnCommand += new OutGoingCommandHandler(Plugin_OnCommand);
                                            ipi.Initialize();
                                            loadedPlugins.Add(ipi);
                                        }
                                        else
                                        {
                                            System.Diagnostics.Debug.WriteLine("obj type is null:" + menuItem.ToolTipText);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        WriteErrorFile(connection, "ReLoadPlugins", ex);
                                    }
                                }
                            }
                            break;                        
                        case "/addtext":
                            if (data.Length > 0)
                            {
                                inputPanel.AppendText(data);
                                FocusInputBox();
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
                        case "/amsg":   //send a message to all channels 
                            if (connection != null && data.Length > 0)
                            {
                                foreach (IceTabPage t in FormMain.Instance.TabMain.TabPages)
                                {
                                    if (t.WindowStyle == IceTabPage.WindowType.Channel)
                                    {
                                        if (t.Connection == connection)
                                        {
                                            SendData(connection, "PRIVMSG " + t.TabCaption + " :" + data);
                                            string msg = GetMessageFormat("Self Channel Message");
                                            msg = msg.Replace("$nick", t.Connection.ServerSetting.NickName).Replace("$channel", t.TabCaption);

                                            //assign $color to the nickname 
                                            if (msg.Contains("$color"))
                                            {
                                                User u = CurrentWindow.GetNick(t.Connection.ServerSetting.NickName);
                                                for (int i = 0; i < u.Level.Length; i++)
                                                {
                                                    if (u.Level[i])
                                                    {
                                                        if (connection.ServerSetting.StatusModes[0][i] == 'v')
                                                            msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelVoiceColor.ToString("00"));
                                                        else if (connection.ServerSetting.StatusModes[0][i] == 'h')
                                                            msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelHalfOpColor.ToString("00"));
                                                        else if (connection.ServerSetting.StatusModes[0][i] == 'o')
                                                            msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelOpColor.ToString("00"));
                                                        else if (connection.ServerSetting.StatusModes[0][i] == 'a')
                                                            msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelAdminColor.ToString("00"));
                                                        else if (connection.ServerSetting.StatusModes[0][i] == 'q')
                                                            msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelOwnerColor.ToString("00"));
                                                        else
                                                            msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelOwnerColor.ToString("00"));

                                                        break;
                                                    }
                                                }
                                                if (msg.Contains("$color"))
                                                    msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelRegularColor.ToString("00"));

                                            }

                                            msg = msg.Replace("$status", CurrentWindow.GetNick(t.Connection.ServerSetting.NickName).ToString().Replace(t.Connection.ServerSetting.NickName, ""));
                                            msg = msg.Replace("$message", data);


                                            t.TextWindow.AppendText(msg, 1);
                                            t.TextWindow.ScrollToBottom();
                                            t.LastMessageType = ServerMessageType.Message;

                                        }
                                    }
                                }
                            }
                            break;
                        case "/anick":
                            if (data.Length > 0)
                            {
                                foreach (IRCConnection c in serverTree.ServerConnections.Values)
                                    if (c.IsConnected)
                                        SendData(c, "NICK " + data);
                            }
                            break;                        
                        case "/autojoin":
                            if (connection != null)
                            {
                                if (connection.ServerSetting.AutoJoinChannels != null)
                                {
                                    foreach (string chan in connection.ServerSetting.AutoJoinChannels)
                                    {
                                        if (!chan.StartsWith(";"))
                                            SendData(connection, "JOIN " + chan);
                                    }
                                }
                            }
                            break;
                        case "/autoperform":
                            if (connection != null)
                            {
                                if (connection.ServerSetting.AutoPerform != null)
                                {
                                    foreach (string ap in connection.ServerSetting.AutoPerform)
                                    {
                                        string autoCommand = ap.Replace("\r", String.Empty);
                                        if (!autoCommand.StartsWith(";"))
                                            ParseOutGoingCommand(connection, autoCommand);
                                    }
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
                        case "/chaninfo":
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
                                        SendData(connection, "TOPIC :" + t.TabCaption);
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
                                        SendData(connection, "TOPIC :" + CurrentWindow.TabCaption);
                                        fci.ShowDialog(this);
                                    }
                                }
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
                        case "/close":
                            if (connection != null && data.Length > 0)
                            {
                                //check if it is a channel list window
                                if (data == "Channels")
                                {
                                    IceTabPage c = GetWindow(connection, "", IceTabPage.WindowType.ChannelList);
                                    if (c != null)
                                        RemoveWindow(connection, "", IceTabPage.WindowType.ChannelList);
                                    return;
                                }

                                //check if it is a query window
                                IceTabPage q = GetWindow(connection, data, IceTabPage.WindowType.Query);
                                if (q != null)
                                {
                                    RemoveWindow(connection, q.TabCaption, IceTabPage.WindowType.Query);
                                }
                                else if (CurrentWindowType == IceTabPage.WindowType.Query)
                                {
                                    RemoveWindow(connection, CurrentWindow.TabCaption, CurrentWindow.WindowStyle);
                                }
                            }
                            else if (connection != null)
                            {
                                //check if current window is channel
                                if (CurrentWindowType == IceTabPage.WindowType.Query)
                                    RemoveWindow(connection, CurrentWindow.TabCaption, CurrentWindow.WindowStyle);
                            }
                            else
                            {
                                //check if the current window is the debug window
                                if (CurrentWindowType == IceTabPage.WindowType.Debug)
                                {
                                    RemoveWindow(null, "Debug", IceTabPage.WindowType.Debug);
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
                                if (ctcp.ToUpper() == "PING")
                                    SendData(connection, "PRIVMSG " + nick + " :" + ctcp.ToUpper() + " " + System.Environment.TickCount.ToString() + "");
                                else
                                    SendData(connection, "PRIVMSG " + nick + " " + ctcp.ToUpper() + "");
                            }
                            break;
                        case "/dcc":
                            if (connection != null && data.IndexOf(' ') > 0)
                            {
                                //get the type of dcc
                                string dccType = data.Substring(0, data.IndexOf(' ')).ToUpper();
                                //get who it is being sent to
                                string nick = data.Substring(data.IndexOf(' ') + 1);
                                System.Diagnostics.Debug.WriteLine(dccType + ":" + nick);
                                switch (dccType)
                                {
                                    case "CHAT":
                                        //start a dcc chat
                                        if (nick.IndexOf(' ') == -1)    //make sure no space in the nick name
                                        {
                                            //check if we already have a dcc chat open with this person
                                            if (!mainTabControl.WindowExists(connection, nick, IceTabPage.WindowType.DCCChat))
                                            {
                                                //create a new window
                                                AddWindow(connection, nick, IceTabPage.WindowType.DCCChat);
                                                IceTabPage t = GetWindow(connection, nick, IceTabPage.WindowType.DCCChat);
                                                if (t != null)
                                                {
                                                    t.RequestDCCChat();
                                                    string msg = GetMessageFormat("DCC Chat Outgoing");
                                                    msg = msg.Replace("$nick", nick);
                                                    t.TextWindow.AppendText(msg, 1);
                                                    t.TextWindow.ScrollToBottom();
                                                }
                                            }
                                            else
                                                mainTabControl.SelectTab(GetWindow(connection, nick, IceTabPage.WindowType.DCCChat));

                                        }
                                        break;
                                    case "SEND":
                                        //was a filename specified, if not try and select one
                                        if (nick.IndexOf(' ') > 0)
                                        {
                                            System.Diagnostics.Debug.WriteLine("try it");
                                            //more to it, maybe a file to send
                                            //if (!mainTabControl.WindowExists(:
                                            if (!mainTabControl.WindowExists(null, "DCC Files", IceTabPage.WindowType.DCCFile))
                                                AddWindow(null, "DCC Files", IceTabPage.WindowType.DCCFile);


                                        }
                                        else
                                        {
                                            //ask for a file name
                                            OpenFileDialog dialog = new OpenFileDialog();
                                            dialog.InitialDirectory = iceChatOptions.DCCSendFolder;
                                            dialog.CheckFileExists = true;
                                            dialog.CheckPathExists = true;
                                            if (dialog.ShowDialog() == DialogResult.OK)
                                            {
                                                //returns the full path
                                                System.Diagnostics.Debug.WriteLine(dialog.FileName);
                                            }

                                        }
                                        break;
                                }
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
                        case "/font":
                            //change the font of the current window
                            //check if data is a channel
                            if (connection != null && data.Length > 0)
                            {
                                if (data.IndexOf(' ') == -1)
                                {
                                    IceTabPage t = GetWindow(connection, data, IceTabPage.WindowType.Channel);
                                    if (t != null)
                                    {
                                        //bring up a font dialog
                                        FontDialog fd = new FontDialog();
                                        //load the current font
                                        fd.Font = t.TextWindow.Font;
                                        if (fd.ShowDialog() != DialogResult.Cancel && fd.Font.Style == FontStyle.Regular)
                                        {
                                            t.TextWindow.Font = fd.Font;
                                        }
                                    }
                                }
                            }
                            break;
                        case "/forcequit":
                            if (connection != null)
                                connection.ForceDisconnect();
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
                        case "/ignore":
                            if (connection != null)
                            {
                                if (data.Length > 0)
                                {
                                    //check if just a nick/host , no extra params
                                    if (data.IndexOf(" ") == -1)
                                    {
                                        //check if already in ignore list or not
                                        for (int i = 0; i < connection.ServerSetting.IgnoreList.Length;i++ )
                                        {
                                            string checkNick = connection.ServerSetting.IgnoreList[i];
                                            if (connection.ServerSetting.IgnoreList[i].StartsWith(";"))
                                                checkNick = checkNick.Substring(1);
                                            
                                            if (checkNick.ToLower() == data.ToLower())
                                            {
                                                if (connection.ServerSetting.IgnoreList[i].StartsWith(";"))
                                                    connection.ServerSetting.IgnoreList[i] = checkNick;
                                                else
                                                    connection.ServerSetting.IgnoreList[i] = ";" + checkNick;

                                                
                                                serverTree.SaveServers(serverTree.ServersCollection);
                                                return;
                                            }
                                        }

                                        //no match found, add the new item to the IgnoreList
                                        System.Collections.Specialized.StringCollection sc = new System.Collections.Specialized.StringCollection();
                                        foreach (string n in connection.ServerSetting.IgnoreList)
                                            sc.Add(n);
                                        sc.Add(data);
                                        connection.ServerSetting.IgnoreList = new string[sc.Count];
                                        sc.CopyTo(connection.ServerSetting.IgnoreList, 0);
                                        sc.Clear();
                                        serverTree.SaveServers(serverTree.ServersCollection);
                                    }
                                }
                            }
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
                                string msg2 = data.Substring(data.IndexOf(' ') + 1);
                                SendData(connection, "PRIVMSG " + nick + " :" + msg2);

                                //get the color for the private message
                                string msg = GetMessageFormat("Self Channel Message");
                                msg = msg.Replace("$nick", connection.ServerSetting.NickName).Replace("$channel", nick);

                                if (msg.StartsWith("&#x3;"))
                                {
                                    //get the color
                                    string color = msg.Substring(0, 6);
                                    int result;
                                    if (Int32.TryParse(msg.Substring(6, 1), out result))
                                        color += msg.Substring(6, 1);

                                    msg = color + "*" + nick + "* " + data.Substring(data.IndexOf(' ') + 1); ;
                                }
                                 
                                CurrentWindowMessage(connection, msg, 1, true);
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

                                string nmsg = GetMessageFormat("Self Notice");
                                nmsg = nmsg.Replace("$nick", nick).Replace("$message", msg);
                                
                                CurrentWindowMessage(connection, nmsg, 1, true);
                            }
                            break;
                        case "/part":
                            if (connection != null && data.Length > 0)
                            {
                                //check if it is a query window
                                IceTabPage q = GetWindow(connection, data, IceTabPage.WindowType.Query);
                                if (q != null)
                                {
                                    RemoveWindow(connection, q.TabCaption, IceTabPage.WindowType.Query);
                                    return;
                                }
                                else if (CurrentWindowType == IceTabPage.WindowType.Query)
                                {
                                    RemoveWindow(connection, CurrentWindow.TabCaption, IceTabPage.WindowType.Query);
                                    return;
                                }

                                //is there a part message
                                if (data.IndexOf(' ') > -1)
                                {
                                    //check if channel is a valid channel
                                    if (Array.IndexOf(connection.ServerSetting.ChannelTypes, data[0]) != -1)
                                    {
                                        SendData(connection, "PART " + data.Substring(0, data.IndexOf(' ')) + " :" + data.Substring(data.IndexOf(' ') + 1));
                                        RemoveWindow(connection, data.Substring(0, data.IndexOf(' ')), IceTabPage.WindowType.Channel);
                                    }
                                    else
                                    {
                                        //not a valid channel, use the current window
                                        if (CurrentWindowType == IceTabPage.WindowType.Channel)
                                        {
                                            SendData(connection, "PART " + CurrentWindow.TabCaption + " :" + data);
                                            RemoveWindow(connection, CurrentWindow.TabCaption, IceTabPage.WindowType.Channel);
                                        }
                                    }
                                }
                                else
                                {
                                    //see if data is a valid channel;
                                    if (Array.IndexOf(connection.ServerSetting.ChannelTypes, data[0]) != -1)
                                    {
                                        SendData(connection, "PART " + data);
                                        RemoveWindow(connection, data, IceTabPage.WindowType.Channel);
                                    }
                                    else
                                    {
                                        if (CurrentWindowType == IceTabPage.WindowType.Channel)
                                        {
                                            SendData(connection, "PART " + CurrentWindow.TabCaption + " :" + data);
                                            RemoveWindow(connection, CurrentWindow.TabCaption, IceTabPage.WindowType.Channel);
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
                                    RemoveWindow(connection, CurrentWindow.TabCaption, IceTabPage.WindowType.Channel);
                                }
                                else if (CurrentWindowType == IceTabPage.WindowType.Query)
                                {
                                    RemoveWindow(connection, CurrentWindow.TabCaption, IceTabPage.WindowType.Query);
                                }
                            }
                            break;
                        case "/partall":
                            if (connection != null)
                            {
                                for (int i = mainTabControl.TabPages.Count - 1; i >= 0; i--)
                                {
                                    if (mainTabControl.TabPages[i].WindowStyle == IceTabPage.WindowType.Channel)
                                    {
                                        if (mainTabControl.TabPages[i].Connection == connection)
                                        {
                                            SendData(connection, "PART " + mainTabControl.TabPages[i].TabCaption);
                                            RemoveWindow(connection, mainTabControl.TabPages[i].TabCaption, IceTabPage.WindowType.Channel);
                                        }
                                    }
                                }
                            }
                            break;
                        case "/ping":
                            if (connection != null && data.Length > 0 && data.IndexOf(' ') == -1)
                            {
                                //ctcp nick ping
                                string msg = GetMessageFormat("Ctcp Send");
                                msg = msg.Replace("$nick", data); ;
                                msg = msg.Replace("$ctcp", "PING");
                                CurrentWindowMessage(connection, msg, 7, true);
                                SendData(connection, "PRIVMSG " + data + " :PING");
                            }                            
                            break;
                        case "/play":   //play a WAV sound
                            if (data.Length > 0)
                            {
                                //check if the WAV file exists in the Sounds Folder
                                
                                if (File.Exists(soundsFolder + System.IO.Path.DirectorySeparatorChar + data))
                                {
                                    try
                                    {
                                        player.SoundLocation = soundsFolder + System.IO.Path.DirectorySeparatorChar + data;
                                        player.Play();
                                    }
                                    catch { }
                                }
                            }
                            break;                        
                        case "/query":
                            if (connection != null && data.Length > 0)
                            {
                                string nick = "";
                                string msg = "";
                                
                                if (data.IndexOf(" ") > 0)
                                {
                                    //check if there is a message added
                                    nick = data.Substring(0, data.IndexOf(' '));
                                    msg = data.Substring(data.IndexOf(' ') + 1);
                                }
                                else
                                    nick = data;

                                if (!mainTabControl.WindowExists(connection, nick, IceTabPage.WindowType.Query))
                                    AddWindow(connection, nick, IceTabPage.WindowType.Query);
                                
                                mainTabControl.SelectTab(GetWindow(connection, nick, IceTabPage.WindowType.Query));

                                if (msg.Length > 0)
                                {
                                    SendData(connection, "PRIVMSG " + nick + " :" + msg);

                                    string nmsg = GetMessageFormat("Self Private Message");
                                    nmsg = nmsg.Replace("$nick", inputPanel.CurrentConnection.ServerSetting.NickName).Replace("$message", msg);
                                    
                                    CurrentWindow.TextWindow.AppendText(nmsg, 1);
                                    CurrentWindow.LastMessageType = ServerMessageType.Message;
                                }
                            }
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
                        case "/aquit":
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
                        case "/run":
                            if (data.Length > 0)
                                System.Diagnostics.Process.Start(data);
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
                                                    msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelVoiceColor.ToString("00"));
                                                else if (connection.ServerSetting.StatusModes[0][i] == 'h')
                                                    msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelHalfOpColor.ToString("00"));
                                                else if (connection.ServerSetting.StatusModes[0][i] == 'o')
                                                    msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelOpColor.ToString("00"));
                                                else if (connection.ServerSetting.StatusModes[0][i] == 'a')
                                                    msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelAdminColor.ToString("00"));
                                                else if (connection.ServerSetting.StatusModes[0][i] == 'q')
                                                    msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelOwnerColor.ToString("00"));
                                                else
                                                    msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelOwnerColor.ToString("00"));

                                                break;
                                            }
                                        }
                                        if (msg.Contains("$color"))
                                            msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelRegularColor.ToString("00"));

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
                        case "/joinserv":       //joinserv irc.server.name #channel
                            if (data.Length > 0 && data.IndexOf(' ') > 0)
                            {
                                //check if default nick name has been set
                                if (iceChatOptions.DefaultNick == null || iceChatOptions.DefaultNick.Length == 0)
                                {
                                    CurrentWindowMessage(connection, "No Default Nick Name Assigned. Go to Server Settings and set one under the Default Server Settings section.", 1, false);
                                }
                                else
                                {
                                    ServerSetting s = new ServerSetting();
                                    //get the server name
                                    //if there is a port name. extract it
                                    string server = data.Substring(0,data.IndexOf(' '));
                                    string channel = data.Substring(data.IndexOf(' ')+1);
                                    if (server.Contains(":"))
                                    {
                                        s.ServerName = server.Substring(0, server.IndexOf(':'));
                                        s.ServerPort = server.Substring(server.IndexOf(':') + 1);
                                        if (s.ServerPort.IndexOf(' ') > 0)
                                        {
                                            s.ServerPort = s.ServerPort.Substring(0, s.ServerPort.IndexOf(' '));
                                        }
                                        //check for + in front of port, SSL Connection
                                        if (s.ServerPort.StartsWith("+"))
                                        {
                                            s.ServerPort = s.ServerPort.Substring(1);
                                            s.UseSSL = true;
                                        }
                                    }
                                    else
                                    {
                                        s.ServerName = server;
                                        s.ServerPort = "6667";
                                    }

                                    s.NickName = iceChatOptions.DefaultNick;
                                    s.AltNickName = iceChatOptions.DefaultNick + "_";
                                    s.AwayNickName = iceChatOptions.DefaultNick + "[A]";
                                    s.FullName = iceChatOptions.DefaultFullName;
                                    s.QuitMessage = iceChatOptions.DefaultQuitMessage;
                                    s.IdentName = iceChatOptions.DefaultIdent;
                                    s.IAL = new Hashtable();
                                    s.AutoJoinChannels = new string[] { channel };
                                    s.AutoJoinEnable = true;
                                    Random r = new Random();
                                    s.ID = r.Next(50000, 99999);
                                    NewServerConnection(s);
                                }
                            }                            
                            break;
                        case "/server":
                            if (data.Length > 0)
                            {
                                //check if default nick name has been set
                                if (iceChatOptions.DefaultNick == null || iceChatOptions.DefaultNick.Length == 0)
                                {
                                    CurrentWindowMessage(connection, "No Default Nick Name Assigned. Go to Server Settings and set one under the Default Server Settings section.", 1, false);
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
                                        //check for + in front of port, SSL Connection
                                        if (s.ServerPort.StartsWith("+"))
                                        {
                                            s.ServerPort = s.ServerPort.Substring(1);
                                            s.UseSSL = true;
                                            System.Diagnostics.Debug.WriteLine("use ssl");
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
                                        //check for + in front of port, SSL Connection
                                        if (s.ServerPort.StartsWith("+"))
                                        {
                                            s.ServerPort = s.ServerPort.Substring(1);
                                            s.UseSSL = true;
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
                                    s.FullName = iceChatOptions.DefaultFullName;
                                    s.QuitMessage = iceChatOptions.DefaultQuitMessage;
                                    s.IdentName = iceChatOptions.DefaultIdent;                                    
                                    s.IAL = new Hashtable();

                                    Random r = new Random();
                                    s.ID = r.Next(50000, 99999);
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
                        case "/userinfo":
                            if (connection != null && data.Length > 0)
                            {
                                FormUserInfo fui = new FormUserInfo(connection);
                                //find the user
                                fui.NickName(data);
                                fui.ShowDialog(this);
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
                        case "/aline":  //for adding lines to @windows
                            if (data.Length > 0 && data.IndexOf(" ") > -1)
                            {
                                string window = data.Substring(0, data.IndexOf(' '));
                                string msg = data.Substring(data.IndexOf(' ') + 1);
                                if (GetWindow(null, window, IceTabPage.WindowType.Window) == null)
                                    AddWindow(null, window, IceTabPage.WindowType.Window);

                                IceTabPage t = GetWindow(null, window, IceTabPage.WindowType.Window);
                                if (t != null)
                                    t.TextWindow.AppendText(msg, 1);
                            }
                            break;
                        case "/window":
                            if (data.Length > 0)
                            {
                                if (data.StartsWith("@") && data.IndexOf(" ") == -1)
                                {
                                    if (GetWindow(null, data, IceTabPage.WindowType.Window) == null)
                                        AddWindow(null, data, IceTabPage.WindowType.Window);
                                }
                            }
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
                                                msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelVoiceColor.ToString("00"));
                                            else if (connection.ServerSetting.StatusModes[0][i] == 'h')
                                                msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelHalfOpColor.ToString("00"));
                                            else if (connection.ServerSetting.StatusModes[0][i] == 'o')
                                                msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelOpColor.ToString("00"));
                                            else if (connection.ServerSetting.StatusModes[0][i] == 'a')
                                                msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelAdminColor.ToString("00"));
                                            else if (connection.ServerSetting.StatusModes[0][i] == 'q')
                                                msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelOwnerColor.ToString("00"));
                                            else
                                                msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelOwnerColor.ToString("00"));

                                            break;
                                        }
                                    }

                                    if (msg.Contains("$color"))
                                        msg = msg.Replace("$color", ((char)3).ToString() + iceChatColors.ChannelRegularColor.ToString("00"));
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
                            else if (CurrentWindowType == IceTabPage.WindowType.DCCChat)
                            {
                                CurrentWindow.SendDCCData(data);

                                string msg = GetMessageFormat("Self DCC Chat Message");
                                msg = msg.Replace("$nick", inputPanel.CurrentConnection.ServerSetting.NickName).Replace("$message", data);

                                CurrentWindow.TextWindow.AppendText(msg, 1);
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
                WriteErrorFile(connection, "ParseOutGoingCommand", e);
            }
        }
        /// <summary>
        /// Input Panel Text Box had Entered Key Pressed or Send Button Pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void inputPanel_OnCommand(object sender, string data)
        {
            PluginArgs args = new PluginArgs(inputPanel.CurrentConnection);
            args.Extra = data;
            
            foreach (IPluginIceChat ipc in loadedPlugins)
            {
                args = ipc.InputText(args);
            }              
                        
            ParseOutGoingCommand(inputPanel.CurrentConnection, args.Extra);
            if (CurrentWindowType == IceTabPage.WindowType.Console)
                mainTabControl.CurrentTab.CurrentConsoleWindow().ScrollToBottom();
            else if (CurrentWindowType != IceTabPage.WindowType.DCCFile && CurrentWindowType != IceTabPage.WindowType.ChannelList)
                CurrentWindow.TextWindow.ScrollToBottom();
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
                            data = ReplaceFirst(data, m.Value, connection.ServerSetting.NickName);
                        else
                            data = ReplaceFirst(data, m.Value, "$null");
                        break;
                    case "$altnick":
                        if (connection != null)
                            data = ReplaceFirst(data, m.Value, connection.ServerSetting.AltNickName);
                        else
                            data = ReplaceFirst(data, m.Value, "$null");
                        break;
                    case "$ident":
                        if (connection != null)
                            data = ReplaceFirst(data, m.Value, connection.ServerSetting.IdentName);
                        else
                            data = ReplaceFirst(data, m.Value, "$null");
                        break;
                    case "$host":
                        if (connection != null)
                            data = ReplaceFirst(data, m.Value, connection.ServerSetting.LocalHost);
                        else
                            data = ReplaceFirst(data, m.Value, "$null");
                        break;
                    case "$fullhost":
                        if (connection != null)
                            data = ReplaceFirst(data, m.Value, connection.ServerSetting.NickName + "!" + connection.ServerSetting.LocalHost);
                        else
                            data = ReplaceFirst(data, m.Value, "$null");
                        break;                    
                    case "$fullname":
                        if (connection != null)
                            data = ReplaceFirst(data, m.Value, connection.ServerSetting.FullName);
                        else
                            data = ReplaceFirst(data, m.Value, "$null");
                        break;
                    case "$ip":
                        if (connection != null)
                            data = ReplaceFirst(data, m.Value, connection.ServerSetting.LocalIP.ToString());
                        else
                            data = ReplaceFirst(data, m.Value, "$null");
                        break;
                    case "$network":
                        if (connection != null)
                            data = ReplaceFirst(data, m.Value, connection.ServerSetting.NetworkName);
                        else
                            data = ReplaceFirst(data, m.Value, "$null");
                        break;
                    case "$port":
                        if (connection != null)
                            data = ReplaceFirst(data, m.Value, connection.ServerSetting.ServerPort);
                        else
                            data = ReplaceFirst(data, m.Value, "$null");
                        break;
                    case "$quitmessage":
                        if (connection != null)
                            data = ReplaceFirst(data, m.Value, connection.ServerSetting.QuitMessage);
                        else
                            data = ReplaceFirst(data, m.Value, "$null");
                        break;
                    case "$servermode":
                        data = ReplaceFirst(data, m.Value, string.Empty);
                        break;
                    case "$server":
                        if (connection != null)
                        {
                            if (connection.ServerSetting.RealServerName.Length > 0)
                                data = ReplaceFirst(data, m.Value, connection.ServerSetting.RealServerName);
                            else
                                data = ReplaceFirst(data, m.Value, connection.ServerSetting.ServerName);
                        }
                        else
                            data = ReplaceFirst(data, m.Value, "$null");
                        break;
                    case "$online":
                        if (connection != null)
                        {
                            //check the datediff
                            TimeSpan online = DateTime.Now.Subtract(connection.ServerSetting.ConnectedTime);
                            data = ReplaceFirst(data, m.Value, GetDuration((int)online.TotalSeconds));
                        }
                        else
                            data = ReplaceFirst(data, m.Value, "$null");
                        break;
                    case "$serverip":
                        if (connection != null)
                            data = ReplaceFirst(data, m.Value, connection.ServerSetting.ServerIP);
                        else
                            data = ReplaceFirst(data, m.Value, "$null");
                        break;
                    case "$localip":
                        if (connection != null)
                            data = ReplaceFirst(data, m.Value, connection.ServerSetting.LocalIP.ToString());
                        else
                            data = ReplaceFirst(data, m.Value, "$null");
                        break;
                    
                    //identifiers that do not require a connection                                
                    case "$appdata":
                        data = ReplaceFirst(data, m.Value, Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).ToString());
                        break;
                    case "$ossp":
                        data = ReplaceFirst(data, m.Value, Environment.OSVersion.ServicePack.ToString());
                        break;
                    case "$osbuild":
                        data = ReplaceFirst(data, m.Value, Environment.OSVersion.Version.Build.ToString());
                        break;
                    case "$osplatform":
                        data = ReplaceFirst(data, m.Value, Environment.OSVersion.Platform.ToString());
                        break;
                    case "$os":
                        data = ReplaceFirst(data, m.Value, GetOperatingSystemName());
                        break;
                    case "$icepath":
                    case "$icechatexedir":
                        data = ReplaceFirst(data, m.Value, Directory.GetCurrentDirectory());
                        break;
                    case "$aliasfile":
                        data = ReplaceFirst(data, m.Value,aliasesFile);
                        break;
                    case "$serverfile":
                        data = ReplaceFirst(data, m.Value, serversFile);
                        break;
                    case "$popupfile":
                        data = ReplaceFirst(data, m.Value, popupsFile);
                        break;
                    case "$icechatver":
                        data = ReplaceFirst(data, m.Value, Settings.Default.Version);
                        break;
                    case "$version":
                        data = ReplaceFirst(data, m.Value, Settings.Default.ProgramID + " " + Settings.Default.Version);
                        break;
                    case "$icechatdir":
                        data = ReplaceFirst(data, m.Value, currentFolder);
                        break;
                    case "$icechathandle":
                        data = ReplaceFirst(data, m.Value, this.Handle.ToString());
                        break;
                    case "$icechat":
                        data = ReplaceFirst(data, m.Value, Settings.Default.ProgramID + " " + Settings.Default.Version + " http://www.icechat.net");
                        break;
                    case "$logdir":
                        data = ReplaceFirst(data, m.Value, logsFolder);
                        break;
                    case "$randquit":
                        Random rand = new Random();
                        int rq = rand.Next(0, QuitMessages.RandomQuitMessages.Length);
                        data = ReplaceFirst(data, m.Value, QuitMessages.RandomQuitMessages[rq]);
                        break;
                    case "$randcolor":
                        Random randcolor = new Random();
                        int rc = randcolor.Next(0, (IrcColor.colors.Length-1));
                        data = ReplaceFirst(data, m.Value, rc.ToString());
                        break;
                    case "$tickcount":
                        data = ReplaceFirst(data, m.Value, System.Environment.TickCount.ToString());
                        break;
                    case "$totalwindows":
                        data = ReplaceFirst(data, m.Value, mainTabControl.TabCount.ToString());
                        break;
                    case "$uptime2":
                        int systemUpTime = System.Environment.TickCount / 1000;
                        TimeSpan ts = TimeSpan.FromSeconds(systemUpTime);
                        data = ReplaceFirst(data, m.Value, GetDuration(ts.TotalSeconds));
                        break;
                    case "$uptime":
                        System.Diagnostics.PerformanceCounter pc = new System.Diagnostics.PerformanceCounter("System", "System Up Time");
                        pc.NextValue();
                        TimeSpan ts2 = TimeSpan.FromSeconds(pc.NextValue());
                        data = ReplaceFirst(data, m.Value, GetDuration(ts2.TotalSeconds));
                        break;
                }
                m = m.NextMatch();
            }

            return data;
        }
        private string GetOperatingSystemName()
        {
            string OSName = "Unknown";
            System.OperatingSystem osInfo = System.Environment.OSVersion;
            switch (osInfo.Platform)
            {
                
                case PlatformID.Unix:
                    OSName = Environment.OSVersion.ToString();
                    break;                
                case PlatformID.Win32NT:

                    switch (osInfo.Version.Major)
                    {
                        case 3:
                            OSName = "Windows NT 3.51";
                            break;
                        case 4:
                            OSName = "Windows NT 4.0";
                            break;
                        case 5:
                            switch (osInfo.Version.Minor)
                            {
                                case 0:
                                    OSName = "Windows 2000";
                                    break;
                                case 1:
                                    OSName = "Windows XP";
                                    break;
                                case 2:
                                    OSName = "Windows 2003";
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case 6:
                            OSName = "Windows Vista";
                            break;
                        default:
                            OSName = "Unknown Win32NT Windows";
                            break;

                    }
                    break;

                case PlatformID.Win32S:
                    break;

                case PlatformID.Win32Windows:
                    switch (osInfo.Version.Major)
                    {
                        case 0:
                            OSName = "Windows 95";
                            break;
                        case 10:
                            if (osInfo.Version.Revision.ToString() == "2222A")
                                OSName = "Windows 98 Second Edition";
                            else
                                OSName = "Windows 98";
                            break;
                        case 90:
                            OSName = "Windows ME";
                            break;
                        default:
                            OSName = "Unknown Win32 Windows";
                            break;
                    }
                    break;
                case PlatformID.WinCE:
                    break;
                default:
                    break;

            }
            return OSName;

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
                string extra = "";
                bool askExtra = false;


                foreach (string word in parsedData)
                {
                    count++;

                    if (word.StartsWith("//") && count == 0)
                        changedData[count] = word.Substring(1);

                    if (askExtra)
                    {
                        //continueing a $?= 
                        extra += " " + word;
                        changedData[count] = null;
                        if (extra[extra.Length - 1] == extra[0])
                        {
                            askExtra = false;
                            //ask the question
                            InputBoxDialog i = new InputBoxDialog();
                            i.FormCaption = "Enter Value";
                            i.FormPrompt = extra.Substring(1,extra.Length-2);

                            i.ShowDialog();
                            if (i.InputResponse.Length > 0)
                                changedData[count] = i.InputResponse;
                            i.Dispose();                            
                        }
                    }

                    //parse out identifiers (start with a $)
                    if (word.StartsWith("$"))
                    {
                        switch (word)
                        {

                            default:
                                int result;
                                
                                if (word.StartsWith("$?=") && word.Length > 5)
                                {
                                    //check for 2 quotes (single or double)
                                    string ask = word.Substring(3);
                                    //check what kind of a quote it is
                                    char quote = ask[0];
                                    if (quote == ask[ask.Length - 1])
                                    {
                                        //ask the question
                                        extra = ask;
                                        InputBoxDialog i = new InputBoxDialog();
                                        i.FormCaption = "Enter Value";
                                        i.FormPrompt = extra.Substring(1, extra.Length - 2);

                                        i.ShowDialog();
                                        if (i.InputResponse.Length > 0)
                                            changedData[count] = i.InputResponse;
                                        else
                                            changedData[count] = null;
                                        i.Dispose();
                                    }
                                    else
                                    {
                                        //go to the next word until we find a quote at the end
                                        extra = ask;
                                        askExtra = true;
                                        changedData[count] = null;
                                    }
                                }
                                
                                if (word.StartsWith("$read(") && word.IndexOf(')') > word.IndexOf('('))
                                {
                                    string file = ReturnBracketValue(word);
                                    //check if we have passed a path or just a filename
                                    if (file.IndexOf(System.IO.Path.DirectorySeparatorChar) > -1)
                                    {
                                        //its a full folder
                                        if (File.Exists(file))
                                        {
                                            //count the number of lines in the file                                            
                                            //load the file in and read a random line from it
                                            string[] lines = File.ReadAllLines(file);
                                            if (lines.Length > 0)
                                            {
                                                //pick a random line
                                                Random r = new Random();
                                                int line = r.Next(0, lines.Length - 1);
                                                changedData[count] = lines[line];
                                            }
                                            else
                                                changedData[count] = "$null";

                                        }
                                    }
                                    else
                                    {
                                        //just check in the Scripts Folder
                                        if (File.Exists(scriptsFolder + System.IO.Path.DirectorySeparatorChar + file))
                                        {
                                            //load the file in and read a random line from it
                                            string[] lines = File.ReadAllLines(scriptsFolder + System.IO.Path.DirectorySeparatorChar + file);
                                            if (lines.Length > 0)
                                            {
                                                //pick a random line
                                                Random r = new Random();
                                                int line = r.Next(0, lines.Length - 1);
                                                changedData[count] = lines[line];
                                            }
                                            else
                                                changedData[count] = "$null";
                                        }
                                    }
                                }
                                if (connection != null)
                                {
                                    if (word.StartsWith("$ial(") && word.IndexOf(')') > word.IndexOf('('))
                                    {
                                        string nick = ReturnBracketValue(word);
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
                                        string values = ReturnBracketValue(word);
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
                                        string channel = ReturnBracketValue(word);
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
                WriteErrorFile(connection, "ParseIdentifiers" + data, e);
            }
            //return String.Join(" ", changedData);
            return JoinString(changedData);
        }
        
        //rejoin an arrayed string into a single string, not adding null values
        private string JoinString(string[] joinString)
        {
            string joined = "";
            foreach (string j in joinString)
            {
                if (j != null)
                    joined += j + " ";
            }
            if (joined.Length > 0)
                joined = joined.Substring(0, joined.Length - 1);
            return joined;
        }

        private string ReturnBracketValue(string data)
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

        //replace 1st occurence of a string inside another string
        private string ReplaceFirst(string haystack, string needle, string replacement)
        {
            int pos = haystack.IndexOf(needle);
            if (pos < 0) return haystack;

            return haystack.Substring(0, pos) + replacement + haystack.Substring(pos + needle.Length);
        }

        private string GetDuration(double seconds)
        {
            TimeSpan t = new TimeSpan(0, 0,(int)seconds);

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
            FormSettings fs = new FormSettings(iceChatOptions, iceChatFonts, iceChatEmoticons, iceChatSounds);
            fs.SaveOptions += new FormSettings.SaveOptionsDelegate(fs_SaveOptions);
            
            fs.ShowDialog(this);
        }

        private void fs_SaveOptions()
        {
            SaveOptions();           
            SaveFonts();
            SaveSounds();

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

            //change the fonts for the Left and Right Dock Panels
            panelDockLeft.Initialize();
            panelDockRight.Initialize();
            
            //change the main Menu Bar Font
            menuMainStrip.Font = new Font(iceChatFonts.FontSettings[7].FontName, iceChatFonts.FontSettings[7].FontSize);

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
            splitterLeft.Visible = serverListToolStripMenuItem.Checked;
            panelDockLeft.Visible = serverListToolStripMenuItem.Checked;
            iceChatOptions.ShowServerTree = serverListToolStripMenuItem.Checked;
        }

        private void nickListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            splitterRight.Visible = nickListToolStripMenuItem.Checked;
            panelDockRight.Visible = nickListToolStripMenuItem.Checked;
            iceChatOptions.ShowNickList = nickListToolStripMenuItem.Checked;
        }

        private void statusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStripMain.Visible = statusBarToolStripMenuItem.Checked;
            iceChatOptions.ShowStatusBar = statusBarToolStripMenuItem.Checked;
        }

        private void toolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripMain.Visible = toolBarToolStripMenuItem.Checked;
            iceChatOptions.ShowToolBar = toolBarToolStripMenuItem.Checked;
        }

        private void channelBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            iceChatOptions.ShowTabBar = channelBarToolStripMenuItem.Checked;
            mainTabControl.ShowTabs = iceChatOptions.ShowTabBar;
        }

        private void codePlexPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://icechat.codeplex.com/");
            }
            catch { }
        }

        private void forumsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://www.icechat.net/forums");
            }
            catch { }
        }

        private void iceChatHomePageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://www.icechat.net/");
            }
            catch { }
        }

        private void facebookFanPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://www.facebook.com/IceChat");
            }
            catch { }
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
            buddyList.SetListColors();
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

            mainTabControl.SelectTab(mainTabControl.GetTabPage("Debug"));
            serverTree.Invalidate();
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

        public void LoadScripts()
        {
            // loads all script 
            loadedScripts.Clear();

            if (FormMain.Instance.IceChatOptions.ScriptFiles == null) return;

            System.Diagnostics.Debug.WriteLine(Application.StartupPath + System.IO.Path.DirectorySeparatorChar + "IceChat2009.exe");

            foreach (string scriptFile in iceChatOptions.ScriptFiles)
            {
                System.Diagnostics.Debug.WriteLine("Load Script " + scriptFile);

                System.CodeDom.Compiler.CodeDomProvider cp = new Microsoft.CSharp.CSharpCodeProvider();
                string[] referenceAssemblies = { "System.dll", "System.Windows.Forms.dll" };

                System.CodeDom.Compiler.CompilerParameters par = new System.CodeDom.Compiler.CompilerParameters(referenceAssemblies);
                //par.ReferencedAssemblies.Add(Application.StartupPath + System.IO.Path.DirectorySeparatorChar + "IceChatScript.dll");
                par.ReferencedAssemblies.Add(Application.StartupPath + System.IO.Path.DirectorySeparatorChar + "IceChat2009.exe");
                
                par.GenerateExecutable = false;
                par.GenerateInMemory = true;
                par.CompilerOptions = "/target:library";
                par.IncludeDebugInformation = true;
                par.TreatWarningsAsErrors = false;
                par.MainClass = "IceChat.Script";

                System.Diagnostics.Debug.WriteLine("Total References Assemblies " + par.ReferencedAssemblies.Count);

                System.CodeDom.Compiler.CompilerResults err = cp.CompileAssemblyFromSource(par, File.ReadAllText(scriptFile));
                if (err.Errors.Count > 0)
                {
                    foreach (System.CodeDom.Compiler.CompilerError ce in err.Errors)
                    {                        
                        FormMain.Instance.WindowMessage(null, "Console", "Script Error: " + ce.ErrorNumber + ":" + ce.ToString(), 4, true);
                    }
                    FormMain.Instance.WindowMessage(null, "Console", "ERROR: Script \"" + scriptFile + "\". has " + err.Errors.Count + " errors. Script not loaded", 4, true);
                    
                    continue;  // jump to next script without loading this. 
                }
                
                object o = err.CompiledAssembly.CreateInstance("IceChat.Script");
                FormMain.Instance.WindowMessage(null, "Console", "Script \"" + scriptFile + "\". loaded.", 4, true);
                loadedScripts.Add(o);
                // run the loaded event. gives the script a change to initialize.
                MethodInfo info = o.GetType().GetMethod("script_loaded");
                if (info != null)
                {
                    info.Invoke(o, new object[] { (IntPtr) this.Handle});
                }
            }
        }

        private void LoadPlugins()
        {
            string[] pluginFiles = Directory.GetFiles(pluginsFolder, "*.DLL");
            
            for (int i = 0; i < pluginFiles.Length; i++)
            {
                //System.Diagnostics.Debug.WriteLine("checking:" + pluginFiles[i]);

                string args = pluginFiles[i].Substring(pluginFiles[i].LastIndexOf("\\") + 1);
                args = args.Substring(0, args.Length - 4);
                
                //System.Diagnostics.Debug.WriteLine(args);
                if (!args.ToUpper().StartsWith("IPLUGIN"))
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
                        WriteErrorFile(inputPanel.CurrentConnection, "LoadPlugins Cast", ex);
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
                            ipi.CurrentFolder = currentFolder;
                            
                            WindowMessage(null, "Console", "Loaded Plugin - " + ipi.Name + " v" + ipi.Version + " by " + ipi.Author, 4, true);
                            
                            //add the menu items
                            ToolStripMenuItem t = new ToolStripMenuItem(ipi.Name);
                            t.Tag = ipi;
                            t.ToolTipText = pluginFiles[i].Substring(pluginFiles[i].LastIndexOf("\\") + 1);
                            t.Click += new EventHandler(OnPluginMenuItemClick);
                            pluginsToolStripMenuItem.DropDownItems.Add(t);

                            ipi.OnCommand += new OutGoingCommandHandler(Plugin_OnCommand);
                            ipi.Initialize();
                            loadedPlugins.Add(ipi);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("obj type is null:" + args);
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteErrorFile(inputPanel.CurrentConnection, "LoadPlugins", ex);
                    }
                }
            }
        }

        private void Plugin_OnCommand(object sender, PluginArgs e)
        {
            if (e.Command != null)
            {
                if (e.Connection != null)
                    ParseOutGoingCommand((IRCConnection)e.Connection, e.Command);
            }

        }

        internal void UnloadPlugin(ToolStripMenuItem menuItem)
        {
            ParseOutGoingCommand(null, "/unloadplugin " + menuItem.ToolTipText);
        }

        internal void ReloadPlugin(ToolStripMenuItem menuItem)
        {
            ParseOutGoingCommand(null, "/reloadplugin " + menuItem.ToolTipText);
        }

        /// <summary>
        /// Write out to the errors file, specific to the Connection
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="method"></param>
        /// <param name="e"></param>
        internal void WriteErrorFile(IRCConnection connection, string method, Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message + ":" + e.StackTrace);
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(e, true);
            WindowMessage(connection, "Console", "Error:" + method + ":" + e.Message + ":" + e.StackTrace + ":" + trace.GetFrame(0).GetFileLineNumber(), 4, true);
            
            if (errorFile != null)
            {
                try
                {
                    errorFile.WriteLine(DateTime.Now.ToString("G") + ":" + method + ":" + e.Message + ":" + e.StackTrace + ":" + trace.GetFrame(0).GetFileLineNumber());
                }
                catch { }
                finally { errorFile.Flush(); }
            }
        }

        /// <summary>
        /// Write out to the errors file, not Connection Specific
        /// </summary>
        /// <param name="method"></param>
        /// <param name="e"></param>
        internal void WriteErrorFile(string method, FileNotFoundException e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message + ":" + e.StackTrace);
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(e, true);
            WindowMessage(inputPanel.CurrentConnection, "Console", "Error:" + method + ":" + e.Message + ":" + e.StackTrace + ":" + trace.GetFrame(0).GetFileLineNumber(), 4, true);

            if (errorFile != null)
            {
                errorFile.WriteLine(DateTime.Now.ToString("G") + ":" + method + ":" + e.Message + ":" + e.StackTrace + ":" + trace.GetFrame(0).GetFileLineNumber());
                errorFile.Flush();
            }
        }

        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //check for newer version
            System.Diagnostics.FileVersionInfo fv = System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            double currentVersion = Convert.ToDouble(fv.FileVersion.Replace(".", String.Empty));
            
            try
            {
                System.Net.WebClient webClient = new System.Net.WebClient();
                webClient.DownloadFile("http://www.icechat.net/update.xml", currentFolder + System.IO.Path.DirectorySeparatorChar + "update.xml");
                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                xmlDoc.Load(currentFolder + System.IO.Path.DirectorySeparatorChar + "update.xml");
                System.Xml.XmlNodeList version = xmlDoc.GetElementsByTagName("version");
                System.Xml.XmlNodeList versiontext = xmlDoc.GetElementsByTagName("versiontext");
                
                if (Convert.ToDouble(version[0].InnerText) > currentVersion)
                {
                    CurrentWindowMessage(inputPanel.CurrentConnection, "There is a newer version of IceChat available (" + versiontext[0].InnerText + ")", 4, true);
                    DialogResult result = MessageBox.Show("Would you like to update to the newer version of IceChat?", "IceChat " + versiontext[0].InnerText + " available", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        System.Diagnostics.Debug.WriteLine("run update program : " + Application.StartupPath);
                        System.Diagnostics.Process.Start(Application.StartupPath + System.IO.Path.DirectorySeparatorChar + "IceChatUpdater.exe","\"" + currentFolder + "\"");                    
                    }
                }
                else
                {
                    CurrentWindowMessage(inputPanel.CurrentConnection, "You are running the latest version of IceChat (" + fv.FileVersion + ") -- Version Online = " + versiontext[0].InnerText, 4, true);
                }
            }
            catch (Exception ex)
            {
                CurrentWindowMessage(inputPanel.CurrentConnection, "Error checking for update:" + ex.Message, 4, true);
            }
           
        }
        
        private void tabControl_DoubleClick(object sender, EventArgs e)
        {
            TabControl t = (TabControl)sender;
            if (t.SelectedTab.Controls[0].GetType() == typeof(Panel))
            {
                Panel p = (Panel)t.SelectedTab.Controls[0];
                UnDockPanel(p);
            }
        }
        /// <summary>
        /// Undock the Specified Panel to a Floating Window
        /// </summary>
        /// <param name="p">The panel to remove and add to a Floating Window</param>
        internal void UnDockPanel(Panel p)
        {
            if (p.Parent.GetType() == typeof(TabPage))
            {
                //System.Diagnostics.Debug.WriteLine(panel1.Parent.Name);
                //remove the tab from the tabStrip
                TabControl t = (TabControl)p.Parent.Parent;
                TabPage tp = (TabPage)p.Parent;
                ((TabControl)p.Parent.Parent).TabPages.Remove((TabPage)p.Parent);
                ((TabPage)p.Parent).Controls.Remove(p);

                if (t.TabPages.Count == 0)
                {
                    //hide the splitter bar along with the panel
                    if (t.Parent == panelDockLeft)
                        splitterLeft.Visible = false;
                    else if (t.Parent == panelDockRight)
                        splitterRight.Visible = false;

                    t.Parent.Visible = false;
                }

                FormFloat formFloat = new FormFloat(ref p, this, tp.Text);
                formFloat.Show();
                formFloat.Left = Cursor.Position.X - (formFloat.Width /2);
                formFloat.Top = Cursor.Position.Y;
            }
        }

        /// <summary>
        /// Re-Dock the Panel checking whether it is closer to the right or left
        /// </summary>
        /// <param name="panel">The panel to re-dock</param>
        /// <param name="formLocation">Current Location of the Floating Form</param>
        /// <param name="tabName">The panels caption</param>
        internal void SetPanel(ref Panel panel, Point formLocation, string tabName)
        {
            if (formLocation.X < (this.Left + 200))
            {
                TabPage p = new TabPage(tabName);
                p.Controls.Add(panel);
                panel.Dock = DockStyle.Fill;
                this.panelDockLeft.TabControl.TabPages.Add(p);
                this.panelDockLeft.TabControl.Visible = true;
                panelDockLeft.Visible = true;
                splitterLeft.Visible = true;
                this.panelDockLeft.TabControl.SelectedTab = p;
            }
            else
            {
                TabPage p = new TabPage(tabName);
                p.Controls.Add(panel);
                panel.Dock = DockStyle.Fill;
                this.panelDockRight.TabControl.TabPages.Add(p);
                this.panelDockRight.TabControl.Visible = true;
                panelDockRight.Visible = true;
                splitterRight.Visible = true;
                this.panelDockRight.TabControl.SelectedTab = p;
            }
        }

        private void browseDataFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ParseOutGoingCommand(null, "//run $icechatdir");
        }

        private void closeCurrentWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //close the current window
            mainTabControl.CloseCurrentTab();
        }

        private void selectNickListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //give the nick list the current focus
            nickList.Focus();
        }

        private void selectServerTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //give the server tree the current focus
            serverTree.Focus();
        }

        private void selectInputBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FocusInputBox();
        }


    }

    #region IceDockPanel Class

    //custom panel class for docking
    internal class IceDockPanel : Panel
    {
        private TabControl _tabControl;
        private bool _docked;
        private int _oldDockWidth;

        public IceDockPanel()
        {
            _tabControl = new TabControl();
            _tabControl.Dock = DockStyle.Fill;
            //_tabControl.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _tabControl.Multiline = true;
            _tabControl.TabStop = false;
            _tabControl.DoubleClick += new EventHandler(OnDoubleClick);
            _tabControl.MouseDown += new MouseEventHandler(OnMouseDown);
            _tabControl.MouseHover += new EventHandler(OnMouseHover);
            _tabControl.MouseLeave += new EventHandler(OnMouseLeave);
            
            this.Controls.Add(_tabControl);

        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (_docked)
            {
                this.Width = _oldDockWidth;

                if (this.Dock == DockStyle.Left)
                    FormMain.Instance.splitterLeft.Visible = true;
                else
                    FormMain.Instance.splitterRight.Visible = true;
                
                _docked = false;

            }
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            if (_docked)
                this.Width = _tabControl.ItemSize.Width * _tabControl.RowCount;
        }

        private void OnMouseHover(object sender, EventArgs e)
        {
            if (_docked)
                this.Width = _oldDockWidth;
        }

        internal bool IsDocked
        {
            get { return _docked; }
        }

        internal void DockControl()
        {
            if (!_docked)
            {
                _docked = true;
                _oldDockWidth = this.Width;
                
                this.Width = _tabControl.ItemSize.Width * _tabControl.RowCount;

                if (this.Dock == DockStyle.Left)
                    FormMain.Instance.splitterLeft.Visible = false;
                else
                    FormMain.Instance.splitterRight.Visible = false;
            }
        }

        internal TabControl TabControl
        {
            get { return _tabControl; }
        }

        private void OnResize(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            //System.Diagnostics.Debug.WriteLine(_tabControl.Width + ":" + _tabControl.ItemSize.Width + ":" + _tabControl.RowCount + ":" + (_tabControl.Width - _tabControl.DisplayRectangle.Width));
        }

        private void OnDoubleClick(object sender, EventArgs e)
        {
            if (_tabControl.SelectedTab.Controls[0].GetType() == typeof(Panel))
            {
                Panel p = (Panel)_tabControl.SelectedTab.Controls[0];
                UnDockPanel(p);
            }
        }

        /// <summary>
        /// Setup the Tabs Font to the setting
        /// </summary>
        internal void Initialize()
        {
            _tabControl.Font = new System.Drawing.Font(FormMain.Instance.IceChatFonts.FontSettings[6].FontName, FormMain.Instance.IceChatFonts.FontSettings[6].FontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        }

        /// <summary>
        /// Undock the Specified Panel to a Floating Window
        /// </summary>
        /// <param name="p">The panel to remove and add to a Floating Window</param>
        internal void UnDockPanel(Panel p)
        {
            if (p.Parent.GetType() == typeof(TabPage))
            {
                //System.Diagnostics.Debug.WriteLine(panel1.Parent.Name);
                //remove the tab from the tabStrip
                TabPage tp = (TabPage)p.Parent;
                _tabControl.TabPages.Remove((TabPage)p.Parent);
                ((TabPage)p.Parent).Controls.Remove(p);

                if (_tabControl.TabPages.Count == 0)
                {
                    //hide the splitter bar along with the panel
                    if (this.Dock == DockStyle.Left)
                        FormMain.Instance.splitterLeft.Visible = false;
                    else
                        FormMain.Instance.splitterRight.Visible = false;

                    this.Visible = false;
                }

                FormFloat formFloat = new FormFloat(ref p, FormMain.Instance, tp.Text);
                formFloat.Show();
                formFloat.Left = Cursor.Position.X - (formFloat.Width / 2);
                formFloat.Top = Cursor.Position.Y;
            }
        }

    }
    
    #endregion
    
}