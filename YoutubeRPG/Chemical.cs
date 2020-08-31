using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
        public string Name;
        public string NickName;
        public int Level;
        public int Experience;
        public int Reactivity;
        public int Isomers;         //'Branching' defense skill
        public State State;         //determines order of attack
        public Series Series;
        public Halogen Halogen;

        public float Health;        //sum of bond enthalpy
        public float Mass;          //atomic mass of 1 molecule
        public float Damage;        //determined by reaction
        public float Dodge;         //chance to dodge, determined by difference in mass
        public float Defense;       //attack mofidier
        public float Accuracy;      //chance to explode

        public bool  Solubility;    //Environmental Factor

        Dictionary<Element, int> Elements;  
        Dictionary<string, int> Products;   //products from a reaction
        Dictionary<string, int> Reactants;
        List<float> FormationEnthalpy;

        public Chemical()
        {
            Dimensions = new Vector2(128, 128);
            Name = NickName = String.Empty;
            Level = 1;
            Experience = 0;
            Reactivity = 0;
            Isomers = 0;
            State = State.Gas;
            Series = Series.Alkane;
            Halogen = Halogen.None;
            Health = Mass = Damage = Defense = Dodge = Accuracy = 0;
            Solubility = false;
            Elements = new Dictionary<Element, int>();
            Products = new Dictionary<string, int>();
            Reactants = new Dictionary<string, int>();
            FormationEnthalpy = new List<float>();
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
        public void Update(GameTime gameTime, Player player)
        {
            Vector2 v = player.Velocity;
            v.Normalize();
            Image.Position = player.Image.Position - v * Dimensions.X;
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
        private void CompleteCombustion()
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
                    break;
                case Series.Alkene:
                    Name += "ene";
                    Elements.Add(Element.C, Level);
                    Elements.Add(Element.H, Level * 2);
                    Health = (Elements[Element.C] - 2) * BondEnthalpies.C_C + BondEnthalpies.C__C + Elements[Element.H] * BondEnthalpies.C_H; 
                    Mass = Elements[Element.C] * AtomicMass.C + Elements[Element.H] * AtomicMass.H;
                    Solubility = false;
                    break;
                case Series.Alcohol:
                    Name += "anol";
                    Elements.Add(Element.C, Level);
                    Elements.Add(Element.H, Level * 2 + 2);
                    Elements.Add(Element.O, 1);
                    Health = (Elements[Element.C] - 1) * BondEnthalpies.C_C + BondEnthalpies.C_O + BondEnthalpies.O_H + (Elements[Element.H] - 1) * BondEnthalpies.C_H;
                    Mass = Elements[Element.C] * AtomicMass.C + Elements[Element.H] * AtomicMass.H + Elements[Element.O] * AtomicMass.O;
                    Solubility = true;
                    break;
                case Series.Halogenoalkane:
                    Name.ToLower();
                    Name = Halogen.ToString() + Name +  "ane";
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
        }
        public void Combustion() //choose what combusion
        {
            string o = "oxygen";
            float CO2 = (float)((Elements[Element.H] / 2 + Elements[Element.C] * 2) / 2);
            float CO = (float)((Elements[Element.H] / 2 + Elements[Element.C]) / 2);
            float C = (float)(Elements[Element.H] / 4);

            if (Reactants[o] >= CO2)
            {
                CompleteCombustion();
                if (Products.ContainsKey("carbondioxide"))
                    Products["carbondioxide"] += Level;
                else
                    Products.Add("carbonmonoxide", Level);
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
