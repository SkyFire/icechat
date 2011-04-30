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

namespace IceChat
{
    public partial class NickList : UserControl
    {
        private delegate void AddNickCallBack(User nick);
        private delegate void ClearListDelegate();
        private delegate void UpdateHeaderDelegate(string data);

        private IceTabPage currentWindow;
        
        //the index of the top item in the nick list
        private int topIndex = 0;
        private bool mouseFocus = false;
        private bool controlKeyDown = false;

        private int headerHeight = 23;
        private int selectedIndex = -1;
        private string headerCaption = "";

        private ToolTip toolTip;
        private int toolTipNode = -1;

        private ArrayList sortedNickNames = null;
        private int totalSelected = 0;

        private ContextMenuStrip popupMenu;

        internal class Nick : IComparable
        {
            public string nick;
            public string host;
            public bool selected;
            public int nickColor;
            public bool[] Level;

            public int CompareTo(object obj)
            {
                Nick u = (Nick)obj;

                int compareNickValue = 0;
                int thisNickValue = 0;

                bool[] userCompareLevel = new bool[u.Level.Length];
                bool[] thisCompareLevel = new bool[this.Level.Length];

                for (int i = 0; i < userCompareLevel.Length; i++)
                    userCompareLevel[i] = u.Level[i];

                for (int i = 0; i < thisCompareLevel.Length; i++)
                    thisCompareLevel[i] = this.Level[i];

                Array.Reverse(userCompareLevel);
                Array.Reverse(thisCompareLevel);

                for (int i = userCompareLevel.Length - 1; i >= 0; i--)
                {
                    if (userCompareLevel[i])
                    {
                        compareNickValue = i + 1;
                        break;
                    }
                }

                for (int i = thisCompareLevel.Length - 1; i >= 0; i--)
                {
                    if (thisCompareLevel[i])
                    {
                        thisNickValue = i + 1;
                        break;
                    }
                }

                if (compareNickValue > thisNickValue)
                    return 1;
                else if (compareNickValue == thisNickValue)
                    return this.nick.CompareTo(u.nick);
                else
                    return -1;

            }

            public override string ToString()
            {
                return this.nick;
            }
        }

