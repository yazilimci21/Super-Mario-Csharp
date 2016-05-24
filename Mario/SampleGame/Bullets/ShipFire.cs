using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Mario.SampleGame.Bullets
{
    public class ShipFire : Core.Material
    {
        private string Imagepath = Application.StartupPath + "\\Images\\";
        Core.Animation.Animator animator = new Core.Animation.Animator();
        int i = 0;

        public ShipFire()
        {
            this.Properties.isRotate = true;
            Image fireImage = ImageLoader.LoadImage(Imagepath + "fire.png");
            animator.AddFrame(fireImage, @"", new SizeF(fireImage.Width, fireImage.Height));
            this.AnimatorList.Add(animator);
            setAnimation(0);
            this.Properties.CollisionRect.Size = AnimatorList[0].AnimSize;
        }
        public override void Update(float time, PointF CameraLocation)
        {
            i++;
            if (i > 200) this.Properties.isALive = false;
        }

        public override void WeCollided(Core.Sprite.Sprite sprite, Core.Sprite.SpriteProperties.Directions collidedDirection, PointF CameraLocation)
        {
            if (sprite.GetType() == typeof(Bullets.ShipFire) || sprite.GetType() == typeof(Guns.Ship))
                return;
            if (sprite.Properties.isTile || sprite.Properties.isPlayer)
            {
                this.Properties.isALive = false;
            }
        }

        public override bool OtherCollided(Core.Sprite.Sprite sprite, Core.Sprite.SpriteProperties.Directions collidedDirection, PointF CameraLocation)
        {
            if (sprite.GetType() == typeof(Bullets.ShipFire) || sprite.GetType() == typeof(Guns.Ship)) return false;
            if (sprite.Properties.isTile || sprite.Properties.isPlayer)
            {
                return this.Properties.isALive = false;
            }
            return false;
        }
    }
}
