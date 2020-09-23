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
        public string newPartyMember;
        public string DialoguePath, PrevDialoguePath;
        Menu menu;
        bool isTransitioning;
        bool isDescription;
        string ID;
        string currentMenuID;
        List<Image> scrollingText;
        Dictionary<string, MenuItem> dialogue; //LinkID, Text
        string currentDialogue, previousDialogue;
        Image arrow;
        Image background;


        public ConversationManager()
        {
            newPartyMember = String.Empty;
            DialoguePath = String.Empty;
            PrevDialoguePath = String.Empty;//".";
            isDescription = false;
            background = new Image();
            currentMenuID = currentDialogue = ID = String.Empty;
            previousDialogue = ".";
            arrow = new Image();
            scrollingText = new List<Image>();
            dialogue = new Dictionary<string, MenuItem>();
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
            XmlManager<Menu> XmlMenuManager = new XmlManager<Menu>();
            if (!currentMenuID.Contains(".xml"))
            {
                menu.UnloadContent();
                if (DialoguePath != String.Empty)
                    menu = XmlMenuManager.Load("Content/Load/Conversation/"+DialoguePath+".xml");
                else 
                    menu = XmlMenuManager.Load("Content/Load/Conversation/Intro.xml");
                conversationMenu();
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
            if (currentDialogue != String.Empty && currentDialogue != previousDialogue)
                scrollingDescription(Color.Black);
                

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
                if (menuPath.Contains(".xml"))
                {
                    menu.ID = menuPath;
                    currentMenuID = menuPath;
                }
                else
                {
                    DialoguePath = menuPath;
                    menu.ID = "Content/Load/Conversation/" + DialoguePath + ".xml";
                }
                arrow.FontName = "Fonts/OCRAsmall";
                arrow.Path = "Misc/arrow_down";
                arrow.Position = new Vector2(ScreenManager.Instance.Dimensions.X -35, ScreenManager.Instance.Dimensions.Y - 19);
                arrow.Effects = "FadeEffect";
                arrow.IsActive = true;
                arrow.LoadContent();
                background = new Image();
                background.Path = "Misc/conversation_menu";
                background.Position = new Vector2(540, 562);
                background.LoadContent();
            }
        }
        public void UnloadContent()
        {
            background.UnloadContent();
            menu.UnloadContent();
            arrow.UnloadContent();
            foreach (Image i in scrollingText)
                i.UnloadContent();
        }
        public void Update(GameTime gameTime, ref Player player)
        {
            if (IsActive)
            { 
                if (!isTransitioning)
                    menu.Update(gameTime);
                Transition(gameTime);
                arrow.Update(gameTime);
                background.Update(gameTime);
                foreach (Image i in scrollingText)
                    i.Update(gameTime);
                if (newPartyMember != String.Empty)
                {
                    XmlManager<Chemical> chemicalLoader = new XmlManager<Chemical>();
                    Chemical chemical = chemicalLoader.Load("Content/Load/Chemical/" + newPartyMember + ".xml");
                    chemical.LoadContent();
                    string[] str = newPartyMember.Split('/');
                    string chemicalName = str[str.Length - 1];
                    player.ChemicalManager.AddChemical(chemical);
                    newPartyMember = String.Empty;
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                background.Draw(spriteBatch); 
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
                //scrollingDialogue();
                scrollingDescription(Color.Black);
                currentMenuID = menu.Items[0].LinkID;
                menu.Items.Clear();
                menu.Items.Add(dialogue["0"]);
            }
            else if (dialogue.ContainsKey(ID))
            {
                menu.Items.Clear();
                menu.Items.Add(dialogue[ID]);
                previousDialogue = currentDialogue;
                currentDialogue = dialogue[ID].Image.Text;
                menu.Items[0].Image.Text = " ";
                menu.Items[0].Image.FontName = "Fonts/OCRA";
            }
            else if (menu.Active)
            {
                menu.Items.Clear();
                menu.Active = false;
                previousDialogue = ".";
                currentDialogue = String.Empty;
                dialogue.Clear();
            }
        }

        void setDialogue()
        {
            if (ID != String.Empty && dialogue.ContainsKey(ID))
            {
                previousDialogue = currentDialogue;
                currentDialogue = dialogue[ID].Image.Text;
                switch (dialogue[ID].LinkType)
                {
                    case "Question":
                        currentMenuID = "Content/Load/Menu/QuestionMenu.xml";
                        break;
                    case "End":
                        currentMenuID = "Content/Load/Menu/EndMenu.xml";
                        break;
                    case "Battle":
                        ScreenManager.Instance.Enemy = DialoguePath;
                        ScreenManager.Instance.ChangeScreens("BattleScreen");
                        break;
                    case "Party":
                        string[] str = currentDialogue.Split(':');
                        currentDialogue = str[1];
                        newPartyMember = str[0];
                        currentMenuID = "Content/Load/Menu/EndMenu.xml";
                        break;
                }
            }
                
        }
        
        #endregion
        #region Helper Functions
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
                {
                    menu.Active = false;
                }
                else
                {
                    currentMenuID = String.Empty;
                    currentDialogue = String.Empty;
                    dialogue.Clear();
                    if (DialoguePath != String.Empty)
                        menu.ID = "Content/Load/Conversation/" + DialoguePath + ".xml";
                    menu.Active = true;
                }
            }
        }
        public void MenuSelect(eButtonState buttonState)
        {
            if (buttonState == eButtonState.DOWN && !isTransitioning && IsActive)
            {
                if (isDescription)
                {
                    if (scrollingText.Count > 3)
                        scrollingText.RemoveRange(0, Math.Min(3, scrollingText.Count));
                    for (int i = 0; i < Math.Min(scrollingText.Count, 3); i++)
                        scrollingText[i].IsVisible = true;
                    if (scrollingText.Count < 3)
                        isDescription = false;
                }
                if (menu.Items[menu.ItemNumber].LinkType == "Screen")
                    ScreenManager.Instance.ChangeScreens(menu.Items[menu.ItemNumber].LinkID);
                else if (menu.Items[menu.ItemNumber].LinkType == "None")
                {/*no action*/}
                else if (scrollingText.Count < 3)
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

        #region Misc Functions
        void scrollingTextClear()
        {
            foreach (Image i in scrollingText)
                i.UnloadContent();
            scrollingText.Clear();
        }
        void scrollingTextLoadContent()
        {
            foreach (Image i in scrollingText)
                i.LoadContent();
        }
        void scrollingDescription(Color textColor)
        {
            scrollingTextClear();
            List<Image> imageList = new List<Image>();
            Image i = new Image();
            Vector2 dimensions = new Vector2(570, 580f);
            i.FontName = "Fonts/OCRAsmall";
            i.TextColor = textColor;
            i.Position = dimensions;
            string[] parts = currentDialogue.Split(' ');
            string text = String.Empty;
            int rowLength = 0;
            int count = 1;
            foreach (string s in parts)
            {
                if (s == "[row]")
                {
                    i.Text = text;
                    imageList.Add(i);
                    i = new Image();
                    rowLength = 0;
                    dimensions.Y += 42f;
                    if (count % 3 == 0)
                    {
                        count = 0;
                        dimensions = new Vector2(570, 580);
                    }
                    count++;
                    i.FontName = "Fonts/OCRAsmall";
                    i.TextColor = textColor;
                    i.Position = dimensions;
                    text = String.Empty;
                }
                else if ((rowLength + s.Length) < 30)
                {
                    rowLength += s.Length + 1;
                    text += s + " ";
                    if (s == parts[parts.Length - 1]) //if string is last word in dialogue
                    {
                        i = new Image();
                        i.Text = text;
                        i.FontName = "Fonts/OCRAsmall";
                        i.TextColor = textColor;
                        i.Position = dimensions;
                        imageList.Add(i);
                    }
                }
                else
                {
                    i.Text = text;
                    imageList.Add(i);
                    i = new Image();
                    dimensions.Y += 42;
                    if (count % 3 == 0)
                    {
                        count = 0;
                        dimensions = new Vector2(570, 580);
                    }
                    count++;
                    i.Position = dimensions;
                    i.TextColor = textColor;
                    i.FontName = "Fonts/OCRAsmall";

                    text = s + " ";
                    rowLength = s.Length + 1;

                    if (s == parts[parts.Length - 1]) //if string is last word in dialogue
                    {
                        i = new Image();
                        i.Text = text;
                        i.FontName = "Fonts/OCRAsmall";
                        i.TextColor = textColor;
                        i.Position = dimensions;
                        imageList.Add(i);
                    }
                }
            }
            if (imageList.Count > 3)
            {
                for (int j = 3; j < imageList.Count; j++)
                    imageList[j].IsVisible = false;
                isDescription = true;
            }
            scrollingText = imageList;
            scrollingTextLoadContent();
        }
        void scrollingDescriptionContinued(Color textColor)
        {
            scrollingTextClear();
            List<Image> imageList = new List<Image>();
            Image i = new Image();
            Vector2 dimensions = new Vector2(570, 580);
            i.FontName = "Fonts/OCRAsmall";
            i.TextColor = textColor;
            i.Position = dimensions;
            string[] parts = currentDialogue.Split(' ');
            string text = String.Empty;
            int rowLength = 0;
            int count = 1;
            foreach (string s in parts)
            {
                if (s == "[row]")
                {
                    i.Text = text;
                    imageList.Add(i);
                    i = new Image();
                    rowLength = 0;
                    dimensions.Y += 42f;
                    if (count % 3 == 0)
                    {
                        count = 0;
                        dimensions = new Vector2(570, 580);
                    }
                    count++;
                    i.FontName = "Fonts/OCRAsmall";
                    i.TextColor = textColor;
                    i.Position = dimensions;
                    text = String.Empty;
                }
                else if ((rowLength + s.Length) < 30)
                {
                    rowLength += s.Length + 1;
                    text += s + " ";
                    if (s == parts[parts.Length - 1]) //if string is last word in dialogue
                    {
                        i = new Image();
                        i.Text = text;
                        i.FontName = "Fonts/OCRAsmall";
                        i.TextColor = textColor;
                        i.Position = dimensions;
                        imageList.Add(i);
                    }
                }
                else
                {
                    i.Text = text;
                    imageList.Add(i);
                    i = new Image();
                    dimensions.Y += 42;
                    if (count % 3 == 0)
                    {
                        count = 0;
                        dimensions = new Vector2(570, 580);
                    }
                    count++;
                    i.Position = dimensions;
                    i.TextColor = textColor;
                    i.FontName = "Fonts/OCRAsmall";

                    text = s + " ";
                    rowLength = s.Length + 1;

                    if (s == parts[parts.Length - 1]) //if string is last word in dialogue
                    {
                        i = new Image();
                        i.Text = text;
                        i.FontName = "Fonts/OCRAsmall";
                        i.TextColor = textColor;
                        i.Position = dimensions;
                        imageList.Add(i);
                    }
                }
            }
            for (int j = 0; j < imageList.Count; j++)
                imageList[j].IsVisible = false;
            isDescription = true;
            scrollingText = imageList;
            scrollingTextLoadContent();
        }

        #endregion
    }
}
