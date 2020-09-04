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
    public class ChemicalManager
    {
        [XmlElement("ChemicalSource")]
        public List<string> ChemicalSource;
        public string CurrentChemicalName;

        Dictionary<string, Chemical> chemicals;
        public List<string> chemicalName;

        int maxVisibleChemicals; 

        public ChemicalManager()
        {
            ChemicalSource = new List<string>();
            CurrentChemicalName = String.Empty;
            chemicals = new Dictionary<string, Chemical>();
            chemicalName = new List<string>();
            maxVisibleChemicals = 3;
        }

        public Chemical CurrentChemical
        {
            get { return chemicals[CurrentChemicalName]; }
        }
        public void LoadContent()
        {
            XmlManager<Chemical> chemicalLoader = new XmlManager<Chemical>();
            foreach (string chemicalSource in ChemicalSource)
            {
                string[] split = chemicalSource.Split('/');
                string s = (split[split.Length - 1]).Replace(".xml", String.Empty);
                Chemical chemical = chemicalLoader.Load(chemicalSource);
                if (chemical.NickName != String.Empty)
                    s = chemical.NickName;

                while (chemicals.ContainsKey(s))
                    s += "*";
                chemicalName.Add(s);
                chemical.LoadContent();
                chemicals.Add(s, chemical);
            }
            if (chemicalName.Count() > 0)
                CurrentChemicalName = chemicalName[0];
        }
        public void UnloadContent()
        {
            foreach (string name in chemicalName)
            {
                chemicals[name].UnloadContent();
            }
        }
        public void Update(GameTime gameTime, Player player)
        {
            Chemical chemical = new Chemical();
            for (int count = 0; count < maxVisibleChemicals/*chemicalName.Count*/; count++)
            {
                if (count > 0)
                {
                    chemical = chemicals[chemicalName[count - 1]];
                    if (Vector2.Distance(chemical.Image.Position, chemicals[chemicalName[count]].Image.Position) > chemicals[chemicalName[count]].Dimensions.X * 3)
                        chemicals[chemicalName[count]].Image.Position = player.Image.Position;
                }
                else if (Vector2.Distance(player.Image.Position, chemicals[chemicalName[count]].Image.Position) > chemicals[chemicalName[count]].Dimensions.X * 3)
                    chemicals[chemicalName[count]].Image.Position = player.Image.Position;

                chemicals[chemicalName[count]].Update(gameTime, ref player, chemical, count);
            } 
        }
        public void Update(GameTime gameTime)
        {
            foreach (string name in chemicalName)
            {
                chemicals[name].Update(gameTime);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int count = 0; count < maxVisibleChemicals; count++)
            {
                chemicals[chemicalName[count]].Draw(spriteBatch);
            }
        }
        public Chemical GetChemical(string chemicalName)
        {
            if (chemicals.ContainsKey(chemicalName))
                return chemicals[chemicalName];
            return null;
        }
        public void ChangeChemical(string chemicalName)
        {
            if (chemicals.ContainsKey(chemicalName))
            {
                CurrentChemicalName = chemicalName;
                return;
            }
            throw new Exception("Chemical not found.");
        }
    }
}
