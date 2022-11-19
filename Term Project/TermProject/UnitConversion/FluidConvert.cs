using System;
using System.Collections.Generic;
using System.Text;

namespace TermProject.UnitConversion
{
    public static class FluidConvert
    {
        public static double ConvertGallonsToLiters(double gallons)
        {
            return gallons * 3.78541;
        }

        public static double ConvertLitersToGallons(double l)
        {
            return l * 0.264172;
        }

        public static List<string> Units
        {
            get
            {
                return new List<string> { "L", "GAL" };
            }
        }

        public static string GetUnit(string unitSystem)
        {
            switch (unitSystem.ToLower())
            {
                case "metric":
                    return "L";
                case "imperial":
                    return "GAL";
                default:
                    return "";
            }
        }

        public static double Convert(string toUnit, double value)
        {
            switch (toUnit.ToLower())
            {
                case "l":
                case "metric":
                    return ConvertGallonsToLiters(value);
                case "gal":
                case "imperial":
                    return ConvertLitersToGallons(value);
                default:
                    return 0;
            }
        }
    }
}
