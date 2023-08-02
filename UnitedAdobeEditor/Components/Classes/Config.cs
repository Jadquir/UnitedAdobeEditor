using System;
using System.Collections.Generic;
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

        public Operation? ToOperation(out string error)
        {
            error = "";
            var AppType = GetAppType();
            if(!AppType .HasValue)
            {
                error = "Invalid Adobe application Type!";
                return null;
            }
            var SplashScreen = Misc.ImageFromBase64String(image ?? image_base64);
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

        private AdobeType? GetAppType()
        {
            return app_type switch
            {
                "ps" => AdobeType.Photoshop,
                "pr" => AdobeType.PremierePro,
                "ae" => AdobeType.AfterEffects,
                "ai" => AdobeType.Illustrator,
                "me" => AdobeType.MediaEncoder,
                "an" => AdobeType.Animate,
                "au" => AdobeType.Audition,
                "lr" => AdobeType.Lightroom,
                "lrc" => AdobeType.LightroomClassic,
                "id" => AdobeType.InDesign,
                "dw" => AdobeType.Dreamweaver,
                "ic" => AdobeType.InCopy,
                "ch" => AdobeType.CharacterAnimator,
                _ => null
            };
        }
    }
}
