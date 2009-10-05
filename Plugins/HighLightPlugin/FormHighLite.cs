using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IceChatPlugin
{
    public partial class FormHighLite : Form
    {

        public delegate void SaveHighLiteDelegate(HighLiteItem hli, int listIndex);
        public event SaveHighLiteDelegate SaveHighLite;
        
        private ColorButtonArray colorPicker;
        private HighLiteItem highLiteItem;
        private int listIndex = 0;

        public FormHighLite(HighLiteItem hli, int index)
        {
            InitializeComponent();

            colorPicker = new ColorButtonArray(panelColorPicker);
            colorPicker.OnClick += new ColorButtonArray.ColorSelected(colorPicker_OnClick);
            
            this.highLiteItem = hli;
            this.listIndex = index;

            textHiLite.Text = highLiteItem.Match;

            textCommand.Text = highLiteItem.Command;
            if (highLiteItem.Color == 0)
            {
                colorPicker.SelectedColor = 1;
                highLiteItem.Color = 1;
            }
            else
            {
                colorPicker.SelectedColor = highLiteItem.Color;
            }
            
            textHiLite.ForeColor = IrcColor.colors[highLiteItem.Color];
            textHiLite.Tag = highLiteItem.Color;

        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            highLiteItem.Match = textHiLite.Text;
            highLiteItem.Command = textCommand.Text;
            highLiteItem.Color = (int)textHiLite.Tag;
            
            //update or add new item
            if (SaveHighLite != null)
                SaveHighLite(this.highLiteItem, listIndex);

            this.Close();     
        }

        private void colorPicker_OnClick(int colorSelected)
        {
            //change the color of the textbox
            textHiLite.ForeColor = IrcColor.colors[colorSelected];
            textHiLite.Tag = colorSelected;
        }
    }
    public class ColorButtonArray
    {
        //initialize 32 boxes for the 32 default colors

        private readonly System.Windows.Forms.Panel hostPanel;

        public delegate void ColorSelected(int ColorNumber);
        public event ColorSelected OnClick;

        private int selectedColor;

        private void OnPaint(object sender, PaintEventArgs e)
        {
            //draw the 32 colors, in 2 rows of 16
            for (int i = 0; i <= 15; i++)
            {

                e.Graphics.FillRectangle(new SolidBrush(IrcColor.colors[i]), (i * 17), 0, 15, 15);
                e.Graphics.DrawRectangle(new Pen(Color.Gray), (i * 17), 0, 15, 15);

                e.Graphics.FillRectangle(new SolidBrush(IrcColor.colors[i + 16]), (i * 17), 20, 15, 15);
                e.Graphics.DrawRectangle(new Pen(Color.Gray), (i * 17), 20, 15, 15);

                if (i == selectedColor)
                {
                    //draw a selection rectangle
                    e.Graphics.DrawRectangle(new Pen(Color.Black, 3), (i * 17), 0, 15, 15);
                }
                if (i + 16 == selectedColor)
                {
                    //draw a selection rectangle
                    e.Graphics.DrawRectangle(new Pen(Color.Black, 3), (i * 17), 20, 15, 15);
                }
            }
        }

        internal int SelectedColor
        {
            get { return selectedColor; }
            set { selectedColor = value; hostPanel.Invalidate(); }
        }

        internal ColorButtonArray(System.Windows.Forms.Panel host)
        {
            this.hostPanel = host;

            host.Paint += new PaintEventHandler(OnPaint);
            host.MouseUp += new MouseEventHandler(OnMouseUp);
        }

        private void OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            int xPos;
            if (e.Y < 18)
            {
                xPos = e.X / 17;
                if (OnClick != null)
                {
                    selectedColor = xPos;
                    hostPanel.Invalidate();
                    OnClick(xPos);
                }

            }
            else if ((e.Y > 19) || e.Y < 38)
            {
                xPos = e.X / 17;
                if (OnClick != null)
                {
                    selectedColor = xPos + 16;
                    hostPanel.Invalidate();
                    OnClick(xPos + 16);
                }
            }
        }
    }
    public static class IrcColor
    {
        public static Color[] colors;

        static IrcColor()
        {
            //Color color;
            colors = new Color[32];

            colors[0] = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
            colors[1] = System.Drawing.ColorTranslator.FromHtml("#000000");
            colors[2] = System.Drawing.ColorTranslator.FromHtml("#00007F");
            colors[3] = System.Drawing.ColorTranslator.FromHtml("#009300");
            colors[4] = System.Drawing.ColorTranslator.FromHtml("#FF0000");
            colors[5] = System.Drawing.ColorTranslator.FromHtml("#7F0000");
            colors[6] = System.Drawing.ColorTranslator.FromHtml("#9C009C");
            colors[7] = System.Drawing.ColorTranslator.FromHtml("#FC7F00");

            colors[8] = System.Drawing.ColorTranslator.FromHtml("#FFFF00");
            colors[9] = System.Drawing.ColorTranslator.FromHtml("#00FC00");
            colors[10] = System.Drawing.ColorTranslator.FromHtml("#009393");
            colors[11] = System.Drawing.ColorTranslator.FromHtml("#00FFFF");
            colors[12] = System.Drawing.ColorTranslator.FromHtml("#0000FC");
            colors[13] = System.Drawing.ColorTranslator.FromHtml("#FF00FF");
            colors[14] = System.Drawing.ColorTranslator.FromHtml("#7F7F7F");
            colors[15] = System.Drawing.ColorTranslator.FromHtml("#D2D2D2");

            colors[16] = System.Drawing.ColorTranslator.FromHtml("#CCFFCC");
            colors[17] = System.Drawing.ColorTranslator.FromHtml("#0066FF");
            colors[18] = System.Drawing.ColorTranslator.FromHtml("#FAEBD7");
            colors[19] = System.Drawing.ColorTranslator.FromHtml("#FFD700");
            colors[20] = System.Drawing.ColorTranslator.FromHtml("#E6E6E6");
            colors[21] = System.Drawing.ColorTranslator.FromHtml("#4682B4");
            colors[22] = System.Drawing.ColorTranslator.FromHtml("#993333");
            colors[23] = System.Drawing.ColorTranslator.FromHtml("#FF99FF");

            colors[24] = System.Drawing.ColorTranslator.FromHtml("#DDA0DD");
            colors[25] = System.Drawing.ColorTranslator.FromHtml("#8B4513");
            colors[26] = System.Drawing.ColorTranslator.FromHtml("#CC0000");
            colors[27] = System.Drawing.ColorTranslator.FromHtml("#FFFF99");
            colors[28] = System.Drawing.ColorTranslator.FromHtml("#339900");
            colors[29] = System.Drawing.ColorTranslator.FromHtml("#FF9900");
            colors[30] = System.Drawing.ColorTranslator.FromHtml("#FFDAB9");
            colors[31] = System.Drawing.ColorTranslator.FromHtml("#2F4F4F");


        }
    }
}
