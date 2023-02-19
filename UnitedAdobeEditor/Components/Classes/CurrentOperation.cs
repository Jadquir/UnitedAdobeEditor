using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitedAdobeEditor.Components.Enums;

namespace UnitedAdobeEditor.Components.Classes
{
    public class CurrentOperation
    {
        public static AdobeType AppType { get; set; }
        public static SelectedPath SelectedPath { get; set; }  

        public static OperationType operationType { get; set; }
    }
}
