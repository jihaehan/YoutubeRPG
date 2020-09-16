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
        Player player;
        Character enemy;
        ItemManager itemManager;
        SPXManager spxManager;
        bool isTransitioning;
        bool isPlayerTurn;
        bool isDescription;

        string prevMenuID;
        string currentMenuID;
        string selectedItem, selectedChemical;
        int prevSelectedItem, battleMenuSelectedItem;
        int turnCount;
        List<Image> infoImage; //for visual text distinct from menuButtons
        List<SPX> spxImage;    //for special effects distinct from menuButtons

        List<string> moveList; //for individual chemicals
        List<string> environmentEffects; //for AOE effects for all chemicals
        SpriteFont font;
        Image page;
        Image cardDown, cardUp;
        Image O2Empty, O2Filled, O2Label;
        float totalOxygen;
        float currentOxygen;
        string pageText;

        public BattleManager()
        {
            player = new Player();
            enemy = new Character();
            totalOxygen = currentOxygen = 2;
            prevSelectedItem = battleMenuSelectedItem = 0;
            turnCount = 1; 
            prevMenuID = currentMenuID = selectedItem = String.Empty;
            pageText = "1/3";
            isPlayerTurn = true;
            isDescription = false;
            page = new Image();
            cardDown = new Image();
            cardUp = new Image();
            O2Empty = new Image();
            O2Filled = new Image();
            O2Label = new Image();
            spxManager = new SPXManager();
            spxImage = new List<SPX>();
            infoImage = new List<Image>();
            moveList = environmentEffects = new List<string>();
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
            if (!currentMenuID.Contains("/Flee") && !menu.Type.Contains("Move"))
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
            if (currentMenuID.Contains("/Move"))
                moveMenu();
            else if (currentMenuID.Contains("OptionMove"))
                optionMoveMenu();
            else if (currentMenuID.Contains("OptionItem"))
                optionItemMenu();
            else if (currentMenuID.Contains("Description"))
                descriptionMenu();
            else if (currentMenuID.Contains("Battling"))
                battlingMenu();
            
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
            if (!currentMenuID.Contains("Description"))
            {
                infoImageClear();
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
                itemInfoMenu();
            else if (menu.Type == "Book")
            {
                menu.ItemNumber = prevSelectedItem;
                bookMenu();
            }
            else if (menu.Type == "Move")
                optionMenuPage();
            else
                menu.ItemNumber = prevSelectedItem;
        }

        #region Main Methods
        public void LoadContent(string menuPath)
        {
            initializeParties();
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
                cardUp.TextColor = Color.SaddleBrown;
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
            //First save player content into another file before unloading...
            //and then redirect screen to level up screen depending on EXP gains
            player.UnloadContent();
            enemy.UnloadContent();
            menu.UnloadContent();
            page.UnloadContent();
            cardDown.UnloadContent();
            cardUp.UnloadContent();
            O2Empty.UnloadContent();
            O2Filled.UnloadContent();
            O2Label.UnloadContent();
            if (itemManager != null)
                itemManager.UnloadContent();
            foreach (Menu m in clone)
                m.UnloadContent();
            foreach (SPX spx in spxImage)
                spx.UnloadContent();
            foreach (Image i in infoImage)
                i.UnloadContent();
        }
        public void Update(GameTime gameTime)
        {
            player.BattleUpdate(gameTime);
            enemy.BattleUpdate(gameTime);
            if (!isTransitioning)
               menu.Update(gameTime);
            Transition(gameTime);
            itemManager = player.ItemManager;
            foreach (SPX spx in spxImage)
                spx.Update(gameTime);
            foreach (Image i in infoImage)
                i.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            enemy.BattleDraw(spriteBatch);
            player.BattleDraw(spriteBatch);
            foreach (SPX spx in spxImage)
                spx.Draw(spriteBatch);
            foreach (Menu m in clone)
                m.Draw(spriteBatch);
            menu.Draw(spriteBatch);
            cardDown.Draw(spriteBatch);
            cardUp.Draw(spriteBatch);
            drawOxygen(spriteBatch);
            if (menu.Type.Contains("Option") || menu.Type == "Move")
            {
                page.Draw(spriteBatch);
                spriteBatch.DrawString(page.Font, pageText, page.Position + new Vector2(2, 0), Color.White);
            }
            foreach (Image i in infoImage)
                i.Draw(spriteBatch);
        }
        #endregion

        #region Battle Helper
        void searchBackpack(string infoText, string reactant)
        {
            bool found = false;
            for (int j = 0; j < itemManager.Items.Count; j++)
            {
                if (itemManager.Items[j].Name == reactant && !found)
                {
                    infoImage = scrollingDescription(infoText);
                    found = true;
                    itemManager.Items.RemoveAt(j);
                }
            }
            if (!found)
            {
                infoImage = scrollingDescription(selectedChemical + " does not have " + reactant);
                player.ChemicalManager.GetBattleChemical(selectedChemical).BattleMove = String.Empty;
            }
        }
        string moveReactants(string move)
        {
            string req = String.Empty;
            switch (move)
            {
                case "Free Radical Sub":     //alkane to halogenoalkane
                    req = "Bromine";
                    //requires +UV Light
                    break;
                case "Addition Polymeriz":
                    req = "Nickeldihydride"; //alkene to alkane
                    //requires +HEAT
                    break;
                case "Oxidation":
                    req = "Chromate";        //alcohol to alkanal
                    //requires +HEAT
                    break;
                case "SN2 Nucleophil Sub":   //halogenoalkane to alcohol
                    req = "Sodiumhydroxide";
                    //requires +HEAT
                    break;
            }
            //check itemManager for items
            return req;
        }
        void generateMoveList(Chemical chemical)
        {
            moveList.Clear();
            moveList.Add("Formation");
            if (!chemical.Name.Contains("Bromomethane"))
                moveList.Add("Combustion");
            else
                moveList.Add("Extinguisher");
            if (chemical.Isomers > 0)
                moveList.Add("Branching");
            switch (chemical.Series)
            {
                case Series.Alkane:
                    moveList.Add("Free Radical Sub");
                    break;
                case Series.Alkene:
                    moveList.Add("Addition Polymeriz");
                    break;
                case Series.Alcohol:
                    moveList.Add("Oxidation");
                    break;
                case Series.Halogenoalkane:
                    moveList.Add("SN2 Nucleophil Sub");
                    break;
            }
        }
        public void SetChemicalMove(string name, string moveType)
        {
            player.ChemicalManager.GetBattleChemical(name).BattleMove = moveType;
        }
        #endregion

        #region Battle Methods
        string battleCard()
        {
            Random rnd = new Random();
            List<string> notInParty = new List<string>();
            
            foreach (string chemicalName in player.ChemicalManager.chemicalName)
                if (!player.ChemicalManager.GetChemical(chemicalName).InBattle)
                    notInParty.Add(chemicalName);

            if (notInParty.Count > 0)
            {
                int randomIndex = rnd.Next(notInParty.Count);
                player.ChemicalManager.GetChemical(notInParty[randomIndex]).InBattle = true;

                cardUp.IsVisible = true;
                cardUp.Position += Vector2.Zero;
                cardUp.Text = notInParty[randomIndex] + "(" + player.ChemicalManager.GetChemical(notInParty[randomIndex]).State.ToString().ToLower()[0] + ")";
                cardUp.LoadContent();
                return notInParty[randomIndex];
            }
            else
            {
                cardUp.IsVisible = false;
                cardUp.Text = String.Empty;
                cardUp.LoadContent();
                return String.Empty;
            }
        }
        void battlingMenu()
        {
            isDescription = true;

            //infoImage
            infoImageClear();
            Image i = new Image();
            i.FontName = "Fonts/OCRAsmall";
            i.TextColor = Color.SaddleBrown;
            Chemical chemical = player.ChemicalManager.GetBattleChemical(selectedChemical);
            string s = String.Empty;

            //Manage enemyTurn
            //enemyTurn();
            //display enemy actions in infoImage

            EndPlayerTurn();
            //clear special effects
            foreach (SPX spx in spxImage)
                spx.FadeOut = true;
            //draw card and add new chemical to team
            battleCard();
        }
        void enemyTurn() //NPC logic
        {
            foreach (string battleChemicalName in enemy.ChemicalManager.battleChemicalName)
            {
                Chemical chemical = enemy.ChemicalManager.GetBattleChemical(battleChemicalName);
                generateMoveList(chemical);
                //if (moveList.Contains())
            }
        }
        public void EndPlayerTurn()
        {
            //refresh moves for party members
            foreach (string name in player.ChemicalManager.battleChemicalName)
            {
                player.ChemicalManager.GetBattleChemical(name).BattleMove = String.Empty;
            }
            turnCount++;
            totalOxygen = turnCount * 2;
            currentOxygen = turnCount * 2;
        }
        #endregion

        #region Battle UI
        void descriptionMenu()  //Describes Moves of Specific Chemical
        {
            //Button that leads back to main menu
            #region setup
            menu.Items.Clear();
            MenuItem item = new MenuItem();
            item.Image = new Image();
            item.Image.Text = "."; 
            item.LinkType = "Move";

            //if there are no chemicals left with moves
            int playableChemicals = 0;
            foreach (string battleChemicalName in player.ChemicalManager.battleChemicalName)
                if (player.ChemicalManager.GetBattleChemical(battleChemicalName).BattleMove == String.Empty)
                    playableChemicals++;
            if (playableChemicals > 0)
                item.LinkID = "Content/Load/Menu/OptionMoveMenu.xml";
            else //switch to battling Menu
                item.LinkID = "Content/Load/Menu/BattlingMenu.xml";
            menu.Items.Add(item);

            //InfoImage
            infoImageClear();
            Image i = new Image();
            i.FontName = "Fonts/OCRAsmall";
            i.TextColor = Color.SaddleBrown;
            Chemical chemical = player.ChemicalManager.GetBattleChemical(selectedChemical);
            string s = String.Empty;

            //If using items from backpack
            string reactant = moveReactants(selectedItem);
            #endregion
            //depending on which move has been selected
            switch (selectedItem)
            {
                case "Formation":
                    s = selectedChemical + " releases an Enthalpy of Formation of " + chemical.BaseDamage.ToString() + " kJ/mol";
                    infoImage = scrollingDescription(s);
                    //Add special effects here!
                    break;
                case "Combustion":
                    chemical.SetOxygen(currentOxygen);
                    chemical.Combustion();
                    if (chemical.GetProduct("carbondioxide") > 0)
                    {
                        string CO2 = "carbondioxide";
                        s = chemical.ChemicalFormula + "(" + chemical.State.ToString().ToLower()[0] + ")" + " + " + chemical.CalculateOxygen(CO2) + "O2(g) = " + chemical.GetProduct(CO2).ToString() + "CO2(g) + " + chemical.GetProduct("water").ToString() + "H2O(l)";
                        infoImage = scrollingDescription(s);
                        i.Text = "\n\rCOMP COMBUST: ";
                        i.Text += chemical.Damage.ToString() + "kJ/mol";
                        i.Position = infoImage[infoImage.Count - 1].Position + new Vector2(0, 10f);
                        infoImage.Add(i);
                        //add special effects
                        spxCombustion("CO2", chemical.Level);
                        //add damage
                    }
                    else if (chemical.GetProduct("carbonmonoxide") > 0)
                    {
                        string CO = "carbonmonoxide";
                        s = chemical.ChemicalFormula + "(" + chemical.State.ToString().ToLower()[0] + ")" + " + " + chemical.CalculateOxygen(CO) + "O2(g) = " + chemical.GetProduct(CO).ToString() + "CO(g) + " + chemical.GetProduct("water").ToString() + "H2O(l)";
                        infoImage = scrollingDescription(s);
                        i.Text = "\n\rINCOMP COMB: ";
                        i.Text += chemical.Damage.ToString() + "kJ/mol";
                        i.Position = infoImage[infoImage.Count - 1].Position + new Vector2(0, 10f);
                        infoImage.Add(i);
                        //add special effects
                        spxCombustion("CO", chemical.Level);
                        //add damage
                    }
                    else if (chemical.GetProduct("carbon") > 0)
                    {
                        string C = "carbon";
                        s = chemical.ChemicalFormula + "(" + chemical.State.ToString().ToLower()[0] + ")" + " + " + chemical.CalculateOxygen(C) + "O2(g) = " + chemical.GetProduct(C).ToString() + "C(s) + " + chemical.GetProduct("water").ToString() + "H2O(l)";
                        infoImage = scrollingDescription(s);
                        i.Text = "\n\rINCOMP COMB: ";
                        i.Text += chemical.Damage.ToString() + "kJ/mol";
                        i.Position = infoImage[infoImage.Count - 1].Position + new Vector2(0, 10f);
                        infoImage.Add(i);
                        //add special effects
                        spxCombustion("C", chemical.Level);
                        //add damage
                    }
                    else
                        infoImage = scrollingDescription("Insufficient O2 for Combustion.");
                    break;
                case "Branching":
                    int isomerState = Math.Min(chemical.Isomers, chemical.CheckMoveCount("Branching")+1);
                    if (chemical.CheckMoveCount("Branching") + 1> chemical.Isomers)
                    {
                        infoImage = scrollingDescription(chemical.Name + " has no structural isomers with further branching.");
                    }
                    else
                    {
                        infoImage = scrollingDescription(chemical.Name + " isomer: " + isomerState.ToString() + " branch. [row] Boiling pt decr as London Dispersion Forces decr.");
                        player.ChemicalManager.LoadIsomer(chemical.Name, isomerState);
                    }
                    //increase defense rating of chemical
                    break;
                case "Free Radical Sub":
                    searchBackpack("Initiation: [row] Br2 -> 2Cl*", reactant);
                    break;
                case "Addition Polymeriz":
                    //randomize polymerization dependent on what items are available
                    searchBackpack(chemical.ChemicalFormula + "(" + chemical.State.ToString().ToLower()[0] + ") + NiH2(catalyst) + HEAT -> ", reactant);
                    break;
                case "Oxidation": //alcohol to alkanal
                    //TO BE FIXED LATER
                    searchBackpack(chemical.ChemicalFormula + "(l) + Chromate(catalyst) -> Alkanal + water", reactant);
                    break;
                case "SN2 Nucleophil Sub":
                    //dependent on the catalyst available
                    searchBackpack(chemical.ChemicalFormula + "(" + chemical.State.ToString().ToLower()[0] + ")" + " NaOH(aq) + HEAT -> " + chemical.ChemicalFormula.Substring(0, chemical.ChemicalFormula.Length-2) + "OH(l) + NaBr(aq) [row] An Alcohol may join in the next turn!", reactant);
                    //Add another chemical at end of turn
                    break;
                case "Extinguisher":
                    s = "Bromomethane interrupts chain reactions propogating combustion! All fires are extinguished.";
                    infoImage = scrollingDescription(s);
                    environmentEffects.Add("Extinguisher");
                    spxClear();
                    spxImage.Add(new SPX(spxManager.EnvironmentXml("Extinguisher")));
                    break;
            }
            //Record Move
            player.ChemicalManager.GetBattleChemical(selectedChemical).RecordMove(player.ChemicalManager.GetBattleChemical(selectedChemical).BattleMove);
            foreach (Image img in infoImage)
                img.LoadContent();
        }
        void moveMenu()
        {
            //menu Items
            menu.Items.Clear();
            Chemical chemical = player.ChemicalManager.GetBattleChemical(selectedItem);
            Chemical tempChemical = new Chemical();
            string tempChemicalName = String.Empty;
            generateMoveList(chemical);
            bool isMultiStepMove = false;
            string[] multiStepMoves = { "Free Radical Sub", "Addition Polymeriz", "Oxidation", "SN2 Nucleophil Sub" };

            foreach (string str in multiStepMoves)
                if (!isMultiStepMove && chemical.GetMoveHistory(1, str))
                {
                    isMultiStepMove = true;
                    player.ChemicalManager.GetBattleChemical(selectedItem).BattleTag = str;
                    player.ChemicalManager.GetBattleChemical(selectedItem).RecordMove(str);
                    //infoImage
                    infoImageClear();
                    //depending on selected Item
                    switch (str)
                    {
                        case "Free Radical Sub":
                            if (chemical.GetMoveHistory(3, str))
                            {
                                string intermediate = chemical.ChemicalFormula.Substring(0, chemical.ChemicalFormula.Length - 1) + (chemical.GetElement(Element.H) - 1).ToString();
                                infoImage = scrollingDescription("Termination: [row] Br* + Br* -> Br2 [row] Br* + *" +  intermediate + " -> " + intermediate + "Br");
                                //add SPX and apply move effect here
                                //Add TEMP chemical here`
                            }
                            else 
                            {
                                infoImage = scrollingDescription("Propagation: [row] Br* " + chemical.ChemicalFormula + " -> HBr + *" + chemical.ChemicalFormula.Substring(0,chemical.ChemicalFormula.Length-1) + (chemical.GetElement(Element.H)-1).ToString());
                            }
                            break;
                        case "Addition Polymeriz":
                            if (chemical.CheckMoveCount(selectedItem) > 2)
                                isMultiStepMove = false;
                            else
                            {
                                tempChemicalName = chemical.Name.Replace("ene", "ane");
                                if (tempChemicalName.Contains("*"))
                                {
                                    string[] tempStr = tempChemicalName.Split('*');
                                    tempChemicalName = tempStr[0];
                                }
                                tempChemical = player.ChemicalManager.LoadTempChemical(tempChemicalName, "Alkane");
                                infoImage = scrollingDescription(tempChemicalName + " joins the battle!");
                            }
                            break;
                        case "Oxidation":
                            if (chemical.CheckMoveCount(selectedItem) > 2)
                                isMultiStepMove = false;
                            else
                            {
                                tempChemicalName = chemical.Name.Replace("anol", "anal");
                                if (tempChemicalName.Contains("*"))
                                {
                                    string[] tempStr = tempChemicalName.Split('*');
                                    tempChemicalName = tempStr[0];
                                }
                                tempChemical = player.ChemicalManager.LoadTempChemical(tempChemicalName, "Aldehyde");
                                infoImage = scrollingDescription(tempChemicalName + " joins the battle!");
                            }
                            break;
                        case "SN2 Nucleophil Sub":
                            if (chemical.CheckMoveCount(selectedItem) > 2)
                                isMultiStepMove = false;
                            else
                            {
                                tempChemicalName = chemical.Name.Replace("Bromo", String.Empty);
                                tempChemicalName = tempChemicalName.Replace("ane", "anol");
                                if (tempChemicalName.Contains("*"))
                                {
                                    string[] tempStr = tempChemicalName.Split('*');
                                    tempChemicalName = tempStr[0];
                                }
                                tempChemical = player.ChemicalManager.LoadTempChemical(tempChemicalName, "Alcohol");
                                infoImage = scrollingDescription(tempChemicalName + " joins the battle!");
                            }
                            break;
                    }
                    foreach (Image img in infoImage)
                        img.LoadContent();
                }
            if (!isMultiStepMove)                
                foreach (string move in moveList)
                {
                    MenuItem item = new MenuItem();
                    item.Image = new Image();
                    item.Image.Text = move; //Test Text
                    item.Image.TextColor = Color.Black;
                    item.Image.FontName = "Fonts/OCRAsmall";
                    item.LinkType = "Move";
                    item.LinkID = "Content/Load/Menu/DescriptionMenu.xml";
                    string h = String.Empty;
                    h = moveReactants(move);
                    menu.Items.Add(item);
                }
        }
        #endregion

        #region Option Menus
        void optionMoveMenu() //Currently alligned for CHEMICALS name
        {
            menu.Alignment.X = 340;
            menu.Items.Clear();
            foreach (string battleChemicalName in player.ChemicalManager.battleChemicalName)
            {
                Chemical chemical = player.ChemicalManager.GetBattleChemical(battleChemicalName);
                //if current move for battleChemical is not set...
                if (chemical.BattleMove == String.Empty)
                {
                    MenuItem item = new MenuItem();
                    item.Image = new Image();
                    item.Image.Text = battleChemicalName.ToUpper();
                    string s = (player.ChemicalManager.GetChemical(battleChemicalName).State).ToString().ToLower();
                    item.Image.Text += "(" + s.Substring(0, 1) + ")";

                    string h = (player.ChemicalManager.GetChemical(battleChemicalName).CurrentHealth).ToString() + "/" + (player.ChemicalManager.GetChemical(battleChemicalName).Health).ToString();

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
            infoImageClear();
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
            infoImageClear();
            Vector2 dimensions = new Vector2(menu.Image.Position.X + menu.Image.SourceRect.Width / 2, 50);
            itemManager.CurrentItemNumber = prevSelectedItem;
            Item item = itemManager.CurrentItem;

            //1: Item Name
            Image i = new Image();
            i.FontName = "Fonts/OCRAsmall";
            i.Text = selectedItem.ToUpper();
            i.TextColor = Color.Black;
            i.Position = new Vector2(dimensions.X - menu.Image.Font.MeasureString(i.Text).X / 2f, dimensions.Y);
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
        void spxCombustion(string combustionType, int level)
        {
            if (level > 5 && combustionType == "CO2")
                spxImage.Add(new SPX(spxManager.AOEXml(combustionType + "_big", true)));
            else if (level > 3 && combustionType != "CO2")
                spxImage.Add(new SPX(spxManager.AOEXml(combustionType + "_big", true)));
            else if (level > 3 && combustionType == "CO2")
                spxImage.Add(new SPX(spxManager.AOEXml(combustionType + "_mid", true)));
            else
                spxImage.Add(new SPX(spxManager.AOEXml(combustionType + "_small", true)));
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
        void optionMenuPage()
        {
            if (menu.ItemNumber < 0)
                menu.ItemNumber = 0;
            else if (menu.ItemNumber > menu.Items.Count - 1)
                menu.ItemNumber = menu.Items.Count - 1;
            if (menu.Items.Count > 0)
            {
                pageText = ((int)(menu.ItemNumber / 3 + 1)).ToString() + "/" + ((int)((menu.Items.Count + 2) / 3)).ToString();
                if (!menu.Items[menu.ItemNumber].Image.IsVisible)
                {
                    int invisible = menu.ItemNumber - menu.ItemNumber % 3;
                    for (int i = 0; (i < invisible) && (i < menu.Items.Count()); i++)
                        menu.Items[i].Image.IsVisible = false;
                    for (int j = invisible; j < (invisible + 3) && j < (menu.Items.Count()); j++)
                        menu.Items[j].Image.IsVisible = true;
                    for (int k = invisible + 3; k < menu.Items.Count(); k++)
                        menu.Items[k].Image.IsVisible = false;
                    pageText = ((int)(menu.ItemNumber / 3 + 1)).ToString() + "/" + ((int)((menu.Items.Count + 2) / 3)).ToString();
                }
            }

        }
        void drawOxygen(SpriteBatch spriteBatch)
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
        List<Image> scrollingDescription(string description)
        {
            List<Image> imageList = new List<Image>();
            Image i = new Image();
            Vector2 dimensions = new Vector2(340f, 580.5f);
            i.FontName = "Fonts/OCRAsmall";
            i.TextColor = Color.Black;
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
                        count = 1;
                        dimensions = new Vector2(340f, 580.5f);
                    }
                    count++;
                    i.FontName = "Fonts/OCRAsmall";
                    i.TextColor = Color.Black;
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
                        i.TextColor = Color.Black;
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
                        count = 1;
                        dimensions = new Vector2(340f, 580.5f);
                    }
                    count++;
                    i.Position = dimensions;
                    i.TextColor = Color.Black;
                    i.FontName = "Fonts/OCRAsmall";

                    text = s + " ";
                    rowLength = s.Length + 1;
                   
                    if (s == parts[parts.Length - 1]) //if string is last word in dialogue
                    {
                        i = new Image();
                        i.Text = text;
                        i.FontName = "Fonts/OCRAsmall";
                        i.TextColor = Color.Black;
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
        #endregion

        #region Initializations
        void initializeParties()
        {
            XmlManager<Player> playerLoader = new XmlManager<Player>();
            player = playerLoader.Load("Content/Load/Gameplay/Player.xml");
            player.LoadContent();
            player.Image.Position = new Vector2(128, 360);
            player.Image.SpriteSheetEffect.CurrentFrame.Y = 7;
            player.Image.SpriteSheetEffect.SwitchFrame = 500;
            player.Image.IsActive = true;
            player.InitializeBattle();

            XmlManager<Character> characterLoader = new XmlManager<Character>();
            enemy = characterLoader.Load("Content/Load/Gameplay/Markovnikov.xml");
            enemy.LoadContent("Content/Load/Gameplay/Battle/Markovnikov.xml");
            enemy.Image.Position = new Vector2(1064, 175);
            enemy.Image.SpriteSheetEffect.CurrentFrame.Y = 1;
            enemy.Image.IsActive = true;
            enemy.InitializeBattle();
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
            if (menu.Type == "Move")
            {
                optionMenuPage();
            }
        }
        public void SelectUp(eButtonState buttonState)
        {
            if (buttonState == eButtonState.DOWN)
                menu.ItemNumber--;
            else if (menu.Type.Contains("Option"))
                optionMenuPage();
            if (menu.Type == "Move")
            {
                optionMenuPage();
            }

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
                if (isDescription)
                {
                    infoImage.RemoveRange(0, Math.Min(3, infoImage.Count));
                    for (int i = 0; i < Math.Min(infoImage.Count, 3); i++)
                        infoImage[i].IsVisible = true;
                    if (infoImage.Count < 3)
                        isDescription = false;
                }
                else if (menu.Items.Count < 1)
                {
                    if (isPlayerTurn)
                        isPlayerTurn = false;
                    else
                        isPlayerTurn = true;
                }
                else if (menu.Items[menu.ItemNumber].LinkType == "Screen")
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

                    if (menu.Type.Contains("Option") || menu.Type == "Move")
                    {
                        string str = menu.Items[menu.ItemNumber].Image.Text;
                        if (str != String.Empty)
                        {
                            if (str.Contains("("))
                                str = str.Substring(0, str.IndexOf('(')).ToLower();

                            if (menu.Type == "Move")
                            {
                                player.ChemicalManager.GetBattleChemical(selectedItem).BattleMove = str;
                                //player.ChemicalManager.GetBattleChemical(selectedItem).RecordMove(str);
                                selectedChemical = selectedItem;
                            }
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
            if (buttonState == eButtonState.DOWN && !isTransitioning && !currentMenuID.Contains("BattleMenu") && menu.Type != "Flee" && menu.Type != "Description")
            {
                if (currentMenuID.Contains("Move") && !prevMenuID.Contains("BattleMenu"))
                {
                    prevMenuID = "Content/Load/Menu/BattleMenu.xml";
                    currentMenuID = "Content/Load/Menu/OptionMoveMenu.xml";
                    menu.ID = currentMenuID;
                    menu.Active = true;
                }
                else if (prevMenuID != String.Empty && prevMenuID != currentMenuID)
                {
                    if (clone.Count > 0 && !currentMenuID.Contains("Move"))
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

        #region Shortcuts
        void spxImageFade()
        {
            foreach (SPX spx in spxImage)
                spx.FadeOut = true; 
        }
        void spxImageClear()
        {
            foreach (SPX spx in spxImage)
                spx.UnloadContent();
            spxImage.Clear();
        }
        void infoImageClear()
        {
            foreach (Image image in infoImage)
                image.UnloadContent();
            infoImage.Clear();
        }
        void spxClear()
        {
            foreach (SPX spx in spxImage)
                spx.UnloadContent();
            spxImage.Clear();
        }
        #endregion
    }
}
