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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using IceChatPlugin;

namespace IceChat
{
    public partial class FormSettings : Form
    {
        private IceChatOptions iceChatOptions;
        private IceChatFontSetting iceChatFonts;
        private IceChatEmoticon iceChatEmoticons;
        private IceChatSounds iceChatSounds;

        private ListViewItem listMoveItem = null;

        private System.Media.SoundPlayer player;
        

        public delegate void SaveOptionsDelegate();
        public event SaveOptionsDelegate SaveOptions;

        private enum FontType
        {
            Console = 0,
            Channel = 1,
            Query = 2,
            NickList = 3,
            ServerList = 4,
            InputBox = 5,
            ChannelBar = 6
        }


        public FormSettings(IceChatOptions Options, IceChatFontSetting Fonts, IceChatEmoticon Emoticons, IceChatSounds Sounds)
        {
            InitializeComponent();
            
            /*
            listBoxSounds.Items.Add("New Message in Console");
            listBoxSounds.Items.Add("New Channel Message");
            listBoxSounds.Items.Add("New Private Message");
            listBoxSounds.Items.Add("New User Notice");
            listBoxSounds.Items.Add("Your Nickname is said in a Channel");
            listBoxSounds.Items.Add("Your Nickname is said in a Private Message");
            listBoxSounds.Items.Add("A Buddy has come Online");
            listBoxSounds.Items.Add("Server Disconnection");
            */

            foreach(IceChatSounds.soundEntry x in Sounds.soundList)
            {
                listBoxSounds.Items.Add(x.Description);
            }

            this.iceChatOptions = Options;
            this.iceChatFonts = Fonts;
            this.iceChatEmoticons = Emoticons;
            this.iceChatSounds = Sounds;

            this.listViewEmot.MouseDown += new MouseEventHandler(listViewEmot_MouseDown);
            this.listViewEmot.MouseUp += new MouseEventHandler(listViewEmot_MouseUp);

            //populate the font settings
            textConsoleFont.Font = new Font(iceChatFonts.FontSettings[0].FontName, 10);
            textConsoleFont.Text = iceChatFonts.FontSettings[0].FontName;
            textConsoleFontSize.Text = iceChatFonts.FontSettings[0].FontSize.ToString();

            textChannelFont.Font = new Font(iceChatFonts.FontSettings[1].FontName, 10);
            textChannelFont.Text = iceChatFonts.FontSettings[1].FontName;
            textChannelFontSize.Text = iceChatFonts.FontSettings[1].FontSize.ToString();

            textQueryFont.Font = new Font(iceChatFonts.FontSettings[2].FontName, 10);
            textQueryFont.Text = iceChatFonts.FontSettings[2].FontName;
            textQueryFontSize.Text = iceChatFonts.FontSettings[2].FontSize.ToString();

            textNickListFont.Font = new Font(iceChatFonts.FontSettings[3].FontName, 10);
            textNickListFont.Text = iceChatFonts.FontSettings[3].FontName;
            textNickListFontSize.Text= iceChatFonts.FontSettings[3].FontSize.ToString();

            textServerListFont.Font = new Font(iceChatFonts.FontSettings[4].FontName, 10);
            textServerListFont.Text = iceChatFonts.FontSettings[4].FontName;
            textServerListFontSize.Text = iceChatFonts.FontSettings[4].FontSize.ToString();

            textInputFont.Font = new Font(iceChatFonts.FontSettings[5].FontName, 10);
            textInputFont.Text = iceChatFonts.FontSettings[5].FontName;
            textInputFontSize.Text = iceChatFonts.FontSettings[5].FontSize.ToString();

            //populate the settings
            textTimeStamp.Text = iceChatOptions.TimeStamp;
            checkSaveWindowPosition.Checked = iceChatOptions.SaveWindowPosition;
            checkLogConsole.Checked = iceChatOptions.LogConsole;
            checkLogChannel.Checked = iceChatOptions.LogChannel;
            checkLogQuery.Checked = iceChatOptions.LogQuery;
            checkSeperateLogs.Checked = iceChatOptions.SeperateLogs;
            comboLogFormat.Text = iceChatOptions.LogFormat;

            if (iceChatEmoticons != null)
            {
                //load in the emoticons
                foreach (EmoticonItem emot in iceChatEmoticons.listEmoticons)
                {
                    Bitmap bm = new Bitmap(FormMain.Instance.EmoticonsFolder + System.IO.Path.DirectorySeparatorChar + emot.EmoticonImage);
                    int i = imageListEmoticons.Images.Add(bm, Color.Fuchsia);
                    ListViewItem lvi = new ListViewItem(emot.Trigger, i);
                    lvi.SubItems.Add(emot.EmoticonImage);
                    listViewEmot.Items.Add(lvi);
                }
            }

            checkEmoticons.Checked = iceChatOptions.ShowEmoticons;
            checkEmoticonPicker.Checked = iceChatOptions.ShowEmoticonPicker;
            checkColorPicker.Checked = iceChatOptions.ShowColorPicker;
            checkStatusBar.Checked = iceChatOptions.ShowStatusBar;
            checkDisableQueries.Checked = iceChatOptions.DisableQueries;
            checkNewQueryForegound.Checked = iceChatOptions.NewQueryForegound;
            checkWhoisNewQuery.Checked = iceChatOptions.WhoisNewQuery;
            checkShowUnreadLine.Checked = iceChatOptions.ShowUnreadLine;

            //dcc settings
            checkAutoDCCChat.Checked = iceChatOptions.DCCChatAutoAccept;
            checkIgnoreDCCChat.Checked = iceChatOptions.DCCChatIgnore;
            textDCCChatTimeout.Text = iceChatOptions.DCCChatTimeOut.ToString();
            textDCCPortLow.Text = iceChatOptions.DCCPortLower.ToString();
            textDCCPortHigh.Text = iceChatOptions.DCCPortUpper.ToString();

            //load any plugin addons
            foreach (IPluginIceChat ipc in FormMain.Instance.IceChatPlugins)
            {
                ipc.LoadSettingsForm(this.tabControlOptions);
            }

            comboBoxLanguage.DataSource = FormMain.Instance.IceChatLanguageFiles;
            comboBoxLanguage.SelectedItem = FormMain.Instance.IceChatCurrentLanguageFile;
            
            //Event Settings
            comboJoinEvent.SelectedIndex = iceChatOptions.JoinEventLocation;
            comboPartEvent.SelectedIndex = iceChatOptions.PartEventLocation;
            comboQuitEvent.SelectedIndex = iceChatOptions.QuitEventLocation;
            comboModeEvent.SelectedIndex = iceChatOptions.ModeEventLocation;
            comboKickEvent.SelectedIndex = iceChatOptions.KickEventLocation;
            comboTopicEvent.SelectedIndex = iceChatOptions.TopicEventLocation;
            comboChannelMessageEvent.SelectedIndex = iceChatOptions.ChannelMessageEventLocation;
            comboChannelActionEvent.SelectedIndex = iceChatOptions.ChannelActionEventLocation;


            ApplyLanguage();

            player = new System.Media.SoundPlayer();
        }

