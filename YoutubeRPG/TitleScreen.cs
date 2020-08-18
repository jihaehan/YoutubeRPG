using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace YoutubeRPG
{
    public class TitleScreen : GameScreen
    {
        MenuManager menuManager;

        public TitleScreen()
        {
            menuManager = new MenuManager();
        }
        public override void LoadContent()
        {
            base.LoadContent();
            menuManager.LoadContent("Content/Load/Menu/TitleMenu.xml");
            InitializeBindings();
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
            menuManager.UnloadContent();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            menuManager.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            spriteBatch.Begin();
            menuManager.Draw(spriteBatch);
            spriteBatch.End();
        }
        private void InitializeBindings()
        {
            InputManager.AddKeyboardBinding(Keys.Enter, menuManager.MenuSelect);
            InputManager.AddKeyboardBinding(Keys.W, menuManager.SelectUp);
            InputManager.AddKeyboardBinding(Keys.S, menuManager.SelectDown);
            InputManager.AddKeyboardBinding(Keys.A, menuManager.SelectLeft);
            InputManager.AddKeyboardBinding(Keys.D, menuManager.SelectRight);
        }

    }
}