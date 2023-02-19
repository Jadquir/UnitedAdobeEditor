using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using UnitedAdobeEditor.Components.Classes;
using UnitedAdobeEditor.Components.Enums;
using UnitedAdobeEditor.Components.Helpers;
using Wpf.Ui.Controls;

namespace UnitedAdobeEditor.Components.ColorChanger
{
    public class ColorChanger
    {
        private static string[] UIColorsFile;
        private static string PhotoshopFolder { get { return CurrentOperation.SelectedPath.AdobeFolder; } }
        private static string BackupFolder { get { return Files.AdobeAppBackupFolder; } }
        private static string UIColorFilePath
        {
            get
            {
                return CurrentOperation.SelectedPath.UIColorsPath;
            }
        }
        private static bool IsCelfile
        {
            get
            {
                return new FileInfo(UIColorFilePath).Extension == "cel";
            }
        }
        private static string GetValue(ColorHolder colorHolder)
        {
            if (IsCelfile)
            {
                return colorHolder.GetCelValue();
            }
            return colorHolder.GetTxtValue();
        }
        private static string GetValue(string name)
        {
            ColorHolder value = UIColorsData.Instance.MainColor;
            if (UIColorsData.Instance.IsAdvancedActivated)
            {
                value = UIColorsData.GetColorValue(name);
            }
            return GetValue(value);
        }

        public static async Task Change()
        {
            await Misc.RunBackground((s, e) => { ChangeFunc(); });
        }
        public static async void ChangeFunc()
        {
                (bool, string) path = GetUIColorsFile.GetPath(PhotoshopFolder);
                if (!path.Item1)
                {
                    MessageBoxJ.ShowOK(path.Item2);
                    return;
                }
                try
                {
                    if (!(await Backup()))
                    {
                        return;
                    }
                    string[] obj = (IsCelfile ? ColorValues.CelValues : ColorValues.TxtValues);
                    string text = obj[^1];
                    string[] array = obj;
                    foreach (string text2 in array)
                    {
                        await ChangeInUIColors(text2, GetValue(text2), text == text2, false);
                        if (IsCelfile && text2.Contains("Start"))
                        {
                            string text3 = text2.Clone() as string;
                            text3 = text3.Replace("Start", "End");
                            await ChangeInUIColors(text3, GetValue(text3), text == text2, false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBoxJ.ShowOK("Error while changing UIColors : " + ex.Message);
                    return;
                }
                MessageBoxJ.ShowOK("UIColors successfully changed!");
        }
        private static async Task<bool> Backup()
        {
            if (!CreateBackup())
            {
                bool continueChanging = false;
                await MessageBoxJ.ShowYesNo
                    ("Application couldn't create the backup.\nPlease backup your UIColors.txt/cel.\nDo you want to continue changing color?",
                     (s, e) => { continueChanging = true; }
                    );

                if (!continueChanging)
                {
                    return false;
                }
            }
            return true;
        }
        public static async Task ChangeInUIColors(string valueToChange, ColorHolder colorHolder, bool SaveAfterEdit = false, bool backup = true)
        {
            string newValue = GetValue(colorHolder);
            await ChangeInUIColors(valueToChange,newValue,SaveAfterEdit,backup,true);
        }

        public static async Task ChangeInUIColors(string valueToChange, string newValue, bool SaveAfterEdit = false,bool backup = true,bool allThemes = false)
        {
            if (backup)
            {
               if(!(await Backup()))
               {
                   return;
               }
            }
            if (UIColorsFile == null)
            {
                ReadUIColors();
            }
            int num = 0;
            try
            {
                for (int i = 0; i < UIColorsFile.Length; i++)
                {
                    if (UIColorsFile[i].Trim() == valueToChange + ":")
                    {
                        num = i;
                        break;
                    }
                }
            }
            catch (Exception)
            {
            }
            try
            {
                string text = newValue;
                if (allThemes)
                {
                    UIColorsFile[num + 2] = text + ",";
                    UIColorsFile[num + 3] = text + ",";
                    UIColorsFile[num + 4] = text + ",";
                    UIColorsFile[num + 5] = text;
                }
                else
                {
                    DesiredTheme desiredTheme = UIColorsData.Instance.desiredTheme;
                    
                    if (desiredTheme != DesiredTheme.Dark)
                    {
                        text += ",";
                    }
                    int num2 = (int)desiredTheme;
                    UIColorsFile[num + num2] = text;
                }
            }
            catch (Exception)
            {
            }
            if (SaveAfterEdit)
            {
                SaveUIColors();
            }
        }
        public static void ReadUIColors()
        {
            Debug.WriteLine("Reading UI Colors");
            UIColorsFile = File.ReadAllLines(UIColorFilePath);
        }
        public static void SaveUIColors()
        {
            Debug.WriteLine("Saving UI Colors");
            if (File.Exists(UIColorFilePath))
                File.Delete(UIColorFilePath);
            File.WriteAllLines(UIColorFilePath, UIColorsFile);
        }

        public static bool CreateBackup()
        {
            if (File.Exists(BackupFolder + "\\UIColors.txt") || File.Exists(BackupFolder + "\\UIColors.cel"))
            {
                return true;
            }
            try
            {
                if (IsCelfile)
                {
                    File.Copy(UIColorFilePath, BackupFolder + "\\UIColors.cel", overwrite: false);
                }
                else
                {
                    File.Copy(UIColorFilePath, BackupFolder + "\\UIColors.txt", overwrite: false);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static (bool,string) Restore()
        {
            Debug
                .WriteLine("BackupFolder : " + BackupFolder);
            if (!Directory.Exists(BackupFolder))
            {
                return (false, "Application couldn't find the backup folder.");
            }

            string UIColors = null;
            if(File.Exists(BackupFolder + "\\UIColors.txt"))
            {
                UIColors = BackupFolder + "\\UIColors.txt";
            }
            else if(File.Exists(BackupFolder + "\\UIColors.cel"))
            {
                UIColors = BackupFolder + "\\UIColors.cel"; 
            }
            if (UIColors == null)
            {
                return (false, "Application couldn't find the backup file.");
            }
            try
            {
                if (File.Exists(UIColorFilePath))
                {
                    File.Delete(UIColorFilePath);
                }
                if (IsCelfile)
                {
                    File.Copy(BackupFolder + "\\UIColors.cel", UIColorFilePath, overwrite: true);
                }
                else
                {
                    File.Copy(BackupFolder + "\\UIColors.txt", UIColorFilePath, overwrite: true);
                }
                return (true, "Done");
            }
            catch (Exception ex)
            {
                return (false, "Error : " + ex.Message);
            }
        }
    }
}
