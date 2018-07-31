using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltaBO.views;
using AltaArchiveApp;
using AltaBO;
using AltaBO.specifics;
using AltaTransport;
using altaik.baseapp.ext;
using System.Diagnostics;

namespace AltaTradingSystemApp.Services
{
    public static class DocumentService
    {
        private static ArchiveManager archiveManager = new ArchiveManager(DataManagerService.TradingInstance(), "ftp://192.168.11.5", "anonymous", "v.kovalev@altatender.kz", "Archive/");


        public static List<DocumentView> ReadDocuments(int fileListId)
        {
            return DataManagerService.TradingInstance().ReadDocuments(fileListId);
        }


        public static bool DownloadDocument(DocumentView document, string saveAs)
        {
            var req = archiveManager.GetDocumentParams(document.id);
            var result = archiveManager.GetDocument(req, saveAs);

            return result;
        }


        public static string GetNewDocumentPath(string title, string filter, string fileName = "")
        {
            var result = Service.PutFile(title, filter, fileName);

            return result == null ? null : result.FullName;
        }


        public static string GetDocumentPath(string title, string filter)
        {
            var result = Service.GetFile(title, filter);

            return result == null ? null : result.FullName;
        }


        public static void UploadDocument(DocumentType documentType, string fileName, Auction auction)
        {
            DocumentRequisite documentRequisite = new DocumentRequisite() {
                fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1),
                date = auction.Date,
                extension = fileName.Substring(fileName.LastIndexOf(".") + 1),
                filesListId = auction.FilesListId,
                market = (MarketPlaceEnum)auction.SiteId,
                number = auction.Number,
                section = DocumentSectionEnum.Auction,
                type = (DocumentTypeEnum)documentType.id
            };

            var result = archiveManager.PutDocument(fileName, documentRequisite, auction.FilesListId);
        }


        public static void DeleteDocument(int documentId)
        {
            DataManagerService.TradingInstance().DeleteDocument(documentId);
        }


        public static int CreateFilesList(string description)
        {
            return DataManagerService.TradingInstance().CreateFilesList(description);
        }


        public static void OpenFolder(string path)
        {
            Process.Start("explorer", path);
        }
    }
}
