using Core;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Mario.SampleGame.Materials
{
    public class MovablePlatform : Material
    {
        Core.Animation.Animator animator = new Core.Animation.Animator();
        private string Itempath = Application.StartupPath + "\\Images\\items\\";

        public PointF MaxPoint = new PointF(0f, 0f), VelocityVal = new PointF(0f, 0f);
        private PointF Point = new PointF(0f, 0f);

        public MovablePlatform()
        {
            this.Properties.isTile = true;
            this.MaxPoint = new PointF(2f, 2f);
            this.VelocityVal = new PointF(0.10f, 0.10f);
            this.Properties.isTile = true;
            animator.duractionTime = 200;
            Image platformImage = ImageLoader.LoadImage(Itempath + "Red_Platform_2.png");
            animator.AddFrame(platformImage, @"", new SizeF(platformImage.Width * 1.5f, platformImage.Height * 1.5f));
            this.AnimatorList.Add(animator);
            setAnimation(0);
            this.Properties.CollisionRect.Size = AnimatorList[0].AnimSize;
        }

        public override void Update(float time, PointF CameraLocation)
        {
            if (this.Point.Y > MaxPoint.Y || this.Point.Y < -MaxPoint.Y)
            {
                VelocityVal.Y = -VelocityVal.Y;
            }
            if (this.Point.X > MaxPoint.X || this.Point.X < -MaxPoint.X)
            {
                VelocityVal.X = -VelocityVal.X;
            }
            this.Point.X += VelocityVal.X;
            this.Point.Y += VelocityVal.Y;
            Properties.VelocityRect.Y = VelocityVal.Y;
            Properties.VelocityRect.X = VelocityVal.X;
        }

        public override bool OtherCollided(Core.Sprite.Sprite sprite, Core.Sprite.SpriteProperties.Directions collidedDirection, PointF CameraLocation)
        {
            if (collidedDirection == Core.Sprite.SpriteProperties.Directions.Down)
            {
                if (sprite.Properties.VelocityRect.Y < 0) sprite.Properties.VelocityRect.Y = -sprite.Properties.VelocityRect.Y;
            }
            if (collidedDirection == Core.Sprite.SpriteProperties.Directions.Up)
            {
                sprite.Properties.CollisionRect.X += (VelocityVal.X * this.Properties.GameScreen.time);
            }
            return base.OtherCollided(sprite, collidedDirection, CameraLocation);
        }
    }
}
