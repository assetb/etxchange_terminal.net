using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using altaik.baseapp.vm;
using System.Windows.Input;
using altaik.baseapp.vm.command;
using System.Windows;
using AltaBO.specifics;
using AltaBO;
using AltaArchiveUI.service;
using AltaArchiveApp.Services;
using AltaDock.vm;
using AltaBO.archive;

namespace AltaArchiveUI.vm
{
    public class DocumentCreationVM : BaseAppViewModel
    {
        #region Variables
        private PresentTreeVM presentTreeVM;
        private Document document;
        #endregion

        #region Methods
        public DocumentCreationVM(PresentTreeVM presentTreeVM)
        {
            this.presentTreeVM = presentTreeVM;

            Init();
        }


        private void Init()
        {
            DocumentTypesList = DocumentService.ReadDocumentTypes();
            SelectedDocumentType = DocumentTypesList[0];
            BrokersList = DataManagerService.TradingInstance().ReadBrokers();
            SelectedBroker = BrokersList[0];
            CompaniesStorageList = DataManagerService.TradingInstance().GetCompanies();
            SelectedCompany = CompaniesStorageList[0];
            AuthorsList = DataManagerService.TradingInstance().ReadTraders();
            SelectedAuthor = AuthorsList[0];
            DocumentDate = DateTime.Now;
        }


        public void UploadDocumentParams(Document document)
        {
            this.document = document;

            DocumentName = document.name;
            DocumentNumber = document.number;
            SelectedDocumentType = DocumentTypesList.First(d => d.id == document.type);
            SelectedBroker = BrokersList.First(b => b.ShortName == document.broker);

            if (CompaniesStorageList.Count(c => c.name.ToLower().Contains(document.company.ToLower())) > 0) SelectedCompany = CompaniesStorageList.First(c => c.name.ToLower().Contains(document.company.ToLower()));

            SearchCompanyTxt = document.company;
            SelectedAuthor = AuthorsList.FirstOrDefault(a => a.name == document.author);

            if (SelectedAuthor == null) SelectedAuthor = AuthorsList.First(a => a.name.ToLower().Contains("не наз"));

            DocumentDate = document.createdDate;
            DocumentDescription = document.description;

            if (CurrentPresentation == PresentationEnum.Archive)
            {
                DocumentYear = document.year;
                DocumentCase = document.case_;
                DocumentVolume = document.volume;
                DocumentSerialNumber = document.serialNumber == null ? 0 : (int)document.serialNumber;
            }

            AttachedDocument = DataManagerService.Instanse().ReadDocumentLink(document.id);

            if (!string.IsNullOrEmpty(AttachedDocument)) IsAttach = true;
        }


        public ICommand AttachDocumentCmd => new DelegateCommand(AttachDocument);
        private void AttachDocument()
        {
            AttachedDocument = FileSystemService.GetFile();

            if (!string.IsNullOrEmpty(AttachedDocument))
            {
                IsAttach = true;
            }
        }


        private Document FillDocument()
        {
            Document doc = new Document()
            {
                name = string.IsNullOrEmpty(DocumentName) ? "_" : DocumentName,
                number = string.IsNullOrEmpty(DocumentNumber) ? "_" : DocumentNumber,
                description = string.IsNullOrEmpty(DocumentDescription) ? "_" : DocumentDescription,
                type = SelectedDocumentType.id,
                author = SelectedAuthor.name,
                createdDate = DocumentDate,
                uploadDate = DateTime.Now,
                company = SelectedCompany != null ? string.IsNullOrEmpty(SearchCompanyTxt) ? "_" : SearchCompanyTxt : SelectedCompany.name
            };

            doc.company.Replace("'", "\"");

            return doc;
        }


