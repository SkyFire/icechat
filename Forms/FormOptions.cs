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
        private IceChatFontSetting icechatFonts;
        private IceChatEmoticon iceChatEmoticons;

        private ListViewItem listMoveItem = null;


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


        public FormSettings(IceChatOptions Options, IceChatFontSetting Fonts, IceChatEmoticon Emoticons)
        {
            InitializeComponent();

            this.iceChatOptions = Options;
            this.icechatFonts = Fonts;
            this.iceChatEmoticons = Emoticons;

            this.listViewEmot.MouseDown += new MouseEventHandler(listViewEmot_MouseDown);
            this.listViewEmot.MouseUp += new MouseEventHandler(listViewEmot_MouseUp);

            //populate the font settings
            textConsoleFont.Font = new Font(icechatFonts.FontSettings[0].FontName, 10);
            textConsoleFont.Text = icechatFonts.FontSettings[0].FontName;
            textConsoleFontSize.Text = icechatFonts.FontSettings[0].FontSize.ToString();

            textChannelFont.Font = new Font(icechatFonts.FontSettings[1].FontName, 10);
            textChannelFont.Text = icechatFonts.FontSettings[1].FontName;
            textChannelFontSize.Text = icechatFonts.FontSettings[1].FontSize.ToString();

            textQueryFont.Font = new Font(icechatFonts.FontSettings[2].FontName, 10);
            textQueryFont.Text = icechatFonts.FontSettings[2].FontName;
            textQueryFontSize.Text = icechatFonts.FontSettings[2].FontSize.ToString();

            textNickListFont.Font = new Font(icechatFonts.FontSettings[3].FontName, 10);
            textNickListFont.Text = icechatFonts.FontSettings[3].FontName;
            textNickListFontSize.Text= icechatFonts.FontSettings[3].FontSize.ToString();

            textServerListFont.Font = new Font(icechatFonts.FontSettings[4].FontName, 10);
            textServerListFont.Text = icechatFonts.FontSettings[4].FontName;
            textServerListFontSize.Text = icechatFonts.FontSettings[4].FontSize.ToString();

            textInputFont.Font = new Font(icechatFonts.FontSettings[5].FontName, 10);
            textInputFont.Text = icechatFonts.FontSettings[5].FontName;
            textInputFontSize.Text = icechatFonts.FontSettings[5].FontSize.ToString();


            //textChannelBarFont.Font = new Font(icechatFonts.FontSettings[6].FontName, 10);
            //textChannelBarFont.Text = icechatFonts.FontSettings[6].FontName;
            //textChannelBarFontSize.Text = icechatFonts.FontSettings[6].FontSize.ToString();

            //populate the settings
            textTimeStamp.Text = iceChatOptions.TimeStamp;
            checkSaveWindowPosition.Checked = iceChatOptions.SaveWindowPosition;
            checkLogConsole.Checked = iceChatOptions.LogConsole;
            checkLogChannel.Checked = iceChatOptions.LogChannel;
            checkLogQuery.Checked = iceChatOptions.LogQuery;
            checkSeperateLogs.Checked = iceChatOptions.SeperateLogs;

            textDefaultNick.Text = iceChatOptions.DefaultNick;
            checkIdentServer.Checked = iceChatOptions.IdentServer;
            checkServerReconnect.Checked = iceChatOptions.ReconnectServer;

            //load in the emoticons
            foreach (EmoticonItem emot in iceChatEmoticons.listEmoticons)
            {
                Bitmap bm = new Bitmap(FormMain.Instance.EmoticonsFolder + System.IO.Path.DirectorySeparatorChar + emot.EmoticonImage);
                int i = imageListEmoticons.Images.Add(bm, Color.Fuchsia);
                ListViewItem lvi = new ListViewItem(emot.Trigger, i);
                lvi.SubItems.Add(emot.EmoticonImage);
                listViewEmot.Items.Add(lvi);                
            }

            //load any plugin addons
            foreach (IPluginIceChat ipc in FormMain.Instance.IceChatPlugins)
            {
                ipc.LoadSettingsForm(this.tabControlOptions);
            }

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
            iceChatOptions.DefaultNick = textDefaultNick.Text;
            iceChatOptions.IdentServer = checkIdentServer.Checked;
            iceChatOptions.ReconnectServer = checkServerReconnect.Checked;
            iceChatOptions.LogConsole = checkLogConsole.Checked;
            iceChatOptions.LogChannel = checkLogChannel.Checked;
            iceChatOptions.LogQuery = checkLogQuery.Checked;
            iceChatOptions.SeperateLogs = checkSeperateLogs.Checked;

            //set all the fonts
            icechatFonts.FontSettings[0].FontName = textConsoleFont.Text;
            icechatFonts.FontSettings[0].FontSize = float.Parse(textConsoleFontSize.Text);

            icechatFonts.FontSettings[1].FontName = textChannelFont.Text;
            icechatFonts.FontSettings[1].FontSize = float.Parse(textChannelFontSize.Text);

            icechatFonts.FontSettings[2].FontName = textQueryFont.Text;
            icechatFonts.FontSettings[2].FontSize = float.Parse(textQueryFontSize.Text);

            icechatFonts.FontSettings[3].FontName = textNickListFont.Text;
            icechatFonts.FontSettings[3].FontSize = float.Parse(textNickListFontSize.Text);

            icechatFonts.FontSettings[4].FontName = textServerListFont.Text;
            icechatFonts.FontSettings[4].FontSize = float.Parse(textServerListFontSize.Text);

            icechatFonts.FontSettings[5].FontName = textInputFont.Text;
            icechatFonts.FontSettings[5].FontSize = float.Parse(textInputFontSize.Text);
            
            //icechatFonts.FontSettings[6].FontName = textChannelBarFont.Text;
            //icechatFonts.FontSettings[6].FontSize = float.Parse(textChannelBarFontSize.Text);
            
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
            ofd.InitialDirectory = FormMain.Instance.CurrentFolder + System.IO.Path.DirectorySeparatorChar + "IceChatEmoticons";            
            //ofd.Filter = "Images (*.PNG)|*.PNG)";
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

    }
}