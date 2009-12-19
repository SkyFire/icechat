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
using System.Text;

namespace IceChat2009
{
    public partial class IRCConnection
    {        
           
        internal event OutGoingCommandDelegate OutGoingCommand;

        internal event ChannelMessageDelegate ChannelMessage;
        internal event ChannelActionDelegate ChannelAction;
        internal event QueryMessageDelegate QueryMessage;
        internal event QueryActionDelegate QueryAction;

        internal event GenericChannelMessageDelegate GenericChannelMessage;

        internal event ChangeNickDelegate ChangeNick;
        internal event JoinChannelDelegate JoinChannel;
        internal event PartChannelDelegate PartChannel;
        internal event QuitServerDelegate QuitServer;
        internal event ChannelNoticeDelegate ChannelNotice;

        internal event KickNickDelegate KickNick;
        internal event KickMyselfDelegate KickMyself;

        internal event ChannelTopicDelegate ChannelTopic;

        internal event ChannelModeChangeDelegate ChannelMode;
        internal event UserModeChangeDelegate UserMode;

        internal event ChannelInviteDelegate ChannelInvite;
        internal event UserHostReplyDelegate UserHostReply;
        internal event JoinChannelMyselfDelegate JoinChannelMyself;
        internal event PartChannelMyselfDelegate PartChannelMyself;
        
        internal event ServerMessageDelegate ServerMessage;
        internal event ServerMOTDDelegate ServerMOTD;
        internal event ServerErrorDelegate ServerError;
        internal event WhoisDataDelegate WhoisData;
        internal event CtcpMessageDelegate CtcpMessage;
        internal event UserNoticeDelegate UserNotice;
    
        internal event ServerNoticeDelegate ServerNotice;

        internal event ChannelListDelegate ChannelList;

        internal event RawServerIncomingDataDelegate RawServerIncomingData;
        internal event RawServerOutgoingDataDelegate RawServerOutgoingData;

        internal event IALUserDataDelegate IALUserData;

        private bool triedAltNickName = false;
        private bool initialLogon = false;

