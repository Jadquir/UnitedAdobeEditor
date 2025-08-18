using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;

namespace UnitedAdobeEditor.Components.ColorChanger
{
    public enum UIColorsTheme
    {
        Light,
        LightGray,
        DarkGray, 
        Dark
    }
    public class UIColorSet
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public Dictionary<string, Dictionary<UIColorsTheme, double[]>> Colors { get; set; } = new();
    }
    public static class UIColorParser
    {
        public static readonly UIColorsTheme[] ThemeOrder = { UIColorsTheme.Light, UIColorsTheme.LightGray, UIColorsTheme.DarkGray, UIColorsTheme.Dark };

        public static UIColorSet Deserialize(string text)
        {
            var set = new UIColorSet();

            // Extract Name
            var nameMatch = Regex.Match(text, @"Name:\s*""([^""]+)""");
            if (nameMatch.Success) set.Name = nameMatch.Groups[1].Value;

            // Extract Version
            var verMatch = Regex.Match(text, @"Version:\s*([0-9.]+)");
            if (verMatch.Success) set.Version = verMatch.Groups[1].Value;

            // Extract Colors block
            var colorsBlockMatch = Regex.Match(text, @"Colors:\s*\{([\s\S]*?)\}", RegexOptions.Multiline);
            if (!colorsBlockMatch.Success)
                throw new Exception("Could not find Colors section.");

            string colorsBlock = colorsBlockMatch.Groups[1].Value;

            // Match each color entry
            var colorMatches = Regex.Matches(colorsBlock, @"([A-Za-z0-9_]+)\s*:\s*\[\s*\[([\s\S]*?)\]\s*\]", RegexOptions.Multiline);

            foreach (Match m in colorMatches)
            {
                string colorName = m.Groups[1].Value;
                string innerBlock = m.Groups[2].Value; // This will contain all 4 RGBA arrays

                var themeColors = new Dictionary<UIColorsTheme, double[]>();
                var rgbaMatches = Regex.Matches($"[{innerBlock}]", @"\[\s*(\d{1,3}\s*,\s*\d{1,3}\s*,\s*\d{1,3}\s*,\s*[\d.]+\s*)\]");

                for (int i = 0; i < rgbaMatches.Count && i < ThemeOrder.Length; i++)
                {
                    string rgbaText = rgbaMatches[i].Groups[1].Value.Trim();
                    var rgba = rgbaText.Split(',')
                                       .Select(p => double.Parse(p.Trim(), CultureInfo.InvariantCulture))
                                       .ToArray();
                    themeColors[ThemeOrder[i]] = rgba;
                }

                set.Colors[colorName] = themeColors;
            }

            return set;
        }


        private static double[] ParseRGBA(string rgbaText)
        {
            // Detect CEL hex format: "0xFFFF, 0x0000, 0x8080, 1.0"
            if (rgbaText.Contains("0x"))
            {
                var parts = rgbaText.Split(',').Select(p => p.Trim()).ToArray();
                if (parts.Length != 4) throw new FormatException("Invalid CEL RGBA format");

                byte r = Convert.ToByte(parts[0].Substring(2, 2), 16);
                byte g = Convert.ToByte(parts[1].Substring(2, 2), 16);
                byte b = Convert.ToByte(parts[2].Substring(2, 2), 16);
                double a = double.Parse(parts[3], CultureInfo.InvariantCulture);

                return new double[] { r, g, b, a };
            }
            else
            {
                // Normal format: "255, 255, 255, 1.0"
                var parts = rgbaText.Split(',').Select(p => double.Parse(p.Trim(), CultureInfo.InvariantCulture)).ToArray();
                if (parts.Length != 4) throw new FormatException("Invalid standard RGBA format");
                return parts;
            }
        }

        public static string Serialize(UIColorSet set, bool celFormat = false)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine($"Name: \"{set.Name}\",");
            sb.AppendLine("");
            sb.AppendLine($"Version: {set.Version},");
            sb.AppendLine("");
            sb.AppendLine("Colors: ");
            sb.AppendLine("");
            sb.AppendLine("\t{");

            foreach (var kvp in set.Colors)
            {
                sb.AppendLine($"\t{kvp.Key}:");
                sb.AppendLine("\t\t[");

                for (int i = 0; i < ThemeOrder.Length; i++)
                {
                    var rgba = kvp.Value[ThemeOrder[i]];
                    string line = celFormat
                        ? ToCelFormat(rgba)
                        : $"[ {rgba[0]}, {rgba[1]}, {rgba[2]}, {rgba[3].ToString("F1").Replace(",",".")} ]";

                    if (i < ThemeOrder.Length - 1) line += ",";
                    sb.AppendLine("\t\t" + line);
                }

                sb.AppendLine("\t\t],\n");
            }

            sb.AppendLine("\t}");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string ToCelFormat(double[] rgba)
        {
            string r = ((byte)rgba[0]).ToString("X2");
            string g = ((byte)rgba[1]).ToString("X2");
            string b = ((byte)rgba[2]).ToString("X2");
            return $"[ 0x{r}{r}, 0x{g}{g}, 0x{b}{b}, {rgba[3].ToString("F1").Replace(",", ".")} ]";
        }
    }

}

