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
    public partial class FormPluginInfo : Form
    {
        IPluginIceChat plugin;
        ToolStripMenuItem menuItem;

        public FormPluginInfo(IPluginIceChat plugin, ToolStripMenuItem menuItem)
        {
            InitializeComponent();

            labelName.Text = plugin.Name;
            labelAuthor.Text = plugin.Author;
            labelVersion.Text = plugin.Version;

            this.plugin = plugin;
            this.menuItem = menuItem;

            ApplyLanguage();
        }

        private void ApplyLanguage()
        {

        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonUnload_Click(object sender, EventArgs e)
        {
            FormMain.Instance.UnloadPlugin(menuItem);
            this.Close();
        }

        private void buttonReLoad_Click(object sender, EventArgs e)
        {
            FormMain.Instance.ReloadPlugin(menuItem);
            this.Close();
        }
    }
}
