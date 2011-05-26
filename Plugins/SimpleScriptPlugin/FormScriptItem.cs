using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IceChatPlugin
{
    public partial class FormScriptItem : Form
    {

        public delegate void SaveScriptItemDelegate(ScriptItem scr, int listIndex);
        public event SaveScriptItemDelegate SaveScriptItem;
        
        private ScriptItem scriptItem;
        private int listIndex = 0;

        public FormScriptItem(ScriptItem src, int index)
        {
            InitializeComponent();
            comboScriptEvent.Items.Add("Channel Message");
            comboScriptEvent.Items.Add("Channel Action");
            comboScriptEvent.Items.Add("Private Message");
            comboScriptEvent.Items.Add("Private Action");
            comboScriptEvent.Items.Add("Channel Join");
            
            this.scriptItem = src;
            this.listIndex = index;

            textTextMatch.Text = scriptItem.TextMatch;
            textCommand.Text = scriptItem.Command;
            textChannelMatch.Text = scriptItem.ChannelMatch;
            comboScriptEvent.Text = scriptItem.ScriptEvent;
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            scriptItem.ChannelMatch = textChannelMatch.Text;
            scriptItem.Command = textCommand.Text;
            scriptItem.ScriptEvent = comboScriptEvent.Text;
            scriptItem.TextMatch = textTextMatch.Text;
            
            //update or add new item
            if (SaveScriptItem != null)
                SaveScriptItem(this.scriptItem, listIndex);

            this.Close();     
        }
    }
   
}
