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
        public Vector2 TileDimensions;
        public Vector2 StartingPoint;
        public Vector2 PortalLeft, PortalRight, PortalTop, PortalBottom, PortalMid;
        public Image Image;

        [XmlElement("NPC")]
        public List<string> NPCSource;
        [XmlIgnore]
        public Dictionary<string, NPC> Npcs;

        public Map()
        {
            NPCSource = new List<string>();
            Npcs = new Dictionary<string, NPC>();
            Layer = new List<Layer>();
            TileDimensions = new Vector2(128,128);
            StartingPoint = new Vector2(-1,-1);
            PortalLeft = PortalRight = PortalTop = PortalBottom = PortalMid = Vector2.Zero;
        }

        public void LoadContent()
        {
            XmlManager<NPC> npcLoader = new XmlManager<NPC>();
            Image.LoadContent();
            StartingPoint = new Vector2(StartingPoint.X * TileDimensions.X, StartingPoint.Y * TileDimensions.Y);
            foreach (Layer l in Layer)
            {
                l.LoadContent(TileDimensions);
                foreach (string n in l.NpcName().Keys)
                {
                    NPC npc = npcLoader.Load("Content/Load/Gameplay/Npc/" + n + ".xml");
                    npc.LoadContent();
                    npc.GetCharacter().Image.Position = l.NpcName()[n]*128;
                    Npcs.Add(n, npc);
                }
            }
        }
        public void UnloadContent()
        {
            Image.UnloadContent();
            foreach (Layer l in Layer)
                l.UnloadContent();
            foreach (NPC npc in Npcs.Values)
                npc.UnloadContent();
        }
        public void Update(GameTime gameTime)
        {
            Image.Update(gameTime);
            foreach (Layer l in Layer)
                l.Update(gameTime);
            foreach (NPC npc in Npcs.Values)
                npc.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch, string drawType)
        {
            foreach (Layer l in Layer)
                l.Draw(spriteBatch, drawType);
            if (drawType.Contains("Under"))
                DrawNpcs(spriteBatch);
        }
        public void DrawNpcs(SpriteBatch spriteBatch)
        {
            foreach (NPC npc in Npcs.Values)
                npc.Draw(spriteBatch);
        }
        public void Background(SpriteBatch spriteBatch)
        {
            Image.Draw(spriteBatch);
        }

    }
}
