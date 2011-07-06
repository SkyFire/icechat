/******************************************************************************\
 * IceChat 9 Internet Relay Chat Client
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
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;

namespace IceChatPlugin
{
    public class Plugin : IPluginIceChat
    {
        private string m_Name;
        private string m_Author;
        private string m_Version;
        private AppDomain m_domain;


        private Form m_MainForm;
        private MenuStrip m_MenuStrip;
        private Panel m_BottomPanel;
        private string currentFolder;

        private ToolStripMenuItem m_EnableMonitor;

        private string m_ServerTreeCurrentTab;
        private IceChat.IRCConnection m_ServerTreeCurrentConnection;

        //all the events get declared here
        public event OutGoingCommandHandler OnCommand;

        private struct cMonitor
        {
            public IceChat.IRCConnection connection;
            public string channel;
            public cMonitor(IceChat.IRCConnection connection, string channel)
            {
                this.connection = connection;
                this.channel = channel;
            }
        }

        List<cMonitor> monitoredChannels = new List<cMonitor>();

        private ListView listMonitor;
        private ColumnHeader columnTime;
        private ColumnHeader columnChannel;
        private ColumnHeader columnMessage;

        private delegate void UpdateMonitorDelegate(string Channel, string Message);

        private const char colorChar = (char)3;
        private const char underlineChar = (char)31;
        private const char boldChar = (char)2;
        private const char plainChar = (char)15;
        private const char reverseChar = (char)22;
        private const char italicChar = (char)29;

        Panel panel;

        public Plugin()
        {
            //set your default values here
            m_Name = "Channel Monitor Plugin";
            m_Author = "Snerf";
            m_Version = "1.0";
        }

        public void Dispose()
        {
            //remove the listview/panel
            m_BottomPanel.Controls.Remove(panel);
        }

        public void Initialize()
        {

            panel = new Panel();
            panel.Dock = DockStyle.Fill;

            listMonitor = new ListView();
            columnTime = new ColumnHeader();
            columnChannel = new ColumnHeader();
            columnMessage = new ColumnHeader();


            columnTime.Width = 175;
            columnTime.Text = "Time";

            columnChannel.Width = 150;
            columnChannel.Text = "Channel/Nick";

            columnMessage.Width = 1000;
            columnMessage.Text = "Message";


            listMonitor.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnTime,
            columnChannel,
            columnMessage});

            listMonitor.View = System.Windows.Forms.View.Details;
            listMonitor.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            listMonitor.Dock = DockStyle.Fill;
            panel.Controls.Add(listMonitor);
            m_BottomPanel.Controls.Add(panel);

            m_EnableMonitor = new ToolStripMenuItem();
            m_EnableMonitor.Text = "Toggle Monitor";
            m_EnableMonitor.Checked = true;
            m_EnableMonitor.Click += new EventHandler(OnEnableMonitor_Click);

        }

        //declare the standard properties
        public string Name
        {
            get { return m_Name; }
        }

        public string Author
        {
            get { return m_Author; }
        }

        public string Version
        {
            get { return m_Version; }
        }

        public Form MainForm
        {
            get { return m_MainForm; }
            set { m_MainForm = value; }
        }

        public AppDomain domain
        {
            get { return m_domain; }
            set { m_domain = value; }
        }

        public string CurrentFolder
        {
            get { return currentFolder; }
            set { currentFolder = value; }
        }

        public MenuStrip MainMenuStrip
        {
            get { return m_MenuStrip; }
            set { m_MenuStrip = value; }
        }

        public Panel BottomPanel
        {
            get { return m_BottomPanel; }
            set { m_BottomPanel = value; }
        }

        public string ServerTreeCurrentTab
        {
            get { return m_ServerTreeCurrentTab; }
            set { m_ServerTreeCurrentTab = value; }
        }

        public IceChat.IRCConnection ServerTreeCurrentConnection
        {
            get { return m_ServerTreeCurrentConnection; }
            set { m_ServerTreeCurrentConnection = value; }
        }

        //declare the standard methods
        public void ShowInfo()
        {
            MessageBox.Show(m_Name + " Loaded", m_Name + " " + m_Author);
        }
        
        public void LoadSettingsForm(TabControl SettingsTab)
        {
            //when the Settings Form gets loaded, ability to add tabs

        }
        
        public void LoadColorsForm(TabControl OptionsTab)
        {
            //when the Options Form gets loaded, ability to add tabs

        }

        public ToolStripItem[] AddChannelPopups()
        {
            return (new System.Windows.Forms.ToolStripItem[] { m_EnableMonitor });
            //return null;
        }

        public ToolStripItem[] AddQueryPopups()
        {
            return null;
        }
        
        public ToolStripItem[] AddServerPopups()
        {            
            return null;
        }


        public void MainProgramLoaded()
        {

        }

        private void OnEnableMonitor_Click(object sender, EventArgs e)
        {
            //get the current selected item for the popup menu
            cMonitor newChan = new cMonitor(ServerTreeCurrentConnection, ServerTreeCurrentTab);
            if (((ToolStripMenuItem)sender).CheckState == CheckState.Checked)
            {
                //remove the channel from being monitored
                if (monitoredChannels.IndexOf(newChan) > -1)
                {
                    monitoredChannels.Remove(newChan);
                    AddMonitorMessage(newChan.channel, "Stopped Monitoring channel:" + monitoredChannels.Count);
                }
                ((ToolStripMenuItem)sender).CheckState = CheckState.Unchecked;
            }
            else
            {
                //add the channel for monitoring
                if (monitoredChannels.IndexOf(newChan) == -1)
                {
                    monitoredChannels.Add(newChan);
                    AddMonitorMessage(newChan.channel, "Started Monitoring channel:" + monitoredChannels.Count);
                }
                ((ToolStripMenuItem)sender).CheckState = CheckState.Checked;
            }
        
        }

        public void SaveColorsForm()
        {

        }

        public void SaveSettingsForm()
        {
        
        }

        public void LoadEditorForm(TabControl ScriptsTab)
        {

        }

        public void SaveEditorForm()
        {

        }

        private void AddMonitorMessage(string Channel, string Message)
        {
            if (m_BottomPanel.InvokeRequired)
            {
                UpdateMonitorDelegate umd = new UpdateMonitorDelegate(AddMonitorMessage);
                m_BottomPanel.Invoke(umd, new object[] { Channel, Message });
            }
            else
            {
                DateTime now = DateTime.Now;
                ListViewItem lvi = new ListViewItem(now.ToString());
                
                lvi.SubItems.Add(Channel);
                lvi.SubItems.Add(Message);

                listMonitor.Items.Add(lvi);

                //scroll the listview to the bottom
                listMonitor.EnsureVisible(listMonitor.Items.Count - 1);
                
            }
        }



        private string StripColorCodes(string line)
        {
            //strip out all the color codes, bold , underline and reverse codes
            string ParseBackColor = @"\x03([0-9]{1,2}),([0-9]{1,2})";
            string ParseForeColor = @"\x03[0-9]{1,2}";
            string ParseColorChar = @"\x03";
            string ParseBoldChar = @"\x02";
            string ParseUnderlineChar = @"\x1F";    //code 31
            string ParseReverseChar = @"\x16";      //code 22
            string ParseItalicChar = @"\x1D";      //code 29

            line = line.Replace("&#x3;", colorChar.ToString());

            StringBuilder sLine = new StringBuilder();
            sLine.Append(line);

            Regex ParseIRCCodes = new Regex(ParseBackColor + "|" + ParseForeColor + "|" + ParseColorChar + "|" + ParseBoldChar + "|" + ParseUnderlineChar + "|" + ParseReverseChar + "|" + ParseItalicChar);

            Match m = ParseIRCCodes.Match(sLine.ToString());

            while (m.Success)
            {
                sLine.Remove(m.Index, m.Length);
                m = ParseIRCCodes.Match(sLine.ToString(), m.Index);
            }

            return sLine.ToString();
        }

        //declare all the necessary events

        public PluginArgs ChannelMessage(PluginArgs args)
        {
            //check if monitoring is enabled for this channel
            cMonitor newChan = new cMonitor(args.Connection, args.Channel);
            if (monitoredChannels.IndexOf(newChan) > -1)
                AddMonitorMessage(args.Channel, StripColorCodes(args.Message));
            return args;
        }

        public PluginArgs ChannelAction(PluginArgs args)
        {
            cMonitor newChan = new cMonitor(args.Connection, args.Channel);
            if (monitoredChannels.IndexOf(newChan) > -1)
                AddMonitorMessage(args.Channel, StripColorCodes(args.Message));
            return args;
        }

        public PluginArgs QueryMessage(PluginArgs args)
        {
            return args;
        }

        public PluginArgs QueryAction(PluginArgs args)
        {
            return args;
        }
        
        public PluginArgs ChannelJoin(PluginArgs args)
        {
            if (args.Nick == args.Connection.ServerSetting.NickName)
            {                
                //add the channel to the list
                cMonitor newChan = new cMonitor(args.Connection, args.Channel);
                monitoredChannels.Add(newChan);
                
                AddMonitorMessage(args.Channel, "Started Monitoring channel:" + monitoredChannels.Count);
            }
            return args;
        }
        
        public PluginArgs ChannelPart(PluginArgs args)
        {
            if (args.Nick == args.Connection.ServerSetting.NickName)
            {
                //remove the channel from the list
                cMonitor newChan = new cMonitor(args.Connection, args.Channel);
                if (monitoredChannels.IndexOf(newChan) > -1)
                {
                    monitoredChannels.Remove(newChan);
                    AddMonitorMessage(args.Channel, "Stopped Monitoring channel:" + monitoredChannels.Count);
                }

            }
            return args;
        }

        public PluginArgs ServerQuit(PluginArgs args)
        {
            return args;
        }
        //args.Connection   -- current connection
        //args.Command        -- command data 
        public PluginArgs InputText(PluginArgs args)
        {
            return args;
        }

        public PluginArgs ChannelKick(PluginArgs args)
        {
            return args;
        }

        public PluginArgs ServerNotice(PluginArgs args)
        {
            return args;
        }

        public PluginArgs UserNotice(PluginArgs args)
        {
            return args;
        }

        public PluginArgs CtcpMessage(PluginArgs args)
        {
            //args.Extra        -- ctcp message 
            AddMonitorMessage(args.Nick, "CTCP : " + args.Extra);

            return args;
        }

        public PluginArgs CtcpReply(PluginArgs args)
        {
            //args.Extra        -- ctcp message 
            return args;
        }

        public PluginArgs ServerMessage(PluginArgs args)
        {
            
            return args;
        }

        public void NickChange(PluginArgs args)
        {

        }

        public void ServerRaw(PluginArgs args)
        {

        }

        public void ServerError(PluginArgs args)
        {

        }

        public void ServerConnect(PluginArgs args)
        {

        }

        public void ServerDisconnect(PluginArgs args)
        {

        }

        public void WhoisUser(PluginArgs args)
        {

        }

        public PluginArgs ChannelNotice(PluginArgs args)
        {

            return args;
        }

        public void ChannelTopic(PluginArgs args)
        {

        }

        public void ChannelInvite(PluginArgs args)
        {

        }

        public void ChannelMode(PluginArgs args)
        {
            //args.Extra --  full channel mode

        }

        public void BuddyList(PluginArgs args)
        {
            //args.Extra -- "online" or "offline"

        }

    }
}
