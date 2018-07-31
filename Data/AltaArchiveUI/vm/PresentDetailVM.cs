using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using altaik.baseapp.vm;
using System.Collections.ObjectModel;
using AltaBO.archive;
using AltaArchiveApp.Services;
using AltaArchiveUI.service;
using System.Windows;
using altaik.baseapp.vm.command;
using System.Windows.Input;
using AltaDock.vm;
using System.Diagnostics;
using DocumentFormation;
using AltaBO;

namespace AltaArchiveUI.vm
{
    public class PresentDetailVM : BaseAppViewModel
    {
        #region Variables
        private Node parent;
        private PresentTreeVM presentTreeVM;
        private PresentationEnum present;
        #endregion

        #region Methods
        public PresentDetailVM(PresentTreeVM presentTreeVM, PresentationEnum present)
        {
            this.present = present;
            this.presentTreeVM = presentTreeVM;
        }


        public List<Document> GetBoDocuments()
        {
            List<Document> result = new List<Document>();
            foreach (DocumentVM doc in Documents) {
                result.Add(doc.Document);
            }
            return result;
        }


        public void LoadDocuments(List<Document> docs)
        {
            if (docs != null && docs.Count > 0) Documents = ConvertDocumentToDocumentVM(docs);
        }


        public ObservableCollection<DocumentVM> ConvertDocumentToDocumentVM(List<Document> docs)
        {
            List<DocumentVM> result = new List<DocumentVM>();
            foreach (Document doc in docs) {
                doc.company = ServiceFunctions.CompanyRenamer(doc.company);
                result.Add(new DocumentVM(doc));
            }
            return new ObservableCollection<DocumentVM>(result);
        }


        public void ChangeParent(Node newNodeValue, PresentationEnum present)
        {
            parent = newNodeValue;
        }


        public ICommand OpenDocumentCmd => new DelegateCommand(OpenDocument);
        private void OpenDocument()
        {
            if (SelectedDocument != null && SelectedDocument.Document != null) {
                try {
                    try {
                        Process.Start(DocumentService.ReadDocumentLink(SelectedDocument.Document.id));
                    } catch { MessagesService.Show("ОШИБКА ОТКРЫТИЯ", "Файла не найдено по сохраненному пути"); }
                } catch { MessagesService.Show("ОШИБКА ОТКРЫТИЯ", "Произошла ошибка во время попытки открыть документ."); }
            } else MessagesService.Show("ИЗВЛЕЧЕНИЕ ДОКУМЕНТА", "Не выбран документ для извлечения.");
        }


        public ICommand CreateDocumentCmd => new DelegateCommand(CreateDocument);
        private void CreateDocument()
        {
            if (presentTreeVM.SelectedNodeVM != null) {
                if (presentTreeVM.SelectedNodeVM.IsLastLevel) {
                    presentTreeVM.DocumentCreationVM.TitleHeader = "СОЗДАНИЕ ДОКУМЕНТА";
                    presentTreeVM.DocumentCreationVM.CurrentNode = presentTreeVM.SelectedNodeVM;
                    presentTreeVM.DocumentCreationVM.CurrentPresentation = present;
                    presentTreeVM.DocumentCreationVM.IsAttach = false;

                    if (present == PresentationEnum.Archive) {
                        if (presentTreeVM.DocumentCreationVM.BrokersList.Count(bl => bl.ShortName.ToLower().Contains(presentTreeVM.SelectedNodeVM.Parent.Parent.Node.Name.ToLower())) > 0) {
                            presentTreeVM.DocumentCreationVM.SelectedBroker = presentTreeVM.DocumentCreationVM.BrokersList.First(bl => bl.ShortName.ToLower().Contains(presentTreeVM.SelectedNodeVM.Parent.Parent.Node.Name.ToLower()));
                            presentTreeVM.SelectedNodeVM.Parent.Parent.Node.Name = presentTreeVM.DocumentCreationVM.SelectedBroker.ShortName;
                        }

                        presentTreeVM.DocumentCreationVM.DocumentYear = presentTreeVM.SelectedNodeVM.Parent.Parent.Parent.Node.Name;
                        presentTreeVM.DocumentCreationVM.DocumentCase = presentTreeVM.SelectedNodeVM.Parent.Node.Name;
                        presentTreeVM.DocumentCreationVM.DocumentVolume = presentTreeVM.SelectedNodeVM.Node.Name;
                        presentTreeVM.DocumentCreationVM.DocumentSerialNumber = DocumentService.GetNextSerialNumber(presentTreeVM.DocumentCreationVM.DocumentYear,
                                                                                                    presentTreeVM.SelectedNodeVM.Parent.Parent.Node.Name,
                                                                                                    presentTreeVM.DocumentCreationVM.DocumentCase,
                                                                                                    presentTreeVM.DocumentCreationVM.DocumentVolume) + 1;
                        presentTreeVM.DocumentCreationVM.ArchivePresentVis = Visibility.Visible;
                    }

                    presentTreeVM.DocumentCreationVM.DocumentPanelVis = Visibility.Visible;
                } else MessagesService.Show("СОЗДАНИЕ ДОКУМЕНТА", "Необходимо выбрать или создать низшую ветку.");
            } else MessagesService.Show("СОЗДАНИЕ ДОКУМЕНТА", "Не выбрана ветка");
        }


