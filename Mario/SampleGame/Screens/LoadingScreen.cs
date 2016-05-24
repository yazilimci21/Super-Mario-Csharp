using Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Mario.SampleGame.Screens
{
    public class LoadingScreen : Core.Screen
    {
        int loadLevel = 0;
        int loading = 0;
        MarioGameScreen marioscreen;
        System.Threading.Thread dThread;

        public LoadingScreen(SizeF GameSize, float time, MainScreen MainScreen)
            : base(GameSize, time, MainScreen)
        {
            MainScreen.currentScreen = this;
            CreateMarioScreen();  
            LevelLoader.GameScreen = this;
            LevelLoader.LevelFiles = new string[] { Application.StartupPath + "\\maps\\map1.txt", Application.StartupPath + "\\maps\\map2.txt", Application.StartupPath + "\\maps\\map3.txt" };
        }

        private void LoadScreen()
        {
            if (loadLevel >= LevelLoader.LevelFiles.Length) return;
            LevelLoader.LoadLevel(loadLevel, new Size(32, 32), ref loading);
            System.Threading.Thread.Sleep(1000);
            loadLevel++;
            marioscreen.Materials = this.Materials;
            MainScreen.currentScreen = marioscreen;
        }

        public override void Draw(Graphics g)
        {
            LinearGradientBrush lbrush = new LinearGradientBrush(new Rectangle(0, 0, (int)ScreenSize.Width, (int)ScreenSize.Height),
                        Color.DarkSlateGray, Color.SteelBlue, 90.0f);
            g.FillRectangle(lbrush, 0, 0, ScreenSize.Width, ScreenSize.Height);
            if (loadLevel < LevelLoader.LevelFiles.Length)
            {
                if (loading > 0)
                {
                    DrawLoadingBar("Level " + (loadLevel + 1) + " Loading", g);
                }
                else
                {
                    DrawTable("Mario Keys", new string[] { "Start: S", "Exit: ESC", "Shoot: A", "Jump: Space" }, g);
                }
            }
            else
            {
                DrawTable("Thank you for playing 'Mario'", new string[] { "Play Again: F5", 
                    "Programming: Emrah KAYNAR", "Copy Right: www.emrahkaynar.com" }, g);
            }
        }

        private void FinalScreen()
        {
            loading = 0;
            MainScreen.currentScreen = this;
            CreateMarioScreen();
            for (int i = 0; i < Materials.Count; i++)
            {
                Materials[i].Dispose();
            }
            Materials.Clear();
            dThread = new System.Threading.Thread(new System.Threading.ThreadStart(LoadScreen));
            dThread.Start();
        }

        private void ExitScreen()
        {
            if (!MainScreen.currentScreen.isFinish)
            {
                loadLevel--;
            }
            loading = 0;
            CreateMarioScreen();
            MainScreen.currentScreen = this;
            for (int i = 0; i < Materials.Count; i++)
            {
                Materials[i].Dispose();
            }
            Materials.Clear();
        }

        private void LevelLoad()
        {
            dThread = new System.Threading.Thread(new System.Threading.ThreadStart(LoadScreen));
            dThread.Start();
        }

        public override void KeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S && loading == 0 && loadLevel < LevelLoader.LevelFiles.Length)
            {
                LevelLoad();
            }
            if (e.KeyCode == Keys.F5 && loadLevel >= LevelLoader.LevelFiles.Length)
            {
                loadLevel = 0;
                FinalScreen();
            }
        }

        private void CreateMarioScreen()
        {
            if (marioscreen != null)
            {
                marioscreen.Dispose();
                GC.SuppressFinalize(marioscreen);
            }
            marioscreen = new MarioGameScreen(this.ScreenSize, this.time, this.MainScreen);
            marioscreen.FinalHandler += new MethodInvoker(FinalScreen);
            marioscreen.ExitHandler += new MethodInvoker(ExitScreen);
        }

        private void DrawTable(string title,string[] Params, Graphics g)
        {
            Font font = new Font("Arial", 16.0f, FontStyle.Bold);
            SizeF textSize = g.MeasureString(title, font);
            float startw = 50, starth = 50, width = ScreenSize.Width - 100, height = ScreenSize.Height - 100;

            g.DrawRectangle(Pens.Black, new Rectangle((int)startw, (int)starth,
                (int)width, (int)height));

            g.DrawString(title, font, Brushes.Red, startw + (width / 2) - (textSize.Width / 2),
                starth - textSize.Height);

            List<string> Keys = new List<string>(Params);
            for (int i = 0; i < Keys.Count; i++)
            {
                textSize = g.MeasureString(Keys[i], font);
                g.DrawString(Keys[i], font, Brushes.Black, startw, starth + (textSize.Height * i));
                g.DrawLine(Pens.Black, startw, starth + (textSize.Height * (i + 1)), width + 50, starth + (textSize.Height * (i + 1)));
            }
        }

        private void DrawLoadingBar(string title, Graphics g)
        {
            Font font = new Font("Arial", 16.0f, FontStyle.Bold);
            SizeF textSize = g.MeasureString(title, font);
            g.DrawString(title, font, Brushes.Black, (int)(this.ScreenSize.Width / 2) - (textSize.Width / 2), (int)(this.ScreenSize.Height / 2) - (textSize.Height / 2) - 25);
            ControlPaint.DrawVisualStyleBorder(g, new Rectangle((int)(this.ScreenSize.Width / 2) - 100, (int)(this.ScreenSize.Height / 2) - 10, 200, 20));
            LinearGradientBrush brush = new LinearGradientBrush(new Rectangle((int)(this.ScreenSize.Width / 2) - 100, (int)(this.ScreenSize.Height / 2) - 10, 200, 20), Color.Yellow, Color.Black, LinearGradientMode.Vertical);
            g.FillRectangle(brush, (this.ScreenSize.Width / 2) - 100, (this.ScreenSize.Height / 2) - 10, loading * 2, 20);
        }
    }
}
