using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YoutubeRPG
{
    public class ConversationManager
    {
        Menu menu;
        List<Menu> clone;

        CharacterManager characterManager;
        bool isTransitioning;

        string prevMenuID;
        string currentMenuID;
        string selectedItem;
        List<Image> scrollingText;
        Image arrow;

        public ConversationManager()
        {
            prevMenuID = currentMenuID = selectedItem = String.Empty;
            arrow = new Image();
            scrollingText = new List<Image>();
            clone = new List<Menu>();
            menu = new Menu();
            menu.OnMenuChanged += menu_OnMenuChange;    //OnMenuChanged = event;
                                                        //Adds the method, "menu_OnMenuChanged" into event OnMenuChanged
        }
        public bool IsActive
        {
            get { return menu.Active; }
        }
        #region Event
        public void menu_OnMenuChange(object sender, EventArgs e)
        {
            if (currentMenuID != String.Empty)
                clone.Add(menu);
            else
                clone.Clear();

            XmlManager<Menu> XmlMenuManager = new XmlManager<Menu>();
            if (menu.ID.Contains(".xml") || menu.ID == String.Empty)
            {
                menu.UnloadContent();
                menu = XmlMenuManager.Load(menu.ID);
                menu.LoadContent();
                menu.OnMenuChanged += menu_OnMenuChange;
                menu.Transition(0.0f);
            }

            foreach (MenuItem item in menu.Items)
            {
                item.Image.StoreEffects();
                item.Image.ActivateEffect("FadeEffect");
            }
            if (!currentMenuID.Contains("Conversation"))
            {
                foreach (Image image in scrollingText)
                    image.UnloadContent();
                scrollingText.Clear();
            }
        }
        #endregion

        #region Main Methods
        public void LoadContent(string menuPath)
        {
            if (menuPath != String.Empty)
            {
                menu.ID = menuPath;
                prevMenuID = currentMenuID = menuPath;
                arrow.FontName = "Fonts/OCRAsmall";
                arrow.Path = "Misc/arrow_down";
                arrow.Position = new Vector2(ScreenManager.Instance.Dimensions.X -35, ScreenManager.Instance.Dimensions.Y - 25);
                arrow.Effects = "FadeEffect";
                arrow.IsActive = true;
                arrow.LoadContent();
            }
        }
        public void UnloadContent()
        {
            menu.UnloadContent();
            arrow.UnloadContent();
            if (characterManager != null)
                characterManager.UnloadContent();
            foreach (Menu m in clone)
                m.UnloadContent();
            foreach (Image i in scrollingText)
                i.UnloadContent();
        }
        public void Update(GameTime gameTime, ref Player player)
        {
            if (!isTransitioning)
                menu.Update(gameTime);
            Transition(gameTime);
            arrow.Update(gameTime);
            characterManager = player.CharacterManager;
            foreach (Image i in scrollingText)
                i.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Menu m in clone)
                m.Draw(spriteBatch);
            menu.Draw(spriteBatch);
            if (menu.Type.Contains("Conversation") && IsActive)
            {
                arrow.Draw(spriteBatch);
            }
            foreach (Image i in scrollingText)
                i.Draw(spriteBatch);
        }
        #endregion
        #region Option Menus
        void scrollingTextConvert()
        {
            //Have some test dialogue
        }
        void yesnoMenu()
        {

        } 
        #endregion
        #region Misc Functions
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
        #endregion
        #region User Input
        public void SelectLeft(eButtonState buttonState)
        {
            if (buttonState == eButtonState.DOWN)
            {
                menu.ItemNumber--;
            }
        }
        public void SelectRight(eButtonState buttonState)
        {
            if (buttonState == eButtonState.DOWN)
            {
                menu.ItemNumber++;
            }
        }
        public void SelectDown(eButtonState buttonState)
        {
            if (buttonState == eButtonState.DOWN)
                menu.ItemNumber++;
        }
        public void SelectUp(eButtonState buttonState)
        {
            if (buttonState == eButtonState.DOWN)
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
                    currentMenuID = "Content/Load/Conversation/Intro.xml";
                    menu.ID = currentMenuID;
                    menu.Active = true;
                }
            }
        }
        public void MenuSelect(eButtonState buttonState)
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
        #endregion
    }
}
