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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace IceChat
{
    public partial class ChannelList : UserControl
    {
        //private string favoriteChannelsFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + System.IO.Path.DirectorySeparatorChar + "IceChatChannels.xml";
        
        //internal bool collapsed;
        //internal int oldCollapse;
        
        public ChannelList()
        {
            InitializeComponent();

            labelHeader.Paint += new PaintEventHandler(OnHeaderPaint);

            DoubleBuffered = true;
            //load channel list from XML File
            ReadSettings();
        }


        /// <summary>
        /// Paint the header with a Gradient Background
        /// </summary>
        private void OnHeaderPaint(object sender, PaintEventArgs e)
        {
            Brush l = new LinearGradientBrush(e.Graphics.ClipBounds, Color.Silver, Color.White, 90);
            e.Graphics.FillRectangle(l, e.Graphics.ClipBounds);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;

            Rectangle centered = e.ClipRectangle;
            centered.Offset(0, (int)(e.ClipRectangle.Height - e.Graphics.MeasureString(labelHeader.Text, labelHeader.Font).Height) / 2);
            
            e.Graphics.DrawString(labelHeader.Text, labelHeader.Font, new SolidBrush(Color.Black), centered, sf);


            l.Dispose();            
            /*
            //draw the close/expand button
            Rectangle box = new Rectangle((int)e.Graphics.ClipBounds.Left + 1, 1, 20, (int)e.Graphics.ClipBounds.Height-6);

            //draw the highlight box around
            l = new LinearGradientBrush(box, Color.Silver, Color.White, 270);
            e.Graphics.FillEllipse(l, box);

            if (!collapsed)
            {
                //draw the top hash
                e.Graphics.DrawLine(new Pen(Color.Black, 2), box.Left + 6, box.Top + 5, box.Left + 10, box.Top + 9);
                e.Graphics.DrawLine(new Pen(Color.Black, 2), box.Left + 14, box.Top + 5, box.Left + 10, box.Top + 9);
                //draw the bottom hash
                e.Graphics.DrawLine(new Pen(Color.Black, 2), box.Left + 6, box.Top + 10, box.Left + 10, box.Top + 14);
                e.Graphics.DrawLine(new Pen(Color.Black, 2), box.Left + 14, box.Top + 10, box.Left + 10, box.Top + 14);
            }
            */
        }

        /// <summary>
        /// Check if Collapse Button has been pressed and dock to Side Panel
        /// </summary>
        /*
        private void OnHeaderMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {                
                if (e.X <= labelHeader.Left + 21)
                {
                    //collapse button clicked
                    if (!collapsed)
                    {
                        CollapseChannelList();
                    }
                }
            }
        }
        
        internal void CollapseChannelList()
        {
            if (!collapsed)
            {
                //oldCollapse = ((SplitContainer)this.Parent.Parent).SplitterDistance;

                //((SplitContainer)this.Parent.Parent).Panel2Collapsed = true;
                //FormMain.Instance.AddDockItem("Right", "Favorite Channels");
                collapsed = true;
            }
        }
        */
        /// <summary>
        /// Read in all the Favorite Channels from the XML File
        /// </summary>
        private void ReadSettings()
        {
            if (!File.Exists(FormMain.Instance.FavoriteChannelsFile)) return;

            FileStream fs = new FileStream(FormMain.Instance.FavoriteChannelsFile, FileMode.Open);
            XmlTextReader r = new XmlTextReader(fs);
            string currentElement = "";            
            while (r.Read())
            {
                if (r.Name.Length > 0)
                    currentElement = r.Name;
                else if (r.NodeType == XmlNodeType.Text && r.Value.Length > 1)
                    listChannels.Items.Add(r.Value);
            }

            r.Close();
            fs.Close();
        }
        
        /// <summary>
        /// Write out all the Favorite Servers to the XML File
        /// </summary>
        private void WriteSettings()
        {
            FileStream fs = new FileStream(FormMain.Instance.FavoriteChannelsFile, FileMode.Create);
            XmlTextWriter w = new XmlTextWriter(fs, System.Text.Encoding.UTF8);
            w.Formatting = Formatting.Indented;

            w.WriteStartDocument();
            w.WriteStartElement("FavoriteChannels");

            //int i = listChannels.Items.Count;
            for (int i = 0; i < listChannels.Items.Count; i++)
                w.WriteElementString("Channel" + i.ToString(),listChannels.Items[i].ToString());

            w.WriteEndElement();
            w.WriteEndDocument();

            w.Flush();
            w.Close();
            fs.Close();

            FormMain.Instance.FocusInputBox();
        }
        
        /// <summary>
        /// Join the Channel selected to the Current Server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listChannels_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            int s = listChannels.SelectedIndex;

            if (s == -1) return;

            IRCConnection c = FormMain.Instance.InputPanel.CurrentConnection;
            if (c != null)
                FormMain.Instance.ParseOutGoingCommand(c, "/join " + listChannels.Items[s].ToString());
        }

        /// <summary>
        /// Use a Dialog Box to ask for a New Favorite Channel to Add
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            //ask for a new channel to add
            InputBoxDialog i = new InputBoxDialog();
            i.FormCaption = "Add Favorite Channel";
            i.FormPrompt = "Enter a channel to add";

            i.ShowDialog();
            if (i.InputResponse.Length > 0)
                listChannels.Items.Add(i.InputResponse);
            
            //write out the settings file
            WriteSettings();
        }

        /// <summary>
        /// Join the Channel selected to the Current Server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonJoin_Click(object sender, EventArgs e)
        {
            //join the channel selected
            int s = listChannels.SelectedIndex;

            if (s == -1) return;

            IRCConnection c = FormMain.Instance.InputPanel.CurrentConnection;
            if (c != null)
            {
                FormMain.Instance.ParseOutGoingCommand(c, "/join " + listChannels.Items[s].ToString());
            }
            FormMain.Instance.FocusInputBox();
        }

        /// <summary>
        /// Edit a Favorite Channel selected with a Dialog Box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonEdit_Click(object sender, EventArgs e)
        {
            int s = listChannels.SelectedIndex;
            
            if (s == -1) return;

            InputBoxDialog i = new InputBoxDialog();
            i.FormCaption = "Edit Favorite Channel";
            i.FormPrompt = "Enter the new channel name";
            i.DefaultValue = listChannels.Items[s].ToString();

            i.ShowDialog();
            if (i.InputResponse.Length > 0)
                listChannels.Items[s] = i.InputResponse;

            WriteSettings();
        }

        /// <summary>
        /// Remove the Selected Favorite Channel from the List
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRemove_Click(object sender, EventArgs e)
        {
            int s = listChannels.SelectedIndex;

            if (s == -1) return;

            listChannels.Items.RemoveAt(s);

            WriteSettings();

            FormMain.Instance.FocusInputBox();
        }
    }
}
