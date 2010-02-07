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
using System.Text.RegularExpressions;
using System.Windows.Forms;
using IceChatPlugin;


namespace IceChat
{
    public partial class FormColors : Form
    {
        public delegate void SaveColorsDelegate();        
        public event SaveColorsDelegate SaveColors;

        private ColorButtonArray colorPicker;

        private Hashtable messageIdentifiers;

        private IceChatMessageFormat iceChatMessages;
        private IceChatColors iceChatColors;

        private object currentColorPick;
        
        private const char newColorChar = '\xFF03';

        public FormColors(IceChatMessageFormat MessageFormat, IceChatColors IceChatColors)
        {
            InitializeComponent();

            this.textFormattedBasic = new TextWindow();
            this.textFormattedText = new TextWindow();



            // 
            // textFormattedBasic
            // 
            this.textFormattedBasic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textFormattedBasic.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textFormattedBasic.IRCBackColor = 0;
            this.textFormattedBasic.IRCForeColor = 0;
            this.textFormattedBasic.Location = new System.Drawing.Point(16, 316);
            this.textFormattedBasic.Name = "textFormattedBasic";
            this.textFormattedBasic.NoColorMode = false;
            this.textFormattedBasic.ShowTimeStamp = true;
            this.textFormattedBasic.SingleLine = true;
            this.textFormattedBasic.Size = new System.Drawing.Size(607, 26);
            this.textFormattedBasic.TabIndex = 48;
            // 
            // textFormattedText
            // 
            this.textFormattedText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textFormattedText.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textFormattedText.IRCBackColor = 0;
            this.textFormattedText.IRCForeColor = 0;
            this.textFormattedText.Location = new System.Drawing.Point(13, 314);
            this.textFormattedText.Name = "textFormattedText";
            this.textFormattedText.NoColorMode = false;
            this.textFormattedText.ShowTimeStamp = true;
            this.textFormattedText.SingleLine = true;
            this.textFormattedText.Size = new System.Drawing.Size(607, 26);
            this.textFormattedText.TabIndex = 46;

            this.tabBasic.Controls.Add(this.textFormattedBasic);
            this.tabAdvanced.Controls.Add(this.textFormattedText);


            //add the events for the Tab Bar Color Picker
            this.pictureTabCurrent.Click += new EventHandler(OnTabBarColor_Click);
            this.pictureTabMessage.Click += new EventHandler(OnTabBarColor_Click);
            this.pictureTabJoin.Click += new EventHandler(OnTabBarColor_Click);
            this.pictureTabPart.Click += new EventHandler(OnTabBarColor_Click);
            this.pictureTabQuit.Click += new EventHandler(OnTabBarColor_Click);
            this.pictureTabServer.Click += new EventHandler(OnTabBarColor_Click);
            this.pictureTabOther.Click += new EventHandler(OnTabBarColor_Click);
            this.pictureTabDefault.Click += new EventHandler(OnTabBarColor_Click);

            //add the events for the Nick Color Picker
            this.pictureAdmin.Click += new EventHandler(OnNickColor_Click);
            this.pictureOwner.Click += new EventHandler(OnNickColor_Click);
            this.pictureOperator.Click += new EventHandler(OnNickColor_Click);
            this.pictureHalfOperator.Click += new EventHandler(OnNickColor_Click);
            this.pictureVoice.Click += new EventHandler(OnNickColor_Click);
            this.pictureDefault.Click += new EventHandler(OnNickColor_Click);

            this.pictureConsole.Click += new EventHandler(OnBackColor_Click);
            this.pictureChannel.Click += new EventHandler(OnBackColor_Click);
            this.pictureQuery.Click += new EventHandler(OnBackColor_Click);
            this.pictureNickList.Click += new EventHandler(OnBackColor_Click);
            this.pictureServerList.Click += new EventHandler(OnBackColor_Click);
            this.pictureTabBarCurrent1.Click += new EventHandler(OnBackColor_Click);
            this.pictureTabBarCurrent2.Click += new EventHandler(OnBackColor_Click);
            this.pictureTabBarOther1.Click += new EventHandler(OnBackColor_Click);
            this.pictureTabBarOther2.Click += new EventHandler(OnBackColor_Click);

            this.pictureTabBarHover1.Click += new EventHandler(OnTabBarHover1_Click);
            this.pictureTabBarHover2.Click += new EventHandler(OnTabBarHover2_Click);

            this.picturePanelHeaderBG1.Click += new EventHandler(OnPanelHeaderBG1_Click);
            this.picturePanelHeaderBG2.Click += new EventHandler(OnPanelHeaderBG2_Click);
            this.picturePanelHeaderForeColor.Click += new EventHandler(OnPanelHeaderForeColor_Click);
            
            //this.textFormattedText.IRCBackColor = 5;
            
            this.iceChatColors = IceChatColors;
            
            this.pictureOwner.BackColor = IrcColor.colors[iceChatColors.ChannelOwnerColor];
            this.pictureOwner.Tag = iceChatColors.ChannelOwnerColor;

            this.pictureAdmin.BackColor = IrcColor.colors[iceChatColors.ChannelAdminColor];
            this.pictureAdmin.Tag = iceChatColors.ChannelAdminColor;
            
            this.pictureOperator.BackColor = IrcColor.colors[iceChatColors.ChannelOpColor];
            this.pictureOperator.Tag = iceChatColors.ChannelOpColor;
            
            this.pictureHalfOperator.BackColor = IrcColor.colors[iceChatColors.ChannelHalfOpColor];
            this.pictureHalfOperator.Tag = iceChatColors.ChannelHalfOpColor;
            
            this.pictureVoice.BackColor = IrcColor.colors[iceChatColors.ChannelVoiceColor];
            this.pictureVoice.Tag = iceChatColors.ChannelVoiceColor;
            
            this.pictureDefault.BackColor = IrcColor.colors[iceChatColors.ChannelRegularColor];
            this.pictureDefault.Tag = iceChatColors.ChannelRegularColor;

            this.pictureTabCurrent.BackColor = IrcColor.colors[iceChatColors.TabBarCurrent];
            this.pictureTabCurrent.Tag = iceChatColors.TabBarCurrent;

            this.pictureTabMessage.BackColor = IrcColor.colors[iceChatColors.TabBarNewMessage];
            this.pictureTabMessage.Tag = iceChatColors.TabBarNewMessage;

            this.pictureTabJoin.BackColor = IrcColor.colors[iceChatColors.TabBarChannelJoin];
            this.pictureTabJoin.Tag = iceChatColors.TabBarChannelJoin;

            this.pictureTabPart.BackColor = IrcColor.colors[iceChatColors.TabBarChannelPart];
            this.pictureTabPart.Tag = iceChatColors.TabBarChannelPart;

            this.pictureTabQuit.BackColor = IrcColor.colors[iceChatColors.TabBarServerQuit];
            this.pictureTabQuit.Tag = iceChatColors.TabBarServerQuit;

            this.pictureTabServer.BackColor = IrcColor.colors[iceChatColors.TabBarServerMessage];
            this.pictureTabServer.Tag = iceChatColors.TabBarServerMessage;

            this.pictureTabOther.BackColor = IrcColor.colors[iceChatColors.TabBarOtherMessage];
            this.pictureTabOther.Tag = iceChatColors.TabBarOtherMessage;

            this.pictureTabDefault.BackColor = IrcColor.colors[iceChatColors.TabBarDefault];
            this.pictureTabDefault.Tag = iceChatColors.TabBarDefault;

            this.pictureTabBarCurrent1.BackColor = IrcColor.colors[iceChatColors.TabBarCurrentBG1];
            this.pictureTabBarCurrent1.Tag = iceChatColors.TabBarCurrentBG1;
            
            this.pictureTabBarCurrent2.BackColor = IrcColor.colors[iceChatColors.TabBarCurrentBG2];
            this.pictureTabBarCurrent2.Tag = iceChatColors.TabBarCurrentBG2;
            
            this.pictureTabBarOther1.BackColor = IrcColor.colors[iceChatColors.TabBarOtherBG1];
            this.pictureTabBarOther1.Tag = iceChatColors.TabBarOtherBG1;
            
            this.pictureTabBarOther2.BackColor = IrcColor.colors[iceChatColors.TabBarOtherBG2];
            this.pictureTabBarOther2.Tag = iceChatColors.TabBarOtherBG2;

            this.pictureTabBarHover1.BackColor = IrcColor.colors[iceChatColors.TabBarHoverBG1];
            this.pictureTabBarHover1.Tag = iceChatColors.TabBarHoverBG1;

            this.pictureTabBarHover2.BackColor = IrcColor.colors[iceChatColors.TabBarHoverBG2];
            this.pictureTabBarHover2.Tag = iceChatColors.TabBarHoverBG2;

            this.picturePanelHeaderBG1.BackColor = IrcColor.colors[iceChatColors.PanelHeaderBG1];
            this.picturePanelHeaderBG1.Tag = iceChatColors.PanelHeaderBG1;

            this.picturePanelHeaderBG2.BackColor = IrcColor.colors[iceChatColors.PanelHeaderBG2];
            this.picturePanelHeaderBG2.Tag = iceChatColors.PanelHeaderBG2;

            this.picturePanelHeaderForeColor.BackColor = IrcColor.colors[iceChatColors.PanelHeaderForeColor];
            this.picturePanelHeaderForeColor.Tag = iceChatColors.PanelHeaderForeColor;

            this.pictureConsole.BackColor = IrcColor.colors[iceChatColors.ConsoleBackColor];
            this.pictureConsole.Tag = iceChatColors.ConsoleBackColor;

            this.pictureChannel.BackColor = IrcColor.colors[iceChatColors.ChannelBackColor];
            this.pictureChannel.Tag = iceChatColors.ChannelBackColor;

            this.pictureQuery.BackColor = IrcColor.colors[iceChatColors.QueryBackColor];
            this.pictureQuery.Tag = iceChatColors.QueryBackColor;

            this.pictureNickList.BackColor = IrcColor.colors[iceChatColors.NickListBackColor];
            this.pictureNickList.Tag = iceChatColors.NickListBackColor;

            this.pictureServerList.BackColor = IrcColor.colors[iceChatColors.ServerListBackColor];
            this.pictureServerList.Tag = iceChatColors.ServerListBackColor;

            messageIdentifiers = new Hashtable();
            AddMessageIdentifiers();

            colorPicker = new ColorButtonArray(panelColorPicker);
            colorPicker.OnClick += new ColorButtonArray.ColorSelected(colorPicker_OnClick);
            
            treeMessages.AfterSelect += new TreeViewEventHandler(treeMessages_AfterSelect);
            textRawMessage.TextChanged+=new EventHandler(textRawMessage_TextChanged);
            textRawMessage.KeyDown += new KeyEventHandler(textRawMessage_KeyDown);
            listIdentifiers.DoubleClick += new EventHandler(listIdentifiers_DoubleClick);

            treeBasicMessages.AfterSelect += new TreeViewEventHandler(treeBasicMessages_AfterSelect);

            tabMessages.SelectedTab = tabBasic;

            iceChatMessages = MessageFormat;

            textFormattedText.SingleLine = true;
            textFormattedText.NoEmoticons = true;

            //populate Message Settings            
            if (iceChatMessages.MessageSettings != null)
            {
                foreach (ServerMessageFormatItem msg in iceChatMessages.MessageSettings)
                {
                    if (msg.MessageName.StartsWith("Channel"))
                    {
                        TreeNode t = new TreeNode(msg.MessageName);
                        t.Tag = msg.FormattedMessage;                        
                        treeMessages.Nodes[0].Nodes.Add(t);

                        TreeNode t2 = new TreeNode(msg.MessageName);
                        t2.Tag = msg.FormattedMessage;                        
                        treeBasicMessages.Nodes[0].Nodes.Add(t2);
                    }
                    else if (msg.MessageName.StartsWith("Server"))
                    {
                        TreeNode t = new TreeNode(msg.MessageName);
                        t.Tag = msg.FormattedMessage;
                        treeMessages.Nodes[1].Nodes.Add(t);

                        TreeNode t2 = new TreeNode(msg.MessageName);
                        t2.Tag = msg.FormattedMessage;
                        treeBasicMessages.Nodes[1].Nodes.Add(t2);
                    }
                    else if (msg.MessageName.StartsWith("Private"))
                    {
                        TreeNode t = new TreeNode(msg.MessageName);
                        t.Tag = msg.FormattedMessage;
                        treeMessages.Nodes[2].Nodes.Add(t);
                        
                        TreeNode t2 = new TreeNode(msg.MessageName);
                        t2.Tag = msg.FormattedMessage;
                        treeBasicMessages.Nodes[2].Nodes.Add(t2);

                    }
                    else if (msg.MessageName.StartsWith("Self"))
                    {
                        TreeNode t = new TreeNode(msg.MessageName);
                        t.Tag = msg.FormattedMessage;
                        treeMessages.Nodes[3].Nodes.Add(t);

                        TreeNode t2 = new TreeNode(msg.MessageName);
                        t2.Tag = msg.FormattedMessage;
                        treeBasicMessages.Nodes[3].Nodes.Add(t2);
                    }
                    else if (msg.MessageName.StartsWith("Ctcp"))
                    {
                        TreeNode t = new TreeNode(msg.MessageName);
                        t.Tag = msg.FormattedMessage;
                        treeMessages.Nodes[4].Nodes.Add(t);

                        TreeNode t2 = new TreeNode(msg.MessageName);
                        t2.Tag = msg.FormattedMessage;
                        treeBasicMessages.Nodes[4].Nodes.Add(t2);
                    }
                    else if (msg.MessageName.StartsWith("DCC"))
                    {
                        TreeNode t = new TreeNode(msg.MessageName);
                        t.Tag = msg.FormattedMessage;
                        treeMessages.Nodes[5].Nodes.Add(t);

                        TreeNode t2 = new TreeNode(msg.MessageName);
                        t2.Tag = msg.FormattedMessage;
                        treeBasicMessages.Nodes[5].Nodes.Add(t2);
                    }
                    else
                    {
                        TreeNode t = new TreeNode(msg.MessageName);
                        t.Tag = msg.FormattedMessage;
                        treeMessages.Nodes[6].Nodes.Add(t);

                        TreeNode t2 = new TreeNode(msg.MessageName);
                        t2.Tag = msg.FormattedMessage;
                        treeBasicMessages.Nodes[6].Nodes.Add(t2);
                    }
                }
            }

            treeMessages.ExpandAll();
            treeBasicMessages.ExpandAll();    

            //load any plugin addons
            foreach (IPluginIceChat ipc in FormMain.Instance.IceChatPlugins)
            {
                ipc.LoadColorsForm(this.tabControlColors);
            }

        }


