using System.Drawing;
using System.Windows.Forms;

namespace Mario.SampleGame.Materials
{
    public class Coin : Core.Material
    {
        Core.Animation.Animator animator = new Core.Animation.Animator();
        private string Itempath = Application.StartupPath + "\\Images\\items\\";
        public bool isdie = false;
        private float jump = 0f;

        public Coin()
        {
            Image[] image = new Image[] { ImageLoader.LoadImage(Itempath + "Coin_1.png"), ImageLoader.LoadImage(Itempath + "Coin_2.png"), ImageLoader.LoadImage(Itempath + "Coin_3.png"),
            ImageLoader.LoadImage(Itempath+"Coin_4.png"), ImageLoader.LoadImage(Itempath+"Coin_5.png"), ImageLoader.LoadImage(Itempath+"Coin_6.png"), ImageLoader.LoadImage(Itempath+"Coin_7.png"), ImageLoader.LoadImage(Itempath+"Coin_8.png") };
            for (int i = 0; i < image.Length;i++)
            {
                animator.AddFrame(image[i], @"", new SizeF(24, 24));
            }
            this.AnimatorList.Add(animator);
            setAnimation(0);
            this.Properties.CollisionRect.Size = AnimatorList[0].AnimSize;
        }

        public override void Update(float time, PointF CameraLocation)
        {
            base.Update(time, CameraLocation);
            if(isdie)
            {
                jump += 1;
                if (jump >= 10) this.Properties.isALive = false;
            }
        }

        public override bool OtherCollided(Core.Sprite.Sprite sprite, Core.Sprite.SpriteProperties.Directions collidedDirection, PointF CameraLocation)
        {
            if (sprite.Properties.isPlayer)
            {
                this.Properties.isCollisionable = false;
                this.Properties.VelocityRect.Y -= 0.60f;
                this.isdie = true;
            }
            return false;
        }

        public override void WeCollided(Core.Sprite.Sprite sprite, Core.Sprite.SpriteProperties.Directions collidedDirection, PointF CameraLocation)
        {
            
        }
    }
}
