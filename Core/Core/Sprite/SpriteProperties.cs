using System;
using System.Drawing;

namespace Core.Sprite
{
    public class SpriteProperties : DisposableObject
    {
        public enum StatusTypes
        {
            Attack, Protection, Normal
        }

        public enum Directions
        {
            Left, Right, Up, Down, None
        }

        public StatusTypes Status = StatusTypes.Normal;
        public Directions Direction = Directions.Right;
        public RectangleF CollisionRect = new RectangleF(0, 0, 0, 0),
            VelocityRect = new RectangleF(0, 0, 0, 0);

        public float RotateAngle = 0, RotateStep = 0;
        public float GravityValue = 0f;
        public bool isCollisionable = true, isALive = true, isRotate = false, isPlayer = false, isTile = false;
        public Screen GameScreen;
        public int Alpha = 0;

        public override void Disposed(bool dispose)
        {
            base.Disposed(dispose);
        }

        public void Step(float time,PointF CameraLocation)
        {
            float xvalocity = 0, yvalocity = 0, wvalocity = 0, hvalocity = 0;

            if (isRotate)
            {
                double rotateVal = 2 * Math.PI * RotateAngle / 360;
                xvalocity = (float)Math.Cos(rotateVal) * (time * RotateStep);
                yvalocity = (float)Math.Sin(rotateVal) * (time * RotateStep);
                wvalocity = (time * VelocityRect.Width);
                hvalocity = (time * VelocityRect.Height);
            }
            else
            {
                xvalocity = (time * VelocityRect.X);
                yvalocity = (time * VelocityRect.Y);
                wvalocity = (time * VelocityRect.Width);
                hvalocity = (time * VelocityRect.Height);
            }
            if (!isPlayer)
            {
                CollisionRect.X += xvalocity;
                CollisionRect.Y += yvalocity;
                CollisionRect.Width += wvalocity;
                CollisionRect.Height += hvalocity;
            }
            else
            {
                RectangleF thisrect = new RectangleF(CollisionRect.X + xvalocity, CollisionRect.Y + yvalocity, CollisionRect.Width, CollisionRect.Height);

                if ((thisrect.X + CameraLocation.X) > 0 && ((thisrect.X + CameraLocation.X) + thisrect.Width) < this.GameScreen.ScreenSize.Width)
                    CollisionRect.X += xvalocity;
                //if ((thisrect.Y + CameraLocation.Y) > 0 && ((thisrect.Y + CameraLocation.Y) + thisrect.Height) < this.GameScreen.ScreenSize.Height)
                CollisionRect.Y += yvalocity;
                CollisionRect.Width += wvalocity;
                CollisionRect.Height += hvalocity;
            }
            if (isRotate)
            {
                if (RotateAngle >= 360) RotateAngle = 0;
                if (RotateAngle < 0) RotateAngle = 360;
            }
        }

        public PointF CollisionCenter
        {
            get
            {
                RectangleF thisrect = CollisionRect;
                if (!isPlayer)
                {
                    thisrect.X += GameScreen.CameraLocation.X;
                    thisrect.Y += GameScreen.CameraLocation.Y;
                }
                return new PointF(thisrect.X + (thisrect.Width / 2), thisrect.Y + (thisrect.Height / 2));
            }
        }

        public SizeF FindNewSize(SizeF oldSize, float angle)
        {
            float w = oldSize.Width;
            float h = oldSize.Height;
            double an = 2 * Math.PI * angle / 360;
            double cos = Math.Abs(Math.Cos(an));
            double sin = Math.Abs(Math.Sin(an));
            float nw = (float)(w * cos + h * sin);
            float nh = (float)(w * sin + h * cos);

            return new SizeF(nw, nh);
        }
    }
}
