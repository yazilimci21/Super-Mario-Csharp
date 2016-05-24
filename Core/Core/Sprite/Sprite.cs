using System;
using System.Collections.Generic;
using System.Drawing;

namespace Core.Sprite
{
    public class Sprite : DisposableObject
    {
        public List<Animation.Animator> AnimatorList = new List<Animation.Animator>();
        public Animation.Animator currentAnimator;
        public int currentAnimatorIndex = -1;
        public SpriteProperties Properties = new SpriteProperties();

        public override void Disposed(bool dispose)
        {
            if (dispose)
            {
                for (int i = 0; i < AnimatorList.Count; i++)
                {
                    AnimatorList[i].Dispose();
                }
                Properties.Dispose();
                AnimatorList.Clear();
                GC.SuppressFinalize(AnimatorList);
                GC.SuppressFinalize(Properties);
                if(currentAnimator != null) GC.SuppressFinalize(currentAnimator);
            }
            base.Disposed(dispose);
        }

        public virtual void Draw(Graphics g, PointF CameraLocation)
        {
            if(currentAnimator != null)
            {
                RectangleF thisrect = AddRect(this.Properties.CollisionRect, CameraLocation);
                if (isCollision(thisrect, g.VisibleClipBounds) && this.Properties.isALive)
                {
                    if (this.Properties.isRotate)
                    {
                        g.TranslateTransform(thisrect.X + (thisrect.Width / 2), thisrect.Y + (thisrect.Height / 2));
                        g.RotateTransform(this.Properties.RotateAngle);
                        g.TranslateTransform(-(thisrect.X + (thisrect.Width / 2)), -(thisrect.Y + (thisrect.Height / 2)));
                    }
                    if (this.Properties.Alpha > 0 && this.Properties.Alpha <= 255)
                    {
                        Image img = setAlpha(currentAnimator.currentAnimation.Anim, 255-this.Properties.Alpha);
                        g.DrawImage(img, thisrect);
                        img.Dispose();
                        GC.SuppressFinalize(img);
                    }
                    else
                    {
                        g.DrawImage(currentAnimator.currentAnimation.Anim, thisrect);
                    }
                    if (this.Properties.isRotate)
                    {
                        thisrect = FindRotateRectangle(thisrect, this.Properties.RotateAngle);
                        g.ResetTransform();
                    }
                    //g.DrawRectangle(Pens.Black, new Rectangle((int)thisrect.X, (int)thisrect.Y, (int)thisrect.Width, (int)thisrect.Height));
                }
            }
        }

        public void UpdateStep(float time, PointF CameraLocation)
        {
            if (currentAnimator != null) currentAnimator.AnimTick(this.Properties.GameScreen.time);
            if(this.Properties.isALive)
            {
                this.Properties.Step(time, CameraLocation);
            }
        }

        public void UpdateCollisions(float time, PointF CameraLocation)
        {
            if (this.Properties.isTile || !this.Properties.isCollisionable || !this.Properties.isALive) return;
            bool Gravity = true;
            RectangleF thisrect = AddRect(this.Properties.CollisionRect, CameraLocation);
            if (this.Properties.isRotate)
            {
                thisrect = FindRotateRectangle(thisrect, this.Properties.RotateAngle);
            }
            for (int i = 0; i < this.Properties.GameScreen.Materials.Count; i++)
            {
                if (this.Properties.GameScreen.Materials[i] != this && this.Properties.GameScreen.Materials[i].Properties.isCollisionable)
                {
                    LookMaterialForCollision(thisrect, this.Properties.GameScreen.Materials[i], CameraLocation, ref Gravity);
                }
            }
            if (Gravity)
            {
                this.NoCollided();
            }
        }

        public virtual bool OtherCollided(Sprite sprite, SpriteProperties.Directions collidedDirection,PointF CameraLocation)
        {
            RectangleF thisrect = this.Properties.CollisionRect;
            RectangleF spriterect = sprite.Properties.CollisionRect;

            if(this.Properties.isRotate)
            {
                thisrect = FindRotateRectangle(thisrect, this.Properties.RotateAngle);
            }
            if (sprite.Properties.isRotate)
            {
                spriterect = FindRotateRectangle(spriterect, sprite.Properties.RotateAngle);
            }

            bool result = false;
            if (collidedDirection == SpriteProperties.Directions.Left)
            {
                spriterect.X = thisrect.X - spriterect.Width;
                result = true;
            }
            if (collidedDirection == SpriteProperties.Directions.Right)
            {
                spriterect.X = thisrect.X + thisrect.Width;
                result = true;
            }
            if (collidedDirection == SpriteProperties.Directions.Up)
            {
                spriterect.Y = thisrect.Y - spriterect.Height;
            }
            if (collidedDirection == SpriteProperties.Directions.Down)
            {
                spriterect.Y = thisrect.Y + thisrect.Height;
            }
            sprite.Properties.CollisionRect = spriterect;
            return result;
        }

