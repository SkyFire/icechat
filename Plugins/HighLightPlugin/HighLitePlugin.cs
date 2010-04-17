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
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Collections;
using System.IO;


namespace IceChatPlugin
{
    public class Plugin : IPluginIceChat
    {
        private string m_Name;
        private string m_Author;
        private string m_Version;

        private Form m_MainForm;
        private MenuStrip m_MenuStrip;

        //all the events get declared here, do not change
        public event OutGoingCommandHandler OnCommand;

        private TabPage tabPageHighlight;
        private Button buttonAdd;
        private Button buttonRemove;
        private Button buttonEdit;
        private ListView listHighLite;

        private ColumnHeader columnMatch;
        private ColumnHeader columnCommand;
        private ColumnHeader columnColor;

        private IceChatHighLites iceChatHighLites;
        private string highlitesFile;
        private string currentFolder;

        public Plugin()
        {
            //set your default values here
            m_Name = "HighLite Plugin";
            m_Author = "Snerf";
            m_Version = "1.0";
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

        public void ShowInfo()
        {
            MessageBox.Show(m_Name + " Loaded", m_Name + " " + m_Author);
        }

        public void Dispose()
        {

        }

        public void Initialize()
        {
            highlitesFile = currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatHighLites.xml";
            //currentFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //if (File.Exists(currentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatHighLites.xml"))
            //else
            //    highlitesFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + System.IO.Path.DirectorySeparatorChar + "IceChatHighLites.xml";
            LoadHighLites();

        }

        public void MainProgramLoaded()
        {

        }

        public void LoadSettingsForm(TabControl SettingsTab)
        {


        }
        
        public void LoadColorsForm(TabControl OptionsTab)
        {
            //when the Options Form gets loaded, ability to add tabs
            //add the Highlite Tab

            //when the Settings Form gets loaded, ability to add tabs

            tabPageHighlight = new System.Windows.Forms.TabPage();
            buttonAdd = new Button();
            buttonRemove = new Button();
            buttonEdit = new Button();
            listHighLite = new ListView();
            columnMatch = new ColumnHeader();
            columnCommand = new ColumnHeader();
            columnColor = new ColumnHeader();

            columnMatch.Text = "Text Highlite";
            columnMatch.Width = 250;
            columnCommand.Width = 0;
            columnColor.Width = 0;

            tabPageHighlight.SuspendLayout();

            buttonRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonRemove.Location = new System.Drawing.Point(291, 96);
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
            buttonEdit.Location = new System.Drawing.Point(291, 63);
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
            buttonAdd.Location = new System.Drawing.Point(291, 30);
            buttonAdd.Name = "buttonAdd";
            buttonAdd.Size = new System.Drawing.Size(75, 27);
            buttonAdd.TabIndex = 2;
            buttonAdd.Text = "Add";
            buttonAdd.UseVisualStyleBackColor = true;
            buttonAdd.Click += new EventHandler(buttonAdd_Click);
            // listHighLite
            // 
            listHighLite.CheckBoxes = true;
            listHighLite.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnMatch,
            columnCommand,
            columnColor});
            
            
            listHighLite.FullRowSelect = true;
            listHighLite.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            listHighLite.HideSelection = false;
            listHighLite.LabelWrap = false;
            listHighLite.Location = new System.Drawing.Point(25, 30);
            listHighLite.MultiSelect = false;
            listHighLite.Name = "listHighLite";
            listHighLite.ShowGroups = false;
            listHighLite.Size = new System.Drawing.Size(250, 288);
            listHighLite.TabIndex = 1;
            listHighLite.UseCompatibleStateImageBehavior = false;
            listHighLite.View = System.Windows.Forms.View.Details;

            tabPageHighlight.BackColor = System.Drawing.SystemColors.Control;
            tabPageHighlight.Controls.Add(buttonRemove);
            tabPageHighlight.Controls.Add(buttonEdit);
            tabPageHighlight.Controls.Add(buttonAdd);
            tabPageHighlight.Controls.Add(listHighLite);
            tabPageHighlight.Location = new System.Drawing.Point(4, 25);
            tabPageHighlight.Name = "tabPageHighlight2";
            tabPageHighlight.Padding = new System.Windows.Forms.Padding(3);
            tabPageHighlight.Size = new System.Drawing.Size(710, 339);
            
            
            tabPageHighlight.Text = "High Lite";

            tabPageHighlight.ResumeLayout();

            OptionsTab.Controls.Add(tabPageHighlight);

            ShowHighLites();

        }

        public void SaveColorsForm()
        {
            //MessageBox.Show("Saving:" + iceChatHighLites.listHighLites.Count + ":" + listHighLite.Items.Count);

            iceChatHighLites.listHighLites.Clear();

            foreach (ListViewItem item in listHighLite.Items)
            {
                HighLiteItem hli = new HighLiteItem();
                hli.Match = item.Text;
                hli.Command = item.SubItems[1].Text;
                hli.Color = Convert.ToInt32(item.SubItems[2].Text);
                hli.Enabled = item.Checked;
                iceChatHighLites.AddHighLight(hli);
            }
            
            SaveHighLites();
        }

        public void SaveSettingsForm()
        {
            //
        }
        private void ShowHighLites()
        {
            foreach (HighLiteItem hli in iceChatHighLites.listHighLites)
            {
                ListViewItem lvi = this.listHighLite.Items.Add(hli.Match);
                lvi.SubItems.Add(hli.Command);
                lvi.SubItems.Add(hli.Color.ToString());
                lvi.ForeColor = IrcColor.colors[hli.Color];
                lvi.Checked = hli.Enabled;
            }
        }

