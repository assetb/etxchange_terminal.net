using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using altaik.baseapp.vm;
using AltaArchiveApp.models;
using AltaArchiveApp.Services;
using System.Collections.ObjectModel;
using AltaBO.archive;
using AltaArchiveUI.service;
using altaik.baseapp.ext;
using System.Windows.Input;
using altaik.baseapp.vm.command;
using AltaDock.vm;
using System.Windows;
using System.Diagnostics;

namespace AltaArchiveUI.vm
{
    public class PresentTreeVM : PanelViewModelBase
    {
        #region Constructors and Initializations
        private PresentTreeSettings settings;


        public PresentTreeVM(PresentationEnum present)
        {
            Init(present, NodeService.ReadRoot((int)present));
        }


        private void Init(PresentationEnum present, List<string> nodesList)
        {
            this.settings = new PresentTreeSettings() { presentation = present };
            DisplayName = settings.presentation.GetName();
            NodeVMs = ConvertToNodes(nodesList, 1);
            DetailsVM = new PresentDetailVM(this, settings.presentation);
            DocumentCreationVM = new DocumentCreationVM(this);
            DetailsInfoVM = new DetailsInfoVM();
        }
        #endregion

        #region Methods
        private void SelectedNodeChangedHandler()
        {
            if (SelectedNodeVM.IsLastLevel)
            {
                string query = string.Format("{0}={1},{2}", SelectedNodeVM.Node.Level_id, SelectedNodeVM.Node.Name, GetParentValues(SelectedNodeVM.Parent));
                var documents = DocumentService.ReadDocuments((int)settings.presentation, SelectedNodeVM.Node.Level_id, query);

                if (documents != null && documents.Count > 0)
                {
                    DetailsVM.Documents = new ObservableCollection<DocumentVM>(DetailsVM.ConvertDocumentToDocumentVM(documents));
                }
            }
            else
            {
                if (DetailsVM.Documents != null) DetailsVM.Documents.Clear();

                if (SelectedNodeVM.Children == null || SelectedNodeVM.Children.Count() == 0)
                {
                    var result = GetChildNodes();
                    if (result != null) SelectedNodeVM.Children = result;
                }
            }
        }


        private ObservableCollection<NodeVM> GetChildNodes()
        {
            string query = "";

            if (SelectedNodeVM.Node.Level_id == 1) query = string.Format("{0}={1},", SelectedNodeVM.Node.Level_id, SelectedNodeVM.Node.Name);
            else query = string.Format("{0}={1},{2}", SelectedNodeVM.Node.Level_id, SelectedNodeVM.Node.Name, GetParentValues(SelectedNodeVM.Parent));

            var result = ConvertToNodes(NodeService.ReadNodes((int)settings.presentation, SelectedNodeVM.Node.Level_id + 1, query), SelectedNodeVM.Node.Level_id + 1);

            if (result != null) return new ObservableCollection<NodeVM>(result);
            else return null;
        }


        public string GetParentValues(NodeVM parent)
        {
            string query = string.Format("{0}={1},", parent.Node.Level_id, parent.Node.Name);

            if (parent.Node.Level_id == 1) return query;
            else return query + GetParentValues(parent.Parent);
        }


        private ObservableCollection<NodeVM> ConvertToNodes(List<string> nodesList, int level)
        {
            if (nodesList != null)
            {
                bool isLastLevel = level == NodeService.GetLastLevel((int)settings.presentation) ? true : false;

                List<NodeVM> result = new List<NodeVM>();

                foreach (var node in nodesList)
                {
                    if (!string.IsNullOrEmpty(node)) result.Add(new NodeVM(new Node() { Level_id = level, Name = node }, false) { IsLastLevel = isLastLevel, Parent = this.SelectedNodeVM });
                }

                return new ObservableCollection<NodeVM>(result);
            }
            else return null;
        }


        #region Work with node
        public ICommand AddNodeCmd => new DelegateCommand(() => AddNode(false));
        public ICommand AddChildNodeCmd => new DelegateCommand(() => AddNode(true));

