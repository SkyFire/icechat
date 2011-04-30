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
using System.IO;
using System.Xml.Serialization;

namespace IceChat
{
    [XmlRoot("IceChatColors")]
    public class IceChatColors
    {
        //default color values
        private int _channelOwnerColor = 4;
        private int _channelAdminColor = 4;
        private int _channelOpColor = 4;
        private int _channelHalfOpColor = 4;
        private int _channelVoiceColor = 9;
        private int _channelRegularColor = 1;

        private int _tabBarCurrent = 1;
        private int _tabBarNewMessage = 4;
        private int _tabBarChannelJoin = 3;
        private int _tabBarChannelPart = 7;
        private int _tabBarServerQuit = 10;
        private int _tabBarServerMessage = 13;
        private int _tabBarOtherMessage = 6;
        private int _tabBarDefault = 1;

        private int _tabBarCurrentBG1 = 29;
        private int _tabBarCurrentBG2 = 0;
        private int _tabBarOtherBG1 = 14;
        private int _tabBarOtherBG2 = 0;
        private int _tabBarHoverBG1 = 0;
        private int _tabBarHoverBG2 = 7;

        private int _panelHeaderBGColor1 = 15;
        private int _panelHeaderBGColor2 = 0;
        private int _panelHeaderForeColor = 1;

        private int _consoleBack = 0;
        private int _channelBack = 0;
        private int _queryBack = 0;
        private int _nicklistBack = 0;
        private int _serverlistBack = 0;
        private int _inputboxBack = 0;
        private int _inputboxFore = 1;
        private int _channellistBack = 0;
        private int _channellistFore = 1;
        private int _tabbarBack = 20;
        private int _menubarBack = 20;
        private int _toolbarBack = 20;
        private int _statusbarBack = 20;
        private int _statusbarFore = 1;
        private int _unreadTextMarker = 4;

        private bool _randomizeNickColors = false;
        private bool _newMessageEnabled = true;
        private bool _channelJoinEnabled = true;
        private bool _channelPartEnabled = true;
        private bool _serverQuitEnabled = true;
        private bool _serverMessageEnabled = true;
        private bool _otherMessageEnabled = true;

        [XmlElement("ConsoleBackColor")]
        public int ConsoleBackColor
        { get { return _consoleBack; }  set { _consoleBack = value; } }

        [XmlElement("ChannelBackColor")]
        public int ChannelBackColor
        { get { return _channelBack; }  set { _channelBack = value; } }

        [XmlElement("QueryBackColor")]
        public int QueryBackColor
        { get { return _queryBack; }  set { _queryBack = value; } }

        [XmlElement("NickListBackColor")]
        public int NickListBackColor
        { get { return _nicklistBack; }  set { _nicklistBack = value; } }

        [XmlElement("ServerListBackColor")]
        public int ServerListBackColor
        { get { return _serverlistBack; }  set { _serverlistBack = value; } }

        [XmlElement("ChannelListBackColor")]
        public int ChannelListBackColor
        { get { return _channellistBack; } set { _channellistBack = value; } }

        [XmlElement("ChannelListForeColor")]
        public int ChannelListForeColor
        { get { return _channellistFore; } set { _channellistFore = value; } }

        [XmlElement("InputboxBackColor")]
        public int InputboxBackColor
        { get { return _inputboxBack; } set { _inputboxBack = value; } }

        [XmlElement("InputboxForeColor")]
        public int InputboxForeColor
        { get { return _inputboxFore; } set { _inputboxFore = value; } }

        [XmlElement("TabbarBackColor")]
        public int TabbarBackColor
        { get { return _tabbarBack; } set { _tabbarBack = value; } }

        [XmlElement("MenubarBackColor")]
        public int MenubarBackColor
        { get { return _menubarBack; } set { _menubarBack = value; } }

        [XmlElement("ToolbarBackColor")]
        public int ToolbarBackColor
        { get { return _toolbarBack; } set { _toolbarBack = value; } }

        [XmlElement("StatusbarBackColor")]
        public int StatusbarBackColor
        { get { return _statusbarBack; } set { _statusbarBack = value; } }

        [XmlElement("StatusbarForeColor")]
        public int StatusbarForeColor
        { get { return _statusbarFore; } set { _statusbarFore = value; } }

