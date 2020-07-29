using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace YoutubeRPG
{
    public class FadeEffect : ImageEffect
    {
        public float FadeSpeed;
        public bool Increase;

        public FadeEffect()
        {
            FadeSpeed = 1;
            Increase = false; 
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
                if (!Increase)
                    Image.Alpha -= FadeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                else
                    Image.Alpha += FadeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (Image.Alpha < 0.0f)
                {
                    Increase = true;
                    Image.Alpha = 0.0f;
                }
                else if (Image.Alpha > 1.0f)
                {
                    Increase = false;
                    Image.Alpha = 1.0f;
                }
            }
            else
                Image.Alpha = 1.0f;
        }
    }
}
