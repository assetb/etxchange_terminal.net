using System;
using System.Collections.Generic;
using System.Linq;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using AltaDock.vm;
using AltaMySqlDB.model.tables;
using AltaMySqlDB.model.views;
using AltaTransport;
using System.Collections.ObjectModel;
using System.Windows.Input;
using AltaLog;
using AltaMySqlDB.service;
using AltaMySqlDB.model;

namespace AltaArchive.vm
{
    public class SupplierOrderViewModel : PanelViewModelBase
    {
        #region Variables  
        private IDataManager dataManager = new EntityContext();
        private int auctionId;
        private AuctionViewModel auctionViewModel;
        private List<CompaniesWithContractView> SuppliersListStorage;
        #endregion


        #region Methods
        public SupplierOrderViewModel(int auctionId, AuctionViewModel auctionViewModel, SupplierOrderEF supplierOrderInfo = null)
        {
            this.auctionViewModel = auctionViewModel;
            this.auctionId = auctionId;

            if (supplierOrderInfo != null)
            {
                FormTitle = "Просмотр/редактирование заявки на участие";
                SupplierOrder = supplierOrderInfo;
            }
            else
            {
                FormTitle = "Создания заявки на участие";
            }

            DefaultParametrs(SupplierOrder);
        }


        private void DefaultParametrs(SupplierOrderEF supplierOrder = null, bool refresh = false)
        {
            IsDropDown = false;

            if (!refresh)
            {
                SuppliersListStorage = dataManager.GetSuppliersWithContract();  //DataBaseClient.GetSuppliersWithContract();
                SuppliersList = SuppliersListStorage;

                BrokersList = DataBaseClient.ReadBrokers();
            }

            if (supplierOrder == null)
            {
                SupplierOrder = new SupplierOrderEF();
                SupplierOrder.auctionid = auctionId;
                SupplierOrder.date = DateTime.Now;
                UpdateLotsList();
            }
            else
            {
                try
                {
                    /*if(SupplierOrder.auction.siteid == 4) {
                        RequestDocsList = new ObservableCollection<string>();
                        RequestDocsList.Add("Документы (лицензия, патент, свидетельство) и (или) документы, подтверждающие право потенциального поставщика на производство, переработку, поставку и реализацию закупаемых товаров или письма об отсутствии необходимости наличия документов, подтверждающих право потенциального поставщика на производство, переработку, поставку и реализацию закупаемых товаров.");
                        RequestDocsList.Add("Сертификат представителя завода производителя либо Сертификат CТ-KZ, Сертификаты, в случае, если предмет закупа подлежит обязательной сертификации на соответствие требованиям стандарта или иного нормативного документа в соответствии с законодательством Республики Казахстан о сертификации.");
                        RequestDocsList.Add("Завод-производитель.");
                        RequestDocsList.Add("Официальный представитель завода-производителя.");
                        RequestDocsList.Add("Завод-производитель. Официальный представитель завода-производителя.");
                        RequestDocsList.Add("Дилер.");

                        DocsList = new ObservableCollection<RequestedDocEF>(DataBaseClient.ReadRequestedDocs(SupplierOrder.id));
                        RequestedListVis = System.Windows.Visibility.Visible;
                    } else {
                        RequestedListVis = System.Windows.Visibility.Hidden;
                    }*/

                    UpdateLotsList(false);

                    SelectedSupplier = SuppliersList.FirstOrDefault(x => x.companyId == supplierOrder.supplier.companyid);
                    SearchTxt = SelectedSupplier.companyName;
                    if(supplierOrder.contract!=null) SelectedBroker = BrokersList.FirstOrDefault(x => x.id == supplierOrder.contract.brokerid);
                }
                catch (Exception) { }
            }
        }


        private void UpdateLotsList(bool isNew = true)
        {
            var lots = DataBaseClient.ReadLots(auctionId);

            if (lots != null && lots.Count > 0)
            {
                if (LotsList == null) LotsList = new List<LotsList>();

                LotsList.Clear();

                foreach (var item in lots)
                {
                    LotsList.Add(new LotsList()
                    {
                        id = item.id,
                        number = item.number,
                        description = item.description
                    });
                }

                if (!isNew)
                {
                    if (SupplierOrder.lots != null && SupplierOrder.lots.Count > 0)
                    {
                        foreach (var item in SupplierOrder.lots)
                        {
                            var lotInfo = LotsList.FirstOrDefault(ll => ll.id == item.id);

                            if (lotInfo != null) lotInfo.inplay = true;
                        }
                    }
                }
                else
                {
                    if (LotsList.Count == 1) LotsList[0].inplay = true;
                }

                LotsList = LotsList;
            }
        }