        private void OnBackColor_Click(object sender, EventArgs e)
        {
            currentColorPick = sender;
            colorPicker.SelectedColor = (int)((PictureBox)sender).Tag;
            labelCurrent.Text = "Current Selected:\r\n" + GetLabelText((PictureBox)sender);            
        }

        private void OnTabBarColor_Click(object sender, EventArgs e)
        {
            currentColorPick = sender;
            colorPicker.SelectedColor = (int)((PictureBox)sender).Tag;
            labelCurrent.Text = "Current Selected:\r\n" + GetLabelText((PictureBox)sender);            
        }
        
        private void OnTabBarHover2_Click(object sender, EventArgs e)
        {
            currentColorPick = sender;
            colorPicker.SelectedColor = (int)((PictureBox)sender).Tag;
            labelCurrent.Text = "Current Selected:\r\n" + GetLabelText((PictureBox)sender);
        }

        private void OnTabBarHover1_Click(object sender, EventArgs e)
        {
            currentColorPick = sender;
            colorPicker.SelectedColor = (int)((PictureBox)sender).Tag;
            labelCurrent.Text = "Current Selected:\r\n" + GetLabelText((PictureBox)sender);
        }

        private void OnPanelHeaderBG1_Click(object sender, EventArgs e)
        {
            currentColorPick = sender;
            colorPicker.SelectedColor = (int)((PictureBox)sender).Tag;
            labelCurrent.Text = "Current Selected:\r\n" + GetLabelText((PictureBox)sender);
        }

