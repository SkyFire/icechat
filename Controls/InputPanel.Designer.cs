namespace IceChat2009
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
            this.textInput = new IceChat2009.IceInputBox();
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
            // textInput
            // 
            this.textInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textInput.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textInput.HideSelection = false;
            this.textInput.Location = new System.Drawing.Point(0, 0);
            this.textInput.MaxLength = 512;
            this.textInput.Name = "textInput";
            this.textInput.Size = new System.Drawing.Size(562, 23);
            this.textInput.TabIndex = 0;
            this.textInput.WordWrap = false;
            // 
            // InputPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textInput);
            this.Controls.Add(this.buttonSend);
            this.Name = "InputPanel";
            this.Size = new System.Drawing.Size(631, 23);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private IceInputBox textInput;
        private System.Windows.Forms.Button buttonSend;
    }
}
