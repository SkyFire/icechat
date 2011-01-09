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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;

namespace IceChat
{

    public partial class ServerTree : UserControl
    {

        private FormServers f;
        private SortedList serverConnections;

        private IceChatServers serversCollection;
        private int topIndex = 0;
        private int headerHeight = 23;
        private int selectedNodeIndex = 0;
        private int selectedServerID = 0;

        private string headerCaption = "";
        private ToolTip toolTip;
        private int toolTipNode = -1;

        private List<KeyValuePair<string,object>> serverNodes;

        internal event NewServerConnectionDelegate NewServerConnection;

        internal delegate void SaveDefaultDelegate();
        internal event SaveDefaultDelegate SaveDefault;

        public ServerTree()
        {
            InitializeComponent();
            headerCaption = "Favorite Servers";
            
            this.MouseUp += new MouseEventHandler(OnMouseUp);
            this.MouseDown += new MouseEventHandler(OnMouseDown);
            this.MouseMove += new MouseEventHandler(OnMouseMove);
            this.MouseWheel += new MouseEventHandler(OnMouseWheel);
            this.DoubleClick += new EventHandler(OnDoubleClick);
            this.Paint += new PaintEventHandler(OnPaint);
            this.FontChanged += new EventHandler(OnFontChanged);
            this.Resize += new EventHandler(OnResize);
            this.KeyDown += new KeyEventHandler(OnKeyDown);
            this.panelButtons.Resize += new EventHandler(panelButtons_Resize);
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
                s.IAL = new Hashtable();
            }

