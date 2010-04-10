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
        
        private int headerHeight = 23;
        private int selectedIndex = -1;
        private string headerCaption = "";

        private ToolTip toolTip;
        private int toolTipNode = -1;
        private bool docked = false;
        private int oldDockWidth = 0;

        private ArrayList sortedNicks = null;
        
        private ContextMenuStrip popupMenu;

        public NickList()
        {
            InitializeComponent();

            this.MouseUp += new MouseEventHandler(OnMouseUp);
            this.MouseDown += new MouseEventHandler(OnMouseDown);
            //this.MouseMove += new MouseEventHandler(OnMouseMove);
            this.DoubleClick += new EventHandler(OnDoubleClick);
            this.Paint += new PaintEventHandler(OnPaint);
            this.FontChanged += new EventHandler(OnFontChanged);
            this.panelButtons.Resize += new EventHandler(panelButtons_Resize);
            
            this.vScrollBar.Scroll += new ScrollEventHandler(OnScroll);
            this.DoubleBuffered = true;            

            SetStyle(ControlStyles.ResizeRedraw |  ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            this.UpdateStyles();

            toolTip = new ToolTip();
            toolTip.AutoPopDelay = 3000;

            popupMenu = new ContextMenuStrip();
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

        private void OnDoubleClick(object sender, EventArgs e)
        {
            if (selectedIndex >= 0)
            {
                string nick = sortedNicks[selectedIndex].ToString();

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

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Y <= headerHeight)
            {
                if (e.X < 20)
                {
                    if (!docked)
                    {
                        oldDockWidth = ((Panel)this.Parent).Width;
                        FormMain.Instance.tabPanelRight.Visible = false;
                        FormMain.Instance.splitterRight.Visible = false;
                        docked = true;
                        panelButtons.Visible = false;
                        ((Panel)this.Parent).Width = 20;
                    }
                    else
                    {
                        FormMain.Instance.tabPanelRight.Visible = true;
                        FormMain.Instance.splitterRight.Visible = true;
                        docked = false;
                        panelButtons.Visible = true;
                        ((Panel)this.Parent).Width = oldDockWidth;
                    }
                }
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
                    selectedIndex = nickNumber;
                else
                    selectedIndex = -1;

                g.Dispose();
                Invalidate();
            }
            
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
                        
                        string nick = sortedNicks[selectedIndex].ToString();
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
                                caption = menuItem.Substring(0,menuItem.IndexOf(':'));
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
                                    ((ToolStripMenuItem)popupMenu.Items[subMenu]).DropDownItems.Add(t);

                                t = null;
                            }
                        }
                        popupMenu.Show(this, e.Location);
                    }
                }
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

                if (nickNumber < currentWindow.Nicks.Count)
                {
                    if (toolTipNode != nickNumber)
                    {
                        //User u = currentWindow.GetNick(sortedNicks[nickNumber].ToString());                        
                        string nick = sortedNicks[nickNumber].ToString();
                        for (int i = 0; i < currentWindow.Connection.ServerSetting.StatusModes[1].Length; i++)
                            nick = nick.Replace(currentWindow.Connection.ServerSetting.StatusModes[1][i].ToString(), string.Empty);

                        InternalAddressList u = ((InternalAddressList)currentWindow.Connection.ServerSetting.IAL[nick]);
                        
                        toolTip.ToolTipTitle = "User Information";
                        if (u != null)
                        {
                            if (u.Host != null)
                                toolTip.SetToolTip(this, u.Nick + "\r\n" + u.Host);
                            else
                                toolTip.SetToolTip(this, u.Nick);
                        }
                        
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

        private void OnPaint(object sender, PaintEventArgs e)
        {
            try
            {
                //make the buffer we draw this all to
                Bitmap buffer = new Bitmap(this.Width, this.Height, e.Graphics);
                Graphics g = Graphics.FromImage(buffer);

                //draw the header
                g.Clear(IrcColor.colors[FormMain.Instance.IceChatColors.NickListBackColor]);
                //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                g.InterpolationMode = InterpolationMode.Low;
                g.SmoothingMode = SmoothingMode.HighSpeed;
                g.PixelOffsetMode = PixelOffsetMode.None;
                g.CompositingQuality = CompositingQuality.HighSpeed;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
                
                Font headerFont = new Font("Verdana", 10);

                if (!docked)
                {
                    Rectangle headerR = new Rectangle(0, 0, this.Width, headerHeight);
                    //get the header colors here
                    Brush l = new LinearGradientBrush(headerR, IrcColor.colors[FormMain.Instance.IceChatColors.PanelHeaderBG1], IrcColor.colors[FormMain.Instance.IceChatColors.PanelHeaderBG2], 300);
                    g.FillRectangle(l, headerR);

                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    Rectangle centered = headerR;
                    centered.Offset(0, (int)(headerR.Height - e.Graphics.MeasureString(headerCaption, headerFont).Height) / 2);

                    g.DrawString(headerCaption, headerFont, new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.PanelHeaderForeColor]), centered, sf);
                    g.DrawImageUnscaled(global::IceChat.Properties.Resources.pin, new Rectangle(0, 2, 16, 16));

                    //draw the nicks 
                    Rectangle listR = new Rectangle(0, headerHeight, this.Width, this.Height - headerHeight - panelButtons.Height);

                    if (currentWindow != null && currentWindow.WindowStyle == IceTabPage.WindowType.Channel)
                    {
                        if (sortedNicks != null)
                            sortedNicks = null;

                        sortedNicks = new ArrayList(currentWindow.Nicks.Values);
                        sortedNicks.Sort();

                        int currentY = listR.Y;
                        int _lineSize = Convert.ToInt32(this.Font.GetHeight(g));
                        string host = "";

                        int randColor = -1;

                        for (int i = topIndex; i < sortedNicks.Count; i++)
                        {
                            Brush b = null;
                            User u = currentWindow.GetNick(sortedNicks[i].ToString());
                            if (FormMain.Instance.IceChatColors.RandomizeNickColors)
                            {
                                randColor++;
                                if (randColor > 31)
                                    randColor = 0;

                                //make sure its not the same color as the background
                                if (randColor == FormMain.Instance.IceChatColors.NickListBackColor)
                                {
                                    randColor++;

                                    if (randColor > 31)
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
                            if (i == selectedIndex)
                            {
                                g.FillRectangle(new SolidBrush(SystemColors.Highlight), 0, currentY, this.Width, _lineSize);
                                b = null;
                                b = new SolidBrush(SystemColors.HighlightText);
                            }
                            //draw the nickname
                            g.DrawString(sortedNicks[i].ToString(), this.Font, b, 2, currentY);

                            //draw the host
                            if (currentWindow.Connection.ServerSetting.IAL.ContainsKey(u.NickName))
                            {
                                host = ((InternalAddressList)currentWindow.Connection.ServerSetting.IAL[u.NickName]).Host;
                                if (host.Length > 0)
                                    g.DrawString(host, this.Font, b, (this.Font.SizeInPoints * 14), currentY);
                            }
                            currentY += _lineSize;
                            if (currentY >= listR.Height + listR.Y)
                            {
                                vScrollBar.Maximum = sortedNicks.Count - ((listR.Height - _lineSize) / _lineSize);
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
                }
                else
                {
                    Rectangle headerR = new Rectangle(0, 0, this.Width, this.Height);                    
                    Brush l = new LinearGradientBrush(headerR, IrcColor.colors[FormMain.Instance.IceChatColors.PanelHeaderBG1], IrcColor.colors[FormMain.Instance.IceChatColors.PanelHeaderBG2], 30);
                    g.FillRectangle(l, headerR);
                    Matrix x = new Matrix();
                    x.Rotate(270);
                    g.Transform = x;
                    g.DrawImageUnscaled(global::IceChat.Properties.Resources.pin, new Rectangle(-24, 0, 16, 16));
                    x.Rotate(180);
                    g.Transform = x;
                    g.DrawString(headerCaption, headerFont, new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.PanelHeaderForeColor]), 30, -20);

                    l.Dispose();
                }
                
                //paint the buffer onto the usercontrol
                e.Graphics.DrawImageUnscaled(buffer, 0, 0);
                buffer.Dispose();
                g.Dispose();
                headerFont.Dispose();                    
            }
            catch (Exception ee)
            {
                FormMain.Instance.WriteErrorFile("NickList OnPaint Error:" + ee.Message, ee.StackTrace);
            }
        }

        /// <summary>
        /// Return focus back to the InputText Box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            FormMain.Instance.FocusInputBox();
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
                UpdateHeader("Channels(" + page.TotalChannels + ")");
            }
            else if (page.WindowStyle == IceTabPage.WindowType.DCCChat)
            {
                this.currentWindow = page;
                UpdateHeader("DCC Chat:" + page.TabCaption);
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


        internal bool Docked
        {
            get { return docked; }
        }

        private void buttonOp_Click(object sender, EventArgs e)
        {
            if (selectedIndex >= 0 && currentWindow.WindowStyle == IceTabPage.WindowType.Channel)
            {
                if (selectedIndex < sortedNicks.Count)
                {
                    string nick = sortedNicks[selectedIndex].ToString();
                    User u = currentWindow.GetNick(nick);
                    if (u != null)
                    {
                        //check if opped or not
                        for (int y = 0; y < u.Level.Length; y++)
                        {
                            if (currentWindow.Connection.ServerSetting.StatusModes[0][y] == 'o')
                            {
                                if (u.Level[y])
                                    FormMain.Instance.ParseOutGoingCommand(currentWindow.Connection, "/mode " + currentWindow.TabCaption + " -o " + u.NickName);
                                else
                                    FormMain.Instance.ParseOutGoingCommand(currentWindow.Connection, "/mode " + currentWindow.TabCaption + " +o " + u.NickName);
                            }
                        }
                    }
                }
            }
            FormMain.Instance.FocusInputBox();
        }

        private void buttonVoice_Click(object sender, EventArgs e)
        {
            if (selectedIndex >= 0 && currentWindow.WindowStyle == IceTabPage.WindowType.Channel)
            {
                if (selectedIndex < sortedNicks.Count)
                {
                    string nick = sortedNicks[selectedIndex].ToString();
                    User u = currentWindow.GetNick(nick);
                    if (u != null)
                    {
                        //check if voiced or not
                        for (int y = 0; y < u.Level.Length; y++)
                        {
                            if (currentWindow.Connection.ServerSetting.StatusModes[0][y] == 'v')
                            {
                                if (u.Level[y])
                                    FormMain.Instance.ParseOutGoingCommand(currentWindow.Connection, "/mode " + currentWindow.TabCaption + " -v " + u.NickName);
                                else
                                    FormMain.Instance.ParseOutGoingCommand(currentWindow.Connection, "/mode " + currentWindow.TabCaption + " +v " + u.NickName);
                            }
                        }
                    }
                }
            }
            FormMain.Instance.FocusInputBox();
        }

        private void buttonQuery_Click(object sender, EventArgs e)
        {
            if (selectedIndex >= 0 && currentWindow.WindowStyle == IceTabPage.WindowType.Channel)
            {
                if (selectedIndex < sortedNicks.Count)
                {
                    string nick = sortedNicks[selectedIndex].ToString();
                    for (int i = 0; i < FormMain.Instance.CurrentWindow.Connection.ServerSetting.StatusModes[1].Length; i++)
                        nick = nick.Replace(FormMain.Instance.CurrentWindow.Connection.ServerSetting.StatusModes[1][i].ToString(), string.Empty);

                    if (!FormMain.Instance.TabMain.WindowExists(FormMain.Instance.CurrentWindow.Connection, nick, IceTabPage.WindowType.Query))
                        FormMain.Instance.AddWindow(FormMain.Instance.CurrentWindow.Connection, nick, IceTabPage.WindowType.Query);
                    else
                        FormMain.Instance.TabMain.SelectTab(FormMain.Instance.GetWindow(currentWindow.Connection, nick, IceTabPage.WindowType.Query));
                }
            }
            FormMain.Instance.FocusInputBox();
        }

        private void buttonHop_Click(object sender, EventArgs e)
        {
            if (selectedIndex >= 0 && currentWindow.WindowStyle == IceTabPage.WindowType.Channel)
            {
                if (selectedIndex < sortedNicks.Count)
                {
                    string nick = sortedNicks[selectedIndex].ToString();
                    User u = currentWindow.GetNick(nick);
                    if (u != null)
                    {
                        //check if voiced or not
                        for (int y = 0; y < u.Level.Length; y++)
                        {
                            if (currentWindow.Connection.ServerSetting.StatusModes[0][y] == 'h')
                            {
                                if (u.Level[y])
                                    FormMain.Instance.ParseOutGoingCommand(currentWindow.Connection, "/mode " + currentWindow.TabCaption + " -h " + u.NickName);
                                else
                                    FormMain.Instance.ParseOutGoingCommand(currentWindow.Connection, "/mode " + currentWindow.TabCaption + " +h " + u.NickName);
                            }
                        }
                    }
                }
            }
            FormMain.Instance.FocusInputBox();
        }

        private void buttonInfo_Click(object sender, EventArgs e)
        {
            if (selectedIndex >= 0)
            {
                if (selectedIndex < sortedNicks.Count)
                {
                    string nick = sortedNicks[selectedIndex].ToString();
                    for (int i = 0; i < FormMain.Instance.CurrentWindow.Connection.ServerSetting.StatusModes[1].Length; i++)
                        nick = nick.Replace(FormMain.Instance.CurrentWindow.Connection.ServerSetting.StatusModes[1][i].ToString(), string.Empty);

                    FormMain.Instance.ParseOutGoingCommand(currentWindow.Connection, "/userinfo " + nick);
                }
            }
            FormMain.Instance.FocusInputBox();
        }

        private void buttonBan_Click(object sender, EventArgs e)
        {
            if (selectedIndex >= 0 && currentWindow.WindowStyle == IceTabPage.WindowType.Channel)
            {
                if (selectedIndex < sortedNicks.Count)
                {
                    string nick = sortedNicks[selectedIndex].ToString();
                    for (int i = 0; i < FormMain.Instance.CurrentWindow.Connection.ServerSetting.StatusModes[1].Length; i++)
                        nick = nick.Replace(FormMain.Instance.CurrentWindow.Connection.ServerSetting.StatusModes[1][i].ToString(), string.Empty);

                    FormMain.Instance.ParseOutGoingCommand(currentWindow.Connection, "/mode " + currentWindow.TabCaption + " +b " + nick); ;
                }
            }
            FormMain.Instance.FocusInputBox();
        }

        private void buttonKick_Click(object sender, EventArgs e)
        {
            if (selectedIndex >= 0 && currentWindow.WindowStyle == IceTabPage.WindowType.Channel)
            {
                if (selectedIndex < sortedNicks.Count)
                {
                    string nick = sortedNicks[selectedIndex].ToString();
                    for (int i = 0; i < FormMain.Instance.CurrentWindow.Connection.ServerSetting.StatusModes[1].Length; i++)
                        nick = nick.Replace(FormMain.Instance.CurrentWindow.Connection.ServerSetting.StatusModes[1][i].ToString(), string.Empty);

                    FormMain.Instance.ParseOutGoingCommand(currentWindow.Connection, "/kick " + currentWindow.TabCaption + " " + nick); ;
                }
            }
            FormMain.Instance.FocusInputBox();
        }

        private void buttonWhois_Click(object sender, EventArgs e)
        {
            if (selectedIndex >= 0)
            {
                if (selectedIndex < sortedNicks.Count)
                {
                    string nick = sortedNicks[selectedIndex].ToString();
                    for (int i = 0; i < FormMain.Instance.CurrentWindow.Connection.ServerSetting.StatusModes[1].Length; i++)
                        nick = nick.Replace(FormMain.Instance.CurrentWindow.Connection.ServerSetting.StatusModes[1][i].ToString(), string.Empty);

                    FormMain.Instance.ParseOutGoingCommand(currentWindow.Connection, "/whois " + nick); ;
                }
            }
            FormMain.Instance.FocusInputBox();
        }

    }
}
