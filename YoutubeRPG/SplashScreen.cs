using System;
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

        public override void LoadContent()
        {
            base.LoadContent();
            Image.LoadContent();
            InitializeBindings();
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
            Image.UnloadContent();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Image.Update(gameTime);

            //if (InputManager.Instance.KeyPressed(Keys.Enter, Keys.Z))
                //ScreenManager.Instance.ChangeScreens("TitleScreen");
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            Image.Draw(spriteBatch);
        }

        private void InitializeBindings()
        {
            InputManager.AddKeyboardBinding(Keys.Z, ChangeScreen);
        }
        public void ChangeScreen(eButtonState buttonstate)
        {
            if (buttonstate == eButtonState.DOWN)
            {
                ScreenManager.Instance.ChangeScreens("TitleScreen");
            }
        }

    }
}