        public virtual void WeCollided(Sprite sprite, SpriteProperties.Directions collidedDirection, PointF CameraLocation)
        {

        }

        public virtual void NoCollided()
        {
            this.Properties.VelocityRect.Y = this.Properties.GravityValue;
        }

        public virtual void Update(float time, PointF CameraLocation)
        {

        }

        public void FlipY()
        {
            for (int i = 0; i < AnimatorList.Count; i++)
            {
                AnimatorList[i].FlipY();
            }
        }

        public void FlipX()
        {
            for (int i = 0; i < AnimatorList.Count; i++)
            {
                AnimatorList[i].FlipX();
            }
        }

        public void setAnimation(int index)
        {
            if (index >= AnimatorList.Count) return;
            if (index != currentAnimatorIndex)
            {
                currentAnimatorIndex = index;
                currentAnimator = AnimatorList[index];
                currentAnimator.gotoAndPlay(0);
            }
            if (this.Properties.CollisionRect != RectangleF.Empty)
            {
                if (currentAnimator.AnimSize.Height > this.Properties.CollisionRect.Height)
                    this.Properties.CollisionRect.Y -= (currentAnimator.AnimSize.Height - this.Properties.CollisionRect.Height);
                else
                    this.Properties.CollisionRect.Y += (this.Properties.CollisionRect.Height - currentAnimator.AnimSize.Height);

                if (currentAnimator.AnimSize.Width > this.Properties.CollisionRect.Width)
                    this.Properties.CollisionRect.X -= (currentAnimator.AnimSize.Width - this.Properties.CollisionRect.Width);
                else
                    this.Properties.CollisionRect.X += (this.Properties.CollisionRect.Width - currentAnimator.AnimSize.Width);

                this.Properties.CollisionRect.Width = currentAnimator.AnimSize.Width;
                this.Properties.CollisionRect.Height = currentAnimator.AnimSize.Height;
            }
        }

        #region Private Voids