        [XmlElement("ChannelOwnerColor")]
        public int ChannelOwnerColor
        { get { return _channelOwnerColor; }  set { _channelOwnerColor = value; } }

        [XmlElement("ChannelAdminColor")]
        public int ChannelAdminColor
        { get { return this._channelAdminColor; } set { this._channelAdminColor = value; } }

        [XmlElement("ChannelOpColor")]
        public int ChannelOpColor
        { get { return this._channelOpColor; } set { this._channelOpColor = value; } }

        [XmlElement("ChannelHalfOpColor")]
        public int ChannelHalfOpColor
        { get { return this._channelHalfOpColor; } set { this._channelHalfOpColor = value; } }

        [XmlElement("ChannelVoiceColor")]
        public int ChannelVoiceColor
        { get { return this._channelVoiceColor; } set { this._channelVoiceColor = value; } }

        [XmlElement("ChannelRegularColor")]
        public int ChannelRegularColor
        { get { return this._channelRegularColor; } set { this._channelRegularColor = value; } }

        [XmlElement("TabBarCurrent")]
        public int TabBarCurrent
        { get { return _tabBarCurrent; } set { _tabBarCurrent = value; } }
        
        //new messages and actions
        [XmlElement("TabBarNewMessage")]
        public int TabBarNewMessage
        { get { return _tabBarNewMessage; } set { _tabBarNewMessage = value; } }

        [XmlElement("TabBarChannelJoin")]
        public int TabBarChannelJoin
        { get { return _tabBarChannelJoin; } set { _tabBarChannelJoin = value; } }

        [XmlElement("TabBarChannelPart")]
        public int TabBarChannelPart
        { get { return _tabBarChannelPart; } set { _tabBarChannelPart = value; } }

        [XmlElement("TabBarServerQuit")]
        public int TabBarServerQuit
        { get { return _tabBarServerQuit; } set { _tabBarServerQuit = value; } }

        [XmlElement("TabBarServerMessage")]
        public int TabBarServerMessage
        { get { return _tabBarServerMessage; } set { _tabBarServerMessage = value; } }

        [XmlElement("TabBarOtherMessage")]
        public int TabBarOtherMessage
        { get { return _tabBarOtherMessage; } set { _tabBarOtherMessage = value; } }

        [XmlElement("TabBarDefault")]
        public int TabBarDefault
        { get { return _tabBarDefault; } set { _tabBarDefault = value; } }

        [XmlElement("TabBarCurrentBG1")]
        public int TabBarCurrentBG1
        { get { return _tabBarCurrentBG1; } set { _tabBarCurrentBG1= value; } }

        [XmlElement("TabBarCurrentBG2")]
        public int TabBarCurrentBG2
        { get { return _tabBarCurrentBG2; } set { _tabBarCurrentBG2 = value; } }

        [XmlElement("TabBarOtherBG1")]
        public int TabBarOtherBG1
        { get { return _tabBarOtherBG1; } set { _tabBarOtherBG1 = value; } }

        [XmlElement("TabBarOtherBG2")]
        public int TabBarOtherBG2
        { get { return _tabBarOtherBG2; } set { _tabBarOtherBG2 = value; } }

        [XmlElement("TabBarHoverBG1")]
        public int TabBarHoverBG1
        { get { return _tabBarHoverBG1; } set { _tabBarHoverBG1 = value; } }

        [XmlElement("TabBarHoverBG2")]
        public int TabBarHoverBG2
        { get { return _tabBarHoverBG2; } set { _tabBarHoverBG2 = value; } }

        [XmlElement("PanelHeaderBG1")]
        public int PanelHeaderBG1
        { get { return _panelHeaderBGColor1; } set { _panelHeaderBGColor1 = value; } }

        [XmlElement("PanelHeaderBG2")]
        public int PanelHeaderBG2
        { get { return _panelHeaderBGColor2; } set { _panelHeaderBGColor2 = value; } }

        [XmlElement("PanelHeaderForeColor")]
        public int PanelHeaderForeColor
        { get { return _panelHeaderForeColor; } set { _panelHeaderForeColor = value; } }

        [XmlElement("UnreadTextMarker")]
        public int UnreadTextMarkerColor
        { get { return this._unreadTextMarker; } set { this._unreadTextMarker = value; } }

