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
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;


namespace IceChatPlugin
{
    public class Plugin : IPluginIceChat
    {
        private string m_Name;
        private string m_Author;
        private string m_Version;

        private AppDomain m_domain;

        private Form m_MainForm;
        private MenuStrip m_MenuStrip;
        private Panel m_BottomPanel;
        private TabControl m_RightPanel;
        private TabControl m_LeftPanel;

        private string m_ServerTreeCurrentTab;
        private IceChat.IRCConnection m_ServerTreeCurrentConnection;

        //all the events get declared here, do not change
        public event OutGoingCommandHandler OnCommand;

        private TabPage tabPageHighlight;
        private Button buttonAdd;
        private Button buttonRemove;
        private Button buttonEdit;
        private ListView listScripts;

        private ColumnHeader columnTextMatch;
        private ColumnHeader columnCommand;
        private ColumnHeader columnChannelMatch;
        private ColumnHeader columnEventType;

        private IceChatScripts iceChatScripts;
        private string scriptsFile;
        private string currentFolder;

        public Plugin()
        {
            //set your default values here
            m_Name = "Simple Script Plugin";
            m_Author = "Snerf";
            m_Version = "1.1";
        }

        public string Name
        {
            get { return m_Name; }
        }

        public string Author
        {
            get { return m_Author; }
        }

        public string Version
        {
            get { return m_Version; }
        }

        public Form MainForm
        {
            get { return m_MainForm; }
            set { m_MainForm = value; }
        }

        public string CurrentFolder
        {
            get { return currentFolder; }
            set { currentFolder = value; }
        }

        public MenuStrip MainMenuStrip
        {
            get { return m_MenuStrip; }
            set { m_MenuStrip = value; }
        }

        public Panel BottomPanel
        {
            get { return m_BottomPanel; }
            set { m_BottomPanel = value; }
        }

        public TabControl LeftPanel
        {
            get { return m_LeftPanel; }
            set { m_LeftPanel = value; }
        }

        public TabControl RightPanel
        {
            get { return m_RightPanel; }
            set { m_RightPanel = value; }
        }

        public string ServerTreeCurrentTab
        {
            get { return m_ServerTreeCurrentTab; }
            set { m_ServerTreeCurrentTab = value; }
        }

        public IceChat.IRCConnection ServerTreeCurrentConnection
        {
            get { return m_ServerTreeCurrentConnection; }
            set { m_ServerTreeCurrentConnection = value; }
        }

        public AppDomain domain
        {
            get { return m_domain; }
            set { m_domain = value; }
        }

        public void ShowInfo()
        {
            MessageBox.Show(m_Name + " Loaded", m_Name + " " + m_Author);
        }

        public void Dispose()
        {

        }

        public void Initialize()
        {
            scriptsFile = currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatSimpleScript.xml";
            LoadScriptSettings();

        }

        public void MainProgramLoaded()
        {

        }

        public void LoadSettingsForm(TabControl SettingsTab)
        {


        }

        public ToolStripItem[] AddChannelPopups()
        {
            return null;
        }

        public ToolStripItem[] AddQueryPopups()
        {
            return null;
        }

        public ToolStripItem[] AddServerPopups()
        {
            return null;
        }