        private void OnPanelHeaderBG2_Click(object sender, EventArgs e)
        {
            currentColorPick = sender;
            colorPicker.SelectedColor = (int)((PictureBox)sender).Tag;
            labelCurrent.Text = "Current Selected:\r\n" + GetLabelText((PictureBox)sender);
        }

        private void OnPanelHeaderForeColor_Click(object sender, EventArgs e)
        {
            currentColorPick = sender;
            colorPicker.SelectedColor = (int)((PictureBox)sender).Tag;
            labelCurrent.Text = "Current Selected:\r\n" + GetLabelText((PictureBox)sender);
        }

        private void OnNickColor_Click(object sender, EventArgs e)
        {
            currentColorPick = sender;
            colorPicker.SelectedColor = (int)((PictureBox)sender).Tag;
            labelCurrent.Text = "Current Selected:\r\n" + GetLabelText((PictureBox)sender);            
        }

        private string GetLabelText(PictureBox sender)
        {
            try
            {
                Label l = this.GetType().GetField("label" + sender.Name.Substring(7).ToString(), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(this) as Label;
                return l.Text;
            }
            catch (NullReferenceException)
            {
                return "";
            }
        }

        private void textRawMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
            {
                if (e.KeyCode == Keys.K)
                {
                    textRawMessage.SelectedText = ((char)3).ToString();
                    e.Handled = true;
                }
                if (e.KeyCode == Keys.B)
                {
                    textRawMessage.SelectedText = ((char)2).ToString();
                    e.Handled = true;
                }
                if (e.KeyCode == Keys.U)
                {
                    textRawMessage.SelectedText = ((char)31).ToString();
                    e.Handled = true;
                }
            }
        }

