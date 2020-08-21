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
        bool leaveRoom = false;
        public override void LoadContent()
        {
            base.LoadContent();           
            XmlManager<Player> playerLoader = new XmlManager<Player>();
            XmlManager<World> worldLoader = new XmlManager<World>();
            player = playerLoader.Load("Content/Load/Gameplay/Player.xml");
            player.LoadContent();
            world = worldLoader.Load("Content/Load/Gameplay/World/Intro.xml");
            world.LoadContent();

            camera = new Camera();
            player.Image.Position = world.CurrentMap.StartingPoint;

            InitializeBindings();
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
            player.UnloadContent();
            world.UnloadContent();
        }
        public override void Update(GameTime gameTime)
        {
            player.Velocity = Vector2.Zero;
            base.Update(gameTime);
            player.Update(gameTime, world.CurrentMap);
            world.Update(gameTime);

            camera.LockToSprite(world.CurrentMap.Layer[0], player.Image);

            //Leave Room to above
            //Portal Manager
            if (player.Image.Position.Y < 64 && world.CurrentMapName == "Room1" && leaveRoom == false)
            {
                ScreenManager.Instance.FadeScreen();
                leaveRoom = true;
            }
            if (ScreenManager.Instance.IsFadeEffect)
            {
                ScreenManager.Instance.Image.Update(gameTime);
                if (ScreenManager.Instance.Image.Alpha == 1.0f)
                {
                    world.ChangeMap("Room1_2");
                    player.Image.Position = world.CurrentMap.StartingPoint + new Vector2(0, 64);
                    player.Image.SpriteSheetEffect.CurrentFrame.Y = 0; 
                    leaveRoom = false;
                }
                else if (ScreenManager.Instance.Image.Alpha == 0.0f)
                {
                    ScreenManager.Instance.Image.IsActive = false;
                    ScreenManager.Instance.IsFadeEffect = false;
                }
            }

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
            world.Draw(spriteBatch, "Overlay");

            spriteBatch.End();
        }
        private void InitializeBindings()
        {
            InputManager.AddKeyboardBinding(Keys.W, player.MoveUp);
            InputManager.AddKeyboardBinding(Keys.S, player.MoveDown);
            InputManager.AddKeyboardBinding(Keys.A, player.MoveLeft);
            InputManager.AddKeyboardBinding(Keys.D, player.MoveRight);
            InputManager.AddKeyboardBinding(Keys.F1, ChangeMap1);
            InputManager.AddKeyboardBinding(Keys.F2, ChangeMap2);
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