            toolTip = new ToolTip();
            toolTip.AutoPopDelay = 3000;
            Invalidate();
            
        }
        //this is to make the arrow keys work in the user control
        protected override bool IsInputKey(Keys AKeyData)
        {
            return true;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                selectedNodeIndex--;
                SelectNodeByIndex(selectedNodeIndex, false);
            }
            else if (e.KeyCode == Keys.Down)
            {
                selectedNodeIndex++;
                SelectNodeByIndex(selectedNodeIndex, false);
            }
            else if (e.KeyCode == Keys.Apps)
            {
                //right mouse key
                this.OnMouseUp(new MouseEventArgs(MouseButtons.Right, 1,0,0,0));
            }
        }

        private void OnResize(object sender, EventArgs e)
        {
            this.vScrollBar.Left = this.Width - this.vScrollBar.Width;
            this.vScrollBar.Height = this.Height - this.headerHeight - this.panelButtons.Height;
        }

        internal void ApplyLanguage()
        {
            IceChatLanguage iceChatLanguage = FormMain.Instance.IceChatLanguage;
            buttonConnect.Text = iceChatLanguage.serverTreeButtonConnect;
            buttonEdit.Text = iceChatLanguage.serverTreeButtonEdit;
            buttonDisconnect.Text = iceChatLanguage.serverTreeButtonDisconnect;
            buttonAdd.Text = iceChatLanguage.serverTreeButtonAdd;
            headerCaption = iceChatLanguage.serverTreeHeader;
            Invalidate();
        }

        private void panelButtons_Resize(object sender, EventArgs e)
        {
            buttonConnect.Width = (panelButtons.Width / 2) - 4;
            buttonDisconnect.Width = buttonConnect.Width;

            buttonAdd.Width = buttonConnect.Width;
            buttonEdit.Width = buttonConnect.Width;
            
            buttonEdit.Left = (panelButtons.Width / 2) + 1;
            buttonAdd.Left = (panelButtons.Width / 2) + 1;
            
        }


        private void OnScroll(object sender, ScrollEventArgs e)
        {
            topIndex = ((VScrollBar)sender).Value;
            Invalidate();
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            if (vScrollBar.Visible == true)
            {
                if (e.Delta < 0)
                {
                    if (vScrollBar.Maximum < (vScrollBar.Value + 2))
                    {
                        vScrollBar.Value = vScrollBar.Maximum;
                    }
                    else
                    {
                        vScrollBar.Value += 2;
                    }
                }
                else if (e.Delta > 0)
                {
                    if (0 > (vScrollBar.Value - 2))
                    {
                        vScrollBar.Value = 0;
                    }
                    else
                    {
                        vScrollBar.Value -= 2;
                    }
                }

                topIndex = vScrollBar.Value;
                Invalidate();
            }
        }

        private void OnDoubleClick(object sender, EventArgs e)
        {
            //check if the header was double clicked
            Point p = this.PointToClient(Cursor.Position);
            if (p.Y <= headerHeight)
            {
                //undock this baby
                if (this.Parent.Parent.GetType() == typeof(TabPage))
                {
                    FormMain.Instance.UnDockPanel((Panel)this.Parent);
                    return;
                }
            }

            if (selectedServerID == 0)
                return;            
            
            //only disconnect/connect if an actual server is selected, not just any window
            object findNode = FindNodeValue(selectedNodeIndex);
            if (findNode != null)
            {
                if (findNode.GetType() == typeof(ServerSetting))
                {

                    IRCConnection c = (IRCConnection)serverConnections[selectedServerID];
                    if (c != null)
                    {
                        if (c.IsConnected)
                        {
                            FormMain.Instance.ParseOutGoingCommand(c, "//quit " + c.ServerSetting.QuitMessage);
                        }
                        else
                        {
                            //switch to Console
                            FormMain.Instance.TabMain.SelectedIndex = 0;
                            c.ConnectSocket();
                        }
                        return;
                    }

                    if (NewServerConnection != null)
                        NewServerConnection(GetServerSetting(selectedServerID));
                }
            }
        }

        private void OnFontChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (this.Parent.Parent.GetType() != typeof(FormFloat))
            {
                if (e.Y <= headerHeight)
                {
                    //which side are we docked on
                    if (((IceDockPanel)this.Parent.Parent.Parent.Parent).Dock == DockStyle.Right && e.X < 22)
                    {
                        ((IceDockPanel)this.Parent.Parent.Parent.Parent).DockControl();
                        return;
                    }
                    else if (((IceDockPanel)this.Parent.Parent.Parent.Parent).Dock == DockStyle.Left && e.X > (this.Width - 22))
                    {
                        ((IceDockPanel)this.Parent.Parent.Parent.Parent).DockControl();
                        return;
                    }
                }
            }

            if (e.Y <= headerHeight)
                return;

            Graphics g = this.CreateGraphics();
            
            int _lineSize = Convert.ToInt32(this.Font.GetHeight(g));
            //find the server number, add 1 to it to make it a non-zero value
            int nodeNumber = Convert.ToInt32((e.Location.Y - headerHeight) / _lineSize) + 1 + topIndex;
            g.Dispose();
            
            SelectNodeByIndex(nodeNumber, true);

            if (e.Button == MouseButtons.Middle)
            {
                object findNode = FindNodeValue(nodeNumber);
                if (findNode != null)
                {
                    if (findNode.GetType() == typeof(IceTabPage))
                    {
                        if (((IceTabPage)findNode).WindowStyle == IceTabPage.WindowType.Channel || ((IceTabPage)findNode).WindowStyle == IceTabPage.WindowType.Query)
                        {
                            //part the channel/close the query window
                            FormMain.Instance.ParseOutGoingCommand(((IceTabPage)findNode).Connection, "/part " + ((IceTabPage)findNode).TabCaption);
                        }
                    }
                }
            }

        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Y <= headerHeight)
                return;

            Graphics g = this.CreateGraphics();

            int _lineSize = Convert.ToInt32(this.Font.GetHeight(g));
            //find the server number, add 1 to it to make it a non-zero value
            int nodeNumber = Convert.ToInt32((e.Location.Y - headerHeight) / _lineSize) + 1;

            if (nodeNumber <= serverNodes.Count)
            {
                object findNode = FindNodeValue(nodeNumber);
                if (findNode != null)
                {
                    if (findNode.GetType() == typeof(ServerSetting))
                    {
                        if (toolTipNode != nodeNumber)
                        {
                            string t = "";
                            if (((ServerSetting)findNode).RealServerName.Length > 0 )
                                t = ((ServerSetting)findNode).RealServerName + ":" + ((ServerSetting)findNode).ServerPort;
                            else
                                t = ((ServerSetting)findNode).ServerName + ":" + ((ServerSetting)findNode).ServerPort;

                            toolTip.ToolTipTitle = t; 
                            toolTip.SetToolTip(this, ((ServerSetting)findNode).NickName);
                            
                            toolTipNode = nodeNumber;
                        }
                    }
                    else if (findNode.GetType() == typeof(IceTabPage))
                    {
                        //this is a window, switch to this channel/query
                        if (toolTipNode != nodeNumber)
                        {
                            if (((IceTabPage)findNode).WindowStyle == IceTabPage.WindowType.Channel)
                            {
                                toolTip.ToolTipTitle = "Channel Information";
                                toolTip.SetToolTip(this, ((IceTabPage)findNode).TabCaption + " {" + ((IceTabPage)findNode).Nicks.Count + "} " + "[" + ((IceTabPage)findNode).ChannelModes + "]");
                            }
                            else
                            {
                                toolTip.ToolTipTitle = "User Information";
                                toolTip.SetToolTip(this, ((IceTabPage)findNode).TabCaption);
                            }
                            toolTipNode = nodeNumber;
                        }
                    }
                }
            }
            else
            {
                toolTip.RemoveAll();
            }

            g.Dispose();
        }

        internal void SelectNodeByIndex(int nodeNumber, bool RefreshMainTab)
        {
            try
            {
                if (nodeNumber < 0)
                    selectedNodeIndex = 0;
                else if (nodeNumber <= serverNodes.Count)
                    selectedNodeIndex = nodeNumber;
                else
                    selectedNodeIndex = 0;

                selectedServerID = 0;

                //System.Diagnostics.Debug.WriteLine("select by index :" + selectedNodeIndex + ":" + nodeNumber);

                this.Invalidate();
                object findNode = FindNodeValue(selectedNodeIndex);
                if (findNode != null)
                {
                    if (findNode.GetType() == typeof(ServerSetting))
                    {
                        //this is a server, switch to console
                        if (RefreshMainTab)
                            FormMain.Instance.TabMain.SelectTab(FormMain.Instance.TabMain.GetTabPage("Console"));

                        //find the correct tab for the server tab
                        foreach (ConsoleTab c in FormMain.Instance.TabMain.GetTabPage("Console").ConsoleTab.TabPages)
                        {
                            if (c.Connection != null)
                            {
                                if (c.Connection.ServerSetting == ((ServerSetting)findNode))
                                {
                                    //found the connection, switch to this tab in the Console Tab Window
                                    FormMain.Instance.TabMain.GetTabPage("Console").SelectConsoleTab(c);
                                    return;
                                }
                            }
                        }
                        //select the default console window
                        FormMain.Instance.TabMain.GetTabPage("Console").ConsoleTab.SelectedIndex = 0;
                        return;
                    }

                    else if (findNode.GetType() == typeof(IceTabPage))
                    {
                        //this is a window, switch to this channel/query
                        if (RefreshMainTab)
                            FormMain.Instance.TabMain.SelectTab((IceTabPage)findNode);
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                FormMain.Instance.WriteErrorFile(FormMain.Instance.InputPanel.CurrentConnection,"SelectNodeByIndex", e);
            }
        }

        internal void SelectTab(object selectedNode, bool RefreshMainTab)
        {
            //System.Diagnostics.Debug.WriteLine("SelectTabTree :" + selectedNode.GetType().ToString());
            BuildServerNodes();

            if (selectedNode.GetType() == typeof(ServerSetting))
            {
                //System.Diagnostics.Debug.WriteLine("SELECT server setting 1:" + ((ServerSetting)selectedNode).ServerName);
                
                //this is a console tab
                int node = FindServerNodeMatch(selectedNode);
                //System.Diagnostics.Debug.WriteLine("find node:" + node);

                SelectNodeByIndex(node, RefreshMainTab);
                
                //System.Diagnostics.Debug.WriteLine("select tab server setting 2");
            }
            else if (selectedNode.GetType() == typeof(IceTabPage))
            {
                //this is a window tab
                //check if it is a console tab or not
                //System.Diagnostics.Debug.WriteLine("select tab:" + ((IceTabPage)selectedNode).WindowStyle);
                
                if (((IceTabPage)selectedNode).WindowStyle == IceTabPage.WindowType.Console)
                {
                    if (((ConsoleTab)((IceTabPage)selectedNode).ConsoleTab.SelectedTab).Connection != null)
                        SelectNodeByIndex(FindServerNodeMatch(((ConsoleTab)((IceTabPage)selectedNode).ConsoleTab.SelectedTab).Connection.ServerSetting), RefreshMainTab);
                    else
                        SelectNodeByIndex(FindWindowNodeMatch(selectedNode), RefreshMainTab);
                }
                else
                    SelectNodeByIndex(FindWindowNodeMatch(selectedNode), RefreshMainTab);


            }
            
            this.Invalidate();                
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
                if (de.Value == (IceTabPage)nodeMatch)
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
                            this.forceDisconnectToolStripMenuItem,
                            this.toolStripMenuItemBlank,
                            this.editToolStripMenuItem,
                            this.autoJoinToolStripMenuItem,
                            this.autoPerformToolStripMenuItem,
                            this.openLogFolderToolStripMenuItem});

                        //add in the popup menu
                        AddPopupMenu("Console", contextMenuServer);

                        this.contextMenuServer.Show(this, new Point(e.X, e.Y));
                    }
                    else if (findNode.GetType() == typeof(IceTabPage))
                    {
                        //check if it is a channel or query window
                        if (((IceTabPage)findNode).WindowStyle == IceTabPage.WindowType.Channel)
                        {
                            contextMenuChannel.Items.Clear();
                            this.contextMenuChannel.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                            this.clearWindowToolStripMenuItem,
                            this.closeChannelToolStripMenuItem,
                            this.reJoinChannelToolStripMenuItem,
                            this.channelInformationToolStripMenuItem,
                            this.channelFontToolStripMenuItem});
                            
                            //add in the popup menu
                            AddPopupMenu("Channel", contextMenuChannel);

                            this.contextMenuChannel.Show(this, new Point(e.X, e.Y));
                        }
                        else if (((IceTabPage)findNode).WindowStyle == IceTabPage.WindowType.Query)
                        {
                            contextMenuQuery.Items.Clear();
                            this.contextMenuQuery.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                            this.clearWindowToolStripMenuItem1,
                            this.closeWindowToolStripMenuItem,
                            this.userInformationToolStripMenuItem,
                            this.silenceUserToolStripMenuItem});

                            //add in the popup menu
                            AddPopupMenu("Query", contextMenuQuery);

                            this.contextMenuQuery.Show(this, new Point(e.X, e.Y));
                        }
                        else if (((IceTabPage)findNode).WindowStyle == IceTabPage.WindowType.DCCChat)
                        {
                            this.contextMenuDCCChat.Show(this, new Point(e.X, e.Y));
                        }
                    }
                }
            }
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
                    ToolStripItem t;
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


                        if (caption.Length > 0)
                        {

                            //parse out $identifiers    
                            object findNode = FindNodeValue(selectedNodeIndex);
                            if (findNode != null)
                            {
                                if (p.PopupType == "Channel")
                                {
                                    if (findNode.GetType() == typeof(IceTabPage))
                                    {
                                        caption = caption.Replace("$chan", ((IceTabPage)findNode).TabCaption);
                                        command = command.Replace("$chan", ((IceTabPage)findNode).TabCaption);
                                        caption = caption.Replace("$1", ((IceTabPage)findNode).TabCaption);
                                        command = command.Replace("$1", ((IceTabPage)findNode).TabCaption);
                                    }
                                }

                                if (p.PopupType == "Query")
                                {
                                    if (findNode.GetType() == typeof(IceTabPage))
                                    {
                                        caption = caption.Replace("$nick", ((IceTabPage)findNode).TabCaption);
                                        command = command.Replace("$nick", ((IceTabPage)findNode).TabCaption);
                                        caption = caption.Replace("$1", ((IceTabPage)findNode).TabCaption);
                                        command = command.Replace("$1", ((IceTabPage)findNode).TabCaption);
                                    }
                                }

                                if (p.PopupType == "Console")
                                {
                                    if (findNode.GetType() == typeof(ServerSetting))
                                    {
                                        if (((ServerSetting)findNode).RealServerName.Length > 0)
                                        {
                                            caption = caption.Replace("$server", ((ServerSetting)findNode).RealServerName);
                                            command = command.Replace("$server", ((ServerSetting)findNode).RealServerName);
                                        }
                                        else
                                        {
                                            caption = caption.Replace("$server", ((ServerSetting)findNode).ServerName);
                                            command = command.Replace("$server", ((ServerSetting)findNode).ServerName);
                                        }
                                    }
                                }
                            }
                            
                            if (caption == "-")
                                t = new ToolStripSeparator();
                            else
                            {
                                t = new ToolStripMenuItem(caption);

                                t.Click += new EventHandler(OnPopupMenuClick);
                                t.Tag = command;
                            }

                            if (menuDepth == 0)
                                subMenu = mainMenu.Items.Add(t);
                            else
                            {
                                if (mainMenu.Items[subMenu].GetType() != typeof(ToolStripSeparator))
                                    ((ToolStripMenuItem)mainMenu.Items[subMenu]).DropDownItems.Add(t);
                            }
                            t = null;
                        }
                    }
                }
            }

        }

        private void OnPopupMenuClick(object sender, EventArgs e)
        {
            if (((ToolStripMenuItem)sender).Tag == null) return;

            if (selectedNodeIndex == 0 || selectedServerID == 0) return;

            string command = ((ToolStripMenuItem)sender).Tag.ToString();

            IRCConnection c = (IRCConnection)serverConnections[selectedServerID];
            if (c!=null)
            {
                if (c.IsConnected)
                {
                    FormMain.Instance.ParseOutGoingCommand(c, command);
                }
                return;
            }

        }

        private void panelTop_Paint(object sender, PaintEventArgs e)
        {
            Bitmap buffer = new Bitmap(this.Width, headerHeight, e.Graphics);
            Graphics g = Graphics.FromImage(buffer);
            
            Font headerFont = new Font("Verdana", 10);
            Rectangle headerR = new Rectangle(0, 0, this.Width, headerHeight);
            Brush l = new LinearGradientBrush(headerR, IrcColor.colors[FormMain.Instance.IceChatColors.PanelHeaderBG1], IrcColor.colors[FormMain.Instance.IceChatColors.PanelHeaderBG2], 300);
            g.FillRectangle(l, headerR);

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            Rectangle centered = headerR;
            centered.Offset(0, (int)(headerR.Height - e.Graphics.MeasureString(headerCaption, headerFont).Height) / 2);

            g.DrawString(headerCaption, headerFont, new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.PanelHeaderForeColor]), centered, sf);

            e.Graphics.DrawImageUnscaled(buffer, 0, 0);

            buffer.Dispose();
            headerFont.Dispose();
            //g.Dispose();
        }


        private void OnPaint(object sender, PaintEventArgs e)
        {
            try
            {
                //make the buffer we draw this all to
                Bitmap buffer = new Bitmap(this.Width, this.Height, e.Graphics);
                Graphics g = Graphics.FromImage(buffer);

                //draw the header
                g.Clear(IrcColor.colors[FormMain.Instance.IceChatColors.ServerListBackColor]);

                g.InterpolationMode = InterpolationMode.Low;
                g.SmoothingMode = SmoothingMode.HighSpeed;
                g.PixelOffsetMode = PixelOffsetMode.None;
                g.CompositingQuality = CompositingQuality.HighSpeed;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;

                Font headerFont = new Font("Verdana", 10);

                Rectangle headerR = new Rectangle(0, 0, this.Width, headerHeight);
                Brush l = new LinearGradientBrush(headerR, IrcColor.colors[FormMain.Instance.IceChatColors.PanelHeaderBG1], IrcColor.colors[FormMain.Instance.IceChatColors.PanelHeaderBG2], 300);
                g.FillRectangle(l, headerR);

                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                Rectangle centered = headerR;
                centered.Offset(0, (int)(headerR.Height - e.Graphics.MeasureString(headerCaption, headerFont).Height) / 2);

                g.DrawString(headerCaption, headerFont, new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.PanelHeaderForeColor]), centered, sf);
                    
                if (this.Parent.Parent.GetType() != typeof(FormFloat))
                {
                    if (Application.RenderWithVisualStyles)
                    {
                        if (System.Windows.Forms.VisualStyles.VisualStyleRenderer.IsElementDefined(System.Windows.Forms.VisualStyles.VisualStyleElement.ExplorerBar.NormalGroupCollapse.Normal))
                        {
                            System.Windows.Forms.VisualStyles.VisualStyleRenderer renderer = new System.Windows.Forms.VisualStyles.VisualStyleRenderer(System.Windows.Forms.VisualStyles.VisualStyleElement.ExplorerBar.NormalGroupCollapse.Normal);
                            //which side are we docked on
                            Rectangle rect = Rectangle.Empty;
                            if (((IceDockPanel)this.Parent.Parent.Parent.Parent).Dock == DockStyle.Right)
                                rect = new Rectangle(0, 0, 22, 22);
                            else
                                rect = new Rectangle(this.Width - 22, 0, 22, 22);
                            renderer.DrawBackground(g, rect);
                        }
                    }
                }
                //draw each individual server
                Rectangle listR = new Rectangle(0, headerHeight, this.Width, this.Height - headerHeight - panelButtons.Height);

                int currentY = listR.Y;
                int _lineSize = Convert.ToInt32(this.Font.GetHeight(g));

                BuildServerNodes();

                int nodeCount = 0;

                //System.Diagnostics.Debug.WriteLine("selectedNode:" + selectedNodeIndex);

                foreach (KeyValuePair<string, object> de in serverNodes)
                {
                    //get the object type for this node
                    string node = (string)de.Key;
                    string[] nodes = node.Split(':');

                    object value = de.Value;

                    int x = 0;
                    Brush b;
                    nodeCount++;
                    if (nodeCount <= topIndex)
                        continue;

                    if (nodeCount == selectedNodeIndex)
                    {
                        g.FillRectangle(new SolidBrush(SystemColors.Highlight), 0, currentY, this.Width, _lineSize);
                        b = new SolidBrush(SystemColors.HighlightText);
                    }
                    else
                        b = new SolidBrush(IrcColor.colors[Convert.ToInt32(nodes[2])]);

                    if (value.GetType() == typeof(ServerSetting))
                    {
                        if (nodeCount == selectedNodeIndex)
                        {
                            selectedServerID = ((ServerSetting)value).ID;
                        }

                        x = 0;
                    }

                    if (value.GetType() == typeof(IceTabPage))
                    {
                        x = 16;
                        if (((IceTabPage)value).WindowStyle == IceTabPage.WindowType.Channel || ((IceTabPage)value).WindowStyle == IceTabPage.WindowType.Query || ((IceTabPage)value).WindowStyle == IceTabPage.WindowType.DCCChat)
                        {
                            if (nodeCount == selectedNodeIndex)
                                selectedServerID = ((IceTabPage)value).Connection.ServerSetting.ID;
                        }
                        else if (((IceTabPage)value).WindowStyle == IceTabPage.WindowType.Window)
                        {
                            if (((IceTabPage)value).Connection == null)
                                x = 0;
                        }
                    }
                    switch (nodes[1])
                    {
                        case "0":   //disconnected
                            g.DrawImage(StaticMethods.LoadResourceImage("disconected.png"), x, currentY, 16, 16);
                            break;
                        case "1":   //connected
                            g.DrawImage(StaticMethods.LoadResourceImage("connected.png"), x, currentY, 16, 16);
                            break;
                        case "2":   //connecting
                            g.DrawImage(StaticMethods.LoadResourceImage("refresh.png"), x, currentY, 16, 16);
                            break;
                        case "3":   //channel
                            g.DrawImage(StaticMethods.LoadResourceImage("channel.png"), x, currentY, 16, 16);
                            break;
                        case "4":   //query
                        case "5":   //dcc chat
                            g.DrawImage(StaticMethods.LoadResourceImage("query.png"), x, currentY, 16, 16);
                            break;
                        case "6":   //channel list
                            g.DrawImage(StaticMethods.LoadResourceImage("channellist.png"), x, currentY, 16, 16);
                            break;
                        case "7":
                            g.DrawImage(StaticMethods.LoadResourceImage("window.png"), x, currentY, 16, 16);
                            break;
                    }

                    g.DrawString(nodes[3], this.Font, b, x + 16, currentY);

                    b.Dispose();

                    if (currentY >= (listR.Height + listR.Y))
                    {
                        vScrollBar.Maximum = serverNodes.Count - 2;
                        vScrollBar.LargeChange = ((listR.Height - _lineSize) / _lineSize);
                        break;
                    }

                    currentY += _lineSize;
                }

                if (currentY > listR.Height || vScrollBar.Value > 0)
                    vScrollBar.Visible = true;
                else
                    vScrollBar.Visible = false;

                l.Dispose();
                sf.Dispose();

                //paint the buffer onto the usercontrol
                e.Graphics.DrawImageUnscaled(buffer, 0, 0);

                buffer.Dispose();
                headerFont.Dispose();
                g.Dispose();
            }
            catch (Exception ee)
            {
                FormMain.Instance.WriteErrorFile(FormMain.Instance.InputPanel.CurrentConnection,"ServerTree OnPaint", ee);
            }
        }

        private void BuildServerNodes()
        {
            try
            {
                serverNodes.Clear();

                int nodeCount = 0;

                foreach (IceTabPage t in FormMain.Instance.TabMain.TabPages)
                {
                    if (t.Connection == null)
                    {
                        if (t.WindowStyle == IceTabPage.WindowType.Window)
                        {
                            //get the color
                            int colorQ = 0;
                            if (t.LastMessageType == FormMain.ServerMessageType.Default)
                                colorQ = FormMain.Instance.IceChatColors.TabBarCurrent;
                            else if (t.LastMessageType == FormMain.ServerMessageType.Message || t.LastMessageType == FormMain.ServerMessageType.Action)
                                colorQ = FormMain.Instance.IceChatColors.TabBarNewMessage;
                            else
                                colorQ = FormMain.Instance.IceChatColors.TabBarDefault;

                            nodeCount++;
                            serverNodes.Add(new KeyValuePair<string, object>(nodeCount.ToString() + ":7:" + colorQ.ToString() + ":" + t.TabCaption, t));
                        }
                    }
                }

                //make a list of all the servers/windows open
                foreach (ServerSetting s in serversCollection.listServers)
                {
                    nodeCount++;
                    //icon_number:color:text
                    //1st check for server name/connected
                    if (s.DisplayName.Length > 0)
                        serverNodes.Add(new KeyValuePair<string, object>(nodeCount.ToString() + ":" + IsServerConnected(s) + ":" + FormMain.Instance.IceChatColors.TabBarDefault + ":" + s.DisplayName, s));
                    else
                        serverNodes.Add(new KeyValuePair<string, object>(nodeCount.ToString() + ":" + IsServerConnected(s) + ":" + FormMain.Instance.IceChatColors.TabBarDefault + ":" + s.ServerName, s));

                    //find all open windows for this server                
                    //add the channels 1st
                    foreach (IceTabPage t in FormMain.Instance.TabMain.TabPages)
                    {
                        if (t.Connection != null)
                        {
                            if (t.Connection.ServerSetting == s && t.WindowStyle == IceTabPage.WindowType.Channel)
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
                                serverNodes.Add(new KeyValuePair<string, object>(nodeCount.ToString() + ":3:" + color.ToString() + ":" + t.TabCaption, t));
                            }
                        }
                    }

                    //add the queries next
                    foreach (IceTabPage t in FormMain.Instance.TabMain.TabPages)
                    {
                        if (t.Connection != null)
                        {
                            if (t.Connection.ServerSetting == s && t.WindowStyle == IceTabPage.WindowType.Query)
                            {
                                //get the color
                                int colorQ = 0;
                                if (t.LastMessageType == FormMain.ServerMessageType.Default)
                                    colorQ = FormMain.Instance.IceChatColors.TabBarCurrent;
                                else if (t.LastMessageType == FormMain.ServerMessageType.Message || t.LastMessageType == FormMain.ServerMessageType.Action)
                                    colorQ = FormMain.Instance.IceChatColors.TabBarNewMessage;
                                else
                                    colorQ = FormMain.Instance.IceChatColors.TabBarDefault;

                                nodeCount++;
                                serverNodes.Add(new KeyValuePair<string, object>(nodeCount.ToString() + ":4:" + colorQ.ToString() + ":" + t.TabCaption, t));
                            }
                        }
                    }

                    //add dcc chat windows
                    foreach (IceTabPage t in FormMain.Instance.TabMain.TabPages)
                    {
                        if (t.Connection != null)
                        {
                            if (t.Connection.ServerSetting == s && t.WindowStyle == IceTabPage.WindowType.DCCChat)
                            {
                                //get the color
                                int colorQ = 0;
                                if (t.LastMessageType == FormMain.ServerMessageType.Default)
                                    colorQ = FormMain.Instance.IceChatColors.TabBarCurrent;
                                else if (t.LastMessageType == FormMain.ServerMessageType.Message || t.LastMessageType == FormMain.ServerMessageType.Action)
                                    colorQ = FormMain.Instance.IceChatColors.TabBarNewMessage;
                                else
                                    colorQ = FormMain.Instance.IceChatColors.TabBarDefault;

                                nodeCount++;
                                serverNodes.Add(new KeyValuePair<string, object>(nodeCount.ToString() + ":5:" + colorQ.ToString() + ":" + t.TabCaption, t));
                            }
                        }
                    }
                    //add any channel lists
                    foreach (IceTabPage t in FormMain.Instance.TabMain.TabPages)
                    {
                        if (t.Connection != null)
                        {
                            if (t.Connection.ServerSetting == s && t.WindowStyle == IceTabPage.WindowType.ChannelList)
                            {
                                //get the color
                                int colorQ = FormMain.Instance.IceChatColors.TabBarDefault;

                                nodeCount++;
                                serverNodes.Add(new KeyValuePair<string, object>(nodeCount.ToString() + ":6:" + colorQ.ToString() + ":" + t.TabCaption, t));
                            }
                        }
                    }

                    //add @window windows
                    foreach (IceTabPage t in FormMain.Instance.TabMain.TabPages)
                    {
                        if (t.Connection != null)
                        {
                            if (t.Connection.ServerSetting == s && t.WindowStyle == IceTabPage.WindowType.Window)
                            {
                                //get the color
                                int colorQ = 0;
                                if (t.LastMessageType == FormMain.ServerMessageType.Default)
                                    colorQ = FormMain.Instance.IceChatColors.TabBarCurrent;
                                else if (t.LastMessageType == FormMain.ServerMessageType.Message || t.LastMessageType == FormMain.ServerMessageType.Action)
                                    colorQ = FormMain.Instance.IceChatColors.TabBarNewMessage;
                                else
                                    colorQ = FormMain.Instance.IceChatColors.TabBarDefault;

                                nodeCount++;
                                serverNodes.Add(new KeyValuePair<string, object>(nodeCount.ToString() + ":7:" + colorQ.ToString() + ":" + t.TabCaption, t));
                            }
                        }
                    }


                }
            }
            catch (Exception e)
            {
                FormMain.Instance.WriteErrorFile(FormMain.Instance.InputPanel.CurrentConnection,"BuildServerNodes", e);
            }
        }

        private string IsServerConnected(ServerSetting s)
        {
            foreach (IRCConnection c in serverConnections.Values)
            {
                //see if the server is connected
                if (c.ServerSetting == s)
                    if (c.IsFullyConnected)
                        return "1";
                    else if (c.IsConnected)
                        return "2";
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

        internal void SaveServers(IceChatServers servers)
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

        internal IceChatServers ServersCollection
        {
            get
            {
                return serversCollection;
            }
        }

        internal bool Docked
        {
            get { return false; }
        }

        #region Server Tree Buttons
        
        private void buttonConnect_Click(object sender, EventArgs e)
        {
            FormMain.Instance.FocusInputBox();

            if (selectedNodeIndex == 0 || selectedServerID == 0) return;

            IRCConnection c = (IRCConnection)serverConnections[selectedServerID];
            if (c != null)
            {
                if (!c.IsConnected)
                {
                    //switch to the Console
                    FormMain.Instance.TabMain.SelectedIndex = 0;
                    c.ConnectSocket();
                }
                return;
            }
            if (NewServerConnection != null)
                NewServerConnection(GetServerSetting(selectedServerID));
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            FormMain.Instance.FocusInputBox();

            if (selectedNodeIndex == 0 || selectedServerID == 0) return;

            IRCConnection c = (IRCConnection)serverConnections[selectedServerID];
            if (c != null)
            {
                c.AttemptReconnect = false;
                if (c.IsConnected)
                {
                    FormMain.Instance.ParseOutGoingCommand(c, "//quit " + c.ServerSetting.QuitMessage);                    
                }
                return;
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine(selectedServerID);
            //open up the Server Editor
            //check if a server is selected or not
            if (selectedServerID > 0)
            {
                f = new FormServers(GetServerSetting(selectedServerID));
                f.SaveServer += new FormServers.SaveServerDelegate(OnSaveServer);
                f.SaveDefaultServer += new FormServers.SaveDefaultServerDelegate(OnSaveDefaultServer);
            }
            else
            {
                f = new FormServers();
                f.NewServer += new FormServers.NewServerDelegate(OnNewServer);
                f.SaveDefaultServer += new FormServers.SaveDefaultServerDelegate(OnSaveDefaultServer);
            }

            f.ShowDialog(this.Parent);
        }
        
        /// <summary>
        /// Save the Default Server Settings
        /// </summary>
        private void OnSaveDefaultServer()
        {
            if (SaveDefault != null)
                SaveDefault();
        }
        
        private void OnSaveServer(ServerSetting s, bool removeServer)
        {
            //check if the server needs to be removed
            if (removeServer)
            {
                serversCollection.RemoveServer(s);
            }
            SaveServerSettings();
            f = null;
        }

        private void OnNewServer(ServerSetting s)
        {
            s.ID = serversCollection.GetNextID();
            s.IAL = new Hashtable();
            serversCollection.AddServer(s);
            SaveServerSettings();
            f = null;
        }

        public void AddConnection(IRCConnection c)
        {
            if (ServerConnections.ContainsKey(c.ServerSetting.ID))
            {
                Random r = new Random();
                do
                {
                    c.ServerSetting.ID = r.Next(10000, 49999);
                } while (ServerConnections.ContainsKey(c.ServerSetting.ID));
            }
            ServerConnections.Add(c.ServerSetting.ID, c);
            //check if it exists in the servers collection
            foreach (ServerSetting s in serversCollection.listServers)
            {
                if (s.ID == c.ServerSetting.ID)
                    return;
            }
            serversCollection.AddServer(c.ServerSetting);
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
            f.NewServer += new FormServers.NewServerDelegate(OnNewServer);
            f.SaveDefaultServer += new FormServers.SaveDefaultServerDelegate(OnSaveDefaultServer);            
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

        private void forceDisconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormMain.Instance.FocusInputBox();

            if (selectedNodeIndex == 0 || selectedServerID == 0) return;

            IRCConnection c = (IRCConnection)serverConnections[selectedServerID];
            if (c != null)
            {
                c.ForceDisconnect();
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonEdit.PerformClick();
        }

        private void autoJoinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormMain.Instance.FocusInputBox();

            if (selectedNodeIndex == 0 || selectedServerID == 0) return;

            IRCConnection c = (IRCConnection)serverConnections[selectedServerID];
            if (c!=null)
            {
                if (c.IsConnected)
                    FormMain.Instance.ParseOutGoingCommand(c, "/autojoin");
                return;
            }
        }

        private void autoPerformToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormMain.Instance.FocusInputBox();

            if (selectedNodeIndex == 0 || selectedServerID == 0) return;

            IRCConnection c = (IRCConnection)serverConnections[selectedServerID];
            {
                if (c.IsConnected)
                    FormMain.Instance.ParseOutGoingCommand(c, "/autoperform");
                return;
            }

        }

        private void openLogFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormMain.Instance.FocusInputBox();

            if (selectedNodeIndex == 0 || selectedServerID == 0) return;

            ServerSetting s = GetServerSetting(selectedServerID);
            if (s != null)
            {
                FormMain.Instance.ParseOutGoingCommand(null, "/run " + FormMain.Instance.LogsFolder + System.IO.Path.DirectorySeparatorChar + s.ServerName);
                return;
            }
        }

        private void clearWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //clear the channel window for the selected channel
            object findNode = FindNodeValue(selectedNodeIndex);
            if (findNode.GetType() == typeof(IceTabPage))
                ((IceTabPage)findNode).TextWindow.ClearTextWindow();

        }

        private void closeChannelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //close the channel
            object findNode = FindNodeValue(selectedNodeIndex);
            if (findNode.GetType() == typeof(IceTabPage))
                FormMain.Instance.ParseOutGoingCommand(((IceTabPage)findNode).Connection, "/part " + ((IceTabPage)findNode).TabCaption);
        }

        private void reJoinChannelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //do a channel hop
            object findNode = FindNodeValue(selectedNodeIndex);
            if (findNode.GetType() == typeof(IceTabPage))
            {
                ((IceTabPage)findNode).Connection.SendData("PART " + ((IceTabPage)findNode).TabCaption);
                ((IceTabPage)findNode).Connection.SendData("JOIN " + ((IceTabPage)findNode).TabCaption);
            }
        }

        private void channelInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //popup channel information window
            object findNode = FindNodeValue(selectedNodeIndex);
            if (findNode.GetType() == typeof(IceTabPage))
                FormMain.Instance.ParseOutGoingCommand(((IceTabPage)findNode).Connection, "/chaninfo " + ((IceTabPage)findNode).TabCaption);
        }

        private void channelFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //popup change font window for channel
            object findNode = FindNodeValue(selectedNodeIndex);
            if (findNode.GetType() == typeof(IceTabPage))
                FormMain.Instance.ParseOutGoingCommand(((IceTabPage)findNode).Connection, "/font " + ((IceTabPage)findNode).TabCaption);

        }

        private void clearWindowToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //clear query window
            object findNode = FindNodeValue(selectedNodeIndex);
            if (findNode.GetType() == typeof(IceTabPage))
                ((IceTabPage)findNode).TextWindow.ClearTextWindow();
        }

        private void closeWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //close query window
            object findNode = FindNodeValue(selectedNodeIndex);
            if (findNode.GetType() == typeof(IceTabPage))
                FormMain.Instance.ParseOutGoingCommand(((IceTabPage)findNode).Connection, "/part " + ((IceTabPage)findNode).TabCaption);

        }

        private void userInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //user information for query nick
            object findNode = FindNodeValue(selectedNodeIndex);
            if (findNode.GetType() == typeof(IceTabPage))
                FormMain.Instance.ParseOutGoingCommand(((IceTabPage)findNode).Connection, "/userinfo " + ((IceTabPage)findNode).TabCaption);
        }

        private void silenceUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //silence query user
            object findNode = FindNodeValue(selectedNodeIndex);
            if (findNode.GetType() == typeof(IceTabPage))
                FormMain.Instance.ParseOutGoingCommand(((IceTabPage)findNode).Connection, "/silence +" + ((IceTabPage)findNode).TabCaption);
        }

        private void clearWindowDCCChat_Click(object sender, EventArgs e)
        {
            object findNode = FindNodeValue(selectedNodeIndex);
            if (findNode.GetType() == typeof(IceTabPage))
                ((IceTabPage)findNode).TextWindow.ClearTextWindow();
        }

        private void closeWindowDCCChat_Click(object sender, EventArgs e)
        {
            object findNode = FindNodeValue(selectedNodeIndex);
            if (findNode.GetType() == typeof(IceTabPage))
                FormMain.Instance.ParseOutGoingCommand(((IceTabPage)findNode).Connection, "/close " + ((IceTabPage)findNode).TabCaption);
        }

        private void disconnectDCCChat_Click(object sender, EventArgs e)
        {
            object findNode = FindNodeValue(selectedNodeIndex);
            if (findNode.GetType() == typeof(IceTabPage))
                ((IceTabPage)findNode).DisconnectDCC();
        }

        #endregion




    }
}