        private void ApplyLanguage()
        {

        }

        private void listViewEmot_MouseUp(object sender, MouseEventArgs e)
        {
            if (listMoveItem == null)
                return;

            ListViewItem itemOver = listViewEmot.GetItemAt(e.X, e.Y);
            if (itemOver == null)
                return;
   
            listViewEmot.Items.Remove(listMoveItem);
            listViewEmot.Items.Insert(itemOver.Index + 1, listMoveItem);

            listViewEmot.Invalidate();

            listMoveItem = null;
            listViewEmot.Cursor = Cursors.Default;
        }

        private void listViewEmot_MouseDown(object sender, MouseEventArgs e)
        {
            foreach (ListViewItem eachItem in listViewEmot.SelectedItems)
            {
                listMoveItem = eachItem;
                listViewEmot.Cursor = Cursors.Hand;
                return;
            }
            listMoveItem = null;
            listViewEmot.Cursor = Cursors.Default;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            //set all the options accordingly

            iceChatOptions.SaveWindowPosition = checkSaveWindowPosition.Checked;
            iceChatOptions.TimeStamp = textTimeStamp.Text;
            iceChatOptions.LogConsole = checkLogConsole.Checked;
            iceChatOptions.LogChannel = checkLogChannel.Checked;
            iceChatOptions.LogQuery = checkLogQuery.Checked;
            iceChatOptions.SeperateLogs = checkSeperateLogs.Checked;
            iceChatOptions.LogFormat = comboLogFormat.Text;
            iceChatOptions.ShowEmoticons = checkEmoticons.Checked;
            iceChatOptions.ShowEmoticonPicker = checkEmoticonPicker.Checked;
            iceChatOptions.ShowColorPicker = checkColorPicker.Checked;
            iceChatOptions.ShowStatusBar = checkStatusBar.Checked;
            iceChatOptions.DisableQueries = checkDisableQueries.Checked;
            iceChatOptions.NewQueryForegound = checkNewQueryForegound.Checked;
            iceChatOptions.WhoisNewQuery = checkWhoisNewQuery.Checked;

            iceChatOptions.ShowUnreadLine = checkShowUnreadLine.Checked;
            iceChatOptions.Language = ((LanguageItem)comboBoxLanguage.SelectedItem).LanguageName;

            //set all the fonts
            iceChatFonts.FontSettings[0].FontName = textConsoleFont.Text;
            iceChatFonts.FontSettings[0].FontSize = float.Parse(textConsoleFontSize.Text);

            iceChatFonts.FontSettings[1].FontName = textChannelFont.Text;
            iceChatFonts.FontSettings[1].FontSize = float.Parse(textChannelFontSize.Text);

            iceChatFonts.FontSettings[2].FontName = textQueryFont.Text;
            iceChatFonts.FontSettings[2].FontSize = float.Parse(textQueryFontSize.Text);

            iceChatFonts.FontSettings[3].FontName = textNickListFont.Text;
            iceChatFonts.FontSettings[3].FontSize = float.Parse(textNickListFontSize.Text);

            iceChatFonts.FontSettings[4].FontName = textServerListFont.Text;
            iceChatFonts.FontSettings[4].FontSize = float.Parse(textServerListFontSize.Text);

            iceChatFonts.FontSettings[5].FontName = textInputFont.Text;
            iceChatFonts.FontSettings[5].FontSize = float.Parse(textInputFontSize.Text);
            
            //icechatFonts.FontSettings[6].FontName = textChannelBarFont.Text;
            //icechatFonts.FontSettings[6].FontSize = float.Parse(textChannelBarFontSize.Text);

            //dcc settings
            iceChatOptions.DCCChatAutoAccept = checkAutoDCCChat.Checked;
            iceChatOptions.DCCChatIgnore = checkIgnoreDCCChat.Checked;
            iceChatOptions.DCCChatTimeOut = Convert.ToInt32(textDCCChatTimeout.Text);
            iceChatOptions.DCCPortLower = Convert.ToInt32(textDCCPortLow.Text);
            iceChatOptions.DCCPortUpper = Convert.ToInt32(textDCCPortHigh.Text);

            //save the emoticons
            iceChatEmoticons.listEmoticons.Clear();
            //re-add them all back in
            int i = 0;
            foreach (ListViewItem lvi in listViewEmot.Items)
            {
                EmoticonItem ei = new EmoticonItem();
                ei.EmoticonImage = lvi.SubItems[1].Text;
                ei.Trigger = lvi.Text;
                ei.ID = i++;
                iceChatEmoticons.AddEmoticon(ei);
            }

            FormMain.Instance.IceChatEmoticons = iceChatEmoticons;

            // apply language change
            FormMain.Instance.IceChatCurrentLanguageFile = (LanguageItem) comboBoxLanguage.SelectedItem;

            //Event Settings
            iceChatOptions.JoinEventLocation = comboJoinEvent.SelectedIndex;
            iceChatOptions.PartEventLocation = comboPartEvent.SelectedIndex;
            iceChatOptions.QuitEventLocation = comboQuitEvent.SelectedIndex;
            iceChatOptions.ModeEventLocation = comboModeEvent.SelectedIndex;
            iceChatOptions.KickEventLocation = comboKickEvent.SelectedIndex;
            iceChatOptions.TopicEventLocation = comboTopicEvent.SelectedIndex;
            iceChatOptions.ChannelMessageEventLocation = comboChannelMessageEvent.SelectedIndex;
            iceChatOptions.ChannelActionEventLocation = comboChannelActionEvent.SelectedIndex;

            if (SaveOptions != null)
                SaveOptions();

            this.Close();
        }