        private void LoadHighLites()
        {
            if (File.Exists(highlitesFile))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(IceChatHighLites));
                TextReader textReader = new StreamReader(highlitesFile);
                iceChatHighLites = (IceChatHighLites)deserializer.Deserialize(textReader);
                textReader.Close();
                textReader.Dispose();
            }
            else
                iceChatHighLites = new IceChatHighLites();
        }

        private void SaveHighLites()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(IceChatHighLites));
            TextWriter textWriter = new StreamWriter(highlitesFile);
            serializer.Serialize(textWriter, iceChatHighLites);
            textWriter.Close();
            textWriter.Dispose();
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listHighLite.SelectedItems)
            {
                HighLiteItem hli = new HighLiteItem();

                hli.Match = item.Text;
                hli.Command = item.SubItems[1].Text;
                hli.Color = Convert.ToInt32(item.SubItems[2].Text);

                FormHighLite fi = new FormHighLite(hli, item.Index);
                fi.SaveHighLite += new FormHighLite.SaveHighLiteDelegate(UpdateHighLite);                
                fi.ShowDialog(m_MainForm);
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            FormHighLite fi = new FormHighLite(new HighLiteItem(), 0);
            fi.SaveHighLite += new FormHighLite.SaveHighLiteDelegate(SaveNewHighLite);
            fi.ShowDialog(m_MainForm);

        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listHighLite.SelectedItems)
            {
                listHighLite.Items.Remove(item);
            }

        }

        private void SaveNewHighLite(HighLiteItem hli, int listIndex)
        {
            if (hli.Match.Length > 0)
            {
                ListViewItem lvi = this.listHighLite.Items.Add(hli.Match);
                lvi.SubItems.Add(hli.Command);
                lvi.SubItems.Add(hli.Color.ToString());
                lvi.ForeColor = IrcColor.colors[hli.Color];
                lvi.Checked = true;
            }
        }

        private void UpdateHighLite(HighLiteItem hli, int listIndex)
        {
            foreach (ListViewItem item in listHighLite.SelectedItems)
            {
                if (item.Index == listIndex)
                {
                    item.Text = hli.Match;
                    item.SubItems[1].Text = hli.Command;
                    item.SubItems[2].Text = hli.Color.ToString();
                    item.ForeColor = IrcColor.colors[hli.Color];
                    break;
                }
            }
        }

        private string CheckTextHighLite(string message)
        {
            //parse out any identifiers for the channel/nick, etc

            foreach (HighLiteItem hli in iceChatHighLites.listHighLites)
            {
                if (hli.Enabled)
                {
                    //string match = hli.Match.Replace("$chan", t.WindowName);
                    //match = match.Replace("$me", t.Connection.ServerSetting.NickName);
                    string match = hli.Match;

                    if (message.IndexOf(match, StringComparison.InvariantCultureIgnoreCase) > -1)
                    {
                        if (message.StartsWith(((char)3).ToString()))
                        {
                            //find the color number and replace
                            int result;
                            if (Int32.TryParse(message.Substring(1, 2), out result))
                                message = "&#x3;" + hli.Color + message.Substring(3);
                            else
                                message = "&#x3;" + hli.Color + message.Substring(2);
                        }
                        else if (message.StartsWith("&#x3;"))
                        {
                            //find the color number and replace
                            int result;
                            if (Int32.TryParse(message.Substring(5, 2), out result))
                                message = "&#x3;" + hli.Color + message.Substring(7);
                            else
                                message = "&#x3;" + hli.Color + message.Substring(6);

                        }
                        else
                            message = "&#x3;" + hli.Color.ToString() + message;

                        //System.Diagnostics.Debug.WriteLine("matched:" + message + "::" + hli.Match);

                        break;
                    }
                }
            }

            return message;
        }

        public PluginArgs ChannelMessage(PluginArgs args)
        {
            args.Message = CheckTextHighLite(args.Message);
            return args;
        }

        public PluginArgs ChannelAction(PluginArgs args)
        {
            args.Message = CheckTextHighLite(args.Message);
            return args;
        }

        public PluginArgs QueryMessage(PluginArgs args)
        {
            args.Message = CheckTextHighLite(args.Message);
            return args;
        }

        public PluginArgs QueryAction(PluginArgs args)
        {
            args.Message = CheckTextHighLite(args.Message);
            return args;
        }

        public PluginArgs ChannelJoin(PluginArgs args)
        {
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

        public void ServerRaw(PluginArgs args)
        {

        }

    }

    //seperate file for all the highlite items
    public class IceChatHighLites
    {
        [XmlArray("HighLites")]
        [XmlArrayItem("Item", typeof(HighLiteItem))]
        public ArrayList listHighLites;

        public IceChatHighLites()
        {
            listHighLites = new ArrayList();
        }
        public void AddHighLight(HighLiteItem hli)
        {
            listHighLites.Add(hli);
        }
    }
    
    public class HighLiteItem
    {
        [XmlElement("Match")]
        public string Match
        { get; set; }

        [XmlElement("Color")]
        public int Color
        { get; set; }

        [XmlElement("Command")]
        public string Command
        { get; set; }

        [XmlElement("Enabled")]
        public bool Enabled
        { get; set; }
    }

}
