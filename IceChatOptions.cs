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

        private int _consoleBack = 0;
        private int _channelBack = 0;
        private int _queryBack = 0;
        private int _nicklistBack = 0;
        private int _serverlistBack = 0;

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

        [XmlElement("ChannelOwnerColor")]
        public int ChannelOwnerColor
        { get { return _channelOwnerColor; }  set { _channelOwnerColor = value; } }

        [XmlElement("ChannelAdminColor")]
        public int ChannelAdminColor
        { 
          get { return this._channelAdminColor; }
          set { this._channelAdminColor = value; } 
        }

        [XmlElement("ChannelOpColor")]
        public int ChannelOpColor
        {
            get { return this._channelOpColor; }
            set { this._channelOpColor = value; }
        }

        [XmlElement("ChannelHalfOpColor")]
        public int ChannelHalfOpColor
        {
            get { return this._channelHalfOpColor; }
            set { this._channelHalfOpColor = value; }
        }

        [XmlElement("ChannelVoiceColor")]
        public int ChannelVoiceColor
        {
            get { return this._channelVoiceColor; }
            set { this._channelVoiceColor = value; }
        }

        [XmlElement("ChannelRegularColor")]
        public int ChannelRegularColor
        {
            get { return this._channelRegularColor; }
            set { this._channelRegularColor = value; }
        }


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

        private bool _showEmoticons = false;
        private bool _showEmoticonPicker = true;
        private bool _showColorPicker = true;
        private bool _showStatusBar = true;
        private bool _disableQueries = false;
        private bool _showQueryForegound = true;
        private bool _whoisNewQuery = true;

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

        [XmlElement("DefaultNick")]
        public string DefaultNick
        { get; set; }

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

}
