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
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;

namespace IceChat
{
    public partial class IceTabControl : TabControl
    {
        private Point DragStartPosition = Point.Empty;
        
        private TabWindow drag_Tab;
        private int selectedTabIndex;

        private IList tabPages;

        private delegate int SelectedTabIndexDelegate();
        private delegate void RefreshTabsDelegate();

        private ContextMenuStrip popupMenu;

        public IceTabControl()
        {
            InitializeComponent();
			
            this.MouseDown += new MouseEventHandler(OnMouseDown);
            this.MouseMove += new MouseEventHandler(OnMouseMove);
            this.DragOver += new DragEventHandler(OnDragOver);
            this.MouseUp += new MouseEventHandler(OnMouseUp);
            this.SelectedIndexChanged += new EventHandler(OnSelectedIndexChanged);
            this.FontChanged += new EventHandler(OnFontChanged);

            this.ControlAdded += new ControlEventHandler(OnControlAdded);
            this.ControlRemoved += new ControlEventHandler(OnControlRemoved);

            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            
            this.UpdateStyles();

            tabPages = new List<TabPage>();

            popupMenu = ConsolePopupMenu();
            popupMenu.ItemClicked += new ToolStripItemClickedEventHandler(OnPopupMenu_ItemClicked);
            
            
            // http://www.koders.com/csharp/fid169847566777A4F89EE9CAE80755155A457B13E0.aspx
        }

        private void OnPopupMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //send the command to the proper window
            if (e.ClickedItem.Tag == null) return;
            
            string command = e.ClickedItem.Tag.ToString();
            
            if (selectedTabIndex == 0)
            {
                //a console command, find out which is the current tab
                command = command.Replace("$1", "Console");
                FormMain.Instance.ParseOutGoingCommand(((ConsoleTabWindow)FormMain.Instance.TabMain.TabPages[0]).CurrentConnection, command);
            }
            else
            {
                TabWindow t = ((TabWindow)FormMain.Instance.TabMain.TabPages[selectedTabIndex]);
                if (t != null)
                {
                    command = command.Replace("$1", t.WindowName);
                    FormMain.Instance.ParseOutGoingCommand(t.Connection, command);
                }
            }
        }

        private ContextMenuStrip ConsolePopupMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();           

            menu.Items.Add(NewMenuItem("Clear","/clear $1"));
            menu.Items.Add(NewMenuItem("Clear All", "/clear all console"));
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(NewMenuItem("Quit Server","/quit"));
            
