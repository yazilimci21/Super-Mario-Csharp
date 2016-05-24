using System;
using System.Drawing;
using System.Windows.Forms;

namespace Mario.SampleGame.Materials
{
    public class QuestionBlock : Core.Material
    {
        Core.Animation.Animator animator = new Core.Animation.Animator();
        private string Itempath = Application.StartupPath + "\\Images\\items\\";
        Core.Animation.Animator diedanimator = new Core.Animation.Animator();
        private bool isdie = false;
        float jump = 0f, length = -0.10f;

        public QuestionBlock()
        {
            this.Properties.isTile = true;
            Image[] image = new Image[] { ImageLoader.LoadImage(Itempath + "Question_Block_0.png"), 
                ImageLoader.LoadImage(Itempath + "Question_Block_1.png"), 
                ImageLoader.LoadImage(Itempath + "Question_Block_2.png"),
                ImageLoader.LoadImage(Itempath+"Question_Block_3.png")};
            for (int i = 0; i < image.Length; i++)
            {
                animator.AddFrame(image[i], @"", new SizeF(32, 32));
            }
            Image deadImage = ImageLoader.LoadImage(Itempath + "Question_Block_Dead.png");
            diedanimator.AddFrame(deadImage, @"", new SizeF(32, 32));
            AnimatorList.Add(animator);
            AnimatorList.Add(diedanimator);
            setAnimation(0);
            this.Properties.CollisionRect.Size = AnimatorList[0].AnimSize;
        }

        public override void Update(float time, PointF CameraLocation)
        {
            if (isdie)
            {
                if (jump < 2.0f)
                {
                    this.Properties.VelocityRect.Y = length;
                    jump++;
                }
                else if (jump < 4.0f)
                {
                    this.Properties.VelocityRect.Y = -length;
                    jump++;
                }
                else this.Properties.VelocityRect.Y = 0;
            }
        }

        public override bool OtherCollided(Core.Sprite.Sprite sprite, Core.Sprite.SpriteProperties.Directions collidedDirection, PointF CameraLocation)
        {
            if (collidedDirection == Core.Sprite.SpriteProperties.Directions.Down)
            {
                if (!isdie && sprite.Properties.isPlayer)
                {
                    Coin coin = new Coin();
                    coin.Properties.CollisionRect = new RectangleF(this.Properties.CollisionRect.X, this.Properties.CollisionRect.Y,
                        coin.Properties.CollisionRect.Width, coin.Properties.CollisionRect.Height);
                    this.Properties.GameScreen.Materials.Add(coin);
                    coin.Properties.isCollisionable = false;
                    coin.Properties.VelocityRect.Y = -0.60f;
                    coin.isdie = true;
                    this.isdie = true;
                    setAnimation(1);
                }
                if (sprite.Properties.VelocityRect.Y < 0) sprite.Properties.VelocityRect.Y = -sprite.Properties.VelocityRect.Y;
            }
            return base.OtherCollided(sprite, collidedDirection, CameraLocation);
        }
    }
}
