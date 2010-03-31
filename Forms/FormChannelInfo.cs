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

        private bool modeN = false;
        private bool modeT = false;
        private bool modeS = false;
        private bool modeI = false;
        private bool modeM = false;
        private bool modeL = false;
        private bool modeK = false;
        private string topic = "";

        private delegate void ChannelTopicSetByDelegate(string nick, string date);

        public FormChannelInfo(IceTabPage Channel)
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(OnFormClosing);
            this.textTopic.KeyDown += new KeyEventHandler(textTopic_KeyDown);

            this.channel = Channel;
            //this.textTopic.Text = StripAllCodes(channel.ChannelTopic);
            this.textTopic.Text = channel.ChannelTopic.Replace("&#x3;", ((char)3).ToString());
            this.topic = textTopic.Text;

            this.Text = channel.TabCaption + " [" + channel.ChannelModes + "]  {" + channel.Connection.ServerSetting.RealServerName + "}";
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
                        modeN = true;
                        break;
                    case 't':
                        checkModet.Checked = true;
                        modeT = true;
                        break;
                    case 's':
                        checkModes.Checked = true;
                        modeS = true;
                        break;
                    case 'i':
                        checkModei.Checked = true;
                        modeI = true;
                        break;
                    case 'm':
                        checkModem.Checked = true;
                        modeM = true;
                        break;
                    case 'l':
                        checkModel.Checked = true;
                        modeL = true;
                        textMaxUsers.Text = cm.param;
                        break;
                    case 'k':
                        checkModek.Checked = true;
                        modeK = true;
                        textChannelKey.Text = cm.param;
                        break;
                
                }
            }

            ApplyLanguage();
        }

        private void ApplyLanguage()
        {
            
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

        internal void ChannelTopicSetBy(string nick, string date)
        {
            if (this.InvokeRequired)
            {
                ChannelTopicSetByDelegate c = new ChannelTopicSetByDelegate(ChannelTopicSetBy);
                this.Invoke(c, new object[] { nick, date });
            }
            else 
                labelTopicSetBy.Text = "Topic Set By: " + nick + " on " + date;
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

        private void textTopic_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
            {
                if (e.KeyCode == Keys.K)
                {
                    textTopic.SelectedText = ((char)3).ToString();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.B)
                {
                    textTopic.SelectedText = ((char)2).ToString();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.U)
                {
                    textTopic.SelectedText = ((char)31).ToString();
                    e.Handled = true;
                }
            }
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

        private void buttonApply_Click(object sender, EventArgs e)
        {
            //check for new mode settings
            string addModes = "";
            string removeModes = "";
            string addModeParams = "";

            if (checkModen.Checked != modeN)
            {
                if (checkModen.Checked)
                    addModes += "n";
                else
                    removeModes += "n";

                modeN = checkModen.Checked;
            }
            
            if (checkModet.Checked != modeT)
            {
                if (checkModet.Checked)
                    addModes += "t";
                else
                    removeModes += "t";
                modeT = checkModet.Checked;
            }

            if (checkModei.Checked != modeI)
            {
                if (checkModei.Checked)
                    addModes += "i";
                else
                    removeModes += "i";
                modeI = checkModei.Checked;
            }
            
            if (checkModem.Checked != modeM)
            {
                if (checkModem.Checked)
                    addModes += "m";
                else
                    removeModes += "m";
                modeM = checkModem.Checked;
            }
            
            if (checkModes.Checked != modeS)
            {
                if (checkModes.Checked)
                    addModes += "s";
                else
                    removeModes += "s";
                modeS = checkModes.Checked;
            }
            
            if (checkModel.Checked != modeL)
            {
                if (checkModel.Checked)
                {
                    addModes += "l";
                    addModeParams += textMaxUsers.Text;
                }
                else
                    removeModes += "l";
                modeL = checkModel.Checked;
            }
            
            if (checkModek.Checked != modeK)
            {
                if (checkModek.Checked)
                {
                    addModes += "k";
                    addModeParams += textChannelKey.Text;
                }
                else
                    removeModes += "k";
                modeK = checkModek.Checked;
            }

            if (addModes.Length > 0)
                addModes = "+" + addModes;

            if (removeModes.Length > 0)
                removeModes = "-" + removeModes;

            string fullMode = addModes + removeModes;
            if (addModeParams.Length > 0)
                fullMode = fullMode + " " + addModeParams;

            if (fullMode.Length > 0)
                FormMain.Instance.ParseOutGoingCommand(channel.Connection, "/mode " + channel.TabCaption + " " + fullMode);

            if (this.topic != textTopic.Text)
            {
                FormMain.Instance.ParseOutGoingCommand(channel.Connection, "/topic " + channel.TabCaption + " " + textTopic.Text);
                this.topic = textTopic.Text;
            }
        }
    }
}