        [XmlElement("RandomizeNickColors")]
        public bool RandomizeNickColors
        { get { return this._randomizeNickColors; } set { this._randomizeNickColors = value; } }

        [XmlElement("NewMessageColorChange")]
        public bool NewMessageColorChange
        { get { return this._newMessageEnabled; } set { this._newMessageEnabled = value; } }

        [XmlElement("ChannelJoinColorChange")]
        public bool ChannelJoinColorChange
        { get { return this._channelJoinEnabled; } set { this._channelJoinEnabled = value; } }

        [XmlElement("ChannePartColorChange")]
        public bool ChannelPartColorChange
        { get { return this._channelPartEnabled; } set { this._channelPartEnabled = value; } }

        [XmlElement("ServerQuitColorChange")]
        public bool ServerQuitColorChange
        { get { return this._serverQuitEnabled; } set { this._serverQuitEnabled = value; } }

        [XmlElement("ServerMessageColorChange")]
        public bool ServerMessageColorChange
        { get { return this._serverMessageEnabled; } set { this._serverMessageEnabled = value; } }

        [XmlElement("OtherMessageColorChange")]
        public bool OtherMessageColorChange
        { get { return this._otherMessageEnabled; } set { this._otherMessageEnabled = value; } }

    }
    
    
    [XmlRoot("IceChatOptions")]
    public class IceChatOptions
    {
        //set the default values
        private string _timeStamp = "[hh:mm.ss] ";
        private bool _saveWindow = true;
        private bool _identServer = true;
        private bool _reconnectServer = true;

        private bool _logConsole = true;
        private bool _logChannel = true;
        private bool _logQuery = true;
        private bool _seperateLogs = true;
        private string _logFormat = "Plain Text";

        private bool _showEmoticons = false;
        private bool _showEmoticonPicker = true;
        private bool _showColorPicker = true;
        private bool _showStatusBar = true;
        private bool _showServerTree = true;
        private bool _showNickList = true;
        private bool _showToolBar = true;
        private bool _showTabBar = true;
        private bool _disableQueries = false;
        private bool _showQueryForegound = true;
        private bool _whoisNewQuery = true;
        private bool _showUnreadLine = false;
        private bool _minimizeTray = false;
        private bool _isOnTray = false;

        private int _panelRightWidth = 200;
        private int _panelLeftWidth = 175;
        private int _dccChatTimeOut = 60;
        private int _dccPortLower = 5000;
        private int _dccPortUpper = 10000;
        private string _dccReceiveFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private string _dccSendFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private int _dccBufferSize = 1024;

        private string _language = "English";
        private string _identName = "IceChat09";
        private string _fullName = "The Chat Cool People Use";
        private string _quitMessage = "$randquit";

        [XmlElement("TimeStamp")]
        public string TimeStamp
        { 
            get { return this._timeStamp; }
            set { this._timeStamp = value; }
        }

        [XmlElement("SaveWindowPosition")]
        public bool SaveWindowPosition
        {
            get { return this._saveWindow; }
            set { this._saveWindow = value; }
        }

        [XmlElement("WindowSize")]
        public System.Drawing.Size WindowSize
        { get; set; }

        [XmlElement("WindowLocation")]
        public System.Drawing.Point WindowLocation
        { get; set; }

        [XmlElement("RightPanelWidth")]
        public int RightPanelWidth
        {
            get { return this._panelRightWidth; }
            set { this._panelRightWidth = value; }
        }

        [XmlElement("LeftPanelWidth")]
        public int LeftPanelWidth
        {
            get { return this._panelLeftWidth; }
            set { this._panelLeftWidth = value; }
        }

        [XmlElement("DefaultNick")]
        public string DefaultNick
        { get; set; }

        [XmlElement("DefaultIdent")]
        public string DefaultIdent
        {
            get { return this._identName; }
            set { this._identName = value; }
        }

        [XmlElement("DefaultFullName")]
        public string DefaultFullName
        {
            get { return this._fullName; }
            set { this._fullName = value; }
        }

        [XmlElement("DefaultQuitMessage")]
        public string DefaultQuitMessage
        {
            get { return this._quitMessage; }
            set { this._quitMessage = value; }
        }

        [XmlElement("IdentServer")]
        public bool IdentServer
        {
            get { return this._identServer; }
            set { this._identServer = value; }
        }

