using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Core
{
    public class MaterialWithKeyAndMouse : Material
    {
        public class PlayerKey : DisposableObject
        {
            public enum ClickTypes { Press, Up, Down }

            public ClickTypes ClickType = ClickTypes.Press;
            public KeyEventArgs Key;
            public int BetweenClick = 0;

            public PlayerKey(ClickTypes ClickType, KeyEventArgs Key, int BetweenClick)
            {
                this.ClickType = ClickType;
                this.Key = Key;
                this.BetweenClick = BetweenClick;
            }
        }

        public class PlayerMouse : DisposableObject
        {
            public enum MouseTypes { Move, Up, Down, Click, DoubleClick }

            public MouseTypes MouseType = MouseTypes.Click;
            public MouseEventArgs Event;
            public int BetweenClick = 0;

            public PlayerMouse(MouseTypes MouseType, MouseEventArgs Event, int BetweenClick)
            {
                this.MouseType = MouseType;
                this.Event = Event;
                this.BetweenClick = BetweenClick;
            }
        }


        private DateTime lastKeyClicked = DateTime.Now, lastMouseClicked = DateTime.Now;
        public List<PlayerKey> PlayerKeyList = new List<PlayerKey>();
        public List<PlayerMouse> PlayerMouseList = new List<PlayerMouse>();
        public bool listenMouse = false, listedKey = false;

        public override void Disposed(bool dispose)
        {
            if (dispose)
            {
                for (int i = 0; i < PlayerKeyList.Count; i++)
                {
                    PlayerKeyList[i].Dispose();
                }
                for (int i = 0; i < PlayerMouseList.Count; i++)
                {
                    PlayerMouseList[i].Dispose();
                }
                GC.SuppressFinalize(PlayerMouseList);
                GC.SuppressFinalize(PlayerKeyList);
            }
            base.Disposed(dispose);
        }

        #region KeyAndMouse
        public virtual void KeyPress(KeyEventArgs e)
        {
            if (!listedKey) return;
            TimeSpan betweentime = DateTime.Now - lastKeyClicked;
            if(AddKey(new PlayerKey(PlayerKey.ClickTypes.Press, e, betweentime.Seconds)))
                lastKeyClicked = DateTime.Now;
        }

        public virtual void KeyUp(KeyEventArgs e)
        {
            if (!listedKey) return;
            TimeSpan betweentime = DateTime.Now - lastKeyClicked;
            if(AddKey(new PlayerKey(PlayerKey.ClickTypes.Up, e, betweentime.Seconds)))
                lastKeyClicked = DateTime.Now;
        }

        public virtual void KeyDown(KeyEventArgs e)
        {
            if (!listedKey) return;
            TimeSpan betweentime = DateTime.Now - lastKeyClicked;
            if(AddKey(new PlayerKey(PlayerKey.ClickTypes.Down, e, betweentime.Seconds)))
                lastKeyClicked = DateTime.Now;
        }

        public virtual void MouseDown(MouseEventArgs e)
        {
            if (!listenMouse) return;
            TimeSpan betweentime = DateTime.Now - lastMouseClicked;
            if(AddMouse(new PlayerMouse(PlayerMouse.MouseTypes.Down, e, betweentime.Seconds)))
                lastMouseClicked = DateTime.Now;
        }

        public virtual void MouseUp(MouseEventArgs e)
        {
            if (!listenMouse) return;
            TimeSpan betweentime = DateTime.Now - lastMouseClicked;
            if(AddMouse(new PlayerMouse(PlayerMouse.MouseTypes.Up, e, betweentime.Seconds)))
                lastMouseClicked = DateTime.Now;
        }

        public virtual void MouseMove(MouseEventArgs e)
        {
            if (!listenMouse) return;
            TimeSpan betweentime = DateTime.Now - lastMouseClicked;
            if(AddMouse(new PlayerMouse(PlayerMouse.MouseTypes.Move, e, betweentime.Seconds)))
                lastMouseClicked = DateTime.Now;
        }

        public virtual void MouseClick(MouseEventArgs e)
        {
            if (!listenMouse) return;
            TimeSpan betweentime = DateTime.Now - lastMouseClicked;
            if(AddMouse(new PlayerMouse(PlayerMouse.MouseTypes.Click, e, betweentime.Seconds)))
                lastMouseClicked = DateTime.Now;
        }

        public virtual void MouseDoubleClick(MouseEventArgs e)
        {
            if (!listenMouse) return;
            TimeSpan betweentime = DateTime.Now - lastMouseClicked;
            if(AddMouse(new PlayerMouse(PlayerMouse.MouseTypes.DoubleClick, e, betweentime.Seconds)))
                lastMouseClicked = DateTime.Now;
        }
        #endregion

        private bool AddKey(PlayerKey playkey)
        {
            if (PlayerKeyList.Count == 0)
                PlayerKeyList.Add(playkey);
            else
            {
                if(PlayerKeyList[PlayerKeyList.Count-1].ClickType == playkey.ClickType &&
                    PlayerKeyList[PlayerKeyList.Count-1].Key == playkey.Key)
                    return false;
                else
                    PlayerKeyList.Add(playkey);
            }
            return true;
        }

        private bool AddMouse(PlayerMouse playmouse)
        {
            if (PlayerMouseList.Count == 0)
                PlayerMouseList.Add(playmouse);
            else
            {
                if (PlayerMouseList[PlayerMouseList.Count - 1].MouseType == playmouse.MouseType &&
                    PlayerMouseList[PlayerMouseList.Count - 1].Event == playmouse.Event)
                    return false;
                else
                    PlayerMouseList.Add(playmouse);
            }
            return true;
        }
    }
}
