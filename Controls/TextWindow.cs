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
//using System.Runtime.InteropServices;
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

        //works but no www.
        private string _wwwMatch = @"((https?|ftp|telnet|file|news|irc):((//)|(\\\\))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)";
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
        }

        #endregion

        private delegate void ScrollValueDelegate(int value);

        private readonly int _maxTextLines = 500;

        private Logging _logClass;

        private int _unreadMarker;  // Unread marker
        private bool _unreadReset; // Unread marker 

        private bool _reformatLines;

        private Bitmap _backgroundImage = null;
        private string _backgroundImageFile;

        public TextWindow()
        {
            InitializeComponent();

            _displayLines = new DisplayLine[_maxTextLines * 4];
            _textLines = new TextLine[_maxTextLines];

            this.MouseUp += new MouseEventHandler(OnMouseUp);
            this.MouseDown += new MouseEventHandler(OnMouseDown);
            this.MouseMove += new MouseEventHandler(OnMouseMove);
            this.Paint += new PaintEventHandler(OnPaint);
            this.FontChanged += new EventHandler(OnFontChanged);
            this.Resize += new EventHandler(OnResize);
            this.vScrollBar.Scroll += new ScrollEventHandler(OnScroll);
            this.DoubleClick += new EventHandler(OnDoubleClick);
            
            this.DoubleBuffered = true;
            
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            LoadTextSizes();
            
            
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

            //System.Diagnostics.Debug.WriteLine(_linkedWord+ ":" + StripString(_linkedWord));

            if (_linkedWord.Length > 0)
            {                
                Regex re = new Regex(_wwwMatch);
                MatchCollection matches = re.Matches(_linkedWord);
                if (matches.Count > 0)
                    this.Cursor = Cursors.Hand;
                else if (this.Parent.GetType() == typeof(IceTabPage) && this.Cursor != Cursors.Hand)
                {
                    IceTabPage t = (IceTabPage)this.Parent;
                    if (t.WindowStyle != IceTabPage.WindowType.Debug)
                    {
                        //check if we are over a channel name
                        if (Array.IndexOf(t.Connection.ServerSetting.ChannelTypes, _linkedWord[0]) != -1)
                            this.Cursor = Cursors.Hand;

                        string _linkedWordNick = StripString(_linkedWord);

                        //check if over a nick name
                        if (t.WindowStyle == IceTabPage.WindowType.Channel && this.Cursor != Cursors.Hand)
                        {
                            foreach (User u in t.Nicks.Values)
                            {
                                //System.Diagnostics.Debug.WriteLine(u.NickName + ":" + u.ToString() + ":" + _linkedWord);
                                if (u.NickName == _linkedWordNick)
                                {
                                    this.Cursor = Cursors.Hand;
                                    return;
                                }
                                else if (u.NickName == _linkedWord)
                                {
                                    this.Cursor = Cursors.Hand;
                                    return;
                                }
                            }
                        }
                    }
                }
                else if (this.Parent.GetType() == typeof(ConsoleTab) && this.Cursor != Cursors.Hand)
                {
                    ConsoleTab c = (ConsoleTab)this.Parent;
                    if (c.Connection != null)
                    {
                        //check if we are over a channel name
                        if (c.Connection.IsFullyConnected)
                            if (Array.IndexOf(c.Connection.ServerSetting.ChannelTypes, _linkedWord[0]) != -1)
                                this.Cursor = Cursors.Hand;
                    }
                }

            }
            else
            {
                this.Cursor = Cursors.Default;
            }

            //get the current character the mouse is over. 
            _curHighLine = ((this.Height + (_lineSize / 2)) - e.Y) / _lineSize;
            _curHighLine = _totaldisplayLines - _curHighLine;
            _curHighLine = (_curHighLine - (_totaldisplayLines - vScrollBar.Value));            
            _curHighChar = ReturnChar(line, e.Location.X);
            
            if (_startHighLine!=-1)
                Invalidate();

        }

        private string ReturnWord(int lineNumber, int x)
        {
            if (lineNumber < _totaldisplayLines && lineNumber >= 0)
            {
                Graphics g = this.CreateGraphics();
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                StringFormat sf = StringFormat.GenericTypographic;
                sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

                string line = StripAllCodes(_displayLines[lineNumber].line);

                int width = (int)g.MeasureString(line, this.Font, 0, sf).Width;

                if (x > width)
                    return "";

                int space = 0;
                bool foundSpace = false;
                float lookWidth = 0;
                for (int i = 0; i < line.Length; i++)
                {
                    if (lookWidth >= x && foundSpace)
                    {
                        //System.Diagnostics.Debug.WriteLine(line.Substring(space, i - space));
                        if (_displayLines[lineNumber].previous && lineNumber > 0 && space == 0)
                        {
                            // this line wraps from the previous one. 
                            string prevline = StripAllCodes(_displayLines[lineNumber-1].line);
                            int prevwidth = (int)g.MeasureString(prevline, this.Font, 0, sf).Width;
                            return ReturnWord(lineNumber - 1, prevwidth);
                        }                         
                        
                        return line.Substring(space, i - space);
                    }

                    if (line[i] == (char)32)
                    {
                        if (!foundSpace)
                        {
                            //System.Diagnostics.Debug.WriteLine("found space:" + i + ":" + x + ":" + lookWidth);
                            if (lookWidth >= x)
                                foundSpace = true;
                            else
                                space = i + 1;
                        }
                    }

                    lookWidth += g.MeasureString(line[i].ToString(), this.Font, 0, sf).Width;
                }
                if (_displayLines[lineNumber].previous && lineNumber>0 && space==0)
                {
                    // this line wraps from the previous one. 
                    string prevline = StripAllCodes(_displayLines[lineNumber-1].line);
                    if (prevline[prevline.Length - 1] != ' ')
                    {
                        int prevwidth = (int)g.MeasureString(prevline, this.Font, 0, sf).Width;
                        return ReturnWord(lineNumber - 1, prevwidth);
                    }
                }
                
                if (!foundSpace && space<line.Length)                
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

                        //System.Diagnostics.Debug.WriteLine(lineNumber + ":" + line.Substring(space) + extra);
                        return line.Substring(space) + extra;

                    }
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

                StringFormat sf = StringFormat.GenericTypographic;
                sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

                string line = StripAllCodes(_displayLines[lineNumber].line);

                int width = (int)g.MeasureString(line, this.Font, 0, sf).Width;

                if (x > width)
                    return line.Length;

                float lookWidth = 0;
                for (int i = 0; i < line.Length; i++)
                {
                    lookWidth += g.MeasureString(line[i].ToString(), this.Font, 0, sf).Width;
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
                            break;
                        }
                        else if (u.NickName == _linkedWord)
                        {
                            popupType = "NickList";
                            _linkedWordNick = _linkedWord;
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
                                    ((ToolStripMenuItem)_popupMenu.Items[subMenu]).DropDownItems.Add(t);

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
                            System.Diagnostics.Process.Start(clickedWord);
                        }
                        catch (Win32Exception)
                        {
                        }
                        return;
                    }

                    //check if it is a irc:// link
                    if (clickedWord.StartsWith("irc://"))
                    {
                        FormMain.Instance.ParseOutGoingCommand(null, "/server " + clickedWord.Substring(6).TrimEnd());
                        return;
                    }

                    if (this.Parent.GetType() == typeof(IceTabPage))
                    {
                        IceTabPage t = (IceTabPage)this.Parent;
                        //check if it is a channel
                        if (Array.IndexOf(t.Connection.ServerSetting.ChannelTypes, clickedWord[0]) != -1)
                        {
                            FormMain.Instance.ParseOutGoingCommand(t.Connection, "/join " + clickedWord);
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
                                if (Array.IndexOf(c.Connection.ServerSetting.ChannelTypes, clickedWord[0]) != -1)
                                {
                                    FormMain.Instance.ParseOutGoingCommand(c.Connection, "/join " + clickedWord);
                                    return;
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

        internal bool SingleLine
        {
            get
            {
                return _singleLine;
            }
            set
            {
                _singleLine = value;
                vScrollBar.Visible = !value;
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

                    //check of _foreColor is less then 32     
                    if (_foreColor > 31)
                    {
                        _foreColor = _foreColor - 32;
                        if (_foreColor > 31)
                            _foreColor = _foreColor - 32;
                    }
                }
                else
                    _foreColor = color;

                if (!_singleLine && _showTimeStamp)
                    newLine = DateTime.Now.ToString(FormMain.Instance.IceChatOptions.TimeStamp) + newLine;

                if (_noColorMode)
                    newLine = StripCodes(newLine);
                else
                    newLine = RedefineColorCodes(newLine);

                if (_logClass != null)
                    _logClass.WriteLogFile(newLine);
                
                _totalLines++;

                newLine = ParseEmoticons(newLine);

                if (_singleLine) _totalLines = 1;

                //check if 500 lines, and trim if so
                if (_totalLines >= (_maxTextLines - 5))
                {
                    //System.Diagnostics.Debug.WriteLine("Reset Lines back to " + Max_textLines + ":" + totalLines);
                    int x = 1;
                    for (int i = _totalLines - (_maxTextLines - 50); i <= _totalLines - 1; i++)
                    {
                        _textLines[x].totalLines = _textLines[i].totalLines;
                        _textLines[x].width = _textLines[i].width;
                        _textLines[x].line = _textLines[i].line;
                        _textLines[x].textColor = _textLines[i].textColor;
                        x++;
                    }

                    for (int i = (_maxTextLines - 49); i < _maxTextLines; i++)
                    {
                        _textLines[i].totalLines = 0;
                        _textLines[i].line = "";
                        _textLines[i].width = 0;
                    }

                    _totalLines = _maxTextLines - 50;

                    if (this.Height != 0)
                    {
                        _totaldisplayLines = FormatLines(_totalLines, 1, 0);
                        UpdateScrollBar(_totaldisplayLines);
                        Invalidate();
                    }

                    _totalLines++;
                }

                //add line numbers for temp measure
                //newLine = _totalLines.ToString() + ":" + newLine; 

                _textLines[_totalLines].line = newLine;

                Graphics g = this.CreateGraphics();
                //properly measure for bold characters needed
                StringFormat sf = StringFormat.GenericTypographic;
                sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                _textLines[_totalLines].width = (int)g.MeasureString(StripCodes(newLine), this.Font, 0, sf).Width;

                g.Dispose();

                _textLines[_totalLines].textColor = _foreColor;

                int addedLines = FormatLines(_totalLines, _totalLines, _totaldisplayLines);
                addedLines -= _totaldisplayLines;

                _textLines[_totalLines].totalLines = addedLines;

                for (int i = _totaldisplayLines + 1; i < _totaldisplayLines + addedLines; i++)
                    _displayLines[i].textLine = _totalLines;

                _totaldisplayLines += addedLines;

                if (_singleLine)
                {
                    _totalLines = 1;
                    _totaldisplayLines = 1;
                    _displayLines[1].textLine = 1;
                    _textLines[1].totalLines = 1;
                }

                UpdateScrollBar(_totaldisplayLines);

                Invalidate();
            }
            catch (Exception e)
            {
                FormMain.Instance.WriteErrorFile(FormMain.Instance.InputPanel.CurrentConnection,"AppendText", e);
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
                        System.Diagnostics.Debug.WriteLine("match:" + (x + totalChars));
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
            return Regex.Replace(targetString, @"[^A-Za-z0-9_-|\[\]\\\/`\^{}]", "");
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
            //_totaldisplayLines = FormatLines(_totalLines, 1, 0);
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
                
                if (newValue != 0)
                {
                    vScrollBar.Minimum = 1;
                    vScrollBar.Maximum = newValue + vScrollBar.LargeChange-1;
                    if (newValue <= vScrollBar.Value + (vScrollBar.LargeChange-1) )
                        vScrollBar.Value = newValue;
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
            if (((VScrollBar)sender).Value <= 1)
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
                        line = line.Replace(@eachEmot[i], emotChar + i.ToString("000"));
                    }
                }
            }
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
                    //get the back color
                    int bc = int.Parse(intstr[1]);

                    currentBackColor = bc;

                    sLine.Insert(m.Index, newColorChar.ToString() + fc.ToString("00") + bc.ToString("00"));
                    oldLen--;
                }
                else if (Regex.Match(m.Value, ParseForeColor).Success)
                {
                    int fc = int.Parse(m.Value.Remove(0, 1));

                    if (currentBackColor > -1)
                        sLine.Insert(m.Index, newColorChar.ToString() + fc.ToString("00") + currentBackColor.ToString("00"));
                    else
                        sLine.Insert(m.Index, newColorChar.ToString() + fc.ToString("00") + "99");

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

            string lastColor="";
            string nextColor="";

            bool lineSplit;
            int ii = line;
            Graphics g = this.CreateGraphics();

            StringFormat sf = StringFormat.GenericTypographic;
            sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            for (int currentLine = endLine; currentLine <= startLine ; currentLine++)
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
                        //System.Diagnostics.Debug.WriteLine("fits 1 line");
                        _displayLines[line].line = _textLines[currentLine].line;
                        _displayLines[line].textLine = currentLine;
                        _displayLines[line].textColor = _textLines[currentLine].textColor;
                        line++;
                    } 
                    catch (Exception e)
                    {
                        FormMain.Instance.WriteErrorFile(FormMain.Instance.InputPanel.CurrentConnection, "FormatLines Error1:", e);
                    }
                }
                else
                {
                    lineSplit = false;
                    string curLine = _textLines[currentLine].line;
                    
                    StringBuilder buildString = new StringBuilder();
                    
                    char[] ch;
                    try
                    {
                        for (int i = 0; i < curLine.Length; i++)
                        {
                            ch = curLine.Substring(i, 1).ToCharArray();
                            switch (ch[0])
                            {
                                //case boldChar:
                                //    break;
                                case newColorChar:
                                    buildString.Append(curLine.Substring(i, 5));
                                    if (lastColor.Length == 0)
                                        lastColor = curLine.Substring(i, 5);
                                    else
                                        nextColor = curLine.Substring(i, 5);

                                    i = i + 4;
                                    break;
                                case emotChar:
                                    i = i + 3;
                                    break;
                                default:
                                    //check if there needs to be a linewrap
                                    if ((int)g.MeasureString(StripAllCodes(buildString.ToString()),this.Font, 0, sf).Width > displayWidth)
                                    {
                                        if (lineSplit)
                                            _displayLines[line].line = lastColor + buildString;
                                        else
                                            _displayLines[line].line = buildString.ToString();

                                        _displayLines[line].textLine = currentLine;
                                        _displayLines[line].wrapped = true;
                                        _displayLines[line].textColor = _textLines[currentLine].textColor;

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
                                        buildString.Append(ch[0]);
                                    }
                                    else
                                        buildString.Append(ch[0]);
                                    break;
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Line:" + curLine.Length + ":" + curLine);
                        FormMain.Instance.WriteErrorFile(FormMain.Instance.InputPanel.CurrentConnection,"FormatLines Error2:",e);
                    }
                    
                    //get the remainder
                    if (lineSplit)                    
                        _displayLines[line].line = lastColor + buildString.ToString();
                    else
                        _displayLines[line].line = buildString.ToString();

                    buildString = null;

                    _displayLines[line].textLine = currentLine;
                    _displayLines[line].textColor = _textLines[currentLine].textColor;
                    line++;
                }
            }

            sf.Dispose();
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
                //Bitmap buffer = new Bitmap(this.Width, this.Height, e.Graphics);
                Bitmap buffer = new Bitmap(this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                Graphics g = Graphics.FromImage(buffer);
                //g.Clear(IrcColor.colors[_backColor]);
                
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
                    curLine = 0;
                }
                else
                    startY = this.Height - (_lineSize * LinesToDraw) - (_lineSize / 2);

                StringFormat sf = StringFormat.GenericTypographic;
                sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                int lineCounter = 0;
                
                bool underline = false;
                bool isInUrl = false;
                Font font = new Font(this.Font.Name, this.Font.Size, FontStyle.Regular);
            
                int redline = -1;
                if (FormMain.Instance.IceChatOptions.ShowUnreadLine)
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
 
                while (lineCounter < LinesToDraw)
                {
                    int i = 0, j = 0;
                    bool highlight = false;
                    bool oldHighlight = false;

                    if (redline == curLine)
                    {
                        Pen p = new Pen(Color.Red);
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
                        underline = false;
                        font = null;
                        font = new Font(this.Font.Name, this.Font.Size, FontStyle.Regular);
                    }
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
                                        Bitmap bm = new Bitmap(FormMain.Instance.EmoticonsFolder + System.IO.Path.DirectorySeparatorChar + ((EmoticonItem)FormMain.Instance.IceChatEmoticons.listEmoticons[emotNumber]).EmoticonImage);

                                        if (curBackColor != _backColor)
                                        {
                                            textSize = (int)g.MeasureString(buildString.ToString(), this.Font, 0, sf).Width + 1;
                                            Rectangle r = new Rectangle((int)startX, startY, textSize + 1, _lineSize + 1);
                                            g.FillRectangle(new SolidBrush(IrcColor.colors[curBackColor]), r);
                                        }

                                        g.DrawImage((Image)bm, startX + (int)g.MeasureString(buildString.ToString(), this.Font, 0, sf).Width, startY, _lineSize, _lineSize);

                                        g.DrawString(buildString.ToString(), this.Font, new SolidBrush(IrcColor.colors[curForeColor]), startX, startY, sf);

                                        startX += bm.Width + (int)g.MeasureString(buildString.ToString(), this.Font, 0, sf).Width;

                                        buildString = null;
                                        buildString = new StringBuilder();
                                    }
                                    else
                                    {
                                        buildString.Append(((EmoticonItem)FormMain.Instance.IceChatEmoticons.listEmoticons[emotNumber]).Trigger);
                                    }
                                    break;
                                case urlStart:
                                    if (curBackColor != _backColor)
                                    {
                                        textSize = (int)g.MeasureString(buildString.ToString(), this.Font, 0, sf).Width + 1;
                                        Rectangle r = new Rectangle((int)startX, startY, textSize + 1, _lineSize + 1);
                                        g.FillRectangle(new SolidBrush(IrcColor.colors[curBackColor]), r);
                                    }
                                    g.DrawString(buildString.ToString(), font, new SolidBrush(IrcColor.colors[curForeColor]), startX, startY, sf);

                                    startX += g.MeasureString(buildString.ToString(), font, 0, sf).Width;   //textSizes[32]

                                    buildString = null;
                                    buildString = new StringBuilder();

                                    //remove whats drawn from string
                                    line.Remove(0, i);
                                    line.Remove(0, 1);
                                    i = -1;
                                    font = null;
                                    font = new Font(this.Font.Name, this.Font.Size, FontStyle.Underline);
                                    isInUrl = true;
                                    break;

                                case urlEnd:
                                    if (curBackColor != _backColor)
                                    {
                                        textSize = (int)g.MeasureString(buildString.ToString(), font, 0, sf).Width + 1;
                                        Rectangle r = new Rectangle((int)startX, startY, textSize + 1, _lineSize + 1);
                                        g.FillRectangle(new SolidBrush(IrcColor.colors[curBackColor]), r);
                                    }
                                    g.DrawString(buildString.ToString(), font, new SolidBrush(IrcColor.colors[curForeColor]), startX, startY, sf);

                                    startX += g.MeasureString(buildString.ToString(), font, 0, sf).Width;   //textSizes[32]

                                    buildString = null;
                                    buildString = new StringBuilder();

                                    //remove whats drawn from string
                                    line.Remove(0, i);
                                    line.Remove(0, 1);
                                    i = -1;
                                    font = null;
                                    font = new Font(this.Font.Name, this.Font.Size, FontStyle.Regular);
                                    isInUrl = false;
                                    break;
                                case underlineChar:
                                    if (curBackColor != _backColor)
                                    {
                                        textSize = (int)g.MeasureString(buildString.ToString(), font, 0, sf).Width + 1;
                                        Rectangle r = new Rectangle((int)startX, startY, textSize + 1, _lineSize + 1);
                                        g.FillRectangle(new SolidBrush(IrcColor.colors[curBackColor]), r);
                                    }
                                    g.DrawString(buildString.ToString(), font, new SolidBrush(IrcColor.colors[curForeColor]), startX, startY, sf);

                                    startX += g.MeasureString(buildString.ToString(), font, 0, sf).Width;   //textSizes[32]

                                    buildString = null;
                                    buildString = new StringBuilder();

                                    //remove whats drawn from string
                                    line.Remove(0, i);
                                    line.Remove(0, 1);
                                    i = -1;
                                    font = null;
                                    underline = !underline;
                                    font = new Font(this.Font.Name, this.Font.Size, (underline == false) ? FontStyle.Regular : FontStyle.Underline);
                                    break;
                                case newColorChar:
                                    //draw whats previously in the string
                                    if (curBackColor != _backColor)
                                    {
                                        textSize = (int)g.MeasureString(buildString.ToString(), font, 0, sf).Width + 1;
                                        Rectangle r = new Rectangle((int)startX, startY, textSize + 1, _lineSize + 1);
                                        g.FillRectangle(new SolidBrush(IrcColor.colors[curBackColor]), r);
                                    }
                                    g.DrawString(buildString.ToString(), font, new SolidBrush(IrcColor.colors[curForeColor]), startX, startY, sf);

                                    startX += g.MeasureString(buildString.ToString(), font, 0, sf).Width;   //textSizes[32]

                                    buildString = null;
                                    buildString = new StringBuilder();

                                    //remove whats drawn from string
                                    line.Remove(0, i);

                                    //get the new fore and back colors
                                    if (!highlight)
                                    {
                                        curForeColor = Convert.ToInt32(line.ToString().Substring(1, 2));
                                        curBackColor = Convert.ToInt32(line.ToString().Substring(3, 2));

                                        //check to make sure that FC and BC are in range 0-31
                                        if (curForeColor > 31)
                                            curForeColor = _displayLines[curLine].textColor;
                                        if (curBackColor > 31)
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
                                            textSize = (int)g.MeasureString(buildString.ToString(), font, 0, sf).Width + 1;
                                            Rectangle r = new Rectangle((int)startX, startY, textSize + 1, _lineSize + 1);
                                            g.FillRectangle(new SolidBrush(IrcColor.colors[curBackColor]), r);
                                        }
                                        g.DrawString(buildString.ToString(), font, new SolidBrush(IrcColor.colors[curForeColor]), startX, startY, sf);

                                        startX += g.MeasureString(buildString.ToString(), font, 0, sf).Width;   //textSizes[32]

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
                            textSize = (int)g.MeasureString(buildString.ToString(), font, 0, sf).Width + 1;
                            Rectangle r = new Rectangle((int)startX, startY, textSize + 1, _lineSize + 1);
                            g.FillRectangle(new SolidBrush(IrcColor.colors[curBackColor]), r);
                        }

                        g.DrawString(buildString.ToString(), font, new SolidBrush(IrcColor.colors[curForeColor]), startX, startY, sf);

                    }

                    startY += _lineSize;
                    startX = 0;
                    curLine++;
                    buildString = null;
                    buildString = new StringBuilder();

                }
                buildString = null;

                e.Graphics.DrawImageUnscaled(buffer, 0, 0);
                buffer.Dispose();
                sf.Dispose();
                g.Dispose();
            }
            catch(Exception ee)
            {
                FormMain.Instance.WriteErrorFile(FormMain.Instance.InputPanel.CurrentConnection,"TextWindow OnDisplayText", ee);
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


    }

    #region ColorButton Class
    public class ColorButtonArray
    {
        //initialize 32 boxes for the 32 default colors

        private readonly System.Windows.Forms.Panel hostPanel;

        public delegate void ColorSelected(int ColorNumber);
        public event ColorSelected OnClick;

        private int selectedColor;

        private void OnPaint(object sender, PaintEventArgs e)
        {
            //draw the 32 colors, in 2 rows of 16
            for (int i = 0; i <= 15; i++)
            {

                e.Graphics.FillRectangle(new SolidBrush(IrcColor.colors[i]), (i * 17), 0, 15, 15);
                e.Graphics.DrawRectangle(new Pen(Color.Gray), (i * 17), 0, 15, 15);

                e.Graphics.FillRectangle(new SolidBrush(IrcColor.colors[i + 16]), (i * 17), 20, 15, 15);
                e.Graphics.DrawRectangle(new Pen(Color.Gray), (i * 17), 20, 15, 15);

                if (i == selectedColor)
                {
                    //draw a selection rectangle
                    e.Graphics.DrawRectangle(new Pen(Color.Black, 3), (i * 17), 0, 15, 15);
                }
                if (i + 16 == selectedColor)
                {
                    //draw a selection rectangle
                    e.Graphics.DrawRectangle(new Pen(Color.Black, 3), (i * 17), 20, 15, 15);
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
            else if ((e.Y > 19) || e.Y < 38)
            {
                xPos = e.X / 17;
                if (OnClick != null)
                {
                    selectedColor = xPos + 16;
                    hostPanel.Invalidate();
                    OnClick(xPos + 16);
                }
            }
        }
    }
    #endregion

    #region IRC Colors Class (32 colors)

    public static class IrcColor
    {
        public static Color[] colors;

        static IrcColor()
        {
            //Color color;
            colors = new Color[32];

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


            /*
    
        Case 16: AnsiColor = RGB(&HCC, &HFF, &HCC)
        Case 17: AnsiColor = RGB(0, &H66, &HFF)
        Case 18: AnsiColor = RGB(&HFA, &HEB, &HD7)
        Case 19: AnsiColor = RGB(&HFF, &HD7, 0)
        Case 20: AnsiColor = RGB(&HE6, &HE6, &HE6)
        Case 21: AnsiColor = RGB(&H46, &H82, &HB4)
        Case 22: AnsiColor = RGB(&H99, &H33, &H33)
        Case 23: AnsiColor = RGB(&HFF, &H99, &HFF)
        
        Case 24: AnsiColor = RGB(&HDD, &HA0, &HDD)
        Case 25: AnsiColor = RGB(&H8B, &H45, &H13)
        Case 26: AnsiColor = RGB(&HCC, 0, 0)
        Case 27: AnsiColor = RGB(&HFF, &HFF, &H99)
        Case 28: AnsiColor = RGB(&H33, &H99, 0)
        Case 29: AnsiColor = RGB(&HFF, &H99, 0)
        Case 30: AnsiColor = RGB(&HFF, &HDA, &HB9)
        Case 31: AnsiColor = RGB(&H2F, &H4F, &H4F)
        
        'extended support for 72 colors now (another 40 colors)
        
        Case 32: AnsiColor = &HD8E9EC
        Case 33: AnsiColor = &HE2FF00
        Case 34: AnsiColor = &H5FDAEE
        Case 35: AnsiColor = &H9E0000
        
        'yellow / orange
        Case 36: AnsiColor = &HCCFFFF
        Case 37: AnsiColor = &H99FFFF
        Case 38: AnsiColor = &H66FFFF
        Case 39: AnsiColor = &H33CCFF
        Case 40: AnsiColor = &H3399FF
        Case 41: AnsiColor = &H3366FF
        
        'greens
        Case 42: AnsiColor = 13041606
        Case 43: AnsiColor = 8716164
        Case 44: AnsiColor = 65280
        Case 45: AnsiColor = 50944
        Case 46: AnsiColor = 34304
        Case 47: AnsiColor = 16640
        
        'blues
        Case 48: AnsiColor = 16777158
        Case 49: AnsiColor = 16777092
        Case 50: AnsiColor = 16776960
        Case 51: AnsiColor = &HFF9966
        Case 52: AnsiColor = &HFF6666
        Case 53: AnsiColor = &HFF0033
        
        'reds
        Case 54: AnsiColor = &H99CCFF
        Case 55: AnsiColor = &H6699FF
        Case 56: AnsiColor = &H3366FF
        Case 57: AnsiColor = &H3300FF
        Case 58: AnsiColor = &HCC
        Case 59: AnsiColor = &HAA
        
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
