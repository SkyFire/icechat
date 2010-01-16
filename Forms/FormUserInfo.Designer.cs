namespace IceChat
{
    partial class FormUserInfo
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
            this.labelTopicSetBy = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.listChannels = new System.Windows.Forms.ListBox();
            this.buttonClose = new System.Windows.Forms.Button();
            this.textNick = new System.Windows.Forms.TextBox();
            this.textHost = new System.Windows.Forms.TextBox();
            this.textFullName = new System.Windows.Forms.TextBox();
            this.textIdleTime = new System.Windows.Forms.TextBox();
            this.textClient = new System.Windows.Forms.TextBox();
            this.textLogonTime = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // labelTopicSetBy
            // 
            this.labelTopicSetBy.AutoSize = true;
            this.labelTopicSetBy.Location = new System.Drawing.Point(12, 9);
            this.labelTopicSetBy.Name = "labelTopicSetBy";
            this.labelTopicSetBy.Size = new System.Drawing.Size(82, 16);
            this.labelTopicSetBy.TabIndex = 2;
            this.labelTopicSetBy.Text = "Nick Name:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Host:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "Full Name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "Idle Time:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 125);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 16);
            this.label4.TabIndex = 6;
            this.label4.Text = "Client:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 154);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(101, 16);
            this.label5.TabIndex = 7;
            this.label5.Text = "Logged on at:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 183);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(96, 16);
            this.label6.TabIndex = 8;
            this.label6.Text = "On Channels:";
            // 
            // listChannels
            // 
            this.listChannels.FormattingEnabled = true;
            this.listChannels.ItemHeight = 16;
            this.listChannels.Location = new System.Drawing.Point(12, 202);
            this.listChannels.Name = "listChannels";
            this.listChannels.Size = new System.Drawing.Size(324, 116);
            this.listChannels.TabIndex = 9;
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(261, 324);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 10;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // textNick
            // 
            this.textNick.Location = new System.Drawing.Point(130, 6);
            this.textNick.Name = "textNick";
            this.textNick.Size = new System.Drawing.Size(206, 23);
            this.textNick.TabIndex = 11;
            // 
            // textHost
            // 
            this.textHost.Location = new System.Drawing.Point(130, 35);
            this.textHost.Name = "textHost";
            this.textHost.Size = new System.Drawing.Size(206, 23);
            this.textHost.TabIndex = 12;
            // 
            // textFullName
            // 
            this.textFullName.Location = new System.Drawing.Point(130, 64);
            this.textFullName.Name = "textFullName";
            this.textFullName.Size = new System.Drawing.Size(206, 23);
            this.textFullName.TabIndex = 13;
            // 
            // textIdleTime
            // 
            this.textIdleTime.Location = new System.Drawing.Point(130, 93);
            this.textIdleTime.Name = "textIdleTime";
            this.textIdleTime.Size = new System.Drawing.Size(206, 23);
            this.textIdleTime.TabIndex = 14;
            // 
            // textClient
            // 
            this.textClient.Location = new System.Drawing.Point(130, 122);
            this.textClient.Name = "textClient";
            this.textClient.ReadOnly = true;
            this.textClient.Size = new System.Drawing.Size(206, 23);
            this.textClient.TabIndex = 15;
            this.textClient.Text = "<not available>";
            // 
            // textLogonTime
            // 
            this.textLogonTime.Location = new System.Drawing.Point(130, 151);
            this.textLogonTime.Name = "textLogonTime";
            this.textLogonTime.Size = new System.Drawing.Size(206, 23);
            this.textLogonTime.TabIndex = 16;
            // 
            // FormUserInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(348, 356);
            this.Controls.Add(this.textLogonTime);
            this.Controls.Add(this.textClient);
            this.Controls.Add(this.textIdleTime);
            this.Controls.Add(this.textFullName);
            this.Controls.Add(this.textHost);
            this.Controls.Add(this.textNick);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.listChannels);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelTopicSetBy);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormUserInfo";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "User Information";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTopicSetBy;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListBox listChannels;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.TextBox textNick;
        private System.Windows.Forms.TextBox textHost;
        private System.Windows.Forms.TextBox textFullName;
        private System.Windows.Forms.TextBox textIdleTime;
        private System.Windows.Forms.TextBox textClient;
        private System.Windows.Forms.TextBox textLogonTime;
    }
}