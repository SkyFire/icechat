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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;

namespace IceChat2009
{

    public partial class ServerTree : UserControl
    {

        private FormServers f;
        private SortedList serverConnections;

        private IceChatServers serversCollection;
        private int topIndex = 0;
        private int headerHeight = 23;
        private int selectedNodeIndex = 0;
        private int selectedServerIndex = 0;

        private string headerCaption = "";
        
        private List<KeyValuePair<string,object>> serverNodes;

        internal event NewServerConnectionDelegate NewServerConnection;

        public ServerTree()
        {
            
            InitializeComponent();

            headerCaption = "Favorite Servers";
            
            this.MouseUp += new MouseEventHandler(OnMouseUp);
            this.MouseDown += new MouseEventHandler(OnMouseDown);
            this.DoubleClick += new EventHandler(OnDoubleClick);
            this.Paint += new PaintEventHandler(OnPaint);
            this.FontChanged += new EventHandler(OnFontChanged);

            this.vScrollBar.Scroll += new ScrollEventHandler(OnScroll);
            this.DoubleBuffered = true;

            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            
            this.UpdateStyles();

            serverConnections = new SortedList();

            serverNodes = new List<KeyValuePair<string,object>>();

            serversCollection = LoadServers();

            foreach (ServerSetting s in serversCollection.listServers)
            {
                if (s.AltNickName == null)
                    s.AltNickName = s.NickName + "_";
            }
            
            Invalidate();

        }

        private void OnScroll(object sender, ScrollEventArgs e)
        {
            topIndex = ((VScrollBar)sender).Value;
            Invalidate();
        }

        private void OnDoubleClick(object sender, EventArgs e)
        {
            if (selectedServerIndex == 0)
                return;            
            
            foreach (IRCConnection c in serverConnections.Values)
            {
                if (c.ServerSetting.ID == selectedServerIndex)
                {
                    if (c.IsConnected)
                        c.SendData("QUIT :" + c.ServerSetting.QuitMessage);
                    else
                    {
                        //switch to Console
                        FormMain.Instance.TabMain.SelectedIndex = 0;
                        c.ConnectSocket();
                    }
                    return;
                }
            }

            if (NewServerConnection != null)
                NewServerConnection(GetServerSetting(selectedServerIndex));
            
        }

        private void OnFontChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Y <= headerHeight)
                return;

            Graphics g = this.CreateGraphics();
            
            int lineSize = Convert.ToInt32(this.Font.GetHeight(g));
            //find the server number, add 1 to it to make it a non-zero value
            int nodeNumber = Convert.ToInt32((e.Location.Y - headerHeight) / lineSize) + 1;
            
            if (nodeNumber <= serverNodes.Count)
                selectedNodeIndex = nodeNumber;
            else
            {
                selectedNodeIndex = 0;
                selectedServerIndex = 0;
            }


            g.Dispose();

            Invalidate();

