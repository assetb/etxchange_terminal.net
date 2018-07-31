using AltaBO;
using AltaBO.archive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaArchiveApp.Services
{
    public static class DocumentService
    {
        public static List<DocumentType> ReadDocumentTypes()
        {
            return DataManagerService.Instanse().ReadDocumentTypes();
        }


        public static int CreateDocument(Document document, int presentId, string query)
        {
            return DataManagerService.Instanse().CreateDocument(document.number, document.name, document.description, document.type, document.author, document.createdDate, document.uploadDate, (int)document.linkId, document.company, presentId, query);
        }


        public static void UpdateDocument(Document document, int documentId)
        {
            DataManagerService.Instanse().UpdateDocument(document.number, document.name, document.description, document.type, document.author, document.createdDate, document.company, documentId);
        }


        public static void UpdateDocument(int documentId, int presentId, string query)
        {
            DataManagerService.Instanse().UpdateDocument(documentId, presentId, query);
        }


        public static void DeleteDocument(int documentId)
        {
            DataManagerService.Instanse().DeleteDocument(documentId);
        }


        public static int CreateDocumentLink(string path, string extension)
        {
            return DataManagerService.Instanse().CreateDocumentLink(path, extension);
        }


        public static void UpdateDocumentLink(string path, string extension, int linkId)
        {
            DataManagerService.Instanse().UpdateDocumentLink(path, extension, linkId);
        }


        public static List<Document> ReadDocuments(int presentId, int reqLevel, string query)
        {
            return DataManagerService.Instanse().ReadDocuments(presentId, reqLevel, query);
        }


        public static List<Document> ReadDocuments(bool withOutAN)
        {
            return DataManagerService.Instanse().ReadDocuments(withOutAN);
        }


        public static Document ReadDocument(int documentId)
        {
            return DataManagerService.Instanse().ReadDocument(documentId);
        }


        public static string ReadDocumentLink(int documentId)
        {
            return DataManagerService.Instanse().ReadDocumentLink(documentId);
        }


        public static void UpdateDocumentWithASN(int documentId, int documentSerialNumber)
        {
            DataManagerService.Instanse().UpdateDocumentWithASN(documentId, documentSerialNumber);
        }


        public static List<Document> SearchDocuments(string searchQuery)
        {
            return DataManagerService.Instanse().SearchDocuments(searchQuery);
        }


        public static List<Document> SearchDocumentsInTS(string searchQuery)
        {
            return DataManagerService.TradingInstance().SearchDocumentsInTS(searchQuery);
        }


        public static int GetNextSerialNumber(string dYear, string dOffice, string dCase, string dVolume)
        {
            return DataManagerService.Instanse().GetNextSerialNumber(dYear, dOffice, dCase, dVolume);
        }
    }
}
