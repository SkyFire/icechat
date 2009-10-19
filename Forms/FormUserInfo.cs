using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IceChat2009
{
    public partial class FormUserInfo : Form
    {
        private User user;
        
        public FormUserInfo()
        {
            InitializeComponent();

        }
        
        public void SetUser(User nick)
        {
            user = nick;

        }

    }
}
