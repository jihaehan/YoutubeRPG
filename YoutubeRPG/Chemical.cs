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
        public State State;
        public Series Series;
        public Halogen Halogen;

        public float Health;        //sum of bond enthalpy
        public float Damage;        //determined by reaction
        public float Dodge;         //chance to dodge
        public float Defense;       //attack mofidier

        public int   Solubility;    //Environmental Factor

        Dictionary<Element, int> Elements;

        public Chemical()
        {
            Dimensions = new Vector2(128, 128);
            Name = NickName = String.Empty;
            Level = 1;
            Experience = 0;
            Reactivity = 0;
            State = State.Gas;
            Series = Series.Alkane;
            Halogen = Halogen.None;
            Health = Damage = Defense = Solubility = 0;
            Image.IsActive = true;
            Elements = new Dictionary<Element, int>();
        }
        public void LoadContent()
        {
            Image.LoadContent();
            NameChemical();
        }
        public void UnloadContent()
        {
            Image.UnloadContent();
        }
        public void Update(GameTime gameTime)
        {
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
        private void NameChemical()
        {
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
                    break;
                case Series.Alkene:
                    Name += "ene";
                    Elements.Add(Element.C, Level);
                    Elements.Add(Element.H, Level * 2);
                    Health = (Elements[Element.C] - 2) * BondEnthalpies.C_C + BondEnthalpies.C__C + Elements[Element.H] * BondEnthalpies.C_H; 
                    break;
                case Series.Alcohol:
                    Name += "anol";
                    Elements.Add(Element.C, Level);
                    Elements.Add(Element.H, Level * 2 + 2);
                    Elements.Add(Element.O, 1);
                    Health = (Elements[Element.C] - 1) * BondEnthalpies.C_C + BondEnthalpies.C_O + BondEnthalpies.O_H + (Elements[Element.H] - 1) * BondEnthalpies.C_H;
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
                    }
                    else if (Halogen == Halogen.Bromo)
                    {
                        Elements.Add(Element.Br, 1);
                        Health = BondEnthalpies.C_Br;
                    }
                    Health += (Elements[Element.C] - 1) * BondEnthalpies.C_C + (Elements[Element.H] - 1) * BondEnthalpies.C_H;
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
                    }
                    else if (Halogen == Halogen.Bromo)
                    {
                        Elements.Add(Element.Br, 2);
                        Health = BondEnthalpies.C_Br * 2;
                    }
                    Health += (Elements[Element.C] - 1) * BondEnthalpies.C_C + (Elements[Element.H] - 2) * BondEnthalpies.C_H;
                    break;
            }
        }
        private void InitializeChemical()
        {
            if (Series == Series.Alkane)
            {
                switch(Name.ToLower())
                {
                    case "methane":
                        Solubility = 0;
                        break;
                    case "ethane":
                        Solubility = 0;
                        break;
                    case "propane":
                        Solubility = 0;
                        break;
                    case "butane":
                        Solubility = 0;
                        break;
                    case "pentane":
                        Solubility = 0;
                        break;
                    case "hexane":
                        Solubility = 0;
                        break;
                    case "heptane":
                        Solubility = 0;
                        break;
                    case "octane":
                        Solubility = 0;
                        break;
                }
            }
            else if (Series == Series.Alkene) //Useful for breeding, not really for attacking
                switch(Name.ToLower())
                {
                    case "ethene":
                        break;
                    case "propene":
                        break;
                    case "butene":
                        break;
                    case "pentene":
                        break;
                    case "hexene":
                        break;
                    case "heptene":
                        break;
                    case "octene":
                        break;
                }
        }

    }
}
