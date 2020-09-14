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
        List<string> moveList;
        SpriteFont font;
        Image page;
        Image cardDown, cardUp;
        Image O2Empty, O2Filled, O2Label;
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
            cardDown = cardUp = new Image();
            O2Empty = new Image();
            O2Filled = new Image();
            O2Label = new Image();
            infoImage = new List<Image>();
            moveList = new List<string>();
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
            if (!currentMenuID.Contains("/Flee"))
            {
                if (currentMenuID != String.Empty && !currentMenuID.Contains("Battle"))
                    clone.Add(menu);
                else
                    clone.Clear();
            }

            XmlManager<Menu> XmlMenuManager = new XmlManager<Menu>();

            if (menu.ID.Contains(".xml") || menu.ID == String.Empty)
            {
                menu.UnloadContent();
                menu = XmlMenuManager.Load(menu.ID);
            }
            /*if (currentMenuID.Contains("/Move"))
                MoveMenu();
            else*/ if (currentMenuID.Contains("OptionMove"))
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
                page.Position = new Vector2(307, ScreenManager.Instance.Dimensions.Y - 21);
                page.LoadContent();
                cardDown.FontName = cardUp.FontName = "Fonts/OCRAsmall";
                cardDown.Position = cardUp.Position = new Vector2(928.5f, 636f);
                cardUp.Path = "Misc/card_up";
                cardUp.IsVisible = false;
                cardUp.LoadContent();
                cardDown.Path = "Misc/card_down";
                cardDown.IsVisible = true;
                cardDown.LoadContent();
                O2Empty.Path = "Misc/oxygen_empty";
                O2Empty.LoadContent();   
                O2Filled.Path = "Misc/oxygen_filled";
                O2Filled.LoadContent();
                O2Label.Path = "Misc/oxygen_label";
                O2Label.FontName = "Fonts/OCRAsmall";
                O2Label.Position = new Vector2(1151, 586);
                O2Label.LoadContent();
            }
        }
        public void UnloadContent()
        {
            menu.UnloadContent();
            page.UnloadContent();
            cardDown.UnloadContent();
            cardUp.UnloadContent();
            O2Empty.UnloadContent();
            O2Filled.UnloadContent();
            O2Label.UnloadContent();
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
            cardDown.Draw(spriteBatch);
            cardUp.Draw(spriteBatch);
            drawOxygen(spriteBatch);
            if (menu.Type.Contains("Option"))
            {
                page.Draw(spriteBatch);
                spriteBatch.DrawString(page.Font, pageText, page.Position + new Vector2(2, 0), Color.White);
            }
            foreach (Image i in infoImage)
                i.Draw(spriteBatch);
        }
        #endregion

        #region Battle Methods
        void moveInfoMenu()
        {
        }
        void MoveMenu()
        {
            menu.Items.Clear();
            Chemical chemical = playerChemicals.GetBattleChemical(selectedItem);

            MenuItem item = new MenuItem();
            item.Image = new Image();
            //item.Image.Text = 
            item.Image.TextColor = Color.Black;
            item.Image.FontName = "Fonts/OCRAsmall";
            item.LinkType = "Move";
            item.LinkID = "Content/Load/Menu/OptionMoveMenu.xml";
            menu.Items.Add(item);
        }
        void generateMoveList(Chemical chemical)
        {
            moveList.Add("Formation");
            if (!chemical.Name.Contains("Bromomethane"))
                moveList.Add("Combustion");
            else
                moveList.Add("Extinguisher");
            if (chemical.Isomers > 0)
                moveList.Add("Branching");
            if (chemical.Series == Series.Alkane)
                moveList.Add("Free Radical Sub");
            if (chemical.Series == Series.Alkene)
                moveList.Add("Hydrogenation");
 
        }

        #endregion

        #region Option Menus
        void optionMoveMenu() //Currently alligned for CHEMICALS name
        {
            menu.Alignment.X = 340;
            menu.Items.Clear();
            foreach (string battleChemicalName in playerChemicals.battleChemicalName)
            {
                Chemical chemical = playerChemicals.GetBattleChemical(battleChemicalName);
                //if current move for battleChemical is not set...
                if (chemical.BattleMove == String.Empty)
                {
                    MenuItem item = new MenuItem();
                    item.Image = new Image();
                    item.Image.Text = battleChemicalName.ToUpper();
                    string s = (playerChemicals.GetChemical(battleChemicalName).State).ToString().ToLower();
                    item.Image.Text += "(" + s.Substring(0, 1) + ")";

                    string h = (playerChemicals.GetChemical(battleChemicalName).CurrentHealth).ToString() + "/" + (playerChemicals.GetChemical(battleChemicalName).Health).ToString();

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
                    item.LinkType = "Move";
                    item.LinkID = "Content/Load/Menu/MoveMenu.xml";
                    
                    menu.Items.Add(item);
                }
            }
        }
        void optionItemMenu()
        {
            menu.Alignment.X = 340;
            menu.Image.Position.Y = 720;
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
        private void drawOxygen(SpriteBatch spriteBatch)
        {
            O2Label.Draw(spriteBatch);
            string O2 = currentOxygen.ToString() + "/" + totalOxygen.ToString();
            Vector2 position = O2Label.Position;
            position.X -= O2Label.Font.MeasureString(O2 + " ").X;
            spriteBatch.DrawString(O2Label.Font, O2, position, Color.SaddleBrown);

            O2Empty.Position = O2Filled.Position = new Vector2(923, 522);
            for (int i = 0; i < 8; i++)
            {
                O2Empty.Draw(spriteBatch);
                O2Empty.Position.X += 44;
            }
            for (int i = 0; i < (int)totalOxygen / 2; i++)
            {
                O2Filled.Draw(spriteBatch);
                O2Filled.Position.X += 44;
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
            if (buttonState == eButtonState.DOWN && !isTransitioning && !currentMenuID.Contains("BattleMenu"))
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
