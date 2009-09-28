namespace IceChat2009
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormServers));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.tabControlSettings = new System.Windows.Forms.TabControl();
            this.tabPageMain = new System.Windows.Forms.TabPage();
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
            this.checkRejoinChannel = new System.Windows.Forms.CheckBox();
            this.checkPingPong = new System.Windows.Forms.CheckBox();
            this.checkMOTD = new System.Windows.Forms.CheckBox();
            this.checkModeI = new System.Windows.Forms.CheckBox();
            this.tabPageAutoJoin = new System.Windows.Forms.TabPage();
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
            this.tabControlSettings.SuspendLayout();
            this.tabPageMain.SuspendLayout();
            this.tabPageExtra.SuspendLayout();
            this.tabPageAutoJoin.SuspendLayout();
            this.tabPageAutoPerform.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCancel.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCancel.Location = new System.Drawing.Point(517, 244);
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
            this.buttonSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSave.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSave.Location = new System.Drawing.Point(409, 245);
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
            this.tabControlSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControlSettings.Location = new System.Drawing.Point(0, 0);
            this.tabControlSettings.Margin = new System.Windows.Forms.Padding(0);
            this.tabControlSettings.Name = "tabControlSettings";
            this.tabControlSettings.SelectedIndex = 0;
            this.tabControlSettings.Size = new System.Drawing.Size(625, 237);
            this.tabControlSettings.TabIndex = 36;
            // 
            // tabPageMain
            // 
            this.tabPageMain.BackColor = System.Drawing.SystemColors.Control;
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
            this.tabPageMain.Location = new System.Drawing.Point(4, 25);
            this.tabPageMain.Margin = new System.Windows.Forms.Padding(0);
            this.tabPageMain.Name = "tabPageMain";
            this.tabPageMain.Size = new System.Drawing.Size(617, 208);
            this.tabPageMain.TabIndex = 0;
            this.tabPageMain.Text = "Main Settings";
            // 
            // textAltNickName
            // 
            this.textAltNickName.Location = new System.Drawing.Point(391, 69);
            this.textAltNickName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textAltNickName.Name = "textAltNickName";
            this.textAltNickName.Size = new System.Drawing.Size(150, 23);
            this.textAltNickName.TabIndex = 48;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(283, 68);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(99, 16);
            this.label9.TabIndex = 47;
            this.label9.Text = "Alt Nick Name";
            // 
            // textDisplayName
            // 
            this.textDisplayName.Location = new System.Drawing.Point(115, 36);
            this.textDisplayName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textDisplayName.Name = "textDisplayName";
            this.textDisplayName.Size = new System.Drawing.Size(150, 23);
            this.textDisplayName.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 35);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 16);
            this.label5.TabIndex = 46;
            this.label5.Text = "Display Name";
            // 
            // textQuitMessage
            // 
            this.textQuitMessage.Location = new System.Drawing.Point(115, 162);
            this.textQuitMessage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textQuitMessage.Name = "textQuitMessage";
            this.textQuitMessage.Size = new System.Drawing.Size(218, 23);
            this.textQuitMessage.TabIndex = 6;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(5, 161);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(97, 16);
            this.label8.TabIndex = 45;
            this.label8.Text = "Quit Message";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(5, 130);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 16);
            this.label7.TabIndex = 43;
            this.label7.Text = "Full Name";
            // 
            // textFullName
            // 
            this.textFullName.Location = new System.Drawing.Point(115, 131);
            this.textFullName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textFullName.Name = "textFullName";
            this.textFullName.Size = new System.Drawing.Size(218, 23);
            this.textFullName.TabIndex = 5;
            // 
            // textIdentName
            // 
            this.textIdentName.Location = new System.Drawing.Point(115, 100);
            this.textIdentName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textIdentName.Name = "textIdentName";
            this.textIdentName.Size = new System.Drawing.Size(150, 23);
            this.textIdentName.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 99);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(84, 16);
            this.label6.TabIndex = 41;
            this.label6.Text = "Ident Name";
            // 
            // textServerPort
            // 
            this.textServerPort.Location = new System.Drawing.Point(391, 8);
            this.textServerPort.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textServerPort.Name = "textServerPort";
            this.textServerPort.Size = new System.Drawing.Size(53, 23);
            this.textServerPort.TabIndex = 1;
            // 
            // textServername
            // 
            this.textServername.Location = new System.Drawing.Point(115, 8);
            this.textServername.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textServername.Name = "textServername";
            this.textServername.Size = new System.Drawing.Size(150, 23);
            this.textServername.TabIndex = 0;
            // 
            // textNickName
            // 
            this.textNickName.Location = new System.Drawing.Point(115, 69);
            this.textNickName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textNickName.Name = "textNickName";
            this.textNickName.Size = new System.Drawing.Size(150, 23);
            this.textNickName.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(283, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 16);
            this.label3.TabIndex = 39;
            this.label3.Text = "Server Port";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(5, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 16);
            this.label2.TabIndex = 38;
            this.label2.Text = "Server Name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(5, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 16);
            this.label1.TabIndex = 36;
            this.label1.Text = "Nick Name";
            // 
            // tabPageExtra
            // 
            this.tabPageExtra.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageExtra.Controls.Add(this.checkRejoinChannel);
            this.tabPageExtra.Controls.Add(this.checkPingPong);
            this.tabPageExtra.Controls.Add(this.checkMOTD);
            this.tabPageExtra.Controls.Add(this.checkModeI);
            this.tabPageExtra.Location = new System.Drawing.Point(4, 25);
            this.tabPageExtra.Name = "tabPageExtra";
            this.tabPageExtra.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageExtra.Size = new System.Drawing.Size(617, 208);
            this.tabPageExtra.TabIndex = 3;
            this.tabPageExtra.Text = "Extra Settings";
            // 
            // checkRejoinChannel
            // 
            this.checkRejoinChannel.AutoSize = true;
            this.checkRejoinChannel.Location = new System.Drawing.Point(8, 89);
            this.checkRejoinChannel.Name = "checkRejoinChannel";
            this.checkRejoinChannel.Size = new System.Drawing.Size(220, 20);
            this.checkRejoinChannel.TabIndex = 3;
            this.checkRejoinChannel.Text = "Re-Join Channels on Connect";
            this.checkRejoinChannel.UseVisualStyleBackColor = true;
            // 
            // checkPingPong
            // 
            this.checkPingPong.AutoSize = true;
            this.checkPingPong.Location = new System.Drawing.Point(8, 63);
            this.checkPingPong.Name = "checkPingPong";
            this.checkPingPong.Size = new System.Drawing.Size(209, 20);
            this.checkPingPong.TabIndex = 2;
            this.checkPingPong.Text = "Show PING PONG Messages";
            this.checkPingPong.UseVisualStyleBackColor = true;
            // 
            // checkMOTD
            // 
            this.checkMOTD.AutoSize = true;
            this.checkMOTD.Location = new System.Drawing.Point(8, 37);
            this.checkMOTD.Name = "checkMOTD";
            this.checkMOTD.Size = new System.Drawing.Size(188, 20);
            this.checkMOTD.TabIndex = 1;
            this.checkMOTD.Text = "Show MOTD on Connect";
            this.checkMOTD.UseVisualStyleBackColor = true;
            // 
            // checkModeI
            // 
            this.checkModeI.AutoSize = true;
            this.checkModeI.Location = new System.Drawing.Point(8, 11);
            this.checkModeI.Name = "checkModeI";
            this.checkModeI.Size = new System.Drawing.Size(188, 20);
            this.checkModeI.TabIndex = 0;
            this.checkModeI.Text = "Set Mode +i on Connect";
            this.checkModeI.UseVisualStyleBackColor = true;
            // 
            // tabPageAutoJoin
            // 
            this.tabPageAutoJoin.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageAutoJoin.Controls.Add(this.listChannel);
            this.tabPageAutoJoin.Controls.Add(this.checkAutoJoin);
            this.tabPageAutoJoin.Controls.Add(this.buttonRemove);
            this.tabPageAutoJoin.Controls.Add(this.buttonAdd);
            this.tabPageAutoJoin.Controls.Add(this.textChannel);
            this.tabPageAutoJoin.Controls.Add(this.label4);
            this.tabPageAutoJoin.Location = new System.Drawing.Point(4, 25);
            this.tabPageAutoJoin.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPageAutoJoin.Name = "tabPageAutoJoin";
            this.tabPageAutoJoin.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPageAutoJoin.Size = new System.Drawing.Size(617, 208);
            this.tabPageAutoJoin.TabIndex = 1;
            this.tabPageAutoJoin.Text = "AutoJoin";
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
            this.checkAutoJoin.AutoSize = true;
            this.checkAutoJoin.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.checkAutoJoin.Location = new System.Drawing.Point(3, 184);
            this.checkAutoJoin.Name = "checkAutoJoin";
            this.checkAutoJoin.Size = new System.Drawing.Size(611, 20);
            this.checkAutoJoin.TabIndex = 31;
            this.checkAutoJoin.Text = "Enable AutoJoin";
            this.checkAutoJoin.UseVisualStyleBackColor = true;
            // 
            // buttonRemove
            // 
            this.buttonRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRemove.Location = new System.Drawing.Point(373, 41);
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
            this.buttonAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAdd.Location = new System.Drawing.Point(373, 8);
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
            // FormServers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(625, 279);
            this.Controls.Add(this.tabControlSettings);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormServers";
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
    }
}