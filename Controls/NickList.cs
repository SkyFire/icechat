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

namespace IceChat2009
{
    public partial class NickList : UserControl
    {
        private delegate void AddNickCallBack(User nick);
        private delegate void ClearListDelegate();
        private delegate void UpdateHeaderDelegate(string data);

        private TabWindow currentWindow;
        
        //the index of the top item in the nick list
        private int topIndex = 0;
        
        private int headerHeight = 23;
        private int selectedIndex = -1;
        private string headerCaption = "";

        private ArrayList sortedNicks = null;
        
        private ContextMenuStrip popupMenu;

        public NickList()
        {
            InitializeComponent();

            this.MouseUp += new MouseEventHandler(OnMouseUp);
            this.MouseDown += new MouseEventHandler(OnMouseDown);
            this.DoubleClick += new EventHandler(OnDoubleClick);
            this.Paint += new PaintEventHandler(OnPaint);
            this.FontChanged += new EventHandler(OnFontChanged);

            
            this.vScrollBar.Scroll += new ScrollEventHandler(OnScroll);
            this.DoubleBuffered = true;            

            SetStyle(ControlStyles.ResizeRedraw |  ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            this.UpdateStyles();

            popupMenu = new ContextMenuStrip();
        }

        private void OnScroll(object sender, ScrollEventArgs e)
        {
            topIndex = ((VScrollBar)sender).Value;
            Invalidate();
        }

        private void OnDoubleClick(object sender, EventArgs e)
        {
            if (selectedIndex >= 0)
            {
                string nick = sortedNicks[selectedIndex].ToString();

                //replace any of the modes
                for (int i = 0; i < FormMain.Instance.CurrentWindow.Connection.ServerSetting.StatusModes[1].Length; i++)
                    nick = nick.Replace(FormMain.Instance.CurrentWindow.Connection.ServerSetting.StatusModes[1][i].ToString(), string.Empty);

                if (!FormMain.Instance.TabMain.WindowExists(FormMain.Instance.CurrentWindow.Connection, nick, TabWindow.WindowType.Query))
                    FormMain.Instance.AddWindow(FormMain.Instance.CurrentWindow.Connection, nick, TabWindow.WindowType.Query);
                else
                    FormMain.Instance.TabMain.SelectedTab = FormMain.Instance.GetWindow(FormMain.Instance.CurrentWindow.Connection, nick, TabWindow.WindowType.Query);

            }
        }

        private void OnFontChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            
            if (e.Y <= headerHeight)
                return;
            
            if (currentWindow != null && currentWindow.WindowStyle == TabWindow.WindowType.Channel)
            {
                //do the math
                Graphics g = this.CreateGraphics();

                int lineSize = Convert.ToInt32(this.Font.GetHeight(g));
                //find the nickname number, add 1 to it to make it a non-zero value
                int nickNumber = Convert.ToInt32((e.Location.Y - headerHeight) / lineSize) + topIndex;

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
                        ToolStripMenuItem t;
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
                            
                            t = new ToolStripMenuItem(caption);
                            
                            //parse out the command/$identifiers                            
                            command = command.Replace("$1", nick);
                            command = command.Replace("$nick", nick);
                            
                            t.Click += new EventHandler(OnPopupMenuClick);
                            t.Tag = command;

                            if (menuDepth == 0)
                                subMenu = popupMenu.Items.Add(t);
                            else                               
                                ((ToolStripMenuItem)popupMenu.Items[subMenu]).DropDownItems.Add(t);
                            
                            t = null;
                        }
                        popupMenu.Show(this, e.Location);
                    }
                }
            }            


        }

        private void OnPopupMenuClick(object sender, EventArgs e)
        {
            if (((ToolStripMenuItem)sender).Tag == null) return;

            string command = ((ToolStripMenuItem)sender).Tag.ToString();
            //System.Diagnostics.Debug.WriteLine(command);
            FormMain.Instance.ParseOutGoingCommand(currentWindow.Connection, command);
            
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            //make the buffer we draw this all to
            Bitmap buffer = new Bitmap(this.Width, this.Height, e.Graphics);
            Graphics g = Graphics.FromImage(buffer);

            //draw the header
            g.Clear(IrcColor.colors[FormMain.Instance.IceChatColors.NickListBackColor]);           
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

            //draw the nicks            
            Rectangle listR = new Rectangle(0, headerHeight, this.Width, this.Height - headerHeight);
            
            if (currentWindow != null && currentWindow.WindowStyle == TabWindow.WindowType.Channel)
            {
                if (sortedNicks != null)
                    sortedNicks = null;

                sortedNicks = new ArrayList(currentWindow.Nicks.Values);
                sortedNicks.Sort();

                int currentY = listR.Y;
                int lineSize = Convert.ToInt32(this.Font.GetHeight(g));

                for (int i = topIndex; i < sortedNicks.Count; i++)
                {
                    Brush b = null;
                    //get the correct nickname color for channel status
                    User u = currentWindow.GetNick(sortedNicks[i].ToString());
                    for (int y = 0; y < u.Level.Length; y++)
                    {
                        if (u.Level[y])
                        {
                            if (currentWindow.Connection.ServerSetting.StatusModes[0][y] == 'q')
                                b = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.ChannelOwnerColor]);
                            else if (currentWindow.Connection.ServerSetting.StatusModes[0][y] == 'a')
                                b = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.ChannelAdminColor]);
                            else if (currentWindow.Connection.ServerSetting.StatusModes[0][y] == 'o')
                                b = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.ChannelOpColor]);
                            else if (currentWindow.Connection.ServerSetting.StatusModes[0][y] == 'h')
                                b = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.ChannelHalfOpColor]);
                            else if (currentWindow.Connection.ServerSetting.StatusModes[0][y] == 'v')
                                b = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.ChannelVoiceColor]);
                            else
                                b = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.ChannelOwnerColor]);

                        }
                    }

                    if (b == null)
                        b = new SolidBrush(IrcColor.colors[FormMain.Instance.IceChatColors.ChannelRegularColor]);
                    
                    //check if selected, if so, draw the selector bar
                    if (i == selectedIndex)
                    {
                        g.FillRectangle(new SolidBrush(SystemColors.Highlight), 0, currentY, this.Width, lineSize);
                        b = null;
                        b = new SolidBrush(SystemColors.HighlightText);
                    }
                    //draw the nickname
                    g.DrawString(sortedNicks[i].ToString(), this.Font, b, 2, currentY);
                    
                    currentY += lineSize;
                    if (currentY >= listR.Height)
                    {
                        vScrollBar.Maximum = sortedNicks.Count - ((currentY-lineSize) / lineSize) + 1;                        
                        break;
                    }
                }

                if (currentY > listR.Height)
                    vScrollBar.Visible = true;
                else
                {
                    if (vScrollBar.Value == 1)
                        vScrollBar.Visible = false;
                }
                
            }
            
            //paint the buffer onto the usercontrol
            e.Graphics.DrawImageUnscaled(buffer, 0, 0);
            headerFont.Dispose();
            buffer.Dispose();
            l.Dispose();
            sf.Dispose();
            g.Dispose();

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
       /// Replace user mode chars and open a query with the selected nick
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
       /*
        private void OnNickListDoubleClick(object sender, EventArgs e)
       {
            //open a query, if it does not exist, if not, go to it
            if (listNicks.Text.Length > 0)
            {
                string nick = listNicks.Text;
                //replace any of the modes

                for (int i = 0; i < FormMain.Instance.CurrentWindow.Connection.ServerSetting.StatusModes[1].Length; i++)
                    nick = nick.Replace(FormMain.Instance.CurrentWindow.Connection.ServerSetting.StatusModes[1][i].ToString(), string.Empty);
                
                if (!FormMain.Instance.TabMain.WindowExists(FormMain.Instance.CurrentWindow.Connection, nick, TabWindow.WindowType.Query))
                    FormMain.Instance.AddWindow(FormMain.Instance.CurrentWindow.Connection, nick, TabWindow.WindowType.Query);
                else
                    FormMain.Instance.TabMain.SelectedTab = FormMain.Instance.GetWindow(FormMain.Instance.CurrentWindow.Connection, nick, TabWindow.WindowType.Query);
            }
        }
        
       
        /// <summary>
        /// Erase the Nicklist and change the Header to the Console
        /// </summary>
        /// <param name="cwindow"></param>
        */
        internal void RefreshList(ConsoleTabWindow cwindow)
        {
            this.currentWindow = null;
            UpdateHeader("Console");            
        }
        
        /// <summary>
        /// Refresh the Nicklist with the Current Channel/Query Selected
        /// </summary>
        /// <param name="window"></param>
        internal void RefreshList(TabWindow window)
        {
            if (window.WindowStyle == TabWindow.WindowType.Channel)
            {
                if (this.currentWindow != window)
                {
                    selectedIndex = -1;
                    topIndex = 0;
                    vScrollBar.Value = 0;
                    vScrollBar.Visible = false;
                }
                
                this.currentWindow = window;
                UpdateHeader(window.WindowName + ":" + window.Nicks.Count);
            }
            else if (window.WindowStyle == TabWindow.WindowType.Query)
            {
                this.currentWindow = window;
                UpdateHeader("Query:" + window.WindowName);
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
                if (currentWindow.WindowStyle == TabWindow.WindowType.Channel)
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
        internal TabWindow CurrentWindow
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

    }
}
