using Core;
using System.Drawing;
using System.Windows.Forms;

namespace Mario.SampleGame.Screens
{
    public class MarioGameScreen : Core.Screen
    {
        public MarioGameScreen(SizeF GameSize, float time, MainScreen MainScreen) : base(GameSize, time, MainScreen)
        {
            BackGrounds.Add(new Core.BackGround(ImageLoader.LoadImage(Application.StartupPath + "\\Images\\backgrounds\\Icy_Background.png"),
                new Size((int)ScreenSize.Width, (int)(ScreenSize.Height*1.3f)), new PointF(4, 4), true));
        }
    }
}
