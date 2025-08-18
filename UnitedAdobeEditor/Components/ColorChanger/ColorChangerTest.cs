using PhotoshopExtractorByJadquir;
using PhotoshopExtractorByJadquir.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitedAdobeEditor.Components.Classes.SplashScreenData;

namespace UnitedAdobeEditor.Components.ColorChanger
{
    internal class ColorChangerTest
    {
        //static IEnumerable<string> GetPossibleNameList(List<ImageDataBase> resourceData)
        //{
        //    var nameList = new List<string>();
        //    foreach (var item in resourceData)
        //    {
        //        var data = (ResourceImageData)item;
        //        if (data == null) { continue; }
        //        nameList.Add(data.Name);
        //        foreach (var name in data.PossibleNames)
        //        {
        //            nameList.Add(name);
        //            break;
        //        }
        //    }
        //    return nameList.Distinct().Select(x => x.Replace("_s0", "").Replace(".png", ""));
        //}
        //public static void Test()
        //{
        //    string workingDirectory = Path.Combine("C:\\Users\\Acer\\Desktop\\PhotoshopTest\\Working");
        //    string idxPath = Path.Combine("C:\\Users\\Acer\\Desktop\\PhotoshopTest\\Original", "IconResources.idx");
        //    var handler = new IconResources(idxPath);

        //    var names = GetPossibleNameList(Classes.SplashScreenData.Main.Get(Enums.AdobeType.Photoshop).ResourceData);

        //    Image testImage = Image.FromFile(@"C:\Users\Acer\AppData\Roaming\Jadquir\United Adobe Editor\Cache\rRYOHbb51gs.png");


        //    ResourceInfo? info = null;
        //    foreach (var name in names)
        //    {
        //        info = handler.GetResourceInfo(name);
        //        if (info == null) continue;
        //        break;
        //    }
        //    var imageCache = new Dictionary<(int width, int height), byte[]>();
        //    if(info != null)
        //    {
        //        var changeMap1 = new List<ModifyIconInstance>();
        //        var resourceInfo = handler.GetResourceInfo(info.IconName);
        //        var changeinstance = handler.CreateModifyInstance(info.IconName);
        //        foreach (var key in resourceInfo.Resolutions.Keys)
        //        {
        //            var resolution = resourceInfo.Resolutions[key];
        //            if(resolution.Width == 0 || resolution.Height == 0) { continue; }

        //            // Resize Image
        //            byte[]? image = null;

        //            if(!imageCache.TryGetValue((resolution.Width, resolution.Height), out image))
        //            {
        //                image = testImage.ResizeImage(resolution.Width, resolution.Height).ToByteArray();
        //                imageCache[(resolution.Width, resolution.Height)] = image;
        //            }
        //            // Probably always zero but just to make sure
        //            var indexes = resolution.GetValidPicIndexes();
        //            foreach (var index in indexes)
        //            {
        //                changeinstance.AddPicData(key, index, image);
        //            }

        //            changeMap1.Add(changeinstance);
        //        }
        //        handler.ModifyIconDataEnhanced(workingDirectory, changeMap1.ToArray());
        //    }
           
        //}
    }
}
