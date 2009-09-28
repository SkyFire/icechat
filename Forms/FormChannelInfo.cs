using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IceChat2009
{
    public partial class FormChannelInfo : Form
    {
        private TabWindow channel;
        
        public FormChannelInfo(TabWindow Channel)
        {
            InitializeComponent();

            this.channel = Channel;
            this.Text = channel.WindowName;
        }

    }
}