        public ICommand SaveCmd => new DelegateCommand(Save);
        private void Save()
        {
            if (IsAttach && !string.IsNullOrEmpty(SearchCompanyTxt) && !string.IsNullOrEmpty(DocumentNumber))
            {
                Document newDocument = FillDocument();

                // Get path from tree nodes
                string url = CurrentNode == null ? "" : GetNewPath(CurrentNode);

                // Check folder exist
                string path = string.IsNullOrEmpty(url) ? "" : FolderService.GetFolderPath((int)CurrentPresentation, CurrentNode.Node.Name, url) + string.Format("{0} {1}.{2}", SelectedDocumentType.descriptionRU, newDocument.number.Replace("/", "_").Replace("\\", "_"), AttachedDocument.Substring(AttachedDocument.LastIndexOf(".") + 1));

                if (document != null && document.id != null && document.id > 0) // Update
                {
                    // Update link if need
                    if (AttachedDocument != DocumentService.ReadDocumentLink(document.id))
                    {
                        if (!string.IsNullOrEmpty(path))
                        {
                            DocumentService.UpdateDocumentLink(path, path.Substring(path.LastIndexOf(".") + 1), (int)document.linkId);

                            // Copy attached file to destination
                            if (FileSystemService.CopyFile(AttachedDocument, path))
                            {
                                MessagesService.Show("РЕДАКТИРОВАНИЕ", "Файл удачно занесен в архив");
                            }
                        }
                        else MessagesService.Show("РЕДАКТИРОВАНИЕ", "Изменение прикрепленного файла не возможно, так как не выбрана ветка пути.\nБудут изменены лишь основные данные.");
                    }

                    // Update document
                    DocumentService.UpdateDocument(newDocument, document.id);

                    // Update serial number
                    if (CurrentPresentation == PresentationEnum.Archive)
                    {
                        DocumentService.UpdateDocumentWithASN(document.id, DocumentSerialNumber);
                    }

                    // Update view
                    presentTreeVM.DetailsVM.SelectedDocument.Document = DocumentService.ReadDocument(document.id);

                    // Close panel
                    Cancel();
                }
                else // Create
                {
                    // Create link
                    int linkId = DocumentService.CreateDocumentLink(path, path.Substring(path.LastIndexOf(".") + 1));

                    if (linkId > 0)
                    {
                        newDocument.linkId = linkId;
                        string query = string.Format("{0}={1},{2}", CurrentNode.Node.Level_id, CurrentNode.Node.Name, presentTreeVM.GetParentValues(CurrentNode.Parent));

                        // Create document in base
                        int newDocId = DocumentService.CreateDocument(newDocument, (int)CurrentPresentation, query);

                        if (newDocId > 0)
                        {
                            // Set archive serial number if archive presentation
                            if (CurrentPresentation == PresentationEnum.Archive)
                            {
                                DocumentService.UpdateDocumentWithASN(newDocId, DocumentSerialNumber);
                            }

                            // Copy attached file to destination
                            if (FileSystemService.CopyFile(AttachedDocument, path))
                            {
                                MessagesService.Show("СОХРАНЕНИЕ", "Файл удачно занесен в архив");

                                // Update view
                                presentTreeVM.DetailsVM.Documents.Add(new DocumentVM(DocumentService.ReadDocument(newDocId)));

                                // Close panel
                                Cancel();
                            }
                            else MessagesService.Show("СОХРАНЕНИЕ", "Произошла ошибка во время занесения файла в архив");
                        }
                        else MessagesService.Show("СОХРАНЕНИЕ", "Произошла ошибка во время сохранения документа в базе");
                    }
                    else MessagesService.Show("СОХРАНЕНИЕ", "Произошла ошибка во время сохранения пути");
                }
            }
            else MessagesService.Show("СОХРАНЕНИЕ", "Нет прикрепленного документа или не все данные заполненны.");
        }


        private string GetNewPath(NodeVM nodeVM)
        {
            if (nodeVM.Node.Level_id == 1) return string.Format("{0}", nodeVM.Node.Name);
            else return string.Format("{1}\\{0}", nodeVM.Node.Name, GetNewPath(nodeVM.Parent));
        }


        public ICommand CancelCmd => new DelegateCommand(Cancel);
        private void Cancel()
        {
            IsAttach = false;
            DocumentPanelVis = Visibility.Collapsed;
        }


        public ICommand UpdateDateCmd => new DelegateCommand(UpdateDate);
        private void UpdateDate()
        {
            DocumentDate = DateTime.Now;
        }


        protected override List<CommandViewModel> CreateCommands()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Bindings
        private bool _isAttach = false;
        public bool IsAttach {
            get { return _isAttach; }
            set { _isAttach = value; RaisePropertyChangedEvent("IsAttach"); }
        }


        private string _attachedDocument;
        public string AttachedDocument {
            get { return _attachedDocument; }
            set { _attachedDocument = value; RaisePropertyChangedEvent("AttachedDocument"); }
        }


        private Visibility _documentPanelVis = Visibility.Collapsed;
        public Visibility DocumentPanelVis {
            get { return _documentPanelVis; }
            set {
                _documentPanelVis = value;

                if (value == Visibility.Collapsed)
                {
                    if (document == null) document = new Document();

                    document.id = 0;

                    ArchivePresentVis = Visibility.Collapsed;
                }

                RaisePropertyChangedEvent("DocumentPanelVis");
            }
        }


        private string _documentName;
        public string DocumentName {
            get { return _documentName; }
            set { _documentName = value; RaisePropertyChangedEvent("DocumentName"); }
        }


        private string _documentNumber;
        public string DocumentNumber {
            get { return _documentNumber; }
            set { _documentNumber = value; RaisePropertyChangedEvent("DocumentNumber"); }
        }


        private string _documentDescription;
        public string DocumentDescription {
            get { return _documentDescription; }
            set { _documentDescription = value; RaisePropertyChangedEvent("DocumentDescription"); }
        }


        private DateTime _documentDate;
        public DateTime DocumentDate {
            get { return _documentDate; }
            set { _documentDate = value; RaisePropertyChangedEvent("DocumentDate"); }
        }


