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
        ChemicalManager ChemicalManager;
        public Image Image;
        public bool Battled;

        public Character()
        {
            ChemicalManager = new ChemicalManager();
            Image = new Image();
            Battled = false; 
        }
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
        }
        public void LoadContent()
        {
            Image.LoadContent();
            Image.IsActive = true;
        }
        public void UnloadContent()
        {
            Image.UnloadContent();
            ChemicalManager.UnloadContent();
        }
        public void Update(GameTime gameTime)
        {
            Image.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Image.Draw(spriteBatch);
        }
    }
}
