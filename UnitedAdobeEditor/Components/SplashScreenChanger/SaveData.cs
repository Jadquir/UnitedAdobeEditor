using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitedAdobeEditor.Components.ColorChanger;
using UnitedAdobeEditor.Components.Helpers;

namespace UnitedAdobeEditor.Components.SplashScreenChanger
{
    public class SaveData
    {
        public static SaveData Instance;
        public SaveData()
        {
            if(Instance != null)
            {
                return;
            }
            Instance = this;
            Load();
        }
        public enum SplashScreenColor
        {
            BackgroundColor,
            TextColor
        }
        public Dictionary<SplashScreenColor, ColorHolder> SplashScreenColors = new Dictionary<SplashScreenColor, ColorHolder>()
        {
            {SplashScreenColor.TextColor , new ColorHolder("TextForegroundColor",Color.FromArgb(142, 142, 142),1f) },
            {SplashScreenColor.BackgroundColor , new ColorHolder("TextBackgroundColor",Color.White,1f) }
        };


        public static void Load()
        {
            try
            {
                string path = Files.SplashScreenDataPath;
                if (!File.Exists(path))
                {
                    return;
                }
                using (StreamReader reader = new StreamReader(path))
                {
                    var loadedData = JsonConvert.DeserializeObject<SaveData>(reader.ReadToEnd());
                    if (loadedData == null)
                    {
                        return;
                    }
                    Instance.SplashScreenColors = loadedData.SplashScreenColors;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while loading SplashScreenData : " + ex.Message);
            }
        }
        public static void Save()
        {
            try
            {
                var list = JsonConvert.SerializeObject(Instance, Formatting.Indented);
                using (StreamWriter reader = new StreamWriter(Files.SplashScreenDataPath))
                {
                    reader.WriteLine(list);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while saving SplashScreenData : " + ex.Message);
            }
        }
    }
}
