using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Core.Animation
{
    public class Animation : DisposableObject
    {
        public Image Anim;
        public SizeF AnimSize { get; set; }
        public float duractionTime = 125;
        public string sound = "";
        public bool isPauseAnim = false;

        public override void Disposed(bool dispose)
        {
            if (dispose)
            {
                if (Anim != null)
                {
                    Anim.Dispose();
                    GC.SuppressFinalize(Anim);
                }
            }
            base.Disposed(dispose);
        }

        public Animation()
        {

        }

        public Animation(Image image)
            : this(image, 125, false, 0, 0)
        {
        }

        public Animation(Image image, int duractionTime)
            : this(image, duractionTime, false, 0, 0)
        {

        }

        public Animation(Image image, bool isPauseAnim)
            : this(image, 125, isPauseAnim, 0, 0)
        {
        }

        public Animation(Image image, int duractionTime, bool isPauseAnim, int width, int height)
        {
            this.Anim = image;
            if ((width + height) == 0 | width == 0 || height == 0) AnimSize = new Size(image.Width, image.Height);
            else AnimSize = new Size(width, height);
            this.isPauseAnim = isPauseAnim;
            this.duractionTime = duractionTime;
        }
    }
}
