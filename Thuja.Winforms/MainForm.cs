using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Thuja.Winforms
{
    internal class MainForm : Form
    {
        internal bool clear;
        internal List<(Cooridantes, Style, string)> actions;

        public MainForm()
        {
            Controls.Add(new Label {Text = "Hello world!"});
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Application.Run(new MainForm());
        }

        internal static Rectangle CoordinatesToPixels(Cooridantes coordinates)
        {
            const int charWidth = 9;
            const int charHeight = 19;

            return new Rectangle(
                coordinates.Column * charWidth, coordinates.Row * charHeight,
                charWidth, charHeight
            );
        }

        internal static Rectangle GetRectangleForText(Cooridantes cooridantes, string text)
        {
            var start = MainForm.CoordinatesToPixels(cooridantes);
            var end = MainForm.CoordinatesToPixels(
                new Cooridantes(cooridantes.Column + text.Length, cooridantes.Row));
            var width = end.Right - start.Left;
            var height = end.Bottom - start.Top;
            return new Rectangle(start.Location, new Size(width, height));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (clear)
            {
                e.Graphics.Clear(Color.White);
            }

            foreach (var (coords, color, s) in actions)
            {
                var font = new Font(FontFamily.GenericMonospace, 9, GraphicsUnit.Pixel);
                var brush = new SolidBrush(Color.Black);
                e.Graphics.DrawString(s, font, brush, GetRectangleForText(coords, s));
            }

            base.OnPaint(e);
        }
    }
}