using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IceChat
{
    public partial class FormColorPicker : Form
    {
        private ColorButtonArray colorPicker;

        public FormColorPicker()
        {
            InitializeComponent();

            colorPicker = new ColorButtonArray(panelColorPicker);
            colorPicker.OnClick += new ColorButtonArray.ColorSelected(colorPicker_OnClick);

            ApplyLanguage();
        }

        private void ApplyLanguage()
        {
            buttonClose.Text = FormMain.Instance.IceChatLanguage.buttonClose;
        }

        private void colorPicker_OnClick(int colorSelected)
        {
            FormMain.Instance.InputPanel.AppendText((char)3 + colorSelected.ToString());
            this.Close();
        }
        
        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
