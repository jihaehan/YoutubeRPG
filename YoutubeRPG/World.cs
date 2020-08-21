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
    public class World
    {
        [XmlElement("MapSource")]
        public List<string> MapSource;
        public string CurrentMapName;

        Dictionary<string, Map> maps;
        List<string> mapName;

        public World()
        {
            MapSource = new List<string>();
            mapName = new List<string>();
            maps = new Dictionary<string, Map>();
            CurrentMapName = String.Empty;
        }
        public Map CurrentMap
        {
            get { return maps[CurrentMapName]; }
        }
        public void LoadContent()
        {
            XmlManager<Map> mapLoader = new XmlManager<Map>();
            foreach (string mapSource in MapSource)
            {
                string[] split = mapSource.Split('/');
                string s = (split[split.Length - 1]).Replace(".xml", String.Empty);
                Map map = mapLoader.Load(mapSource);
                mapName.Add(s);
                maps.Add(s, map);
                map.LoadContent();
            }
            if (mapName.Count() > 0)
                CurrentMapName = mapName[0];
        }
        public void UnloadContent()
        {
            foreach (string name in mapName)
            {
                maps[name].UnloadContent();
            }
        }
        public void Update(GameTime gameTime)
        {
            foreach (string name in mapName)
            {
                maps[name].Update(gameTime);
            }
        }
        public void Draw(SpriteBatch spriteBatch, string drawType)
        {
            maps[CurrentMapName].Draw(spriteBatch, drawType);
        }
        public Map GetMap(string mapName)   
        {
            if (maps.ContainsKey(mapName))
                return maps[mapName];
            return null;
        }
        public void ChangeMap(string mapName)
        {
            if (maps.ContainsKey(mapName))
            {
                CurrentMapName = mapName;
                return;
            }
            throw new Exception("Map name or portal name not found.");
        }
    }
}
