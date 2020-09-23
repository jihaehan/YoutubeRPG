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
        ItemManager itemManager;
        CharacterManager characterManager;
        List<string> reactionHistory;
        bool isTransitioning;

        string prevMenuID;
        string currentMenuID;
        string selectedItem;
        int prevSelectedItem, gameplayMenuSelectedItem;
        List<Image> infoImage;
        SpriteFont font;
        Image page;
        string pageText;
        bool isIntroduction;
        string id;

        public MenuManager()
        {
            isIntroduction = false;
            id = String.Empty;
            isIntroduction = false; 
            prevSelectedItem = gameplayMenuSelectedItem = 0;
            prevMenuID = currentMenuID = selectedItem = String.Empty;
            pageText = "1/3";
            page = new Image();
            infoImage = new List<Image>();
            reactionHistory = new List<string>();
            clone = new List<Menu>();
            menu = new Menu();
            menu.OnMenuChanged += menu_OnMenuChange;    //OnMenuChanged = event;
                                                        //Adds the method, "menu_OnMenuChanged" into event OnMenuChanged
        }
        public void SetIntroduction()
        {
            isIntroduction = true;
        }
        public string ID()
        {
            return id;
        }
        public bool IsActive
        {
            get { return menu.Active; }
        }
        #region Event
        public void menu_OnMenuChange(object sender, EventArgs e)
        {
            if (!currentMenuID.Contains("GameplayMenu") && currentMenuID != String.Empty) 
                clone.Add(menu);
            else
                clone.Clear();

            XmlManager<Menu> XmlMenuManager = new XmlManager<Menu>();

            if (menu.ID.Contains(".xml") || menu.ID == String.Empty)
            {
                menu.UnloadContent();
                menu = XmlMenuManager.Load(menu.ID);
            }
            if (currentMenuID.Contains("OptionInfo"))
                optionInfoMenu();
            else if (currentMenuID.Contains("OptionItem"))
                optionItemMenu();
            else if (currentMenuID.Contains("OptionPlan"))
                optionPlanMenu();

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
            if (!currentMenuID.Contains("Info") && !currentMenuID.Contains("Option"))
            {
                foreach (Image image in infoImage)
                    image.UnloadContent();
                infoImage.Clear();
            }
            if (currentMenuID.Contains("GameplayMenu"))
            {
                menu.Active = true;
                menu.ItemNumber = gameplayMenuSelectedItem;
                font = menu.Image.Font;
                //prevSelectedItem = 0;
            }
            else if (menu.Type.Contains("Option"))
            {
                menu.ItemNumber = prevSelectedItem; 
                optionMenuPage();
            }
            else if (menu.Type == "ChemicalInfo")
            {
                chemicalInfoMenu();
            }
            else if (menu.Type == "ItemInfo")
            {
                itemInfoMenu();
            }
            else if (menu.Type == "PlanInfo")
            {
                planInfoMenu();
            }
            else if (menu.Type == "Book")
            {
                menu.ItemNumber = prevSelectedItem;
                bookMenu();
            }
            else
                menu.ItemNumber = prevSelectedItem;
        }
        #endregion

        #region Main Methods
        public void LoadContent(string menuPath)
        {
            if (menuPath != String.Empty)
            {
                menu.ID = menuPath;
                prevMenuID = currentMenuID = menuPath;
                page.FontName = "Fonts/OCRAExt";
                page.Path = "Misc/page";
                page.Position = new Vector2(ScreenManager.Instance.Dimensions.X - 47, ScreenManager.Instance.Dimensions.Y - 23);
                page.LoadContent();
            }
        }
        public void UnloadContent()
        {
            menu.UnloadContent();
            page.UnloadContent();
            if (chemicalManager != null)
                chemicalManager.UnloadContent();
            if (itemManager != null)
                itemManager.UnloadContent();
            if (characterManager != null)
                characterManager.UnloadContent();
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
        public void Update(GameTime gameTime, ref Player player)
        {
            if (!isTransitioning)
               menu.Update(gameTime);
            Transition(gameTime);
            chemicalManager = player.ChemicalManager;
            itemManager = player.ItemManager;
            characterManager = player.CharacterManager;
            foreach (Image i in infoImage)
                i.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Menu m in clone)
                m.Draw(spriteBatch);
            menu.Draw(spriteBatch);
            if (menu.Type.Contains("Info") || currentMenuID.Contains("Option"))
            {
                page.Draw(spriteBatch);
                spriteBatch.DrawString(page.Font, pageText, page.Position + new Vector2(2,0), Color.White);
            }
            foreach (Image i in infoImage)
                i.Draw(spriteBatch);
        }
        #endregion

        #region Description Menus
        void bookMenu()
        {
            foreach (Image image in infoImage)
                image.UnloadContent();
            infoImage.Clear();
            Vector2 dimensions = new Vector2(menu.Image.Position.X + menu.Image.SourceRect.Width *3/4, 45);
            Image i = new Image();

            switch (currentMenuID)
            {
                case "Series":
                    i = new Image();
                    i.Path = "Book/series";
                    i.Position = new Vector2(1280 - 376, 0);
                    infoImage.Add(i); 
                    break;
                case "Chemicals":
                    i = new Image();
                    i.Path = "Book/chemical";
                    i.Position = new Vector2(1280 - 376, 0);
                    infoImage.Add(i);
                    break;
                case "Thermodynamics":
                    i = new Image();
                    i.Path = "Book/thermo_reactions";
                    i.Position = new Vector2(1280 - 376, 0);
                    infoImage.Add(i);
                    break;
                case "Organicreaction":
                    i = new Image();
                    i.Path = "Book/organic_reactions";
                    i.Position = new Vector2(1280 - 376, 0);
                    infoImage.Add(i);
                    break;
                case "Journal":
                    i = new Image();
                    i.Path = "Book/temp_journal";
                    i.Position = new Vector2(1280 - 376, 0);
                    infoImage.Add(i);
                    break;
                case "People":
                    i = new Image();
                    i.Path = "Book/people";
                    i.Position = new Vector2(1280 - 376, 0);
                    infoImage.Add(i);
                    break;
                case "Saving":
                    i = new Image();
                    i.Path = "Book/coming_soon";
                    i.Position = new Vector2(1280 - 376, 0);
                    infoImage.Add(i);
                    break;
                case "Controls":
                    i = new Image();
                    i.Path = "Book/controls";
                    i.Position = new Vector2(1280 - 376, 0);
                    infoImage.Add(i);
                    break;
                case "Settings":
                    i = new Image();
                    i.Path = "Book/coming_soon";
                    i.Position = new Vector2(1280 - 376, 0);
                    infoImage.Add(i);
                    break;
                case "Credits":
                    i = new Image();
                    i.Path = "Book/credits";
                    i.Position = new Vector2(1280 - 376, 0);
                    infoImage.Add(i);
                    break;
            }
            foreach (Image image in infoImage)
                image.LoadContent();
        }
        void planInfoMenu()
        {
            foreach (Image image in infoImage)
                image.UnloadContent();
            infoImage.Clear();
            Vector2 dimensions = new Vector2(menu.Image.Position.X + menu.Image.SourceRect.Width / 2, 50);
            Character character = characterManager.GetCharacter(characterManager.characterName[prevSelectedItem]);

            //1: Title, 'PLAN'
            Image i = new Image();
            i.FontName = "Fonts/OCRAsmall";
            if (character.Name == String.Empty)
                i.Text = character.QuestName;
            else
                i.Text = character.Name;
            i.TextColor = Color.Black;
            i.Position = new Vector2(dimensions.X - font.MeasureString(i.Text).X / 2f, dimensions.Y);
            dimensions.Y += 10f;
            infoImage.Add(i);

            //2: Map Outline
            dimensions = new Vector2(menu.Image.Position.X + 63f, 100f);
            i = new Image();
            i.Path = "Misc/minimap_outline";
            i.Position = dimensions;
            infoImage.Add(i);

            //3: Minimap

            //4: Description
            dimensions = new Vector2(menu.Image.Position.X, 373f);
            i = new Image();
            i.Path = character.QuestDescription;
            i.Position = dimensions;
            infoImage.Add(i);

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
        void chemicalInfoMenu()
        {
            foreach (Image image in infoImage)
                image.UnloadContent();
            infoImage.Clear();
            Vector2 dimensions = new Vector2(menu.Image.Position.X + menu.Image.SourceRect.Width/2, 50);
            Chemical chemical = chemicalManager.GetChemical(selectedItem);

            //1: Chemical Name
            Image i = new Image();
            i.FontName = "Fonts/OCRAsmall";
            i.Text = selectedItem.ToUpper();
            i.TextColor = Color.Black;
            i.Position = new Vector2(dimensions.X - font.MeasureString(i.Text).X/2f, dimensions.Y);
            dimensions.Y += 50f;
            infoImage.Add(i);

            //2: Chemical Image
            i = new Image();
            i.Path = "Chemical/" + chemical.Series.ToString() + "/" + chemical.Name;
            i.Effects = "SpriteSheetEffect";
            i.SpriteSheetEffect = new SpriteSheetEffect();
            i.SpriteSheetEffect.AmountOfFrames = new Vector2(2, 2);
            i.SpriteSheetEffect.SwitchFrame = 500;
            i.Position = new Vector2(dimensions.X - 64, dimensions.Y);
            i.IsActive = true;
            dimensions.Y += 100f;
            infoImage.Add(i);

            //3: Chemical Formula
            i = new Image();
            i.Path = "Chemical/Diagram/" + chemical.Name;
            i.Position = new Vector2(dimensions.X - 187.5f, dimensions.Y);
            dimensions.Y = menu.Image.Position.Y + menu.Image.SourceRect.Height - 120f;
            infoImage.Add(i);

            //4: Reaction History Label
            i = new Image();
            i.FontName = "Fonts/OCRAExt";
            if (chemical.Series == Series.Alkene)
                i.Text = "Addition Reactions:";      
            else 
                i.Text = "Reaction History:";      
            i.TextColor = Color.Black;
            i.Position = new Vector2(dimensions.X - menu.Image.Font.MeasureString(i.Text).X / 2f, dimensions.Y);
            dimensions.Y += menu.Image.Font.MeasureString(i.Text).Y + 3;
            infoImage.Add(i);

            //5: Reaction History/Potential Unlock
            i = new Image();
            i.FontName = "Fonts/OCRAExt";
            i.Text = "???";
            //TEST: Unlock breeding route
            //if (!reactionHistory.Contains("Alkane")) reactionHistory.Add("Alkane");
            i.Text = reactionHistoryFormula(chemical);
            i.TextColor = Color.Black;
            i.Position = new Vector2(dimensions.X - menu.Image.Font.MeasureString(i.Text).X / 2f, dimensions.Y);
            infoImage.Add(i);

            //6: Page Number
            i = new Image();
            i.FontName = "Fonts/OCRAExt";
            i.Text = "1/2";
            i.Path = "Misc/page";
            i.Position = new Vector2(ScreenManager.Instance.Dimensions.X - 47, menu.Image.SourceRect.Height - 23);
            page.LoadContent();
            infoImage.Add(i);

            foreach (Image image in infoImage)
                image.LoadContent();
        }
        void propertyInfoMenu()
        {
            foreach (Image image in infoImage)
                image.UnloadContent();
            infoImage.Clear();
            Vector2 dimensions = new Vector2(menu.Image.Position.X + menu.Image.SourceRect.Width / 2, 50);
            Chemical chemical = chemicalManager.GetChemical(selectedItem);

            //1: Chemical Name
            Image i = new Image();
            i.FontName = "Fonts/OCRAsmall";
            i.Text = selectedItem.ToUpper();
            i.TextColor = Color.Black;
            i.Position = new Vector2(dimensions.X - font.MeasureString(i.Text).X / 2f, dimensions.Y);
            dimensions.Y += 55f;
            dimensions.X = menu.Image.Position.X + 38f;
            infoImage.Add(i);

            //2: Chemical Properties Labels
            i = new Image();
            i.Text = "Series: " + chemical.Series.ToString();
            i.Text += "\n\r\n\r";

            i.Text += "Bond Enthalpy:\n\r\n\r";
            i.Text += "Formation Enthalpy:\n\r\n\r";
            i.Text += "Combustion Enthalpy:\n\r\n\r\n\r";
            
            i.Text += "Boiling Pt:\n\r";
            i.Text += "Atomic Mass:\n\r";
            i.Text += "Experience:\n\r\n\r";
            i.Text += "INFO: ";

            switch(chemical.Series.ToString())
            {
                case "Alkane":
                    i.Text += "used as fuels.\n\rrelatively unreactive.";
                    break;
                case "Alkene":
                    i.Text += "double bonds.\n\rused in polymers.\n\rsmoky combustion.";
                    break;
                case "Alcohol":
                    i.Text += "hydroxyl group.\n\r";
                    if (chemical.Name == "Ethanol")
                        i.Text += "used in drinks.";
                    break;
                case "Halogenoalkane":
                    if (chemical.Name == "Bromomethane")
                        i.Text += "not flammable.\n\r*fire extinguisher*";
                    else
                        i.Text += "flammable.\n\rfairly reactive.\n\r";
                    break;
            }

            i.FontName = "Fonts/OCRAExt";
            i.TextColor = Color.Black;
            i.Position = new Vector2(dimensions.X, dimensions.Y);
            dimensions.Y += menu.Image.Font.MeasureString(" ").Y * 3;
            infoImage.Add(i);

            //3: Chemical Thermodynamic Properties (orange)
            i = new Image();
            i.Text = "  " + chemical.CurrentHealth.ToString() + "/" + chemical.Health.ToString() + " kJ/mol" + "\n\r\n\r";
            i.Text += "  " + chemical.BaseDamage.ToString() + " kJ/mol\n\r\n\r";
            i.Text += "  " + chemical.MaxDamage.ToString() + " kJ/mol\n\r\n\r";
            i.FontName = "Fonts/OCRAExt";
            i.TextColor = Color.DarkOrange;
            i.Position = new Vector2(dimensions.X, dimensions.Y);
            dimensions.Y += menu.Image.Font.MeasureString(" ").Y * 6;
            infoImage.Add(i);

            //4: Chemical Brief Properties (blue)
            i = new Image();
            i.Text = "              " + chemical.BoilingPoint + " K\n\r";
            i.Text += "              " + chemical.Mass + "\n\r";
            i.Text += "              ";
            i.Text += chemical.Experience;
            i.Text += "\n\r\n\r";
            i.FontName = "Fonts/OCRAExt";
            i.TextColor = Color.CornflowerBlue;
            i.Position = new Vector2(dimensions.X, dimensions.Y);
            infoImage.Add(i);

            //5: Page Number
            i = new Image();
            i.FontName = "Fonts/OCRAExt";
            i.Text = "2/2";
            i.Path = "Misc/page";
            i.Position = new Vector2(ScreenManager.Instance.Dimensions.X - 47, menu.Image.SourceRect.Height - 23);
            page.LoadContent();
            infoImage.Add(i);

            foreach (Image image in infoImage)
                image.LoadContent();
        }
        #endregion

        #region Option Menus
        void optionPlanMenu()
        {
            menu.Items.Clear();
            foreach (string characterName in characterManager.characterName)
            {
                MenuItem item = new MenuItem();
                Character character = characterManager.GetCharacter(characterName);

                item.Image = new Image();
                item.Image.Text = character.QuestName;
                item.Image.TextColor = Color.Black;
                item.Image.FontName = "Fonts/OCRAsmall";
                item.LinkType = "Plan";
                item.LinkID = "Content/Load/Menu/PlanMenu.xml";
                menu.Items.Add(item);
            }
        }
        void optionInfoMenu()
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

        #region Misc Functions
        string reactionHistoryFormula(Chemical chemical)
        {
            string s = String.Empty;
            switch(chemical.Series)
            {
                case Series.Alkane:
                    if (reactionHistory.Contains("Alkane"))
                    {
                        if (chemical.Name == "Methane")
                            s = "CO2 + 4H2 = CH4";
                        else
                        {
                            s = chemical.Name.Replace("ane", "ene").ToLower() + " + H2Ni + HEAT" + "\n\r" + "= " + chemical.Name.ToLower();
                        }
                    }
                    else
                    {
                        string element = chemical.GetElement(Element.C).ToString();
                        if (element == "0" || element == "1")
                            element = String.Empty;
                        s += "??? + ??? = " + "C" + element;
                        element = chemical.GetElement(Element.H).ToString();
                        s += "H" + element;
                    }
                    break;
                case Series.Alkene:
                    int count = 0;
                    string[] str = { "Alkene_Hydration", "Alkene_Halogenation", "Alkene_Hydrohalogenation", "Alkene_Hydrogenation" };
                    foreach (string reaction in str)
                    {
                        if (reactionHistory.Contains(reaction))
                        {
                            s += reaction.Replace("Alkene_", "") + ", ";
                            if (count == 2)
                                s += "\n\r";
                        }
                        else
                            s += "???, ";
                        if (count == 1)
                            s += "\n\r";
                        count++;
                    }
                    break;
                case Series.Alcohol:
                    if (reactionHistory.Contains("Alcohol"))
                    {
                        s = chemical.Name.Replace("anol", "ene").ToLower() + " + NaOH + HEAT" + "\n\r" + "= " + chemical.Name.ToLower();
                    }
                    else
                    {
                        string element = chemical.GetElement(Element.C).ToString();
                        if (element == "0" || element == "1")
                            element = String.Empty;
                        s += "??? + ??? = " + "C" + element;
                        element = (chemical.GetElement(Element.H)-1).ToString();
                        s += "H" + element + "OH";
                    }
                    break;
                case Series.Halogenoalkane:
                    s = String.Empty;
                    if (reactionHistory.Contains("Halogenoalkane_Halogenation"))
                    {
                        string a = chemical.Name.Replace("ane", "ene").ToLower();
                        s = a + " + HBr + HEAT" + "\n\r" + "= " + chemical.Name.ToLower();
                    }
                    // CAUTION: halogenoalkane_Hydrohalogenation is missing, but not a priority to sort out here
                    else
                    {
                        string element = chemical.GetElement(Element.C).ToString();
                        if (element == "0" || element == "1")
                            element = String.Empty;
                        s += "??? + ??? = " + "C" + element;
                        element = chemical.GetElement(Element.H).ToString();
                        s += "H" + element + "Br";
                    }
                    break;
            }
            return s;
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
            if (/*menu.Axis == "X" && */buttonState == eButtonState.DOWN)
            {
                menu.ItemNumber--;
                if (currentMenuID.Contains("GameplayMenu"))
                    menu.ItemNumber--;
                else if (menu.Type == "ChemicalInfo")
                    chemicalInfoMenu();
            }
        }
        public void SelectRight(eButtonState buttonState)
        {
            if (/*menu.Axis == "X" && */buttonState == eButtonState.DOWN)
            {
                menu.ItemNumber++;
                if (currentMenuID.Contains("GameplayMenu"))
                    menu.ItemNumber++;
                else if (menu.Type == "ChemicalInfo")
                    propertyInfoMenu();
            }
        }
        public void SelectDown(eButtonState buttonState)
        {
            if (/*menu.Axis == "Y" && */buttonState == eButtonState.DOWN)
                menu.ItemNumber++;
            else if (menu.Type.Contains("Option"))
                optionMenuPage();
            else if (menu.Type == "ChemicalInfo")
                propertyInfoMenu();
        }
        public void SelectUp(eButtonState buttonState)
        {
            if (/*menu.Axis == "Y" && */buttonState == eButtonState.DOWN)
                menu.ItemNumber--;
            else if (menu.Type.Contains("Option"))
                optionMenuPage();
            else if (menu.Type == "ChemicalInfo")
                chemicalInfoMenu();
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
        
        #endregion
    }
}