        public void LoadEditorForm(TabControl ScriptsTab)
        {
            //when the Editor Form gets loaded, ability to add tabs
            
            tabPageHighlight = new System.Windows.Forms.TabPage();
            buttonAdd = new Button();
            buttonRemove = new Button();
            buttonEdit = new Button();
            listScripts = new ListView();
            columnTextMatch = new ColumnHeader();
            columnCommand = new ColumnHeader();
            columnChannelMatch = new ColumnHeader();
            columnEventType = new ColumnHeader();

            columnTextMatch.Width = 200;
            columnChannelMatch.Width = 200;
            columnEventType.Text = "Script Event";
            columnEventType.Width = 200;
            columnCommand.Width = 0;

            tabPageHighlight.SuspendLayout();

            buttonRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonRemove.Location = new System.Drawing.Point(361, 96);
            buttonRemove.Name = "buttonRemove";
            buttonRemove.Size = new System.Drawing.Size(75, 27);
            buttonRemove.TabIndex = 4;
            buttonRemove.Text = "Remove";
            buttonRemove.UseVisualStyleBackColor = true;
            buttonRemove.Click += new EventHandler(buttonRemove_Click);
            // 
            // buttonEdit
            // 
            buttonEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonEdit.Location = new System.Drawing.Point(361, 63);
            buttonEdit.Name = "buttonEdit";
            buttonEdit.Size = new System.Drawing.Size(75, 27);
            buttonEdit.TabIndex = 3;
            buttonEdit.Text = "Edit";
            buttonEdit.UseVisualStyleBackColor = true;
            buttonEdit.Click += new EventHandler(buttonEdit_Click);
            // 
            // buttonAdd
            // 
            buttonAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonAdd.Location = new System.Drawing.Point(361, 30);
            buttonAdd.Name = "buttonAdd";
            buttonAdd.Size = new System.Drawing.Size(75, 27);
            buttonAdd.TabIndex = 2;
            buttonAdd.Text = "Add";
            buttonAdd.UseVisualStyleBackColor = true;
            buttonAdd.Click += new EventHandler(buttonAdd_Click);
            // listScripts
            // 
            listScripts.CheckBoxes = true;
            listScripts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnEventType,
            columnCommand,
            columnTextMatch, 
            columnChannelMatch });


            listScripts.FullRowSelect = true;
            listScripts.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            listScripts.HideSelection = false;
            listScripts.LabelWrap = false;
            listScripts.Location = new System.Drawing.Point(25, 30);
            listScripts.MultiSelect = false;
            listScripts.Name = "listScripts";
            listScripts.ShowGroups = false;
            listScripts.Size = new System.Drawing.Size(350, 288);
            listScripts.Dock = DockStyle.Left;
            listScripts.TabIndex = 1;
            listScripts.UseCompatibleStateImageBehavior = false;
            listScripts.View = System.Windows.Forms.View.Details;

            tabPageHighlight.BackColor = System.Drawing.SystemColors.Control;
            tabPageHighlight.Controls.Add(buttonRemove);
            tabPageHighlight.Controls.Add(buttonEdit);
            tabPageHighlight.Controls.Add(buttonAdd);
            tabPageHighlight.Controls.Add(listScripts);
            tabPageHighlight.Location = new System.Drawing.Point(4, 25);
            tabPageHighlight.Name = "tabPageHighlight2";
            tabPageHighlight.Padding = new System.Windows.Forms.Padding(3);
            //tabPageHighlight.Size = new System.Drawing.Size(710, 339);


            tabPageHighlight.Text = "Simple Script";

            tabPageHighlight.ResumeLayout();

            ScriptsTab.Controls.Add(tabPageHighlight);

