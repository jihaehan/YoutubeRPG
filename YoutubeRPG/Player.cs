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
        public int TileLength; 
        
        public Player()
        {
            Velocity = Vector2.Zero;
            TileLength = 128; 
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
                    TileCollision tileCollision = layer.GetTile(x, y);
                    List<Rectangle> rectCollisions = new List<Rectangle>();
                    List<Triangle> triCollisions = new List<Triangle>();

                    switch (tileCollision)
                    {
                        case TileCollision.Solid:
                            rectCollisions.Add(new Rectangle(x * TileLength, y * TileLength, TileLength, TileLength));
                            break;
                        case TileCollision.LeftEdge:
                            rectCollisions.Add(new Rectangle(x * TileLength + TileLength / 3, y * TileLength, TileLength * 2 / 3, TileLength));
                            break;
                        case TileCollision.RightEdge:
                            rectCollisions.Add(new Rectangle(x * TileLength, y * TileLength, TileLength * 2 / 3, TileLength));
                            break;
                        case TileCollision.TopEdge:
                            rectCollisions.Add(new Rectangle(x * TileLength, y * TileLength, TileLength, TileLength / 2));
                            break;
                        case TileCollision.LeftCorner:
                            rectCollisions.Add(new Rectangle(x * TileLength + TileLength / 3, y * TileLength, TileLength * 2 / 3, TileLength / 2));
                            break;
                        case TileCollision.RightCorner:
                            rectCollisions.Add(new Rectangle(x * TileLength, y * TileLength, TileLength * 2 / 3, TileLength / 2));
                            break;
                        case TileCollision.NWCorner:
                            triCollisions.Add(new Triangle(x * TileLength, y * TileLength, TileLength / 4, TileLength / 4, -1));
                            break;
                        case TileCollision.NECorner:
                            triCollisions.Add(new Triangle((x + 1) * TileLength, y * TileLength, -TileLength * 3 / 4, TileLength / 4, 1));
                            break;
                        case TileCollision.SECorner:
                            triCollisions.Add(new Triangle((x + 1) * TileLength, (y + 1) * TileLength, -TileLength, TileLength, -1));
                            break;
                        case TileCollision.SWCorner:
                            triCollisions.Add(new Triangle(x * TileLength, (y + 1) * TileLength, -TileLength, TileLength, 1));
                            break;
                        default:
                            break;
                    }

                    foreach (Rectangle r in rectCollisions)
                    {
                        if (boundingCircle.Intersects(r))
                        {
                            //Consider removing "sticky" walls
                            Velocity = Vector2.Zero;
                            break;
                        }
                    }
                }
            }
        }

    }
}
