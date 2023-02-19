using JadUpdate.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitedAdobeEditor.Components.Enums;
using UnitedAdobeEditor.Components.SplashScreenChanger.ResourceHacker;

namespace UnitedAdobeEditor.Components.Classes.SplashScreenData
{
    public class Main
    {
        public class AdobeApp 
        {
            public AdobeApp(AdobeType adobeType,
                string file,
                ChangerType splashscreenChangerType,
                List<ImageDataBase> resourceData,
                string LogoName,
                string PathSelectorComment = null)
            {
                this.type = adobeType;
                this.FileName = file;
                this.LogoName = LogoName;
                this.SplashScreenChangerType = splashscreenChangerType;
                this.PathSelectorComment = PathSelectorComment;
                this.ResourceData = resourceData;
            }
            public AdobeType type;
            public string FileName;
            public string LogoName;
            public string PathSelectorComment;
            public ChangerType SplashScreenChangerType;
            public List<ImageDataBase> ResourceData;

        }
        public static List<AdobeApp> AdobeAppList = new List<AdobeApp>()
        {
            new AdobeApp(AdobeType.Photoshop,"Photoshop.exe",ChangerType.Photoshop,null,"photoshop.png",null),

            new AdobeApp(AdobeType.Illustrator,"Illustrator.exe",ChangerType.Normal,new List<ImageDataBase>()
                {
                    new NormalImageData("ai_cc_splash.png","",new System.Drawing.Size(750,500)),
                    new NormalImageData("ai_cc_splash@2x.png","",new System.Drawing.Size(1500,1000)),
                    new NormalImageData("ai_cc_splash@3to2x.png","",new System.Drawing.Size(1125,750))
                },"illustrator.png",null),
            new AdobeApp(AdobeType.PremierePro,"Adobe Premiere Pro.exe",ChangerType.Normal,new List<ImageDataBase>()
                {
                    new NormalImageData("pr_splash.png","PNG",new System.Drawing.Size(750,500)),
                    new NormalImageData("pr_splash@2x.png","PNG",new System.Drawing.Size(1500,1000)),
                    new NormalImageData("pr_splash@3to2x.png","PNG",new System.Drawing.Size(1125,750))
                },"premiere_pro.png",null),
            new AdobeApp(AdobeType.InDesign,"InDesign.exe",ChangerType.Normal, new List<ImageDataBase>()
                {
                    new NormalImageData("400.idrc","(InDesign Resources)\\idrc_PNGA",new System.Drawing.Size(750,500)),
                    new NormalImageData("6400.idrc","(InDesign Resources)\\idrc_PNGA",new System.Drawing.Size(1500,1000)),
                    new NormalImageData("12400.idrc","(InDesign Resources)\\idrc_PNGA",new System.Drawing.Size(1125,750))
                },"indesign.png",null),
            new AdobeApp(AdobeType.InCopy,"InCopy.exe",ChangerType.Normal, new List<ImageDataBase>()
                {
                    new NormalImageData("400.idrc","(InCopy Resources)\\idrc_PNGA",new System.Drawing.Size(750,500)),
                    new NormalImageData("6400.idrc","(InCopy Resources)\\idrc_PNGA",new System.Drawing.Size(1500,1000)),
                    new NormalImageData("12400.idrc","(InCopy Resources)\\idrc_PNGA",new System.Drawing.Size(1125,750))
                },"incopy.png",null),

            new AdobeApp(AdobeType.AfterEffects,"AfterFXLib.dll",ChangerType.ResourceHacker,new List<ImageDataBase>()
                {
                    new ResourceImageData("AE_SPLASH","PNG",new System.Drawing.Size(750,500)),
                    new ResourceImageData("AE_SPLASH_AT_2X","PNG",new System.Drawing.Size(1500,1000)),
                    new ResourceImageData("AE_SPLASH_AT_3TO2X","PNG",new System.Drawing.Size(1125,750)),
                },"after_effects.png","This file should be in where AfterFX.exe located." ),
            new AdobeApp(AdobeType.MediaEncoder,"Adobe Media Encoder.exe",ChangerType.ResourceHacker,new List<ImageDataBase>()
                {
                    new ResourceImageData("LAUNCHAMEBACKGROUND","PNG",new System.Drawing.Size(750,500)),
                    new ResourceImageData("LAUNCHAMEBACKGROUND_AT_3TO2X","PNG",new System.Drawing.Size(1125,750)),
                    new ResourceImageData("LAUNCHAMEBACKGROUND_AT_2X","PNG",new System.Drawing.Size(1500,1000)),
                },"mediaencoder.png",null),
            new AdobeApp(AdobeType.Animate,"Animate.exe",ChangerType.ResourceHacker,new List<ImageDataBase>()
                {
                    new ResourceImageData("AN_SPLASH","PNG",new System.Drawing.Size(750,500)),
                    new ResourceImageData("AN_SPLASH_AT_2X","PNG",new System.Drawing.Size(1500,1000)),
                    new ResourceImageData("AN_SPLASH_AT_3TO2X","PNG",new System.Drawing.Size(1125,750)),
                },"animate.png",null),
            new AdobeApp(AdobeType.CharacterAnimator,"Character Animator.exe",ChangerType.ResourceHacker,new List<ImageDataBase>()
                {
                    new ResourceImageData("CH_SPLASH","PNG",new System.Drawing.Size(750,500)),
                    new ResourceImageData("CH_SPLASH_AT_2X","PNG",new System.Drawing.Size(1500,1000)),
                    new ResourceImageData("CH_SPLASH_AT_3TO2X","PNG",new System.Drawing.Size(1125,750)),
                },"ch_animator.png",null),
            new AdobeApp(AdobeType.Audition,"AuUI.dll",ChangerType.ResourceHacker,new List<ImageDataBase>()
                {
                    new ResourceImageData("AU_SPLASH","PNG",new System.Drawing.Size(750,500)),
                    new ResourceImageData("AU_SPLASH_AT_3TO2X","PNG",new System.Drawing.Size(1125,750)),
                    new ResourceImageData("AU_SPLASH_AT_2X","PNG",new System.Drawing.Size(1500,1000)),
                },"audition.png","This file should be in where Audition.exe located."),
            new AdobeApp(AdobeType.Lightroom,"lightroom.exe",ChangerType.ResourceHacker, new List<ImageDataBase>()
                {
                    new ResourceImageData("LR_SPLASH.PNG","PNG",new System.Drawing.Size(750,500)),
                    new ResourceImageData("LR_SPLASH~1.5X.PNG","PNG",new System.Drawing.Size(1125,750)),
                    new ResourceImageData("LR_SPLASH~2.5X.PNG","PNG",new System.Drawing.Size(1875,1250)),
                    new ResourceImageData("LR_SPLASH~2X.PNG","PNG",new System.Drawing.Size(1500,1000)),
                },"lightroom.png",null),
            new AdobeApp(AdobeType.LightroomClassic,"Lightroom.exe",ChangerType.ResourceHacker,new List<ImageDataBase>()
                {
                    new ResourceImageData("LRC_SPLASH.PNG","PNG",new System.Drawing.Size(750,500)),
                    new ResourceImageData("LRC_SPLASH~1.5X.PNG","PNG",new System.Drawing.Size(1125,750)),
                    new ResourceImageData("LRC_SPLASH~2.5X.PNG","PNG",new System.Drawing.Size(1875,1250)),
                    new ResourceImageData("LRC_SPLASH~2X.PNG","PNG",new System.Drawing.Size(1500,1000)),
                },"lightroomclassic.png",null),
            new AdobeApp(AdobeType.Dreamweaver,"Resources.dll",ChangerType.ResourceHacker,  new List<ImageDataBase>()
                {
                    new ResourceImageData("SPLASHNORMAL","PNG",new System.Drawing.Size(750,500)),
                    new ResourceImageData("SPLASHNORMAL_AT_3TO2X","PNG",new System.Drawing.Size(1125,750)),
                    new ResourceImageData("SPLASHNORMAL_AT_2X","PNG",new System.Drawing.Size(1500,1000)),
                },"dreamweaver.png","This file should be in [Installed Directory]/[Your locale (ex. en_US)]/Resources/."),

        };

