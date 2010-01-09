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
            this.buttonSend = new System.Windows.Forms.Button();
            this.buttonEmoticonPicker = new System.Windows.Forms.Button();
            this.buttonColorPicker = new System.Windows.Forms.Button();
            this.textInput = new IceChat.IceInputBox();
            this.SuspendLayout();
            // 
            // buttonSend
            // 
            this.buttonSend.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSend.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSend.Location = new System.Drawing.Point(562, 0);
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
            this.buttonEmoticonPicker.Image = global::IceChat.Properties.Resources.Smile;
            this.buttonEmoticonPicker.Location = new System.Drawing.Point(0, 0);
            this.buttonEmoticonPicker.Name = "buttonEmoticonPicker";
            this.buttonEmoticonPicker.Size = new System.Drawing.Size(28, 23);
            this.buttonEmoticonPicker.TabIndex = 2;
            this.buttonEmoticonPicker.UseVisualStyleBackColor = true;
            this.buttonEmoticonPicker.Click += new System.EventHandler(this.buttonEmoticonPicker_Click);
            // 
            // buttonColorPicker
            // 
            this.buttonColorPicker.Dock = System.Windows.Forms.DockStyle.Left;
            this.buttonColorPicker.Image = global::IceChat.Properties.Resources.color;
            this.buttonColorPicker.Location = new System.Drawing.Point(28, 0);
            this.buttonColorPicker.Name = "buttonColorPicker";
            this.buttonColorPicker.Size = new System.Drawing.Size(28, 23);
            this.buttonColorPicker.TabIndex = 3;
            this.buttonColorPicker.UseVisualStyleBackColor = true;
            this.buttonColorPicker.Click += new System.EventHandler(this.buttonColorPicker_Click);
            // 
            // textInput
            // 
            this.textInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textInput.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textInput.HideSelection = false;
            this.textInput.Location = new System.Drawing.Point(56, 0);
            this.textInput.MaxLength = 512;
            this.textInput.Multiline = true;
            this.textInput.Name = "textInput";
            this.textInput.Size = new System.Drawing.Size(506, 23);
            this.textInput.TabIndex = 0;
            // 
            // InputPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textInput);
            this.Controls.Add(this.buttonColorPicker);
            this.Controls.Add(this.buttonEmoticonPicker);
            this.Controls.Add(this.buttonSend);
            this.Name = "InputPanel";
            this.Size = new System.Drawing.Size(631, 23);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private IceInputBox textInput;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.Button buttonEmoticonPicker;
        private System.Windows.Forms.Button buttonColorPicker;
    }
}
