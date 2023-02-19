using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnitedAdobeEditor.Components.Classes;
using UnitedAdobeEditor.Components.ColorChanger;
using UnitedAdobeEditor.Components.Enums;
using UnitedAdobeEditor.Components.Helpers;
using Formatting = Newtonsoft.Json.Formatting;

namespace UnitedAdobeEditor.Components.Scripts
{
    public class Settings
    {
        [JsonIgnore]
        public static Settings Instance;
        public Settings()
        {
            if(Instance != null)
            {
                return;
            }
            Instance = this;

            SelectedPaths = new PathList();
            new UIColorsData();

            Load();
        }

        public PathList SelectedPaths;

        public UIColorsData UIColorsData;
        public void Load()
        {
            try
            {
                string path = Files.SettingsPath;
                if (!File.Exists(path))
                {
                    return;
                }
                using (StreamReader reader = new StreamReader(path))
                {
                    var settings = JsonConvert.DeserializeObject<Settings>(reader.ReadToEnd());
                    if(settings == null)
                    {
                        return;
                    }
                    Settings.Instance = settings;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while loading settings : " + ex.Message);
            }
        }
        public void Save()
        {
            try
            {
                var list = JsonConvert.SerializeObject(Settings.Instance, Formatting.Indented);
                using (StreamWriter reader = new StreamWriter(Files.SettingsPath))
                {
                    reader.WriteLine(list);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while saving settings : " + ex.Message);
            }
        }
    }

}
