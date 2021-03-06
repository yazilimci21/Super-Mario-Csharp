﻿using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Mario.SampleGame.Materials
{
    public class Stone : Core.Material
    {
        Core.Animation.Animator animator = new Core.Animation.Animator();
        Core.Sprite.SpriteReader animReader = new Core.Sprite.SpriteReader();
        private string Tilepath = Application.StartupPath + "\\Images\\tiles\\";

        public Stone()
        {
            this.Properties.isTile = true;
            List<Image> images = new List<Image>();
            Bitmap anim = (Bitmap)ImageLoader.LoadImage(Tilepath + "Plain_Tiles.png");
            animReader.GetTransparentColor(anim, 1, 1);
            animReader.spriteWidth = 16;
            animReader.spriteHeight = 16;
            animReader.ReadImage(anim, 6, 16, 0, 0);
            for (int y = 0; y < animReader.Images.Count; y++)
            {
                for (int x = 0; x < animReader.Images[y].Count; x++)
                {
                    images.Add(animReader.Images[y][x]);
                }
            }
            animator.AddFrame(images[77], @"", new SizeF(images[11].Width*2, images[11].Height*2));
            this.AnimatorList.Add(animator);
            setAnimation(0);
            this.Properties.CollisionRect.Size = AnimatorList[0].AnimSize;
        }

        public override bool OtherCollided(Core.Sprite.Sprite sprite, Core.Sprite.SpriteProperties.Directions collidedDirection, PointF CameraLocation)
        {
            if (collidedDirection == Core.Sprite.SpriteProperties.Directions.Left || collidedDirection == Core.Sprite.SpriteProperties.Directions.Right)
            {
                sprite.NoCollided();
            }
            if (collidedDirection == Core.Sprite.SpriteProperties.Directions.Down)
            {
                if (sprite.Properties.VelocityRect.Y < 0) sprite.Properties.VelocityRect.Y = -sprite.Properties.VelocityRect.Y;
            }
            return base.OtherCollided(sprite, collidedDirection, CameraLocation);
        }
    }
}
