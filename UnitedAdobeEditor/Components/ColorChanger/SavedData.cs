using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitedAdobeEditor.Components.Enums;
using UnitedAdobeEditor.Components.Helpers;
using UnitedAdobeEditor.Components.Scripts;

namespace UnitedAdobeEditor.Components.ColorChanger
{
    public class UIColorsData
    {
        public static UIColorsData Instance { get; private set; }
        public UIColorsData()
        {
            if(Instance != null)
            {
                return;
            }
            Instance = this;
            List<ColorHolder> tempList = new List<ColorHolder>();

            Random rnd = new Random();
            foreach (var item in Components.ColorChanger.ColorValues.TxtValues)
            {
                tempList.Add(new ColorHolder(item, Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256)), 1.0f));
            }
            foreach (var item in Components.ColorChanger.ColorValues.CelValues)
            {
                tempList.Add(new ColorHolder(item, Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256)), 1.0f));
            }
            ColorValues = tempList;
            Load();
            ColorValues = ColorValues.GroupBy(x => x.Name).Select(x => x.First()).ToList();
        }

        public ColorHolder MainColor { get; set; } = new ColorHolder("Main",Color.White,1f); 
        public bool IsAdvancedActivated { get; set; } = false;
        public DesiredTheme desiredTheme { get; set; } = DesiredTheme.Dark;
        public List<ColorHolder> ColorValues { get; set; }

        public static ColorHolder? GetColorValue(string name)
        {
            return Instance.ColorValues.Find(v => v.Name == name);
        }
        public static int? GetColorValueIndex(string name)
        {
            return Instance.ColorValues.IndexOf(GetColorValue(name));
        }
        public static void EditColorHolder(ColorHolder editedcolorHolder)
        {
            var indexPossible = GetColorValueIndex(editedcolorHolder.Name);
            if (!indexPossible.HasValue)
            {
                return;
            }
            var index = indexPossible.Value;
            Instance.ColorValues[index].Color = editedcolorHolder.Color;
            Instance.ColorValues[index].Opacity = editedcolorHolder.Opacity;

        }



        public static void Load()
        {
            try
            {
                string path = Files.UIColorsDataPath;
                if (!File.Exists(path))
                {
                    return;
                }
                using (StreamReader reader = new StreamReader(path))
                {
                    var loadedData = JsonConvert.DeserializeObject<UIColorsData>(reader.ReadToEnd());
                    if (loadedData == null)
                    {
                        return;
                    }
                    UIColorsData.Instance.ColorValues = loadedData.ColorValues;
                    UIColorsData.Instance.IsAdvancedActivated = loadedData.IsAdvancedActivated;
                    UIColorsData.Instance.MainColor = loadedData.MainColor;
                    UIColorsData.Instance.desiredTheme = loadedData.desiredTheme;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while loading UIColorsData : " + ex.Message);
            }
        }
        public static void Save()
        {
            try
            {
                var list = JsonConvert.SerializeObject(UIColorsData.Instance, Formatting.Indented);
                using (StreamWriter reader = new StreamWriter(Files.UIColorsDataPath))
                {
                    reader.WriteLine(list);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while saving UIColorsData : " + ex.Message);
            }
        }
    }
}
