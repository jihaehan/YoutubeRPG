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

        Dictionary<string, Chemical> battleChemicals;
        public List<string> battleChemicalName;
        Image tag;
        Image shadow;
        int maxVisibleChemicals;


        public ChemicalManager()
        {
            ChemicalSource = new List<string>();
            CurrentChemicalName = String.Empty;
            chemicals = new Dictionary<string, Chemical>();
            chemicalName = new List<string>();
            maxVisibleChemicals = 3;
            battleChemicals = new Dictionary<string, Chemical>();
            battleChemicalName = new List<string>();
            tag = new Image();
            shadow = new Image();
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
            tag.FontName = "Fonts/OCRAExt";
            tag.Path = "Misc/off_white";
            tag.LoadContent();
            shadow.Path = "Misc/shadow";
            shadow.LoadContent();
        }
        public void UnloadContent()
        {
            foreach (string name in chemicalName)           
                chemicals[name].UnloadContent();
            tag.UnloadContent();
            shadow.UnloadContent();
        }

        /// <summary>
        /// GameplayScreen: Update all chemicals in player party  
        /// </summary>
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
        #region Battle Methods
        /// <summary>
        /// BattleScreen: update/draw all chemicals currently in battle
        /// </summary>
        
        public void BattleReady(string name)
        {
            chemicals[name].InBattle = true;
        }
        public void InitializeBattle(Vector2 startingPosition)
        {
            for (int i = 0; i < chemicalName.Count; i++)
            { 
                if (i == 0)
                    chemicals[chemicalName[i]].Image.Position = startingPosition;
                else 
                    chemicals[chemicalName[i]].Image.Position = chemicals[chemicalName[i - 1]].Image.Position;
                chemicals[chemicalName[i]].Image.IsActive = true;
            }
            for (int i = 0; i < Math.Min(3, chemicalName.Count); i++)
                BattleReady(chemicalName[i]);
        }
        public void BattleUpdate(GameTime gameTime, Vector2 position, bool isPlayer)
        {
            Vector2 targetPosition = position;
            shadow.Position = targetPosition + new Vector2(-11, 100);
            float increment = 95;
            if (isPlayer)
            {
                targetPosition += new Vector2(155, -18);
            }
            else
            {
                targetPosition -= new Vector2(155, -9);
                increment *= -1;
            }
            // add any chemicals tagged Inbattle to battle chemicals
            foreach (string name in chemicalName) 
            {
                if (chemicals[name].InBattle && !battleChemicals.ContainsKey(name))
                {
                    battleChemicals.Add(name, chemicals[name]);
                    battleChemicalName.Add(name);
                    //add initial position before it walks to target position
                    battleChemicals[name].Image.Position = targetPosition + new Vector2(increment * (battleChemicalName.Count - 2), 0);
                    //When adding new battle chemical, also create new battle tag
                    int lastCharacter = MathHelper.Min(name.Length - 1, 7);
                    if (isPlayer) //if Player, add Horizontal Tag
                    {
                        battleChemicals[name].BattleTag = name.Substring(0, lastCharacter).ToUpper();
                    }
                    else //if Enemy, add Vertical Tag
                    {
                        string vName = String.Empty;
                        for (int i = 0; i < lastCharacter; i++)
                            vName += name[i].ToString() + "\n\r";
                        battleChemicals[name].BattleTag = vName.ToUpper();
                    }
                }
            }
            foreach (string name in battleChemicalName)
            {
                //Update battle chemicals to targeted position
                battleChemicals[name].Update(gameTime, targetPosition);
                targetPosition.X += increment;

                Rectangle r = tag.SourceRect;
                r.Width *= (int)tag.Font.MeasureString("T").X;
                r.Height *= (int)(tag.Font.MeasureString("T").Y * battleChemicals[name].CurrentHealth.ToString().Length + 4);
                r.X = (int)battleChemicals[name].Image.Position.X + 64;
                r.Y = 0;
                battleChemicals[name].TagRectangle = r;
            }
        }
        public void BattleDraw(SpriteBatch spriteBatch)
        {
            shadow.Draw(spriteBatch);
            for (int i = battleChemicalName.Count - 1; i >= 0; i-- )
            {
                shadow.Position = battleChemicals[battleChemicalName[i]].Image.Position + new Vector2(-11, 100);
                shadow.Draw(spriteBatch);
                battleChemicals[battleChemicalName[i]].Draw(spriteBatch);
            }
        }
        public void DrawVerticalTag(SpriteBatch spriteBatch)
        {
            foreach (string name in battleChemicalName)
            {
                string currentHealth = battleChemicals[name].CurrentHealth.ToString();
                string vHealth = String.Empty;
                for (int i = 0; i < currentHealth.Length; i++)
                    vHealth += currentHealth[i].ToString() + "\n\r";

                Vector2 tagPosition = new Vector2(battleChemicals[name].TagRectangle.X, 0);
                spriteBatch.Draw(tag.Texture, battleChemicals[name].TagRectangle, Color.White);
                if (battleChemicals[name].CurrentHealth / battleChemicals[name].Health > 0.3)
                    spriteBatch.DrawString(tag.Font, vHealth, tagPosition, Color.Gray);
                else 
                    spriteBatch.DrawString(tag.Font, vHealth, tagPosition, Color.Orange);
                tagPosition.X -= 22;
                spriteBatch.DrawString(tag.Font, battleChemicals[name].BattleTag, tagPosition, Color.Black);
            }
        }
        public void DrawHorizontalTag(SpriteBatch spriteBatch)
        {
            foreach (string name in battleChemicalName)
            {
                float rowHeight = tag.Font.MeasureString(" ").Y;
                Vector2 imgPos = battleChemicals[name].Image.Position;
                Vector2 tagPosition = new Vector2(imgPos.X + 64 - tag.Font.MeasureString(battleChemicals[name].BattleTag).X / 2, imgPos.Y + 128 + rowHeight);
                spriteBatch.DrawString(tag.Font, battleChemicals[name].BattleTag, tagPosition, Color.Black);
                tagPosition.Y += rowHeight;
                spriteBatch.DrawString(tag.Font, battleChemicals[name].CurrentHealth.ToString(), tagPosition, Color.LightGray);
            }
        }
        #endregion

        #region Getter/Setter Methods
        public Chemical GetChemical(string chemicalName)
        {
            if (chemicals.ContainsKey(chemicalName))
                return chemicals[chemicalName];
            else 
                return null;
        }
        public Chemical GetBattleChemical(string chemicalName)
        {
            if (battleChemicals.ContainsKey(chemicalName))
                return chemicals[chemicalName];
            else 
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
        #endregion
    }
}
