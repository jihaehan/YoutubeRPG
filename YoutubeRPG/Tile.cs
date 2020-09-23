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
        Passive,
        Solid,
        Portal,
        Random, 
        Water,
        UV, 
        Heat, 
        LeftEdge,
        RightEdge,
        TopEdge,
        LeftCorner,
        RightCorner,
        NWCorner,
        NECorner,
        SWCorner,
        SECorner,
        RightWall,
        LeftWall,
        TopWall,
        BottomWall,
        BottomDoor,
        SWWallCorner,
        SEWallCorner,
        NEWallCorner,
        NWWallCorner,
        LeftHalf,
        RightHalf,
        Chemical,
        NPC,
        Battle,
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
