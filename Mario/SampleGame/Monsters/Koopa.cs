using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Mario.SampleGame.Monsters
{
    public class Koopa : Core.Material
    {
        Core.Animation.Animator Still = new Core.Animation.Animator();
        Core.Animation.Animator Step = new Core.Animation.Animator();
        Core.Animation.Animator Shell = new Core.Animation.Animator();
        private string Baddiepath = Application.StartupPath + "\\Images\\baddies\\";

        public Koopa()
        {
            this.Properties.GravityValue = 0.28f;
            Image KoopaImage1 = ImageLoader.LoadImage(Baddiepath + "Koopa_Red_1.png");
            Step.AddFrame(KoopaImage1, @"", new SizeF(KoopaImage1.Width * 1.5f, KoopaImage1.Height * 1.5f));
            Image KoopaImage2 = ImageLoader.LoadImage(Baddiepath + "Koopa_Red_2.png");
            Step.AddFrame(KoopaImage2, @"", new SizeF(KoopaImage2.Width * 1.5f, KoopaImage2.Height * 1.5f));
            this.AnimatorList.Add(Step);

            Image KoopaShell1 = ImageLoader.LoadImage(Baddiepath + "Red_Shell_1.png");
            Shell.AddFrame(KoopaShell1, @"", new SizeF(KoopaShell1.Width * 1.5f, KoopaShell1.Height * 1.5f));
            Image KoopaShell2 = ImageLoader.LoadImage(Baddiepath + "Red_Shell_2.png");
            Shell.AddFrame(KoopaShell2, @"", new SizeF(KoopaShell2.Width * 1.5f, KoopaShell2.Height * 1.5f));
            Image KoopaShell3 = ImageLoader.LoadImage(Baddiepath + "Red_Shell_3.png");
            Shell.AddFrame(KoopaShell3, @"", new SizeF(KoopaShell3.Width * 1.5f, KoopaShell3.Height * 1.5f));
            Image KoopaShell4 = ImageLoader.LoadImage(Baddiepath + "Red_Shell_4.png");
            Shell.AddFrame(KoopaShell4, @"", new SizeF(KoopaShell4.Width * 1.5f, KoopaShell4.Height * 1.5f));
            this.AnimatorList.Add(Shell);

            Image StillImage = ImageLoader.LoadImage(Baddiepath + "Red_Shell_1.png");
            Still.AddFrame(StillImage, @"", new SizeF(StillImage.Width * 1.5f, StillImage.Height * 1.5f));
            this.AnimatorList.Add(Still);

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

        float dieTimer = 0, flip = 0, maxX = 5;
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
                if (flip != 0)
                {
                    this.Properties.Alpha += 15;
                    dieTimer += 15;
                    if (dieTimer >= 255)
                    {
                        this.Properties.isALive = false;
                    }
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
                if (this.currentAnimatorIndex == 2)
                {
                    setAnimation(1);
                    this.Properties.VelocityRect.X = sprite.Properties.VelocityRect.X;
                    return;
                }
                if (this.currentAnimatorIndex == 1)
                {
                    setAnimation(2);
                    this.Properties.VelocityRect.X = 0;
                    return;
                }
                if (this.currentAnimatorIndex == 0)
                {
                    this.Properties.VelocityRect.X = 0f;
                    setAnimation(2);
                    return;
                }
            }
            if (sprite.Properties.isPlayer && (collidedDirection == Core.Sprite.SpriteProperties.Directions.Left ||
                collidedDirection == Core.Sprite.SpriteProperties.Directions.Right))
            {
                if (this.Properties.Alpha == 0 && this.currentAnimatorIndex == 2)
                {
                    sprite.OtherCollided(this, collidedDirection, CameraLocation);
                    setAnimation(1);
                    this.Properties.VelocityRect.X = sprite.Properties.VelocityRect.X;
                    return;
                }
                if (this.Properties.Alpha == 0 && (this.currentAnimatorIndex == 1 || this.currentAnimatorIndex == 0))
                {
                    if (sprite.Properties.VelocityRect.Y < 0f) return;
                    if (this.currentAnimatorIndex == 1 && sprite.Properties.VelocityRect.X != this.Properties.VelocityRect.X && this.Properties.VelocityRect.X != 0)
                        ((Mario.SampleGame.Players.Mario)sprite).isdead = true;
                    if(this.currentAnimatorIndex == 0)
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
                if (this.currentAnimatorIndex == 1) this.Properties.VelocityRect.X = -this.Properties.VelocityRect.X;
                else flipMove();
            }
            if(sprite.GetType() == typeof(Koopa) && 
                (this.currentAnimatorIndex == 1 || sprite.currentAnimatorIndex == 1) &&
                (this.Properties.VelocityRect.X != 0 || sprite.Properties.VelocityRect.X != 0))
            {
                this.Kill();
                ((Koopa)sprite).Kill();
            }
            if (sprite.GetType() == typeof(Goomba) && this.currentAnimatorIndex == 1 &&
                this.Properties.VelocityRect.X != 0)
            {
                ((Goomba)sprite).Kill();
            }
        }

        public void Kill()
        {
            if (flip == 0)
            {
                this.Properties.VelocityRect.X = 0f;
                setAnimation(2);
                flip = 1;
                FlipX();
            }
            else
            {
                this.Properties.VelocityRect.X = 0f;
                setAnimation(2);
            }
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
