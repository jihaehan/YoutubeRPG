using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YoutubeRPG
{
    public class MenuManager
    {
        Menu menu; 
        public MenuManager()
        {
            menu = new Menu();
            menu.OnMenuChanged += menu_OnMenuChange;    //OnMenuChanged = event;
                                                        //Adds the method, "menu_OnMenuChanged" into event OnMenuChanged
        }
        public void menu_OnMenuChange(object sender, EventArgs e)
        {
            XmlManager<Menu> XmlMenuManager = new XmlManager<Menu>();
            menu.UnloadContent();
            //Transition
            menu = XmlMenuManager.Load(menu.ID);
            menu.LoadContent();
        }
        public void LoadContent(string menuPath)
        {
            if (menuPath != String.Empty)
                menu.ID = menuPath;
        }
        public void UnloadContent()
        {
            menu.UnloadContent();
        }
        public void Update(GameTime gameTime)
        {
            menu.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            menu.Draw(spriteBatch);
        }
    }
}
