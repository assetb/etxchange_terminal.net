using AltaArchiveApp;
using AltaBO;
using AltaMySqlDB.service;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace ServerApp.Controllers
{
    [RoutePrefix("api/archive")]
    public class ArchiveController : BaseApiController
    {
        const int MAX_FILE_LIST_UPDALODED = 10;
        List<string> tempArhiveFiles = new List<string>();

        #region List
        [HttpGet, Route("list/{id}")]
        public List<DocumentRequisite> GetFilesInList(int id, [FromUri] List<int> types)
        {
            var documentReqList = ArchiveManager.GetFilesFromList(id);
            if (types != null && types.Count() > 0)
            {
                return (from r in documentReqList where types.Contains((int)r.type) select r).ToList();
            }
            return documentReqList;
        }

        [HttpGet, Route("list/zip")]
        public HttpResponseMessage DonwloadZip([FromUri] List<int> list, [FromUri] List<int> docsType)
        {
            if ((list == null && (list.Count == 0 || list.Count > MAX_FILE_LIST_UPDALODED)) && (docsType == null && docsType.Count == 0))
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var tempFolderArchive = HttpContext.Current.Server.MapPath(string.Format("~/App_Data/{0}", Guid.NewGuid()));
            var zipFileName = string.Format("{0}.zip", tempFolderArchive);
            var acrhive = new ArchiveManager(DataManager);
            int countFilesFound = 0;
            foreach (int listId in list)
            {
                var descriptionList = ArchiveManager.GetFileListDescription(listId);
                if (string.IsNullOrEmpty(descriptionList))
                {
                    descriptionList = string.Format("Папка № {0}", listId);
                }
                var files = ArchiveManager.GetFilesFromList(listId);
                foreach (var docReq in files)
                {
                    if (!docsType.Any(d => d == (int)docReq.type))
                        continue;
                    using (var file = ArchiveManager.GetDocument(docReq))
                    {
                        if (file == null)
                            continue;
                        var dir = Directory.CreateDirectory(string.Format("{0}/{1}", tempFolderArchive, descriptionList.Replace("/", "_")));
                        if (dir != null)
                        {
                            using (var f = File.Open(string.Format("{0}/{1}", dir.FullName, docReq.fileName.Replace("/", "_")), FileMode.Create))
                            {
                                file.CopyTo(f);
                                countFilesFound++;
                            }
                        }
                    }
                }
            }
            if (countFilesFound == 0)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            ZipFile.CreateFromDirectory(tempFolderArchive, zipFileName);
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            Stream stream = File.OpenRead(zipFileName);
            result.Content = new StreamContent(stream);

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileNameStar = string.Format("Архив файлов({0}).zip", DateTime.Now.ToShortDateString());
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType.CharSet = "UTF-8";
            result.Content.Headers.ContentLength = stream.Length;

            Directory.Delete(tempFolderArchive, true);
            tempArhiveFiles.Add(zipFileName);
            return result;
        }

        [HttpDelete, Route("file-list/{fileListId}/document/{documentId}")]
        public HttpStatusCode RemoveDocumentInList([FromUri] int fileListId, [FromUri] int documentId)
        {
            return DataManager.RemoveDocumentInList(fileListId, documentId) ? HttpStatusCode.OK : HttpStatusCode.Conflict;
        }
        #endregion

        #region File
        [HttpGet, Route("file")]
        public List<DocumentRequisite> GetFiles([FromUri] int fileListId = 0, [FromUri] List<int> types = null)
        {
            return ArchiveManager.GetFilesInfo(fileListId, types);
        }

        [HttpGet, Route("file/{id}")]
        public HttpResponseMessage GetFile(int id)
        {
            HttpResponseMessage result = null;
            var docReq = ArchiveManager.GetDocumentParams(id);
            if (docReq != null)
            {
                var f = ArchiveManager.GetDocument(docReq);
                if (f != null)
                {
                    result = new HttpResponseMessage(HttpStatusCode.OK);
                    result.Content = new StreamContent(f);
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    result.Content.Headers.ContentDisposition.FileNameStar = docReq.fileName;
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    result.Content.Headers.ContentType.CharSet = "UTF-8";
                    result.Content.Headers.ContentLength = f.Length;
                    return result;

                }
            }
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        [HttpPost, Route("file")]
        public int DonwloadFile()
        {
            var files = HttpContext.Current.Request.Files;
            var form = HttpContext.Current.Request.Form;
            var currentDate = DateTime.Now;

            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.UnsupportedMediaType));
            }

            if (files.Count == 0 || form.Count == 0)
            {
                var errorResponce = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errorResponce.Headers.Add("X-Error-Description", "No files specified or data of the form specified.");
                throw new HttpResponseException(errorResponce);
            }

            var file = HttpContext.Current.Request.Files["file"];
            if (file == null)
            {
                var errorResponce = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errorResponce.Headers.Add("X-Error-Description", "Not found file in request body.");
                throw new HttpResponseException(errorResponce);
            };

            JavaScriptSerializer jss = new JavaScriptSerializer();
            DocumentRequisite docReq = jss.Deserialize<DocumentRequisite>(form.Get("documentRequisite"));
            if (docReq == null)
            {
                var errorResponce = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errorResponce.Headers.Add("X-Error-Description", "Error in form data");
                throw new HttpResponseException(errorResponce);
            };

            docReq.fileName = file.FileName;
            docReq.date = currentDate;

            if (form.Get("fileListId") != null)
                docReq.filesListId = int.Parse(form.Get("fileListId"));

            var archive = new ArchiveManager(DataManager);

            int fileId = archive.PutDocument(file.InputStream, docReq);
            if (fileId > 0)
                return fileId;
            else
            {
                var errorResponce = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                errorResponce.Headers.Add("X-Error-Description", "An error occurred while saving the file");
                throw new HttpResponseException(errorResponce);
            }
        }
        #endregion

        [HttpPost, Route("documentRequisite")]
        public DocumentRequisite AddDocumentReuisite(DocumentRequisite documentRequisite)
        {
            return DataManager.AddDocumentRequisite(documentRequisite);
        }

        /// <summary>
        /// Удаление файла из списка.
        /// 
        /// Примечание. Файл удаляется из списка но при этом не удаляется полностью.
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [HttpPost, Route("remove/list/{listId}/file/{fileId}")]
        public bool RemoveFileInList(int listId, int fileId)
        {
            return DataManager.RemoveDocumentInList(listId, fileId);
        }

        public new void Dispose()
        {
            foreach (var fileName in tempArhiveFiles)
            {
                File.Delete(fileName);
            }
            base.Dispose();
        }

        // TODO: Delete
        [HttpGet, Route("download")]
        public HttpResponseMessage Download(DateTime auctionDate, string auctionNumber, int auctionSite, string fileType)
        {
            string filePattern = "";

            switch (fileType)
            {
                case "applicants":
                    filePattern = "Список претендентов*.pdf";
                    break;
                case "reportToSupplier":
                    filePattern = "Отчет поставщику*.pdf";
                    break;
                case "invoice":
                    filePattern = "Счет для*.pdf";
                    break;
                case "supplierOrder":
                    filePattern = "Заявка на участие*.docx";
                    break;
            }

            try
            {
                var filePath = @"\\192.168.11.5\Archive\Auctions\" + (auctionSite == 4 ? "ETS" : "UTB") + "\\" + auctionDate.ToShortDateString() + "\\" + (auctionSite != 4 ? auctionNumber : auctionNumber.Substring(auctionNumber.Length - 4)).Replace("/", "_") + "\\";
                string[] files = Directory.GetFiles(filePath, filePattern);
                filePath = files[0];
                string fileName = filePath.Substring(filePath.LastIndexOf(@"\") + 1);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] bytes = new byte[file.Length];
                        file.Read(bytes, 0, (int)file.Length);
                        ms.Write(bytes, 0, (int)file.Length);

                        HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                        httpResponseMessage.Content = new ByteArrayContent(bytes.ToArray());
                        httpResponseMessage.Content.Headers.Add("x-filename", fileName);
                        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                        httpResponseMessage.Content.Headers.ContentDisposition.FileName = fileName;
                        httpResponseMessage.StatusCode = HttpStatusCode.OK;
                        return httpResponseMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
