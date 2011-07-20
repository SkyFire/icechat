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
using System.Xml;
using System.Xml.Serialization;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Collections;
using System.IO;

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
        private TabControl m_RightPanel;
        private TabControl m_LeftPanel;
        
        private string m_ServerTreeCurrentTab;
        private IceChat.IRCConnection m_ServerTreeCurrentConnection;

        private TabPage twitterTab;
        private TreeView twitterTree;
        private Panel twitterPanel;
        
        private string usersFile;
        private IceChatTwitter twitterUsers;

        private string headerCaption = "Twitter Feeds";
        private int headerHeight = 23;

        //all the events get declared here
        public event OutGoingCommandHandler OnCommand;

        public Plugin()
        {
            //set your default values here
            m_Name = "Twitter Plugin";
            m_Author = "Snerf";
            m_Version = "1.0";
        }

        public void Dispose()
        {
            m_LeftPanel.TabPages.Remove(twitterTab);
        }

        public void Initialize()
        {

            usersFile = currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatTwitter.xml";
            
            twitterTab = new TabPage("Twitter");
            twitterPanel = new Panel();
            twitterPanel.Dock = DockStyle.Fill;

            twitterPanel.Paint += new PaintEventHandler(twitterPanel_Paint);
            twitterPanel.Resize += new EventHandler(twitterPanel_Resize);

            twitterTree = new TreeView();
            twitterTree.Top = headerHeight;
            twitterTree.Left = 0;
            
            twitterPanel.Controls.Add(twitterTree);

            twitterTab.Controls.Add(twitterPanel);

            m_LeftPanel.TabPages.Add(twitterTab);

            //load in your twitter settings
            LoadTwitterSettings();
        }

        private void twitterPanel_Resize(object sender, EventArgs e)
        {
            twitterTree.Width = twitterPanel.Width;
            twitterTree.Height = twitterPanel.Height - headerHeight;
        }

        private void twitterPanel_Paint(object sender, PaintEventArgs e)
        {
            Bitmap buffer = new Bitmap(twitterPanel.Width, twitterPanel.Height, e.Graphics);
            Graphics g = Graphics.FromImage(buffer);

            //draw the header
            Rectangle headerR = new Rectangle(0, 0, twitterPanel.Width, headerHeight);
            Brush l = new SolidBrush(Color.Red);
            g.FillRectangle(l, headerR);

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;

            Font headerFont = new Font("Verdana", 10);

            Rectangle centered = headerR;
            centered.Offset(0, (int)(headerR.Height - g.MeasureString(headerCaption, headerFont).Height) / 2);

            g.DrawString(headerCaption, headerFont, new SolidBrush(Color.Black), centered, sf);

            e.Graphics.DrawImageUnscaled(buffer, 0, 0);
            buffer.Dispose();
            g.Dispose();
            l.Dispose();
            headerFont.Dispose();     
            
            
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

        public TabControl LeftPanel
        {
            get { return m_LeftPanel; }
            set { m_LeftPanel = value; }
        }

        public TabControl RightPanel
        {
            get { return m_RightPanel; }
            set { m_RightPanel = value; }
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

        public AppDomain domain
        {
            get { return m_domain; }
            set { m_domain = value; }
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

        public void MainProgramLoaded()
        {
            //when the main program has finished loading
        }

        public void SaveColorsForm()
        {
            //when the save button is clicked on the Colors Settings Window

        }

        public void SaveSettingsForm()
        {
            //when the save button is clicked on the IceChat Settings Window
        }

        public void LoadEditorForm(TabControl ScriptsTab)
        {


        }

        public void SaveEditorForm()
        {

        }

        public ToolStripItem[] AddChannelPopups()
        {
            return null;
        }

        public ToolStripItem[] AddQueryPopups()
        {
            return null;
        }

        public ToolStripItem[] AddServerPopups()
        {
            return null;
        }

        //declare all the necessary events

        public PluginArgs ChannelMessage(PluginArgs args)
        {
            return args;
        }

        public PluginArgs ChannelAction(PluginArgs args)
        {
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
            return args;
        }
        
        public PluginArgs ChannelPart(PluginArgs args)
        {
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
        
        //newly added here
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

        private void LoadTwitterUser(string user)
        {
            twitterTree.Nodes.Clear();

            TreeNode root = twitterTree.Nodes.Add("icechat");

            //get the 5 latest tweets
            // http://api.twitter.com/1/statuses/user_timeline.xml?screen_name=icechat&count=5

            //shows the latest tweet, we can read the tweet ID then
            // http://api.twitter.com/1/users/show.xml?screen_name=icechat

            System.Net.WebClient webClient = new System.Net.WebClient();
            webClient.DownloadFile("http://api.twitter.com/1/statuses/user_timeline.xml?screen_name=" + user + "&count=5", currentFolder + System.IO.Path.DirectorySeparatorChar + "twitter_" + user + ".xml");
            
            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.Load(currentFolder + System.IO.Path.DirectorySeparatorChar + "twitter_" + user + ".xml");

            XmlNodeList statuses = xmlDoc.DocumentElement.ChildNodes;
            foreach (XmlNode status in statuses)
            {
                XmlNodeList items = status.ChildNodes;
                foreach (XmlNode item in items)
                {
                    //MessageBox.Show(item.InnerXml + ":" + item.Name);
                    switch (item.Name)
                    {
                        case "id":
                            //the twitter ID
                            break;
                        case "text":
                            //the text of the tweet
                            root.Nodes.Add(item.InnerText);
                            break;
                        case "created_at":
                            //when it was created
                            break;

                    }

                }
            }

            System.IO.File.Delete(currentFolder + System.IO.Path.DirectorySeparatorChar + "twitter_" + user + ".xml");

        }

        private void LoadTwitterSettings()
        {
            // load the twitter settings
            // and show them in the tree
            LoadUsers();

            foreach (TwitterItem item in twitterUsers.listUsers)
            {
                LoadTwitterUser(item.Username);
            }

        }

        private void LoadUsers()
        {
            if (File.Exists(usersFile))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(IceChatTwitter));
                TextReader textReader = new StreamReader(usersFile);
                twitterUsers = (IceChatTwitter)deserializer.Deserialize(textReader);
                textReader.Close();
                textReader.Dispose();
            }
            else
                twitterUsers = new IceChatTwitter();
        }

    }

    public class IceChatTwitter
    {
        [XmlArray("Usernames")]
        [XmlArrayItem("Item", typeof(TwitterItem))]
        public ArrayList listUsers;

        public IceChatTwitter()
        {
            listUsers = new ArrayList();
        }
        public void AddTwitter(TwitterItem item)
        {
            listUsers.Add(item);
        }
    }

    public class TwitterItem
    {
        [XmlElement("Username")]
        public string Username
        { get; set; }

        [XmlElement("LastID")]
        public long lastID
        { get; set; }

    }
}