        private void buttonConsoleFont_Click(object sender, EventArgs e)
        {
            FontDialog fd = new FontDialog();
            //load the current font
            fd.Font = new Font(textConsoleFont.Text,float.Parse( textConsoleFontSize.Text) , textConsoleFont.Font.Style);
            if (fd.ShowDialog() != DialogResult.Cancel && fd.Font.Style == FontStyle.Regular)
            {
                textConsoleFont.Text = fd.Font.Name;
                textConsoleFontSize.Text = fd.Font.Size.ToString();
                textConsoleFont.Font = new Font(fd.Font.Name, 10, fd.Font.Style);
            }
        }

        private void buttonChannelFont_Click(object sender, EventArgs e)
        {
            FontDialog fd = new FontDialog();
            //load the current font
            fd.Font = new Font(textChannelFont.Text, float.Parse(textChannelFontSize.Text), textChannelFont.Font.Style);
            if (fd.ShowDialog() != DialogResult.Cancel && fd.Font.Style == FontStyle.Regular)
            {
                textChannelFont.Text = fd.Font.Name;
                textChannelFontSize.Text = fd.Font.Size.ToString();
                textChannelFont.Font = new Font(fd.Font.Name, 10, fd.Font.Style);
            }
        }

        private void buttonQueryFont_Click(object sender, EventArgs e)
        {
            FontDialog fd = new FontDialog();
            //load the current font
            fd.Font = new Font(textQueryFont.Text, float.Parse(textQueryFontSize.Text), textQueryFont.Font.Style);
            if (fd.ShowDialog() != DialogResult.Cancel && fd.Font.Style == FontStyle.Regular)
            {
                textQueryFont.Text = fd.Font.Name;
                textQueryFontSize.Text = fd.Font.Size.ToString();
                textQueryFont.Font = new Font(fd.Font.Name, 10, fd.Font.Style);
            }
        }

