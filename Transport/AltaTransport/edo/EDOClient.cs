using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AltaTransport
{
    /// <summary>
    /// Client to EDO
    /// </summary>
    public class EDOClient
    {
        //private const string Path = @"C:\EDIMail\";

        internal string[] Files { get; private set; }
        private bool _has;

        internal string Path { get; set; }

        internal EDOClient(string path)
        {
            Path = path;
        }


        /// <summary>
        /// Returns new files from EDO.
        /// </summary>
        /// <returns></returns>
        internal List<FileInfo> GetNew()
        {
            if (!_has && !HasNew()) return new List<FileInfo>();
            return Files.Select(file => new FileInfo(file)).ToList();
        }


        /// <summary>
        /// Checks if new files exist in EDO.
        /// </summary>
        /// <returns></returns>
        internal bool HasNew()
        {
            LoadFilesFromEDO();
            _has = (Files != null && Files.Length > 0);
            return _has;
        }

        //private static string[] LoadFilesFromEDO()
        //{
        //    if (Directory.Exists(path)) {
        //        string[] filenames = Directory.GetFiles(path, "IPO_RPT_*.xml");
        //        return filenames;
        //    }
        //    return new string[0];
        //}


        private void LoadFilesFromEDO()
        {
            Files = null;
            if (!Directory.Exists(Path)) return;
            Files = Directory.GetFiles(Path, "IPO_RPT_2");
        }


    }
}