        public ICommand UpdateDocumentCmd => new DelegateCommand(UpdateDocument);
        private void UpdateDocument()
        {
            if (SelectedDocument != null) {
                presentTreeVM.DocumentCreationVM.TitleHeader = "РЕДАКТИРОВНИЕ ДОКУМЕНТА";
                presentTreeVM.DocumentCreationVM.CurrentNode = presentTreeVM.SelectedNodeVM;
                presentTreeVM.DocumentCreationVM.CurrentPresentation = present;
                presentTreeVM.DocumentCreationVM.UploadDocumentParams(SelectedDocument.Document);

                if (present == PresentationEnum.Archive) presentTreeVM.DocumentCreationVM.ArchivePresentVis = Visibility.Visible;

                presentTreeVM.DocumentCreationVM.DocumentPanelVis = Visibility.Visible;
            } else MessagesService.Show("РЕДАКТИРОВАНИЕ ДОКУМЕНТА", "Не выбрана ветка");
        }


        public ICommand DeleteDocumentCmd => new DelegateCommand(DeleteDocument);
        private void DeleteDocument()
        {
            if (SelectedDocument != null) {
                DocumentService.DeleteDocument(SelectedDocument.Document.id);
                Documents.Remove(SelectedDocument);
            } else MessagesService.Show("УДАЛЕНИЕ", "Не выбран документ для удаления");
        }


        public ICommand PutOnPathCmd => new DelegateCommand(PutOnPath);
        private void PutOnPath()
        {
            if (SelectedDocument != null) {
                if (SelectedDocument.Document.exchange.Contains("Archive")) {
                    SelectedDocument = DocumentFromTS(SelectedDocument);
                }

                var curNodeVM = TemporaryDataService.GetCurrentNodeVM();

                if (curNodeVM != null && curNodeVM.IsLastLevel) {
                    // Get archive number path
                    string query = string.Format("{0}={1},{2}", curNodeVM.Node.Level_id, curNodeVM.Node.Name, presentTreeVM.GetParentValues(curNodeVM.Parent));

                    // Update document with archive data path
                    DocumentService.UpdateDocument(SelectedDocument.Document.id, (int)present, query);

                    // Update document with serial number
                    var curPresentTreeVM = TemporaryDataService.GetCurrentPresentTreeVM();
                    int serialNumber = 1;

                    if (curPresentTreeVM.DetailsVM.Documents != null && curPresentTreeVM.DetailsVM.Documents.Count > 0) {
                        serialNumber = curPresentTreeVM.DetailsVM.Documents.Max(d => (int)d.Document.serialNumber) + 1;
                    }

                    DocumentService.UpdateDocumentWithASN(SelectedDocument.Document.id, serialNumber);

                    // Update details view with document
                    curPresentTreeVM.DetailsVM.Documents.Add(new DocumentVM(DocumentService.ReadDocument(SelectedDocument.Document.id)));

                    // Update current view by deleting element
                    Documents.Remove(SelectedDocument);
                } else MessagesService.Show("ПОЛОЖИТЬ В АРХИВ", "Не выбрана конечная ветка");
            } else MessagesService.Show("ПОЛОЖИТЬ В АРХИВ", "Не выбран документ");
        }