        private void listIdentifiers_DoubleClick(object sender, EventArgs e)
        {
            int s = listIdentifiers.SelectedIndex;

            if (s == -1) return;

            string t = listIdentifiers.Items[s].ToString();
            t = t.Substring(0,t.IndexOf(" "));
            textRawMessage.SelectedText = t;
        }

        #region AddMessageIdentifiers

        private void AddMsgIdent(string MessageName, string Identifier)
        {
            if (messageIdentifiers.Contains(MessageName))
            {
                ArrayList idents = (ArrayList)messageIdentifiers[MessageName];
                bool Found = false;
                int i = 0;

                IEnumerator myEnum = idents.GetEnumerator();
                while (myEnum.MoveNext())
                {
                    if (myEnum.Current.ToString().IndexOf(Identifier.Substring(0, Identifier.IndexOf(' '))) > -1)
                    {
                        Found = true;
                        break;
                    }
                    i++;
                }

                if (!Found)
                    idents.Add(Identifier);
                else
                    idents[i] = Identifier;

                messageIdentifiers[MessageName] = idents;
            }
            else
            {
                ArrayList a = new ArrayList();
                a.Add(Identifier);
                messageIdentifiers.Add(MessageName, a);
            }

        }

        private void AddMessageIdentifiers()
        {
            AddMsgIdent("Server Connect", "$server - actual server name");
            AddMsgIdent("Server Connect", "$serverip - server IP Address");
            AddMsgIdent("Server Connect", "$port - server port");

            AddMsgIdent("Server Disconnect", "$server - server name");
            AddMsgIdent("Server Disconnect", "$serverip - server IP Address");
            AddMsgIdent("Server Disconnect", "$port - server port");

            AddMsgIdent("Server Reconnect", "$server - server name");
            AddMsgIdent("Server Reconnect", "$serverip - server IP Address");
            AddMsgIdent("Server Reconnect", "$port - server port");

            AddMsgIdent("Channel Invite", "$nick - nickname who invited you");
            AddMsgIdent("Channel Invite", "$host - hostname of nick");
            AddMsgIdent("Channel Invite", "$channel - channel you were invited to");

            AddMsgIdent("Channel Message", "$nick - nickname who messaged");
            AddMsgIdent("Channel Message", "$host - hostname of nick");
            AddMsgIdent("Channel Message", "$status - op/voice status of nick");
            AddMsgIdent("Channel Message", "$channel - channel name");
            AddMsgIdent("Channel Message", "$message - channel message");
            AddMsgIdent("Channel Message", "$color - nickname nicklist color");

            AddMsgIdent("Channel Action", "$nick - nickname who performed action");
            AddMsgIdent("Channel Action", "$host - hostname of nick");
            AddMsgIdent("Channel Action", "$status - op/voice status of nick");
            AddMsgIdent("Channel Action", "$channel - channel name");
            AddMsgIdent("Channel Action", "$message - channel action");

            AddMsgIdent("Channel Join", "$nick - nickname who joined");
            AddMsgIdent("Channel Join", "$host - hostname of nick");
            //AddMsgIdent("Channel Join", "$status - op/voice status of nick");
            AddMsgIdent("Channel Join", "$channel - channel name");

            AddMsgIdent("Channel Part", "$nick - nickname who parted");
            AddMsgIdent("Channel Part", "$host - hostname of nick");
            AddMsgIdent("Channel Part", "$status - op/voice status of nick");
            AddMsgIdent("Channel Part", "$channel - channel name");
            AddMsgIdent("Channel Part", "$reason - part reason");

            AddMsgIdent("Channel Kick", "$nick - nickname who performed kick");
            AddMsgIdent("Channel Kick", "$host - hostname of nick who performed");
            //AddMsgIdent("Channel Kick", "$status - op/voice status of nick");
            AddMsgIdent("Channel Kick", "$channel - channel name");
            AddMsgIdent("Channel Kick", "$kickee - the nick who was kicked");
            AddMsgIdent("Channel Kick", "$reason - kick reason");

            AddMsgIdent("Channel Mode", "$nick - who performed the mode change");
            AddMsgIdent("Channel Mode", "$host - host of who performed the mode change");
            AddMsgIdent("Channel Mode", "$channel - what channel had the mode change");
            AddMsgIdent("Channel Mode", "$mode - the channel mode that changed");
            AddMsgIdent("Channel Mode", "$modeparam - the parameters for the mode");

            AddMsgIdent("Channel Notice", "$nick - nickname who noticed");
            AddMsgIdent("Channel Notice", "$host - host of nick");
            AddMsgIdent("Channel Notice", "$status - the status level in the channel the notice was sent to");
            AddMsgIdent("Channel Notice", "$channel - the channel the notice was sent to");
            AddMsgIdent("Channel Notice", "$message - the notice message sent");

            AddMsgIdent("Channel Other", "$message - the message");

            AddMsgIdent("Channel Nick Change", "$nick - the old nick name");
            AddMsgIdent("Channel Nick Change", "$host - hostname");
            AddMsgIdent("Channel Nick Change", "$newnick - the new nick name");

            AddMsgIdent("Self Channel Join", "$nick - yourself");
            AddMsgIdent("Self Channel Join", "$host - your hostname");
            AddMsgIdent("Self Channel Join", "$channel - channel name");

            AddMsgIdent("Self Channel Part", "$nick - yourself");
            AddMsgIdent("Self Channel Part", "$host - your hostname");
            AddMsgIdent("Self Channel Part", "$status - your op/voice status");
            AddMsgIdent("Self Channel Part", "$channel - channel name");
            AddMsgIdent("Self Channel Part", "$reason - part reason");

            AddMsgIdent("Self Channel Kick", "$nick - yourself");
            //AddMsgIdent("Self Channel Kick", "$status - your op/voice status");
            AddMsgIdent("Self Channel Kick", "$channel - channel name");
            AddMsgIdent("Self Channel Kick", "$kicker - the nick who performed the kick");
            AddMsgIdent("Self Channel Kick", "$host - the host of who performed the kick");
            AddMsgIdent("Self Channel Kick", "$reason - kick reason");

            AddMsgIdent("Self Channel Message", "$nick - yourself");
            //AddMsgIdent("Self Channel Message", "$host - your hostname ");
            AddMsgIdent("Self Channel Message", "$status - your op/voice status");
            AddMsgIdent("Self Channel Message", "$channel - channel name");
            AddMsgIdent("Self Channel Message", "$message - channel message");
            AddMsgIdent("Self Channel Message", "$color - nickname nicklist color");

            AddMsgIdent("Self Channel Action", "$nick - yourself");
            //AddMsgIdent("Self Channel Action", "$host - your hostname");
            //AddMsgIdent("Self Channel Action", "$status - your op/voice status");
            AddMsgIdent("Self Channel Action", "$channel - channel name");
            AddMsgIdent("Self Channel Action", "$message - channel action");

            AddMsgIdent("Self Nick Change", "$nick - your old nick name");
            AddMsgIdent("Self Nick Change", "$host - your hostname");
            AddMsgIdent("Self Nick Change", "$newnick - your new nick name");

            AddMsgIdent("Private Message", "$nick - nickname who messaged");
            AddMsgIdent("Private Message", "$host - hostname of nick");
            AddMsgIdent("Private Message", "$message - channel message");

            AddMsgIdent("Self Private Message", "$nick - yourself");
            AddMsgIdent("Self Private Message", "$host - your hostname");
            AddMsgIdent("Self Private Message", "$message - private message");

            AddMsgIdent("Private Action", "$nick - nickname who performed action");
            AddMsgIdent("Private Action", "$host - hostname of nick");
            AddMsgIdent("Private Action", "$message - private action");

            AddMsgIdent("Self Private Action", "$nick - yourself");
            AddMsgIdent("Self Private Action", "$host - your hostname");
            AddMsgIdent("Self Private Action", "$message - private action");

            AddMsgIdent("Server Mode", "$mode - the mode the server changed for you");
            AddMsgIdent("Server Mode", "$nick - your nickname");
            AddMsgIdent("Server Mode", "$server - the server name");

            AddMsgIdent("User Notice", "$nick - who sent the notice");
            AddMsgIdent("User Notice", "$message - the notice");

            AddMsgIdent("User Echo", "$message - the message to echo");

            AddMsgIdent("User Whois", "$nick - the nick for the whois");
            AddMsgIdent("User Whois", "$data - the whois information");

            AddMsgIdent("User Error", "$message - the error message");

            AddMsgIdent("Server Notice", "$server - the server name");
            AddMsgIdent("Server Notice", "$message - the notice");
            AddMsgIdent("Server Notice", "$nick - your nickname");

            AddMsgIdent("Server MOTD", "$message - the MOTD message");

            AddMsgIdent("Server Quit", "$nick - nickname who quit");
            AddMsgIdent("Server Quit", "$host - hostname of nick");
            AddMsgIdent("Server Quit", "$reason - quit reason");

            AddMsgIdent("Server Message", "$server - the server name");
            AddMsgIdent("Server Message", "$message - the message");

            AddMsgIdent("Server Error", "$server - the server name");
            AddMsgIdent("Server Error", "$message - the error message");

            AddMsgIdent("CTCP Request", "$nick - the nick the Ctcp request is for");
            AddMsgIdent("CTCP Request", "$ctcp - the Ctcp you wish to request for");

            AddMsgIdent("CTCP Reply", "$nick - who you send the Ctcp Request to");
            AddMsgIdent("CTCP Reply", "$host - the host of the nick");
            AddMsgIdent("CTCP Reply", "$ctcp - which Ctcp was requested");
            AddMsgIdent("CTCP Reply", "$reply - the Ctcp was reply");

            AddMsgIdent("CTCP Send", "$nick - the nick you want to send a Ctcp Request to");
            AddMsgIdent("CTCP Send", "$ctcp - the Ctcp you wish to request");

            AddMsgIdent("Channel Topic Change", "$topic - channel topic");
            AddMsgIdent("Channel Topic Change", "$channel - channel name");
            AddMsgIdent("Channel Topic Change", "$nick - who changed topic");
            AddMsgIdent("Channel Topic Change", "$host - who changed topic host");

            AddMsgIdent("Channel Topic Text", "$channel - channel name");
            AddMsgIdent("Channel Topic Text", "$topic - channel topic");

            AddMsgIdent("DCC Chat Request", "$nick - nickname of person who requests chat");
            AddMsgIdent("DCC Chat Request", "$host - host of nickname");

            AddMsgIdent("DCC File Send", "$nick - nickname of person who is sending file");
            AddMsgIdent("DCC File Send", "$host - host of nickname");
            AddMsgIdent("DCC File Send", "$file - name of the file trying to be sent");
            AddMsgIdent("DCC File Send", "$filesize - the size in bytes of file");

            AddMsgIdent("DCC Chat Message", "$nick - nickname who messaged");
            AddMsgIdent("DCC Chat Message", "$message - chat message");

            AddMsgIdent("Self DCC Chat Message", "$nick - yourself");
            AddMsgIdent("Self DCC Chat Message", "$host - your hostname");
            AddMsgIdent("Self DCC Chat Message", "$message - chat message");

            AddMsgIdent("DCC Chat Action", "$nick - nickname who performed action");
            AddMsgIdent("DCC Chat Action", "$message - the action performed");

            AddMsgIdent("Self DCC Chat Action", "$nick - yourself");
            AddMsgIdent("Self DCC Chat Action", "$host - your hostname");
            AddMsgIdent("Self DCC Chat Action", "$message - the action performed");

        }

