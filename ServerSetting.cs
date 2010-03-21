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
using System.IO;
using System.Xml.Serialization;

namespace IceChat
{
    [XmlRoot("IceChatServers")]
    public class IceChatServers
    {
        [XmlArray("Servers")]
        [XmlArrayItem("Item",typeof(ServerSetting))]
        public ArrayList listServers;

        public IceChatServers() 
        {
            listServers = new ArrayList();
        }

        public void AddServer(ServerSetting server)
        {
            listServers.Add(server);
        }

        public void RemoveServer(ServerSetting server)
        {
            listServers.Remove(server);
        }

        public int GetNextID()
        {
            if (listServers.Count ==0)
                return 1;
            return ((ServerSetting)listServers[listServers.Count-1]).ID+1;
        }
    }
    
    public class ServerSetting
    {

        //set the default values (only for specific settings)
        private string _serverPort = "6667";
        private string _displayName = "";
        private string _identName = "Ice2009";
        private string _fullName = "The Chat Cool People Use";
        private string _realServerName = "";
        private string _quitMessage = "$randquit";
        private string _encoding = System.Text.Encoding.Default.WebName.ToString();
        private bool _setModeI = true;
        private bool _showPingPong = false;
        private bool _autoJoinDelay = false;

        [XmlAttribute("ServerID")]
        public int ID
        { get; set; }

        [XmlElement("ServerName")]
        public string ServerName
        { get; set; }
        
        [XmlElement("DisplayName")]
        public string DisplayName
        { get { return this._displayName; } set { this._displayName = value; } }

        [XmlElement("ServerPort")]
        public string ServerPort
        { get { return this._serverPort; } set { this._serverPort = value; } }

        [XmlElement("NickName")]
        public string NickName
        { get; set; }

        [XmlElement("Password")]
        public string Password
        { get; set; }

        [XmlElement("NickservPassword")]
        public string NickservPassword
        { get; set; }

        [XmlElement("AltNickName")]
        public string AltNickName
        { get; set; }

        [XmlElement("AwayNickName")]
        public string AwayNickName
        { get; set; }

        [XmlElement("QuitMessage")]
        public string QuitMessage
        { get { return this._quitMessage; } set { this._quitMessage = value; } }

        [XmlElement("FullName")]
        public string FullName
        { get { return this._fullName; } set { this._fullName = value; } }

        [XmlElement("IdentName")]
        public string IdentName
        { get { return this._identName; } set { this._identName = value; } }

        [XmlElement("SetModeI")]
        public bool SetModeI
        { get { return this._setModeI; } set { this._setModeI = value; } }

        [XmlElement("ShowMOTD")]
        public bool ShowMOTD
        { get; set; }

        [XmlElement("AutoStart")]
        public bool AutoStart
        { get; set; }

        [XmlElement("ShowPingPong")]
        public bool ShowPingPong
        { get { return this._showPingPong; } set { this._showPingPong = value; } }

        [XmlElement("AutoJoinDelay")]
        public bool AutoJoinDelay
        { get { return this._autoJoinDelay; } set { this._autoJoinDelay = value; } }

        [XmlArray("AutoPerform")]
        [XmlArrayItem("Item")]
        public string[] AutoPerform
        { get; set; }

        [XmlElement("AutoPerformEnable")]
        public bool AutoPerformEnable
        { get; set; }

        [XmlArray("AutoJoin")]
        [XmlArrayItem("Item", typeof(String))]
        public string[] AutoJoinChannels
        { get; set; }

        [XmlElement("AutoJoinEnable")]
        public bool AutoJoinEnable
        { get; set; }

        [XmlElement("RejoinChannels")]
        public bool RejoinChannels
        { get; set; }

        [XmlElement("Encoding")]
        public string Encoding
        { get { return this._encoding; } set { this._encoding = value; } }

        [XmlElement("DisableCTCP")]
        public bool DisableCTCP
        { get; set; }

        [XmlArray("IgnoreList")]
        [XmlArrayItem("Item")]
        public string[] IgnoreList
        { get; set; }

        [XmlElement("IgnoreListEnable")]
        public bool IgnoreListEnable
        { get; set; }

        //these are all temporary server settings, not saved to the XML file

        [XmlIgnore()]
        public string RealServerName
        { get {return this._realServerName; } set { this._realServerName = value; }  }

        //server settings not stored in the XML File
        [XmlIgnore()]
        public string ServerIP
        { get; set; }

        //the NetWork name obtained from NETWORK 005 Reply
        [XmlIgnore()]
        public string NetworkName
        { get; set; }

        //the channel modes which have parameters from CHANMODES 005 Reply
        [XmlIgnore()]
        public string ChannelModeParams
        { get; set; }

        [XmlIgnore()]
        public string ChannelModeNoParams
        { get; set; }

        //the STATUSMG 005 Reply
        [XmlIgnore()]
        public char[] StatusMSG
        { get; set; }

        //the user status modes from PREFIX 005 Reply
        [XmlIgnore()]
        public char[][] StatusModes
        { get; set; }

        //which channel prefixes are allowed
        [XmlIgnore()]
        public char[] ChannelTypes
        { get; set; }
        
        //allow the MOTD to show if /motd command is used, but ShowMOTD is disabled
        [XmlIgnore()]
        public bool ForceMOTD
        { get; set; }

        [XmlIgnore()]
        public Hashtable IAL
        { get; set; }

        //whether you are away or not
        [XmlIgnore()]
        public bool Away
        { get; set; }

        //remember your nickname before you set yourself away
        [XmlIgnore()]
        public string DefaultNick
        { get; set; }

        [XmlIgnore()]
        public DateTime AwayStart
        { get; set; }

        [XmlIgnore()]
        public DateTime ConnectedTime
        { get; set; }

        [XmlIgnore()]
        public System.Net.IPAddress LocalIP
        { get; set; }
    }
    
    public class InternalAddressList
    {
        private string _nick;
        private string _host;
        private ArrayList _channels;
        
        public InternalAddressList(string nick, string host, string channel)
        {
            _nick = nick;
            _host = host;
            _channels = new ArrayList();
            _channels.Add(channel);
        }
        
        public string Nick
        {
            get { return _nick; }
            set { _nick = value; }
        }
        
        public string Host
        {
            get { return _host; }
            set { _host = value; }
        }

        public ArrayList Channels
        {
            get { return this._channels; }
        }

        public void AddChannel(string channel)
        {
            if (_channels.IndexOf(channel) == -1)
                _channels.Add(channel);
        }
        
        public void RemoveChannel(string channel)
        {
            if (_channels.IndexOf(channel) != -1)
                _channels.Remove(channel);
        }
    }
}
