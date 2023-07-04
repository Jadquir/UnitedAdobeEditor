using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitedAdobeEditor.Components.SplashScreenCreator
{
//    public class Creator
//    {
//        public static Image BackgroundImage = Image.FromFile("./Images/SplashScreenCreator/Background.png");
//        public static Image CreativeCloudLogo = Image.FromFile("./Images/SplashScreenCreator/Creative Cloud Logo.png");
//        public static Image Merge(Image CustomImage, Image AdobeLogo,Color BackgroundColor, bool adobeVisible,bool ccVisible)
//        {
//            Bitmap final = new Bitmap(BackgroundImage);

//            var bg = final
//                    .ColorReplace(10, Color.White, BackgroundColor)
//                    .RoundCorners(15);
//            var adobeLogo = new Bitmap(AdobeLogo);
//            var creativecloudLogo = new Bitmap(CreativeCloudLogo);
//            var customImage = new Bitmap(CustomImage).RoundCorners(15);

//            using (Graphics gr = Graphics.FromImage(final))
//            {
//                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
//                gr.DrawImage(bg, new Point(0, 0));
//                if (adobeVisible)
//                {
//                    gr.DrawImage(adobeLogo, new Point(64, 64));
//                }
//                if (ccVisible)
//                {
//                    gr.DrawImage(creativecloudLogo, new Point(64, 888));
//                }
//                gr.DrawImage(customImage, new Point(640, 32));
//            }

//            adobeLogo.Dispose();
//            creativecloudLogo.Dispose();
//            customImage.Dispose();

//            return final;
//        }
//    }

}
