using AltaBO;
using AltaBO.archive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaMySqlDB.service
{
    public interface IArchiveDataManager : IDisposable
    {

        #region Nodes
        int GetLastLevel(int presentId);
        List<string> ReadRoot(int presentId);
        List<string> ReadNodes(int presentId, int reqLevel, string parentValues);
        #endregion

        #region Documents
        List<DocumentType> ReadDocumentTypes();
        int CreateDocument(string documentNumber, string documentName, string documentDescription, int documentType, string documentAuthor, DateTime createdDate, DateTime uploadDate, int linkId, string documentCompany, int presentId, string query);
        void UpdateDocument(string documentNumber, string documentName, string documentDescription, int documentType, string documentAuthor, DateTime createdDate, string documentCompany, int documentId);
        void UpdateDocument(int documentId, int presentId, string query);
        void DeleteDocument(int documentId);
        int CreateDocumentLink(string path, string extension);
        void UpdateDocumentLink(string path, string extension, int linkId);
        List<Document> ReadDocuments(int presentId, int reqLevel, string query);
        List<Document> ReadDocuments(bool withOutAN);
        Document ReadDocument(int documentId);
        string ReadDocumentLink(int documentId);
        List<Document> SearchDocuments(string searchQuery);
        #endregion

        #region ArchiveNumbers
        void UpdateDocumentWithASN(int documentId, int documentSerialNumber);
        int GetNextSerialNumber(string dYear, string dOffice, string dCase, string dVolume);
        #endregion
    }
}
