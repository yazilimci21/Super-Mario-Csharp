using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Core
{
    public class MainScreen : DisposableObject
    {
        public Screen currentScreen;
        [DllImport("psapi.dll")]
        public static extern bool EmptyWorkingSet(IntPtr hProcess);

        public override void Disposed(bool dispose)
        {
            if (dispose)
            {
                if (currentScreen != null)
                {
                    currentScreen.Dispose();
                }
            }
            base.Disposed(dispose);
        }

        public void Draw(Graphics g)
        {
            EmptyWorkingSet(Process.GetCurrentProcess().Handle);
            if (currentScreen != null)
            {
                currentScreen.Draw(g);
            }
        }

        #region KeyAndMouse
        public void KeyPress(KeyEventArgs e)
        {
            if(currentScreen != null)
            {
                currentScreen.KeyPress(e);
            }
        }

        public void KeyUp(KeyEventArgs e)
        {
            if (currentScreen != null)
            {
                currentScreen.KeyUp(e);
            }
        }

        public void KeyDown(KeyEventArgs e)
        {
            if (currentScreen != null)
            {
                currentScreen.KeyDown(e);
            }
        }

        public virtual void MouseDown(MouseEventArgs e)
        {
            if (currentScreen != null)
            {
                currentScreen.MouseDown(e);
            }
        }

        public virtual void MouseUp(MouseEventArgs e)
        {
            if (currentScreen != null)
            {
                currentScreen.MouseUp(e);
            }
        }

        public virtual void MouseMove(MouseEventArgs e)
        {
            if (currentScreen != null)
            {
                currentScreen.MouseMove(e);
            }
        }

        public virtual void MouseClick(MouseEventArgs e)
        {
            if (currentScreen != null)
            {
                currentScreen.MouseClick(e);
            }
        }

        public virtual void MouseDoubleClick(MouseEventArgs e)
        {
            if (currentScreen != null)
            {
                currentScreen.MouseDoubleClick(e);
            }
        }
        #endregion
    }
}
