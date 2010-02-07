using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IceChat
{
    public partial class FormChannelInfo : Form
    {
        private IceTabPage channel;
        
        public FormChannelInfo(IceTabPage Channel)
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(OnFormClosing);

            this.channel = Channel;
            this.textTopic.Text = StripAllCodes(channel.ChannelTopic);
            this.Text = channel.TabCaption + "[" + channel.ChannelModes + "]";
            this.channel.HasChannelInfo = true;
            this.channel.ChannelInfoForm = this;

            buttonRemoveBan.Enabled = false;

            User u = channel.GetNick(channel.Connection.ServerSetting.NickName);
            for (int y = 0; y < u.Level.Length; y++)
            {
                if (u.Level[y])
                {
                    if (channel.Connection.ServerSetting.StatusModes[0][y] == 'q')
                        buttonRemoveBan.Enabled = true;
                    else if (channel.Connection.ServerSetting.StatusModes[0][y] == 'a')
                        buttonRemoveBan.Enabled = true;
                    else if (channel.Connection.ServerSetting.StatusModes[0][y] == 'o')
                        buttonRemoveBan.Enabled = true;
                }
            }

            //parse out the modes
            foreach (IceTabPage.channelMode cm in channel.ChannelModesHash.Values)
            {
                switch (cm.mode)
                {
                    case 'n':
                        checkModen.Checked = true;
                        break;
                    case 't':
                        checkModet.Checked = true;
                        break;
                    case 's':
                        checkModes.Checked = true;
                        break;
                    case 'i':
                        checkModei.Checked = true;
                        break;
                    case 'm':
                        checkModem.Checked = true;
                        break;
                    case 'l':
                        checkModel.Checked = true;
                        textMaxUsers.Text = cm.param;
                        break;
                }
            }
        }
        internal void AddChannelBan(string host, string bannedBy)
        {
            ListViewItem lvi = new ListViewItem(host);
            lvi.SubItems.Add(bannedBy);
            listViewBans.Items.Add(lvi);
        }

        internal void AddChannelException(string host, string bannedBy)
        {
            ListViewItem lvi = new ListViewItem(host);
            lvi.SubItems.Add(bannedBy);
            listViewExceptions.Items.Add(lvi);
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            this.channel.HasChannelInfo = false;
            this.channel.ChannelInfoForm = null;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string StripAllCodes(string line)
        {
            if (line == null)
                return "";
            if (line.Length > 0)
            {
                line = line.Replace("&#x3;", ((char)3).ToString());

                string ParseBackColor = @"\x03([0-9]{1,2}),([0-9]{1,2})";
                string ParseForeColor = @"\x03[0-9]{1,2}";
                string ParseColorChar = @"\x03";
                
                System.Text.RegularExpressions.Regex parseStuff = new System.Text.RegularExpressions.Regex(ParseBackColor + "|" + ParseForeColor + "|" + ParseColorChar);
                return parseStuff.Replace(line, "");
            }
            else
                return "";
        }

        private void buttonRemoveBan_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem eachItem in listViewBans.SelectedItems)
            {
                FormMain.Instance.ParseOutGoingCommand(channel.Connection, "/mode " + channel.TabCaption + " -b " + eachItem.Text);
                listViewBans.Items.Remove(eachItem);
            }

        }

    }
}
