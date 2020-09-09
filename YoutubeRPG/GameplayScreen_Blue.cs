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
    public class GameplayScreen_Blue : GameScreen
    {
        Player player;
        World world;
        Camera camera;
        MenuManager menuManager;
        Character mole;
        Character mark; 

        Chemical chemical;
        
        public override void LoadContent()
        {
            base.LoadContent();
            XmlManager<Player> playerLoader = new XmlManager<Player>();
            XmlManager<World> worldLoader = new XmlManager<World>();
            player = playerLoader.Load("Content/Load/Gameplay/Player.xml");
            player.LoadContent();
            //If player save exists, load Save files here
            world = worldLoader.Load("Content/Load/Gameplay/World/Blue_Test.xml");
            world.LoadContent();

            camera = new Camera();
            menuManager = new MenuManager();
            menuManager.LoadContent("Content/Load/Menu/GameplayMenu.xml");

            player.Image.Position = world.CurrentMap.StartingPoint;
            InitializeBindings();

            //TEST:chemical
            XmlManager<Chemical> chemicalLoader = new XmlManager<Chemical>();
            chemical = chemicalLoader.Load("Content/Load/Chemical/Alkane/Pentane.xml");
            chemical.LoadContent();
            chemical.Image.Position = player.Image.Position + new Vector2(128, 128);

            //TEST:character
            XmlManager<Character> characterLoader = new XmlManager<Character>();
            mole = characterLoader.Load("Content/Load/Gameplay/Mole.xml");
            mole.LoadContent();
            mole.Image.Position = player.Image.Position + new Vector2(128, -128);
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
            player.UnloadContent();
            chemical.UnloadContent();
            world.UnloadContent();
            menuManager.UnloadContent();

            //TEST:character
            mole.UnloadContent();
        }
        public override void Update(GameTime gameTime)
        {
            player.Velocity = Vector2.Zero;
            base.Update(gameTime);
            player.Update(gameTime, world);
            chemical.Update(gameTime);
            world.Update(gameTime);
            menuManager.Update(gameTime);
            menuManager.Update(gameTime, ref player);

            camera.LockToSprite(world.CurrentMap.Layer[0], player.Image);

            //TEST:character
            mole.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            spriteBatch.Begin();
            world.CurrentMap.Background(spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.Transformation);
            world.Draw(spriteBatch, "Underlay");
            player.Draw(spriteBatch);
            chemical.Draw(spriteBatch);
            mole.Draw(spriteBatch);
            world.Draw(spriteBatch, "Overlay");
            spriteBatch.End();

            spriteBatch.Begin();
            menuManager.Draw(spriteBatch);
            spriteBatch.End();
        }
        private void InitializeBindings()
        {
            InputManager.AddKeyboardBinding(Keys.W, Toggle_Up);
            InputManager.AddKeyboardBinding(Keys.S, Toggle_Down);
            InputManager.AddKeyboardBinding(Keys.A, Toggle_Left);
            InputManager.AddKeyboardBinding(Keys.D, Toggle_Right);
            InputManager.AddKeyboardBinding(Keys.Enter, menuManager.MenuSelect_Test);
            InputManager.AddKeyboardBinding(Keys.J, menuManager.Activate);
            InputManager.AddKeyboardBinding(Keys.X, menuManager.PrevMenuSelect);
        }

        private void Toggle_Up(eButtonState buttonState)
        {
            if (menuManager.IsActive)
                menuManager.SelectUp(buttonState);
            else
                player.MoveUp(buttonState);
        }
        private void Toggle_Down(eButtonState buttonState)
        {
            if (menuManager.IsActive)
                menuManager.SelectDown(buttonState);
            else
                player.MoveDown(buttonState);
        }
        private void Toggle_Left(eButtonState buttonState)
        {
            if (menuManager.IsActive)
                menuManager.SelectLeft(buttonState);
            else
                player.MoveLeft(buttonState);
        }
        private void Toggle_Right(eButtonState buttonState)
        {
            if (menuManager.IsActive)
                menuManager.SelectRight(buttonState);
            else
                player.MoveRight(buttonState);
        }
    }
}
