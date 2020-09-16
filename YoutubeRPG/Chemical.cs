using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YoutubeRPG
{
    public enum Status
    {
        Heat,
        UV,
        Water,
    }

    public class Chemical
    {
        public Image Image;
        public Vector2 Dimensions;
        public Vector2 Velocity;
        public string Name;
        public string NickName;
        public string BattleTag;
        public string BattleMove;   
        public string ChemicalFormula;
        public int Level;
        public int Experience;
        public int Reactivity;
        public int Isomers;         //'Branching' defense skill
        public State State;         //determines order of attack
        public Series Series;
        public Halogen Halogen;

        public float CurrentHealth; //current health
        public float BoilingPoint;  //boiling point in K
        public float Health;        //sum of bond enthalpy
        public float Mass;          //atomic mass of 1 molecule
        public float Damage;        //determined by reaction
        public float BaseDamage;    //determined by formation enthalpy
        public float MaxDamage;     //determined by complete combustion
        public float Dodge;         //chance to dodge, determined by difference in mass
        public float Defense;       //attack mofidier
        public float Accuracy;      //chance to explode

        public bool Solubility;     //Environmental Factor
        public bool InBattle;       //Determines whether on the battlegrounds
        public bool TurnTaken;      //did the chemical take a turn?
        public Rectangle TagRectangle;

        Dictionary<Element, int> Elements;
        Dictionary<string, int> Products;   //products from a reaction
        Dictionary<string, float> Reactants;
        List<float> FormationEnthalpy;
        List<string> MoveHistory;

        #region Fields
        public void RecordMove(string move)
        {
            MoveHistory.Add(move);
        }
        public bool GetMoveHistory(int timeline, string move)
        {
            int count = 0;
            int iHistory = MoveHistory.Count - 1;
            if (MoveHistory.Count > timeline)
                for (int i = iHistory; i > iHistory - timeline; i--)
                {
                    if (MoveHistory[i] == move)
                        count++;
                }
            if (count == timeline)
                return true;
            else 
                return false;
        }
        public int CheckMoveCount(string move)
        {
            bool streak = true;
            int count = 0;
            if (MoveHistory.Count > 0)
                for (int i = MoveHistory.Count - 1; i >= 0 && streak; i--)
                {
                    if (MoveHistory[i] == move)
                        count++;
                    else
                        streak = false;
                }
            return count; 
        }
        public int GetElement(Element element)
        {
            if (Elements.ContainsKey(element))
                return Elements[element];
            else return 0;
        }
        public int GetProduct(string product)
        {
            if (Products.ContainsKey(product))
                return Products[product];
            else return 0;
        }
        public void AddReactant(string reactant, int num)
        {
            if (Reactants.ContainsKey(reactant))
                Reactants[reactant] += num;
            else
                Reactants.Add(reactant, num);
        }
        public void SetOxygen(float num)
        {
            if (Reactants.ContainsKey("oxygen"))
                Reactants["oxygen"] = num;
            else
                Reactants.Add("oxygen", num);
        }
        public float GetReactant(string reactant)
        {
            if (Reactants.ContainsKey(reactant))
                return Reactants[reactant];
            else return 0;
        }
        #endregion

        public Chemical()
        {
            Dimensions = new Vector2(128, 128);
            Velocity = Vector2.Zero;
            Name = NickName = BattleTag = BattleMove = ChemicalFormula = String.Empty;
            TagRectangle = new Rectangle();
            Level = 1;
            Experience = 0;
            Reactivity = 0;
            Isomers = 0;
            State = State.Gas;
            Series = Series.Alkane;
            Halogen = Halogen.None;
            Health = CurrentHealth = Mass = Damage = BaseDamage = MaxDamage = Defense = Dodge = Accuracy = BoilingPoint = 0;
            Solubility = false;
            InBattle = false;
            TurnTaken = false;
            Elements = new Dictionary<Element, int>();
            Products = new Dictionary<string, int>();
            Reactants = new Dictionary<string, float>();
            FormationEnthalpy = new List<float>();
            MoveHistory = new List<string>();
        }
        public void LoadContent()
        {
            Image.LoadContent();
            InitializeFormationEnthalpyList();
            NameChemical();
        }
        public void UnloadContent()
        {
            Image.UnloadContent();
        }
        public void Update(GameTime gameTime)
        {
            Image.IsActive = true;
            Image.Update(gameTime);
        }
        /// <summary>
        /// chemical walks towards a certain point, BattleScreen
        /// </summary>
        public void Update(GameTime gameTime, Vector2 position)
        {
            float distance = Vector2.Distance(Image.Position, position);
            
            if (distance > 3)
            {
                Velocity = position - Image.Position;
                Velocity.Normalize();
                Velocity *= 200f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Image.Position += Velocity;
            }
            Image.Update(gameTime);
            Image.IsActive = true;
        }
        /// <summary>
        /// chemical follows player, GameplayScreen
        /// </summary>
        public void Update(GameTime gameTime, ref Player player, Chemical chemical, int count)
        {
            Vector2 v, p;
            if (count == 0)
            {
                v = player.Velocity;
                p = player.Image.Position;
            }
            else
            {
                v = chemical.Velocity;
                p = chemical.Image.Position;
            }
            v.Normalize();
            bool right = v.X > 0 && p.X > (Image.Position.X);
            bool left = v.X < 0 && p.X < (Image.Position.X);
            bool down = v.Y > 0 && p.Y > (Image.Position.Y);
            bool up = v.Y < 0 && p.Y < (Image.Position.Y);

            Velocity = Vector2.Zero;
            int padding = 5;
            float distance = Vector2.Distance(p, Image.Position);
            if (distance > (Dimensions.X + Dimensions.Y) * 0.5f)
            {
                float halfDimensionsX = Dimensions.X / 1.9f;
                float halfDimensionsY = Dimensions.Y / 1.9f;
                halfDimensionsX = halfDimensionsY = (halfDimensionsX + halfDimensionsY) / 2f;
                if (right && up)
                {
                    if ((p.X > Image.Position.X + halfDimensionsX) && (p.Y < Image.Position.Y - halfDimensionsY))
                        Velocity = new Vector2(1, -1);
                    else if (p.X < Image.Position.X + halfDimensionsX)
                        Velocity = new Vector2(-1, -1);
                    else if (p.X > Image.Position.Y - halfDimensionsY)
                        Velocity = new Vector2(1, 1);
                }
                else if (right && down)
                {
                    if ((p.X > Image.Position.X + halfDimensionsX) && (p.Y > Image.Position.Y + halfDimensionsY))
                        Velocity = new Vector2(1, 1);
                    else if (p.X < Image.Position.X + halfDimensionsX)
                        Velocity = new Vector2(-1, 1);
                    else if (p.Y < Image.Position.Y + halfDimensionsY)
                        Velocity = new Vector2(1, -1);
                }
                else if (left && down)
                {
                    if ((p.X < Image.Position.X - halfDimensionsX) && (p.Y > Image.Position.Y + halfDimensionsY))
                        Velocity = new Vector2(-1, 1);
                    else if (p.X > Image.Position.X - halfDimensionsX)
                        Velocity = new Vector2(1, 1);
                    else if (p.Y < Image.Position.Y + halfDimensionsY)
                        Velocity = new Vector2(-1, -1);
                }
                else if (left && up)
                {
                    if ((p.X < Image.Position.X - halfDimensionsX) && (p.Y < Image.Position.Y - halfDimensionsY))
                        Velocity = new Vector2(-1, -1);
                    else if (p.X > Image.Position.X - halfDimensionsX)
                        Velocity = new Vector2(1, -1);
                    else if (p.Y > Image.Position.Y - halfDimensionsY)
                        Velocity = new Vector2(-1, 1);
                }
                else if (right || left)
                {
                    if (p.Y > (Image.Position.Y + padding))
                        Velocity = new Vector2(0, 1);
                    else if (p.Y < (Image.Position.Y - padding))
                        Velocity = new Vector2(0, -1);
                    else if (right && p.X > (Image.Position.X + Dimensions.X + padding))
                        Velocity = new Vector2(1, 0);
                    else if (left)
                        Velocity = new Vector2(-1, 0);
                }
                else if (down || up)
                {
                    if (p.X > (Image.Position.X + padding))
                        Velocity = new Vector2(1, 0);
                    else if (p.X < (Image.Position.X - padding))
                        Velocity = new Vector2(-1, 0);
                    else if (down)
                        Velocity = new Vector2(0, 1);
                    else if (count > 0)
                    {
                        if (up && p.Y < (Image.Position.Y - Dimensions.Y - padding * 2))
                            Velocity = new Vector2(0, -1);
                    }
                    else if (count == 0 && up)
                            Velocity = new Vector2(0, -1);
              
                }
                Image.IsActive = true;
            }

            if (Velocity != Vector2.Zero)
            {
                Velocity.Normalize();
                if (distance > (Dimensions.X + Dimensions.Y)*0.77f)
                   Velocity *= (player.MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds * 1.5f);
                else
                   Velocity *= (player.MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                Image.Position += Velocity;
                if (player.Velocity.X < 0)
                    Image.SpriteSheetEffect.CurrentFrame.Y = 0;
                else if (player.Velocity.X > 0)
                    Image.SpriteSheetEffect.CurrentFrame.Y = 1;
            }
            Image.Update(gameTime);

        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Image.Draw(spriteBatch);
        }
        private void InitializeFormationEnthalpyList()
        {
            FormationEnthalpy.Clear();
            switch (Series)
            {
                case Series.Alkane:
                    FormationEnthalpy.Add(FormationEnthalpies.methane);
                    FormationEnthalpy.Add(FormationEnthalpies.ethane);
                    FormationEnthalpy.Add(FormationEnthalpies.propane);
                    FormationEnthalpy.Add(FormationEnthalpies.butane);
                    FormationEnthalpy.Add(FormationEnthalpies.pentane);
                    FormationEnthalpy.Add(FormationEnthalpies.hexane);
                    FormationEnthalpy.Add(FormationEnthalpies.heptane);
                    FormationEnthalpy.Add(FormationEnthalpies.octane);
                    break;
                case Series.Alkene:
                    FormationEnthalpy.Add(0);
                    FormationEnthalpy.Add(FormationEnthalpies.ethene);
                    FormationEnthalpy.Add(FormationEnthalpies.propene);
                    FormationEnthalpy.Add(FormationEnthalpies.but_1_ene);
                    FormationEnthalpy.Add(FormationEnthalpies.pentene);
                    FormationEnthalpy.Add(FormationEnthalpies.hexene);
                    FormationEnthalpy.Add(FormationEnthalpies.heptene);
                    FormationEnthalpy.Add(FormationEnthalpies.octene);
                    break;
                case Series.Alcohol:
                    FormationEnthalpy.Add(FormationEnthalpies.methanol);
                    FormationEnthalpy.Add(FormationEnthalpies.ethanol);
                    FormationEnthalpy.Add(FormationEnthalpies.propanol);
                    FormationEnthalpy.Add(FormationEnthalpies.butanol);
                    FormationEnthalpy.Add(FormationEnthalpies.pentanol);
                    FormationEnthalpy.Add(FormationEnthalpies.hexanol);
                    FormationEnthalpy.Add(FormationEnthalpies.heptanol);
                    FormationEnthalpy.Add(FormationEnthalpies.octanol);
                    break;
                case Series.Halogenoalkane:
                    FormationEnthalpy.Add(FormationEnthalpies.bromomethane);
                    FormationEnthalpy.Add(FormationEnthalpies.bromoethane);
                    FormationEnthalpy.Add(FormationEnthalpies.bromopropane);
                    FormationEnthalpy.Add(FormationEnthalpies.bromobutane);
                    FormationEnthalpy.Add(FormationEnthalpies.bromopentane);
                    FormationEnthalpy.Add(FormationEnthalpies.bromohexane);
                    FormationEnthalpy.Add(FormationEnthalpies.bromoheptane);
                    FormationEnthalpy.Add(FormationEnthalpies.bromooctane);
                    break;
            }
        }
        public float CompleteCombustion()
        {
            switch (Series)
            {
                case Series.Alkane:
                    switch (Level)
                    {
                        case 1:
                            Damage = CombustionEnthalpies.methane;
                            break;
                        case 2:
                            Damage = CombustionEnthalpies.ethane;
                            break;
                        case 3:
                            Damage = CombustionEnthalpies.propane;
                            break;
                        case 4:
                            Damage = CombustionEnthalpies.butane;
                            break;
                        case 5:
                            Damage = CombustionEnthalpies.pentane;
                            break;
                        case 6:
                            Damage = CombustionEnthalpies.hexane;
                            break;
                        case 7:
                            Damage = CombustionEnthalpies.heptane;
                            break;
                        case 8:
                            Damage = CombustionEnthalpies.octane;
                            break;
                    }
                    break;
                case Series.Alkene:
                    switch (Level)
                    {
                        case 2:
                            Damage = CombustionEnthalpies.ethene;
                            break;
                        case 3:
                            Damage = CombustionEnthalpies.propene;
                            break;
                        case 4:
                            Damage = CombustionEnthalpies.butene;
                            break;
                        case 5:
                            Damage = CombustionEnthalpies.pentene;
                            break;
                        case 6:
                            Damage = CombustionEnthalpies.hexene;
                            break;
                        case 7:
                            Damage = CombustionEnthalpies.heptene;
                            break;
                        case 8:
                            Damage = CombustionEnthalpies.octene;
                            break;
                    }
                    break;
                case Series.Alcohol:
                    switch (Level)
                    {
                        case 1:
                            Damage = CombustionEnthalpies.methanol;
                            break;
                        case 2:
                            Damage = CombustionEnthalpies.ethanol;
                            break;
                        case 3:
                            Damage = CombustionEnthalpies.propanol;
                            break;
                        case 4:
                            Damage = CombustionEnthalpies.butanol;
                            break;
                        case 5:
                            Damage = CombustionEnthalpies.pentanol;
                            break;
                        case 6:
                            Damage = CombustionEnthalpies.hexanol;
                            break;
                        case 7:
                            Damage = CombustionEnthalpies.heptanol;
                            break;
                        case 8:
                            Damage = CombustionEnthalpies.octanol;
                            break;
                    }
                    break;
                case Series.Halogenoalkane:
                    switch (Level)
                    {
                        case 1:
                            Damage = 0;
                            break;
                        case 2:
                            Damage = CombustionEnthalpies.bromoethane;
                            break;
                        case 3:
                            Damage = CombustionEnthalpies.bromopropane;
                            break;
                        case 4:
                            Damage = CombustionEnthalpies.bromobutane;
                            break;
                        case 5:
                            Damage = CombustionEnthalpies.bromopentane;
                            break;
                        case 6:
                            Damage = CombustionEnthalpies.bromohexane;
                            break;
                        case 7:
                            Damage = CombustionEnthalpies.bromoheptane;
                            break;
                        case 8:
                            Damage = CombustionEnthalpies.bromooctane;
                            break;
                    }
                    break;
            }
            return Damage;
        }
        public void NameChemical()
        {
            //necessary 
            switch (Level)
            {
                case 1:
                    Name = "Meth";
                    break;
                case 2:
                    Name = "Eth";
                    break;
                case 3:
                    Name = "Prop";
                    break;
                case 4:
                    Name = "But";
                    break;
                case 5:
                    Name = "Pent";
                    break;
                case 6:
                    Name = "Hex";
                    break;
                case 7:
                    Name = "Hept";
                    break;
                case 8:
                    Name = "Oct";
                    break;
                case 9:
                    Name = "Non";
                    break;
                case 10:
                    Name = "Dec";
                    break;
                default:
                    Name = String.Empty;
                    break;
            }
            switch (Series)
            {
                case Series.Alkane:
                    Name += "ane";
                    Elements.Add(Element.C, Level);
                    Elements.Add(Element.H, Level * 2 + 2);
                    Health = (Elements[Element.C] - 1) * BondEnthalpies.C_C + Elements[Element.H] * BondEnthalpies.C_H;
                    Mass = Elements[Element.C] * AtomicMass.C + Elements[Element.H] * AtomicMass.H;
                    Solubility = false;
                    if (Level == 1)
                        ChemicalFormula = "CH4";
                    else
                        ChemicalFormula = "C" + Level.ToString() + "H" + Elements[Element.H].ToString();
                    break;
                case Series.Alkene:
                    Name += "ene";
                    Elements.Add(Element.C, Level);
                    Elements.Add(Element.H, Level * 2);
                    Health = (Elements[Element.C] - 2) * BondEnthalpies.C_C + BondEnthalpies.C__C + Elements[Element.H] * BondEnthalpies.C_H; 
                    Mass = Elements[Element.C] * AtomicMass.C + Elements[Element.H] * AtomicMass.H;
                    Solubility = false;
                    ChemicalFormula = "C" + Level.ToString() + "H" + Elements[Element.H].ToString();
                    break;
                case Series.Alcohol:
                    Name += "anol";
                    Elements.Add(Element.C, Level);
                    Elements.Add(Element.H, Level * 2 + 2);
                    Elements.Add(Element.O, 1);
                    Health = (Elements[Element.C] - 1) * BondEnthalpies.C_C + BondEnthalpies.C_O + BondEnthalpies.O_H + (Elements[Element.H] - 1) * BondEnthalpies.C_H;
                    Mass = Elements[Element.C] * AtomicMass.C + Elements[Element.H] * AtomicMass.H + Elements[Element.O] * AtomicMass.O;
                    Solubility = true;
                    if (Level == 1)
                        ChemicalFormula = "CH3OH";
                    else
                        ChemicalFormula = "C" + Level.ToString() + "H" + (Elements[Element.H] - 1).ToString() + "OH";
                    break;
                case Series.Halogenoalkane:
                    Name = Halogen.ToString() + Name.ToLower() +  "ane";
                    Elements.Add(Element.C, Level);
                    Elements.Add(Element.H, Level * 2 + 1);
                    if (Halogen == Halogen.Chloro)
                    {
                        Elements.Add(Element.Cl, 1);
                        Health = BondEnthalpies.C_Cl;
                        Mass = Elements[Element.Cl] * AtomicMass.Cl;
                    }
                    else if (Halogen == Halogen.Bromo)
                    {
                        Elements.Add(Element.Br, 1);
                        Health = BondEnthalpies.C_Br;
                        Mass = Elements[Element.Br] * AtomicMass.Br;
                        if (Level == 1)
                            ChemicalFormula = "CH3Br";
                        else
                            ChemicalFormula = "C" + Level.ToString() + "H" + Elements[Element.H].ToString() + "Br";
                    }
                    Health += (Elements[Element.C] - 1) * BondEnthalpies.C_C + (Elements[Element.H] - 1) * BondEnthalpies.C_H;
                    Mass += Elements[Element.C] * AtomicMass.C + Elements[Element.H];
                    if (Level > 3)
                        Solubility = false;
                    else
                        Solubility = true;
                    break;
                case Series.Twohaloalkane:
                    Name.ToLower();
                    Name = "Di" + Halogen.ToString() + Name + "ane";
                    Elements.Add(Element.C, Level);
                    Elements.Add(Element.H, Level * 2);
                    if (Halogen == Halogen.Chloro)
                    {
                        Elements.Add(Element.Cl, 2);
                        Health = BondEnthalpies.C_Cl * 2;
                        Mass = Elements[Element.Cl] * AtomicMass.Cl;
                    }
                    else if (Halogen == Halogen.Bromo)
                    {
                        Elements.Add(Element.Br, 2);
                        Health = BondEnthalpies.C_Br * 2;
                        Mass = Elements[Element.Br] * AtomicMass.Br;
                    }
                    Health += (Elements[Element.C] - 1) * BondEnthalpies.C_C + (Elements[Element.H] - 2) * BondEnthalpies.C_H;
                    //Solubility???
                    break;
            }
            CurrentHealth = Health;
            BaseDamage = FormationEnthalpy[Level];
            MaxDamage = CompleteCombustion();
            if (Level > 3)
                Isomers = Level - 3;
        }
        public float CalculateOxygen(string product)
        {
            float O2 = 0;
            switch (product)
            {
                case "carbondioxide":
                    O2 = (float)((Elements[Element.H] / 2 + Elements[Element.C] * 2) / 2);
                    break;
                case "carbonmonoxide":
                    O2 = (float)((Elements[Element.H] / 2 + Elements[Element.C]) / 2);
                    break;
                case "carbon":
                    O2 = (float)(Elements[Element.H] / 4);
                    break;
            }
            return O2;
        }
        public void Combustion() //choose what combustion
        {
            string o = "oxygen";
            float CO2 = (float)((Elements[Element.H] / 2 + Elements[Element.C] * 2) / 2);
            float CO = (float)((Elements[Element.H] / 2 + Elements[Element.C]) / 2);
            float C = (float)(Elements[Element.H] / 4);

            if (Reactants[o] >= CO2)
            {
                Damage = CompleteCombustion();
                if (Products.ContainsKey("carbondioxide"))
                    Products["carbondioxide"] += Level;
                else
                    Products.Add("carbondioxide", Level);
            }
            else if (Reactants[o] >= CO)
            {
                Damage = (float)(Elements[Element.H] / 2 * FormationEnthalpies.water + Elements[Element.C] * FormationEnthalpies.carbonmonoxide) - FormationEnthalpy[Level];
                if (Products.ContainsKey("carbonmonoxide"))
                    Products["carbonmonoxide"] += Level;
                else 
                    Products.Add("carbonmonoxide", Level);
            }
            else if (Reactants[o] >= C)
            {
                Damage = (float)(Elements[Element.H] / 2 * FormationEnthalpies.water) - FormationEnthalpy[Level];
                if (Products.ContainsKey("carbon"))
                    Products["carbon"] += Level;
                else
                    Products.Add("carbon", Level);
            }
            else
                Damage = 0;

            if (Products.ContainsKey("water"))
                Products["water"] += Level;
            else Products.Add("water", Level);
        }
    }
}
