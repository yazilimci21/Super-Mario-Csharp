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

namespace Core
{
    public class BackGround : DisposableObject
    {
        public bool Scroll = true, Repeat = true;
        Image background;
        Size backgroundSize;
        PointF backgroundSpeed;

        public override void Disposed(bool dispose)
        {
            if(dispose)
            {
                if (background != null)
                {
                    background.Dispose();
                    GC.SuppressFinalize(background);
                }
            }
            base.Disposed(dispose);
        }

        public BackGround(Image background)
            : this(background, Size.Empty, PointF.Empty, true, true)
        {
        }

        public BackGround(Image background, Size backgroundSize)
            : this(background, backgroundSize, PointF.Empty, true, true)
        {
        }

        public BackGround(Image background, Size backgroundSize, PointF backgroundSpeed)
            : this(background, backgroundSize, backgroundSpeed, true, true)
        {
        }

        public BackGround(Image background, Size backgroundSize, PointF backgroundSpeed, bool Scroll)
            : this(background, backgroundSize, backgroundSpeed, Scroll, true)
        {
        }

        public BackGround(Image background, Size backgroundSize, PointF backgroundSpeed, bool Scroll,bool Repeat)
        {
            this.background = background;
            this.backgroundSize = backgroundSize;
            this.backgroundSpeed = backgroundSpeed;
            this.Scroll = Scroll;
            this.Repeat = Repeat;
        }

        public virtual void Draw(Graphics g,PointF CameraLocation,SizeF ScreenSize)
        {
            if (this.background != null)
            {
                float x = CameraLocation.X;
                float y = CameraLocation.Y;
                if (this.Scroll)
                {
                    if (this.backgroundSpeed.X == 0f)
                    {
                        this.backgroundSpeed.X = 1f;
                    }
                    x /= this.backgroundSpeed.X;
                    if (this.backgroundSpeed.Y == 0f)
                    {
                        this.backgroundSpeed.Y = 1f;
                    }
                    y /= this.backgroundSpeed.Y;
                }
                if (this.backgroundSize == Size.Empty)
                {
                    this.backgroundSize = new Size(this.background.Width, this.background.Height);
                }
                DrawXImage(g, x, y, ScreenSize);
            }

        }

        private void DrawXImage(Graphics g, float x, float y, SizeF ScreenSize)
        {
            float x0 = x;
            float len = 0f;
            float w = backgroundSize.Width;
            if (x0 < 0f)
            {
                len = x0 / -w;
                x0 += (w * (int)len);
            }
            else
            {
                len = x0 / w;
                x0 -= (w * (int)len);
            }
            if (x0 > 0f) x0 += -w;

            while (x0 < ScreenSize.Width)
            {
                if (((((x0 + this.backgroundSize.Width) > 0f) && ((y + this.backgroundSize.Height) > 0f)) && (x0 < ScreenSize.Width)) && (y < ScreenSize.Height))
                {
                    g.DrawImage(this.background, x0, y, (float)(this.backgroundSize.Width + 2), (float)this.backgroundSize.Height);
                }
                DrawYImage(g, x0, y, ScreenSize);
                x0 += this.backgroundSize.Width;
            }
        }

        private void DrawYImage(Graphics g, float x, float y, SizeF ScreenSize)
        {
            float y0 = y;
            float len = 0f;
            float h = backgroundSize.Height;
            if (y0 < 0f)
            {
                len = y0 / -h;
                y0 += (h * (int)len);
            }
            else
            {
                len = y0 / h;
                y0 -= (h * (int)len);
            }
            if (y0 > 0f) y0 -= h;

            while (y0 < ScreenSize.Height)
            {
                if (((((x + this.backgroundSize.Width) > 0f) && ((y0 + this.backgroundSize.Height) > 0f)) && (x < ScreenSize.Width)) && (y0 < ScreenSize.Height))
                {
                    g.DrawImage(this.background, x, y0, (float)(this.backgroundSize.Width + 2), (float)this.backgroundSize.Height);
                }
                y0 += this.backgroundSize.Height;
            }
        }
    }
}
