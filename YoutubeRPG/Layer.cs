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
        public string OverlayTiles;
        public string SolidTiles, LeftEdge, RightEdge, TopEdge, LeftCorner, RightCorner, NWCorner, NECorner, SWCorner, SECorner, RightWall, LeftWall, TopWall, BottomWall, BottomDoor, SEWallCorner, SWWallCorner, NEWallCorner, NWWallCorner, LeftHalf, RightHalf; 
        
        List<Tile> underlayTiles;
        List<Tile> overlayTiles; 
        List<TileCollision> tilesCount;
        int rowLength;
        int tileLength;

        public Layer()
        {
            Image = new Image();
            underlayTiles = new List<Tile>();
            overlayTiles = new List<Tile>();

            tilesCount = new List<TileCollision>();
            OverlayTiles = String.Empty;
            
            SolidTiles = LeftEdge = RightEdge = TopEdge = LeftCorner = RightCorner = NWCorner = NECorner = SWCorner = SECorner = RightWall = LeftWall = TopWall = BottomWall = BottomDoor = SEWallCorner = SWWallCorner = NEWallCorner = NWWallCorner = LeftHalf = RightHalf = String.Empty;
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
                rowLength = split.Length-1;
                foreach(string s in split)
                {
                    if (s!= String.Empty)
                    {
                        position.X += tileDimensions.X;
                        tilesCount.Add(TileCollision.Passive);

                        if (!s.Contains("x"))
                        {
                            Tile tile = new Tile();
                            string str = s.Replace("[", String.Empty);
                            int value1 = int.Parse(str.Substring(0, str.IndexOf(':')));
                            int value2 = int.Parse(str.Substring(str.IndexOf(':') + 1));

                            //Set TileTypes
                            if (SolidTiles.Contains("[" + value1.ToString() + ":" + value2.ToString() + "]"))
                                tilesCount[tilesCount.Count() - 1] = TileCollision.Solid;
                            else if (LeftWall.Contains("[" + value1.ToString() + ":" + value2.ToString() + "]"))
                                tilesCount[tilesCount.Count() - 1] = TileCollision.LeftWall;
                            else if (RightWall.Contains("[" + value1.ToString() + ":" + value2.ToString() + "]"))
                                tilesCount[tilesCount.Count() - 1] = TileCollision.RightWall;
                            else if (TopWall.Contains("[" + value1.ToString() + ":" + value2.ToString() + "]"))
                                tilesCount[tilesCount.Count() - 1] = TileCollision.TopWall;
                            else if (BottomWall.Contains("[" + value1.ToString() + ":" + value2.ToString() + "]"))
                                tilesCount[tilesCount.Count() - 1] = TileCollision.BottomWall;
                            else if (BottomDoor.Contains("[" + value1.ToString() + ":" + value2.ToString() + "]"))
                                tilesCount[tilesCount.Count() - 1] = TileCollision.BottomDoor;
                            else if (SEWallCorner.Contains("[" + value1.ToString() + ":" + value2.ToString() + "]"))
                                tilesCount[tilesCount.Count() - 1] = TileCollision.SEWallCorner;
                            else if (SWWallCorner.Contains("[" + value1.ToString() + ":" + value2.ToString() + "]"))
                                tilesCount[tilesCount.Count() - 1] = TileCollision.SWWallCorner;
                            else if (NEWallCorner.Contains("[" + value1.ToString() + ":" + value2.ToString() + "]"))
                                tilesCount[tilesCount.Count() - 1] = TileCollision.NEWallCorner;
                            else if (NWWallCorner.Contains("[" + value1.ToString() + ":" + value2.ToString() + "]"))
                                tilesCount[tilesCount.Count() - 1] = TileCollision.NWWallCorner;
                            else if (RightHalf.Contains("[" + value1.ToString() + ":" + value2.ToString() + "]"))
                                tilesCount[tilesCount.Count() - 1] = TileCollision.RightHalf;
                            else if (LeftHalf.Contains("[" + value1.ToString() + ":" + value2.ToString() + "]"))
                                tilesCount[tilesCount.Count() - 1] = TileCollision.LeftHalf;

                            tile.LoadContent(position, new Rectangle(
                                (int)(value1 * tileDimensions.X), (int)(value2 * tileDimensions.Y),
                                (int)tileDimensions.X, (int)tileDimensions.Y), tilesCount[tilesCount.Count() - 1]);

                            if (OverlayTiles.Contains("[" + value1.ToString() + ":" + value2.ToString() + "]"))
                                overlayTiles.Add(tile);
                            else
                                underlayTiles.Add(tile);

                            tileLength = tile.SourceRect.Width;
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
            foreach (Tile tile in underlayTiles)
                tile.Update(gameTime);
            foreach (Tile tile in overlayTiles)
                tile.Update(gameTime);
            Image.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch, string drawType)
        {
            List<Tile> tiles;
            if (drawType == "Underlay")
                tiles = underlayTiles;
            else
                tiles = overlayTiles;

            foreach (Tile tile in tiles)
            {
                Image.Position = tile.Position;
                Image.SourceRect = tile.SourceRect;
                Image.Draw(spriteBatch);
            }
        }
        public TileCollision GetTile(int x, int y)
        {
            int count = y * rowLength + x;
            if (count < 0)
                count = 0;
            if (count > tilesCount.Count() -1)
                count = tilesCount.Count() - 1;
            return tilesCount[count];
        }
        public int Width()
        {
            return rowLength * tileLength;
        }
        public int Height()
        {
            return Tile.Row.Count() * tileLength;
        }
    }
}
