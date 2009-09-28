using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace IceChat2009
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
    }
}