            object findNode = FindNodeValue(selectedNodeIndex);
            if (findNode != null)
            {
                //System.Diagnostics.Debug.WriteLine(findNode.ToString());
                if (findNode.GetType() == typeof(ServerSetting))
                {
                    //this is a server, switch to console
                    FormMain.Instance.TabMain.SelectTab(0);

                    //(ConsoleTabWindow)tabMain.TabPages[0]//find the correct tab for the server tab
                    foreach (ConsoleTab c in ((ConsoleTabWindow)FormMain.Instance.TabMain.TabPages[0]).ConsoleTab.TabPages)
                    {
                        if (c.Connection != null)
                        {
                            if (c.Connection.ServerSetting == ((ServerSetting)findNode))
                            {
                                //found the connection, switch to this tab in the Console Tab Window
                                ((ConsoleTabWindow)FormMain.Instance.TabMain.TabPages[0]).ConsoleTab.SelectedTab = c;
                                return;
                            }
                        }
                    }

                    //select the default console window
                    ((ConsoleTabWindow)FormMain.Instance.TabMain.TabPages[0]).ConsoleTab.SelectedIndex = 0;
                    return;
                }
                
                else if (findNode.GetType() == typeof(TabWindow))
                {
                    //this is a window, switch to this channel/query
                    FormMain.Instance.TabMain.SelectTab((TabWindow)findNode);
                    return;
                }
                
                
            }
        }

        public void SelectTab(object selectedNode)
        {
            if (selectedNode.GetType() == typeof(ServerSetting))
            {
                //this is a console tab
                selectedNodeIndex = FindServerNodeMatch(selectedNode);
            }
            else if (selectedNode.GetType() == typeof(TabWindow))
            {
                //this is a window tab
                selectedNodeIndex = FindWindowNodeMatch(selectedNode);
            }
            else if (selectedNode.GetType() == typeof(ConsoleTabWindow))
            {
                //this is the main console tab
                if (((ConsoleTab)((ConsoleTabWindow)selectedNode).ConsoleTab.SelectedTab).Connection != null)
                    selectedNodeIndex = FindServerNodeMatch(((ConsoleTab)((ConsoleTabWindow)selectedNode).ConsoleTab.SelectedTab).Connection.ServerSetting);
                else
                    selectedNodeIndex = 0;
            }
            
            Invalidate();                

        }
        
        private int FindServerNodeMatch(object nodeMatch)
        {
            int nodeCount = 0;
            foreach (KeyValuePair<string, object> de in serverNodes)
            {
                nodeCount++;
                if (de.Value == (ServerSetting)nodeMatch)
                {
                    return nodeCount;
                }
            }
            return 0;
        }

        private int FindWindowNodeMatch(object nodeMatch)
        {
            int nodeCount = 0;
            foreach (KeyValuePair<string, object> de in serverNodes)
            {
                nodeCount++;
                if (de.Value == (TabWindow)nodeMatch)
                {
                    return nodeCount;
                }
            }
            return 0;
        }

        /// <summary>
        /// Find a node by the index and return its value (node type)
        /// </summary>
        /// <param name="nodeIndex"></param>
        /// <returns></returns>
        private object FindNodeValue(int nodeIndex)
        {
            int nodeCount = 0;
            foreach (KeyValuePair<string, object> de in serverNodes)
            {
                nodeCount++;
                if (nodeCount == nodeIndex)
                {
                    return de.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// Return focus back to the InputText Box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            //see what menu to popup according to the nodetype
            if (e.Button == MouseButtons.Right)
            {
                //see what kind of a node we right clicked
                object findNode = FindNodeValue(selectedNodeIndex);
                if (findNode != null)
                {
                    if (findNode.GetType() == typeof(ServerSetting))
                    {
                        //make the default menu
                        this.contextMenuServer.Items.Clear();
                        
                        this.contextMenuServer.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                            this.connectToolStripMenuItem,
                            this.disconnectToolStripMenuItem,
                            this.editToolStripMenuItem,
                            this.autoJoinToolStripMenuItem,
                            this.autoPerformToolStripMenuItem});

                        //add in the popup menu
                        AddPopupMenu("Console", contextMenuServer);

                        contextMenuServer.Show(this, new Point(e.X, e.Y));
                    }
                    else if (findNode.GetType() == typeof(TabWindow))
                    {
                        //check if it is a channel or query window
                        if (((TabWindow)findNode).WindowStyle == TabWindow.WindowType.Channel)
                        {
                            contextMenuChannel.Items.Clear();
                            this.contextMenuChannel.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                            this.clearWindowToolStripMenuItem,
                            this.closeChannelToolStripMenuItem,
                            this.reJoinChannelToolStripMenuItem,
                            this.channelInformationToolStripMenuItem});

                            //add in the popup menu
                            AddPopupMenu("Channel", contextMenuChannel);

                            contextMenuChannel.Show(this, new Point(e.X, e.Y));
                        }
                    }
                }
            }
            else
                FormMain.Instance.FocusInputBox();
        }

        private void AddPopupMenu(string PopupType, ContextMenuStrip mainMenu)
        {
            //add the console menu popup
            foreach (PopupMenuItem p in FormMain.Instance.IceChatPopupMenus.listPopups)
            {
                if (p.PopupType == PopupType && p.Menu.Length > 0)
                {
                    //add a break
                    mainMenu.Items.Add(new ToolStripSeparator());
                    
                    string[] menuItems = p.Menu;

                    //build the menu
                    ToolStripMenuItem t;
                    int subMenu = 0;

                    foreach (string menu in menuItems)
                    {
                        string caption;
                        string command;
                        string menuItem = menu;
                        int menuDepth = 0;

                        //get the menu depth
                        while (menuItem.StartsWith("."))
                        {
                            menuItem = menuItem.Substring(1);
                            menuDepth++;
                        }

                        if (menu.IndexOf(':') > 0)
                        {
                            caption = menuItem.Substring(0, menuItem.IndexOf(':'));
                            command = menuItem.Substring(menuItem.IndexOf(':') + 1);
                        }
                        else
                        {
                            caption = menuItem;
                            command = "";
                        }

                        //parse out the command/$identifiers    

                        if (caption.Length > 0)
                        {
                            t = new ToolStripMenuItem(caption);

                            t.Click += new EventHandler(OnPopupMenuClick);
                            t.Tag = command;

                            if (menuDepth == 0)
                                subMenu = mainMenu.Items.Add(t);
                            else
                                ((ToolStripMenuItem)mainMenu.Items[subMenu]).DropDownItems.Add(t);

                            t = null;
                        }
                    }
                }
            }

        }

        private void OnPopupMenuClick(object sender, EventArgs e)
        {
            if (((ToolStripMenuItem)sender).Tag == null) return;

            if (selectedNodeIndex == 0) return;

            string command = ((ToolStripMenuItem)sender).Tag.ToString();

            foreach (IRCConnection c in serverConnections.Values)
            {
                if (c.ServerSetting.ID == selectedServerIndex)
                {
                    if (c.IsConnected)
                    {
                        FormMain.Instance.ParseOutGoingCommand(c, command);
                    }
                    return;
                }
            }

        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            //make the buffer we draw this all to
            Bitmap buffer = new Bitmap(this.Width, this.Height, e.Graphics);
            Graphics g = Graphics.FromImage(buffer);

            //draw the header
            g.Clear(IrcColor.colors[FormMain.Instance.IceChatColors.ServerListBackColor]);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Rectangle headerR = new Rectangle(0, 0, this.Width, headerHeight);
            Brush l = new LinearGradientBrush(headerR, Color.Silver, Color.White, 300);
            g.FillRectangle(l, headerR);

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            Font headerFont = new Font("Verdana", 10);
            Rectangle centered = headerR;
            centered.Offset(0, (int)(headerR.Height - e.Graphics.MeasureString(headerCaption, headerFont).Height) / 2);

            g.DrawString(headerCaption, headerFont, new SolidBrush(Color.Black), centered, sf);

            //draw each individual server
            Rectangle listR = new Rectangle(0, headerHeight, this.Width, this.Height - headerHeight);

            int currentY = listR.Y;
            int lineSize = Convert.ToInt32(this.Font.GetHeight(g));

            BuildServerNodes();

            int nodeCount = 0;
            int serverCount = 0;

            foreach (KeyValuePair<string,object> de in serverNodes)
            {
                //get the object type for this node
                string node = (string)de.Key;
                string[] nodes = node.Split(':');
                
                object value = de.Value;
                
                int x = 0;
                Brush b;
                nodeCount++;
                if (nodeCount == selectedNodeIndex)
                {
                    g.FillRectangle(new SolidBrush(SystemColors.Highlight), 0, currentY, this.Width, lineSize);
                    b = new SolidBrush(SystemColors.HighlightText);
                }
                else
                    b = new SolidBrush(IrcColor.colors[Convert.ToInt32(nodes[2])]);                
                
                if (value.GetType() == typeof(ServerSetting))
                {
                    serverCount++;
                    
                    if (nodeCount == selectedNodeIndex)
                        selectedServerIndex = serverCount;
                    
                    x = 0;
                }
                
                if (value.GetType() == typeof(TabWindow))
                    x = 16;
                
                g.DrawImage(imageListServers.Images[Convert.ToInt32(nodes[1])], x, currentY);
                
                g.DrawString(nodes[0] + ":" + nodes[3], this.Font, b, x+ 16, currentY);
                currentY += lineSize;
                b.Dispose();
            }


            //paint the buffer onto the usercontrol
            e.Graphics.DrawImageUnscaled(buffer, 0, 0);

            buffer.Dispose();
            headerFont.Dispose();
            l.Dispose();
            sf.Dispose();
            g.Dispose();

        }

        private void BuildServerNodes()
        {
            serverNodes.Clear();

            int nodeCount = 0;

            //make a list of all the servers/windows open
            foreach (ServerSetting s in serversCollection.listServers)
            {
                nodeCount++;

                //icon_number:color:text
                //1st check for server name/connected
                if (s.DisplayName.Length > 0)
                    serverNodes.Add(new KeyValuePair<string,object>(nodeCount.ToString() + ":" + IsServerConnected(s) + ":" + FormMain.Instance.IceChatColors.TabBarDefault + ":" + s.DisplayName,s));
                else
                    serverNodes.Add(new KeyValuePair<string,object>(nodeCount.ToString() + ":" + IsServerConnected(s) + ":" + FormMain.Instance.IceChatColors.TabBarDefault + ":" + s.ServerName, s));

                //find all open windows for this server                

                //add the channels 1st
                foreach (TabWindow t in FormMain.Instance.TabMain.WindowTabs)
                {
                    if (t.Connection != null)
                    {
                        if (t.Connection.ServerSetting == s && t.WindowStyle == TabWindow.WindowType.Channel)
                        {
                            int color = 0;
                            if (t.LastMessageType == FormMain.ServerMessageType.Default)
                                color = FormMain.Instance.IceChatColors.TabBarCurrent;
                            else if (t.LastMessageType == FormMain.ServerMessageType.JoinChannel)
                                color = FormMain.Instance.IceChatColors.TabBarChannelJoin;
                            else if (t.LastMessageType == FormMain.ServerMessageType.PartChannel)
                                color = FormMain.Instance.IceChatColors.TabBarChannelPart;
                            else if (t.LastMessageType == FormMain.ServerMessageType.Message || t.LastMessageType == FormMain.ServerMessageType.Action)
                                color = FormMain.Instance.IceChatColors.TabBarNewMessage;
                            else if (t.LastMessageType == FormMain.ServerMessageType.ServerMessage)
                                color = FormMain.Instance.IceChatColors.TabBarServerMessage;
                            else if (t.LastMessageType == FormMain.ServerMessageType.QuitServer)
                                color = FormMain.Instance.IceChatColors.TabBarServerQuit;
                            else if (t.LastMessageType == FormMain.ServerMessageType.Other)
                                color = FormMain.Instance.IceChatColors.TabBarOtherMessage;
                            else
                                color = FormMain.Instance.IceChatColors.TabBarDefault;

                            nodeCount++;
                            serverNodes.Add(new KeyValuePair<string,object>(nodeCount.ToString() + ":2:" + color.ToString() + ":" + t.WindowName, t));
                        }
                    }
                }
                
                //add the queries next
                foreach (TabWindow t in FormMain.Instance.TabMain.WindowTabs)
                {
                    if (t.Connection != null)
                    {
                        if (t.Connection.ServerSetting == s && t.WindowStyle == TabWindow.WindowType.Query)
                        {
                            nodeCount++;
                            serverNodes.Add(new KeyValuePair<string,object>(nodeCount.ToString() + ":3:1:" + t.WindowName, t));
                        }
                    }
                }
            }


        }

        private string IsServerConnected(ServerSetting s)
        {
            foreach (IRCConnection c in serverConnections.Values)
            {
                //see if the server is connected
                if (c.ServerSetting == s)
                    if (c.IsConnected)
                        return "1";
            }
            return "0";
        }


        private ServerSetting GetServerSetting(int id)
        {
            ServerSetting ss = null;
            foreach (ServerSetting s in serversCollection.listServers)
            {
                if (s.ID == id)
                    ss = s;
            }
            return ss;
        }

        private void SaveServers(IceChatServers servers)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(IceChatServers));
            TextWriter textWriter = new StreamWriter(FormMain.Instance.ServersFile);
            serializer.Serialize(textWriter, servers);
            textWriter.Close();
            textWriter.Dispose();
        }

        private IceChatServers LoadServers()
        {
            IceChatServers servers;

            XmlSerializer deserializer = new XmlSerializer(typeof(IceChatServers));
            if (File.Exists(FormMain.Instance.ServersFile))
            {
                TextReader textReader = new StreamReader(FormMain.Instance.ServersFile);
                servers = (IceChatServers)deserializer.Deserialize(textReader);
                textReader.Close();
                textReader.Dispose();
            }
            else
            {
                //create default server settings
                servers = new IceChatServers();
                ServerSetting s = new ServerSetting();
                
                s.ID = 1;
                s.ServerName = "irc.quakenet.org";

                Random partNick = new Random();
                s.NickName = "Guest09_" + partNick.Next(100, 999).ToString();
                s.AltNickName = s.NickName + "_";
                s.AutoJoinChannels = new string[] { "#icechat2009" };
                s.AutoJoinEnable = true;
                servers.AddServer(s);

                FormMain.Instance.IceChatOptions.DefaultNick = s.NickName;

                SaveServers(servers);
            }
            return servers;
        }
        

        internal SortedList ServerConnections
        {
            get
            {
                return serverConnections;
            }
        }

        #region Server Tree Buttons
        
        private void buttonConnect_Click(object sender, EventArgs e)
        {
            FormMain.Instance.FocusInputBox();

            if (selectedNodeIndex == 0) return;

            foreach (IRCConnection c in serverConnections.Values)
            {
                if (c.ServerSetting.ID == selectedServerIndex)
                {
                    //System.Diagnostics.Debug.WriteLine("c:" + c.ServerSetting.ID + ":" + c.IsConnected);
                    if (!c.IsConnected)
                    {
                        //switch to the Console
                        FormMain.Instance.TabMain.SelectedIndex = 0;
                        c.ConnectSocket();
                    }
                    return;
                }                    
            }
            
            if (NewServerConnection != null)
                NewServerConnection(GetServerSetting(selectedServerIndex));
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            FormMain.Instance.FocusInputBox();

            if (selectedNodeIndex == 0) return;

            foreach (IRCConnection c in serverConnections.Values)
            {
                if (c.ServerSetting.ID == selectedServerIndex)
                {
                    if (c.IsConnected)
                    {
                        c.AttemptReconnect = false;
                        c.SendData("QUIT :" + c.ServerSetting.QuitMessage);
                    }
                    return;
                }
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            //open up the Server Editor
            //check if a server is selected or not
            if (selectedServerIndex > 0)
            {
                f = new FormServers(GetServerSetting(selectedServerIndex));
                f.SaveServer += new FormServers.SaveServerDelegate(f_SaveServer);
            }
            else
            {
                f = new FormServers();
                f.NewServer += new FormServers.NewServerDelegate(f_NewServer);
            }

            f.ShowDialog(this.Parent);
        }
        private void f_SaveServer()
        {
            SaveServerSettings();
            f = null;
        }

        private void f_NewServer(ServerSetting s)
        {
            s.ID = serversCollection.listServers.Count + 1;
            serversCollection.AddServer(s);
            SaveServerSettings();
            f = null;
        }

        private void SaveServerSettings()
        {
            //save the XML File
            SaveServers(serversCollection);

            //update the Server Tree
            Invalidate();            

            FormMain.Instance.FocusInputBox();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            FormServers f = new FormServers();
            f.NewServer += new FormServers.NewServerDelegate(f_NewServer);
            f.ShowDialog(this.Parent);
        }


        #endregion

        #region Server Popup Menus

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonConnect.PerformClick();
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonDisconnect.PerformClick();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonEdit.PerformClick();
        }

        private void autoJoinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormMain.Instance.FocusInputBox();

            if (selectedNodeIndex == 0) return;

            foreach (IRCConnection c in serverConnections.Values)
            {
                if (c.ServerSetting.ID == selectedServerIndex)
                {
                    if (c.IsConnected)
                        FormMain.Instance.ParseOutGoingCommand(c, "/autojoin");
                    return;
                }
            }
        }

        private void autoPerformToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormMain.Instance.FocusInputBox();

            if (selectedNodeIndex == 0) return;

            foreach (IRCConnection c in serverConnections.Values)
            {
                if (c.ServerSetting.ID == selectedServerIndex)
                {
                    if (c.IsConnected)
                        FormMain.Instance.ParseOutGoingCommand(c, "/autoperform");
                    return;
                }
            }

        }
        
        private void clearWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //clear the channel window for the selected channel
            object findNode = FindNodeValue(selectedNodeIndex);
            if (findNode.GetType() == typeof(TabWindow))
                ((TabWindow)findNode).TextWindow.ClearTextWindow();

        }

        private void closeChannelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //close the channel
            object findNode = FindNodeValue(selectedNodeIndex);
            if (findNode.GetType() == typeof(TabWindow))
                FormMain.Instance.ParseOutGoingCommand(((TabWindow)findNode).Connection, "/part " + ((TabWindow)findNode).WindowName);
        }

        private void reJoinChannelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //do a channel hop
            object findNode = FindNodeValue(selectedNodeIndex);
            if (findNode.GetType() == typeof(TabWindow))
            {
                ((TabWindow)findNode).Connection.SendData("PART " + ((TabWindow)findNode).WindowName);
                ((TabWindow)findNode).Connection.SendData("JOIN " + ((TabWindow)findNode).WindowName);
            }
        }

        private void channelInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //popup channel information window
            object findNode = FindNodeValue(selectedNodeIndex);
            if (findNode.GetType() == typeof(TabWindow))
                FormMain.Instance.ParseOutGoingCommand(((TabWindow)findNode).Connection, "/channelinfo " + ((TabWindow)findNode).WindowName);
        }

        #endregion


        
    }
}
