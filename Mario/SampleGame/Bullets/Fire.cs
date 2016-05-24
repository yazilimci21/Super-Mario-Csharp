using System;
using System.Drawing;
using System.Windows.Forms;

namespace Mario.SampleGame.Bullets
{
    public class Fire : Core.Material
    {
        private string Baddiepath = Application.StartupPath + "\\Images\\baddies\\";
        Core.Animation.Animator animator = new Core.Animation.Animator();
        int i = 0;
        float maxjump = -2.0f, nowjump = 0.0f;

        public Fire()
        {
            this.Properties.GravityValue = 0.28f;
            Image[] image = new Image[] { ImageLoader.LoadImage(Baddiepath + "Fireball_1.png"), 
                ImageLoader.LoadImage(Baddiepath + "Fireball_2.png"), ImageLoader.LoadImage(Baddiepath + "Fireball_3.png"),
                ImageLoader.LoadImage(Baddiepath + "Fireball_4.png") };
            for (int i = 0; i < image.Length; i++)
            {
                animator.AddFrame(image[i], @"", new SizeF(image[i].Width * 1.5f, image[i].Height * 1.5f));
            }
            this.AnimatorList.Add(animator);
            setAnimation(0);
            this.Properties.CollisionRect.Size = AnimatorList[0].AnimSize;
            this.Properties.VelocityRect.Y = 0.18f;
        }

        public override void Update(float time, PointF CameraLocation)
        {
            nowjump += this.Properties.VelocityRect.Y;
            if (nowjump < maxjump)
                this.Properties.VelocityRect.Y = -this.Properties.VelocityRect.Y;
            if (i < 60) i++;
            else this.Properties.isALive = false;
        }

        public override bool OtherCollided(Core.Sprite.Sprite sprite, Core.Sprite.SpriteProperties.Directions collidedDirection, PointF CameraLocation)
        {
            return true;
        }

        public override void WeCollided(Core.Sprite.Sprite sprite, Core.Sprite.SpriteProperties.Directions collidedDirection, PointF CameraLocation)
        {
            if (sprite.Properties.isTile)
            {
                if (collidedDirection == Core.Sprite.SpriteProperties.Directions.Up || collidedDirection == Core.Sprite.SpriteProperties.Directions.Down)
                {
                    this.Properties.VelocityRect.Y = -this.Properties.VelocityRect.Y;
                }
                if (collidedDirection == Core.Sprite.SpriteProperties.Directions.Left || collidedDirection == Core.Sprite.SpriteProperties.Directions.Right)
                {
                    this.Properties.isALive = false;
                }
            }
        }

        public override void NoCollided()
        {
            
        }
    }
}
