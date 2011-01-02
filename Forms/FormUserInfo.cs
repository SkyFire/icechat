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
    public partial class FormUserInfo : Form
    {
        private string _nick;
        private IRCConnection _connection;

        private delegate void ChangeValueDelegate(string text);

        public FormUserInfo(IRCConnection connection)
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(OnFormClosing);
            
            _connection = connection;
            _connection.UserInfoWindow = this;

            ApplyLanguage();
        }

        private void ApplyLanguage()
        {

        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            _connection.UserInfoWindow = null;
        }
        
        public void NickName(string value)
        {
            if (this.InvokeRequired)
            {
                ChangeValueDelegate c = new ChangeValueDelegate(NickName);
                this.Invoke(c, new object[] { value });
            }
            else
            {
                _nick = value;
                FormMain.Instance.ParseOutGoingCommand(_connection, "/whois " + _nick + " " + _nick);
                this.Text = "User Information: " + _nick;
                this.textNick.Text = value;
            }
        }

        public string Nick
        {
            get { return _nick; }
        }

        public void HostName(string value)
        {
            if (this.InvokeRequired)
            {
                ChangeValueDelegate c = new ChangeValueDelegate(HostName);
                this.Invoke(c, new object[] { value });
            }
            else
                this.textHost.Text = value;
        }
        
        public void FullName(string value)
        {
            if (this.InvokeRequired)
            {
                ChangeValueDelegate c = new ChangeValueDelegate(FullName);
                this.Invoke(c, new object[] { value });
            }
            else
                this.textFullName.Text = value;
        }

        public void IdleTime(string value)
        {
            if (this.InvokeRequired)
            {
                ChangeValueDelegate c = new ChangeValueDelegate(IdleTime);
                this.Invoke(c, new object[] { value });
            }
            else
                this.textIdleTime.Text = value;
        }

        public void Client(string value)
        {
            if (this.InvokeRequired)
            {
                ChangeValueDelegate c = new ChangeValueDelegate(Client);
                this.Invoke(c, new object[] { value });
            }
            else
                this.textClient.Text = value;
        }

        public void LogonTime(string value)
        {
            if (this.InvokeRequired)
            {
                ChangeValueDelegate c = new ChangeValueDelegate(LogonTime);
                this.Invoke(c, new object[] { value });
            }
            else
                this.textLogonTime.Text = value;
        }

        public void Channel(string value)
        {
            if (this.InvokeRequired)
            {
                ChangeValueDelegate c = new ChangeValueDelegate(Channel);
                this.Invoke(c, new object[] { value });
            }
            else
                this.listChannels.Items.Add(value);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
