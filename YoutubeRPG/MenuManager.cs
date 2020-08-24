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
        bool isTransitioning; 
        
        void Transition(GameTime gameTime)
        {
            if (isTransitioning)
            {
                for (int i = 0; i < menu.Items.Count; ++i)
                {
                    menu.Items[i].Image.Update(gameTime);
                    float first = menu.Items[0].Image.Alpha;
                    float last = menu.Items[menu.Items.Count - 1].Image.Alpha;
                    if (first == 0.0f && last == 0.0f)
                        menu.ID = menu.Items[menu.ItemNumber].LinkID;
                    else if (first == 1.0f && last == 1.0f)
                    {
                        isTransitioning = false;
                        foreach (MenuItem item in menu.Items)
                            item.Image.RestoreEffects(); 
                    }
                }
            }
        }
        public MenuManager()
        {
            menu = new Menu();
            menu.OnMenuChanged += menu_OnMenuChange;    //OnMenuChanged = event;
                                                        //Adds the method, "menu_OnMenuChanged" into event OnMenuChanged
        }
        public bool IsActive
        {
            get { return menu.Active; }
        }
        public void menu_OnMenuChange(object sender, EventArgs e)
        {
            XmlManager<Menu> XmlMenuManager = new XmlManager<Menu>();
            menu.UnloadContent();
            menu = XmlMenuManager.Load(menu.ID);
            menu.LoadContent();
            menu.OnMenuChanged += menu_OnMenuChange;
            menu.Transition(0.0f);

            foreach (MenuItem item in menu.Items)
            {
                item.Image.StoreEffects();
                item.Image.ActivateEffect("FadeEffect");
            }
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
            if (!isTransitioning)
               menu.Update(gameTime);
            Transition(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            menu.Draw(spriteBatch);
        }
        public void MenuSelect(eButtonState buttonState)
        {
            if (buttonState == eButtonState.DOWN && !isTransitioning)
            {
                if (menu.Items[menu.ItemNumber].LinkType == "Screen")
                    ScreenManager.Instance.ChangeScreens(menu.Items[menu.ItemNumber].LinkID);
                else
                {
                    isTransitioning = true;
                    menu.Transition(1.0f);
                    foreach (MenuItem item in menu.Items)
                    {
                        item.Image.StoreEffects();
                        item.Image.ActivateEffect("FadeEffect");
                    }
                }
            }
        }
        public void SelectRight(eButtonState buttonState)
        {
            if (/*menu.Axis == "X" && */buttonState == eButtonState.DOWN)
                menu.ItemNumber++;
        }
        public void SelectLeft(eButtonState buttonState)
        {
            if (/*menu.Axis == "X" && */buttonState == eButtonState.DOWN)
                menu.ItemNumber--;
        }
        public void SelectDown(eButtonState buttonState)
        {
            if (/*menu.Axis == "Y" && */buttonState == eButtonState.DOWN)
                menu.ItemNumber++;
        }
        public void SelectUp(eButtonState buttonState)
        {
            if (/*menu.Axis == "Y" && */buttonState == eButtonState.DOWN)
                menu.ItemNumber--;
        }
        public void Activate(eButtonState buttonState)
        {
            if (buttonState == eButtonState.DOWN)
            {
                if (menu.Active)
                    menu.Active = false;
                else
                    menu.Active = true; 
            }
        }
    }
}
