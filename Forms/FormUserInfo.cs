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