        #endregion

        private void colorPicker_OnClick(int colorSelected)
        {

            if (tabControlColors.SelectedTab.Text == "Messages")
            {
                //check if we are in basic or advanced
                if (tabMessages.SelectedTab.Text == "Advanced")
                {

                    if (treeMessages.SelectedNode == null)
                        return;

                    //add in the color code in the current place in the textbox
                    if (this.checkBGColor.Checked == true)
                    {
                        textFormattedText.IRCBackColor = colorSelected;
                    }
                    else
                    {
                        if (textRawMessage.Text.StartsWith(""))
                        {
                            if (textRawMessage.SelectionStart == 0)
                            {
                                int result;
                                if (int.TryParse(textRawMessage.Text.Substring(1, 2), out result))
                                    textRawMessage.Text = "" + colorSelected.ToString() + textRawMessage.Text.Substring(3);
                                else
                                    textRawMessage.Text = "" + colorSelected.ToString() + textRawMessage.Text.Substring(2);
                            }
                            else
                                this.textRawMessage.SelectedText = "" + colorSelected.ToString();
                        }
                        else
                            this.textRawMessage.Text = "" + colorSelected.ToString() + textRawMessage.Text;
                    }
                }
                else
                {
                    //basic settings
                    if (treeBasicMessages.SelectedNode == null)
                        return;
                    if (this.checkChangeBGBasic.Checked == true)
                    {
                        textFormattedBasic.IRCBackColor = colorSelected;
                    }
                    else
                    {
                        
                        string message = treeBasicMessages.SelectedNode.Tag.ToString();
                        message = message.Replace("&#x3;", ((char)3).ToString());
                        message = RemoveColorCodes(message);
                        
                        message = "" + colorSelected.ToString() + message;
                        message = message.Replace(((char)3).ToString(),"&#x3;");

                        treeBasicMessages.SelectedNode.Tag = message;
                        
                        UpdateBasicText();
                    }
                }
            }

            if (tabControlColors.SelectedTab.Text == "Nick Names")
            {
                if (currentColorPick != null)
                {
                    ((PictureBox)currentColorPick).BackColor = IrcColor.colors[colorSelected];
                    ((PictureBox)currentColorPick).Tag = colorSelected;
                }
            }
            
            if (tabControlColors.SelectedTab.Text == "Tab Bar")
            {
                if (currentColorPick != null)
                {
                    ((PictureBox)currentColorPick).BackColor = IrcColor.colors[colorSelected];
                    ((PictureBox)currentColorPick).Tag = colorSelected;
                }
            }
            
            if (tabControlColors.SelectedTab.Text == "Background")
            {
                if (currentColorPick != null)
                {
                    ((PictureBox)currentColorPick).BackColor = IrcColor.colors[colorSelected];
                    ((PictureBox)currentColorPick).Tag = colorSelected;
                }
            }


            /*
            if (this.panelColorPicker.Tag.ToString() == "TextHighLite")
            {
                if (textHighLightWord.Text.Substring(0, 1) == "")
                {
                    if (PublicIRC.IsNumeric(textHighLightWord.Text.Substring(1, 2)))
                        textHighLightWord.Text = "" + colorSelected.ToString() + textHighLightWord.Text.Substring(3);
                    else
                        textHighLightWord.Text = "" + colorSelected.ToString() + textHighLightWord.Text.Substring(2);
                }
                else
                    textHighLightWord.Text = "" + colorSelected.ToString() + textHighLightWord.Text;
            }

            */
        }