        private void buttonChannelBarFont_Click(object sender, EventArgs e)
        {
            /*
            FontDialog fd = new FontDialog();
            //load the current font
            fd.Font = new Font(textChannelBarFont.Text, float.Parse(textChannelBarFontSize.Text));
            if (fd.ShowDialog() != DialogResult.Cancel)
            {
                textChannelBarFont.Text = fd.Font.Name;
                textChannelBarFontSize.Text = fd.Font.Size.ToString();
                textChannelBarFont.Font = new Font(fd.Font.Name, 10);
            }
            */
        }

        private void buttonNickListFont_Click(object sender, EventArgs e)
        {
            FontDialog fd = new FontDialog();
            //load the current font
            fd.Font = new Font(textNickListFont.Text, float.Parse(textNickListFontSize.Text), textNickListFont.Font.Style);
            if (fd.ShowDialog() != DialogResult.Cancel && fd.Font.Style == FontStyle.Regular)
            {
                textNickListFont.Text = fd.Font.Name;
                textNickListFontSize.Text = fd.Font.Size.ToString();
                textNickListFont.Font = new Font(fd.Font.Name, 10,fd.Font.Style);
            }
            
        }

        private void buttonServerListFont_Click(object sender, EventArgs e)
        {
            FontDialog fd = new FontDialog();
            //load the current font            
            fd.Font = new Font(textServerListFont.Text, float.Parse(textServerListFontSize.Text), textServerListFont.Font.Style);
            if (fd.ShowDialog() != DialogResult.Cancel && fd.Font.Style == FontStyle.Regular)
            {
                textServerListFont.Text = fd.Font.Name;
                textServerListFontSize.Text = fd.Font.Size.ToString();
                textServerListFont.Font = new Font(fd.Font.Name, 10, fd.Font.Style);
            }

        }

