/******************************************************************************\
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;
using IceChatPlugin;

namespace IceChat
{
    public partial class IceTabControl : UserControl 
    {

        //Tab font which affect the cards' height and width
        //private Font _tabFont = new Font("Verdana", 10F);

        //Tabs Area Rectangle grouped by the tab selectindex
        //private Dictionary<int, Rectangle> _tabSizeRects = new Dictionary<int, Rectangle>();
        //private Dictionary<int, Rectangle> _tabTextRects = new Dictionary<int, Rectangle>();

        //IceTabPage(inherited from Panel) of this TabControl
        private List<IceTabPage> _TabPages = new List<IceTabPage>();
        
        //tabs' height
        //private int _TabRowHeight = 30;
        
        //how many rows of tabs to display
        //private int _TotalTabRows = 1;

        //TabControl's selectedIndex
        private int _selectedIndex = -1;
        private int _previousSelectedIndex = 0;

        //TabControl's hoveredIndex
        //private int _hoveredIndex = -1;

        //starting position of dragging
        //private Point _dragStartPosition = Point.Empty;
        
        //which tab will be dragged
        //private IceTabPage _dragTab;

        private int _tabStartXPos = 0;

        //for the popupmenu
        //private ContextMenuStrip _popupMenu;

        //private Panel pnlCloseButton;

        //private bool showTabs;

        //private System.Timers.Timer flashTabTimer;

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
            
            //_popupMenu = ConsolePopupMenu();
            //_popupMenu.ItemClicked += new ToolStripItemClickedEventHandler(OnPopupMenu_ItemClicked);

            //flashTabTimer = new System.Timers.Timer();
            //flashTabTimer.Interval = 1000;
            //flashTabTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnFlashTabTimerElapsed);
        }

        private void OnFlashTabTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //this.Invalidate();
            //FormMain.Instance.ServerTree.Invalidate();
        }

        internal List<IceTabPage> TabPages 
        {
            get
            {
                return this._TabPages;
            }
        }

        public int SelectedIndex 
        {
            set 
            {
                this._previousSelectedIndex = _selectedIndex;
                this._selectedIndex = value;
                                        
                if (this.SelectedIndexChanged != null)
                {
                    TabEventArgs e = new TabEventArgs();
                    e.IsHandled = true;
                    SelectedIndexChanged(this, e);
                }                    
            }
            get
            {
                return this._selectedIndex;
            }
        }

        internal int TabCount 
        {
            get 
            {
                return this._TabPages.Count;
            }
        }
        /*
        internal Font TabFont 
        {
            set 
            {
                //this._tabFont = value;
            }
        }

        internal bool ShowTabs
        {
            set
            {
                //this.showTabs = value;
                //re-draw the panel accordingly
                //this.Invalidate();
            }
        }
        */
        
        internal IceTabPage CurrentTab
        {
            get
            {
                if (_selectedIndex == -1) _selectedIndex = 0;
                if (_selectedIndex > (_TabPages.Count - 1)) _selectedIndex = 0;
                return _TabPages[_selectedIndex];
            }
        }

        internal void SelectTab(IceTabPage page)
        {
            if (CurrentTab != null && CurrentTab.TextWindow != null)
                CurrentTab.TextWindow.resetUnreadMarker();

            for (int i = 0; i < _TabPages.Count; i++)
            {
                if (_TabPages[i] == page)
                {
                    SelectedIndex = i;
                    this.Invalidate();
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
            for (int i = 0; i < _TabPages.Count; i++)
            {
                if (_TabPages[i].TabCaption.Equals(sCaption))
                    return _TabPages[i];
            }
            return null;
        }

        internal bool WindowExists(IRCConnection connection, string windowName, IceTabPage.WindowType windowType)
        {
            foreach (IceTabPage t in this.TabPages)
            {
                if (t.Connection == null)
                {
                    if (t.WindowStyle == IceTabPage.WindowType.DCCFile)
                    {
                        if (t.TabCaption.ToLower() == windowName.ToLower())
                            return true;
                    }                
                }
                else if (t.Connection == connection)
                {
                    if (t.WindowStyle == windowType)
                    {
                        if (t.TabCaption.ToLower() == windowName.ToLower())
                            return true;
                    }
                }
            }
            return false;
        }

        internal IceTabPage GetTabPage(int iTabIndex)
        {
            if (iTabIndex < _TabPages.Count)
                return _TabPages[iTabIndex];
            return null;
        }

        internal void CloseCurrentTab()
        {
            IceTabPage current = GetTabPage(_selectedIndex);
            if (current != null)
            {
                if (this.OnTabClosed != null)
                    OnTabClosed(SelectedIndex);
            }
        }

        private void InitializeCustom() 
        {
            this.ControlRemoved += new ControlEventHandler(OnControlRemoved);
            this.AutoSize = false;
        }

        protected override void OnPaint(PaintEventArgs e) 
        {
            if (FormMain.Instance != null)
                DrawControl(e.Graphics);
        }


        internal void DrawControl(Graphics g) 
        {
            try
            {
                
                this.BackColor = IrcColor.colors[FormMain.Instance.IceChatColors.TabbarBackColor];

                if (this._TabPages.Count == 0) return;

                if (this._TabPages.Count != 0 && _selectedIndex == -1)
                    SelectedIndex = 0;

                if (this._selectedIndex > (_TabPages.Count - 1))
                    SelectedIndex = 0;
                
                for (int i = 0; i < _TabPages.Count; i++)
                {
                    _TabPages[i].Location = new Point(_tabStartXPos, 1);
                    _TabPages[i].Width = this.Width;
                    _TabPages[i].Height = this.Height - 1;
                    
                    if (!this.Controls.Contains(_TabPages[i]))
                         this.Controls.Add(_TabPages[i]);
                    
                }

                if (GetTabPage(_selectedIndex) != null)
                {
                    if (StaticMethods.IsRunningOnMono())
                        GetTabPage(0).BringToFront();
                    
                    GetTabPage(_selectedIndex).BringToFront();
                }

            }
            catch (Exception e) 
            {
                System.Diagnostics.Debug.WriteLine("IceChatControl DrawControl Error:" + e.Message);
                FormMain.Instance.WriteErrorFile(FormMain.Instance.InputPanel.CurrentConnection, "IceTabControl DrawControl",e);
            }
        }


        private void OnControlRemoved(object sender, ControlEventArgs e) 
        {
            if (e.Control is IceTabPage) 
            {
                _TabPages.Remove((IceTabPage)e.Control);
                ((IceTabPage)e.Control).Dispose();
                SelectedIndex = _previousSelectedIndex;
                this.Invalidate();
                FormMain.Instance.ServerTree.SelectTab(this.CurrentTab, false);
            }
        }
        
    }
    public class TabEventArgs : System.EventArgs
    {
        public bool IsHandled;
    }
}
