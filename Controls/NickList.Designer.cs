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
            this.panelButtons.Controls.Add(this.buttonVoice);
            this.panelButtons.Controls.Add(this.buttonOp);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelButtons.Location = new System.Drawing.Point(0, 281);
            this.panelButtons.Margin = new System.Windows.Forms.Padding(4);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(200, 57);
            this.panelButtons.TabIndex = 2;
            this.panelButtons.Visible = false;
            // 
            // buttonVoice
            // 
            this.buttonVoice.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonVoice.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonVoice.Location = new System.Drawing.Point(51, 4);
            this.buttonVoice.Margin = new System.Windows.Forms.Padding(1);
            this.buttonVoice.Name = "buttonVoice";
            this.buttonVoice.Size = new System.Drawing.Size(63, 27);
            this.buttonVoice.TabIndex = 1;
            this.buttonVoice.Text = "Voice";
            this.buttonVoice.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonVoice.UseVisualStyleBackColor = true;
            // 
            // buttonOp
            // 
            this.buttonOp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOp.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOp.Location = new System.Drawing.Point(4, 4);
            this.buttonOp.Margin = new System.Windows.Forms.Padding(1);
            this.buttonOp.Name = "buttonOp";
            this.buttonOp.Size = new System.Drawing.Size(44, 27);
            this.buttonOp.TabIndex = 0;
            this.buttonOp.Text = "Op";
            this.buttonOp.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonOp.UseVisualStyleBackColor = true;
            // 
            // vScrollBar
            // 
            this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar.LargeChange = 2;
            this.vScrollBar.Location = new System.Drawing.Point(183, 0);
            this.vScrollBar.Maximum = 1;
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Padding = new System.Windows.Forms.Padding(0, 23, 0, 0);
            this.vScrollBar.Size = new System.Drawing.Size(17, 281);
            this.vScrollBar.TabIndex = 2;
            this.vScrollBar.Value = 1;
            this.vScrollBar.Visible = false;
            // 
            // contextMenuNickList
            // 
            this.contextMenuNickList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.opToolStripMenuItem});
            this.contextMenuNickList.Name = "contextMenuNickList";
            this.contextMenuNickList.Size = new System.Drawing.Size(153, 48);
            // 
            // opToolStripMenuItem
            // 
            this.opToolStripMenuItem.Name = "opToolStripMenuItem";
            this.opToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
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

    }
}
