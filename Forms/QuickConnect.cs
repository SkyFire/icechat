using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IceChat
{
    public partial class QuickConnect : Form
    {
        public delegate void QuickConnectServerDelegate(ServerSetting s);
        public event QuickConnectServerDelegate QuickConnectServer;
        
        public QuickConnect()
        {
            InitializeComponent();
            ApplyLanguage();
        }

        public void ApplyLanguage()
        {
            IceChatLanguage iceChatLanguage = FormMain.Instance.IceChatLanguage;

            label1.Text = iceChatLanguage.quickConnectLblServer;
            label2.Text = iceChatLanguage.quickConnectLblNick;
            label3.Text = iceChatLanguage.quickConnectLblChannel;
            buttonConnect.Text = iceChatLanguage.quickConnectButtonConnect;
            Text = iceChatLanguage.quickConnectTitle;
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (textServer.Text.Length == 0)
            {
                MessageBox.Show(FormMain.Instance.IceChatLanguage.quickConnectErrorNoServer, FormMain.Instance.IceChatLanguage.quickConnectTitle);
                return;
            }
            if (textNick.Text.Length == 0)
            {
                MessageBox.Show(FormMain.Instance.IceChatLanguage.quickConnectErrorNoNick, FormMain.Instance.IceChatLanguage.quickConnectTitle);
                return;
            }

            if (QuickConnectServer != null)
            {
                ServerSetting s = new ServerSetting();
                //check if a server:port was entered
                if (textServer.Text.IndexOf(':') != -1)
                {
                    s.ServerName = textServer.Text.Substring(0, textServer.Text.IndexOf(':'));
                    s.ServerPort = textServer.Text.Substring(textServer.Text.IndexOf(':') + 1);
                }
                else if (textServer.Text.IndexOf(' ') != -1)
                {
                    s.ServerName = textServer.Text.Substring(0, textServer.Text.IndexOf(' '));
                    s.ServerPort = textServer.Text.Substring(textServer.Text.IndexOf(' ') + 1);
                }
                else
                    s.ServerName = textServer.Text;

                s.NickName = textNick.Text;
                if (textChannel.Text.Length > 0)
                {
                    s.AutoJoinEnable = true;
                    s.AutoJoinChannels = new string[] { textChannel.Text };
                }

                QuickConnectServer(s);
            }

            this.Close();
        }
    }
}
