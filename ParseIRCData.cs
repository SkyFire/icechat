/******************************************************************************\
 * IceChat 2009 Internet Relay Chat Client
 *
 * Copyright (C) 2010 Paul Vanderzee <snerf@icechat.net>
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
        internal event UserNoticeDelegate UserNotice;
    
        internal event ServerNoticeDelegate ServerNotice;

        internal event ChannelListDelegate ChannelList;

        internal event RawServerIncomingDataDelegate RawServerIncomingData;
        internal event RawServerOutgoingDataDelegate RawServerOutgoingData;

        internal event IALUserDataDelegate IALUserData;
        internal event IALUserChangeDelegate IALUserChange;
        internal event IALUserPartDelegate IALUserPart;
        internal event IALUserQuitDelegate IALUserQuit;

        private bool triedAltNickName = false;
        private bool initialLogon = false;

        public FormUserInfo UserInfoWindow = null;

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
                        case "042":
                            msg = JoinString(ircData, 4, true) + " " + ircData[3];
                            ServerMessage(this, msg);                            
                            break;
                        case "219":
                            ServerMessage(this, JoinString(ircData, 4, true));
                            break;
                        case "221":
                            break;
                        
                        case "250":
                        case "251":
                        case "255":
                            ServerMessage(this, JoinString(ircData, 3, true));
                            break;

                        case "252":
                        case "253":
                        case "254":
                            break;

                        case "265":
                        case "266":
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
                                string[] hosts = msg.Split(' ');
                                foreach (string h in hosts)
                                    UserHostReply(this, h);
                            }
                            break;
                        case "311":     //whois information
                            if (this.UserInfoWindow != null && this.UserInfoWindow.Nick == ircData[3])
                            {
                                this.UserInfoWindow.HostName(ircData[4] + "@" + ircData[5]);
                                this.UserInfoWindow.FullName(JoinString(ircData, 7, true));
                                return;
                            }
                            msg = "is " + ircData[4] + "@" + ircData[5] + " (" + JoinString(ircData, 7, true) + ")";
                            WhoisData(this, ircData[3], msg);
                            IALUserData(this, nick, host, "");
                            break;
                        case "312":     //whois information
                            if (this.UserInfoWindow != null && this.UserInfoWindow.Nick == ircData[3])
                                return;
                            msg = "using " + ircData[4] + " (" + JoinString(ircData, 5, true) + ")";
                            WhoisData(this, ircData[3], msg);
                            break;
                        case "301":     //whois information
                        case "307":     //whois information
                        case "313":     //whois information
                            if (this.UserInfoWindow != null && this.UserInfoWindow.Nick == ircData[3])
                                return;
                            msg = JoinString(ircData, 4, true);
                            WhoisData(this, ircData[3], msg);
                            break;
                        case "317":     //whois information
                            DateTime date1 = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                            date1 = date1.AddSeconds(Convert.ToDouble(ircData[5]));
                            if (this.UserInfoWindow != null && this.UserInfoWindow.Nick == ircData[3])
                            {
                                this.UserInfoWindow.IdleTime(GetDuration(Convert.ToInt32(ircData[4])) + " " + JoinString(ircData, 6, true));
                                this.UserInfoWindow.LogonTime(date1.ToShortTimeString() + " " + date1.ToShortDateString());
                                return;
                            }
                            msg = GetDuration(Convert.ToInt32(ircData[4])) + " " + JoinString(ircData, 6, true) + " " + date1.ToShortTimeString() + " " + date1.ToShortDateString();
                            WhoisData(this, ircData[3], msg);
                            break;
                        case "318":     //whois information
                            if (this.UserInfoWindow != null && this.UserInfoWindow.Nick == ircData[3])
                                return;
                            msg = JoinString(ircData, 4, false);
                            WhoisData(this, ircData[3], msg);
                            break;
                        case "319":     //whois information 
                            if (this.UserInfoWindow != null && this.UserInfoWindow.Nick == ircData[3])
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
                            if (this.UserInfoWindow != null && this.UserInfoWindow.Nick == ircData[3])
                                return;
                            msg = JoinString(ircData, 4, true);
                            WhoisData(this, ircData[3], msg);
                            break;
                        case "330":     //whois information
                            if (this.UserInfoWindow != null && this.UserInfoWindow.Nick == ircData[3])
                                return;
                            msg = JoinString(ircData, 5, true) + " " + ircData[4];
                            WhoisData(this, ircData[3], msg);
                            break;
                        case "338":     //whois information
                            if (this.UserInfoWindow != null && this.UserInfoWindow.Nick == ircData[3])
                                return;
                            msg = JoinString(ircData, 4, false);
                            WhoisData(this, ircData[3], msg);
                            break;                        
                        case "378":     //whois information
                            if (this.UserInfoWindow != null && this.UserInfoWindow.Nick == ircData[3])
                                return;
                            msg = RemoveColon(ircData[4]) + " " + JoinString(ircData, 5, true);
                            WhoisData(this, ircData[3], msg);
                            break;
                        case "379":     //whois information
                            if (this.UserInfoWindow != null && this.UserInfoWindow.Nick == ircData[3])
                                return;
                            msg = RemoveColon(ircData[4]) + " " + JoinString(ircData, 5, true);
                            WhoisData(this, ircData[3], msg);
                            break;
                        
                        case "321":     //start channel list
                            break;

                        case "322":     //channel list
                            //3 4 rc(5+)
                            ChannelList(this, ircData[3], ircData[4], RemoveColon(JoinString(ircData, 5, true)));
                            break;

                        case "323": //end channel list
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
                        case "332":     //channel topic
                            //mediatraffic2.fi.quakenet.org 332 Snerf #icechat :Official IceChat Support Channel
                            channel = ircData[3];
                            ChannelTopic(this, channel, "", "", JoinString(ircData, 4, true));
                            break;
                        case "333":     //channel time
                            //mediatraffic2.fi.quakenet.org 333 Snerf #icechat IceBot 1234725840    
                            channel = ircData[3];
                            nick = ircData[4];
                            DateTime date2 = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                            date2 = date2.AddSeconds(Convert.ToDouble(ircData[5]));
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
                                if (FormMain.Instance.CurrentWindowType == IceTabPage.WindowType.Channel)
                                    if ((FormMain.Instance.NickList.CurrentWindow.TabCaption == t.TabCaption) && (FormMain.Instance.NickList.CurrentWindow.Connection == t.Connection))
                                        FormMain.Instance.NickList.RefreshList(t);

                            }
                            break;
                        case "352": //who reply
                            //:stockholm.se.quakenet.org 352 Snerf2009 #icechat2009 Ice2009 IceChat.users.quakenet.org *.quakenet.org Snerf2009 H@x :0 The Chat Cool People Use
                            channel = ircData[3];                           
                            t = FormMain.Instance.GetWindow(this, channel, IceTabPage.WindowType.Channel);
                            if (t != null)
                            {
                                //t.UpdateNick(ircData[7], ircData[4] + "@" + ircData[5]);
                                IALUserData(this, ircData[7], ircData[4] + "@" + ircData[5], channel);
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
                            
                            string[] nicks = JoinString(ircData, 5, true).Split(' ');
                            foreach (string nickName in nicks)
                            {
                                if (nickName.Length > 0)
                                {
                                    JoinChannel(this, channel, nickName, "", false);
                                    IALUserData(this, nickName, "", channel);
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
                                //send a WHO command to get all the hosts
                                if (t.Nicks.Count < 200)
                                    SendData("WHO " + t.TabCaption);
                                
                                t.IsFullyJoined = true;
                                if (FormMain.Instance.CurrentWindowType == IceTabPage.WindowType.Channel)
                                    if ((FormMain.Instance.NickList.CurrentWindow.TabCaption == t.TabCaption) && (FormMain.Instance.NickList.CurrentWindow.Connection == t.Connection))
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
                            break;

                        case "396":     //mode X message
                            msg = ircData[3] + " " + JoinString(ircData, 4, true);
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
                                            CtcpMessage(this, nick, msg.Split(' ')[0].ToUpper());
                                            break;
                                        default:
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
                                            CtcpMessage(this, nick, msg.Split(' ')[0].ToUpper());
                                            break;
                                        default:
                                            if (msg.Substring(7).ToUpper() == "ACTION ")
                                            {
                                                msg = msg.Substring(7);
                                                ChannelAction(this, channel, nick, host, msg);
                                                IALUserData(this, nick, host, channel);
                                            }
                                            else
                                            //check for DCC SEND, DCC CHAT, DCC ACCEPT, DCC RESUME
                                            {
                                                ChannelNotice(this, nick, host,(char)32,channel, msg);
                                                IALUserData(this, nick, host, channel);
                                            }
                                            break;
                                    }
                                }
                                else
                                {
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
                            if (nick == serverSetting.RealServerName)
                                ServerNotice(this, msg);
                            else
                            {
                                if (CheckIgnoreList(nick, host)) return;
                                
                                if (initialLogon && serverSetting.StatusMSG == null)
                                {
                                    serverSetting.StatusMSG = new char[serverSetting.StatusModes[1].Length];
                                    for (int j = 0; j < serverSetting.StatusModes[1].Length; j++)
                                        serverSetting.StatusMSG[j] = serverSetting.StatusModes[1][j];
                                }
                                if (initialLogon && Array.IndexOf(serverSetting.ChannelTypes, ircData[2][0]) != -1)
                                {
                                    ChannelNotice(this, nick, host, '0', ircData[2], msg);
                                    IALUserData(this, nick, host, ircData[2]);
                                }
                                else if (initialLogon && Array.IndexOf(serverSetting.StatusMSG, ircData[2][0]) != -1 && Array.IndexOf(serverSetting.ChannelTypes, ircData[2][1]) != -1)
                                {
                                    ChannelNotice(this, nick, host, ircData[2][0], ircData[2].Substring(1), msg);
                                    IALUserData(this, nick, host, ircData[2]);
                                }
                                else
                                {
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
                            if (nick == serverSetting.NickName)
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
                            if (nick == serverSetting.NickName)
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

                            if (serverSetting.NickName == nick)
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
                            if (nick == serverSetting.NickName)
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

                        case "TOPIC":   //channel topic change
                            channel = ircData[2];
                            msg = JoinString(ircData, 3, true);
                            ChannelTopic(this, channel, nick, host, msg);
                            break;
                        case "AUTH":    //NOTICE AUTH
                            ServerMessage(this, JoinString(ircData, 2, true));
                            break;

                        //errors
                        case "401": //no such nick
                        case "412": //no text to send
                        case "421": //unknown command
                        case "472": //unknown char to me (channel mode)
                            ServerError(this, JoinString(ircData, 3, false));
                            break;
                        case "473":     //can not join channel invite only
                            msg = ircData[3] + " " + JoinString(ircData, 4, true);
                            ServerMessage(this, msg);
                            break;
                        case "474": //Cannot join channel (+b)
                            msg = ircData[3] + " " + JoinString(ircData, 4, true);
                            ServerError(this, msg);                            
                            break;

                        case "433": //nickname in use
                            nick = NickFromFullHost(RemoveColon(ircData[0]));

                            ServerMessage(this, JoinString(ircData, 4, true) + " : " + ircData[3]);
                           
                            if (!triedAltNickName && !initialLogon)
                            {
                                SendData("NICK " + serverSetting.AltNickName);
                                ChangeNick(this, nick, serverSetting.AltNickName, HostFromFullHost(RemoveColon(ircData[0])));
                                serverSetting.NickName = serverSetting.AltNickName;
                                triedAltNickName = true;
                            }

                            break;
                        case "513":
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
                FormMain.Instance.WriteErrorFile("ParseData Error:" + e.Message + "::" + data, e.StackTrace);
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
