using PShop;
using PShop.PShop.CC2017;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace UnitedAdobeEditor.Components.SplashScreenChanger.Photoshop
{
  
    public class IconResourceManager
    {
        private IconResources _iconResources;
        static readonly string indexFilename = "IconResources.idx";

        public IconResourceManager(string indexFilePath) : this(
            Func.New.FromIndexFile(Path.Combine(indexFilePath, indexFilename))
            )
        {
        }
        private IconResourceManager(IconResources resources) 
        {
            _iconResources = resources;
        }

        public void Extract(string resourcesPath, string outputPath)
        {
            Debug.WriteLine("Extracting...");
            Directory.CreateDirectory(outputPath);
            Directory.CreateDirectory(Path.Combine(outputPath, "Low"));
            Directory.CreateDirectory(Path.Combine(outputPath, "High"));

            IconResources iconResources = _iconResources;
            int extracted = 0;
            foreach (ResourceIcon icon in iconResources.Icons)
            {
                foreach (var resolution in new[] { icon.Low, icon.High })
                {
                    for (int i = 0; i < resolution.Pics.Count; i++)
                    {
                        var picInfo = resolution.Pics[i];

                        if (picInfo.Size != 0)
                        {
                            byte[] buffer = new byte[picInfo.Size];
                            string resolutionPath = resolution == icon.Low ? iconResources.LowResolutionDataFile : iconResources.HighResolutionDataFile;

                            using (FileStream inputStream = File.OpenRead(Path.Combine(resourcesPath, resolutionPath)))
                            {
                                inputStream.Seek(picInfo.Offset, SeekOrigin.Begin);
                                inputStream.Read(buffer, 0, buffer.Length);
                            }

                            string outputPathFolder = resolution == icon.Low ? "Low" : "High";
                            string outputPathFile = Path.Combine(outputPath, outputPathFolder, $"{icon.Key}_s{i}.png");

                            File.WriteAllBytes(outputPathFile, buffer);
                            extracted++;
                        }
                    }
                }
            }
            //Debug.WriteLine("Extracted : " + extracted);
            File.Copy(Path.Combine(resourcesPath, indexFilename), Path.Combine(outputPath, indexFilename), true);
            Debug.WriteLine("Extract Complete.");
        }

        public void Pack(string inputPath, string outputPath)
        {
            Debug.WriteLine("Packing...");
            Directory.CreateDirectory(outputPath);
            IconResources iconResources = _iconResources;
            List<byte> lowResData = new List<byte>() { 0x66, 0x64, 0x72, 0x61 };
            List<byte> highResData = new List<byte>() { 0x66, 0x64, 0x72, 0x61 };
            int lowResOffset = 4;
            int highResOffset = 4;

            int packed = 0;
            foreach (ResourceIcon icon in iconResources.Icons)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (icon.Low.Pics[i].Size != 0)
                    {
                        string path = Path.Combine(inputPath, "Low", $"{icon.Key}_s{i}.png");
                        byte[] imageData = File.ReadAllBytes(path);
                        lowResData.AddRange(imageData);
                        icon.Low.Pics[i].Offset = lowResOffset;
                        icon.Low.Pics[i].Size = imageData.Length;
                        lowResOffset += imageData.Length;
                    }

                    if (icon.High.Pics[i].Size != 0)
                    {
                        string path = Path.Combine(inputPath, "High", $"{icon.Key}_s{i}.png");
                        byte[] imageData = File.ReadAllBytes(path);
                        highResData.AddRange(imageData);
                        icon.High.Pics[i].Offset = highResOffset;
                        icon.High.Pics[i].Size = imageData.Length;
                        highResOffset += imageData.Length;
                    }
                }
            }
            File.WriteAllBytes(Path.Combine(outputPath, iconResources.LowResolutionDataFile), lowResData.ToArray());
            File.WriteAllBytes(Path.Combine(outputPath, iconResources.HighResolutionDataFile), highResData.ToArray());
            File.WriteAllBytes(Path.Combine(outputPath, iconResources.IndexFileName), iconResources.ToIndexFile().ToArray());

            Debug.WriteLine("Packing Complete.");
        }

    }
    internal static class Func
    {
        public static ResourceIcon TestResourceIcon(byte[] buffer)
        {
            string Key = Encoding.ASCII.GetString(buffer.Take(48).ToArray()).TrimEnd(new char[1]);
            List<int> list = new List<int>();

            string Key1 = Encoding.ASCII.GetString(buffer.Take(348).ToArray()).TrimEnd(new char[1]);
            double num = 0.0;
            do
            {
                list.Add(BitConverter.ToInt32(buffer, Convert.ToInt32(48.0 + num * 4.0)));
                num += 1.0;
            }
            while (num <= 79.0);
            var Low = new IconData
            {
                Width = list[0],
                Height = list[4],
                X = list[8],
                Y = list[12]
            };
            int num2 = 0;
            checked
            {
                do
                {
                    Low.Pics.Add(new PicInfo
                    {
                        Offset = list[16 + num2],
                        Size = list[48 + num2]
                    });
                    num2++;
                }
                while (num2 <= 7);
                var High = new IconData
                {
                    Width = list[1],
                    Height = list[5],
                    X = list[9],
                    Y = list[13]
                };
                int num3 = 0;
                do
                {
                    High.Pics.Add(new PicInfo
                    {
                        Offset = list[24 + num3],
                        Size = list[56 + num3]
                    });
                    num3++;
                }
                while (num3 <= 7);
            }
            return new ResourceIcon(buffer);
        }
        internal static class New
        {
            public static IconResources FromIndexFile(string indexFilePath)
            {
                return FromIndexFile(File.OpenRead(indexFilePath), Path.GetFileName(indexFilePath));
            }

            public static IconResources FromIndexFile(Stream indexFileStream, string indexFilename = "IconResources.idx")
            {
                IconResources ıconResources = new IconResources
                {
                    IndexFileName = indexFilename
                };
                byte[] indexFileBytes;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    indexFileStream.CopyTo(memoryStream);
                    indexFileBytes = memoryStream.ToArray();
                }

                int num = 0;
                checked
                {
                    int num2 = indexFileBytes.Count() - 1;
                    for (int i = 0; i <= num2; i++)
                    {
                        if (indexFileBytes[i] == 10)
                        {
                            ıconResources.MarkerIndexes.Add(i);
                            num++;
                            if (num > 4)
                            {
                                break;
                            }
                        }
                    }

                    ıconResources.Name = Encoding.ASCII.GetString(indexFileBytes.Take(ıconResources.MarkerIndexes[0]).ToArray()).TrimEnd(new char[1]);
                    ıconResources.LowResolutionDataFile = Encoding.ASCII.GetString(indexFileBytes.Skip(ıconResources.MarkerIndexes[0] + 1).Take(ıconResources.MarkerIndexes[1] - ıconResources.MarkerIndexes[0] - 1).ToArray()).TrimEnd(new char[1]);
                    ıconResources.HighResolutionDataFile = Encoding.ASCII.GetString(indexFileBytes.Skip(ıconResources.MarkerIndexes[1] + 1).Take(ıconResources.MarkerIndexes[2] - ıconResources.MarkerIndexes[1] - 1).ToArray()).TrimEnd(new char[1]);
                    ıconResources.XLowResolutionDataFile = Encoding.ASCII.GetString(indexFileBytes.Skip(ıconResources.MarkerIndexes[2] + 1).Take(ıconResources.MarkerIndexes[3] - ıconResources.MarkerIndexes[2] - 1).ToArray()).TrimEnd(new char[1]);
                    ıconResources.XHighResolutionDataFile = Encoding.ASCII.GetString(indexFileBytes.Skip(ıconResources.MarkerIndexes[3] + 1).Take(ıconResources.MarkerIndexes[4] - ıconResources.MarkerIndexes[3] - 1).ToArray()).TrimEnd(new char[1]);

                    int startIndex = ıconResources.MarkerIndexes[4] + 1;
                    int num4 = 368;
                    double num5 = (double)(indexFileBytes.Count() - startIndex) / (double)num4;
                    byte[] array2 = new byte[num4 - 1 + 1];
                    double num6 = num5 - 1.0;


                    for (double num7 = 0.0; num7 <= num6; num7 += 1.0)
                    {
                        Buffer.BlockCopy(indexFileBytes, Convert.ToInt32((double)startIndex + num7 * (double)num4), array2, 0, num4);
                        var bufferIcon = TestResourceIcon(array2);
                        LogIcon(bufferIcon, ıconResources.Icons.Count + 1);
                        ıconResources.Icons.Add(bufferIcon);
                    }

                    return ıconResources;
                }
            }
            public static void LogIcon(ResourceIcon icon, int index)
            {
                return;
                Debug.WriteLine(index.ToString().PadRight(4) + " | " + "Icon: " + 
                    icon.Key.PadRight("TextToImage_clio_v2_reference_Color_and_Light_1".Length) + " | "
                    + $"Low: ({LogIcon(icon.Low)})" + $"| High: ({LogIcon(icon.High)})");
            }
            public static string LogIcon(IconData iconData)
            {
                return "";
                return $"X: " +
                    $"{iconData.X.ToString().PadRight(3)} Y:" +
                    $" {iconData.Y.ToString().PadRight(3)} " +
                    $"| Width: {iconData.Width.ToString().PadRight(4)} " +
                    $"Height: {iconData.Height.ToString().PadRight(4)} | Count: {iconData.Pics.Count.ToString().PadRight(4)}";
            }
            public static void Pack(string inputPath, string outputPath, string indexFilename = "IconResources.idx")
            {
                Directory.CreateDirectory(outputPath);
                IconResources iconResources = FromIndexFile(Path.Combine(inputPath, indexFilename));
                List<byte> lowResData = new List<byte>() { 0x66, 0x64, 0x72, 0x61 };
                List<byte> highResData = new List<byte>() { 0x66, 0x64, 0x72, 0x61 };
                int lowResOffset = 4;
                int highResOffset = 4;

                int packed = 0;
                foreach (ResourceIcon icon in iconResources.Icons)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (icon.Low.Pics[i].Size != 0)
                        {
                            string path = Path.Combine(inputPath, "Low", $"{icon.Key}_s{i}.png");
                            byte[] imageData = File.ReadAllBytes(path);
                            lowResData.AddRange(imageData);
                            icon.Low.Pics[i].Offset = lowResOffset;
                            icon.Low.Pics[i].Size = imageData.Length;
                            lowResOffset += imageData.Length;
                        }

                        if (icon.High.Pics[i].Size != 0)
                        {
                            string path = Path.Combine(inputPath, "High", $"{icon.Key}_s{i}.png");
                            byte[] imageData = File.ReadAllBytes(path);
                            highResData.AddRange(imageData);
                            icon.High.Pics[i].Offset = highResOffset;
                            icon.High.Pics[i].Size = imageData.Length;
                            highResOffset += imageData.Length;
                        }
                    }
                }
                Debug.WriteLine("Packed " + packed);
                File.WriteAllBytes(Path.Combine(outputPath, iconResources.LowResolutionDataFile), lowResData.ToArray());
                File.WriteAllBytes(Path.Combine(outputPath, iconResources.HighResolutionDataFile), highResData.ToArray());
                File.WriteAllBytes(Path.Combine(outputPath, iconResources.IndexFileName), iconResources.ToIndexFile().ToArray());
            }

            public static IconResources Extract(string resourcesPath, string outputPath, string indexFilename = "IconResources.idx")
            {
                Directory.CreateDirectory(outputPath);
                Directory.CreateDirectory(Path.Combine(outputPath, "Low"));
                Directory.CreateDirectory(Path.Combine(outputPath, "High"));

                IconResources iconResources = FromIndexFile(Path.Combine(resourcesPath, indexFilename));
                int extracted = 0;
                foreach (ResourceIcon icon in iconResources.Icons)
                {
                    foreach (var resolution in new[] { icon.Low, icon.High })
                    {
                        for (int i = 0; i < resolution.Pics.Count; i++)
                        {
                            var picInfo = resolution.Pics[i];

                            if (picInfo.Size != 0)
                            {
                                byte[] buffer = new byte[picInfo.Size];
                                string resolutionPath = resolution == icon.Low ? iconResources.LowResolutionDataFile : iconResources.HighResolutionDataFile;

                                using (FileStream inputStream = File.OpenRead(Path.Combine(resourcesPath, resolutionPath)))
                                {
                                    inputStream.Seek(picInfo.Offset, SeekOrigin.Begin);
                                    inputStream.Read(buffer, 0, buffer.Length);
                                }

                                string outputPathFolder = resolution == icon.Low ? "Low" : "High";
                                string outputPathFile = Path.Combine(outputPath, outputPathFolder, $"{icon.Key}_s{i}.png");

                                File.WriteAllBytes(outputPathFile, buffer);
                                extracted++;
                            }
                        }
                    }
                }
                Debug.WriteLine("Extracted : " + extracted);
                File.Copy(Path.Combine(resourcesPath, indexFilename), Path.Combine(outputPath, indexFilename), true);
                return iconResources;
            }

        }

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
                    for (int i = 0; i < 8; i++)
                    {
                        if (ıcon.Low.Pics[i].Size != 0)
                        {
                            string path = Path.Combine(inputPath, "Low", $"{ıcon.Key}_s{i}.png");

                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                using (var stream = File.OpenRead(path))
                                {
                                    stream.CopyTo(memoryStream);
                                }
                                byte[] array = memoryStream.ToArray();
                                list.AddRange(array);
                                ıcon.Low.Pics[i].Offset = num;
                                ıcon.Low.Pics[i].Size = array.Count();
                                num += array.Count();
                            }

                        }

                        if (ıcon.High.Pics[i].Size != 0)
                        {
                            string path2 = Path.Combine(inputPath, "High", $"{ıcon.Key}_s{i}.png");
                            using (MemoryStream memoryStream2 = new MemoryStream())
                            {
                                using (var stream = File.OpenRead(path2))
                                {
                                    stream.CopyTo(memoryStream2);
                                }
                                byte[] array2 = memoryStream2.ToArray();
                                list2.AddRange(array2);
                                ıcon.High.Pics[i].Offset = num2;
                                ıcon.High.Pics[i].Size = array2.Count();
                                num2 += array2.Count();
                            }

                        }
                    }              
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

        public static void ExtractNew(string resourcesPath, string outputPath, string indexFilename = "IconResources.idx")
        {
            Directory.CreateDirectory(Path.Combine(outputPath));
            Directory.CreateDirectory(Path.Combine(outputPath, "Low"));
            Directory.CreateDirectory(Path.Combine(outputPath, "High"));

            IconResources iconResources = IconResources.FromIndexFile(Path.Combine(resourcesPath, indexFilename));

            foreach (ResourceIcon icon in iconResources.Icons)
            {
                foreach (var resolution in new[] { icon.Low, icon.High })
                {
                    for (int i = 0; i < resolution.Pics.Count; i++)
                    {
                        var picInfo = resolution.Pics[i];

                        if (picInfo.Size != 0)
                        {
                            byte[] buffer = new byte[picInfo.Size];
                            string outputPathFolder = resolution == icon.Low ? "Low" : "High";
                            string outputPathFile = Path.Combine(outputPath, outputPathFolder, $"{icon.Key}_s{i}.png");

                            using (FileStream inputStream = File.OpenRead(Path.Combine(resourcesPath, resolution == icon.Low ? iconResources.LowResolutionDataFile : iconResources.HighResolutionDataFile)))
                            {
                                inputStream.Seek(picInfo.Offset, SeekOrigin.Begin);
                                inputStream.Read(buffer, 0, buffer.Length);
                            }

                            using (FileStream outputStream = File.OpenWrite(outputPathFile))
                            {
                                using (BinaryWriter writer = new BinaryWriter(outputStream))
                                {
                                    writer.Write(buffer);
                                }
                            }
                        }
                    }
                }
            }

            File.Copy(Path.Combine(resourcesPath, indexFilename), Path.Combine(outputPath, indexFilename));
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
                byte[] array = new byte[
                    Math.Max(
                        ıconResources.Icons.Max(
                            (ResourceIcon ico) => ico.Low.Pics.Max((PicInfo pic) => pic.Size)
                            )
                        , 
                        ıconResources.Icons.Max(
                            (ResourceIcon ico) => ico.High.Pics.Max((PicInfo pic) => pic.Size)
                            ))
                    - 1 + 1];
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
                    for (int i = 0; i < 8; i++)
                    {
                        PicInfo picInfo = ıcon.Low.Pics[i];
                        if (picInfo.Size != 0)
                        {
                            Buffer.BlockCopy(src, picInfo.Offset, array, 0, picInfo.Size);
                            using (FileStream output = File.OpenWrite(Path.Combine(outputPath, "Low", $"{ıcon.Key}_s{i}.png")))
                            {
                                using (BinaryWriter binaryWriter = new BinaryWriter(output))
                                {
                                    binaryWriter.Write(array, 0, picInfo.Size);
                                }
                            }
                        }
                    }                   
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
                    for (int i = 0; i < 8; i++)
                    {
                        PicInfo picInfo2 = ıcon2.High.Pics[i];
                        if (picInfo2.Size != 0)
                        {
                            Buffer.BlockCopy(src2, picInfo2.Offset, array, 0, picInfo2.Size);
                            using (FileStream output2 = File.OpenWrite(Path.Combine(outputPath, "High", $"{ıcon2.Key}_s{i}.png")))
                            {
                                using (BinaryWriter binaryWriter2 = new BinaryWriter(output2))
                                {
                                    binaryWriter2.Write(array, 0, picInfo2.Size);
                                }
                            }

                        }
                    }
                }
                File.Copy(text, Path.Combine(outputPath, indexFilename));
            }
        }
  public static void PackNew(string inputPath, string outputPath, string indexFilename = "IconResources.idx")
        {
            Directory.CreateDirectory(outputPath);
            PShop.PShop.CC2017.IconResources iconResources = IconResources.FromIndexFile(Path.Combine(inputPath, indexFilename));

            List<byte> lowResolutionData = new List<byte>();
            List<byte> highResolutionData = new List<byte>();

            foreach (ResourceIcon icon in iconResources.Icons)
            {
                PackIconData(icon.Low, icon.Key, inputPath, "Low", ref lowResolutionData);
                PackIconData(icon.High, icon.Key, inputPath, "High", ref highResolutionData);
            }

            WriteDataToFile(Path.Combine(outputPath, iconResources.LowResolutionDataFile), lowResolutionData);
            WriteDataToFile(Path.Combine(outputPath, iconResources.HighResolutionDataFile), highResolutionData);

            WriteIndexFile(Path.Combine(outputPath, iconResources.IndexFileName), iconResources.ToIndexFile());
        }

        private static void PackIconData(IconData iconData,string key, string inputPath, string subfolder, ref List<byte> data)
        {
            for (int i = 0; i < iconData.Pics.Count; i++)
            {
                var picInfo = iconData.Pics[i];
                if (picInfo.Size != 0)
                {
                    string filePath = Path.Combine(inputPath, subfolder, $"{key}_s{i}.png");

                    byte[] imageData = File.ReadAllBytes(filePath);
                    data.AddRange(imageData);
                }
            }
        }

        private static void WriteDataToFile(string filePath, List<byte> data)
        {
            using (FileStream stream = File.OpenWrite(filePath))
            {
                stream.Write(data.ToArray(), 0, data.Count);
            }
        }

        private static void WriteIndexFile(string filePath, byte[] indexData)
        {
            using (FileStream stream = File.OpenWrite(filePath))
            {
                stream.Write(indexData, 0, indexData.Length);
            }
        }


    }
}
