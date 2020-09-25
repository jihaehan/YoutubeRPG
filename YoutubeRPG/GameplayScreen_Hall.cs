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
    public class GameplayScreen_Hall : GameScreen
    {
        Player player;
        World world;
        Camera camera;
        MenuManager menuManager;
        ConversationManager conversationManager;
        List<SPX> spxImage;
        SPXManager spxManager;

        public override void LoadContent()
        {
            base.LoadContent();
            XmlManager<Player> playerLoader = new XmlManager<Player>();
            XmlManager<World> worldLoader = new XmlManager<World>();
            player = playerLoader.Load("Content/Load/Gameplay/Player.xml");
            player.LoadContent();
            world = worldLoader.Load("Content/Load/Gameplay/World/City_Heal.xml");
            world.LoadContent();
            player.Image.Position = world.CurrentMap.StartingPoint;
            player.Image.SpriteSheetEffect.CurrentFrame.Y = 0;
            player.Keys.Add("GameplayScreen_Blue");
            if (ScreenManager.Instance.Party.Count > 0)
            {
                player.ChemicalManager.LoadParty();
                foreach (string name in player.ChemicalManager.chemicalName)
                {
                    player.ChemicalManager.GetChemical(name).Image.FadeEffect.IsActive = false;
                    player.ChemicalManager.GetChemical(name).Image.SpriteSheetEffect.IsActive = true;
                    player.ChemicalManager.GetChemical(name).Image.Alpha = 1.0f;
                }
            }

            camera = new Camera();
            menuManager = new MenuManager();
            menuManager.LoadContent("Content/Load/Menu/GameplayMenu.xml");
            conversationManager = new ConversationManager();
            conversationManager.LoadContent("Content/Load/Conversation/Intro.xml");
            InitializeBindings();

            spxManager = new SPXManager();
            spxImage = new List<SPX>();

        }
        public override void UnloadContent()
        {
            base.UnloadContent();
            ScreenManager.Instance.PlayerPosition = new Vector2(16, 12) * 128;
            //TO DELETE LATER...
            player.ChemicalManager.HealParty();
            player.ChemicalManager.SaveParty();
            player.UnloadContent();
            world.UnloadContent();
            menuManager.UnloadContent();
            conversationManager.UnloadContent();
            foreach (SPX spx in spxImage)
                spx.UnloadContent();
        }
        public override void Update(GameTime gameTime)
        {
            player.Velocity = Vector2.Zero;
            base.Update(gameTime);
            conversationManager.Update(gameTime, ref player);
            player.Update(gameTime, world);
            world.Update(gameTime);
            menuManager.Update(gameTime, ref player);
            foreach (SPX spx in spxImage)
                spx.Update(gameTime);

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
            player.Image.Draw(spriteBatch);
            foreach (SPX spx in spxImage)
                spx.Draw(spriteBatch);
            world.Draw(spriteBatch, "Overlay");

            spriteBatch.End();

            spriteBatch.Begin();
            menuManager.Draw(spriteBatch);
            conversationManager.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void InitializeBindings()
        {
            InputManager.AddKeyboardBinding(Keys.W, Toggle_Up);
            InputManager.AddKeyboardBinding(Keys.S, Toggle_Down);
            InputManager.AddKeyboardBinding(Keys.A, Toggle_Left);
            InputManager.AddKeyboardBinding(Keys.D, Toggle_Right);
            InputManager.AddKeyboardBinding(Keys.Enter, Toggle_Select);
            InputManager.AddKeyboardBinding(Keys.X, menuManager.PrevMenuSelect);
        }
        private void Toggle_Select(eButtonState buttonState)
        {
            if (!conversationManager.IsActive && player.DialoguePathName() != String.Empty)
            {
                conversationManager.LoadContent(player.DialoguePathName());
                conversationManager.Activate(buttonState);
                player.IsNPC = false;
            }
            else if (conversationManager.IsActive)
                conversationManager.MenuSelect(buttonState);
            else
                menuManager.MenuSelect_Test(buttonState);
        }

        private void Toggle_Up(eButtonState buttonState)
        {
            if (conversationManager.IsActive)
                conversationManager.SelectUp(buttonState);
            else if (menuManager.IsActive)
                menuManager.SelectUp(buttonState);
            else
                player.MoveUp(buttonState);
        }
        private void Toggle_Down(eButtonState buttonState)
        {

            if (conversationManager.IsActive)
                conversationManager.SelectDown(buttonState);
            else if (menuManager.IsActive)
                menuManager.SelectDown(buttonState);
            else
                player.MoveDown(buttonState);
        }
        private void Toggle_Left(eButtonState buttonState)
        {
            if (conversationManager.IsActive)
                conversationManager.SelectLeft(buttonState);
            else if (menuManager.IsActive)
                menuManager.SelectLeft(buttonState);
            else
                player.MoveLeft(buttonState);
        }
        private void Toggle_Right(eButtonState buttonState)
        {
            if (conversationManager.IsActive)
                conversationManager.SelectRight(buttonState);
            else if (menuManager.IsActive)
                menuManager.SelectRight(buttonState);
            else
                player.MoveRight(buttonState);
        }

        private void AddKey(eButtonState buttonState)
        {
            player.Keys.Add("GameplayScreen_Blue");
            player.Keys.Add("GameplayScreen_Health");
        }


    }
}
