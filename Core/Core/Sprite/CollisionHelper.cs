using System.Drawing;

namespace Core.Sprite
{
    public static class CollisionHelper
    {
        private static bool CollisionLeftRight(RectangleF collision, RectangleF affected)
        {
            return ((collision.X + collision.Width > affected.X
                && collision.X < (affected.X + affected.Width)));
        }

        public static bool CollisionUp(RectangleF collision, RectangleF affected, RectangleF velocity)
        {
            bool leftright = CollisionLeftRight(collision, affected);
            return (((collision.Y + collision.Height) - (velocity.Y * 2)) < affected.Y && leftright);
        }

        public static bool CollisionDown(RectangleF collision, RectangleF affected, RectangleF velocity)
        {
            bool leftright = CollisionLeftRight(collision, affected);
            if (velocity.Y > 0) velocity.Y = -velocity.Y;
            return ((collision.Y + -(velocity.Y * 2)) > (affected.Y+affected.Height) && leftright);
        }

        public static bool CollisionLeft(RectangleF collision, RectangleF affected, RectangleF velocity)
        {
            return ((collision.X + collision.Width) > affected.X && collision.X < affected.X);
        }

        public static bool CollisionRight(RectangleF collision, RectangleF affected, RectangleF velocity)
        {
            return (collision.X < (affected.X + affected.Width) 
                && (collision.X+collision.Width) > (affected.X+affected.Width));
        }
    }
}
