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
using System.Text.RegularExpressions;

namespace IceChat
{
    public partial class TextWindow : UserControl
    {

        #region Private Variables

        private int _totalLines;
        private int _totaldisplayLines;

        private int _showMaxLines;
        private int _lineSize;

        private const char colorChar = (char)3;
        private const char underlineChar = (char)31;
        private const char boldChar = (char)2;
        private const char plainChar = (char)15;
        private const char reverseChar = (char)22;
        private const char italicChar = (char)29;

        private const char newColorChar = '\xFF03';
        private const char emotChar = '\xFF0A';
        private const char urlStart = '\xFF0B';
        private const char urlEnd = '\xFF0C';

        private DisplayLine[] _displayLines;
        private TextLine[] _textLines;

        private int _backColor = 0;
        private int _foreColor;

        private bool _showTimeStamp = true;
        private bool _singleLine = false;
        private bool _noColorMode = false;
        private bool _noEmoticons = false;

        private ContextMenuStrip _popupMenu;
        private string _linkedWord = "";

        //private string _wwwMatch = @"((www\.|(http|https|ftp|news|file|irc)+\:\/\/)[a-z0-9-]+\.[a-z0-9\/:@=.+?,#%&~-]*[^.|\'|\# |!|\(|?|,| |>|<|;|\)])";

        //works with www.
        private string _wwwMatch = @"((www\.|www\d\.|(https?|ftp|telnet|file|news|irc):((//)|(\\\\)))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)";
        //works but no www.
        //private string _wwwMatch = @"((https?|ftp|telnet|file|news|irc):((//)|(\\\\))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)";
        private string _emotMatch = "";
        private int _startHighLine = -1;
        private int _startHighChar;
        private int _curHighLine;
        private int _curHighChar;

        #endregion

        #region Structs

        private struct TextLine
        {
            public string line;
            public int width;
            public int totalLines;
            public int textColor;
        }

        private struct DisplayLine
        {
            public string line;
            public int textLine;
            public bool wrapped;
            public bool previous;
            public int textColor;
            public int lineHeight;
        }

        #endregion

        private delegate void ScrollValueDelegate(int value);

        private int _maxTextLines = 500;

        private Logging _logClass;

        private int _unreadMarker;  // Unread marker
        private bool _unreadReset; // Unread marker 

        private bool _reformatLines;

        private Bitmap _backgroundImage = null;
        private string _backgroundImageFile;
        private StringFormat stringFormat;

        public TextWindow()
        {
            InitializeComponent();

            stringFormat = StringFormat.GenericTypographic;
            stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

            this.MouseUp += new MouseEventHandler(OnMouseUp);
            this.MouseDown += new MouseEventHandler(OnMouseDown);
            this.MouseMove += new MouseEventHandler(OnMouseMove);
            this.Paint += new PaintEventHandler(OnPaint);
            this.FontChanged += new EventHandler(OnFontChanged);
            this.Resize += new EventHandler(OnResize);
            this.vScrollBar.Scroll += new ScrollEventHandler(OnScroll);
            this.DoubleClick += new EventHandler(OnDoubleClick);
            
            this.BorderStyle = BorderStyle.Fixed3D;

            this.DoubleBuffered = true;

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            LoadTextSizes();

            this._maxTextLines = FormMain.Instance.IceChatOptions.MaximumTextLines;

            _displayLines = new DisplayLine[_maxTextLines * 4];
            _textLines = new TextLine[_maxTextLines];

            //this.vScrollBar.ValueChanged += new EventHandler(vScrollBar_ValueChanged);

            if (FormMain.Instance != null && FormMain.Instance.IceChatEmoticons != null)
            {
                if (FormMain.Instance.IceChatEmoticons.listEmoticons.Count > 0)
                {
                    foreach (EmoticonItem emot in FormMain.Instance.IceChatEmoticons.listEmoticons)
                    {
                        _emotMatch += emot.Trigger + ((char)0);
                    }
                    _emotMatch = _emotMatch.Substring(0, _emotMatch.Length - 1);
                }
            }

            _popupMenu = new ContextMenuStrip();

        }

        private void vScrollBar_ValueChanged(object sender, EventArgs e)
        {
            if (this.Parent != null)
                if (this.Parent.Name == "panelTopic")
                    System.Diagnostics.Debug.WriteLine("value changed:" + ((VScrollBar)sender).Value);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            //get the current line mouse is over
            int line = 0;

            if (!SingleLine)
            {
                // Get the line count from the bottom... 
                line = ((this.Height + (_lineSize / 2)) - e.Y) / _lineSize;

                // Then, convert it to count from the top. 
                line = vScrollBar.Value - line;
            }

            _linkedWord = ReturnWord(line, e.Location.X).Trim();

            if (_linkedWord.Length > 0)
            {
                Regex re = new Regex(_wwwMatch);
                MatchCollection matches = re.Matches(_linkedWord);
                if (matches.Count > 0)
                {
                    if (this.Cursor != Cursors.Hand)
                        this.Cursor = Cursors.Hand;
                    return;
                }
                else if (this.Parent.GetType() == typeof(IceTabPage))
                {
                    IceTabPage t = (IceTabPage)this.Parent;
                    if (t.WindowStyle != IceTabPage.WindowType.Debug)
                    {
                        //check if we are over a channel name
                        string chan = _linkedWord;
                        if (t.Connection.ServerSetting.StatusModes != null)
                            for (int i = 0; i < t.Connection.ServerSetting.StatusModes[1].Length; i++)
                                chan = chan.Replace(t.Connection.ServerSetting.StatusModes[1][i].ToString(), string.Empty);
                        
                        if (chan.Length > 0 && t.Connection.ServerSetting.ChannelTypes != null && Array.IndexOf(t.Connection.ServerSetting.ChannelTypes, chan[0]) != -1)
                        {
                            if (this.Cursor != Cursors.Hand)
                                this.Cursor = Cursors.Hand;
                            return;
                        }
                        string _linkedWordNick = StripString(_linkedWord);
                        
                        //check if over a nick name
                        if (t.WindowStyle == IceTabPage.WindowType.Channel)
                        {
                            foreach (User u in t.Nicks.Values)
                            {
                                if (u.NickName == _linkedWordNick)
                                {
                                    if (this.Cursor != Cursors.Hand)
                                        this.Cursor = Cursors.Hand;
                                    return;
                                }
                                else if (u.NickName == _linkedWord)
                                {
                                    if (this.Cursor != Cursors.Hand)
                                        this.Cursor = Cursors.Hand;
                                    return;
                                }
                            }
                        }
                    }
                }
                else if (this.Parent.GetType() == typeof(ConsoleTab))
                {
                    ConsoleTab c = (ConsoleTab)this.Parent;
                    if (c.Connection != null)
                    {
                        //check if we are over a channel name
                        if (c.Connection.IsFullyConnected)
                        {
                            string chan = _linkedWord;
                            if (c.Connection.ServerSetting.StatusModes != null)
                                for (int i = 0; i < c.Connection.ServerSetting.StatusModes[1].Length; i++)
                                    chan = chan.Replace(c.Connection.ServerSetting.StatusModes[1][i].ToString(), string.Empty);

                            if (chan.Length > 0 && c.Connection.ServerSetting.ChannelTypes != null && Array.IndexOf(c.Connection.ServerSetting.ChannelTypes, chan[0]) != -1)
                            {
                                if (this.Cursor != Cursors.Hand)
                                    this.Cursor = Cursors.Hand;
                                return;
                            }
                        }
                    }
                }
            }

            this.Cursor = Cursors.Default;

            //get the current character the mouse is over. 
            _curHighLine = ((this.Height + (_lineSize / 2)) - e.Y) / _lineSize;
            _curHighLine = _totaldisplayLines - _curHighLine;
            _curHighLine = (_curHighLine - (_totaldisplayLines - vScrollBar.Value));
            _curHighChar = ReturnChar(line, e.Location.X);

            if (_startHighLine != -1)
                Invalidate();

        }

        private string ReturnWord(int lineNumber, int x)
        {
            if (lineNumber < _totaldisplayLines && lineNumber >= 0)
            {
                Graphics g = this.CreateGraphics();
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                string line = StripAllCodes(_displayLines[lineNumber].line);

                int width = (int)g.MeasureString(line, this.Font, 0, stringFormat).Width;

                if (x > width)
                    return "";

                int space = 0;
                bool foundSpace = false;
                float lookWidth = 0;
                for (int i = 0; i < line.Length; i++)
                {
                    if (lookWidth >= x && foundSpace)
                    {
                        if (_displayLines[lineNumber].previous && lineNumber > 0 && space == 0)
                        {
                            // this line wraps from the previous one. 
                            string prevline = StripAllCodes(_displayLines[lineNumber - 1].line);
                            int prevWidth = (int)g.MeasureString(prevline, this.Font, 0, stringFormat).Width;
                            return ReturnWord(lineNumber - 1, prevWidth);
                        }

                        return line.Substring(space, i - space);
                    }

                    if (line[i] == (char)32)
                    {
                        if (!foundSpace)
                        {
                            if (lookWidth >= x)
                                foundSpace = true;
                            else
                                space = i + 1;
                        }
                    }

                    lookWidth += g.MeasureString(line[i].ToString(), this.Font, 0, stringFormat).Width;
                }
                if (_displayLines[lineNumber].previous && lineNumber > 0 && space == 0)
                {
                    // this line wraps from the previous one. 
                    string prevline = StripAllCodes(_displayLines[lineNumber - 1].line);
                    if (prevline[prevline.Length - 1] != ' ')
                    {
                        int prevWidth = (int)g.MeasureString(prevline, this.Font, 0, stringFormat).Width;
                        return ReturnWord(lineNumber - 1, prevWidth);
                    }
                }

                if (!foundSpace && space < line.Length)
                {
                    //wrap to the next line
                    if (lineNumber < _totaldisplayLines)
                    {
                        string extra = "";
                        int currentLine = _displayLines[lineNumber].textLine;

                        while (lineNumber < _totaldisplayLines)
                        {
                            lineNumber++;
                            if (_displayLines[lineNumber].textLine != currentLine)
                                break;

                            extra += StripAllCodes(_displayLines[lineNumber].line);
                            if (extra.IndexOf(' ') > -1)
                            {
                                extra = extra.Substring(0, extra.IndexOf(' '));
                                break;
                            }
                        }

                        return line.Substring(space) + extra;
                    }

                }
                else if (foundSpace && space < line.Length)
                {
                    return line.Substring(space);
                }

                g.Dispose();
            }
            return "";
        }

