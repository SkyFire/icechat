using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IceChat
{
    public partial class FormEmoticons : Form
    {
        public FormEmoticons()
        {
            InitializeComponent();

            Bitmap emots = new Bitmap(this.Width, this.Height);
            
            Graphics g = Graphics.FromImage(emots);            
            
            int x = 0;
            int y = 0;

            
            foreach (EmoticonItem emot in FormMain.Instance.IceChatEmoticons.listEmoticons)
            {
                try
                {
                    Bitmap bm = new Bitmap(FormMain.Instance.EmoticonsFolder + System.IO.Path.DirectorySeparatorChar + emot.EmoticonImage);
                    int i = imageListEmoticons.Images.Add(bm, Color.Fuchsia);
                    
                    //System.Diagnostics.Debug.WriteLine(x + ":" + y + ":" + this.Width + ":" + emot.Trigger);
                    
                    g.DrawImage(imageListEmoticons.Images[i], x, y);
                    
                    x = x + 21;
                    if (x >= (this.Width - 30))
                    {
                        x = 0;
                        y = y + 25;
                    }
                }
                catch { }
            }
            
            pictureEmoticons.Image = emots;
            
            g.Dispose();

            pictureEmoticons.MouseDown += new MouseEventHandler(pictureEmoticons_MouseDown);

        }

        private void pictureEmoticons_MouseDown(object sender, MouseEventArgs e)
        {
            //figure out the index
            int x = e.X / 21;
            int y = e.Y / 25;

            int emot = (y * 22) + x;
            if (emot < FormMain.Instance.IceChatEmoticons.listEmoticons.Count)
            {
                FormMain.Instance.InputPanel.AppendText(((EmoticonItem)FormMain.Instance.IceChatEmoticons.listEmoticons[emot]).Trigger);
                this.Close();
            }
        }
    }
}
