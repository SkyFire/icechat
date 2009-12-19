namespace IceChat2009
{
    partial class FormChannelInfo
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.textTopic = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.buttonClose = new System.Windows.Forms.Button();
            this.labelTopicSetBy = new System.Windows.Forms.Label();
            this.labelChannelCreate = new System.Windows.Forms.Label();
            this.checkModen = new System.Windows.Forms.CheckBox();
            this.checkModet = new System.Windows.Forms.CheckBox();
            this.checkModei = new System.Windows.Forms.CheckBox();
            this.checkModem = new System.Windows.Forms.CheckBox();
            this.checkModes = new System.Windows.Forms.CheckBox();
            this.checkModel = new System.Windows.Forms.CheckBox();
            this.textMaxUsers = new System.Windows.Forms.TextBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(445, 296);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.textMaxUsers);
            this.tabPage1.Controls.Add(this.checkModel);
            this.tabPage1.Controls.Add(this.checkModes);
            this.tabPage1.Controls.Add(this.checkModem);
            this.tabPage1.Controls.Add(this.checkModei);
            this.tabPage1.Controls.Add(this.checkModet);
            this.tabPage1.Controls.Add(this.checkModen);
            this.tabPage1.Controls.Add(this.labelChannelCreate);
            this.tabPage1.Controls.Add(this.labelTopicSetBy);
            this.tabPage1.Controls.Add(this.textTopic);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(437, 267);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Channel Info";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // textTopic
            // 
            this.textTopic.Location = new System.Drawing.Point(6, 6);
            this.textTopic.Name = "textTopic";
            this.textTopic.Size = new System.Drawing.Size(425, 23);
            this.textTopic.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.listView1);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(437, 267);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Ban List";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(348, 302);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(93, 27);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // labelTopicSetBy
            // 
            this.labelTopicSetBy.AutoSize = true;
            this.labelTopicSetBy.Location = new System.Drawing.Point(6, 32);
            this.labelTopicSetBy.Name = "labelTopicSetBy";
            this.labelTopicSetBy.Size = new System.Drawing.Size(99, 16);
            this.labelTopicSetBy.TabIndex = 1;
            this.labelTopicSetBy.Text = "Topic Set By:";
            // 
            // labelChannelCreate
            // 
            this.labelChannelCreate.AutoSize = true;
            this.labelChannelCreate.Location = new System.Drawing.Point(8, 62);
            this.labelChannelCreate.Name = "labelChannelCreate";
            this.labelChannelCreate.Size = new System.Drawing.Size(123, 16);
            this.labelChannelCreate.TabIndex = 2;
            this.labelChannelCreate.Text = "Channel Created:";
            // 
            // checkModen
            // 
            this.checkModen.AutoSize = true;
            this.checkModen.Location = new System.Drawing.Point(6, 89);
            this.checkModen.Name = "checkModen";
            this.checkModen.Size = new System.Drawing.Size(204, 20);
            this.checkModen.TabIndex = 3;
            this.checkModen.Text = "Only ops can change topic";
            this.checkModen.UseVisualStyleBackColor = true;
            // 
            // checkModet
            // 
            this.checkModet.AutoSize = true;
            this.checkModet.Location = new System.Drawing.Point(6, 115);
            this.checkModet.Name = "checkModet";
            this.checkModet.Size = new System.Drawing.Size(171, 20);
            this.checkModet.TabIndex = 4;
            this.checkModet.Text = "No external messages";
            this.checkModet.UseVisualStyleBackColor = true;
            // 
            // checkModei
            // 
            this.checkModei.AutoSize = true;
            this.checkModei.Location = new System.Drawing.Point(6, 141);
            this.checkModei.Name = "checkModei";
            this.checkModei.Size = new System.Drawing.Size(99, 20);
            this.checkModei.TabIndex = 5;
            this.checkModei.Text = "Invite Only";
            this.checkModei.UseVisualStyleBackColor = true;
            // 
            // checkModem
            // 
            this.checkModem.AutoSize = true;
            this.checkModem.Location = new System.Drawing.Point(6, 167);
            this.checkModem.Name = "checkModem";
            this.checkModem.Size = new System.Drawing.Size(97, 20);
            this.checkModem.TabIndex = 6;
            this.checkModem.Text = "Moderated";
            this.checkModem.UseVisualStyleBackColor = true;
            // 
            // checkModes
            // 
            this.checkModes.AutoSize = true;
            this.checkModes.Location = new System.Drawing.Point(6, 193);
            this.checkModes.Name = "checkModes";
            this.checkModes.Size = new System.Drawing.Size(71, 20);
            this.checkModes.TabIndex = 7;
            this.checkModes.Text = "Secret";
            this.checkModes.UseVisualStyleBackColor = true;
            // 
            // checkModel
            // 
            this.checkModel.AutoSize = true;
            this.checkModel.Location = new System.Drawing.Point(6, 219);
            this.checkModel.Name = "checkModel";
            this.checkModel.Size = new System.Drawing.Size(127, 20);
            this.checkModel.TabIndex = 8;
            this.checkModel.Text = "Maximum Users";
            this.checkModel.UseVisualStyleBackColor = true;
            // 
            // textMaxUsers
            // 
            this.textMaxUsers.Location = new System.Drawing.Point(129, 216);
            this.textMaxUsers.Name = "textMaxUsers";
            this.textMaxUsers.Size = new System.Drawing.Size(32, 23);
            this.textMaxUsers.TabIndex = 9;
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(8, 6);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(419, 244);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // FormChannelInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 336);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormChannelInfo";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Channel Information";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.TextBox textTopic;
        private System.Windows.Forms.Label labelChannelCreate;
        private System.Windows.Forms.Label labelTopicSetBy;
        private System.Windows.Forms.CheckBox checkModem;
        private System.Windows.Forms.CheckBox checkModei;
        private System.Windows.Forms.CheckBox checkModet;
        private System.Windows.Forms.CheckBox checkModen;
        private System.Windows.Forms.TextBox textMaxUsers;
        private System.Windows.Forms.CheckBox checkModel;
        private System.Windows.Forms.CheckBox checkModes;
        private System.Windows.Forms.ListView listView1;
    }
}