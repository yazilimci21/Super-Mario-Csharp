using Core.Sprite;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Mario.SampleGame.Players
{
    public class Mario : Core.Player
    {
        Core.Animation.Animator Stillanim = new Core.Animation.Animator();
        Core.Animation.Animator Stepanim = new Core.Animation.Animator();
        Core.Animation.Animator Crouchanim = new Core.Animation.Animator();
        Core.Animation.Animator Jumpanim = new Core.Animation.Animator();
        Core.Animation.Animator Runanim = new Core.Animation.Animator();
        Core.Animation.Animator Slopeanim = new Core.Animation.Animator();
        private string Mariopath = Application.StartupPath + "\\Images\\Mario\\";
        private DateTime lastfire = DateTime.Now;
        public bool haveGun = false;

        public Mario()
        {
            this.listedKey = true;
            this.Properties.GravityValue = .28f;
            Image crouchImage = ImageLoader.LoadImage(Mariopath + "Mario_Big_Crouch.png");
            Image stillImage = ImageLoader.LoadImage(Mariopath + "Mario_Big_Still.png");
            Image step1Image = ImageLoader.LoadImage(Mariopath + "Mario_Big_Step1.png");
            Image step2Image = ImageLoader.LoadImage(Mariopath + "Mario_Big_Step2.png");
            Image jumpImage = ImageLoader.LoadImage(Mariopath + "Mario_Big_Jump.png");
            Image run1Image = ImageLoader.LoadImage(Mariopath + "Mario_Big_Run1.png");
            Image run2Image = ImageLoader.LoadImage(Mariopath + "Mario_Big_Run2.png");
            Image slopeImage = ImageLoader.LoadImage(Mariopath + "Mario_Slope.png");

            Crouchanim.AddFrame(crouchImage, "", new SizeF(24, 32));
            Stillanim.AddFrame(stillImage, "", new SizeF(32, 52));
            Stepanim.AddFrame(step1Image, @"", new SizeF(32, 52));
            Stepanim.AddFrame(step2Image, @"", new SizeF(32, 52));
            Jumpanim.AddFrame(jumpImage, @"", new SizeF(32, 52));
            Runanim.AddFrame(run1Image, @"", new SizeF(38, 52));
            Runanim.AddFrame(run2Image, @"", new SizeF(38, 52));
            Slopeanim.AddFrame(slopeImage, @"", new SizeF(32, 52));

            AnimatorList.Add(Stillanim);
            AnimatorList.Add(Stepanim);
            AnimatorList.Add(Crouchanim);
            AnimatorList.Add(Jumpanim);
            AnimatorList.Add(Runanim);
            AnimatorList.Add(Slopeanim);
            crouchImage.Dispose();
            stillImage.Dispose();
            step1Image.Dispose();
            step2Image.Dispose();
            jumpImage.Dispose();
            run1Image.Dispose();
            run2Image.Dispose();
            slopeImage.Dispose();
            setAnimation(0);
            this.Properties.Direction = SpriteProperties.Directions.Left;
            this.Properties.CollisionRect.Size = AnimatorList[0].AnimSize;
        }

        public float runmaxspeed = 2.0f, runspeed = 0.1f, dietime = 0;
        public bool jumping = false, run = false, crouch = false, isdead = false;
        public RectangleF JumpLength = RectangleF.Empty;

        public override void Update(float time, PointF CameraLocation)
        {
            if(this.Properties.CollisionRect.Y > this.Properties.GameScreen.GameRect.Height)
            {
                this.Properties.GameScreen.Reset();
            }
            if (jumping && this.JumpLength != RectangleF.Empty)
            {
                if (this.Properties.CollisionRect.Y <= this.JumpLength.Y)
                {
                    jumping = false;
                }
            }
            LookisKill();
            for(int i=0;i<PlayerKeyList.Count;i++)
            {
                if (PlayerKeyList[i].Key.KeyCode == System.Windows.Forms.Keys.Left)
                {
                    if (PlayerKeyList[i].ClickType == PlayerKey.ClickTypes.Down)
                    {
                        MoveLeftOrRight(true, -0.38f);
                    }
                    else if (PlayerKeyList[i].ClickType == PlayerKey.ClickTypes.Up)
                    {
                        MoveLeftOrRight(false, 0f);
                    }
                }
                if (PlayerKeyList[i].Key.KeyCode == System.Windows.Forms.Keys.Right)
                {
                    if (PlayerKeyList[i].ClickType == PlayerKey.ClickTypes.Down)
                    {
                        MoveLeftOrRight(true, 0.38f);
                    }
                    else if (PlayerKeyList[i].ClickType == PlayerKey.ClickTypes.Up)
                    {
                        MoveLeftOrRight(false, 0f);
                    }
                }
                if (PlayerKeyList[i].Key.KeyCode == System.Windows.Forms.Keys.Down)
                {
                    if (PlayerKeyList[i].ClickType == PlayerKey.ClickTypes.Down)
                    {
                        Crouch(true);
                    }
                    else if (PlayerKeyList[i].ClickType == PlayerKey.ClickTypes.Up)
                    {
                        Crouch(false);
                    }
                }
                if (PlayerKeyList[i].Key.KeyCode == System.Windows.Forms.Keys.Space)
                {
                    if (PlayerKeyList[i].ClickType == PlayerKey.ClickTypes.Down)
                    {
                        Jump(true, -0.38f);
                    }
                    else if (PlayerKeyList[i].ClickType == PlayerKey.ClickTypes.Up)
                    {
                        Jump(false, 0f);
                    }
                }
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

        public override void WeCollided(Core.Sprite.Sprite sprite, SpriteProperties.Directions collidedDirection, PointF CameraLocation)
        {
            if (sprite.GetType() == typeof(Bullets.Fire)) return;
            if (sprite.GetType() == typeof(Bullets.Bullet) || sprite.GetType() == typeof(Bullets.ShipFire))
            {
                isdead = true;
                return;
            }
            if (collidedDirection == SpriteProperties.Directions.Up)
            {
                jumping = false;
                JumpLength = RectangleF.Empty;
                if (this.Properties.VelocityRect.X == 0)
                {
                    if (!crouch) this.setAnimation(0);
                    else this.setAnimation(2);
                }
                else
                {
                    if (sprite.Properties.VelocityRect.X == this.Properties.VelocityRect.X)
                    {
                        this.setAnimation(0);
                    }
                    else
                    {
                        if (run) this.setAnimation(4);
                        else this.setAnimation(1);
                    }
                }
            }
            if(collidedDirection == SpriteProperties.Directions.Down)
            {
                jumping = false;
            }
        }

        public override void NoCollided()
        {
            if (!jumping)
            {
                jumping = true;
                JumpLength = RectangleF.Empty;
                base.NoCollided();
            }
        }

        public override bool OtherCollided(Sprite sprite, SpriteProperties.Directions collidedDirection, PointF CameraLocation)
        {
            if (sprite.GetType() == typeof(Bullets.Fire)) return false;
            if (sprite.GetType() == typeof(Bullets.Bullet) || sprite.GetType() == typeof(Bullets.ShipFire))
            {
                isdead = true;
                return false;
            }
            return false;
        }

        #region Actions
        public void Shoot()
        {
            if (haveGun) return;
            DateTime dt = DateTime.Now;
            if (dt > lastfire)
            {
                lastfire = dt.AddMilliseconds(800);
                Bullets.Fire fire = new Bullets.Fire();
                fire.Properties.CollisionRect = new RectangleF(this.Properties.CollisionRect.X, this.Properties.CollisionRect.Y+(this.Properties.CollisionRect.Height/2),
                fire.Properties.CollisionRect.Width, fire.Properties.CollisionRect.Height);
                if (this.Properties.Direction == SpriteProperties.Directions.Left)
                {
                    fire.Properties.VelocityRect.X = 0.48f;
                }
                if (this.Properties.Direction == SpriteProperties.Directions.Right)
                {
                    fire.Properties.VelocityRect.X = -0.48f;
                    fire.Properties.CollisionRect.X += fire.Properties.CollisionRect.Width;
                }
                this.Properties.GameScreen.Materials.Add(fire);
            }
        }

        public void Jump(bool JumpIn,float VelocityY)
        {
            if (JumpIn)
            {
                if (!jumping && JumpLength == RectangleF.Empty)
                {
                    JumpLength = new RectangleF(this.Properties.CollisionRect.X,
                        this.Properties.CollisionRect.Y - (this.Properties.CollisionRect.Height * 2),
                        this.Properties.CollisionRect.Width, this.Properties.CollisionRect.Height);
                    this.Properties.VelocityRect.Y = VelocityY;
                    jumping = true;
                    this.setAnimation(3);
                }
            }
            else
            {
                if (jumping && JumpLength != RectangleF.Empty)
                    JumpLength.Y += this.Properties.CollisionRect.Height;
            }
        }

        public void Crouch(bool CrouchIn)
        {
            if (this.Properties.VelocityRect.X != 0f) return;
            if (CrouchIn)
            {
                crouch = true;
                if (!jumping)
                {
                    this.setAnimation(2);
                }
                else
                {
                    this.setAnimation(5);
                }
            }
            else
            {
                crouch = false;
                this.setAnimation(0);
            }
        }

        public void MoveLeftOrRight(bool MoveIn,float VelocityX)
        {
            if (Control.ModifierKeys == Keys.Shift && !jumping)
            {
                run = true;
                if (runspeed < runmaxspeed) runspeed += 0.1f;
                this.Properties.VelocityRect.X = VelocityX * runspeed;
            }
            else
            {
                run = false;
                runspeed = 0.1f;
                this.Properties.VelocityRect.X = VelocityX;
            }
            if (MoveIn)
            {
                if(VelocityX > 0)
                {
                    if (this.Properties.Direction == SpriteProperties.Directions.Right)
                    {
                        runspeed = 0.1f;
                        this.Properties.Direction = SpriteProperties.Directions.Left;
                        this.FlipY();
                    }
                }
                else
                {
                    if (this.Properties.Direction == SpriteProperties.Directions.Left)
                    {
                        runspeed = 0.1f;
                        this.Properties.Direction = SpriteProperties.Directions.Right;
                        this.FlipY();
                    }
                }
                if (jumping) this.setAnimation(3);
                else
                {
                    if (run)
                    {
                        this.setAnimation(4);
                    }
                    else
                    {
                        this.setAnimation(1);
                    }
                }
            }
            else
            {
                if (run)
                {
                    run = false;
                    runspeed = 0.1f;
                }
                this.Properties.VelocityRect.X = 0f;
                this.setAnimation(0);
            }
        }

        public void LookisKill()
        {
            if (isdead)
            {
                if (this.Properties.Alpha == 0) this.Properties.Alpha = 200;
                else this.Properties.Alpha = 0;
                dietime += 1;
                if (dietime > 20)
                {
                    dietime = 0;
                    isdead = false;
                }
            }
            else
            {
                this.Properties.Alpha = 0;
            }
        }

        #endregion
    }
}
