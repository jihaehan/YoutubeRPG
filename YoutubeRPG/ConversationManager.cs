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
        string currentMenuType;
        List<Image> scrollingText;
        Dictionary<string, MenuItem> dialogue; //LinkID, Text
        string currentDialogue;
        Image arrow;

        public ConversationManager()
        {
            prevMenuID = currentMenuID = currentMenuType = currentDialogue = String.Empty;
            arrow = new Image();
            scrollingText = new List<Image>();
            dialogue = new Dictionary<string, MenuItem>();
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
            }

            if (currentMenuID.Contains("Conversation"))
            {
                conversationMenu();
                scrollingDialogue();
            }

            if (menu.ID.Contains(".xml") || menu.ID == String.Empty)
            { 
                menu.LoadContent();
                menu.OnMenuChanged += menu_OnMenuChange;
                menu.Transition(0.0f);
            }

            foreach (MenuItem item in menu.Items)
            {
                item.Image.StoreEffects();
                item.Image.ActivateEffect("FadeEffect");
            }
            if (currentMenuID.Contains("Conversation"))
            {
                scrollingDialogue();
            }
            else if (currentMenuID.Contains("Question"))
                questionMenu();
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
                arrow.Position = new Vector2(ScreenManager.Instance.Dimensions.X -35, ScreenManager.Instance.Dimensions.Y - 19);
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

            string s = currentMenuID;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Menu m in clone)
                m.Draw(spriteBatch);
            menu.Draw(spriteBatch);
            if (menu.Type.Contains("Conversation") && IsActive)
            {
                arrow.Draw(spriteBatch);
                if (scrollingText.Count > 0)
                    scrollingText[0].Draw(spriteBatch);
            }
            foreach (Image i in scrollingText)
                i.Draw(spriteBatch);
        }
        #endregion
        #region Option Menus
        void questionMenu()
        {
            //Yes:
            menu.Items[0].LinkID = prevMenuID + "1";
            //No:
            menu.Items[1].LinkID = prevMenuID + "0";

        }
        void conversationMenu()
        {
            if (currentDialogue == String.Empty)
            {
                foreach (MenuItem item in menu.Items)
                    dialogue.Add(item.LinkID, item);
                currentDialogue = dialogue["0"].Image.Text;
                currentMenuType = menu.Items[0].LinkType;
                currentMenuID = menu.Items[0].LinkID;
                menu.Items.Clear();
                menu.Items.Add(dialogue["0"]);
                menu.Items[0].Image.Text = " ";
                menu.Items[0].Image.FontName = "Fonts/OCRA";
            }
            else
            {
                menu.Items.Clear();
                menu.Items.Add(dialogue[currentMenuID]);
                currentDialogue = dialogue[currentMenuID].Image.Text;
                menu.Items[0].Image.Text = " ";
                menu.Items[0].Image.FontName = "Fonts/OCRA";
            }
            //unload menu here...
        }
        void setDialogue()
        {
            currentDialogue = dialogue[currentMenuID].Image.Text;
            switch (dialogue[currentMenuID].LinkType)
            {
                case "Question":
                    currentMenuID = "Content/Load/Menu/QuestionMenu.xml";
                    break;
                case "End":
                    currentDialogue = String.Empty;
                    menu.Active = false;
                    clone.Clear();
                    break;
                case "Battle":
                    break;
                case "Party":
                    break;
            }
        }
        void scrollingDialogue()
        {
            //Unload images from previous scrolling Text
            foreach (Image image in scrollingText)
                image.UnloadContent();
            scrollingText.Clear();
            Vector2 dimensions = menu.Alignment;
            Image i = new Image();

            //Cut dialogue into individual bits
            string[] parts = currentDialogue.Split(' ');
            string text = String.Empty;
            int rowLength = 0;
            int count = 0;
            foreach (string s in parts)
            {
                if (rowLength + s.Length < 36)
                {
                    rowLength += s.Length;
                    text += s + " "; 
                    if (s == parts[parts.Length - 1]) //if string is last word in dialogue
                    {
                        i = new Image();
                        i.Text = text;
                        i.FontName = "Fonts/OCRAsmall";
                        i.TextColor = Color.Black;
                        i.Position = dimensions;
                        scrollingText.Add(i);
                        text = String.Empty;
                    }
                }
                else
                {
                    if (count < 3)
                    {
                        count++;
                        text += "\n\r" + s;
                    }
                    else
                    {
                        count = 0;
                        i = new Image();
                        i.Text = text;
                        i.FontName = "Fonts/OCRAsmall";
                        i.TextColor = Color.Black;
                        i.Position = dimensions;
                        scrollingText.Add(i);
                        text = s;
                    }
                    rowLength = s.Length;
                }
            }
            foreach (Image image in scrollingText)
                image.LoadContent();
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
                    currentMenuType = menu.Items[menu.ItemNumber].LinkType;
                    setDialogue(); //PROBLEM: questionMenu answers are not linking properly
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
