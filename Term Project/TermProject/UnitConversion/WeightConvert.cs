using System;
using System.Collections.Generic;
using System.Text;

namespace TermProject.UnitConversion
{
    public static class WeightConvert
    {
        public static double ConvertPoundsToKG(double pounds)
        {
            return pounds * 0.453592;
        }

        public static double ConvertKGToPounds(double kg)
        {
            return kg * 2.20462;
        }

        public static List<string> Units
        {
            get
            {
                return new List<string> { "KG", "LB" };
            }
        }

        public static string GetUnit(string unitSystem)
        {
            switch (unitSystem.ToLower())
            {
                case "metric":
                    return "KG";
                case "imperial":
                    return "LB";
                default:
                    return "";
            }
        }

        public static double Convert(string toUnit, double value)
        {
            switch (toUnit.ToLower())
            {
                case "kg":
                    return ConvertPoundsToKG(value);
                case "lb":
                    return ConvertPoundsToKG(value);
                default:
                    return 0;
            }
        }

    }
}