        [XmlElement("ReconnectServer")]
        public bool ReconnectServer
        {
            get { return this._reconnectServer; }
            set { this._reconnectServer = value; }
        }

        [XmlElement("LogConsole")]
        public bool LogConsole
        {
            get { return this._logConsole; }
            set { this._logConsole = value; }
        }

        [XmlElement("LogChannel")]
        public bool LogChannel
        {
            get { return this._logChannel; }
            set { this._logChannel = value; }
        }

        [XmlElement("LogQuery")]
        public bool LogQuery
        {
            get { return this._logQuery; }
            set { this._logQuery = value; }
        }

        [XmlElement("SeperateLogs")]
        public bool SeperateLogs
        {
            get { return this._seperateLogs; }
            set { this._seperateLogs = value; }
        }

        [XmlElement("LogFormat")]
        public string LogFormat
        {
            get { return this._logFormat; }
            set { this._logFormat = value; }
        }

        [XmlElement("ShowEmoticons")]
        public bool ShowEmoticons
        {
            get { return this._showEmoticons; }
            set { this._showEmoticons = value; }
        }


        [XmlElement("ShowEmoticonPicker")]
        public bool ShowEmoticonPicker
        {
            get { return this._showEmoticonPicker; }
            set { this._showEmoticonPicker = value; }
        }

        [XmlElement("ShowColorPicker")]
        public bool ShowColorPicker
        {
            get { return this._showColorPicker; }
            set { this._showColorPicker = value; }
        }
        
        [XmlElement("ShowStatusBar")]
        public bool ShowStatusBar
        {
            get { return this._showStatusBar; }
            set { this._showStatusBar = value; }
        }

        [XmlElement("ShowToolBar")]
        public bool ShowToolBar
        {
            get { return this._showToolBar; }
            set { this._showToolBar = value; }
        }

        [XmlElement("ShowTabBar")]
        public bool ShowTabBar
        {
            get { return this._showTabBar; }
            set { this._showTabBar = value; }
        }

        [XmlElement("ShowServerTree")]
        public bool ShowServerTree
        {
            get { return this._showServerTree; }
            set { this._showServerTree = value; }
        }

        [XmlElement("ShowNickList")]
        public bool ShowNickList
        {
            get { return this._showNickList; }
            set { this._showNickList = value; }
        }

        [XmlElement("DisableQueries")]
        public bool DisableQueries
        {
            get { return this._disableQueries; }
            set { this._disableQueries = value; }
        }

        [XmlElement("NewQueryForegound")]
        public bool NewQueryForegound
        {
            get { return this._showQueryForegound; }
            set { this._showQueryForegound = value; }
        }

        [XmlElement("WhoisNewQuery")]
        public bool WhoisNewQuery
        {
            get { return this._whoisNewQuery; }
            set { this._whoisNewQuery = value; }
        }

        [XmlElement("ShowUnreadLine")]
        public bool ShowUnreadLine
        {
            get { return this._showUnreadLine; }
            set { this._showUnreadLine = value; }
        }

        [XmlElement("MinimizeToTray")]
        public bool MinimizeToTray
        {
            get { return this._minimizeTray; }
            set { this._minimizeTray = value; }
        }

        [XmlElement("IsOnTray")]
        public bool IsOnTray
        {
            get { return this._isOnTray; }
            set { this._isOnTray = value; }
        }

        [XmlElement("Language")]
        public string Language
        {
            get { return this._language; }
            set { this._language = value; }
        }

        [XmlElement("DCCChatAutoAccept")]
        public bool DCCChatAutoAccept
        { get; set; }

        [XmlElement("DCCFileAutoAccept")]
        public bool DCCFileAutoAccept
        { get; set; }

        [XmlElement("DCCChatIgnore")]
        public bool DCCChatIgnore
        { get; set; }

        [XmlElement("DCCFileIgnore")]
        public bool DCCFileIgnore
        { get; set; }

        [XmlElement("DCCChatTimeOut")]
        public int DCCChatTimeOut
        {
            get { return this._dccChatTimeOut; }
            set { this._dccChatTimeOut = value; }
        }

        [XmlElement("DCCPortLower")]
        public int DCCPortLower
        {
            get { return this._dccPortLower; }
            set { this._dccPortLower = value; }
        }