        public ICommand ExportToExcelCmd => new DelegateCommand(ExportToExcel);
        public void ExportToExcel()
        {
            if (Documents != null && Documents.Count > 0) {
                // Choose path and name to save
                string fileName = FileSystemService.SaveFile("(*.xls;*.xlsx) | *.xls;*.xlsx");

                if (!string.IsNullOrEmpty(fileName)) {
                    // Create file
                    // Collect info
                    List<Document> documentsList = new List<Document>();
                    var documentTypes = DocumentService.ReadDocumentTypes();

                    foreach (var item in Documents) {
                        if (documentTypes != null) item.Document.name = documentTypes.FirstOrDefault(d => d.id == item.Document.type).descriptionRU;

                        documentsList.Add(item.Document);
                    }

                    // Formate document
                    ArchiveVolumeRegistryService.FormateDocument(fileName, documentsList);

                    // Open folder with generated file
                    FolderService.OpenFolder(fileName.Substring(0, fileName.LastIndexOf("\\")));
                }
            } else MessagesService.Show("ЭКСПОРТ В EXCEL", "Список документов пуст");
        }


        protected override List<CommandViewModel> CreateCommands()
        {
            throw new NotImplementedException();
        }


        private DocumentVM DocumentFromTS(DocumentVM documentVM)
        {
            // Create document in archive base from trading system
            Document newDocument = new Document() {
                name = documentVM.Document.name,
                number = documentVM.Document.number,
                description = "_",
                type = 21,
                author = "_",
                createdDate = documentVM.Document.createdDate,
                uploadDate = DateTime.Now,
                company = documentVM.Document.company.Replace("'","")
            };

            // Create link
            var CurrentNode = TemporaryDataService.GetCurrentNodeVM();

            string url = CurrentNode == null ? "" : GetNewPath(CurrentNode);
            string path = string.IsNullOrEmpty(url) ? "" : FolderService.GetFolderPath(1, CurrentNode.Node.Name, url) + string.Format("{0} {1}.{2}", "Договор", newDocument.number.Replace("/", "_").Replace("\\", "_"), documentVM.Document.exchange.Substring(documentVM.Document.exchange.LastIndexOf(".") + 1));

            int linkId = DocumentService.CreateDocumentLink(path, path.Substring(path.LastIndexOf(".") + 1));

            if (linkId > 0) {
                newDocument.linkId = linkId;
                string query = string.Format("{0}={1},{2}", CurrentNode.Node.Level_id, CurrentNode.Node.Name, presentTreeVM.GetParentValues(CurrentNode.Parent));

                // Create document in base
                int newDocId = DocumentService.CreateDocument(newDocument, 1, query);

                if (newDocId > 0) {
                    // Set archive serial number if archive presentation
                    DocumentService.UpdateDocumentWithASN(newDocId, 0);

                    // Copy attached file to destination
                    FileSystemService.CopyFile(documentVM.Document.exchange, path);

                    var newDoc = DocumentService.ReadDocument(newDocId);

                    if (newDoc == null) return null;

                    documentVM.Document = newDoc;

                    return documentVM;
                }
            }

            return null;
        }


        private string GetNewPath(NodeVM nodeVM)
        {
            if (nodeVM.Node.Level_id == 1) return string.Format("{0}", nodeVM.Node.Name);
            else return string.Format("{1}\\{0}", nodeVM.Node.Name, GetNewPath(nodeVM.Parent));
        }
        #endregion

        #region Bindings
        private ObservableCollection<DocumentVM> documents = new ObservableCollection<DocumentVM>();
        public ObservableCollection<DocumentVM> Documents { get { return documents; } set { if (value != documents) { documents = value; RaisePropertyChangedEvent("Documents"); selectedDocument = documents[0]; } } }


        private DocumentVM selectedDocument;
        public DocumentVM SelectedDocument {
            get { return selectedDocument; }
            set {
                selectedDocument = value;
                try {
                    if (value != null) presentTreeVM.DetailsInfoVM.ShowDocument(value.Document);
                    RaisePropertyChangedEvent("SelectedDocument");
                } catch (NullReferenceException) {
                    //NOP
                }
            }
        }


        private string address;
        public string Address { get { return address; } set { if (value != address) { address = value; RaisePropertyChangedEvent("Address"); } } }


        private Visibility _listVis;
        public Visibility ListVis {
            get { return _listVis; }
            set { _listVis = value; RaisePropertyChangedEvent("ListVis"); }
        }
        #endregion
    }
}
