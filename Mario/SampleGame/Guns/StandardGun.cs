using Core.Sprite;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Mario.SampleGame.Guns
{
    public class StandardGun : Core.MaterialWithKeyAndMouse
    {
        private Core.Material owner;
        private string Imagepath = Application.StartupPath + "\\Images\\";
        Core.Animation.Animator animator = new Core.Animation.Animator();
        int bulletInt = 50;
        private DateTime lastfire = DateTime.Now;

        public StandardGun()
        {
            Image gunImage = ImageLoader.LoadImage(Imagepath + "gun.png");
            animator.AddFrame(gunImage, @"", new SizeF(gunImage.Width, gunImage.Height));
            this.AnimatorList.Add(animator);
            setAnimation(0);
            this.Properties.CollisionRect.Size = AnimatorList[0].AnimSize;
            this.Properties.Direction = Core.Sprite.SpriteProperties.Directions.Left;
        }

        public override void Update(float time, System.Drawing.PointF CameraLocation)
        {
            if (owner != null)
            {
                this.listedKey = true;
                float x = owner.Properties.CollisionRect.X + (owner.Properties.CollisionRect.Width/2);
                float y = owner.Properties.CollisionRect.Y + (owner.Properties.CollisionRect.Height/2);
                if (owner.Properties.Direction != this.Properties.Direction)
                {
                    this.FlipY();
                    this.Properties.Direction = owner.Properties.Direction;
                }
                x += owner.Properties.VelocityRect.X * owner.Properties.GameScreen.time;
                if (owner.Properties.Direction == Core.Sprite.SpriteProperties.Directions.Right)
                {
                    x -= owner.Properties.CollisionRect.Width;
                }
                y -= (this.Properties.CollisionRect.Height / 2);
                this.Properties.CollisionRect = new RectangleF(x, y, this.Properties.CollisionRect.Width, this.Properties.CollisionRect.Height);
                for (int i = 0; i < PlayerKeyList.Count; i++)
                {
                    if (PlayerKeyList[i].Key.KeyCode == System.Windows.Forms.Keys.A)
                    {
                        if (PlayerKeyList[i].ClickType == PlayerKey.ClickTypes.Up)
                        {
                            Shoot();
                        }
                    }
                    PlayerKeyList.RemoveAt(i);
                    i--;
                }
            }
        }

        public override bool OtherCollided(Core.Sprite.Sprite sprite, Core.Sprite.SpriteProperties.Directions collidedDirection, PointF CameraLocation)
        {
            if(sprite.Properties.isPlayer)
            {
                if (!((SampleGame.Players.Mario)sprite).haveGun)
                {
                    ((SampleGame.Players.Mario)sprite).haveGun = true;
                    this.owner = (Core.Material)sprite;
                    this.Properties.isCollisionable = false;
                }
            }
            return true;
        }

        public void Shoot()
        {
            DateTime dt = DateTime.Now;
            if (dt > lastfire)
            {
                float x = this.Properties.CollisionRect.X, y = this.Properties.CollisionRect.Y;
                lastfire = dt.AddMilliseconds(800);

                bulletInt--;
                y += (this.Properties.CollisionRect.Height / 4);
                Bullets.Bullet bullet = new Bullets.Bullet();
                if (this.Properties.Direction == SpriteProperties.Directions.Left)
                {
                    x += this.Properties.CollisionRect.Width / 2;
                    bullet.Properties.VelocityRect.X = 0.56f;
                }
                if (this.Properties.Direction == SpriteProperties.Directions.Right)
                {
                    bullet.Properties.VelocityRect.X = -0.56f;
                }
                bullet.Properties.CollisionRect = new RectangleF(x, y,
                bullet.Properties.CollisionRect.Width, bullet.Properties.CollisionRect.Height);
                this.Properties.GameScreen.Materials.Add(bullet);
                if (bulletInt <= 0)
                {
                    ((SampleGame.Players.Mario)owner).haveGun = false;
                    this.Properties.isALive = false;
                }
            }
        }
    }
}
