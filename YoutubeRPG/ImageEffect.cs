﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace YoutubeRPG
{
    public class ImageEffect
    {
        protected Image Image;
        public bool IsActive;
        public ImageEffect()
        {
            IsActive = false; 
        }
        public virtual void LoadContent(ref Image Image)
        {
            this.Image = Image;
        }
        public virtual void UnloadContent()
        {

        }
        public virtual void Update(GameTime gameTime)
        {

        }
    }
}
