using System;
using System.IO;
using System.Web;

namespace ServerApp.Services
{
    public class HttpFileStore
    {
        string path;
        public HttpFileStore()
        {
            path = HttpContext.Current.Server.MapPath("~/App_Data/");
        }

        private string GetPath(string guid)
        {
            return string.Format("{0}/{1}.tmp", path, guid);
        }

        public string PutFile(HttpPostedFile file)
        {
            string guid, newPathFile;
            do
            {
                guid = Guid.NewGuid().ToString();
                newPathFile = GetPath(guid);
            } while (System.IO.File.Exists(newPathFile));

            file.SaveAs(newPathFile);
            return guid;
        }

        public FileStream GetFileStream(string guid)
        {
            var pathToFile = GetPath(guid);

            if (System.IO.File.Exists(pathToFile))
            {
                var fs = System.IO.File.Open(pathToFile, System.IO.FileMode.Open);
                return fs;
            }
            return null;
        }

        public void Delete(string guid)
        {
            var pathToFile = GetPath(path);
            if (System.IO.File.Exists(pathToFile))
            {
                System.IO.File.Delete(pathToFile);
            }
        }
    }
}