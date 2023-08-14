using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitedAdobeEditor.Components.Enums;

namespace UnitedAdobeEditor.Components.Classes
{
    public class CurrentOperation
    {
        internal static bool isRunStat;
        internal static string runStatId;

        public static AdobeType AppType { get; set; }
        public static SelectedPath SelectedPath { get; set; }

        public static OperationType operationType { get; set; }

        public static bool IsConfigActivated { get; set; }
        public static Operation Operation { get; set; }
    }
    public class Operation
    {
        public AdobeType AppType { get; set; }

        public OperationType operationType { get; set; }
        public Image SplashScreen { get; set; }
    }
}
