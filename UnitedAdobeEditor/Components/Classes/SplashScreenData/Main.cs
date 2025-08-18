using JadUpdate.Class;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnitedAdobeEditor.Components.Enums;
using UnitedAdobeEditor.Components.SplashScreenChanger.ResourceHacker;

namespace UnitedAdobeEditor.Components.Classes.SplashScreenData
{
    public class Main
    {
        /// <summary>
        /// Represents an Adobe application with its splash screen metadata.
        /// </summary>
        public class AdobeApp
        {
            public AdobeApp(
                AdobeType adobeType,
                string file,
                ChangerType splashscreenChangerType,
                List<ImageDataBase> resourceData,
                string logoName,
                string? pathSelectorComment = null
                )
            {
                Type = adobeType;
                FileName = file;
                SplashScreenChangerType = splashscreenChangerType;
                ResourceData = resourceData;
                LogoName = logoName;
                PathSelectorComment = pathSelectorComment;
                this.HasDarkVariant = ResourceData.Any(x => x.HasDarkVariant());
            }

            public AdobeType Type { get; }
            public string FileName { get; }
            public bool HasDarkVariant { get; }
            public string LogoName { get; }
            public string? PathSelectorComment { get; }
            public ChangerType SplashScreenChangerType { get; }
            public List<ImageDataBase> ResourceData { get; }
        }

