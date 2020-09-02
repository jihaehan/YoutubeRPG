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

        Chemical chemical;
        ChemicalManager chemicalManager;

        public override void LoadContent()
        {
            base.LoadContent();
            XmlManager<Player> playerLoader = new XmlManager<Player>();
            XmlManager<World> worldLoader = new XmlManager<World>();
            player = playerLoader.Load("Content/Load/Gameplay/Player.xml");
            player.LoadContent();
            //If player save exists, load Save files here
            world = worldLoader.Load("Content/Load/Gameplay/World/Blue.xml");
            world.LoadContent();

            camera = new Camera();
            menuManager = new MenuManager();
            menuManager.LoadContent("Content/Load/Menu/OptionMenu.xml");
            //menuManager.LoadContent("Content/Load/Menu/InfoMenu.xml");
            //menuManager.LoadContent("Content/Load/Menu/GameplayMenu.xml");

            player.Image.Position = world.CurrentMap.StartingPoint;
            InitializeBindings();

            //TEST:chemical
            XmlManager<Chemical> chemicalLoader = new XmlManager<Chemical>();
            chemical = chemicalLoader.Load("Content/Load/Chemical/Alkane/Pentane.xml");
            chemical.LoadContent();
            chemical.Image.Position = player.Image.Position + new Vector2(128, 128);

            //TEST:chemicalManager
            XmlManager<ChemicalManager> chemicalManagerLoader = new XmlManager<ChemicalManager>();
            //if Party saves exist, load Save files here
            chemicalManager = chemicalManagerLoader.Load("Content/Load/Gameplay/Party.xml");
            chemicalManager.LoadContent(player.Image.Position);
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
            player.UnloadContent();
            chemical.UnloadContent();
            world.UnloadContent();
            menuManager.UnloadContent();
            chemicalManager.UnloadContent();
        }
        public override void Update(GameTime gameTime)
        {
            player.Velocity = Vector2.Zero;
            base.Update(gameTime);
            player.Update(gameTime, world);
            chemical.Update(gameTime);
            world.Update(gameTime);
            menuManager.Update(gameTime, ref chemicalManager);
            chemicalManager.Update(gameTime, player);

            camera.LockToSprite(world.CurrentMap.Layer[0], player.Image);

        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            spriteBatch.Begin();
            world.CurrentMap.Background(spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.Transformation);
            world.Draw(spriteBatch, "Underlay");
            chemicalManager.Draw(spriteBatch);
            player.Draw(spriteBatch);
            chemical.Draw(spriteBatch);
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
