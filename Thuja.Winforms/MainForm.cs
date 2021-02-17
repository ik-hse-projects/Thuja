using System;
using System.Windows.Forms;

namespace Thuja.Winforms
{
    internal class MainForm : Form
    {
        public MainForm()
        {
            Controls.Add(new Label {Text = "Hello world!"});
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Application.Run(new MainForm());
        }
    }
}