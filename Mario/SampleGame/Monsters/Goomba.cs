using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Mario.SampleGame.Monsters
{
    public class Goomba : Core.Material
    {
        Core.Animation.Animator Step = new Core.Animation.Animator();
        Core.Animation.Animator Dead = new Core.Animation.Animator();
        private string Baddiepath = Application.StartupPath + "\\Images\\baddies\\";

        public Goomba()
        {
            this.Properties.GravityValue = 0.28f;
            Image GoombaImage1 = ImageLoader.LoadImage(Baddiepath + "Goomba_1.png");
            Step.AddFrame(GoombaImage1, @"", new SizeF(GoombaImage1.Width * 1.5f, GoombaImage1.Height * 1.5f));
            Image GoombaImage2 = ImageLoader.LoadImage(Baddiepath + "Goomba_2.png");
            Step.AddFrame(GoombaImage2, @"", new SizeF(GoombaImage2.Width * 1.5f, GoombaImage2.Height * 1.5f));
            this.AnimatorList.Add(Step);

            Image GoombaDiedImage = ImageLoader.LoadImage(Baddiepath + "Goomba_Dead.png");
            Dead.AddFrame(GoombaDiedImage, @"", new SizeF(GoombaDiedImage.Width * 1.5f, GoombaDiedImage.Height * 1.5f));
            this.AnimatorList.Add(Dead);

            setAnimation(0);
            this.Properties.CollisionRect.Size = AnimatorList[0].AnimSize;
            Random rnd = new Random();
            if (rnd.Next(0, 10) > 5)
            {
                maxX = 25;
                this.Properties.Direction = Core.Sprite.SpriteProperties.Directions.Right;
            }
            else
            {
                maxX = -25;
                this.Properties.Direction = Core.Sprite.SpriteProperties.Directions.Left;
                this.FlipY();
            }
        }

        float dieTimer = 0, maxX = 25;
        Random rnd = new Random();

        public override void Update(float time, PointF CameraLocation)
        {
            if (this.Properties.CollisionRect.Y > this.Properties.GameScreen.GameRect.Height)
            {
                Kill();
            }
            if (this.currentAnimatorIndex == 0)
            {
                if (maxX < 0)
                {
                    maxX++;
                    this.Properties.VelocityRect.X = -0.2f;
                }
                else if (maxX > 0)
                {
                    maxX--;
                    this.Properties.VelocityRect.X = 0.2f;
                }
                else
                {
                    flipMove();
                }
            }
            else
            {
                this.Properties.Alpha += 15;
                dieTimer += 15;
                if (dieTimer >= 255)
                {
                    this.Properties.isALive = false;
                }
            }
        }

        public override bool OtherCollided(Core.Sprite.Sprite sprite, Core.Sprite.SpriteProperties.Directions collidedDirection, PointF CameraLocation)
        {
            Actions(sprite, collidedDirection, CameraLocation);
            return false;
        }

        public override void WeCollided(Core.Sprite.Sprite sprite, Core.Sprite.SpriteProperties.Directions collidedDirection, PointF CameraLocation)
        {
            Actions(sprite, collidedDirection, CameraLocation);
        }

        public void Actions(Core.Sprite.Sprite sprite, Core.Sprite.SpriteProperties.Directions collidedDirection, PointF CameraLocation)
        {
            if (sprite.Properties.isPlayer && collidedDirection == Core.Sprite.SpriteProperties.Directions.Up)
            {
                sprite.Properties.VelocityRect.Y = -0.38f;
                ((Mario.SampleGame.Players.Mario)sprite).JumpLength = new RectangleF(sprite.Properties.CollisionRect.X,
                        sprite.Properties.CollisionRect.Y - (sprite.Properties.CollisionRect.Height),
                        sprite.Properties.CollisionRect.Width, sprite.Properties.CollisionRect.Height);
                ((Mario.SampleGame.Players.Mario)sprite).jumping = true;
                sprite.setAnimation(3);
                this.Properties.VelocityRect.X = 0f;
                setAnimation(1);
            }
            if (sprite.Properties.isPlayer && (collidedDirection == Core.Sprite.SpriteProperties.Directions.Left || collidedDirection == Core.Sprite.SpriteProperties.Directions.Right))
            {
                if (this.Properties.Alpha == 0)
                {
                    ((Mario.SampleGame.Players.Mario)sprite).isdead = true;
                }
            }
            if (sprite.GetType() == typeof(SampleGame.Bullets.Fire) || sprite.GetType() == typeof(SampleGame.Bullets.Bullet))
            {
                sprite.Properties.isALive = false;
                Kill();
            }
            if (sprite.GetType() == typeof(SampleGame.Materials.QuestionBlock) && sprite.Properties.VelocityRect.Y != 0)
            {
                Kill();
            }
            if (sprite.Properties.isTile && (collidedDirection == Core.Sprite.SpriteProperties.Directions.Right || collidedDirection == Core.Sprite.SpriteProperties.Directions.Left))
            {
                flipMove();
            }
        }

        public void Kill()
        {
            this.Properties.VelocityRect.X = 0f;
            setAnimation(1);
        }

        public void flipMove()
        {
            if (this.Properties.Direction == Core.Sprite.SpriteProperties.Directions.Right)
            {
                this.Properties.Direction = Core.Sprite.SpriteProperties.Directions.Left;
                maxX = -50;
            }
            else
            {
                this.Properties.Direction = Core.Sprite.SpriteProperties.Directions.Right;
                maxX = 50;
            }
            this.FlipY();
        }
    }
}
