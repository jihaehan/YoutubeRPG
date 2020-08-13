using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YoutubeRPG
{
    public enum TileCollision
    {
        Passive = 0,
        Solid = 1,
        Portal = 2,
        Random = 3, 
        Water = 4,
        UV = 5, 
        Heat = 6,
        LeftEdge = 7,
        RightEdge = 8,
        TopEdge = 9,
        LeftCorner = 10,
        RightCorner = 11,
        NWCorner = 12,
        NECorner = 13,
        SWCorner = 14,
        SECorner = 15,

    }
    public class Tile
    {
        Vector2 position;
        Rectangle sourceRect;
        TileCollision state; 

        public Rectangle SourceRect
        {
            get { return sourceRect; }
        }
        public Vector2 Position
        {
            get { return position; }
        }
        public TileCollision State
        {
            get { return state; }
        }
        public void LoadContent(Vector2 position, Rectangle sourceRect, TileCollision state)
        {
            this.position = position;
            this.sourceRect = sourceRect;
            this.state = state; 
        }
        public void UnloadContent()
        {

        }
        public void Update(GameTime gameTime)
        {            
        }
        public void Draw(SpriteBatch spriteBatch)
        {
        }
    }
}
