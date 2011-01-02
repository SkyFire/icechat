/******************************************************************************\
 * IceChat 2009 Internet Relay Chat Client
 *
 * Copyright (C) 2011 Paul Vanderzee <snerf@icechat.net>
 *                                    <www.icechat.net> 
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
 *
 * Please consult the LICENSE.txt file included with this project for
 * more details
 *
\******************************************************************************/

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
