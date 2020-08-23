using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace YoutubeRPG
{
    public class HighlightEffect : ImageEffect
    {
        public float Speed;
        public bool Increase;

        public HighlightEffect()
        {
            Speed = 1;
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
                Image.TextColor = Color.Blue;
            }
            else
            {
                Image.TextColor = Color.Yellow;
            }
        }
    }
}
