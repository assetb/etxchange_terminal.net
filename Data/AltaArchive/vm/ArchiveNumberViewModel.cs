using AltaDock.vm;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using AltaMySqlDB.model;
using AltaMySqlDB.model.tables;
using AltaMySqlDB.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AltaArchive.vm {
    public class ArchiveNumberViewModel : PanelViewModelBase {
        #region Variables
        private IDataManager dataManager = new EntityContext();
        private DocumentEF documentInfo;
        private int brokerId;
        #endregion

        #region Methods
        public ArchiveNumberViewModel(DocumentEF documentItem, int brokerIdItem) {
            documentInfo = documentItem;
            brokerId = brokerIdItem;

            SetDefualtParametrs();
        }


        private void SetDefualtParametrs() {
            DocumentsListVis = Visibility.Collapsed;
            BrokersList = dataManager.GetBrokers();

            FileDescription = documentInfo.name + "." + documentInfo.extension;
            DocumentRequesites = "Тип документа: " + documentInfo.documentType.description_ru + "\n" +
                                 "Дата создания: " + documentInfo.date + "\n";

            // Check for archive number exist            
            if(documentInfo.archiveNumberId != null && documentInfo.archiveNumberId > 0) {
                ArchiveNumber = documentInfo.archiveNumber;
                ArchiveNumberCaseName = ArchiveNumber.Case.Name;
                ArchiveNumberVolumeName = ArchiveNumber.Volume.Name;
            } else {
                ArchiveNumber = new ArchiveNumberEF();

                ArchiveNumber.Year = documentInfo.date != null ? documentInfo.date.Year : DateTime.Now.Year;
                ArchiveNumber.BrokerId = brokerId;
                ArchiveNumber.DocumentNumber = 1;

                ArchiveNumber.Case = new CaseEF();
                ArchiveNumber.Volume = new VolumeEF();
            }

            ArchiveNumberYear = ArchiveNumber.Year;
            ArchiveDocumentNumber = ArchiveNumber.DocumentNumber;

            UpdateCases();

            SelectedBroker = BrokersList.Find(b => b.id == ArchiveNumber.BrokerId);
        }


        private void UpdateCases() {
            CasesList = new List<CaseEF>(dataManager.GetCases(ArchiveNumber.Year, ArchiveNumber.BrokerId));
        }


        private void UpdateVolumes() {
            VolumesList = new List<VolumeEF>(dataManager.GetVolumes(SelectedCase.Id));
        }


        public ICommand SaveCmd { get { return new DelegateCommand(Save); } }
        private void Save() {
            if(SelectedVolume != null) {
                if(documentInfo.archiveNumber == null || documentInfo.archiveNumberId == null || documentInfo.archiveNumberId == 0) {
                    // Create archive number                
                    // I. Create archive number
                    int archiveNumberId = dataManager.AddArchiveNumber(ArchiveNumber);

                    if(archiveNumberId == 0) MessagesService.Show("Создание архивного номера", "Произошла ошибка во время сохранения архивного номера в базе");
                    else {
                        // II. Set archive number to file
                        if(!dataManager.UpdateDocumentWithArchiveNumber(documentInfo.id, archiveNumberId)) MessagesService.Show("Создание архивного номера", "Произошла ошибка во время сохранения архивного номера в базе");
                        else MessagesService.Show("Создание архивного номера", "Архивный номер присвоен успешно");
                    }
                } else {
                    // Update exist
                    if(!dataManager.UpdateArchiveNumber(ArchiveNumber)) MessagesService.Show("Обновление архивного номера", "Произошла ошибка во время обновления архивного номера в базе");
                    else MessagesService.Show("Обновление архивного номера", "Архивный номер обновлен успешно");
                }

                documentInfo = dataManager.GetDocument(documentInfo.id);
            } else MessagesService.Show("Сохранение архивного номера", "Архивному номеру не присвоен том");
        }


        public ICommand AddCaseCmd { get { return new DelegateCommand(AddCase); } }
        private void AddCase() {
            if(!string.IsNullOrEmpty(CaseName)) {
                if(dataManager.AddCase(ArchiveNumber.Year, ArchiveNumber.BrokerId, CaseName) == 0) MessagesService.Show("Создание дела", "Произошла ошибка во время сохранения дела в базе");
                else UpdateCases();
            } else MessagesService.Show("Создание дела", "Не введено наименование дела");
        }


        public ICommand UpdateCaseCmd { get { return new DelegateCommand(UpdateCase); } }
        private void UpdateCase() {
            if(SelectedCase != null) {
                if(!string.IsNullOrEmpty(CaseName)) {
                    if(!dataManager.UpdateCase(SelectedCase.Id, CaseName)) MessagesService.Show("Обновление дела", "Произошла ошибка во время сохранения дела в базе");
                    else UpdateCases();
                } else MessagesService.Show("Обновление дела", "Наименование дела пусто");
            } else MessagesService.Show("Обновление дела", "Не выбрано дело");
        }


        public ICommand DeleteCaseCmd { get { return new DelegateCommand(DeleteCase); } }
        private void DeleteCase() {
            // Check for selection
            // Check for documents in
            // Ask before delete
        }


        public ICommand SearchCaseCmd { get { return new DelegateCommand(SearchCase); } }
        private void SearchCase() {
            if(!string.IsNullOrEmpty(CaseName)) {
                var searchResult = CasesList.FirstOrDefault(cl => cl.Name.ToLower().Contains(CaseName));

                if(searchResult != null) {
                    SelectedCase = searchResult;
                } else MessagesService.Show("Поиск дела", "Дело не найдено");
            } else MessagesService.Show("Поиск дела", "Строка поиска пуста");
        }


        public ICommand AddVolumeCmd { get { return new DelegateCommand(AddVolume); } }
        private void AddVolume() {
            if(SelectedCase != null) {
                if(!string.IsNullOrEmpty(VolumeName)) {
                    if(dataManager.AddVolume(VolumeName, SelectedCase.Id) == 0) MessagesService.Show("Создание тома", "Произошла ошибка во время сохранения тома в базе");
                    else UpdateVolumes();
                } else MessagesService.Show("Создание тома", "Не введено наименование тома");
            } else MessagesService.Show("Создание тома", "Не выбрано дело");
        }


        public ICommand UpdateVolumeCmd { get { return new DelegateCommand(UpdateVolume); } }
        private void UpdateVolume() {
            if(SelectedVolume != null) {
                if(!string.IsNullOrEmpty(VolumeName)) {
                    if(!dataManager.UpdateVolume(SelectedVolume.Id, VolumeName)) MessagesService.Show("Обновление тома", "Произошла ошибка во время сохранения тома в базе");
                    else UpdateVolumes();
                } else MessagesService.Show("Обновление тома", "Наименование тома пусто");
            } else MessagesService.Show("Обновление тома", "Не выбран том");
        }


        public ICommand DeleteVolumeCmd { get { return new DelegateCommand(DeleteVolume); } }
        private void DeleteVolume() {
            // Check for selection
            // Check for documents in
            // Ask before delete
        }


        public ICommand SearchVolumeCmd { get { return new DelegateCommand(SearchVolume); } }
        private void SearchVolume() {
            if(!string.IsNullOrEmpty(VolumeName)) {
                var searchResult = VolumesList.FirstOrDefault(vl => vl.Name.ToLower().Contains(VolumeName));

                if(searchResult != null) {
                    SelectedVolume = searchResult;
                } else MessagesService.Show("Поиск тома", "Том не найден");
            } else MessagesService.Show("Поиск тома", "Строка поиска пуста");
        }


        public ICommand CloseVolumeCmd { get { return new DelegateCommand(CloseVolume); } }
        private void CloseVolume() {
            if(SelectedVolume != null) {
                if(dataManager.SetVolumeStatus(SelectedVolume.Id, 21)) {
                    dataManager.SetVolumeName(SelectedVolume.Id, SelectedVolume.Name + " (закрыт) | (" + dataManager.GetVolumeFirstDocNum(SelectedVolume.Id) + "-" + dataManager.GetVolumeLastDocNum(SelectedVolume.Id) + ")");                    
                    UpdateVolumes();
                }else MessagesService.Show("Закрытие тома", "Произошла ошибка при закрытии тома");
            }
        }


        public ICommand ApplyCmd { get { return new DelegateCommand(Apply); } }
        private void Apply() {
            if(SelectedVolume != null && SelectedCase != null) {
                if(SelectedVolume.StatusId != 21) {
                    ArchiveNumber.Case = SelectedCase;
                    ArchiveNumber.CaseId = SelectedCase.Id;
                    ArchiveNumber.Volume = SelectedVolume;
                    ArchiveNumber.VolumeId = SelectedVolume.Id;

                    ArchiveDocumentNumber = dataManager.GetDocsCountInCase(SelectedCase.Id) + (dataManager.CheckDocInVolume(SelectedVolume.Id, ArchiveNumber.Id) > 0 ? 0 : 1);
                    ArchiveNumberCaseName = ArchiveNumber.Case.Name;
                    ArchiveNumberVolumeName = ArchiveNumber.Volume.Name;

                    ArchiveNumber = ArchiveNumber;
                }else MessagesService.Show("Применить дело и том к архивному номеру", "Том является закрытым");
            } else MessagesService.Show("Применить дело и том к архивному номеру", "Не выбрано дело или том");
        }


        public ICommand VolumeFilesCmd { get { return new DelegateCommand(VolumeFiles); } }
        private void VolumeFiles() {
            if(DocumentsListVis == Visibility.Collapsed) DocumentsListVis = Visibility.Visible;
            else DocumentsListVis = Visibility.Collapsed;
        }
        #region Avalon
        protected override List<CommandViewModel> CreateCommands() {
            throw new NotImplementedException();
        }
        #endregion

        #endregion

        #region Bindings
        private string _fileDescription;
        public string FileDescription {
            get { return _fileDescription; }
            set { _fileDescription = value; RaisePropertyChangedEvent("FileDescription"); }
        }


        private string _documentRequesites;
        public string DocumentRequesites {
            get { return _documentRequesites; }
            set { _documentRequesites = value; RaisePropertyChangedEvent("DocumentRequesites"); }
        }


        private ArchiveNumberEF _archiveNumber;
        public ArchiveNumberEF ArchiveNumber {
            get { return _archiveNumber; }
            set {
                _archiveNumber = value; RaisePropertyChangedEvent("ArchiveNumber");
            }
        }


        private ContractEF _contract;
        public ContractEF Contract {
            get { return _contract; }
            set { _contract = value; RaisePropertyChangedEvent("Contract"); }
        }


        private List<BrokerEF> _brokersList;
        public List<BrokerEF> BrokersList {
            get { return _brokersList; }
            set { _brokersList = value; RaisePropertyChangedEvent("BrokersList"); }
        }


        private BrokerEF _selectedBroker;
        public BrokerEF SelectedBroker {
            get { return _selectedBroker; }
            set {
                _selectedBroker = value;

                if(value != null) {
                    ArchiveNumber.BrokerId = value.id;

                    ArchiveNumber.Volume = null;
                    ArchiveNumber.Case = null;
                    ArchiveNumber = ArchiveNumber;

                    UpdateCases();
                }

                RaisePropertyChangedEvent("SelectedBroker");
            }
        }


        private List<CaseEF> _casesList;
        public List<CaseEF> CasesList {
            get { return _casesList; }
            set {
                _casesList = value;

                if(value == null) {
                    CaseName = "";
                    SelectedCase = null;
                }

                RaisePropertyChangedEvent("CasesList");
            }
        }


        private CaseEF _selectedCase;
        public CaseEF SelectedCase {
            get { return _selectedCase; }
            set {
                _selectedCase = value;

                if(value != null) {
                    CaseName = value.Name;
                    VolumesList = new List<VolumeEF>(dataManager.GetVolumes(value.Id));
                } else VolumesList = new List<VolumeEF>();

                RaisePropertyChangedEvent("SelectedCase");
            }
        }


        private string _caseName;
        public string CaseName {
            get { return _caseName; }
            set { _caseName = value; RaisePropertyChangedEvent("CaseName"); }
        }


        private List<VolumeEF> _volumesList;
        public List<VolumeEF> VolumesList {
            get { return _volumesList; }
            set {
                _volumesList = value;

                if(value == null) {
                    VolumeName = "";
                    SelectedVolume = null;
                }

                RaisePropertyChangedEvent("VolumesList");
            }
        }


        private VolumeEF _selectedVolume;
        public VolumeEF SelectedVolume {
            get { return _selectedVolume; }
            set {
                _selectedVolume = value;

                if(value != null) {
                    VolumeName = value.Name;

                    DocumentsList = dataManager.GetDocumentsWithArchiveNumbersInVolume(value.Id);
                }

                RaisePropertyChangedEvent("SelectedVolume");
            }
        }


        private string _volumeName;
        public string VolumeName {
            get { return _volumeName; }
            set { _volumeName = value; RaisePropertyChangedEvent("VolumeName"); }
        }


        private int _archiveNumberYear;
        public int ArchiveNumberYear {
            get { return _archiveNumberYear; }
            set {
                _archiveNumberYear = value;
                ArchiveNumber.Year = value;

                ArchiveNumber.Volume = null;
                ArchiveNumber.Case = null;
                ArchiveNumber = ArchiveNumber;

                UpdateCases();
                RaisePropertyChangedEvent("ArchiveNumberYear");
            }
        }


        private int _archiveDocumentNumber;
        public int ArchiveDocumentNumber {
            get { return _archiveDocumentNumber; }
            set {
                _archiveDocumentNumber = value;
                ArchiveNumber.DocumentNumber = value;

                RaisePropertyChangedEvent("ArchiveDocumentNumber");
            }
        }


        private string _archiveNumberCaseName;
        public string ArchiveNumberCaseName {
            get { return _archiveNumberCaseName; }
            set { _archiveNumberCaseName = value; RaisePropertyChangedEvent("ArchiveNumberCaseName"); }
        }


        private string _archiveNumberVolumeName;
        public string ArchiveNumberVolumeName {
            get { return _archiveNumberVolumeName; }
            set { _archiveNumberVolumeName = value; RaisePropertyChangedEvent("ArchiveNumberVolumeName"); }
        }


        private List<DocumentEF> _documentsList;
        public List<DocumentEF> DocumentsList {
            get { return _documentsList; }
            set { _documentsList = value;RaisePropertyChangedEvent("DocumentsList"); }
        }


        private Visibility _documentsListVis;
        public Visibility DocumentsListVis {
            get { return _documentsListVis; }
            set { _documentsListVis = value;RaisePropertyChangedEvent("DocumentsListVis"); }
        }
        #endregion
    }
}