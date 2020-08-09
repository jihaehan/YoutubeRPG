using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace YoutubeRPG
{
    public class SpriteSheetEffect : ImageEffect
    {
        public int FrameCounter;
        public int SwitchFrame;
        public Vector2 CurrentFrame;
        public Vector2 AmountOfFrames;

        public int FrameWidth
        {
            get 
            {
                if (Image.Texture != null)
                    return Image.Texture.Width / (int)AmountOfFrames.X;
                return 0;
            }
        }
        public int FrameHeight
        {
            get
            {
                if (Image.Text != null)
                    return Image.Texture.Height / (int)AmountOfFrames.Y;
                return 0; 
            }
        }
        public SpriteSheetEffect()
        {
            AmountOfFrames = new Vector2(2, 8);
            CurrentFrame = new Vector2(0, 1);
            SwitchFrame = 100;
            FrameCounter = 0; 
        }

        public override void LoadContent(ref Image Image)
        {
            base.LoadContent(ref Image);
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Image.IsActive)
            {
                FrameCounter += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (FrameCounter >= SwitchFrame)
                {
                    FrameCounter = 0;
                    CurrentFrame.X++;

                    if (CurrentFrame.X * FrameWidth >= Image.Texture.Width)
                        CurrentFrame.X = 0;
                }
            }
            else
                CurrentFrame.X = 1;

            Image.SourceRect = new Rectangle((int)CurrentFrame.X * FrameWidth,
                (int)CurrentFrame.Y * FrameHeight, FrameWidth, FrameHeight); 
                
        }
    }
}
