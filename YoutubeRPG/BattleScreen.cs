using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace YoutubeRPG
{
    public class BattleScreen : GameScreen 
    {
        List<Image> specialEffects;
        Image background;
        BattleManager battleManager;

        public BattleScreen()
        {
            battleManager = new BattleManager();
            specialEffects = new List<Image>();
            background = new Image();

        }
        public override void LoadContent()
        {
            base.LoadContent();

            //Setup background, menu, etc.
            background.Path = "BattleBackground/Interior";
            background.Position = Vector2.Zero;
            background.FontName = "Fonts/OCRAExt";
            background.LoadContent();

            //Setup BattleManager
            battleManager = new BattleManager();
            battleManager.LoadContent("Content/Load/Gameplay/Markovnikov.xml", "Content/Load/Gameplay/Battle/Markovnikov.xml");

            InitializeBindings();

        }
        public override void UnloadContent()
        {
            base.UnloadContent();
            background.UnloadContent();
            foreach (Image spx in specialEffects)
                spx.UnloadContent();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            battleManager.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.Begin();
            background.Draw(spriteBatch);
            battleManager.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void InitializeBindings()
        {
            InputManager.AddKeyboardBinding(Keys.W, battleManager.SelectUp);
            InputManager.AddKeyboardBinding(Keys.S, battleManager.SelectDown);
            InputManager.AddKeyboardBinding(Keys.A, battleManager.SelectLeft);
            InputManager.AddKeyboardBinding(Keys.D, battleManager.SelectRight);
            InputManager.AddKeyboardBinding(Keys.Enter, battleManager.MenuSelect_Test);
            InputManager.AddKeyboardBinding(Keys.X, battleManager.PrevMenuSelect);
        }
    }
}
