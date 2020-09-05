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
        List<Menu> clone; 

        ChemicalManager chemicalManager;
        bool isTransitioning;

        string prevMenuID;
        string currentMenuID;
        int prevSelectedItem, gameplayMenuSelectedItem;

        SpriteFont font;

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
                        menu.ID = currentMenuID;
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
            prevSelectedItem = gameplayMenuSelectedItem = 0;
            prevMenuID = currentMenuID = String.Empty;
            clone = new List<Menu>();
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
            if (!currentMenuID.Contains("GameplayMenu") && currentMenuID != String.Empty)
                clone.Add(menu);
            else
                clone.Clear();

            XmlManager<Menu> XmlMenuManager = new XmlManager<Menu>();
            menu.UnloadContent();
            menu = XmlMenuManager.Load(menu.ID);

            if (currentMenuID.Contains("OptionInfo"))
                OptionInfoMenu();

            menu.LoadContent();
            menu.OnMenuChanged += menu_OnMenuChange;
            menu.Transition(0.0f);

            foreach (MenuItem item in menu.Items)
            {
                item.Image.StoreEffects();
                item.Image.ActivateEffect("FadeEffect");
            }

            if (currentMenuID.Contains("GameplayMenu"))
            {
                menu.Active = true;
                menu.ItemNumber = gameplayMenuSelectedItem;
                font = menu.Image.Font;
                //prevSelectedItem = 0;
            }
            else 
                menu.ItemNumber = prevSelectedItem;
        }
        public void LoadContent(string menuPath)
        {
            if (menuPath != String.Empty)
            {
                menu.ID = menuPath;
                prevMenuID = currentMenuID = menuPath;
            }
        }
        public void UnloadContent()
        {
            menu.UnloadContent();
            if (chemicalManager != null)
                chemicalManager.UnloadContent();
            foreach (Menu m in clone)
                m.UnloadContent();    
        }
        public void Update(GameTime gameTime)
        {
            if (!isTransitioning)
                menu.Update(gameTime);
            Transition(gameTime);
        }
        public void Update(GameTime gameTime, ref ChemicalManager manager)
        {
            if (!isTransitioning)
               menu.Update(gameTime);
            Transition(gameTime);
            chemicalManager = manager;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Menu m in clone)
                m.Draw(spriteBatch);
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
                    prevMenuID = currentMenuID;
                    currentMenuID = menu.Items[menu.ItemNumber].LinkID;
                    
                    menu.Transition(1.0f);
                    foreach (MenuItem item in menu.Items)
                    {
                        item.Image.StoreEffects();
                        item.Image.ActivateEffect("FadeEffect");
                    }
                }
            }
        }
        public void MenuSelect_Test(eButtonState buttonState)
        {
            if (buttonState == eButtonState.DOWN && !isTransitioning && IsActive)
            {
                if (menu.Items[menu.ItemNumber].LinkType == "Screen")
                    ScreenManager.Instance.ChangeScreens(menu.Items[menu.ItemNumber].LinkID);
                else if (menu.Items[menu.ItemNumber].LinkType == "None")
                {/*no action*/}
                else
                {
                    prevMenuID = currentMenuID;
                    if (prevMenuID.Contains("GameplayMenu"))
                        gameplayMenuSelectedItem = menu.ItemNumber;
                    prevSelectedItem = menu.ItemNumber;
                    currentMenuID = menu.Items[menu.ItemNumber].LinkID;

                    isTransitioning = true;
                    if (isTransitioning)
                    {
                        menu.ID = currentMenuID;
                        isTransitioning = false;
                    }
                }
            }
            else if (!IsActive)
            {
                Activate(buttonState);
            }

        }
        public void PrevMenuSelect(eButtonState buttonState)
        {
            if (buttonState == eButtonState.DOWN && !isTransitioning)
            {
                if (prevMenuID != String.Empty && prevMenuID != currentMenuID)
                {
                    if (clone.Count > 0)
                        clone.Remove(clone[clone.Count - 1]);
                    currentMenuID = prevMenuID;
                    menu.ID = currentMenuID;

                }
                else if (!prevMenuID.Contains("Gameplay"))
                {
                    currentMenuID = prevMenuID = "Content/Load/Menu/GameplayMenu.xml";
                    menu.ID = currentMenuID;
                    menu.Active = true;
                    clone.Clear();
                }
                else if (IsActive)
                    Activate(buttonState);
            }
        }
        public void SelectRight(eButtonState buttonState)
        {
            if (/*menu.Axis == "X" && */buttonState == eButtonState.DOWN)
            {
                menu.ItemNumber++;
                if (currentMenuID.Contains("GameplayMenu"))
                    menu.ItemNumber++;
            }
        }
        public void SelectLeft(eButtonState buttonState)
        {
            if (/*menu.Axis == "X" && */buttonState == eButtonState.DOWN)
            {
                menu.ItemNumber--;
                if (currentMenuID.Contains("GameplayMenu"))
                    menu.ItemNumber--;
            }
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
                {
                    currentMenuID = "Content/Load/Menu/GameplayMenu.xml";
                    menu.ID = currentMenuID;
                    
                    menu.Active = true;
                    prevSelectedItem = 0;
                }
            }
        }
        public void OptionInfoMenu()
        {
            menu.Items.Clear();
            foreach (string chemicalName in chemicalManager.chemicalName)
            {
                MenuItem item = new MenuItem();
                item.Image = new Image();
                item.Image.Text = chemicalName.ToUpper();
                string s = (chemicalManager.GetChemical(chemicalName).State).ToString().ToLower();
                item.Image.Text += "(" + s.Substring(0, 1) + ")";

                string h = (chemicalManager.GetChemical(chemicalName).CurrentHealth).ToString() + "/" + (chemicalManager.GetChemical(chemicalName).Health).ToString() ;

                if (font != null)
                {
                    string space = " ";
                    int spaceNum = (int)((ScreenManager.Instance.Dimensions.X - 730 - font.MeasureString(h).X - font.MeasureString(item.Image.Text).X)/font.MeasureString(space).X);
                    for (int i = 0; i < spaceNum; i++)
                        item.Image.Text += " ";
                    item.Image.Text += h;
                }

                item.Image.TextColor = Color.Black;
                item.Image.FontName = "Fonts/OCRAsmall";
                item.LinkType = "Info";
                if (!chemicalName.Contains("*"))
                    item.LinkID = "Content/Chemical/Image/" + chemicalName + ".xml";
                else
                {
                    string[] str = chemicalName.Split('*');
                    item.LinkID = "Content/Chemical/Image" + str[0] + ".xml";
                }
                menu.Items.Add(item);
            }
        }
    }
}
