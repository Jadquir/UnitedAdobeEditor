
using JadUpdate.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Version = JadUpdate.Class.Version;

namespace UnitedAdobeEditor.Components.Scripts
{
    public class UpdateChecker
    {
        public static UpdateEventArgs CurrentUpdate => JadUpdate.JadUpdate.CurrentUpdate;
        public static Version Version => JadUpdate.JadUpdate.LatestVersion;
        public static bool IsCheckingUpdates => JadUpdate.JadUpdate.IsCheckingUpdates;
        public static void Check(EventHandler<UpdateEventArgs> OnUpdateCheckCompleted = null)
        {
            JadUpdate.JadUpdate.UpdateUrl = App.UpdateUrl;
            JadUpdate.JadUpdate.CurrentVersion = App.Version;
            JadUpdate.JadUpdate.Check(OnUpdateCheckCompleted);
        }
    }
}
