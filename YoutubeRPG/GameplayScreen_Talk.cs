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
    public class GameplayScreen_Talk : GameScreen
    {
        Player player;
        World world;
        Camera camera;
        MenuManager menuManager;
        //DialogueManager dialogue;
        ConversationManager dialogue;

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
            dialogue = new ConversationManager();
            dialogue.LoadContent("Content/Load/Conversation/Intro.xml");
            player.Image.Position = world.CurrentMap.StartingPoint;
            InitializeBindings();

            //TEST:Dialogue
            dialogue = new ConversationManager();
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
            player.UnloadContent();
            world.UnloadContent();
            menuManager.UnloadContent();
            dialogue.UnloadContent();

        }
        public override void Update(GameTime gameTime)
        {
            player.Velocity = Vector2.Zero;
            base.Update(gameTime);
            player.Update(gameTime, world);
            //chemical.Update(gameTime);
            world.Update(gameTime);
            menuManager.Update(gameTime, ref player);
            dialogue.Update(gameTime, ref player);
            camera.LockToSprite(world.CurrentMap.Layer[0], player.Image);
            //TEST:character
            //mole.Update(gameTime);
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
            //chemical.Draw(spriteBatch);
            //mole.Draw(spriteBatch);
            world.Draw(spriteBatch, "Overlay");
            spriteBatch.End();

            spriteBatch.Begin();
            menuManager.Draw(spriteBatch);
            dialogue.Draw(spriteBatch);
            spriteBatch.End();
        }
        private void InitializeBindings()
        {
            InputManager.AddKeyboardBinding(Keys.W, Toggle_Up);
            InputManager.AddKeyboardBinding(Keys.S, Toggle_Down);
            InputManager.AddKeyboardBinding(Keys.A, Toggle_Left);
            InputManager.AddKeyboardBinding(Keys.D, Toggle_Right);
            InputManager.AddKeyboardBinding(Keys.Enter, Toggle_Select);
            InputManager.AddKeyboardBinding(Keys.J, Toggle_Activate);
            InputManager.AddKeyboardBinding(Keys.X, menuManager.PrevMenuSelect);
        }
        private void Toggle_Select(eButtonState buttonState)
        {
            if (!dialogue.IsActive)
                dialogue.Activate(buttonState);
            else if (dialogue.IsActive)
                dialogue.MenuSelect(buttonState);
            else 
                menuManager.MenuSelect_Test(buttonState);
        }
        private void Toggle_Activate(eButtonState buttonState)
        {
            if (!dialogue.IsActive)
                dialogue.Activate(buttonState);
            else
                menuManager.Activate(buttonState);
        }

        private void Toggle_Up(eButtonState buttonState)
        {
            if (dialogue.IsActive)
                dialogue.SelectUp(buttonState);
            else if (menuManager.IsActive)
                menuManager.SelectUp(buttonState);
            else
                player.MoveUp(buttonState);
        }
        private void Toggle_Down(eButtonState buttonState)
        {
            if (dialogue.IsActive)
                dialogue.SelectDown(buttonState);
            else if (menuManager.IsActive)
                menuManager.SelectDown(buttonState);
            else               
                player.MoveDown(buttonState);
        }
        private void Toggle_Left(eButtonState buttonState)
        {
            if (dialogue.IsActive)
                dialogue.SelectLeft(buttonState);
            else if (menuManager.IsActive)
                menuManager.SelectLeft(buttonState);
            else
                player.MoveLeft(buttonState);
        }
        private void Toggle_Right(eButtonState buttonState)
        {
            if (dialogue.IsActive)
                dialogue.SelectRight(buttonState);
            else if (menuManager.IsActive)
                menuManager.SelectRight(buttonState);
            else
                player.MoveRight(buttonState);
        }
    }
}
