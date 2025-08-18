using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnitedAdobeEditor.Components.Enums;

namespace UnitedAdobeEditor.Components.Classes
{
    public class CurrentOperation
    {
        internal static bool isRunStat;
        internal static string runStatId;

        public static AdobeType AppType { get; set; }
        public static SelectedPath SelectedPath { get; set; }

        public static OperationType operationType { get; set; }

        public static bool IsConfigActivated { get; set; }
        public static Operation Operation { get; set; }

        public static bool IsPhotoshopLaterThan2024()
        {
            if (SelectedPath == null) return false;
            var year = ExtractYear(SelectedPath.AdobeFolder);
            if (year == -1)
                return false;
            return year >= 2024;
        }
        public static int GetAppYear()
        {
            if (SelectedPath == null) return -1;
            var year = ExtractYear(SelectedPath.AdobeFolder);
            return year;
        }
        static int ExtractYear(string text)
        {
            // Using regular expression to find the year
            Regex regex = new Regex(@"\b\d{4}\b");
            Match match = regex.Match(text);
            if (match.Success)
            {
                int year;
                if (int.TryParse(match.Value, out year))
                {
                    return year;
                }
            }
            // Return a default value if no year found or parsing fails
            return -1;
        }
    }
    public class Operation
    {
        public AdobeType AppType { get; set; }

        public OperationType operationType { get; set; }
        public Image SplashScreen { get; set; }
    }
}
