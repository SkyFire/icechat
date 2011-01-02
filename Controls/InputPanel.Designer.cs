namespace IceChat
{
    partial class InputPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InputPanel));
            this.buttonSend = new System.Windows.Forms.Button();
            this.buttonEmoticonPicker = new System.Windows.Forms.Button();
            this.buttonColorPicker = new System.Windows.Forms.Button();
            this.panelSearch = new System.Windows.Forms.Panel();
            this.buttonPrevious = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.labelFind = new System.Windows.Forms.Label();
            this.textSearch = new System.Windows.Forms.TextBox();
            this.buttonHelp = new System.Windows.Forms.Button();
            this.contextHelpMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuNickName = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuChannel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuServer = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.textInput = new IceChat.IceInputBox();
            this.panelSearch.SuspendLayout();
            this.contextHelpMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonSend
            // 
            this.buttonSend.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonSend.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSend.Location = new System.Drawing.Point(562, 30);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(69, 23);
            this.buttonSend.TabIndex = 1;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // buttonEmoticonPicker
            // 
            this.buttonEmoticonPicker.Dock = System.Windows.Forms.DockStyle.Left;
            //this.buttonEmoticonPicker.Image = ((System.Drawing.Image)(resources.GetObject("buttonEmoticonPicker.Image")));
            this.buttonEmoticonPicker.Location = new System.Drawing.Point(0, 30);
            this.buttonEmoticonPicker.Name = "buttonEmoticonPicker";
            this.buttonEmoticonPicker.Size = new System.Drawing.Size(28, 23);
            this.buttonEmoticonPicker.TabIndex = 2;
            this.buttonEmoticonPicker.UseVisualStyleBackColor = true;
            this.buttonEmoticonPicker.Click += new System.EventHandler(this.buttonEmoticonPicker_Click);
            // 
            // buttonColorPicker
            // 
            this.buttonColorPicker.Dock = System.Windows.Forms.DockStyle.Left;
            //this.buttonColorPicker.Image = ((System.Drawing.Image)(resources.GetObject("buttonColorPicker.Image")));
            this.buttonColorPicker.Location = new System.Drawing.Point(28, 30);
            this.buttonColorPicker.Name = "buttonColorPicker";
            this.buttonColorPicker.Size = new System.Drawing.Size(28, 23);
            this.buttonColorPicker.TabIndex = 3;
            this.buttonColorPicker.UseVisualStyleBackColor = true;
            this.buttonColorPicker.Click += new System.EventHandler(this.buttonColorPicker_Click);
            // 
            // panelSearch
            // 
            this.panelSearch.BackColor = System.Drawing.SystemColors.Menu;
            this.panelSearch.Controls.Add(this.buttonPrevious);
            this.panelSearch.Controls.Add(this.buttonNext);
            this.panelSearch.Controls.Add(this.labelFind);
            this.panelSearch.Controls.Add(this.textSearch);
            this.panelSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSearch.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelSearch.Location = new System.Drawing.Point(0, 0);
            this.panelSearch.Name = "panelSearch";
            this.panelSearch.Size = new System.Drawing.Size(631, 30);
            this.panelSearch.TabIndex = 4;
            this.panelSearch.Visible = false;
            // 
            // buttonPrevious
            // 
            this.buttonPrevious.Location = new System.Drawing.Point(269, 2);
            this.buttonPrevious.Name = "buttonPrevious";
            this.buttonPrevious.Size = new System.Drawing.Size(75, 23);
            this.buttonPrevious.TabIndex = 3;
            this.buttonPrevious.Text = "Previous";
            this.buttonPrevious.UseVisualStyleBackColor = true;
            this.buttonPrevious.Click += new System.EventHandler(this.buttonPrevious_Click);
            // 
            // buttonNext
            // 
            this.buttonNext.Location = new System.Drawing.Point(350, 2);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(75, 23);
            this.buttonNext.TabIndex = 2;
            this.buttonNext.Text = "Next";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // labelFind
            // 
            this.labelFind.AutoSize = true;
            this.labelFind.Location = new System.Drawing.Point(15, 6);
            this.labelFind.Name = "labelFind";
            this.labelFind.Size = new System.Drawing.Size(41, 16);
            this.labelFind.TabIndex = 1;
            this.labelFind.Text = "Find:";
            // 
            // textSearch
            // 
            this.textSearch.Location = new System.Drawing.Point(62, 3);
            this.textSearch.Name = "textSearch";
            this.textSearch.Size = new System.Drawing.Size(201, 23);
            this.textSearch.TabIndex = 0;
            // 
            // buttonHelp
            // 
            this.buttonHelp.Dock = System.Windows.Forms.DockStyle.Left;
            this.buttonHelp.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonHelp.Location = new System.Drawing.Point(56, 30);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(28, 23);
            this.buttonHelp.TabIndex = 5;
            this.buttonHelp.Text = "?";
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttonHelp_MouseDown);
            // 
            // contextHelpMenu
            // 
            this.contextHelpMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuNickName,
            this.menuChannel,
            this.menuServer,
            this.toolStripMenuItem3});
            this.contextHelpMenu.Name = "contextMenuStrip1";
            this.contextHelpMenu.Size = new System.Drawing.Size(194, 92);
            // 
            // menuNickName
            // 
            this.menuNickName.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5});
            this.menuNickName.Name = "menuNickName";
            this.menuNickName.Size = new System.Drawing.Size(193, 22);
            this.menuNickName.Text = "Nickname Commands";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(266, 22);
            this.toolStripMenuItem1.Tag = "/nick $?=\'Choose new Nick name\'";
            this.toolStripMenuItem1.Text = "Change Your Nick name";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(266, 22);
            this.toolStripMenuItem2.Tag = "/msg $?=\'Insert Nick name\' message";
            this.toolStripMenuItem2.Text = "Send a private message to some one";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(266, 22);
            this.toolStripMenuItem4.Tag = "/whois $?=\'Insert Nick name\'";
            this.toolStripMenuItem4.Text = "Perform a whois";
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(266, 22);
            this.toolStripMenuItem5.Tag = "/ping $?=\'Insert Nick name\'";
            this.toolStripMenuItem5.Text = "Ping some one";
            // 
            // menuChannel
            // 
            this.menuChannel.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem6,
            this.toolStripMenuItem7,
            this.toolStripMenuItem8});
            this.menuChannel.Name = "menuChannel";
            this.menuChannel.Size = new System.Drawing.Size(193, 22);
            this.menuChannel.Text = "Channel Commands";
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(158, 22);
            this.toolStripMenuItem6.Tag = "/join $?=\'Insert Channel name\'";
            this.toolStripMenuItem6.Text = "Join a channel";
            this.toolStripMenuItem6.Click += new System.EventHandler(this.toolStripHelpMenuOnClick);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(158, 22);
            this.toolStripMenuItem7.Tag = "/part";
            this.toolStripMenuItem7.Text = "Leave a channel";
            this.toolStripMenuItem7.Click += new System.EventHandler(this.toolStripHelpMenuOnClick);
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(158, 22);
            this.toolStripMenuItem8.Tag = "/me action";
            this.toolStripMenuItem8.Text = "Send an action";
            this.toolStripMenuItem8.Click += new System.EventHandler(this.toolStripHelpMenuOnClick);
            // 
            // menuServer
            // 
            this.menuServer.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem9,
            this.toolStripMenuItem10});
            this.menuServer.Name = "menuServer";
            this.menuServer.Size = new System.Drawing.Size(193, 22);
            this.menuServer.Text = "Server Commands";
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Size = new System.Drawing.Size(176, 22);
            this.toolStripMenuItem9.Tag = "/server $?=\'Insert Server name\'";
            this.toolStripMenuItem9.Text = "Connect to a server";
            this.toolStripMenuItem9.Click += new System.EventHandler(this.toolStripHelpMenuOnClick);
            // 
            // toolStripMenuItem10
            // 
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            this.toolStripMenuItem10.Size = new System.Drawing.Size(176, 22);
            this.toolStripMenuItem10.Tag = "/quit";
            this.toolStripMenuItem10.Text = "Quit a server";
            this.toolStripMenuItem10.Click += new System.EventHandler(this.toolStripHelpMenuOnClick);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(193, 22);
            this.toolStripMenuItem3.Tag = "/help";
            this.toolStripMenuItem3.Text = "Get More Help";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripHelpMenuOnClick);
            // 
            // textInput
            // 
            this.textInput.AccessibleDescription = "Main input area";
            this.textInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textInput.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textInput.HideSelection = false;
            this.textInput.Location = new System.Drawing.Point(84, 30);
            this.textInput.MaxLength = 512;
            this.textInput.Name = "textInput";
            this.textInput.Size = new System.Drawing.Size(478, 16);
            this.textInput.TabIndex = 0;
            // 
            // InputPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textInput);
            this.Controls.Add(this.buttonSend);
            this.Controls.Add(this.buttonHelp);
            this.Controls.Add(this.buttonColorPicker);
            this.Controls.Add(this.buttonEmoticonPicker);
            this.Controls.Add(this.panelSearch);
            this.Name = "InputPanel";
            this.Size = new System.Drawing.Size(631, 53);
            this.panelSearch.ResumeLayout(false);
            this.panelSearch.PerformLayout();
            this.contextHelpMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private IceInputBox textInput;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.Button buttonEmoticonPicker;
        private System.Windows.Forms.Button buttonColorPicker;
        private System.Windows.Forms.Panel panelSearch;
        private System.Windows.Forms.Label labelFind;
        private System.Windows.Forms.TextBox textSearch;
        private System.Windows.Forms.Button buttonPrevious;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.ContextMenuStrip contextHelpMenu;
        private System.Windows.Forms.ToolStripMenuItem menuNickName;
        private System.Windows.Forms.ToolStripMenuItem menuChannel;
        private System.Windows.Forms.ToolStripMenuItem menuServer;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem8;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem9;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem10;
    }
}
