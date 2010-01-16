namespace IceChat
{
    partial class NickList
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
            this.panelButtons = new System.Windows.Forms.Panel();
            this.buttonWhois = new System.Windows.Forms.Button();
            this.buttonKick = new System.Windows.Forms.Button();
            this.buttonQuery = new System.Windows.Forms.Button();
            this.buttonHop = new System.Windows.Forms.Button();
            this.buttonInfo = new System.Windows.Forms.Button();
            this.buttonBan = new System.Windows.Forms.Button();
            this.buttonVoice = new System.Windows.Forms.Button();
            this.buttonOp = new System.Windows.Forms.Button();
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.contextMenuNickList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.opToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelButtons.SuspendLayout();
            this.contextMenuNickList.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.buttonWhois);
            this.panelButtons.Controls.Add(this.buttonKick);
            this.panelButtons.Controls.Add(this.buttonQuery);
            this.panelButtons.Controls.Add(this.buttonHop);
            this.panelButtons.Controls.Add(this.buttonInfo);
            this.panelButtons.Controls.Add(this.buttonBan);
            this.panelButtons.Controls.Add(this.buttonVoice);
            this.panelButtons.Controls.Add(this.buttonOp);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelButtons.Location = new System.Drawing.Point(0, 275);
            this.panelButtons.Margin = new System.Windows.Forms.Padding(4);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(200, 63);
            this.panelButtons.TabIndex = 2;
            // 
            // buttonWhois
            // 
            this.buttonWhois.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonWhois.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonWhois.Location = new System.Drawing.Point(147, 31);
            this.buttonWhois.Margin = new System.Windows.Forms.Padding(1);
            this.buttonWhois.Name = "buttonWhois";
            this.buttonWhois.Size = new System.Drawing.Size(46, 25);
            this.buttonWhois.TabIndex = 7;
            this.buttonWhois.Text = "Whois";
            this.buttonWhois.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonWhois.UseVisualStyleBackColor = true;
            this.buttonWhois.Click += new System.EventHandler(this.buttonWhois_Click);
            // 
            // buttonKick
            // 
            this.buttonKick.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonKick.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonKick.Location = new System.Drawing.Point(99, 31);
            this.buttonKick.Margin = new System.Windows.Forms.Padding(1);
            this.buttonKick.Name = "buttonKick";
            this.buttonKick.Size = new System.Drawing.Size(46, 25);
            this.buttonKick.TabIndex = 6;
            this.buttonKick.Text = "Kick";
            this.buttonKick.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonKick.UseVisualStyleBackColor = true;
            this.buttonKick.Click += new System.EventHandler(this.buttonKick_Click);
            // 
            // buttonQuery
            // 
            this.buttonQuery.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonQuery.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonQuery.Location = new System.Drawing.Point(51, 31);
            this.buttonQuery.Margin = new System.Windows.Forms.Padding(1);
            this.buttonQuery.Name = "buttonQuery";
            this.buttonQuery.Size = new System.Drawing.Size(46, 25);
            this.buttonQuery.TabIndex = 5;
            this.buttonQuery.Text = "Query";
            this.buttonQuery.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonQuery.UseVisualStyleBackColor = true;
            this.buttonQuery.Click += new System.EventHandler(this.buttonQuery_Click);
            // 
            // buttonHop
            // 
            this.buttonHop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonHop.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonHop.Location = new System.Drawing.Point(4, 32);
            this.buttonHop.Margin = new System.Windows.Forms.Padding(1);
            this.buttonHop.Name = "buttonHop";
            this.buttonHop.Size = new System.Drawing.Size(44, 25);
            this.buttonHop.TabIndex = 4;
            this.buttonHop.Text = "H-Op";
            this.buttonHop.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonHop.UseVisualStyleBackColor = true;
            this.buttonHop.Click += new System.EventHandler(this.buttonHop_Click);
            // 
            // buttonInfo
            // 
            this.buttonInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonInfo.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonInfo.Location = new System.Drawing.Point(147, 4);
            this.buttonInfo.Margin = new System.Windows.Forms.Padding(1);
            this.buttonInfo.Name = "buttonInfo";
            this.buttonInfo.Size = new System.Drawing.Size(46, 25);
            this.buttonInfo.TabIndex = 3;
            this.buttonInfo.Text = "Info";
            this.buttonInfo.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonInfo.UseVisualStyleBackColor = true;
            this.buttonInfo.Click += new System.EventHandler(this.buttonInfo_Click);
            // 
            // buttonBan
            // 
            this.buttonBan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonBan.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonBan.Location = new System.Drawing.Point(99, 4);
            this.buttonBan.Margin = new System.Windows.Forms.Padding(1);
            this.buttonBan.Name = "buttonBan";
            this.buttonBan.Size = new System.Drawing.Size(46, 25);
            this.buttonBan.TabIndex = 2;
            this.buttonBan.Text = "Ban";
            this.buttonBan.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonBan.UseVisualStyleBackColor = true;
            this.buttonBan.Click += new System.EventHandler(this.buttonBan_Click);
            // 
            // buttonVoice
            // 
            this.buttonVoice.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonVoice.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonVoice.Location = new System.Drawing.Point(51, 4);
            this.buttonVoice.Margin = new System.Windows.Forms.Padding(1);
            this.buttonVoice.Name = "buttonVoice";
            this.buttonVoice.Size = new System.Drawing.Size(46, 25);
            this.buttonVoice.TabIndex = 1;
            this.buttonVoice.Text = "Voice";
            this.buttonVoice.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonVoice.UseVisualStyleBackColor = true;
            this.buttonVoice.Click += new System.EventHandler(this.buttonVoice_Click);
            // 
            // buttonOp
            // 
            this.buttonOp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOp.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOp.Location = new System.Drawing.Point(4, 4);
            this.buttonOp.Margin = new System.Windows.Forms.Padding(1);
            this.buttonOp.Name = "buttonOp";
            this.buttonOp.Size = new System.Drawing.Size(44, 25);
            this.buttonOp.TabIndex = 0;
            this.buttonOp.Text = "Op";
            this.buttonOp.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonOp.UseVisualStyleBackColor = true;
            this.buttonOp.Click += new System.EventHandler(this.buttonOp_Click);
            // 
            // vScrollBar
            // 
            this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar.LargeChange = 2;
            this.vScrollBar.Location = new System.Drawing.Point(183, 0);
            this.vScrollBar.Maximum = 1;
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Padding = new System.Windows.Forms.Padding(0, 23, 0, 0);
            this.vScrollBar.Size = new System.Drawing.Size(17, 275);
            this.vScrollBar.TabIndex = 2;
            this.vScrollBar.Value = 1;
            this.vScrollBar.Visible = false;
            // 
            // contextMenuNickList
            // 
            this.contextMenuNickList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.opToolStripMenuItem});
            this.contextMenuNickList.Name = "contextMenuNickList";
            this.contextMenuNickList.Size = new System.Drawing.Size(91, 26);
            // 
            // opToolStripMenuItem
            // 
            this.opToolStripMenuItem.Name = "opToolStripMenuItem";
            this.opToolStripMenuItem.Size = new System.Drawing.Size(90, 22);
            this.opToolStripMenuItem.Text = "Op";
            // 
            // NickList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.vScrollBar);
            this.Controls.Add(this.panelButtons);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "NickList";
            this.Size = new System.Drawing.Size(200, 338);
            this.panelButtons.ResumeLayout(false);
            this.contextMenuNickList.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button buttonOp;
        private System.Windows.Forms.Button buttonVoice;
        private System.Windows.Forms.VScrollBar vScrollBar;
        private System.Windows.Forms.ContextMenuStrip contextMenuNickList;
        private System.Windows.Forms.ToolStripMenuItem opToolStripMenuItem;
        private System.Windows.Forms.Button buttonWhois;
        private System.Windows.Forms.Button buttonKick;
        private System.Windows.Forms.Button buttonQuery;
        private System.Windows.Forms.Button buttonHop;
        private System.Windows.Forms.Button buttonInfo;
        private System.Windows.Forms.Button buttonBan;

    }
}