        private async void AddNode(bool isChild)
        {
            if (SelectedNodeVM != null)
            {
                if (SelectedNodeVM.IsLastLevel && isChild)
                {
                    MessagesService.Show("ДОБАВЛЕНИЕ", "Выбран низший уровень. Добавление не возможно.");
                    return;
                }

                string nodeName = await MessagesService.GetInput("ДОБАВЛЕНИЕ ВЕТКИ", "Введите наименование");

                if (!string.IsNullOrEmpty(nodeName))
                {
                    if (!isChild)
                    {
                        if (SelectedNodeVM.Node.Level_id == 1)
                        {
                            if (NodeVMs.Count(n => n.Node != null && n.Node.Name != null && n.Node.Name.ToLower() == nodeName.ToLower()) == 0) NodeVMs.Add(new NodeVM(new Node() { Level_id = SelectedNodeVM.Node.Level_id, Name = nodeName }, false) { IsLastLevel = SelectedNodeVM.IsLastLevel, Parent = SelectedNodeVM.Parent });
                            else MessagesService.Show("ДОБАВЛЕНИЕ ВЕТКИ", "Ветка с таким именем уже существует");
                        }
                        else
                        {
                            if (SelectedNodeVM.Parent.Children.Count(n => n.Node.Name.ToLower() == nodeName.ToLower()) == 0) SelectedNodeVM.Parent.Children.Add(new NodeVM(new Node() { Level_id = SelectedNodeVM.Node.Level_id, Name = nodeName }, false) { IsLastLevel = SelectedNodeVM.IsLastLevel, Parent = SelectedNodeVM.Parent });
                            else MessagesService.Show("ДОБАВЛЕНИЕ ВЕТКИ", "Ветка с таким именем уже существует");
                        }
                    }
                    else
                    {
                        if (SelectedNodeVM.Children.Count(n => n.Node.Name.ToLower() == nodeName.ToLower()) == 0)
                        {
                            SelectedNodeVM.Children.Add(new NodeVM(new Node() { Level_id = SelectedNodeVM.Node.Level_id + 1, Name = nodeName }, false) { IsLastLevel = (SelectedNodeVM.Node.Level_id + 1 == NodeService.GetLastLevel((int)settings.presentation) ? true : false), Parent = SelectedNodeVM });
                        }
                        else MessagesService.Show("ДОБАВЛЕНИЕ ВЕТКИ", "Ветка с таким именем уже существует");
                    }
                }
            }
            else MessagesService.Show("ДОБАВЛЕНИЕ", "Не выбрана ветка.");
        }


        public ICommand UpdateNodeCmd => new DelegateCommand(UpdateNode);
        private async void UpdateNode()
        {
        }


        public ICommand DeleteNodeCmd => new DelegateCommand(DeleteNode);
        private async void DeleteNode()
        {
            if (SelectedNodeVM != null)
            {
                if ((SelectedNodeVM.Children != null && SelectedNodeVM.Children.Count > 0) || (DetailsVM.Documents != null && DetailsVM.Documents.Count > 0))
                {
                    MessagesService.Show("УДАЛЕНИЕ", "Сначала удалите вложения.");
                    return;
                }

                if (await MessagesService.AskDialog("УДАЛЕНИЕ", "Вы точно хотите удалить ветку")) SelectedNodeVM.Parent.Children.Remove(SelectedNodeVM);
            }
            else MessagesService.Show("УДАЛЕНИЕ ВЕТКИ", "Не выбрана ветка для удаления");
        }
        #endregion


        public ICommand CreateDocumentCmd => new DelegateCommand(CreateDocument);
        private void CreateDocument()
        {
            if (SelectedNodeVM != null)
            {
                if (SelectedNodeVM.IsLastLevel)
                {
                    DocumentCreationVM.TitleHeader = "СОЗДАНИЕ ДОКУМЕНТА";
                    DocumentCreationVM.CurrentNode = SelectedNodeVM;
                    DocumentCreationVM.CurrentPresentation = settings.presentation;
                    DocumentCreationVM.IsAttach = false;

                    if (settings.presentation == PresentationEnum.Archive)
                    {
                        if (DocumentCreationVM.BrokersList.Count(bl => bl.ShortName.ToLower().Contains(SelectedNodeVM.Parent.Parent.Node.Name.ToLower())) > 0)
                        {
                            DocumentCreationVM.SelectedBroker = DocumentCreationVM.BrokersList.First(bl => bl.ShortName.ToLower().Contains(SelectedNodeVM.Parent.Parent.Node.Name.ToLower()));
                            SelectedNodeVM.Parent.Parent.Node.Name = DocumentCreationVM.SelectedBroker.ShortName;
                        }

                        DocumentCreationVM.DocumentYear = SelectedNodeVM.Parent.Parent.Parent.Node.Name;
                        DocumentCreationVM.DocumentCase = SelectedNodeVM.Parent.Node.Name;
                        DocumentCreationVM.DocumentVolume = SelectedNodeVM.Node.Name;
                        DocumentCreationVM.DocumentSerialNumber = DocumentService.GetNextSerialNumber(DocumentCreationVM.DocumentYear,
                                                                                                    SelectedNodeVM.Parent.Parent.Node.Name,
                                                                                                    DocumentCreationVM.DocumentCase,
                                                                                                    DocumentCreationVM.DocumentVolume) + 1;
                        DocumentCreationVM.ArchivePresentVis = Visibility.Visible;
                    }

                    DocumentCreationVM.DocumentPanelVis = Visibility.Visible;
                }
                else MessagesService.Show("СОЗДАНИЕ ДОКУМЕНТА", "Необходимо выбрать " + (settings.presentation == PresentationEnum.Archive ? "том" : "дату") + ".");
            }
            else MessagesService.Show("СОЗДАНИЕ ДОКУМЕНТА", "Не выбрана ветка");
        }


