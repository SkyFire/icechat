/******************************************************************************\
 * IceChat 2009 Internet Relay Chat Client
 *
 * Copyright (C) 2009 Paul Vanderzee <snerf@icechat.net>
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

namespace IceChat2009
{
    public partial class FormServers : Form
    {
        private ServerSetting serverSetting;
        
        private bool newServer;

        public delegate void NewServerDelegate(ServerSetting s);
        public event NewServerDelegate NewServer;

        public delegate void SaveServerDelegate();
        public event SaveServerDelegate SaveServer;

        public FormServers()
        {
            InitializeComponent();
            newServer = true;

            this.Text = "Server Editor: New Server";

        }

        public FormServers(ServerSetting s)
        {
            InitializeComponent();
            serverSetting = s;
            
            newServer = false;
            LoadSettings();

            this.Text = "Server Editor: " + s.ServerName;

        }
        
        /// <summary>
        /// Load the Server Settings into the text boxes
        /// </summary>
        private void LoadSettings()
        {
            this.textNickName.Text = serverSetting.NickName;
            this.textAltNickName.Text = serverSetting.AltNickName;
            this.textServername.Text = serverSetting.ServerName;
            this.textServerPort.Text = serverSetting.ServerPort;
            this.textDisplayName.Text = serverSetting.DisplayName;
            
            this.textIdentName.Text = serverSetting.IdentName;
            this.textFullName.Text = serverSetting.FullName;
            this.textQuitMessage.Text = serverSetting.QuitMessage;
            
            this.checkAutoJoin.Checked = serverSetting.AutoJoinEnable;
            this.checkAutoPerform.Checked = serverSetting.AutoPerformEnable;

            this.checkModeI.Checked = serverSetting.SetModeI;
            this.checkMOTD.Checked = serverSetting.ShowMOTD;
            this.checkPingPong.Checked = serverSetting.ShowPingPong;
            this.checkRejoinChannel.Checked = serverSetting.RejoinChannels;

            if (serverSetting.AutoJoinChannels != null)
            {
                foreach (string chan in serverSetting.AutoJoinChannels)
                {
                    if (!chan.StartsWith(";"))
                    {
                        ListViewItem lvi = new ListViewItem(chan);
                        lvi.Checked = true;
                        listChannel.Items.Add(lvi);
                    }
                    else
                        listChannel.Items.Add(chan.Substring(1));
                }
            }

            if (serverSetting.AutoPerform != null)
            {
                foreach (string command in serverSetting.AutoPerform)
                    textAutoPerform.AppendText(command + Environment.NewLine);
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
            
            serverSetting.ServerName = textServername.Text;
            serverSetting.DisplayName = textDisplayName.Text;            

            if (textServerPort.Text.Length == 0)
                textServerPort.Text = "6667";
            serverSetting.ServerPort = textServerPort.Text;

            if (textIdentName.Text.Length == 0)
                textIdentName.Text = "IceChat09";
            serverSetting.IdentName = textIdentName.Text;

            if (textFullName.Text.Length == 0)
                textFullName.Text = "IceChat 2009 Developer Edition";
            serverSetting.FullName = textFullName.Text;

            if (textQuitMessage.Text.Length == 0)
                textQuitMessage.Text = "IceChat 2009 Developer Edition";

            serverSetting.QuitMessage = textQuitMessage.Text;

            serverSetting.AutoJoinEnable = checkAutoJoin.Checked;
            serverSetting.AutoPerformEnable = checkAutoPerform.Checked;

            serverSetting.AutoJoinChannels = new string[listChannel.Items.Count];
            for (int i = 0; i < listChannel.Items.Count; i++)
            {
                if (listChannel.Items[i].Checked == false)
                    serverSetting.AutoJoinChannels[i] = ";" + listChannel.Items[i].Text;
                else
                    serverSetting.AutoJoinChannels[i] = listChannel.Items[i].Text;
            }

            serverSetting.AutoPerform = textAutoPerform.Text.Trim().Split(new String[] { "\r\n" }, StringSplitOptions.None);
            
            serverSetting.SetModeI = checkModeI.Checked;
            serverSetting.ShowMOTD = checkMOTD.Checked;
            serverSetting.ShowPingPong = checkPingPong.Checked;
            serverSetting.RejoinChannels = checkRejoinChannel.Checked;

            if (newServer == true)
            {
                //add in the server to the server collection
                if (NewServer != null)
                    NewServer(serverSetting);
            }
            else
                if (SaveServer != null)
                    SaveServer();

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
                ListViewItem lvi = new ListViewItem(textChannel.Text);
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

    }
}
