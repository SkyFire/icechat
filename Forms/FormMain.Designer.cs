namespace IceChat2009
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
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.Splitter splitterLeft;
        private System.Windows.Forms.Splitter splitterRight;

        private ServerTree serverTree;
        private System.Windows.Forms.ToolStripMenuItem iceChatColorsToolStripMenuItem;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Panel panelRightBottom;
        private System.Windows.Forms.TabControl tabPanelRight;
        private System.Windows.Forms.TabPage tabPageNicks;
        private System.Windows.Forms.TabPage tabPageFaveChannels;
        private ChannelList channelList;
        private System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.ToolStripButton toolStripQuickConnect;
        private System.Windows.Forms.ToolStripButton toolStripSettings;
        private System.Windows.Forms.ToolStripButton toolStripColors;
        private System.Windows.Forms.ToolStripButton toolStripSystemTray;
        private System.Windows.Forms.ContextMenuStrip contextMenuToolBar;
        private System.Windows.Forms.ToolStripMenuItem hideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iceChatEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripEditor;
        private System.Windows.Forms.ToolStripMenuItem forumsToolStripMenuItem;


        private System.Windows.Forms.ImageList tabPageImages;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private NickList nickList;
        private IceTabControl tabMain;
        private System.Windows.Forms.ToolStripMenuItem minimizeToTrayToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ToolStripMenuItem iceChatSettingsToolStripMenuItem;

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
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.codePlexPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iceChatHomePageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forumsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelRight = new System.Windows.Forms.Panel();
            this.panelRightBottom = new System.Windows.Forms.Panel();
            this.tabPanelRight = new System.Windows.Forms.TabControl();
            this.tabPageNicks = new System.Windows.Forms.TabPage();
            this.tabPageFaveChannels = new System.Windows.Forms.TabPage();
            this.splitterLeft = new System.Windows.Forms.Splitter();
            this.splitterRight = new System.Windows.Forms.Splitter();
            this.tabPageImages = new System.Windows.Forms.ImageList(this.components);
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuNotify = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.restoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.contextMenuToolBar = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.hideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripQuickConnect = new System.Windows.Forms.ToolStripButton();
            this.toolStripSettings = new System.Windows.Forms.ToolStripButton();
            this.toolStripColors = new System.Windows.Forms.ToolStripButton();
            this.toolStripEditor = new System.Windows.Forms.ToolStripButton();
            this.toolStripSystemTray = new System.Windows.Forms.ToolStripButton();
            this.statusStripMain = new System.Windows.Forms.StatusStrip();
            this.toolStripStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.inputPanel = new IceChat2009.InputPanel();
            this.tabMain = new IceChat2009.IceTabControl();
            this.menuMainStrip.SuspendLayout();
            this.panelRightBottom.SuspendLayout();
            this.tabPanelRight.SuspendLayout();
            this.contextMenuNotify.SuspendLayout();
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
            this.minimizeToTrayToolStripMenuItem.Name = "minimizeToTrayToolStripMenuItem";
            this.minimizeToTrayToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.minimizeToTrayToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.minimizeToTrayToolStripMenuItem.Text = "Minimize to Tray";
            this.minimizeToTrayToolStripMenuItem.Click += new System.EventHandler(this.minimizeToTrayToolStripMenuItem_Click);
            // 
            // debugWindowToolStripMenuItem
            // 
            this.debugWindowToolStripMenuItem.Name = "debugWindowToolStripMenuItem";
            this.debugWindowToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.debugWindowToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.debugWindowToolStripMenuItem.Text = "Debug Window";
            this.debugWindowToolStripMenuItem.Click += new System.EventHandler(this.debugWindowToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
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
            this.iceChatEditorToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(70, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // iceChatSettingsToolStripMenuItem
            // 
            this.iceChatSettingsToolStripMenuItem.Name = "iceChatSettingsToolStripMenuItem";
            this.iceChatSettingsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.iceChatSettingsToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
            this.iceChatSettingsToolStripMenuItem.Text = "Program Settings";
            this.iceChatSettingsToolStripMenuItem.Click += new System.EventHandler(this.iceChatSettingsToolStripMenuItem_Click);
            // 
            // iceChatColorsToolStripMenuItem
            // 
            this.iceChatColorsToolStripMenuItem.Name = "iceChatColorsToolStripMenuItem";
            this.iceChatColorsToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
            this.iceChatColorsToolStripMenuItem.Text = "Colors Settings";
            this.iceChatColorsToolStripMenuItem.Click += new System.EventHandler(this.iceChatColorsToolStripMenuItem_Click);
            // 
            // iceChatEditorToolStripMenuItem
            // 
            this.iceChatEditorToolStripMenuItem.Name = "iceChatEditorToolStripMenuItem";
            this.iceChatEditorToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.iceChatEditorToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
            this.iceChatEditorToolStripMenuItem.Text = "IceChat Editor";
            this.iceChatEditorToolStripMenuItem.Click += new System.EventHandler(this.iceChatEditorToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.codePlexPageToolStripMenuItem,
            this.iceChatHomePageToolStripMenuItem,
            this.forumsToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // codePlexPageToolStripMenuItem
            // 
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
            // panelRight
            // 
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(621, 63);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(175, 465);
            this.panelRight.TabIndex = 14;
            // 
            // panelRightBottom
            // 
            this.panelRightBottom.BackColor = System.Drawing.SystemColors.Control;
            this.panelRightBottom.Controls.Add(this.tabPanelRight);
            this.panelRightBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelRightBottom.Location = new System.Drawing.Point(0, 451);
            this.panelRightBottom.Name = "panelRightBottom";
            this.panelRightBottom.Size = new System.Drawing.Size(175, 22);
            this.panelRightBottom.TabIndex = 4;
            // 
            // tabPanelRight
            // 
            this.tabPanelRight.Controls.Add(this.tabPageNicks);
            this.tabPanelRight.Controls.Add(this.tabPageFaveChannels);
            this.tabPanelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabPanelRight.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPanelRight.Location = new System.Drawing.Point(0, 0);
            this.tabPanelRight.Name = "tabPanelRight";
            this.tabPanelRight.SelectedIndex = 0;
            this.tabPanelRight.Size = new System.Drawing.Size(175, 22);
            this.tabPanelRight.TabIndex = 0;
            // 
            // tabPageNicks
            // 
            this.tabPageNicks.Location = new System.Drawing.Point(4, 22);
            this.tabPageNicks.Name = "tabPageNicks";
            this.tabPageNicks.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageNicks.Size = new System.Drawing.Size(167, 0);
            this.tabPageNicks.TabIndex = 0;
            this.tabPageNicks.Text = "Nick List";
            this.tabPageNicks.UseVisualStyleBackColor = true;
            // 
            // tabPageFaveChannels
            // 
            this.tabPageFaveChannels.Location = new System.Drawing.Point(4, 22);
            this.tabPageFaveChannels.Name = "tabPageFaveChannels";
            this.tabPageFaveChannels.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageFaveChannels.Size = new System.Drawing.Size(167, 0);
            this.tabPageFaveChannels.TabIndex = 1;
            this.tabPageFaveChannels.Text = "Fave Channels";
            this.tabPageFaveChannels.UseVisualStyleBackColor = true;
            // 
            // splitterLeft
            // 
            this.splitterLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.splitterLeft.Location = new System.Drawing.Point(181, 63);
            this.splitterLeft.Name = "splitterLeft";
            this.splitterLeft.Size = new System.Drawing.Size(3, 465);
            this.splitterLeft.TabIndex = 15;
            this.splitterLeft.TabStop = false;
            // 
            // splitterRight
            // 
            this.splitterRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.splitterRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitterRight.Location = new System.Drawing.Point(618, 63);
            this.splitterRight.Name = "splitterRight";
            this.splitterRight.Size = new System.Drawing.Size(3, 465);
            this.splitterRight.TabIndex = 16;
            this.splitterRight.TabStop = false;
            // 
            // tabPageImages
            // 
            this.tabPageImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("tabPageImages.ImageStream")));
            this.tabPageImages.TransparentColor = System.Drawing.Color.Transparent;
            this.tabPageImages.Images.SetKeyName(0, "console.ico");
            this.tabPageImages.Images.SetKeyName(1, "channel.ico");
            this.tabPageImages.Images.SetKeyName(2, "query.ico");
            this.tabPageImages.Images.SetKeyName(3, "close.ico");
            this.tabPageImages.Images.SetKeyName(4, "window.ico");
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
            // panelLeft
            // 
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 63);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(181, 465);
            this.panelLeft.TabIndex = 13;
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
            this.toolStripSettings.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSettings.Image")));
            this.toolStripSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSettings.Name = "toolStripSettings";
            this.toolStripSettings.Size = new System.Drawing.Size(85, 36);
            this.toolStripSettings.Text = "Settings";
            this.toolStripSettings.Click += new System.EventHandler(this.toolStripSettings_Click);
            // 
            // toolStripColors
            // 
            this.toolStripColors.Image = ((System.Drawing.Image)(resources.GetObject("toolStripColors.Image")));
            this.toolStripColors.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripColors.Name = "toolStripColors";
            this.toolStripColors.Size = new System.Drawing.Size(77, 36);
            this.toolStripColors.Text = "Colors";
            this.toolStripColors.Click += new System.EventHandler(this.toolStripColors_Click);
            // 
            // toolStripEditor
            // 
            this.toolStripEditor.Image = ((System.Drawing.Image)(resources.GetObject("toolStripEditor.Image")));
            this.toolStripEditor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripEditor.Name = "toolStripEditor";
            this.toolStripEditor.Size = new System.Drawing.Size(74, 36);
            this.toolStripEditor.Text = "Editor";
            this.toolStripEditor.Click += new System.EventHandler(this.toolStripEditor_Click);
            // 
            // toolStripSystemTray
            // 
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
            this.statusStripMain.Location = new System.Drawing.Point(0, 554);
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
            // inputPanel
            // 
            this.inputPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.inputPanel.Location = new System.Drawing.Point(0, 528);
            this.inputPanel.Name = "inputPanel";
            this.inputPanel.Size = new System.Drawing.Size(796, 26);
            this.inputPanel.TabIndex = 0;
            // 
            // tabMain
            // 
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Location = new System.Drawing.Point(184, 63);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(434, 465);
            this.tabMain.TabIndex = 2;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(796, 576);
            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.splitterRight);
            this.Controls.Add(this.splitterLeft);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.panelLeft);
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
            this.panelRightBottom.ResumeLayout(false);
            this.tabPanelRight.ResumeLayout(false);
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

        private System.Windows.Forms.ToolStripMenuItem debugWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem codePlexPageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iceChatHomePageToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStripMain;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatus;
        private System.Windows.Forms.ContextMenuStrip contextMenuNotify;
        private System.Windows.Forms.ToolStripMenuItem restoreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem1;




    }
}