        private int ReturnChar(int lineNumber, int x)
        {
            if (lineNumber < _totaldisplayLines && lineNumber >= 0)
            {
                Graphics g = this.CreateGraphics();
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                string line = StripAllCodes(_displayLines[lineNumber].line);

                int width = (int)g.MeasureString(line, this.Font, 0, stringFormat).Width;

                if (x > width)
                    return line.Length;

                float lookWidth = 0;
                for (int i = 0; i < line.Length; i++)
                {
                    lookWidth += g.MeasureString(line[i].ToString(), this.Font, 0, stringFormat).Width;
                    if (lookWidth >= x)
                    {
                        return i;
                    }

                }
                g.Dispose();
                return line.Length;
            }
            return 0;
        }

        private void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            resetUnreadMarker();

            //get the current character the mouse is over. 
            _startHighLine = ((this.Height + (_lineSize / 2)) - e.Y) / _lineSize;
            _startHighLine = _totaldisplayLines - _startHighLine;
            _startHighLine = (_startHighLine - (_totaldisplayLines - vScrollBar.Value));

            _startHighChar = ReturnChar(_startHighLine, e.Location.X);

            //what kind of a popupmenu do we want?
            string popupType = "";
            string windowName = "";
            string _linkedWordNick = StripString(_linkedWord);

            if (this.Parent.GetType() == typeof(IceTabPage))
            {
                IceTabPage t = (IceTabPage)this.Parent;
                if (t.WindowStyle == IceTabPage.WindowType.Channel)
                {
                    //check if over a nick name
                    foreach (User u in t.Nicks.Values)
                    {
                        if (u.NickName == _linkedWordNick)
                        {
                            popupType = "NickList";
                            //highlight the nick in the nick list
                            if (e.Button == MouseButtons.Left)
                                FormMain.Instance.NickList.SelectNick(_linkedWordNick);
                            break;
                        }
                        else if (u.NickName == _linkedWord)
                        {
                            popupType = "NickList";
                            _linkedWordNick = _linkedWord;
                            //highlight the nick in the nick list
                            if (e.Button == MouseButtons.Left)
                                FormMain.Instance.NickList.SelectNick(_linkedWordNick);
                            break;
                        }
                    }
                    if (popupType.Length == 0)
                        popupType = "Channel";
                }
                if (t.WindowStyle == IceTabPage.WindowType.Query)
                    popupType = "Query";

                windowName = t.TabCaption;
            }
            if (this.Parent.GetType() == typeof(ConsoleTab))
            {
                ConsoleTab c = (ConsoleTab)this.Parent;
                popupType = "Console";

                if (c.Connection != null)
                {
                    if (c.Connection.ServerSetting.RealServerName.Length > 0)
                        windowName = c.Connection.ServerSetting.RealServerName;
                    else
                        windowName = c.Connection.ServerSetting.ServerName;
                }
            }