        private void ParseData(string data)
        {
            try
            {
                string[] ircData = data.Split(' ');
                string channel;
                string nick;
                string host;
                string msg;
                string tempValue;

                if (RawServerIncomingData != null)
                    RawServerIncomingData(this, data);

                if (data.Length > 4)
                {
                    if (data.Substring(0, 4) == "PING")
                    {
                        SendData("PONG " + ircData[1]);

                        if (ServerMessage != null && serverSetting.ShowPingPong)
                            ServerMessage(this, "Ping? Pong!");
                        return;
                    }
                }

                //parse then 2nd word
                string IrcNumeric;

                if (data.IndexOf(' ') > -1)
                {

                    IrcNumeric = ircData[1];

                    nick = NickFromFullHost(RemoveColon(ircData[0]));
                    host = HostFromFullHost(RemoveColon(ircData[0]));
                    
                    // A great list of IRC Numerics http://www.mirc.net/raws/
                    
                    switch (IrcNumeric)
                    {
                        case "001":
                            //get the real server name
                            serverSetting.RealServerName = RemoveColon(ircData[0]);
                            
                            //update the status bar
                            if (FormMain.Instance.CurrentWindowType == TabWindow.WindowType.Console)
                            {
                                if (FormMain.Instance.InputPanel.CurrentConnection == this)
                                {
                                    FormMain.Instance.StatusText(serverSetting.NickName + " connected to " + serverSetting.RealServerName);
                                }
                            }

                            if (serverSetting.NickName != ircData[2])
                            {
                                if (ChangeNick != null)
                                    ChangeNick(this, serverSetting.NickName, ircData[2], HostFromFullHost(ircData[0]));
                                serverSetting.NickName = ircData[2];
                            }

                            if (ServerMessage != null)
                                ServerMessage(this, JoinString(ircData, 3, true));

                            initialLogon = true;
                            break;
                        case "002":
                        case "003":
                            if (ServerMessage != null)
                                ServerMessage(this, JoinString(ircData, 3, true));
                            break;

                        case "004":
                        case "005":
                            if (ServerMessage != null)
                                ServerMessage(this, JoinString(ircData, 3, false));

                            //parse out all the 005 data
                            for (int i = 0; i < ircData.Length; i++)
                            {
                                //parse out all the status modes for user prefixes
                                //parse out all the status modes for user prefixes
                                if (ircData[i].Length >= 7)
                                {
                                    if (ircData[i].StartsWith("PREFIX="))
                                    {
                                        string[] modes = ircData[i].Substring(8).Split(')');
                                        serverSetting.StatusModes = new char[2][];

                                        serverSetting.StatusModes[0] = modes[0].ToCharArray();
                                        serverSetting.StatusModes[1] = modes[1].ToCharArray();
                                    }
                                }

                                //add all the channel modes that have parameters into a variable
                                if (ircData[i].Length >= 10)
                                {
                                    if (ircData[i].Substring(0, 10) == "CHANMODES=")
                                    {
                                        //CHANMODES=b,k,l,imnpstrDducCNMT
                                        string[] modes = ircData[i].Substring(ircData[i].IndexOf("=") + 1).Split(',');
                                        
                                        for (int j = 0; j < modes.Length; j++)
                                        {
                                            if (j != (modes.Length - 1))
                                                serverSetting.ChannelModeParams += modes[j];
                                            else
                                                serverSetting.ChannelModeNoParams = modes[j];
                                        }

                                    }
                                }

                                //parse STATUSMSG symbols
                                if (ircData[i].Length > 10)
                                {
                                    if (ircData[i].StartsWith("STATUSMSG="))
                                        serverSetting.StatusMSG = ircData[i].Substring(10).ToCharArray();
                                }

                                //extract the network name                            
                                if (ircData[i].Length > 8)
                                {
                                    if (ircData[i].Substring(0, 8) == "NETWORK=")
                                        serverSetting.NetworkName = ircData[i].Substring(8);
                                }

                                //parse CHANTYPES symbols
                                if (ircData[i].Length > 10)
                                {
                                    if (ircData[i].StartsWith("CHANTYPES="))
                                        serverSetting.ChannelTypes = ircData[i].Substring(10).ToCharArray();
                                }

                                if (ircData[i].Length > 8)
                                {
                                    if (ircData[i].Substring(0, 8) == "CHARSET=")
                                    {
                                        //do something about character sets
                                    }
                                }

                                //tell server this client supports NAMESX
                                if (ircData[i] == "NAMESX")
                                {
                                    SendData("PROTOCTL NAMESX");
                                }
                            }


                            break;
                        case "219":
                            if (ServerMessage != null)
                                ServerMessage(this, JoinString(ircData, 4, true));
                            break;
                        case "221":
                            break;
                        
                        case "250":
                        case "251":
                        case "255":
                            if (ServerMessage != null)
                                ServerMessage(this, JoinString(ircData, 3, true));
                            break;

                        case "252":
                        case "253":
                        case "254":
                            break;

                        case "265":
                        case "266":
                            msg = JoinString(ircData, 3, true);
                            if (ServerMessage != null)
                                ServerMessage(this, msg);
                            break;
                        case "302": //parse out a userhost
                            msg = JoinString(ircData, 3, true);
                            if (msg.Length == 0) return;
                            if (msg.IndexOf(' ') == -1)
                            {
                                //single host
                                host = msg.Substring(msg.IndexOf('@')+1);
                                if (msg.IndexOf('*') > -1)
                                    nick = msg.Substring(0, msg.IndexOf('*'));
                                else
                                    nick = msg.Substring(0, msg.IndexOf('='));

                                System.Net.IPAddress[] addresslist = System.Net.Dns.GetHostAddresses(host);
                                foreach (System.Net.IPAddress address in addresslist)
                                {
                                    FormMain.Instance.ParseOutGoingCommand(this, "/echo " + nick + " resolved to " + address.ToString());
                                    if (UserHostReply != null)
                                        UserHostReply(this, msg);
                                }
                            }
                            else
                            {
                                //multiple hosts
                                string[] hosts = msg.Split(' ');
                                foreach (string h in hosts)
                                {
                                    if (UserHostReply != null)
                                        UserHostReply(this, h);
                                }
                            }
                            break;
                        case "311":     //whois information
                            msg = "->> " + ircData[3] + " is " + ircData[4] + "@" + ircData[5] + " (" + JoinString(ircData, 7, true) + ")";
                            if (WhoisData != null)
                                WhoisData(this, ircData[3], msg);
                            if (IALUserData != null)
                                IALUserData(this, nick, host, "");
                            break;
                        case "312":     //whois information
                            msg = "->> " + ircData[3] + " using " + ircData[4] + " (" + JoinString(ircData, 5, true) + ")";
                            if (WhoisData != null)
                                WhoisData(this, ircData[3], msg);
                            break;
                        case "317":     //whois information
                            DateTime date1 = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                            date1 = date1.AddSeconds(Convert.ToDouble(ircData[5]));
                            msg = "->> " + ircData[3] + " " + GetDuration(Convert.ToInt32(ircData[4])) + " " + JoinString(ircData, 6, true) + " " + date1.ToShortTimeString() + " " + date1.ToShortDateString();
                            if (WhoisData != null)
                                WhoisData(this, ircData[3], msg);
                            break;
                        case "318":     //whois information
                            msg = "->> " + ircData[3] + " " + JoinString(ircData, 4, false);
                            if (WhoisData != null)
                                WhoisData(this, ircData[3], msg);
                            break;
                        case "319":     //whois information 
                            msg = "->> " + ircData[3] + " is on: " + JoinString(ircData, 4, true);
                            if (WhoisData != null)
                                WhoisData(this, ircData[3], msg);
                            break;
                        case "320":     //whois information
                            msg = "->> " + ircData[3] + " " + JoinString(ircData, 4, true);
                            if (WhoisData != null)
                                WhoisData(this, ircData[3], msg);
                            break;
                        case "321":     //start channel list
                            break;

                        case "322":     //channel list
                            //3 4 rc(5+)
                            if (ChannelList != null)
                                ChannelList(this, ircData[3], ircData[4], RemoveColon(JoinString(ircData, 5, true)));
                            break;

                        case "323": //end channel list
                            break;

                        case "324":     //channel modes
                            //mediatraffic2.fi.quakenet.org 324 Snerf #icechat +tn
                            channel = ircData[3];
                            msg = "Channel modes for " + channel + " are :" + JoinString(ircData, 4, false);
                            if (ChannelMode != null)
                                ChannelMode(this, channel, "", channel, JoinString(ircData, 4, false));

                            if (GenericChannelMessage != null)
                                GenericChannelMessage(this, channel, msg);
                            break;
                        case "328":     //channel url
                            channel = ircData[3];
                            msg = "Channel URL is " + JoinString(ircData, 4, true);
                            if (GenericChannelMessage != null)
                                GenericChannelMessage(this, channel, msg);
                            break;
                        case "329":     //channel creation time
                            channel = ircData[3];
                            DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                            date = date.AddSeconds(Convert.ToDouble(ircData[4]));
                            msg = "Channel Created on: " + date.ToShortTimeString() + " " + date.ToShortDateString();
                            if (GenericChannelMessage != null)
                                GenericChannelMessage(this, channel, msg);
                            break;
                        case "330":     //whois information
                            msg = "->> " + ircData[3] + " " + JoinString(ircData, 5, true) + " " + ircData[4];
                            if (WhoisData != null)
                                WhoisData(this, ircData[3], msg);
                            break;
                        case "332":     //channel topic
                            //mediatraffic2.fi.quakenet.org 332 Snerf #icechat :Official IceChat Support Channel
                            channel = ircData[3];
                            if (ChannelTopic != null)
                                ChannelTopic(this, channel, "", "", JoinString(ircData, 4, true));
                            break;
                        case "333":     //channel time
                            //mediatraffic2.fi.quakenet.org 333 Snerf #icechat IceBot 1234725840    
                            channel = ircData[3];
                            nick = ircData[4];
                            DateTime date2 = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                            date2 = date2.AddSeconds(Convert.ToDouble(ircData[5]));
                            msg = "Channel Topic Set by: " + nick + " on " + date2.ToShortTimeString() + " " + date2.ToShortDateString();
                            if (GenericChannelMessage != null)
                                GenericChannelMessage(this, channel, msg);
                            break;

                        case "353": //channel user list
                            channel = ircData[4];
                            TabWindow t = FormMain.Instance.GetWindow(this, channel, TabWindow.WindowType.Channel);
                            if (t != null)
                            {
                                if (t.IsFullyJoined)
                                {
                                    //just show the message to the console
                                    if (ServerMessage != null)
                                        ServerMessage(this, JoinString(ircData, 4, true));
                                    return;
                                }
                            }
                            
                            string[] nicks = JoinString(ircData, 5, true).Split(' ');
                            foreach (string nickName in nicks)
                            {
                                if (nickName.Length > 0)
                                    JoinChannel(this, channel, nickName, "", false);
                            }
                            break;
                        case "365":  //End of Links
                            msg = JoinString(ircData, 4, true);
                            if (ServerMessage != null)
                                ServerMessage(this, msg);                            
                            break;
                        case "366":     //end of names
                            channel = ircData[3];
                            //channel is fully joined                            
                            TabWindow c = FormMain.Instance.GetWindow(this, channel, TabWindow.WindowType.Channel);
                            if (c != null)
                            {
                                c.IsFullyJoined = true;
                                if (FormMain.Instance.CurrentWindowType == TabWindow.WindowType.Channel)
                                    if ((FormMain.Instance.NickList.CurrentWindow.WindowName == c.WindowName) && (FormMain.Instance.NickList.CurrentWindow.Connection == c.Connection))
                                        FormMain.Instance.NickList.RefreshList(c);
                            }
                            break;
                            //1::xs4all.nl.quakenet.org 367 Snerf2009 #icechat9 *!*@*.hotmail.com Snerf 1260602965
                            //1::xs4all.nl.quakenet.org 368 Snerf2009 #icechat9 :End of Channel Ban List
                        case "367": //channel ban list
                        case "368": //end of channel ban list
                            break;
                        case "372": //motd
                        case "375":
                            msg = JoinString(ircData, 3, true);                            
                            if (ServerMOTD != null && (serverSetting.ShowMOTD || serverSetting.ForceMOTD))
                                ServerMOTD(this, msg);
                            break;

                        case "378":     //whois information
                            msg = "->> " + ircData[3] + " " + RemoveColon(ircData[4]) + " " + JoinString(ircData, 5, true);
                            if (WhoisData != null)
                                WhoisData(this, ircData[3], msg);
                            break;

                        case "379":     //whois information
                            msg = "->> " + ircData[3] + " " + RemoveColon(ircData[4]) + " " + JoinString(ircData, 5, true);
                            if (WhoisData != null)
                                WhoisData(this, ircData[3], msg);
                            break;

                        case "376": //end of motd
                        case "422": //missing motd
                            if (serverSetting.ForceMOTD)
                            {
                                serverSetting.ForceMOTD = false;
                                return;
                            }
                            
                            if (ServerMessage != null)
                                ServerMessage(this, "You have successfully connected to " + serverSetting.RealServerName);

                            if (serverSetting.SetModeI)
                                SendData("MODE " + serverSetting.NickName + " +i");

                            //run autoperform
                            if (serverSetting.AutoPerformEnable)
                            {
                                if (ServerMessage != null)
                                    ServerMessage(this, "Running AutoPerform command(s)..");

                                string autoCommand;
                                foreach (string command in serverSetting.AutoPerform)
                                {
                                    autoCommand = command.Replace("\r", String.Empty);
                                    if (!autoCommand.StartsWith(";"))
                                        OutGoingCommand(this, autoCommand);
                                }
                            }

                            if (serverSetting.RejoinChannels)
                            {
                                //rejoin any channels that are open
                                foreach (TabWindow tw in FormMain.Instance.TabMain.WindowTabs)
                                {
                                    if (tw.WindowStyle == TabWindow.WindowType.Channel)
                                    {
                                        if (tw.Connection == this)
                                            SendData("JOIN " + tw.WindowName);
                                    }
                                }
                            }

                            //run autojoins
                            if (serverSetting.AutoJoinEnable)
                            {
                                if (ServerMessage != null)
                                    ServerMessage(this, "Auto-joining Channels");

                                foreach (string chan in serverSetting.AutoJoinChannels)
                                {
                                    if (!chan.StartsWith(";"))
                                        SendData("JOIN " + chan);
                                }
                            }

                            fullyConnected = true;
                            
                            //read the command queue
                            if (commandQueue.Count > 0)
                            {
                                foreach (string command in commandQueue)
                                {
                                    SendData(command);
                                }
                            }
                            commandQueue.Clear();
                            break;

                        case "396":     //mode X message
                            msg = ircData[3] + " " + JoinString(ircData, 4, true);
                            if (ServerMessage != null)
                                ServerMessage(this, msg);
                            break;

                        case "439":
                        case "931":
                            msg = JoinString(ircData, 3, true);
                            if (ServerMessage != null)
                                ServerMessage(this, msg);
                            break;

                        case "PRIVMSG":
                            channel = ircData[2];
                            msg = JoinString(ircData, 3, true);
                            
                            if (channel == serverSetting.NickName)
                            {
                                //this is a private message to you
                                //check if it was an notice/action
                                if (msg[0] == (char)1)
                                {
                                    //drop the 1st and last CTCP Character
                                    msg = msg.Substring(1, msg.Length - 2);
                                    //check for action
                                    switch (msg.Split(' ')[0].ToUpper())
                                    {
                                        case "ACTION":
                                            msg = msg.Substring(6);
                                            if (QueryAction != null)
                                                QueryAction(this, nick, host, msg);
                                            if (IALUserData != null)
                                                IALUserData(this, nick, host, "");
                                            break;
                                        case "VERSION":
                                        case "TIME":
                                        case "PING":
                                        case "USERINFO":
                                        case "CLIENTINFO":
                                        case "SOURCE":
                                        case "FINGER":
                                            if (CtcpMessage != null)
                                                CtcpMessage(this, nick, msg.Split(' ')[0].ToUpper());
                                            break;
                                        default:
                                            if (UserNotice != null)
                                                UserNotice(this, nick, msg);
                                            break;
                                    }
                                }
                                else
                                {
                                    if (QueryMessage != null)
                                        QueryMessage(this, nick, host, msg);
                                    if (IALUserData != null)
                                        IALUserData(this, nick, host, "");
                                }
                            }
                            else
                            {
                                if (msg[0] == (char)1)
                                {
                                    msg = msg.Substring(1, msg.Length - 2);
                                    switch (msg.Split(' ')[0].ToUpper())
                                    {
                                        case "ACTION":
                                            msg = msg.Substring(7);
                                            if (ChannelAction != null)
                                                ChannelAction(this, channel, nick, host, msg);
                                            if (IALUserData != null)
                                                IALUserData(this, nick, host, channel);
                                            break;
                                        case "VERSION":
                                        case "TIME":
                                        case "PING":
                                        case "USERINFO":
                                        case "CLIENTINFO":
                                        case "SOURCE":
                                        case "FINGER":
                                            if (CtcpMessage != null)
                                                CtcpMessage(this, nick, msg.Split(' ')[0].ToUpper());
                                            break;
                                        default:
                                            if (msg.Substring(7).ToUpper() == "ACTION ")
                                            {
                                                msg = msg.Substring(7);
                                                if (ChannelAction != null)
                                                    ChannelAction(this, channel, nick, host, msg);
                                                if (IALUserData != null)
                                                    IALUserData(this, nick, host, channel);
                                            }
                                            else
                                            //check for DCC SEND, DCC CHAT, DCC ACCEPT, DCC RESUME
                                            {
                                                if (ChannelNotice != null)
                                                    ChannelNotice(this, nick, host,(char)32,channel, msg);
                                                if (IALUserData != null)
                                                    IALUserData(this, nick, host, channel);

                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    if (ChannelMessage != null)
                                        ChannelMessage(this, channel, nick, host, msg);
                                    if (IALUserData != null)
                                        IALUserData(this, nick, host, channel);

                                }
                            }
                            break;
                        case "INVITE":      //channel invite
                            channel = ircData[3];
                            if (ChannelInvite != null)
                                ChannelInvite(this, channel, nick, host);
                            break;

                        case "NOTICE":
                            msg = JoinString(ircData, 3, true);
                            //check if its a user notice or a server notice
                            if (nick == serverSetting.RealServerName)
                            {
                                if (ServerNotice != null)
                                    ServerNotice(this, msg);
                            }
                            else
                            {
                                if (initialLogon && serverSetting.StatusMSG == null)
                                {
                                    serverSetting.StatusMSG = new char[serverSetting.StatusModes[1].Length];
                                    for (int j = 0; j < serverSetting.StatusModes[1].Length; j++)
                                        serverSetting.StatusMSG[j] = serverSetting.StatusModes[1][j];
                                }
                                if (initialLogon && Array.IndexOf(serverSetting.ChannelTypes, ircData[2][0]) != -1)
                                {
                                    if (ChannelNotice != null)
                                        ChannelNotice(this, nick, host, '0', ircData[2], msg);
                                    if (IALUserData != null)
                                        IALUserData(this, nick, host, ircData[2]);
                                }
                                else if (initialLogon && Array.IndexOf(serverSetting.StatusMSG, ircData[2][0]) != -1 && Array.IndexOf(serverSetting.ChannelTypes, ircData[2][1]) != -1)
                                {
                                    if (ChannelNotice != null)
                                        ChannelNotice(this, nick, host, ircData[2][0], ircData[2].Substring(1), msg);
                                    if (IALUserData != null)
                                        IALUserData(this, nick, host, ircData[2]);
                                }
                                else
                                {
                                    if (UserNotice != null)
                                        UserNotice(this, nick, msg);
                                }
                            }
                            break;

                        case "MODE":
                            channel = ircData[2];

                            if (channel == serverSetting.NickName)
                            {
                                //user mode
                                tempValue = JoinString(ircData, 3, true);
                                if (UserMode != null)
                                    UserMode(this, channel, tempValue);
                            }
                            else
                            {
                                //channel mode
                                tempValue = JoinString(ircData, 3, false);

                                if (ChannelMode != null)
                                    ChannelMode(this, nick, HostFromFullHost(ircData[0]), channel, tempValue);
                            }
                            break;

                        case "JOIN":
                            channel = RemoveColon(ircData[2]);
                            //check if it is our own nickname
                            if (nick == serverSetting.NickName)
                            {
                                JoinChannelMyself(this, channel);
                                SendData("MODE " + channel);
                            }
                            else
                            {
                                JoinChannel(this, channel, nick, host, true);
                                if (IALUserData != null)
                                    IALUserData(this, nick, host, channel);
                            }
                            break;

                        case "PART":
                            channel = RemoveColon(ircData[2]);

                            tempValue = JoinString(ircData, 3, true); //part reason
                            //check if it is our own nickname
                            if (nick == serverSetting.NickName)
                            {
                                //part self
                                if (PartChannelMyself != null)
                                    PartChannelMyself(this, channel);
                            }
                            else
                            {
                                tempValue = JoinString(ircData, 3, true);
                                if (PartChannel != null)
                                    PartChannel(this, channel, nick, host, tempValue);

                            }
                            break;

                        case "QUIT":
                            nick = NickFromFullHost(RemoveColon(ircData[0]));
                            host = HostFromFullHost(RemoveColon(ircData[0]));
                            tempValue = JoinString(ircData, 2, true);

                            if (QuitServer != null)
                                QuitServer(this, nick, host, tempValue);

                            break;

                        case "NICK":
                            //old nickname
                            nick = NickFromFullHost(RemoveColon(ircData[0]));
                            host = HostFromFullHost(RemoveColon(ircData[0]));

                            //new nickname
                            tempValue = RemoveColon(ircData[2]);

                            if (serverSetting.NickName == nick)
                            {
                                //if it is your own nickname, update it
                                serverSetting.NickName = tempValue;

                            }

                            if (ChangeNick != null)
                                ChangeNick(this, nick, tempValue, HostFromFullHost(ircData[0]));

                            break;

                        case "KICK":
                            msg = JoinString(ircData, 4, true);  //kick message                        
                            channel = ircData[2];
                            //this is WHO got kicked
                            nick = ircData[3];
                            //check if it is our own nickname who got kicked
                            if (nick == serverSetting.NickName)
                            {
                                //we got kicked
                                if (KickMyself != null)
                                    KickMyself(this, channel, msg, ircData[0]);

                            }
                            else
                            {
                                if (KickNick != null)
                                    KickNick(this, channel, nick, msg, ircData[0]);

                            }
                            break;

                        case "TOPIC":   //channel topic change
                            channel = ircData[2];
                            msg = JoinString(ircData, 3, true);

                            if (ChannelTopic != null)
                                ChannelTopic(this, channel, nick, host, msg);

                            break;

                        case "AUTH":    //NOTICE AUTH
                            if (ServerMessage != null)
                                ServerMessage(this, JoinString(ircData, 2, true));
                            break;

                        //errors
                        case "421": //unknown command
                            if (ServerError != null)
                                ServerError(this, JoinString(ircData, 3, false));
                            break;
                        
                        case "433": //nickname in use
                            nick = NickFromFullHost(RemoveColon(ircData[0]));

                            if (ServerMessage != null)
                                ServerMessage(this, JoinString(ircData, 4, true) + " : " + ircData[3]);
                           
                            if (!triedAltNickName && !initialLogon)
                            {
                                SendData("NICK " + serverSetting.AltNickName);
                                if (ChangeNick != null)
                                    ChangeNick(this, nick, serverSetting.AltNickName, HostFromFullHost(RemoveColon(ircData[0])));
                                serverSetting.NickName = serverSetting.AltNickName;
                                triedAltNickName = true;
                            }

                            break;
                        case "513":
                            if (ServerError != null)
                                ServerError(this, JoinString(ircData, 3, true));                            
                            break;
                        
                        default:
                            if (ServerMessage != null)
                                ServerMessage(this, data);
                            break;
                        //                            
                    }
                }
            }
            catch (Exception e)
            {
                FormMain.Instance.ReportError(e.Message, e.StackTrace, "ParseIRCData::ParseData");
            }
        }
        
        #region Parsing Methods

        private string GetDuration(int seconds)
        {
            TimeSpan t = new TimeSpan(0, 0, seconds);
            
            string s = t.Seconds.ToString();
            if (t.Minutes > 0)
                s = t.Minutes.ToString() + " mins " + s;
            if (t.Hours > 0)
                s = t.Hours.ToString() + " hrs " + s;
            if (t.Days > 0)
                s = t.Days.ToString() + " days " + s;

            return s;
        }

        private string HostFromFullHost(string host)
        {
            if (host.IndexOf("!") > -1)
                return host.Substring(host.IndexOf("!") + 1);
            else
                return host;
        }

        private string NickFromFullHost(string host)
        {
            if (host.StartsWith(":"))
                host = host.Substring(1);
            
            if (host.IndexOf("!") > -1)
                return host.Substring(0, host.IndexOf("!"));
            else
                return host;
        }

        private string RemoveColon(string data)
        {
            if (data.StartsWith(":"))
                return data.Substring(1);
            else
                return data;
        }   

        private string JoinString(string[] strData, int startIndex, bool removeColon)
        {
            if (startIndex > strData.GetUpperBound(0)) return "";

            string tempString = String.Join(" ", strData, startIndex, strData.GetUpperBound(0) + 1 - startIndex );
            if (removeColon)
            {
                tempString = RemoveColon(tempString);
            }
            return tempString;
        }
        
        #endregion

    }
}