        [XmlElement("DCCPortUpper")]
        public int DCCPortUpper
        {
            get { return this._dccPortUpper; }
            set { this._dccPortUpper = value; }
        }

        [XmlElement("DCCReceiveFolder")]
        public string DCCReceiveFolder
        {
            get { return this._dccReceiveFolder; }
            set { this._dccReceiveFolder = value; }
        }

        [XmlElement("DCCSendFolder")]
        public string DCCSendFolder
        {
            get { return this._dccSendFolder; }
            set { this._dccSendFolder = value; }
        }

        [XmlElement("DCCLocalIP")]
        public string DCCLocalIP
        { get; set; }

        [XmlElement("DCCBufferSize")]
        public int DCCBufferSize
        {
            get { return this._dccBufferSize; }
            set { this._dccBufferSize = value; }
        }

        [XmlArray("ScriptFiles")]
        [XmlArrayItem("Item")]
        public string[] ScriptFiles
        { get; set; }

        [XmlElement("JoinEventLocation")]
        public int JoinEventLocation
        { get; set; }

        [XmlElement("PartEventLocation")]
        public int PartEventLocation
        { get; set; }

        [XmlElement("QuitEventLocation")]
        public int QuitEventLocation
        { get; set; }

        [XmlElement("ModeEventLocation")]
        public int ModeEventLocation
        { get; set; }

        [XmlElement("TopicEventLocation")]
        public int TopicEventLocation
        { get; set; }

        [XmlElement("KickEventLocation")]
        public int KickEventLocation
        { get; set; }

        [XmlElement("ChannelMessageEventLocation")]
        public int ChannelMessageEventLocation
        { get; set; }

        [XmlElement("ChannelActionEventLocation")]
        public int ChannelActionEventLocation
        { get; set; }

        [XmlElement("NickChangeEventLocation")]
        public int NickChangeEventLocation
        { get; set; }

        [XmlElement("ServerNoticeEventLocation")]
        public int ServerNoticeEventLocation
        { get; set; }

        [XmlElement("UserNoticeEventLocation")]
        public int UserNoticeEventLocation
        { get; set; }

        [XmlElement("WhoisEventLocation")]
        public int WhoisEventLocation
        { get; set; }

        [XmlElement("CtcpEventLocation")]
        public int CtcpEventLocation
        { get; set; }

        [XmlElement("ServerErrorEventLocation")]
        public int ServerErrorEventLocation
        { get; set; }

        [XmlElement("BuddyEventLocation")]
        public int BuddyEventLocation
        { get; set; }

        [XmlElement("DccEventLocation")]
        public int DccEventLocation
        { get; set; }

        [XmlElement("CurrentTheme")]
        public string CurrentTheme
        { get; set; }

        [XmlArray("Themes")]
        [XmlArrayItem("Item")]
        public string[] Themes
        { get; set; }
    
    }
    
    [XmlRoot("IceChatMessageFormat")]
    public class IceChatMessageFormat
    {
        [XmlArray("MessageSettings")]
        [XmlArrayItem("Item", typeof(ServerMessageFormatItem))]
        public ServerMessageFormatItem[] MessageSettings
        { get; set; }

    }

    public class ServerMessageFormatItem
    {
        public string MessageName;
        public string FormattedMessage;
    }

    [XmlRoot("IceChatFonts")]
    public class IceChatFontSetting
    {
        [XmlArray("FontSettings")]
        [XmlArrayItem("Item", typeof(FontSettingItem))]
        public FontSettingItem[] FontSettings
        { get; set; }    
    }

    public class FontSettingItem
    {
        public string WindowType;
        public string FontName;
        public float FontSize;

    }

    //seperate file(s) for all the aliases
    public class IceChatAliases
    {
        [XmlArray("Aliases")]
        [XmlArrayItem("Item", typeof(AliasItem))]
        public ArrayList listAliases;

        public IceChatAliases()
        {
            listAliases = new ArrayList();
        }
        public void AddAlias(AliasItem alias)
        {
            listAliases.Add(alias);
        }
    }

    public class AliasItem
    {
        [XmlElement("AliasName")]
        public string AliasName
        { get; set; }

