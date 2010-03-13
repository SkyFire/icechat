namespace IceChat
{
    partial class FormServers
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
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormServers));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.tabControlSettings = new System.Windows.Forms.TabControl();
            this.tabPageMain = new System.Windows.Forms.TabPage();
            this.textAwayNick = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textAltNickName = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textDisplayName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textQuitMessage = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textFullName = new System.Windows.Forms.TextBox();
            this.textIdentName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textServerPort = new System.Windows.Forms.TextBox();
            this.textServername = new System.Windows.Forms.TextBox();
            this.textNickName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPageExtra = new System.Windows.Forms.TabPage();
            this.label12 = new System.Windows.Forms.Label();
            this.textServerPassword = new System.Windows.Forms.TextBox();
            this.checkDisableCTCP = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.comboEncoding = new System.Windows.Forms.ComboBox();
            this.checkRejoinChannel = new System.Windows.Forms.CheckBox();
            this.checkPingPong = new System.Windows.Forms.CheckBox();
            this.checkMOTD = new System.Windows.Forms.CheckBox();
            this.checkModeI = new System.Windows.Forms.CheckBox();
            this.tabPageAutoJoin = new System.Windows.Forms.TabPage();
            this.checkAutoJoinDelay = new System.Windows.Forms.CheckBox();
            this.buttonEdit = new System.Windows.Forms.Button();
            this.listChannel = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.checkAutoJoin = new System.Windows.Forms.CheckBox();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.textChannel = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tabPageAutoPerform = new System.Windows.Forms.TabPage();
            this.textAutoPerform = new System.Windows.Forms.TextBox();
            this.checkAutoPerform = new System.Windows.Forms.CheckBox();
            this.tabPageIgnore = new System.Windows.Forms.TabPage();
            this.listIgnore = new System.Windows.Forms.ListView();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.checkIgnore = new System.Windows.Forms.CheckBox();
            this.textIgnore = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.buttonEditIgnore = new System.Windows.Forms.Button();
            this.buttonRemoveIgnore = new System.Windows.Forms.Button();
            this.buttonAddIgnore = new System.Windows.Forms.Button();
            this.imageListEditor = new System.Windows.Forms.ImageList(this.components);
            this.buttonRemoveServer = new System.Windows.Forms.Button();
            this.labelNote = new System.Windows.Forms.Label();
            this.tabControlSettings.SuspendLayout();
            this.tabPageMain.SuspendLayout();
            this.tabPageExtra.SuspendLayout();
            this.tabPageAutoJoin.SuspendLayout();
            this.tabPageAutoPerform.SuspendLayout();
            this.tabPageIgnore.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCancel.Image = global::IceChat.Properties.Resources.disconected;
            this.buttonCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonCancel.Location = new System.Drawing.Point(525, 245);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(96, 32);
            this.buttonCancel.TabIndex = 19;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSave.Image = global::IceChat.Properties.Resources.save;
            this.buttonSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonSave.Location = new System.Drawing.Point(416, 246);
            this.buttonSave.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(103, 31);
            this.buttonSave.TabIndex = 17;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // tabControlSettings
            // 
            this.tabControlSettings.AccessibleDescription = "";
            this.tabControlSettings.Controls.Add(this.tabPageMain);
            this.tabControlSettings.Controls.Add(this.tabPageExtra);
            this.tabControlSettings.Controls.Add(this.tabPageAutoJoin);
            this.tabControlSettings.Controls.Add(this.tabPageAutoPerform);
            this.tabControlSettings.Controls.Add(this.tabPageIgnore);
            this.tabControlSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControlSettings.ImageList = this.imageListEditor;
            this.tabControlSettings.Location = new System.Drawing.Point(0, 0);
            this.tabControlSettings.Margin = new System.Windows.Forms.Padding(0);
            this.tabControlSettings.Name = "tabControlSettings";
            this.tabControlSettings.SelectedIndex = 0;
            this.tabControlSettings.Size = new System.Drawing.Size(625, 237);
            this.tabControlSettings.TabIndex = 36;
            // 
            // tabPageMain
            // 
            this.tabPageMain.BackColor = System.Drawing.Color.Transparent;
            this.tabPageMain.Controls.Add(this.textAwayNick);
            this.tabPageMain.Controls.Add(this.label11);
            this.tabPageMain.Controls.Add(this.textAltNickName);
            this.tabPageMain.Controls.Add(this.label9);
            this.tabPageMain.Controls.Add(this.textDisplayName);
            this.tabPageMain.Controls.Add(this.label5);
            this.tabPageMain.Controls.Add(this.textQuitMessage);
            this.tabPageMain.Controls.Add(this.label8);
            this.tabPageMain.Controls.Add(this.label7);
            this.tabPageMain.Controls.Add(this.textFullName);
            this.tabPageMain.Controls.Add(this.textIdentName);
            this.tabPageMain.Controls.Add(this.label6);
            this.tabPageMain.Controls.Add(this.textServerPort);
            this.tabPageMain.Controls.Add(this.textServername);
            this.tabPageMain.Controls.Add(this.textNickName);
            this.tabPageMain.Controls.Add(this.label3);
            this.tabPageMain.Controls.Add(this.label2);
            this.tabPageMain.Controls.Add(this.label1);
            this.tabPageMain.ImageIndex = 0;
            this.tabPageMain.Location = new System.Drawing.Point(4, 25);
            this.tabPageMain.Margin = new System.Windows.Forms.Padding(0);
            this.tabPageMain.Name = "tabPageMain";
            this.tabPageMain.Size = new System.Drawing.Size(617, 208);
            this.tabPageMain.TabIndex = 0;
            this.tabPageMain.Text = "Main Settings";
            this.tabPageMain.UseVisualStyleBackColor = true;
            // 
            // textAwayNick
            // 
            this.textAwayNick.Location = new System.Drawing.Point(459, 100);
            this.textAwayNick.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textAwayNick.Name = "textAwayNick";
            this.textAwayNick.Size = new System.Drawing.Size(150, 23);
            this.textAwayNick.TabIndex = 50;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(333, 103);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(117, 16);
            this.label11.TabIndex = 49;
            this.label11.Text = "Away Nick Name";
            // 
            // textAltNickName
            // 
            this.textAltNickName.Location = new System.Drawing.Point(459, 69);
            this.textAltNickName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textAltNickName.Name = "textAltNickName";
            this.textAltNickName.Size = new System.Drawing.Size(150, 23);
            this.textAltNickName.TabIndex = 48;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(351, 72);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(99, 16);
            this.label9.TabIndex = 47;
            this.label9.Text = "Alt Nick Name";
            // 
            // textDisplayName
            // 
            this.textDisplayName.Location = new System.Drawing.Point(115, 33);
            this.textDisplayName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textDisplayName.Name = "textDisplayName";
            this.textDisplayName.Size = new System.Drawing.Size(150, 23);
            this.textDisplayName.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 36);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 16);
            this.label5.TabIndex = 46;
            this.label5.Text = "Display Name";
            // 
            // textQuitMessage
            // 
            this.textQuitMessage.Location = new System.Drawing.Point(115, 159);
            this.textQuitMessage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textQuitMessage.Name = "textQuitMessage";
            this.textQuitMessage.Size = new System.Drawing.Size(228, 23);
            this.textQuitMessage.TabIndex = 6;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 162);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(97, 16);
            this.label8.TabIndex = 45;
            this.label8.Text = "Quit Message";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 131);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 16);
            this.label7.TabIndex = 43;
            this.label7.Text = "Full Name";
            // 
            // textFullName
            // 
            this.textFullName.Location = new System.Drawing.Point(115, 128);
            this.textFullName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textFullName.Name = "textFullName";
            this.textFullName.Size = new System.Drawing.Size(228, 23);
            this.textFullName.TabIndex = 5;
            // 
            // textIdentName
            // 
            this.textIdentName.Location = new System.Drawing.Point(115, 99);
            this.textIdentName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textIdentName.Name = "textIdentName";
            this.textIdentName.Size = new System.Drawing.Size(150, 23);
            this.textIdentName.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 102);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(84, 16);
            this.label6.TabIndex = 41;
            this.label6.Text = "Ident Name";
            // 
            // textServerPort
            // 
            this.textServerPort.Location = new System.Drawing.Point(459, 4);
            this.textServerPort.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textServerPort.Name = "textServerPort";
            this.textServerPort.Size = new System.Drawing.Size(53, 23);
            this.textServerPort.TabIndex = 1;
            // 
            // textServername
            // 
            this.textServername.Location = new System.Drawing.Point(115, 4);
            this.textServername.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textServername.Name = "textServername";
            this.textServername.Size = new System.Drawing.Size(228, 23);
            this.textServername.TabIndex = 0;
            // 
            // textNickName
            // 
            this.textNickName.Location = new System.Drawing.Point(115, 65);
            this.textNickName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textNickName.Name = "textNickName";
            this.textNickName.Size = new System.Drawing.Size(150, 23);
            this.textNickName.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(367, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 16);
            this.label3.TabIndex = 39;
            this.label3.Text = "Server Port";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(15, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 16);
            this.label2.TabIndex = 38;
            this.label2.Text = "Server Name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(15, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 16);
            this.label1.TabIndex = 36;
            this.label1.Text = "Nick Name";
            // 
            // tabPageExtra
            // 
            this.tabPageExtra.BackColor = System.Drawing.Color.Transparent;
            this.tabPageExtra.Controls.Add(this.label12);
            this.tabPageExtra.Controls.Add(this.textServerPassword);
            this.tabPageExtra.Controls.Add(this.checkDisableCTCP);
            this.tabPageExtra.Controls.Add(this.label10);
            this.tabPageExtra.Controls.Add(this.comboEncoding);
            this.tabPageExtra.Controls.Add(this.checkRejoinChannel);
            this.tabPageExtra.Controls.Add(this.checkPingPong);
            this.tabPageExtra.Controls.Add(this.checkMOTD);
            this.tabPageExtra.Controls.Add(this.checkModeI);
            this.tabPageExtra.ImageIndex = 1;
            this.tabPageExtra.Location = new System.Drawing.Point(4, 25);
            this.tabPageExtra.Name = "tabPageExtra";
            this.tabPageExtra.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageExtra.Size = new System.Drawing.Size(617, 208);
            this.tabPageExtra.TabIndex = 3;
            this.tabPageExtra.Text = "Extra Settings";
            this.tabPageExtra.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(272, 12);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(118, 16);
            this.label12.TabIndex = 45;
            this.label12.Text = "Server Password";
            // 
            // textServerPassword
            // 
            this.textServerPassword.Location = new System.Drawing.Point(412, 9);
            this.textServerPassword.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textServerPassword.Name = "textServerPassword";
            this.textServerPassword.Size = new System.Drawing.Size(143, 23);
            this.textServerPassword.TabIndex = 44;
            // 
            // checkDisableCTCP
            // 
            this.checkDisableCTCP.AutoSize = true;
            this.checkDisableCTCP.Location = new System.Drawing.Point(15, 115);
            this.checkDisableCTCP.Name = "checkDisableCTCP";
            this.checkDisableCTCP.Size = new System.Drawing.Size(163, 20);
            this.checkDisableCTCP.TabIndex = 6;
            this.checkDisableCTCP.Text = "Disable CTCP Replies";
            this.checkDisableCTCP.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 170);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(67, 16);
            this.label10.TabIndex = 5;
            this.label10.Text = "Encoding";
            // 
            // comboEncoding
            // 
            this.comboEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboEncoding.FormattingEnabled = true;
            this.comboEncoding.Location = new System.Drawing.Point(88, 167);
            this.comboEncoding.Name = "comboEncoding";
            this.comboEncoding.Size = new System.Drawing.Size(147, 24);
            this.comboEncoding.TabIndex = 4;
            // 
            // checkRejoinChannel
            // 
            this.checkRejoinChannel.AutoSize = true;
            this.checkRejoinChannel.Location = new System.Drawing.Point(15, 89);
            this.checkRejoinChannel.Name = "checkRejoinChannel";
            this.checkRejoinChannel.Size = new System.Drawing.Size(220, 20);
            this.checkRejoinChannel.TabIndex = 3;
            this.checkRejoinChannel.Text = "Re-Join Channels on Connect";
            this.checkRejoinChannel.UseVisualStyleBackColor = true;
            // 
            // checkPingPong
            // 
            this.checkPingPong.AutoSize = true;
            this.checkPingPong.Location = new System.Drawing.Point(15, 63);
            this.checkPingPong.Name = "checkPingPong";
            this.checkPingPong.Size = new System.Drawing.Size(209, 20);
            this.checkPingPong.TabIndex = 2;
            this.checkPingPong.Text = "Show PING PONG Messages";
            this.checkPingPong.UseVisualStyleBackColor = true;
            // 
            // checkMOTD
            // 
            this.checkMOTD.AutoSize = true;
            this.checkMOTD.Location = new System.Drawing.Point(15, 37);
            this.checkMOTD.Name = "checkMOTD";
            this.checkMOTD.Size = new System.Drawing.Size(188, 20);
            this.checkMOTD.TabIndex = 1;
            this.checkMOTD.Text = "Show MOTD on Connect";
            this.checkMOTD.UseVisualStyleBackColor = true;
            // 
            // checkModeI
            // 
            this.checkModeI.AutoSize = true;
            this.checkModeI.Location = new System.Drawing.Point(15, 11);
            this.checkModeI.Name = "checkModeI";
            this.checkModeI.Size = new System.Drawing.Size(188, 20);
            this.checkModeI.TabIndex = 0;
            this.checkModeI.Text = "Set Mode +i on Connect";
            this.checkModeI.UseVisualStyleBackColor = true;
            // 
            // tabPageAutoJoin
            // 
            this.tabPageAutoJoin.BackColor = System.Drawing.Color.Transparent;
            this.tabPageAutoJoin.Controls.Add(this.checkAutoJoinDelay);
            this.tabPageAutoJoin.Controls.Add(this.buttonEdit);
            this.tabPageAutoJoin.Controls.Add(this.listChannel);
            this.tabPageAutoJoin.Controls.Add(this.checkAutoJoin);
            this.tabPageAutoJoin.Controls.Add(this.buttonRemove);
            this.tabPageAutoJoin.Controls.Add(this.buttonAdd);
            this.tabPageAutoJoin.Controls.Add(this.textChannel);
            this.tabPageAutoJoin.Controls.Add(this.label4);
            this.tabPageAutoJoin.ImageIndex = 2;
            this.tabPageAutoJoin.Location = new System.Drawing.Point(4, 25);
            this.tabPageAutoJoin.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPageAutoJoin.Name = "tabPageAutoJoin";
            this.tabPageAutoJoin.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPageAutoJoin.Size = new System.Drawing.Size(617, 208);
            this.tabPageAutoJoin.TabIndex = 1;
            this.tabPageAutoJoin.Text = "AutoJoin";
            this.tabPageAutoJoin.UseVisualStyleBackColor = true;
            // 
            // checkAutoJoinDelay
            // 
            this.checkAutoJoinDelay.Location = new System.Drawing.Point(183, 184);
            this.checkAutoJoinDelay.Name = "checkAutoJoinDelay";
            this.checkAutoJoinDelay.Size = new System.Drawing.Size(263, 20);
            this.checkAutoJoinDelay.TabIndex = 34;
            this.checkAutoJoinDelay.Text = "Enable AutoJoin Delay (5 seconds)";
            this.checkAutoJoinDelay.UseVisualStyleBackColor = true;
            // 
            // buttonEdit
            // 
            this.buttonEdit.Location = new System.Drawing.Point(362, 74);
            this.buttonEdit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(87, 27);
            this.buttonEdit.TabIndex = 33;
            this.buttonEdit.Text = "Edit";
            this.buttonEdit.UseVisualStyleBackColor = true;
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // listChannel
            // 
            this.listChannel.CheckBoxes = true;
            this.listChannel.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listChannel.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listChannel.Location = new System.Drawing.Point(6, 41);
            this.listChannel.Name = "listChannel";
            this.listChannel.Size = new System.Drawing.Size(350, 137);
            this.listChannel.TabIndex = 32;
            this.listChannel.UseCompatibleStateImageBehavior = false;
            this.listChannel.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Channel";
            this.columnHeader1.Width = 246;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "ChannelKey";
            this.columnHeader2.Width = 100;
            // 
            // checkAutoJoin
            // 
            this.checkAutoJoin.Location = new System.Drawing.Point(6, 184);
            this.checkAutoJoin.Name = "checkAutoJoin";
            this.checkAutoJoin.Size = new System.Drawing.Size(151, 20);
            this.checkAutoJoin.TabIndex = 31;
            this.checkAutoJoin.Text = "Enable AutoJoin";
            this.checkAutoJoin.UseVisualStyleBackColor = true;
            // 
            // buttonRemove
            // 
            this.buttonRemove.Location = new System.Drawing.Point(362, 39);
            this.buttonRemove.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(87, 27);
            this.buttonRemove.TabIndex = 30;
            this.buttonRemove.Text = "Remove";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(362, 6);
            this.buttonAdd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(87, 27);
            this.buttonAdd.TabIndex = 29;
            this.buttonAdd.Text = "Add";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // textChannel
            // 
            this.textChannel.Location = new System.Drawing.Point(78, 7);
            this.textChannel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textChannel.Name = "textChannel";
            this.textChannel.Size = new System.Drawing.Size(278, 23);
            this.textChannel.TabIndex = 26;
            this.textChannel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textChannel_KeyDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 16);
            this.label4.TabIndex = 27;
            this.label4.Text = "Channel";
            // 
            // tabPageAutoPerform
            // 
            this.tabPageAutoPerform.Controls.Add(this.textAutoPerform);
            this.tabPageAutoPerform.Controls.Add(this.checkAutoPerform);
            this.tabPageAutoPerform.ImageIndex = 3;
            this.tabPageAutoPerform.Location = new System.Drawing.Point(4, 25);
            this.tabPageAutoPerform.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPageAutoPerform.Name = "tabPageAutoPerform";
            this.tabPageAutoPerform.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPageAutoPerform.Size = new System.Drawing.Size(617, 208);
            this.tabPageAutoPerform.TabIndex = 2;
            this.tabPageAutoPerform.Text = "AutoPerform";
            this.tabPageAutoPerform.UseVisualStyleBackColor = true;
            // 
            // textAutoPerform
            // 
            this.textAutoPerform.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textAutoPerform.Location = new System.Drawing.Point(3, 4);
            this.textAutoPerform.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textAutoPerform.Multiline = true;
            this.textAutoPerform.Name = "textAutoPerform";
            this.textAutoPerform.Size = new System.Drawing.Size(611, 180);
            this.textAutoPerform.TabIndex = 28;
            // 
            // checkAutoPerform
            // 
            this.checkAutoPerform.AutoSize = true;
            this.checkAutoPerform.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.checkAutoPerform.Location = new System.Drawing.Point(3, 184);
            this.checkAutoPerform.Name = "checkAutoPerform";
            this.checkAutoPerform.Size = new System.Drawing.Size(611, 20);
            this.checkAutoPerform.TabIndex = 29;
            this.checkAutoPerform.Text = "Enable AutoPerform";
            this.checkAutoPerform.UseVisualStyleBackColor = true;
            // 
            // tabPageIgnore
            // 
            this.tabPageIgnore.Controls.Add(this.labelNote);
            this.tabPageIgnore.Controls.Add(this.listIgnore);
            this.tabPageIgnore.Controls.Add(this.checkIgnore);
            this.tabPageIgnore.Controls.Add(this.textIgnore);
            this.tabPageIgnore.Controls.Add(this.label13);
            this.tabPageIgnore.Controls.Add(this.buttonEditIgnore);
            this.tabPageIgnore.Controls.Add(this.buttonRemoveIgnore);
            this.tabPageIgnore.Controls.Add(this.buttonAddIgnore);
            this.tabPageIgnore.Location = new System.Drawing.Point(4, 25);
            this.tabPageIgnore.Name = "tabPageIgnore";
            this.tabPageIgnore.Size = new System.Drawing.Size(617, 208);
            this.tabPageIgnore.TabIndex = 4;
            this.tabPageIgnore.Text = "Ignore List";
            this.tabPageIgnore.UseVisualStyleBackColor = true;
            // 
            // listIgnore
            // 
            this.listIgnore.CheckBoxes = true;
            this.listIgnore.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3});
            this.listIgnore.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listIgnore.Location = new System.Drawing.Point(6, 39);
            this.listIgnore.Name = "listIgnore";
            this.listIgnore.Size = new System.Drawing.Size(350, 137);
            this.listIgnore.TabIndex = 40;
            this.listIgnore.UseCompatibleStateImageBehavior = false;
            this.listIgnore.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Nick / Host";
            this.columnHeader3.Width = 339;
            // 
            // checkIgnore
            // 
            this.checkIgnore.Location = new System.Drawing.Point(6, 182);
            this.checkIgnore.Name = "checkIgnore";
            this.checkIgnore.Size = new System.Drawing.Size(151, 20);
            this.checkIgnore.TabIndex = 39;
            this.checkIgnore.Text = "Enable Ignore List";
            this.checkIgnore.UseVisualStyleBackColor = true;
            // 
            // textIgnore
            // 
            this.textIgnore.Location = new System.Drawing.Point(78, 5);
            this.textIgnore.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textIgnore.Name = "textIgnore";
            this.textIgnore.Size = new System.Drawing.Size(278, 23);
            this.textIgnore.TabIndex = 37;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(8, 9);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(71, 16);
            this.label13.TabIndex = 38;
            this.label13.Text = "Nick/Host";
            // 
            // buttonEditIgnore
            // 
            this.buttonEditIgnore.Location = new System.Drawing.Point(362, 73);
            this.buttonEditIgnore.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonEditIgnore.Name = "buttonEditIgnore";
            this.buttonEditIgnore.Size = new System.Drawing.Size(87, 27);
            this.buttonEditIgnore.TabIndex = 36;
            this.buttonEditIgnore.Text = "Edit";
            this.buttonEditIgnore.UseVisualStyleBackColor = true;
            this.buttonEditIgnore.Click += new System.EventHandler(this.buttonEditIgnore_Click);
            // 
            // buttonRemoveIgnore
            // 
            this.buttonRemoveIgnore.Location = new System.Drawing.Point(362, 38);
            this.buttonRemoveIgnore.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonRemoveIgnore.Name = "buttonRemoveIgnore";
            this.buttonRemoveIgnore.Size = new System.Drawing.Size(87, 27);
            this.buttonRemoveIgnore.TabIndex = 35;
            this.buttonRemoveIgnore.Text = "Remove";
            this.buttonRemoveIgnore.UseVisualStyleBackColor = true;
            this.buttonRemoveIgnore.Click += new System.EventHandler(this.buttonRemoveIgnore_Click);
            // 
            // buttonAddIgnore
            // 
            this.buttonAddIgnore.Location = new System.Drawing.Point(362, 5);
            this.buttonAddIgnore.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonAddIgnore.Name = "buttonAddIgnore";
            this.buttonAddIgnore.Size = new System.Drawing.Size(87, 27);
            this.buttonAddIgnore.TabIndex = 34;
            this.buttonAddIgnore.Text = "Add";
            this.buttonAddIgnore.UseVisualStyleBackColor = true;
            this.buttonAddIgnore.Click += new System.EventHandler(this.buttonAddIgnore_Click);
            // 
            // imageListEditor
            // 
            this.imageListEditor.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListEditor.ImageStream")));
            this.imageListEditor.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListEditor.Images.SetKeyName(0, "quickconnect.png");
            this.imageListEditor.Images.SetKeyName(1, "info.png");
            this.imageListEditor.Images.SetKeyName(2, "refresh.png");
            this.imageListEditor.Images.SetKeyName(3, "autoperform.png");
            // 
            // buttonRemoveServer
            // 
            this.buttonRemoveServer.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRemoveServer.Location = new System.Drawing.Point(4, 245);
            this.buttonRemoveServer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonRemoveServer.Name = "buttonRemoveServer";
            this.buttonRemoveServer.Size = new System.Drawing.Size(117, 31);
            this.buttonRemoveServer.TabIndex = 37;
            this.buttonRemoveServer.Text = "Remove Server";
            this.buttonRemoveServer.UseVisualStyleBackColor = true;
            this.buttonRemoveServer.Click += new System.EventHandler(this.buttonRemoveServer_Click);
            // 
            // labelNote
            // 
            this.labelNote.Location = new System.Drawing.Point(362, 114);
            this.labelNote.Name = "labelNote";
            this.labelNote.Size = new System.Drawing.Size(252, 62);
            this.labelNote.TabIndex = 41;
            this.labelNote.Text = "Note: For wildcards, use the . character for nick names";
            // 
            // FormServers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(625, 279);
            this.Controls.Add(this.buttonRemoveServer);
            this.Controls.Add(this.tabControlSettings);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormServers";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Server Editor";
            this.tabControlSettings.ResumeLayout(false);
            this.tabPageMain.ResumeLayout(false);
            this.tabPageMain.PerformLayout();
            this.tabPageExtra.ResumeLayout(false);
            this.tabPageExtra.PerformLayout();
            this.tabPageAutoJoin.ResumeLayout(false);
            this.tabPageAutoJoin.PerformLayout();
            this.tabPageAutoPerform.ResumeLayout(false);
            this.tabPageAutoPerform.PerformLayout();
            this.tabPageIgnore.ResumeLayout(false);
            this.tabPageIgnore.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.TabControl tabControlSettings;
        private System.Windows.Forms.TabPage tabPageMain;
        private System.Windows.Forms.TextBox textQuitMessage;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textFullName;
        private System.Windows.Forms.TextBox textIdentName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textServerPort;
        private System.Windows.Forms.TextBox textServername;
        private System.Windows.Forms.TextBox textNickName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPageAutoJoin;
        private System.Windows.Forms.TextBox textChannel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabPage tabPageAutoPerform;
        private System.Windows.Forms.TextBox textAutoPerform;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.TextBox textDisplayName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox checkAutoJoin;
        private System.Windows.Forms.CheckBox checkAutoPerform;
        private System.Windows.Forms.ListView listChannel;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.TabPage tabPageExtra;
        private System.Windows.Forms.CheckBox checkPingPong;
        private System.Windows.Forms.CheckBox checkMOTD;
        private System.Windows.Forms.CheckBox checkModeI;
        private System.Windows.Forms.TextBox textAltNickName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox checkRejoinChannel;
        private System.Windows.Forms.ComboBox comboEncoding;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button buttonEdit;
        private System.Windows.Forms.Button buttonRemoveServer;
        private System.Windows.Forms.TextBox textAwayNick;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox checkDisableCTCP;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textServerPassword;
        private System.Windows.Forms.CheckBox checkAutoJoinDelay;
        private System.Windows.Forms.ImageList imageListEditor;
        private System.Windows.Forms.TabPage tabPageIgnore;
        private System.Windows.Forms.ListView listIgnore;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.CheckBox checkIgnore;
        private System.Windows.Forms.TextBox textIgnore;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button buttonEditIgnore;
        private System.Windows.Forms.Button buttonRemoveIgnore;
        private System.Windows.Forms.Button buttonAddIgnore;
        private System.Windows.Forms.Label labelNote;
    }
}