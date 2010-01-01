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
        private TabWindow channel;
        
        public FormChannelInfo(TabWindow Channel)
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(OnFormClosing);

            this.channel = Channel;
            this.textTopic.Text = StripAllCodes(channel.ChannelTopic);
            this.Text = channel.WindowName + "[" + channel.ChannelModes + "]";
            this.channel.HasChannelInfo = true;
            this.channel.ChannelInfoForm = this;

            //parse out the modes
            foreach (TabWindow.channelMode cm in channel.ChannelModesHash.Values)
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

    }
}
