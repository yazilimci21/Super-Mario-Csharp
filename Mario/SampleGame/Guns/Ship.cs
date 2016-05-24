using Core.Sprite;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Mario.SampleGame.Guns
{
    public class Ship : Core.MaterialWithKeyAndMouse
    {
        Core.Animation.Animator animator = new Core.Animation.Animator();
        private string Imagepath = Application.StartupPath + "\\Images\\";
        Random rnd = new Random();
        private bool isdie = false;

        public Ship()
        {
            listedKey = true;
            this.Properties.isCollisionable = false;
            this.Properties.isRotate = true;
            animator.duractionTime = 200;
            Image shipImage = ImageLoader.LoadImage(Imagepath + "Ship.png");
            animator.AddFrame(shipImage, @"", new SizeF(shipImage.Width, shipImage.Height));
            this.AnimatorList.Add(animator);
            setAnimation(0);
            this.Properties.CollisionRect.Size = AnimatorList[0].AnimSize;
        }

        private float dietime = 0;
        public override void Update(float time, PointF CameraLocation)
        {
            if (isdie)
            {
                if (this.Properties.Alpha == 0) this.Properties.Alpha = 200;
                else this.Properties.Alpha = 0;
                dietime += 1;
                if (dietime > 20)
                {
                    dietime = 0;
                    isdie = false;
                }
            }
            else
            {
                this.Properties.Alpha = 0;
            }
            this.Properties.RotateAngle += 1;
            List<int> shootPositions = new List<int>() { 15, 30, 45, 60, 75, 90, 105, 120, 135, 150,
            165, 180, 195, 210, 225, 240, 255, 270, 285, 300, 315, 330, 345, 360 };
            if (shootPositions.IndexOf((int)this.Properties.RotateAngle) > -1)
            {

                Bullets.ShipFire fire = new Bullets.ShipFire();
                fire.Properties.CollisionRect.X = this.Properties.CollisionRect.X + (this.Properties.CollisionRect.Width / 2);
                fire.Properties.CollisionRect.X -= (fire.Properties.CollisionRect.Width / 2);
                fire.Properties.CollisionRect.Y = this.Properties.CollisionRect.Y + (this.Properties.CollisionRect.Height / 2);
                fire.Properties.CollisionRect.Y -= (fire.Properties.CollisionRect.Height / 2);
                fire.Properties.RotateAngle = this.Properties.RotateAngle;
                fire.Properties.RotateStep = 0.2f;
                this.Properties.GameScreen.Materials.Add(fire);
            }
        }

        public override bool OtherCollided(Sprite sprite, SpriteProperties.Directions collidedDirection, PointF CameraLocation)
        {
            //if (sprite.GetType() == typeof(Bullets.ShipFire)) return false;
            //if (sprite.GetType() == typeof(Bullets.Fire) || sprite.GetType() == typeof(Bullets.Bullet))
            //{
            //    sprite.Properties.isALive = false;
            //    isdie = true;
            //    return false;
            //}
            return false;
        }
    }
}
