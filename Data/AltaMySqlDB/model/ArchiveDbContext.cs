using AltaBO;
using AltaBO.archive;
using AltaMySqlDB.service;
using MySql.Data.Entity;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace AltaMySqlDB.model
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class ArchiveDbContext : DbContext, IArchiveDataManager, IDisposable
    {
        static string connStringVPN = "server=10.1.2.11;port=3306;database=archive;uid=broker;password=KorPas$77&db;charset=utf8";
        static string connStringDB26 = "server=88.204.230.204;port=50505;database=archive;uid=Archive;password=Lzd!&emU!#W12;charset=utf8";
        static string connString = "server=88.204.230.203;port=3306;database=archive;uid=broker;password=KorPas$77&db;charset=utf8";

        //public ArchiveDbContext() : this(connStringVPN) { }
        //public ArchiveDbContext() : this(connString) { }
        public ArchiveDbContext() : this(connStringDB26) { }

        public ArchiveDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }


        #region Nodes
        public int GetLastLevel(int presentId)
        {
            return Database.SqlQuery<int>("call getLastLevel(@presentId)", new MySqlParameter("presentId", presentId)).First();
        }


        public List<string> ReadRoot(int presentId)
        {
            return Database.SqlQuery<string>("call readRoot(@presentId)", new MySqlParameter("presentId", presentId)).ToList();
        }


        public List<string> ReadNodes(int presentId, int reqLevel, string parentValues)
        {
            try
            {
                return Database.SqlQuery<string>("call readNodes(@presentId, @reqLevel, @query)",
                    new MySqlParameter("presentId", presentId),
                    new MySqlParameter("reqLevel", reqLevel),
                    new MySqlParameter("query", parentValues)).ToList();
            }
            catch { return null; }
        }
        #endregion

        #region Documents
        public List<DocumentType> ReadDocumentTypes()
        {
            return Database.SqlQuery<DocumentType>("call readDocumentTypes", new MySqlParameter("", null)).ToList();
        }


        public int CreateDocumentLink(string path, string extension)
        {
            return Database.SqlQuery<int>("call createDocumentLink(@path, @extension)", new MySqlParameter("path", path),
                new MySqlParameter("extension", extension)).First();
        }


        public void UpdateDocumentLink(string path, string extension, int linkId)
        {
            Database.SqlQuery<int>("call updateDocumentLink(@path, @extension, @linkId)",
                new MySqlParameter("path", path),
                new MySqlParameter("extension", extension),
                new MySqlParameter("linkId", linkId)).First();
        }


        public int CreateDocument(string documentNumber, string documentName, string documentDescription, int documentType, string documentAuthor, DateTime createdDate, DateTime uploadDate, int linkId, string documentCompany, int presentId, string query)
        {
            return Database.SqlQuery<int>("call createDocument(@documentNumber, @documentName, @documentDescription, @documentType, @documentAuthor, @createdDate, @uploadDate, @linkId, @documentCompany, @presentId, @query)",
                new MySqlParameter("documentNumber", documentNumber),
                new MySqlParameter("documentName", documentName),
                new MySqlParameter("documentDescription", documentDescription),
                new MySqlParameter("documentType", documentType),
                new MySqlParameter("documentAuthor", documentAuthor),
                new MySqlParameter("createdDate", createdDate),
                new MySqlParameter("uploadDate", uploadDate),
                new MySqlParameter("linkId", linkId),
                new MySqlParameter("documentCompany", documentCompany),
                new MySqlParameter("presentId", presentId),
                new MySqlParameter("query", query)).First();
        }


        public void UpdateDocument(string documentNumber, string documentName, string documentDescription, int documentType, string documentAuthor, DateTime createdDate, string documentCompany, int documentId)
        {
            Database.SqlQuery<int>("call updateDocument(@documentNumber, @documentName, @documentDescription, @documentType, @documentAuthor, @createdDate, @documentCompany, @documentId)",
                new MySqlParameter("documentNumber", documentNumber),
                new MySqlParameter("documentName", documentName),
                new MySqlParameter("documentDescription", documentDescription),
                new MySqlParameter("documentType", documentType),
                new MySqlParameter("documentAuthor", documentAuthor),
                new MySqlParameter("createdDate", createdDate),
                new MySqlParameter("documentCompany", documentCompany),
                new MySqlParameter("documentId", documentId)).First();
        }
        

        public void UpdateDocument(int documentId, int presentId, string query)
        {
            Database.SqlQuery<int>("call updateDocumentQuery(@documentId, @presentId, @query)",
                new MySqlParameter("documentId",documentId),
                new MySqlParameter("presentId", presentId),
                new MySqlParameter("query", query)).First();
        }


        public void DeleteDocument(int documentId)
        {
            Database.SqlQuery<int>("delete from documents where id=@documentId; select last_insert_id()", new MySqlParameter("documentId", documentId)).First();
        }


        public List<Document> ReadDocuments(int presentId, int reqLevel, string query)
        {
            return Database.SqlQuery<Document>("call readDocuments(@presentId,@reqLevel, @query)",
                new MySqlParameter("presentId", presentId),
                new MySqlParameter("reqLevel", reqLevel),
                new MySqlParameter("query", query)).ToList();
        }


        public List<Document> ReadDocuments(bool withOutAN)
        {
            return Database.SqlQuery<Document>("select * from documents" + (withOutAN ? " where serialNumber is null" : ""),
                new MySqlParameter("", null)).ToList();
        }


        public Document ReadDocument(int documentId)
        {
            return Database.SqlQuery<Document>("select * from documents where id = @documentId",
                new MySqlParameter("documentId", documentId)).First();
        }


        public string ReadDocumentLink(int documentId)
        {
            return Database.SqlQuery<string>("select l.url from documents as d left join links as l on l.id = d.linkId where d.id = @documentId",
                new MySqlParameter("documentId", documentId)).First();
        }


        public void UpdateDocumentWithASN(int documentId, int documentSerialNumber)
        {
            Database.SqlQuery<int>("update documents set serialNumber = @documentSerialNumber where id = @documentId; select last_insert_id();",
                new MySqlParameter("documentId", documentId),
                new MySqlParameter("documentSerialNumber", documentSerialNumber)).First();
        }


        public List<Document> SearchDocuments(string searchQuery)
        {
            return Database.SqlQuery<Document>("call searchDocuments(@searchQuery)", new MySqlParameter("searchQuery", searchQuery)).ToList();
        }
        #endregion

        #region ArchiveNumbers
        public int GetNextSerialNumber(string dYear, string dOffice, string dCase, string dVolume)
        {
            return Database.SqlQuery<int>("call getNextSerialNumber(@dYear, @dOffice, @dCase, @dVolume)",
                        new MySqlParameter("dYear", dYear),
                        new MySqlParameter("dOffice", dOffice),
                        new MySqlParameter("dCase", dCase),
                        new MySqlParameter("dVolume", dVolume)).First();
        }


        /*public int AddArchiveNumber(string archiveNumberItem) {
            //archivenumbers.Add(archiveNumberItem);
            //SaveChanges();

            //return archiveNumberItem.Id;

            return 0;
        }


        public bool UpdateArchiveNumber(string archiveNumberInfo) {
            //var archiveNumberItem = archivenumbers.FirstOrDefault(a => a.Id == archiveNumberInfo.Id);

            //if(archiveNumberItem != null) {
            //    archiveNumberItem = archiveNumberInfo;
            //    SaveChanges();

            //    return true;
            //}

            return false;
        }


        public int GetDocsCountInVolume(int volumeId) {
            //return archivenumbers.Count(a => a.VolumeId == volumeId);
            return 0;
        }


        public int GetDocsCountInCase(int caseId) {
            //return archivenumbers.Where(a => a.CaseId == caseId).GroupBy(a => a.DocumentNumber).Select(g => new { Cnt = g.Count() }).Count();
            return 0;
        }


        public int CheckDocInVolume(int volumeId, int archiveNumberId) {
            //return archivenumbers.Count(a => a.Id == archiveNumberId && a.VolumeId == volumeId);
            return 0;
        }*/
        #endregion
    }
}
