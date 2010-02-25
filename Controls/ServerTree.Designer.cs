namespace IceChat
{
    partial class ServerTree
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerTree));
            this.panelButtons = new System.Windows.Forms.Panel();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonEdit = new System.Windows.Forms.Button();
            this.buttonDisconnect = new System.Windows.Forms.Button();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.imageListServers = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuChannel = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.clearWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeChannelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reJoinChannelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.channelInformationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoJoinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoPerformToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuServer = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openLogFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuQuery = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.clearWindowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.closeWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userInformationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.silenceUserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelButtons.SuspendLayout();
            this.contextMenuChannel.SuspendLayout();
            this.contextMenuServer.SuspendLayout();
            this.contextMenuQuery.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelButtons
            // 
            this.panelButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelButtons.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelButtons.Controls.Add(this.buttonAdd);
            this.panelButtons.Controls.Add(this.buttonEdit);
            this.panelButtons.Controls.Add(this.buttonDisconnect);
            this.panelButtons.Controls.Add(this.buttonConnect);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Location = new System.Drawing.Point(0, 266);
            this.panelButtons.Margin = new System.Windows.Forms.Padding(2);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(180, 57);
            this.panelButtons.TabIndex = 0;
            this.panelButtons.TabStop = true;
            // 
            // buttonAdd
            // 
            this.buttonAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAdd.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAdd.Location = new System.Drawing.Point(99, 28);
            this.buttonAdd.Margin = new System.Windows.Forms.Padding(2);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(77, 24);
            this.buttonAdd.TabIndex = 3;
            this.buttonAdd.TabStop = false;
            this.buttonAdd.Text = "Add";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonEdit
            // 
            this.buttonEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEdit.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEdit.Location = new System.Drawing.Point(99, 2);
            this.buttonEdit.Margin = new System.Windows.Forms.Padding(2);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(77, 24);
            this.buttonEdit.TabIndex = 2;
            this.buttonEdit.TabStop = false;
            this.buttonEdit.Text = "Edit";
            this.buttonEdit.UseVisualStyleBackColor = true;
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // buttonDisconnect
            // 
            this.buttonDisconnect.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDisconnect.Location = new System.Drawing.Point(2, 28);
            this.buttonDisconnect.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonDisconnect.Name = "buttonDisconnect";
            this.buttonDisconnect.Size = new System.Drawing.Size(83, 24);
            this.buttonDisconnect.TabIndex = 2;
            this.buttonDisconnect.TabStop = false;
            this.buttonDisconnect.Text = "Disconnect";
            this.buttonDisconnect.UseVisualStyleBackColor = true;
            this.buttonDisconnect.Click += new System.EventHandler(this.buttonDisconnect_Click);
            // 
            // buttonConnect
            // 
            this.buttonConnect.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonConnect.Location = new System.Drawing.Point(2, 2);
            this.buttonConnect.Margin = new System.Windows.Forms.Padding(2);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(83, 24);
            this.buttonConnect.TabIndex = 1;
            this.buttonConnect.TabStop = false;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // vScrollBar
            // 
            this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar.LargeChange = 2;
            this.vScrollBar.Location = new System.Drawing.Point(163, 0);
            this.vScrollBar.Maximum = 1;
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Padding = new System.Windows.Forms.Padding(0, 25, 0, 0);
            this.vScrollBar.Size = new System.Drawing.Size(17, 266);
            this.vScrollBar.TabIndex = 3;
            this.vScrollBar.Visible = false;
            // 
            // imageListServers
            // 
            this.imageListServers.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListServers.ImageStream")));
            this.imageListServers.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListServers.Images.SetKeyName(0, "disconnect.ico");
            this.imageListServers.Images.SetKeyName(1, "check.ico");
            this.imageListServers.Images.SetKeyName(2, "channel.ico");
            this.imageListServers.Images.SetKeyName(3, "query.ico");
            // 
            // contextMenuChannel
            // 
            this.contextMenuChannel.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearWindowToolStripMenuItem,
            this.closeChannelToolStripMenuItem,
            this.reJoinChannelToolStripMenuItem,
            this.channelInformationToolStripMenuItem});
            this.contextMenuChannel.Name = "contextMenuChannel";
            this.contextMenuChannel.Size = new System.Drawing.Size(185, 92);
            // 
            // clearWindowToolStripMenuItem
            // 
            this.clearWindowToolStripMenuItem.Name = "clearWindowToolStripMenuItem";
            this.clearWindowToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.clearWindowToolStripMenuItem.Text = "Clear Window";
            this.clearWindowToolStripMenuItem.Click += new System.EventHandler(this.clearWindowToolStripMenuItem_Click);
            // 
            // closeChannelToolStripMenuItem
            // 
            this.closeChannelToolStripMenuItem.Name = "closeChannelToolStripMenuItem";
            this.closeChannelToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.closeChannelToolStripMenuItem.Text = "Close Channel";
            this.closeChannelToolStripMenuItem.Click += new System.EventHandler(this.closeChannelToolStripMenuItem_Click);
            // 
            // reJoinChannelToolStripMenuItem
            // 
            this.reJoinChannelToolStripMenuItem.Name = "reJoinChannelToolStripMenuItem";
            this.reJoinChannelToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.reJoinChannelToolStripMenuItem.Text = "Rejoin Channel";
            this.reJoinChannelToolStripMenuItem.Click += new System.EventHandler(this.reJoinChannelToolStripMenuItem_Click);
            // 
            // channelInformationToolStripMenuItem
            // 
            this.channelInformationToolStripMenuItem.Name = "channelInformationToolStripMenuItem";
            this.channelInformationToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.channelInformationToolStripMenuItem.Text = "Channel Information";
            this.channelInformationToolStripMenuItem.Click += new System.EventHandler(this.channelInformationToolStripMenuItem_Click);
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.connectToolStripMenuItem.Text = "Connect";
            this.connectToolStripMenuItem.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // disconnectToolStripMenuItem
            // 
            this.disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            this.disconnectToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.disconnectToolStripMenuItem.Text = "Disconnect";
            this.disconnectToolStripMenuItem.Click += new System.EventHandler(this.disconnectToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // autoJoinToolStripMenuItem
            // 
            this.autoJoinToolStripMenuItem.Name = "autoJoinToolStripMenuItem";
            this.autoJoinToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.autoJoinToolStripMenuItem.Text = "Auto Join";
            this.autoJoinToolStripMenuItem.Click += new System.EventHandler(this.autoJoinToolStripMenuItem_Click);
            // 
            // autoPerformToolStripMenuItem
            // 
            this.autoPerformToolStripMenuItem.Name = "autoPerformToolStripMenuItem";
            this.autoPerformToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.autoPerformToolStripMenuItem.Text = "Auto Perform";
            this.autoPerformToolStripMenuItem.Click += new System.EventHandler(this.autoPerformToolStripMenuItem_Click);
            // 
            // contextMenuServer
            // 
            this.contextMenuServer.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectToolStripMenuItem,
            this.disconnectToolStripMenuItem,
            this.editToolStripMenuItem,
            this.autoJoinToolStripMenuItem,
            this.autoPerformToolStripMenuItem,
            this.openLogFolderToolStripMenuItem});
            this.contextMenuServer.Name = "contextMenuServer";
            this.contextMenuServer.Size = new System.Drawing.Size(163, 136);
            // 
            // openLogFolderToolStripMenuItem
            // 
            this.openLogFolderToolStripMenuItem.Name = "openLogFolderToolStripMenuItem";
            this.openLogFolderToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.openLogFolderToolStripMenuItem.Text = "Open Log Folder";
            this.openLogFolderToolStripMenuItem.Click += new System.EventHandler(this.openLogFolderToolStripMenuItem_Click);
            // 
            // contextMenuQuery
            // 
            this.contextMenuQuery.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearWindowToolStripMenuItem1,
            this.closeWindowToolStripMenuItem,
            this.userInformationToolStripMenuItem,
            this.silenceUserToolStripMenuItem});
            this.contextMenuQuery.Name = "contextMenuQuery";
            this.contextMenuQuery.Size = new System.Drawing.Size(164, 92);
            // 
            // clearWindowToolStripMenuItem1
            // 
            this.clearWindowToolStripMenuItem1.Name = "clearWindowToolStripMenuItem1";
            this.clearWindowToolStripMenuItem1.Size = new System.Drawing.Size(163, 22);
            this.clearWindowToolStripMenuItem1.Text = "Clear Window";
            this.clearWindowToolStripMenuItem1.Click += new System.EventHandler(this.clearWindowToolStripMenuItem1_Click);
            // 
            // closeWindowToolStripMenuItem
            // 
            this.closeWindowToolStripMenuItem.Name = "closeWindowToolStripMenuItem";
            this.closeWindowToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.closeWindowToolStripMenuItem.Text = "Close Window";
            this.closeWindowToolStripMenuItem.Click += new System.EventHandler(this.closeWindowToolStripMenuItem_Click);
            // 
            // userInformationToolStripMenuItem
            // 
            this.userInformationToolStripMenuItem.Name = "userInformationToolStripMenuItem";
            this.userInformationToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.userInformationToolStripMenuItem.Text = "User Information";
            this.userInformationToolStripMenuItem.Click += new System.EventHandler(this.userInformationToolStripMenuItem_Click);
            // 
            // silenceUserToolStripMenuItem
            // 
            this.silenceUserToolStripMenuItem.Name = "silenceUserToolStripMenuItem";
            this.silenceUserToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.silenceUserToolStripMenuItem.Text = "Silence User";
            this.silenceUserToolStripMenuItem.Click += new System.EventHandler(this.silenceUserToolStripMenuItem_Click);
            // 
            // ServerTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.vScrollBar);
            this.Controls.Add(this.panelButtons);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ServerTree";
            this.Size = new System.Drawing.Size(180, 323);
            this.panelButtons.ResumeLayout(false);
            this.contextMenuChannel.ResumeLayout(false);
            this.contextMenuServer.ResumeLayout(false);
            this.contextMenuQuery.ResumeLayout(false);
            this.ResumeLayout(false);

        }



        #endregion

        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button buttonEdit;
        private System.Windows.Forms.Button buttonDisconnect;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.VScrollBar vScrollBar;
        private System.Windows.Forms.ImageList imageListServers;
        private System.Windows.Forms.ContextMenuStrip contextMenuChannel;
        private System.Windows.Forms.ToolStripMenuItem clearWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeChannelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reJoinChannelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem channelInformationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disconnectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoJoinToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoPerformToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuServer;
        private System.Windows.Forms.ContextMenuStrip contextMenuQuery;
        private System.Windows.Forms.ToolStripMenuItem clearWindowToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem closeWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem userInformationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem silenceUserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openLogFolderToolStripMenuItem;
    }
}
