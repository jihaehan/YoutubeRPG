using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YoutubeRPG
{

    public class DialogueManager
    {
        Menu menu;
        List<Menu> clone;
        bool isTransitioning;
        string prevMenuID;
        string currentMenuID;
        string npcName;

        List<Image> infoImage;
        bool isDescription;

        public DialogueManager()
        {
            isDescription = false;
            prevMenuID = currentMenuID = String.Empty;
            npcName = String.Empty;
            infoImage = new List<Image>();
            clone = new List<Menu>();
            menu = new Menu();
            menu.OnMenuChanged += menu_OnMenuChange;
            menu.Active = false;
        }
        public bool IsActive
        {
            get { return menu.Active; }
        }
        public void menu_OnMenuChange(object sender, EventArgs e)
        {
            if (currentMenuID != String.Empty && !currentMenuID.Contains("Dialogue"))
                clone.Add(menu);
            else
                clone.Clear();
            XmlManager<Menu> XmlMenuManager = new XmlManager<Menu>();
            if (menu.ID.Contains(".xml") || menu.ID == String.Empty)
            {
                menu.UnloadContent();
                menu = XmlMenuManager.Load(menu.ID);
            }

            if (currentMenuID.Contains("Dialogue"))
                dialogue();

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
        }

        void dialogue()
        {
            infoImageClear();
            infoImage = scrollingDescription("Welcome to a world where chemical compounds are alive and conscious!", Color.Black);

            infoImageLoadContent();
        }

        public void UpdateNpcName(string name)
        {
            npcName = name;
        }

        #region Main Methods
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
            foreach (Image i in infoImage)
                i.UnloadContent();
        }
        public void Update(GameTime gameTime)
        { 
            if (!isTransitioning)
                menu.Update(gameTime);
            Transition(gameTime);
            foreach (Image i in infoImage)
                i.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Menu m in clone)
                m.Draw(spriteBatch);
            menu.Draw(spriteBatch);
            foreach (Image i in infoImage)
                i.Draw(spriteBatch);
        }
        #endregion

        #region InputMethods
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
                else if (npcName != String.Empty)
                {
                    currentMenuID = "Content/Load/Dialogue/Intro.xml";
                    menu.ID = currentMenuID;
                    menu.Active = true;
                }
                else
                {
                    menu.Active = true;
                }
            }
        }
        public void Activate_Test(eButtonState buttonState)
        {
            if (buttonState == eButtonState.DOWN)
            {
                if (menu.Active)
                    menu.Active = false;
                else
                {
                    currentMenuID = "Content/Load/Dialogue/Intro.xml";
                    menu.ID = currentMenuID;
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
                    infoImage.RemoveRange(0, Math.Min(3, infoImage.Count));
                    for (int i = 0; i < Math.Min(infoImage.Count, 3); i++)
                        infoImage[i].IsVisible = true;
                    if (infoImage.Count < 3)
                        isDescription = false;
                }
                else if (menu.Items.Count <= 0)
                {
                    menu.Active = false;
                }
                else if (menu.Items[menu.ItemNumber].LinkType == "Battle")
                {
                    ScreenManager.Instance.ChangeScreens(menu.Items[menu.ItemNumber].LinkID);
                    //ScreenManager.Instance.Enemy = "name of enemy"; 
                }
                else if (menu.Items[menu.ItemNumber].LinkType == "None")
                {/*no action*/}
                else
                {
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

        #region Scrolling Dialogue
        void infoImageLoadContent()
        {
            foreach (Image image in infoImage)
                image.LoadContent();
        }
        void infoImageClear()
        {
            foreach (Image image in infoImage)
                image.UnloadContent();
            infoImage.Clear();
        }
        List<Image> scrollingDescription(string description, Color textColor)
        {
            List<Image> imageList = new List<Image>();
            Image i = new Image();
            Vector2 dimensions = new Vector2(340f, 580.5f);
            i.FontName = "Fonts/OCRAsmall";
            i.TextColor = textColor;
            i.Position = dimensions;
            string[] parts = description.Split(' ');
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
                        dimensions = new Vector2(340f, 580.5f);
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
                        dimensions = new Vector2(340f, 580.5f);
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
            return imageList;
        }
        List<Image> scrollingDescriptionContinued(string description, Color textColor)
        {
            List<Image> imageList = new List<Image>();
            Image i = new Image();
            Vector2 dimensions = new Vector2(340f, 580.5f);
            i.FontName = "Fonts/OCRAsmall";
            i.TextColor = textColor;
            i.Position = dimensions;
            string[] parts = description.Split(' ');
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
                        dimensions = new Vector2(340f, 580.5f);
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
                        dimensions = new Vector2(340f, 580.5f);
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
            return imageList;
        }

        #endregion

        #region Misc Methods
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
    }
}
