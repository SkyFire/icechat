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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;

namespace IceChat
{
    public partial class FormFirstRun : Form
    {
        private string[] Default_Servers = new string[] { "irc.quakenet.org", "irc.undernet.org", "irc.freenode.net", "ice.coldfront.net", "irc.ice-irc.net", "irc.evolution-z.org", "irc.koach.com", "irc.0v1.org", "irc.virtualife.com.br" };
        private string[] Default_Aliases = new string[] { "/op /mode # +o $1", "/deop /mode # -o $1", "/voice /mode # +v $1", "/devoice /mode # -v $1", "/b /ban # $1", "/j /join $1 $2-", "/n /names $1", "/w /whois $1 $1", "/k /kick # $1 $2-", "/q /query $1", "/v /version $1", "/about //say Operating System  [$os Build No. $osbuild]  - Uptime [$uptime]  - $icechat" };

        private string[] Nicklist_Popup = new string[] { "Information", ".Display User Info:/userinfo $nick", ".Whois user:/whois $nick $nick", ".DNS User:/dns $nick", "Commands", ".Invite user:/invite $nick #$$?=\"Enter Channel Name\"", ".Notice user:notice $nick $$?=\"Enter Message\"", ".Query user:/query $nick", "Op Commands", ".Voice user:/mode # +v $nick", ".DeVoice user:/mode # -v $nick", ".Op user:/mode # +o $nick", ".Deop user:mode # -o $nick", ".Kick:kick # $nick $$?=\"Enter Kick Reason\"", ".Ban Kick NoReason:/ban # $mask($host,2) | /kick # $nick bye", ".Ban Kick:ban # $mask($host,2) | kick # $nick $$?=\"Enter Kick Reason\"", ".Ban:/ban # $mask($host,2)", "CTCP", ".Ping:/ping $nick", ".Version:/version $nick", "DCC", ".Send:/dcc send $nick", ".Chat:/dcc chat $nick", "Slaps", ".Brick:/me slaps $nick with a big red brick", ".Trout:/me slaps $nick with a &#x3;4r&#x3;8a&#x3;9i&#x3;11n&#x3;13b&#x3;17o&#x3;26w trout" };
        private string[] Channel_Popup = new string[] { "Information", ".Channel Info:/chaninfo" };
        private string[] Console_Popup = new string[] { "Server Commands", ".Server Links Here:/links", ".Message of the Day:/motd", "AutoPerform:/autoperform", "Autojoin:/autojoin" };
        private string[] Query_Popup = new string[] { "Info:/userinfo $1", "Whois:/whois $nick", "-", "Ignore:/ignore $1 1 | /closemsg $1", "-", ".Ping:/ctcp $1 ping", ".Time:/ctcp $1 time", ".Version:/ctcp $1 version", "DCC", ".Send:/dcc send $1", ".Chat:/dcc chat $1" };
        private string[] Buddy_Popup = new string[] { "Query:/query $1", "Whois:/whois $1" };


        private int _currentStep;

        private string _nickName;
        private string _currentFolder;

        public FormFirstRun(string currentFolder)
        {
            InitializeComponent();

            //this.FormClosing += new FormClosingEventHandler(FormFirstRun_FormClosing);

            _nickName = "Default";
            _currentFolder = currentFolder;

            foreach (string s in Default_Servers)
            {
                comboData.Items.Add(s);
            }

            comboData.Text = "irc.quakenet.org";

            CurrentStep = 0;
        }

        private void FormFirstRun_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(_currentStep);
            if (e.CloseReason == CloseReason.UserClosing && _currentStep != 3)
                e.Cancel = true;
        }
        
        private int CurrentStep
        {
            get { return _currentStep; }
            set
            {
                _currentStep = value;
                
                switch (value)
                {
                    case 0:
                        textData.Text = _nickName;
                        labelHeader.Text = "Nick Name";
                        labelDesc.Text = "Enter a nickname in the field below.\r\nA nickname (or handle) is what you will be known as on IRC. It's similar to your real life name except on IRC, your nickname will be unique. You will be the only person on the network with your chosen nickname. Usually nicknames are limited to 9 characters in length.";
                        labelTip.Text = "Tip: You can change your nickname while connected by typing'/nick NewNick'";
                        labelField.Text = "Nickname:";
                        
                        comboData.Visible = false;
                        textData.Visible = true;
                        buttonBack.Visible = false;
                        buttonNext.Visible = true;

                        break;

                    case 1:
                        _nickName = textData.Text;
                        labelHeader.Text = "Server Name";
                        labelDesc.Text = "Enter a server in the field below.\r\nIRC is made up of several things called 'servers'. You can think of a server like a building which contains people and rooms (channels). There are hundreds of servers which you can connect to on IRC. Some servers make up single networks. These could be thought of as several buildings all joined together to make one big one.";
                        labelTip.Text = "Tip: You can use the server editor located at the bottom right or type '/server Address' to connect";
                        labelField.Text = "Server Address:";

                        textData.Visible = false;
                        comboData.Visible = true;
                        buttonBack.Visible = true;
                        buttonNext.Visible = true;

                        break;

                    case 2:
                        labelHeader.Text = "Done";
                        labelDesc.Text = "Your information has been saved. Simply select a server from the Favorite Server List, and click the 'Connect' button.";
                        labelTip.Text = "Tip: Use the '?' in the bottom left corner beside the Input Box for all your basic needs";
                        labelField.Text = "";

                        textData.Visible = false;
                        comboData.Visible = false;
                        buttonBack.Visible = true;
                        buttonNext.Visible = true;

                        break;

                    case 3:
                        //save the information
                        MakeDefaultFiles();

                        this.Close();
                        break;
                }
                
            }
            
        }
        
