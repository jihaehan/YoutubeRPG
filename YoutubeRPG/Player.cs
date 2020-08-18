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
            if (Velocity != Vector2.Zero)
            {
                Image.IsActive = true;
                Velocity.Normalize();
                Velocity *= (MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                HandleCollisions(collisionLayer);
                Image.Position += Velocity;
            }
            else
                Image.IsActive = false;

            Image.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Image.Draw(spriteBatch);
        }

        public void MoveDown(eButtonState buttonState)
        {
            if (buttonState == eButtonState.PRESSED)
            {
                Velocity.Y = 1;
                if (Velocity.X == 0)
                    Image.SpriteSheetEffect.CurrentFrame.Y = 1;
                else if (Velocity.X > 0)
                    Image.SpriteSheetEffect.CurrentFrame.Y = 5;
                else if (Velocity.X < 0)
                    Image.SpriteSheetEffect.CurrentFrame.Y = 4;
            }
        }
        public void MoveUp(eButtonState buttonState)
        {
            if (buttonState == eButtonState.PRESSED)
            {
                Velocity.Y = -1;
                if (Velocity.X == 0)
                    Image.SpriteSheetEffect.CurrentFrame.Y = 0;
                else if (Velocity.X > 0)
                    Image.SpriteSheetEffect.CurrentFrame.Y = 7;
                else if (Velocity.X < 0)
                    Image.SpriteSheetEffect.CurrentFrame.Y = 6;
            }
        }

        public void MoveLeft(eButtonState buttonState)
        {
            if (buttonState == eButtonState.PRESSED)
            {
                Velocity.X = -1;
                if (Velocity.Y == 0)
                    Image.SpriteSheetEffect.CurrentFrame.Y = 3;
                else if (Velocity.Y > 0)
                    Image.SpriteSheetEffect.CurrentFrame.Y = 4;
                else if (Velocity.Y < 0)
                    Image.SpriteSheetEffect.CurrentFrame.Y = 6;
            }
        }
        public void MoveRight(eButtonState buttonState)
        {
            if (buttonState == eButtonState.PRESSED)
            {
                Velocity.X = 1;
                if (Velocity.Y == 0)
                    Image.SpriteSheetEffect.CurrentFrame.Y = 2;
                else if (Velocity.Y > 0)
                    Image.SpriteSheetEffect.CurrentFrame.Y = 5;
                else if (Velocity.Y < 0)
                    Image.SpriteSheetEffect.CurrentFrame.Y = 7;
            }
        }

        void HandleCollisions(Layer layer)
        {
            Circle boundingCircle = new Circle(new Vector2(
                (int)Image.Position.X + Image.SourceRect.Width / 2 + Velocity.X,
                (int)Image.Position.Y + Image.SourceRect.Height / 2 + Velocity.Y),
                (int)Image.SourceRect.Width/2 + (Velocity.X + Velocity.Y)/2);
            Rectangle boundingBox = new Rectangle(
                (int)Math.Floor(Image.Position.X + Image.SourceRect.Width/4 + Velocity.X), 
                (int)Math.Floor(Image.Position.Y + Image.SourceRect.Height/2 + Velocity.Y),
                Image.SourceRect.Width/2, Image.SourceRect.Height/2);

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
                        if (boundingBox.Intersects(r))
                        {
                            if ((r.Center.X > boundingBox.Center.X && Velocity.X > 0)
                                ||(r.Center.X < boundingBox.Center.X && Velocity.X < 0))
                                Velocity.X = 0;
                            
                            if ((r.Center.Y > boundingBox.Center.Y && Velocity.Y > 0)
                                || (r.Center.Y < boundingBox.Center.Y && Velocity.Y < 0))
                                Velocity.Y = 0;
                           
                        }
                    }
                    
                    foreach (Triangle t in triCollisions)
                    {
                        if (RectangleIntersectTriangle(boundingBox, t))
                            Velocity = Vector2.Zero;
                    }
                    
                }
            }
        }
        private bool RectangleIntersectTriangle(Rectangle rectangle, Triangle triangle)
        {
            Vector2 rCenter = new Vector2(rectangle.Center.X, rectangle.Center.Y);
            Vector2 v = rCenter - triangle.Corner;
            Vector2 vi = v - (v / v.Length() * rectangle.Width);
            Vector2 vii = new Vector2(vi.X, triangle.Offset + (triangle.Slope) * vi.X);

            float distanceSquared = vi.LengthSquared();
            float internalSquared = vii.LengthSquared();

            return (distanceSquared < triangle.Length * triangle.Length && distanceSquared <= internalSquared);
        }
    }
}