        private List<DocumentType> _documentTypesList;
        public List<DocumentType> DocumentTypesList {
            get { return _documentTypesList; }
            set {
                _documentTypesList = value;
                SelectedDocumentType = value[0];
                RaisePropertyChangedEvent("DocumentTypesList");
            }
        }


        private DocumentType _selectedDocumentType;
        public DocumentType SelectedDocumentType {
            get { return _selectedDocumentType; }
            set {
                _selectedDocumentType = value;
                DocumentName = value.descriptionRU;
                RaisePropertyChangedEvent("SelectedDocumentType");
            }
        }


        private string _documentAuthor;
        public string DocumentAuthor {
            get { return _documentAuthor; }
            set { _documentAuthor = value; RaisePropertyChangedEvent("DocumentAuthor"); }
        }


        private PresentationEnum _currentPresentation;
        public PresentationEnum CurrentPresentation {
            get { return _currentPresentation; }
            set { _currentPresentation = value; RaisePropertyChangedEvent("CurrentPresentation"); }
        }


        private NodeVM _currentNode;
        public NodeVM CurrentNode {
            get { return _currentNode; }
            set { _currentNode = value; RaisePropertyChangedEvent("CurrentNode"); }
        }


        private List<Broker> _brokersList;
        public List<Broker> BrokersList {
            get { return _brokersList; }
            set { _brokersList = value; RaisePropertyChangedEvent("BrokersList"); }
        }


        private Broker _selectedBroker;
        public Broker SelectedBroker {
            get { return _selectedBroker; }
            set { _selectedBroker = value; RaisePropertyChangedEvent("SelectedBroker"); }
        }


        private string _documentCompany;
        public string DocumentCompany {
            get { return _documentCompany; }
            set { _documentCompany = value; RaisePropertyChangedEvent("DocumentCompany"); }
        }


        private List<Company> _companiesStorageList;
        public List<Company> CompaniesStorageList {
            get { return _companiesStorageList; }
            set { _companiesStorageList = value; RaisePropertyChangedEvent("CompaniesStorageList"); }
        }


        private List<Company> _companiesList;
        public List<Company> CompaniesList {
            get { return _companiesList; }
            set { _companiesList = value; RaisePropertyChangedEvent("CompaniesList"); }
        }


        private Company _selectedCompany;
        public Company SelectedCompany {
            get { return _selectedCompany; }
            set { _selectedCompany = value; RaisePropertyChangedEvent("SelectedCompany"); }
        }


        private string _searchCompanyTxt;
        public string SearchCompanyTxt {
            get { return _searchCompanyTxt; }
            set {
                _searchCompanyTxt = value;

                if (value.Length > 2 && value.Length < 15)
                {
                    CompaniesList = CompaniesStorageList.Where(c => c.name.ToLower().Contains(value.ToLower())).ToList();

                    if (CompaniesList.Count > 0 && CompaniesList.Count < 10) IsDropDown = true;
                }

                RaisePropertyChangedEvent("SearchCompanyTxt");
            }
        }


        private bool _isDropDown;
        public bool IsDropDown {
            get { return _isDropDown; }
            set { _isDropDown = value; RaisePropertyChangedEvent("IsDropDown"); }
        }


        private List<Trader> _authorsList;
        public List<Trader> AuthorsList {
            get { return _authorsList; }
            set { _authorsList = value; RaisePropertyChangedEvent("AuthorsList"); }
        }


        private Trader _selectedAuthor;
        public Trader SelectedAuthor {
            get { return _selectedAuthor; }
            set { _selectedAuthor = value; RaisePropertyChangedEvent("SelectedAuthor"); }
        }


        private Visibility _archivePresentVis = Visibility.Collapsed;
        public Visibility ArchivePresentVis {
            get { return _archivePresentVis; }
            set { _archivePresentVis = value; RaisePropertyChangedEvent("ArchivePresentVis"); }
        }


        private string _documentYear;
        public string DocumentYear {
            get { return _documentYear; }
            set { _documentYear = value; RaisePropertyChangedEvent("DocumentYear"); }
        }


        private string _documentCase;
        public string DocumentCase {
            get { return _documentCase; }
            set { _documentCase = value; RaisePropertyChangedEvent("DocumentCase"); }
        }


        private string _documentVolume;
        public string DocumentVolume {
            get { return _documentVolume; }
            set { _documentVolume = value; RaisePropertyChangedEvent("DocumentVolume"); }
        }


        private int _documentSerialNumber;
        public int DocumentSerialNumber {
            get { return _documentSerialNumber; }
            set { _documentSerialNumber = value; RaisePropertyChangedEvent("DocumentSerialNumber"); }
        }


        private string _titleHeader;
        public string TitleHeader {
            get { return _titleHeader; }
            set { _titleHeader = value; RaisePropertyChangedEvent("TitleHeader"); }
        }

        #endregion
    }
}