        private void MakeDefaultFiles()
        {
            //make the server file
            string serversFile = _currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatServer.xml";

            IceChatServers servers = new IceChatServers();

            int ID = 1;

            _nickName = _nickName.Replace(" ", "");
            _nickName = _nickName.Replace("#", "");
            _nickName = _nickName.Replace(",", "");
            _nickName = _nickName.Replace("`", "");

            FormMain.Instance.IceChatOptions.DefaultNick = _nickName;

            if (comboData.Text.Length > 0)
            {
                ServerSetting s = new ServerSetting();
                s.ID = ID;
                s.ServerName = comboData.Text;
                s.NickName = _nickName;
                s.AltNickName = _nickName + "_";
                s.ServerPort = "6667";
                s.SetModeI = true;

                if (comboData.Text.ToLower() == "irc.quakenet.org")
                {
                    s.AutoJoinChannels = new string[] { "#icechat2009" };
                    s.AutoJoinEnable = true;
                }

                ID++;

                servers.AddServer(s);
            }
            
            foreach (string server in comboData.Items)
            {
                if (server != comboData.Text && server.Length > 0)
                {
                    ServerSetting ss = new ServerSetting();
                    ss.ID = ID;
                    ss.ServerName = server;
                    ss.NickName = _nickName;
                    ss.AltNickName = _nickName + "_";
                    ss.ServerPort = "6667";
                    ss.SetModeI = true;

                    ID++;

                    servers.AddServer(ss);
                }
            }

            XmlSerializer serializer = new XmlSerializer(typeof(IceChatServers));
            TextWriter textWriter = new StreamWriter(FormMain.Instance.ServersFile);
            serializer.Serialize(textWriter, servers);
            textWriter.Close();
            textWriter.Dispose();


            //make the default aliases file
            string aliasesFile = _currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatAliases.xml";
            IceChatAliases aliasList = new IceChatAliases();
            
            foreach (string alias in Default_Aliases)
            {
                AliasItem a = new AliasItem();
                string name = alias.Substring(0,alias.IndexOf(" ")).Trim();
                string command = alias.Substring(alias.IndexOf(" ") + 1);
                a.AliasName = name;
                a.Command = new String[] { command };

                aliasList.AddAlias(a);
            }

            XmlSerializer serializerA = new XmlSerializer(typeof(IceChatAliases));
            TextWriter textWriterA = new StreamWriter(aliasesFile);
            serializerA.Serialize(textWriterA, aliasList);
            textWriterA.Close();
            textWriterA.Dispose();

            
            //make the default popups file
            string popupsFile = _currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatPopups.xml";
            IceChatPopupMenus popupList = new IceChatPopupMenus();

            popupList.AddPopup(newPopupMenu("NickList", Nicklist_Popup));
            popupList.AddPopup(newPopupMenu("Console", Console_Popup));
            popupList.AddPopup(newPopupMenu("Channel", Channel_Popup));
            popupList.AddPopup(newPopupMenu("Query", Query_Popup));

            XmlSerializer serializerP = new XmlSerializer(typeof(IceChatPopupMenus));
            TextWriter textWriterP = new StreamWriter(popupsFile);
            serializerP.Serialize(textWriterP, popupList);
            textWriterP.Close();
            textWriterP.Dispose();


        }

        private PopupMenuItem newPopupMenu(string type, string[] menu)
        {
            PopupMenuItem p = new PopupMenuItem();
            p.PopupType = type;
            p.Menu = menu;
            return p;
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            //check if a nickname has been set
            if (_currentStep == 0)
            {
                if (textData.Text == "Default")
                {
                    MessageBox.Show("Please Choose a Default Nick Name");
                    return;
                }
            }
            CurrentStep++;
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            CurrentStep--;
        }
    }
}