        public ICommand UpdateDocumentCmd => new DelegateCommand(UpdateDocument);
        private void UpdateDocument()
        {
            if (DetailsVM.SelectedDocument != null)
            {
                if (DocumentCreationVM == null) DocumentCreationVM = new DocumentCreationVM(this);

                DocumentCreationVM.TitleHeader = "РЕДАКТИРОВНИЕ ДОКУМЕНТА";
                DocumentCreationVM.CurrentNode = SelectedNodeVM;
                DocumentCreationVM.CurrentPresentation = settings.presentation;
                DocumentCreationVM.UploadDocumentParams(DetailsVM.SelectedDocument.Document);

                if (settings.presentation == PresentationEnum.Archive) DocumentCreationVM.ArchivePresentVis = Visibility.Visible;

                DocumentCreationVM.DocumentPanelVis = Visibility.Visible;
            }
            else MessagesService.Show("РЕДАКТИРОВАНИЕ ДОКУМЕНТА", "Не выбрана ветка");
        }


        public ICommand DeleteDocumentCmd => new DelegateCommand(DeleteDocument);
        private void DeleteDocument()
        {
            if (DetailsVM.SelectedDocument != null)
            {
                DocumentService.DeleteDocument(DetailsVM.SelectedDocument.Document.id);
                DetailsVM.Documents.Remove(DetailsVM.SelectedDocument);
            }
            else MessagesService.Show("УДАЛЕНИЕ", "Не выбран документ для удаления");
        }


        public ICommand OpenDocumentCmd => new DelegateCommand(OpenDocument);
        private void OpenDocument()
        {
            if (DetailsVM.SelectedDocument != null && DetailsVM.SelectedDocument.Document != null)
            {
                try
                {
                    Process.Start(DocumentService.ReadDocumentLink(DetailsVM.SelectedDocument.Document.id));
                }
                catch { MessagesService.Show("ОШИБКА ОТКРЫТИЯ", "Файла не найдено по сохраненному пути"); }
            }
            else MessagesService.Show("ИЗВЛЕЧЕНИЕ ДОКУМЕНТА", "Не выбран документ для извлечения.");
        }


        public ICommand ShowDocumentsWithoutANCmd => new DelegateCommand(ShowDocumentsWithoutAN);
        private void ShowDocumentsWithoutAN()
        {
            if (InfoDetailsVM == null) InfoDetailsVM = new PresentDetailVM(this, settings.presentation);

            InfoDetailsVM.Documents = InfoDetailsVM.ConvertDocumentToDocumentVM(DocumentService.ReadDocuments(true));
        }


        public ICommand SearchDocumentCmd => new DelegateCommand(SearchDocument);
        private async void SearchDocument()
        {
            string searchQuery = await MessagesService.GetInput("ПОИСК", "Введите текст для поиска:");

            if (!string.IsNullOrEmpty(searchQuery))
            {
                var foundedDocuments = DocumentService.SearchDocuments(searchQuery);
                var foundedInTS = DocumentService.SearchDocumentsInTS(searchQuery);

                if (foundedInTS != null && foundedInTS.Count > 0) foundedDocuments.AddRange(foundedInTS);

                if (foundedDocuments != null && foundedDocuments.Count > 0)
                {
                    if (InfoDetailsVM == null) InfoDetailsVM = new PresentDetailVM(this, settings.presentation);

                    InfoDetailsVM.Documents.Clear();

                    var filteredDocuments = foundedDocuments; //.Where(f => f.serialNumber == null).ToList();

                    //var documentsWith = foundedDocuments.Where(f => f.serialNumber != null).ToList();

                    //if (filteredDocuments != null && documentsWith != null) filteredDocuments.AddRange(documentsWith);
                    //else if (filteredDocuments == null && documentsWith != null) filteredDocuments = documentsWith;

                    //if (documentsWith != null && documentsWith.Count > 0) DetailsVM.Documents = new ObservableCollection<DocumentVM>(DetailsVM.ConvertDocumentToDocumentVM(foundedDocuments));

                    if (filteredDocuments != null && filteredDocuments.Count > 0) InfoDetailsVM.Documents = new ObservableCollection<DocumentVM>(InfoDetailsVM.ConvertDocumentToDocumentVM(filteredDocuments));
                }
                else MessagesService.Show("ПОИСК ДОКУМЕНТОВ","Результатов нет");
            }
        }


