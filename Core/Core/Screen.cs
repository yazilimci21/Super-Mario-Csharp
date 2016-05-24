/*
 * GameCore 0.1
 * 
 * Copyright (c) 2015 Emrah KAYNAR
 * 
 * Author : Emrah KAYNAR
 * Email : yazilimci21@gmail.com - info@emrahkaynar.com
 * Tel : +(90)539-607-06-94
 * Site : http://www.emrahkaynar.com < Update versions
 * Date: 01.01.2015
 * 
 * Description :
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the author be held liable for any damages arising from 
 * the use of this software.
 * Permission to use, copy, modify, distribute and sell this software for any 
 * purpose is hereby granted without fee, provided that the above copyright 
 * notice appear in all copies and that both that copyright notice and this 
 * permission notice appear in supporting documentation.
 * 
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Core
{
    public class Screen : DisposableObject
    {
        public List<BackGround> BackGrounds = new List<BackGround>();
        public List<Material> Materials = new List<Material>();
        public PointF CameraLocation = new PointF(0f, 0f);
        public float time = 0f;
        public SizeF ScreenSize = new SizeF(800, 400);
        public RectangleF GameRect = new RectangleF(0, 0, 0, 0);
        public bool Pause = false, isFinish = false, GameOver = false, MoveCameraOnly = false;
        public Keys PauseButton = Keys.P, ExitButton = Keys.Escape;
        public event MethodInvoker FinalHandler, ExitHandler;
        public MainScreen MainScreen;

        public override void Disposed(bool dispose)
        {
            if (dispose)
            {
                for(int i=0;i<BackGrounds.Count;i++)
                {
                    BackGrounds[i].Dispose();
                }
                for (int i = 0; i < Materials.Count; i++)
                {
                    Materials[i].Dispose();
                }
            }
            base.Disposed(dispose);
        }

        public Screen(SizeF GameSize, float time, MainScreen MainScreen)
        {
            this.MainScreen = MainScreen;
            ScreenSize = new SizeF(GameSize.Width, GameSize.Height);
            if (penSize == 0) penSize = ScreenSize.Height;
            this.time = time;
        }

        private bool Start = false, setAllSettings = false;
        private float penSize = 0;

        private void GetGameRect()
        {
            if (setAllSettings) return;
            setAllSettings = true;
            float x = 0, y = 0, width = 0, height = 0;
            for (int i = 0; i < Materials.Count; i++)
            {
                if (Materials[i].Properties.isALive)
                {
                    RectangleF matrect = findMaterialCollidedRect(Materials[i]);
                    if (matrect.Width > width) width = matrect.Width;
                    if (matrect.Height > height) height = matrect.Height;
                    if (matrect.X < x) x = matrect.X;
                    if (matrect.Y < y) y = matrect.Y;
                }
                else
                {
                    Materials[i].Dispose();
                    GC.SuppressFinalize(Materials[i]);
                    Materials.RemoveAt(i);
                    i--;
                }
            }
            if (width != GameRect.Width && GameRect.Width == 0) GameRect.Width = width;
            if (height != GameRect.Height && GameRect.Height == 0) GameRect.Height = height;
            if (x != GameRect.X && GameRect.X == 0) GameRect.X = x;
            if (y != GameRect.Y && GameRect.Y == 0) GameRect.Y = y;
        }

        private void setCameraOnPlayer()
        {
            Player player = null;
            for (int i = 0; i < Materials.Count; i++)
            {
                if (Materials[i].Properties.isPlayer && player == null)
                {
                    player = (Player)Materials[i];
                    break;
                }
            }
            if (player == null || MoveCameraOnly)
            {
                if(player != null)
                {
                    CameraLocation.X = -(player.Properties.CollisionRect.X - (ScreenSize.Width / 2));
                    CameraLocation.Y = -(player.Properties.CollisionRect.Y - (ScreenSize.Height / 2));
                }
                return;
            }
            if (player.Properties.CollisionRect.X > ScreenSize.Width)
            {
                if (player.Properties.CollisionRect.X < (GameRect.Width - (ScreenSize.Width / 2)))
                {
                    CameraLocation.X = -(player.Properties.CollisionRect.X - (ScreenSize.Width / 2));
                }
                else
                {
                    CameraLocation.X = -(GameRect.Width - ScreenSize.Width);
                }
            }
            else
            {
                if (player.Properties.CollisionRect.X < (ScreenSize.Width / 2))
                {
                    CameraLocation.X = GameRect.X;
                }
                else
                {
                    CameraLocation.X = -(player.Properties.CollisionRect.X - (ScreenSize.Width / 2));
                }
            }
            if (player.Properties.CollisionRect.Y > ScreenSize.Height)
            {
                if (player.Properties.CollisionRect.Y < (GameRect.Height - (ScreenSize.Height / 2)))
                {
                    CameraLocation.Y = -(player.Properties.CollisionRect.Y - (ScreenSize.Height / 2));
                }
                else
                {
                    CameraLocation.Y = -(GameRect.Height - ScreenSize.Height);
                }
            }
            else
            {
                if (player.Properties.CollisionRect.Y < (ScreenSize.Height / 2))
                {
                    CameraLocation.Y = GameRect.Y;
                }
                else
                {
                    CameraLocation.Y = -(player.Properties.CollisionRect.Y - (ScreenSize.Height / 2));
                }
            }
        }

        private bool isCollision(RectangleF collided, RectangleF affected)
        {
            return (collided.X < (affected.X + affected.Width) && (collided.X + collided.Width) > affected.X &&
                collided.Y < (affected.Y + affected.Height) && (collided.Y + collided.Height) > affected.Y);
        }

        public virtual void Draw(Graphics g)
        {
            //Bitmap bmp = new Bitmap((int)ScreenSize.Width, (int)ScreenSize.Height);
            //Graphics g = Graphics.FromImage(bmp);

            GetGameRect();
            setCameraOnPlayer();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            for (int i = 0; i < BackGrounds.Count; i++)
            {
                BackGrounds[i].Draw(g, CameraLocation, ScreenSize);
            }
            for (int i = 0; i < Materials.Count; i++)
            {
                if (Materials[i].Properties.isALive)
                {
                    RectangleF colRect = Materials[i].Properties.CollisionRect;
                    colRect.X += CameraLocation.X;
                    colRect.Y += CameraLocation.Y;
                    if (!isCollision(colRect, new RectangleF(-(ScreenSize.Width / 2), -(ScreenSize.Height / 2), ScreenSize.Width * 2, ScreenSize.Height * 2)) && !Materials[i].Properties.isPlayer) continue;
                    Materials[i].Properties.GameScreen = this;
                    if (!Pause && !isFinish && Start)
                    {
                        Materials[i].Update(time, CameraLocation);
                        Materials[i].UpdateStep(time, CameraLocation);
                        Materials[i].UpdateCollisions(time, CameraLocation);
                    }
                    if (!isCollision(colRect, new RectangleF(0, 0, ScreenSize.Width, ScreenSize.Height))) continue;
                    Materials[i].Draw(g, CameraLocation);
                }
                else
                {
                    Materials.RemoveAt(i);
                    i--;
                }
            }
            if (!isFinish && !Start)
            {
                penSize-=10;
                LinearGradientBrush lbrush = new LinearGradientBrush(new Rectangle(0, 0, (int)ScreenSize.Width, (int)ScreenSize.Height),
                    Color.DarkSlateGray, Color.SteelBlue, 90.0f);

                Pen pen = new Pen(lbrush, penSize);
                g.DrawRectangle(pen, 0, 0, ScreenSize.Width, ScreenSize.Height);
                if (penSize <= 0) Start = true;
            }
            if(isFinish && Start)
            {
                penSize += 10;
                LinearGradientBrush lbrush = new LinearGradientBrush(new Rectangle(0, 0, (int)ScreenSize.Width, (int)ScreenSize.Height),
                    Color.DarkSlateGray, Color.SteelBlue, 90.0f);
                Pen pen = new Pen(lbrush, penSize);
                g.DrawRectangle(pen, 0, 0, ScreenSize.Width, ScreenSize.Height);
                if (penSize >= ScreenSize.Height)
                {
                    if (FinalHandler != null)
                        FinalHandler();
                }
            }

            //grp.DrawImage(bmp, 0, 0,ScreenSize.Width, ScreenSize.Height);
            //bmp.Dispose();
        }

        public void Reset()
        {
            Start = false;
            setAllSettings = false;
            penSize = ScreenSize.Height;
            if (ExitHandler != null)
                ExitHandler();
        }

        #region KeyAndMouse
        public virtual void KeyPress(KeyEventArgs e)
        {
            for (int i = 0; i < Materials.Count; i++)
            {
                if (findType(Materials[i], typeof(MaterialWithKeyAndMouse)))
                    ((Core.MaterialWithKeyAndMouse)Materials[i]).KeyPress(e);
            }
        }

        public virtual void KeyUp(KeyEventArgs e)
        {
            for (int i = 0; i < Materials.Count; i++)
            {
                if (findType(Materials[i], typeof(MaterialWithKeyAndMouse)))
                    ((Core.MaterialWithKeyAndMouse)Materials[i]).KeyUp(e);
            }
        }

        public virtual void KeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == PauseButton)
            {
                Pause = !Pause;
                for (int i = 0; i < Materials.Count; i++)
                {
                    if (Materials[i].Properties.isPlayer) Materials[i].Properties.VelocityRect.X = 0;
                    if (findType(Materials[i], typeof(MaterialWithKeyAndMouse)))
                    {
                        ((Core.MaterialWithKeyAndMouse)Materials[i]).PlayerMouseList.Clear();
                        ((Core.MaterialWithKeyAndMouse)Materials[i]).PlayerKeyList.Clear();
                    }
                }
            }
            if (e.KeyCode == ExitButton)
            {
                Start = false;
                setAllSettings = false;
                penSize = ScreenSize.Height;
                if (ExitHandler != null)
                    ExitHandler();
            }
            for (int i = 0; i < Materials.Count; i++)
            {
                if (findType(Materials[i], typeof(MaterialWithKeyAndMouse)))
                    ((Core.MaterialWithKeyAndMouse)Materials[i]).KeyDown(e);
            }
        }

        public virtual void MouseDown(MouseEventArgs e)
        {
            for (int i = 0; i < Materials.Count; i++)
            {
                if (findType(Materials[i], typeof(MaterialWithKeyAndMouse)))
                    ((Core.MaterialWithKeyAndMouse)Materials[i]).MouseDown(e);
            }
        }

        public virtual void MouseUp(MouseEventArgs e)
        {
            for (int i = 0; i < Materials.Count; i++)
            {
                if (findType(Materials[i], typeof(MaterialWithKeyAndMouse)))
                    ((Core.MaterialWithKeyAndMouse)Materials[i]).MouseUp(e);
            }
        }

        public virtual void MouseMove(MouseEventArgs e)
        {
            for (int i = 0; i < Materials.Count; i++)
            {
                if (findType(Materials[i], typeof(MaterialWithKeyAndMouse)))
                    ((Core.MaterialWithKeyAndMouse)Materials[i]).MouseMove(e);
            }
        }

        public virtual void MouseClick(MouseEventArgs e)
        {
            for (int i = 0; i < Materials.Count; i++)
            {
                if (findType(Materials[i], typeof(MaterialWithKeyAndMouse)))
                    ((Core.MaterialWithKeyAndMouse)Materials[i]).MouseClick(e);
            }
        }

        public virtual void MouseDoubleClick(MouseEventArgs e)
        {
            for (int i = 0; i < Materials.Count; i++)
            {
                if (findType(Materials[i], typeof(MaterialWithKeyAndMouse)))
                    ((Core.MaterialWithKeyAndMouse)Materials[i]).MouseDoubleClick(e);
            }
        }
        #endregion

        private bool findType(object obj, Type finding)
        {
            Type type = obj.GetType();
            if (type == finding) return true;
            while (type != null)
            {
                if (type == finding) return true;
                type = type.BaseType;
            }
            return false;
        }

        private RectangleF findMaterialCollidedRect(Material material)
        {
            float x = 0, y = 0, width = 0, height = 0;
            
            x = width = material.Properties.CollisionRect.X;
            y = height = material.Properties.CollisionRect.Y;
            width += material.Properties.CollisionRect.Width;
            height += material.Properties.CollisionRect.Height;

            return new RectangleF(x, y, width, height);
        }
    }
}
