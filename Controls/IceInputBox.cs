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
		private ArrayList buffer;		
		private int currentHistoryItem = -1;

		//Nick complete variables
        private int nickNumber = -1;
        private string partialNick;
        private ArrayList nickCompleteNames;
		
		//store the maximum number of lines in the back buffer
		private const int MaxBufferSize = 100;

		public delegate void SendCommand(object sender, string data);
		public event SendCommand OnCommand;

        private delegate void ScrollWindowDelegate(bool scrollup);
        private delegate void ScrollConsoleWindowDelegate(bool scrollup);

        private delegate void ScrollWindowPageDelegate(bool scrollup);
        private delegate void ScrollConsoleWindowPageDelegate(bool scrollup);

        
        public IceInputBox()
		{
			InitializeComponent();
			buffer = new ArrayList();
            this.MouseWheel += new MouseEventHandler(IceInputBox_MouseWheel);

            nickCompleteNames = new ArrayList();

		}
        
        private void IceInputBox_MouseWheel(object sender, MouseEventArgs e)
        {
            //120 -- scroll up
            //see which control has focus in the main program
            if (FormMain.Instance.CurrentWindowType != TabWindow.WindowType.Console)
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
                ((ConsoleTabWindow)FormMain.Instance.TabMain.TabPages[0]).CurrentWindow.ScrollWindow(scrollUp);

        }

        private void ScrollWindow(bool scrollUp)
        {
            if (this.InvokeRequired)
            {
                ScrollWindowDelegate s = new ScrollWindowDelegate(ScrollWindow);
                this.Invoke(s, new object[] { scrollUp });
            }
            else
                FormMain.Instance.CurrentWindow.TextWindow.ScrollWindow(scrollUp);
        }

        private void ScrollConsoleWindowPage(bool scrollUp)
        {
            if (this.InvokeRequired)
            {
                ScrollConsoleWindowPageDelegate s = new ScrollConsoleWindowPageDelegate(ScrollConsoleWindowPage);
                this.Invoke(s, new object[] { scrollUp });
            }
            else
                ((ConsoleTabWindow)FormMain.Instance.TabMain.TabPages[0]).CurrentWindow.ScrollWindowPage(scrollUp);

        }

        private void ScrollWindowPage(bool scrollUp)
        {
            if (this.InvokeRequired)
            {
                ScrollWindowPageDelegate s = new ScrollWindowPageDelegate(ScrollWindowPage);
                this.Invoke(s, new object[] { scrollUp });
            }
            else
                FormMain.Instance.CurrentWindow.TextWindow.ScrollWindowPage(scrollUp);
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
			
            buffer = null;
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
			if (keyData == Keys.Tab)
			{
                NickComplete();
                return true;
			}
            else
                nickNumber = -1;

            return false;			
		}

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 770:
                    if (Clipboard.ContainsText())
                    {
                        string[] lines = Clipboard.GetText(TextDataFormat.Text).Split('\n');
                        if (lines.Length == 1)
                        {
                            //just 1 line, add to end of text box
                            this.Text += lines[0];
                            this.SelectionStart = this.Text.Length;
                        }
                        else
                        {
                            foreach (string line in lines)
                            {
                                if (line.Length > 0)
                                {
                                    addToBuffer(line);
                                    OnCommand(this, line);
                                    base.Text = "";
                                }
                            }
                        }
                    }
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private void NickComplete()
        {
            if (FormMain.Instance.CurrentWindowType == TabWindow.WindowType.Console)
            {
                //tab complete in Console, just send current nick
                if (FormMain.Instance.InputPanel.CurrentConnection != null)
                {
                    this.Text += FormMain.Instance.InputPanel.CurrentConnection.ServerSetting.NickName;
                    this.SelectionStart = this.Text.Length;
                }
            }
            else if (FormMain.Instance.CurrentWindowType == TabWindow.WindowType.Channel)
            {
                if (this.Text.Length == 0)
                    return;

                //get the partial nick
                if (this.Text.IndexOf(' ') == -1)
                    partialNick = this.Text;
                else
                    partialNick = this.Text.Substring(this.Text.LastIndexOf(' ') + 1);

                if (partialNick.Length == 0)
                    return;
                
                if (Array.IndexOf(FormMain.Instance.InputPanel.CurrentConnection.ServerSetting.ChannelTypes, partialNick[0]) != -1)
                {
                    //channel name complete
                    this.Text = this.Text.Substring(0, this.Text.Length - partialNick.Length) + FormMain.Instance.CurrentWindow.WindowName;
                    this.SelectionStart = this.Text.Length;
                    this.nickNumber = -1;
                    return;
                }
                
                if (nickNumber == -1)
                {
                    nickCompleteNames.Clear();

                    foreach (User u in FormMain.Instance.CurrentWindow.Nicks.Values)
                    {
                        if (u.NickName.Length > partialNick.Length)
                        {
                            if (u.NickName.Substring(0, partialNick.Length).ToLower() == partialNick.ToLower())
                                nickCompleteNames.Add(u.NickName);
                        }
                    }
                    if (nickCompleteNames.Count == 0)
                        return;

                    nickNumber = 0;
                }
                else
                {
                    if (nickCompleteNames.Count == 0)
                    {
                        nickNumber = -1;
                        return;
                    }
                    
                    nickNumber++;
                    if ( nickNumber > (nickCompleteNames.Count - 1))
                        nickNumber = 0;
                }
                
                this.Text = this.Text.Substring(0,this.Text.Length - partialNick.Length) + nickCompleteNames[nickNumber].ToString();
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
				if (e.KeyCode == Keys.B)
				{
					base.SelectedText = ((char)2).ToString();
					e.Handled=true;
				}
				if (e.KeyCode == Keys.U)
				{
					base.SelectedText = ((char)31).ToString();
					e.Handled=true;
				}

			}

			//code below is for the single line Inputbox
			//UP Key
			if (e.KeyCode == Keys.Up)
			{
				e.Handled=true;

				if (currentHistoryItem <= 0)
				{
					if (buffer.Count == 1)
						currentHistoryItem=1;
					else
						return;
				}
				
				if ((currentHistoryItem != buffer.Count-1) || (base.Text.ToString() == buffer[buffer.Count-1].ToString()))
				{
					currentHistoryItem--;
				}
				else
				{
					currentHistoryItem = buffer.Count-1;
				}
				
				if (currentHistoryItem > -1)
				{
					base.Text = buffer[currentHistoryItem].ToString();					
					base.SelectionStart = base.Text.Length;
				}
				return;
			}
			
			if (e.KeyCode == Keys.Down)
			{
				//DOWN Key
				e.Handled=true;
				
				if (currentHistoryItem >= buffer.Count -1)
				{
					currentHistoryItem = buffer.Count - 1;
					base.Text = "";
					return;
				}
				else if (currentHistoryItem == -1)
				{
					base.Text = "";
					return;
				}
				
				currentHistoryItem++;
				base.Text = buffer[currentHistoryItem].ToString();
				base.SelectionStart = base.Text.Length;
				return;
			}


            if (e.KeyCode == Keys.PageDown)
            {
                //scroll window down one page
                if (FormMain.Instance.CurrentWindowType != TabWindow.WindowType.Console)
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
                if (FormMain.Instance.CurrentWindowType != TabWindow.WindowType.Console)
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


			if (e.KeyCode == Keys.Escape)
			{
				e.Handled=true;				
				base.Text = "";
				return;
			}
			base.OnKeyDown (e);			
		
		}
        
        public void OnEnterKey()
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

				//add the text to the buffer
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
		
		public void addToBuffer(string data)
		{
			//add text to back buffer
			if (data.Length == 0) return;
			//check for maximum back buffer history here
			//remove 1st item if exceeded size
			if (buffer.Count > MaxBufferSize)
				buffer.Remove(buffer[0]);
			
			buffer.Add(data);
			currentHistoryItem = buffer.Count-1;
		}

	}
}
