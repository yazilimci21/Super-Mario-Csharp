using Core.Sprite;
using System.Drawing;
using System.Windows.Forms;

namespace Mario.SampleGame.Players
{
    public class Pacman : Core.MaterialWithKeyAndMouse
    {
        Core.Animation.Animator Still = new Core.Animation.Animator();
        private string Mariopath = Application.StartupPath + "\\Images\\";
        private int maxrotate = 18, rotateint = 0;
        private bool rotate = false;

        public Pacman()
        {
            this.listedKey = true;
            this.Properties.isRotate = true;
            Image stillImage = ImageLoader.LoadImage(Mariopath + "pacman.png");
            Still.AddFrame(stillImage, "", new SizeF(80f, 52f));
            AnimatorList.Add(Still);
            setAnimation(0);
            this.Properties.Direction = SpriteProperties.Directions.Left;
            this.Properties.CollisionRect.Size = AnimatorList[0].AnimSize;
            this.Properties.RotateAngle = 20;
        }

        private void rotateHandle()
        {
            float rotate0 = (360 / maxrotate);
            if (this.Properties.Direction == SpriteProperties.Directions.Left) rotate0 = -rotate0;
            this.Properties.RotateAngle += rotate0;
            rotateint++;
            if (rotateint >= maxrotate)
            {
                this.Properties.RotateAngle = 20;
                rotateint = 0;
                rotate = false;
            }
        }

        public override void Update(float time, System.Drawing.PointF CameraLocation)
        {
            if (rotate) rotateHandle();
            for (int i = 0; i < PlayerKeyList.Count; i++)
            {
                if (PlayerKeyList[i].Key.KeyCode == System.Windows.Forms.Keys.Right)
                {
                    if (PlayerKeyList[i].ClickType == PlayerKey.ClickTypes.Down)
                    {
                        if (this.Properties.Direction == SpriteProperties.Directions.Right)
                        {
                            this.Properties.Direction = SpriteProperties.Directions.Left;
                            //this.FlipY();
                        }
                    }
                }
                if (PlayerKeyList[i].Key.KeyCode == System.Windows.Forms.Keys.Left)
                {
                    if (PlayerKeyList[i].ClickType == PlayerKey.ClickTypes.Down)
                    {
                        if (this.Properties.Direction == SpriteProperties.Directions.Left)
                        {
                            this.Properties.Direction = SpriteProperties.Directions.Right;
                            //this.FlipY();
                        }
                    }
                }
                PlayerKeyList.RemoveAt(i);
                i--;
            }
        }

        public override bool OtherCollided(Core.Sprite.Sprite sprite, Core.Sprite.SpriteProperties.Directions collidedDirection, PointF CameraLocation)
        {
            if(collidedDirection == SpriteProperties.Directions.Down)
            {
                if (sprite.Properties.VelocityRect.Y < 0) sprite.Properties.VelocityRect.Y = -sprite.Properties.VelocityRect.Y;
                rotate = true;
            }
            return false;
        }

        public override void WeCollided(Core.Sprite.Sprite sprite, Core.Sprite.SpriteProperties.Directions collidedDirection, System.Drawing.PointF CameraLocation)
        {
            base.WeCollided(sprite, collidedDirection, CameraLocation);
        }
    }
}
