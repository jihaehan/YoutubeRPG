using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YoutubeRPG
{
    public class BattleManager
    {
        Menu menu;
        List<Menu> clone;

        ChemicalManager playerChemicals, enemyChemicals;
        ItemManager itemManager;
        bool isTransitioning;

        string prevMenuID;
        string currentMenuID;
        string selectedItem;
        int prevSelectedItem, battleMenuSelectedItem;
        List<Image> infoImage;
        SpriteFont font;
        Image page;
        Image card;
        List<Image> oxygen;
        float totalOxygen;
        float currentOxygen;
        string pageText;

        public BattleManager()
        {
            totalOxygen = currentOxygen = 2;
            prevSelectedItem = battleMenuSelectedItem = 0;
            prevMenuID = currentMenuID = selectedItem = String.Empty;
            pageText = "1/3";
            page = new Image();
            card = new Image();
            infoImage = new List<Image>();
            clone = new List<Menu>();
            menu = new Menu();
            menu.OnMenuChanged += menu_OnMenuChange;
        }
        public bool IsActive
        {
            get { return menu.Active; }
        }
        public void menu_OnMenuChange(object sender, EventArgs e)
        {
            if (!currentMenuID.Contains("Battle") && currentMenuID != String.Empty)
                clone.Add(menu);
            else
                clone.Clear();

            XmlManager<Menu> XmlMenuManager = new XmlManager<Menu>();

            if (menu.ID.Contains(".xml") || menu.ID == String.Empty)
            {
                menu.UnloadContent();
                menu = XmlMenuManager.Load(menu.ID);
            }
            if (currentMenuID.Contains("OptionMove"))
                optionMoveMenu();
            else if (currentMenuID.Contains("OptionItem"))
                optionItemMenu();
            
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
            if (!currentMenuID.Contains("Move") && !currentMenuID.Contains("Option"))
            {
                foreach (Image image in infoImage)
                    image.UnloadContent();
                infoImage.Clear();
            }
            if (currentMenuID.Contains("BattleMenu"))
            {
                menu.Active = true;
                menu.ItemNumber = battleMenuSelectedItem;
                font = menu.Image.Font;
            }
            else if (menu.Type.Contains("Option"))
            {
                menu.ItemNumber = prevSelectedItem;
                optionMenuPage();
            }
            else if (menu.Type == "ItemInfo")
            {
                itemInfoMenu();
            }
            else if (menu.Type == "Book")
            {
                menu.ItemNumber = prevSelectedItem;
                bookMenu();
            }
            else
                menu.ItemNumber = prevSelectedItem;
        }

        #region Main Methods
        public void LoadContent(string menuPath)
        {
            if (menuPath != String.Empty)
            {
                menu.ID = menuPath;
                prevMenuID = currentMenuID = menuPath;
                page.FontName = "Fonts/OCRAExt";
                page.Path = "Misc/page";
                page.Position = new Vector2(307, ScreenManager.Instance.Dimensions.Y - 23);
                page.LoadContent();
                card.Path = "Misc/card";
                card.Effects = "SpriteSheetEffect";
                card.SpriteSheetEffect.AmountOfFrames.X = 1;
                card.SpriteSheetEffect.AmountOfFrames.Y = 2;
                card.SpriteSheetEffect.SwitchFrame = 50;
                card.IsActive = false;
                card.FontName = "Fonts/OCRAsmall";
                card.LoadContent();
                Image O2 = new Image();
                O2.Effects = "SpriteSheetEffect";
                O2.SpriteSheetEffect.AmountOfFrames.X = 1;
                O2.SpriteSheetEffect.AmountOfFrames.Y = 2;
                O2.SpriteSheetEffect.SwitchFrame = 50;
                O2.IsActive = false;
                O2.Position = new Vector2(923, 522);
                O2.Path = "Misc/oxygen";
                O2.LoadContent();
                oxygen.Add(O2);
                for (int i = 0; i < 7; i++)
                {
                    O2.Position.X += 44;
                    oxygen.Add(O2);
                }
                
            }
        }
        public void UnloadContent()
        {
            menu.UnloadContent();
            page.UnloadContent();
            card.UnloadContent();
            foreach (Image O2 in oxygen)
                O2.UnloadContent();
            if (playerChemicals != null)
                playerChemicals.UnloadContent();
            if (enemyChemicals != null)
                enemyChemicals.UnloadContent();
            if (itemManager != null)
                itemManager.UnloadContent();
            foreach (Menu m in clone)
                m.UnloadContent();
            foreach (Image i in infoImage)
                i.UnloadContent();
        }
        public void Update(GameTime gameTime)
        {
            if (!isTransitioning)
                menu.Update(gameTime);
            Transition(gameTime);
        }
        public void Update(GameTime gameTime, ref Player player, ref ChemicalManager enemyChemicalManager)
        {
            if (!isTransitioning)
               menu.Update(gameTime);
            Transition(gameTime);
            playerChemicals = player.ChemicalManager;
            enemyChemicals = enemyChemicalManager;
            itemManager = player.ItemManager;
            foreach (Image i in infoImage)
                i.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Menu m in clone)
                m.Draw(spriteBatch);
            menu.Draw(spriteBatch);
            card.Draw(spriteBatch);
            foreach (Image O2 in oxygen)
                O2.Draw(spriteBatch);
            if (currentMenuID.Contains("Option"))
            {
                page.Draw(spriteBatch);
                spriteBatch.DrawString(page.Font, pageText, page.Position + new Vector2(2, 0), Color.White);
            }
            foreach (Image i in infoImage)
                i.Draw(spriteBatch);
        }
        #endregion

        #region Option Menus
        void optionMoveMenu() //Currently alligned for CHEMICALS name
        {
            menu.Items.Clear();
            foreach (string chemicalName in playerChemicals.chemicalName)
            {
                MenuItem item = new MenuItem();
                item.Image = new Image();
                item.Image.Text = chemicalName.ToUpper();
                string s = (playerChemicals.GetChemical(chemicalName).State).ToString().ToLower();
                item.Image.Text += "(" + s.Substring(0, 1) + ")";

                string h = (playerChemicals.GetChemical(chemicalName).CurrentHealth).ToString() + "/" + (playerChemicals.GetChemical(chemicalName).Health).ToString();

                if (font != null)
                {
                    string space = " ";
                    int spaceNum = (int)((ScreenManager.Instance.Dimensions.X - 730 - font.MeasureString(h).X - font.MeasureString(item.Image.Text).X) / font.MeasureString(space).X);
                    for (int i = 0; i < spaceNum; i++)
                        item.Image.Text += " ";
                    item.Image.Text += h;
                }
                item.Image.TextColor = Color.Black;
                item.Image.FontName = "Fonts/OCRAsmall";
                item.LinkType = "Info";
                item.LinkID = "Content/Load/Menu/InfoMenu.xml";
                menu.Items.Add(item);
            }
        }
        void optionItemMenu()
        {
            menu.Items.Clear();
            foreach (Item i in itemManager.Items)
            {
                MenuItem item = new MenuItem();
                item.Image = new Image();
                string state = i.State.ToLower();
                if (state[0] == 'A' || state[0] == 'a')
                    state = "aq";
                else state = state[0].ToString();
                item.Image.Text = i.Name.ToUpper() + "(" + state + ")";
                item.Image.TextColor = Color.Black;
                item.Image.FontName = "Fonts/OCRAsmall";
                item.LinkType = "Item";
                item.LinkID = "Content/Load/Menu/ItemMenu.xml";
                menu.Items.Add(item);
            }
        }
        #endregion

        #region Description Menus
        void bookMenu()
        {
            foreach (Image image in infoImage)
                image.UnloadContent();
            infoImage.Clear();
            Vector2 dimensions = new Vector2(menu.Image.Position.X + menu.Image.SourceRect.Width * 3 / 4, 45);
            Image i = new Image();

            switch (currentMenuID)
            {
                case "Series":
                    i = new Image();
                    i.FontName = "Fonts/OCRAExt";
                    i.Text = "PLACEHOLDER";
                    i.TextColor = Color.Black;
                    i.Position = new Vector2(dimensions.X - menu.Image.Font.MeasureString(i.Text).X / 2f + 15f, 182);
                    infoImage.Add(i);
                    break;
                case "Chemicals":
                    i = new Image();
                    i.FontName = "Fonts/OCRAExt";
                    i.Text = "PLACEHOLDER";
                    i.TextColor = Color.Black;
                    i.Position = new Vector2(dimensions.X - menu.Image.Font.MeasureString(i.Text).X / 2f + 15f, 182);
                    infoImage.Add(i);
                    break;
                case "Redoxreaction":
                    i = new Image();
                    i.FontName = "Fonts/OCRAExt";
                    i.Text = "PLACEHOLDER";
                    i.TextColor = Color.Black;
                    i.Position = new Vector2(dimensions.X - menu.Image.Font.MeasureString(i.Text).X / 2f + 15f, 182);
                    infoImage.Add(i);
                    break;
                case "Organicreaction":
                    i = new Image();
                    i.FontName = "Fonts/OCRAExt";
                    i.Text = "PLACEHOLDER";
                    i.TextColor = Color.Black;
                    i.Position = new Vector2(dimensions.X - menu.Image.Font.MeasureString(i.Text).X / 2f + 15f, 182);
                    infoImage.Add(i);
                    break;
                case "Journal":
                    i = new Image();
                    i.FontName = "Fonts/OCRAExt";
                    i.Text = "PLACEHOLDER";
                    i.TextColor = Color.Black;
                    i.Position = new Vector2(dimensions.X - menu.Image.Font.MeasureString(i.Text).X / 2f + 15f, 182);
                    infoImage.Add(i);
                    break;
                case "People":
                    i = new Image();
                    i.FontName = "Fonts/OCRAExt";
                    i.Text = "PLACEHOLDER";
                    i.TextColor = Color.Black;
                    i.Position = new Vector2(dimensions.X - menu.Image.Font.MeasureString(i.Text).X / 2f + 15f, 182);
                    infoImage.Add(i);
                    break;
                case "Saving":
                    i = new Image();
                    i.FontName = "Fonts/OCRAExt";
                    i.Text = "PLACEHOLDER";
                    i.TextColor = Color.Black;
                    i.Position = new Vector2(dimensions.X - menu.Image.Font.MeasureString(i.Text).X / 2f + 15f, 182);
                    infoImage.Add(i);
                    break;
                case "Controls":
                    i = new Image();
                    i.FontName = "Fonts/OCRAExt";
                    i.Text = "WASD  = MOVE\n\r\n\rENTER = INTERACT\n\r        MENU\n\r\n\rX     = BACK";
                    i.TextColor = Color.Black;
                    i.Position = new Vector2(dimensions.X - menu.Image.Font.MeasureString(i.Text).X / 2f + 15f, 182);
                    infoImage.Add(i);
                    break;
                case "Settings":
                    i = new Image();
                    i.FontName = "Fonts/OCRAExt";
                    i.Text = "Coming Soon!";
                    i.TextColor = Color.Black;
                    i.Position = new Vector2(dimensions.X - menu.Image.Font.MeasureString(i.Text).X / 2f, 230);
                    infoImage.Add(i);
                    break;
                case "Credits":
                    i = new Image();
                    i.FontName = "Fonts/OCRAExt";
                    i.Text = "Demo Developed\n\rby Jihae Han";
                    i.TextColor = Color.Black;
                    i.Position = new Vector2(dimensions.X - menu.Image.Font.MeasureString(i.Text).X / 2f, 230);
                    infoImage.Add(i);
                    break;
            }
            foreach (Image image in infoImage)
                image.LoadContent();
        }
        void itemInfoMenu()
        {
            foreach (Image image in infoImage)
                image.UnloadContent();
            infoImage.Clear();
            Vector2 dimensions = new Vector2(menu.Image.Position.X + menu.Image.SourceRect.Width / 2, 50);
            itemManager.CurrentItemNumber = prevSelectedItem;
            Item item = itemManager.CurrentItem;

            //1: Item Name
            Image i = new Image();
            i.FontName = "Fonts/OCRAsmall";
            i.Text = selectedItem.ToUpper();
            i.TextColor = Color.Black;
            i.Position = new Vector2(dimensions.X - font.MeasureString(i.Text).X / 2f, dimensions.Y);
            dimensions.Y += 10f;
            infoImage.Add(i);

            //2: Chemical Image
            i = new Image();
            i.Path = "Chemical/Diagram/" + item.Name;
            i.Position = new Vector2(dimensions.X - 188f, dimensions.Y);
            dimensions.Y += 200;
            infoImage.Add(i);

            //3: Text Description
            i = new Image();
            i.Path = item.Description;
            i.Position = new Vector2(menu.Image.Position.X, dimensions.Y);
            infoImage.Add(i);

            foreach (Image image in infoImage)
                image.LoadContent();
        }
        void moveInfoMenu()
        {
            //fill in info image here...
        }
        #endregion

        #region Misc Functions
        void optionMenuPage()
        {
            if (!menu.Items[menu.ItemNumber].Image.IsVisible)
            {
                int invisible = menu.ItemNumber - menu.ItemNumber % 3;
                for (int i = 0; (i < invisible) && (i < menu.Items.Count()); i++)
                    menu.Items[i].Image.IsVisible = false;
                for (int j = invisible; j < (invisible + 3) && j < (menu.Items.Count()); j++)
                    menu.Items[j].Image.IsVisible = true;
                for (int k = invisible + 3; k < menu.Items.Count(); k++)
                    menu.Items[k].Image.IsVisible = false;

                pageText = ((int)(menu.ItemNumber / 3 + 1)).ToString() + "/3";
            }
        }
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

        #region Input Methods
        public void SelectLeft(eButtonState buttonState)
        {
            if (buttonState == eButtonState.DOWN)
            {
                menu.ItemNumber--;
                if (currentMenuID.Contains("BattleMenu"))
                    menu.ItemNumber--;
            }
        }
        public void SelectRight(eButtonState buttonState)
        {
            if (buttonState == eButtonState.DOWN)
            {
                menu.ItemNumber++;
                if (currentMenuID.Contains("BattleMenu"))
                    menu.ItemNumber++;
            }
        }
        public void SelectDown(eButtonState buttonState)
        {
            if (buttonState == eButtonState.DOWN)
                menu.ItemNumber++;
            else if (menu.Type.Contains("Option"))
                optionMenuPage();

        }
        public void SelectUp(eButtonState buttonState)
        {
            if (buttonState == eButtonState.DOWN)
                menu.ItemNumber--;
            else if (menu.Type.Contains("Option"))
                optionMenuPage();

        }
        public void Activate(eButtonState buttonState)
        {
            if (buttonState == eButtonState.DOWN)
            {
                if (menu.Active)
                    menu.Active = false;
                else
                {
                    currentMenuID = "Content/Load/Menu/BattleMenu.xml";
                    menu.ID = currentMenuID;
                    menu.Active = true;
                    prevSelectedItem = 0;
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
                    if (prevMenuID.Contains("BattleMenu"))
                        battleMenuSelectedItem = menu.ItemNumber;
                    prevSelectedItem = menu.ItemNumber;
                    currentMenuID = menu.Items[menu.ItemNumber].LinkID;

                    if (menu.Type.Contains("Option"))
                    {
                        string str = menu.Items[menu.ItemNumber].Image.Text;
                        if (str != String.Empty)
                        {
                            if (str.Contains("("))
                                str = str.Substring(0, str.IndexOf('(')).ToLower();

                            selectedItem = str[0].ToString().ToUpper() + str.Substring(1);

                        }
                    }
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
                else if (!prevMenuID.Contains("Battle"))
                {
                    currentMenuID = prevMenuID = "Content/Load/Menu/BattleMenu.xml";
                    menu.ID = currentMenuID;
                    menu.Active = true;
                    clone.Clear();
                }
                else if (IsActive)
                    Activate(buttonState);

            }
        }
        #endregion
    }
}
