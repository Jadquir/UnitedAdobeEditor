using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadUpdate.Class
{
    public class UpdateData
    {
        public string UpdateUrl { get; set; }
        public string DownloadLink { get; set; }
        public VersionList UpdateChangeLog { get; set; }
    }
    public class UpdateEventArgs : EventArgs
    {
        public bool UpdateAvailable { get; set; }
        public UpdateData UpdateData { get; set; }
    }
}
