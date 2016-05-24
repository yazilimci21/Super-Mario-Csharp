using System;
using System.Drawing;
using System.Windows.Forms;

namespace Mario.SampleGame.Bullets
{
    public class Bullet : Core.Material
    {
        private string Imagepath = Application.StartupPath + "\\Images\\";
        Core.Animation.Animator animator = new Core.Animation.Animator();
        int i = 0;

        public Bullet()
        {
            Image bulletImage = ImageLoader.LoadImage(Imagepath + "bullet.png");
            animator.AddFrame(bulletImage, @"", new SizeF(bulletImage.Width, bulletImage.Height));
            this.AnimatorList.Add(animator);
            setAnimation(0);
            this.Properties.CollisionRect.Size = AnimatorList[0].AnimSize;
            this.Properties.Direction = Core.Sprite.SpriteProperties.Directions.Left;
        }

        public override void Update(float time, PointF CameraLocation)
        {
            if (this.Properties.VelocityRect.X < 0 && this.Properties.Direction == Core.Sprite.SpriteProperties.Directions.Left)
            {
                FlipY();
                this.Properties.Direction = Core.Sprite.SpriteProperties.Directions.Right;
            }
            if (i < 60) i++;
            else this.Properties.isALive = false;
        }

        public override void WeCollided(Core.Sprite.Sprite sprite, Core.Sprite.SpriteProperties.Directions collidedDirection, PointF CameraLocation)
        {
            if (sprite.Properties.isTile)
            {
                this.Properties.isALive = false;
            }
        }

        public override bool OtherCollided(Core.Sprite.Sprite sprite, Core.Sprite.SpriteProperties.Directions collidedDirection, PointF CameraLocation)
        {
            if (sprite.Properties.isPlayer)
            {
                return base.OtherCollided(this, collidedDirection, CameraLocation);
            }
            else
            {
                return false;
            }
        }
    }
}
