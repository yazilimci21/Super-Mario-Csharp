using System.Drawing;
using System.Windows.Forms;

namespace Mario.SampleGame.Materials
{
    public class RotatingBlock : Core.Material
    {
        Core.Animation.Animator animator = new Core.Animation.Animator();
        Core.Animation.Animator stillanimator = new Core.Animation.Animator();
        private string Itempath = Application.StartupPath + "\\Images\\items\\";
        private int i = 0;

        public RotatingBlock()
        {
            this.Properties.isTile = true;
            Image[] image = new Image[] { ImageLoader.LoadImage(Itempath + "Rotating_Block_Hit_1.png"), 
                ImageLoader.LoadImage(Itempath + "Rotating_Block_Hit_2.png"), 
                ImageLoader.LoadImage(Itempath + "Rotating_Block_Hit_3.png")};
            for (int i = 0; i < image.Length; i++)
            {
                animator.AddFrame(image[i], @"", new SizeF(32, 32));
            }
            Image stillImage = ImageLoader.LoadImage(Itempath + "Rotating_Block_Still.png");
            stillanimator.AddFrame(stillImage, @"", new SizeF(32, 32));
            AnimatorList.Add(stillanimator);
            animator.RePlay += new MethodInvoker(animFinish);
            AnimatorList.Add(animator);
            setAnimation(0);
            this.Properties.CollisionRect.Size = AnimatorList[0].AnimSize;
        }

        private void animFinish()
        {
            i++;
            if (i >= 2)
            {
                setAnimation(0);
                i = 0;
            }
        }

        public override bool OtherCollided(Core.Sprite.Sprite sprite, Core.Sprite.SpriteProperties.Directions collidedDirection, PointF CameraLocation)
        {
            if (collidedDirection == Core.Sprite.SpriteProperties.Directions.Down)
            {
                if (sprite.Properties.isPlayer)
                {
                    i = 0;
                    setAnimation(1);
                }
                if (sprite.Properties.VelocityRect.Y < 0) sprite.Properties.VelocityRect.Y = -sprite.Properties.VelocityRect.Y;
            }
            return base.OtherCollided(sprite, collidedDirection, CameraLocation);
        }
    }
}
