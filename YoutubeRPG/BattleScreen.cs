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
        Player player;
        Character enemy;
        BattleManager battleManager;

        public BattleScreen()
        {
            battleManager = new BattleManager();
            specialEffects = new List<Image>();
            background = new Image();

            //Battle Components
            player = new Player();
            enemy = new Character();
        }
        public override void LoadContent()
        {
            base.LoadContent();

            XmlManager<Player> playerLoader = new XmlManager<Player>();
            player = playerLoader.Load("Content/Load/Gameplay/Player.xml");
            player.LoadContent();
            player.Image.Position = new Vector2(128, 360);
            player.Image.SpriteSheetEffect.CurrentFrame.Y = 7;
            player.Image.SpriteSheetEffect.SwitchFrame = 500;
            player.Image.IsActive = true;
            player.InitializeBattle();

            XmlManager<Character> characterLoader = new XmlManager<Character>();
            enemy = characterLoader.Load("Content/Load/Gameplay/Markovnikov.xml");
            enemy.LoadContent("Content/Load/Gameplay/Battle/Markovnikov.xml");
            enemy.Image.Position = new Vector2(1064, 175);
            enemy.Image.SpriteSheetEffect.CurrentFrame.Y = 1;
            enemy.Image.IsActive = true;
            enemy.InitializeBattle();

            //Setup background, menu, etc.
            background.Path = "BattleBackground/Interior";
            background.Position = Vector2.Zero;
            background.FontName = "Fonts/OCRAExt";
            background.LoadContent();

            //Setup BattleManager
            battleManager = new BattleManager();
            battleManager.LoadContent("Content/Load/Menu/BattleMenu.xml");

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
            player.BattleUpdate(gameTime);
            enemy.BattleUpdate(gameTime);
            battleManager.Update(gameTime, ref player, ref enemy.ChemicalManager);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.Begin();
            background.Draw(spriteBatch);
            enemy.BattleDraw(spriteBatch);
            player.BattleDraw(spriteBatch);
            //Draw special effects here
            //PLACEHOLDER CODE
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
