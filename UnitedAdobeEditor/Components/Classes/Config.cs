using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UnitedAdobeEditor.Components.Enums;

namespace UnitedAdobeEditor.Components.Classes
{
    public class Config
    {
        public string app_type;
        public string image_base64;
        public string image;
        public bool is_silent;
        public bool closeAfterChanging;
        public string selected_folder;

        public virtual Image? GetImage()
        {
            return Misc.ImageFromBase64String(image ?? image_base64);
        }
        public Operation? ToOperation(out string error)
        {
            error = "";
            var AppType = GetAppType();
            if(!AppType .HasValue)
            {
                error = "Invalid Adobe application Type!";
                return null;
            }
            var SplashScreen = GetImage();
            if (SplashScreen is null)
            {
                error = "Invalid Splash Screen Data!";
                return null;
            }
            return new Operation()
            {
                AppType = AppType.Value,
                operationType = Enums.OperationType.SplashScreen,
                SplashScreen = SplashScreen
            };
        }
        public static readonly Dictionary<string, AdobeType> appTypes = new Dictionary<string, AdobeType>()
        {
            { "ps" , AdobeType.Photoshop },
            { "ps_beta" , AdobeType.PhotoshopBeta },
            { "pr" ,AdobeType.PremierePro },
            { "ae" ,AdobeType.AfterEffects },
            { "ai" ,AdobeType.Illustrator },
            { "me" ,AdobeType.MediaEncoder },
            { "an" ,AdobeType.Animate },
            { "au" ,AdobeType.Audition },
            { "lr" ,AdobeType.Lightroom },
            { "lrc", AdobeType.LightroomClassic },
            { "id" ,AdobeType.InDesign },
            { "dw" ,AdobeType.Dreamweaver },
            { "ic" ,AdobeType.InCopy },
            { "ch" ,AdobeType.CharacterAnimator } 
        };
        private AdobeType? GetAppType()
        {
            if (appTypes.TryGetValue(app_type,out var adobeType))
            {
                return adobeType;
            }
            return null;
        }
    }
}
