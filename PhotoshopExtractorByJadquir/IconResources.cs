using PhotoshopExtractorByJadquir.Types;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace PhotoshopExtractorByJadquir
{
    public sealed class IconResources
    {
        #region Constants and Properties
        private const int NameSize = 48;
        private const int DataSize = 320;

        public string IndexFileName { get; private set; }
        public string DirectoryPath { get; private set; }
        public string Name { get; private set; }
        public string LowResolutionDataFile { get; private set; }
        public string HighResolutionDataFile { get; private set; }
        public string XLowResolutionDataFile { get; private set; }
        public string XHighResolutionDataFile { get; private set; }

        internal List<ResourceIcon> Icons { get; private set; }
        #endregion

        #region Private Fields
        private readonly string _lowResolutionDataFile;
        private readonly string _highResolutionDataFile;
        private readonly string _xlowResolutionDataFile;
        private readonly string _xhighResolutionDataFile;
        private List<int> _markerIndexes = new List<int>();

        // Cache for extracted files to improve packing performance
        private Dictionary<string, Dictionary<string, Dictionary<int, string>>> _extractedFilesCache = new();

        // Thread synchronization objects for parallel processing
        private object _lowLock = new object();
        private object _highLock = new object();
        private object _xlowLock = new object();
        private object _xhighLock = new object();

        // Store original file headers to preserve them during packing
        private Dictionary<string, string> _originalHeaders = new Dictionary<string, string>
        {
            { "low", "fdra" },
            { "high", "fdra" },
            { "xlow", "fdra" },
            { "xhigh", "fdra" }
        };
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of IconResources with the specified index file
        /// </summary>
        /// <param name="indexFileName">Path to the IconResources.idx file</param>
        public IconResources(string indexFileName)
        {
            IndexFileName = indexFileName;
            DirectoryPath = Path.GetDirectoryName(indexFileName);
            Name = Path.GetFileName(indexFileName);

            // Initialize data file paths
            _lowResolutionDataFile = Path.Combine(DirectoryPath, "PSIconsLowRes.dat");
            _highResolutionDataFile = Path.Combine(DirectoryPath, "PSIconsHighRes.dat");
            _xlowResolutionDataFile = Path.Combine(DirectoryPath, "PSIconsXLowRes.dat");
            _xhighResolutionDataFile = Path.Combine(DirectoryPath, "PSIconsXHighRes.dat");

            Icons = new List<ResourceIcon>();
            LoadIndexFile();
            DetectOriginalHeaders();
        }
        #endregion

        #region Header Detection
        /// <summary>
        /// Detects and stores the original file headers from existing .dat files
        /// </summary>
        private void DetectOriginalHeaders()
        {
            var dataFiles = new Dictionary<string, string>
            {
                { "low", _lowResolutionDataFile },
                { "high", _highResolutionDataFile },
                { "xlow", _xlowResolutionDataFile },
                { "xhigh", _xhighResolutionDataFile }
            };

            foreach (var kvp in dataFiles)
            {
                try
                {
                    if (File.Exists(kvp.Value))
                    {
                        var buffer = File.ReadAllBytes(kvp.Value);
                        if (buffer.Length >= 4)
                        {
                            string header = Encoding.ASCII.GetString(buffer, 0, 4);
                            if (IsValidHeader(header))
                            {
                                _originalHeaders[kvp.Key] = header;
                                //Debug.WriteLine($"Detected header '{header}' for {kvp.Key} resolution file");
                            }
                            else
                            {
                                Debug.WriteLine($"Warning: Invalid header '{header}' detected in {kvp.Value}, using default 'fdra'");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error detecting header for {kvp.Value}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Checks if a header is valid (fdra or fdrq)
        /// </summary>
        /// <param name="header">4-character header string</param>
        /// <returns>True if header is valid</returns>
        private static bool IsValidHeader(string header)
        {
            return header == "fdra" || header == "fdrq";
        }

        /// <summary>
        /// Gets the original header for a specific resolution
        /// </summary>
        /// <param name="resolution">Resolution key (low, high, xlow, xhigh)</param>
        /// <returns>Original header string</returns>
        public string GetOriginalHeader(string resolution)
        {
            return _originalHeaders.TryGetValue(resolution.ToLower(), out string header) ? header : "fdra";
        }

        /// <summary>
        /// Sets the header for a specific resolution
        /// </summary>
        /// <param name="resolution">Resolution key (low, high, xlow, xhigh)</param>
        /// <param name="header">Header string (fdra or fdrq)</param>
        public void SetHeader(string resolution, string header)
        {
            if (!IsValidHeader(header))
                throw new ArgumentException($"Invalid header '{header}'. Must be 'fdra' or 'fdrq'");

            _originalHeaders[resolution.ToLower()] = header;
            Debug.WriteLine($"Set header '{header}' for {resolution} resolution");
        }
        #endregion

        #region Index File Operations
        /// <summary>
        /// Loads and parses the index file to populate Icons collection
        /// </summary>
        private void LoadIndexFile()
        {
            try
            {
                var buffer = File.ReadAllBytes(IndexFileName);
                if (buffer.Length == 0)
                    throw new InvalidDataException("Index file is empty");

                // Find marker positions (0x0A bytes that separate sections)
                _markerIndexes.Clear();
                int idx = 0;
                for (int i = 0; i < buffer.Length; i++)
                {
                    if (buffer[i] == 0x0A)
                    {
                        _markerIndexes.Add(i);
                        idx++;
                        if (idx > 4) break;
                    }
                }

                if (_markerIndexes.Count < 5)
                    throw new InvalidDataException("Invalid index file format: insufficient marker indexes");

                // Parse header sections
                Name = Encoding.ASCII.GetString(buffer.Take(_markerIndexes[0]).ToArray()).TrimEnd('\0');
                LowResolutionDataFile = Encoding.ASCII.GetString(buffer.Skip(_markerIndexes[0] + 1).Take(_markerIndexes[1] - _markerIndexes[0] - 1).ToArray()).TrimEnd('\0');
                HighResolutionDataFile = Encoding.ASCII.GetString(buffer.Skip(_markerIndexes[1] + 1).Take(_markerIndexes[2] - _markerIndexes[1] - 1).ToArray()).TrimEnd('\0');
                XLowResolutionDataFile = Encoding.ASCII.GetString(buffer.Skip(_markerIndexes[2] + 1).Take(_markerIndexes[3] - _markerIndexes[2] - 1).ToArray()).TrimEnd('\0');
                XHighResolutionDataFile = Encoding.ASCII.GetString(buffer.Skip(_markerIndexes[3] + 1).Take(_markerIndexes[4] - _markerIndexes[3] - 1).ToArray()).TrimEnd('\0');

                // Parse icon entries
                int offset = _markerIndexes[4] + 1;
                int structSize = NameSize + DataSize;
                int count = (buffer.Length - offset) / structSize;
                byte[] dst = new byte[structSize];

                for (int i = 0; i < count; i++)
                {
                    Array.Copy(buffer, offset + i * structSize, dst, 0, structSize);
                    var resourceIcon = new ResourceIcon(dst);
                    Icons.Add(resourceIcon);

                    // Validate icon data and clean up invalid entries
                    bool hasValidData = false;
                    var invalidEntries = new List<string>();

                    foreach (var res in new[] { ("Low", resourceIcon.Low), ("High", resourceIcon.High), ("XLow", resourceIcon.XLow), ("XHigh", resourceIcon.XHigh) })
                    {
                        int picIndex = 0;
                        foreach (var pic in res.Item2.Pics)
                        {
                            if (pic.Size > 0 && pic.Offset >= 8)
                                hasValidData = true;
                            else if (pic.Size != 0 || pic.Offset != 0)
                            {
                                invalidEntries.Add($"{res.Item1} pic{picIndex}: Offset={pic.Offset}, Size={pic.Size}");
                                // Reset invalid entries to prevent corruption
                                pic.Offset = 0;
                                pic.Size = 0;
                            }
                            picIndex++;
                        }
                    }

                    if (!hasValidData && invalidEntries.Any())
                    {
                        Debug.WriteLine($"Warning: Icon {resourceIcon.Key} has invalid data entries: {string.Join(", ", invalidEntries)}");
                    }
                }
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException($"Index file not found at {IndexFileName}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading index file: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Outputs the updated index file to the specified directory
        /// </summary>
        /// <param name="workingDirectory">Directory to write the index file</param>
        private void OutputIndexFile(string workingDirectory)
        {
            try
            {
                var buf = new List<byte>();

                // Write header sections with proper padding
                var nameBuf = Encoding.ASCII.GetBytes(Name ?? "");
                buf.AddRange(nameBuf);
                for (int i = 0; i < _markerIndexes[0] - nameBuf.Length; i++)
                    buf.Add(0);
                buf.Add(0x0A);

                var lowFileBuf = Encoding.ASCII.GetBytes(LowResolutionDataFile ?? "PSIconsLowRes.dat");
                buf.AddRange(lowFileBuf);
                for (int i = 0; i < _markerIndexes[1] - _markerIndexes[0] - lowFileBuf.Length - 1; i++)
                    buf.Add(0);
                buf.Add(0x0A);

                var highFileBuf = Encoding.ASCII.GetBytes(HighResolutionDataFile ?? "PSIconsHighRes.dat");
                buf.AddRange(highFileBuf);
                for (int i = 0; i < _markerIndexes[2] - _markerIndexes[1] - highFileBuf.Length - 1; i++)
                    buf.Add(0);
                buf.Add(0x0A);

                var xLowFileBuf = Encoding.ASCII.GetBytes(XLowResolutionDataFile ?? "PSIconsXLowRes.dat");
                buf.AddRange(xLowFileBuf);
                for (int i = 0; i < _markerIndexes[3] - _markerIndexes[2] - xLowFileBuf.Length - 1; i++)
                    buf.Add(0);
                buf.Add(0x0A);

                var xHighFileBuf = Encoding.ASCII.GetBytes(XHighResolutionDataFile ?? "PSIconsXHighRes.dat");
                buf.AddRange(xHighFileBuf);
                for (int i = 0; i < _markerIndexes[4] - _markerIndexes[3] - xHighFileBuf.Length - 1; i++)
                    buf.Add(0);
                buf.Add(0x0A);

                // Write icon data
                foreach (var icon in Icons)
                    buf.AddRange(icon.ToByteArray());

                string outputPath = Path.Combine(workingDirectory, "IconResources.idx");
                File.WriteAllBytes(outputPath, buf.ToArray());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error writing index file: {ex.Message}");
                throw;
            }
        }
        #endregion

        #region File Type Detection
        /// <summary>
        /// Determines file extension based on file header/magic bytes
        /// </summary>
        /// <param name="data">File data to analyze</param>
        /// <returns>Appropriate file extension</returns>
        private static string GetFileExtension(byte[] data)
        {
            if (data == null || data.Length < 4) return ".bin";

            string asciiHeader = Encoding.ASCII.GetString(data, 0, Math.Min(data.Length, 8));

            // Check for common file formats
            if (asciiHeader.StartsWith("<?xml") || asciiHeader.StartsWith("<svg")) return ".svg";

            // PNG signature
            if (data.Length >= 8 &&
                data[0] == 0x89 && data[1] == 0x50 && data[2] == 0x4E && data[3] == 0x47 &&
                data[4] == 0x0D && data[5] == 0x0A && data[6] == 0x1A && data[7] == 0x0A)
                return ".png";

            // JPEG signature
            if (data.Length >= 2 && data[0] == 0xFF && data[1] == 0xD8) return ".jpg";

            // ICO signature
            if (data.Length >= 4 && data[0] == 0x00 && data[1] == 0x00 && data[2] == 0x01 && data[3] == 0x00) return ".ico";

            // WEBP signature
            if (data.Length >= 12 && asciiHeader.StartsWith("RIFF") && Encoding.ASCII.GetString(data, 8, 4) == "WEBP") return ".webp";

            return ".bin";
        }
        #endregion

        #region Extraction Operations
        /// <summary>
        /// Extracts all icon data from .dat files to individual image files
        /// </summary>
        /// <param name="workingDirectory">Directory to extract files to</param>
        public void Extract(string workingDirectory)
        {
            try
            {
                _extractedFilesCache.Clear();
                Directory.CreateDirectory(workingDirectory);

                var dataFiles = new Dictionary<string, string>
            {
                { "low", _lowResolutionDataFile },
                { "high", _highResolutionDataFile },
                { "xlow", _xlowResolutionDataFile },
                { "xhigh", _xhighResolutionDataFile }
            };

                // Load all data files
                var buffers = new Dictionary<string, byte[]>();
                foreach (var kvp in dataFiles)
                {
                    try
                    {
                        if (File.Exists(kvp.Value))
                        {
                            buffers[kvp.Key] = File.ReadAllBytes(kvp.Value);
                            Debug.WriteLine($"Successfully loaded: {kvp.Value} ({buffers[kvp.Key].Length} bytes)");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error loading data file {kvp.Value}: {ex.Message}");
                    }
                }

                // Extract each resolution
                foreach (var resolution in new[] { "low", "high", "xlow", "xhigh" })
                {
                    if (buffers.ContainsKey(resolution))
                    {
                        ExtractResolution(resolution, workingDirectory, Icons, buffers[resolution]);
                    }
                }

                Debug.WriteLine($"Extraction completed successfully to: {workingDirectory}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during extraction: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Extracts icons from a specific resolution data file
        /// </summary>
        private void ExtractResolution(string resolutionKey, string workingDirectory, List<ResourceIcon> icons, byte[] buffer)
        {
            // Validate data file header - support both fdra and fdrq
            if (buffer.Length < 8)
            {
                Debug.WriteLine($"Error: Data file too short for {resolutionKey}");
                return;
            }

            string header = Encoding.ASCII.GetString(buffer, 0, 4);
            if (!IsValidHeader(header))
            {
                Debug.WriteLine($"Error: Invalid .dat file header '{header}' for {resolutionKey}. Expected 'fdra' or 'fdrq'");
                return;
            }

            Debug.WriteLine($"Processing {resolutionKey} resolution with header '{header}'");

            string folderName = resolutionKey switch
            {
                "low" => "Low",
                "high" => "High",
                "xlow" => "XLow",
                "xhigh" => "XHigh",
                _ => resolutionKey
            };

            string folderPath = Path.Combine(workingDirectory, folderName);
            Directory.CreateDirectory(folderPath);

            foreach (var icon in icons)
            {
                // Get the appropriate icon data for this resolution
                IconData? data = resolutionKey switch
                {
                    "low" => icon.Low,
                    "high" => icon.High,
                    "xlow" => icon.XLow,
                    "xhigh" => icon.XHigh,
                    _ => null
                };

                if (data == null) continue;

                int picIndex = 0;
                foreach (var pic in data.Pics)
                {
                    // Validate pic data bounds
                    if (pic.Size <= 0 || pic.Offset < 8 || pic.Offset + pic.Size > buffer.Length)
                    {
                        if (pic.Size != 0 || pic.Offset != 0)
                            Debug.WriteLine($"Warning: Invalid PicInfo for {icon.Key} {resolutionKey} pic{picIndex}: Offset={pic.Offset}, Size={pic.Size}");
                        picIndex++;
                        continue;
                    }

                    try
                    {
                        // Extract image data
                        byte[] extractedData = new byte[pic.Size];
                        Array.Copy(buffer, pic.Offset, extractedData, 0, pic.Size);

                        string ext = GetFileExtension(extractedData);
                        pic.FileType = ext;
                        string outputFile = Path.Combine(folderPath, $"{icon.Key}_pic{picIndex}_s{ext}");

                        if (ext != ".bin")
                        {
                            // Save as recognized image format
                            File.WriteAllBytes(outputFile, extractedData);
                        }
                        else
                        {
                            // Try to convert raw ARGB data to PNG
                            int width = data.Width;
                            int height = data.Height;
                            if (width > 0 && height > 0 && extractedData.Length == width * height * 4)
                            {
                                using (var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb))
                                {
                                    var bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bmp.PixelFormat);
                                    System.Runtime.InteropServices.Marshal.Copy(extractedData, 0, bmpData.Scan0, extractedData.Length);
                                    bmp.UnlockBits(bmpData);
                                    outputFile = Path.Combine(folderPath, $"{icon.Key}_pic{picIndex}_s.png");
                                    pic.FileType = ".png";
                                    bmp.Save(outputFile, ImageFormat.Png);
                                }
                            }
                            else
                            {
                                // Save as raw binary data
                                outputFile = Path.Combine(folderPath, $"{icon.Key}_pic{picIndex}_raw.bin");
                                File.WriteAllBytes(outputFile, extractedData);
                            }
                        }

                        // Cache the filename for efficient packing later
                        if (!_extractedFilesCache.TryGetValue(icon.Key, out var resDict))
                            _extractedFilesCache[icon.Key] = resDict = new Dictionary<string, Dictionary<int, string>>();
                        if (!resDict.TryGetValue(resolutionKey, out var picDict))
                            resDict[resolutionKey] = picDict = new Dictionary<int, string>();
                        picDict[picIndex] = outputFile;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error extracting {icon.Key} {resolutionKey} pic{picIndex}: {ex.Message}");
                    }
                    picIndex++;
                }
            }
        }
        #endregion

        #region Packing Operations
        /// <summary>
        /// Packs extracted image files back into .dat files
        /// </summary>
        /// <param name="workingDirectory">Directory containing extracted files</param>
        public void Pack(string workingDirectory)
        {
            try
            {
                // Initialize buffers for each resolution with proper headers (preserving original)
                var lowBuffer = new List<byte>(Encoding.ASCII.GetBytes(GetOriginalHeader("low")).Concat(new byte[4]));
                var highBuffer = new List<byte>(Encoding.ASCII.GetBytes(GetOriginalHeader("high")).Concat(new byte[4]));
                var xlowBuffer = new List<byte>(Encoding.ASCII.GetBytes(GetOriginalHeader("xlow")).Concat(new byte[4]));
                var xhighBuffer = new List<byte>(Encoding.ASCII.GetBytes(GetOriginalHeader("xhigh")).Concat(new byte[4]));

                int lowOffset = 8;
                int highOffset = 8;
                int xlowOffset = 8;
                int xhighOffset = 8;

                Debug.WriteLine($"Packing with headers: Low='{GetOriginalHeader("low")}', High='{GetOriginalHeader("high")}', XLow='{GetOriginalHeader("xlow")}', XHigh='{GetOriginalHeader("xhigh")}'");

                // Process each icon in parallel for better performance
                foreach (var icon in Icons)
                {
                    Action[] resolutionTasks = new Action[]
                    {
                    () => ProcessResolution(workingDirectory, icon, "Low", icon.Low, lowBuffer, ref lowOffset, _lowLock),
                    () => ProcessResolution(workingDirectory, icon, "High", icon.High, highBuffer, ref highOffset, _highLock),
                    () => ProcessResolution(workingDirectory, icon, "XLow", icon.XLow, xlowBuffer, ref xlowOffset, _xlowLock),
                    () => ProcessResolution(workingDirectory, icon, "XHigh", icon.XHigh, xhighBuffer, ref xhighOffset, _xhighLock)
                    };

                    Parallel.Invoke(resolutionTasks);
                }

                // Write all output files
                try
                {
                    File.WriteAllBytes(Path.Combine(workingDirectory, "PSIconsLowRes.dat"), lowBuffer.ToArray());
                    File.WriteAllBytes(Path.Combine(workingDirectory, "PSIconsHighRes.dat"), highBuffer.ToArray());
                    File.WriteAllBytes(Path.Combine(workingDirectory, "PSIconsXLowRes.dat"), xlowBuffer.ToArray());
                    File.WriteAllBytes(Path.Combine(workingDirectory, "PSIconsXHighRes.dat"), xhighBuffer.ToArray());
                    OutputIndexFile(workingDirectory);

                    Debug.WriteLine($"Packing completed successfully to: {workingDirectory}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error writing output files: {ex.Message}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during packing: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Extracts pic index from filename (e.g., "icon_pic2_s.png" returns 2)
        /// </summary>
        private int GetPicIndex(string file)
        {
            string name = Path.GetFileNameWithoutExtension(file);
            if (name.Contains("_pic"))
            {
                string picPart = name.Split('_').LastOrDefault(s => s.StartsWith("pic"));
                if (picPart != null && int.TryParse(picPart.Substring(3), out int idx) && idx >= 0 && idx < 8)
                    return idx;
            }
            return -1;
        }

        /// <summary>
        /// Processes files for a specific resolution and adds them to the buffer
        /// </summary>
        private void ProcessResolution(string workingDirectory, ResourceIcon icon, string folder, IconData iconData, List<byte> buffer, ref int offset, object lockObj)
        {
            // Initialize empty pic info array
            iconData.Pics.Clear();
            for (int i = 0; i < 8; i++)
                iconData.Pics.Add(new PicInfo(0, 0));

            string folderPath = Path.Combine(workingDirectory, folder);
            List<(string File, int Index)> files = new List<(string, int)>();

            // Try to get files from cache first (more efficient)
            if (_extractedFilesCache.TryGetValue(icon.Key, out var resDict) && resDict.TryGetValue(folder.ToLower(), out var picDict))
            {
                files = picDict.Select(kv => (File: kv.Value, Index: kv.Key))
                              .Where(f => f.Index >= 0 && f.Index < 8)
                              .OrderBy(f => f.Index)
                              .ToList();
            }
            else if (Directory.Exists(folderPath))
            {
                // Scan directory for matching files
                var dirFiles = Directory.GetFiles(folderPath, $"{icon.Key}_pic*_s.*")
                    .Select(f => (File: f, Index: GetPicIndex(f)))
                    .Where(f => f.Index >= 0 && f.Index < 8)
                    .OrderBy(f => f.Index)
                    .ToList();
                files = dirFiles;
            }
            else
            {
                return; // No files found for this icon/resolution
            }

            // Process each file
            foreach (var (file, idx) in files)
            {
                byte[] data;
                try
                {
                    data = File.ReadAllBytes(file);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error reading file {file}: {ex.Message}");
                    continue;
                }

                if (data.Length == 0)
                {
                    Debug.WriteLine($"Warning: File {file} is empty, skipping for {icon.Key} pic{idx}");
                    continue;
                }

                string ext = Path.GetExtension(file).ToLower();

                // Handle raw ARGB data converted to PNG during extraction
                if (ext == ".bin" && iconData.Width > 0 && iconData.Height > 0 && data.Length == iconData.Width * iconData.Height * 4)
                {
                    ext = ".png";
                }

                // Verify file type matches content
                string detectedExt = GetFileExtension(data);
                if (ext != detectedExt)
                {
                    Debug.WriteLine($"Warning: File {file} has extension {ext} but detected as {detectedExt}");
                    ext = detectedExt;
                }

                // Update pic info and add data to buffer
                iconData.Pics[idx] = new PicInfo(offset, data.Length, ext);
                lock (lockObj)
                {
                    buffer.AddRange(data);
                    offset += data.Length;
                }
            }
        }
        #endregion


        #region Resource Info Methods

        /// <summary>
        /// Gets detailed resource information for a specific icon
        /// </summary>
        /// <param name="iconName">Name of the icon to get info for</param>
        /// <returns>ResourceInfo object or null if not found</returns>
        public ResourceInfo GetResourceInfo(string iconName)
        {
            var icon = Icons.FirstOrDefault(i => i.Key.Equals(iconName, StringComparison.OrdinalIgnoreCase));
            if (icon == null)
                return null;

            var resourceInfo = new ResourceInfo(icon.Key);
            resourceInfo.Resolutions["low"] = new ResolutionInfo(icon.Low);
            resourceInfo.Resolutions["high"] = new ResolutionInfo(icon.High);
            resourceInfo.Resolutions["xlow"] = new ResolutionInfo(icon.XLow);
            resourceInfo.Resolutions["xhigh"] = new ResolutionInfo(icon.XHigh);

            return resourceInfo;
        }

        /// <summary>
        /// Gets resource information for all icons
        /// </summary>
        /// <returns>Dictionary of all resource information</returns>
        public Dictionary<string, ResourceInfo> GetAllResourceInfo()
        {
            var result = new Dictionary<string, ResourceInfo>();
            foreach (var icon in Icons)
            {
                result[icon.Key] = GetResourceInfo(icon.Key);
            }
            return result;
        }

        /// <summary>
        /// Gets detailed pic information for a specific icon and resolution
        /// </summary>
        /// <param name="iconName">Name of the icon</param>
        /// <param name="resolution">Resolution (low, high, xlow, xhigh)</param>
        /// <returns>List of PicInfoDetail objects</returns>
        public List<PicInfoDetail> GetPicInfo(string iconName, string resolution)
        {
            var resourceInfo = GetResourceInfo(iconName);
            return resourceInfo?.GetResolution(resolution)?.Pics ?? new List<PicInfoDetail>();
        }

        /// <summary>
        /// Gets pic data from the .dat files for a specific icon, resolution, and pic index
        /// </summary>
        /// <param name="iconName">Name of the icon</param>
        /// <param name="resolution">Resolution (low, high, xlow, xhigh)</param>
        /// <param name="picIndex">Pic index (0-7)</param>
        /// <returns>Byte array of pic data or null if not found/invalid</returns>
        public byte[] GetPicData(string iconName, string resolution, int picIndex)
        {
            if (picIndex < 0 || picIndex >= 8)
                return null;

            var picInfo = GetPicInfo(iconName, resolution);
            if (picIndex >= picInfo.Count || !picInfo[picIndex].IsValid)
                return null;

            var pic = picInfo[picIndex];
            var dataFile = resolution.ToLower() switch
            {
                "low" => _lowResolutionDataFile,
                "high" => _highResolutionDataFile,
                "xlow" => _xlowResolutionDataFile,
                "xhigh" => _xhighResolutionDataFile,
                _ => null
            };

            if (dataFile == null || !File.Exists(dataFile))
                return null;

            try
            {
                var buffer = File.ReadAllBytes(dataFile);
                if (pic.Offset + pic.Size > buffer.Length)
                    return null;

                var data = new byte[pic.Size];
                Array.Copy(buffer, pic.Offset, data, 0, pic.Size);
                return data;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading pic data: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Creates a ModifyIconInstance from existing resource data
        /// </summary>
        /// <param name="iconName">Name of the icon</param>
        /// <param name="resolutions">Resolutions to include (null = all available)</param>
        /// <param name="picIndexes">Pic indexes to include (null = all valid pics)</param>
        /// <returns>ModifyIconInstance with current data</returns>
        public ModifyIconInstance CreateModifyInstance(string iconName, string[] resolutions = null, int[] picIndexes = null)
        {
            var instance = new ModifyIconInstance(iconName);
            var resourceInfo = GetResourceInfo(iconName);

            if (resourceInfo == null)
                return instance;

            var targetResolutions = resolutions ?? new[] { "low", "high", "xlow", "xhigh" };
            var targetPics = picIndexes ?? Enumerable.Range(0, 8).ToArray();

            foreach (var resolution in targetResolutions)
            {
                foreach (var picIndex in targetPics)
                {
                    var data = GetPicData(iconName, resolution, picIndex);
                    if (data != null)
                    {
                        instance.AddPicData(resolution, picIndex, data);
                    }
                }
            }

            return instance;
        }

        /// <summary>
        /// Creates a ModifyIconInstance for cloning data from one icon to another
        /// </summary>
        /// <param name="sourceIconName">Source icon name</param>
        /// <param name="targetIconName">Target icon name</param>
        /// <param name="resolutions">Resolutions to clone (null = all)</param>
        /// <param name="picIndexes">Pic indexes to clone (null = all valid)</param>
        /// <returns>ModifyIconInstance for the target icon</returns>
        public ModifyIconInstance CreateCloneModifyInstance(string sourceIconName, string targetIconName,
            string[] resolutions = null, int[] picIndexes = null)
        {
            var sourceInstance = CreateModifyInstance(sourceIconName, resolutions, picIndexes);
            return new ModifyIconInstance(targetIconName, sourceInstance.ModifiedData);
        }

        #endregion

        #region Modification Methods

        /// <summary>
        /// Enhanced version of ModifyIconData with better error handling and validation
        /// </summary>
        /// <param name="workingDirectory">Directory to output modified files</param>
        /// <param name="instances">ModifyIconInstance objects to process</param>
        public void ModifyIconDataEnhanced(string workingDirectory, params ModifyIconInstance[] instances)
        {
            if (instances == null || !instances.Any())
            {
                Debug.WriteLine("No modification instances provided");
                return;
            }

            Debug.WriteLine($"Modifying icon data for {instances.Length} instances...");

            try
            {
                // Validate all instances before processing
                var validInstances = new List<ModifyIconInstance>();
                foreach (var instance in instances)
                {
                    if (string.IsNullOrEmpty(instance.IconName))
                    {
                        Debug.WriteLine("Warning: Skipping instance with empty icon name");
                        continue;
                    }

                    var icon = Icons.FirstOrDefault(i => i.Key.Equals(instance.IconName, StringComparison.OrdinalIgnoreCase));
                    if (icon == null)
                    {
                        Debug.WriteLine($"Warning: Icon '{instance.IconName}' not found, skipping");
                        continue;
                    }

                    if (instance.GetTotalModifiedPics() == 0)
                    {
                        Debug.WriteLine($"Warning: No modified pics found for '{instance.IconName}', skipping");
                        continue;
                    }

                    validInstances.Add(instance);
                }

                if (!validInstances.Any())
                {
                    Debug.WriteLine("No valid instances to process");
                    return;
                }

                Debug.WriteLine($"Processing {validInstances.Count} valid instances");

                // Call the original ModifyIconData method with validated instances
                ModifyIconData(workingDirectory, validInstances.ToArray());

                Debug.WriteLine("Modification completed successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in modify icon data: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Batch modify multiple icons with the same data
        /// </summary>
        /// <param name="workingDirectory">Output directory</param>
        /// <param name="iconNames">Icons to modify</param>
        /// <param name="resolution">Resolution to modify</param>
        /// <param name="picIndex">Pic index to modify</param>
        /// <param name="data">Data to apply to all icons</param>
        public void BatchModifyIcons(string workingDirectory, string[] iconNames, string resolution, int picIndex, byte[] data)
        {
            var instances = iconNames.Select(name =>
            {
                var instance = new ModifyIconInstance(name);
                instance.AddPicData(resolution, picIndex, data);
                return instance;
            }).ToArray();

            ModifyIconDataEnhanced(workingDirectory, instances);
        }

        /// <summary>
        /// Replace all pics of an icon with data from files
        /// </summary>
        /// <param name="workingDirectory">Output directory</param>
        /// <param name="iconName">Icon to modify</param>
        /// <param name="resolution">Resolution to modify</param>
        /// <param name="filePaths">Array of file paths (index = pic index)</param>
        public void ReplaceIconFromFiles(string workingDirectory, string iconName, string resolution, string[] filePaths)
        {
            var instance = new ModifyIconInstance(iconName);

            for (int i = 0; i < Math.Min(filePaths.Length, 8); i++)
            {
                if (!string.IsNullOrEmpty(filePaths[i]) && File.Exists(filePaths[i]))
                {
                    instance.AddPicDataFromFile(resolution, i, filePaths[i]);
                }
            }

            ModifyIconDataEnhanced(workingDirectory, instance);
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Gets a summary of all icons and their pic counts
        /// </summary>
        /// <returns>Dictionary with icon names and their valid pic counts per resolution</returns>
        public Dictionary<string, Dictionary<string, int>> GetIconSummary()
        {
            var summary = new Dictionary<string, Dictionary<string, int>>();

            foreach (var icon in Icons)
            {
                var iconSummary = new Dictionary<string, int>();
                iconSummary["low"] = icon.Low.Pics.Count(p => p.Size > 0 && p.Offset >= 8);
                iconSummary["high"] = icon.High.Pics.Count(p => p.Size > 0 && p.Offset >= 8);
                iconSummary["xlow"] = icon.XLow.Pics.Count(p => p.Size > 0 && p.Offset >= 8);
                iconSummary["xhigh"] = icon.XHigh.Pics.Count(p => p.Size > 0 && p.Offset >= 8);

                summary[icon.Key] = iconSummary;
            }

            return summary;
        }

        /// <summary>
        /// Validates that all required data files exist
        /// </summary>
        /// <returns>List of missing files</returns>
        public List<string> ValidateDataFiles()
        {
            var missingFiles = new List<string>();
            var files = new[]
            {
                _lowResolutionDataFile,
                _highResolutionDataFile,
                _xlowResolutionDataFile,
                _xhighResolutionDataFile
            };

            foreach (var file in files)
            {
                if (!File.Exists(file))
                    missingFiles.Add(file);
            }

            return missingFiles;
        }

        /// <summary>
        /// Finds icons that match a search pattern
        /// </summary>
        /// <param name="pattern">Search pattern (supports wildcards)</param>
        /// <returns>List of matching icon names</returns>
        public List<string> FindIcons(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                return Icons.Select(i => i.Key).ToList();

            // Simple wildcard matching
            pattern = pattern.Replace("*", ".*").Replace("?", ".");
            var regex = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            return Icons.Where(i => regex.IsMatch(i.Key)).Select(i => i.Key).ToList();
        }

        #endregion

        #region Original ModifyIconData Method

        /// <summary>
        /// Original ModifyIconData method 
        /// </summary>
        /// <param name="workingDirectory">Directory to output modified files</param>
        /// <param name="icons">ModifyIconInstance objects to process</param>
        public void ModifyIconData(string workingDirectory, params ModifyIconInstance[] icons)
        {
            Debug.WriteLine($"Modifying icon data for {icons.Length} icons...");

            try
            {
                // Ensure working directory exists
                Directory.CreateDirectory(workingDirectory);

                var dataFiles = new Dictionary<string, string>
                {
                    { "low", _lowResolutionDataFile },
                    { "high", _highResolutionDataFile },
                    { "xlow", _xlowResolutionDataFile },
                    { "xhigh", _xhighResolutionDataFile }
                };

                var outputDataFiles = new Dictionary<string, string>
                {
                    { "low", Path.Combine(workingDirectory, "PSIconsLowRes.dat") },
                    { "high", Path.Combine(workingDirectory, "PSIconsHighRes.dat") },
                    { "xlow", Path.Combine(workingDirectory, "PSIconsXLowRes.dat") },
                    { "xhigh", Path.Combine(workingDirectory, "PSIconsXHighRes.dat") }
                };

                var buffers = new Dictionary<string, (List<byte> Buffer, int Offset, object LockObj)>
                {
                    { "low", (new List<byte>(Encoding.ASCII.GetBytes(GetOriginalHeader("low")).Concat(new byte[4])), 8, _lowLock) },
                    { "high", (new List<byte>(Encoding.ASCII.GetBytes(GetOriginalHeader("high")).Concat(new byte[4])), 8, _highLock) },
                    { "xlow", (new List<byte>(Encoding.ASCII.GetBytes(GetOriginalHeader("xlow")).Concat(new byte[4])), 8, _xlowLock) },
                    { "xhigh", (new List<byte>(Encoding.ASCII.GetBytes(GetOriginalHeader("xhigh")).Concat(new byte[4])), 8, _xhighLock) }
                };

                // Copy original .dat files to working directory and load into buffers
                foreach (var kvp in dataFiles)
                {
                    string resKey = kvp.Key;
                    string srcPath = kvp.Value;
                    string destPath = outputDataFiles[resKey];

                    try
                    {
                        if (File.Exists(srcPath))
                        {
                            byte[] existingData = File.ReadAllBytes(srcPath);
                            string header = Encoding.ASCII.GetString(existingData, 0, 4);

                            if (header != "fdra" && header != "fdrq")
                            {
                                Debug.WriteLine($"Error: Invalid file header for {srcPath}");
                                buffers[resKey] = (new List<byte>(Encoding.ASCII.GetBytes(GetOriginalHeader(resKey)).Concat(new byte[4])), 8, buffers[resKey].LockObj);
                            }
                            else
                            {
                                File.WriteAllBytes(destPath, existingData);
                                buffers[resKey] = (new List<byte>(existingData), existingData.Length, buffers[resKey].LockObj);
                                Debug.WriteLine($"Copied {srcPath} to {destPath}: {existingData.Length} bytes");
                            }
                        }
                        else
                        {
                            Debug.WriteLine($"Info: Original data file {srcPath} not found, creating new {destPath}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error copying {srcPath} to {destPath}: {ex.Message}");
                        throw;
                    }
                }

                // Copy original IconResources.idx to working directory
                string outputIndexPath = Path.Combine(workingDirectory, "IconResources.idx");
                try
                {
                    if (File.Exists(IndexFileName))
                    {
                        File.Copy(IndexFileName, outputIndexPath, true);
                        Debug.WriteLine($"Copied {IndexFileName} to {outputIndexPath}");
                    }
                    else
                    {
                        Debug.WriteLine($"Error: Index file {IndexFileName} not found");
                        throw new FileNotFoundException($"Index file {IndexFileName} not found");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error copying index file: {ex.Message}");
                    throw;
                }

                // Process each icon instance
                foreach (var iconInstance in icons)
                {
                    string iconName = iconInstance.IconName;
                    var modifiedData = iconInstance.ModifiedData;

                    var icon = Icons.FirstOrDefault(i => i.Key.Equals(iconName, StringComparison.OrdinalIgnoreCase));
                    if (icon == null)
                    {
                        Debug.WriteLine($"Error: Icon {iconName} not found in Icons list.");
                        continue;
                    }

                    foreach (var res in new[] { ("low", icon.Low), ("high", icon.High), ("xlow", icon.XLow), ("xhigh", icon.XHigh) })
                    {
                        string resKey = res.Item1;
                        IconData iconData = res.Item2;

                        if (!modifiedData.ContainsKey(resKey))
                            continue;

                        var picDict = modifiedData[resKey];
                        var (buffer, offset, lockObj) = buffers[resKey];

                        // Extract original data for validation
                        byte[] originalBuffer = buffer.Count > 8 ? buffer.ToArray() : Array.Empty<byte>();

                        for (int i = 0; i < 8; i++)
                        {
                            if (picDict.ContainsKey(i))
                            {
                                byte[] newData = picDict[i];

                                if (newData == null || newData.Length == 0)
                                {
                                    Debug.WriteLine($"Warning: Null or empty data for {iconName} {resKey} pic{i}, skipping.");
                                    iconData.Pics[i] = new PicInfo(0, 0);
                                    continue;
                                }

                                string ext = GetFileExtension(newData);
                                if (ext == ".bin" && iconData.Width > 0 && iconData.Height > 0 && newData.Length == iconData.Width * iconData.Height * 4)
                                    ext = ".png";

                                // Update cache for consistency
                                string folder = resKey switch
                                {
                                    "low" => "Low",
                                    "high" => "High",
                                    "xlow" => "XLow",
                                    "xhigh" => "XHigh",
                                    _ => resKey
                                };

                                string fileName = $"{iconName}_pic{i}_s{ext}";
                                if (!_extractedFilesCache.TryGetValue(iconName, out var resDict))
                                    _extractedFilesCache[iconName] = resDict = new Dictionary<string, Dictionary<int, string>>();

                                if (!resDict.TryGetValue(resKey, out var picCache))
                                    resDict[resKey] = picCache = new Dictionary<int, string>();

                                picCache[i] = Path.Combine(workingDirectory, folder, fileName);

                                // Append new data
                                lock (lockObj)
                                {
                                    iconData.Pics[i] = new PicInfo(offset, newData.Length, ext);
                                    buffer.AddRange(newData);
                                    offset += newData.Length;
                                }

                                Debug.WriteLine($"Injected {iconName} {resKey} pic{i}: File={fileName}, Offset={offset - newData.Length}, Size={newData.Length}, Type={ext}");
                            }
                            else
                            {
                                // Preserve existing data if not modified
                                var pic = iconData.Pics[i];
                                if (pic.Offset >= 8 && pic.Size > 0 && originalBuffer.Length >= pic.Offset + pic.Size)
                                {
                                    byte[] existingData = new byte[pic.Size];
                                    Array.Copy(originalBuffer, pic.Offset, existingData, 0, pic.Size);

                                    lock (lockObj)
                                    {
                                        iconData.Pics[i] = new PicInfo(offset, pic.Size, pic.FileType);
                                        buffer.AddRange(existingData);
                                        offset += pic.Size;
                                    }

                                    Debug.WriteLine($"Preserved {iconName} {resKey} pic{i}: Offset={offset - pic.Size}, Size={pic.Size}, Type={pic.FileType}");
                                }
                                else
                                {
                                    iconData.Pics[i] = new PicInfo(0, 0);
                                }
                            }
                        }
                    }
                }

                // Write updated .dat files to working directory
                foreach (var kvp in buffers)
                {
                    string resKey = kvp.Key;
                    var (buffer, _, _) = kvp.Value;
                    string outputPath = outputDataFiles[resKey];

                    try
                    {
                        Debug.WriteLine($"Writing {outputPath}: {buffer.Count} bytes");
                        File.WriteAllBytes(outputPath, buffer.ToArray());
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error writing {outputPath}: {ex.Message}");
                        throw;
                    }
                }

                // Update copied IconResources.idx
                OutputIndexFile(workingDirectory);

                Debug.WriteLine($"Successfully modified {icons.Length} icons");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error modifying icon data for {icons.Length} icons: {ex.Message}");
                throw;
            }
        }

        #endregion
    }
  
}