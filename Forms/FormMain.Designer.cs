namespace IceChat
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        
        private InputPanel inputPanel;
        private System.Windows.Forms.MenuStrip menuMainStrip;
        private System.Windows.Forms.ToolStripMenuItem mainToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private IceDockPanel panelDockRight;
        private IceDockPanel panelDockLeft;
        internal System.Windows.Forms.Splitter splitterLeft;
        internal System.Windows.Forms.Splitter splitterRight;
        private ServerTree serverTree;
        private ChannelList channelList;
        private BuddyList buddyList;
        private NickList nickList;
        internal System.Windows.Forms.ToolStripMenuItem iceChatColorsToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem iceChatEditorToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem iceChatSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.ToolStripButton toolStripQuickConnect;
        private System.Windows.Forms.ToolStripButton toolStripSettings;
        private System.Windows.Forms.ToolStripButton toolStripColors;
        private System.Windows.Forms.ToolStripButton toolStripSystemTray;
        private System.Windows.Forms.ContextMenuStrip contextMenuToolBar;
        private System.Windows.Forms.ToolStripMenuItem hideToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripEditor;
        private System.Windows.Forms.ToolStripMenuItem forumsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem minimizeToTrayToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon notifyIcon;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuMainStrip = new System.Windows.Forms.MenuStrip();
            this.mainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.minimizeToTrayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeCurrentWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iceChatSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iceChatColorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iceChatEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.muteAllSoundsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadAPluginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.serverListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nickListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusBarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolBarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.channelBarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.selectNickListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectServerTreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectInputBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.codePlexPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iceChatHomePageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forumsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.facebookFanPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.browseDataFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitterLeft = new System.Windows.Forms.Splitter();
            this.splitterRight = new System.Windows.Forms.Splitter();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuNotify = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.restoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.contextMenuToolBar = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.hideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripQuickConnect = new System.Windows.Forms.ToolStripButton();
            this.toolStripSettings = new System.Windows.Forms.ToolStripButton();
            this.toolStripColors = new System.Windows.Forms.ToolStripButton();
            this.toolStripEditor = new System.Windows.Forms.ToolStripButton();
            this.toolStripAway = new System.Windows.Forms.ToolStripButton();
            this.toolStripSystemTray = new System.Windows.Forms.ToolStripButton();
            this.toolStripUpdate = new System.Windows.Forms.ToolStripButton();
            this.statusStripMain = new System.Windows.Forms.StatusStrip();
            this.toolStripStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.mainTabControl = new IceChat.IceTabControl();
            this.panelDockRight = new IceChat.IceDockPanel();
            this.panelDockLeft = new IceChat.IceDockPanel();
            this.inputPanel = new IceChat.InputPanel();
            this.downloadPluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainStrip.SuspendLayout();
            this.contextMenuNotify.SuspendLayout();
            this.toolStripMain.SuspendLayout();
            this.contextMenuToolBar.SuspendLayout();
            this.statusStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuMainStrip
            // 
            this.menuMainStrip.AccessibleDescription = "Main Menu Bar";
            this.menuMainStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuBar;
            this.menuMainStrip.AllowItemReorder = true;
            this.menuMainStrip.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuMainStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mainToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuMainStrip.Location = new System.Drawing.Point(0, 0);
            this.menuMainStrip.Name = "menuMainStrip";
            this.menuMainStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuMainStrip.Size = new System.Drawing.Size(796, 25);
            this.menuMainStrip.TabIndex = 12;
            this.menuMainStrip.Text = "menuStripMain";
            // 
            // mainToolStripMenuItem
            // 
            this.mainToolStripMenuItem.BackColor = System.Drawing.SystemColors.Menu;
            this.mainToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.minimizeToTrayToolStripMenuItem,
            this.debugWindowToolStripMenuItem,
            this.closeCurrentWindowToolStripMenuItem,
            this.toolStripMenuItem2,
            this.exitToolStripMenuItem});
            this.mainToolStripMenuItem.ForeColor = System.Drawing.SystemColors.MenuText;
            this.mainToolStripMenuItem.Name = "mainToolStripMenuItem";
            this.mainToolStripMenuItem.Size = new System.Drawing.Size(49, 21);
            this.mainToolStripMenuItem.Text = "Main";
            // 
            // minimizeToTrayToolStripMenuItem
            // 
            this.minimizeToTrayToolStripMenuItem.BackColor = System.Drawing.SystemColors.Menu;
            this.minimizeToTrayToolStripMenuItem.Name = "minimizeToTrayToolStripMenuItem";
            this.minimizeToTrayToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.minimizeToTrayToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.minimizeToTrayToolStripMenuItem.Text = "Minimize to Tray";
            this.minimizeToTrayToolStripMenuItem.Click += new System.EventHandler(this.minimizeToTrayToolStripMenuItem_Click);
            // 
            // debugWindowToolStripMenuItem
            // 
            this.debugWindowToolStripMenuItem.BackColor = System.Drawing.SystemColors.Menu;
            this.debugWindowToolStripMenuItem.ForeColor = System.Drawing.SystemColors.MenuText;
            this.debugWindowToolStripMenuItem.Name = "debugWindowToolStripMenuItem";
            this.debugWindowToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.debugWindowToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.debugWindowToolStripMenuItem.Text = "Debug Window";
            this.debugWindowToolStripMenuItem.Click += new System.EventHandler(this.debugWindowToolStripMenuItem_Click);
            // 
            // closeCurrentWindowToolStripMenuItem
            // 
            this.closeCurrentWindowToolStripMenuItem.BackColor = System.Drawing.SystemColors.Menu;
            this.closeCurrentWindowToolStripMenuItem.Name = "closeCurrentWindowToolStripMenuItem";
            this.closeCurrentWindowToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.closeCurrentWindowToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.closeCurrentWindowToolStripMenuItem.Text = "Close Current Window";
            this.closeCurrentWindowToolStripMenuItem.Click += new System.EventHandler(this.closeCurrentWindowToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(252, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.BackColor = System.Drawing.SystemColors.Menu;
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iceChatSettingsToolStripMenuItem,
            this.iceChatColorsToolStripMenuItem,
            this.iceChatEditorToolStripMenuItem,
            this.muteAllSoundsToolStripMenuItem,
            this.pluginsToolStripMenuItem});
            this.optionsToolStripMenuItem.ForeColor = System.Drawing.SystemColors.MenuText;
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(66, 21);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // iceChatSettingsToolStripMenuItem
            // 
            this.iceChatSettingsToolStripMenuItem.ForeColor = System.Drawing.SystemColors.MenuText;
            this.iceChatSettingsToolStripMenuItem.Name = "iceChatSettingsToolStripMenuItem";
            this.iceChatSettingsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.iceChatSettingsToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.iceChatSettingsToolStripMenuItem.Text = "Program Settings...";
            this.iceChatSettingsToolStripMenuItem.Click += new System.EventHandler(this.iceChatSettingsToolStripMenuItem_Click);
            // 
            // iceChatColorsToolStripMenuItem
            // 
            this.iceChatColorsToolStripMenuItem.Name = "iceChatColorsToolStripMenuItem";
            this.iceChatColorsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.iceChatColorsToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.iceChatColorsToolStripMenuItem.Text = "Colors Settings...";
            this.iceChatColorsToolStripMenuItem.Click += new System.EventHandler(this.iceChatColorsToolStripMenuItem_Click);
            // 
            // iceChatEditorToolStripMenuItem
            // 
            this.iceChatEditorToolStripMenuItem.Name = "iceChatEditorToolStripMenuItem";
            this.iceChatEditorToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.iceChatEditorToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.iceChatEditorToolStripMenuItem.Text = "IceChat Editor...";
            this.iceChatEditorToolStripMenuItem.Click += new System.EventHandler(this.iceChatEditorToolStripMenuItem_Click);
            // 
            // muteAllSoundsToolStripMenuItem
            // 
            this.muteAllSoundsToolStripMenuItem.BackColor = System.Drawing.SystemColors.Menu;
            this.muteAllSoundsToolStripMenuItem.ForeColor = System.Drawing.SystemColors.MenuText;
            this.muteAllSoundsToolStripMenuItem.Name = "muteAllSoundsToolStripMenuItem";
            this.muteAllSoundsToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.muteAllSoundsToolStripMenuItem.Text = "Mute all Sounds";
            this.muteAllSoundsToolStripMenuItem.Click += new System.EventHandler(this.muteAllSoundsToolStripMenuItem_Click);
            // 
            // pluginsToolStripMenuItem
            // 
            this.pluginsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadAPluginToolStripMenuItem});
            this.pluginsToolStripMenuItem.Name = "pluginsToolStripMenuItem";
            this.pluginsToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.pluginsToolStripMenuItem.Text = "Loaded Plugins";
            // 
            // loadAPluginToolStripMenuItem
            // 
            this.loadAPluginToolStripMenuItem.Name = "loadAPluginToolStripMenuItem";
            this.loadAPluginToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.loadAPluginToolStripMenuItem.Text = "Load a Plugin...";
            this.loadAPluginToolStripMenuItem.ToolTipText = "Load a new Plugin";
            this.loadAPluginToolStripMenuItem.Click += new System.EventHandler(this.loadAPluginToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.BackColor = System.Drawing.SystemColors.Menu;
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.serverListToolStripMenuItem,
            this.nickListToolStripMenuItem,
            this.statusBarToolStripMenuItem,
            this.toolBarToolStripMenuItem,
            this.channelBarToolStripMenuItem,
            this.toolStripMenuItem3,
            this.selectNickListToolStripMenuItem,
            this.selectServerTreeToolStripMenuItem,
            this.selectInputBoxToolStripMenuItem});
            this.viewToolStripMenuItem.ForeColor = System.Drawing.SystemColors.MenuText;
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(47, 21);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // serverListToolStripMenuItem
            // 
            this.serverListToolStripMenuItem.Checked = true;
            this.serverListToolStripMenuItem.CheckOnClick = true;
            this.serverListToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.serverListToolStripMenuItem.Name = "serverListToolStripMenuItem";
            this.serverListToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.serverListToolStripMenuItem.Text = "Left Panel";
            this.serverListToolStripMenuItem.Click += new System.EventHandler(this.serverListToolStripMenuItem_Click);
            // 
            // nickListToolStripMenuItem
            // 
            this.nickListToolStripMenuItem.Checked = true;
            this.nickListToolStripMenuItem.CheckOnClick = true;
            this.nickListToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.nickListToolStripMenuItem.Name = "nickListToolStripMenuItem";
            this.nickListToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.nickListToolStripMenuItem.Text = "Right Panel";
            this.nickListToolStripMenuItem.Click += new System.EventHandler(this.nickListToolStripMenuItem_Click);
            // 
            // statusBarToolStripMenuItem
            // 
            this.statusBarToolStripMenuItem.Checked = true;
            this.statusBarToolStripMenuItem.CheckOnClick = true;
            this.statusBarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.statusBarToolStripMenuItem.Name = "statusBarToolStripMenuItem";
            this.statusBarToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.statusBarToolStripMenuItem.Text = "Status Bar";
            this.statusBarToolStripMenuItem.Click += new System.EventHandler(this.statusBarToolStripMenuItem_Click);
            // 
            // toolBarToolStripMenuItem
            // 
            this.toolBarToolStripMenuItem.Checked = true;
            this.toolBarToolStripMenuItem.CheckOnClick = true;
            this.toolBarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolBarToolStripMenuItem.Name = "toolBarToolStripMenuItem";
            this.toolBarToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.toolBarToolStripMenuItem.Text = "Tool Bar";
            this.toolBarToolStripMenuItem.Click += new System.EventHandler(this.toolBarToolStripMenuItem_Click);
            // 
            // channelBarToolStripMenuItem
            // 
            this.channelBarToolStripMenuItem.Checked = true;
            this.channelBarToolStripMenuItem.CheckOnClick = true;
            this.channelBarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.channelBarToolStripMenuItem.Name = "channelBarToolStripMenuItem";
            this.channelBarToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.channelBarToolStripMenuItem.Text = "Channel Bar";
            this.channelBarToolStripMenuItem.Click += new System.EventHandler(this.channelBarToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(178, 6);
            // 
            // selectNickListToolStripMenuItem
            // 
            this.selectNickListToolStripMenuItem.Name = "selectNickListToolStripMenuItem";
            this.selectNickListToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.selectNickListToolStripMenuItem.Text = "Select Nick List";
            this.selectNickListToolStripMenuItem.Click += new System.EventHandler(this.selectNickListToolStripMenuItem_Click);
            // 
            // selectServerTreeToolStripMenuItem
            // 
            this.selectServerTreeToolStripMenuItem.Name = "selectServerTreeToolStripMenuItem";
            this.selectServerTreeToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.selectServerTreeToolStripMenuItem.Text = "Select Server Tree";
            this.selectServerTreeToolStripMenuItem.Click += new System.EventHandler(this.selectServerTreeToolStripMenuItem_Click);
            // 
            // selectInputBoxToolStripMenuItem
            // 
            this.selectInputBoxToolStripMenuItem.Name = "selectInputBoxToolStripMenuItem";
            this.selectInputBoxToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.selectInputBoxToolStripMenuItem.Text = "Select Input box";
            this.selectInputBoxToolStripMenuItem.Click += new System.EventHandler(this.selectInputBoxToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.BackColor = System.Drawing.SystemColors.Menu;
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.codePlexPageToolStripMenuItem,
            this.iceChatHomePageToolStripMenuItem,
            this.forumsToolStripMenuItem,
            this.facebookFanPageToolStripMenuItem,
            this.downloadPluginsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.browseDataFolderToolStripMenuItem,
            this.checkForUpdateToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.ForeColor = System.Drawing.SystemColors.MenuText;
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(47, 21);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // codePlexPageToolStripMenuItem
            // 
            this.codePlexPageToolStripMenuItem.Name = "codePlexPageToolStripMenuItem";
            this.codePlexPageToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.codePlexPageToolStripMenuItem.Text = "CodePlex Page";
            this.codePlexPageToolStripMenuItem.Click += new System.EventHandler(this.codePlexPageToolStripMenuItem_Click);
            // 
            // iceChatHomePageToolStripMenuItem
            // 
            this.iceChatHomePageToolStripMenuItem.Name = "iceChatHomePageToolStripMenuItem";
            this.iceChatHomePageToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.iceChatHomePageToolStripMenuItem.Text = "IceChat Home Page";
            this.iceChatHomePageToolStripMenuItem.Click += new System.EventHandler(this.iceChatHomePageToolStripMenuItem_Click);
            // 
            // forumsToolStripMenuItem
            // 
            this.forumsToolStripMenuItem.Name = "forumsToolStripMenuItem";
            this.forumsToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.forumsToolStripMenuItem.Text = "Forums";
            this.forumsToolStripMenuItem.Click += new System.EventHandler(this.forumsToolStripMenuItem_Click);
            // 
            // facebookFanPageToolStripMenuItem
            // 
            this.facebookFanPageToolStripMenuItem.Name = "facebookFanPageToolStripMenuItem";
            this.facebookFanPageToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.facebookFanPageToolStripMenuItem.Text = "Facebook Fan page";
            this.facebookFanPageToolStripMenuItem.Click += new System.EventHandler(this.facebookFanPageToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(187, 6);
            // 
            // browseDataFolderToolStripMenuItem
            // 
            this.browseDataFolderToolStripMenuItem.Name = "browseDataFolderToolStripMenuItem";
            this.browseDataFolderToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.browseDataFolderToolStripMenuItem.Text = "Browse Data Folder";
            this.browseDataFolderToolStripMenuItem.Click += new System.EventHandler(this.browseDataFolderToolStripMenuItem_Click);
            // 
            // checkForUpdateToolStripMenuItem
            // 
            this.checkForUpdateToolStripMenuItem.Name = "checkForUpdateToolStripMenuItem";
            this.checkForUpdateToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.checkForUpdateToolStripMenuItem.Text = "Check for Update";
            this.checkForUpdateToolStripMenuItem.Click += new System.EventHandler(this.checkForUpdateToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // splitterLeft
            // 
            this.splitterLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.splitterLeft.Location = new System.Drawing.Point(181, 50);
            this.splitterLeft.Name = "splitterLeft";
            this.splitterLeft.Size = new System.Drawing.Size(3, 358);
            this.splitterLeft.TabIndex = 15;
            this.splitterLeft.TabStop = false;
            // 
            // splitterRight
            // 
            this.splitterRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.splitterRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitterRight.Location = new System.Drawing.Point(618, 50);
            this.splitterRight.Name = "splitterRight";
            this.splitterRight.Size = new System.Drawing.Size(3, 358);
            this.splitterRight.TabIndex = 16;
            this.splitterRight.TabStop = false;
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuNotify;
            this.notifyIcon.Text = "IceChat 2009";
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIconMouseDoubleClick);
            // 
            // contextMenuNotify
            // 
            this.contextMenuNotify.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.restoreToolStripMenuItem,
            this.exitToolStripMenuItem1});
            this.contextMenuNotify.Name = "contextMenuNotify";
            this.contextMenuNotify.Size = new System.Drawing.Size(114, 48);
            // 
            // restoreToolStripMenuItem
            // 
            this.restoreToolStripMenuItem.Name = "restoreToolStripMenuItem";
            this.restoreToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.restoreToolStripMenuItem.Text = "Restore";
            this.restoreToolStripMenuItem.Click += new System.EventHandler(this.restoreToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem1
            // 
            this.exitToolStripMenuItem1.Name = "exitToolStripMenuItem1";
            this.exitToolStripMenuItem1.Size = new System.Drawing.Size(113, 22);
            this.exitToolStripMenuItem1.Text = "Exit";
            this.exitToolStripMenuItem1.Click += new System.EventHandler(this.exitToolStripMenuItem1_Click);
            // 
            // toolStripMain
            // 
            this.toolStripMain.AccessibleDescription = "Main Tool bar";
            this.toolStripMain.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar;
            this.toolStripMain.AllowItemReorder = true;
            this.toolStripMain.ContextMenuStrip = this.contextMenuToolBar;
            this.toolStripMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripMain.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripQuickConnect,
            this.toolStripSettings,
            this.toolStripColors,
            this.toolStripEditor,
            this.toolStripAway,
            this.toolStripSystemTray,
            this.toolStripUpdate});
            this.toolStripMain.Location = new System.Drawing.Point(0, 25);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStripMain.Size = new System.Drawing.Size(796, 25);
            this.toolStripMain.TabIndex = 17;
            this.toolStripMain.Text = "toolStripMain";
            // 
            // contextMenuToolBar
            // 
            this.contextMenuToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideToolStripMenuItem});
            this.contextMenuToolBar.Name = "contextMenuToolBar";
            this.contextMenuToolBar.Size = new System.Drawing.Size(100, 26);
            // 
            // hideToolStripMenuItem
            // 
            this.hideToolStripMenuItem.Name = "hideToolStripMenuItem";
            this.hideToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
            this.hideToolStripMenuItem.Text = "Hide";
            this.hideToolStripMenuItem.Click += new System.EventHandler(this.hideToolStripMenuItem_Click);
            // 
            // toolStripQuickConnect
            // 
            this.toolStripQuickConnect.AccessibleDescription = "Bring up Quick Connect Window";
            this.toolStripQuickConnect.BackColor = System.Drawing.Color.Transparent;
            this.toolStripQuickConnect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripQuickConnect.Margin = new System.Windows.Forms.Padding(0);
            this.toolStripQuickConnect.Name = "toolStripQuickConnect";
            this.toolStripQuickConnect.Size = new System.Drawing.Size(90, 25);
            this.toolStripQuickConnect.Text = "Quick Connect";
            this.toolStripQuickConnect.Click += new System.EventHandler(this.toolStripQuickConnect_Click);
            // 
            // toolStripSettings
            // 
            this.toolStripSettings.AccessibleDescription = "Open settings window";
            this.toolStripSettings.BackColor = System.Drawing.Color.Transparent;
            this.toolStripSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSettings.Name = "toolStripSettings";
            this.toolStripSettings.Size = new System.Drawing.Size(53, 22);
            this.toolStripSettings.Text = "Settings";
            this.toolStripSettings.Click += new System.EventHandler(this.toolStripSettings_Click);
            // 
            // toolStripColors
            // 
            this.toolStripColors.AccessibleDescription = "Open color settings window";
            this.toolStripColors.BackColor = System.Drawing.Color.Transparent;
            this.toolStripColors.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripColors.Name = "toolStripColors";
            this.toolStripColors.Size = new System.Drawing.Size(45, 22);
            this.toolStripColors.Text = "Colors";
            this.toolStripColors.Click += new System.EventHandler(this.toolStripColors_Click);
            // 
            // toolStripEditor
            // 
            this.toolStripEditor.AccessibleDescription = "Open IceChat Editor";
            this.toolStripEditor.BackColor = System.Drawing.Color.Transparent;
            this.toolStripEditor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripEditor.Name = "toolStripEditor";
            this.toolStripEditor.Size = new System.Drawing.Size(42, 22);
            this.toolStripEditor.Text = "Editor";
            this.toolStripEditor.Click += new System.EventHandler(this.toolStripEditor_Click);
            // 
            // toolStripAway
            // 
            this.toolStripAway.AccessibleDescription = "Set yourself as away, or return";
            this.toolStripAway.BackColor = System.Drawing.Color.Transparent;
            this.toolStripAway.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripAway.Name = "toolStripAway";
            this.toolStripAway.Size = new System.Drawing.Size(59, 22);
            this.toolStripAway.Text = "Set Away";
            this.toolStripAway.ToolTipText = "Set Away";
            this.toolStripAway.Click += new System.EventHandler(this.toolStripAway_Click);
            // 
            // toolStripSystemTray
            // 
            this.toolStripSystemTray.AccessibleDescription = "Put IceChat on the System Tray";
            this.toolStripSystemTray.BackColor = System.Drawing.Color.Transparent;
            this.toolStripSystemTray.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSystemTray.Name = "toolStripSystemTray";
            this.toolStripSystemTray.Size = new System.Drawing.Size(75, 22);
            this.toolStripSystemTray.Text = "System Tray";
            this.toolStripSystemTray.Click += new System.EventHandler(this.toolStripSystemTray_Click);
            // 
            // toolStripUpdate
            // 
            this.toolStripUpdate.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripUpdate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripUpdate.Name = "toolStripUpdate";
            this.toolStripUpdate.Size = new System.Drawing.Size(100, 22);
            this.toolStripUpdate.Text = "Update Available";
            this.toolStripUpdate.Click += new System.EventHandler(this.toolStripUpdate_Click);
            // 
            // statusStripMain
            // 
            this.statusStripMain.AccessibleDescription = "Main status bar";
            this.statusStripMain.AccessibleRole = System.Windows.Forms.AccessibleRole.StatusBar;
            this.statusStripMain.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatus});
            this.statusStripMain.Location = new System.Drawing.Point(0, 434);
            this.statusStripMain.Name = "statusStripMain";
            this.statusStripMain.Size = new System.Drawing.Size(796, 22);
            this.statusStripMain.SizingGrip = false;
            this.statusStripMain.TabIndex = 18;
            // 
            // toolStripStatus
            // 
            this.toolStripStatus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatus.Name = "toolStripStatus";
            this.toolStripStatus.Size = new System.Drawing.Size(58, 17);
            this.toolStripStatus.Text = "Status:";
            // 
            // mainTabControl
            // 
            this.mainTabControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl.Location = new System.Drawing.Point(184, 50);
            this.mainTabControl.Margin = new System.Windows.Forms.Padding(0);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = -1;
            this.mainTabControl.Size = new System.Drawing.Size(434, 358);
            this.mainTabControl.TabIndex = 20;
            // 
            // panelDockRight
            // 
            this.panelDockRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelDockRight.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelDockRight.Location = new System.Drawing.Point(621, 50);
            this.panelDockRight.Name = "panelDockRight";
            this.panelDockRight.Size = new System.Drawing.Size(175, 358);
            this.panelDockRight.TabIndex = 14;
            // 
            // panelDockLeft
            // 
            this.panelDockLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelDockLeft.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelDockLeft.Location = new System.Drawing.Point(0, 50);
            this.panelDockLeft.Name = "panelDockLeft";
            this.panelDockLeft.Size = new System.Drawing.Size(181, 358);
            this.panelDockLeft.TabIndex = 13;
            // 
            // inputPanel
            // 
            this.inputPanel.AccessibleDescription = "";
            this.inputPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.inputPanel.Location = new System.Drawing.Point(0, 408);
            this.inputPanel.Name = "inputPanel";
            this.inputPanel.Size = new System.Drawing.Size(796, 26);
            this.inputPanel.TabIndex = 0;
            // 
            // downloadPluginsToolStripMenuItem
            // 
            this.downloadPluginsToolStripMenuItem.Name = "downloadPluginsToolStripMenuItem";
            this.downloadPluginsToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.downloadPluginsToolStripMenuItem.Text = "Download Plugins";
            this.downloadPluginsToolStripMenuItem.Click += new System.EventHandler(this.downloadPluginsToolStripMenuItem_Click);
            // 
            // FormMain
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(796, 456);
            this.Controls.Add(this.mainTabControl);
            this.Controls.Add(this.splitterRight);
            this.Controls.Add(this.splitterLeft);
            this.Controls.Add(this.panelDockRight);
            this.Controls.Add(this.panelDockLeft);
            this.Controls.Add(this.inputPanel);
            this.Controls.Add(this.statusStripMain);
            this.Controls.Add(this.toolStripMain);
            this.Controls.Add(this.menuMainStrip);
            this.MainMenuStrip = this.menuMainStrip;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "IceChat 2009";
            this.menuMainStrip.ResumeLayout(false);
            this.menuMainStrip.PerformLayout();
            this.contextMenuNotify.ResumeLayout(false);
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.contextMenuToolBar.ResumeLayout(false);
            this.statusStripMain.ResumeLayout(false);
            this.statusStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.ToolStripMenuItem debugWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem codePlexPageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iceChatHomePageToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStripMain;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatus;
        private System.Windows.Forms.ContextMenuStrip contextMenuNotify;
        private System.Windows.Forms.ToolStripMenuItem restoreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem pluginsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem serverListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nickListToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripAway;
        private IceTabControl mainTabControl;
        private System.Windows.Forms.ToolStripMenuItem statusBarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolBarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem facebookFanPageToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem browseDataFolderToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem closeCurrentWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem selectNickListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectServerTreeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem selectInputBoxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem channelBarToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripUpdate;
        private System.Windows.Forms.ToolStripMenuItem muteAllSoundsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadAPluginToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadPluginsToolStripMenuItem;




    }
}