        private Image setAlpha(Image img, int Alpha)
        {
            Bitmap bmp = (Bitmap)img.Clone();
            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    Color color = bmp.GetPixel(x, y);
                    bmp.SetPixel(x, y, Color.FromArgb(Alpha, color));
                }
            }
            bmp.MakeTransparent(bmp.GetPixel(1, 1));
            return bmp;
        }

        private bool isCollision(RectangleF collided, RectangleF affected)
        {
            collided.Width -= collided.Width / 5;
            collided.X += collided.Width / 5;
            return (collided.X < (affected.X + affected.Width) && (collided.X + collided.Width) > affected.X &&
                collided.Y < (affected.Y + affected.Height) && (collided.Y + collided.Height) > affected.Y);
        }

        private RectangleF AddRect(RectangleF rect1, PointF rect2)
        {
            rect1.X += rect2.X;
            rect1.Y += rect2.Y;
            return rect1;
        }

        private void findOtherCollideds(SpriteProperties.Directions mainDirection, RectangleF thisrect, Material mainMaterial, PointF CameraLocation)
        {
            for (int i = 0; i < this.Properties.GameScreen.Materials.Count; i++)
            {
                if (this.Properties.GameScreen.Materials[i] != this && this.Properties.GameScreen.Materials[i] != mainMaterial
                    && this.Properties.GameScreen.Materials[i].Properties.isCollisionable)
                {
                    RectangleF matrect = AddRect(this.Properties.GameScreen.Materials[i].Properties.CollisionRect, CameraLocation);
                    if (this.Properties.GameScreen.Materials[i].Properties.isRotate)
                    {
                        matrect = FindRotateRectangle(matrect, this.Properties.GameScreen.Materials[i].Properties.RotateAngle);
                    }
                    if (isCollision(thisrect, matrect))
                    {
                        SpriteProperties.Directions Direction = FindCollidedDirection(this.Properties.GameScreen.Materials[i], CameraLocation);
                        if (Direction != SpriteProperties.Directions.None && Direction == mainDirection)
                        {
                            RectangleF rect = this.Properties.CollisionRect;
                            this.WeCollided(this.Properties.GameScreen.Materials[i], Direction, CameraLocation);
                            this.Properties.GameScreen.Materials[i].OtherCollided(this, Direction, CameraLocation);
                            this.Properties.CollisionRect = rect;
                        }
                    }
                }
            }
        }

        private SpriteProperties.Directions FindCollidedDirection(Sprite sprite, PointF CameraLocation)
        {
            RectangleF collided = this.Properties.CollisionRect, affected = sprite.Properties.CollisionRect;
            RectangleF velocity = new RectangleF((this.Properties.VelocityRect.X * this.Properties.GameScreen.time),
                (this.Properties.VelocityRect.Y * this.Properties.GameScreen.time),
                (this.Properties.VelocityRect.Width * this.Properties.GameScreen.time),
                (this.Properties.VelocityRect.Height * this.Properties.GameScreen.time));
            if(this.Properties.isRotate)
            {
                velocity = new RectangleF((this.Properties.RotateStep * this.Properties.GameScreen.time),
                (this.Properties.RotateStep * this.Properties.GameScreen.time),
                (this.Properties.VelocityRect.Width * this.Properties.GameScreen.time),
                (this.Properties.VelocityRect.Height * this.Properties.GameScreen.time));
            }

            collided = AddRect(collided, CameraLocation);
            affected = AddRect(affected, CameraLocation);
            if (this.Properties.isRotate)
            {
                collided = FindRotateRectangle(collided, this.Properties.RotateAngle);
            }
            if (sprite.Properties.isRotate)
            {
                affected = FindRotateRectangle(affected, sprite.Properties.RotateAngle);
            }

            SpriteProperties.Directions Direction = SpriteProperties.Directions.None;
            if (CollisionHelper.CollisionUp(collided, affected, velocity))
            {
                if ((velocity.Y == 0 && !this.Properties.isRotate) || (this.Properties.RotateStep == 0 && this.Properties.isRotate)) return SpriteProperties.Directions.None;
                Direction = SpriteProperties.Directions.Up;
            }
            else if  (CollisionHelper.CollisionDown(collided, affected, velocity))
            {
                if ((velocity.Y == 0 && !this.Properties.isRotate) || (this.Properties.RotateStep == 0 && this.Properties.isRotate)) return SpriteProperties.Directions.None;
                Direction = SpriteProperties.Directions.Down;
            }
            else if (CollisionHelper.CollisionLeft(collided, affected, velocity))
            {
                if ((velocity.X == 0 && !this.Properties.isRotate) || (this.Properties.RotateStep == 0 && this.Properties.isRotate)) return SpriteProperties.Directions.None;
                Direction = SpriteProperties.Directions.Left;
            }
            else if (CollisionHelper.CollisionRight(collided, affected, velocity))
            {
                if ((velocity.X == 0 && !this.Properties.isRotate) || (this.Properties.RotateStep == 0 && this.Properties.isRotate)) return SpriteProperties.Directions.None;
                Direction = SpriteProperties.Directions.Right;
            }
            return Direction;
        }

        private void LookMaterialForCollision(RectangleF thisrect, Material material, PointF CameraLocation, ref bool Gravity)
        {
            RectangleF matrect = AddRect(material.Properties.CollisionRect, CameraLocation);
            if (material.Properties.isRotate)
            {
                matrect = FindRotateRectangle(matrect, material.Properties.RotateAngle);
            }
            if (isCollision(thisrect, matrect))
            {
                SpriteProperties.Directions Direction = FindCollidedDirection(material, CameraLocation);
                if (Direction != SpriteProperties.Directions.None)
                {
                    findOtherCollideds(Direction, thisrect, material, CameraLocation);
                    this.WeCollided(material, Direction, CameraLocation);
                    Gravity = material.OtherCollided(this, Direction, CameraLocation);
                }
            }
        }

        private RectangleF FindRotateRectangle(RectangleF rect, float RotateAngle)
        {
            RectangleF thisrect = rect;
            RectangleF newRect = RectangleF.Empty;

            SizeF newSize = this.Properties.FindNewSize(thisrect.Size, RotateAngle);

            newRect.Width = newSize.Width;
            newRect.Height = newSize.Height;
            newRect.X = thisrect.X;
            newRect.Y = thisrect.Y;

            if (newSize.Height > thisrect.Height)
                newRect.Y -= (newSize.Height - thisrect.Height) / 2;
            else
                newRect.Y += (thisrect.Height - newSize.Height) / 2;

            if (newSize.Width > thisrect.Width)
                newRect.X -= (newSize.Width - thisrect.Width) / 2;
            else
                newRect.X += (thisrect.Width - newSize.Width) / 2;

            return newRect;
        }

        private bool findType(object obj, Type finding)
        {
            Type type = obj.GetType();
            if (type == finding) return true;
            while (type != null)
            {
                if (type == finding) return true;
                type = type.BaseType;
            }
            return false;
        }
        #endregion
    }
}
