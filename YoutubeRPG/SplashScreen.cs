﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace YoutubeRPG
{
    
    public class SplashScreen : GameScreen
    {
        public Image Image;
        public Image TextImage;

        public override void LoadContent()
        {
            base.LoadContent();
            Image.LoadContent();
            TextImage.LoadContent();
            InitializeBindings();
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
            Image.UnloadContent();
            TextImage.UnloadContent();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Image.Update(gameTime);
            TextImage.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            spriteBatch.Begin();
            Image.Draw(spriteBatch);
            TextImage.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void InitializeBindings()
        {
            InputManager.AddKeyboardBinding(Keys.Enter, ChangeScreen);
        }
        private void ChangeScreen(eButtonState buttonstate)
        {
            if (buttonstate == eButtonState.DOWN)
            {
                ScreenManager.Instance.ChangeScreens("TitleScreen");
            }
        }

    }
}
