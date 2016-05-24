using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace Core.Sprite
{
    public class SpriteReader : DisposableObject
    {
        public Color transparentColor = Color.Black;
        public List<List<Bitmap>> Images = new List<List<Bitmap>>();
        public float spriteWidth, spriteHeight;

        public override void Disposed(bool dispose)
        {
            if(dispose)
            {
                for (int x = 0; x < Images.Count; x++)
                {
                    for (int y = 0; y < Images[x].Count; y++)
                    {
                        Images[x][y].Dispose();
                        GC.SuppressFinalize(Images[x][y]);
                    }
                }
                GC.SuppressFinalize(Images);
            }
            base.Disposed(dispose);
        }

        public void GetTransparentColor(Bitmap image, int x, int y)
        {
            transparentColor = image.GetPixel(x, y);
        }

        public void ReadImage(Bitmap image, float maxReadX, float maxReadY, float xspace, float yspace)
        {
            image.MakeTransparent(transparentColor);
            float w = image.Width % spriteWidth;
            if (maxReadX > 0f) w = maxReadX;
            float wi = 0;
            float h = image.Height % spriteHeight;
            if (maxReadY > 0f) h = maxReadY;
            float hi = 0;
            while (hi < h)
            {
                if (Images.Count == hi) Images.Add(new List<Bitmap>());
                while (wi < w)
                {
                    Images[(int)hi].Add(image.Clone(new Rectangle((int)((wi * spriteWidth)+xspace), (int)((hi * spriteHeight)+yspace), (int)spriteWidth, (int)spriteHeight), PixelFormat.Format32bppArgb));
                    wi++;
                }
                wi = 0;
                hi++;
            }
        }

        public Bitmap CombineXImages(int spriteIndex, params int[] index)
        {
            Bitmap bmp = new Bitmap((int)(spriteWidth * index.Length), (int)spriteHeight);
            Graphics grp = Graphics.FromImage(bmp);
            for (int i = 0; i < index.Length;i++)
            {
                grp.DrawImage(Images[spriteIndex][index[i]], i * spriteWidth, spriteHeight);
            }
            return bmp;
        }

        public Bitmap CombineYImages(int spriteIndex, params int[] index)
        {
            Bitmap bmp = new Bitmap((int)spriteWidth, (int)(spriteHeight * index.Length));
            Graphics grp = Graphics.FromImage(bmp);
            for (int i = 0; i < index.Length; i++)
            {
                grp.DrawImage(Images[index[i]][spriteIndex], spriteWidth, i * spriteHeight);
            }
            return bmp;
        }

        public void findSprites(Bitmap image, Point start)
        {
            image.MakeTransparent(transparentColor);
            Rectangle lastRectangle = new Rectangle(start, new Size(0, 0));
            int i = 0;
            while (true)
            {
                try
                {
                    Rectangle temprect = findImageRect0(image, lastRectangle.Location);

                    if (temprect.Width > 1 && temprect.Height > 1)
                    {
                        if (Images.Count == i) Images.Add(new List<Bitmap>());
                        Images[i].Add(image.Clone(temprect, System.Drawing.Imaging.PixelFormat.Format32bppArgb));
                        lastRectangle = temprect;
                        if ((lastRectangle.X + lastRectangle.Width) < image.Width)
                        {
                            lastRectangle.X += lastRectangle.Width + 1;
                        }
                        else if ((lastRectangle.Y + lastRectangle.Height) < image.Height)
                        {
                            lastRectangle.Y += lastRectangle.Height + 1;
                            i++;
                        }
                        else break;
                    }
                    else if (temprect.X >= image.Width && temprect.Y < image.Height-1)
                    {
                        lastRectangle.Y += lastRectangle.Height + 1;
                        i++;
                    }
                    else if (temprect.Y <= image.Height-1)
                        break;
                }
                catch { break; }
            }
        }

        private Rectangle findImageRect0(Bitmap image, Point start)
        {
            Rectangle rect = Rectangle.Empty;
            while (true)
            {
                Rectangle temprect = findImageRect(image, start);
                if (rect == temprect)
                {
                    break;
                }
                else
                {
                    rect = temprect;
                }
            }
            return rect;
        }

        private Rectangle findImageRect(Bitmap image, Point start)
        {
            int ix = start.X, width = 0, iy = start.Y, height = 0;
            int first = 0;
            bool next = false;
            for (int x = start.X; x < image.Width; x++)
            {
                for (int y = start.Y; y < image.Height; y++)
                {
                    Color clr = image.GetPixel(x, y);
                    if (clr == Color.FromArgb(0, 0, 0, 0))
                    {
                        next = true;
                    }
                    else
                    {
                        next = false;
                        break;
                    }
                }
                if (next)
                {
                    if (first == 0)
                        ix = x;
                    else if (first == 1)
                        width = x;
                }
                else
                {
                    first = 1;
                }
                if (first == 1 && width > 0) break;
            }
            next = false;
            first = 0;
            for (int y = start.Y; y < image.Height; y++)
            {
                for (int x = start.X; x < image.Width; x++)
                {
                    Color clr = image.GetPixel(x, y);
                    if (clr == Color.FromArgb(0, 0, 0, 0))
                    {
                        next = true;
                    }
                    else
                    {
                        next = false;
                        break;
                    }
                }
                if(next)
                {
                    if (first == 0)
                        iy = y;
                    else if (first == 1)
                        height = y;
                }
                else
                {
                    first = 1;
                }
                if (first == 1 && height > 0) break;
            }
            return new Rectangle(ix, iy, width - ix, height - iy);
        }
    }
}