        public NickList()
        {
            InitializeComponent();
            this.MouseUp += new MouseEventHandler(OnMouseUp);
            this.MouseDown += new MouseEventHandler(OnMouseDown);
            //this.MouseMove += new MouseEventHandler(OnMouseMove);
            this.MouseWheel += new MouseEventHandler(OnMouseWheel);            
            this.DoubleClick += new EventHandler(OnDoubleClick);
            this.Paint += new PaintEventHandler(OnPaint);
            this.Resize += new EventHandler(OnResize);            
            this.FontChanged += new EventHandler(OnFontChanged);
            this.panelButtons.Resize += new EventHandler(panelButtons_Resize);
            this.KeyDown += new KeyEventHandler(OnKeyDown);
            this.KeyUp += new KeyEventHandler(OnKeyUp);
            this.vScrollBar.Scroll += new ScrollEventHandler(OnScroll);
            this.DoubleBuffered = true;

            this.MouseEnter += new EventHandler(OnMouseEnter);
            this.MouseLeave += new EventHandler(OnMouseLeave);

            SetStyle(ControlStyles.ResizeRedraw |  ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            this.UpdateStyles();

            toolTip = new ToolTip();
            toolTip.AutoPopDelay = 3000;

            popupMenu = new ContextMenuStrip();
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            mouseFocus = false;
        }

        private void OnMouseEnter(object sender, EventArgs e)
        {
            mouseFocus = true;
        }

        internal bool MouseHasFocus
        {
            get { return mouseFocus; }
        }

        //this is to make the arrow keys work in the user control
        protected override bool IsInputKey(Keys AKeyData)
        {
            return true;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
                controlKeyDown = true;
            
            if (e.KeyCode == Keys.Up)
            {
                selectedIndex--;
                Invalidate();
            }
            else if (e.KeyCode == Keys.Down)
            {
                selectedIndex++;
                Invalidate();
            }
            else if (e.KeyCode == Keys.Apps)
            {
                //right mouse key
                this.OnMouseUp(new MouseEventArgs(MouseButtons.Right, 1, 0, 0, 0));
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            controlKeyDown = false;
        }

        private void OnResize(object sender, EventArgs e)
        {
            this.vScrollBar.Left = this.Width - this.vScrollBar.Width;
            this.vScrollBar.Height = this.Height - this.headerHeight - this.panelButtons.Height;
        }

        internal void ApplyLanguage()
        {
            IceChatLanguage iceChatLanguage = FormMain.Instance.IceChatLanguage;
            if (FormMain.Instance.TabMain.CurrentTab == FormMain.Instance.TabMain.GetTabPage("Console")) Header = FormMain.Instance.IceChatLanguage.consoleTabTitle;
            // TODO: add code to load button texts from language class
        }

        private void panelButtons_Resize(object sender, EventArgs e)
        {
            buttonOp.Width = (this.panelButtons.Width / 4) - 4;
            buttonVoice.Width = buttonOp.Width;
            buttonBan.Width = buttonOp.Width;
            buttonInfo.Width = buttonOp.Width;
            buttonHop.Width = buttonOp.Width;
            buttonQuery.Width = buttonOp.Width;
            buttonKick.Width = buttonOp.Width;
            buttonWhois.Width = buttonOp.Width;

            buttonVoice.Left = this.panelButtons.Width / 4 + 1;
            buttonQuery.Left = buttonVoice.Left;
            buttonBan.Left = this.panelButtons.Width / 2;
            buttonKick.Left = buttonBan.Left;
            buttonInfo.Left = this.panelButtons.Width / 4 * 3 + 1;
            buttonWhois.Left = buttonInfo.Left;
        }

        private void OnScroll(object sender, ScrollEventArgs e)
        {
            topIndex = e.NewValue;
            Invalidate();
        }

        internal void ScrollWindow(bool scrollUp)
        {
            if (scrollUp && (topIndex > 0))
            {                
                topIndex--;
                vScrollBar.Value--;
                Invalidate();
            }
            else if ((topIndex + vScrollBar.LargeChange) < vScrollBar.Maximum)
            {
                topIndex++;
                vScrollBar.Value++;
                Invalidate();
            }
        }

        private void OnDoubleClick(object sender, EventArgs e)
        {
            //check if header was double clicked
            Point p = this.PointToClient(Cursor.Position);
            if (p.Y <= headerHeight)
            {
                if (this.Parent.Parent.GetType() == typeof(TabPage))
                {
                    FormMain.Instance.UnDockPanel((Panel)this.Parent);
                    return;
                }
            }

            if (selectedIndex >= 0)
            {
                //string nick = sortedNicks[selectedIndex].ToString();
                string nick = ((Nick)sortedNickNames[selectedIndex]).nick;

                //replace any of the modes
                for (int i = 0; i < currentWindow.Connection.ServerSetting.StatusModes[1].Length; i++)
                    nick = nick.Replace(currentWindow.Connection.ServerSetting.StatusModes[1][i].ToString(), string.Empty);

                FormMain.Instance.ParseOutGoingCommand(currentWindow.Connection, "/query " + nick);
            }
        }

        private void OnFontChanged(object sender, EventArgs e)
        {
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
            {
                //de-select any previous items
                DeSelectAllNicks();

                totalSelected = 0;

                Invalidate();

                return;
            }
            
            if (currentWindow != null && currentWindow.WindowStyle == IceTabPage.WindowType.Channel)
            {
                //do the math
                Graphics g = this.CreateGraphics();

                int _lineSize = Convert.ToInt32(this.Font.GetHeight(g));
                //find the nickname number, add 1 to it to make it a non-zero value
                int nickNumber = Convert.ToInt32((e.Location.Y - headerHeight) / _lineSize) + topIndex;

                if (nickNumber < currentWindow.Nicks.Count)
                {
                    selectedIndex = nickNumber;
                    bool selected = ((Nick)sortedNickNames[selectedIndex]).selected;

                    if (selected)
                        totalSelected--;
                    else
                        totalSelected++;

                    //if the CTRL-Key is down, we can do a multi-select
                    if (controlKeyDown)
                    {
                        ((Nick)sortedNickNames[selectedIndex]).selected = !selected;
                        currentWindow.GetNick(((Nick)sortedNickNames[selectedIndex]).nick).Selected = !selected;
                    }
                    else
                    {
                        if (totalSelected > 1)
                        {
                            //deselect all the previous ones
                            DeSelectAllNicks();

                            totalSelected = 1;
                        }

                        ((Nick)sortedNickNames[selectedIndex]).selected = !selected;
                        currentWindow.GetNick(((Nick)sortedNickNames[selectedIndex]).nick).Selected = !selected;

                    }
                }
                else
                {
                    DeSelectAllNicks();
                    totalSelected = 0;                
                    selectedIndex = -1;
                }

                g.Dispose();
                Invalidate();
            }
            
        }

        private void DeSelectAllNicks()
        {
            if (sortedNickNames == null) return;

            for (int i = 0; i < sortedNickNames.Count; i++)
            {
                ((Nick)sortedNickNames[i]).selected = false;
                currentWindow.GetNick(((Nick)sortedNickNames[i]).nick).Selected = false;
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Y <= headerHeight)
                return;

            if (currentWindow != null && currentWindow.WindowStyle == IceTabPage.WindowType.Channel)
            {
                Graphics g = this.CreateGraphics();

                int _lineSize = Convert.ToInt32(this.Font.GetHeight(g));
                //find the nickname number, add 1 to it to make it a non-zero value
                int nickNumber = Convert.ToInt32((e.Location.Y - headerHeight) / _lineSize) + topIndex;

                if (nickNumber < sortedNickNames.Count)
                {
                    if (toolTipNode != nickNumber)
                    {
                        string nick = ((Nick)sortedNickNames[nickNumber]).nick;
                        for (int i = 0; i < currentWindow.Connection.ServerSetting.StatusModes[1].Length; i++)
                            nick = nick.Replace(currentWindow.Connection.ServerSetting.StatusModes[1][i].ToString(), string.Empty);

                        toolTip.ToolTipTitle = "User Information";
                        if (((Nick)sortedNickNames[nickNumber]).host.Length > 0)
                            toolTip.SetToolTip(this, nick + Environment.NewLine + ((Nick)sortedNickNames[nickNumber]).host);
                        else
                            toolTip.SetToolTip(this, nick);

                        toolTipNode = nickNumber;
                    }
                }
                else
                {
                    toolTipNode = -1;
                    toolTip.RemoveAll();
                }
                g.Dispose();
            }
            
        }

        private void OnPopupMenuClick(object sender, EventArgs e)
        {
            if (((ToolStripMenuItem)sender).Tag == null) return;

            string command = ((ToolStripMenuItem)sender).Tag.ToString();
            FormMain.Instance.ParseOutGoingCommand(currentWindow.Connection, command);
        }

        private void PopulateNicks()
        {
            if (sortedNickNames != null)
            {
                sortedNickNames.Clear();
                sortedNickNames = null;
            }
            
            sortedNickNames = new ArrayList();
            
            try
            {
                foreach (User nick in currentWindow.Nicks.Values)
                {
                    Nick n = new Nick();
                    n.nick = nick.ToString();
                    n.selected = nick.Selected;
                    n.nickColor = nick.nickColor;
                    n.Level = nick.Level;

                    //System.Diagnostics.Debug.WriteLine(n.nick + ":" + nick.Level.Length + ":" + n.Level.Length);
                    if (currentWindow.Connection.ServerSetting.IAL.ContainsKey(nick.NickName))
                        n.host = ((InternalAddressList)currentWindow.Connection.ServerSetting.IAL[nick.NickName]).Host;
                    else
                        n.host = "";
                    
                    sortedNickNames.Add(n);

                }
                
                sortedNickNames.Sort();
                 
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message + ":" + e.Source);
            }
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            try
            {
                //make the buffer we draw this all to
                Bitmap buffer = new Bitmap(this.Width, this.Height, e.Graphics);
                Graphics g = Graphics.FromImage(buffer);

                //draw the header
                g.Clear(IrcColor.colors[FormMain.Instance.IceChatColors.NickListBackColor]);

                g.InterpolationMode = InterpolationMode.Low;
                g.SmoothingMode = SmoothingMode.HighSpeed;
                g.PixelOffsetMode = PixelOffsetMode.None;
                g.CompositingQuality = CompositingQuality.HighSpeed;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
                
                Font headerFont = new Font("Verdana", 10);

                
                Rectangle headerR = new Rectangle(0, 0, this.Width, headerHeight);
                //get the header colors here
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

                //draw the nicks 
                Rectangle listR = new Rectangle(0, headerHeight, this.Width, this.Height - headerHeight - panelButtons.Height);

                if (currentWindow != null && currentWindow.WindowStyle == IceTabPage.WindowType.Channel)
                {
                    //if (sortedNicks != null)
                    //    sortedNicks = null;

                    //sortedNicks = new ArrayList(currentWindow.Nicks.Values);
                    //sortedNicks.Sort();

                    PopulateNicks();

                    int currentY = listR.Y;
                    int _lineSize = Convert.ToInt32(this.Font.GetHeight(g));
                    //string host = "";

                    int randColor = -1;

                    for (int i = topIndex; i < sortedNickNames.Count; i++)
                    {
                        Brush b = null;
                        User u = currentWindow.GetNick(((Nick)sortedNickNames[i]).nick);
                        if (FormMain.Instance.IceChatColors.RandomizeNickColors)
                        {
                            randColor++;
                            if (randColor > (IrcColor.colors.Length-1))
                                randColor = 0;

                            //make sure its not the same color as the background
                            if (randColor == FormMain.Instance.IceChatColors.NickListBackColor)
                            {
                                randColor++;

                                if (randColor > (IrcColor.colors.Length-1))
                                    randColor = 0;
                            }

                            b = new SolidBrush(IrcColor.colors[randColor]);
                            u.nickColor = randColor;
                        }
                        else
                        {
                            //get the correct nickname color for channel status
                            for (int y = 0; y < u.Level.Length; y++)
                            {
                                if (u.Level[y])
                                {
                                    switch (currentWindow.Connection.ServerSetting.StatusModes[0][y])
                                    {
                                        case 'q':
                                            b = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.ChannelOwnerColor]);
                                            break;
                                        case 'a':
                                            b = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.ChannelAdminColor]);
                                            break;
                                        case 'o':
                                            b = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.ChannelOpColor]);
                                            break;
                                        case 'h':
                                            b = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.ChannelHalfOpColor]);
                                            break;
                                        case 'v':
                                            b = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.ChannelVoiceColor]);
                                            break;
                                        default:
                                            b = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.ChannelRegularColor]);
                                            break;
                                    }

                                    break;
                                }
                            }

                            if (b == null)
                                b = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.ChannelRegularColor]);
                        }
                        //check if selected, if so, draw the selector bar
                        //if (i == selectedIndex)
                        if (((Nick)sortedNickNames[i]).selected)
                        {
                            g.FillRectangle(new SolidBrush(SystemColors.Highlight), 0, currentY, this.Width, _lineSize);
                            b = null;
                            b = new SolidBrush(SystemColors.HighlightText);
                        }
                        
                        //draw the nickname
                        g.DrawString(((Nick)sortedNickNames[i]).nick, this.Font, b, 2, currentY);
                        
                        //draw the host
                        if (currentWindow.Connection.ServerSetting.IAL.ContainsKey(u.NickName))
                        {
                            //host = ((InternalAddressList)currentWindow.Connection.ServerSetting.IAL[u.NickName]).Host;
                            if (((Nick)sortedNickNames[i]).host.Length > 0)
                                g.DrawString(((Nick)sortedNickNames[i]).host, this.Font, b, (this.Font.SizeInPoints * 14), currentY);
                        }
                        
                        currentY += _lineSize;
                        if (currentY >= (listR.Height + listR.Y))
                        {
                            vScrollBar.Maximum = sortedNickNames.Count - 2;
                            vScrollBar.LargeChange = ((listR.Height - _lineSize) / _lineSize);
                            break;
                        }
                    }

                    if (currentY >= listR.Height || vScrollBar.Value > 0)
                        vScrollBar.Visible = true;
                    else
                    {
                        vScrollBar.Visible = false;
                    }
                }
                l.Dispose();
                sf.Dispose();
                
                //paint the buffer onto the usercontrol
                e.Graphics.DrawImageUnscaled(buffer, 0, 0);
                buffer.Dispose();
                g.Dispose();
                headerFont.Dispose();                    
            }
            catch (Exception ee)
            {
                FormMain.Instance.WriteErrorFile(currentWindow.Connection, "NickList OnPaint:" + currentWindow.Nicks.Values.Count, ee);
            }
        }

        /// <summary>
        /// Show the popup Menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && selectedIndex != -1)
            {
                //show the popup menu
                foreach (PopupMenuItem p in FormMain.Instance.IceChatPopupMenus.listPopups)
                {
                    if (p.PopupType == "NickList")
                    {
                        string[] menuItems = p.Menu;

                        //build the menu
                        ToolStripItem t;

                        int subMenu = 0;

                        popupMenu.Items.Clear();

                        string nick = sortedNickNames[selectedIndex].ToString();
                        //replace any of the modes
                        for (int i = 0; i < currentWindow.Connection.ServerSetting.StatusModes[1].Length; i++)
                            nick = nick.Replace(currentWindow.Connection.ServerSetting.StatusModes[1][i].ToString(), string.Empty);

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
                                if (currentWindow.WindowStyle == IceTabPage.WindowType.Channel)
                                {
                                    caption = caption.Replace("$chan", currentWindow.TabCaption);
                                    command = command.Replace("$chan", currentWindow.TabCaption);

                                    caption = caption.Replace(" # ", " " + currentWindow.TabCaption + " ");
                                    command = command.Replace(" # ", " " + currentWindow.TabCaption + " ");
                                }
                                caption = caption.Replace("$nick", nick);
                                command = command.Replace("$nick", nick);

                                if (caption == "-")
                                    t = new ToolStripSeparator();
                                else
                                {
                                    t = new ToolStripMenuItem(caption);

                                    //parse out the command/$identifiers                            
                                    command = command.Replace("$1", nick);
                                    command = command.Replace("$nick", nick);

                                    t.Click += new EventHandler(OnPopupMenuClick);
                                    t.Tag = command;
                                }
                                if (menuDepth == 0)
                                    subMenu = popupMenu.Items.Add(t);
                                else
                                    if (popupMenu.Items[subMenu].GetType() != typeof(ToolStripSeparator))
                                        ((ToolStripMenuItem)popupMenu.Items[subMenu]).DropDownItems.Add(t);

                                t = null;
                            }
                        }
                        popupMenu.Show(this, e.Location);
                    }
                }
            }            
        }
        
        internal void SelectNick(string nick)
        {
            //select a specific nickname in the nicklist
            for (int i = 0; i < sortedNickNames.Count; i++)
            {
                if (nick == sortedNickNames[i].ToString())
                {
                    //matched
                    DeSelectAllNicks();

                    selectedIndex = i;
                    int p = (selectedIndex / vScrollBar.LargeChange);

                    if ((topIndex + vScrollBar.LargeChange) < selectedIndex && vScrollBar.Visible)
                        topIndex += (p * vScrollBar.LargeChange);
                    else if ((topIndex > selectedIndex) && vScrollBar.Visible)
                        topIndex = (p * vScrollBar.LargeChange);

                    ((Nick)sortedNickNames[selectedIndex]).selected = true;
                    currentWindow.GetNick(((Nick)sortedNickNames[selectedIndex]).nick).Selected = true;

                    Invalidate();
                    return;
                }
                else if (nick == sortedNickNames[i].ToString().Substring(1))
                {
                    //matched
                    DeSelectAllNicks();

                    selectedIndex = i;
                    int p = (selectedIndex / vScrollBar.LargeChange);

                    if ((topIndex + vScrollBar.LargeChange) < selectedIndex && vScrollBar.Visible)
                        topIndex += (p * vScrollBar.LargeChange);
                    else if ((topIndex > selectedIndex) && vScrollBar.Visible)
                        topIndex = (p * vScrollBar.LargeChange);

                    ((Nick)sortedNickNames[selectedIndex]).selected = true;
                    currentWindow.GetNick(((Nick)sortedNickNames[selectedIndex]).nick).Selected = true;

                    Invalidate();
                    return;
                }
            }
        }

        /// <summary>
        /// Erase the Nicklist and change the Header to the Console
        /// </summary>
        /// <param name="cwindow"></param>
        internal void RefreshList()
        {
            this.currentWindow = null;
            UpdateHeader("Console");            
        }
        
        /// <summary>
        /// Refresh the Nicklist with the Current Channel/Query Selected
        /// </summary>
        /// <param name="window"></param>
        internal void RefreshList(IceTabPage page)
        {
            if (page.WindowStyle == IceTabPage.WindowType.Channel)
            {
                if (this.currentWindow != page)
                {
                    selectedIndex = -1;
                    topIndex = 0;

                    vScrollBar.Value = 0;
                    vScrollBar.Visible = false;
                }

                this.currentWindow = page;
                UpdateHeader(page.TabCaption + ":" + page.Nicks.Count);
            }
            else if (page.WindowStyle == IceTabPage.WindowType.Query)
            {
                this.currentWindow = page;
                UpdateHeader("Query:" + page.TabCaption);
            }
            else if (page.WindowStyle == IceTabPage.WindowType.ChannelList)
            {
                this.currentWindow = page;
                UpdateHeader("Channels (" + page.TotalChannels + ")");
            }
            else if (page.WindowStyle == IceTabPage.WindowType.DCCChat)
            {
                this.currentWindow = page;
                UpdateHeader("DCC Chat:" + page.TabCaption);
            }
            else if (page.WindowStyle == IceTabPage.WindowType.Window)
            {
                this.currentWindow = page;
                UpdateHeader(page.TabCaption);
            }
            else if (page.WindowStyle == IceTabPage.WindowType.DCCFile)
            {
                this.currentWindow = page;
                UpdateHeader(page.TabCaption);
            }
            else if (page.WindowStyle == IceTabPage.WindowType.Debug)
            {
                this.currentWindow = page;
                UpdateHeader("Debug");
            }
        }

        /// <summary>
        /// Update the Header of the Nick List
        /// </summary>
        /// <param name="data"></param>
        private void UpdateHeader(string data)
        {
            if (this.InvokeRequired)
            {
                UpdateHeaderDelegate u = new UpdateHeaderDelegate(UpdateHeader);
                this.Invoke(u, new object[] { data });
            }
            else
            {
                headerCaption = data;
                Invalidate();
            }
        }
        /// <summary>
        /// Returns total nicknames in NickList
        /// </summary>
        public int TotalNicks
        {
            get
            {
                if (currentWindow.WindowStyle == IceTabPage.WindowType.Channel)
                    return currentWindow.Nicks.Count;
                else
                    return 0;
            }
        }
        /// <summary>
        /// Returns the Header of the Nicklist
        /// </summary>
        public string Header
        {
            get
            {
                return headerCaption;
            }
            set
            {
                UpdateHeader(value);
            }
        }
        /// <summary>
        /// Returns the TabWindow which is currently being shown in the Nicklist
        /// </summary>
        internal IceTabPage CurrentWindow
        {
            get
            {
                return this.currentWindow;
            }
            set
            {
                this.currentWindow = value;
            }
        }

        private void buttonOp_Click(object sender, EventArgs e)
        {
            if (currentWindow == null) return;

            if (currentWindow.WindowStyle == IceTabPage.WindowType.Channel)
            {
                //check for all the selected nicks in the nick list
                if (selectedIndex < sortedNickNames.Count)
                {
                    int totalSelected = 0;
                    string addModes = "";
                    string removeModes = "";
                    string addNicks = "";
                    string removeNicks = "";
                    for (int i = 0; i < sortedNickNames.Count; i++)
                    {
                        if (((Nick)sortedNickNames[i]).selected == true)
                        {
                            if (totalSelected <= currentWindow.Connection.ServerSetting.MaxModes)
                            {
                                Nick u = ((Nick)sortedNickNames[i]);
                                string nickName = u.nick;
                                for (int x = 0; x < currentWindow.Connection.ServerSetting.StatusModes[1].Length; x++)
                                {
                                    if (nickName.StartsWith(currentWindow.Connection.ServerSetting.StatusModes[1][x].ToString()))
                                        nickName = nickName.Substring(1);
                                }

                                if (u != null)
                                {
                                    totalSelected++;

                                    //check if voiced or not
                                    for (int y = 0; y < u.Level.Length; y++)
                                    {
                                        if (currentWindow.Connection.ServerSetting.StatusModes[0][y] == 'o')
                                        {
                                            if (u.Level[y])
                                            {
                                                removeModes += "o";
                                                removeNicks += " " + nickName;
                                            }
                                            else
                                            {
                                                addModes += "o";
                                                addNicks += " " + nickName;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    string totalModes = "";
                    if (addModes.Length > 0)
                        totalModes += "+" + addModes;
                    if (removeModes.Length > 0)
                        totalModes += "-" + removeModes;
                    if (addNicks.Length > 0)
                        totalModes += addNicks;
                    if (removeNicks.Length > 0)
                        totalModes += removeNicks;

                    if (totalModes.Length > 0)
                        FormMain.Instance.ParseOutGoingCommand(currentWindow.Connection, "/mode " + currentWindow.TabCaption + " " + totalModes);

                }
            }

            FormMain.Instance.FocusInputBox();
        }

        private void buttonVoice_Click(object sender, EventArgs e)
        {
            if (currentWindow == null) return;

            if (currentWindow.WindowStyle == IceTabPage.WindowType.Channel)
            {
                //check for all the selected nicks in the nick list
                if (selectedIndex < sortedNickNames.Count)
                {
                    int totalSelected = 0;
                    string addModes = "";
                    string removeModes = "";
                    string addNicks = "";
                    string removeNicks = "";
                    for (int i = 0; i < sortedNickNames.Count; i++)
                    {
                        if (((Nick)sortedNickNames[i]).selected == true)
                        {
                            if (totalSelected <= currentWindow.Connection.ServerSetting.MaxModes)
                            {
                                Nick u = ((Nick)sortedNickNames[i]);
                                string nickName = u.nick;
                                for (int x = 0; x < currentWindow.Connection.ServerSetting.StatusModes[1].Length; x++)
                                {
                                    if (nickName.StartsWith(currentWindow.Connection.ServerSetting.StatusModes[1][x].ToString()))
                                        nickName = nickName.Substring(1);
                                }

                                if (u != null)
                                {
                                    totalSelected++;

                                    //check if voiced or not
                                    for (int y = 0; y < u.Level.Length; y++)
                                    {
                                        if (currentWindow.Connection.ServerSetting.StatusModes[0][y] == 'v')
                                        {
                                            if (u.Level[y])
                                            {
                                                removeModes += "v";
                                                removeNicks += " " + nickName;                                                                            
                                            } 
                                            else
                                            {
                                                addModes += "v";
                                                addNicks += " " + nickName;                                                                            
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    string totalModes = "";
                    if (addModes.Length > 0)
                        totalModes += "+" + addModes;
                    if (removeModes.Length > 0)
                        totalModes += "-" + removeModes;
                    if (addNicks.Length > 0)
                        totalModes += addNicks;
                    if (removeNicks.Length > 0)
                        totalModes += removeNicks;

                    if (totalModes.Length > 0)
                        FormMain.Instance.ParseOutGoingCommand(currentWindow.Connection, "/mode " + currentWindow.TabCaption + " " + totalModes);
                }
            }
            
            FormMain.Instance.FocusInputBox();
        }

        private void buttonQuery_Click(object sender, EventArgs e)
        {
            if (currentWindow == null) return;

            if (currentWindow.WindowStyle == IceTabPage.WindowType.Channel)
            {
                //check for all the selected nicks in the nick list
                if (selectedIndex < sortedNickNames.Count)
                {
                    for (int i = 0; i < sortedNickNames.Count; i++)
                    {
                        if (((Nick)sortedNickNames[i]).selected == true)
                        {
                            Nick u = ((Nick)sortedNickNames[i]);
                            string nickName = u.nick;
                            for (int x = 0; x < currentWindow.Connection.ServerSetting.StatusModes[1].Length; x++)
                            {
                                if (nickName.StartsWith(currentWindow.Connection.ServerSetting.StatusModes[1][x].ToString()))
                                    nickName = nickName.Substring(1);
                            }


                            if (!FormMain.Instance.TabMain.WindowExists(FormMain.Instance.CurrentWindow.Connection, nickName, IceTabPage.WindowType.Query))
                                FormMain.Instance.AddWindow(FormMain.Instance.CurrentWindow.Connection, nickName, IceTabPage.WindowType.Query);
                            else
                                FormMain.Instance.TabMain.SelectTab(FormMain.Instance.GetWindow(currentWindow.Connection, nickName, IceTabPage.WindowType.Query));

                            return;
                        }
                    }
                }
            }
            FormMain.Instance.FocusInputBox();
        }

        private void buttonHop_Click(object sender, EventArgs e)
        {
            if (currentWindow == null) return;

            if (currentWindow.WindowStyle == IceTabPage.WindowType.Channel)
            {
                //check for all the selected nicks in the nick list
                if (selectedIndex < sortedNickNames.Count)
                {
                    int totalSelected = 0;
                    string addModes = "";
                    string removeModes = "";
                    string addNicks = "";
                    string removeNicks = "";
                    for (int i = 0; i < sortedNickNames.Count; i++)
                    {
                        if (((Nick)sortedNickNames[i]).selected == true)
                        {
                            if (totalSelected <= currentWindow.Connection.ServerSetting.MaxModes)
                            {
                                Nick u = ((Nick)sortedNickNames[i]);
                                string nickName = u.nick;
                                for (int x = 0; x < currentWindow.Connection.ServerSetting.StatusModes[1].Length; x++)
                                {
                                    if (nickName.StartsWith(currentWindow.Connection.ServerSetting.StatusModes[1][x].ToString()))
                                        nickName = nickName.Substring(1);
                                }

                                if (u != null)
                                {
                                    totalSelected++;

                                    //check if voiced or not
                                    for (int y = 0; y < u.Level.Length; y++)
                                    {
                                        if (currentWindow.Connection.ServerSetting.StatusModes[0][y] == 'h')
                                        {
                                            if (u.Level[y])
                                            {
                                                removeModes += "h";
                                                removeNicks += " " + nickName;
                                            }
                                            else
                                            {
                                                addModes += "h";
                                                addNicks += " " + nickName;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    string totalModes = "";
                    if (addModes.Length > 0)
                        totalModes += "+" + addModes;
                    if (removeModes.Length > 0)
                        totalModes += "-" + removeModes;
                    if (addNicks.Length > 0)
                        totalModes += addNicks;
                    if (removeNicks.Length > 0)
                        totalModes += removeNicks;

                    if (totalModes.Length > 0)
                        FormMain.Instance.ParseOutGoingCommand(currentWindow.Connection, "/mode " + currentWindow.TabCaption + " " + totalModes);

                }
            }

            FormMain.Instance.FocusInputBox();
        }

        private void buttonInfo_Click(object sender, EventArgs e)
        {
            if (currentWindow == null) return;

            if (currentWindow.WindowStyle == IceTabPage.WindowType.Channel)
            {
                //check for all the selected nicks in the nick list
                if (selectedIndex < sortedNickNames.Count)
                {
                    for (int i = 0; i < sortedNickNames.Count; i++)
                    {
                        if (((Nick)sortedNickNames[i]).selected == true)
                        {
                            Nick u = ((Nick)sortedNickNames[i]);
                            string nickName = u.nick;
                            for (int x = 0; x < currentWindow.Connection.ServerSetting.StatusModes[1].Length; x++)
                            {
                                if (nickName.StartsWith(currentWindow.Connection.ServerSetting.StatusModes[1][x].ToString()))
                                    nickName = nickName.Substring(1);
                            }


                            FormMain.Instance.ParseOutGoingCommand(currentWindow.Connection, "/userinfo " + nickName);
                            
                            FormMain.Instance.FocusInputBox();
                            return;                            
                        }
                    }
                }
            }
            FormMain.Instance.FocusInputBox();
        }

        private void buttonBan_Click(object sender, EventArgs e)
        {
            if (currentWindow == null) return;

            if (currentWindow.WindowStyle == IceTabPage.WindowType.Channel)
            {
                //check for all the selected nicks in the nick list
                if (selectedIndex < sortedNickNames.Count)
                {
                    for (int i = 0; i < sortedNickNames.Count; i++)
                    {
                        if (((Nick)sortedNickNames[i]).selected == true)
                        {
                            Nick u = ((Nick)sortedNickNames[i]);
                            string nickName = u.nick;
                            for (int x = 0; x < currentWindow.Connection.ServerSetting.StatusModes[1].Length; x++)
                            {
                                if (nickName.StartsWith(currentWindow.Connection.ServerSetting.StatusModes[1][x].ToString()))
                                    nickName = nickName.Substring(1);
                            }


                            FormMain.Instance.ParseOutGoingCommand(currentWindow.Connection, "/mode " + currentWindow.TabCaption + " +b " + nickName); ;

                            FormMain.Instance.FocusInputBox();
                            return;
                        }
                    }
                }
            }
            FormMain.Instance.FocusInputBox();
        }

        private void buttonKick_Click(object sender, EventArgs e)
        {
            if (currentWindow == null) return;

            if (currentWindow.WindowStyle == IceTabPage.WindowType.Channel)
            {
                //check for all the selected nicks in the nick list
                if (selectedIndex < sortedNickNames.Count)
                {
                    for (int i = 0; i < sortedNickNames.Count; i++)
                    {
                        if (((Nick)sortedNickNames[i]).selected == true)
                        {
                            Nick u = ((Nick)sortedNickNames[i]);
                            string nickName = u.nick;
                            for (int x = 0; x < currentWindow.Connection.ServerSetting.StatusModes[1].Length; x++)
                            {
                                if (nickName.StartsWith(currentWindow.Connection.ServerSetting.StatusModes[1][x].ToString()))
                                    nickName = nickName.Substring(1);
                            }


                            FormMain.Instance.ParseOutGoingCommand(currentWindow.Connection, "/kick " + currentWindow.TabCaption + " " + nickName);

                            FormMain.Instance.FocusInputBox();
                            return;
                        }
                    }
                }
            }
            FormMain.Instance.FocusInputBox();
        }

        private void buttonWhois_Click(object sender, EventArgs e)
        {
            if (currentWindow == null) return;

            if (currentWindow.WindowStyle == IceTabPage.WindowType.Channel)
            {
                //check for all the selected nicks in the nick list
                if (selectedIndex < sortedNickNames.Count)
                {
                    for (int i = 0; i < sortedNickNames.Count; i++)
                    {
                        if (((Nick)sortedNickNames[i]).selected == true)
                        {
                            Nick u = ((Nick)sortedNickNames[i]);
                            string nickName = u.nick;
                            for (int x = 0; x < currentWindow.Connection.ServerSetting.StatusModes[1].Length; x++)
                            {
                                if (nickName.StartsWith(currentWindow.Connection.ServerSetting.StatusModes[1][x].ToString()))
                                    nickName = nickName.Substring(1);
                            }


                            FormMain.Instance.ParseOutGoingCommand(currentWindow.Connection, "/whois " + nickName); ;

                            FormMain.Instance.FocusInputBox();
                            return;
                        }
                    }
                }
            }
            FormMain.Instance.FocusInputBox();
        }

    }
}
