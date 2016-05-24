using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace Mario
{
    public partial class Form1 : Form
    {
        Core.MainScreen mainscreen = new Core.MainScreen();
        private System.Threading.Timer gamerunner;
        private float period = 20;

        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            mainscreen.currentScreen = new SampleGame.Screens.LoadingScreen(new SizeF(pictureBox1.Width, pictureBox1.Height), 20, mainscreen);
            mainscreen.currentScreen.time = period;
            gamerunner = new System.Threading.Timer(run, null, 1, -1);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                mainscreen.Draw(e.Graphics);
            }
            catch { }
        }

        private void run(object state)
        {
            gamerunner.Change(-1, -1);
            pictureBox1.Refresh();
            gamerunner.Change((int)period, -1);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            mainscreen.KeyDown(e);
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyEventArgs ev = new KeyEventArgs((Keys)e.KeyChar);
            mainscreen.KeyPress(ev);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            mainscreen.KeyUp(e);
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Process.Start("http://www.emrahkaynar.com");
            }
        }
    }
}
