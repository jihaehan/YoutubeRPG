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

        int exit;
        string ID;
        string currentMenuID;
        List<Image> scrollingText;
        Dictionary<string, MenuItem> dialogue; //LinkID, Text
        string currentDialogue;
        Image arrow;


        public ConversationManager()
        {
            exit = 1;
            currentMenuID = currentDialogue = ID = String.Empty;
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
            if (!currentMenuID.Contains(".xml"))
            {
                menu.UnloadContent();
                menu = XmlMenuManager.Load("Content/Load/Conversation/Intro.xml");
                menu.Active = true;
                conversationMenu();
                //scrollingDialogue();
                menu.LoadContent();
                menu.OnMenuChanged += menu_OnMenuChange;
                menu.Transition(0.0f);

            }
            else if (menu.ID.Contains(".xml") || menu.ID == String.Empty)
            {
                menu.UnloadContent();
                if (currentMenuID == String.Empty)
                    menu = XmlMenuManager.Load(menu.ID);
                else
                    menu = XmlMenuManager.Load(currentMenuID);
                menu.LoadContent();
                menu.OnMenuChanged += menu_OnMenuChange;
                menu.Transition(0.0f);
                if (currentMenuID.Contains("Question"))
                { 
                    questionMenu();
                }
            }
            if (currentDialogue != String.Empty)// && !currentMenuID.Contains("Question"))
                scrollingDialogue();


            foreach (MenuItem item in menu.Items)
            {
                item.Image.StoreEffects();
                item.Image.ActivateEffect("FadeEffect");
            }

        }
        #endregion

        #region Main Methods
        public void LoadContent(string menuPath)
        {
            if (menuPath != String.Empty)
            {
                menu.ID = menuPath;
                currentMenuID = menuPath;
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
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                foreach (Menu m in clone)
                    m.Draw(spriteBatch);   
                menu.Draw(spriteBatch);
                if (menu.Type.Contains("Conversation"))
                {
                    arrow.Draw(spriteBatch);
                }
                foreach (Image i in scrollingText)
                    i.Draw(spriteBatch);
            }
        }
        #endregion
        #region Option Menus
        void questionMenu()
        {
            //Yes:
            string id = ID + "1";
            menu.Items[0].LinkID = id;
            //menu.Items[0].LinkType = dialogue[id].LinkType;
            //No:
            id = ID + "0";
            menu.Items[1].LinkID = id;
            //menu.Items[1].LinkType = dialogue[id].LinkType;

        }
        void conversationMenu()
        {
            if (currentDialogue == String.Empty)
            {
                foreach (MenuItem item in menu.Items)
                    dialogue.Add(item.LinkID, item);
                currentDialogue = dialogue["0"].Image.Text;
                scrollingDialogue();
                currentMenuID = menu.Items[0].LinkID;
                menu.Items.Clear();
                menu.Items.Add(dialogue["0"]);
            }
            else
            {
                menu.Items.Clear();
                menu.Items.Add(dialogue[ID]);
                currentDialogue = dialogue[ID].Image.Text;
                menu.Items[0].Image.Text = " ";
                menu.Items[0].Image.FontName = "Fonts/OCRA";
            }
        }

        void setDialogue()
        {
            if (ID != String.Empty)
            {
                currentDialogue = dialogue[ID].Image.Text;
                switch (dialogue[ID].LinkType)
                {
                    case "Question":
                        currentMenuID = "Content/Load/Menu/QuestionMenu.xml";
                        break;
                    case "End":
                        menu.Active = false;
                        clone.Clear();
                        break;
                    case "Battle":
                        break;
                    case "Party":
                        break;
                }
            }
                
        }
        void scrollingDialogue()
        {
            //Unload images from previous scrolling Text
            foreach (Image image in scrollingText)
                image.UnloadContent();
            scrollingText.Clear();
            Vector2 dimensions = new Vector2(570, 580);
            Image i = new Image();

            //Cut dialogue into individual bits
            string[] parts = currentDialogue.Split(' ');
            string text = String.Empty;
            int rowLength = 0;
            int count = 0;
            foreach (string s in parts)
            {
                if ((rowLength + s.Length) < 36)
                {
                    rowLength += s.Length + 1;
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
                        text += "\n\r" + s + " ";
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
                    rowLength = s.Length + 1;
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
                    currentMenuID = menu.Items[menu.ItemNumber].LinkID;
                    if (!ID.Contains(".xml") && currentMenuID!= String.Empty)
                        ID = currentMenuID;
                    setDialogue();

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
