using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadUpdate.Class
{
    public class VersionList : List<Version>
    {
        public Version GetLatest()
        {
            Version version = this.FirstOrDefault();
            for (int i = 0; i < this.Count; i++)
            {
                if (version?.GetVersionInt() < this[i].GetVersionInt())
                {
                    version = this[i];
                }
            }
            return version;
        }
    }

}