            ShowScriptItems();


        }

        public void LoadColorsForm(TabControl OptionsTab)
        {
            //when the Options Form gets loaded, ability to add tabs
           
        }

        public void SaveColorsForm()
        {
            //
        }

        public void SaveSettingsForm()
        {
            //
        }

        public void SaveEditorForm()
        {
            iceChatScripts.listScripts.Clear();

            foreach (ListViewItem item in listScripts.Items)
            {
                ScriptItem scr = new ScriptItem();
                scr.ScriptEvent = item.Text;
                scr.Command = item.SubItems[1].Text;
                scr.TextMatch = item.SubItems[2].Text;
                scr.ChannelMatch = item.SubItems[3].Text;
                scr.Enabled = item.Checked;
                iceChatScripts.AddScriptItem(scr);
            }

            SaveScriptSettings();
        }

        private void ShowScriptItems()
        {
            foreach (ScriptItem scr in iceChatScripts.listScripts)
            {
                ListViewItem lvi = this.listScripts.Items.Add(scr.ScriptEvent);
                lvi.SubItems.Add(scr.Command);
                lvi.SubItems.Add(scr.TextMatch);
                lvi.SubItems.Add(scr.ChannelMatch);
                lvi.Checked = scr.Enabled;
            }
        }

        private void LoadScriptSettings()
        {
            if (File.Exists(scriptsFile))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(IceChatScripts));
                TextReader textReader = new StreamReader(scriptsFile);
                iceChatScripts = (IceChatScripts)deserializer.Deserialize(textReader);
                textReader.Close();
                textReader.Dispose();
            }
            else
                iceChatScripts = new IceChatScripts();
        }

        private void SaveScriptSettings()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(IceChatScripts));
            TextWriter textWriter = new StreamWriter(scriptsFile);
            serializer.Serialize(textWriter, iceChatScripts);
            textWriter.Close();
            textWriter.Dispose();
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listScripts.SelectedItems)
            {
                ScriptItem scr = new ScriptItem();

                scr.ScriptEvent = item.Text;
                scr.Command = item.SubItems[1].Text;
                scr.TextMatch = item.SubItems[2].Text;
                scr.ChannelMatch = item.SubItems[3].Text;
                scr.Enabled = item.Checked;

                FormScriptItem fi = new FormScriptItem(scr, item.Index);
                fi.SaveScriptItem += new FormScriptItem.SaveScriptItemDelegate(UpdateScriptItem);                
                fi.ShowDialog(m_MainForm);
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            FormScriptItem fsi = new FormScriptItem(new ScriptItem(), 0);
            fsi.SaveScriptItem += new FormScriptItem.SaveScriptItemDelegate(SaveNewScriptItem);
            fsi.ShowDialog(m_MainForm);
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listScripts.SelectedItems)
            {
                listScripts.Items.Remove(item);
            }

        }

        private void SaveNewScriptItem(ScriptItem scr, int listIndex)
        {
            if (scr.TextMatch.Length > 0)
            {
                ListViewItem lvi = this.listScripts.Items.Add(scr.ScriptEvent);
                lvi.SubItems.Add(scr.Command);
                lvi.SubItems.Add(scr.TextMatch);
                lvi.SubItems.Add(scr.ChannelMatch);
                lvi.Checked = true;
            }
        }

        private void UpdateScriptItem(ScriptItem scr, int listIndex)
        {
            foreach (ListViewItem item in listScripts.SelectedItems)
            {
                if (item.Index == listIndex)
                {
                    item.Text= scr.ScriptEvent;
                    item.SubItems[1].Text = scr.Command;
                    item.SubItems[2].Text = scr.TextMatch;
                    item.SubItems[3].Text = scr.ChannelMatch;
                    item.Checked = scr.Enabled;
                    break;
                }
            }
        }

        private PluginArgs CheckScripts(PluginArgs args, string eventType)
        {
            foreach (ScriptItem scr in iceChatScripts.listScripts)
            {
                string command = "";
                if (scr.Enabled && eventType == scr.ScriptEvent)
                {
                    switch (scr.ScriptEvent)
                    {
                        case "Channel Message":
                        case "Channel Action":
                            //use a regex match of sorts down the road
                            Regex regChannel = new Regex(scr.ChannelMatch.Replace(@".", @"\.").Replace(@"*", @".*"), RegexOptions.IgnoreCase);
                            if (args.Channel != null && regChannel.IsMatch(args.Channel))
                            {
                                Regex regMatch = new Regex(scr.TextMatch.Replace(@".", @"\.").Replace(@"*", @".*"), RegexOptions.IgnoreCase);
                                if (regMatch.IsMatch(args.Extra))                                
                                {
                                    command = scr.Command.Replace("$chan", args.Channel);
                                    command = command.Replace("$match", scr.TextMatch);
                                    command = command.Replace("$message", args.Extra);

                                    args.Command = command;
                                    
                                    if (OnCommand != null)
                                        OnCommand(args);

                                }
                            }                            
                            break;
                        case "Private Message":
                        case "Private Action":
                            Regex regChannel2 = new Regex(scr.ChannelMatch.Replace(@".", @"\.").Replace(@"*", @".*"), RegexOptions.IgnoreCase);
                            if (args.Channel != null && regChannel2.IsMatch(args.Channel))
                            {
                                Regex regMatch = new Regex(scr.TextMatch.Replace(@".", @"\.").Replace(@"*", @".*"), RegexOptions.IgnoreCase);
                                if (regMatch.IsMatch(args.Extra))
                                {
                                    command = scr.Command.Replace("$chan", scr.ChannelMatch);
                                    command = command.Replace("$nick", scr.ChannelMatch);
                                    command = command.Replace("$match", scr.TextMatch);
                                    command = command.Replace("$message", args.Extra);

                                    args.Command = command;

                                    if (OnCommand != null)
                                        OnCommand(args);

                                }
                            }
                        
                            break;                        
                        case "Channel Join":
                            Regex regChannel3 = new Regex(scr.ChannelMatch.Replace(@".", @"\.").Replace(@"*", @".*"), RegexOptions.IgnoreCase);                           
                            if (args.Channel != null && regChannel3.IsMatch(args.Channel))
                            {
                                command = scr.Command.Replace("$chan", scr.ChannelMatch);
                                command = command.Replace("$nick", scr.ChannelMatch);
                                command = command.Replace("$match", scr.TextMatch);
                                command = command.Replace("$message", args.Extra);

                                args.Command = command;

                                if (OnCommand != null)
                                    OnCommand(args);
                            
                            } 
                            break;
                    }

                }
            }
           
            return args;
        }

        public PluginArgs ChannelMessage(PluginArgs args)
        {
            args = CheckScripts(args, "Channel Message");
            return args;
        }

        public PluginArgs ChannelAction(PluginArgs args)
        {
            args = CheckScripts(args, "Channel Action");
            return args;
        }

        public PluginArgs QueryMessage(PluginArgs args)
        {
            args = CheckScripts(args, "Private Message");
            return args;
        }

        public PluginArgs QueryAction(PluginArgs args)
        {
            args = CheckScripts(args, "Private Action");
            return args;
        }

        public PluginArgs ChannelJoin(PluginArgs args)
        {
            args = CheckScripts(args, "Channel Join");
            return args;
        }

        public PluginArgs ChannelPart(PluginArgs args)
        {
            return args;
        }

        public PluginArgs ServerQuit(PluginArgs args)
        {
            return args;
        }

        public PluginArgs InputText(PluginArgs args)
        {
            return args;
        }


        public PluginArgs ChannelKick(PluginArgs args)
        {
            return args;
        }

        public PluginArgs ServerNotice(PluginArgs args)
        {
            return args;
        }

        public PluginArgs UserNotice(PluginArgs args)
        {
            return args;
        }

        public PluginArgs CtcpMessage(PluginArgs args)
        {
            //args.Extra        -- ctcp message 
            return args;
        }

        public PluginArgs CtcpReply(PluginArgs args)
        {
            //args.Extra        -- ctcp message 
            return args;
        }

        public PluginArgs ServerMessage(PluginArgs args)
        {
            return args;
        }

        public void NickChange(PluginArgs args)
        {

        }

        public void ServerRaw(PluginArgs args)
        {

        }

        public void ServerError(PluginArgs args)
        {

        }

        public void ServerConnect(PluginArgs args)
        {

        }

        public void ServerPreConnect(PluginArgs args)
        {

        }

        public void ServerDisconnect(PluginArgs args)
        {

        }

        public void WhoisUser(PluginArgs args)
        {

        }

        public PluginArgs ChannelNotice(PluginArgs args)
        {

            return args;
        }

        public void ChannelTopic(PluginArgs args)
        {

        }

        public void ChannelInvite(PluginArgs args)
        {

        }

        public void ChannelMode(PluginArgs args)
        {
            //args.Extra --  full channel mode

        }

        public void BuddyList(PluginArgs args)
        {
            //args.Extra -- "online" or "offline"

        }

    }

    //seperate file for all the highlite items
    public class IceChatScripts
    {
        [XmlArray("ScriptItems")]
        [XmlArrayItem("Item", typeof(ScriptItem))]
        public ArrayList listScripts;

        public IceChatScripts()
        {
            listScripts = new ArrayList();
        }
        public void AddScriptItem(ScriptItem scr)
        {
            listScripts.Add(scr);
        }
    }
    
    public class ScriptItem
    {
        [XmlElement("ScriptEvent")]
        public string ScriptEvent
        { get; set; }
        
        //watch text to match
        [XmlElement("TextMatch")]
        public string TextMatch
        { get; set; }

        //match a user or a nick
        [XmlElement("ChannelMatch")]
        public string ChannelMatch
        { get; set; }

        //the command to run on a match
        [XmlElement("Command")]
        public string Command
        { get; set; }

        //whether the script item is enabled
        [XmlElement("Enabled")]
        public bool Enabled
        { get; set; }
    }

}