        protected override List<CommandViewModel> CreateCommands()
        {
            return new List<CommandViewModel> {
                new CommandViewModel("Сохранить", new DelegateCommand(Save)),
                new CommandViewModel("Отмена", new DelegateCommand(Cancel))
            };
        }


        private void CheckLots()
        {
            if (SupplierOrder.lots == null) SupplierOrder.lots = new List<LotEF>();

            SupplierOrder.lots.Clear();

            foreach (var item in LotsList.Where(ll => ll.inplay == true))
            {
                var lot = DataBaseClient.ReadLot(item.id);

                if (lot != null) SupplierOrder.lots.Add(lot);
            }
        }


        private void Save()
        {
            AppJournal.Write("SupplierOrder", "Save", true);

            if (SelectedSupplier != null /*&& SelectedBroker != null && SupplierOrder.contractid != 0*/)
            {
                try
                {
                    CheckLots();

                    if (SupplierOrder.id != 0)
                    {
                        DataBaseClient.UpdateSupplierOrder(SupplierOrder);

                        if (auctionViewModel.Auction.siteid != 4) MessagesService.Show("Обновление заявки", "Заявка обновленна");
                    }
                    else
                    {
                        // Create filelist for supplierorder
                        SupplierOrder.fileListId = DataBaseClient.CreateFileList(new FilesListEF() { description = "Заявки поставщика - " + SelectedSupplier.companyName });
                        SupplierOrder.statusid = 1;

                        UpdateView(DataBaseClient.CreateSupplierOrder(SupplierOrder));

                        if (auctionViewModel.Auction.siteid != 4) MessagesService.Show("Создание заявки", "Заявка создана");
                    }

                    auctionViewModel.UpdateSupplierOrdersList();

                }
                catch (Exception ex)
                {
                    MessagesService.Show("ОШИБКА", "Ошибка во время сохранения");
                    AppJournal.Write("SupplierOrder", "Saving in db error :" + ex.ToString(), true);
                }

                Workspace.This.Panels.Remove(Workspace.This.ActiveDocument);
                //else MessagesService.Show("Оповещение", "Не забудьте ввести перечень необходимых документов");
            }
            else MessagesService.Show("ОПОВЕЩЕНИЕ", "Не все поля имеют значения \n1. Hе выбран поставщик или брокер\n2. Нет договора между выбранным брокером и поставщиком");
        }


        private void UpdateView(int id)
        {
            AppJournal.Write("SupplierOrder", "Update supplier order", true);

            try
            {
                SupplierOrder = DataBaseClient.ReadSupplierOrder(id);
            }
            catch (Exception ex) { AppJournal.Write("SupplierOrder", "Get supplier order from db error :" + ex.ToString(), true); }

            FormTitle = "Просмотр/редактирование заявки на участие";
            DefaultParametrs(SupplierOrder, true);
        }


        private void Cancel()
        {
            Workspace.This.Panels.Remove(Workspace.This.ActiveDocument);
        }


        private int GetContractId()
        {
            ContractEF contractId = DataBaseClient.GetContractByCompany(SelectedSupplier.companyId, SelectedBroker.id);

            if (contractId != null && contractId.brokerid == SelectedBroker.id) return contractId.id;
            else return 0;
        }


