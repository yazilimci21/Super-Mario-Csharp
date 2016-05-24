using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Mario.SampleGame.Materials
{
    public class FinishLevel : Core.Material
    {
        Core.Animation.Animator animator = new Core.Animation.Animator();
        private string Imagepath = Application.StartupPath + "\\Images\\";

        public FinishLevel()
        {
            Image platformImage = ImageLoader.LoadImage(Imagepath + "door.png");
            animator.AddFrame(platformImage, @"", new SizeF(platformImage.Width, platformImage.Height));
            this.AnimatorList.Add(animator);
            setAnimation(0);
            this.Properties.CollisionRect.Size = AnimatorList[0].AnimSize;
        }

        public override bool OtherCollided(Core.Sprite.Sprite sprite, Core.Sprite.SpriteProperties.Directions collidedDirection, PointF CameraLocation)
        {
            if(sprite.Properties.isPlayer)
                this.Properties.GameScreen.isFinish = true;
            return false;
        }
    }
}
