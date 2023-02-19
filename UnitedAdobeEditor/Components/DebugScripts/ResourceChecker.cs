using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using UnitedAdobeEditor.Components.Classes;
using Vestris.ResourceLib;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace UnitedAdobeEditor.Components.DebugScripts
{
    public class ResourceChecker
    {
        public static void check(string folder)
        {
            bool FileCheck(string name)
            {
                return name.ToLower().Contains("splash") ||
                                name.ToLower().Contains("background") || name.ToLower().Contains("russel");
            }
            List<FileInfo> GetFiles()
            {
                SearchOption searchOption = SearchOption.AllDirectories;
                List<FileInfo> values = new List<FileInfo>();
                string[] files = Directory.GetFiles(folder, "*.dll" , searchOption);
                string[] files2 = Directory.GetFiles(folder, "*.exe", searchOption);
                foreach (string file in files2)
                {
                    var info = new FileInfo(file);
                    if (FileCheck(file))
                    {
                        Debug.WriteLine(info.FullName);
                    }
                    values.Add(info);
                }
                foreach (string file in files)
                {
                    var info = new FileInfo(file);
                    if (FileCheck(file))
                    {
                        Debug.WriteLine(info.FullName);
                    }
                    values.Add(info);
                }
                return values;
            }

            SearchOption searchOption = SearchOption.AllDirectories;
            List<FileInfo> values = new List<FileInfo>();
            string[] files = Directory.GetFiles(folder, "*.txt", searchOption);
            string[] files2 = Directory.GetFiles(folder, "*.json", searchOption);
            foreach (var item in files)
            {
                if (FileCheck(File.ReadAllText(item)))
                {
                    Debug.WriteLine(item);
                }
            }
            foreach (var item in files2)
            {
                if (FileCheck(File.ReadAllText(item)))
                {
                    Debug.WriteLine(item);
                }
            }
            return;
            var list = GetFiles();  
            try
            {
                foreach (FileInfo file in list)
                {

                    using (ResourceInfo vi = new ResourceInfo())
                    {
                        try
                        {
                            vi.Load(file.FullName);
                            foreach (ResourceId id in vi.ResourceTypes)
                            {
                                try
                                {
                                    Console.WriteLine(id);
                                    foreach (StringResource rc in vi[Kernel32.ResourceTypes.RT_STRING])
                                    {
                                        foreach (var item in rc.Strings)
                                        {
                                            if (FileCheck(item.Value))
                                            {
                                                Debug.WriteLine(file.FullName + " : " + rc.Name.Name);
                                            }
                                        }
                                    }
                                    foreach (Resource resource in vi.Resources[id])
                                    {
                                        try
                                        {
                                            if (FileCheck(resource.Name.Name))
                                            {
                                                Debug.WriteLine(file.FullName + " : " + resource.Name.Name);
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }

                                    }
                                }
                                catch (Exception)
                                {
                                }

                            }

                        }
                        catch (Exception)
                        {
                        }
                          }
                }
            }
            catch (Exception ex)
            {
                MessageBoxJ.ShowOK("Error while modifying resources : " + ex.Message);
                return;
            }
            Debug.WriteLine("Resource check completed");
        }
    }
}