            //add the console popup menu
            AddPopupMenu("Console", menu);
            return menu;
        }

        private ContextMenuStrip ChannelPopupMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            
            menu.Items.Add(NewMenuItem("Clear Window", "/clear $1"));
            menu.Items.Add(NewMenuItem("Close Channel", "/part $1"));
            menu.Items.Add(NewMenuItem("Rejoin Channel", "/hop $1"));
            menu.Items.Add(NewMenuItem("Channel Information", "/channelinfo $1"));

            //add then channel popup menu
            AddPopupMenu("Channel", menu);
            return menu;
        }

        private ContextMenuStrip QueryPopupMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();

            menu.Items.Add(NewMenuItem("Clear Window", "/clear $1"));
            menu.Items.Add(NewMenuItem("Close Window", "/part $1"));
            menu.Items.Add(NewMenuItem("User Information", "/userinfo $1"));
            menu.Items.Add(NewMenuItem("Silence User", "/silence +$1"));

            //add then channel popup menu
            AddPopupMenu("Query", menu);
            return menu;

        }

        private ToolStripMenuItem NewMenuItem(string caption, string command)
        {
            ToolStripMenuItem t = new ToolStripMenuItem(caption);
            t.Tag = command;
            return t;
        }

        private void AddPopupMenu(string PopupType, ContextMenuStrip mainMenu)
        {
            //add the console menu popup
            if (FormMain.Instance == null) return;
            
            if (FormMain.Instance.IceChatPopupMenus == null) return;

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
                            TabWindow tw = null;
                            if (selectedTabIndex != 0)
                            {
                                tw = ((TabWindow)FormMain.Instance.TabMain.TabPages[selectedTabIndex]);
                            }

                            if (p.PopupType == "Channel")
                            {
                                if (tw != null)
                                {
                                    caption = caption.Replace("$chan", tw.WindowName);
                                    command = command.Replace("$chan", tw.WindowName);
                                    caption = caption.Replace("$1", tw.WindowName);
                                    command = command.Replace("$1", tw.WindowName);
                                }
                            }

                            if (p.PopupType == "Query")
                            {
                                if (tw != null)
                                {
                                    caption = caption.Replace("$nick", tw.WindowName);
                                    command = command.Replace("$nick", tw.WindowName);
                                    caption = caption.Replace("$1", tw.WindowName);
                                    command = command.Replace("$1", tw.WindowName);
                                }
                            }
                            
                            if (caption == "-")
                                t = new ToolStripSeparator();
                            else
                            {
                                t = new ToolStripMenuItem(caption);

                                t.Click += new EventHandler(OnPopupExtraMenuClick);
                                t.Tag = command;
                            }

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

        private void OnPopupExtraMenuClick(object sender, EventArgs e)
        {
            if (((ToolStripMenuItem)sender).Tag == null) return;

            string command = ((ToolStripMenuItem)sender).Tag.ToString();

            //System.Diagnostics.Debug.WriteLine(selectedTabIndex +  ":command:" + command);

            if (selectedTabIndex == 0)
            {
                //a console command, find out which is the current tab
                command = command.Replace("$1", "Console");
                FormMain.Instance.ParseOutGoingCommand(((ConsoleTabWindow)FormMain.Instance.TabMain.TabPages[0]).CurrentConnection, command);
            }
            else
            {
                
                TabWindow t = ((TabWindow)FormMain.Instance.TabMain.TabPages[selectedTabIndex]);
                if (t != null)
                {
                    command = command.Replace("$1", t.WindowName);
                    FormMain.Instance.ParseOutGoingCommand(t.Connection, command);
                }
            }


        }
        private void OnFontChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            FormMain.Instance.ServerTree.SelectTab(this.SelectedTab);
        }

        private void OnControlRemoved(object sender, ControlEventArgs e)
        {
            if (e.Control.GetType() != typeof(ConsoleTabWindow))
            {
                ((TabWindow)e.Control).TextWindow.CloseLogFile();
                tabPages.Remove((TabWindow)e.Control);
            }
        }

        private void OnControlAdded(object sender, ControlEventArgs e)
        {
            if (e.Control.GetType() != typeof(IceChat.ConsoleTabWindow))
                tabPages.Add((TabWindow)e.Control);
        }

        internal IList WindowTabs
        {
            get { return tabPages; }
        }
        
        internal bool WindowExists(IRCConnection connection, string windowName, TabWindow.WindowType windowType)
        {
            foreach (TabWindow t in this.tabPages)
            {
                if (t.Connection == connection)
                {
                    if (t.WindowStyle == windowType)
                    {
                        if (t.WindowName == windowName)
                            return true;
                    }
                }
            }
            return false;
        }

        internal void SelectTab(TabWindow tabWindow)
        {
            foreach (TabWindow t in this.tabPages)
            {
                if (t == tabWindow)
                {
                    this.SelectedTab = t;
                    break;
                }
            }
        }

        internal int SelectedWindowTabIndex()
        {
            if (this.InvokeRequired)
            {
                SelectedTabIndexDelegate s = new SelectedTabIndexDelegate(SelectedWindowTabIndex);
                return (int)this.Invoke(s, new object[] { });
            }
            else
            {
                return this.TabPages.IndexOf(this.SelectedTab);
            }
        }

        private int SelectedMenuTab()
        {
            for (int index = 0; index <= this.TabCount - 1; index++)
            {
                if (this.GetTabRect(index).Contains(this.PointToClient(Cursor.Position)))
                    return index;
            }
            return -1;
        }

        #region Swap Tab Pages Methods

        private TabWindow HoverTab()
        {
            for (int index = 1; index <= this.TabCount - 1; index++)
            {
                if (this.GetTabRect(index).Contains(this.PointToClient(Cursor.Position)))
                    return (TabWindow)this.TabPages[index];
            }
            return null;
        }

        private void SwapTabPages(TabWindow tp1, TabWindow tp2)
        {
            int Index1 = this.TabPages.IndexOf(tp1);
            int Index2 = this.TabPages.IndexOf(tp2);
            
            this.TabPages[Index1] = tp2;
            this.TabPages[Index2] = tp1;
            
        }
      
        private void OnDragOver(object sender, DragEventArgs e)
        {            
            TabWindow hover_Tab = HoverTab();            

            if (hover_Tab == null)
                e.Effect = DragDropEffects.None;
            else
            {
                if (hover_Tab.WindowName == "Console") return;

                if (e.Data.GetDataPresent(typeof(TabWindow)))
                {                    
                    e.Effect = DragDropEffects.Move;

                    if (drag_Tab == null) return;                    
                    if (hover_Tab == drag_Tab) return;
                    
                    Rectangle TabRect = this.GetTabRect(this.TabPages.IndexOf(hover_Tab));
                    
                    TabRect.Inflate(-3, -3);

                    if (TabRect.Contains(this.PointToClient(new Point(e.X, e.Y))))
                    {
                        SwapTabPages(drag_Tab, hover_Tab);
                        this.SelectedTab = drag_Tab;
                        //this.Invalidate();
                    }
                }
            }
            
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //show the proper popup menu according to what kind of tab
                if (selectedTabIndex == 0)
                {
                    //console tab
                    popupMenu.ItemClicked -= new ToolStripItemClickedEventHandler(OnPopupMenu_ItemClicked);
                    popupMenu.Items.Clear();
                    popupMenu = ConsolePopupMenu();
                    popupMenu.ItemClicked += new ToolStripItemClickedEventHandler(OnPopupMenu_ItemClicked);
                    popupMenu.Show(this, e.Location);
                }
                else
                {
                    if (((TabWindow)TabPages[selectedTabIndex]).WindowStyle == TabWindow.WindowType.Channel)
                    {
                        popupMenu.ItemClicked -= new ToolStripItemClickedEventHandler(OnPopupMenu_ItemClicked);
                        popupMenu.Items.Clear();
                        popupMenu = ChannelPopupMenu();
                        popupMenu.ItemClicked += new ToolStripItemClickedEventHandler(OnPopupMenu_ItemClicked);
                        popupMenu.Show(this, e.Location);
                    }
                    else if (((TabWindow)TabPages[selectedTabIndex]).WindowStyle == TabWindow.WindowType.Query)
                    {
                        popupMenu.ItemClicked -= new ToolStripItemClickedEventHandler(OnPopupMenu_ItemClicked);
                        popupMenu.Items.Clear();
                        popupMenu = QueryPopupMenu();
                        popupMenu.ItemClicked += new ToolStripItemClickedEventHandler(OnPopupMenu_ItemClicked);
                        popupMenu.Show(this, e.Location);
                    }
                    
                }
            }
            
            drag_Tab = null;
            FormMain.Instance.FocusInputBox();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            Rectangle r = new Rectangle(DragStartPosition, Size.Empty);

            r.Inflate(SystemInformation.DragSize);

            if (drag_Tab != null)
            {
                if (!r.Contains(e.X, e.Y))
                    this.DoDragDrop(drag_Tab, DragDropEffects.Move);
            }
            
            DragStartPosition = Point.Empty;
            
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            DragStartPosition = new Point(e.X, e.Y);
            drag_Tab = HoverTab();

            //which tab was selected
            selectedTabIndex = SelectedMenuTab();
            //FormMain.Instance.WindowMessage(null, "Console", "Selected:" + selectedTabIndex, 1);

        }

        #endregion

        internal void RefreshTabs()
        {
            if (this.InvokeRequired)
            {
                RefreshTabsDelegate r = new RefreshTabsDelegate(RefreshTabs);
                this.Invoke(r, new object[] { });
            }
            else
                this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {            
            for (int i = 0; i < this.TabCount; i++)
                DrawTab(e.Graphics, this.TabPages[i], i);

        }

        private void DrawTab(Graphics g, TabPage tabPage, int nIndex)
        {
            //http://www.codeproject.com/KB/tabs/flattabcontrol.aspx

            Rectangle r = (Rectangle)this.GetTabRect(nIndex);
            RectangleF tabTextArea = (RectangleF)this.GetTabRect(nIndex);

            bool bSelected = (this.SelectedIndex == nIndex);

            Point[] pt = new Point[7];
            pt[0] = new Point(r.Left + 1, r.Bottom);
            if (bSelected)
            {
                pt[1] = new Point(r.Left + 1, r.Top + 3);
                pt[2] = new Point(r.Left + 4, r.Top);
                pt[3] = new Point(r.Right - 4, r.Top);
                pt[4] = new Point(r.Right - 1, r.Top + 3);
            }
            else
            {
                pt[1] = new Point(r.Left + 1, r.Top + 6);
                pt[2] = new Point(r.Left + 4, r.Top + 3);
                pt[3] = new Point(r.Right - 4, r.Top + 3);
                pt[4] = new Point(r.Right - 1, r.Top + 6);
            }
            pt[5] = new Point(r.Right - 1, r.Bottom);
            pt[6] = new Point(r.Left + 1, r.Bottom);

            string title;
            if (nIndex == 0)
            {
                title = "Console";
            }
            else
            {
                title = ((TabWindow)tabPage).WindowName;
            }

            Brush b;
            Brush l;

            if (bSelected)
            {
                b = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.TabBarCurrent]);
                l = new LinearGradientBrush(r, IrcColor.colors[FormMain.Instance.IceChatColors.TabBarCurrentBG1], IrcColor.colors[FormMain.Instance.IceChatColors.TabBarCurrentBG2], 90);
            }
            else
            {
                if (nIndex == 0)
                    b = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.TabBarDefault]);
                else
                {
                    //get the font color from the last message type in the channel            
                    switch (((TabWindow)tabPage).LastMessageType)
                    {
                        case FormMain.ServerMessageType.JoinChannel:
                            b = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.TabBarChannelJoin]);
                            break;
                        case FormMain.ServerMessageType.PartChannel:
                            b = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.TabBarChannelPart]);
                            break;
                        case FormMain.ServerMessageType.Message:
                        case FormMain.ServerMessageType.Action:
                            b = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.TabBarNewMessage]);
                            break;
                        case FormMain.ServerMessageType.QuitServer:
                            b = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.TabBarServerQuit]);
                            break;
                        case FormMain.ServerMessageType.ServerMessage:
                            b = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.TabBarServerMessage]);
                            break;
                        case FormMain.ServerMessageType.Other:
                            b = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.TabBarOtherMessage]);
                            break;
                        default:
                            b = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.TabBarDefault]);
                            break;
                    }
                }
                l = new LinearGradientBrush(r, IrcColor.colors[FormMain.Instance.IceChatColors.TabBarOtherBG1], IrcColor.colors[FormMain.Instance.IceChatColors.TabBarOtherBG2], 270);
            }


            // fill the polygon region with the brush color            
            g.FillPolygon(l, pt);
            // draw the border around the control
            g.DrawPolygon(new Pen(Color.Black, 1), pt);

            //draw the icon
            Image img = this.ImageList.Images[tabPage.ImageIndex];
            Rectangle rimage = new Rectangle(r.X, r.Y, img.Width, img.Height);
            
            Rectangle closeButton = new Rectangle(r.Right - 12, 8, 10, 12);

            if (bSelected)
            {
                rimage.Offset(2, 4);
                g.DrawImage(img, rimage);
                tabTextArea.Offset(17, 2);
                closeButton.Offset(0, -2);
            }
            else
            {
                rimage.Offset(2, 6);
                g.DrawImage(img, rimage);
                tabTextArea.Offset(17, 4);
            }

            g.DrawString(title, this.Font, b, tabTextArea);
            /*
            //draw the close button
            if (nIndex != 0)
            {
                g.FillRectangle(new SolidBrush(Color.Gray), closeButton);
                g.DrawRectangle(new Pen(Color.LightGray), closeButton);
                g.DrawString("x", new Font("Verdana", 8), b, closeButton);
            }
            */
            l.Dispose();
            b.Dispose();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Font = new System.Drawing.Font("Verdana", 19.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResumeLayout(false);

        }
    }
}
