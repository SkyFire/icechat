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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.menuMainStrip = new System.Windows.Forms.MenuStrip();
            this.mainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.minimizeToTrayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iceChatSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iceChatColorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iceChatEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.serverListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nickListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusBarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolBarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.codePlexPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iceChatHomePageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forumsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelDockRight = new IceDockPanel();
            this.splitterLeft = new System.Windows.Forms.Splitter();
            this.splitterRight = new System.Windows.Forms.Splitter();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuNotify = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.restoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.panelDockLeft = new IceDockPanel();
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.contextMenuToolBar = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.hideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripQuickConnect = new System.Windows.Forms.ToolStripButton();
            this.toolStripSettings = new System.Windows.Forms.ToolStripButton();
            this.toolStripColors = new System.Windows.Forms.ToolStripButton();
            this.toolStripEditor = new System.Windows.Forms.ToolStripButton();
            this.toolStripAway = new System.Windows.Forms.ToolStripButton();
            this.toolStripSystemTray = new System.Windows.Forms.ToolStripButton();
            this.statusStripMain = new System.Windows.Forms.StatusStrip();
            this.toolStripStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.mainTabControl = new IceChat.IceTabControl();
            this.inputPanel = new IceChat.InputPanel();
            this.menuMainStrip.SuspendLayout();
            this.contextMenuNotify.SuspendLayout();
            this.panelDockLeft.SuspendLayout();
            this.toolStripMain.SuspendLayout();
            this.contextMenuToolBar.SuspendLayout();
            this.statusStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuMainStrip
            // 
            this.menuMainStrip.AllowItemReorder = true;
            this.menuMainStrip.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuMainStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mainToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuMainStrip.Location = new System.Drawing.Point(0, 0);
            this.menuMainStrip.Name = "menuMainStrip";
            this.menuMainStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuMainStrip.Size = new System.Drawing.Size(796, 24);
            this.menuMainStrip.TabIndex = 12;
            this.menuMainStrip.Text = "menuStripMain";
            // 
            // mainToolStripMenuItem
            // 
            this.mainToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.minimizeToTrayToolStripMenuItem,
            this.debugWindowToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.mainToolStripMenuItem.Name = "mainToolStripMenuItem";
            this.mainToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.mainToolStripMenuItem.Text = "Main";
            // 
            // minimizeToTrayToolStripMenuItem
            // 
            this.minimizeToTrayToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("minimizeToTrayToolStripMenuItem.Image")));
            this.minimizeToTrayToolStripMenuItem.Name = "minimizeToTrayToolStripMenuItem";
            this.minimizeToTrayToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.minimizeToTrayToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.minimizeToTrayToolStripMenuItem.Text = "Minimize to Tray";
            this.minimizeToTrayToolStripMenuItem.Click += new System.EventHandler(this.minimizeToTrayToolStripMenuItem_Click);
            // 
            // debugWindowToolStripMenuItem
            // 
            this.debugWindowToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("debugWindowToolStripMenuItem.Image")));
            this.debugWindowToolStripMenuItem.Name = "debugWindowToolStripMenuItem";
            this.debugWindowToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.debugWindowToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.debugWindowToolStripMenuItem.Text = "Debug Window";
            this.debugWindowToolStripMenuItem.Click += new System.EventHandler(this.debugWindowToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("exitToolStripMenuItem.Image")));
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iceChatSettingsToolStripMenuItem,
            this.iceChatColorsToolStripMenuItem,
            this.iceChatEditorToolStripMenuItem,
            this.pluginsToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(70, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // iceChatSettingsToolStripMenuItem
            // 
            this.iceChatSettingsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("iceChatSettingsToolStripMenuItem.Image")));
            this.iceChatSettingsToolStripMenuItem.Name = "iceChatSettingsToolStripMenuItem";
            this.iceChatSettingsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.iceChatSettingsToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
            this.iceChatSettingsToolStripMenuItem.Text = "Program Settings";
            this.iceChatSettingsToolStripMenuItem.Click += new System.EventHandler(this.iceChatSettingsToolStripMenuItem_Click);
            // 
            // iceChatColorsToolStripMenuItem
            // 
            this.iceChatColorsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("iceChatColorsToolStripMenuItem.Image")));
            this.iceChatColorsToolStripMenuItem.Name = "iceChatColorsToolStripMenuItem";
            this.iceChatColorsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.iceChatColorsToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
            this.iceChatColorsToolStripMenuItem.Text = "Colors Settings";
            this.iceChatColorsToolStripMenuItem.Click += new System.EventHandler(this.iceChatColorsToolStripMenuItem_Click);
            // 
            // iceChatEditorToolStripMenuItem
            // 
            this.iceChatEditorToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("iceChatEditorToolStripMenuItem.Image")));
            this.iceChatEditorToolStripMenuItem.Name = "iceChatEditorToolStripMenuItem";
            this.iceChatEditorToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.iceChatEditorToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
            this.iceChatEditorToolStripMenuItem.Text = "IceChat Editor";
            this.iceChatEditorToolStripMenuItem.Click += new System.EventHandler(this.iceChatEditorToolStripMenuItem_Click);
            // 
            // pluginsToolStripMenuItem
            // 
            this.pluginsToolStripMenuItem.Name = "pluginsToolStripMenuItem";
            this.pluginsToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
            this.pluginsToolStripMenuItem.Text = "Loaded Plugins";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.serverListToolStripMenuItem,
            this.nickListToolStripMenuItem,
            this.statusBarToolStripMenuItem,
            this.toolBarToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // serverListToolStripMenuItem
            // 
            this.serverListToolStripMenuItem.Checked = true;
            this.serverListToolStripMenuItem.CheckOnClick = true;
            this.serverListToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.serverListToolStripMenuItem.Name = "serverListToolStripMenuItem";
            this.serverListToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.serverListToolStripMenuItem.Text = "Left Panel";
            this.serverListToolStripMenuItem.Click += new System.EventHandler(this.serverListToolStripMenuItem_Click);
            // 
            // nickListToolStripMenuItem
            // 
            this.nickListToolStripMenuItem.Checked = true;
            this.nickListToolStripMenuItem.CheckOnClick = true;
            this.nickListToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.nickListToolStripMenuItem.Name = "nickListToolStripMenuItem";
            this.nickListToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.nickListToolStripMenuItem.Text = "Right Panel";
            this.nickListToolStripMenuItem.Click += new System.EventHandler(this.nickListToolStripMenuItem_Click);
            // 
            // statusBarToolStripMenuItem
            // 
            this.statusBarToolStripMenuItem.Checked = true;
            this.statusBarToolStripMenuItem.CheckOnClick = true;
            this.statusBarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.statusBarToolStripMenuItem.Name = "statusBarToolStripMenuItem";
            this.statusBarToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.statusBarToolStripMenuItem.Text = "Status Bar";
            this.statusBarToolStripMenuItem.Click += new System.EventHandler(this.statusBarToolStripMenuItem_Click);
            // 
            // toolBarToolStripMenuItem
            // 
            this.toolBarToolStripMenuItem.Checked = true;
            this.toolBarToolStripMenuItem.CheckOnClick = true;
            this.toolBarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolBarToolStripMenuItem.Name = "toolBarToolStripMenuItem";
            this.toolBarToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.toolBarToolStripMenuItem.Text = "Tool Bar";
            this.toolBarToolStripMenuItem.Click += new System.EventHandler(this.toolBarToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.codePlexPageToolStripMenuItem,
            this.iceChatHomePageToolStripMenuItem,
            this.forumsToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.checkForUpdateToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // codePlexPageToolStripMenuItem
            // 
            this.codePlexPageToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("codePlexPageToolStripMenuItem.Image")));
            this.codePlexPageToolStripMenuItem.Name = "codePlexPageToolStripMenuItem";
            this.codePlexPageToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.codePlexPageToolStripMenuItem.Text = "CodePlex Page";
            this.codePlexPageToolStripMenuItem.Click += new System.EventHandler(this.codePlexPageToolStripMenuItem_Click);
            // 
            // iceChatHomePageToolStripMenuItem
            // 
            this.iceChatHomePageToolStripMenuItem.Name = "iceChatHomePageToolStripMenuItem";
            this.iceChatHomePageToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.iceChatHomePageToolStripMenuItem.Text = "IceChat Home Page";
            this.iceChatHomePageToolStripMenuItem.Click += new System.EventHandler(this.iceChatHomePageToolStripMenuItem_Click);
            // 
            // forumsToolStripMenuItem
            // 
            this.forumsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("forumsToolStripMenuItem.Image")));
            this.forumsToolStripMenuItem.Name = "forumsToolStripMenuItem";
            this.forumsToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.forumsToolStripMenuItem.Text = "Forums";
            this.forumsToolStripMenuItem.Click += new System.EventHandler(this.forumsToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // checkForUpdateToolStripMenuItem
            // 
            this.checkForUpdateToolStripMenuItem.Name = "checkForUpdateToolStripMenuItem";
            this.checkForUpdateToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.checkForUpdateToolStripMenuItem.Text = "Check for Update";
            this.checkForUpdateToolStripMenuItem.Click += new System.EventHandler(this.checkForUpdateToolStripMenuItem_Click);
            // 
            // panelDockRight
            // 
            this.panelDockRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelDockRight.Location = new System.Drawing.Point(621, 63);
            this.panelDockRight.Name = "panelDockRight";
            this.panelDockRight.Size = new System.Drawing.Size(175, 345);
            this.panelDockRight.TabIndex = 14;
            this.panelDockRight.TabControl.Alignment = System.Windows.Forms.TabAlignment.Right;
            // 
            // splitterLeft
            // 
            this.splitterLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.splitterLeft.Location = new System.Drawing.Point(181, 63);
            this.splitterLeft.Name = "splitterLeft";
            this.splitterLeft.Size = new System.Drawing.Size(3, 345);
            this.splitterLeft.TabIndex = 15;
            this.splitterLeft.TabStop = false;
            // 
            // splitterRight
            // 
            this.splitterRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.splitterRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitterRight.Location = new System.Drawing.Point(618, 63);
            this.splitterRight.Name = "splitterRight";
            this.splitterRight.Size = new System.Drawing.Size(3, 345);
            this.splitterRight.TabIndex = 16;
            this.splitterRight.TabStop = false;
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuNotify;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
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
            // panelDockLeft
            // 
            this.panelDockLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelDockLeft.Location = new System.Drawing.Point(0, 63);
            this.panelDockLeft.Name = "panelDockLeft";
            this.panelDockLeft.Size = new System.Drawing.Size(181, 345);
            this.panelDockLeft.TabIndex = 13;
            this.panelDockLeft.TabControl.Alignment = System.Windows.Forms.TabAlignment.Left;
            // 
            // toolStripMain
            // 
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
            this.toolStripSystemTray});
            this.toolStripMain.Location = new System.Drawing.Point(0, 24);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStripMain.Size = new System.Drawing.Size(796, 39);
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
            this.toolStripQuickConnect.BackColor = System.Drawing.Color.Transparent;
            this.toolStripQuickConnect.Image = ((System.Drawing.Image)(resources.GetObject("toolStripQuickConnect.Image")));
            this.toolStripQuickConnect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripQuickConnect.Margin = new System.Windows.Forms.Padding(0);
            this.toolStripQuickConnect.Name = "toolStripQuickConnect";
            this.toolStripQuickConnect.Size = new System.Drawing.Size(122, 39);
            this.toolStripQuickConnect.Text = "Quick Connect";
            this.toolStripQuickConnect.Click += new System.EventHandler(this.toolStripQuickConnect_Click);
            // 
            // toolStripSettings
            // 
            this.toolStripSettings.BackColor = System.Drawing.Color.Transparent;
            this.toolStripSettings.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSettings.Image")));
            this.toolStripSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSettings.Name = "toolStripSettings";
            this.toolStripSettings.Size = new System.Drawing.Size(85, 36);
            this.toolStripSettings.Text = "Settings";
            this.toolStripSettings.Click += new System.EventHandler(this.toolStripSettings_Click);
            // 
            // toolStripColors
            // 
            this.toolStripColors.BackColor = System.Drawing.Color.Transparent;
            this.toolStripColors.Image = ((System.Drawing.Image)(resources.GetObject("toolStripColors.Image")));
            this.toolStripColors.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripColors.Name = "toolStripColors";
            this.toolStripColors.Size = new System.Drawing.Size(77, 36);
            this.toolStripColors.Text = "Colors";
            this.toolStripColors.Click += new System.EventHandler(this.toolStripColors_Click);
            // 
            // toolStripEditor
            // 
            this.toolStripEditor.BackColor = System.Drawing.Color.Transparent;
            this.toolStripEditor.Image = ((System.Drawing.Image)(resources.GetObject("toolStripEditor.Image")));
            this.toolStripEditor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripEditor.Name = "toolStripEditor";
            this.toolStripEditor.Size = new System.Drawing.Size(74, 36);
            this.toolStripEditor.Text = "Editor";
            this.toolStripEditor.Click += new System.EventHandler(this.toolStripEditor_Click);
            // 
            // toolStripAway
            // 
            this.toolStripAway.BackColor = System.Drawing.Color.Transparent;
            this.toolStripAway.Image = ((System.Drawing.Image)(resources.GetObject("toolStripAway.Image")));
            this.toolStripAway.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripAway.Name = "toolStripAway";
            this.toolStripAway.Size = new System.Drawing.Size(91, 36);
            this.toolStripAway.Text = "Set Away";
            this.toolStripAway.ToolTipText = "Set Away";
            this.toolStripAway.Click += new System.EventHandler(this.toolStripAway_Click);
            // 
            // toolStripSystemTray
            // 
            this.toolStripSystemTray.BackColor = System.Drawing.Color.Transparent;
            this.toolStripSystemTray.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSystemTray.Image")));
            this.toolStripSystemTray.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSystemTray.Name = "toolStripSystemTray";
            this.toolStripSystemTray.Size = new System.Drawing.Size(107, 36);
            this.toolStripSystemTray.Text = "System Tray";
            this.toolStripSystemTray.Click += new System.EventHandler(this.toolStripSystemTray_Click);
            // 
            // statusStripMain
            // 
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
            this.mainTabControl.Location = new System.Drawing.Point(184, 63);
            this.mainTabControl.Margin = new System.Windows.Forms.Padding(0);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = -1;
            this.mainTabControl.Size = new System.Drawing.Size(434, 345);
            this.mainTabControl.TabIndex = 20;
            // 
            // inputPanel
            // 
            this.inputPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.inputPanel.Location = new System.Drawing.Point(0, 408);
            this.inputPanel.Name = "inputPanel";
            this.inputPanel.Size = new System.Drawing.Size(796, 26);
            this.inputPanel.TabIndex = 0;
            // 
            // FormMain
            // 
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
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuMainStrip;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "IceChat 2009";
            this.menuMainStrip.ResumeLayout(false);
            this.menuMainStrip.PerformLayout();
            this.contextMenuNotify.ResumeLayout(false);
            this.panelDockLeft.ResumeLayout(false);
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




    }
}

