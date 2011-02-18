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
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace IceChat
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
        internal event CtcpReplyDelegate CtcpReply;
        internal event UserNoticeDelegate UserNotice;
    
        internal event ServerNoticeDelegate ServerNotice;

        internal event ChannelListStartDelegate ChannelListStart;
        internal event ChannelListDelegate ChannelList;

        internal event DCCChatDelegate DCCChat;
        internal event DCCFileDelegate DCCFile;
        internal event DCCPassiveDelegate DCCPassive;

        internal event RawServerIncomingDataDelegate RawServerIncomingData;
        internal event RawServerOutgoingDataDelegate RawServerOutgoingData;

        internal event IALUserDataDelegate IALUserData;
        internal event IALUserChangeDelegate IALUserChange;
        internal event IALUserPartDelegate IALUserPart;
        internal event IALUserQuitDelegate IALUserQuit;

        internal event BuddyListRefreshDelegate BuddyListRefresh;

        private bool triedAltNickName = false;
        private bool initialLogon = false;

        internal FormUserInfo UserInfoWindow = null;

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

                IceTabPage t = null;

                if (RawServerIncomingData != null)
                    RawServerIncomingData(this, data);

                if (data.Length > 4)
                {
                    if (data.Substring(0, 4) == "PING")
                    {
                        SendData("PONG " + ircData[1]);

                        pongTimer.Stop();
                        pongTimer.Start();

                        if (serverSetting.ShowPingPong)
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
                            if (FormMain.Instance.CurrentWindowType == IceTabPage.WindowType.Console)
                            {
                                if (FormMain.Instance.InputPanel.CurrentConnection == this)
                                {
                                    FormMain.Instance.StatusText(serverSetting.NickName + " connected to " + serverSetting.RealServerName);
                                }
                            }

                            if (serverSetting.NickName != ircData[2])
                            {
                                ServerMessage(this, "FORCE CHANGE NICK:" + ircData[2] + ":" + serverSetting.NickName + ":" + data);
                                ChangeNick(this, serverSetting.NickName, ircData[2], HostFromFullHost(ircData[0]));
                                serverSetting.NickName = ircData[2];
                            }

                            ServerMessage(this, JoinString(ircData, 3, true));

                            initialLogon = true;
                            break;
                        case "002":
                        case "003":
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
                                //parse MAX MODES set
                                if (ircData[i].Length > 6)
                                {
                                    if (ircData[i].StartsWith("MODES="))
                                        serverSetting.MaxModes = Convert.ToInt32(ircData[i].Substring(6));
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

                                //check max nick length
                                if (ircData[i].Length > 8)
                                {
                                    if (ircData[i].Substring(0, 8) == "NICKLEN=")
                                    {
                                        serverSetting.MaxNickLength = Convert.ToInt32(ircData[i].Substring(8));
                                    }
                                }
                                if (ircData[i].Length > 11)
                                {
                                    if (ircData[i].Substring(0, 11) == "MAXNICKLEN=")
                                    {
                                        serverSetting.MaxNickLength = Convert.ToInt32(ircData[i].Substring(11));
                                    }
                                }

                                //tell server this client supports NAMESX
                                if (ircData[i] == "NAMESX")
                                {
                                    SendData("PROTOCTL NAMESX");
                                }
                            }
                            break;
                        case "020": //IRCnet message
                            ServerMessage(this, JoinString(ircData, 3, true));
                            break;
                        case "042":
                            msg = JoinString(ircData, 4, true) + " " + ircData[3];
                            ServerMessage(this, msg);                            
                            break;
                        case "219": //end of stats
                            ServerMessage(this, JoinString(ircData, 4, true));
                            break;
                        case "221": //:port80b.se.quakenet.org 221 Snerf2 +i
                            ServerMessage(this, RemoveColon(ircData[0]) + " sets mode for " + ircData[2] + " " + ircData[3]);                            
                            break;
                        
                        case "251": //there are x users on x servers
                        case "255": //I have x users and x servers
                            ServerMessage(this, JoinString(ircData, 3, true));
                            break;

                        case "250": //highest connection count
                        case "252": //operators online
                        case "253": //unknown connections
                        case "254": //channels formed
                            msg = "There are " + ircData[3] + " " + JoinString(ircData, 4, true);
                            ServerMessage(this, msg);
                            break;

                        case "265": //current local users / max
                        case "266": //current global users / max
                            msg = JoinString(ircData, 3, true);
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
                                    OutGoingCommand(this, "/echo " + nick + " resolved to " + address.ToString());
                                    UserHostReply(this, msg);
                                }
                            }
                            else
                            {
                                //multiple hosts
                                string[] hosts = msg.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (string h in hosts)
                                    UserHostReply(this, h);
                            }
                            break;
                        case "303": //parse out ISON information (Buddy List)
                            msg = JoinString(ircData, 3, true);

                            //queue up next batch to send
                            buddyListTimer.Start();

                            if (msg.Length == 0) return;
                            
                            string[] buddies = msg.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
                            foreach (BuddyListItem b in serverSetting.BuddyList)
                            {
                                if (b.IsOnSent && !b.IsOnReceived)
                                {
                                    bool isFound = false;
                                    foreach (string buddy in buddies)
                                    {
                                        //this nick is connected
                                        if (b.Nick.ToLower() == buddy.ToLower())
                                        {
                                            b.Connected = true;
                                            b.IsOnReceived = true;
                                            isFound = true;
                                        }
                                    }
                                    if (!isFound)
                                    {
                                        b.Connected = false;
                                        b.IsOnReceived = true;
                                    }
                                }
                            }

                            if (buddiesIsOnSent == serverSetting.BuddyList.Length)
                            {
                                //reset all the isonsent values
                                foreach (BuddyListItem buddy in serverSetting.BuddyList)
                                {
                                    buddy.IsOnSent = false;
                                    buddy.IsOnReceived = false;
                                }
                                buddiesIsOnSent = 0;

                                //send a user event to refresh the buddy list for this server
                                if (BuddyListRefresh != null)
                                    BuddyListRefresh(this, serverSetting.BuddyList);
                            }
                            break;                        
                        case "311":     //whois information username address
                            if (this.UserInfoWindow != null && this.UserInfoWindow.Nick.ToLower() == ircData[3].ToLower())
                            {
                                this.UserInfoWindow.HostName(ircData[4] + "@" + ircData[5]);
                                this.UserInfoWindow.FullName(JoinString(ircData, 7, true));
                                return;
                            }
                            msg = "is " + ircData[4] + "@" + ircData[5] + " (" + JoinString(ircData, 7, true) + ")";
                            WhoisData(this, ircData[3], msg);
                            IALUserData(this, nick, ircData[4] + "@" + ircData[5], "");
                            break;
                        case "312":     //whois information server info
                            if (this.UserInfoWindow != null && this.UserInfoWindow.Nick.ToLower() == ircData[3].ToLower())
                                return;
                            msg = "using " + ircData[4] + " (" + JoinString(ircData, 5, true) + ")";
                            WhoisData(this, ircData[3], msg);
                            break;
                        case "301":     //whois information nick away
                        case "307":     //whois information nick ips
                        case "310":     //whois is available for help
                        case "313":     //whois information is an IRC operator
                            if (this.UserInfoWindow != null && this.UserInfoWindow.Nick.ToLower() == ircData[3].ToLower())
                                return;
                            msg = JoinString(ircData, 4, true);
                            WhoisData(this, ircData[3], msg);
                            break;
                        case "317":     //whois information signon time
                            DateTime date1 = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                            date1 = date1.AddSeconds(Convert.ToDouble(ircData[5]));
                            if (this.UserInfoWindow != null && this.UserInfoWindow.Nick.ToLower() == ircData[3].ToLower())
                            {
                                this.UserInfoWindow.IdleTime(GetDuration(Convert.ToInt32(ircData[4])) + " " + JoinString(ircData, 6, true));
                                this.UserInfoWindow.LogonTime(date1.ToShortTimeString() + " " + date1.ToShortDateString());
                                return;
                            }
                            msg = GetDuration(Convert.ToInt32(ircData[4])) + " " + JoinString(ircData, 6, true) + " " + date1.ToShortTimeString() + " " + date1.ToShortDateString();
                            WhoisData(this, ircData[3], msg);
                            break;
                        case "318":     //whois information end of whois
                            if (this.UserInfoWindow != null && this.UserInfoWindow.Nick.ToLower() == ircData[3].ToLower())
                                return;
                            msg = JoinString(ircData, 4, false);
                            WhoisData(this, ircData[3], msg);
                            break;
                        case "319":     //whois information channels
                            if (this.UserInfoWindow != null && this.UserInfoWindow.Nick.ToLower() == ircData[3].ToLower())
                            {
                                string[] chans = JoinString(ircData, 4, true).Split(' ');
                                foreach (string chan in chans)
                                    this.UserInfoWindow.Channel(chan);
                                return;
                            }
                            msg = "is on: " + JoinString(ircData, 4, true);
                            WhoisData(this, ircData[3], msg);
                            break;
                        case "320":     //whois information
                            if (this.UserInfoWindow != null && this.UserInfoWindow.Nick.ToLower() == ircData[3].ToLower())
                                return;
                            msg = JoinString(ircData, 4, true);
                            WhoisData(this, ircData[3], msg);
                            break;
                        case "330":     //whois information
                            if (this.UserInfoWindow != null && this.UserInfoWindow.Nick.ToLower() == ircData[3].ToLower())
                                return;
                            msg = JoinString(ircData, 5, true) + " " + ircData[4];
                            WhoisData(this, ircData[3], msg);
                            break;
                        case "335":     //whois information
                            if (this.UserInfoWindow != null && this.UserInfoWindow.Nick.ToLower() == ircData[3].ToLower())
                                return;
                            msg = ircData[3] + " " + JoinString(ircData, 4, true);
                            WhoisData(this, ircData[3], msg);
                            break;
                        case "338":     //whois information
                            if (this.UserInfoWindow != null && this.UserInfoWindow.Nick.ToLower() == ircData[3].ToLower())
                                return;
                            msg = JoinString(ircData, 4, false);
                            WhoisData(this, ircData[3], msg);
                            break;                        
                        case "378":     //whois information
                            if (this.UserInfoWindow != null && this.UserInfoWindow.Nick.ToLower() == ircData[3].ToLower())
                                return;
                            msg = RemoveColon(ircData[4]) + " " + JoinString(ircData, 5, true);
                            WhoisData(this, ircData[3], msg);
                            break;
                        case "379":     //whois information
                            if (this.UserInfoWindow != null && this.UserInfoWindow.Nick.ToLower() == ircData[3].ToLower())
                                return;
                            msg = RemoveColon(ircData[4]) + " " + JoinString(ircData, 5, true);
                            WhoisData(this, ircData[3], msg);
                            break;
                        case "671":     //using secure connection
                            if (this.UserInfoWindow != null && this.UserInfoWindow.Nick.ToLower() == ircData[3].ToLower())
                                return;
                            msg = JoinString(ircData, 4, true);
                            WhoisData(this, ircData[3], msg);
                            break;                        
                        case "321":     //start channel list
                            ChannelListStart(this);
                            break;

                        case "322":     //channel list
                            //3 4 rc(5+)
                            ChannelList(this, ircData[3], ircData[4], RemoveColon(JoinString(ircData, 5, true)));
                            break;

                        case "323": //end channel list
                            ServerMessage(this, "End of Channel List");
                            break;

                        case "324":     //channel modes
                            channel = ircData[3];
                            msg = "Channel modes for " + channel + " are :" + JoinString(ircData, 4, false);
                            ChannelMode(this, channel, "", channel, JoinString(ircData, 4, false));
                            GenericChannelMessage(this, channel, msg);
                            break;
                        case "328":     //channel url
                            channel = ircData[3];
                            msg = "Channel URL is " + JoinString(ircData, 4, true);
                            GenericChannelMessage(this, channel, msg);
                            break;
                        case "329":     //channel creation time
                            channel = ircData[3];
                            DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                            date = date.AddSeconds(Convert.ToDouble(ircData[4]));
                            msg = "Channel Created on: " + date.ToShortTimeString() + " " + date.ToShortDateString();
                            GenericChannelMessage(this, channel, msg);
                            break;
                        case "331":     //no topic is set
                            channel = ircData[3];
                            t = FormMain.Instance.GetWindow(this, channel, IceTabPage.WindowType.Channel);
                            if (t != null)
                                if (t.HasChannelInfo)
                                    break;
                            GenericChannelMessage(this, channel, "No Topic Set");
                            break;
                        case "332":     //channel topic
                            channel = ircData[3];
                            t = FormMain.Instance.GetWindow(this, channel, IceTabPage.WindowType.Channel);
                            if (t != null)
                                if (t.HasChannelInfo)
                                    break;
                            ChannelTopic(this, channel, "", "", JoinString(ircData, 4, true));
                            break;
                        case "333":     //channel time
                            channel = ircData[3];
                            nick = ircData[4];
                            DateTime date2 = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                            date2 = date2.AddSeconds(Convert.ToDouble(ircData[5]));

                            t = FormMain.Instance.GetWindow(this, channel, IceTabPage.WindowType.Channel);
                            if (t != null)
                            {
                                if (t.HasChannelInfo)
                                {
                                    t.ChannelInfoForm.ChannelTopicSetBy(nick, date2.ToShortTimeString() + " " + date2.ToShortDateString());
                                    break;
                                }
                            }                            
                            msg = "Channel Topic Set by: " + nick + " on " + date2.ToShortTimeString() + " " + date2.ToShortDateString();
                            GenericChannelMessage(this, channel, msg);
                            break;
                        case "348": //channel exception list
                            channel = ircData[3];
                            //3 is channel
                            //4 is host
                            //5 added by
                            //6 added time
                            t = FormMain.Instance.GetWindow(this, channel, IceTabPage.WindowType.Channel);
                            if (t != null)
                            {
                                if (t.HasChannelInfo)
                                {
                                    DateTime date4 = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Convert.ToDouble(ircData[6]));
                                    t.ChannelInfoForm.AddChannelException(ircData[4], NickFromFullHost(ircData[5]) + " on " + date4.ToShortTimeString() + " " + date4.ToShortDateString());
                                    break;
                                }
                            }
                            msg = JoinString(ircData, 3, false);
                            ServerMessage(this, msg);                            
                            break;
                        case "349": //end of channel exception list
                            break;
                        case "315": //end of who reply
                            channel = ircData[3];                           
                            t = FormMain.Instance.GetWindow(this, channel, IceTabPage.WindowType.Channel);
                            if (t != null)
                            {
                                //end of who reply, do a channel refresh
                                t.GotWhoList = true;
                                if (FormMain.Instance.NickList.CurrentWindow == t)
                                    FormMain.Instance.NickList.RefreshList(t);

                            }
                            break;
                        case "352": //who reply
                            channel = ircData[3];                           
                            t = FormMain.Instance.GetWindow(this, channel, IceTabPage.WindowType.Channel);
                            if (t != null)
                            {
                                IALUserData(this, ircData[7], ircData[4] + "@" + ircData[5], channel);
                                if (t.GotWhoList)
                                    if (ServerMessage != null)
                                        ServerMessage(this, JoinString(ircData, 2, false));

                            }
                            break;
                        case "353": //channel user list
                            channel = ircData[4];
                            t = FormMain.Instance.GetWindow(this, channel, IceTabPage.WindowType.Channel);
                            if (t != null)
                            {
                                if (t.IsFullyJoined)
                                {
                                    //just show the message to the console
                                    ServerMessage(this, JoinString(ircData, 4, true));
                                    return;
                                }
                            }
                            if (!t.GotNamesList)
                            {
                                string[] nicks = JoinString(ircData, 5, true).Split(' ');
                                foreach (string nickName in nicks)
                                {
                                    if (nickName.Length > 0)
                                    {
                                        JoinChannel(this, channel, nickName, "", false);
                                        IALUserData(this, nickName, "", channel);
                                    }
                                }
                            }
                            break;
                        case "365":  //End of Links
                            msg = JoinString(ircData, 4, true);
                            ServerMessage(this, msg);                            
                            break;
                        case "366":     //end of names
                            channel = ircData[3];
                            //channel is fully joined                            
                            t = FormMain.Instance.GetWindow(this, channel, IceTabPage.WindowType.Channel);
                            if (t != null)
                            {
                                t.GotNamesList = true;
                                //send a WHO command to get all the hosts
                                if (t.Nicks.Count < 200)
                                {
                                    t.GotWhoList = false;
                                    SendData("WHO " + t.TabCaption);
                                }
                                else
                                    t.GotWhoList = true;

                                t.IsFullyJoined = true;
                                if (FormMain.Instance.NickList.CurrentWindow == t)
                                    FormMain.Instance.NickList.RefreshList(t);
                            }
                            break;
                        case "367": //channel ban list
                            channel = ircData[3];
                            //3 is channel
                            //4 is host
                            //5 banned by
                            //6 ban time
                            t = FormMain.Instance.GetWindow(this, channel, IceTabPage.WindowType.Channel);
                            if (t != null)
                            {
                                if (t.HasChannelInfo)
                                {
                                    DateTime date3 = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Convert.ToDouble(ircData[6]));
                                    t.ChannelInfoForm.AddChannelBan(ircData[4], ircData[5] + " on " + date3.ToShortTimeString() + " " + date3.ToShortDateString());
                                    break;
                                }
                            }
                            msg = JoinString(ircData, 3, false);
                            ServerMessage(this, msg);                            
                            break;
                        case "368": //end of channel ban list
                            break;
                        case "372": //motd
                        case "375":
                            msg = JoinString(ircData, 3, true);                            
                            if (serverSetting.ShowMOTD || serverSetting.ForceMOTD)
                                ServerMOTD(this, msg);
                            break;

                        case "376": //end of motd
                        case "422": //missing motd
                            if (serverSetting.ForceMOTD)
                            {
                                serverSetting.ForceMOTD = false;
                                return;
                            }
                            
                            ServerMessage(this, "You have successfully connected to " + serverSetting.RealServerName);

                            if (serverSetting.SetModeI)
                                SendData("MODE " + serverSetting.NickName + " +i");

                            //run autoperform
                            if (serverSetting.AutoPerformEnable)
                            {
                                ServerMessage(this, "Running AutoPerform command(s)..");

                                string autoCommand;
                                foreach (string command in serverSetting.AutoPerform)
                                {
                                    autoCommand = command.Replace("\r", String.Empty);
                                    if (!autoCommand.StartsWith(";"))
                                        OutGoingCommand(this, autoCommand);
                                }
                            }

                            // Nickserv password
                            if (serverSetting.NickservPassword != null && serverSetting.NickservPassword.Length > 0)
                            {
                                OutGoingCommand(this, "/msg NickServ identify " + serverSetting.NickservPassword);
                            }

                            if (serverSetting.RejoinChannels)
                            {
                                //rejoin any channels that are open
                                foreach (IceTabPage tw in FormMain.Instance.TabMain.TabPages)
                                {
                                    if (tw.WindowStyle == IceTabPage.WindowType.Channel)
                                    {
                                        if (tw.Connection == this)
                                        {
                                            if (this.serverSetting.AutoJoinDelay)
                                                OutGoingCommand(this, "/timer rejoin 5 1 /join " + tw.TabCaption);
                                            else
                                                SendData("JOIN " + tw.TabCaption);
                                        }
                                    }
                                }
                            }

                            //run autojoins
                            if (serverSetting.AutoJoinEnable)
                            {
                                ServerMessage(this, "Auto-joining Channels");

                                foreach (string chan in serverSetting.AutoJoinChannels)
                                {
                                    if (!chan.StartsWith(";"))
                                    {
                                        if (this.serverSetting.AutoJoinDelay)
                                            OutGoingCommand(this, "/timer autojoin 5 1 /join " + chan);
                                        else
                                            SendData("JOIN " + chan);
                                    }
                                }
                            }

                            fullyConnected = true;
                            FormMain.Instance.ServerTree.Invalidate();
                            
                            //read the command queue
                            if (commandQueue.Count > 0)
                            {
                                foreach (string command in commandQueue)
                                    SendData(command);
                            }
                            commandQueue.Clear();

                            BuddyListCheck();
                            buddyListTimer.Start();
                            break;
                        case "396":     //mode X message                            
                            msg = ircData[3] + " " + JoinString(ircData, 4, true);
                            if (this.serverSetting.IAL.ContainsKey(this.serverSetting.NickName))
                            {
                                host = ((InternalAddressList)this.serverSetting.IAL[this.serverSetting.NickName]).Host;
                                if (host.IndexOf("@") > -1)
                                    ((InternalAddressList)this.serverSetting.IAL[this.serverSetting.NickName]).Host = host.Substring(0, host.IndexOf("@") + 1) + ircData[3];
                                
                            }
                            ServerMessage(this, msg);
                            break;
                        case "439":
                        case "931":
                            msg = JoinString(ircData, 3, true);
                            ServerMessage(this, msg);
                            break;
                        case "901":
                            msg = JoinString(ircData, 6, true);
                            ServerMessage(this, msg);                            
                            break;

                        case "PRIVMSG":
                            channel = ircData[2];
                            msg = JoinString(ircData, 3, true);
                            
                            if (CheckIgnoreList(nick, host)) return;
                
                            if (channel.ToLower() == serverSetting.NickName.ToLower())
                            {
                                //this is a private message to you
                                //check if it was an notice/action
                                if (msg[0] == (char)1)
                                {
                                    //drop the 1st and last CTCP Character
                                    msg = msg.Trim(new char[] { (char)1 });
                                    //check for action
                                    switch (msg.Split(' ')[0].ToUpper())
                                    {
                                        case "ACTION":
                                            msg = msg.Substring(6);
                                            QueryAction(this, nick, host, msg);
                                            IALUserData(this, nick, host, "");
                                            break;
                                        case "VERSION":
                                        case "TIME":
                                        case "PING":
                                        case "USERINFO":
                                        case "CLIENTINFO":
                                        case "SOURCE":
                                        case "FINGER":
                                            CtcpMessage(this, nick, msg.Split(' ')[0].ToUpper(), msg.Substring(msg.IndexOf(" ") + 1));
                                            break;
                                        default:
                                            //check for DCC SEND, DCC CHAT, DCC ACCEPT, DCC RESUME
                                            if (msg.ToUpper().StartsWith("DCC SEND"))
                                            {
                                                msg = msg.Substring(8).Trim();
                                                System.Diagnostics.Debug.WriteLine("PRIVMSG:" + msg);
                                                
                                                string[] dccData = msg.Split(' ');
                                                //sometimes the filenames can be include in quotes
                                                System.Diagnostics.Debug.WriteLine("length:" + dccData.Length);
                                                if (dccData.Length > 4)
                                                {
                                                    uint uresult;
                                                    if (!uint.TryParse(dccData[dccData.Length - 1], out uresult))
                                                    {
                                                        return;
                                                    }

                                                    //there can be a passive dcc request sent
                                                    //PegMan-default(2010-05-07)-OS.zip 4294967295 0 176016 960
                                                    //960 is the passive DCC ID
                                                    //length:5
                                                    //960:176016:0:
                                                    if (dccData[dccData.Length - 3] == "0")
                                                    {
                                                        //passive DCC
                                                        string id = dccData[dccData.Length - 1];
                                                        uint fileSize = uint.Parse(dccData[dccData.Length - 2]);
                                                        string port = dccData[dccData.Length - 3];
                                                        string ip = dccData[dccData.Length - 4];
                                                        string file = "";
                                                        if (msg.Contains("\""))
                                                        {
                                                            string[] words = msg.Split('"');
                                                            if (words.Length == 3)
                                                                file = words[1];
                                                            System.Diagnostics.Debug.WriteLine(words.Length);
                                                            foreach (string w in words)
                                                            {
                                                                System.Diagnostics.Debug.WriteLine(w);
                                                            }
                                                        }
                                                        else
                                                            file = dccData[dccData.Length - 5];

                                                        //start up a listening socket on a specific port and send back to ip
                                                        //http://trout.snt.utwente.nl/ubbthreads/ubbthreads.php?ubb=showflat&Number=139329&site_id=1#import

                                                        System.Diagnostics.Debug.WriteLine("PASSIVE DCC " + id + ":" + fileSize + ":" + port + ":" + ip + ":" + file);
                                                        if (DCCPassive != null)
                                                            DCCPassive(this, nick, host, ip, file, fileSize, 0, false, id);
                                                        
                                                        return;
                                                    }
                                                    else
                                                    {
                                                        uint fileSize = uint.Parse(dccData[dccData.Length - 1]);
                                                        string port = dccData[dccData.Length - 2];
                                                        string ip = dccData[dccData.Length - 3];
                                                        string file = "";
                                                        if (msg.Contains("\""))
                                                        {
                                                            string[] words = msg.Split('"');
                                                            if (words.Length == 3)
                                                                file = words[1];
                                                            System.Diagnostics.Debug.WriteLine(words.Length);
                                                            foreach (string w in words)
                                                            {
                                                                System.Diagnostics.Debug.WriteLine(w);
                                                            }
                                                        }

                                                        System.Diagnostics.Debug.WriteLine(fileSize + ":" + port + ":" + ip + ":" + file);
                                                        //check that values are numbers

                                                        if (DCCFile != null && file.Length > 0)
                                                            DCCFile(this, nick, host, port, ip, file, fileSize,0, false);
                                                        return;
                                                    }
                                                }
                                                //string fileName = dccData[0];
                                                //string ip = dccData[1];
                                                //string port = dccData[2];
                                                //string fileSize = dccData[3];
                                                System.Diagnostics.Debug.WriteLine("DCC SEND:" + dccData[0] + "::" + dccData[1] + "::" + dccData[2] + "::" + dccData[3]);
                                                
                                                //check if filesize is a valid number
                                                uint result;                                                
                                                if (!uint.TryParse(dccData[3], out result))
                                                    return;

                                                //check if quotes around file name
                                                if (dccData[0].StartsWith("\"") && dccData[0].EndsWith("\""))
                                                {
                                                    dccData[0] = dccData[0].Substring(1, dccData[0].Length - 2);
                                                }

                                                if (DCCFile != null)
                                                    DCCFile(this, nick, host, dccData[2], dccData[1], dccData[0], uint.Parse(dccData[3]), 0, false);
                                                else
                                                    ServerError(this, "Invalid DCC File send from " + nick);
                                            }
                                            else if (msg.ToUpper().StartsWith("DCC RESUME"))
                                            {
                                                //dcc resume, other client requests resuming a file
                                                //PRIVMSG User1 :DCC RESUME "filename" port position
                                                System.Diagnostics.Debug.WriteLine("DCC RESUME:" + data);
                                                //send back a DCC ACCEPT MESSAGE

                                            }
                                            else if (msg.ToUpper().StartsWith("DCC ACCEPT"))
                                            {
                                                //dcc accept, other client accepts the dcc resume
                                                //PRIVMSG User2 :DCC ACCEPT file.ext port position
                                                //System.Diagnostics.Debug.WriteLine("DCC ACCEPT:" + data);
                                                msg = msg.Substring(10).Trim();
                                                //System.Diagnostics.Debug.WriteLine("ACCEPT:" + msg);
                                                string[] dccData = msg.Split(' ');
                                                //System.Diagnostics.Debug.WriteLine("length:" + dccData.Length);
                                                
                                                if (DCCFile != null)
                                                    DCCFile(this, nick, host, dccData[dccData.Length - 2], "ip", "file", 0, uint.Parse(dccData[dccData.Length - 1]), true);
                                                

                                            }
                                            else if (msg.ToUpper().StartsWith("DCC CHAT"))
                                            {
                                                string ip = ircData[6];
                                                string port = ircData[7].TrimEnd(new char[] { (char)1 });
                                                if (DCCChat != null)
                                                    DCCChat(this, nick, host, port, ip);
                                            }
                                            else
                                                UserNotice(this, nick, msg);
                                            break;
                                    }
                                }
                                else
                                {
                                    QueryMessage(this, nick, host, msg);
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
                                            ChannelAction(this, channel, nick, host, msg);
                                            IALUserData(this, nick, host, channel);
                                            break;
                                        case "VERSION":
                                        case "TIME":
                                        case "PING":
                                        case "USERINFO":
                                        case "CLIENTINFO":
                                        case "SOURCE":
                                        case "FINGER":
                                            //we need to send a reply
                                            CtcpMessage(this, nick, msg.Split(' ')[0].ToUpper(), msg);
                                            break;
                                        default:
                                            if (msg.ToUpper().StartsWith("ACTION "))
                                            {
                                                msg = msg.Substring(7);
                                                ChannelAction(this, channel, nick, host, msg);
                                                IALUserData(this, nick, host, channel);
                                            }
                                            else
                                            {
                                                ChannelNotice(this, nick, host,(char)32,channel, msg);
                                                IALUserData(this, nick, host, channel);
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    if (ChannelMessage != null)
                                        ChannelMessage(this, channel, nick, host, msg);
                                    IALUserData(this, nick, host, channel);

                                }
                            }
                            break;
                        case "INVITE":      //channel invite
                            channel = ircData[3];
                            ChannelInvite(this, channel, nick, host);
                            break;

                        case "NOTICE":
                            msg = JoinString(ircData, 3, true);
                            //check if its a user notice or a server notice
                            if (nick.ToLower() == serverSetting.RealServerName.ToLower())
                                ServerNotice(this, msg);
                            else
                            {
                                if (CheckIgnoreList(nick, host)) return;
                                
                                if (initialLogon && serverSetting.StatusMSG == null && serverSetting.StatusModes != null)
                                {
                                    serverSetting.StatusMSG = new char[serverSetting.StatusModes[1].Length];
                                    for (int j = 0; j < serverSetting.StatusModes[1].Length; j++)
                                        serverSetting.StatusMSG[j] = serverSetting.StatusModes[1][j];
                                }
                                if (initialLogon && serverSetting.ChannelTypes != null && Array.IndexOf(serverSetting.ChannelTypes, ircData[2][0]) != -1)
                                {
                                    ChannelNotice(this, nick, host, '0', ircData[2], msg);
                                    IALUserData(this, nick, host, ircData[2]);
                                }
                                else if (initialLogon && serverSetting.StatusMSG != null && Array.IndexOf(serverSetting.StatusMSG, ircData[2][0]) != -1 && Array.IndexOf(serverSetting.ChannelTypes, ircData[2][1]) != -1)
                                {
                                    ChannelNotice(this, nick, host, ircData[2][0], ircData[2].Substring(1), msg);
                                    IALUserData(this, nick, host, ircData[2]);
                                }
                                else
                                {
                                    //System.Diagnostics.Debug.WriteLine("NOTICE:" + msg);
                                    if (msg.ToUpper().StartsWith("DCC SEND"))
                                    {
                                        System.Diagnostics.Debug.WriteLine("NOTICE DCC SEND:" + nick + ":" + msg);                                        
                                        UserNotice(this, nick, msg);
                                    }
                                    else if (msg.ToUpper().StartsWith("DCC CHAT"))
                                    {
                                        if (!FormMain.Instance.IceChatOptions.DCCChatIgnore)
                                            UserNotice(this, nick, msg);
                                    }
                                    else
                                    {
                                        if (msg[0] == (char)1)
                                        {
                                            msg = msg.Substring(1, msg.Length - 2);
                                            string ctcp = msg.Split(' ')[0].ToUpper();
                                            msg = msg.Substring(msg.IndexOf(" ") + 1);
                                            switch (ctcp)
                                            {
                                                case "PING":
                                                    int result;
                                                    if (Int32.TryParse(msg, out result))
                                                    {
                                                        int diff = System.Environment.TickCount - Convert.ToInt32(msg);
                                                        msg = GetDurationMS(diff);
                                                    }
                                                    if (CtcpReply != null)
                                                        CtcpReply(this, nick, ctcp, msg);    
                                                    break;
                                                default:
                                                    if (CtcpReply != null)
                                                        CtcpReply(this, nick, ctcp, msg);
                                                    break;
                                            }
                                        }
                                        else
                                            UserNotice(this, nick, msg);
                                    }
                                }
                            }
                            break;

                        case "MODE":
                            channel = ircData[2];

                            if (channel.ToLower() == serverSetting.NickName.ToLower())
                            {
                                if (host.IndexOf('@') > -1 && this.serverSetting.LocalIP == null)
                                {
                                    this.serverSetting.LocalHost = host;
                                    try
                                    {
                                        host = host.Substring(host.IndexOf('@') + 1);
                                        System.Net.IPAddress[] addresslist = System.Net.Dns.GetHostAddresses(host);
                                        foreach (System.Net.IPAddress address in addresslist)
                                            this.serverSetting.LocalIP = address;
                                    }
                                    catch { }

                                }
                                //user mode
                                tempValue = JoinString(ircData, 3, true);
                                UserMode(this, channel, tempValue);
                            }
                            else
                            {
                                //channel mode
                                tempValue = JoinString(ircData, 3, false);

                                ChannelMode(this, nick, HostFromFullHost(ircData[0]), channel, tempValue);
                            }
                            break;

                        case "JOIN":
                            channel = RemoveColon(ircData[2]);
                            //check if it is our own nickname
                            if (nick.ToLower() == serverSetting.NickName.ToLower())
                            {
                                JoinChannelMyself(this, channel);
                                SendData("MODE " + channel);
                            }
                            else
                            {
                                JoinChannel(this, channel, nick, host, true);
                                IALUserData(this, nick, host, channel);
                            }
                            break;

                        case "PART":
                            channel = RemoveColon(ircData[2]);

                            tempValue = JoinString(ircData, 3, true); //part reason
                            //check if it is our own nickname
                            if (nick.ToLower() == serverSetting.NickName.ToLower())
                            {
                                //part self
                                PartChannelMyself(this, channel);
                            }
                            else
                            {
                                tempValue = JoinString(ircData, 3, true);
                                IALUserPart(this, nick, channel);
                                PartChannel(this, channel, nick, host, tempValue);
                            }
                            break;

                        case "QUIT":
                            nick = NickFromFullHost(RemoveColon(ircData[0]));
                            host = HostFromFullHost(RemoveColon(ircData[0]));
                            tempValue = JoinString(ircData, 2, true);

                            QuitServer(this, nick, host, tempValue);
                            IALUserQuit(this, nick);
                            break;

                        case "NICK":
                            //old nickname
                            nick = NickFromFullHost(RemoveColon(ircData[0]));
                            host = HostFromFullHost(RemoveColon(ircData[0]));

                            //new nickname
                            tempValue = RemoveColon(ircData[2]);

                            if (nick.ToLower() == serverSetting.NickName.ToLower())
                            {
                                //if it is your own nickname, update it
                                serverSetting.NickName = tempValue;
                            }

                            ChangeNick(this, nick, tempValue, HostFromFullHost(ircData[0]));
                            IALUserChange(this, nick, tempValue);

                            break;

                        case "KICK":
                            msg = JoinString(ircData, 4, true);  //kick message                        
                            channel = ircData[2];
                            //this is WHO got kicked
                            nick = ircData[3];
                            //check if it is our own nickname who got kicked
                            if (nick.ToLower() == serverSetting.NickName.ToLower())
                            {
                                //we got kicked
                                KickMyself(this, channel, msg, ircData[0]);
                            }
                            else
                            {
                                KickNick(this, channel, nick, msg, ircData[0]);
                                IALUserPart(this, nick, channel);
                            }
                            break;
                        case "PONG":                            
                            pongTimer.Stop();
                            pongTimer.Start();                            
                            break;
                        case "TOPIC":   //channel topic change
                            channel = ircData[2];
                            msg = JoinString(ircData, 3, true);
                            ChannelTopic(this, channel, nick, host, msg);
                            break;
                        case "AUTH":    //NOTICE AUTH
                            ServerMessage(this, JoinString(ircData, 2, true));
                            break;

                        //errors
                        case "404": //can not send to channel
                        case "432": //erroneus nickname
                        case "438": //nick change too fast
                        case "467": //channel key already set
                        case "468": //only servers can change mode
                        case "482": //not a channel operator
                            msg = ircData[3] + " " + JoinString(ircData, 4, true);
                            ServerError(this, msg);
                            break;
                        case "401": //no such nick
                        case "402": //no such server
                        case "403": //no such channel
                        case "405": //joined too many channels
                        case "407": //no message delivered
                        case "411": //no recipient given
                        case "412": //no text to send
                        case "421": //unknown command
                        case "431": //no nickname given
                        case "470": //forward to other channel
                        case "471": //can not join channel (limit enforced)
                        case "472": //unknown char to me (channel mode)
                            ServerError(this, JoinString(ircData, 3, false));
                            break;
                        case "473": //can not join channel invite only
                            msg = ircData[3] + " " + JoinString(ircData, 4, true);
                            ServerError(this, msg);
                            break;
                        case "442": //you're not in that channel
                        case "474": //Cannot join channel (+b)
                        case "475": //Cannot join channel (+k)
                            msg = ircData[3] + " " + JoinString(ircData, 4, true);
                            ServerError(this, msg);                            
                            break;  

                        case "433": //nickname in use
                            ServerMessage(this, JoinString(ircData, 4, true));

                            if (!triedAltNickName && !initialLogon)
                            {
                                nick = serverSetting.NickName;
                                SendData("NICK " + serverSetting.AltNickName);
                                ChangeNick(this, nick, serverSetting.AltNickName, HostFromFullHost(RemoveColon(ircData[0])));
                                serverSetting.NickName = serverSetting.AltNickName;
                                serverSetting.AltNickName = nick;
                                triedAltNickName = true;
                            }
                            else
                            {                                
                                //pick a random nick
                                Random r = new Random();
                                string randNick = r.Next(10, 99).ToString();
                                if (serverSetting.NickName.Length + 2 <= serverSetting.MaxNickLength)
                                {
                                    serverSetting.NickName = serverSetting.NickName + randNick;
                                    SendData("NICK " + serverSetting.NickName);
                                }
                                else
                                {
                                    serverSetting.NickName = serverSetting.NickName.Substring(0, serverSetting.MaxNickLength - 2);
                                    serverSetting.NickName = serverSetting.NickName + randNick;
                                    SendData("NICK " + serverSetting.NickName);
                                }
                            }

                            break;
                        case "465": //no open proxies
                        case "513": //if you can not connect, type /quote PONG ...
                            ServerError(this, JoinString(ircData, 3, true));                            
                            break;
                        
                        default:
                            ServerMessage(this, data);
                            break;
                        //                            
                    }
                }
            }
            catch (Exception e)
            {
                FormMain.Instance.WriteErrorFile(this, "ParseData", e);
            }
        }

        private bool CheckIgnoreList(string nick, string host)
        {
            if (!this.serverSetting.IgnoreListEnable) return false; //if ignore list is disabled, no match
            if (this.serverSetting.IgnoreList.Length == 0) return false;    //if no items in list, no match

            foreach (string ignore in serverSetting.IgnoreList)
            {
                if (!ignore.StartsWith(";"))    //check to make sure its not disabled
                {
                    //check for an exact match
                    if (nick.ToLower() == ignore.ToLower()) return true;
                    
                    //check if we are looking for a host match
                    if (ignore.Contains("!") && ignore.Contains("@"))
                    {

                    }
                    else
                    {
                        //check for wildcard/regex match for nick name
                        if (Regex.Match(nick, ignore, RegexOptions.IgnoreCase).Success) return true;
                    }
                }
            }


            return false;
        }


        #region Parsing Methods

        private string GetDurationMS(int milliSseconds)
        {
            TimeSpan t = new TimeSpan(0,0,0,0, milliSseconds);

            string s = t.Seconds.ToString() + "." + t.Milliseconds.ToString() + " secs";
            if (t.Minutes > 0)
                s = t.Minutes.ToString() + " mins " + s;
            if (t.Hours > 0)
                s = t.Hours.ToString() + " hrs " + s;
            if (t.Days > 0)
                s = t.Days.ToString() + " days " + s;

            return s;
        }

        private string GetDuration(int seconds)
        {
            TimeSpan t = new TimeSpan(0, 0, seconds);

            string s = t.Seconds.ToString() + " secs";            
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
