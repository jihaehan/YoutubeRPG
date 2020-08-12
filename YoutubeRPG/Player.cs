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
        public void Update(GameTime gameTime, Layer collisionLayer)
        {
            Image.IsActive = true;

            Velocity = Vector2.Zero;
            HandleMovement(gameTime);
            if (Velocity != Vector2.Zero)   
                HandleCollisions(collisionLayer);

            Image.Update(gameTime);
            Image.Position += Velocity; 
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Image.Draw(spriteBatch);
        }
        void HandleMovement(GameTime gameTime)
        {

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
        }
        void HandleCollisions(Layer layer)
        {
            Circle boundingCircle = new Circle(new Vector2(
                Image.Position.X + Image.SourceRect.Width / 2 + Velocity.X,
                Image.Position.Y + Image.SourceRect.Height / 2 + Velocity.Y),
                Image.SourceRect.Width/2);
            Rectangle boundingBox = new Rectangle(
                (int)(Image.Position.X + Velocity.X), (int)(Image.Position.Y + Velocity.Y),
                Image.SourceRect.Width, Image.SourceRect.Height);

            int leftTile = (int)Math.Floor((float) (Image.Position.X)/ Image.SourceRect.Width);
            int rightTile = (int)Math.Ceiling((float) (Image.SourceRect.Width + Image.Position.X) / Image.SourceRect.Width);
            int topTile = (int)Math.Floor((float) (Image.Position.Y) / Image.SourceRect.Height);
            int bottomTile = (int)Math.Ceiling((float) (Image.SourceRect.Height + Image.Position.Y) / Image.SourceRect.Height);

            for (int y = topTile; y < bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    Tile tile = layer.GetTile(x, y);
                    List<Rectangle> rectCollisions = new List<Rectangle>();
                    //List<Triangle> triCollisions = new List<Triangle>();

                    switch (tile.State)
                    {
                        case TileCollision.Passive:
                            break;
                        case TileCollision.Solid:
                            rectCollisions.Add(new Rectangle((int)tile.Position.X, (int)tile.Position.Y, tile.SourceRect.Width, tile.SourceRect.Height));
                            break;
                        default:
                            throw new NotSupportedException(String.Format("Unsupported tile enum type '{0}' at position {1}, {2},", tile.State, x, y));
                    }

                    foreach (Rectangle r in rectCollisions)
                    {
                        if (boundingCircle.Intersects(r))
                        {
                            Velocity = Vector2.Zero;
                            break;
                        }
                    }
                }
            }
        }

    }
}
