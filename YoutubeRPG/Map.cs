using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YoutubeRPG
{
    public class Map
    {
        [XmlElement("Layer")]
        public List<Layer> Layer;
        //[XmlElement("NPC")]
        //public List<NPC> NpcLayer;
        public Vector2 TileDimensions;
        public Vector2 StartingPoint;
        public Image Image;


        public Map()
        {
            Layer = new List<Layer>();
            TileDimensions = new Vector2(128,128);
            StartingPoint = new Vector2(-1,-1);
        }

        public void LoadContent()
        {
            Image.LoadContent();
            StartingPoint = new Vector2(StartingPoint.X * TileDimensions.X, StartingPoint.Y * TileDimensions.Y);
            foreach (Layer l in Layer)
                l.LoadContent(TileDimensions);
        }
        public void UnloadContent()
        {
            Image.UnloadContent();
            foreach (Layer l in Layer)
                l.UnloadContent();
        }
        public void Update(GameTime gameTime)
        {
            Image.Update(gameTime);
            foreach (Layer l in Layer)
                l.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch, string drawType)
        {
            foreach (Layer l in Layer)
                l.Draw(spriteBatch, drawType);
        }
        public void Background(SpriteBatch spriteBatch)
        {
            Image.Draw(spriteBatch);
        }

    }
}
