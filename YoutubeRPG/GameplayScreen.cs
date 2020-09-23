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
    public class GameplayScreen : GameScreen
    {
        Player player;
        World world; 
        Camera camera;
        MenuManager menuManager;
        ConversationManager conversationManager;
        Character methane;
        List<SPX> spxImage;
        SPXManager spxManager;
        int planCount = 0;
        bool moveLeft;
        bool moveUp;
        bool moveRight;
        bool moved;
        bool leaveRoom;
        Vector2 pt1, pt2, pt3, pt4, pt5;

        public override void LoadContent()
        {
            base.LoadContent();           
            XmlManager<Player> playerLoader = new XmlManager<Player>();
            XmlManager<World> worldLoader = new XmlManager<World>();
            player = playerLoader.Load("Content/Load/Gameplay/Player.xml");
            player.LoadContent();
            world = worldLoader.Load("Content/Load/Gameplay/World/Intro.xml");
            world.LoadContent();
            player.Image.Position = world.CurrentMap.StartingPoint;

            camera = new Camera();
            menuManager = new MenuManager();
            menuManager.LoadContent("Content/Load/Menu/GameplayMenu.xml");
            menuManager.SetIntroduction();
            conversationManager = new ConversationManager();
            conversationManager.LoadContent("Content/Load/Conversation/Introduction.xml");
            InitializeBindings();

            XmlManager<Character> characterLoader = new XmlManager<Character>();
            methane = new Character();
            methane = characterLoader.Load("Content/Load/Gameplay/Methane.xml");
            methane.LoadContent();
            methane.Image.Position = new Vector2(7.5f,4f) * 128;

            spxManager = new SPXManager();
            spxImage = new List<SPX>();
            moveLeft = false;
            moveUp = false;
            moveRight = false;
            moved = false;
            leaveRoom = false;
            pt1 = new Vector2(7.5f, 2f) * 128;
            pt2 = new Vector2(5f, 2f) * 128;
            pt3 = new Vector2(5f, -1f) * 128;
            pt4 = new Vector2(4f, 5f) * 128;
            pt5 = new Vector2(8f, 5f) * 128;

        }
        public override void UnloadContent()
        {
            base.UnloadContent();
            methane.UnloadContent();
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
            if (moveLeft)
            {
                if (methane.Image.Position.Y > pt1.Y)
                    methane.Image.Position.Y -= 200 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                else if (methane.Image.Position.X > pt2.X)
                    methane.Image.Position.X -= 200 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                else
                    moveLeft = false;
            }
            if (moveUp)
            {
                if (methane.Image.Position.Y > pt3.Y)
                    methane.Image.Position.Y -= 200 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                else
                    moveUp = false;
            }
            if (world.CurrentMapName.Contains("Room1_3") && !moved)
            {
                moveRight = true;
                moved = true;
                methane.Image.Position.Y = 7 * 128;
                methane.Image.SpriteSheetEffect.CurrentFrame.Y = 0;
            }
            if (moveRight)
            {
                if (methane.Image.Position.Y > pt4.Y)
                    methane.Image.Position.Y -= 200 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                else if (methane.Image.Position.X < pt5.X)
                    methane.Image.Position.X += 200 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                else
                {
                    if (methane.Image.Alpha > 0)
                        methane.Image.Alpha -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    else
                    {
                        methane.Image.Alpha = 0;
                        moveRight = false;
                    }
                }
            }
            methane.Update(gameTime);
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
            if (world.CurrentMapName.Contains("_"))
                methane.Draw(spriteBatch);
            foreach (SPX spx in spxImage)
                spx.Draw(spriteBatch);
            world.Draw(spriteBatch, "Overlay");

            spriteBatch.End();

            spriteBatch.Begin();
            menuManager.Draw(spriteBatch);
            conversationManager.DrawIntroduction(spriteBatch);
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
            if (!conversationManager.IsActive && player.DialoguePathName() != String.Empty)
            {
                conversationManager.LoadContent(player.DialoguePathName());
                conversationManager.Activate(buttonState);
                player.IsNPC = false;
            }
            else if (conversationManager.IsActive && conversationManager.DialoguePath != String.Empty)
            {
                conversationManager.MenuSelect(buttonState);
                if (conversationManager.DialoguePath == "Methane")
                {
                    world.ChangeMap("Room1_1");
                    methane.Image.IsVisible = true;
                    moveUp = true;
                    conversationManager.Activate(buttonState);
                    AddKey(buttonState);
                }
            }
            else if (conversationManager.IsActive && conversationManager.DialoguePath == String.Empty)
            {
                conversationManager.MenuSelect_Intro(buttonState);
                
                if (conversationManager.DialogueLinkType != String.Empty && !conversationManager.DialogueLinkType.Contains(".xml"))
                    switch (conversationManager.DialogueLinkType)
                    {
                        case "01": //the explosion
                            world.ChangeMap("Room1_1");
                            if (!methane.Image.Effects.Contains("FadeEffect"))
                            {
                                methane.Image.Effects += "FadeEffect";
                                methane.Image.LoadContent();
                                methane.Image.FadeEffect.IsActive = true;
                            }
                            if (spxImage.Count < 1)
                                spxImage.Add(new SPX(spxManager.BranchingXml(), methane.Image.Position));
                            break;
                        case "0111": //menumanager
                            if (!menuManager.IsActive)
                                menuManager.Activate(buttonState);
                            else if (menuManager.IsActive)
                                menuManager.MenuSelect_Test(buttonState);
                            break;
                        case "01111":  //infoMenu
                            if (!menuManager.Type().Contains("ChemicalInfo"))
                                menuManager.MenuSelect_Test(buttonState);
                            else if (!menuManager.Type().Contains("PropertyInfo"))
                                menuManager.SelectRight(buttonState);
                            break;
                        case "011111":  //property menu
                                menuManager.SelectRight(buttonState); 
                            break;
                        case "0111111": //ITEMS menu
                            if (!menuManager.ID().Contains("Gameplay"))
                            {
                                menuManager.PrevMenuSelect(buttonState);
                                menuManager.PrevMenuSelect(buttonState);
                            }
                            menuManager.SelectDown(buttonState);
                            break;
                        case "01111111":  //property menu
                            menuManager.MenuSelect_Test(buttonState);
                            for (int count = 0; count < 3; count++)
                                menuManager.SelectDown(buttonState);
                            break;
                        case "0111111111": //BookMenu
                            if (!menuManager.ID().Contains("Gameplay"))
                            {
                                menuManager.PrevMenuSelect(buttonState);
                                menuManager.PrevMenuSelect(buttonState);
                            }
                            menuManager.SelectRight(buttonState);
                            break;
                        case "01111111111":
                            menuManager.MenuSelect_Test(buttonState);
                            menuManager.MenuSelect_Test(buttonState);
                            break;
                        case "011111111111":
                            if (planCount == 0)
                            {
                                menuManager.PrevMenuSelect(buttonState);
                                menuManager.PrevMenuSelect(buttonState);
                                menuManager.SelectUp(buttonState);
                                planCount++;
                            }
                            else if (planCount > 0)
                                menuManager.MenuSelect_Test(buttonState);
                            break;
                        case "0111111111111": //Move chemical here 
                            if (menuManager.IsActive)
                            {
                                menuManager.PrevMenuSelect(buttonState);
                                menuManager.PrevMenuSelect(buttonState);
                            }
                            methane.Image.SpriteSheetEffect.CurrentFrame.Y = 0;
                            moveLeft = true;
                            break;
                        case "01111111111111": //Talk to chemical quest
                            methane.Image.IsVisible = false;
                            world.ChangeMap("Room1_11");
                            break;
                        default:
                            if (methane.Image.Effects.Contains("FadeEffect"))
                                methane.Image.FadeEffect.IsActive = false;
                            methane.Image.Alpha = 1.0f;
                            break;
                    }
            }
            else
                menuManager.MenuSelect_Test(buttonState);
        }
        private void Toggle_Activate(eButtonState buttonState)
        {
            conversationManager.Activate(buttonState);
            //menuManager.Activate(buttonState);
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
        }
        private void ChangeMap1(eButtonState buttonState)
        {
            if (buttonState == eButtonState.DOWN)
            {
                world.ChangeMap("Room1_1");
            }
        }
        private void ChangeMap2(eButtonState buttonState)
        {
            if (buttonState == eButtonState.DOWN)
            {
                world.ChangeMap("Room1");
            }
        }
        
    }
}