        private void buttonInputFont_Click(object sender, EventArgs e)
        {
            FontDialog fd = new FontDialog();
            fd.AllowVerticalFonts = false;
            fd.FontMustExist = true;            
            //load the current font
            fd.Font = new Font(textInputFont.Text, float.Parse(textInputFontSize.Text), textInputFont.Font.Style);
            if (fd.ShowDialog() != DialogResult.Cancel && fd.Font.Style == FontStyle.Regular)
            {
                textInputFont.Text = fd.Font.Name;
                textInputFontSize.Text = fd.Font.Size.ToString();
                textInputFont.Font = new Font(fd.Font.Name, 10, fd.Font.Style);
            }
        }

        private void buttonAddEmoticon_Click(object sender, EventArgs e)
        {
            //add a new emoticon
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = FormMain.Instance.CurrentFolder + System.IO.Path.DirectorySeparatorChar + "Emoticons";            
            ofd.Filter = "Images (*.png)|*.png";
            //ofd.Multiselect = true;
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //check if emoticon exists                
                Bitmap bm = new Bitmap(ofd.FileName);
                int i = imageListEmoticons.Images.Add(bm, Color.Fuchsia);
                ListViewItem lvi = new ListViewItem("<edit>", i);
                lvi.SubItems.Add(ofd.SafeFileName);
                listViewEmot.Items.Add(lvi);
            }
        }

        private void buttonRemoveEmoticon_Click(object sender, EventArgs e)
        {
            //check if one is selected and remove it
            foreach(ListViewItem eachItem in listViewEmot.SelectedItems)
                listViewEmot.Items.Remove(eachItem);
        }

        private void buttonEditTrigger_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem eachItem in listViewEmot.SelectedItems)
                eachItem.BeginEdit();
        }

        private void listBoxSounds_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxSounds.SelectedIndex >= 0)
            {
                textSound.Text = iceChatSounds.getSound(listBoxSounds.SelectedIndex).File;
            }
        }

        private void buttonChooseSound_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                if (textSound.Text.Length > 0)
                {
                    //go to the same folder
                    ofd.InitialDirectory = System.IO.Path.GetDirectoryName(textSound.Text);
                }
                else
                    ofd.InitialDirectory = FormMain.Instance.CurrentFolder + System.IO.Path.DirectorySeparatorChar + "Sounds";

                ofd.RestoreDirectory = true;
                ofd.Filter = "Sounds (*.wav)|*.wav";

                if (ofd.ShowDialog() == DialogResult.OK)
                    textSound.Text = ofd.FileName;
            }
            catch { }
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            if (textSound.Text.Length > 0)
            {
                try
                {
                    player.SoundLocation = @textSound.Text;
                    player.Play();
                }
                catch { }
            }
        }
        
        private void textSound_TextChanged(object sender, EventArgs e)
        {
            if (listBoxSounds.SelectedIndex >= 0)
            {
                iceChatSounds.getSound(listBoxSounds.SelectedIndex).File = textSound.Text;
            }
        }

        private void buttonBrowseLogs_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(FormMain.Instance.LogsFolder);
        }
    }
}