using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadUpdate.Class
{
    public class Version
    {
        public DateTime VersionDate { get; set; }
        public VersionTag VersionTag { get; set; }

        public int[] version;
        public string ChangeLog { get; set; }
        public Version()
        {
        }
        public Version(string version, VersionTag versionTag, string changeLog)
        {
            string[] parts = version.Split('.');
            this.version = new int[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                this.version[i] = int.Parse(parts[i]);
            }
            VersionTag = versionTag;
            VersionDate = DateTime.Now;
            ChangeLog = changeLog;
        }
        public Version(string version, VersionTag versionTag)
        {
            string[] parts = version.Split('.');
            this.version = new int[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                this.version[i] = int.Parse(parts[i]);
            }
            VersionTag = versionTag;
            VersionDate = DateTime.Now;
        }

        public int GetVersionInt()
        {
            string text = "";
            for (int i = 0; i < version.Length; i++)
            {
                text += version[i].ToString();
            }
            // 1000000
            text = text.PadRight(7, '0');
            return int.Parse(text);
        }
        public string GetVersionText()
        {
            string text = "";
            for (int i = 0; i < version.Length; i++)
            {
                text += version[i].ToString();
                if (i != version.Length - 1)
                {
                    text += ".";
                }
            }
            return VersionTag == VersionTag.Release ? text : text + " " + VersionTag;
        }

        public bool IsNewer(Version version)
        {
            if (this.GetVersionInt() == version.GetVersionInt())
            {
                return (int)this.VersionTag > (int)version.VersionTag;
            }
            return this.GetVersionInt() > version.GetVersionInt();
        }
    }

}