        // Requested docs list functions
        /*public ICommand AddRequestDocCmd { get { return new DelegateCommand(() => CreateDoc(SelectedRequestDoc)); } }
        public ICommand CreateDocCmd { get { return new DelegateCommand(() => CreateDoc()); } }

        private async void CreateDoc(string reqDoc = null) {
            AppJournal.Write("SupplierOrder", "Create requested doc", true);

            string requestedDocName = "";

            if(string.IsNullOrEmpty(reqDoc)) requestedDocName = await MessagesService.GetInput("Создание запрашиваемого документа", "Введите наименование документа");
            else requestedDocName = reqDoc;

            if(!string.IsNullOrEmpty(requestedDocName)) {
                try {
                    RequestedDocEF requestedDoc = new RequestedDocEF();

                    requestedDoc.supplierorderid = SupplierOrder.id;
                    requestedDoc.name = requestedDocName;

                    DataBaseClient.CreateRequestedDoc(requestedDoc);
                    UpdateDocView();
                } catch(Exception ex) {
                    MessagesService.Show("Создание запрашиваемого документа", "При создании произошла ошибка");
                    AppJournal.Write("SupplierOrder", "Create requested doc error :" + ex.ToString(), true);
                }
            }
        }


        public ICommand UpdateDocCmd { get { return new DelegateCommand(UpdateDoc); } }

        private async void UpdateDoc() {
            AppJournal.Write("SupplierOrder", "Update requested doc", true);

            if(SelectedDoc != null && SelectedDoc.id > 0) {
                string requestedDocName = await MessagesService.GetInput("Обновление запрашиваемого документа", "Введите наименование документа");

                if(!string.IsNullOrEmpty(requestedDocName)) {
                    try {
                        SelectedDoc.name = requestedDocName;

                        DataBaseClient.UpdateRequestedDoc(SelectedDoc);
                        UpdateDocView();
                    } catch(Exception ex) {
                        MessagesService.Show("Обновление запрашиваемого документа", "При обновлении произошла ошибка");
                        AppJournal.Write("SupplierOrder", "Update requested doc error :" + ex.ToString(), true);
                    }
                }
            } else MessagesService.Show("Обновление запрашиваемого документа", "Не выбран элемент запрашиваемго документа");
        }


        private void UpdateDocView() {
            AppJournal.Write("SupplierOrder", "Update requested docs list", true);

            try {
                DocsList = new ObservableCollection<RequestedDocEF>(DataBaseClient.ReadRequestedDocs(SupplierOrder.id));
            } catch(Exception ex) { AppJournal.Write("SupplierOrder", "Get requested docs from db error :" + ex.ToString(), true); }
        }


        public ICommand DeleteDocCmd { get { return new DelegateCommand(DeleteDoc); } }

        private void DeleteDoc() {
            AppJournal.Write("SupplierOrder", "Delete requested doc", true);

            if(SelectedDoc != null && SelectedDoc.id > 0) {
                try {
                    DataBaseClient.DeleteRequestedDoc(SelectedDoc.id);
                    UpdateDocView();
                } catch(Exception ex) {
                    MessagesService.Show("Удаление запрашиваемого документа", "При удалении произошла ошибка");
                    AppJournal.Write("SupplierOrder", "Delete requested doc from db error :" + ex.ToString(), true);
                }
            } else MessagesService.Show("Удаление запрашиваемого документа", "Не выбран элемент запрашиваемго документа");
        }*/
        #endregion


        #region Bindings
        private String _formTitle;
        public String FormTitle {
            get { return _formTitle; }
            set { _formTitle = value; RaisePropertyChangedEvent("FormTitle"); }
        }

        private SupplierOrderEF _supplierOrder;
        public SupplierOrderEF SupplierOrder {
            get { return _supplierOrder; }
            set { _supplierOrder = value; RaisePropertyChangedEvent("SupplierOrder"); }
        }

        private List<CompaniesWithContractView> _suppliersList;
        public List<CompaniesWithContractView> SuppliersList {
            get { return _suppliersList; }
            set { _suppliersList = value; RaisePropertyChangedEvent("SuppliersList"); }
        }

        private CompaniesWithContractView _selectedSupplier;
        public CompaniesWithContractView SelectedSupplier {
            get { return _selectedSupplier; }
            set {
                _selectedSupplier = value;

                if (value != null && SelectedBroker != null) ContractsList = new ObservableCollection<ContractEF>(DataBaseClient.ReadContracts(value.companyId, SelectedBroker.id));

                SupplierOrder.supplierid = DataBaseClient.GetSupplierId(value.companyId);
                RaisePropertyChangedEvent("SelectedSupplier");
            }
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

                if (value != null && SelectedSupplier != null) ContractsList = new ObservableCollection<ContractEF>(DataBaseClient.ReadContracts(SelectedSupplier.companyId, value.id));

                RaisePropertyChangedEvent("SelectedBroker");
            }
        }

