using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YoutubeRPG
{
    public class Character
    {
        public ChemicalManager ChemicalManager;
        public Image Image;
        public bool Battled;
        public string Name, QuestName, QuestDescription;
        bool hasParty;
        public Character()
        {
            ChemicalManager = new ChemicalManager();
            Image = new Image();
            Battled = false;
            Name = QuestName = String.Empty;
            hasParty = false;
        }
        #region Main Methods
        public void LoadContent(string path)
        {
            Image.LoadContent();
            Image.IsActive = true;
            if (Battled == false)
            {
                XmlManager<ChemicalManager> chemicalManagerLoader = new XmlManager<ChemicalManager>();
                ChemicalManager = chemicalManagerLoader.Load(path);
                ChemicalManager.LoadContent();
            }
            hasParty = true;
        }
        public void LoadContent()
        {
            Image.LoadContent();
            Image.IsActive = true;
        }
        public void LoadRandomContent()
        {
            Image.LoadContent();
            if (!Battled)
            {
                ChemicalManager = new ChemicalManager();
            }
            Image.IsActive = true;
            hasParty = true;
        }
        public void UnloadContent()
        {
            Image.UnloadContent();
            if (hasParty)
                ChemicalManager.UnloadContent();
        }
        public void Update(GameTime gameTime)
        {
            Image.Update(gameTime);
            if (hasParty)
                ChemicalManager.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Image.Draw(spriteBatch);
        }
        #endregion

        #region Battle Methods
        public void InitializeBattle()
        {
            ChemicalManager.InitializeBattle(Image.Position);
        }
        public void BattleUpdate(GameTime gameTime)
        {
            ChemicalManager.BattleUpdate(gameTime, Image.Position, false);
            Image.Update(gameTime);
        }
        public void BattleDraw(SpriteBatch spriteBatch)
        {
            ChemicalManager.BattleDraw(spriteBatch);
            ChemicalManager.DrawVerticalTag(spriteBatch);
            Image.Draw(spriteBatch);
        }
        #endregion
    }
}
