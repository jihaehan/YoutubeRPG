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
        bool isDescription;
        bool isWin;
        bool isLevelling;
        string isLevelled;

        string prevMenuID;
        string currentMenuID;
        string selectedItem, selectedChemical;
        string selectedEnemy;
        int prevSelectedItem, battleMenuSelectedItem;
        int turnCount;
        int levellingCount;
        float EXP;
        List<Image> infoImage; //for visual text distinct from menuButtons
        List<Image> levellingImage;
        List<SPX> spxImage;    //for special effects distinct from menuButtons

        List<string> moveList; //for individual chemicals
        List<string> environmentEffects; //for AOE effects for all chemicals
        List<string> levellingChemicals; //for chemicals that level up
        List<string> levelledChemicals;
        Dictionary<string, float> originalDefense; //originalDefense for extinguisher effect
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
            totalOxygen = currentOxygen = 3;
            prevSelectedItem = battleMenuSelectedItem = 0;
            turnCount = 1;
            EXP = 0;
            levellingCount = 0;
            prevMenuID = currentMenuID = selectedItem = selectedEnemy =  String.Empty;
            pageText = "1/3";
            isDescription = false;
            isLevelling = false;
            isLevelled = String.Empty;
            isWin = false;
            page = new Image();
            cardDown = new Image();
            cardUp = new Image();
            O2Empty = new Image();
            O2Filled = new Image();
            O2Label = new Image();
            spxManager = new SPXManager();
            spxImage = new List<SPX>();
            infoImage = new List<Image>();
            levellingImage = new List<Image>();
            originalDefense = new Dictionary<string, float>();
            moveList = environmentEffects = new List<string>();
            levellingChemicals = new List<string>();
            levelledChemicals = new List<string>();
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
            else if (currentMenuID.Contains("Enemy"))
                enemyMenu();
            else if (currentMenuID.Contains("EndBattle"))
                endBattleMenu();
            else if (currentMenuID.Contains("Levelling"))
                levellingMenu();
            
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
            if (menu.Type != "Description" && !currentMenuID.Contains("/Move"))
            {
                infoImageClear();
            }
            if (currentMenuID.Contains("/BattleMenu"))
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
        public void LoadContent(string enemyXml, string enemyPartyXml)
        {
            if (ScreenManager.Instance.Enemy != "Anonymous")
                initializeParties(enemyXml, enemyPartyXml);
            else
                initializeRandomParty();

            menu.ID = "Content/Load/Menu/BattleMenu.xml";
            prevMenuID = currentMenuID = "Content/Load/Menu/BattleMenu.xml";
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
        public void UnloadContent()
        {
            foreach (string name in player.ChemicalManager.chemicalName)                    player.ChemicalManager.GetChemical(name).Image.FadeEffect.IsActive = false;
            player.ChemicalManager.SaveParty();
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
            foreach (Image i in levellingImage)
                i.UnloadContent();
        }
        public void Update(GameTime gameTime)
        {
            if (!isLevelling)
            {
                player.BattleUpdate(gameTime);
                enemy.BattleUpdate(gameTime);
            }
            if (!isTransitioning)
               menu.Update(gameTime);
            Transition(gameTime);
            itemManager = player.ItemManager;
            foreach (SPX spx in spxImage)
                spx.Update(gameTime);
            foreach (Image i in infoImage)
                i.Update(gameTime);
            foreach (Image i in levellingImage)
            {
                if (i.Alpha >= 1.0f)
                    i.IsActive = false;
                i.Update(gameTime);
            }

            for (int count = 0; count < levellingCount; count++)
            {
                if (count != levellingCount - 1)
                    player.ChemicalManager.LevellingHide(gameTime, levellingChemicals[count], levelledChemicals[count]);
                else
                {

                    player.ChemicalManager.LevellingTransitionUpdate(gameTime, levellingChemicals[count], levelledChemicals[count]);
                    isLevelled = levelledChemicals[count];
                }
            }
            
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!isLevelling)
            {
                enemy.BattleDraw(spriteBatch);
                player.BattleDraw(spriteBatch);
                foreach (SPX spx in spxImage)
                    spx.Draw(spriteBatch);
            }
            foreach (Menu m in clone)
                m.Draw(spriteBatch);
            menu.Draw(spriteBatch);
            if (!isLevelling)
            {
                cardDown.Draw(spriteBatch);
                cardUp.Draw(spriteBatch);
                drawOxygen(spriteBatch);
            }
            else
                foreach (Image img in levellingImage)
                    img.Draw(spriteBatch);
            foreach (string n in levelledChemicals)
                player.ChemicalManager.GetChemical(n).Draw(spriteBatch);

            if (levellingCount > 0 && levellingChemicals.Count >= levellingCount)
            {
                string n = levellingChemicals[levellingCount - 1];
                string n1 = levelledChemicals[levellingCount - 1];
                if (player.ChemicalManager.chemicalName.Contains(n))
                    player.ChemicalManager.LevellingDraw(spriteBatch, n);
                else if (player.ChemicalManager.chemicalName.Contains(n1))
                    player.ChemicalManager.LevellingDraw(spriteBatch, n1);
            }


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
        bool checkWinCondition(bool isPlayer)
        {
            bool win = false;
            int deadCount = 0;
            int battleCount = 0;
            if (isPlayer)
            {
                foreach (string n in enemy.ChemicalManager.battleChemicalName)
                {
                    if (enemy.ChemicalManager.GetBattleChemical(n).InBattle)
                    {
                        if (enemy.ChemicalManager.GetBattleChemical(n).IsDead)
                            deadCount++;
                        battleCount++;
                    }
                }
                if (deadCount == battleCount) 
                {
                    win = true;
                    isWin = true;
                }
            }
            else
            {
                foreach (string n in player.ChemicalManager.battleChemicalName)
                {
                    if (player.ChemicalManager.GetBattleChemical(n).InBattle)
                    {
                        if (player.ChemicalManager.GetBattleChemical(n).IsDead)
                            deadCount++;
                        battleCount++;
                    }
                }
                if (deadCount == battleCount)
                    win = true;
            }
            return win;
        }
        string checkPlayableChemicals()
        {
            int playableChemicals = 0;
            foreach (string battleChemicalName in player.ChemicalManager.battleChemicalName)
                if (player.ChemicalManager.GetBattleChemical(battleChemicalName).BattleMove == String.Empty)
                    playableChemicals++;
            if (playableChemicals > 0)
                return  "Content/Load/Menu/OptionMoveMenu.xml";
            else //switch to battling Menu
            {
                //Manage Extinguisher effect
                originalDefense.Clear();
                if (spxManager.EnvironmentEffects["Extinguisher"])
                {
                    foreach (string n in player.ChemicalManager.battleChemicalName)
                    {
                        originalDefense.Add(n, player.ChemicalManager.GetBattleChemical(n).Defense);
                        player.ChemicalManager.GetBattleChemical(n).Defense -= .2f;
                        if (player.ChemicalManager.GetBattleChemical(n).Defense < .3f)
                            player.ChemicalManager.GetBattleChemical(n).Defense = .3f;
                    }
                }
                return "Content/Load/Menu/BattlingMenu.xml";
            }
        }
        void searchBackpack(string infoText, string reactant)
        {
            bool found = false;
            for (int j = 0; j < itemManager.Items.Count; j++)
            {
                if (itemManager.Items[j].Name == reactant && !found)
                {
                    infoImage = scrollingDescription(infoText, Color.Black);
                    found = true;
                    itemManager.Items.RemoveAt(j);
                }
            }
            if (!found)
            {
                infoImage = scrollingDescription(selectedChemical + " does not have " + reactant, Color.Black);
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
                    break;
                case "Addition Polymeriz":
                    req = "Nickeldihydride"; //alkene to alkane
                    break;
                case "Oxidation":
                    req = "Chromate";        //alcohol to alkanal
                    break;
                case "SN2 Nucleophil Sub":   //halogenoalkane to alcohol
                    req = "Sodiumhydroxide";
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
        void enemyCard()
        {
            Random rnd = new Random();
            List<string> notInParty = new List<string>();
            foreach (string chemicalName in enemy.ChemicalManager.chemicalName)
                if (!enemy.ChemicalManager.GetChemical(chemicalName).InBattle)
                    notInParty.Add(chemicalName);
            if (notInParty.Count > 0)
            {
                int randomIndex = rnd.Next(notInParty.Count);
                enemy.ChemicalManager.GetChemical(notInParty[randomIndex]).InBattle = true;
            }
        }
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
            //infoImage
            infoImageClear();
            string s = String.Empty;
            //trigger Temporary Chemical to leave battle
            s = player.ChemicalManager.RemoveRandomTempChemical();
            if (s != String.Empty)
                infoImage = scrollingDescription(s + " leaves the battle! [row] Thanks for the help!", Color.Black);
            foreach (Image image in infoImage)
                image.LoadContent();

            //Manage enemyTurn
            string e = enemy.ChemicalManager.EnemyInstance();

            if (e != String.Empty)
            {
                menu.Items.Clear();
                MenuItem item = new MenuItem();
                item.Image = new Image();
                item.Image.Position = new Vector2(-5, -5);
                item.Image.Text = ".";

                if (checkWinCondition(false) || checkWinCondition(true))
                {   //if EnemyWins
                    item.LinkID = "Content/Load/Menu/EndBattleMenu.xml";
                }
                else
                {
                    selectedEnemy = e;
                    item.LinkID = "Content/Load/Menu/EnemyMenu.xml";
                }
                menu.Items.Add(item);
            }
            else //manage end of turn
            {
                if (checkWinCondition(true))
                {
                    menu.Items.Clear();
                    MenuItem item = new MenuItem();
                    item.Image = new Image();
                    item.Image.Position = new Vector2(-5, -5);
                    item.Image.Text = " ";
                    item.LinkID = "Content/Load/Menu/EndBattleMenu.xml";
                    menu.Items.Add(item);
                }
                //Refresh values for player and enemy turn
                EndTurn();
                //clear special effects and manage Extinguisher effect
                if (spxManager.EnvironmentEffects["Extinguisher"])
                {
                    foreach (string n in player.ChemicalManager.battleChemicalName)
                    {
                        if (originalDefense.ContainsKey(n))
                            player.ChemicalManager.GetBattleChemical(n).Defense = originalDefense[n];
                    }
                }
                spxManager.EnvironmentEffects["Extinguisher"] = false;
                foreach (SPX spx in spxImage)
                    spx.FadeOut = true;
                //draw card and add new chemical to team
                battleCard();
                enemyCard();
            }
        }
        void endBattleMenu()
        {
            infoImageClear();

            foreach (string n in enemy.ChemicalManager.deadChemicalName)
            {
                Chemical c = enemy.ChemicalManager.GetChemical(n);
                if (c.InBattle)
                    EXP += c.Mass;
            }
            if (player.ChemicalManager.BattlePartySize() > 0)
                EXP = EXP / player.ChemicalManager.BattlePartySize();
            else
                EXP = 0;
            if (isWin)
                infoImage = scrollingDescription("Marie has won the battle! [row] Each Party member gains " + EXP.ToString() + " of Atomic Mass!", Color.Black);
            else
                infoImage = scrollingDescription(enemy.Name + " has won the battle! [row] Each Party gains " + (EXP/3).ToString() + " of Atomic Mass!", Color.SaddleBrown);

            foreach (string n in player.ChemicalManager.battleChemicalName)
            {
                if (isWin)
                    //player.ChemicalManager.GetBattleChemical(n).Experience += (int)EXP;
                    player.ChemicalManager.GetBattleChemical(n).Experience += (int)EXP/ player.ChemicalManager.GetBattleChemical(n).Level;
                else
                    //player.ChemicalManager.GetBattleChemical(n).Experience += (int)(EXP/ 3);
                    player.ChemicalManager.GetBattleChemical(n).Experience += (int)(EXP/ player.ChemicalManager.GetBattleChemical(n).Level/3);
                //check if levelling conditions are met
                if (player.ChemicalManager.GetBattleChemical(n).Experience > player.ChemicalManager.GetBattleChemical(n).Mass)
                {
                    levellingChemicals.Add(n);
                    menu.Items.Clear();
                    MenuItem item = new MenuItem();
                    item.Image = new Image();
                    item.Image.Position = new Vector2(-5, -5);
                    item.Image.Text = ".";
                    item.LinkType = "Menu";
                    item.LinkID = "Content/Load/Menu/LevellingMenu.xml";
                    menu.Items.Add(item);
                }
            }
            foreach (Image i in infoImage)
                i.LoadContent();
        }
        void levellingMenu()
        {
            isLevelling = true;
            infoImageClear();
            string s = String.Empty;
            string evolvedName = String.Empty;
            Chemical chemical = new Chemical();
            XmlManager<Chemical> chemicalLoader = new XmlManager<Chemical>();
            
            if (levellingCount < levellingChemicals.Count)
            {
                foreach (Image img in levellingImage)
                    img.UnloadContent();
                levellingImage.Clear();
                string n = levellingChemicals[levellingCount];
                chemical = player.ChemicalManager.GetBattleChemical(n);
                if (n.Contains("Bromo"))
                    evolvedName = getTempName(n, chemical.NameLevel(chemical.Level).ToLower(), chemical.NameLevel(chemical.Level + 1).ToLower());
                else
                    evolvedName = getTempName(n, chemical.NameLevel(chemical.Level), chemical.NameLevel(chemical.Level + 1));
                if ((chemical.Level + 1 < 6 && chemical.Series != Series.Alkene) || (chemical.Level + 1 < 7 && chemical.Series == Series.Alkene))
                {
                    s += n + " has levelled up! Has gained enough atomic mass to evolve into a " + evolvedName + ".";
                    Chemical c = chemicalLoader.Load("Content/Load/Chemical/" + chemical.Series.ToString() + "/" + evolvedName + ".xml");
                    c.LoadContent();
                    player.ChemicalManager.AddChemical(c);
                    player.ChemicalManager.LevellingTransition(n, evolvedName);
                    levelledChemicals.Add(evolvedName);
                    displayLevellingInfo(chemical, c);
                }
                else
                {
                    s += n + " has reached Level Cap. Expand your knowledge of chemistry to unlock next levels! [row] ";
                    player.ChemicalManager.LevellingStop(n);
                    levelledChemicals.Add(n);
                    displayLevellingInfo(chemical, chemical);
                }
                infoImage = scrollingDescription(s, Color.Black);
                //move other chemicals elsewhere
                foreach (string name in levellingChemicals)
                    if (name != n && player.ChemicalManager.chemicalName.Contains(name))
                        player.ChemicalManager.GetBattleChemical(name).Image.Position = new Vector2(-128, -128);
                levellingCount++;
                foreach (Image i in levellingImage)
                    i.LoadContent();
            }
            else
            {
                menu.Items.Clear();
                MenuItem item = new MenuItem();
                item.Image = new Image();
                item.Image.Text = " ";
                item.LinkType = "Screen";
                item.LinkID = "GameplayScreen_Blue";
                menu.Items.Add(item);
            }
            foreach (Image i in infoImage)
                i.LoadContent();

        }
        string enemyMoveName(string name) //NPC logic
        {
            Chemical chemical = enemy.ChemicalManager.GetBattleChemical(name);
            generateMoveList(chemical);
            enemy.ChemicalManager.GetBattleChemical(name).BattleMove = pickMove(chemical);
            return enemy.ChemicalManager.GetBattleChemical(name).BattleMove;
        }
        public void EndTurn()
        {
            //refresh moves for party members
            foreach (string name in player.ChemicalManager.battleChemicalName)
                player.ChemicalManager.GetBattleChemical(name).BattleMove = String.Empty;
            foreach (string name in enemy.ChemicalManager.battleChemicalName)
                enemy.ChemicalManager.GetBattleChemical(name).TurnTaken = false;
            turnCount++;
            if (totalOxygen < 27)
            {
                totalOxygen = turnCount * 3;
                currentOxygen = turnCount * 3;
            }
        }
        #endregion

        #region Enemy Logic
        void enemyMenu()
        {
            #region setup
            //Menu Items
            if (checkWinCondition(false))
            {
                menu.Items.Clear();
                MenuItem item = new MenuItem();
                item.Image = new Image();
                item.Image.Text = "  ";
                item.LinkID = "Content/Load/Menu/EndBattleMenu.xml";
                menu.Items.Add(item);
            }
            //InfoImage
            infoImageClear();
           
            Chemical chemical = enemy.ChemicalManager.GetBattleChemical(selectedEnemy);
            if (chemical == null)
            {
                selectedEnemy = enemy.ChemicalManager.EnemyInstance();
                if (selectedEnemy == String.Empty)
                {
                    menu.Items.Clear();
                    MenuItem item = new MenuItem();
                    item.Image = new Image();
                    item.Image.Text = "  ";
                    item.LinkID = "Content/Load/Menu/BattlingMenu.xml";
                    menu.Items.Add(item);
                }
                else 
                    chemical = enemy.ChemicalManager.GetBattleChemical(selectedEnemy);
            }
            if (chemical != null)
            {
                string s = String.Empty;
                string tempChemicalName = String.Empty;
                string randomChemical = String.Empty;
                List<string> randomChemicals = new List<string>();
                bool isMultiStepMove = false;
                string[] multiStepMoves = { "Free Radical Sub", "Addition Polymeriz", "Oxidation", "SN2 Nucleophil Sub" };
                #endregion
                foreach (string str in multiStepMoves)
                    if (!isMultiStepMove && chemical != null && chemical.GetMoveHistory(1, str))
                    {
                        isMultiStepMove = true;
                        enemy.ChemicalManager.GetBattleChemical(selectedEnemy).BattleMove = str;
                        enemy.ChemicalManager.GetBattleChemical(selectedEnemy).RecordMove(str);
                        switch (str)
                        {
                            case "Free Radical Sub": //alkane to halogenoalkane
                                if (chemical.CheckMoveCount(str) > 3)
                                    exitMultiStep(ref isMultiStepMove, false);
                                else if (chemical.GetMoveHistory(3, str))
                                {
                                    tempChemicalName = getTempName("Bromo" + chemical.Name.ToLower(), String.Empty, String.Empty);
                                    string intermediate = chemical.ChemicalFormula.Substring(0, chemical.ChemicalFormula.Length - 1) + (chemical.GetElement(Element.H) - 1).ToString();
                                    infoImage = scrollingDescription("Last step of Free Radical Substitution. [row] Termination: [row] Br* + Br* -> Br2 [row] Br* + *" + intermediate + " -> " + intermediate + "Br [row] " + tempChemicalName + " joins the battle!", Color.SaddleBrown);
                                    enemy.ChemicalManager.LoadTempChemical(tempChemicalName, "Halogenoalkane");
                                }
                                else
                                {
                                    infoImage = scrollingDescription(chemical.Name + " enters second step of Free Radical Substitution. [row] Propagation: [row] Br* " + chemical.ChemicalFormula + " -> HBr + *" + chemical.ChemicalFormula.Substring(0, chemical.ChemicalFormula.Length - 1) + (chemical.GetElement(Element.H) - 1).ToString(), Color.SaddleBrown);
                                }
                                break;
                            case "Addition Polymeriz": //alkene to alkane
                                if (chemical.CheckMoveCount(str) > 2)
                                    exitMultiStep(ref isMultiStepMove, false);
                                else
                                {
                                    tempChemicalName = getTempName(chemical.Name, "ene", "ane");
                                    enemy.ChemicalManager.LoadTempChemical(tempChemicalName, "Alkane");
                                    infoImage = scrollingDescription(tempChemicalName + " joins the battle!", Color.SaddleBrown);
                                }
                                break;
                            case "SN2 Nucleophil Sub":
                                if (chemical.CheckMoveCount(str) > 2)
                                    exitMultiStep(ref isMultiStepMove, false);
                                else
                                {
                                    tempChemicalName = chemical.Name.Replace("Bromo", String.Empty);
                                    tempChemicalName = getTempName(tempChemicalName, "ane", "anol");
                                    tempChemicalName = tempChemicalName[0].ToString().ToUpper() + tempChemicalName.Substring(1);
                                    enemy.ChemicalManager.LoadTempChemical(tempChemicalName, "Alcohol");
                                    infoImage = scrollingDescription(tempChemicalName + " joins the battle!", Color.SaddleBrown);
                                }
                                break;
                        }
                    }

                if (!isMultiStepMove)
                {
                    string move = enemyMoveName(selectedEnemy);
                    enemy.ChemicalManager.GetBattleChemical(selectedEnemy).BattleMove = move;
                    enemy.ChemicalManager.GetBattleChemical(selectedEnemy).RecordMove(move);
                    switch (move)
                    {
                        case "Formation":
                            randomChemical = getRandomChemical(false);
                            s = selectedEnemy + " releases an Enthalpy of Formation of " + chemical.BaseDamage.ToString() + " kJ/mol";
                            infoImage = scrollingDescription(s, Color.SaddleBrown);
                            s = randomChemical + " takes " + calculateDamage(randomChemical, selectedEnemy, chemical.BaseDamage, false).ToString() + " kJ/mol of damage!";
                            continueDescription(s, Color.Black);
                            spxImage.Add(new SPX(spxManager.TargetXml(), player.ChemicalManager.GetBattleChemical(randomChemical).Image.Position));
                            break;
                        case "Combustion":
                            chemical.ClearProducts();
                            chemical.SetOxygen(currentOxygen);
                            chemical.Combustion();
                            if (chemical.GetProduct("carbondioxide") > 0)
                            {
                                s = selectedEnemy + " reacts with Oxygen to produce Carbondioxide and Water! [row] Releases Enthalpy of Combustion of " + chemical.Damage.ToString() + "kJ/mol";
                                infoImage = scrollingDescription(s, Color.SaddleBrown);
                                spxCombustion("CO2", chemical.Level, false);
                                damagedCombustion(chemical, "red", false); //add damage from combustion
                            }
                            else if (chemical.GetProduct("carbonmonoxide") > 0)
                            {
                                s = selectedEnemy + " reacts with Oxygen to produce Carbonmonoxide and Water! [row] Incomplete Combustion releases " + chemical.Damage.ToString() + "kJ/mol";
                                infoImage = scrollingDescription(s, Color.SaddleBrown);
                                spxCombustion("CO", chemical.Level, false);
                                damagedCombustion(chemical, "white", false); //add damage from combustion
                            }
                            else if (chemical.GetProduct("carbon") > 0)
                            {
                                s = selectedEnemy + " reacts with Oxygen to produce Soot and Water! [row] Incomplete Combustion releases " + chemical.Damage.ToString() + "kJ/mol";
                                infoImage = scrollingDescription(s, Color.SaddleBrown);
                                spxCombustion("C", chemical.Level, false);
                                damagedCombustion(chemical, "black", false); //add damage from combustion
                            }
                            else
                                infoImage = scrollingDescription(selectedEnemy + " tries to combust! But insufficient Oxygen.", Color.SaddleBrown);
                            break;
                        case "Branching":
                            int isomerState = Math.Min(chemical.Isomers, chemical.CheckMoveCount("Branching") + 1);
                            if (chemical.CheckMoveCount("Branching") + 1 > chemical.Isomers)
                                infoImage = scrollingDescription(selectedEnemy + " attempts to branch but fails!", Color.SaddleBrown);
                            else
                            {
                                infoImage = scrollingDescription(selectedEnemy + " conforms into a branched isomer: " + isomerState.ToString() + " branch.", Color.SaddleBrown);
                                enemy.ChemicalManager.LoadIsomer(chemical.Name, isomerState);
                            }
                            //increase defense rating of chemical
                            break;
                        case "Free Radical Sub":
                            s = selectedEnemy + " begins Free Radical Substitution. [row] Initiation: [row] Br2-> 2Cl * ";
                            infoImage = scrollingDescription(s, Color.SaddleBrown);
                            break;
                        case "Addition Polymeriz":
                            s = selectedEnemy + " begins Addition Polymerisation. [row] Reacts with Nickel Catalyst and HEAT!";
                            infoImage = scrollingDescription(s, Color.SaddleBrown);
                            break;
                        case "Oxidation": //alcohol to alkanal
                                          //FIX later on...
                            s = selectedEnemy + " begins Oxidation. [row]";
                            infoImage = scrollingDescription(s, Color.SaddleBrown);
                            break;
                        case "SN2 Nucleophil Sub":
                            s = selectedEnemy + " begins SN2 Nucleophilic Substitution. [row] Reacts with Sodium Hydroxide and HEAT!";
                            infoImage = scrollingDescription(s, Color.SaddleBrown);
                            break;
                        case "Extinguisher":
                            s = "Bromomethane interrupts chain reactions propogating combustion! Defense of all party members increases dramatically.";
                            infoImage = scrollingDescription(s, Color.SaddleBrown);
                            if (!environmentEffects.Contains(move))
                                environmentEffects.Add("Extinguisher");
                            spxImage.Add(new SPX(spxManager.EnvironmentXml("Extinguisher")));
                            break;
                    }
                    infoImage = scrollingDescription(s, Color.SaddleBrown);
                }
                foreach (Image image in infoImage)
                    image.LoadContent();
            }
            
        }

        string pickMove(Chemical chemical)
        {
            string selected = String.Empty;
            Random rnd = new Random();

            switch (chemical.Series)
            {
                case Series.Alkane:
                    if (rnd.Next(100) < 20) //20% chance of triggrer
                        selected = "Free Radical Sub";
                    break;
                case Series.Alkene:
                    if (rnd.Next(100) < 15) //15% chance of trigger
                        selected = "Addition Polymeriz";
                    break;
                case Series.Halogenoalkane:
                    if (rnd.Next(100) < 10)  //10% chance of trigger
                        selected = "SN2 Nucleophil Sub";
                    break;
            }
            if (moveList.Contains("Extinguisher") && selected == String.Empty)
            {
                if (rnd.Next(100) < 8)      //17% chance of trigger
                    selected = "Extinguisher";
            }
            else if (moveList.Contains("Branching") && selected == String.Empty)
            {
                if (rnd.Next(100) < 30 && chemical.CurrentHealth < chemical.Health / 2)
                {
                    string isomerState = chemical.Image.Path;
                    for (int i = 3; i > 0; i--)
                    {
                        if (rnd.Next(100) < 50 && selected == String.Empty)
                            if (isomerState.Contains(i.ToString()) && chemical.Isomers > i)
                                selected = "Branching";
                    }
                    if (chemical.Isomers > 0 && !isomerState.Contains("1") && selected == String.Empty)
                        selected = "Branching";
                }
            }
            else if (moveList.Contains("Combustion") && selected == String.Empty)
            {
                if (chemical.CalculateOxygen("carbon") <= currentOxygen && rnd.Next(100) < 90)
                    selected = "Combustion";
            }
            if (selected != String.Empty && moveList.Contains(selected))
                return selected;
            else
                return "Formation";
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
            item.Image.Position = new Vector2(-5, -5);
            item.Image.Text = "."; 
            item.LinkType = "Move";

            //if there are no chemicals left with moves, or check win condition
            if (checkWinCondition(true))
                item.LinkID = "Content/Load/Menu/EndBattleMenu.xml";
            else
                item.LinkID = checkPlayableChemicals();
            menu.Items.Add(item);

            //InfoImage
            infoImageClear();
            Image i = new Image();
            i.FontName = "Fonts/OCRAsmall";
            i.TextColor = Color.SaddleBrown;
            Chemical chemical = player.ChemicalManager.GetBattleChemical(selectedChemical);
            string enemyChemical = String.Empty;
            List<string> enemyChemicals = new List<string>();
            string s = String.Empty;

            //If using items from backpack
            string reactant = moveReactants(selectedItem);
            #endregion
            //depending on which move has been selected
            switch (selectedItem)
            {
                case "Formation":
                    s = selectedChemical + " releases an Enthalpy of Formation of " + chemical.BaseDamage.ToString() + " kJ/mol [row]";
                    enemyChemical = getRandomChemical(true);
                    infoImage = scrollingDescription(s, Color.Black);
                    s = enemyChemical + " takes " + calculateDamage(selectedChemical, enemyChemical, chemical.BaseDamage, true).ToString() +" kJ/mol of damage!";
                    continueDescription(s, Color.SaddleBrown);
                    spxImage.Add(new SPX(spxManager.TargetXml(), enemy.ChemicalManager.GetBattleChemical(enemyChemical).Image.Position));
                    break;
                case "Combustion":
                    chemical.ClearProducts();
                    chemical.SetOxygen(currentOxygen);
                    chemical.Combustion();
                    if (chemical.GetProduct("carbondioxide") > 0)
                    {
                        string CO2 = "carbondioxide";
                        s = chemical.ChemicalFormula + "(" + chemical.State.ToString().ToLower()[0] + ")" + " + " + chemical.CalculateOxygen(CO2) + "O2(g) = " + chemical.GetProduct(CO2).ToString() + "CO2(g) + " + chemical.GetProduct("water").ToString() + "H2O(l)";
                        infoImage = scrollingDescription(s, Color.Black);
                        i.Text = "\n\rCOMP COMBUST: ";
                        i.Text += chemical.Damage.ToString() + "kJ/mol";
                        i.Position = infoImage[infoImage.Count - 1].Position + new Vector2(0, 10f);
                        infoImage.Add(i);
                        currentOxygen -= chemical.CalculateOxygen(CO2);
                        spxCombustion("CO2", chemical.Level, true);
                        damagedCombustion(chemical, "red", true); //add damage from combustion
                    }
                    else if (chemical.GetProduct("carbonmonoxide") > 0)
                    {
                        string CO = "carbonmonoxide";
                        s = chemical.ChemicalFormula + "(" + chemical.State.ToString().ToLower()[0] + ")" + " + " + chemical.CalculateOxygen(CO) + "O2(g) = " + chemical.GetProduct(CO).ToString() + "CO(g) + " + chemical.GetProduct("water").ToString() + "H2O(l)";
                        infoImage = scrollingDescription(s, Color.Black);
                        i.Text = "\n\rINCOMP COMB: ";
                        i.Text += chemical.Damage.ToString() + "kJ/mol";
                        i.Position = infoImage[infoImage.Count - 1].Position + new Vector2(0, 10f);
                        infoImage.Add(i);
                        spxCombustion("CO", chemical.Level, true);
                        currentOxygen -= chemical.CalculateOxygen(CO);
                        damagedCombustion(chemical, "white", true); //add damage from combustion
                    }
                    else if (chemical.GetProduct("carbon") > 0)
                    {
                        string C = "carbon";
                        s = chemical.ChemicalFormula + "(" + chemical.State.ToString().ToLower()[0] + ")" + " + " + chemical.CalculateOxygen(C) + "O2(g) = " + chemical.GetProduct(C).ToString() + "C(s) + " + chemical.GetProduct("water").ToString() + "H2O(l)";
                        infoImage = scrollingDescription(s, Color.Black);
                        i.Text = "\n\rINCOMP COMB: ";
                        i.Text += chemical.Damage.ToString() + "kJ/mol";
                        i.Position = infoImage[infoImage.Count - 1].Position + new Vector2(0, 10f);
                        infoImage.Add(i);
                        spxCombustion("C", chemical.Level, true);
                        currentOxygen -= chemical.CalculateOxygen(C);
                        damagedCombustion(chemical, "black", true); //add damage from combustion
                    }
                    else
                        infoImage = scrollingDescription("Insufficient O2 for Combustion.", Color.Black);
                    break;
                case "Branching":
                    int isomerState = Math.Min(chemical.Isomers, chemical.CheckBranching()+1);
                    if (chemical.CheckMoveCount("Branching") + 1> chemical.Isomers)
                    {
                        infoImage = scrollingDescription(chemical.Name + " has no structural isomers with further branching.", Color.Black);
                    }
                    else
                    {
                        infoImage = scrollingDescription(chemical.Name + " isomer: " + isomerState.ToString() + " branch. [row] Boiling pt decr as London Dispersion Forces decr.", Color.Black);
                        player.ChemicalManager.LoadIsomer(chemical.Name, isomerState);
                        //increase defense rating of chemical
                        player.ChemicalManager.GetBattleChemical(selectedChemical).Defense -= .10f;
                    }
                    break;
                case "Free Radical Sub": //alkane to halogenoalkane
                    searchBackpack(selectedChemical + " begins Free Radical Substitution: Initiation. [row] Br2 -> 2Cl*" , reactant);
                    break;
                case "Addition Polymeriz": //alkene to alkane
                    searchBackpack(selectedChemical + "begins Addition Polymerization. [row] " + chemical.ChemicalFormula + "(" + chemical.State.ToString().ToLower()[0] + ") + NiH2(catalyst) + HEAT -> ", reactant);
                    break;
                case "Oxidation": //alcohol to alkanal, TO BE FIXED LATER
                    searchBackpack(selectedChemical + "begins Oxidation. [row] " + chemical.ChemicalFormula + "(l) + Chromate(catalyst) -> Alkanal + water", reactant);
                    break;
                case "SN2 Nucleophil Sub": 
                    //dependent on the catalyst available
                    searchBackpack(selectedChemical + "begins SN2 Nucleophilic Substitution. [row] " + chemical.ChemicalFormula + "(" + chemical.State.ToString().ToLower()[0] + ")" + " + NaOH(aq) + HEAT -> " + chemical.ChemicalFormula.Substring(0, chemical.ChemicalFormula.Length-2) + "OH(l) + NaBr(aq) [row] An Alcohol may join in the next turn!", reactant);
                    break;
                case "Extinguisher":
                    s = "Bromomethane interrupts chain reactions propogating combustion! Defense of all party members increase dramatically.";
                    infoImage = scrollingDescription(s, Color.Black);
                    environmentEffects.Add("Extinguisher");
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
            #region setup
            menu.Items.Clear();
            Chemical chemical = player.ChemicalManager.GetBattleChemical(selectedItem);
            Chemical tempChemical = new Chemical();
            string tempChemicalName = String.Empty;
            bool isMultiStepMove = false;
            string[] multiStepMoves = { "Free Radical Sub", "Addition Polymeriz", "Oxidation", "SN2 Nucleophil Sub" };
            #endregion

            foreach (string str in multiStepMoves)
                if (!isMultiStepMove && chemical.GetMoveHistory(1, str))
                {
                    isMultiStepMove = true;
                    player.ChemicalManager.GetBattleChemical(selectedItem).BattleMove = str;
                    player.ChemicalManager.GetBattleChemical(selectedItem).RecordMove(str);
                    infoImageClear();
                    switch (str)
                    {
                        case "Free Radical Sub": //alkane to halogenoalkane
                            if (chemical.CheckMoveCount(str) > 3)
                                exitMultiStep(ref isMultiStepMove, true);
                            else if (chemical.GetMoveHistory(3, str))
                            {
                                tempChemicalName = getTempName("Bromo" + chemical.Name.ToLower(), String.Empty, String.Empty);
                                string intermediate = chemical.ChemicalFormula.Substring(0, chemical.ChemicalFormula.Length - 1) + (chemical.GetElement(Element.H) - 1).ToString();
                                infoImage = scrollingDescription("Last step of Free Radical Substitution. [row] Termination: [row] Br* + Br* -> Br2 [row] Br* + *" + intermediate + " -> " + intermediate + "Br [row] " + tempChemicalName + " joins the battle!", Color.Black);  
                                player.ChemicalManager.LoadTempChemical(tempChemicalName, "Halogenoalkane");
                            }
                            else
                            {
                                infoImage = scrollingDescription(chemical.Name + " enters second step of Free Radical Substitution. [row] Propagation: [row] Br* " + chemical.ChemicalFormula + " -> HBr + *" + chemical.ChemicalFormula.Substring(0, chemical.ChemicalFormula.Length - 1) + (chemical.GetElement(Element.H) - 1).ToString(), Color.Black);
                            }
                            break;
                        case "Addition Polymeriz": //alkene to alkane
                            if (chemical.CheckMoveCount(str) > 2)
                                exitMultiStep(ref isMultiStepMove, true);
                            else
                            {
                                tempChemicalName = getTempName(chemical.Name, "ene", "ane");
                                player.ChemicalManager.LoadTempChemical(tempChemicalName, "Alkane");
                                infoImage = scrollingDescription(tempChemicalName + " joins the battle!", Color.Black);
                            }
                            break;
                        case "Oxidation": //alcohol to alkanal
                            if (chemical.CheckMoveCount(str) > 2)
                                exitMultiStep(ref isMultiStepMove, true);
                            else
                            {
                                tempChemicalName = getTempName(chemical.Name, "anol", "anal");
                                //player.ChemicalManager.LoadTempChemical(tempChemicalName, "Aldehyde");
                                infoImage = scrollingDescription(tempChemicalName + " joins the battle!", Color.Black);
                            }
                            break;
                        case "SN2 Nucleophil Sub":
                            if (chemical.CheckMoveCount(str) > 2)
                                exitMultiStep(ref isMultiStepMove, true);
                            else
                            {
                                tempChemicalName = chemical.Name.Replace("Bromo", String.Empty);
                                tempChemicalName = getTempName(tempChemicalName, "ane", "anol");
                                tempChemicalName = tempChemicalName[0].ToString().ToUpper() + tempChemicalName.Substring(1);
                                player.ChemicalManager.LoadTempChemical(tempChemicalName, "Alcohol");
                                infoImage = scrollingDescription(tempChemicalName + " joins the battle!", Color.Black);
                            }
                            break;
                    }
                    if (isMultiStepMove)
                    {
                        foreach (Image img in infoImage)
                            img.LoadContent();
                        MenuItem item = new MenuItem();
                        item.Image = new Image();
                        item.Image.Text = " ";
                        if (checkWinCondition(true))
                            item.LinkID = "Content/Load/Menu/EndBattleMenu.xml";
                        else
                            item.LinkID = checkPlayableChemicals();
                        menu.Items.Add(item);
                    }
                    else
                        infoImageClear();
                }
            if (checkWinCondition(true))
            {
                MenuItem item = new MenuItem();
                item.Image = new Image();
                item.Image.Text = " ";
                item.LinkID = "Content/Load/Menu/EndBattleMenu.xml";
                menu.Items.Add(item);
            } 
            else if (!isMultiStepMove)
            {
                generateMoveList(chemical);
                foreach (string move in moveList)
                {
                    MenuItem item = new MenuItem();
                    item.Image = new Image();
                    item.Image.Text = move; //Test Text
                    item.Image.TextColor = Color.Black;
                    item.Image.FontName = "Fonts/OCRAsmall";
                    item.LinkType = "Move";
                    item.LinkID = "Content/Load/Menu/DescriptionMenu.xml";
                    menu.Items.Add(item);
                }
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
            if (checkWinCondition(true))
            {
                menu.Items.Clear();
                MenuItem item = new MenuItem();
                item.Image = new Image();
                item.Image.Text = ".";
                item.LinkID = "Content/Load/Menu/EndBattleMenu.xml";
                menu.Items.Add(item);
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
        void displayLevellingInfo(Chemical chemical, Chemical c)
        {
            int gap1 = 8 - chemical.ChemicalFormula.Length;
            int gap2 = 8 - c.ChemicalFormula.Length;
            string g1, g2;
            g1 = g2 = String.Empty;
            for (int g = 0; g < gap1; g++)
                g1 += " ";
            for (int g = 0; g < gap2; g++)
                g2 += " ";
            c.SetOxygen(100);
            c.Combustion();
            string combustionFormula = c.ChemicalFormula + " + " + c.CalculateOxygen("carbondioxide") + "O2 -> " + c.GetProduct("carbondioxide").ToString() + "CO2 + " + c.GetProduct("water").ToString() + "H2O";
            generateMoveList(c);
            List<string> specialMove = new List<string>();
            string[] moveInfo = { "Free Radical Sub", "Addition Polymeriz", "Oxidation", "SN2 Nucleophil Sub" };
            bool safetyCheck = false;
            foreach (string info in moveInfo)
                if (moveList.Contains(info) && !safetyCheck)
                {
                    safetyCheck = true;
                    switch (info)
                    {
                        case "Free Radical Sub":
                            specialMove.Add("Free Radical Substitution");
                            specialMove.Add(removeAsterisk(c.Name) + " + Br2 + UV -> Bromo" + removeAsterisk(c.Name.ToLower()));
                            break;
                        case "Addition Polymeriz":
                            specialMove.Add("Addition Polymerisation:");
                            specialMove.Add(removeAsterisk(c.Name) + " + NiH2 + HEAT -> " + getTempName(c.Name, "ene", "ane"));
                            break;
                        case "Oxidation":
                            specialMove.Add("Oxidation");
                            specialMove.Add(removeAsterisk(c.Name) + " + Chromate + HEAT -> " + "Alkanal");
                            break;
                        case "SN2 Nucleophil Sub":
                            specialMove.Add("SN2 Nucleophilic Substitution");
                            specialMove.Add(removeAsterisk(c.Name) + " + NaOH + HEAT -> " + getTempName(getTempName(c.Name, "Bromo", String.Empty), "ane", "anol"));
                            break;
                    }
                }
            if (specialMove.Count > 1) //make new line if possible for special Move
                if (specialMove[1].Length > 25)
                {
                    int count = 0;
                    string[] words = specialMove[1].Split(' ');
                    specialMove[1] = String.Empty;
                    specialMove.Add(String.Empty);
                    foreach (string w in words)
                    {
                        count += w.Length + 1;
                        if (count < 25)
                            specialMove[1] += w + " ";
                        else
                            specialMove[2] += w + " ";
                    }
                }

            string[] levellingInfo = { c.Series.ToString().ToUpper() + " SERIES:", " ", chemical.ChemicalFormula + g1 + chemical.Name, c.ChemicalFormula + g2 + c.Name, " ", "Isomers:", c.Isomers.ToString(), "Complete Combustion:", combustionFormula, specialMove[0], specialMove[1] };

            int infoCount = 0;
            Vector2 dimensions = new Vector2(711, 141);
            foreach (string info in levellingInfo)
            {
                Image i = new Image();
                i.FontName = "Fonts/OCRAsmall";
                i.Text = info;
                i.Position = dimensions;
                i.Effects = "FadeEffect";
                i.IsActive = true;
                i.TextColor.R = 255;
                i.Alpha = 0;
                if (infoCount == 6)
                {
                    i.TextColor.G = 189;
                    i.TextColor.B = 0;
                }
                else if (infoCount == 8)
                {
                    i.TextColor.G = 235;
                    i.TextColor.B = 79;
                }
                else if (infoCount == 10)
                {
                    i.TextColor.G = 241;
                    i.TextColor.B = 160;
                }
                else
                    i.TextColor = Color.Black;
                infoCount++;
                dimensions.Y += 30f;
                levellingImage.Add(i);
            }
            if (specialMove.Count > 2)
            {
                Image i = new Image();
                i.FontName = "Fonts/OCRAsmall";
                i.Text = specialMove[2];
                i.Position = dimensions;
                i.Effects = "FadeEffect";
                i.IsActive = true;
                i.Alpha = 0;
                i.TextColor.R = 255;
                i.TextColor.G = 241;
                i.TextColor.B = 160;
                levellingImage.Add(i);
            }
        }
        void spxCombustion(string combustionType, int level, bool isPlayer)
        {
            if (level > 5 && combustionType == "CO2")
                spxImage.Add(new SPX(spxManager.AOEXml(combustionType + "_big", isPlayer)));
            else if (level > 3 && combustionType != "CO2")
                spxImage.Add(new SPX(spxManager.AOEXml(combustionType + "_big", isPlayer)));
            else if (level > 3 && combustionType == "CO2")
                spxImage.Add(new SPX(spxManager.AOEXml(combustionType + "_mid", isPlayer)));
            else
                spxImage.Add(new SPX(spxManager.AOEXml(combustionType + "_small", isPlayer)));
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
            for (int i = 0; i < (int)totalOxygen / 3; i++)
            {
                O2Filled.Draw(spriteBatch);
                O2Filled.Position.X += 44;
            }
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

        #region Initializations
        void initializeParties(string enemyXml, string enemyPartyXml)
        {
            XmlManager<Player> playerLoader = new XmlManager<Player>();
            player = playerLoader.Load("Content/Load/Gameplay/Player.xml");
            player.LoadContent();
            if (ScreenManager.Instance.Party.Count > 0)
            {
                //player.ChemicalManager.UnloadContent();
                player.ChemicalManager.LoadParty();
            }
            player.Image.Position = new Vector2(128, 360);
            player.Image.SpriteSheetEffect.CurrentFrame.Y = 7;
            player.Image.SpriteSheetEffect.SwitchFrame = 500;
            player.Image.IsActive = true;
            player.InitializeBattle();

            XmlManager<Character> characterLoader = new XmlManager<Character>();
            enemy = characterLoader.Load(enemyXml);
            enemy.LoadContent(enemyPartyXml);
            enemy.Image.Position = new Vector2(1064, 175);
            enemy.Image.SpriteSheetEffect.CurrentFrame.Y = 1;
            enemy.Image.IsActive = true;
            enemy.InitializeBattle();
        }
        void initializeRandomParty()
        {
            XmlManager<Player> playerLoader = new XmlManager<Player>();
            player = playerLoader.Load("Content/Load/Gameplay/Player.xml");
            player.LoadContent();
            if (ScreenManager.Instance.Party.Count > 0)
                player.ChemicalManager.LoadParty();
            player.Image.Position = new Vector2(128, 360);
            player.Image.SpriteSheetEffect.CurrentFrame.Y = 7;
            player.Image.SpriteSheetEffect.SwitchFrame = 500;
            player.Image.IsActive = true;
            player.InitializeBattle();

            XmlManager<Character> characterLoader = new XmlManager<Character>();
            enemy = characterLoader.Load("Content/Load/Gameplay/Anonymous.xml");
            ChemicalManager chemicalManager = new ChemicalManager();
            Random rnd = new Random();
            int rndPartySize = rnd.Next(Math.Max(1, player.ChemicalManager.PartySize() - 2), Math.Min(player.ChemicalManager.PartySize() + 2, 7));
            Series[] rndSeries = { Series.Alkane, Series.Alkene, Series.Alcohol, Series.Halogenoalkane};
            for (int i = 0; i < rndPartySize; i++)
            {
                int rndPartyLevel = 1; 
                for (int j = 0; j < player.ChemicalManager.chemicalName.Count; j++)
                    rndPartyLevel += player.ChemicalManager.GetChemical(player.ChemicalManager.chemicalName[j]).Level;
                rndPartyLevel = rnd.Next(1, Math.Min((int)(rndPartyLevel / player.ChemicalManager.chemicalName.Count),5));
                //Add random chemicals
                Chemical rndChemical = new Chemical();
                rndChemical.Level = rndPartyLevel;
                rndChemical.Series = rndSeries[rnd.Next(0, rndSeries.Length)];
                if (rndChemical.Level == 1 && rndChemical.Series == Series.Alkene)
                    rndChemical.Series = Series.Alkane;
                if (rndChemical.Series == Series.Halogenoalkane)
                    rndChemical.Halogen = Halogen.Bromo;
                rndChemical.InitializeFormationEnthalpyList();
                rndChemical.NameChemical();
                chemicalManager.ChemicalSource.Add("Content/Load/Chemical/" + rndChemical.Series.ToString() + "/" + rndChemical.Name + ".xml"); ;
            }
            enemy.ChemicalManager = chemicalManager;
            enemy.ChemicalManager.LoadContent();
            enemy.Image.Position = new Vector2(1064, 175);
            enemy.Image.SpriteSheetEffect.CurrentFrame.Y = 1;
            enemy.Image.IsActive = true;
            enemy.LoadContent();
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
                    if (player.ChemicalManager.BattlePartySize() < 1)
                    {
                        if (!checkWinCondition(true))
                        {
                            currentMenuID = "Content/Load/Menu/EndBattleMenu.xml";
                            isTransitioning = true;
                            if (isTransitioning)
                            {
                                menu.ID = currentMenuID;
                                isTransitioning = false;
                            }
                        }
                    }
                }
                else if (menu.Items[menu.ItemNumber].LinkType == "Screen")
                {
                    ScreenManager.Instance.ChangeScreens(menu.Items[menu.ItemNumber].LinkID);
                }
                else if (menu.Items[menu.ItemNumber].LinkType == "None")
                {/*no action*/}
                else if ((isLevelled == String.Empty) || (player.ChemicalManager.chemicalName.Contains(isLevelled) && !player.ChemicalManager.GetChemical(isLevelled).IsLevelling))
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
        void damagedCombustion(Chemical chemical, string color, bool isPlayer)
        {
            string s = String.Empty;
            if (isPlayer)
            {
                foreach (string damaged in getRandomChemicals(isPlayer, Math.Min(chemical.Level, enemy.ChemicalManager.BattlePartySize())))
                {
                    s += damaged + " takes " + calculateDamage(selectedChemical, damaged, chemical.Damage, true).ToString() + " kJ/mol of damage! [row] ";
                    spxImage.Add(new SPX("Content/Load/SPX/" + color + "1.xml", enemy.ChemicalManager.GetBattleChemical(damaged).Image.Position));
                }
                continueDescription(s, Color.SaddleBrown);
            }
            else
            {
                foreach (string damaged in getRandomChemicals(isPlayer, Math.Min(chemical.Level, player.ChemicalManager.BattlePartySize())))
                {
                    s += damaged + " takes " + calculateDamage(damaged, selectedEnemy, chemical.Damage, false).ToString() + " kJ/mol of damage! [row] ";
                    spxImage.Add(new SPX("Content/Load/SPX/" + color + "2.xml", player.ChemicalManager.GetBattleChemical(damaged).Image.Position));
                }
                continueDescription(s, Color.Black);
            }
        }
        int calculateDamage(string playerName, string enemyName, float damage, bool isPlayer)
        {
            float damageTaken = 0;
            if (isPlayer)
            {
                float damageReduction = 1 + (1 / (player.ChemicalManager.GetBattleChemical(playerName).MaxDamage / enemy.ChemicalManager.GetBattleChemical(enemyName).BoilingPoint));
                damageReduction *= enemy.ChemicalManager.GetBattleChemical(enemyName).Defense;
                damageTaken = (int)(damageReduction * damage);
                enemy.ChemicalManager.GetBattleChemical(enemyName).CurrentHealth += (int)damageTaken;
                if (enemy.ChemicalManager.GetBattleChemical(enemyName).CurrentHealth < 0)
                    enemy.ChemicalManager.GetBattleChemical(enemyName).CurrentHealth = 0;
                return (int)damageTaken;
            }
            else
            {
                float damageReduction = 1 + (1 / (enemy.ChemicalManager.GetBattleChemical(enemyName).MaxDamage / player.ChemicalManager.GetBattleChemical(playerName).BoilingPoint));
                damageReduction *= player.ChemicalManager.GetBattleChemical(playerName).Defense;
                damageTaken = (int)(damageReduction * damage);
                player.ChemicalManager.GetBattleChemical(playerName).CurrentHealth += (int)damageTaken;
                if (player.ChemicalManager.GetBattleChemical(playerName).CurrentHealth < 0)
                    player.ChemicalManager.GetBattleChemical(playerName).CurrentHealth = 0;
                return (int)damageTaken;
            }
        }
        List<string> getRandomChemicals(bool isPlayer, int num)
        {
            List<string> aliveChemicals = new List<string>();
            List<string> names = new List<string>();
            Random rnd = new Random();
            if (isPlayer)
            {
                foreach (string n in enemy.ChemicalManager.battleChemicalName)
                    if (!enemy.ChemicalManager.GetBattleChemical(n).IsDead)
                        aliveChemicals.Add(n);
                for (int i = 0; i < Math.Min(num, aliveChemicals.Count); i++)
                {
                    int randomIndex = rnd.Next(0, aliveChemicals.Count);
                    string n = enemy.ChemicalManager.battleChemicalName[randomIndex];
                    while (names.Contains(n))
                    {
                        randomIndex = rnd.Next(0, aliveChemicals.Count);
                        n = enemy.ChemicalManager.battleChemicalName[randomIndex];
                    }
                    names.Add(enemy.ChemicalManager.battleChemicalName[randomIndex]);
                }
            }
            else
            {
                foreach (string n in player.ChemicalManager.battleChemicalName)
                    if (!player.ChemicalManager.GetBattleChemical(n).IsDead)
                        aliveChemicals.Add(n);
                for (int i = 0; i < Math.Min(num, aliveChemicals.Count); i++)
                {
                    int randomIndex = rnd.Next(0, aliveChemicals.Count);
                    string n = player.ChemicalManager.battleChemicalName[randomIndex];
                    while (names.Contains(n))
                    {
                        randomIndex = rnd.Next(0, aliveChemicals.Count);
                        n = player.ChemicalManager.battleChemicalName[randomIndex];
                    }
                    names.Add(player.ChemicalManager.battleChemicalName[randomIndex]);
                }
            }
            return names;
        }
        string getRandomChemical(bool isPlayer)
        {
            string name = String.Empty;
            Random rnd = new Random();
            List<string> aliveChemicals = new List<string>();
            if (isPlayer)
            {
                foreach (string n in enemy.ChemicalManager.battleChemicalName)
                    if (!enemy.ChemicalManager.GetBattleChemical(n).IsDead)
                        aliveChemicals.Add(n);
                int randomIndex = rnd.Next(0, aliveChemicals.Count);
                name = enemy.ChemicalManager.battleChemicalName[randomIndex];
            }
            else
            {
                foreach (string n in player.ChemicalManager.battleChemicalName)
                    if (!player.ChemicalManager.GetBattleChemical(n).IsDead)
                        aliveChemicals.Add(n);
                int randomIndex = rnd.Next(0, aliveChemicals.Count);
                name = player.ChemicalManager.battleChemicalName[randomIndex];
            }
            return name;
        }
        void continueDescription(string description, Color color)
        {
            Image i = new Image();
            i.Text = " ";
            while (infoImage.Count % 3 != 0)
                infoImage.Add(i);
            if (infoImage.Count % 3 == 0)
                foreach (Image img in scrollingDescriptionContinued(description, color))
                    infoImage.Add(img);
        }
        string removeAsterisk(string name)
        {
            if (name.Contains("*"))
            {
                string[] str = name.Split('*');
                name = str[0];
            }
            return name;
        }
        string getTempName(string tempName, string delete, string replace)
        {
            if (delete != String.Empty)
                tempName = tempName.Replace(delete, replace);
            if (tempName.Contains("*"))
            {
                string[] tempStr = tempName.Split('*');
                tempName = tempStr[0];
            }
            return tempName;
        }
        void exitMultiStep(ref bool bMultiStep, bool isPlayer)
        {
            bMultiStep = false;
            if (isPlayer)
            {
                player.ChemicalManager.GetBattleChemical(selectedItem).BattleMove = String.Empty;
                player.ChemicalManager.GetBattleChemical(selectedItem).RecordMove(String.Empty);
            }
            else
            {
                enemy.ChemicalManager.GetBattleChemical(selectedEnemy).BattleMove = String.Empty;
                enemy.ChemicalManager.GetBattleChemical(selectedEnemy).RecordMove(String.Empty);
            }
        }
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
        #endregion
    }
}
