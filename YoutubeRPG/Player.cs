﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace YoutubeRPG
{
    public class Player
    {
        public Image Image;
        public Vector2 Velocity;
        public bool IsDialogue;
        public bool IsNPC;
     
        public float MoveSpeed;
        public int TileLength;
        public string DialoguePath;
        
        string portalDestination; 
        Vector2 portalArrival;
        bool isPortal;
        bool isDownBug;

        public ChemicalManager ChemicalManager;
        public ItemManager ItemManager;
        public CharacterManager CharacterManager;
        public List<string> ReactionHistory;
        public DialogueManager Dialogue;

        public List<string> Keys
        {
            get { return keys; }
            set { keys = value; }
        } List<string> keys;

        public Player()
        {
            DialoguePath = String.Empty;
            IsDialogue = false;
            Dialogue = new DialogueManager();
            ReactionHistory = new List<string>();
            Velocity = Vector2.Zero;
            TileLength = 128;
            isPortal = false;
            IsNPC = false;
            portalDestination = String.Empty;
            portalArrival = Vector2.Zero;
            keys = new List<string>();
            isDownBug = false;
        }
        #region Main Methods
        public void LoadContent()
        {
            Image.LoadContent();
            XmlManager<ChemicalManager> chemicalManagerLoader = new XmlManager<ChemicalManager>();
            XmlManager<ItemManager> itemManagerLoader = new XmlManager<ItemManager>();
            XmlManager<CharacterManager> characterManagerLoader = new XmlManager<CharacterManager>();
            //if Party/Backpack saves exist, load Save files here
            ChemicalManager = chemicalManagerLoader.Load("Content/Load/Gameplay/Party.xml");
            ItemManager = itemManagerLoader.Load("Content/Load/Gameplay/Backpack.xml");
            CharacterManager = characterManagerLoader.Load("Content/Load/Gameplay/Character/IntroWorld.xml");
            ChemicalManager.LoadContent();
            ItemManager.LoadContent();
            CharacterManager.LoadContent();
        }
        public void UnloadContent()
        {
            Image.UnloadContent();
            //if (ChemicalManager != null)
                //ChemicalManager.UnloadContent();
            if (ItemManager != null)
                ItemManager.UnloadContent();
            if (CharacterManager != null)
                CharacterManager.UnloadContent();
        }
        public void Update(GameTime gameTime)
        {
            Image.Update(gameTime);
            ChemicalManager.Update(gameTime);
        }
        public void Update(GameTime gameTime, World world)
        {
            if (Velocity != Vector2.Zero)
            {
                Image.IsActive = true;
                Velocity.Normalize();
                Velocity *= (MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                for(int i = 0; i < world.CurrentMap.Layer.Count(); ++i)
                    HandleCollisions(world.CurrentMap);
                Image.Position += Velocity;
            }
            else
                Image.IsActive = false;

            Image.Update(gameTime);
            ChemicalManager.Update(gameTime, this);
            
            portalTransition(gameTime, world);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            ChemicalManager.Draw(spriteBatch);
            Image.Draw(spriteBatch);
        }
        #endregion

        #region Battle Methods
        public void InitializeBattle() //initialize first 3 chemicals as battle chemicals
        {
            ChemicalManager.InitializeBattle(Image.Position);
        }
        public void BattleUpdate(GameTime gameTime)
        {
            ChemicalManager.BattleUpdate(gameTime, Image.Position, true);
            Image.Update(gameTime);
        }
        public void BattleDraw(SpriteBatch spriteBatch)
        {
            ChemicalManager.BattleDraw(spriteBatch);
            ChemicalManager.DrawHorizontalTag(spriteBatch);
            Image.Draw(spriteBatch);
        }
        #endregion

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
        public string DialoguePathName()
        {
            if (IsNPC)
                return DialoguePath;
            else
                return String.Empty;
        }
        void HandleCollisions(Map map)
        {
            for (int i = 0; i < map.Layer.Count(); ++i)
            {
                Layer layer = map.Layer[i];

                Rectangle boundingBox = new Rectangle(
                    (int)Math.Floor(Image.Position.X + Image.SourceRect.Width / 4 + Velocity.X),
                    (int)Math.Floor(Image.Position.Y + Image.SourceRect.Height / 3 + Velocity.Y * 1.1f),
                    Image.SourceRect.Width / 2, (int)Image.SourceRect.Height * 2 / 3);

                int leftTile = (int)Math.Floor((float)(Image.Position.X) / Image.SourceRect.Width);
                int rightTile = (int)Math.Ceiling((float)(Image.SourceRect.Width + Image.Position.X) / Image.SourceRect.Width);
                int topTile = (int)Math.Floor((float)(Image.Position.Y) / Image.SourceRect.Height);
                int bottomTile = (int)Math.Ceiling((float)(Image.SourceRect.Height + Image.Position.Y) / Image.SourceRect.Height);

                for (int y = topTile; y < bottomTile; ++y)
                {
                    for (int x = leftTile; x <= rightTile; ++x)
                    {
                        TileCollision tileCollision = layer.GetTile(x, y);
                        List<Rectangle> rectCollisions = new List<Rectangle>();
                        List<Rectangle> portalCollisions = new List<Rectangle>();
                        List<Rectangle> npcCollisions = new List<Rectangle>();
                        List<Rectangle> randomCollisions = new List<Rectangle>();
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
                                triCollisions.Add(new Triangle((x + 1) * TileLength, (y + 1) * TileLength + 25, -TileLength, TileLength, -1));
                                break;
                            case TileCollision.SWCorner:
                                triCollisions.Add(new Triangle(x * TileLength, (y + 1) * TileLength + 25, -TileLength, TileLength, 1));
                                break;
                            case TileCollision.RightWall:
                                rectCollisions.Add(new Rectangle(x * TileLength - TileLength / 5, y * TileLength, (int)(TileLength * 1.2), TileLength));
                                break;
                            case TileCollision.LeftWall:
                                rectCollisions.Add(new Rectangle(x * TileLength, y * TileLength, (int)(TileLength * 1.2), TileLength));
                                break;
                            case TileCollision.TopWall:
                                rectCollisions.Add(new Rectangle(x * TileLength, y * TileLength, TileLength, TileLength / 2));
                                break;
                            case TileCollision.BottomWall:
                                rectCollisions.Add(new Rectangle((int)(x * TileLength - TileLength * .1), y * TileLength + TileLength / 2, (int)(TileLength * 1.2), TileLength / 2));
                                break;
                            case TileCollision.BottomDoor:
                                rectCollisions.Add(new Rectangle(x * TileLength - TileLength / 2, (int)(y * TileLength + TileLength * 0.89), TileLength * 2, (int)(TileLength * 0.11)));
                                break;
                            case TileCollision.SEWallCorner:
                                rectCollisions.Add(new Rectangle(x * TileLength + TileLength / 2, y * TileLength + TileLength / 2, TileLength / 2, TileLength / 2));
                                break;
                            case TileCollision.SWWallCorner:
                                rectCollisions.Add(new Rectangle(x * TileLength, y * TileLength + TileLength / 2, TileLength / 2, TileLength / 2));
                                break;
                            case TileCollision.NWWallCorner:
                                rectCollisions.Add(new Rectangle(x * TileLength, y * TileLength, TileLength / 2, TileLength / 2));
                                break;
                            case TileCollision.NEWallCorner:
                                rectCollisions.Add(new Rectangle(x * TileLength + TileLength / 2, y * TileLength, TileLength / 2, TileLength / 2));
                                break;
                            case TileCollision.LeftHalf:
                                rectCollisions.Add(new Rectangle(x * TileLength, y * TileLength, TileLength / 2, TileLength));
                                break;
                            case TileCollision.RightHalf:
                                rectCollisions.Add(new Rectangle(x * TileLength + TileLength / 2, y * TileLength, TileLength / 2, TileLength));
                                break;
                            case TileCollision.Battle:
                                randomCollisions.Add(new Rectangle(x * TileLength + TileLength / 2, y * TileLength, TileLength / 2, TileLength / 2));
                                break;
                            case TileCollision.NPC:
                                npcCollisions.Add(new Rectangle(x * TileLength + TileLength / 10, y * TileLength + (int)(TileLength/6), (int) TileLength * 4 / 5, (int)TileLength * 4/5));
                                break;
                            case TileCollision.Portal:
                                if ((x + 2) > layer.Width() / TileLength)
                                {
                                    portalArrival = map.PortalRight;
                                    portalCollisions.Add(new Rectangle(x * TileLength + TileLength, y * TileLength, TileLength, TileLength));
                                }
                                else if (x - 1 < 0)
                                {
                                    portalArrival = map.PortalLeft;
                                    portalCollisions.Add(new Rectangle(x * TileLength - TileLength, y * TileLength, TileLength, TileLength));
                                }
                                else if (y + 2 > layer.Height() / TileLength)
                                {
                                    portalArrival = map.PortalBottom;
                                    portalCollisions.Add(new Rectangle(x * TileLength, y * TileLength + TileLength, TileLength, TileLength));
                                }
                                else if (y - 1 < 0)
                                {
                                    portalArrival = map.PortalTop;
                                    portalCollisions.Add(new Rectangle(x * TileLength, y * TileLength - TileLength, TileLength, TileLength));
                                }
                                else 
                                {
                                    //portalArrival = Vector2.Zero;
                                    portalArrival = map.PortalMid;
                                    portalCollisions.Add(new Rectangle(x * TileLength, y * TileLength, TileLength, TileLength));
                                }
                                portalArrival *= TileLength;
                                break;
                            default:
                                break;
                        }

                        foreach (Rectangle r in rectCollisions)
                        {
                            if (boundingBox.Intersects(r) && Velocity != Vector2.Zero)
                            {
                                if ((r.Center.X > boundingBox.Center.X && Velocity.X > 0)
                                    || (r.Center.X < boundingBox.Center.X && Velocity.X < 0))
                                    Velocity.X = 0;

                                if ((r.Center.Y > boundingBox.Center.Y && Velocity.Y > 0) || (r.Center.Y < boundingBox.Center.Y && Velocity.Y < 0))
                                    Velocity.Y = 0;                                
                            }
                        }

                        foreach (Triangle t in triCollisions)
                        {
                            if (rectangleIntersectTriangle(boundingBox, t) && Velocity != Vector2.Zero)
                                Velocity = Vector2.Zero;
                        }
                        foreach (Rectangle r in randomCollisions)
                        {
                            if (boundingBox.Intersects(r))
                            {
                                Random rnd = new Random();
                                if (!ScreenManager.Instance.IsTransitioning)
                                {
                                    if (rnd.Next(100) < 1) //10% chance of triggering battle
                                    {
                                        ScreenManager.Instance.Enemy = "Anonymous";
                                        ScreenManager.Instance.ChangeScreens("BattleScreen");
                                    }
                                }
                            }
                        }
                        foreach (Rectangle r in npcCollisions)
                        {
                            if (boundingBox.Intersects(r) && Velocity != Vector2.Zero)
                            {
                                Vector2 npcLocation = new Vector2(x, y);
                                if ((r.Center.X > boundingBox.Center.X && Velocity.X > 0)
                                    || (r.Center.X < boundingBox.Center.X && Velocity.X < 0))
                                    Velocity.X = 0;

                                if ((r.Center.Y > boundingBox.Center.Y && Velocity.Y > 0)
                                    || (r.Center.Y < boundingBox.Center.Y && Velocity.Y < 0))
                                    Velocity.Y = 0;
                                if (layer.NPCs().ContainsKey(npcLocation))
                                {
                                    string npcName = layer.NPCs()[npcLocation];
                                    Character character = map.Npcs[npcName].GetCharacter();
                                    if (Image.Position.X < character.Image.Position.X)
                                        map.Npcs[npcName].GetCharacter().Image.SpriteSheetEffect.CurrentFrame.Y = 0;
                                    else
                                        map.Npcs[npcName].GetCharacter().Image.SpriteSheetEffect.CurrentFrame.Y = 1;
                                    DialoguePath = layer.NPCs()[npcLocation];
                                    IsNPC = true;
                                }
                            }
                        }
                        foreach (Rectangle p in portalCollisions)
                        {
                            if (boundingBox.Intersects(p) && !isPortal)
                            {
                                Vector2 portalLocation = new Vector2(x, y);
                                if (layer.Portals().ContainsKey(portalLocation))
                                {
                                    portalDestination = layer.Portals()[portalLocation];
                                    if (portalDestination.Contains("Screen"))
                                    {
                                        if (keys.Contains(portalDestination))
                                        {
                                            isPortal = true;
                                            ScreenManager.Instance.ChangeScreens(portalDestination);
                                        }
                                        else
                                            break;
                                    }
                                    else
                                        ScreenManager.Instance.FadeScreen();
                                    
                                }
                            }
                        }
                    }
                }

            }
        }
        private void portalTransition(GameTime gameTime, World world)
        {
            if (ScreenManager.Instance.IsFadeEffect)
            {
                ScreenManager.Instance.Image.Update(gameTime);
                if (ScreenManager.Instance.Image.Alpha == 1.0f)
                {
                    world.ChangeMap(portalDestination);
                    if (portalArrival != Vector2.Zero)
                        Image.Position = portalArrival;
                    isPortal = false;
                }
                else if (ScreenManager.Instance.Image.Alpha == 0.0f)
                {
                    ScreenManager.Instance.Image.IsActive = false;
                    ScreenManager.Instance.IsFadeEffect = false;
                }
            }
        }
        private bool rectangleIntersectTriangle(Rectangle rectangle, Triangle triangle)
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
