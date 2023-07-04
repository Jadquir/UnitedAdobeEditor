using PShop;
using PShop.PShop.CC2017;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitedAdobeEditor.Components.SplashScreenChanger.Photoshop
{
    internal class Func
    {
        public static void Pack(string inputPath, string outputPath, string indexFilename = "IconResources.idx")
        {
            Directory.CreateDirectory(Path.Combine(outputPath));
            IconResources ıconResources = IconResources.FromIndexFile(Path.Combine(inputPath, indexFilename));
            List<byte> list = new List<byte>();
            List<byte> list2 = new List<byte>();
            list.AddRange(new byte[4] { 102, 100, 114, 97 });
            list2.AddRange(new byte[4] { 102, 100, 114, 97 });
            int num = list.Count;
            int num2 = list2.Count;
            checked
            {
                foreach (ResourceIcon ıcon in ıconResources.Icons)
                {
                    int num3 = 0;
                    do
                    {
                        if (ıcon.Low.Pics[num3].Size != 0)
                        {
                            int num4 = num3;
                            string path = Path.Combine(inputPath, "Low", $"{ıcon.Key}_s{num4}.png");

                            using(MemoryStream memoryStream = new MemoryStream())
                            {
                                using (var stream = File.OpenRead(path))
                                {
                                    stream.CopyTo(memoryStream);
                                } 
                                byte[] array = memoryStream.ToArray();
                                list.AddRange(array);
                                ıcon.Low.Pics[num3].Offset = num;
                                ıcon.Low.Pics[num3].Size = array.Count();
                                num += array.Count();
                            }
                           
                        }
                        num3++;
                    }
                    while (num3 <= 7);
                    int num5 = 0;
                    do
                    {
                        if (ıcon.High.Pics[num5].Size != 0)
                        {
                            int num6 = num5;
                            string path2 = Path.Combine(inputPath, "High", $"{ıcon.Key}_s{num6}.png");
                            using(MemoryStream memoryStream2 = new MemoryStream())
                            {
                                using(var stream = File.OpenRead(path2))
                                {
                                    stream.CopyTo(memoryStream2);
                                } 
                                byte[] array2 = memoryStream2.ToArray();
                                list2.AddRange(array2);
                                ıcon.High.Pics[num5].Offset = num2;
                                ıcon.High.Pics[num5].Size = array2.Count();
                                num2 += array2.Count();
                            }
                           
                        }
                        num5++;
                    }
                    while (num5 <= 7);
                }
                using (var stream = File.OpenWrite(Path.Combine(outputPath, ıconResources.IndexFileName)))
                {
                    using (BinaryWriter binaryWriter = new BinaryWriter(stream))
                    {
                        binaryWriter.Write(ıconResources.ToIndexFile().ToArray());
                    }
                }
                using (var stream = File.OpenWrite(Path.Combine(outputPath, ıconResources.LowResolutionDataFile)))
                {
                    using (BinaryWriter binaryWriter2 = new BinaryWriter(stream))
                    {
                        binaryWriter2.Write(list.ToArray());
                    }
                }
                using (var stream = File.OpenWrite(Path.Combine(outputPath, ıconResources.HighResolutionDataFile)))
                {
                    using (BinaryWriter binaryWriter3 = new BinaryWriter(stream))
                    {
                        binaryWriter3.Write(list2.ToArray());
                    }
                }
            }
        }

        public static void Extract(string resourcesPath, string outputPath, string indexFilename = "IconResources.idx")
        {
            Directory.CreateDirectory(Path.Combine(outputPath));
            Directory.CreateDirectory(Path.Combine(outputPath, "Low"));
            Directory.CreateDirectory(Path.Combine(outputPath, "High"));
            string text = Path.Combine(resourcesPath, indexFilename);
            IconResources ıconResources = IconResources.FromIndexFile(text);
            checked
            {
                byte[] array = new byte[Math.Max(ıconResources.Icons.Max((ResourceIcon ico) => ico.Low.Pics.Max((PicInfo pic) => pic.Size)), ıconResources.Icons.Max((ResourceIcon ico) => ico.High.Pics.Max((PicInfo pic) => pic.Size))) - 1 + 1];
                byte[] src;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (var stream = File.OpenRead(Path.Combine(resourcesPath, ıconResources.LowResolutionDataFile)))
                    {
                        stream.CopyTo(memoryStream);
                    }
                    src = memoryStream.ToArray();
                }
                foreach (ResourceIcon ıcon in ıconResources.Icons)
                {
                    int num = 0;
                    do
                    {
                        PicInfo picInfo = ıcon.Low.Pics[num];
                        if (picInfo.Size != 0)
                        {
                            Buffer.BlockCopy(src, picInfo.Offset, array, 0, picInfo.Size);
                            using (FileStream output = File.OpenWrite(Path.Combine(outputPath, "Low", $"{ıcon.Key}_s{num}.png")))
                            {
                                using(BinaryWriter binaryWriter = new BinaryWriter(output))
                                { 
                                    binaryWriter.Write(array, 0, picInfo.Size);
                                }
                            }                           
                        }
                        num++;
                    }
                    while (num <= 7);
                }
                byte[] src2;
                using (MemoryStream memoryStream2 = new MemoryStream())
                {
                    using (FileStream stream = File.OpenRead(Path.Combine(resourcesPath, ıconResources.HighResolutionDataFile)))
                    {
                        stream.CopyTo(memoryStream2);
                    }
                    src2 = memoryStream2.ToArray();
                }
                foreach (ResourceIcon ıcon2 in ıconResources.Icons)
                {
                    int num2 = 0;
                    do
                    {
                        PicInfo picInfo2 = ıcon2.High.Pics[num2];
                        if (picInfo2.Size != 0)
                        {
                            Buffer.BlockCopy(src2, picInfo2.Offset, array, 0, picInfo2.Size);
                            using (FileStream output2 = File.OpenWrite(Path.Combine(outputPath, "High", $"{ıcon2.Key}_s{num2}.png")))
                            {
                                using (BinaryWriter binaryWriter2 = new BinaryWriter(output2))
                                {                                    
                                    binaryWriter2.Write(array, 0, picInfo2.Size);
                                }
                            }
                            
                        }
                        num2++;
                    }
                    while (num2 <= 7);
                }
                File.Copy(text, Path.Combine(outputPath, indexFilename));
            }
        }
    }
}
