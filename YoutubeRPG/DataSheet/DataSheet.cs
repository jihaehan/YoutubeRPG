using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeRPG
{
    public struct BondEnthalpies
    {
        public static int C_C = 346;
        public static int C__C = 614;
        public static int C___C = 839;
        public static int C_H = 414;
        public static int O_H = 463;
        public static int C_Cl = 324;
        public static int C_Br = 285;
        public static int C_O = 358;
    }
    public struct AtomicMass
    {
        public static float C = 12.01f;
        public static float H = 1.01f;
        public static float O = 16.0f;
        public static float Br = 79.9f;
    }
    /// <summary>
    /// Experimental Molar Enthalpy of Complete Combustion under 298K and 1.00 * 10^5 Pa (kJ/mol)
    /// </summary>
    public struct CombustionEnthalpies 
    {
        public static int methane = -891;
        public static int ethane = -1561;
        public static int propane = -2219;
        public static int butane = -2878;
        public static int pentane = -3509;
        public static int hexane = -4163;
        public static int heptane = -4817;
        public static int octane = -5470;

        public static int ethene = -1411;
        public static int propene = -2058;
        public static int butene = -2717;
        public static int pentene = -3349;
        public static int hexene = -4001;
        public static int heptene = -4658;
        public static int octene = -5313;

        public static int methanol = -729;
        public static int ethanol = -1367;
        public static int propanol = -2021;
        public static int butanol = -2676;
        public static int pentanol = -3331;
        public static int hexanol = -3984;
        public static int heptanol = -4638;
        public static int octanol = -5286;

        public static int bromoethane = -1284;
        public static int bromopropane = -1890;
        public static int bromobutane = -2716;
        public static int bromopentane = -3369;
        public static int bromohexane = -4025;
        public static int bromoheptane = -4680;
        public static int bromooctane = -5333;

        public static int hydrogen = -286;
        public static int chloroethane = -1413;
        public static int iodoethane = -1463;

    }
    /// <summary>
    /// Enthalpy of Formation (kJ/mol)
    /// </summary>
    public struct FormationEnthalpies
    {
        public static float methane   = -74f;
        public static float ethane    = -84f;
        public static float propane   = -105f;
        public static float butane    = -126f;
        public static float pentane   = -173f;
        public static float hexane    = -199f;
        public static float heptane   = -224f;
        public static float octane    = -250f;

        public static float ethene          = 54f;
        public static float propene         = 20f;
        public static float but_1_ene       = 0.10f;
        public static float cis_but_2_ene   = -7.0f;
        public static float trans_but_2_ene = -11.0f;
        public static float pentene         = -46.3f;
        public static float hexene          = -73.0f;
        public static float heptene         = -97.5f;
        public static float octene          = -122f;

        public static float bromomethane    = -36;
        public static float bromoethane     = -90;
        public static float bromopropane    = -124;
        public static float bromobutane     = -148;
        public static float bromopentane    = -170;
        public static float bromohexane     = -194;
        public static float bromoheptane    = -219;
        public static float bromooctane     = -245;

        public static float methanol = -239f;
        public static float ethanol = -279f;
        public static float propanol = -303f;
        public static float butanol = -328f;
        public static float pentanol = -352f;
        public static float hexanol = -377.5f;
        public static float heptanol = -403f;
        public static float octanol = -435f;

        public static float water = -285.5f;
        public static float steam = -241.8f;
        public static float carbondioxide = -393.5f;
        public static float carbonmonoxide = -110.5f;
        public static float hydrogenbromide = -36.5f;


        public static float hydrogenchloride = -92.3f;
        public static float hydrogenfluoride = -273.3f;
        public static float hydrogeniodide = 26.5f;
        //public static float iodomethane     = -14;
        //public static float chloroethane    = -137;
    }
    /// <summary>
    /// Boiling point in Kelvin (K)
    /// Note that room temp is at 298K
    /// </summary>
    public struct BoilingPoint
    {
        public static float methane = 111.7f;
        public static float ethane = 187.6f;
        public static float propane = 231f;
        public static float butane = 274f;
        public static float pentane = 309f;
        public static float hexane = 342f;
        public static float heptane = 371f;
        public static float octane = 399f;
               
        public static float ethene = 169.4f;
        public static float propene = 225.4f;
        public static float butene = 266.8f;
        public static float pentene = 303.2f;
        public static float hexene = 337f;
        public static float heptene = 366.8f;
        public static float octene = 394.2f;

        public static float methanol = 337.8f;
        public static float ethanol = 351.5f;
        public static float propanol = 370.2f;
        public static float butanol = 380.9f;
        public static float pentanol = 411.2f;
        public static float hexanol = 430.2f;
        public static float heptanol = 448.2f;
        public static float octanol = 468.2f;

        public static float bromomethane = 277f;
        public static float bromoethane = 311.2f;
        public static float bromopropane = 344.2f;
        public static float bromobutane = 375.2f;
        public static float bromopentane = 402.2f;
        public static float bromohexane = 427.2f;
        public static float bromoheptane = 452.2f;
        public static float bromooctane = 474.2f;
    }
    /// <summary>
    /// Chemical Details
    /// </summary>
    public enum Series
    {
        Alkane,
        Halogenoalkane,
        Twohaloalkane,
        Alkene,
        Alcohol,
    }
    public enum State
    {
        Gas,
        Liquid,
        Solid,
    }
    public enum Element
    {
        C,
        H,
        O,
        N,
        Cl,
        Br,
    }
    public enum Halogen
    {
        None,
        Chloro,
        Bromo,
    }
    public enum FunctionalGroup
    {
        Alkyl,
        Alkenyl,
        Hydroxyl,
        Ether,
        Carbonyl,
        Carboxyl,
        Halo,
        Amino,
        Carboxamide,
        Ester,
        Nitrile,
        Phenyl,
    }

}