        public static AdobeApp? Get(AdobeType adobeType)
        {
            AdobeApp? app = AdobeAppList.Find(x => x.type == adobeType);
            if (app == null)
            {
                MessageBoxJ.ShowOK("There is not splash screen data for this application. Please contact with the developer.");
                return null;
            }
            return app;
        }


        /*

        public static Dictionary<AdobeType, string> FileNames = new Dictionary<AdobeType, string>()
        {
            {AdobeType.Photoshop,"Photoshop.exe" },
            {AdobeType.Illustrator,"Illustrator.exe" },
            {AdobeType.PremierePro,"Adobe Premiere Pro.exe" },
            {AdobeType.Animate, "Animate.exe" },
            {AdobeType.AfterEffects,"AfterFXLib.dll" },
            {AdobeType.LightroomClassic,"Lightroom.exe" },
            {AdobeType.Lightroom,"lightroom.exe" },
            {AdobeType.MediaEncoder,"Adobe Media Encoder.exe" },
            {AdobeType.Audition,"AuUI.dll" },
            {AdobeType.InDesign,"InDesign.exe" },
            {AdobeType.Dreamweaver,"Resources.dll" },
        };
        public static Dictionary<AdobeType, ChangerType> SplashScreenChangerType = new Dictionary<AdobeType, ChangerType>()
        {
            {AdobeType.Photoshop, ChangerType.Photoshop },

            {AdobeType.Illustrator,ChangerType.Normal},
            {AdobeType.PremierePro,ChangerType.Normal },
            {AdobeType.InDesign,ChangerType.Normal},

            {AdobeType.Animate, ChangerType.ResourceHacker },
            {AdobeType.AfterEffects,ChangerType.ResourceHacker },
            {AdobeType.LightroomClassic,ChangerType.ResourceHacker },
            {AdobeType.Lightroom,ChangerType.ResourceHacker},
            {AdobeType.MediaEncoder, ChangerType.ResourceHacker},
            {AdobeType.Audition,ChangerType.ResourceHacker},
            {AdobeType.Dreamweaver,ChangerType.ResourceHacker},
        }; 
        public static Dictionary<AdobeType, string> PathSelectorComments = new Dictionary<AdobeType, string>()
        {
            {AdobeType.AfterEffects,"This file should be in where AfterFX.exe located." },
            {AdobeType.Audition,"This file should be in where Audition.exe located." },
            {AdobeType.Dreamweaver,"This file should be in [Installed Directory]/[Your locale (ex. en_US)]/Resources/." },
        };

        public static Dictionary<AdobeType, List<NormalImageData>> NormalDataKeys = new Dictionary<AdobeType, List<NormalImageData>>()
        {
            {
                AdobeType.PremierePro ,
                new List<NormalImageData>()
                {
                    new NormalImageData("pr_splash.png","PNG",new System.Drawing.Size(750,500)),
                    new NormalImageData("pr_splash@2x.png","PNG",new System.Drawing.Size(1500,1000)),
                    new NormalImageData("pr_splash@3to2x.png","PNG",new System.Drawing.Size(1125,750))
                }
            },
            {
                AdobeType.Illustrator,
                new List<NormalImageData>()
                {
                    new NormalImageData("ai_cc_splash.png","",new System.Drawing.Size(750,500)),
                    new NormalImageData("ai_cc_splash@2x.png","",new System.Drawing.Size(1500,1000)),
                    new NormalImageData("ai_cc_splash@3to2x.png","",new System.Drawing.Size(1125,750))
                }
            },
            {
                AdobeType.InDesign ,
                new List<NormalImageData>()
                {
                    new NormalImageData("400.idrc","(InDesign Resources)\\idrc_PNGA",new System.Drawing.Size(750,500)),
                    new NormalImageData("6400.idrc","(InDesign Resources)\\idrc_PNGA",new System.Drawing.Size(1500,1000)),
                    new NormalImageData("12400.idrc","(InDesign Resources)\\idrc_PNGA",new System.Drawing.Size(1125,750))
                }
            }
        };

       public static Dictionary<AdobeType, List<ResourceImageData>> ResourceDataKeys = new Dictionary<AdobeType, List<ResourceImageData>>()
        {
            {
                AdobeType.Animate ,
                new List<ResourceImageData>()
                {
                    new ResourceImageData("AN_SPLASH","PNG",new System.Drawing.Size(750,500)),
                    new ResourceImageData("AN_SPLASH_AT_2X","PNG",new System.Drawing.Size(1500,1000)),
                    new ResourceImageData("AN_SPLASH_AT_3TO2X","PNG",new System.Drawing.Size(1125,750)),
                }
            },
            {
                AdobeType.AfterEffects ,
                new List<ResourceImageData>()
                {
                    new ResourceImageData("AE_SPLASH","PNG",new System.Drawing.Size(750,500)),
                    new ResourceImageData("AE_SPLASH_AT_2X","PNG",new System.Drawing.Size(1500,1000)),
                    new ResourceImageData("AE_SPLASH_AT_3TO2X","PNG",new System.Drawing.Size(1125,750)),
                }
            },
            {
                AdobeType.Lightroom ,
                new List<ResourceImageData>()
                {
                    new ResourceImageData("LR_SPLASH.PNG","PNG",new System.Drawing.Size(750,500)),
                    new ResourceImageData("LR_SPLASH~1.5X.PNG","PNG",new System.Drawing.Size(1125,750)),
                    new ResourceImageData("LR_SPLASH~2.5X.PNG","PNG",new System.Drawing.Size(1875,1250)),
                    new ResourceImageData("LR_SPLASH~2X.PNG","PNG",new System.Drawing.Size(1500,1000)),
                }
            },
            {
                AdobeType.LightroomClassic ,
                new List<ResourceImageData>()
                {
                    new ResourceImageData("LRC_SPLASH.PNG","PNG",new System.Drawing.Size(750,500)),
                    new ResourceImageData("LRC_SPLASH~1.5X.PNG","PNG",new System.Drawing.Size(1125,750)),
                    new ResourceImageData("LRC_SPLASH~2.5X.PNG","PNG",new System.Drawing.Size(1875,1250)),
                    new ResourceImageData("LRC_SPLASH~2X.PNG","PNG",new System.Drawing.Size(1500,1000)),
                }
            },
            {
                AdobeType.MediaEncoder ,
                new List<ResourceImageData>()
                {
                    new ResourceImageData("LAUNCHAMEBACKGROUND","PNG",new System.Drawing.Size(750,500)),
                    new ResourceImageData("LAUNCHAMEBACKGROUND_AT_3TO2X","PNG",new System.Drawing.Size(1125,750)),
                    new ResourceImageData("LAUNCHAMEBACKGROUND_AT_2X","PNG",new System.Drawing.Size(1500,1000)),
                }
            },
            {
                AdobeType.Audition,
                new List<ResourceImageData>()
                {
                    new ResourceImageData("AU_SPLASH","PNG",new System.Drawing.Size(750,500)),
                    new ResourceImageData("AU_SPLASH_AT_3TO2X","PNG",new System.Drawing.Size(1125,750)),
                    new ResourceImageData("AU_SPLASH_AT_2X","PNG",new System.Drawing.Size(1500,1000)),
                }
            },
            {
                AdobeType.Dreamweaver,
                new List<ResourceImageData>()
                {
                    new ResourceImageData("SPLASHNORMAL","PNG",new System.Drawing.Size(750,500)),
                    new ResourceImageData("SPLASHNORMAL_AT_3TO2X","PNG",new System.Drawing.Size(1125,750)),
                    new ResourceImageData("SPLASHNORMAL_AT_2X","PNG",new System.Drawing.Size(1500,1000)),
                }
            }
        };

     */
    }
}
