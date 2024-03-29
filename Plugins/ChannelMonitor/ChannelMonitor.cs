﻿/******************************************************************************\
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

        public override string Name { get { return m_Name; } }
        public override string Version { get { return m_Version; } }
        public override string Author { get { return m_Author; } }

        //all the events get declared here, do not change
        //public override event OutGoingCommandHandler OnCommand;

        private ToolStripMenuItem m_EnableMonitor;

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

        public override void Dispose()
        {
            //remove the listview/panel
            BottomPanel.Controls.Remove(panel);
        }

        public override void Initialize()
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
            
            BottomPanel.Controls.Add(panel);

            m_EnableMonitor = new ToolStripMenuItem();
            m_EnableMonitor.Text = "Toggle Monitor";
            m_EnableMonitor.Checked = true;
            m_EnableMonitor.Click += new EventHandler(OnEnableMonitor_Click);

        }

        //declare the standard properties

        public override ToolStripItem[] AddChannelPopups()
        {
            return (new System.Windows.Forms.ToolStripItem[] { m_EnableMonitor });
            //return null;
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

       
        private void AddMonitorMessage(string Channel, string Message)
        {
            if (BottomPanel.InvokeRequired)
            {
                UpdateMonitorDelegate umd = new UpdateMonitorDelegate(AddMonitorMessage);
                BottomPanel.Invoke(umd, new object[] { Channel, Message });
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

        public override PluginArgs ChannelMessage(PluginArgs args)
        {
            //check if monitoring is enabled for this channel
            cMonitor newChan = new cMonitor(args.Connection, args.Channel);
            if (monitoredChannels.IndexOf(newChan) > -1)
                AddMonitorMessage(args.Channel, StripColorCodes(args.Message));
            return args;
        }

        public override PluginArgs ChannelAction(PluginArgs args)
        {
            cMonitor newChan = new cMonitor(args.Connection, args.Channel);
            if (monitoredChannels.IndexOf(newChan) > -1)
                AddMonitorMessage(args.Channel, StripColorCodes(args.Message));
            return args;
        }
        
        public override PluginArgs ChannelJoin(PluginArgs args)
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
        
        public override PluginArgs ChannelPart(PluginArgs args)
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


        public override PluginArgs CtcpMessage(PluginArgs args)
        {
            //args.Extra        -- ctcp message 
            AddMonitorMessage(args.Nick, "CTCP : " + args.Extra);

            return args;
        }


    }
}
