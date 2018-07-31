using AltaArchiveApp.Services;
using AltaBO;
using AltaBO.specifics;
using AltaLog;
using AltaMySqlDB.service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AltaArchiveApp
{
    public class ArchiveManager
    {
        IDataManager dataManager;
        Dictionary<DocumentTemplateEnum, List<DocumentRequisite>> templateRequisities = new Dictionary<DocumentTemplateEnum, List<DocumentRequisite>>();
        string archiveRootPath = @"Archive/";
        public string ipPath = @"\\192.168.11.5\";

        public ArchiveManager(IDataManager dataManager, string host, string login, string pass, string archiveRootPath) : this(dataManager)
        {
            FTPService.Host = host;
            FTPService.Login = login;
            FTPService.Password = pass;
            this.archiveRootPath = archiveRootPath;
        }

        public ArchiveManager()
        {
            const string DOC_EXTENSION = "docx", ECXEL_EXTENSION = "xlsx";

            #region ForAll
            #endregion

            #region ETS
            AddRule(DocumentTemplateEnum.Applicants, MarketPlaceEnum.ETS, DOC_EXTENSION);
            AddRule(DocumentTemplateEnum.ApplicantsForCustomer, MarketPlaceEnum.ETS, DOC_EXTENSION);
            AddRule(DocumentTemplateEnum.MoneyTransfer, MarketPlaceEnum.ETS, DOC_EXTENSION);
            AddRule(DocumentTemplateEnum.Order, MarketPlaceEnum.ETS, ECXEL_EXTENSION);
            AddRule(DocumentTemplateEnum.OrderAttach, MarketPlaceEnum.ETS, DOC_EXTENSION);
            //AddRule(DocumentTemplateEnum.Procuratory, MarketPlaceEnum.ETS, DOC_EXTENSION);
            AddRule(DocumentTemplateEnum.Procuratory, MarketPlaceEnum.ETS, ECXEL_EXTENSION);
            AddRule(DocumentTemplateEnum.SupplierOrder, MarketPlaceEnum.ETS, DOC_EXTENSION);
            AddRule(DocumentTemplateEnum.TechSpec, MarketPlaceEnum.ETS, ECXEL_EXTENSION);
            AddRule(DocumentTemplateEnum.TechSpecReport, MarketPlaceEnum.ETS, ECXEL_EXTENSION);
            #endregion

            #region KazETS
            AddRule(DocumentTemplateEnum.CompanyProfile, MarketPlaceEnum.KazETS, ECXEL_EXTENSION);
            AddRule(DocumentTemplateEnum.Contract, MarketPlaceEnum.KazETS, ECXEL_EXTENSION);
            AddRule(DocumentTemplateEnum.CoverLetter, MarketPlaceEnum.KazETS, ECXEL_EXTENSION);
            AddRule(DocumentTemplateEnum.CoverLetterAkAltyn, MarketPlaceEnum.KazETS, ECXEL_EXTENSION);
            AddRule(DocumentTemplateEnum.CustomerReport, MarketPlaceEnum.KazETS, DOC_EXTENSION);
            AddRule(DocumentTemplateEnum.Order, MarketPlaceEnum.KazETS, ECXEL_EXTENSION);
            AddRule(DocumentTemplateEnum.Specification, MarketPlaceEnum.KazETS, DOC_EXTENSION);
            AddRule(DocumentTemplateEnum.SupplierOrder, MarketPlaceEnum.KazETS, ECXEL_EXTENSION);
            AddRule(DocumentTemplateEnum.SupplierOrderAkAltyn, MarketPlaceEnum.KazETS, ECXEL_EXTENSION);
            AddRule(DocumentTemplateEnum.SupplierOrderKorund, MarketPlaceEnum.KazETS, ECXEL_EXTENSION);
            AddRule(DocumentTemplateEnum.SupplierReportAkAltyn, MarketPlaceEnum.KazETS, DOC_EXTENSION);
            AddRule(DocumentTemplateEnum.SupplierReportKorund, MarketPlaceEnum.KazETS, DOC_EXTENSION);
            AddRule(DocumentTemplateEnum.WarrantyLetterAkAltyn, MarketPlaceEnum.KazETS, DOC_EXTENSION);
            AddRule(DocumentTemplateEnum.WarrantyLetterKorund, MarketPlaceEnum.KazETS, DOC_EXTENSION);
            #endregion

            #region UTB
            AddRule(DocumentTemplateEnum.Applicants, MarketPlaceEnum.UTB, DOC_EXTENSION);
            AddRule(DocumentTemplateEnum.CustomerReport, MarketPlaceEnum.UTB, DOC_EXTENSION);
            AddRule(DocumentTemplateEnum.Invoice, MarketPlaceEnum.UTB, DOC_EXTENSION);
            AddRule(DocumentTemplateEnum.Order, MarketPlaceEnum.UTB, DOC_EXTENSION);
            AddRule(DocumentTemplateEnum.Protocol, MarketPlaceEnum.UTB, DOC_EXTENSION);
            AddRule(DocumentTemplateEnum.Specification, MarketPlaceEnum.UTB, DOC_EXTENSION);
            AddRule(DocumentTemplateEnum.SupplierOrder, MarketPlaceEnum.UTB, DOC_EXTENSION);
            AddRule(DocumentTemplateEnum.SupplierOrderAkAltyn, MarketPlaceEnum.UTB, DOC_EXTENSION);
            AddRule(DocumentTemplateEnum.SupplierOrderKorund, MarketPlaceEnum.UTB, DOC_EXTENSION);
            AddRule(DocumentTemplateEnum.SupplierReport, MarketPlaceEnum.UTB, DOC_EXTENSION);
            #endregion

            #region Caspy
            AddRule(DocumentTemplateEnum.Applicants, MarketPlaceEnum.Caspy, DOC_EXTENSION);
            AddRule(DocumentTemplateEnum.Contract, MarketPlaceEnum.Caspy, DOC_EXTENSION);
            //AddRule(DocumentTemplateEnum.ContractEn, MarketPlaceEnum.Caspy, DOC_EXTENSION);
            //AddRule(DocumentTemplateEnum.Members, MarketPlaceEnum.Caspy, DOC_EXTENSION);
            AddRule(DocumentTemplateEnum.Order, MarketPlaceEnum.Caspy, ECXEL_EXTENSION);
            AddRule(DocumentTemplateEnum.OrderAttach, MarketPlaceEnum.Caspy, ECXEL_EXTENSION);
            AddRule(DocumentTemplateEnum.Procuratory, MarketPlaceEnum.Caspy, DOC_EXTENSION);
            //AddRule(DocumentTemplateEnum.Report, MarketPlaceEnum.Caspy, DOC_EXTENSION);
            //AddRule(DocumentTemplateEnum.ReportAttach, MarketPlaceEnum.UTB, ECXEL_EXTENSION);
            AddRule(DocumentTemplateEnum.TechSpec, MarketPlaceEnum.UTB, ECXEL_EXTENSION);
            #endregion
        }

        private void AddRule(DocumentTemplateEnum documentTemplate, MarketPlaceEnum market, string exstension)
        {
            AddRule(documentTemplate, market, documentTemplate.ToString(), exstension);
        }

        private void AddRule(DocumentTemplateEnum documentTemplate, MarketPlaceEnum market, string fileName, string exstension)
        {
            if (!templateRequisities.Any(t => t.Key == documentTemplate)) {
                templateRequisities.Add(documentTemplate, new List<DocumentRequisite>());
            }
            templateRequisities[documentTemplate].Add(new DocumentRequisite() {
                section = DocumentSectionEnum.Template,
                fileName = string.Format("{0}.{1}", fileName, exstension),
                market = market,
                extension = exstension
            });
        }

        public ArchiveManager(IDataManager dataManager) : this()
        {
            this.dataManager = dataManager;
        }

        public string GetPath(DocumentRequisite documentRequisite)
        {
            string path = "";

            switch (documentRequisite.section) {
                case DocumentSectionEnum.Company:
                    path = archiveRootPath + @"Companies/";
                    break;
                case DocumentSectionEnum.Order:
                    path = archiveRootPath + @"Orders/";
                    break;
                case DocumentSectionEnum.Auction:
                    path = archiveRootPath + @"Auctions/";
                    break;
                case DocumentSectionEnum.Template:
                    path = archiveRootPath + @"Templates/";
                    break;
            }

            if (documentRequisite.section != DocumentSectionEnum.Company) {
                switch (documentRequisite.market) {
                    case MarketPlaceEnum.UTB:
                        path += @"UTB/";
                        break;
                    case MarketPlaceEnum.ETS:
                        path += @"ETS/";
                        break;
                    case MarketPlaceEnum.KazETS:
                        path += @"KazETS/";
                        break;
                    case MarketPlaceEnum.Caspy:
                        path += @"Caspy/";
                        break;
                }
            }

            path += (documentRequisite.section == DocumentSectionEnum.Company || documentRequisite.section == DocumentSectionEnum.Template) ? "" : (documentRequisite.date.ToShortDateString() + @"/");
            path += documentRequisite.section == DocumentSectionEnum.Template ? "" : String.Format(@"{0}/", documentRequisite.number.Replace("/", "_"));
            path += documentRequisite.fileName;

            return path;
        }

        public string GetFileListDescription(int fileListId)
        {
            return dataManager.GetFileListDescription(fileListId);
        }

        public int CreateFilesList(string description = null)
        {
            return dataManager != null ? dataManager.CreateFilesList(description) : 0;
        }

        public int PutDocument(Stream stream, DocumentRequisite documentRequisite)
        {
            return PutDocument(stream, documentRequisite, documentRequisite.filesListId);
        }

        public int PutDocument(string path, DocumentRequisite documentRequisite, int? filesListId = null)
        {
            if (dataManager == null) { return -1; }

            var remotePath = GetPath(documentRequisite);

            if (!FTPService.CreateDirectory(Path.GetDirectoryName(remotePath))) return -2;
            if (!FTPService.Upload(path, remotePath)) return -3;

            return dataManager.PutDocument(documentRequisite, filesListId);
        }

        public int PutDocument(Stream stream, DocumentRequisite documentRequisite, int? filesListId = null)
        {
            if (dataManager == null) { return -1; }

            var path = GetPath(documentRequisite);

            if (!FTPService.CreateDirectory(Path.GetDirectoryName(path))) return -2;
            if (!FTPService.Upload(stream, path)) return -3;

            return dataManager.PutDocument(documentRequisite, filesListId);
        }

        public int PutDocument(FileStream fileStream, DocumentRequisite documentRequisite, int? filesListId = null)
        {
            return PutDocument((Stream)fileStream, documentRequisite, filesListId);
        }

        public List<DocumentRequisite> GetFilesFromList(int filesListId)
        {
            return dataManager.GetFilesFromList(filesListId);
        }


        public DocumentRequisite GetDocumentParams(int fileId)
        {
            return dataManager.GetFileParams(fileId);
        }

        public List<DocumentRequisite> GetFilesInfo(int fileListId, DocumentTypeEnum documentType)
        {
            return dataManager.GetFiles(fileListId, new List<int> { (int)documentType });
        }

        public List<DocumentRequisite> GetFilesInfo(int fileListId, List<int> types)
        {
            return dataManager.GetFiles(fileListId, types);
        }

        public bool GetDocument(DocumentRequisite documentRequisite, string saveAs)
        {
            var path = GetPath(documentRequisite);
            return GetDocument(path, saveAs);
        }

        public Stream GetDocument(DocumentRequisite documentRequisite)
        {
            var path = GetPath(documentRequisite);
            var stream = GetDocument(path);
            return stream;
        }

        public bool GetDocument(string filePath, string saveAs)
        {
            return FTPService.Donwload(filePath, saveAs);
        }

        public Stream GetDocument(string filePath)
        {
            return FTPService.Donwload(filePath);
        }

        public DocumentRequisite GetTemplateRequisite(MarketPlaceEnum market, DocumentTemplateEnum documentTemplate)
        {
            if (templateRequisities[documentTemplate] == null) {
                return null;
            }
            var templateRequisite = templateRequisities[documentTemplate].FirstOrDefault(t => t.market == market);
            return templateRequisite;
        }

        public string GetTemplatePath(string templateFilePath)
        {
            using (var stream = FTPService.Donwload(templateFilePath)) {
                if (stream != null) {
                    if (!Directory.Exists(@"C:\temp")) Directory.CreateDirectory(@"c:\temp");

                    string targetFileName = "C:\\temp\\" + templateFilePath;

                    using (var targetStream = new FileStream(targetFileName, FileMode.OpenOrCreate)) {
                        if (targetStream == null) {
                            return null;
                        }
                        var bufferSize = 1024;
                        byte[] byteBuffer = new byte[bufferSize];
                        int bytesRead = stream.Read(byteBuffer, 0, bufferSize);
                        try {
                            while (bytesRead > 0) {
                                targetStream.Write(byteBuffer, 0, bytesRead);
                                bytesRead = stream.Read(byteBuffer, 0, bufferSize);
                            }
                        } catch (Exception ex) { Console.WriteLine(ex.ToString()); return null; }
                        targetStream.Seek(0, SeekOrigin.Begin);
                        targetStream.Close();
                    }
                    stream.Close();
                    return targetFileName;
                }
            }
            return null;
        }


        public Stream GetTemplateStream(string templateFilePath)
        {
            return FTPService.Donwload(templateFilePath);
        }


        public string GetTemplate(DocumentRequisite documentRequisite, DocumentTemplateEnum documentTemplate)
        {
            string extension = ".docx";

            switch (documentRequisite.market) {
                case MarketPlaceEnum.UTB:
                    if (documentTemplate == DocumentTemplateEnum.Order) extension = ".xlsx";
                    break;
                case MarketPlaceEnum.ETS:
                    if (documentRequisite.type == DocumentTypeEnum.TechSpecs|| documentTemplate == DocumentTemplateEnum.Procuratory) extension = ".xlsx";
                    break;
                case MarketPlaceEnum.KazETS:
                    if (documentTemplate == DocumentTemplateEnum.SupplierOrderKorund|| documentTemplate == DocumentTemplateEnum.SupplierOrderAkAltyn||
                        documentTemplate == DocumentTemplateEnum.TechSpec|| documentTemplate == DocumentTemplateEnum.CoverLetter||
                        documentTemplate == DocumentTemplateEnum.CoverLetterAkAltyn|| documentTemplate == DocumentTemplateEnum.CoverLetterKorund) extension = ".xlsx";                    
                    break;
                case MarketPlaceEnum.Caspy:
                    if (documentTemplate == DocumentTemplateEnum.Order) extension = ".xlsx";
                    break;
            }

            if (documentRequisite.type == DocumentTypeEnum.CompanyProfile) extension = ".xlsx";

            var templateFileName = ipPath + archiveRootPath.Replace("/", "") + "\\Templates\\" + documentRequisite.market + "\\" + documentTemplate + extension;
            var dateFolder = ipPath + archiveRootPath.Replace("/", "") + "\\Auctions\\" + documentRequisite.market + "\\" + documentRequisite.date.ToShortDateString();
            var endFolder = dateFolder + "\\" + documentRequisite.number.Replace("/", "_");
            var oldEndFolder = dateFolder + "\\" + (documentRequisite.market == MarketPlaceEnum.ETS ? documentRequisite.number.Length > 4 ? documentRequisite.number.Substring(documentRequisite.number.Length - 4) : documentRequisite.number : documentRequisite.number).Replace("/", "_");

            if (!Directory.Exists(dateFolder)) Directory.CreateDirectory(dateFolder);
            if (Directory.Exists(oldEndFolder)) endFolder = oldEndFolder;
            if (!Directory.Exists(endFolder)) Directory.CreateDirectory(endFolder);

            try {
                File.Copy(templateFileName, endFolder + "\\" + documentRequisite.fileName, true);
                return endFolder + "\\" + documentRequisite.fileName;
            } catch (Exception ex) {
                AppJournal.Write("Template copy", "Err: " + ex.ToString(), true);
                return null;
            }
        }


        public int SaveFile(DocumentRequisite documentRequisite)
        {
            return dataManager.PutSimpleDocument(documentRequisite);
        }


        public void SaveFile(DocumentRequisite documentRequisite, int filesListId)
        {
            dataManager.PutDocument(documentRequisite, filesListId);
        }


        public string[] CopyOriginalOrder(DocumentRequisite documentRequisite, string orderFileName, string treatyFileName = null, string orderOriginalFileName = null, string[] schemesFileNames = null)
        {
            var dateFolder = ipPath + archiveRootPath + "\\Auctions\\" + documentRequisite.market + "\\" + documentRequisite.date.ToShortDateString();
            var oldEndFolder = dateFolder.Replace("/", "\\") + "\\" + (documentRequisite.number.Substring(documentRequisite.number.Length - 4)).Replace("/", "_");
            var endFolder = dateFolder.Replace("/", "\\") + "\\" + documentRequisite.number.Replace("/", "_");

            endFolder = Directory.Exists(oldEndFolder) ? oldEndFolder : endFolder;

            string[] fileNames = new string[3 + (schemesFileNames == null ? 0 : schemesFileNames.Length)];

            fileNames[0] = endFolder + "\\" + orderFileName.Substring(orderFileName.LastIndexOf("\\"));
            File.Copy(orderFileName, fileNames[0], true);

            if (treatyFileName != null) {
                fileNames[1] = endFolder + "\\" + treatyFileName.Substring(treatyFileName.LastIndexOf("\\"));
                File.Copy(treatyFileName, fileNames[1], true);
            }

            if (orderOriginalFileName != null) {
                fileNames[2] = endFolder + "\\" + orderOriginalFileName.Substring(orderOriginalFileName.LastIndexOf("\\"));
                File.Copy(orderOriginalFileName, fileNames[2], true);
            }

            int fCount = 3;

            if (schemesFileNames != null && !string.IsNullOrEmpty(schemesFileNames[0])) {
                foreach (var item in schemesFileNames) {
                    fileNames[fCount] = endFolder + "\\" + item.Substring(item.LastIndexOf("\\"));

                    File.Copy(item, fileNames[fCount], true);

                    fCount++;
                }
            }

            return fileNames;
        }
    }
}