namespace IceChat
{
    partial class FormColors
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

        private System.Windows.Forms.TabControl tabControlColors;
        private System.Windows.Forms.TabPage tabPageMessages;
        private System.Windows.Forms.TabPage tabPageNickNames;
        private System.Windows.Forms.Panel panelColorPicker;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.TabPage tabPageTabBar;
        private System.Windows.Forms.TabPage tabPageBackGround;
        private System.Windows.Forms.Label labelTabCurrent;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labelTabMessage;
        private System.Windows.Forms.Label labelTabJoin;
        private System.Windows.Forms.Label labelTabPart;
        private System.Windows.Forms.Label labelTabServer;
        private System.Windows.Forms.Label labelTabQuit;
        private System.Windows.Forms.Label labelTabOther;
        private System.Windows.Forms.PictureBox pictureTabServer;
        private System.Windows.Forms.PictureBox pictureTabQuit;
        private System.Windows.Forms.PictureBox pictureTabPart;
        private System.Windows.Forms.PictureBox pictureTabJoin;
        private System.Windows.Forms.PictureBox pictureTabMessage;
        private System.Windows.Forms.PictureBox pictureTabCurrent;
        private System.Windows.Forms.PictureBox pictureTabOther;
        private System.Windows.Forms.PictureBox pictureDefault;
        private System.Windows.Forms.PictureBox pictureVoice;
        private System.Windows.Forms.PictureBox pictureHalfOperator;
        private System.Windows.Forms.PictureBox pictureOperator;
        private System.Windows.Forms.PictureBox pictureAdmin;
        private System.Windows.Forms.PictureBox pictureOwner;
        private System.Windows.Forms.Label labelDefault;
        private System.Windows.Forms.Label labelVoice;
        private System.Windows.Forms.Label labelHalfOperator;
        private System.Windows.Forms.Label labelOperator;
        private System.Windows.Forms.Label labelAdmin;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label labelOwner;
        private System.Windows.Forms.PictureBox pictureServerList;
        private System.Windows.Forms.Label labelServerList;
        private System.Windows.Forms.PictureBox pictureNickList;
        private System.Windows.Forms.Label labelNickList;
        private System.Windows.Forms.PictureBox pictureQuery;
        private System.Windows.Forms.Label labelQuery;
        private System.Windows.Forms.PictureBox pictureChannel;
        private System.Windows.Forms.Label labelChannel;
        private System.Windows.Forms.PictureBox pictureConsole;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label labelConsole;
        private System.Windows.Forms.PictureBox pictureTabDefault;
        private System.Windows.Forms.Label labelTabDefault;
        private System.Windows.Forms.PictureBox pictureTabBarOther2;
        private System.Windows.Forms.PictureBox pictureTabBarOther1;
        private System.Windows.Forms.PictureBox pictureTabBarCurrent2;
        private System.Windows.Forms.PictureBox pictureTabBarCurrent1;
        private System.Windows.Forms.Label labelTabBarOther2;
        private System.Windows.Forms.Label labelTabBarOther1;
        private System.Windows.Forms.Label labelTabBarCurrent2;
        private System.Windows.Forms.Label labelTabBarCurrent1;
        private System.Windows.Forms.Label labelCurrent;
        private System.Windows.Forms.TabControl tabMessages;
        private System.Windows.Forms.TabPage tabAdvanced;
        private System.Windows.Forms.TextBox textRawMessage;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBGColor;
        private System.Windows.Forms.TreeView treeMessages;
        private System.Windows.Forms.ListBox listIdentifiers;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelEdit;
        private System.Windows.Forms.TabPage tabBasic;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TreeView treeBasicMessages;
        private System.Windows.Forms.CheckBox checkChangeBGBasic;
        private System.Windows.Forms.Label label6;
        private TextWindow textFormattedBasic;
        private TextWindow textFormattedText;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Channel Messages");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Server Messages");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Private Messages");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Self Messages");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Ctcp");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("DCC");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Other");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Channel Messages");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Server Messages");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Private Messages");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Self Messages");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Ctcp");
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("DCC");
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("Other");
            this.tabControlColors = new System.Windows.Forms.TabControl();
            this.tabPageMessages = new System.Windows.Forms.TabPage();
            this.tabMessages = new System.Windows.Forms.TabControl();
            this.tabBasic = new System.Windows.Forms.TabPage();
            this.checkChangeBGBasic = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.treeBasicMessages = new System.Windows.Forms.TreeView();
            this.tabAdvanced = new System.Windows.Forms.TabPage();
            this.textRawMessage = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBGColor = new System.Windows.Forms.CheckBox();
            this.treeMessages = new System.Windows.Forms.TreeView();
            this.listIdentifiers = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelEdit = new System.Windows.Forms.Label();
            this.tabPageTabBar = new System.Windows.Forms.TabPage();
            this.pictureTabDefault = new System.Windows.Forms.PictureBox();
            this.labelTabDefault = new System.Windows.Forms.Label();
            this.pictureTabOther = new System.Windows.Forms.PictureBox();
            this.pictureTabServer = new System.Windows.Forms.PictureBox();
            this.pictureTabQuit = new System.Windows.Forms.PictureBox();
            this.pictureTabPart = new System.Windows.Forms.PictureBox();
            this.pictureTabJoin = new System.Windows.Forms.PictureBox();
            this.pictureTabMessage = new System.Windows.Forms.PictureBox();
            this.pictureTabCurrent = new System.Windows.Forms.PictureBox();
            this.labelTabOther = new System.Windows.Forms.Label();
            this.labelTabServer = new System.Windows.Forms.Label();
            this.labelTabQuit = new System.Windows.Forms.Label();
            this.labelTabPart = new System.Windows.Forms.Label();
            this.labelTabJoin = new System.Windows.Forms.Label();
            this.labelTabMessage = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.labelTabCurrent = new System.Windows.Forms.Label();
            this.tabPageBackGround = new System.Windows.Forms.TabPage();
            this.picturePanelHeaderForeColor = new System.Windows.Forms.PictureBox();
            this.labelPanelHeaderForeColor = new System.Windows.Forms.Label();
            this.picturePanelHeaderBG2 = new System.Windows.Forms.PictureBox();
            this.picturePanelHeaderBG1 = new System.Windows.Forms.PictureBox();
            this.labelPanelHeaderBG2 = new System.Windows.Forms.Label();
            this.labelPanelHeaderBG1 = new System.Windows.Forms.Label();
            this.pictureTabBarHover2 = new System.Windows.Forms.PictureBox();
            this.pictureTabBarHover1 = new System.Windows.Forms.PictureBox();
            this.labelTabBarHover2 = new System.Windows.Forms.Label();
            this.labelTabBarHover1 = new System.Windows.Forms.Label();
            this.pictureTabBarOther2 = new System.Windows.Forms.PictureBox();
            this.pictureTabBarOther1 = new System.Windows.Forms.PictureBox();
            this.pictureTabBarCurrent2 = new System.Windows.Forms.PictureBox();
            this.pictureTabBarCurrent1 = new System.Windows.Forms.PictureBox();
            this.labelTabBarOther2 = new System.Windows.Forms.Label();
            this.labelTabBarOther1 = new System.Windows.Forms.Label();
            this.labelTabBarCurrent2 = new System.Windows.Forms.Label();
            this.labelTabBarCurrent1 = new System.Windows.Forms.Label();
            this.pictureServerList = new System.Windows.Forms.PictureBox();
            this.labelServerList = new System.Windows.Forms.Label();
            this.pictureNickList = new System.Windows.Forms.PictureBox();
            this.labelNickList = new System.Windows.Forms.Label();
            this.pictureQuery = new System.Windows.Forms.PictureBox();
            this.labelQuery = new System.Windows.Forms.Label();
            this.pictureChannel = new System.Windows.Forms.PictureBox();
            this.labelChannel = new System.Windows.Forms.Label();
            this.pictureConsole = new System.Windows.Forms.PictureBox();
            this.label7 = new System.Windows.Forms.Label();
            this.labelConsole = new System.Windows.Forms.Label();
            this.tabPageNickNames = new System.Windows.Forms.TabPage();
            this.pictureDefault = new System.Windows.Forms.PictureBox();
            this.pictureVoice = new System.Windows.Forms.PictureBox();
            this.pictureHalfOperator = new System.Windows.Forms.PictureBox();
            this.pictureOperator = new System.Windows.Forms.PictureBox();
            this.pictureAdmin = new System.Windows.Forms.PictureBox();
            this.pictureOwner = new System.Windows.Forms.PictureBox();
            this.labelDefault = new System.Windows.Forms.Label();
            this.labelVoice = new System.Windows.Forms.Label();
            this.labelHalfOperator = new System.Windows.Forms.Label();
            this.labelOperator = new System.Windows.Forms.Label();
            this.labelAdmin = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.labelOwner = new System.Windows.Forms.Label();
            this.panelColorPicker = new System.Windows.Forms.Panel();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.labelCurrent = new System.Windows.Forms.Label();
            this.tabControlColors.SuspendLayout();
            this.tabPageMessages.SuspendLayout();
            this.tabMessages.SuspendLayout();
            this.tabBasic.SuspendLayout();
            this.tabAdvanced.SuspendLayout();
            this.tabPageTabBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTabDefault)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTabOther)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTabServer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTabQuit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTabPart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTabJoin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTabMessage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTabCurrent)).BeginInit();
            this.tabPageBackGround.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picturePanelHeaderForeColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picturePanelHeaderBG2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picturePanelHeaderBG1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTabBarHover2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTabBarHover1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTabBarOther2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTabBarOther1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTabBarCurrent2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTabBarCurrent1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureServerList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureNickList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureQuery)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureChannel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureConsole)).BeginInit();
            this.tabPageNickNames.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureDefault)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureVoice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureHalfOperator)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureOperator)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureAdmin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureOwner)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControlColors
            // 
            this.tabControlColors.Controls.Add(this.tabPageMessages);
            this.tabControlColors.Controls.Add(this.tabPageTabBar);
            this.tabControlColors.Controls.Add(this.tabPageBackGround);
            this.tabControlColors.Controls.Add(this.tabPageNickNames);
            this.tabControlColors.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControlColors.Location = new System.Drawing.Point(0, 0);
            this.tabControlColors.Margin = new System.Windows.Forms.Padding(4);
            this.tabControlColors.Name = "tabControlColors";
            this.tabControlColors.SelectedIndex = 0;
            this.tabControlColors.Size = new System.Drawing.Size(738, 414);
            this.tabControlColors.TabIndex = 0;
            this.tabControlColors.Tag = "";
            // 
            // tabPageMessages
            // 
            this.tabPageMessages.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageMessages.Controls.Add(this.tabMessages);
            this.tabPageMessages.Location = new System.Drawing.Point(4, 25);
            this.tabPageMessages.Margin = new System.Windows.Forms.Padding(4);
            this.tabPageMessages.Name = "tabPageMessages";
            this.tabPageMessages.Padding = new System.Windows.Forms.Padding(4);
            this.tabPageMessages.Size = new System.Drawing.Size(730, 385);
            this.tabPageMessages.TabIndex = 0;
            this.tabPageMessages.Text = "Messages";
            // 
            // tabMessages
            // 
            this.tabMessages.Controls.Add(this.tabBasic);
            this.tabMessages.Controls.Add(this.tabAdvanced);
            this.tabMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMessages.Location = new System.Drawing.Point(4, 4);
            this.tabMessages.Name = "tabMessages";
            this.tabMessages.SelectedIndex = 0;
            this.tabMessages.Size = new System.Drawing.Size(722, 377);
            this.tabMessages.TabIndex = 39;
            // 
            // tabBasic
            // 
            this.tabBasic.BackColor = System.Drawing.SystemColors.Control;
            this.tabBasic.Controls.Add(this.checkChangeBGBasic);
            this.tabBasic.Controls.Add(this.label6);
            this.tabBasic.Controls.Add(this.label2);
            this.tabBasic.Controls.Add(this.treeBasicMessages);
            this.tabBasic.Location = new System.Drawing.Point(4, 25);
            this.tabBasic.Name = "tabBasic";
            this.tabBasic.Padding = new System.Windows.Forms.Padding(3);
            this.tabBasic.Size = new System.Drawing.Size(714, 348);
            this.tabBasic.TabIndex = 2;
            this.tabBasic.Text = "Basic";
            // 
            // checkChangeBGBasic
            // 
            this.checkChangeBGBasic.Location = new System.Drawing.Point(332, 291);
            this.checkChangeBGBasic.Name = "checkChangeBGBasic";
            this.checkChangeBGBasic.Size = new System.Drawing.Size(236, 24);
            this.checkChangeBGBasic.TabIndex = 50;
            this.checkChangeBGBasic.Text = "Change Background Color";
            // 
            // label6
            // 
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(20, 293);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(264, 16);
            this.label6.TabIndex = 49;
            this.label6.Text = "Formatted message:";
            // 
            // label2
            // 
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(19, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(312, 16);
            this.label2.TabIndex = 47;
            this.label2.Text = "IRC Messages - Click to Select and Edit";
            // 
            // treeBasicMessages
            // 
            this.treeBasicMessages.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeBasicMessages.Location = new System.Drawing.Point(16, 31);
            this.treeBasicMessages.Name = "treeBasicMessages";
            treeNode1.Name = "";
            treeNode1.Text = "Channel Messages";
            treeNode2.Name = "";
            treeNode2.Text = "Server Messages";
            treeNode3.Name = "";
            treeNode3.Text = "Private Messages";
            treeNode4.Name = "";
            treeNode4.Text = "Self Messages";
            treeNode5.Name = "";
            treeNode5.Text = "Ctcp";
            treeNode6.Name = "";
            treeNode6.Text = "DCC";
            treeNode7.Name = "";
            treeNode7.Text = "Other";
            this.treeBasicMessages.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5,
            treeNode6,
            treeNode7});
            this.treeBasicMessages.Size = new System.Drawing.Size(352, 258);
            this.treeBasicMessages.TabIndex = 46;
            // 
            // tabAdvanced
            // 
            this.tabAdvanced.BackColor = System.Drawing.SystemColors.Control;
            this.tabAdvanced.Controls.Add(this.textRawMessage);
            this.tabAdvanced.Controls.Add(this.label4);
            this.tabAdvanced.Controls.Add(this.checkBGColor);
            this.tabAdvanced.Controls.Add(this.treeMessages);
            this.tabAdvanced.Controls.Add(this.listIdentifiers);
            this.tabAdvanced.Controls.Add(this.label3);
            this.tabAdvanced.Controls.Add(this.label1);
            this.tabAdvanced.Controls.Add(this.labelEdit);
            this.tabAdvanced.Location = new System.Drawing.Point(4, 25);
            this.tabAdvanced.Name = "tabAdvanced";
            this.tabAdvanced.Padding = new System.Windows.Forms.Padding(3);
            this.tabAdvanced.Size = new System.Drawing.Size(714, 348);
            this.tabAdvanced.TabIndex = 1;
            this.tabAdvanced.Text = "Advanced";
            // 
            // textRawMessage
            // 
            this.textRawMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textRawMessage.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textRawMessage.Location = new System.Drawing.Point(13, 267);
            this.textRawMessage.Name = "textRawMessage";
            this.textRawMessage.Size = new System.Drawing.Size(607, 23);
            this.textRawMessage.TabIndex = 39;
            // 
            // label4
            // 
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(16, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(312, 16);
            this.label4.TabIndex = 45;
            this.label4.Text = "IRC Messages - Click to Select and Edit";
            // 
            // checkBGColor
            // 
            this.checkBGColor.Location = new System.Drawing.Point(328, 291);
            this.checkBGColor.Name = "checkBGColor";
            this.checkBGColor.Size = new System.Drawing.Size(236, 24);
            this.checkBGColor.TabIndex = 44;
            this.checkBGColor.Text = "Change Background Color";
            // 
            // treeMessages
            // 
            this.treeMessages.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeMessages.Location = new System.Drawing.Point(13, 31);
            this.treeMessages.Name = "treeMessages";
            treeNode8.Name = "";
            treeNode8.Text = "Channel Messages";
            treeNode9.Name = "";
            treeNode9.Text = "Server Messages";
            treeNode10.Name = "";
            treeNode10.Text = "Private Messages";
            treeNode11.Name = "";
            treeNode11.Text = "Self Messages";
            treeNode12.Name = "";
            treeNode12.Text = "Ctcp";
            treeNode13.Name = "";
            treeNode13.Text = "DCC";
            treeNode14.Name = "";
            treeNode14.Text = "Other";
            this.treeMessages.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode8,
            treeNode9,
            treeNode10,
            treeNode11,
            treeNode12,
            treeNode13,
            treeNode14});
            this.treeMessages.Size = new System.Drawing.Size(352, 207);
            this.treeMessages.TabIndex = 38;
            // 
            // listIdentifiers
            // 
            this.listIdentifiers.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listIdentifiers.Location = new System.Drawing.Point(380, 31);
            this.listIdentifiers.Name = "listIdentifiers";
            this.listIdentifiers.Size = new System.Drawing.Size(296, 82);
            this.listIdentifiers.TabIndex = 43;
            this.listIdentifiers.TabStop = false;
            // 
            // label3
            // 
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(16, 293);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(264, 16);
            this.label3.TabIndex = 42;
            this.label3.Text = "Formatted message (Read only):";
            // 
            // label1
            // 
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(370, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(337, 16);
            this.label1.TabIndex = 41;
            this.label1.Text = "Identifiers (Double click to add to Raw Message)";
            // 
            // labelEdit
            // 
            this.labelEdit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.labelEdit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelEdit.Location = new System.Drawing.Point(16, 251);
            this.labelEdit.Name = "labelEdit";
            this.labelEdit.Size = new System.Drawing.Size(496, 16);
            this.labelEdit.TabIndex = 40;
            this.labelEdit.Text = "Edit message here (Click Color below for Color)";
            // 
            // tabPageTabBar
            // 
            this.tabPageTabBar.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageTabBar.Controls.Add(this.pictureTabDefault);
            this.tabPageTabBar.Controls.Add(this.labelTabDefault);
            this.tabPageTabBar.Controls.Add(this.pictureTabOther);
            this.tabPageTabBar.Controls.Add(this.pictureTabServer);
            this.tabPageTabBar.Controls.Add(this.pictureTabQuit);
            this.tabPageTabBar.Controls.Add(this.pictureTabPart);
            this.tabPageTabBar.Controls.Add(this.pictureTabJoin);
            this.tabPageTabBar.Controls.Add(this.pictureTabMessage);
            this.tabPageTabBar.Controls.Add(this.pictureTabCurrent);
            this.tabPageTabBar.Controls.Add(this.labelTabOther);
            this.tabPageTabBar.Controls.Add(this.labelTabServer);
            this.tabPageTabBar.Controls.Add(this.labelTabQuit);
            this.tabPageTabBar.Controls.Add(this.labelTabPart);
            this.tabPageTabBar.Controls.Add(this.labelTabJoin);
            this.tabPageTabBar.Controls.Add(this.labelTabMessage);
            this.tabPageTabBar.Controls.Add(this.label5);
            this.tabPageTabBar.Controls.Add(this.labelTabCurrent);
            this.tabPageTabBar.Location = new System.Drawing.Point(4, 25);
            this.tabPageTabBar.Name = "tabPageTabBar";
            this.tabPageTabBar.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTabBar.Size = new System.Drawing.Size(730, 385);
            this.tabPageTabBar.TabIndex = 3;
            this.tabPageTabBar.Text = "Tab Bar";
            // 
            // pictureTabDefault
            // 
            this.pictureTabDefault.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureTabDefault.Location = new System.Drawing.Point(235, 222);
            this.pictureTabDefault.Name = "pictureTabDefault";
            this.pictureTabDefault.Size = new System.Drawing.Size(20, 20);
            this.pictureTabDefault.TabIndex = 55;
            this.pictureTabDefault.TabStop = false;
            this.pictureTabDefault.Tag = "Other Message";
            // 
            // labelTabDefault
            // 
            this.labelTabDefault.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelTabDefault.Location = new System.Drawing.Point(19, 226);
            this.labelTabDefault.Name = "labelTabDefault";
            this.labelTabDefault.Size = new System.Drawing.Size(123, 16);
            this.labelTabDefault.TabIndex = 54;
            this.labelTabDefault.Text = "Default";
            // 
            // pictureTabOther
            // 
            this.pictureTabOther.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureTabOther.Location = new System.Drawing.Point(235, 195);
            this.pictureTabOther.Name = "pictureTabOther";
            this.pictureTabOther.Size = new System.Drawing.Size(20, 20);
            this.pictureTabOther.TabIndex = 53;
            this.pictureTabOther.TabStop = false;
            this.pictureTabOther.Tag = "Other Message";
            // 
            // pictureTabServer
            // 
            this.pictureTabServer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureTabServer.Location = new System.Drawing.Point(235, 168);
            this.pictureTabServer.Name = "pictureTabServer";
            this.pictureTabServer.Size = new System.Drawing.Size(20, 20);
            this.pictureTabServer.TabIndex = 52;
            this.pictureTabServer.TabStop = false;
            this.pictureTabServer.Tag = "Server Message";
            // 
            // pictureTabQuit
            // 
            this.pictureTabQuit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureTabQuit.Location = new System.Drawing.Point(235, 141);
            this.pictureTabQuit.Name = "pictureTabQuit";
            this.pictureTabQuit.Size = new System.Drawing.Size(20, 20);
            this.pictureTabQuit.TabIndex = 51;
            this.pictureTabQuit.TabStop = false;
            this.pictureTabQuit.Tag = "Server Quit";
            // 
            // pictureTabPart
            // 
            this.pictureTabPart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureTabPart.Location = new System.Drawing.Point(235, 114);
            this.pictureTabPart.Name = "pictureTabPart";
            this.pictureTabPart.Size = new System.Drawing.Size(20, 20);
            this.pictureTabPart.TabIndex = 50;
            this.pictureTabPart.TabStop = false;
            this.pictureTabPart.Tag = "Channel Part";
            // 
            // pictureTabJoin
            // 
            this.pictureTabJoin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureTabJoin.Location = new System.Drawing.Point(235, 87);
            this.pictureTabJoin.Name = "pictureTabJoin";
            this.pictureTabJoin.Size = new System.Drawing.Size(20, 20);
            this.pictureTabJoin.TabIndex = 49;
            this.pictureTabJoin.TabStop = false;
            this.pictureTabJoin.Tag = "Channel Join";
            // 
            // pictureTabMessage
            // 
            this.pictureTabMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureTabMessage.Location = new System.Drawing.Point(235, 62);
            this.pictureTabMessage.Name = "pictureTabMessage";
            this.pictureTabMessage.Size = new System.Drawing.Size(20, 20);
            this.pictureTabMessage.TabIndex = 47;
            this.pictureTabMessage.TabStop = false;
            this.pictureTabMessage.Tag = "New Message";
            // 
            // pictureTabCurrent
            // 
            this.pictureTabCurrent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureTabCurrent.Location = new System.Drawing.Point(235, 35);
            this.pictureTabCurrent.Name = "pictureTabCurrent";
            this.pictureTabCurrent.Size = new System.Drawing.Size(20, 20);
            this.pictureTabCurrent.TabIndex = 46;
            this.pictureTabCurrent.TabStop = false;
            this.pictureTabCurrent.Tag = "Current Tab";
            // 
            // labelTabOther
            // 
            this.labelTabOther.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelTabOther.Location = new System.Drawing.Point(19, 199);
            this.labelTabOther.Name = "labelTabOther";
            this.labelTabOther.Size = new System.Drawing.Size(123, 16);
            this.labelTabOther.TabIndex = 45;
            this.labelTabOther.Text = "Other";
            // 
            // labelTabServer
            // 
            this.labelTabServer.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelTabServer.Location = new System.Drawing.Point(19, 172);
            this.labelTabServer.Name = "labelTabServer";
            this.labelTabServer.Size = new System.Drawing.Size(123, 16);
            this.labelTabServer.TabIndex = 44;
            this.labelTabServer.Text = "Server Message";
            // 
            // labelTabQuit
            // 
            this.labelTabQuit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelTabQuit.Location = new System.Drawing.Point(19, 145);
            this.labelTabQuit.Name = "labelTabQuit";
            this.labelTabQuit.Size = new System.Drawing.Size(123, 16);
            this.labelTabQuit.TabIndex = 43;
            this.labelTabQuit.Text = "Server Quit";
            // 
            // labelTabPart
            // 
            this.labelTabPart.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelTabPart.Location = new System.Drawing.Point(19, 118);
            this.labelTabPart.Name = "labelTabPart";
            this.labelTabPart.Size = new System.Drawing.Size(123, 16);
            this.labelTabPart.TabIndex = 42;
            this.labelTabPart.Text = "Channel Part";
            // 
            // labelTabJoin
            // 
            this.labelTabJoin.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelTabJoin.Location = new System.Drawing.Point(19, 91);
            this.labelTabJoin.Name = "labelTabJoin";
            this.labelTabJoin.Size = new System.Drawing.Size(123, 16);
            this.labelTabJoin.TabIndex = 41;
            this.labelTabJoin.Text = "Channel Join";
            // 
            // labelTabMessage
            // 
            this.labelTabMessage.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelTabMessage.Location = new System.Drawing.Point(19, 66);
            this.labelTabMessage.Name = "labelTabMessage";
            this.labelTabMessage.Size = new System.Drawing.Size(123, 16);
            this.labelTabMessage.TabIndex = 39;
            this.labelTabMessage.Text = "New Message";
            // 
            // label5
            // 
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(25, 4);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(410, 17);
            this.label5.TabIndex = 38;
            this.label5.Text = "IRC Event - Changes the Tab Bar and Server Tree text color";
            // 
            // labelTabCurrent
            // 
            this.labelTabCurrent.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelTabCurrent.Location = new System.Drawing.Point(19, 39);
            this.labelTabCurrent.Name = "labelTabCurrent";
            this.labelTabCurrent.Size = new System.Drawing.Size(123, 16);
            this.labelTabCurrent.TabIndex = 37;
            this.labelTabCurrent.Text = "Current Tab";
            // 
            // tabPageBackGround
            // 
            this.tabPageBackGround.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageBackGround.Controls.Add(this.picturePanelHeaderForeColor);
            this.tabPageBackGround.Controls.Add(this.labelPanelHeaderForeColor);
            this.tabPageBackGround.Controls.Add(this.picturePanelHeaderBG2);
            this.tabPageBackGround.Controls.Add(this.picturePanelHeaderBG1);
            this.tabPageBackGround.Controls.Add(this.labelPanelHeaderBG2);
            this.tabPageBackGround.Controls.Add(this.labelPanelHeaderBG1);
            this.tabPageBackGround.Controls.Add(this.pictureTabBarHover2);
            this.tabPageBackGround.Controls.Add(this.pictureTabBarHover1);
            this.tabPageBackGround.Controls.Add(this.labelTabBarHover2);
            this.tabPageBackGround.Controls.Add(this.labelTabBarHover1);
            this.tabPageBackGround.Controls.Add(this.pictureTabBarOther2);
            this.tabPageBackGround.Controls.Add(this.pictureTabBarOther1);
            this.tabPageBackGround.Controls.Add(this.pictureTabBarCurrent2);
            this.tabPageBackGround.Controls.Add(this.pictureTabBarCurrent1);
            this.tabPageBackGround.Controls.Add(this.labelTabBarOther2);
            this.tabPageBackGround.Controls.Add(this.labelTabBarOther1);
            this.tabPageBackGround.Controls.Add(this.labelTabBarCurrent2);
            this.tabPageBackGround.Controls.Add(this.labelTabBarCurrent1);
            this.tabPageBackGround.Controls.Add(this.pictureServerList);
            this.tabPageBackGround.Controls.Add(this.labelServerList);
            this.tabPageBackGround.Controls.Add(this.pictureNickList);
            this.tabPageBackGround.Controls.Add(this.labelNickList);
            this.tabPageBackGround.Controls.Add(this.pictureQuery);
            this.tabPageBackGround.Controls.Add(this.labelQuery);
            this.tabPageBackGround.Controls.Add(this.pictureChannel);
            this.tabPageBackGround.Controls.Add(this.labelChannel);
            this.tabPageBackGround.Controls.Add(this.pictureConsole);
            this.tabPageBackGround.Controls.Add(this.label7);
            this.tabPageBackGround.Controls.Add(this.labelConsole);
            this.tabPageBackGround.Location = new System.Drawing.Point(4, 25);
            this.tabPageBackGround.Name = "tabPageBackGround";
            this.tabPageBackGround.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageBackGround.Size = new System.Drawing.Size(730, 385);
            this.tabPageBackGround.TabIndex = 4;
            this.tabPageBackGround.Text = "Background";
            // 
            // picturePanelHeaderForeColor
            // 
            this.picturePanelHeaderForeColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picturePanelHeaderForeColor.Location = new System.Drawing.Point(537, 100);
            this.picturePanelHeaderForeColor.Name = "picturePanelHeaderForeColor";
            this.picturePanelHeaderForeColor.Size = new System.Drawing.Size(20, 20);
            this.picturePanelHeaderForeColor.TabIndex = 92;
            this.picturePanelHeaderForeColor.TabStop = false;
            this.picturePanelHeaderForeColor.Tag = "Channel Owner";
            // 
            // labelPanelHeaderForeColor
            // 
            this.labelPanelHeaderForeColor.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelPanelHeaderForeColor.Location = new System.Drawing.Point(321, 104);
            this.labelPanelHeaderForeColor.Name = "labelPanelHeaderForeColor";
            this.labelPanelHeaderForeColor.Size = new System.Drawing.Size(210, 16);
            this.labelPanelHeaderForeColor.TabIndex = 91;
            this.labelPanelHeaderForeColor.Text = "Panel Header - Fore Color";
            // 
            // picturePanelHeaderBG2
            // 
            this.picturePanelHeaderBG2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picturePanelHeaderBG2.Location = new System.Drawing.Point(537, 75);
            this.picturePanelHeaderBG2.Name = "picturePanelHeaderBG2";
            this.picturePanelHeaderBG2.Size = new System.Drawing.Size(20, 20);
            this.picturePanelHeaderBG2.TabIndex = 90;
            this.picturePanelHeaderBG2.TabStop = false;
            this.picturePanelHeaderBG2.Tag = "Channel Owner";
            // 
            // picturePanelHeaderBG1
            // 
            this.picturePanelHeaderBG1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picturePanelHeaderBG1.Location = new System.Drawing.Point(537, 48);
            this.picturePanelHeaderBG1.Name = "picturePanelHeaderBG1";
            this.picturePanelHeaderBG1.Size = new System.Drawing.Size(20, 20);
            this.picturePanelHeaderBG1.TabIndex = 89;
            this.picturePanelHeaderBG1.TabStop = false;
            this.picturePanelHeaderBG1.Tag = "Channel Owner";
            // 
            // labelPanelHeaderBG2
            // 
            this.labelPanelHeaderBG2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelPanelHeaderBG2.Location = new System.Drawing.Point(321, 79);
            this.labelPanelHeaderBG2.Name = "labelPanelHeaderBG2";
            this.labelPanelHeaderBG2.Size = new System.Drawing.Size(210, 16);
            this.labelPanelHeaderBG2.TabIndex = 88;
            this.labelPanelHeaderBG2.Text = "Panel Header - Color 2";
            // 
            // labelPanelHeaderBG1
            // 
            this.labelPanelHeaderBG1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelPanelHeaderBG1.Location = new System.Drawing.Point(321, 52);
            this.labelPanelHeaderBG1.Name = "labelPanelHeaderBG1";
            this.labelPanelHeaderBG1.Size = new System.Drawing.Size(210, 16);
            this.labelPanelHeaderBG1.TabIndex = 87;
            this.labelPanelHeaderBG1.Text = "Panel Header - Color 1";
            // 
            // pictureTabBarHover2
            // 
            this.pictureTabBarHover2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureTabBarHover2.Location = new System.Drawing.Point(235, 325);
            this.pictureTabBarHover2.Name = "pictureTabBarHover2";
            this.pictureTabBarHover2.Size = new System.Drawing.Size(20, 20);
            this.pictureTabBarHover2.TabIndex = 86;
            this.pictureTabBarHover2.TabStop = false;
            this.pictureTabBarHover2.Tag = "Channel Owner";
            // 
            // pictureTabBarHover1
            // 
            this.pictureTabBarHover1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureTabBarHover1.Location = new System.Drawing.Point(235, 298);
            this.pictureTabBarHover1.Name = "pictureTabBarHover1";
            this.pictureTabBarHover1.Size = new System.Drawing.Size(20, 20);
            this.pictureTabBarHover1.TabIndex = 85;
            this.pictureTabBarHover1.TabStop = false;
            this.pictureTabBarHover1.Tag = "Channel Owner";
            // 
            // labelTabBarHover2
            // 
            this.labelTabBarHover2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelTabBarHover2.Location = new System.Drawing.Point(19, 329);
            this.labelTabBarHover2.Name = "labelTabBarHover2";
            this.labelTabBarHover2.Size = new System.Drawing.Size(184, 16);
            this.labelTabBarHover2.TabIndex = 84;
            this.labelTabBarHover2.Text = "Tab Bar Hover - Color 2";
            // 
            // labelTabBarHover1
            // 
            this.labelTabBarHover1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelTabBarHover1.Location = new System.Drawing.Point(19, 302);
            this.labelTabBarHover1.Name = "labelTabBarHover1";
            this.labelTabBarHover1.Size = new System.Drawing.Size(184, 16);
            this.labelTabBarHover1.TabIndex = 83;
            this.labelTabBarHover1.Text = "Tab Bar Hover - Color 1";
            // 
            // pictureTabBarOther2
            // 
            this.pictureTabBarOther2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureTabBarOther2.Location = new System.Drawing.Point(235, 271);
            this.pictureTabBarOther2.Name = "pictureTabBarOther2";
            this.pictureTabBarOther2.Size = new System.Drawing.Size(20, 20);
            this.pictureTabBarOther2.TabIndex = 82;
            this.pictureTabBarOther2.TabStop = false;
            this.pictureTabBarOther2.Tag = "Channel Owner";
            // 
            // pictureTabBarOther1
            // 
            this.pictureTabBarOther1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureTabBarOther1.Location = new System.Drawing.Point(235, 244);
            this.pictureTabBarOther1.Name = "pictureTabBarOther1";
            this.pictureTabBarOther1.Size = new System.Drawing.Size(20, 20);
            this.pictureTabBarOther1.TabIndex = 81;
            this.pictureTabBarOther1.TabStop = false;
            this.pictureTabBarOther1.Tag = "Channel Owner";
            // 
            // pictureTabBarCurrent2
            // 
            this.pictureTabBarCurrent2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureTabBarCurrent2.Location = new System.Drawing.Point(235, 217);
            this.pictureTabBarCurrent2.Name = "pictureTabBarCurrent2";
            this.pictureTabBarCurrent2.Size = new System.Drawing.Size(20, 20);
            this.pictureTabBarCurrent2.TabIndex = 80;
            this.pictureTabBarCurrent2.TabStop = false;
            this.pictureTabBarCurrent2.Tag = "Channel Owner";
            // 
            // pictureTabBarCurrent1
            // 
            this.pictureTabBarCurrent1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureTabBarCurrent1.Location = new System.Drawing.Point(235, 190);
            this.pictureTabBarCurrent1.Name = "pictureTabBarCurrent1";
            this.pictureTabBarCurrent1.Size = new System.Drawing.Size(20, 20);
            this.pictureTabBarCurrent1.TabIndex = 79;
            this.pictureTabBarCurrent1.TabStop = false;
            this.pictureTabBarCurrent1.Tag = "Channel Owner";
            // 
            // labelTabBarOther2
            // 
            this.labelTabBarOther2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelTabBarOther2.Location = new System.Drawing.Point(19, 275);
            this.labelTabBarOther2.Name = "labelTabBarOther2";
            this.labelTabBarOther2.Size = new System.Drawing.Size(184, 16);
            this.labelTabBarOther2.TabIndex = 78;
            this.labelTabBarOther2.Text = "Tab Bar Default - Color 2";
            // 
            // labelTabBarOther1
            // 
            this.labelTabBarOther1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelTabBarOther1.Location = new System.Drawing.Point(19, 248);
            this.labelTabBarOther1.Name = "labelTabBarOther1";
            this.labelTabBarOther1.Size = new System.Drawing.Size(184, 16);
            this.labelTabBarOther1.TabIndex = 77;
            this.labelTabBarOther1.Text = "Tab Bar Default - Color 1";
            // 
            // labelTabBarCurrent2
            // 
            this.labelTabBarCurrent2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelTabBarCurrent2.Location = new System.Drawing.Point(19, 221);
            this.labelTabBarCurrent2.Name = "labelTabBarCurrent2";
            this.labelTabBarCurrent2.Size = new System.Drawing.Size(184, 16);
            this.labelTabBarCurrent2.TabIndex = 76;
            this.labelTabBarCurrent2.Text = "Tab Bar Current - Color 2";
            // 
            // labelTabBarCurrent1
            // 
            this.labelTabBarCurrent1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelTabBarCurrent1.Location = new System.Drawing.Point(19, 194);
            this.labelTabBarCurrent1.Name = "labelTabBarCurrent1";
            this.labelTabBarCurrent1.Size = new System.Drawing.Size(184, 16);
            this.labelTabBarCurrent1.TabIndex = 75;
            this.labelTabBarCurrent1.Text = "Tab Bar Current - Color 1";
            // 
            // pictureServerList
            // 
            this.pictureServerList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureServerList.Location = new System.Drawing.Point(235, 152);
            this.pictureServerList.Name = "pictureServerList";
            this.pictureServerList.Size = new System.Drawing.Size(20, 20);
            this.pictureServerList.TabIndex = 74;
            this.pictureServerList.TabStop = false;
            this.pictureServerList.Tag = "Channel Owner";
            // 
            // labelServerList
            // 
            this.labelServerList.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelServerList.Location = new System.Drawing.Point(19, 156);
            this.labelServerList.Name = "labelServerList";
            this.labelServerList.Size = new System.Drawing.Size(161, 16);
            this.labelServerList.TabIndex = 73;
            this.labelServerList.Text = "Server List";
            // 
            // pictureNickList
            // 
            this.pictureNickList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureNickList.Location = new System.Drawing.Point(235, 126);
            this.pictureNickList.Name = "pictureNickList";
            this.pictureNickList.Size = new System.Drawing.Size(20, 20);
            this.pictureNickList.TabIndex = 72;
            this.pictureNickList.TabStop = false;
            this.pictureNickList.Tag = "Channel Owner";
            // 
            // labelNickList
            // 
            this.labelNickList.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelNickList.Location = new System.Drawing.Point(19, 130);
            this.labelNickList.Name = "labelNickList";
            this.labelNickList.Size = new System.Drawing.Size(161, 16);
            this.labelNickList.TabIndex = 71;
            this.labelNickList.Text = "Nick List";
            // 
            // pictureQuery
            // 
            this.pictureQuery.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureQuery.Location = new System.Drawing.Point(235, 100);
            this.pictureQuery.Name = "pictureQuery";
            this.pictureQuery.Size = new System.Drawing.Size(20, 20);
            this.pictureQuery.TabIndex = 70;
            this.pictureQuery.TabStop = false;
            this.pictureQuery.Tag = "Channel Owner";
            // 
            // labelQuery
            // 
            this.labelQuery.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelQuery.Location = new System.Drawing.Point(19, 104);
            this.labelQuery.Name = "labelQuery";
            this.labelQuery.Size = new System.Drawing.Size(161, 16);
            this.labelQuery.TabIndex = 69;
            this.labelQuery.Text = "Query";
            // 
            // pictureChannel
            // 
            this.pictureChannel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureChannel.Location = new System.Drawing.Point(235, 74);
            this.pictureChannel.Name = "pictureChannel";
            this.pictureChannel.Size = new System.Drawing.Size(20, 20);
            this.pictureChannel.TabIndex = 68;
            this.pictureChannel.TabStop = false;
            this.pictureChannel.Tag = "Channel Owner";
            // 
            // labelChannel
            // 
            this.labelChannel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelChannel.Location = new System.Drawing.Point(19, 78);
            this.labelChannel.Name = "labelChannel";
            this.labelChannel.Size = new System.Drawing.Size(161, 16);
            this.labelChannel.TabIndex = 67;
            this.labelChannel.Text = "Channel";
            // 
            // pictureConsole
            // 
            this.pictureConsole.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureConsole.Location = new System.Drawing.Point(235, 48);
            this.pictureConsole.Name = "pictureConsole";
            this.pictureConsole.Size = new System.Drawing.Size(20, 20);
            this.pictureConsole.TabIndex = 66;
            this.pictureConsole.TabStop = false;
            this.pictureConsole.Tag = "Channel Owner";
            // 
            // label7
            // 
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(25, 4);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(343, 16);
            this.label7.TabIndex = 65;
            this.label7.Text = "Background colors for each type of Window";
            // 
            // labelConsole
            // 
            this.labelConsole.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelConsole.Location = new System.Drawing.Point(19, 52);
            this.labelConsole.Name = "labelConsole";
            this.labelConsole.Size = new System.Drawing.Size(161, 16);
            this.labelConsole.TabIndex = 64;
            this.labelConsole.Text = "Console";
            // 
            // tabPageNickNames
            // 
            this.tabPageNickNames.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageNickNames.Controls.Add(this.pictureDefault);
            this.tabPageNickNames.Controls.Add(this.pictureVoice);
            this.tabPageNickNames.Controls.Add(this.pictureHalfOperator);
            this.tabPageNickNames.Controls.Add(this.pictureOperator);
            this.tabPageNickNames.Controls.Add(this.pictureAdmin);
            this.tabPageNickNames.Controls.Add(this.pictureOwner);
            this.tabPageNickNames.Controls.Add(this.labelDefault);
            this.tabPageNickNames.Controls.Add(this.labelVoice);
            this.tabPageNickNames.Controls.Add(this.labelHalfOperator);
            this.tabPageNickNames.Controls.Add(this.labelOperator);
            this.tabPageNickNames.Controls.Add(this.labelAdmin);
            this.tabPageNickNames.Controls.Add(this.label20);
            this.tabPageNickNames.Controls.Add(this.labelOwner);
            this.tabPageNickNames.Location = new System.Drawing.Point(4, 25);
            this.tabPageNickNames.Margin = new System.Windows.Forms.Padding(4);
            this.tabPageNickNames.Name = "tabPageNickNames";
            this.tabPageNickNames.Padding = new System.Windows.Forms.Padding(4);
            this.tabPageNickNames.Size = new System.Drawing.Size(730, 385);
            this.tabPageNickNames.TabIndex = 1;
            this.tabPageNickNames.Text = "Nick Names";
            // 
            // pictureDefault
            // 
            this.pictureDefault.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureDefault.Location = new System.Drawing.Point(235, 171);
            this.pictureDefault.Name = "pictureDefault";
            this.pictureDefault.Size = new System.Drawing.Size(20, 20);
            this.pictureDefault.TabIndex = 70;
            this.pictureDefault.TabStop = false;
            this.pictureDefault.Tag = "Default User";
            // 
            // pictureVoice
            // 
            this.pictureVoice.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureVoice.Location = new System.Drawing.Point(235, 145);
            this.pictureVoice.Name = "pictureVoice";
            this.pictureVoice.Size = new System.Drawing.Size(20, 20);
            this.pictureVoice.TabIndex = 67;
            this.pictureVoice.TabStop = false;
            this.pictureVoice.Tag = "Channel Voice";
            // 
            // pictureHalfOperator
            // 
            this.pictureHalfOperator.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureHalfOperator.Location = new System.Drawing.Point(235, 118);
            this.pictureHalfOperator.Name = "pictureHalfOperator";
            this.pictureHalfOperator.Size = new System.Drawing.Size(20, 20);
            this.pictureHalfOperator.TabIndex = 66;
            this.pictureHalfOperator.TabStop = false;
            this.pictureHalfOperator.Tag = "Channel Half Operator";
            // 
            // pictureOperator
            // 
            this.pictureOperator.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureOperator.Location = new System.Drawing.Point(235, 91);
            this.pictureOperator.Name = "pictureOperator";
            this.pictureOperator.Size = new System.Drawing.Size(20, 20);
            this.pictureOperator.TabIndex = 65;
            this.pictureOperator.TabStop = false;
            this.pictureOperator.Tag = "Channel Operator";
            // 
            // pictureAdmin
            // 
            this.pictureAdmin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureAdmin.Location = new System.Drawing.Point(235, 64);
            this.pictureAdmin.Name = "pictureAdmin";
            this.pictureAdmin.Size = new System.Drawing.Size(20, 20);
            this.pictureAdmin.TabIndex = 64;
            this.pictureAdmin.TabStop = false;
            this.pictureAdmin.Tag = "Channel Admin";
            // 
            // pictureOwner
            // 
            this.pictureOwner.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureOwner.Location = new System.Drawing.Point(235, 37);
            this.pictureOwner.Name = "pictureOwner";
            this.pictureOwner.Size = new System.Drawing.Size(20, 20);
            this.pictureOwner.TabIndex = 63;
            this.pictureOwner.TabStop = false;
            this.pictureOwner.Tag = "Channel Owner";
            // 
            // labelDefault
            // 
            this.labelDefault.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelDefault.Location = new System.Drawing.Point(19, 175);
            this.labelDefault.Name = "labelDefault";
            this.labelDefault.Size = new System.Drawing.Size(123, 16);
            this.labelDefault.TabIndex = 62;
            this.labelDefault.Text = "Default";
            // 
            // labelVoice
            // 
            this.labelVoice.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelVoice.Location = new System.Drawing.Point(19, 149);
            this.labelVoice.Name = "labelVoice";
            this.labelVoice.Size = new System.Drawing.Size(161, 16);
            this.labelVoice.TabIndex = 59;
            this.labelVoice.Text = "+ Channel Voice";
            // 
            // labelHalfOperator
            // 
            this.labelHalfOperator.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelHalfOperator.Location = new System.Drawing.Point(19, 122);
            this.labelHalfOperator.Name = "labelHalfOperator";
            this.labelHalfOperator.Size = new System.Drawing.Size(175, 16);
            this.labelHalfOperator.TabIndex = 58;
            this.labelHalfOperator.Text = "% Channel Half Operator";
            // 
            // labelOperator
            // 
            this.labelOperator.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelOperator.Location = new System.Drawing.Point(19, 95);
            this.labelOperator.Name = "labelOperator";
            this.labelOperator.Size = new System.Drawing.Size(161, 16);
            this.labelOperator.TabIndex = 57;
            this.labelOperator.Text = "@ Channel Operator";
            // 
            // labelAdmin
            // 
            this.labelAdmin.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelAdmin.Location = new System.Drawing.Point(19, 68);
            this.labelAdmin.Name = "labelAdmin";
            this.labelAdmin.Size = new System.Drawing.Size(161, 16);
            this.labelAdmin.TabIndex = 56;
            this.labelAdmin.Text = "&& Channel Admin";
            // 
            // label20
            // 
            this.label20.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label20.Location = new System.Drawing.Point(25, 4);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(343, 16);
            this.label20.TabIndex = 55;
            this.label20.Text = "Channel User Types - Click Color Box and select new Color";
            // 
            // labelOwner
            // 
            this.labelOwner.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelOwner.Location = new System.Drawing.Point(19, 41);
            this.labelOwner.Name = "labelOwner";
            this.labelOwner.Size = new System.Drawing.Size(161, 16);
            this.labelOwner.TabIndex = 54;
            this.labelOwner.Text = "~ Channel Owner";
            // 
            // panelColorPicker
            // 
            this.panelColorPicker.BackColor = System.Drawing.SystemColors.Control;
            this.panelColorPicker.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.panelColorPicker.Location = new System.Drawing.Point(2, 428);
            this.panelColorPicker.Name = "panelColorPicker";
            this.panelColorPicker.Size = new System.Drawing.Size(288, 40);
            this.panelColorPicker.TabIndex = 20;
            // 
            // buttonCancel
            // 
            this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCancel.Location = new System.Drawing.Point(618, 436);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(96, 32);
            this.buttonCancel.TabIndex = 22;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSave.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSave.Location = new System.Drawing.Point(497, 436);
            this.buttonSave.Margin = new System.Windows.Forms.Padding(4);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(103, 32);
            this.buttonSave.TabIndex = 21;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // labelCurrent
            // 
            this.labelCurrent.Location = new System.Drawing.Point(303, 427);
            this.labelCurrent.Name = "labelCurrent";
            this.labelCurrent.Size = new System.Drawing.Size(187, 40);
            this.labelCurrent.TabIndex = 23;
            this.labelCurrent.Text = "Current:";
            // 
            // FormColors
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(742, 477);
            this.Controls.Add(this.labelCurrent);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.panelColorPicker);
            this.Controls.Add(this.tabControlColors);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormColors";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Color Settings";
            this.tabControlColors.ResumeLayout(false);
            this.tabPageMessages.ResumeLayout(false);
            this.tabMessages.ResumeLayout(false);
            this.tabBasic.ResumeLayout(false);
            this.tabAdvanced.ResumeLayout(false);
            this.tabAdvanced.PerformLayout();
            this.tabPageTabBar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureTabDefault)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTabOther)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTabServer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTabQuit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTabPart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTabJoin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTabMessage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTabCurrent)).EndInit();
            this.tabPageBackGround.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picturePanelHeaderForeColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picturePanelHeaderBG2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picturePanelHeaderBG1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTabBarHover2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTabBarHover1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTabBarOther2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTabBarOther1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTabBarCurrent2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTabBarCurrent1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureServerList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureNickList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureQuery)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureChannel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureConsole)).EndInit();
            this.tabPageNickNames.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureDefault)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureVoice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureHalfOperator)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureOperator)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureAdmin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureOwner)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelTabBarHover2;
        private System.Windows.Forms.Label labelTabBarHover1;
        private System.Windows.Forms.PictureBox pictureTabBarHover2;
        private System.Windows.Forms.PictureBox pictureTabBarHover1;
        private System.Windows.Forms.PictureBox picturePanelHeaderBG2;
        private System.Windows.Forms.PictureBox picturePanelHeaderBG1;
        private System.Windows.Forms.Label labelPanelHeaderBG2;
        private System.Windows.Forms.Label labelPanelHeaderBG1;
        private System.Windows.Forms.PictureBox picturePanelHeaderForeColor;
        private System.Windows.Forms.Label labelPanelHeaderForeColor;

    }
}