        /// <summary>
        /// Static list containing all Adobe application configurations.
        /// </summary>
        private static readonly List<AdobeApp> AdobeAppList = new()
        {
            // ----------------- Photoshop -----------------
            new AdobeApp(
                AdobeType.Photoshop,
                "Photoshop.exe",
                ChangerType.Photoshop,
                new List<ImageDataBase>
                {
                    new ResourceImageData("SplashBackground_s0.png", "High", new Size(1500, 1000), "Splash1080Background_s0.png", "SplashBackground_pic0_s"),
                    new ResourceImageData("SplashBackground_s0.png", "Low",  new Size(750,  500),  "Splash1080Background_s0.png", "SplashBackground_pic0_s"),
                },
                "photoshop.png"
            ),

            // ----------------- Photoshop Beta -----------------
            new AdobeApp(
                AdobeType.PhotoshopBeta,
                "Photoshop.exe",
                ChangerType.Photoshop,
                new List<ImageDataBase>
                {
                    new ResourceImageData("SplashAlt_s0.png", "High", new Size(1500, 1000)),
                    new ResourceImageData("SplashAlt_s0.png", "Low",  new Size(750,  500)),
                },
                "photoshop_beta.png"
            ),

            // ----------------- Illustrator ----------------- checked
            new AdobeApp(
                AdobeType.Illustrator,
                "Illustrator.exe",
                ChangerType.Normal,
                new List<ImageDataBase>
                {
                    // Legacy
                    new NormalImageData("ai_cc_splash.png",       "", new Size(750,  500)),
                    new NormalImageData("ai_cc_splash@2x.png",    "", new Size(1500, 1000)),
                    new NormalImageData("ai_cc_splash@3to2x.png", "", new Size(1125, 750)),

                    // 2025 version
                    new NormalImageData("ai_cc_splash.png",       "\\Support Files\\Contents\\Windows", new Size(750,  500)),
                    new NormalImageData("ai_cc_splash@2x.png",    "\\Support Files\\Contents\\Windows", new Size(1500, 1000)),
                    new NormalImageData("ai_cc_splash@3to2x.png", "\\Support Files\\Contents\\Windows", new Size(1125, 750)),
                },
                "illustrator.png"
            ),

            // ----------------- Premiere Pro ----------------- checked
            new AdobeApp(
                AdobeType.PremierePro,
                "Adobe Premiere Pro.exe",
                ChangerType.Normal,
                new List<ImageDataBase>
                {
                    new NormalImageData("pr_splash.png",       "PNG", new Size(750,  500)),
                    new NormalImageData("pr_splash@2x.png",    "PNG", new Size(1500, 1000)),
                    new NormalImageData("pr_splash@3to2x.png", "PNG", new Size(1125, 750)),
                },
                "premiere_pro.png"
            ),

            // ----------------- InDesign ----------------- checked
            new AdobeApp(
                AdobeType.InDesign,
                "InDesign.exe",
                ChangerType.Normal,
                new List<ImageDataBase>
                {
                    new NormalImageData("400.idrc",   "(InDesign Resources)\\idrc_PNGA", new Size(750,  500)),
                    new NormalImageData("6400.idrc",  "(InDesign Resources)\\idrc_PNGA", new Size(1500, 1000)),
                    new NormalImageData("12400.idrc", "(InDesign Resources)\\idrc_PNGA", new Size(1125, 750)),
                },
                "indesign.png"
            ),

            // ----------------- InCopy ----------------- checked
            new AdobeApp(
                AdobeType.InCopy,
                "InCopy.exe",
                ChangerType.Normal,
                new List<ImageDataBase>
                {
                    new NormalImageData("400.idrc",   "(InCopy Resources)\\idrc_PNGA", new Size(750,  500)),
                    new NormalImageData("6400.idrc",  "(InCopy Resources)\\idrc_PNGA", new Size(1500, 1000)),
                    new NormalImageData("12400.idrc", "(InCopy Resources)\\idrc_PNGA", new Size(1125, 750)),
                },
                "incopy.png"
            ),

            // ----------------- After Effects ----------------- 
            new AdobeApp(
                AdobeType.AfterEffects,
                "AfterFXLib.dll",
                ChangerType.ResourceHacker,
                new List<ImageDataBase>
                {
                    new ResourceImageData("AE_SPLASH",          "PNG", new Size(750,  500)),
                    new ResourceImageData("AE_SPLASH_AT_2X",    "PNG", new Size(1500, 1000)),
                    new ResourceImageData("AE_SPLASH_AT_3TO2X", "PNG", new Size(1125, 750)),
                },
                "after_effects.png",
                "This file should be in where AfterFX.exe is located."
            ),

            // ----------------- Media Encoder ----------------- checked
            new AdobeApp(
                AdobeType.MediaEncoder,
                "Adobe Media Encoder.exe",
                ChangerType.ResourceHacker,
                new List<ImageDataBase>
                {
                    new ResourceImageData("LAUNCHAMEBACKGROUND",          "PNG", new Size(750,  500)),
                    new ResourceImageData("LAUNCHAMEBACKGROUND_AT_3TO2X", "PNG", new Size(1125, 750)),
                    new ResourceImageData("LAUNCHAMEBACKGROUND_AT_2X",    "PNG", new Size(1500, 1000)),
                },
                "mediaencoder.png"
            ),

            // ----------------- Animate -----------------
            new AdobeApp(
                AdobeType.Animate,
                "Animate.exe",
                ChangerType.ResourceHacker,
                new List<ImageDataBase>
                {
                    new ResourceImageData("AN_SPLASH",          "PNG", new Size(750,  500)),
                    new ResourceImageData("AN_SPLASH_AT_2X",    "PNG", new Size(1500, 1000)),
                    new ResourceImageData("AN_SPLASH_AT_3TO2X", "PNG", new Size(1125, 750)),
                },
                "animate.png"
            ),

            // ----------------- Character Animator ----------------- checked 
            new AdobeApp(
                AdobeType.CharacterAnimator,
                "Character Animator.exe",
                ChangerType.ResourceHacker,
                new List<ImageDataBase>
                {
                    new ResourceImageData("CH_SPLASH",          "PNG", new Size(750,  500)).AddDarkVariants("CH_SPLASH_DARK"),
                    new ResourceImageData("CH_SPLASH_AT_2X",    "PNG", new Size(1500, 1000)).AddDarkVariants("CH_SPLASH_DARK_AT_2X"),
                    new ResourceImageData("CH_SPLASH_AT_3TO2X", "PNG", new Size(1125, 750)).AddDarkVariants("CH_SPLASH_DARK_AT_3TO2X"),
                },
                "ch_animator.png"
            ),

            // ----------------- Audition -----------------  checked
            new AdobeApp(
                AdobeType.Audition,
                "AuUI.dll",
                ChangerType.ResourceHacker,
                new List<ImageDataBase>
                {
                    new ResourceImageData(
                        "AU_SPLASH","PNG", new Size(750,  500),
                        "AU2023_SPLASH", "AU2024_SPLASH","AU2025_SPLASH_LIGHT")
                    .AddDarkVariants("AU2025_SPLASH_DARK")
                    ,
                    new ResourceImageData("AU_SPLASH_AT_3TO2X",    "PNG", new Size(1125, 750), 
                        "AU2023_SPLASH_AT_3TO2X", "AU2024_SPLASH_AT_3TO2X", "AU2025_SPLASH_LIGHT_AT_3TO2X")
                    .AddDarkVariants("AU2025_SPLASH_DARK_AT_3TO2X"),
                    new ResourceImageData("AU_SPLASH_AT_2X",       "PNG", new Size(1500, 1000),
                        "AU2023_SPLASH_AT_2X","AU2024_SPLASH_AT_2X", "AU2025_SPLASH_LIGHT_AT_2X")
                    .AddDarkVariants("AU2025_SPLASH_DARK_AT_2X"),
                },
                "audition.png",
                "This file should be in where Audition.exe is located."
            ),

            // ----------------- Lightroom ----------------- checked
            new AdobeApp(
                AdobeType.Lightroom,
                "lightroom.exe",
                ChangerType.ResourceHacker,
                new List<ImageDataBase>
                {
                    new ResourceImageData("LR_SPLASH.PNG",       "PNG", new Size(750,  500)),
                    new ResourceImageData("LR_SPLASH~1.5X.PNG",  "PNG", new Size(1125, 750)),
                    new ResourceImageData("LR_SPLASH~2.5X.PNG",  "PNG", new Size(1875, 1250)),
                    new ResourceImageData("LR_SPLASH~2X.PNG",    "PNG", new Size(1500, 1000)),
                },
                "lightroom.png"
            ),

            // ----------------- Lightroom Classic ----------------- checked
            new AdobeApp(
                AdobeType.LightroomClassic,
                "Lightroom.exe",
                ChangerType.ResourceHacker,
                new List<ImageDataBase>
                {
                    new ResourceImageData("LRC_SPLASH.PNG",       "PNG", new Size(750,  500)),
                    new ResourceImageData("LRC_SPLASH~1.5X.PNG",  "PNG", new Size(1125, 750)),
                    new ResourceImageData("LRC_SPLASH~2.5X.PNG",  "PNG", new Size(1875, 1250)),
                    new ResourceImageData("LRC_SPLASH~2X.PNG",    "PNG", new Size(1500, 1000)),
                },
                "lightroomclassic.png"
            ),

            // ----------------- Dreamweaver -----------------  checked
            new AdobeApp(
                AdobeType.Dreamweaver,
                "Resources.dll",
                ChangerType.ResourceHacker,
                new List<ImageDataBase>
                {
                    new ResourceImageData("SPLASHNORMAL",          "PNG", new Size(750,  500)),
                    new ResourceImageData("SPLASHNORMAL_AT_3TO2X", "PNG", new Size(1125, 750)),
                    new ResourceImageData("SPLASHNORMAL_AT_2X",    "PNG", new Size(1500, 1000)),
                },
                "dreamweaver.png",
                "This file should be in [Installed Directory]/[Your locale (ex. en_US)]/Resources/."
            ),
        };

        /// <summary>
        /// Retrieve an Adobe application configuration by type.
        /// </summary>
        public static AdobeApp? Get(AdobeType adobeType)
        {
            var app = AdobeAppList.Find(x => x.Type == adobeType);

            if (app == null)
            {
                MessageBoxJ.ShowOK("No splash screen data found for this application. Please contact the developer.");
                return null;
            }

            return app;
        }
    }
}
