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
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace IceChat
{
    public partial class InputPanel : UserControl
    {
        public delegate void OnCommandDelegate(object sender, string data);
        public event OnCommandDelegate OnCommand;

        //which is the current connection
        private IRCConnection currentConnection;

        public InputPanel()
        {
            InitializeComponent();
            
            this.buttonEmoticonPicker.Image = StaticMethods.LoadResourceImage("Smile.png");
            this.buttonColorPicker.Image = StaticMethods.LoadResourceImage("color.png");


            textInput.OnCommand += new IceInputBox.SendCommand(textInput_OnCommand);
            textSearch.KeyDown += new KeyEventHandler(textSearch_KeyDown);
        }

        internal void ApplyLanguage()
        {
            if (FormMain.Instance != null)
            {
                IceChatLanguage iceChatLanguage = FormMain.Instance.IceChatLanguage;
                buttonSend.Text = iceChatLanguage.buttonSend;
            }
        }

        internal IRCConnection CurrentConnection
        {
            get
            {
                return currentConnection;
            }
            set
            {
                currentConnection = value;
            }
        }

        internal bool ShowSearchPanel
        {
            get { return this.panelSearch.Visible; }
            set 
            { 
                if (value)
                    this.Height = 56;
                else
                    this.Height = 26;

                this.panelSearch.Visible = value;
            }            
        }

        internal bool ShowEmoticonPicker
        {
            get { return this.buttonEmoticonPicker.Visible; }
            set { this.buttonEmoticonPicker.Visible = value; }
        }

        internal bool ShowColorPicker
        {
            get { return this.buttonColorPicker.Visible; }
            set { this.buttonColorPicker.Visible = value; }
        }

        internal Font InputBoxFont
        {
            get { return textInput.Font; }
            set { textInput.Font = value; }
        }

        internal void SetInputBoxColors()
        {
            this.textInput.BackColor = IrcColor.colors[FormMain.Instance.IceChatColors.InputboxBackColor];
            this.textInput.ForeColor = IrcColor.colors[FormMain.Instance.IceChatColors.InputboxForeColor];
            this.BackColor = IrcColor.colors[FormMain.Instance.IceChatColors.InputboxBackColor];
        }

        internal void AppendText(string data)
        {
            textInput.AppendText(data);
        }

        private void textInput_OnCommand(object sender, string data)
        {
            if (OnCommand != null)
            {
                string[] lines = data.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length == 1)
                {
                    //just 1 line, add to end of text box
                    OnCommand(this, lines[0]);
                }
                else
                {
                    if (lines.Length > 4)
                    {
                        //we are pasting 5 lines or more, lets ask
                        DialogResult ask = MessageBox.Show("You will be pasting " + lines.Length + " lines, do you wish to proceed?", "IceChat", MessageBoxButtons.YesNo);
                        if (ask == DialogResult.Yes)
                        {
                            foreach (string line in lines)
                            {
                                if (line.Length > 0)
                                {
                                    textInput.addToBuffer(line);
                                    OnCommand(this, line);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (string line in lines)
                        {
                            if (line.Length > 0)
                            {
                                textInput.addToBuffer(line);
                                OnCommand(this, line);
                            }
                        }
                    }
                }
            }
        }

        internal void FocusTextBox()
        {
            if (!textInput.Focused)
                textInput.Focus();
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            textInput.OnEnterKey();
            FocusTextBox();
        }

        private void buttonEmoticonPicker_Click(object sender, EventArgs e)
        {
            //show the emoticon picker form
            FormEmoticons fe = new FormEmoticons();
            fe.Top = (FormMain.Instance.Top + FormMain.Instance.Height) - 220;
            fe.Left = FormMain.Instance.Left + 10;
            fe.ShowDialog(this);
            FormMain.Instance.FocusInputBox();
        }

        private void buttonColorPicker_Click(object sender, EventArgs e)
        {
            FormColorPicker fc = new FormColorPicker();
            fc.Top = (FormMain.Instance.Top + FormMain.Instance.Height) - 220;
            fc.Left = FormMain.Instance.Left + 10;
            fc.ShowDialog(this);
            FormMain.Instance.FocusInputBox();
        }

        private void textSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //perform the search
                if (FormMain.Instance.CurrentWindowType == IceTabPage.WindowType.Console)
                {
                    //find the current console tab
                    FormMain.Instance.CurrentWindow.CurrentConsoleWindow().SearchText(textSearch.Text, 12);
                }
                else if (FormMain.Instance.CurrentWindowType == IceTabPage.WindowType.Channel || FormMain.Instance.CurrentWindowType == IceTabPage.WindowType.Query)
                {
                    //do a search for the current window
                    IceTabPage searchTab = FormMain.Instance.CurrentWindow;
                    searchTab.TextWindow.SearchText(textSearch.Text, 0);

                }
            }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            //find next search
        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            //find previous search
        }

        private void buttonHelp_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            contextHelpMenu.Show(buttonHelp, e.Location);
        }
        
        private void toolStripHelpMenuOnClick(object sender, System.EventArgs e)
        {
            OnCommand(this, "//addtext " + ((System.Windows.Forms.ToolStripMenuItem)sender).Tag.ToString());
        }
    }
}
