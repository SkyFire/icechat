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
            textInput.OnCommand += new IceInputBox.SendCommand(textInput_OnCommand);

            if (!FormMain.Instance.IceChatOptions.ShowEmoticonPicker)
                buttonEmoticonPicker.Visible = false;

            if (!FormMain.Instance.IceChatOptions.ShowColorPicker)
                buttonColorPicker.Visible = false;
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

        internal void AppendText(string data)
        {
            textInput.AppendText(data);
        }

        internal void SendCommand(string data)
        {
            if (OnCommand != null)
                OnCommand(this, data);
        }

        private void textInput_OnCommand(object sender, string data)
        {
            if (OnCommand != null)
                OnCommand(this, data);
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
            fc.Top = (FormMain.Instance.Top + FormMain.Instance.Height) - 105;
            fc.Left = FormMain.Instance.Left + 10;
            fc.ShowDialog(this);
            FormMain.Instance.FocusInputBox();
        }
    }
}