        public ICommand PutDocumentCmd => new DelegateCommand(PutDocument);
        private void PutDocument()
        {
            if (InfoDetailsVM != null && InfoDetailsVM.SelectedDocument != null)
            {
                var curNode = TemporaryDataService.GetCurrentNodeVM();

                if (curNode != null)
                {
                    if (curNode.IsLastLevel)
                    {
                        string query = string.Format("{0}={1},{2}", curNode.Node.Level_id, curNode.Node.Name, GetParentValues(curNode.Parent));
                        int documentSerialNumber = 1;

                        if (DetailsVM.Documents != null && DetailsVM.Documents.Count > 0) documentSerialNumber = DetailsVM.Documents.Max(d => (int)d.Document.serialNumber) + 1;

                        DocumentService.UpdateDocument(InfoDetailsVM.SelectedDocument.Document.id, (int)settings.presentation, query);
                        DocumentService.UpdateDocumentWithASN(InfoDetailsVM.SelectedDocument.Document.id, documentSerialNumber);

                        DetailsVM.Documents.Add(new DocumentVM(DocumentService.ReadDocument(InfoDetailsVM.SelectedDocument.Document.id)));
                        InfoDetailsVM.Documents.Remove(InfoDetailsVM.SelectedDocument);
                    }
                    else MessagesService.Show("ПЕРЕМЕЩЕНИЕ В АРХИВ", "Выбранная ветка не является томом");
                }
                else MessagesService.Show("ПЕРЕМЕЩЕНИЕ В АРХИВ", "Не выбрана ветка в которую перемещать");
            }
            else MessagesService.Show("ПЕРЕМЕЩЕНИЕ В АРХИВ", "Не выбрано документа для перемещения");
        }


        public ICommand ExportToExcelCmd => new DelegateCommand(ExportToExcel);
        private void ExportToExcel()
        {
            if (DetailsVM != null) DetailsVM.ExportToExcel();
        }

        protected override List<CommandViewModel> CreateCommands()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Bindings
        private NodeVM selectedNodeVM;
        public NodeVM SelectedNodeVM {
            get { return selectedNodeVM; }
            set {
                if (value != selectedNodeVM)
                {
                    selectedNodeVM = value;
                    RaisePropertyChangedEvent("SelectedNodeVM");
                    SelectedNodeChangedHandler();
                    TemporaryDataService.SetCurrentNodeVM(value);
                    TemporaryDataService.SetCurrentPresentTreeVM(this);
                }
            }
        }


        private ObservableCollection<NodeVM> nodeVMs;
        public ObservableCollection<NodeVM> NodeVMs { get { return nodeVMs; } set { if (value != nodeVMs) { nodeVMs = value; RaisePropertyChangedEvent("NodeVMs"); } } }


        private PresentDetailVM detailsVM;
        public PresentDetailVM DetailsVM { get { return detailsVM; } set { if (value != detailsVM) { detailsVM = value; RaisePropertyChangedEvent("DetailsVM"); } } }


        private PresentDetailVM _infoDetailsVM;
        public PresentDetailVM InfoDetailsVM { get { return _infoDetailsVM; } set { if (value != _infoDetailsVM) { _infoDetailsVM = value; RaisePropertyChangedEvent("InfoDetailsVM"); } } }


        private string displayStatus;
        public string DisplayStatus { get { return displayStatus; } set { if (value != displayStatus) { displayStatus = value; RaisePropertyChangedEvent("DisplayStatus"); } } }


        private NodeVM tempNode;
        public NodeVM TempNode { get { return tempNode; } set { if (tempNode != value) { tempNode = value; RaisePropertyChangedEvent("TempNode"); } } }


        private DocumentCreationVM _documentCreationVM;
        public DocumentCreationVM DocumentCreationVM {
            get { return _documentCreationVM; }
            set { _documentCreationVM = value; RaisePropertyChangedEvent("DocumentCreationVM"); }
        }


        private DetailsInfoVM _detailsInfoVM;
        public DetailsInfoVM DetailsInfoVM {
            get { return _detailsInfoVM; }
            set { _detailsInfoVM = value; RaisePropertyChangedEvent("DetailsInfoVM"); }
        }


        private string _previewFile;
        public string PreviewFile {
            get { return _previewFile; }
            set { _previewFile = value; RaisePropertyChangedEvent("PreviewFile"); }
        }
        #endregion
    }
}