        private void treeBasicMessages_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Parent == null)
            {
                textFormattedBasic.ClearTextWindow();
                return;
            }

            if (treeBasicMessages.SelectedNode == null)
                return;

            if (treeBasicMessages.SelectedNode.Parent == null)
                return;

            string type = e.Node.Text.Split(' ').GetValue(0).ToString();
            if (type == "Server")
                textFormattedBasic.IRCBackColor = iceChatColors.ConsoleBackColor;
            else if (type == "User")
                textFormattedBasic.IRCBackColor = iceChatColors.ConsoleBackColor;
            else if (type == "Channel")
                textFormattedBasic.IRCBackColor = iceChatColors.ChannelBackColor;
            else if (type == "Private")
                textFormattedBasic.IRCBackColor = iceChatColors.QueryBackColor;
            else if (type == "Self")
            {
                type = e.Node.Text.Split(' ').GetValue(1).ToString();
                if (type == "Server")
                    textFormattedBasic.IRCBackColor = iceChatColors.ConsoleBackColor;
                else if (type == "Channel")
                    textFormattedBasic.IRCBackColor = iceChatColors.ChannelBackColor;
                else if (type == "Nick")
                    textFormattedBasic.IRCBackColor = iceChatColors.ChannelBackColor;
                else if (type == "Private")
                    textFormattedBasic.IRCBackColor = iceChatColors.QueryBackColor;
                else
                    textFormattedBasic.IRCBackColor = 0;
            }
            else
                textFormattedBasic.IRCBackColor = 0;

