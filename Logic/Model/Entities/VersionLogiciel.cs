using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public struct VersionLogiciel
    {
        public string Version { get; private set; }
        public string Changelog { get; private set; }
        public string DownloadLink { get; private set; }
        public DateTime DatePublication { get; private set; }

        public VersionLogiciel(string version, string changeLog, string downloadLink, DateTime datePublication)
            :this()
        {
            Version = version;
            Changelog = changeLog;
            DownloadLink = downloadLink;
            DatePublication = datePublication;
        }
    }
}
