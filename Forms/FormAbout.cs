using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IceChat
{
    public partial class FormAbout : Form
    {
        public FormAbout()
        {
            InitializeComponent();
            this.Text = IceChat.Properties.Settings.Default.ProgramID + " " + IceChat.Properties.Settings.Default.Version;
            ApplyLanguage();
        }

        private void ApplyLanguage()
        {
            labelAbout.Text = FormMain.Instance.IceChatLanguage.aboutText;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.icechat.net/");
        }
    }
}
