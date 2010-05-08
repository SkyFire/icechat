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
            this.panelSearch = new System.Windows.Forms.Panel();
            this.textSearch = new System.Windows.Forms.TextBox();
            this.labelFind = new System.Windows.Forms.Label();
            this.buttonNext = new System.Windows.Forms.Button();
            this.buttonPrevious = new System.Windows.Forms.Button();
            this.textInput = new IceChat.IceInputBox();
            this.panelSearch.SuspendLayout();
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
            StaticMethods.LoadResourceImage(buttonEmoticonPicker, "Smile.png");
            this.buttonEmoticonPicker.Dock = System.Windows.Forms.DockStyle.Left;
            this.buttonEmoticonPicker.Location = new System.Drawing.Point(0, 30);
            this.buttonEmoticonPicker.Name = "buttonEmoticonPicker";
            this.buttonEmoticonPicker.Size = new System.Drawing.Size(28, 23);
            this.buttonEmoticonPicker.TabIndex = 2;
            this.buttonEmoticonPicker.UseVisualStyleBackColor = true;
            this.buttonEmoticonPicker.Click += new System.EventHandler(this.buttonEmoticonPicker_Click);
            // 
            // buttonColorPicker
            // 
            StaticMethods.LoadResourceImage(buttonColorPicker, "color.png");
            this.buttonColorPicker.Dock = System.Windows.Forms.DockStyle.Left;
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
            // textSearch
            // 
            this.textSearch.Location = new System.Drawing.Point(62, 3);
            this.textSearch.Name = "textSearch";
            this.textSearch.Size = new System.Drawing.Size(201, 23);
            this.textSearch.TabIndex = 0;
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
            // textInput
            // 
            this.textInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textInput.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textInput.HideSelection = false;
            this.textInput.Location = new System.Drawing.Point(56, 30);
            this.textInput.MaxLength = 512;
            this.textInput.Name = "textInput";
            this.textInput.Size = new System.Drawing.Size(506, 16);
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
            this.Controls.Add(this.panelSearch);
            this.Name = "InputPanel";
            this.Size = new System.Drawing.Size(631, 53);
            this.panelSearch.ResumeLayout(false);
            this.panelSearch.PerformLayout();
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
    }
}