            if (e.Button == System.Windows.Forms.MouseButtons.Right && popupType.Length > 0)
            {
                //show the popup menu
                foreach (PopupMenuItem p in FormMain.Instance.IceChatPopupMenus.listPopups)
                {
                    if (p.PopupType == popupType)
                    {
                        string[] menuItems = p.Menu;

                        //build the menu
                        ToolStripItem t;
                        int subMenu = 0;

                        _popupMenu.Items.Clear();

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
                                if (popupType == "Channel")
                                {
                                    caption = caption.Replace("$chan", windowName);
                                    command = command.Replace("$chan", windowName);

                                    caption = caption.Replace(" # ", " " + windowName + " ");
                                    command = command.Replace(" # ", " " + windowName + " ");
                                }
                                else if (popupType == "Query")
                                {
                                    caption = caption.Replace("$nick", windowName);
                                    command = command.Replace("$nick", windowName);
                                }
                                else if (popupType == "NickList")
                                {
                                    caption = caption.Replace("$nick", _linkedWordNick);
                                    command = command.Replace("$nick", _linkedWordNick);
                                    caption = caption.Replace("$chan", windowName);
                                    command = command.Replace("$chan", windowName);
                                }
                                else if (popupType == "Console")
                                {
                                    caption = caption.Replace("$server", windowName);
                                    command = command.Replace("$server", windowName);
                                }

                                if (caption == "-")
                                    t = new ToolStripSeparator();
                                else
                                {
                                    t = new ToolStripMenuItem(caption);

                                    //parse out the command/$identifiers                            
                                    if (popupType == "NickList")
                                        command = command.Replace("$1", _linkedWordNick);
                                    else
                                        command = command.Replace("$1", windowName);

                                    t.Click += new EventHandler(OnPopupMenuClick);
                                    t.Tag = command;
                                }

                                if (menuDepth == 0)
                                    subMenu = _popupMenu.Items.Add(t);
                                else
                                {
                                    //do not allow submenu items for a toolstrip seperator
                                    if (_popupMenu.Items[subMenu].GetType() != typeof(ToolStripSeparator))
                                        ((ToolStripMenuItem)_popupMenu.Items[subMenu]).DropDownItems.Add(t);
                                }
                                t = null;
                            }
                        }

                        _popupMenu.Show(this, e.Location);
                    }
                }

            }
        }

        private void OnPopupMenuClick(object sender, EventArgs e)
        {
            if (((ToolStripMenuItem)sender).Tag == null) return;

            string command = ((ToolStripMenuItem)sender).Tag.ToString();
            if (command.Length == 0) return;

            if (this.Parent.GetType() == typeof(ConsoleTab))
            {
                ConsoleTab c = (ConsoleTab)this.Parent;
                FormMain.Instance.ParseOutGoingCommand(c.Connection, command);
            }
            else if (this.Parent.GetType() == typeof(IceTabPage))
            {
                IceTabPage t = (IceTabPage)this.Parent;
                FormMain.Instance.ParseOutGoingCommand(t.Connection, command);
            }

        }

        private void OnDoubleClick(object sender, EventArgs e)
        {
            MouseEventArgs me = e as MouseEventArgs;

            if (me.Button == MouseButtons.Left)
            {
                //first need to see what word we double clicked, if not, run a command
                if (_linkedWord.Length > 0)
                {
                    //check if it is a URL
                    Regex re = new Regex(_wwwMatch);
                    MatchCollection matches = re.Matches(_linkedWord);
                    String clickedWord = _linkedWord;
                    if (matches.Count > 0)
                    {
                        clickedWord = matches[0].ToString();
                    }
                    if (matches.Count > 0 && !clickedWord.StartsWith("irc://"))
                    {
                        try
                        {
                            if (clickedWord.ToLower().StartsWith("www"))
                                clickedWord = "http://" + clickedWord;
                            System.Diagnostics.Process.Start(clickedWord);
                        }
                        catch (Exception)
                        {
                        }
                        return;
                    }

                    //check if it is a irc:// link
                    if (clickedWord.StartsWith("irc://"))
                    {
                        //check if a channel was specified
                        string server = clickedWord.Substring(6).TrimEnd();
                        if (server.IndexOf("/") != -1)
                        {
                            string host = server.Split('/')[0];
                            string channel = server.Split('/')[1];
                            FormMain.Instance.ParseOutGoingCommand(null, "/joinserv " + host + " #" + channel);
                        }
                        else
                            FormMain.Instance.ParseOutGoingCommand(null, "/server " + clickedWord.Substring(6).TrimEnd());
                        return;
                    }

                    if (this.Parent.GetType() == typeof(IceTabPage))
                    {
                        IceTabPage t = (IceTabPage)this.Parent;
                        //check if it is a channel
                        //remove any user types from the front of the clickedWord
                        string chan = clickedWord;
                        for (int i = 0; i < t.Connection.ServerSetting.StatusModes[1].Length; i++)
                            chan = chan.Replace(t.Connection.ServerSetting.StatusModes[1][i].ToString(), string.Empty);

                        if (chan.Length>0 && Array.IndexOf(t.Connection.ServerSetting.ChannelTypes, chan[0]) != -1)
                        {
                            FormMain.Instance.ParseOutGoingCommand(t.Connection, "/join " + chan);
                            return;
                        }

                        string clickedWordNick = StripString(clickedWord);

                        //check if it is a nickname in the current channel
                        if (t.WindowStyle == IceTabPage.WindowType.Channel)
                        {
                            if (t.WindowStyle == IceTabPage.WindowType.Channel)
                            {
                                foreach (User u in t.Nicks.Values)
                                {
                                    if (u.NickName == clickedWordNick)
                                    {
                                        FormMain.Instance.ParseOutGoingCommand(t.Connection, "/query " + clickedWordNick);
                                        break;
                                    }
                                    else if (u.NickName == clickedWord)
                                    {
                                        FormMain.Instance.ParseOutGoingCommand(t.Connection, "/query " + clickedWord);
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (this.Parent.GetType() == typeof(ConsoleTab))
                    {
                        ConsoleTab c = (ConsoleTab)this.Parent;
                        if (c.Connection != null)
                        {
                            //check if it is a channel
                            if (c.Connection.IsFullyConnected)
                            {
                                string chan = clickedWord;
                                for (int i = 0; i < c.Connection.ServerSetting.StatusModes[1].Length; i++)
                                    chan = chan.Replace(c.Connection.ServerSetting.StatusModes[1][i].ToString(), string.Empty);

                                if (chan.Length>0 && Array.IndexOf(c.Connection.ServerSetting.ChannelTypes, chan[0]) != -1)
                                {
                                    FormMain.Instance.ParseOutGoingCommand(c.Connection, "/join " + chan);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            if (this.Parent.GetType() == typeof(ConsoleTab))
            {
                //console
                ConsoleTab c = (ConsoleTab)this.Parent;
                FormMain.Instance.ParseOutGoingCommand(c.Connection, "/lusers");
            }
            else if (this.Parent.GetType() == typeof(IceTabPage))
            {
                IceTabPage t = (IceTabPage)this.Parent;
                if (t.WindowStyle == IceTabPage.WindowType.Channel)
                    FormMain.Instance.ParseOutGoingCommand(t.Connection, "/chaninfo");
            }
            else if (this.Parent.GetType() == typeof(Panel))
            {
                if (this.Parent.Parent.GetType() == typeof(IceTabPage))
                {
                    IceTabPage t = (IceTabPage)this.Parent.Parent;
                    if (t.WindowStyle == IceTabPage.WindowType.Channel)
                        FormMain.Instance.ParseOutGoingCommand(t.Connection, "/chaninfo");
                }
            }
        }

        #region Public Properties

        internal string BackGroundImage
        {
            get
            {
                return _backgroundImageFile;
            }
            set
            {
                if (value.Length > 0)
                    this._backgroundImage = new Bitmap(value);
                else
                    this._backgroundImage = null;

                this._backgroundImageFile = value;
                
                Invalidate();
            }
        }

        internal System.IO.Stream BackGroundImageURL
        {
            get
            {
                return null;
            }
            set 
            {
                if (value != null)
                    this._backgroundImage = new Bitmap(value);
                else
                    this._backgroundImage = null;

                Invalidate();
            }

        }

        internal int IRCBackColor
        {
            get
            {
                return _backColor;
            }
            set
            {
                _backColor = value;
                Invalidate();
            }
        }

        internal int IRCForeColor
        {
            get
            {
                return _foreColor;
            }
            set
            {
                _foreColor = value;
                Invalidate();
            }
        }

        public bool SingleLine
        {
            get
            {
                return _singleLine;
            }
            set
            {
                _singleLine = value;
                //vScrollBar.Visible = !value;
                Invalidate();
            }
        }

        internal bool NoColorMode
        {
            get
            {
                return _noColorMode;
            }
            set
            {
                _noColorMode = value;
            }
        }

        internal bool NoEmoticons
        {
            get { return _noEmoticons; }
            set { _noEmoticons = value; }
        }

        internal int MaximumTextLines
        {
            get
            {
                return _maxTextLines;
            }
            set { 
                
                _maxTextLines = value;
                
                _displayLines = new DisplayLine[_maxTextLines * 4];
                _textLines = new TextLine[_maxTextLines];
                
                LoadTextSizes();
                
            }
        }

        internal void SetDebugWindow()
        {
            //disable doubleclick when this is a Debug Text Window
            this.DoubleClick -= OnDoubleClick;
        }

        #endregion

        #region Public Methods

        internal void ClearTextWindow()
        {
            //clear the text window of all its lines
            _displayLines.Initialize();
            _textLines.Initialize();

            _totalLines = 0;
            _totaldisplayLines = 0;

            Invalidate();
        }

        internal void SetLogFile()
        {
            if (this.Parent.GetType() == typeof(IceTabPage))
            {
                //get the proper object
                IceTabPage t = (IceTabPage)this.Parent;
                _logClass = new Logging(t);
            }
            else if (this.Parent.GetType() == typeof(ConsoleTab))
            {
                ConsoleTab c = (ConsoleTab)this.Parent;
                _logClass = new Logging(c);
            }
        }

        internal void resetUnreadMarker()
        {
            _unreadReset = true;
        }

        internal bool ShowTimeStamp
        {
            get { return _showTimeStamp; }
            set { _showTimeStamp = value; }
        }

        internal void AppendText(string newLine, int color)
        {
            try
            {
                //adds a new line to the Text Window
                if (newLine.Length == 0)
                    return;

                if (_unreadReset)
                {
                    _unreadMarker = 0;
                    _unreadReset = false;
                }

                ++_unreadMarker;

                newLine = newLine.Replace("\n", " ");
                newLine = newLine.Replace("&#x3;", colorChar.ToString());
                newLine = ParseUrl(newLine);

                //get the color from the line
                if (newLine[0] == colorChar)
                {
                    if (Char.IsNumber(newLine[1]) && Char.IsNumber(newLine[2]))
                        _foreColor = Convert.ToInt32(newLine[1].ToString() + newLine[2].ToString());
                    else if (Char.IsNumber(newLine[1]) && !Char.IsNumber(newLine[2]))
                        _foreColor = Convert.ToInt32(newLine[1].ToString());

                    //check of _foreColor is less then 72     
                    if (_foreColor > (IrcColor.colors.Length - 1))
                        _foreColor = _foreColor - 72;
                }
                else
                    _foreColor = color;

                if (!_singleLine && _showTimeStamp)
                    newLine = DateTime.Now.ToString(FormMain.Instance.IceChatOptions.TimeStamp) + newLine;

                //System.Diagnostics.Debug.WriteLine("NEWLINE1:" + newLine);

                if (_noColorMode)
                    newLine = StripColorCodes(newLine);
                else
                    newLine = RedefineColorCodes(newLine);

                //System.Diagnostics.Debug.WriteLine("NEWLINE2:" + newLine);

                if (_logClass != null)
                    _logClass.WriteLogFile(newLine);

                _totalLines++;

                newLine = ParseEmoticons(newLine);

                //if (_singleLine) _totalLines = 1;

                if (_totalLines >= (_maxTextLines - 10))
                {
                    int x = 1;

                    //System.Diagnostics.Debug.WriteLine("clean up text " + _totalLines);

                    for (int i = _totalLines - (_totalLines - 50); i <= _totalLines - 1; i++)
                    {
                        _textLines[x].totalLines = _textLines[i].totalLines;
                        _textLines[x].width = _textLines[i].width;
                        _textLines[x].line = _textLines[i].line;

                        _textLines[x].textColor = _textLines[i].textColor;
                        x++;
                    }

                    for (int i = (_totalLines - 49); i < _totalLines; i++)
                    {
                        _textLines[i].totalLines = 0;
                        _textLines[i].line = "";
                        _textLines[i].width = 0;
                    }

                    _totalLines = _totalLines - 50;

                    //System.Diagnostics.Debug.WriteLine("cleaned " + _totalLines);

                    if (this.Height != 0)
                    {
                        _totaldisplayLines = FormatLines(_totalLines, 1, 0);
                        UpdateScrollBar(_totaldisplayLines);
                        Invalidate();
                    }

                    _totalLines++;
                }

                //System.Diagnostics.Debug.WriteLine(_totalLines + ":" + _maxTextLines + ":" + _textLines.Length);                

                _textLines[_totalLines].line = newLine;

                Graphics g = this.CreateGraphics();
                //properly measure for bold characters needed

                _textLines[_totalLines].width = (int)g.MeasureString(StripAllCodes(newLine), this.Font, 0, stringFormat).Width;

                g.Dispose();

                _textLines[_totalLines].textColor = _foreColor;

                int addedLines = FormatLines(_totalLines, _totalLines, _totaldisplayLines);
                addedLines -= _totaldisplayLines;

                _textLines[_totalLines].totalLines = addedLines;

                for (int i = _totaldisplayLines + 1; i < _totaldisplayLines + addedLines; i++)
                    _displayLines[i].textLine = _totalLines;

                _totaldisplayLines += addedLines;

                UpdateScrollBar(_totaldisplayLines);

                if (_singleLine)
                    vScrollBar.Value = 1;

                Invalidate();
            }
            catch (OutOfMemoryException)
            {
                //System.Diagnostics.Debug.WriteLine("Out of Memory Exception:" + oe.Message + ":" + oe.StackTrace);
            }
            catch (Exception e)
            {
                FormMain.Instance.WriteErrorFile(FormMain.Instance.InputPanel.CurrentConnection, "AppendText", e);
            }
        }

        internal int SearchText(string data, int start)
        {
            int totalChars = 0;
            System.Diagnostics.Debug.WriteLine(_totalLines + ":" + start);
            for (int i = 1; i <= _totalLines; i++)
            {
                string line = StripAllCodes(_textLines[i].line);
                if ((line.Length + totalChars) > start)
                {
                    int x = line.IndexOf(data);
                    if (x > -1)
                    {
                        //we have a match
                        //now check to make sure it is past the start position
                        if (x > (totalChars + start))
                        {

                        }
                        //System.Diagnostics.Debug.WriteLine("match:" + (x + totalChars));
                    }
                }
                //System.Diagnostics.Debug.WriteLine(_textLines[i].line.IndexOf(data));
                totalChars += line.Length;
            }
            return -1;
        }


        /// <summary>
        /// Used to scroll the Text Window a Page at a Time
        /// </summary>
        /// <param name="scrollUp"></param>
        internal void ScrollWindowPage(bool scrollUp)
        {
            try
            {
                if (vScrollBar.Enabled == false)
                    return;

                if (scrollUp == true)
                {
                    if (vScrollBar.Value > vScrollBar.LargeChange)
                    {
                        vScrollBar.Value = vScrollBar.Value - (vScrollBar.LargeChange - 1);
                        Invalidate();
                    }
                }
                else
                {
                    if (vScrollBar.Value <= vScrollBar.Maximum - (vScrollBar.LargeChange * 2))
                        vScrollBar.Value = vScrollBar.Value + (vScrollBar.LargeChange - 1);
                    else
                        vScrollBar.Value = vScrollBar.Maximum - vScrollBar.LargeChange + 1;
                    Invalidate();
                }
            }
            catch (Exception e)
            {
                FormMain.Instance.WriteErrorFile(FormMain.Instance.InputPanel.CurrentConnection, "ScrollWindowPage", e);
            }

        }
        /// <summary>
        /// Used to scroll the Text Window a Single Line at a Time
        /// </summary>
        /// <param name="scrollUp"></param>
        internal void ScrollWindow(bool scrollUp)
        {
            try
            {
                if (vScrollBar.Enabled == false)
                    return;

                if (scrollUp == true)
                {
                    if (vScrollBar.Value > 1)
                    {
                        vScrollBar.Value--;
                        Invalidate();
                    }
                }
                else
                {
                    if (vScrollBar.Value <= vScrollBar.Maximum - vScrollBar.LargeChange)
                    {
                        vScrollBar.Value++;
                        Invalidate();
                    }
                }
            }
            catch (Exception e)
            {
                FormMain.Instance.WriteErrorFile(FormMain.Instance.InputPanel.CurrentConnection, "ScrollWindow", e);
            }
        }

        private string StripString(string targetString)
        {
            //strip all non-alpha numeric chars from string (for nicknames)
            //only allow chars that are allowed in nicks
            //return Regex.Replace(targetString, @"[^A-Za-z0-9_-|\[\]\\/`\^{}]", "");
            return Regex.Replace(targetString, @"[^A-Za-z0-9_\-|\[\]\\\`\^{}]", "");
        }

        #endregion

        #region TextWindow Events

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (_startHighLine > -1 && _curHighLine > -1)
            {
                if (_curHighLine < _startHighLine || (_curHighLine == _startHighLine && _curHighChar < _startHighChar))
                {
                    int sw = _startHighLine;
                    _startHighLine = _curHighLine;
                    _curHighLine = sw;
                    sw = _startHighChar;
                    _startHighChar = _curHighChar;
                    _curHighChar = sw;
                }

                StringBuilder buildString = new StringBuilder();
                int tl = _displayLines[_startHighLine].textLine;
                for (int curLine = _startHighLine; curLine <= _curHighLine; ++curLine)
                {
                    if (tl != _displayLines[curLine].textLine)
                    {
                        buildString.Append("\r\n");
                        tl = _displayLines[curLine].textLine;
                    }
                    StringBuilder s = new StringBuilder(StripAllCodes(_displayLines[curLine].line));

                    /* Filter out non-text */
                    if (curLine == _curHighLine)
                    {
                        if (s.Length >= _curHighChar)
                            s = s.Remove(_curHighChar, s.Length - _curHighChar);
                    }
                    if (curLine == _startHighLine)
                        s = s.Remove(0, _startHighChar);

                    buildString.Append(s);
                }

                if (buildString.Length > 0)
                    Clipboard.SetText(buildString.ToString());

            }

            // Supress highlighting
            _startHighLine = -1;
            if (_curHighLine != -1)
            {
                _curHighLine = -1;
                Invalidate();
            }

            FormMain.Instance.FocusInputBox();
        }

        private void OnFontChanged(object sender, System.EventArgs e)
        {
            LoadTextSizes();

            _displayLines.Initialize();

            _totaldisplayLines = FormatLines(_totalLines, 1, 0);
            UpdateScrollBar(_totaldisplayLines);

            Invalidate();

        }

        private void OnResize(object sender, System.EventArgs e)
        {
            if (this.Height == 0 || _totalLines == 0)
                return;

            _displayLines.Initialize();

            _reformatLines = true;

            Invalidate();

        }

        /// <summary>
        /// Updates the scrollbar to the given line. 
        /// </summary>
        /// <param name="newValue">Line number to be displayed</param>
        /// <param name="endLine"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        private void UpdateScrollBar(int newValue)
        {
            _showMaxLines = (this.Height / _lineSize) + 1;
            
            if (this.InvokeRequired)
            {
                ScrollValueDelegate s = new ScrollValueDelegate(UpdateScrollBar);
                this.Invoke(s, new object[] { newValue });
            }
            else
            {
                if (_showMaxLines < _totaldisplayLines)
                {
                    vScrollBar.LargeChange = _showMaxLines;
                    vScrollBar.Enabled = true;
                }
                else
                {
                    vScrollBar.LargeChange = _totaldisplayLines;
                    vScrollBar.Enabled = false;
                }

                if (_singleLine && _totaldisplayLines == 1)
                {
                    vScrollBar.Visible = false;
                    vScrollBar.Enabled = true;
                }
                else if (_singleLine)
                {
                    vScrollBar.Visible = true;
                    vScrollBar.Enabled = true;
                }

                if (newValue != 0)
                {
                    vScrollBar.Minimum = 1;
                    vScrollBar.Maximum = newValue + vScrollBar.LargeChange - 1;

                    //if (this.Parent != null)
                    //    if (this.Parent.Name == "panelTopic")
                    //        System.Diagnostics.Debug.WriteLine(_showMaxLines + ":" + _totaldisplayLines + ":" + newValue + ":" + vScrollBar.Value + ":" + vScrollBar.LargeChange + ":" + vScrollBar.Maximum);

                    if (newValue <= (vScrollBar.Value + (vScrollBar.LargeChange / 2)) || vScrollBar.Enabled == false)
                    {
                        if (this.Parent != null)
                            if (this.Parent.Name == "panelTopic")
                                return;
                        
                        vScrollBar.Value = newValue;
                    }
                }
            }
        }

        internal void ScrollToBottom()
        {
            if (_totaldisplayLines > 0)
                vScrollBar.Value = _totaldisplayLines;
        }

        private void OnScroll(object sender, EventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("on scroll " + ((VScrollBar)sender).Value + ":" + ((VScrollBar)sender).Maximum);
            if (((VScrollBar)sender).Value < 1)
            {
                ((VScrollBar)sender).Value = 1;
            }
            Invalidate();
        }

        #endregion

        #region Emoticon and Color Parsing


        private string ParseEmoticons(string line)
        {
            if (FormMain.Instance.IceChatOptions.ShowEmoticons && !_noEmoticons)
            {
                if (_emotMatch.Length > 0)
                {
                    string[] eachEmot = _emotMatch.Split((char)0);
                    for (int i = eachEmot.GetLowerBound(0); i <= eachEmot.GetUpperBound(0); i++)
                    {
                        // hey there :) how are ya (F) and then (sw) (K)(@) sdkjf slk dfj ;sdlfkjs;dfl ggg
                        line = line.Replace(@eachEmot[i], emotChar + i.ToString("000"));
                    }
                }
            }
            //System.Diagnostics.Debug.WriteLine("return:" + line);
            return line;

        }

        private string RedefineColorCodes(string line)
        {
            //redefine the irc server colors to own standard
            // go from \x0003xx,xx to \x0003xxxx
            string ParseBackColor = @"\x03([0-9]{1,2}),([0-9]{1,2})";
            string ParseForeColor = @"\x03[0-9]{1,2}";
            string ParseColorChar = @"\x03";

            Regex ParseIRCCodes = new Regex(ParseBackColor + "|" + ParseForeColor + "|" + ParseColorChar);

            StringBuilder sLine = new StringBuilder();
            sLine.Append(line);

            int oldLen = 0;

            int currentBackColor = -1;

            Match m = ParseIRCCodes.Match(sLine.ToString());
            while (m.Success)
            {
                oldLen = sLine.Length;
                sLine.Remove(m.Index, m.Length);

                if (Regex.Match(m.Value, ParseBackColor).Success)
                {
                    string rem = m.Value.Remove(0, 1);
                    string[] intstr = rem.Split(new Char[] { ',' });
                    //get the fore color                    
                    int fc = int.Parse(intstr[0]);
                    if (fc > (IrcColor.colors.Length - 1))
                        fc = int.Parse(intstr[0].Substring(1, 1));
                    //get the back color
                    int bc = int.Parse(intstr[1]);
                    if (bc > (IrcColor.colors.Length - 1))
                    {
                        bc = int.Parse(intstr[1].Substring(1, 1));
                        currentBackColor = bc;
                        sLine.Insert(m.Index, newColorChar.ToString() + fc.ToString("00") + bc.ToString("00") + intstr[1].Substring(2));
                    }
                    else
                    {
                        currentBackColor = bc;
                        sLine.Insert(m.Index, newColorChar.ToString() + fc.ToString("00") + bc.ToString("00"));
                    }
                    oldLen--;
                }
                else if (Regex.Match(m.Value, ParseForeColor).Success)
                {
                    int fc = int.Parse(m.Value.Remove(0, 1));
                    if (fc > (IrcColor.colors.Length - 1))
                    {
                        fc = int.Parse(m.Value.Substring(1, 1));
                        if (currentBackColor > -1)
                            sLine.Insert(m.Index, newColorChar.ToString() + fc.ToString("00") + currentBackColor.ToString("00") + m.Value.Substring(2));
                        else
                            sLine.Insert(m.Index, newColorChar.ToString() + fc.ToString("00") + "99" + m.Value.Substring(2));
                    }
                    else
                    {
                        if (currentBackColor > -1)
                            sLine.Insert(m.Index, newColorChar.ToString() + fc.ToString("00") + currentBackColor.ToString("00"));
                        else
                            sLine.Insert(m.Index, newColorChar.ToString() + fc.ToString("00") + "99");
                    }
                }
                else if (Regex.Match(m.Value, ParseColorChar).Success)
                {
                    currentBackColor = -1;
                    sLine.Insert(m.Index, newColorChar.ToString() + _foreColor.ToString("00") + "99");
                }
                m = ParseIRCCodes.Match(sLine.ToString(), sLine.Length - oldLen);
            }
            return sLine.ToString();
        }

        private string StripColorCodes(string line)
        {
            //strip out all the color codes, bold , underline and reverse codes
            string ParseBackColor = @"\x03([0-9]{1,2}),([0-9]{1,2})";
            string ParseForeColor = @"\x03[0-9]{1,2}";
            string ParseColorChar = @"\x03";
            string ParseBoldChar = @"\x02";
            string ParseUnderlineChar = @"\x1F";    //code 31
            string ParseReverseChar = @"\x16";      //code 22
            string ParseItalicChar = @"\x1D";      //code 29

            StringBuilder sLine = new StringBuilder();
            sLine.Append(line);

            Regex ParseIRCCodes = new Regex(ParseBackColor + "|" + ParseForeColor + "|" + ParseColorChar + "|" + ParseBoldChar + "|" + ParseUnderlineChar + "|" + ParseReverseChar + "|" + ParseItalicChar);

            Match m = ParseIRCCodes.Match(sLine.ToString());
            
            while (m.Success)
            {                
                sLine.Remove(m.Index, m.Length);                
                m = ParseIRCCodes.Match(sLine.ToString(), m.Index);
            }

            return sLine.ToString();
        }

        #endregion

        /// <summary>
        /// Format the text for each line to show in the Text Window
        /// </summary>
        /// <param name="startLine"></param>
        /// <param name="endLine"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        private int FormatLines(int startLine, int endLine, int line)
        {
            //this formats each line and breaks it up, to fit onto the current display
            int displayWidth = this.ClientRectangle.Width - vScrollBar.Width - 10;

            if (displayWidth <= 0)
                return 0;

            if (_totalLines == 0)
                return 0;

            string lastColor = "";
            string nextColor = "";

            bool lineSplit;
            int ii = line;
            Graphics g = this.CreateGraphics();

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            for (int currentLine = endLine; currentLine <= startLine; currentLine++)
            {
                lastColor = "";
                _displayLines[line].previous = false;
                _displayLines[line].wrapped = false;

                //System.Diagnostics.Debug.WriteLine("checking:" + currentLine + ":" + startLine + ":" + line + ":" + _textLines[currentLine].width + ":" + displayWidth);
                //check of the line width is the same or less then the display width            
                if (_textLines[currentLine].width <= displayWidth)
                {
                    try
                    {
                        //System.Diagnostics.Debug.WriteLine("FORMAT  :" + _textLines[currentLine].line);
                        _displayLines[line].line = _textLines[currentLine].line;
                        _displayLines[line].textLine = currentLine;
                        _displayLines[line].textColor = _textLines[currentLine].textColor;
                        _displayLines[line].lineHeight = _lineSize;
                        line++;
                    }
                    catch (Exception e)
                    {
                        FormMain.Instance.WriteErrorFile(FormMain.Instance.InputPanel.CurrentConnection, "FormatLines Error1:", e);
                    }
                }
                else
                {
                    lastColor = "";
                    lineSplit = false;
                    string curLine = _textLines[currentLine].line;

                    StringBuilder buildString = new StringBuilder();
                    
                    bool bold = false;
                    bool underline = false;
                    bool italic = false;
                    bool reverse = false;

                    int boldPos = 0;
                    int underlinePos = 0;

                    char[] ch;
                    try
                    {
                        for (int i = 0; i < curLine.Length; i++)
                        {
                            ch = curLine.Substring(i, 1).ToCharArray();
                            switch (ch[0])
                            {
                                case boldChar:
                                    bold = !bold;
                                    buildString.Append(ch[0]);
                                    boldPos = i;                                   
                                    break;
                                case italicChar:
                                    italic = !italic;
                                    buildString.Append(ch[0]);
                                    break;
                                case underlineChar:
                                    underline = !underline;
                                    buildString.Append(ch[0]);
                                    underlinePos = i;
                                    break;
                                case reverseChar:
                                    reverse = !reverse;
                                    buildString.Append(ch[0]);
                                    break;
                                case plainChar:
                                    underline = false;
                                    italic = false;
                                    bold = false;
                                    reverse = false;
                                    boldPos = i;
                                    underlinePos = i;
                                    break;
                                case newColorChar:
                                    buildString.Append(curLine.Substring(i, 5));
                                    if (lastColor.Length == 0)
                                        lastColor = curLine.Substring(i, 5);
                                    else
                                        nextColor = curLine.Substring(i, 5);

                                    i = i + 4;
                                    break;
                                case emotChar:
                                    buildString.Append(curLine.Substring(i, 4));
                                    i = i + 3;
                                    break;
                                default:
                                    //check if there needs to be a linewrap                                    
                                    if ((int)g.MeasureString(StripAllCodes(buildString.ToString()), this.Font, 0, stringFormat).Width > displayWidth)
                                    {
                                        //check for line wrapping
                                        int lastSpace = buildString.ToString().LastIndexOf(' ');
                                        if (lastSpace > (buildString.Length * 4 /5))
                                        {
                                            int intNewPos = i - (buildString.Length - lastSpace) + 1;

                                            buildString.Remove(lastSpace, buildString.Length - lastSpace);
                                            
                                            //check for bold and underline accordingly


                                            i = intNewPos;
                                            ch = curLine.Substring(i, 1).ToCharArray();
                                        }
                                        
                                        if (lineSplit)
                                            _displayLines[line].line = lastColor + buildString.ToString();
                                        else
                                            _displayLines[line].line = buildString.ToString();

                                        _displayLines[line].textLine = currentLine;
                                        _displayLines[line].wrapped = true;
                                        _displayLines[line].textColor = _textLines[currentLine].textColor;
                                        _displayLines[line].lineHeight = _lineSize;

                                        lineSplit = true;
                                        if (nextColor.Length != 0)
                                        {
                                            lastColor = nextColor;
                                            nextColor = "";
                                        }
                                        line++;
                                        _displayLines[line].previous = true;
                                        
                                        buildString = null;
                                        buildString = new StringBuilder();

                                        if (underline) buildString.Append(underlineChar);
                                        if (bold) buildString.Append(boldChar);
                                        if (italic) buildString.Append(italicChar);
                                        if (reverse) buildString.Append(reverseChar);
                                        buildString.Append(ch[0]);
                                    }
                                    else
                                        buildString.Append(ch[0]);
                                    break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Line:" + curLine.Length + ":" + curLine);
                        FormMain.Instance.WriteErrorFile(FormMain.Instance.InputPanel.CurrentConnection, "FormatLines Error2:", e);
                    }

                    //get the remainder
                    if (lineSplit)
                        _displayLines[line].line = lastColor + buildString.ToString();
                    else
                        _displayLines[line].line = buildString.ToString();

                    buildString = null;

                    _displayLines[line].textLine = currentLine;
                    _displayLines[line].textColor = _textLines[currentLine].textColor;
                    _displayLines[line].lineHeight = _lineSize;

                    line++;
                }
            }

            g.Dispose();

            return line;
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            if (!e.ClipRectangle.IsEmpty)
                OnDisplayText(e);
        }

        /// <summary>
        /// Method used to draw the actual text data for the Control
        /// </summary>
        private void OnDisplayText(PaintEventArgs e)
        {

            if (_reformatLines)
            {
                _totaldisplayLines = FormatLines(_totalLines, 1, 0);
                UpdateScrollBar(_totaldisplayLines);
                _reformatLines = false;
            }

            try
            {
                int startY;
                float startX = 0;
                int LinesToDraw = 0;

                StringBuilder buildString = new StringBuilder();
                int textSize;

                int curLine;
                int curForeColor, curBackColor;
                char[] ch;

                Rectangle displayRect = new Rectangle(0, 0, this.Width, this.Height);
                Bitmap buffer = new Bitmap(this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                Graphics g = Graphics.FromImage(buffer);

                if (_backgroundImage != null)
                    g.DrawImage((Image)_backgroundImage, displayRect);
                else
                    g.FillRectangle(new SolidBrush(IrcColor.colors[_backColor]), displayRect);

                g.InterpolationMode = InterpolationMode.Low;
                g.SmoothingMode = SmoothingMode.HighSpeed;
                g.PixelOffsetMode = PixelOffsetMode.None;
                g.CompositingQuality = CompositingQuality.HighSpeed;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;

                if (_totalLines == 0)
                {
                    e.Graphics.DrawImageUnscaled(buffer, 0, 0);
                    buffer.Dispose();
                    g.Dispose();
                    return;
                }

                int val = vScrollBar.Value;

                LinesToDraw = (_showMaxLines > val ? val : _showMaxLines);
                curLine = val - LinesToDraw;
                
                if (_singleLine)
                {
                    startY = 0;
                    LinesToDraw = 1;
                    curLine = vScrollBar.Value-1;
                    //_showMaxLines = 1;
                }
                else
                    startY = this.Height - (_lineSize * LinesToDraw) - (_lineSize / 2);

                if (!FormMain.Instance.IceChatOptions.EmoticonsFixedSize)
                {
                    //System.Diagnostics.Debug.WriteLine("old Start Y:" + startY + ":" + LinesToDraw + ":" + this.Height);

                    int totalHeight = 0;
                    int newLinesDraw = LinesToDraw;

                    //recalculate if we have different line heights
                    for (int i = 0; i < LinesToDraw; i++)
                    {
                        totalHeight += _displayLines[i].lineHeight;
                        if (_displayLines[i].lineHeight > _lineSize)
                        {
                            if ((this.Height - totalHeight) < (_lineSize * -1))
                            {
                                //System.Diagnostics.Debug.WriteLine("need to adjust lines:" + totalHeight + ":" + this.Height);
                                newLinesDraw--;
                                //totalHeight -= _displayLines[i].lineHeight - _lineSize;
                                //LinesToDraw
                            }
                        }
                    }

                    //int x = LinesToDraw-1;
                    int x = _totaldisplayLines - 1;
                    int th = 0;
                    int count = 0;

                    while (x >= 0)
                    {
                        count++;
                        th += _displayLines[x].lineHeight;
                        //System.Diagnostics.Debug.WriteLine("calc size:" + x + ":" + _displayLines[x].lineHeight + ":" + _displayLines[x].line);
                        if ((this.Height - th) < (_lineSize * -1))
                        {
                            newLinesDraw = count;
                            totalHeight = th;
                            //System.Diagnostics.Debug.WriteLine("reached limit:" + count + ":" + th + ":" + this.Height);
                            break;
                        }
                        x--;
                    }

                    /*
                    if (LinesToDraw > newLinesDraw)
                    {
                        System.Diagnostics.Debug.WriteLine("OLD total height:" + totalHeight);
                        totalHeight = 0;
                        for (int i = 0; i < newLinesDraw; i++)
                        {
                            totalHeight += _displayLines[i].lineHeight;
                        }
                        System.Diagnostics.Debug.WriteLine("new total height:" + totalHeight + ":" + this.Height);
                    }
                    */

                    startY = ((this.Height - totalHeight) - (_lineSize / 2));

                    curLine = val - newLinesDraw;
                    LinesToDraw = newLinesDraw;

                    //System.Diagnostics.Debug.WriteLine("NEW Y:" + startY + ":" + ((this.Height - totalHeight)- (_lineSize / 2)) + ":" + newLinesDraw + ":" + LinesToDraw);
                    //System.Diagnostics.Debug.WriteLine("NEW Y:" + startY + ":" + newLinesDraw + ":" + LinesToDraw + ":" + totalHeight);
                }


                int lineCounter = 0;

                bool isInUrl = false;

                Font font = new Font(this.Font.Name, this.Font.Size, FontStyle.Regular, GraphicsUnit.Point);

                int redline = -1;

                if (FormMain.Instance.IceChatOptions.ShowUnreadLine && !_singleLine)
                {
                    for (int i = _totaldisplayLines - 1, j = 0; i >= 0; --i)
                    {
                        if (!_displayLines[i].previous)
                        {
                            ++j;
                            if (j >= _unreadMarker)
                            {
                                redline = i;
                                break;
                            }
                        }
                    }
                }

                //if (this.Parent != null)
                //    if (this.Parent.Name == "panelTopic")
                //        System.Diagnostics.Debug.WriteLine("linestodraw=" + LinesToDraw + ":Max=" + _showMaxLines + ":value=" + vScrollBar.Value + ":cur=" + curLine);

                while (lineCounter < LinesToDraw)
                {
                    int i = 0, j = 0;
                    bool highlight = false;
                    bool oldHighlight = false;

                    bool underline = false;
                    bool reverse = false;
                    bool italic = false;
                    bool bold = false;

                    if (redline == curLine)
                    {
                        Pen p = new Pen(IrcColor.colors[FormMain.Instance.IceChatColors.UnreadTextMarkerColor]);
                        g.DrawLine(p, 0, startY, this.Width, startY);
                    }

                    lineCounter++;

                    curForeColor = _displayLines[curLine].textColor;
                    StringBuilder line = new StringBuilder();
                
                    line.Append(_displayLines[curLine].line);
                    curBackColor = _backColor;
                    
                    //check if in a url
                    if (!isInUrl)
                    {
                        //underline = false;
                        font = null;
                        font = new Font(this.Font.Name, this.Font.Size, FontStyle.Regular);
                    }

                    //System.Diagnostics.Debug.WriteLine(lineCounter + ":" + _displayLines[curLine].line + ":" + _displayLines.Length + ":" + line);     

                    if (line.Length > 0)
                    {
                        do
                        {
                            ch = line.ToString().Substring(i, 1).ToCharArray();
                            switch (ch[0])
                            {
                                case emotChar:
                                    //draws an emoticon
                                    //[]001
                                    int emotNumber = Convert.ToInt32(line.ToString().Substring(i + 1, 3));
                                    line.Remove(0, 3);
                                    if (!isInUrl)
                                    {
                                        //select the emoticon here
                                        Bitmap bm = new Bitmap(FormMain.Instance.EmoticonsFolder + System.IO.Path.DirectorySeparatorChar + FormMain.Instance.IceChatEmoticons.listEmoticons[emotNumber].EmoticonImage);

                                        if (curBackColor != _backColor)
                                        {
                                            textSize = (int)g.MeasureString(buildString.ToString(), this.Font, 0, stringFormat).Width + 1;
                                            Rectangle r = new Rectangle((int)startX, startY, textSize + 1, _lineSize + 1);
                                            g.FillRectangle(new SolidBrush(IrcColor.colors[curBackColor]), r);
                                        }

                                        if (FormMain.Instance.IceChatOptions.EmoticonsFixedSize)
                                        {
                                            g.DrawImage((Image)bm, startX + (int)g.MeasureString(buildString.ToString(), this.Font, 0, stringFormat).Width, startY, _lineSize, _lineSize);
                                            g.DrawString(buildString.ToString(), this.Font, new SolidBrush(IrcColor.colors[curForeColor]), startX, startY, stringFormat);
                                            startX += _lineSize + (int)g.MeasureString(buildString.ToString(), this.Font, 0, stringFormat).Width;
                                        }
                                        else
                                        {
                                            g.DrawImage((Image)bm, startX + (int)g.MeasureString(buildString.ToString(), this.Font, 0, stringFormat).Width + 1, startY, bm.Width, bm.Height);

                                            if (bm.Height > _lineSize)
                                            {
                                                //now how much extra height do we need to add?
                                                if (_displayLines[curLine].lineHeight < bm.Height)
                                                {
                                                    _displayLines[curLine].lineHeight = bm.Height;
                                                    //this causes a SCREEN FLASH, need to find out why
                                                    //System.Diagnostics.Debug.WriteLine("FORCE REDRAW");
                                                    buffer.Dispose();
                                                    g.Dispose();
                                                    Invalidate();
                                                    return;
                                                }
                                            }
                                            g.DrawString(buildString.ToString(), this.Font, new SolidBrush(IrcColor.colors[curForeColor]), startX, startY, stringFormat);
                                            startX += bm.Width + (int)g.MeasureString(buildString.ToString(), this.Font, 0, stringFormat).Width;
                                        }

                                        buildString = null;
                                        buildString = new StringBuilder();
                                    }
                                    else
                                    {
                                        buildString.Append(FormMain.Instance.IceChatEmoticons.listEmoticons[emotNumber].Trigger);
                                    }
                                    break;
                                case urlStart:
                                    if (curBackColor != _backColor)
                                    {
                                        textSize = (int)g.MeasureString(buildString.ToString(), this.Font, 0, stringFormat).Width + 1;
                                        Rectangle r = new Rectangle((int)startX, startY, textSize + 1, _lineSize + 1);
                                        g.FillRectangle(new SolidBrush(IrcColor.colors[curBackColor]), r);
                                    }
                                    g.DrawString(buildString.ToString(), font, new SolidBrush(IrcColor.colors[curForeColor]), startX, startY, stringFormat);

                                    startX += g.MeasureString(buildString.ToString(), font, 0, stringFormat).Width;   //textSizes[32]

                                    buildString = null;
                                    buildString = new StringBuilder();

                                    //remove whats drawn from string
                                    line.Remove(0, i);
                                    line.Remove(0, 1);
                                    i = -1;
                                    font = null;
                                    font = new Font(this.Font.Name, this.Font.Size, FontStyle.Underline, GraphicsUnit.Point);
                                    isInUrl = true;
                                    break;

                                case urlEnd:
                                    if (curBackColor != _backColor)
                                    {
                                        textSize = (int)g.MeasureString(buildString.ToString(), font, 0, stringFormat).Width + 1;
                                        Rectangle r = new Rectangle((int)startX, startY, textSize + 1, _lineSize + 1);
                                        g.FillRectangle(new SolidBrush(IrcColor.colors[curBackColor]), r);
                                    }
                                    g.DrawString(buildString.ToString(), font, new SolidBrush(IrcColor.colors[curForeColor]), startX, startY, stringFormat);

                                    startX += g.MeasureString(buildString.ToString(), font, 0, stringFormat).Width;   //textSizes[32]

                                    buildString = null;
                                    buildString = new StringBuilder();

                                    //remove whats drawn from string
                                    line.Remove(0, i);
                                    line.Remove(0, 1);
                                    i = -1;
                                    font = null;
                                    font = new Font(this.Font.Name, this.Font.Size, FontStyle.Regular, GraphicsUnit.Point);
                                    isInUrl = false;
                                    break;
                                case underlineChar:
                                    if (curBackColor != _backColor)
                                    {
                                        textSize = (int)g.MeasureString(buildString.ToString(), font, 0, stringFormat).Width + 1;
                                        Rectangle r = new Rectangle((int)startX, startY, textSize + 1, _lineSize + 1);
                                        g.FillRectangle(new SolidBrush(IrcColor.colors[curBackColor]), r);
                                    }
                                    g.DrawString(buildString.ToString(), font, new SolidBrush(IrcColor.colors[curForeColor]), startX, startY, stringFormat);

                                    startX += g.MeasureString(buildString.ToString(), font, 0, stringFormat).Width;   //textSizes[32]

                                    buildString = null;
                                    buildString = new StringBuilder();

                                    //remove whats drawn from string
                                    line.Remove(0, i);
                                    line.Remove(0, 1);
                                    i = -1;
                                    font = null;
                                    underline = !underline;
                                    FontStyle fs = FontStyle.Regular;
                                    if (underline) fs = FontStyle.Underline;
                                    if (italic) fs = fs | FontStyle.Italic;
                                    if (bold) fs = fs | FontStyle.Bold;

                                    font = new Font(this.Font.Name, this.Font.Size, fs, GraphicsUnit.Point);
                                    break;
                                case italicChar:
                                    //italic character
                                    if (curBackColor != _backColor)
                                    {
                                        textSize = (int)g.MeasureString(buildString.ToString(), font, 0, stringFormat).Width + 1;
                                        Rectangle r = new Rectangle((int)startX, startY, textSize + 1, _lineSize + 1);
                                        g.FillRectangle(new SolidBrush(IrcColor.colors[curBackColor]), r);
                                    }
                                    g.DrawString(buildString.ToString(), font, new SolidBrush(IrcColor.colors[curForeColor]), startX, startY, stringFormat);

                                    startX += g.MeasureString(buildString.ToString(), font, 0, stringFormat).Width;   //textSizes[32]

                                    buildString = null;
                                    buildString = new StringBuilder();

                                    //remove whats drawn from string
                                    line.Remove(0, i);
                                    line.Remove(0, 1);
                                    i = -1;
                                    font = null;
                                    italic = !italic;

                                    FontStyle fsi = FontStyle.Regular;
                                    if (underline) fsi = FontStyle.Underline;
                                    if (italic) fsi = fsi | FontStyle.Italic;
                                    if (bold) fsi = fsi | FontStyle.Bold;

                                    font = new Font(this.Font.Name, this.Font.Size, fsi, GraphicsUnit.Point);
                                    break;
                                case boldChar:
                                    //bold character, currently ignored
                                    /*
                                    if (curBackColor != _backColor)
                                    {
                                        textSize = (int)g.MeasureString(buildString.ToString(), font, 0, stringFormat).Width + 1;
                                        Rectangle r = new Rectangle((int)startX, startY, textSize + 1, _lineSize + 1);
                                        g.FillRectangle(new SolidBrush(IrcColor.colors[curBackColor]), r);
                                    }
                                    g.DrawString(buildString.ToString(), font, new SolidBrush(IrcColor.colors[curForeColor]), startX, startY, stringFormat);

                                    startX += g.MeasureString(buildString.ToString(), font, 0, stringFormat).Width;   //textSizes[32]

                                    buildString = null;
                                    buildString = new StringBuilder();

                                    //remove whats drawn from string
                                    line.Remove(0, i);
                                    line.Remove(0, 1);
                                    i = -1;
                                    font = null;
                                    bold = !bold;                                    

                                    FontStyle fsb = FontStyle.Regular;
                                    if (underline) fsb = FontStyle.Underline;
                                    if (italic) fsb = fsb | FontStyle.Italic;
                                    if (bold) fsb = fsb | FontStyle.Bold;
                                    
                                    font = new Font(this.Font.Name, this.Font.Size, fsb, GraphicsUnit.Point);
                                    */
                                    break;
                                case plainChar:
                                    //draw with the standard fore and back color
                                    if (curBackColor != _backColor)
                                    {
                                        textSize = (int)g.MeasureString(buildString.ToString(), font, 0, stringFormat).Width + 1;
                                        Rectangle r = new Rectangle((int)startX, startY, textSize + 1, _lineSize + 1);
                                        g.FillRectangle(new SolidBrush(IrcColor.colors[curBackColor]), r);
                                    }
                                    g.DrawString(buildString.ToString(), font, new SolidBrush(IrcColor.colors[curForeColor]), startX, startY, stringFormat);

                                    startX += g.MeasureString(buildString.ToString(), font, 0, stringFormat).Width;   //textSizes[32]

                                    buildString = null;
                                    buildString = new StringBuilder();

                                    //remove whats drawn from string
                                    line.Remove(0, i);
                                    line.Remove(0, 1);

                                    curForeColor = _displayLines[curLine].textColor;
                                    curBackColor = _backColor;

                                    font = null;
                                    underline = false;
                                    italic = false;
                                    bold = false;
                                    font = new Font(this.Font.Name, this.Font.Size, FontStyle.Regular, GraphicsUnit.Point);

                                    i = -1;
                                    break;
                                case reverseChar:
                                    //reverse the fore and back colors
                                    if (curBackColor != _backColor)
                                    {
                                        textSize = (int)g.MeasureString(buildString.ToString(), font, 0, stringFormat).Width + 1;
                                        Rectangle r = new Rectangle((int)startX, startY, textSize + 1, _lineSize + 1);
                                        g.FillRectangle(new SolidBrush(IrcColor.colors[curBackColor]), r);
                                    }
                                    g.DrawString(buildString.ToString(), font, new SolidBrush(IrcColor.colors[curForeColor]), startX, startY, stringFormat);

                                    startX += g.MeasureString(buildString.ToString(), font, 0, stringFormat).Width;   //textSizes[32]

                                    buildString = null;
                                    buildString = new StringBuilder();

                                    //remove whats drawn from string
                                    line.Remove(0, i);
                                    line.Remove(0, 1);
                                    if (reverse)
                                    {
                                        curForeColor = _displayLines[curLine].textColor;
                                        curBackColor = _backColor;
                                    }
                                    else
                                    {
                                        curForeColor = _backColor;
                                        curBackColor = _displayLines[curLine].textColor;
                                    }
                                    reverse = !reverse;
                                    i = -1;
                                    break;
                                case newColorChar:
                                    //draw whats previously in the string
                                    if (curBackColor != _backColor)
                                    {
                                        textSize = (int)g.MeasureString(buildString.ToString(), font, 0, stringFormat).Width + 1;
                                        Rectangle r = new Rectangle((int)startX, startY, textSize + 1, _lineSize + 1);
                                        g.FillRectangle(new SolidBrush(IrcColor.colors[curBackColor]), r);
                                    }
                                    g.DrawString(buildString.ToString(), font, new SolidBrush(IrcColor.colors[curForeColor]), startX, startY, stringFormat);

                                    startX += g.MeasureString(buildString.ToString(), font, 0, stringFormat).Width;   //textSizes[32]

                                    buildString = null;
                                    buildString = new StringBuilder();

                                    //remove whats drawn from string
                                    line.Remove(0, i);

                                    //get the new fore and back colors
                                    if (!highlight)
                                    {
                                        curForeColor = Convert.ToInt32(line.ToString().Substring(1, 2));
                                        curBackColor = Convert.ToInt32(line.ToString().Substring(3, 2));

                                        //check to make sure that FC and BC are in range
                                        if (curForeColor > (IrcColor.colors.Length - 1))
                                            curForeColor = _displayLines[curLine].textColor;
                                        if (curBackColor > (IrcColor.colors.Length - 1))
                                            curBackColor = _backColor;
                                    }

                                    //remove the color codes from the string
                                    line.Remove(0, 5);
                                    i = -1;
                                    break;

                                default:
                                    if (_startHighLine >= 0 &&
                                        ((curLine >= _startHighLine && curLine <= _curHighLine) ||
                                        (curLine <= _startHighLine && curLine >= _curHighLine)))
                                    {
                                        if ((curLine > _startHighLine && curLine < _curHighLine) ||
                                            (curLine == _startHighLine && j >= _startHighChar && (curLine <= _curHighLine && j < _curHighChar || curLine < _curHighLine)) ||
                                            (curLine == _curHighLine && j < _curHighChar && (curLine >= _startHighLine && j >= _startHighChar || curLine > _startHighLine)))
                                            highlight = true;
                                        else if ((curLine < _startHighLine && curLine > _curHighLine) ||
                                            (curLine == _startHighLine && j < _startHighChar && (curLine >= _curHighLine && j >= _curHighChar || curLine > _curHighLine)) ||
                                            (curLine == _curHighLine && j >= _curHighChar && (curLine <= _startHighLine && j < _startHighChar || curLine < _startHighLine)))
                                            highlight = true;
                                        else
                                            highlight = false;
                                    }
                                    else
                                        highlight = false;
                                    ++j;


                                    if (highlight != oldHighlight)
                                    {
                                        oldHighlight = highlight;

                                        //draw whats previously in the string                                
                                        if (curBackColor != _backColor)
                                        {
                                            textSize = (int)g.MeasureString(buildString.ToString(), font, 0, stringFormat).Width + 1;
                                            Rectangle r = new Rectangle((int)startX, startY, textSize + 1, _lineSize + 1);
                                            g.FillRectangle(new SolidBrush(IrcColor.colors[curBackColor]), r);
                                        }
                                        g.DrawString(buildString.ToString(), font, new SolidBrush(IrcColor.colors[curForeColor]), startX, startY, stringFormat);

                                        startX += g.MeasureString(buildString.ToString(), font, 0, stringFormat).Width;   //textSizes[32]

                                        buildString = null;
                                        buildString = new StringBuilder();

                                        //remove whats drawn from string
                                        line.Remove(0, i);
                                        i = 0;
                                        if (highlight)
                                        {
                                            curForeColor = 0;
                                            curBackColor = 2;
                                        }
                                        else
                                        {
                                            curForeColor = _displayLines[curLine].textColor;
                                            curBackColor = _backColor;
                                        }

                                    }
                                    buildString.Append(ch[0]);
                                    break;

                            }

                            i++;

                        } while (line.Length > 0 && i != line.Length);
                    }

                    //draw anything that is left over                
                    if (i == line.Length && line.Length > 0)
                    {

                        if (curBackColor != _backColor)
                        {
                            textSize = (int)g.MeasureString(buildString.ToString(), font, 0, stringFormat).Width + 1;
                            Rectangle r = new Rectangle((int)startX, startY, textSize + 1, _lineSize + 1);
                            g.FillRectangle(new SolidBrush(IrcColor.colors[curBackColor]), r);
                        }
                        g.DrawString(buildString.ToString(), font, new SolidBrush(IrcColor.colors[curForeColor]), startX, startY, stringFormat);

                    }

                    startY += _displayLines[curLine].lineHeight;

                    startX = 0;
                    curLine++;
                    buildString = null;
                    buildString = new StringBuilder();

                }
                buildString = null;

                e.Graphics.DrawImageUnscaled(buffer, 0, 0);
                buffer.Dispose();
                g.Dispose();
            }
            catch (Exception ee)
            {
                FormMain.Instance.WriteErrorFile(FormMain.Instance.InputPanel.CurrentConnection, "TextWindow OnDisplayText", ee);
            }

        }

        #region TextWidth and TextSizes Methods

        private string StripCodes(string line)
        {
            Regex parseStuff = new Regex("\xFF03[0-9]{4}");
            return parseStuff.Replace(line, "");
        }

        private string StripAllCodes(string line)
        {
            if (line == null)
                return "";
            if (line.Length > 0)
            {
                Regex parseStuff = new Regex("\xFF03[0-9]{4}|\xFF0A|\xFF0B|\xFF0C");
                return parseStuff.Replace(line, "");
            }
            else
                return "";
        }

        private void LoadTextSizes()
        {
            Graphics g = this.CreateGraphics();

            _lineSize = Convert.ToInt32(this.Font.GetHeight(g));
            _showMaxLines = (this.Height / _lineSize) + 1;
            vScrollBar.LargeChange = _showMaxLines;

            g.Dispose();

        }
        #endregion

        private string ParseUrl(string data)
        {
            Regex re = new Regex(_wwwMatch);
            MatchCollection matches = re.Matches(data);
            foreach (Match m in matches)
            {
                data = data.Replace(StripCodes(m.Value), urlStart + StripCodes(m.Value) + urlEnd);
            }

            return data;
        }

        /// <summary>
        /// Replacement for graphics.MeasureString
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        /*
        private int MeasureStringWidth(Graphics graphics, string text)
        {

            if (text.Length == 0)
                return 0;

            Size size = TextRenderer.MeasureText(graphics, text, new Font(this.Font.Name, this.Font.Size, FontStyle.Regular, GraphicsUnit.Point), new Size(1000,1000), TextFormatFlags.NoPadding);
            //Size size = TextRenderer.MeasureText(graphics, text, this.Font);

            return size.Width + 1;

            System.Drawing.StringFormat format = new System.Drawing.StringFormat(StringFormatFlags.MeasureTrailingSpaces);

            System.Drawing.RectangleF rect = new System.Drawing.RectangleF(0, 0, 1000, 1000);
            System.Drawing.CharacterRange[] ranges = { new System.Drawing.CharacterRange(0, text.Length) };
            System.Drawing.Region[] regions = new System.Drawing.Region[1];

            format.SetMeasurableCharacterRanges(ranges);

            try
            {
                regions = graphics.MeasureCharacterRanges(text, this.Font, rect, format);                
                rect = regions[0].GetBounds(graphics);
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("regions:" + regions.Length + ":" + text.Length + ":" + rect.Right + ":" + rect.Left + ":" + text);
            }
            //return Convert.ToInt32(Math.Round(rect.Right - rect.Left)); 
        }
        */
    }

    #region ColorButton Class
    public class ColorButtonArray
    {
        //initialize 72 boxes for the 72 default colors

        private readonly System.Windows.Forms.Panel hostPanel;

        public delegate void ColorSelected(int ColorNumber);
        public event ColorSelected OnClick;

        private int selectedColor;

        private void OnPaint(object sender, PaintEventArgs e)
        {
            //draw the 72 colors, in 6 rows of 12
            for (int i = 0; i <= 11; i++)
            {

                e.Graphics.FillRectangle(new SolidBrush(IrcColor.colors[i]), (i * 17), 0, 15, 15);
                e.Graphics.DrawRectangle(new Pen(Color.Gray), (i * 17), 0, 15, 15);

                e.Graphics.FillRectangle(new SolidBrush(IrcColor.colors[i + 12]), (i * 17), 20, 15, 15);
                e.Graphics.DrawRectangle(new Pen(Color.Gray), (i * 17), 20, 15, 15);

                e.Graphics.FillRectangle(new SolidBrush(IrcColor.colors[i + 24]), (i * 17), 40, 15, 15);
                e.Graphics.DrawRectangle(new Pen(Color.Gray), (i * 17), 40, 15, 15);

                e.Graphics.FillRectangle(new SolidBrush(IrcColor.colors[i + 36]), (i * 17), 60, 15, 15);
                e.Graphics.DrawRectangle(new Pen(Color.Gray), (i * 17), 60, 15, 15);

                e.Graphics.FillRectangle(new SolidBrush(IrcColor.colors[i + 48]), (i * 17), 80, 15, 15);
                e.Graphics.DrawRectangle(new Pen(Color.Gray), (i * 17), 80, 15, 15);

                e.Graphics.FillRectangle(new SolidBrush(IrcColor.colors[i + 60]), (i * 17), 100, 15, 15);
                e.Graphics.DrawRectangle(new Pen(Color.Gray), (i * 17), 100, 15, 15);

                if (i == selectedColor)
                {
                    //draw a selection rectangle
                    e.Graphics.DrawRectangle(new Pen(Color.Black, 3), (i * 17), 0, 15, 15);
                }
                if (i + 12 == selectedColor)
                {
                    //draw a selection rectangle
                    e.Graphics.DrawRectangle(new Pen(Color.Black, 3), (i * 17), 20, 15, 15);
                }
                if (i + 24 == selectedColor)
                {
                    //draw a selection rectangle
                    e.Graphics.DrawRectangle(new Pen(Color.Black, 3), (i * 17), 40, 15, 15);
                }
                if (i + 36 == selectedColor)
                {
                    //draw a selection rectangle
                    e.Graphics.DrawRectangle(new Pen(Color.Black, 3), (i * 17), 60, 15, 15);
                }
                if (i + 48 == selectedColor)
                {
                    //draw a selection rectangle
                    e.Graphics.DrawRectangle(new Pen(Color.Black, 3), (i * 17), 80, 15, 15);
                }
                if (i + 60 == selectedColor)
                {
                    //draw a selection rectangle
                    e.Graphics.DrawRectangle(new Pen(Color.Black, 3), (i * 17), 100, 15, 15);
                }
            }
        }

        internal int SelectedColor
        {
            get { return selectedColor; }
            set { selectedColor = value; hostPanel.Invalidate(); }
        }

        internal ColorButtonArray(System.Windows.Forms.Panel host)
        {
            this.hostPanel = host;

            host.Paint += new PaintEventHandler(OnPaint);
            host.MouseUp += new MouseEventHandler(OnMouseUp);
        }

        private void OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            int xPos;
            if (e.Y < 18)
            {
                xPos = e.X / 17;
                if (OnClick != null)
                {
                    selectedColor = xPos;
                    hostPanel.Invalidate();
                    OnClick(xPos);
                }

            }
            else if ((e.Y > 19) && e.Y < 38)
            {
                xPos = e.X / 17;
                if (OnClick != null)
                {
                    selectedColor = xPos + 12;
                    hostPanel.Invalidate();
                    OnClick(xPos + 12);
                }
            }
            else if ((e.Y > 39) && e.Y < 58)
            {
                xPos = e.X / 17;
                if (OnClick != null)
                {
                    selectedColor = xPos + 24;
                    hostPanel.Invalidate();
                    OnClick(xPos + 24);
                }
            }
            else if ((e.Y > 59) && e.Y < 79)
            {
                xPos = e.X / 17;
                if (OnClick != null)
                {
                    selectedColor = xPos + 36;
                    hostPanel.Invalidate();
                    OnClick(xPos + 36);
                }
            }
            else if ((e.Y > 79) && e.Y < 99)
            {
                xPos = e.X / 17;
                if (OnClick != null)
                {
                    selectedColor = xPos + 48;
                    hostPanel.Invalidate();
                    OnClick(xPos + 48);
                }
            }
            else if ((e.Y > 99) && e.Y < 119)
            {
                xPos = e.X / 17;
                if (OnClick != null)
                {
                    selectedColor = xPos + 60;
                    hostPanel.Invalidate();
                    OnClick(xPos + 60);
                }
            }
        }
    }
    #endregion

    #region IRC Colors Class (72 colors)

    public static class IrcColor
    {
        public static Color[] colors;

        static IrcColor()
        {
            //Color color;
            colors = new Color[72];

            colors[0] = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
            colors[1] = System.Drawing.ColorTranslator.FromHtml("#000000");
            colors[2] = System.Drawing.ColorTranslator.FromHtml("#00007F");
            colors[3] = System.Drawing.ColorTranslator.FromHtml("#009300");
            colors[4] = System.Drawing.ColorTranslator.FromHtml("#FF0000");
            colors[5] = System.Drawing.ColorTranslator.FromHtml("#7F0000");
            colors[6] = System.Drawing.ColorTranslator.FromHtml("#9C009C");
            colors[7] = System.Drawing.ColorTranslator.FromHtml("#FC7F00");

            colors[8] = System.Drawing.ColorTranslator.FromHtml("#FFFF00");
            colors[9] = System.Drawing.ColorTranslator.FromHtml("#00FC00");
            colors[10] = System.Drawing.ColorTranslator.FromHtml("#009393");
            colors[11] = System.Drawing.ColorTranslator.FromHtml("#00FFFF");
            colors[12] = System.Drawing.ColorTranslator.FromHtml("#0000FC");
            colors[13] = System.Drawing.ColorTranslator.FromHtml("#FF00FF");
            colors[14] = System.Drawing.ColorTranslator.FromHtml("#7F7F7F");
            colors[15] = System.Drawing.ColorTranslator.FromHtml("#D2D2D2");

            colors[16] = System.Drawing.ColorTranslator.FromHtml("#CCFFCC");
            colors[17] = System.Drawing.ColorTranslator.FromHtml("#0066FF");
            colors[18] = System.Drawing.ColorTranslator.FromHtml("#FAEBD7");
            colors[19] = System.Drawing.ColorTranslator.FromHtml("#FFD700");
            colors[20] = System.Drawing.ColorTranslator.FromHtml("#E6E6E6");
            colors[21] = System.Drawing.ColorTranslator.FromHtml("#4682B4");
            colors[22] = System.Drawing.ColorTranslator.FromHtml("#993333");
            colors[23] = System.Drawing.ColorTranslator.FromHtml("#FF99FF");

            colors[24] = System.Drawing.ColorTranslator.FromHtml("#DDA0DD");
            colors[25] = System.Drawing.ColorTranslator.FromHtml("#8B4513");
            colors[26] = System.Drawing.ColorTranslator.FromHtml("#CC0000");
            colors[27] = System.Drawing.ColorTranslator.FromHtml("#FFFF99");
            colors[28] = System.Drawing.ColorTranslator.FromHtml("#339900");
            colors[29] = System.Drawing.ColorTranslator.FromHtml("#FF9900");
            
            colors[30] = System.Drawing.ColorTranslator.FromHtml("#FFDAB9");
            colors[31] = System.Drawing.ColorTranslator.FromHtml("#2F4F4F");
            colors[32] = System.Drawing.ColorTranslator.FromHtml("#D8E9EC");
            colors[33] = System.Drawing.ColorTranslator.FromHtml("#5FDAEE");
            colors[34] = System.Drawing.ColorTranslator.FromHtml("#E2FF00");
            colors[35] = System.Drawing.ColorTranslator.FromHtml("#00009E");

            colors[36] = System.Drawing.ColorTranslator.FromHtml("#FFFFCC");
            colors[37] = System.Drawing.ColorTranslator.FromHtml("#FFFF99");
            colors[38] = System.Drawing.ColorTranslator.FromHtml("#FFFF66");
            colors[39] = System.Drawing.ColorTranslator.FromHtml("#FFCC33");
            colors[40] = System.Drawing.ColorTranslator.FromHtml("#FF9933");
            colors[41] = System.Drawing.ColorTranslator.FromHtml("#FF6633");

            colors[42] = System.Drawing.ColorTranslator.FromHtml("#c6ffc6");
            colors[43] = System.Drawing.ColorTranslator.FromHtml("#84ff84");
            colors[44] = System.Drawing.ColorTranslator.FromHtml("#00ff00");
            colors[45] = System.Drawing.ColorTranslator.FromHtml("#00c700");
            colors[46] = System.Drawing.ColorTranslator.FromHtml("#008600");
            colors[47] = System.Drawing.ColorTranslator.FromHtml("#004100");

            //blues
            colors[48] = System.Drawing.ColorTranslator.FromHtml("#C6FFFF");
            colors[49] = System.Drawing.ColorTranslator.FromHtml("#84FFFF");
            colors[50] = System.Drawing.ColorTranslator.FromHtml("#00FFFF");
            colors[51] = System.Drawing.ColorTranslator.FromHtml("#6699FF");
            colors[52] = System.Drawing.ColorTranslator.FromHtml("#6666FF");
            colors[53] = System.Drawing.ColorTranslator.FromHtml("#3300FF");
            

            
            //reds
            colors[54] = System.Drawing.ColorTranslator.FromHtml("#FFCC99");
            colors[55] = System.Drawing.ColorTranslator.FromHtml("#FF9966");
            colors[56] = System.Drawing.ColorTranslator.FromHtml("#ff6633");
            colors[57] = System.Drawing.ColorTranslator.FromHtml("#FF0033");
            colors[58] = System.Drawing.ColorTranslator.FromHtml("#CC0000");
            colors[59] = System.Drawing.ColorTranslator.FromHtml("#AA0000");


            //pink / purple
            colors[60] = System.Drawing.ColorTranslator.FromHtml("#ffc7ff");
            colors[61] = System.Drawing.ColorTranslator.FromHtml("#ff86ff");
            colors[62] = System.Drawing.ColorTranslator.FromHtml("#ff00ff");
            colors[63] = System.Drawing.ColorTranslator.FromHtml("#FF00CC");
            colors[64] = System.Drawing.ColorTranslator.FromHtml("#CC0099");
            colors[65] = System.Drawing.ColorTranslator.FromHtml("#660099");


            //gray scale
            colors[66] = System.Drawing.ColorTranslator.FromHtml("#EEEEEE");
            colors[67] = System.Drawing.ColorTranslator.FromHtml("#CCCCCC");
            colors[68] = System.Drawing.ColorTranslator.FromHtml("#AAAAAA");
            colors[69] = System.Drawing.ColorTranslator.FromHtml("#888888");
            colors[70] = System.Drawing.ColorTranslator.FromHtml("#666666");
            colors[71] = System.Drawing.ColorTranslator.FromHtml("#444444");

            /*
    
        'extended support for 72 colors now (another 40 colors)
        
       
        'pink / purple
        Case 60: AnsiColor = 16762879
        Case 61: AnsiColor = 16746239
        Case 62: AnsiColor = 16711935
        Case 63: AnsiColor = &HFF00CC
        Case 64: AnsiColor = &HCC0099
        Case 65: AnsiColor = &H660099
        
        
        'the gray scales
        Case 66: AnsiColor = &HEEEEEE
        Case 67: AnsiColor = &HCCCCCC
        Case 68: AnsiColor = &HAAAAAA
        Case 69: AnsiColor = &H888888
        Case 70: AnsiColor = &H666666
        Case 71: AnsiColor = &H444444
*/
        }
    }

    #endregion

}
