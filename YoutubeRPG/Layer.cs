using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace YoutubeRPG
{
    public class Layer
    {
        public class TileMap
        {
            [XmlElement("Row")]
            public List<string> Row;
            public TileMap()
            {
                Row = new List<string>();
            }
        }

        [XmlElement("TileMap")]
        public TileMap Tile;
        public Image Image;
        public string SolidTiles;
        List<Tile> tiles;
        List<int> tilesCount;
        TileCollision state;

        public Layer()
        {
            Image = new Image();
            tiles = new List<Tile>();
            tilesCount = new List<int>();
            SolidTiles = String.Empty;
        }

        public void LoadContent(Vector2 tileDimensions)
        {
            Image.LoadContent();
            Vector2 position = -tileDimensions; //tileDimensions - new Vector2(128,128)

            foreach (string row in Tile.Row)
            {
                string[] split = row.Split(']');
                position.X = -tileDimensions.X;
                position.Y += tileDimensions.Y;
                tilesCount.Add(split.Length-1);
                foreach(string s in split)
                {
                    if (s!= String.Empty)
                    {
                        position.X += tileDimensions.X;
                        state = TileCollision.Passive;
                        tiles.Add(new Tile());

                        if (!s.Contains("x"))
                        {
                            string str = s.Replace("[", String.Empty);
                            int value1 = int.Parse(str.Substring(0, str.IndexOf(':')));
                            int value2 = int.Parse(str.Substring(str.IndexOf(':') + 1));

                            //Set TileTypes
                            if (SolidTiles.Contains("[" + value1.ToString() + ":" + value2.ToString() + "]"))
                                state = TileCollision.Solid;

                            tiles[tiles.Count - 1].LoadContent(position, new Rectangle(
                                (int)(value1 * tileDimensions.X), (int)(value2 * tileDimensions.Y),
                                (int)tileDimensions.X, (int)tileDimensions.Y), state); 
                        }
                    }
                }
            }
        }
        public void UnloadContent()
        {
            Image.UnloadContent();
        }
        public void Update(GameTime gameTime)
        {
            foreach (Tile tile in tiles)
                tile.Update(gameTime);
            Image.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Tile tile in tiles)
            {
                Image.Position = tile.Position;
                Image.SourceRect = tile.SourceRect;
                Image.Draw(spriteBatch);
            }
        }
        public Tile GetTile(int x, int y)
        {
            int count = y * tilesCount[0] + x;
            if (count < 0)
                count = 0;
            if (count > tiles.Count() -1)
                count = tiles.Count() - 1;
            return tiles[count];
        }

    }
}
