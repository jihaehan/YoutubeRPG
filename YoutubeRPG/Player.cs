using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace YoutubeRPG
{
    public class Player
    {
        public Image Image;
        public Vector2 Velocity;
        public float MoveSpeed;
        public Player()
        {
            Velocity = Vector2.Zero;
        }
        public void LoadContent()
        {
            Image.LoadContent();
        }
        public void UnloadContent()
        {
            Image.UnloadContent();
        }
        public void Update(GameTime gameTime)
        {
            Image.IsActive = true;

            if (InputManager.Instance.KeyDown(Keys.S))
            {
                if (Velocity.X == 0)
                {
                    Velocity.Y = MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Image.SpriteSheetEffect.CurrentFrame.Y = 1;
                }
                else
                {
                    Velocity.Y = 0.7071f * MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (Velocity.X > 0)
                        Image.SpriteSheetEffect.CurrentFrame.Y = 5;
                    else if (Velocity.X < 0)
                        Image.SpriteSheetEffect.CurrentFrame.Y = 4;
                }
            }
            else if (InputManager.Instance.KeyDown(Keys.W))
            {
                if (Velocity.X == 0)
                {
                    Velocity.Y = -MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Image.SpriteSheetEffect.CurrentFrame.Y = 0;
                }
                else
                {
                    Velocity.Y = -0.7071f * MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (Velocity.X > 0)
                        Image.SpriteSheetEffect.CurrentFrame.Y = 7;
                    else if (Velocity.X < 0)
                        Image.SpriteSheetEffect.CurrentFrame.Y = 6;

                }
            }
            else
                Velocity.Y = 0;

            if (InputManager.Instance.KeyDown(Keys.D))
            {
                if (Velocity.Y == 0)
                {
                    Velocity.X = MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Image.SpriteSheetEffect.CurrentFrame.Y = 2;
                }
                else
                    Velocity.X = 0.7071f * MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (InputManager.Instance.KeyDown(Keys.A))
            {
                if (Velocity.Y == 0)
                {
                    Velocity.X = -MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Image.SpriteSheetEffect.CurrentFrame.Y = 3;
                }
                else
                    Velocity.X = -0.7071f * MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
                Velocity.X = 0;

            if (Velocity.X == 0 && Velocity.Y == 0)
                Image.IsActive = false; 

            Image.Update(gameTime);
            Image.Position += Velocity; 
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Image.Draw(spriteBatch);
        }

    }
}
