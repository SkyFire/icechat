/******************************************************************************\
 * IceChat 2009 Internet Relay Chat Client
 *
 * Copyright (C) 2010 Paul Vanderzee <snerf@icechat.net>
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
    public partial class FormServers : Form
    {
        private ServerSetting serverSetting;
        
        private bool newServer;

        public delegate void NewServerDelegate(ServerSetting s);
        public event NewServerDelegate NewServer;

        public delegate void SaveServerDelegate(ServerSetting s, bool removeServer);
        public event SaveServerDelegate SaveServer;

        public FormServers()
        {
            InitializeComponent();
            newServer = true;

            this.Text = "Server Editor: New Server";

            foreach (EncodingInfo ei in System.Text.Encoding.GetEncodings())
            {
                try
                {
                    comboEncoding.Items.Add(ei.Name);
                }
                catch { }
            }
            comboEncoding.Text = System.Text.Encoding.Default.WebName.ToString();
            buttonRemoveServer.Enabled = false;

            ApplyLanguage();
        }

        public FormServers(ServerSetting s)
        {
            InitializeComponent();
            serverSetting = s;

            foreach (EncodingInfo ei in System.Text.Encoding.GetEncodings())
            {
                try
                {
                    comboEncoding.Items.Add(ei.Name);
                }
                catch { }
            }

            newServer = false;
            LoadSettings();

            this.Text = "Server Editor: " + s.ServerName;

            ApplyLanguage();
        }

        private void ApplyLanguage()
        {

        }
        
        /// <summary>
        /// Load the Server Settings into the text boxes
        /// </summary>
        private void LoadSettings()
        {
            this.textNickName.Text = serverSetting.NickName;
            this.textAltNickName.Text = serverSetting.AltNickName;
            this.textAwayNick.Text = serverSetting.AwayNickName;
            this.textServername.Text = serverSetting.ServerName;
            this.textServerPort.Text = serverSetting.ServerPort;
            this.textDisplayName.Text = serverSetting.DisplayName;
            
            this.textIdentName.Text = serverSetting.IdentName;
            this.textFullName.Text = serverSetting.FullName;
            this.textQuitMessage.Text = serverSetting.QuitMessage;
            
            this.checkAutoJoin.Checked = serverSetting.AutoJoinEnable;
            this.checkAutoJoinDelay.Checked = serverSetting.AutoJoinDelay;
            this.checkAutoPerform.Checked = serverSetting.AutoPerformEnable;
            this.checkIgnore.Checked = serverSetting.IgnoreListEnable;

            this.checkModeI.Checked = serverSetting.SetModeI;
            this.checkMOTD.Checked = serverSetting.ShowMOTD;
            this.checkPingPong.Checked = serverSetting.ShowPingPong;
            this.checkRejoinChannel.Checked = serverSetting.RejoinChannels;
            this.checkDisableCTCP.Checked = serverSetting.DisableCTCP;
            this.comboEncoding.Text = serverSetting.Encoding;
            this.textServerPassword.Text = serverSetting.Password;
            this.textNickservPassword.Text = serverSetting.NickservPassword;
            this.checkAutoStart.Checked = serverSetting.AutoStart;
            this.checkUseSSL.Checked = serverSetting.UseSSL;

            if (serverSetting.AutoJoinChannels != null)
            {
                foreach (string chan in serverSetting.AutoJoinChannels)
                {
                    if (!chan.StartsWith(";"))
                    {
                        if (chan.IndexOf(' ') > -1)
                        {
                            string channel = chan.Substring(0, chan.IndexOf(' '));
                            string key = chan.Substring(chan.IndexOf(' ') + 1);
                            ListViewItem lvi = new ListViewItem(channel);
                            lvi.SubItems.Add(key);
                            lvi.Checked = true;
                            listChannel.Items.Add(lvi);
                        }
                        else
                        {
                            ListViewItem lvi = new ListViewItem(chan);
                            lvi.Checked = true;
                            listChannel.Items.Add(lvi);
                        }
                    }
                    else
                    {
                        if (chan.IndexOf(' ') > -1)
                        {
                            string channel = chan.Substring(1, chan.IndexOf(' '));
                            string key = chan.Substring(chan.IndexOf(' ') + 1);
                            ListViewItem lvi = new ListViewItem(channel);
                            lvi.SubItems.Add(key);
                            listChannel.Items.Add(lvi);
                        }
                        else
                        {
                            ListViewItem lvi = new ListViewItem(chan.Substring(1));
                            listChannel.Items.Add(lvi);
                        }
                    }
                }
            }

            if (serverSetting.AutoPerform != null)
            {
                foreach (string command in serverSetting.AutoPerform)
                    textAutoPerform.AppendText(command + Environment.NewLine);
            }

            if (serverSetting.IgnoreList != null)
            {
                foreach (string ignore in serverSetting.IgnoreList)
                {
                    if (!ignore.StartsWith(";"))
                    {
                        ListViewItem lvi = new ListViewItem(ignore);
                        lvi.Checked = true;
                        listIgnore.Items.Add(lvi);
                    }
                    else
                        listIgnore.Items.Add(ignore.Substring(1));

                }
            }
        }

        /// <summary>
        /// Update the Server Settings
        /// </summary>
        private void SaveSettings()
        {
            if (serverSetting == null)
                serverSetting = new ServerSetting();

            if (textServername.Text.Length == 0)
                return;

            if (textNickName.Text.Length == 0)
                return;
            
            serverSetting.NickName = textNickName.Text;
            if (textAltNickName.Text.Length > 0)
                serverSetting.AltNickName = textAltNickName.Text;
            else
                serverSetting.AltNickName = textNickName.Text + "_";

            if (textAwayNick.Text.Length > 0)
                serverSetting.AwayNickName = textAwayNick.Text;
            else
                serverSetting.AwayNickName = textNickName.Text + "[A]";

            serverSetting.ServerName = textServername.Text;
            serverSetting.DisplayName = textDisplayName.Text;
            serverSetting.Password = textServerPassword.Text;
            serverSetting.NickservPassword = textNickservPassword.Text;

            if (textServerPort.Text.Length == 0)
                textServerPort.Text = "6667";
            serverSetting.ServerPort = textServerPort.Text;

            if (textIdentName.Text.Length == 0)
                textIdentName.Text = "IceChat09";
            serverSetting.IdentName = textIdentName.Text;

            if (textFullName.Text.Length == 0)
                textFullName.Text = "The Chat Cool People Use";
            serverSetting.FullName = textFullName.Text;

            if (textQuitMessage.Text.Length == 0)
                textQuitMessage.Text = "$randquit";

            serverSetting.QuitMessage = textQuitMessage.Text;

            serverSetting.AutoJoinEnable = checkAutoJoin.Checked;
            serverSetting.AutoJoinDelay = checkAutoJoinDelay.Checked;
            serverSetting.AutoPerformEnable = checkAutoPerform.Checked;
            serverSetting.IgnoreListEnable = checkIgnore.Checked;

            serverSetting.AutoJoinChannels = new string[listChannel.Items.Count];
            for (int i = 0; i < listChannel.Items.Count; i++)
            {
                if (listChannel.Items[i].Checked == false)
                {
                    if (listChannel.Items[i].SubItems.Count > 1)
                        serverSetting.AutoJoinChannels[i] = ";" + listChannel.Items[i].Text + " " + listChannel.Items[i].SubItems[1].Text;
                    else
                        serverSetting.AutoJoinChannels[i] = ";" + listChannel.Items[i].Text;
                }
                else
                {
                    if (listChannel.Items[i].SubItems.Count > 1)
                        serverSetting.AutoJoinChannels[i] = listChannel.Items[i].Text + " " + listChannel.Items[i].SubItems[1].Text;
                    else
                        serverSetting.AutoJoinChannels[i] = listChannel.Items[i].Text;
                }
            }
            
            serverSetting.IgnoreList = new string[listIgnore.Items.Count];
            for (int i = 0; i < listIgnore.Items.Count; i++)
            {
                if (listIgnore.Items[i].Checked == false)
                    serverSetting.IgnoreList[i] = ";" + listIgnore.Items[i].Text;
                else
                    serverSetting.IgnoreList[i] = listIgnore.Items[i].Text;

            }

            serverSetting.AutoPerform = textAutoPerform.Text.Trim().Split(new String[] { "\r\n" }, StringSplitOptions.None);
            
            serverSetting.SetModeI = checkModeI.Checked;
            serverSetting.ShowMOTD = checkMOTD.Checked;
            serverSetting.ShowPingPong = checkPingPong.Checked;
            serverSetting.RejoinChannels = checkRejoinChannel.Checked;
            serverSetting.DisableCTCP = checkDisableCTCP.Checked;
            serverSetting.AutoStart = checkAutoStart.Checked;
            serverSetting.UseSSL = checkUseSSL.Checked;
            serverSetting.Encoding = comboEncoding.Text;

            if (newServer == true)
            {
                //add in the server to the server collection
                if (NewServer != null)
                    NewServer(serverSetting);
            }
            else
                if (SaveServer != null)
                    SaveServer(serverSetting, false);

        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            //save all the server settings first
            SaveSettings();
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (textChannel.Text.Length > 0)
            {
                ListViewItem lvi = null;
                if (textChannel.Text.IndexOf(" ") > -1)
                {
                    string channel = textChannel.Text.Substring(0, textChannel.Text.IndexOf(' '));
                    string key = textChannel.Text.Substring(textChannel.Text.IndexOf(' ') + 1);
                    lvi = new ListViewItem(channel);
                    lvi.SubItems.Add(key);
                }
                else if (textChannel.Text.IndexOf(":") > -1)
                {
                    string channel = textChannel.Text.Substring(0, textChannel.Text.IndexOf(':'));
                    string key = textChannel.Text.Substring(textChannel.Text.IndexOf(':') + 1);
                    lvi = new ListViewItem(channel);
                    lvi.SubItems.Add(key);
                }
                else
                {
                    lvi = new ListViewItem(textChannel.Text);
                }
                
                lvi.Checked = true;                
                listChannel.Items.Add(lvi);
                textChannel.Text = "";
                textChannel.Focus();
            }
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            foreach(ListViewItem eachItem in listChannel.SelectedItems)
                listChannel.Items.Remove(eachItem);
        }

        private void textChannel_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                if (textChannel.Text.Length > 0)
                {
                    ListViewItem lvi = new ListViewItem(textChannel.Text);
                    lvi.Checked = true;
                    listChannel.Items.Add(lvi);
                    textChannel.Text = "";
                    textChannel.Focus();
                }
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem eachItem in listChannel.SelectedItems)
            {
                textChannel.Text = eachItem.Text;
                listChannel.Items.Remove(eachItem);
            }
        }

        private void buttonRemoveServer_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to Remove this Server from the Server Tree?","Remove Server", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                if (SaveServer != null)
                {
                    SaveServer(serverSetting, true);
                    this.Close();
                }
            }
        }

        private void buttonAddIgnore_Click(object sender, EventArgs e)
        {
            if (textIgnore.Text.Length > 0)
            {
                ListViewItem lvi = new ListViewItem(textIgnore.Text);
                lvi.Checked = true;
                listIgnore.Items.Add(lvi);
                textIgnore.Text = "";
                textChannel.Focus();
            }
        }

        private void buttonRemoveIgnore_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem eachItem in listIgnore.SelectedItems)
                listIgnore.Items.Remove(eachItem);

        }

        private void buttonEditIgnore_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem eachItem in listIgnore.SelectedItems)
            {
                textIgnore.Text = eachItem.Text;
                listIgnore.Items.Remove(eachItem);
            }
        }
    }
}