            this.listIdentifiers.Items.Clear();

            IDictionaryEnumerator msgIdent = messageIdentifiers.GetEnumerator();
            while (msgIdent.MoveNext())
            {
                if (msgIdent.Key.ToString().ToLower() == e.Node.Text.ToLower())
                {
                    ArrayList idents = (ArrayList)msgIdent.Value;
                    foreach (object ident in idents)
                        this.listIdentifiers.Items.Add(ident);
                }
            }

            UpdateBasicText();
        
        }

        private void treeMessages_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Parent == null)
            {
                textRawMessage.Text = null;
                textFormattedText.ClearTextWindow();
                return;
            }

            //get the window type and set the background color
            //System.Diagnostics.Debug.WriteLine(e.Node.Text);
            string type = e.Node.Text.Split(' ').GetValue(0).ToString();
            if (type == "Server")
                textFormattedText.IRCBackColor = iceChatColors.ConsoleBackColor;
            else if (type == "User")
                textFormattedText.IRCBackColor = iceChatColors.ConsoleBackColor;
            else if (type == "Channel")
                textFormattedText.IRCBackColor = iceChatColors.ChannelBackColor;
            else if (type == "Private")
                textFormattedText.IRCBackColor = iceChatColors.QueryBackColor;
            else if (type == "Self")
            {
                type = e.Node.Text.Split(' ').GetValue(1).ToString();
                if (type == "Server")
                    textFormattedText.IRCBackColor = iceChatColors.ConsoleBackColor;
                else if (type == "Channel")
                    textFormattedText.IRCBackColor = iceChatColors.ChannelBackColor;
                else if (type == "Nick")
                    textFormattedText.IRCBackColor = iceChatColors.ChannelBackColor;
                else if (type == "Private")
                    textFormattedText.IRCBackColor = iceChatColors.QueryBackColor;
                else
                    textFormattedText.IRCBackColor = 0;
            }
            else
                textFormattedText.IRCBackColor = 0;

            textRawMessage.Text = e.Node.Tag.ToString();
            //replace the color code
            textRawMessage.Text = textRawMessage.Text.Replace("&#x3;", ((char)3).ToString());
            //replace the bold code
            textRawMessage.Text = textRawMessage.Text.Replace("&#x2;", ((char)2).ToString());


            this.listIdentifiers.Items.Clear();

            IDictionaryEnumerator msgIdent = messageIdentifiers.GetEnumerator();
            while (msgIdent.MoveNext())
            {
                if (msgIdent.Key.ToString().ToLower() == e.Node.Text.ToLower())
                {
                    ArrayList idents = (ArrayList)msgIdent.Value;
                    foreach (object ident in idents)
                        this.listIdentifiers.Items.Add(ident);
                }
            }

            UpdateFormattedText();

        }

        private void UpdateBasicText()
        {
            this.textFormattedBasic.ClearTextWindow();

            string message = treeBasicMessages.SelectedNode.Tag.ToString();

            SetMessageFormat(treeBasicMessages.SelectedNode.Text, message);

            //replace some of the basic identifiers to make it look right            
            if (CheckIdentifier("$status"))
                message = message.Replace("$status", "@");

            if (CheckIdentifier("$nick"))
                message = message.Replace("$nick", "Nick");

            if (CheckIdentifier("$newnick"))
                message = message.Replace("$newnick", "NewNick");

            if (CheckIdentifier("$kickee"))
                message = message.Replace("$kickee", "ThisNick");

            if (CheckIdentifier("$kicker"))
                message = message.Replace("$kicker", "WhoKicked");

            if (CheckIdentifier("$channel"))
                message = message.Replace("$channel", "#channel");

            if (CheckIdentifier("$host"))
                message = message.Replace("$host", "ident@host.com");

            if (CheckIdentifier("$reason"))
                message = message.Replace("$reason", "Reason");

            if (CheckIdentifier("$message"))
                message = message.Replace("$message", "message");

            if (CheckIdentifier("$modeparam"))
                message = message.Replace("$modeparam", "nick!ident@host");

            if (CheckIdentifier("$mode"))
                message = message.Replace("$mode", "+o");

            if (CheckIdentifier("$ctcp"))
                message = message.Replace("$ctcp", "VERSION");

            if (CheckIdentifier("$reply"))
                message = message.Replace("$reply", "CTCP Reply");

            if (CheckIdentifier("$serverip"))
                message = message.Replace("$serverip", "192.168.1.101");

            if (CheckIdentifier("$server"))
                message = message.Replace("$server", "irc.server.com");

            if (CheckIdentifier("$port"))
                message = message.Replace("$port", "6667");

            if (CheckIdentifier("$topic"))
                message = message.Replace("$topic", "The Channel Topic");

            if (CheckIdentifier("$filesize"))
                message = message.Replace("$filesize", "512");

            if (CheckIdentifier("$file"))
                message = message.Replace("$file", "file.ext");

            message = message.Replace("$color", ((char)3).ToString() + "12");

            this.textFormattedBasic.AppendText(message, 1);

        }


        private void UpdateFormattedText()
        {
            if (treeMessages.SelectedNode == null)
                return;

            if (treeMessages.SelectedNode.Parent == null)
                return;

            this.textFormattedText.ClearTextWindow();
            string message = this.textRawMessage.Text;

            treeMessages.SelectedNode.Tag = message;

            SetMessageFormat(treeMessages.SelectedNode.Text, message);

            //replace some of the basic identifiers to make it look right            
            if (CheckIdentifier("$status"))
                message = message.Replace("$status", "@");
            
            if (CheckIdentifier("$nick"))            
                message = message.Replace("$nick", "Nick");
            
            if (CheckIdentifier("$newnick"))
                message = message.Replace("$newnick", "NewNick");
            
            if (CheckIdentifier("$kickee"))
                message = message.Replace("$kickee", "ThisNick");

            if (CheckIdentifier("$kicker"))
                message = message.Replace("$kicker", "WhoKicked");

            if (CheckIdentifier("$channel"))
                message = message.Replace("$channel", "#channel");
            
            if (CheckIdentifier("$host"))
                message = message.Replace("$host", "ident@host.com");
            
            if (CheckIdentifier("$reason"))
                message = message.Replace("$reason", "Reason");

            if (CheckIdentifier("$message"))
                message = message.Replace("$message", "message");

            if (CheckIdentifier("$modeparam"))
                message = message.Replace("$modeparam", "nick!ident@host");
            
            if (CheckIdentifier("$mode"))
                message = message.Replace("$mode", "+o");
            
            if (CheckIdentifier("$ctcp"))
                message = message.Replace("$ctcp", "VERSION");
            
            if (CheckIdentifier("$reply"))
                message = message.Replace("$reply", "CTCP Reply");
            
            if (CheckIdentifier("$serverip"))
                message = message.Replace("$serverip", "192.168.1.101");
            
            if (CheckIdentifier("$server"))
                message = message.Replace("$server", "irc.server.com");
            
            if (CheckIdentifier("$port"))
                message = message.Replace("$port", "6667");
            
            if (CheckIdentifier("$topic"))
                message = message.Replace("$topic", "The Channel Topic");
            
            if (CheckIdentifier("$filesize"))
                message = message.Replace("$filesize", "512");
            
            if (CheckIdentifier("$file"))
                message = message.Replace("$file", "file.ext");

            message = message.Replace("$color", ((char)3).ToString() + "12");

            this.textFormattedText.AppendText(message, 1);

        }

        private bool CheckIdentifier(string identifier)
        {
            foreach (string m in listIdentifiers.Items)
            {                
                if (m.StartsWith(identifier))
                    return true;
            }
            return false;
        }

        private void SetMessageFormat(string MessageName, string MessageFormat)
        {
            foreach (ServerMessageFormatItem msg in iceChatMessages.MessageSettings)
            {
                if (msg.MessageName.ToLower() == MessageName.ToLower())
                {
                    msg.FormattedMessage = MessageFormat.Replace(((char)3).ToString(), "&#x3;").Replace(((char)2).ToString(), "&#x2;");
                    return;
                }
            }
        }

        private void textRawMessage_TextChanged(object sender, EventArgs e)
        {
            UpdateFormattedText();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            iceChatColors.ChannelOwnerColor = (int)pictureOwner.Tag;
            iceChatColors.ChannelAdminColor = (int)pictureAdmin.Tag;
            iceChatColors.ChannelOpColor = (int)pictureOperator.Tag;
            iceChatColors.ChannelHalfOpColor = (int)pictureHalfOperator.Tag;
            iceChatColors.ChannelVoiceColor = (int)pictureVoice.Tag;
            iceChatColors.ChannelRegularColor = (int)pictureDefault.Tag;

            iceChatColors.TabBarCurrent = (int)pictureTabCurrent.Tag;
            iceChatColors.TabBarChannelJoin = (int)pictureTabJoin.Tag;
            iceChatColors.TabBarChannelPart = (int)pictureTabPart.Tag;
            iceChatColors.TabBarNewMessage = (int)pictureTabMessage.Tag;
            iceChatColors.TabBarServerMessage = (int)pictureTabServer.Tag;
            iceChatColors.TabBarServerQuit = (int)pictureTabQuit.Tag;
            iceChatColors.TabBarOtherMessage = (int)pictureTabOther.Tag;
            iceChatColors.TabBarDefault = (int)pictureTabDefault.Tag;

            iceChatColors.ConsoleBackColor = (int)pictureConsole.Tag;
            iceChatColors.ChannelBackColor = (int)pictureChannel.Tag;
            iceChatColors.QueryBackColor = (int)pictureQuery.Tag;
            iceChatColors.NickListBackColor = (int)pictureNickList.Tag;
            iceChatColors.ServerListBackColor = (int)pictureServerList.Tag;
            iceChatColors.TabBarCurrentBG1 = (int)pictureTabBarCurrent1.Tag;
            iceChatColors.TabBarCurrentBG2 = (int)pictureTabBarCurrent2.Tag;
            iceChatColors.TabBarOtherBG1 = (int)pictureTabBarOther1.Tag;
            iceChatColors.TabBarOtherBG2 = (int)pictureTabBarOther2.Tag;
            iceChatColors.TabBarHoverBG1 = (int)pictureTabBarHover1.Tag;
            iceChatColors.TabBarHoverBG2 = (int)pictureTabBarHover2.Tag;

            iceChatColors.PanelHeaderBG1 = (int)picturePanelHeaderBG1.Tag;
            iceChatColors.PanelHeaderBG2 = (int)picturePanelHeaderBG2.Tag;
            iceChatColors.PanelHeaderForeColor = (int)picturePanelHeaderForeColor.Tag;

            //load any plugin addons
            foreach (IPluginIceChat ipc in FormMain.Instance.IceChatPlugins)
            {
                ipc.SaveColorsForm();
            }


            if (SaveColors != null)
                SaveColors();
            

            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private string RemoveColorCodes(string line)
        {
            string ParseBackColor = @"\x03([0-9]{1,2}),([0-9]{1,2})";
            string ParseForeColor = @"\x03[0-9]{1,2}";
            string ParseColorChar = @"\x03";

            Regex ParseIRCCodes = new Regex(ParseBackColor + "|" + ParseForeColor + "|" + ParseColorChar);

            return ParseIRCCodes.Replace(line, "");
        }

    }
}
