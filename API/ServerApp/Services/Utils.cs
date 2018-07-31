using AltaArchiveApp;
using AltaBO;
using System;
using System.Web;

namespace ServerApp.Services
{
    public static class Utils
    {
        public static string GenerateFilePath(this DocumentRequisite requisite)
        {
            return GenerateFilePath(requisite.fileName);
        }

        public static string GenerateFilePath(string fileName)
        {
            string tempFolder = HttpContext.Current.Server.MapPath("~/App_Data");
            return string.Format("{0}/{1}_{2}", tempFolder, Guid.NewGuid(), fileName);
        }

        public static string SaveInTemplateFolder(this HttpPostedFile fileStream) {
            var pathToFile = GenerateFilePath(fileStream.FileName);
            fileStream.SaveAs(pathToFile);
            return pathToFile;
        }

        public static string LoadTemplateToLocalStorage(this ArchiveManager archiveManager, DocumentRequisite requisite)
        {
            var localPath = GenerateFilePath(requisite);
            return archiveManager.GetDocument(requisite, localPath) ? localPath : null;
        }
    }
}