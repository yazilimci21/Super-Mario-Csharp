using Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Mario.SampleGame.Materials
{
    public class Platform : Material
    {
        Core.Animation.Animator animator = new Core.Animation.Animator();
        private string Itempath = Application.StartupPath + "\\Images\\items\\";

        public Platform()
        {
            this.Properties.isTile = true;
            animator.duractionTime = 200;
            Image platformImage = ImageLoader.LoadImage(Itempath + "Red_Platform_2.png");
            animator.AddFrame(platformImage, @"", new SizeF(platformImage.Width * 1.5f, platformImage.Height * 1.5f));
            this.AnimatorList.Add(animator);
            setAnimation(0);
            this.Properties.CollisionRect.Size = AnimatorList[0].AnimSize;
        }

        public override bool OtherCollided(Core.Sprite.Sprite sprite, Core.Sprite.SpriteProperties.Directions collidedDirection,PointF CameraLocation)
        {
            if (collidedDirection == Core.Sprite.SpriteProperties.Directions.Down)
            {
                if (sprite.Properties.VelocityRect.Y < 0) sprite.Properties.VelocityRect.Y = -sprite.Properties.VelocityRect.Y;
            }
            return base.OtherCollided(sprite, collidedDirection, CameraLocation);
        }
    }
}
