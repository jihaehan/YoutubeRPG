﻿using System;
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

        public Image Water;
        public Image Image;
        public string OverlayTiles, PortalTiles, WaterTiles, NpcTiles, BattleTiles;
        public string SolidTiles, LeftEdge, RightEdge, TopEdge, LeftCorner, RightCorner, NWCorner, NECorner, SWCorner, SECorner, RightWall, LeftWall, TopWall, BottomWall, BottomDoor, SEWallCorner, SWWallCorner, NEWallCorner, NWWallCorner, LeftHalf, RightHalf; 
        
        List<Tile> underlayTiles;
        List<Tile> overlayTiles;
        Dictionary<string, Vector2> npcName;
        Dictionary<Vector2, string> portalTiles;
        Dictionary<Vector2, string> npcTiles;
        List<TileCollision> tilesCount;
        
        int rowLength;
        int tileLength;

        #region getter/setter
        public Dictionary<string, Vector2> NpcName()
        {
            return npcName;
        }
        #endregion

        public Layer()
        {
            Image = new Image();
            Water = new Image();
            underlayTiles = new List<Tile>();
            overlayTiles = new List<Tile>();
            portalTiles = new Dictionary<Vector2, string>();
            npcTiles = new Dictionary<Vector2, string>();
            npcName = new Dictionary<string, Vector2>();

            tilesCount = new List<TileCollision>();
            OverlayTiles = PortalTiles = String.Empty;
            
            SolidTiles = LeftEdge = RightEdge = TopEdge = LeftCorner = RightCorner = NWCorner = NECorner = SWCorner = SECorner = RightWall = LeftWall = TopWall = BottomWall = BottomDoor = SEWallCorner = SWWallCorner = NEWallCorner = NWWallCorner = LeftHalf = RightHalf = BattleTiles = WaterTiles = NpcTiles = String.Empty;
        }

        public void LoadContent(Vector2 tileDimensions)
        {

            Image.LoadContent();
            if (Water.Path != String.Empty)
                Water.LoadContent();
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
                            else if (SECorner.Contains("[" + value1.ToString() + ":" + value2.ToString() + "]"))
                                tilesCount[tilesCount.Count() - 1] = TileCollision.SECorner;
                            else if (SWCorner.Contains("[" + value1.ToString() + ":" + value2.ToString() + "]"))
                                tilesCount[tilesCount.Count() - 1] = TileCollision.SWCorner;
                            else if (NECorner.Contains("[" + value1.ToString() + ":" + value2.ToString() + "]"))
                                tilesCount[tilesCount.Count() - 1] = TileCollision.NECorner;
                            else if (NWCorner.Contains("[" + value1.ToString() + ":" + value2.ToString() + "]"))
                                tilesCount[tilesCount.Count() - 1] = TileCollision.NWCorner;
                            else if (BattleTiles.Contains("[" + value1.ToString() + ":" + value2.ToString() + "]"))
                                tilesCount[tilesCount.Count() - 1] = TileCollision.Battle;
                            else if (WaterTiles.Contains("[" + value1.ToString() + ":" + value2.ToString() + "]"))
                            {
                                tilesCount[tilesCount.Count() - 1] = TileCollision.Water;
                                Water.IsActive = true;
                            }
                            else if (NpcTiles.Contains("[" + value1.ToString() + ":" + value2.ToString() + "-"))
                            {
                                string[] p = NpcTiles.Split(']');
                                string name = String.Empty;
                                foreach (string pstr in p)
                                {
                                    if (pstr.Contains("[" + value1.ToString() + ":" + value2.ToString() + "-"))
                                    {
                                        name = pstr.Substring(pstr.IndexOf('-') + 1);
                                        tilesCount[tilesCount.Count() - 1] = TileCollision.NPC;
                                        Vector2 npcLocation = new Vector2((int)Math.Floor(position.X / tileDimensions.X),
                                            (int)Math.Floor(position.Y / tileDimensions.Y));
                                        npcTiles.Add(npcLocation, name);
                                        npcName.Add(name, npcLocation);
                                    }
                                }
                            }
                            else if (PortalTiles.Contains("[" + value1.ToString() + ":" + value2.ToString() + "-"))
                            {
                                string[] p = PortalTiles.Split(']');
                                string value3 = String.Empty;
                                foreach (string pstr in p)
                                {
                                    if (pstr.Contains("[" + value1.ToString() + ":" + value2.ToString() + "-"))
                                    {
                                        value3 = pstr.Substring(pstr.IndexOf('-') + 1);
                                        tilesCount[tilesCount.Count() - 1] = TileCollision.Portal;
                                        portalTiles.Add(new Vector2((int)Math.Floor(position.X/tileDimensions.X), 
                                            (int)Math.Floor(position.Y/tileDimensions.Y)), value3);
                                    }
                                }                                
                            }

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
            if (Water.Path != String.Empty)
                Water.UnloadContent();
        }
        public void Update(GameTime gameTime)
        {
            foreach (Tile tile in underlayTiles)
                tile.Update(gameTime);
            foreach (Tile tile in overlayTiles)
                tile.Update(gameTime);
            Image.Update(gameTime);
            Water.Update(gameTime);
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
                if (tile.State == TileCollision.Water)
                {
                    Water.Position = tile.Position;
                    Water.Draw(spriteBatch);
                }
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
        public Dictionary<Vector2, string> Portals()
        {
            return portalTiles;
        }
        public Dictionary<Vector2, string> NPCs()
        {
            return npcTiles;
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