        [XmlArray("Command")]
        [XmlArrayItem("Item")]
        public string[] Command
        { get; set; }        
    }

    public class IceChatEmoticon
    {
        [XmlArray("Emoticons")]
        [XmlArrayItem("Item", typeof(EmoticonItem))]
        public ArrayList listEmoticons;
        public IceChatEmoticon()
        {
            listEmoticons = new ArrayList();
        }
        public void AddEmoticon(EmoticonItem item)
        {
            listEmoticons.Add(item);
        }
    }

    public class EmoticonItem
    {
        [XmlAttribute("ID")]
        public int ID
        { get; set; }
        
        [XmlElement("EmoticonImage")]
        public string EmoticonImage
        { get; set; }

        [XmlElement("Trigger")]
        public string Trigger
        { get; set; }        

    }

    public class IceChatPopupMenus
    {
        [XmlArray("Popups")]
        [XmlArrayItem("Item",typeof(PopupMenuItem))]
        public ArrayList listPopups;

        public IceChatPopupMenus() 
        {
            listPopups = new ArrayList();
        }
        
        public void ReplacePopup(string popupType, PopupMenuItem menu)
        {
            foreach (PopupMenuItem p in listPopups)
            {
                if (p.PopupType == popupType)
                {
                    listPopups.Remove(p);
                    break;
                }
            }
            listPopups.Add(menu);
        }

        public void AddPopup(PopupMenuItem menu) 
        {
            listPopups.Add(menu);
        }

    }
    public class PopupMenuItem
    {
        [XmlAttribute("PopupType")]
        public string PopupType
        { get; set; }

        [XmlArray("Menu")]
        [XmlArrayItem("Item")]
        public string[] Menu
        { get; set; }        

    }

    [XmlRoot("IceChatSounds")]
    public class IceChatSounds
    {
        public class soundEntry
        {
            public soundEntry() { }
            public soundEntry(string k, string d)
            {
                key = k; desc = d;
            }

            public soundEntry(string k, string d, string f)
            {
                key=k; desc=d; file=f;
            }

            public soundEntry(string k, string d, soundEntry p)
            {
                key = k; desc = d; parent = p;
            }

            private string key;
            private string desc;
            private string file;
            soundEntry parent;

            [XmlElement("Key")]
            public string Key
            {
                get { return key; }
                set
                {                    
                    if (key != null)
                        throw new InvalidOperationException("Cannot modify a key after it was assigned a value.");
                    key = value;
                }
            }
            [XmlElement("Description")]
            public string Description
            {
                get { return desc; }
                set { desc = value; }
            }
            [XmlElement("File")]
            public string File
            {
                get { return file; }
                set { file = value; }
            }

            public soundEntry Parent
            {
                get {return parent;}
                set { parent = value; }
            }

            public string getSoundFile()
            {
                if (file!=null && file.Length>0) return file;
                if (parent!=null) return parent.getSoundFile();
                return null;
            }

        }

        [XmlArray("soundList")]
        [XmlArrayItem("soundEntry", typeof(soundEntry))]
        public ArrayList soundList;

        public IceChatSounds()
        {
            soundList = new ArrayList();
        }

        public void AddDefaultSounds()
        {
            Add(new soundEntry("conmsg", "New Message in Console"));
            Add(new soundEntry("chanmsg", "New Channel Message"));
            Add(new soundEntry("privmsg", "New Private Message"));
            Add(new soundEntry("notice", "New User Notice"));
            Add(new soundEntry("nickchan", "Nickname said in channel"));
            Add(new soundEntry("nickpriv", "Nickname said in private message"));
            Add(new soundEntry("buddy", "A buddy has come online"));
            Add(new soundEntry("dropped", "Server Disconnection"));
            Add(new soundEntry("operping", "Request for operator"));
        }

        public void Add(soundEntry s)
        {
            foreach (soundEntry x in soundList)
            {
                if (x.Key.Equals(s.Key))
                {
                    x.Parent = s.Parent;
                    return;
                }
            }
            soundList.Add(s);
        }

        public soundEntry getSound(string key)
        {
            foreach (soundEntry x in soundList)
            {
                if (x.Key.Equals(key))
                {
                    return x;
                }
            }
            return null;
        }

        public soundEntry getSound(int index)
        {
            return (soundEntry)soundList[index];
        }

    }
}