        /*private ObservableCollection<RequestedDocEF> _docsList;
        public ObservableCollection<RequestedDocEF> DocsList {
            get { return _docsList; }
            set { _docsList = value; RaisePropertyChangedEvent("DocsList"); }
        }

        private RequestedDocEF _selectedDoc;
        public RequestedDocEF SelectedDoc {
            get { return _selectedDoc; }
            set { _selectedDoc = value; RaisePropertyChangedEvent("SelectedDoc"); }
        }

        private System.Windows.Visibility _requestedListVis;
        public System.Windows.Visibility RequestedListVis {
            get { return _requestedListVis; }
            set { _requestedListVis = value; RaisePropertyChangedEvent("RequestedListVis"); }
        }*/

        private ObservableCollection<ContractEF> _contractsList;
        public ObservableCollection<ContractEF> ContractsList {
            get { return _contractsList; }
            set {
                if (value != null && value.Count > 0)
                {
                    var selCon = value.FirstOrDefault(c => c.companyid == SelectedSupplier.companyId && c.brokerid == SelectedBroker.id && c.id == SupplierOrder.contractid);

                    if (selCon != null) SelectedContract = selCon;
                    else if (selCon == null && value != null) SelectedContract = value[0];
                }
                else
                {
                    value = new ObservableCollection<ContractEF>();
                    RatesList = new ObservableCollection<RatesListEF>();
                    SupplierOrder.contractid = 0;
                }

                _contractsList = value;
                RaisePropertyChangedEvent("ContractsList");
            }
        }

        private ContractEF _selectedContract;
        public ContractEF SelectedContract {
            get { return _selectedContract; }
            set {
                if (SupplierOrder != null) SupplierOrder.contractid = value.id;

                RatesList = new ObservableCollection<RatesListEF>(DataBaseClient.ReadRatesList(value.id));

                if (RatesList != null && RatesList.Count > 0) SelectedRatesList = RatesList[0];
                else SelectedRatesList = new RatesListEF();

                _selectedContract = value;
                RaisePropertyChangedEvent("SelectedContract");
            }
        }

        private ObservableCollection<RatesListEF> _ratesList;
        public ObservableCollection<RatesListEF> RatesList {
            get { return _ratesList; }
            set { _ratesList = value; RaisePropertyChangedEvent("RatesList"); }
        }

        private RatesListEF _selectedRatesList;
        public RatesListEF SelectedRatesList {
            get { return _selectedRatesList; }
            set { _selectedRatesList = value; RaisePropertyChangedEvent("SelectedRatesList"); }
        }

        private ObservableCollection<string> _requestDocsList;
        public ObservableCollection<string> RequestDocsList {
            get { return _requestDocsList; }
            set { _requestDocsList = value; RaisePropertyChangedEvent("RequestDocsList"); }
        }

        private string _selectedRequestDoc;
        public string SelectedRequestDoc {
            get { return _selectedRequestDoc; }
            set { _selectedRequestDoc = value; RaisePropertyChangedEvent("SelectedRequestDoc"); }
        }

        private string _searchTxt;
        public string SearchTxt {
            get { return _searchTxt; }
            set {
                _searchTxt = value;

                if (value.Length > 1)
                {
                    SuppliersList = SuppliersListStorage.Where(s => s.companyName.ToLower().Contains(value.ToLower())).ToList();

                    if (SuppliersList.Count > 0 && SuppliersList.Count < 10) IsDropDown = true;
                }

                RaisePropertyChangedEvent("SearchTxt");
            }
        }

        private bool _isDropDown;
        public bool IsDropDown {
            get { return _isDropDown; }
            set { _isDropDown = value; RaisePropertyChangedEvent("IsDropDown"); }
        }

        private List<LotsList> _lotsList;
        public List<LotsList> LotsList {
            get { return _lotsList; }
            set { _lotsList = value; RaisePropertyChangedEvent("LotsList"); }
        }
        #endregion
    }

    public class LotsList : LotEF
    {
        public bool inplay { get; set; } = false;
    }
}