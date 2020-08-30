using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YoutubeRPG
{
    public enum ItemType
    {
        Bromide,
        Chloride,
        Iodide,

        HydrogenBromide,
        HydrogenChloride,
        HydrogenIodide,

        NickelDihydride, //H2Ni
        SulphuricAcid,   //H2SO4
        SodiumHydroxide  //NaOH       
    }
    public class Item
    {
        public ItemType ItemType;
        public Image Image;

        public void LoadContent()
        {
            Image.LoadContent();
        }
        public void UnloadContent()
        {
            Image.UnloadContent();
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
