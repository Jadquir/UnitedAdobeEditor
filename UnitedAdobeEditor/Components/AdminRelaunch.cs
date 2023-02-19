using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UnitedAdobeEditor.Components
{
    internal class AdminRelaunch
    {
        private static bool IsRunAsAdmin()
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(id);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static void AdminRelauncher()
        {
            if (!IsRunAsAdmin())
            {
                ProcessStartInfo proc = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = Assembly.GetEntryAssembly()!.CodeBase,
                    Verb = "runas"
                };
                try
                {
                    Process.Start(proc);
                    Application.Current.Shutdown();

                    Process.GetCurrentProcess().Kill();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("This program must be run as an administrator! \n\n" + ex.ToString(), "Error");
                }
            }
        }
    }
}
