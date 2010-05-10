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
    public partial class BuddyList : UserControl
    {
        private string headerCaption = "Buddy List";

        private IceChatBuddyList buddyList;


        public BuddyList()
        {
            InitializeComponent();

            this.Paint += new PaintEventHandler(OnHeaderPaint);
            this.DoubleClick += new EventHandler(OnDoubleClick);
            this.panelButtons.Resize += new EventHandler(panelButtons_Resize);
            this.Resize += new EventHandler(OnResize);
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
            
            //load buddy list from XML File
            buddyList = FormMain.Instance.IceChatBuddyList;
            LoadBuddies();
        }

        private void OnResize(object sender, EventArgs e)
        {
            treeBuddies.Height = this.Height - (panelButtons.Height + treeBuddies.Top);
            treeBuddies.Width = this.Width;
        }

        private void OnDoubleClick(object sender, EventArgs e)
        {
            if (this.Parent.Parent.GetType() == typeof(TabPage))
            {
                FormMain.Instance.UnDockPanel((Panel)this.Parent);
                return;
            }
        }

        internal void ApplyLanguage()
        {
            IceChatLanguage iceChatLanguage = FormMain.Instance.IceChatLanguage;
            headerCaption = iceChatLanguage.buddyListHeader;
            buttonAdd.Text = iceChatLanguage.favChanbuttonAdd;
            buttonMessage.Text = iceChatLanguage.buddyListbuttonMessage;
            buttonEdit.Text = iceChatLanguage.favChanbuttonEdit;
            buttonRemove.Text = iceChatLanguage.favChanbuttonRemove;
        }

        private void panelButtons_Resize(object sender, EventArgs e)
        {
            this.buttonAdd.Width = (panelButtons.Width / 2) - 4;
            this.buttonMessage.Width = buttonAdd.Width;
            this.buttonEdit.Width = buttonAdd.Width;
            this.buttonRemove.Width = buttonAdd.Width;
            this.buttonMessage.Left = (panelButtons.Width / 2) + 1;
            this.buttonRemove.Left = buttonMessage.Left;
        }

        internal void SetListColors()
        {
            this.treeBuddies.BackColor = IrcColor.colors[FormMain.Instance.IceChatColors.ChannelListBackColor];
            this.treeBuddies.ForeColor = IrcColor.colors[FormMain.Instance.IceChatColors.ChannelListForeColor];
        }

        /// <summary>
        /// Paint the header with a Gradient Background
        /// </summary>
        private void OnHeaderPaint(object sender, PaintEventArgs e)
        {
            Brush l = new LinearGradientBrush(e.Graphics.ClipBounds, IrcColor.colors[FormMain.Instance.IceChatColors.PanelHeaderBG1], IrcColor.colors[FormMain.Instance.IceChatColors.PanelHeaderBG2], 300);
            e.Graphics.FillRectangle(l, e.Graphics.ClipBounds);
            // http://www.scip.be/index.php?Page=ArticlesNET01&Lang=EN
            if (Application.RenderWithVisualStyles)
            {
                if (System.Windows.Forms.VisualStyles.VisualStyleRenderer.IsElementDefined(System.Windows.Forms.VisualStyles.VisualStyleElement.ExplorerBar.NormalGroupCollapse.Normal))
                {
                    System.Windows.Forms.VisualStyles.VisualStyleRenderer renderer = new System.Windows.Forms.VisualStyles.VisualStyleRenderer(System.Windows.Forms.VisualStyles.VisualStyleElement.ExplorerBar.NormalGroupCollapse.Normal);
                    Rectangle rect = new Rectangle(0, 0, 22, 22);
                    renderer.DrawBackground(e.Graphics, rect);
                }
            }
            
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            
            Font headerFont = new Font("Verdana", 10);

            Rectangle centered = e.ClipRectangle;
            centered.Offset(0, (int)(e.ClipRectangle.Height - e.Graphics.MeasureString(headerCaption, headerFont).Height) / 2);
            
            e.Graphics.DrawString(headerCaption, headerFont, new SolidBrush(Color.Black), centered, sf);

            l.Dispose();            
        }

        
        /// <summary>
        /// Read all buddies from the buddy list file
        /// </summary>
        private void LoadBuddies()
        {
            if (buddyList.listBuddies == null) return;

            foreach (BuddyListItem buddy in buddyList.listBuddies)
            {
                TreeNode t = new TreeNode();
                t.Text = buddy.BuddyName;
                t.Tag = buddy.Network;
                
                TreeNode root = this.treeBuddies.Nodes[0];
                root.Nodes.Add(t);
                treeBuddies.ExpandAll();
            }
        }
        
        /// <summary>
        /// Write out all the Buddy List to the XML File
        /// </summary>
        private void WriteSettings()
        {
            FormMain.Instance.IceChatBuddyList = buddyList;
            FormMain.Instance.FocusInputBox();
        }
        
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Edit Buddy List in Server Editor");
        }

        /// <summary>
        /// Add a new Buddy List Item to the Buddy List
        /// </summary>
        /// <param name="buddy"></param>
        /// <param name="network"></param>
        /// <param name="newItem"></param>
        private void OnAddBuddyList(string buddy, string network, TreeNode newItem)
        {
            if (buddy.Trim().Length == 0 && network.Trim().Length == 0)
                return;
            
            if (newItem == null)
            {                
                BuddyListItem b = new BuddyListItem();
                b.BuddyName = buddy;
                b.Network = network;

                buddyList.listBuddies.Add(b);

                TreeNode t = new TreeNode();
                t.Text = buddy;
                t.Tag = network;

                this.treeBuddies.Nodes[0].Nodes.Add(t);
                this.treeBuddies.ExpandAll();
            }
            
            WriteSettings();
        }

        private void buttonMessage_Click(object sender, EventArgs e)
        {
            //
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Edit Buddy List in Server Editor");
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Edit Buddy List in Server Editor");
        }
    }
}
