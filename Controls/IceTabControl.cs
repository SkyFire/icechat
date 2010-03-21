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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace IceChat
{
    public partial class IceTabControl : UserControl 
    {

        //Tab font which affect the cards' height and width
        private Font m_fontTab = new Font("Verdana", 10F);

        //Tabs Area Rectangle grouped by the tab selectindex
        private Dictionary<int, Rectangle> m_TabSizeRects = new Dictionary<int, Rectangle>();
        private Dictionary<int, Rectangle> m_TabTextRects = new Dictionary<int, Rectangle>();

        //IceTabPage(inherited from Panel) of this TabControl
        private List<IceTabPage> m_lTabPages = new List<IceTabPage>();
        
        //tabs' height
        private int m_TabRowHeight = 30;
        
        //how many rows of tabs to display
        private int m_TotalTabRows = 1;

        //TabControl's selectedIndex
        private int m_SelectedIndex = -1;
        private int m_PreviousSelectedIndex = 0;

        //TabControl's hoveredIndex
        private int m_iHoveredIndex = -1;

        //starting position of dragging
        private Point DragStartPosition = Point.Empty;
        
        //which tab will be dragged
        private IceTabPage drag_Tab;

        private int m_TabStartXPos = 0;

        //for the popupmenu
        private ContextMenuStrip popupMenu;

        private Panel m_pnlCloseButton;

        //public event System.EventHandler SelectedIndexChanged;
        public delegate void TabEventHandler(object sender, TabEventArgs e);
        public event TabEventHandler SelectedIndexChanged;
        public delegate void TabClosedDelegate(int nIndex);
        public event TabClosedDelegate OnTabClosed;

        public IceTabControl()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            InitializeComponent();
            InitializeCustom();

            popupMenu = ConsolePopupMenu();
            popupMenu.ItemClicked += new ToolStripItemClickedEventHandler(OnPopupMenu_ItemClicked);

        }

        internal List<IceTabPage> TabPages 
        {
            get
            {
                return this.m_lTabPages;
            }
        }

        public int SelectedIndex 
        {
            set 
            {
                //System.Diagnostics.Debug.WriteLine(m_SelectedIndex + ":" + value);
                //if (m_SelectedIndex != value)
                this.m_PreviousSelectedIndex = m_SelectedIndex;
                this.m_SelectedIndex = value;
                                        
                if (this.SelectedIndexChanged != null)
                {
                    TabEventArgs e = new TabEventArgs();
                    e.IsHandled = true;
                    SelectedIndexChanged(this, e);
                }                    
            }
            get
            {
                return this.m_SelectedIndex;
            }
        }

        internal int TabCount 
        {
            get 
            {
                return this.m_lTabPages.Count;
            }
        }

        internal Font TabFont 
        {
            set 
            {
                this.m_fontTab = value;
            }
        }

        internal IceTabPage CurrentTab
        {
            get
            {
                //System.Diagnostics.Debug.WriteLine("Current Tab:" + m_SelectedIndex + ":" + m_lTabPages.Count);
                if (m_SelectedIndex == -1) m_SelectedIndex = 0;
                if (m_SelectedIndex > (m_lTabPages.Count - 1)) m_SelectedIndex = 0;
                return m_lTabPages[m_SelectedIndex];
            }
        }

        internal void SelectTab(IceTabPage page)
        {
            if (CurrentTab!=null && CurrentTab.TextWindow!=null)
                CurrentTab.TextWindow.resetUnreadMarker();

            for (int i = 0; i < m_lTabPages.Count; i++)
            {
                if (m_lTabPages[i] == page)
                {
                    SelectedIndex = i;
                    Invalidate();
                    
                    
                    if (this.SelectedIndexChanged != null)
                    {
                        TabEventArgs e = new TabEventArgs();
                        e.IsHandled = true;
                        SelectedIndexChanged(this, e);
                    }
                    
                    break;                
                }
            }
        }
        
        internal IceTabPage GetTabPage(string sCaption)
        {
            for (int i = 0; i < m_lTabPages.Count; i++)
            {
                if (m_lTabPages[i].TabCaption.Equals(sCaption))
                    return m_lTabPages[i];
            }
            return null;
        }

        internal bool WindowExists(IRCConnection connection, string windowName, IceTabPage.WindowType windowType)
        {
            foreach (IceTabPage t in this.TabPages)
            {
                if (t.Connection == connection)
                {
                    if (t.WindowStyle == windowType)
                    {
                        if (t.TabCaption == windowName)
                            return true;
                    }
                }
            }
            return false;
        }

        internal IceTabPage GetTabPage(int iTabIndex)
        {
            if (iTabIndex < m_lTabPages.Count)
                return m_lTabPages[iTabIndex];
            return null;
        }

        private void InitializeCustom() 
        {
            this.MouseDown += new MouseEventHandler(OnMouseDown);
            this.MouseMove += new MouseEventHandler(OnMouseMove);
            this.MouseLeave += new EventHandler(OnMouseLeave);
            this.MouseUp += new MouseEventHandler(OnMouseUp);
            //this.ControlAdded += new ControlEventHandler(OnControlAdded);
            this.ControlRemoved += new ControlEventHandler(OnControlRemoved);
            
            this.m_pnlCloseButton = new Panel();
            this.m_pnlCloseButton.BackColor = SystemColors.Control;
            this.m_pnlCloseButton.Size = new Size(21, 21);
            this.m_pnlCloseButton.MouseDown += new MouseEventHandler(m_pnlCloseButton_MouseDown);
            this.m_pnlCloseButton.MouseHover += new EventHandler(m_pnlCloseButton_MouseHover);
            this.m_pnlCloseButton.MouseLeave += new EventHandler(m_pnlCloseButton_MouseLeave);
            this.m_pnlCloseButton.Paint += new PaintEventHandler(m_pnlCloseButton_Paint);
            this.m_pnlCloseButton.Dock = DockStyle.Right;

            this.Controls.Add(m_pnlCloseButton);

            this.AutoSize = false;

        }

        private void m_pnlCloseButton_Paint(object sender, PaintEventArgs e)
        {
            DrawCloseButton();
        }

        private void m_pnlCloseButton_MouseLeave(object sender, EventArgs e)
        {
            DrawCloseButton();
        }

        private void m_pnlCloseButton_MouseHover(object sender, EventArgs e)
        {
            DrawCloseButtonHover();
        }

        private void OnPopupMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //send the command to the proper window
            if (e.ClickedItem.Tag == null) return;

            string command = e.ClickedItem.Tag.ToString();

            if (GetTabPage(m_SelectedIndex).TabCaption == "Console")
            {
                //a console command, find out which is the current tab
                command = command.Replace("$1", "Console");
                FormMain.Instance.ParseOutGoingCommand(GetTabPage("Console").CurrentConnection, command);
            }
            else
            {
                
                IceTabPage t = FormMain.Instance.TabMain.TabPages[m_SelectedIndex];
                if (t != null)
                {
                    command = command.Replace("$1", t.TabCaption);
                    FormMain.Instance.ParseOutGoingCommand(t.Connection, command);
                }
            }
        }

        private ContextMenuStrip ConsolePopupMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();

            menu.Items.Add(NewMenuItem("Clear", "/clear $1",global::IceChat.Properties.Resources.clear));
            menu.Items.Add(NewMenuItem("Clear All", "/clear all console", global::IceChat.Properties.Resources.clear));
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(NewMenuItem("Quit Server", "/quit", global::IceChat.Properties.Resources.disconected));

            //add the console popup menu
            AddPopupMenu("Console", menu);
            return menu;
        }

        private ContextMenuStrip ChannelPopupMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();

            menu.Items.Add(NewMenuItem("Clear Window", "/clear $1", global::IceChat.Properties.Resources.clear));
            menu.Items.Add(NewMenuItem("Close Channel", "/part $1", global::IceChat.Properties.Resources.CloseButton));
            menu.Items.Add(NewMenuItem("Rejoin Channel", "/hop $1", global::IceChat.Properties.Resources.refresh));
            menu.Items.Add(NewMenuItem("Channel Information", "/chaninfo $1", global::IceChat.Properties.Resources.info));

            //add then channel popup menu
            AddPopupMenu("Channel", menu);
            return menu;
        }

        private ContextMenuStrip QueryPopupMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();

            menu.Items.Add(NewMenuItem("Clear Window", "/clear $1",global::IceChat.Properties.Resources.clear));
            menu.Items.Add(NewMenuItem("Close Window", "/part $1", global::IceChat.Properties.Resources.CloseButton));
            menu.Items.Add(NewMenuItem("User Information", "/userinfo $1", global::IceChat.Properties.Resources.user_info));
            menu.Items.Add(NewMenuItem("Silence User", "/silence +$1", null));

            //add then channel popup menu
            AddPopupMenu("Query", menu);
            return menu;

        }

        private ToolStripMenuItem NewMenuItem(string caption, string command, Bitmap icon)
        {
            ToolStripMenuItem t = new ToolStripMenuItem(caption);
            //global::IceChat.Properties.Resources.connected
            if (icon != null)
                t.Image = icon;
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
                            IceTabPage tw = null;
                            if (GetTabPage(m_SelectedIndex).TabCaption != "Console")
                            {
                                tw = GetTabPage(m_SelectedIndex);
                            }

                            if (p.PopupType == "Channel")
                            {
                                if (tw != null)
                                {
                                    caption = caption.Replace("$chan", tw.TabCaption);
                                    command = command.Replace("$chan", tw.TabCaption);
                                    caption = caption.Replace("$1", tw.TabCaption);
                                    command = command.Replace("$1", tw.TabCaption);
                                }
                            }

                            if (p.PopupType == "Query")
                            {
                                if (tw != null)
                                {
                                    caption = caption.Replace("$nick", tw.TabCaption);
                                    command = command.Replace("$nick", tw.TabCaption);
                                    caption = caption.Replace("$1", tw.TabCaption);
                                    command = command.Replace("$1", tw.TabCaption);
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

            if (GetTabPage(m_SelectedIndex).TabCaption == "Console")
            {
                //a console command, find out which is the current tab
                command = command.Replace("$1", "Console");
                FormMain.Instance.ParseOutGoingCommand(GetTabPage("Console").CurrentConnection, command);
            }
            else
            {

                IceTabPage t = GetTabPage(m_SelectedIndex);
                if (t != null)
                {
                    command = command.Replace("$1", t.TabCaption);
                    FormMain.Instance.ParseOutGoingCommand(t.Connection, command);
                }
            }
        }

        private void m_pnlCloseButton_MouseDown(object sender, MouseEventArgs e)
        {
            IceTabPage current = GetTabPage(m_SelectedIndex);
            if (current != null)
            {
                //System.Diagnostics.Debug.WriteLine("close tab:" + current.TabCaption);
                if (this.OnTabClosed != null)
                    OnTabClosed(SelectedIndex);
            }
        }

        protected override void OnPaint(PaintEventArgs e) 
        {
            //base.OnPaint(e);
            if (FormMain.Instance != null)
                DrawControl(e.Graphics);
        }


        internal void DrawControl(Graphics g) 
        {
            try
            {
                this.BackColor = IrcColor.colors[FormMain.Instance.IceChatColors.TabbarBackColor];
                this.m_pnlCloseButton.BackColor = IrcColor.colors[FormMain.Instance.IceChatColors.TabbarBackColor];

                if (this.m_lTabPages.Count == 0) return;

                if (this.m_lTabPages.Count != 0 && m_SelectedIndex == -1)
                    SelectedIndex = 0;

                if (this.m_SelectedIndex > (m_lTabPages.Count - 1))
                    SelectedIndex = 0;

                CalculateTabSizes(g);                

                Region rsaved = g.Clip;
                for (int i = 0; i < m_lTabPages.Count; i++)
                {
                    m_lTabPages[i].Location = new Point(m_TabStartXPos, ((m_TabRowHeight + 7) * m_TotalTabRows));
                    if (!this.Controls.Contains(m_lTabPages[i]))
                        this.Controls.Add(m_lTabPages[i]);

                    DrawTab(g, m_lTabPages[i], i);
                }

                g.Clip = rsaved;

                if (GetTabPage(m_SelectedIndex) != null)
                    GetTabPage(m_SelectedIndex).BringToFront();

                DrawCloseButton();
            }
            catch (Exception e) 
            {
                //System.Diagnostics.Debug.WriteLine(e.Message + ":" + e.StackTrace);
                MessageBox.Show(e.StackTrace);
                FormMain.Instance.WriteErrorFile("IceTabControl DrawControl Error:" + e.Message, e.StackTrace);
            }
        }

        internal void DrawTab(Graphics g, IceTabPage tabPage, int nIndex) 
        {
            try
            {
                Rectangle recBounds = m_TabSizeRects[nIndex];
                Rectangle tabTextArea = m_TabTextRects[nIndex];

                Brush br;
                Point[] pt;

                bool bSelected = (this.m_SelectedIndex == nIndex);
                bool bHovered = (this.m_iHoveredIndex == nIndex);

                if (bSelected)
                    br = new LinearGradientBrush(recBounds, IrcColor.colors[FormMain.Instance.IceChatColors.TabBarCurrentBG1], IrcColor.colors[FormMain.Instance.IceChatColors.TabBarCurrentBG2], 90);
                else if (bHovered)
                    br = new LinearGradientBrush(recBounds, IrcColor.colors[FormMain.Instance.IceChatColors.TabBarHoverBG1], IrcColor.colors[FormMain.Instance.IceChatColors.TabBarHoverBG2], 90);
                else
                    br = new LinearGradientBrush(recBounds, IrcColor.colors[FormMain.Instance.IceChatColors.TabBarOtherBG1], IrcColor.colors[FormMain.Instance.IceChatColors.TabBarOtherBG2], 90);

                pt = new Point[7];
                pt[0] = new Point(recBounds.Left + 1, recBounds.Bottom);
                if (bSelected)
                {
                    pt[1] = new Point(recBounds.Left + 1, recBounds.Top + 3);
                    pt[2] = new Point(recBounds.Left + 4, recBounds.Top);
                    pt[3] = new Point(recBounds.Right - 4, recBounds.Top);
                    pt[4] = new Point(recBounds.Right - 1, recBounds.Top + 3);
                }
                else
                {
                    pt[1] = new Point(recBounds.Left + 1, recBounds.Top + 6);
                    pt[2] = new Point(recBounds.Left + 4, recBounds.Top + 3);
                    pt[3] = new Point(recBounds.Right - 4, recBounds.Top + 3);
                    pt[4] = new Point(recBounds.Right - 1, recBounds.Top + 6);
                }
                pt[5] = new Point(recBounds.Right - 1, recBounds.Bottom);
                pt[6] = new Point(recBounds.Left + 1, recBounds.Bottom);


                g.FillPolygon(br, pt);
                // draw the border around the control
                g.DrawPolygon(new Pen(Color.Black, 1), pt);

                br.Dispose();
                Image img = null;

                switch (tabPage.WindowStyle)
                {
                    case IceTabPage.WindowType.Console:
                        img = this.ImageList.Images[0];
                        break;
                    case IceTabPage.WindowType.Channel:
                        img = this.ImageList.Images[1];
                        break;
                    case IceTabPage.WindowType.Query:
                        img = this.ImageList.Images[2];
                        break;
                    case IceTabPage.WindowType.Debug:
                        img = this.ImageList.Images[3];
                        break;
                    default:
                        img = this.ImageList.Images[3];
                        break;

                }
                
                Rectangle rimage = new Rectangle(recBounds.X, recBounds.Y, img.Width, img.Height);
                if (bSelected)
                {
                    rimage.Offset(4, 4);
                    g.DrawImage(img, rimage);
                }
                else
                {
                    rimage.Offset(4, 6);
                    g.DrawImage(img, rimage);
                }

                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Near;
                stringFormat.LineAlignment = StringAlignment.Center;

                //get the tab text color
                if (bSelected)
                {
                    br = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.TabBarCurrent]);
                    tabPage.LastMessageType = FormMain.ServerMessageType.Default;
                }
                else if (bHovered)
                {
                    br = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.TabBarCurrent]);
                }
                else
                {
                    switch (tabPage.LastMessageType)
                    {
                        case FormMain.ServerMessageType.JoinChannel:
                            br = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.TabBarChannelJoin]);
                            break;
                        case FormMain.ServerMessageType.PartChannel:
                            br = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.TabBarChannelPart]);
                            break;
                        case FormMain.ServerMessageType.Message:
                        case FormMain.ServerMessageType.Action:
                            br = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.TabBarNewMessage]);
                            break;
                        case FormMain.ServerMessageType.QuitServer:
                            br = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.TabBarServerQuit]);
                            break;
                        case FormMain.ServerMessageType.ServerMessage:
                            br = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.TabBarServerMessage]);
                            break;
                        case FormMain.ServerMessageType.Other:
                            br = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.TabBarOtherMessage]);
                            break;
                        default:
                            br = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.TabBarDefault]);
                            break;
                    }
                }
                if (tabPage.TabCaption != "Console")
                {
                    if (tabPage.WindowStyle != IceTabPage.WindowType.ChannelList)
                        g.DrawString(tabPage.TabCaption, m_fontTab, br, tabTextArea, stringFormat);
                    else
                        g.DrawString(tabPage.TabCaption + "(" + tabPage.TotalChannels + ")", m_fontTab, br, tabTextArea, stringFormat);

                }
                else
                {
                    g.DrawString(FormMain.Instance.IceChatLanguage.consoleTabTitle, m_fontTab, br, tabTextArea, stringFormat);
                }

                g.DrawString((nIndex + 1).ToString(), new Font("Arial", 8.0F), br, recBounds);
            }            
            catch (Exception e) 
            { 
                FormMain.Instance.WriteErrorFile("IceTabControl DrawTab Error:" + e.Message + ":" + nIndex, e.StackTrace);
            }
        }


        internal void DrawCloseButton()
        {
            Graphics g = m_pnlCloseButton.CreateGraphics();

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            Rectangle m_CloseButton = new Rectangle(0, 0, 20, 20);
            g.FillRectangle(new SolidBrush(m_pnlCloseButton.BackColor), m_CloseButton);
            g.DrawString("X", this.Font, new SolidBrush(Color.Black), m_CloseButton, stringFormat);

            g.Dispose();
        }

        internal void DrawCloseButtonHover()
        {
            Graphics g = m_pnlCloseButton.CreateGraphics();

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            Rectangle m_CloseButton = new Rectangle(0, 0, 20, 20);
            g.FillRectangle(new SolidBrush(SystemColors.GradientActiveCaption), m_CloseButton);
            g.DrawRectangle(new Pen(SystemColors.ActiveBorder), m_CloseButton);
            g.DrawString("X", this.Font, new SolidBrush(Color.Black), m_CloseButton, stringFormat);

            g.Dispose();
        }

        private void CalculateTabSizes(Graphics g)
        {
            try
            {
                m_TabSizeRects.Clear();
                m_TabTextRects.Clear();

                m_TotalTabRows = 1;

                int totalWidth = 0;
                int xPos = m_TabStartXPos;
                int yPos = 0;

                m_TabRowHeight = (int)g.MeasureString("0", m_fontTab).Height + 4;
                if ((m_TabRowHeight / 2) * 2 == m_TabRowHeight)
                    m_TabRowHeight++;

                for (int i = 0; i < m_lTabPages.Count; i++)
                {

                    Rectangle recBounds = new Rectangle();
                    Rectangle recTextArea = new Rectangle();

                    //caclulate the width of the text
                    int textWidth = (int)g.MeasureString(m_lTabPages[i].TabCaption, m_fontTab).Width;
                    if (m_lTabPages[i].WindowStyle == IceTabPage.WindowType.ChannelList)
                    {
                        textWidth += (int)g.MeasureString(" (" + m_lTabPages[i].TotalChannels + ") ", m_fontTab).Width;
                    }
                    recBounds.Width = textWidth + 26;
                    recBounds.Height = m_TabRowHeight + 5;

                    recTextArea.Width = textWidth + 1;
                    recTextArea.Height = (int)g.MeasureString(m_lTabPages[i].TabCaption, m_fontTab).Height + 10;

                    if (totalWidth > 0 && ((totalWidth + recBounds.Width) > (this.Width - 30)))
                    {
                        m_TotalTabRows++;
                        totalWidth = 0;
                        xPos = m_TabStartXPos;
                        yPos = yPos + m_TabRowHeight + 5;
                    }

                    recBounds.X = xPos;
                    recBounds.Y = yPos;

                    recTextArea.X = xPos + 21;  //add area for image and a little extra
                    recTextArea.Y = yPos;

                    m_TabSizeRects.Add(i, recBounds);
                    m_TabTextRects.Add(i, recTextArea);

                    xPos = xPos + recBounds.Width;
                    totalWidth = totalWidth + recBounds.Width;


                }
                for (int i = 0; i < m_lTabPages.Count; i++)
                {
                    m_lTabPages[i].Width = this.Width;
                    m_lTabPages[i].Height = this.Height - ((m_TabRowHeight + 7) * m_TotalTabRows);
                }
            }
            catch (Exception e)
            {
                FormMain.Instance.WriteErrorFile("CalculateTabSizes:" + e.Message, e.StackTrace);
            }
        }


        private void OnControlAdded(object sender, ControlEventArgs e) 
        {
            /*  //not used for the time being
            if (e.Control is IceTabPage)
            {
                IceTabPage page = (IceTabPage)e.Control;

                if (page.WindowStyle == IceTabPage.WindowType.Console)
                    page.IconImg = this.ImageList.Images[0];
                
                if (page.WindowStyle == IceTabPage.WindowType.Channel)
                    page.IconImg = this.ImageList.Images[1];

                if (page.WindowStyle == IceTabPage.WindowType.Query)
                    page.IconImg = this.ImageList.Images[2];

                if (page.WindowStyle == IceTabPage.WindowType.Debug)
                    page.IconImg = this.ImageList.Images[3];

                m_lTabPages.Add((IceTabPage)e.Control);

                Invalidate();
            }
            */
        }

        private void OnControlRemoved(object sender, ControlEventArgs e) 
        {
            if (e.Control is IceTabPage) 
            {                
                m_lTabPages.Remove((IceTabPage)e.Control);
                ((IceTabPage)e.Control).Dispose();
                SelectedIndex = m_PreviousSelectedIndex;
                
                Invalidate();
                FormMain.Instance.ServerTree.Invalidate();
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e) 
        {
            if (m_TabSizeRects.Count == 0)
                return;

            if (e.Button == MouseButtons.Left)
            {
                Rectangle r = new Rectangle(DragStartPosition, Size.Empty);

                r.Inflate(SystemInformation.DragSize);

                if (drag_Tab != null)
                {
                    if (!r.Contains(e.X, e.Y))
                    {
                        IceTabPage hover_Tab = HoverTab(e.Location);
                        if (hover_Tab != null)
                        {
                            SwapTabPages(drag_Tab, hover_Tab);
                            drag_Tab = setSelectedByClickLocation(e.Location);
                            Invalidate();
                        }
                    }
                }

                DragStartPosition = Point.Empty;
                return;
            }


            if (e.Y < m_TabSizeRects[0].Y + 3 || e.Y > m_TabSizeRects[m_TabSizeRects.Count-1].Y + m_TabSizeRects[0].Height) 
            {
                m_iHoveredIndex = -1;
                Invalidate();
                return;
            }


            int iHoveredIndexBeforeClick = m_iHoveredIndex;

            for (int i = 0; i < m_TabSizeRects.Count; i++) 
            {
                Rectangle rectTab = m_TabSizeRects[i];
                if ((e.X > rectTab.X && e.X < (rectTab.X + rectTab.Width)) && (e.Y > rectTab.Y && e.Y < (rectTab.Y + rectTab.Height))) 
                {
                    if (this.m_iHoveredIndex != i) 
                        this.m_iHoveredIndex = i;
                    break;
                }
            }

            if (m_iHoveredIndex == iHoveredIndexBeforeClick)
                return;

            Invalidate();

        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            m_iHoveredIndex = -1;
            Invalidate();
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            DragStartPosition = new Point(e.X, e.Y);
            drag_Tab = setSelectedByClickLocation(e.Location);
        }
        
        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //show the proper popup menu according to what kind of tab
                if (GetTabPage(m_SelectedIndex).TabCaption == "Console")
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
                    
                    if (GetTabPage(m_SelectedIndex).WindowStyle == IceTabPage.WindowType.Channel)
                    {
                        popupMenu.ItemClicked -= new ToolStripItemClickedEventHandler(OnPopupMenu_ItemClicked);
                        popupMenu.Items.Clear();
                        popupMenu = ChannelPopupMenu();
                        popupMenu.ItemClicked += new ToolStripItemClickedEventHandler(OnPopupMenu_ItemClicked);
                        popupMenu.Show(this, e.Location);
                    }
                    else if (GetTabPage(m_SelectedIndex).WindowStyle == IceTabPage.WindowType.Query)
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

        private void SwapTabPages(IceTabPage drag, IceTabPage hover)
        {
            int Index1 = this.TabPages.IndexOf(drag);
            int Index2 = this.TabPages.IndexOf(hover);
            
            if (Index1 == Index2) return;
                       
            this.TabPages.Remove(drag);
            this.TabPages.Insert(Index2, drag);
        }
       
        private IceTabPage HoverTab(Point pClickLocation)
        {
            for (int i = 0; i < m_TabSizeRects.Count; i++)
            {
                Rectangle rectTab = m_TabSizeRects[i];
                if ((pClickLocation.X > rectTab.X && pClickLocation.X < rectTab.X + rectTab.Width) && (pClickLocation.Y > rectTab.Y && pClickLocation.Y < rectTab.Bottom))
                    return GetTabPage(i);
            }
            return null;
        }


        private IceTabPage setSelectedByClickLocation(Point pClickLocation) 
        {
            if (m_TabSizeRects.Count == 0) return null;
            
            for (int i = 0; i < m_TabSizeRects.Count; i++) 
            {
                Rectangle rectTab = m_TabSizeRects[i];
                if ((pClickLocation.X > rectTab.X && pClickLocation.X < rectTab.X + rectTab.Width) && (pClickLocation.Y > rectTab.Y && pClickLocation.Y < rectTab.Bottom)  ) 
                {
                    if (this.m_SelectedIndex != i) 
                        this.m_SelectedIndex = i;
                    break;
                }
            }

            if (GetTabPage(m_SelectedIndex) != null)
            {
                GetTabPage(m_SelectedIndex).BringToFront();
                if (this.SelectedIndexChanged != null)
                {
                    TabEventArgs e = new TabEventArgs();
                    SelectedIndexChanged(this, e);
                }
            }

            Invalidate();

            return GetTabPage(m_SelectedIndex);
        }

    }
    public class TabEventArgs : System.EventArgs
    {
        public bool IsHandled;
    }
}
