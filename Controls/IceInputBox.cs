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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace IceChat
{
	public partial class IceInputBox : System.Windows.Forms.TextBox
	{
		private System.ComponentModel.Container components = null;
		private ArrayList _buffer;		
		private int _currentHistoryItem = -1;

		//Nick complete variables
        private int _nickNumber = -1;
        private string _partialNick;
        private ArrayList _nickCompleteNames;
		
		//store the maximum number of lines in the back _buffer
		private const int _maxBufferSize = 100;

		public delegate void SendCommand(object sender, string data);
		public event SendCommand OnCommand;

        private delegate void ScrollWindowDelegate(bool scrollup);
        private delegate void ScrollConsoleWindowDelegate(bool scrollup);

        private delegate void ScrollWindowPageDelegate(bool scrollup);
        private delegate void ScrollConsoleWindowPageDelegate(bool scrollup);

        
        public IceInputBox()
		{
			InitializeComponent();
			_buffer = new ArrayList();
            this.MouseWheel += new MouseEventHandler(OnMouseWheel);

            _nickCompleteNames = new ArrayList();

		}
        
        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            //120 -- scroll up
            //see which control has focus in the main program
            if (FormMain.Instance.CurrentWindowType != IceTabPage.WindowType.Console)
            {
                if (FormMain.Instance.CurrentWindow != null)
                    ScrollWindow(e.Delta > 0);
            }
            else
            {
                //make a scroll window for the console
                //find the current window for the console
                ScrollConsoleWindow(e.Delta > 0);                
            }
        }

        private void ScrollConsoleWindow(bool scrollUp)
        {
            if (this.InvokeRequired)
            {
                ScrollConsoleWindowDelegate s = new ScrollConsoleWindowDelegate(ScrollConsoleWindow);
                this.Invoke(s, new object[] { scrollUp });
            }
            else
                FormMain.Instance.TabMain.GetTabPage("Console").CurrentConsoleWindow().ScrollWindow(scrollUp);

        }

        private void ScrollWindow(bool scrollUp)
        {
            if (this.InvokeRequired)
            {
                ScrollWindowDelegate s = new ScrollWindowDelegate(ScrollWindow);
                this.Invoke(s, new object[] { scrollUp });
            }
            else
                if (FormMain.Instance.CurrentWindowType != IceTabPage.WindowType.ChannelList)
                {
                    if (FormMain.Instance.CurrentWindowType == IceTabPage.WindowType.Channel)
                    {
                        //check if mousewheel is hovering over nicklist
                        if (FormMain.Instance.NickList.MouseHasFocus)
                        {
                            FormMain.Instance.NickList.ScrollWindow(scrollUp);
                            return;
                        }
                    }
                    FormMain.Instance.CurrentWindow.TextWindow.ScrollWindow(scrollUp);
                }
        }

        private void ScrollConsoleWindowPage(bool scrollUp)
        {
            if (this.InvokeRequired)
            {
                ScrollConsoleWindowPageDelegate s = new ScrollConsoleWindowPageDelegate(ScrollConsoleWindowPage);
                this.Invoke(s, new object[] { scrollUp });
            }
            else
                FormMain.Instance.TabMain.GetTabPage("Console").CurrentConsoleWindow().ScrollWindowPage(scrollUp);

        }

        private void ScrollWindowPage(bool scrollUp)
        {
            if (this.InvokeRequired)
            {
                ScrollWindowPageDelegate s = new ScrollWindowPageDelegate(ScrollWindowPage);
                this.Invoke(s, new object[] { scrollUp });
            }
            else
            {
                if (FormMain.Instance.CurrentWindowType != IceTabPage.WindowType.ChannelList)
                    FormMain.Instance.CurrentWindow.TextWindow.ScrollWindowPage(scrollUp);
            }
        }


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
					components.Dispose();
			}
			base.Dispose( disposing );
			
            _buffer = null;
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// IRCInputBox
			// 
			this.Size = new System.Drawing.Size(272, 20);

		}
				
		#endregion
		
		protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
		{
            if ((keyData == (Keys.Control | Keys.V)) || keyData == (Keys.Shift | Keys.Insert))
            {
                string data = Clipboard.GetText(TextDataFormat.Text);
                if (data.Contains(Environment.NewLine))
                {
                    OnCommand(this, data);
                    return true;
                }
            }

            if (keyData == Keys.Tab)
            {
                NickComplete();
                return true;
            }
            else
                _nickNumber = -1;

            return false;
        }
        
        private void NickComplete()
        {
            if (FormMain.Instance.CurrentWindowType == IceTabPage.WindowType.Console)
            {
                //tab complete in Console, just send current nick
                if (FormMain.Instance.InputPanel.CurrentConnection != null)
                {
                    this.Text += FormMain.Instance.InputPanel.CurrentConnection.ServerSetting.NickName;
                    this.SelectionStart = this.Text.Length;
                }
            }
            else if (FormMain.Instance.CurrentWindowType == IceTabPage.WindowType.Channel)
            {
                if (this.Text.Length == 0)
                    return;

                //get the partial nick
                if (this.Text.IndexOf(' ') == -1)
                    _partialNick = this.Text;
                else
                    _partialNick = this.Text.Substring(this.Text.LastIndexOf(' ') + 1);

                if (_partialNick.Length == 0)
                    return;
                
                if (Array.IndexOf(FormMain.Instance.InputPanel.CurrentConnection.ServerSetting.ChannelTypes, _partialNick[0]) != -1)
                {
                    //channel name complete
                    this.Text = this.Text.Substring(0, this.Text.Length - _partialNick.Length) + FormMain.Instance.CurrentWindow.TabCaption;
                    this.SelectionStart = this.Text.Length;
                    this._nickNumber = -1;
                    return;
                }
                
                if (_nickNumber == -1)
                {
                    _nickCompleteNames.Clear();

                    foreach (User u in FormMain.Instance.CurrentWindow.Nicks.Values)
                    {
                        if (u.NickName.Length > _partialNick.Length)
                        {
                            if (u.NickName.Substring(0, _partialNick.Length).ToLower() == _partialNick.ToLower())
                                _nickCompleteNames.Add(u.NickName);
                        }
                    }
                    if (_nickCompleteNames.Count == 0)
                        return;

                    _nickNumber = 0;
                }
                else
                {
                    if (_nickCompleteNames.Count == 0)
                    {
                        _nickNumber = -1;
                        return;
                    }
                    
                    _nickNumber++;
                    if ( _nickNumber > (_nickCompleteNames.Count - 1))
                        _nickNumber = 0;
                }
                
                this.Text = this.Text.Substring(0,this.Text.Length - _partialNick.Length) + _nickCompleteNames[_nickNumber].ToString();
                this.SelectionStart = this.Text.Length;

            }
            else if (FormMain.Instance.CurrentWindowType == IceTabPage.WindowType.Query || FormMain.Instance.CurrentWindowType == IceTabPage.WindowType.DCCChat)
            {
                this.Text += FormMain.Instance.CurrentWindow.TabCaption;
                this.SelectionStart = this.Text.Length;
            }
        }

		protected override void OnKeyDown(KeyEventArgs e)
		{

			if (e.Modifiers == Keys.Control)
			{
				if (e.KeyCode == Keys.K)
				{
					base.SelectedText = ((char)3).ToString();
					e.Handled=true;
				}
				else if (e.KeyCode == Keys.B)
				{
					base.SelectedText = ((char)2).ToString();
					e.Handled=true;
				}
				else if (e.KeyCode == Keys.U)
				{
					base.SelectedText = ((char)31).ToString();
					e.Handled=true;
				}
                else if (e.KeyCode == Keys.D)
                {
                    FormMain.Instance.debugWindowToolStripMenuItem.PerformClick();
                    e.Handled = true;                
                }
                else if (e.KeyCode == Keys.S)
                {
                    FormMain.Instance.iceChatEditorToolStripMenuItem.PerformClick();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.P)
                {
                    FormMain.Instance.iceChatSettingsToolStripMenuItem.PerformClick();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.W)
                {
                    FormMain.Instance.closeCurrentWindowToolStripMenuItem.PerformClick();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.G)
                {
                    FormMain.Instance.iceChatColorsToolStripMenuItem.PerformClick();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Tab)
                {
                    int nextIndex = FormMain.Instance.TabMain.TabCount == FormMain.Instance.TabMain.SelectedIndex + 1 ? 0 : FormMain.Instance.TabMain.SelectedIndex + 1;
                    FormMain.Instance.TabMain.SelectTab(FormMain.Instance.TabMain.TabPages[nextIndex]);
                    FormMain.Instance.ServerTree.Invalidate();
                    return;
                }
                else if (e.KeyCode == Keys.PageUp)
                {
                    int nextIndex = FormMain.Instance.TabMain.TabCount == FormMain.Instance.TabMain.SelectedIndex + 1 ? 0 : FormMain.Instance.TabMain.SelectedIndex + 1;
                    FormMain.Instance.TabMain.SelectTab(FormMain.Instance.TabMain.TabPages[nextIndex]);
                    FormMain.Instance.ServerTree.Invalidate();
                    return;
                }
                else if (e.KeyCode == Keys.PageDown)
                {
                    int prevIndex = FormMain.Instance.TabMain.SelectedIndex == 0 ? FormMain.Instance.TabMain.TabCount - 1 : FormMain.Instance.TabMain.SelectedIndex - 1;
                    FormMain.Instance.TabMain.SelectTab(FormMain.Instance.TabMain.TabPages[prevIndex]);
                    FormMain.Instance.ServerTree.Invalidate();
                    return;
                }

			}

			//code below is for the single line Inputbox
			//UP Key
			if (e.KeyCode == Keys.Up)
			{
				e.Handled=true;

				if (_currentHistoryItem <= 0)
				{
					if (_buffer.Count == 1)
						_currentHistoryItem=1;
					else
						return;
				}
				
				if ((_currentHistoryItem != _buffer.Count-1) || (base.Text.ToString() == _buffer[_buffer.Count-1].ToString()))
				{
					_currentHistoryItem--;
				}
				else
				{
					_currentHistoryItem = _buffer.Count-1;
				}
				
				if (_currentHistoryItem > -1)
				{
					base.Text = _buffer[_currentHistoryItem].ToString();					
					base.SelectionStart = base.Text.Length;
				}
				return;
			}
			
			if (e.KeyCode == Keys.Down)
			{
				//DOWN Key
				e.Handled=true;
				
				if (_currentHistoryItem >= _buffer.Count -1)
				{
					_currentHistoryItem = _buffer.Count - 1;
					base.Text = "";
					return;
				}
				else if (_currentHistoryItem == -1)
				{
					base.Text = "";
					return;
				}
				
				_currentHistoryItem++;
				base.Text = _buffer[_currentHistoryItem].ToString();
				base.SelectionStart = base.Text.Length;
				return;
			}


            if (e.KeyCode == Keys.PageDown)
            {
                //scroll window down one page
                if (FormMain.Instance.CurrentWindowType != IceTabPage.WindowType.Console)
                {
                    if (FormMain.Instance.CurrentWindow != null)
                        ScrollWindowPage(false);
                }
                else
                {
                    //make a scroll window for the console
                    //find the current window for the console
                    ScrollConsoleWindowPage(false);
                }
            }

            if (e.KeyCode == Keys.PageUp)
            {
                //scroll window down one page
                if (FormMain.Instance.CurrentWindowType != IceTabPage.WindowType.Console)
                {
                    if (FormMain.Instance.CurrentWindow != null)
                        ScrollWindowPage(true);
                }
                else
                {
                    //make a scroll window for the console
                    //find the current window for the console
                    ScrollConsoleWindowPage(true);
                }
            }

            if (e.KeyCode == Keys.F3)
            {
                e.Handled = true;
                //show or hide the search panel
                ((InputPanel)this.Parent).ShowSearchPanel = !((InputPanel)this.Parent).ShowSearchPanel;
                return;
            }

            if (e.KeyCode == Keys.Escape)
			{
				e.Handled=true;				
				base.Text = "";
				return;
			}
			base.OnKeyDown (e);			
		
		}
        
        internal void OnEnterKey()
        {
            OnKeyPress(new KeyPressEventArgs((char)13));
        }

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			string command = base.Text;
			
            bool ctrlKeyUsed = false;
			
			if (e.KeyChar == (char)10)
			{
				if (command.Length > 0)
				{
					ctrlKeyUsed=true;
				}
				else
					return;
			}

			if (e.KeyChar == (char)13 || ctrlKeyUsed)
			{
				if (command.Length == 0) 
				{
					return;
				}

				//add the text to the _buffer
				addToBuffer(command);

				//fire event for server command
				if (OnCommand != null)
				{
					if (ctrlKeyUsed)
						command = "/say " + command;

					OnCommand(this,command);
				}
				
				//clear the text box
				base.Text = "";
				e.Handled = true;

			}	
			else
			{
				base.OnKeyPress (e);
			}
		}
		
		internal void addToBuffer(string data)
		{
			//add text to back _buffer
			if (data.Length == 0) return;
			//check for maximum back _buffer history here
			//remove 1st item if exceeded size
			if (_buffer.Count > _maxBufferSize)
				_buffer.Remove(_buffer[0]);
			
			_buffer.Add(data);
			_currentHistoryItem = _buffer.Count-1;
		}

	}
}
