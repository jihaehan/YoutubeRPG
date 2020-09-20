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
        public List<string> tempChemicalName;
        public List<string> deadChemicalName;
        List<Chemical> clone;
        Image tag;
        Image shadow;
        int maxVisibleChemicals;
        int[] percentage = { 3, 10 }; //percentage chance for Temporary Chemical to stay


        public ChemicalManager()
        {
            ChemicalSource = new List<string>();
            CurrentChemicalName = String.Empty;
            chemicals = new Dictionary<string, Chemical>();
            chemicalName = new List<string>();
            maxVisibleChemicals = 3;
            battleChemicals = new Dictionary<string, Chemical>();
            battleChemicalName = new List<string>();
            tempChemicalName = new List<string>();
            deadChemicalName = new List<string>();
            tag = new Image();
            shadow = new Image();
            clone = new List<Chemical>();
             
        }
        #region Temp Chemical Methods
        public string RemoveRandomTempChemical()
        {
            Random rnd = new Random();
            int r = rnd.Next(0, 100);
            if (tempChemicalName.Count > 0 && r > 70) //30% chance of staying
            {
                int randomIndex = rnd.Next(0, tempChemicalName.Count);
                string removeTemp = tempChemicalName[randomIndex];
                battleChemicals[removeTemp].IsDead = true;
                return removeTemp;
            }
            else
                return String.Empty;
        }
        public void UnloadTempChemicals()
        {
            foreach (string name in tempChemicalName)
            {
                chemicals[name].UnloadContent();
                chemicalName.Remove(name);
                chemicals.Remove(name);
            }
            tempChemicalName.Clear();
        }
        public void LoadTempChemical(string name, string series)
        {
            string xmlPath = "Content/Load/Chemical/" + series + "/" + name + ".xml";
            XmlManager<Chemical> chemicalLoader = new XmlManager<Chemical>();
            Chemical chemical = chemicalLoader.Load(xmlPath);
            chemical.IsTemp = true;
            chemical.InBattle = true;
            while (chemicals.ContainsKey(name))
                name += "*";
            chemical.LoadContent();
            chemicalName.Add(name);
            chemicals.Add(name, chemical);
            tempChemicalName.Add(name);
        }
        #endregion

        public void LoadIsomer(string chemicalName, int branches)
        {
            battleChemicals[chemicalName].IsomerTransition(branches);
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
            UnloadTempChemicals();
        }

        /// <summary>
        /// GameplayScreen: Update all chemicals in player party  
        /// </summary>
        public void Update(GameTime gameTime, Player player)
        {
            Chemical chemical = new Chemical();
            for (int count = 0; count < Math.Min(maxVisibleChemicals, chemicals.Count); count++)
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
            for (int count = 0; count < Math.Min(maxVisibleChemicals, chemicals.Count); count++)
            {
                chemicals[chemicalName[count]].Draw(spriteBatch);
            }
        }
        #region Battle Methods
        /// <summary>
        /// BattleScreen: update/draw all chemicals currently in battle
        /// </summary>
        public int GetTurnCount()
        {
            int count = 0;
            foreach (string name in battleChemicalName)
            {
                if (battleChemicals[name].TurnTaken)
                    count++;
            }
            return count;
        }
        public string EnemyInstance()
        {
            string enemyButton = String.Empty;
            int count = 0;
            for (int i = 0; i < battleChemicalName.Count; i++)
            {
                string name = battleChemicalName[i];
                if (battleChemicals[name].TurnTaken)
                    count++;
                else if (enemyButton == String.Empty && !battleChemicals[name].TurnTaken)
                    enemyButton = name;
            }
            if (enemyButton != String.Empty)
            {
                battleChemicals[enemyButton].TurnTaken = true;  
                if (enemyButton.Contains("*"))
                {
                    string[] str = enemyButton.Split('*');
                    enemyButton = str[0];
                }
            }
            
            return enemyButton;
        }
        public int BattlePartySize()
        {
            return battleChemicals.Count;
        }
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
        public string DrawRandomCard()
        {
            //random card from chemicalsList (not battleready List)
            if (chemicals.Count() != battleChemicals.Count())
            {
                Random rnd = new Random();
                List<string> storedChemicals = new List<string>();
                foreach (string name in chemicalName)
                {
                    if (!chemicals[name].InBattle)
                        storedChemicals.Add(name);
                }
                int randIndex = rnd.Next(storedChemicals.Count());
                return storedChemicals[randIndex];
            }
            else
                return String.Empty;
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
            // Remove Temporary Chemicals
            List<string> deadTemp = new List<string>();
            foreach (string name in tempChemicalName)
            {
                string dead = String.Empty;
                if (battleChemicalName.Contains(name))
                    if (battleChemicals[name].IsDead)
                        dead = name;
                if (dead != String.Empty)
                {
                    clone.Add(battleChemicals[dead]);
                    battleChemicals.Remove(dead);
                    battleChemicalName.Remove(dead);
                    chemicalName.Remove(dead);
                    chemicals.Remove(dead);
                    deadTemp.Add(dead);
                }
            }
            for (int i = 0; i < deadTemp.Count; i++)
                tempChemicalName.Remove(deadTemp[i]);
            deadTemp.Clear();
            // manage chemical clones
            int deadIndex = -1;
            for (int i = 0; i < clone.Count;i++)
            {
                clone[i].LeaveBattleScreen(gameTime, isPlayer);
                if (clone[i].Image.Position.X > 1280 || clone[i].Image.Position.X < -128)
                    deadIndex = i;
            }
            if (deadIndex > -1)
            {
                clone[deadIndex].UnloadContent();
                clone.RemoveAt(deadIndex);
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

                if (battleChemicals[name].CurrentHealth <= 0 || battleChemicals[name].IsDead)
                    deadTemp.Add(name);
            }
            //remove dead battle Chemicals
            for (int i = 0; i < deadTemp.Count; i++)
            {
                battleChemicals[deadTemp[i]].UnloadContent();
                battleChemicals.Remove(deadTemp[i]);
                battleChemicalName.Remove(deadTemp[i]);
                if (!deadChemicalName.Contains(deadTemp[i]))
                    deadChemicalName.Add(deadTemp[i]);
            }
            deadTemp.Clear();

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
            foreach (Chemical c in clone)
                c.Draw(spriteBatch);
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
        public void SaveParty()
        {
            ScreenManager.Instance.Party = chemicals;
        }
        public void LoadParty()
        {
            ScreenManager.Instance.Experience = 0;
            chemicals = ScreenManager.Instance.Party;
        }
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
