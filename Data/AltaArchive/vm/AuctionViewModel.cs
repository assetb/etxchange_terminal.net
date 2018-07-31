using System;
using System.Collections.Generic;
using System.Linq;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using AltaDock.vm;
using AltaMySqlDB.model.tables;
using AltaTransport;
using System.Windows.Input;
using AltaArchive.view;
using AltaBO;
using DocumentFormation;
using AltaArchive.Services;
using System.Collections.ObjectModel;
using AltaBO.specifics;
using AltaArchiveApp;
using AltaMySqlDB.service;
using AltaMySqlDB.model;
using AltaLog;
using System.Text.RegularExpressions;


namespace AltaArchive.vm
{
    public class AuctionViewModel : PanelViewModelBase
    {
        #region Variables  
        private string sourceFileName, newFileName;
        private string orderFileName, treatyFileName, orderOriginalFileName;
        private string[] schemesFiles;
        private string errMsg;
        private bool errStatus;
        private CustomersEnum customerEnum;
        private MarketPlaceEnum marketEnum;
        private List<SupplierOrder> notAllowedApplicants;
        private OrderEF orderInfo;
        private IDataManager dataManager = new EntityContext();
        private ArchiveManager archiveManager;
        private string IP_PATH = "";
        #endregion

        #region Methods
        public AuctionViewModel(AuctionEF auctionInfo = null, OrderEF orderInfo = null, MarketPlaceEnum marketInfo = MarketPlaceEnum.ETS)
        {
            if (auctionInfo != null) {
                FormTitle = "Просмотр/редактирование аукциона";
                Auction = auctionInfo;
            } else {
                if (orderInfo != null) this.orderInfo = orderInfo;

                marketEnum = marketInfo;
                FormTitle = "Создания аукциона";
            }

            DefaultParameters(Auction);
            CreateCmds();
        }


        private void DefaultParameters(AuctionEF auction = null, bool refresh = false, bool saveAndNew = false)
        {
            SOInfoTxt = "Выберите заявку от участника";

            if (saveAndNew && orderInfo != null) orderInfo = null;

            if (!refresh) {
                try {
                    archiveManager = new ArchiveManager(dataManager); // Our
                    IP_PATH = archiveManager.ipPath;

                    SectionList = DataBaseClient.ReadSections();
                    TypeList = DataBaseClient.ReadTypes();
                    StatusList = DataBaseClient.ReadStatuses();//.Where(s => s.id < 5).ToList();
                    SourceList = DataBaseClient.ReadSites();
                    CustomerList = DataBaseClient.ReadCustomers();
                    BrokerList = DataBaseClient.ReadBrokers();
                    TraderList = DataBaseClient.ReadTraders().Where(t => t.id < 10).ToList();
                    QualificationsDictionaryLst = dataManager.GetQualificationDictionary();
                } catch (Exception ex) { AppJournal.Write("Auction", "Get data from data base for default parametrs error :" + ex.ToString(), true); }

                if (auction != null && auction.statusid == 1) ReglamentControl(auction.regulation.opendate);
            }

            if (auction == null) {
                var auctionNumber = saveAndNew ? Auction.number : null;

                Auction = new AuctionEF();
                Auction.number = saveAndNew ? auctionNumber : orderInfo == null ? "" : orderInfo.number;

                Auction.ownerid = 1; // TODO Set to user id when authorization will be on
                Auction.ndsincluded = true;

                Auction.regulation = new RegulationEF() {
                    opendate = orderInfo == null ? !saveAndNew ? DateTime.Now : Order.Date : orderInfo.date,
                    closedate = !saveAndNew ? DateTime.Now : Order.Date,
                    applydate = !saveAndNew ? DateTime.Now : ProcessingDate,
                    applydeadline = !saveAndNew ? DateTime.Now : Order.Deadline,
                    applicantsdeadline = !saveAndNew ? DateTime.Now : Order.Auction.ApplicantsDeadline,
                    provisiondeadline = !saveAndNew ? DateTime.Now : Order.Auction.ExchangeProvisionDeadline
                };

                Auction.date = !saveAndNew ? DateTime.Now : Order.Auction.Date;

                if (!saveAndNew) {
                    ReglamentControl(orderInfo == null ? DateTime.Now : orderInfo.date);

                    Order.Deadline = Convert.ToDateTime(Order.Deadline.ToShortDateString() + " 16:00:00");

                    SelectedSection = SectionList[0];
                    SelectedType = TypeList[0];
                    SelectedStatus = StatusList[3];
                    SelectedSource = orderInfo == null ? SourceList.FirstOrDefault(s => s.id == (int)marketEnum) : SourceList.FirstOrDefault(s => s.id == orderInfo.siteid);
                    SelectedCustomer = orderInfo == null ? marketEnum == MarketPlaceEnum.ETS ? CustomerList.FirstOrDefault(c => c.id == 1) : CustomerList.FirstOrDefault(c => c.id == 2) : CustomerList.FirstOrDefault(c => c.id == orderInfo.customerid);
                    SelectedBroker = BrokerList[0];
                    SelectedTrader = TraderList[0];

                    // Auto number for new KazETS on Karazhira customer
                    try {
                        if (Auction.siteid == 5 && Auction.customerid == 2) Auction.number = (Convert.ToInt32(DataBaseClient.ReadAuctions().OrderByDescending(a => a.id).FirstOrDefault(a => a.siteid == 5 && a.customerid == 2).number.Substring(0, 3)) + 1).ToString() + "/" + DateTime.Now.Month + "-" + DateTime.Now.Year;
                    } catch { Auction.number = ""; }
                } else {
                    Auction.sectionid = SelectedSection.id;
                    Auction.typeid = SelectedType.id;
                    Auction.statusid = SelectedStatus.id;
                    Auction.siteid = orderInfo == null ? SelectedSource.id : orderInfo.siteid;
                    Auction.customerid = orderInfo == null ? SelectedCustomer.id : (int)orderInfo.customerid;
                    Auction.brokerid = SelectedBroker.id;
                    Auction.traderid = SelectedTrader.id;
                }

                Auction = Auction; // For changes have been accepted
            } else {
                try {
                    SelectedSection = SectionList.Where(x => x.id == auction.sectionid).First();
                    SelectedType = TypeList.Where(x => x.id == auction.typeid).First();
                    SelectedStatus = StatusList.Where(x => x.id == auction.statusid).First();
                    SelectedSource = SourceList.Where(x => x.id == auction.siteid).First();
                    SelectedCustomer = CustomerList.Where(x => x.id == auction.customerid).First();
                    SelectedBroker = BrokerList.Where(x => x.id == auction.brokerid).First();
                    SelectedTrader = TraderList.Where(x => x.id == auction.traderid).First();
                    LotList = new ObservableCollection<LotEF>(DataBaseClient.ReadLots(auction.id));
                    SupplierOrdersList = new ObservableCollection<SupplierOrderEF>(DataBaseClient.ReadSupplierOrders(auction.id, 16));
                    QualificationsLst = DataBaseClient.ReadQualifications(Auction.id);

                    // Requested documents panel visibility for ETS
                    if (Auction.siteid == 4) ReqDocsVis = System.Windows.Visibility.Visible;
                    else ReqDocsVis = System.Windows.Visibility.Collapsed;

                    // Dates
                    Order.PropertyChanged -= Order_PropertyChanged;
                    Order.Auction.PropertyChanged -= Auction_PropertyChanged;

                    Order.Date = Auction.regulation.opendate;
                    ProcessingDate = Auction.regulation.applydate;
                    Order.Auction.ExchangeProvisionDeadline = Auction.regulation.provisiondeadline;
                    Order.Auction.ApplicantsDeadline = Auction.regulation.applicantsdeadline;
                    Order.Deadline = Auction.regulation.applydeadline;
                    Order.Auction.Date = Auction.date;

                    Order.PropertyChanged += Order_PropertyChanged;
                    Order.Auction.PropertyChanged += Auction_PropertyChanged;
                } catch (Exception ex) { AppJournal.Write("Auction", "Set default parametrs error :" + ex.ToString(), true); }
            }
        }


        private void CreateCmds()
        {
            CommandsSpc = new ObservableCollection<CommandViewModel>();

            //CommandsSpc.Add(new CommandViewModel("TestPT", new DelegateCommand(() => TestPT())));

            CommandsSpc.Add(new CommandViewModel("Сохранить", new DelegateCommand(() => Save())));

            //if (Auction.id == 0 && orderInfo == null && marketEnum != MarketPlaceEnum.KazETS) CommandsSpc.Add(new CommandViewModel("Сохранить и создать новый", new DelegateCommand(Save_Formate)));

            CommandsSpc.Add(new CommandViewModel("Отмена", new DelegateCommand(Cancel)));

            if (Auction.id != 0) AppendCmds();
        }


        private void TestPT() // Work with FTP
        {
            // Config
            archiveManager = new ArchiveManager(dataManager, "ftp://192.168.11.5", "PaydaTradeUser", "123456", ""); // Slava

            // Get requisites
            var req = archiveManager.GetTemplateRequisite(MarketPlaceEnum.ETS, DocumentTemplateEnum.Order);

            // Save document
            var docReq = archiveManager.PutDocument("fromPath", req);

            // Load document
            var result = archiveManager.GetDocument(req, "toPath");
        }


        private void AppendCmds()
        {
            CommandsSpc.Add(new CommandViewModel("Заявка на биржу", new DelegateCommand(() => CreateOrder())));

            if (Auction.siteid == 4) {
                CommandsSpc.Add(new CommandViewModel("Список претендентов", new DelegateCommand(CreateApplicants)));
                CommandsSpc.Add(new CommandViewModel("Исключить из рассылки", new DelegateCommand(ExcludeFromMailDelivery)));
            }

            if (Auction.siteid == 1 || Auction.siteid == 2) CommandsSpc.Add(new CommandViewModel("Шаблон протокола", new DelegateCommand(ProtocolTemplate)));

            if (Auction.siteid != 4) CommandsSpc.Add(new CommandViewModel("Сформировать отчеты", new DelegateCommand(CreateReports)));

            CommandsSpc.Add(new CommandViewModel("Открыть папку", new DelegateCommand(OpenFolder)));
        }


        private bool Save(bool withoutUpdate = false)
        {
            AppJournal.Write("Auction", "Saving auction", true);

            Auction.regulation.opendate = Order.Date;
            Auction.regulation.applydate = ProcessingDate;
            Auction.regulation.provisiondeadline = Order.Auction.ExchangeProvisionDeadline;
            Auction.regulation.applicantsdeadline = Order.Auction.ApplicantsDeadline;
            Auction.regulation.applydeadline = Order.Deadline;
            Auction.date = Order.Auction.Date;

            if (Auction.siteid == 5 && Auction.number == "New") Auction.number = (Convert.ToInt32(DataBaseClient.ReadAuctions().OrderByDescending(a => a.id).FirstOrDefault(a => a.siteid == 5 && a.customerid == 2 && a.statusid == 4).number.Substring(0, 3)) + 1).ToString() + "/" + DateTime.Now.Month + "-" + DateTime.Now.Year;

            if (!withoutUpdate) {
                if (Auction.id != 0) { // Update exist
                    try {
                        DataBaseClient.UpdateAuction(Auction);
                        MessagesService.Show("Сохранение аукциона", "Аукцион успешно сохранен");
                    } catch (Exception ex) {
                        MessagesService.Show("Ошибка", "Произошла ошибка при обновлении аукциона");
                        AppJournal.Write("Auction", "Update auction data db error :" + ex.ToString(), true);
                        return false;
                    }
                } else { // Create new
                    try {
                        if ((SelectedSource.id == 1 || SelectedSource.id == 2) && SelectedCustomer.id == 2) // UTB (Karazhira), generate next serial number for order and auction
                            Order.Auction.Number = DataBaseClient.CreateNextSerialNumber(1) + "/" + Order.Auction.Date.Month + "-" + Order.Auction.Date.Year.ToString().Substring(2, 2);
                        else if (SelectedSource.id == 4 || (SelectedSource.id == 1 || SelectedSource.id == 2)) Auction.ownerid = 1; // ETS or UTB (Not Karazhira), set default user

                        if (DataBaseClient.ReadAuctions().Count(a => a.number == Auction.number) < 1) {
                            UpdateView(DataBaseClient.CreateAuction(Auction));
                            CreateCmds();

                            if (orderInfo != null) CreateOrder();
                            else MessagesService.Show("Сохранение аукциона", "Аукцион успешно создан");
                        } else {
                            errStatus = true;
                            MessagesService.Show("Ошибка", "Аукцион с таким номером уже существует");
                        }

                    } catch (Exception ex) {
                        MessagesService.Show("Ошибка", "Произошла ошибка при создании аукциона");
                        AppJournal.Write("Auction", "Create auction in db error :" + ex.ToString(), true);
                        return false;
                    }
                }
            } else {
                if ((SelectedSource.id == 1 || SelectedSource.id == 2) && SelectedCustomer.id == 2) // UTB (Karazhira),  generate next serial number for order and auction
                    Order.Auction.Number = DataBaseClient.CreateNextSerialNumber(1) + "/" + Order.Auction.Date.Month + "-" + Order.Auction.Date.Year.ToString().Substring(2, 2);
                else if (SelectedSource.id == 4 || (SelectedSource.id == 1 || SelectedSource.id == 2)) Auction.ownerid = 1; // ETS or UTB (Not Karazhira), set default user                                                                                                                           
            }

            return true;
        }


        public void Save_Formate()
        {
            if (Save(true)) {
                try {
                    if (DataBaseClient.ReadAuctions().Count(a => a.number == Auction.number) < 1) {
                        DataBaseClient.CreateAuction(Auction);
                        errStatus = false;
                    } else {
                        errStatus = true;
                        MessagesService.Show("Ошибка", "Аукцион с таким номером уже существует");
                    }
                } catch (Exception ex) {
                    MessagesService.Show("Ошибка", "Ошибка при создании аукциона\nОшибка базы данных");
                    errStatus = true;
                    AppJournal.Write("Auction", "Create auction in db error :" + ex.ToString(), true);
                }

                if (!errStatus) CreateOrder(false); // Async method!!! Formate order                   

            } else MessagesService.Show("Ошибка", "Ошибка при сохранении аукциона");
        }


        private DocumentRequisite GetDocumentRequisites(string fileName, DocumentTypeEnum docType, DocumentSectionEnum docSec = DocumentSectionEnum.Auction)
        {
            var docReq = new DocumentRequisite() {
                fileName = fileName,
                date = Auction.date,
                market = Auction.siteid == 4 ? MarketPlaceEnum.ETS : Auction.siteid == 5 ? MarketPlaceEnum.KazETS : Auction.siteid == 6 ? MarketPlaceEnum.Caspy : MarketPlaceEnum.UTB,
                number = Auction.number,
                section = docSec,
                type = docType
            };

            return docReq;
        }


        #region FillOrder
        private void FillOrder(int level, int lotId = 0)
        {
            // Disable date control
            Order.PropertyChanged -= Order_PropertyChanged;
            Order.Auction.PropertyChanged -= Auction_PropertyChanged;

            // Order 
            Order.Auction.Id = Auction.id;
            Order.Auction.Customer = Auction.customer.company.name;
            Order.customerid = Auction.customerid;
            Order.Auction.Status = Auction.status.name;
            Order.Auction.Site = (Auction.siteid == 4 || Auction.siteid == 5) ? Auction.site.name : Auction.site.company.name;
            Order.Auction.SiteId = Auction.siteid;
            Order.Auction.Trader = Auction.trader.person.name;
            Order.Auction.Type = Auction.type.name;

            Order.Auction.Broker = new Broker();
            Order.Auction.Broker.Code = (Auction.brokerid == 1 ? "ALTK" : Auction.brokerid == 2 ? "KORD" : Auction.brokerid == 3 ? "ALTA" : "AKAL");

            if (Auction.ownerid > 1) Order.Initiator = Auction.owner.person.name;
            else Order.Initiator = "Саркисян Илья";

            if (Order.Auction.Status.Contains("Новый") && Auction.customerid == 2) Order.Auction.Number = DataBaseClient.CreateNextSerialNumber(1) + "/" + Order.Auction.Date.Month + "-" + Order.Auction.Date.Year.ToString().Substring(2, 2);
            else Order.Auction.Number = Auction.number;

            Order.Auction.Comments = Auction.comments;

            Order.Auction.RegulationId = Auction.regulationid;
            Order.Date = Auction.regulation.opendate;
            Order.Auction.Date = Auction.date;
            Order.Deadline = Auction.regulation.applydeadline;
            Order.Auction.ApplicantsDeadline = Auction.regulation.applicantsdeadline;
            Order.Auction.ExchangeProvisionDeadline = Auction.regulation.provisiondeadline;

            if (LotList != null && LotList.Count > 0) {
                if (Order.Auction.Lots == null) Order.Auction.Lots = new ObservableCollection<Lot>();

                Order.Auction.Lots.Clear();

                if (lotId == 0) {
                    foreach (var item in LotList) {
                        Order.Auction.Lots.Add(new Lot {
                            Id = item.id,
                            Number = item.number,
                            Name = item.description,
                            Unit = item.unit.description,
                            Quantity = item.amount,
                            Price = item.price,
                            Sum = item.price * item.amount,
                            PaymentTerm = item.paymentterm,
                            DeliveryPlace = item.deliveryplace,
                            DeliveryTime = item.deliverytime,
                            Step = (decimal)item.step,
                            Warranty = (decimal)item.warranty,
                            LocalContent = item.localcontent
                        });
                    }
                } else {
                    Order.Auction.Lots.Add(new Lot {
                        Id = (int)SelectedProcuratory.lotid,
                        Number = SelectedProcuratory.lot.number,
                        Name = SelectedProcuratory.lot.description,
                        Unit = SelectedProcuratory.lot.unit.description,
                        Quantity = SelectedProcuratory.lot.amount,
                        Price = SelectedProcuratory.lot.price,
                        Sum = SelectedProcuratory.lot.price * SelectedProcuratory.lot.amount,
                        PaymentTerm = SelectedProcuratory.lot.paymentterm,
                        DeliveryPlace = SelectedProcuratory.lot.deliveryplace,
                        DeliveryTime = SelectedProcuratory.lot.deliverytime,
                        Step = (decimal)SelectedProcuratory.lot.step,
                        Warranty = (decimal)SelectedProcuratory.lot.warranty,
                        LocalContent = SelectedProcuratory.lot.localcontent
                    });
                }
            }

            // Supplier order
            if (level == 1) {
                if (Order.Auction.SupplierOrders == null) Order.Auction.SupplierOrders = new ObservableCollection<SupplierOrder>();

                Order.Auction.SupplierOrders.Clear();

                Order.Auction.SupplierOrders.Add(new SupplierOrder {
                    Id = SelectedSupplierOrder.id,
                    Name = SelectedSupplierOrder.supplier.company.name,
                    BIN = SelectedSupplierOrder.supplier.company.bin,
                    CompanyDirector = SelectedSupplierOrder.supplier.company.director,
                    Address = SelectedSupplierOrder.supplier.company.addresslegal,
                    Phones = SelectedSupplierOrder.supplier.company.telephone,
                    BrokerName = SelectedSupplierOrder.contract == null ? "" : SelectedSupplierOrder.contract.broker.name,
                    BrokerBIN = SelectedSupplierOrder.contract == null ? "" : SelectedSupplierOrder.contract.broker.company.bin,
                    BrokerAddress = SelectedSupplierOrder.contract == null ? "" : SelectedSupplierOrder.contract.broker.company.addresslegal,
                    BrokerPhones = SelectedSupplierOrder.contract == null ? "" : SelectedSupplierOrder.contract.broker.company.telephone,
                    BrokerIIK = SelectedSupplierOrder.contract == null ? "" : SelectedSupplierOrder.contract.broker.company.iik,
                    BrokerKbe = SelectedSupplierOrder.contract == null ? "" : SelectedSupplierOrder.contract.broker.company.kbe.ToString(),
                    Trader = SelectedSupplierOrder.contract == null ? "" : SelectedSupplierOrder.contract.broker.company.director,
                    ContractNum = SelectedSupplierOrder.contract == null ? "" : SelectedSupplierOrder.contract.number,
                    ContractDate = SelectedSupplierOrder.contract == null ? DateTime.Now : (DateTime)SelectedSupplierOrder.contract.agreementdate,
                    SupplierId = (int)SelectedSupplierOrder.supplierid
                });

                if (Auction.siteid == 4) { // ETS
                    string supplierCodeC01 = "";
                    string brokerCodeC01 = "";

                    try {
                        var supplierCodes = DataBaseClient.ReadSuppliersJournals((int)SelectedSupplierOrder.supplierid);

                        if (supplierCodes != null) {
                            var supplierCode = supplierCodes.FirstOrDefault(x => x.brokerid == (SelectedSupplierOrder.contract == null ? 0 : SelectedSupplierOrder.contract.brokerid));

                            if (supplierCode != null) supplierCodeC01 = supplierCode.code;
                            else MessagesService.Show("Не найден код С01 поставщика", "Заявка будет сформирована без него.\nПожалуйста проверьте наличие этого кода.");
                        }

                        var brokerCode = DataBaseClient.GetBrokerCodeC01(SelectedSupplierOrder.contract == null ? 0 : (int)SelectedSupplierOrder.contract.brokerid);

                        if (brokerCode != null) brokerCodeC01 = brokerCode.code;

                    } catch (Exception ex) { AppJournal.Write("Order fill", "Err: " + ex.ToString()); }

                    try {
                        Order.Auction.SupplierOrders[0].Code = supplierCodeC01;
                        Order.Auction.SupplierOrders[0].BrokerCode = brokerCodeC01;
                        Order.Auction.SupplierOrders[0].IIK = SelectedSupplierOrder.supplier.company.iik;
                        Order.Auction.SupplierOrders[0].BIK = SelectedSupplierOrder.contract == null ? "" : SelectedSupplierOrder.contract.bank == null ? "" : SelectedSupplierOrder.contract.bank.company.bik;
                        Order.Auction.SupplierOrders[0].BankName = SelectedSupplierOrder.contract == null ? "" : SelectedSupplierOrder.contract.bank == null ? "" : SelectedSupplierOrder.contract.bank.name;
                        Order.Auction.SupplierOrders[0].MinimalPrice = ProcuratoriesList == null ? Order.Auction.Lots == null ? 0 : Order.Auction.Lots.Sum(l => l.Sum) : ProcuratoriesList.Sum(p => p.minimalprice);
                        Order.Auction.SupplierOrders[0].RequestedDocs = new List<RequestedDoc>();

                        //var reqDocs = DataBaseClient.ReadRequestedDocs(SelectedSupplierOrder.id); // Old 
                        var reqDocs = DataBaseClient.ReadQualifications(Auction.id);

                        if (reqDocs != null) {
                            foreach (var item in reqDocs) {
                                Order.Auction.SupplierOrders[0].RequestedDocs.Add(new RequestedDoc() {
                                    name = item.note
                                });
                            }
                        } else {
                            Order.Auction.SupplierOrders[0].RequestedDocs.Add(new RequestedDoc() {
                                name = Order.Auction.Lots[0].Name
                            });
                        }
                    } catch (Exception ex) { AppJournal.Write("Order fill", "Err: " + ex.ToString()); }
                } else if (Auction.siteid != 4 && Auction.siteid != 5 && Auction.customerid == 2) { // Karazhira
                    if (Order.Auction.Procuratories == null) Order.Auction.Procuratories = new ObservableCollection<Procuratory>();
                    else Order.Auction.Procuratories.Clear();

                    Order.Auction.Procuratories.Add(new Procuratory());

                    if (Order.Auction.SupplierOrders[0].BIN != Order.Auction.SupplierOrders[0].BrokerBIN) {
                        decimal mPrice = 0;

                        try {
                            var procuratory = ProcuratoriesList.FirstOrDefault(p => p.auctionid == Auction.id && p.supplierid == SelectedSupplierOrder.supplierid);

                            if (procuratory != null) mPrice = procuratory.minimalprice;
                            else mPrice = LotList[0].sum;
                        } catch (Exception) { mPrice = LotList[0].sum; }

                        if (mPrice == 0 && Auction.comments[0] != 0) Order.Auction.Procuratories[0].MinimalPrice = Convert.ToDecimal(Auction.comments.Substring(0, Auction.comments.IndexOf("|")));
                        else if (mPrice == 0) Order.Auction.Procuratories[0].MinimalPrice = Order.Auction.Lots[0].Sum;
                        else Order.Auction.Procuratories[0].MinimalPrice = mPrice;
                    } else Order.Auction.Procuratories[0].MinimalPrice = Order.Auction.Lots[0].Sum;
                } else if (Auction.siteid != 4) { // Another UTB customer
                    if (Order.Auction.Procuratories == null) Order.Auction.Procuratories = new ObservableCollection<Procuratory>();
                    else Order.Auction.Procuratories.Clear();

                    int iCount = 0;

                    foreach (var item in LotList) {
                        Order.Auction.Procuratories.Add(new Procuratory());

                        Order.Auction.Procuratories[iCount].MinimalPrice = Order.Auction.Lots[iCount].Sum;
                        iCount++;
                    }
                }
            }

            // Supplier orders for applicants
            if (level == 2) {
                Order.Auction.SupplierOrders.Clear();
                notAllowedApplicants = new List<SupplierOrder>();

                foreach (var item in SupplierOrdersList.Where(s => s.statusid == 15 || s.statusid == 17 || s.statusid == 5 || s.statusid == 22 || s.statusid == 1 || s.statusid == null)) {
                    SupplierOrder supplierOrderItem = new SupplierOrder();

                    supplierOrderItem.Id = item.id;
                    supplierOrderItem.Name = item.supplier.company.name;
                    supplierOrderItem.CompanyDirector = item.supplier.company.director;
                    supplierOrderItem.BrokerName = item.contract == null ? "" : item.contract.broker.name;
                    supplierOrderItem.BrokerBIN = item.contract == null ? "" : item.contract.broker.company.bin;
                    supplierOrderItem.BrokerAddress = item.contract == null ? "" : item.contract.broker.company.addresslegal;
                    supplierOrderItem.BrokerPhones = item.contract == null ? "" : item.contract.broker.company.telephone;
                    supplierOrderItem.Trader = item.contract == null ? "" : item.contract.broker.company.director;
                    supplierOrderItem.ContractNum = item.contract == null ? "" : item.contract.number;
                    supplierOrderItem.ContractDate = item.contract == null ? DateTime.Now : (DateTime)item.contract.agreementdate;

                    if (Auction.siteid == 4) { // ETS
                        var supplierCodes = DataBaseClient.ReadSuppliersJournals((int)item.supplierid);
                        SuppliersJournalEF supplierCode = new SuppliersJournalEF();

                        if (supplierCodes != null) {
                            var supplierJCode = supplierCodes.FirstOrDefault(x => x.brokerid == (item.contract == null ? 0 : item.contract.brokerid));

                            if (supplierJCode == null) MessagesService.Show("Не найден код С01 поставщика", "Документ будет сформирована без него.\nПожалуйста проверьте наличие этого кода.");
                            else supplierCode = supplierJCode;
                        }

                        var brokerCode = item.contract != null ? DataBaseClient.GetBrokerCodeC01((int)item.contract.brokerid) : null;

                        supplierOrderItem.Code = supplierCode != null ? supplierCode.code : "";
                        supplierOrderItem.BIN = item.supplier.company.bin;
                        supplierOrderItem.BrokerCode = brokerCode != null ? brokerCode.code : "";
                        supplierOrderItem.IIK = item.supplier.company.iik;

                        try {
                            supplierOrderItem.BIK = item.contract == null ? "" : item.contract.bank.company.bik;
                            supplierOrderItem.BankName = item.contract == null ? "" : item.contract.bank.name;
                        } catch (Exception) { }

                        supplierOrderItem.Kbe = item.supplier.company.kbe.ToString();
                        supplierOrderItem.Address = item.supplier.company.addresslegal;
                        supplierOrderItem.Phones = item.supplier.company.telephone;
                    }

                    Order.Auction.SupplierOrders.Add(supplierOrderItem);
                }

                if (Auction.siteid == 4) { // ETS
                    foreach (var item in DataBaseClient.ReadSupplierOrders(0)) {
                        if (Order.Auction.SupplierOrders.Where(s => s.Id == item.id).Count() == 0) {
                            SupplierOrder supplierOrderItem = new SupplierOrder();

                            supplierOrderItem.Id = item.id;
                            supplierOrderItem.Name = item.supplier.company.name;
                            supplierOrderItem.CompanyDirector = item.supplier.company.director;
                            supplierOrderItem.BrokerName = item.contract == null ? "" : item.contract.broker.name;
                            supplierOrderItem.BrokerBIN = item.contract == null ? "" : item.contract.broker.company.bin;
                            supplierOrderItem.BrokerAddress = item.contract == null ? "" : item.contract.broker.company.addresslegal;
                            supplierOrderItem.BrokerPhones = item.contract == null ? "" : item.contract.broker.company.telephone;
                            supplierOrderItem.Trader = item.contract == null ? "" : item.contract.broker.company.director;
                            supplierOrderItem.ContractNum = item.contract == null ? "" : item.contract.number;
                            supplierOrderItem.ContractDate = item.contract == null ? DateTime.Now : (DateTime)item.contract.agreementdate;

                            var supplierCode = item.contract != null ? DataBaseClient.ReadSuppliersJournals((int)item.supplierid).Where(x => x.brokerid == item.contract.brokerid).FirstOrDefault() : null;
                            var brokerCode = item.contract != null ? DataBaseClient.GetBrokerCodeC01((int)item.contract.brokerid) : null;

                            supplierOrderItem.Code = supplierCode != null ? supplierCode.code : "";
                            supplierOrderItem.BrokerCode = brokerCode != null ? brokerCode.code : "";
                            supplierOrderItem.IIK = item.supplier.company.iik;
                            supplierOrderItem.BIK = item.contract.bank.company.bik;
                            supplierOrderItem.BankName = item.contract.bank.name;
                            supplierOrderItem.Kbe = item.supplier.company.kbe.ToString();
                            supplierOrderItem.Address = item.supplier.company.addresslegal;

                            notAllowedApplicants.Add(supplierOrderItem);
                        }
                    }
                }
            }

            // Applicants for protocols and reports
            if (level >= 3) {
                if (DataBaseClient.ReadApplicants(Auction.id).Count > 0) {
                    Order.Auction.SupplierOrders.Clear();

                    foreach (var item in DataBaseClient.ReadApplicants(Auction.id)) {
                        Order.Auction.SupplierOrders.Add(new SupplierOrder {
                            Id = (int)item.supplierorderid,
                            Name = item.supplierorder.supplier.company.name,
                            CompanyDirector = item.supplierorder.supplier.company.director,
                            BrokerName = item.supplierorder.contract.broker.name,
                            BrokerBIN = item.supplierorder.contract.broker.company.bin,
                            BrokerAddress = item.supplierorder.contract.broker.company.addresslegal,
                            BrokerPhones = item.supplierorder.contract.broker.company.telephone,
                            Trader = item.supplierorder.contract.broker.company.director,
                            ContractNum = item.supplierorder.contract.number,
                            ContractDate = (DateTime)item.supplierorder.contract.agreementdate
                        });
                    }
                }
            }

            // Protocols (procuratories)
            if (level >= 4) {
                foreach (var item in DataBaseClient.ReadProcuratories(Auction.id)) {
                    Order.Auction.Procuratories.Add(new Procuratory {
                        Id = item.id,
                        SupplierName = item.supplier.company.name,
                        MinimalPrice = item.minimalprice
                    });
                }
            }

            // Activate date control
            Order.PropertyChanged += Order_PropertyChanged;
            Order.Auction.PropertyChanged += Auction_PropertyChanged;
        }
        #endregion


        private string GetFileName(int filesListId, DocumentTypeEnum documentType)
        {
            var orderFile = DataBaseClient.ReadDocument(filesListId, (int)documentType);

            if (orderFile != null) {
                return IP_PATH + @"\Archive\Orders\" + (orderFile.siteid == 4 ? "ETS" : orderFile.siteid == 5 ? "KazETS" : "Caspy") + "\\" + orderFile.date.ToShortDateString() + "\\" + orderFile.number.Replace("/", "_") + "\\" + orderFile.name + "." + orderFile.extension;
            }

            return null;
        }


        private string[] GetSchemes(int filesListId, DocumentTypeEnum documentType = DocumentTypeEnum.Scheme)
        {
            var schemes = DataBaseClient.ReadDocuments(filesListId, (int)documentType);

            if (schemes == null) return null;

            string[] files = new string[schemes.Count];
            int fCount = 0;

            foreach (var item in schemes) {
                files[fCount] = IP_PATH + @"\Archive\Orders\ETS\" + item.date.ToShortDateString() + "\\" + item.number.Replace("/", "_") + "\\" + item.name + "." + item.extension;
                fCount++;
            }

            return files;
        }

        private void CreateOrder(bool createOnce = true)
        {
            AppJournal.Write("Auction", "Formate order", true);

            if (Auction.siteid == 4) FillOrder(0);

            #region UTB
            // UTB (Karazhira)
            if ((Auction.siteid == 1 || Auction.siteid == 2) && Auction.customerid == 2) {
                if (LotList != null || LotList.Count > 0) {
                    try {
                        DataBaseClient.UpdateAuction(Order);
                        try {
                            UTBOrderService.GenerateOrder(Order);
                            errStatus = false;
                        } catch (Exception ex) {
                            AppJournal.Write("Auction", "Formate order error :" + ex.ToString(), true);
                            errStatus = true;
                        }
                    } catch (Exception ex) {
                        errStatus = true;
                        AppJournal.Write("Auction", "Update auction in db error :" + ex.ToString(), true);
                    }
                } else {
                    errStatus = true;
                    errMsg = "Нет лотов для заполнения заявки";
                }
                // UTB (else)
            } else if (Auction.siteid == 1 || Auction.siteid == 2) {
                // Check order
                if (orderInfo == null) { // Hand mode
                    // Get files
                    orderFileName = Service.GetFile("Выберите заявку", "(*.xls;*.xlsx) | *.xls;*.xlsx").FullName;
                    orderOriginalFileName = Service.GetFile("Выберите скан заявки", "(*.pdf) | *.pdf").FullName;
                } else { // Auto mode
                    orderFileName = GetFileName((int)orderInfo.fileslistid, DocumentTypeEnum.OrderSource);
                    orderOriginalFileName = GetFileName((int)orderInfo.fileslistid, DocumentTypeEnum.OrderOriginal);
                }

                // Check for attach order files
                if (!string.IsNullOrEmpty(orderFileName)) {
                    // Copy template files to auction folder
                    var docOrderReq = GetDocumentRequisites("Заявка №" + Auction.number.Replace("/", "_") + ".docx", DocumentTypeEnum.Order);
                    var newOrderFileName = archiveManager.GetTemplate(docOrderReq, DocumentTemplateEnum.Order);

                    // Copy originals to auction folder
                    string[] orderFileNames = archiveManager.CopyOriginalOrder(docOrderReq, orderFileName, null, string.IsNullOrEmpty(orderOriginalFileName) ? null : orderOriginalFileName);

                    // Parse files                   
                    // Fill template with parsed info
                    UTBOrderService.GenerateOrder(UTBOrderService.ParseOrder(orderFileNames[0], Order), newOrderFileName);

                    // Change customer broker

                    // Save files in db
                    archiveManager.SaveFile(docOrderReq, (int)Auction.fileslistid);

                    if (!string.IsNullOrEmpty(orderFileNames[2])) {
                        var docOrderOriginal = GetDocumentRequisites(orderFileNames[2].Substring(orderFileNames[2].LastIndexOf("\\") + 1), DocumentTypeEnum.OrderOriginal);

                        archiveManager.SaveFile(docOrderOriginal, (int)Auction.fileslistid);
                    }
                }
                #endregion

                #region ETS
                // ETS
            } else if (Auction.siteid == 4) {
                if (Auction.customerid == 1) customerEnum = CustomersEnum.Vostok;
                else if (Auction.customerid == 3) customerEnum = CustomersEnum.Inkay;
                else if (Auction.customerid == 4) customerEnum = CustomersEnum.KazMineralsService;

                try {
                    if (orderInfo == null) { // User choose files
                        try {
                            orderFileName = customerEnum == CustomersEnum.Vostok ? OrderTransportUI.GetVostokOrder().FullName : (customerEnum == CustomersEnum.Inkay || customerEnum == CustomersEnum.KazMineralsService) ? OrderTransportUI.GetInkayOrder().FullName : null;

                            if (customerEnum == CustomersEnum.Vostok) treatyFileName = OrderTransportUI.GetTreatyDraft().FullName;

                            try {
                                orderOriginalFileName = Service.GetFile("Выберите скан заявки", "(*.pdf) | *.pdf").FullName;
                            } catch (Exception ex) { AppJournal.Write("Auction", "Choose order scan file error :" + ex.ToString(), true); }
                        } catch (Exception ex) { AppJournal.Write("Auction", "Choose order files error :" + ex.ToString(), true); }
                    } else { // Getting files from archive from order
                        if (customerEnum == CustomersEnum.Inkay || customerEnum == CustomersEnum.KazMineralsService) orderFileName = GetFileName((int)orderInfo.fileslistid, DocumentTypeEnum.OrderSource);
                        else if (customerEnum == CustomersEnum.Vostok) {
                            orderFileName = GetFileName((int)orderInfo.fileslistid, DocumentTypeEnum.OrderSource);
                            treatyFileName = GetFileName((int)orderInfo.fileslistid, DocumentTypeEnum.AgreementSource);
                            schemesFiles = GetSchemes((int)orderInfo.fileslistid);
                        }

                        orderOriginalFileName = GetFileName((int)orderInfo.fileslistid, DocumentTypeEnum.OrderOriginal);

                        if (string.IsNullOrEmpty(orderFileName)) throw new Exception("Файл заявки не найден");
                        if (customerEnum == CustomersEnum.Vostok && string.IsNullOrEmpty(treatyFileName)) throw new Exception("Файл проекта договора не найден");
                    }

                    errStatus = false;
                } catch (Exception ex) {
                    errMsg = ex.ToString();
                    errStatus = true;
                    AppJournal.Write("Auction", "Get order files error :" + ex.ToString(), true);
                }

                if (!errStatus) {
                    try {
                        var docOrderReq = GetDocumentRequisites("Заявка №" + Auction.number.Replace("/", "_") + ".xlsx", DocumentTypeEnum.Order);

                        archiveManager.GetTemplate(docOrderReq, DocumentTemplateEnum.Order);
                        archiveManager.SaveFile(docOrderReq, (int)Auction.fileslistid);

                        var docTreatyReq = GetDocumentRequisites("Приложение к заявке №" + Auction.number.Replace("/", "_") + ".docx", DocumentTypeEnum.Agreement);

                        archiveManager.GetTemplate(docTreatyReq, DocumentTemplateEnum.OrderAttach);
                        archiveManager.SaveFile(docTreatyReq, (int)Auction.fileslistid);

                        // Copy originals to auction folder
                        string[] orderFileNames = archiveManager.CopyOriginalOrder(docOrderReq, orderFileName, customerEnum == CustomersEnum.Vostok ? treatyFileName : null, string.IsNullOrEmpty(orderOriginalFileName) ? null : orderOriginalFileName, ((schemesFiles == null || schemesFiles.Length < 1) ? null : string.IsNullOrEmpty(schemesFiles[0]) ? null : schemesFiles));

                        Order = OrderBP.GenerateOrder(customerEnum, orderFileNames[0], orderFileNames[1], Order, FolderExplorer.GetAuctionPath(MarketPlaceEnum.ETS, Auction.date.ToShortDateString(), Auction.number), Auction.number);
                        List<LotEF> lotsItems = new List<LotEF>();

                        foreach (var item in Order.Auction.Lots) {
                            try {
                                var regex = new Regex(@"[\d]*[,]*[\d]*");
                                item.MinRequerments = regex.Match(item.MinRequerments).ToString();

                                if (string.IsNullOrEmpty(item.MinRequerments)) item.MinRequerments = "0";
                                else if (item.MinRequerments.Contains(",")) item.MinRequerments = item.MinRequerments.Substring(0, item.MinRequerments.IndexOf(","));
                            } catch (Exception) { item.MinRequerments = "0"; }

                            lotsItems.Add(new LotEF() {
                                auctionid = Auction.id,
                                number = "0G",
                                description = item.Name,
                                localcontent = Convert.ToInt32(item.MinRequerments),
                                amount = 1,
                                price = Convert.ToDecimal(item.StartPrice.Replace(" ", "")),
                                sum = Convert.ToDecimal(item.StartPrice.Replace(" ", "")),
                                warranty = Convert.ToDouble(string.IsNullOrEmpty(Order.Auction.ExchangeProvisionSize) ? "0" : Order.Auction.ExchangeProvisionSize),
                                unitid = 1,
                                paymentterm = "Согласно договору",
                                deliveryplace = "Согласно договору",
                                deliverytime = "Согласно договору",
                                contractnumber = ""
                            });
                        }

                        var lotCount = DataBaseClient.ReadLots(Auction.id);

                        try {
                            if (lotCount == null || lotCount.Count < 1) {
                                int lCount = 0;
                                foreach (var item in lotsItems) {
                                    DataBaseClient.CreateLot(item);

                                    Order.Auction.Lots[lCount].Id = item.id;

                                    DataBaseClient.CreateLotsExtended(Order, item.id, item.number, item.description);

                                    lCount++;
                                }
                            } else {
                                int lCount = 0;

                                foreach (var item in lotCount) {
                                    lotsItems[lCount].id = lotCount[lCount].id;

                                    DataBaseClient.UpdateLot(item);

                                    Order.Auction.Lots[lCount].Id = item.id;

                                    DataBaseClient.UpdateLotsExtended(Order, item.id, item.number, item.description);

                                    lCount++;
                                }
                            }
                        } catch (Exception ex) { AppJournal.Write("Auction", "Create lot extended in db error :" + ex.ToString(), true); }

                        // Update order if it exist
                        if (orderInfo != null) {
                            try {
                                DataBaseClient.UpdateOrderStatus(orderInfo, 4, Auction.id);
                                DataBaseClient.UpdateFileSection((int)orderInfo.fileslistid, DocumentTypeEnum.OrderSource, DocumentSectionEnum.Auction, Auction.number.Replace("/", "_"), Auction.date);
                            } catch (Exception ex) { AppJournal.Write("Auction", "Update order status in db error :" + ex.ToString(), true); }

                            if (customerEnum == CustomersEnum.Vostok) DataBaseClient.UpdateFileSection((int)orderInfo.fileslistid, DocumentTypeEnum.AgreementSource, DocumentSectionEnum.Auction, Auction.number.Replace("/", "_"), Auction.date);

                            var orderPdfFile = DataBaseClient.ReadDocument((int)orderInfo.fileslistid, (int)DocumentTypeEnum.OrderOriginal);

                            if (orderPdfFile != null) DataBaseClient.UpdateFileSection((int)orderInfo.fileslistid, DocumentTypeEnum.OrderOriginal, DocumentSectionEnum.Auction, Auction.number.Replace("/", "_"), Auction.date);

                            var orderSchemes = DataBaseClient.ReadDocuments((int)orderInfo.fileslistid, (int)DocumentTypeEnum.Scheme);

                            if (orderSchemes != null) {
                                foreach (var item in orderSchemes) {
                                    DataBaseClient.UpdateFileSection(item.id, DocumentSectionEnum.Auction, Auction.number.Replace("/", "_"), Auction.date);
                                }
                            }
                        } else { // Handy mode without order
                                 // Create order                        
                            OrderEF orderItem = new OrderEF() {
                                initiatorid = Auction.customerid,
                                auctionid = Auction.id,
                                statusid = 4,
                                number = Auction.number,
                                siteid = Auction.siteid,
                                date = DateTime.Now,
                                customerid = Auction.customerid,
                                fileslistid = DataBaseClient.CreateFileList(new FilesListEF { description = "Файлы заявки №" + Auction.number })
                            };

                            try {
                                DataBaseClient.CreateOrder(orderItem);
                            } catch { }

                            var docOrderSource = GetDocumentRequisites(orderFileNames[0].Substring(orderFileNames[0].LastIndexOf("\\") + 1), DocumentTypeEnum.OrderSource);

                            archiveManager.SaveFile(docOrderSource, (int)orderItem.fileslistid);

                            if (customerEnum == CustomersEnum.Vostok) {
                                var docAgreementSource = GetDocumentRequisites(orderFileNames[1].Substring(orderFileNames[1].LastIndexOf("\\") + 1), DocumentTypeEnum.AgreementSource);

                                archiveManager.SaveFile(docAgreementSource, (int)orderItem.fileslistid);
                            }

                            if (!string.IsNullOrEmpty(orderFileNames[2])) {
                                var docOrderOriginal = GetDocumentRequisites(orderFileNames[2].Substring(orderFileNames[2].LastIndexOf("\\") + 1), DocumentTypeEnum.OrderOriginal);

                                archiveManager.SaveFile(docOrderOriginal, (int)orderItem.fileslistid);
                            }

                        }

                        UpdateLotList();
                        errStatus = false;
                    } catch (Exception ex) {
                        errStatus = true;
                        errMsg = ex.ToString();
                        AppJournal.Write("Auction", "Formate order error :" + ex.ToString(), true);
                    }
                }
                #endregion

                #region KazETS
                // KazETS
            } else if (Auction.siteid == 5) {
                AppJournal.Write("Auction", "Formate KazETS order", true);
                errStatus = false;

                // Get templates & copy templates to auction folder
                // Fill document requisites
                var docOrderReq = GetDocumentRequisites("Заявка №" + Auction.number.Replace("/", "_") + ".xlsx", DocumentTypeEnum.Order);
                var docCoverLetter = GetDocumentRequisites("Сопроводительное письмо №" + Auction.number.Replace("/", "_") + ".xlsx", DocumentTypeEnum.CoverLetter);
                var docContract = GetDocumentRequisites("Договор №" + Auction.number.Replace("/", "_") + ".docx", DocumentTypeEnum.SupplyGoodContract);
                var docSpecification = GetDocumentRequisites("Спецификация №" + Auction.number.Replace("/", "_") + ".docx", DocumentTypeEnum.TechSpecs);

                // Copy templates to auction folder
                string[] orderFiles = new string[4];

                try {
                    orderFiles[0] = archiveManager.GetTemplate(docOrderReq, DocumentTemplateEnum.Order);
                    orderFiles[1] = archiveManager.GetTemplate(docCoverLetter, DocumentTemplateEnum.CoverLetter);
                    orderFiles[2] = archiveManager.GetTemplate(docSpecification, DocumentTemplateEnum.Specification);

                    string resultFileName = "";

                    if (Auction.customerid != 2) {
                        try {
                            resultFileName = Service.GetFile("Выберите договор", "(*.docx; *.doc) | *.docx; *.doc").FullName;
                        } catch { MessagesService.Show("Формирование заявки на КазЭТС", "Ошибка прикрепления договора"); return; }

                        if (!string.IsNullOrEmpty(resultFileName)) {
                            docContract = GetDocumentRequisites("Договор №" + Auction.number.Replace("/", "_") + resultFileName.Substring(resultFileName.LastIndexOf(".")), DocumentTypeEnum.SupplyGoodContract);
                            orderFiles[3] = FolderExplorer.GetAuctionPath(MarketPlaceEnum.KazETS, Auction.date.ToShortDateString(), Auction.number.Replace("\\", "_")) + docContract.fileName;

                            FolderExplorer.CopyFile(resultFileName, orderFiles[3], true);
                        } else return;
                    } else {
                        orderFiles[3] = archiveManager.GetTemplate(docContract, DocumentTemplateEnum.Contract);
                    }
                } catch (Exception ex) { AppJournal.Write("Auction", "Get template err:" + ex.ToString(), true); errStatus = true; }

                // Parse order file for lot and specification if VCM is customer
                if (Auction.customerid != 2 && Auction.customerid != 11 && Auction.customerid != 12 && Auction.customerid != 14 && Auction.customerid != 8 && Auction.customerid != 22) {
                    // Get order files
                    if (orderInfo != null) {
                        orderFileName = GetFileName((int)orderInfo.fileslistid, DocumentTypeEnum.OrderSource);
                        treatyFileName = GetFileName((int)orderInfo.fileslistid, DocumentTypeEnum.AgreementSource);

                        // Copy schemes if exist                            
                        foreach (var item in orderInfo.filesList.documents.Where(f => f.documenttypeid == 25)) {
                            FolderExplorer.CopyFile(orderFileName.Substring(0, orderFileName.LastIndexOf("\\") + 1) + item.name + "." + item.extension, orderFiles[0].Substring(0, orderFiles[0].LastIndexOf("\\") + 1) + item.name + "." + item.extension);
                        }
                    } else {
                        try {
                            orderFileName = Service.GetFile("Выберите заявку", "(*.xlsx;*.xls) | *.xlsx;*.xls").FullName;
                        } catch (Exception ex) { AppJournal.Write("Auction", "Choose order file error :" + ex.ToString(), true); }

                        try {
                            treatyFileName = Service.GetFile("Выберите договор", "(*.docx;*.doc) | *.docx;*.doc").FullName;
                        } catch (Exception ex) { AppJournal.Write("Auction", "Choose treaty file error :" + ex.ToString(), true); }

                        try {
                            orderOriginalFileName = Service.GetFile("Выберите скан заявки", "(*.pdf) | *.pdf").FullName;
                        } catch (Exception ex) { AppJournal.Write("Auction", "Choose order scan file error :" + ex.ToString(), true); }
                    }

                    // Copy files to auction
                    FolderExplorer.CopyFile(treatyFileName, orderFiles[3]);
                    FolderExplorer.CopyFile(orderFileName, FolderExplorer.GetAuctionPath(MarketPlaceEnum.KazETS, Auction.date.ToShortDateString(), Auction.number) + orderFileName.Substring(orderFileName.LastIndexOf("\\") + 1));

                    if (!string.IsNullOrEmpty(orderOriginalFileName)) FolderExplorer.CopyFile(orderOriginalFileName, FolderExplorer.GetAuctionPath(MarketPlaceEnum.KazETS, Auction.date.ToShortDateString(), Auction.number) + orderOriginalFileName.Substring(orderOriginalFileName.LastIndexOf("\\") + 1));


                    // Parse data from file lot and tech spec for lot
                    Lot lot = new Lot();
                    lot = KazETSOrderService.GetLotData(orderFileName);

                    // Create lot in data base
                    LotEF lotDB = new LotEF() {
                        amount = lot.Quantity,
                        auctionid = Auction.id,
                        deliveryplace = lot.DeliveryPlace,
                        deliverytime = lot.DeliveryTime,
                        description = lot.Name,
                        localcontent = Convert.ToInt32(lot.LocalContent),
                        number = lot.Number,
                        paymentterm = lot.PaymentTerm,
                        price = lot.Price,
                        step = Convert.ToInt32(lot.Step),
                        sum = lot.Sum,
                        unitid = 11,
                        dks = lot.Dks,
                        warranty = Convert.ToDouble(lot.Warranty)
                    };

                    var lots = DataBaseClient.ReadLots(Auction.id);

                    // Create lotExtended in data base
                    Order orderForLots = new Order();
                    orderForLots.Auction = new AltaBO.Auction();
                    orderForLots.Auction.Lots = new ObservableCollection<Lot>();
                    orderForLots.Auction.Lots.Add(lot);

                    int lotId = 0;

                    if (lots == null || lots.Count < 1) {
                        lotId = DataBaseClient.CreateLot(lotDB);

                        orderForLots.Auction.Lots[0].Id = lotId;

                        DataBaseClient.CreateLotsExtended(orderForLots, lotId);
                    } else {
                        DataBaseClient.UpdateLot(lotDB);

                        orderForLots.Auction.Lots[0].Id = lots[0].id;

                        DataBaseClient.UpdateLotsExtended(orderForLots, lots[0].id);
                    }

                    // Update lotsList
                    UpdateLotList();
                }

                // Convert EF data to BO                
                try {
                    Order = ConvertEFtoBO(Order, OrderPartEnum.Auction); // Update order with auction info                
                    Order = ConvertEFtoBO(Order, OrderPartEnum.Lot); // Update order with lot info
                } catch (Exception ex) { AppJournal.Write("Auction", "Convert EF to BO err:" + ex.ToString(), true); errStatus = true; }

                // Fill templates
                try {
                    KazETSOrderService.FormateOrder(Order, orderFiles);
                } catch (Exception ex) { AppJournal.Write("Auction", "Formate order err:" + ex.ToString(), true); errStatus = true; }

                // 
                // Check for exist of fileList
                try {
                    if (Auction.fileslistid == null || Auction.fileslistid == 0) Auction.fileslistid = archiveManager.CreateFilesList("Файлы аукциона №" + Auction.number);
                } catch (Exception ex) { AppJournal.Write("Auction", "Create files list err:" + ex.ToString(), true); errStatus = true; }

                // Check files exist
                if (DataBaseClient.ReadDocument((int)Auction.fileslistid, (int)DocumentTypeEnum.Order) == null) {
                    // Save files in base
                    try {
                        archiveManager.SaveFile(docOrderReq, (int)Auction.fileslistid);
                        archiveManager.SaveFile(docCoverLetter, (int)Auction.fileslistid);
                        archiveManager.SaveFile(docContract, (int)Auction.fileslistid);
                        archiveManager.SaveFile(docSpecification, (int)Auction.fileslistid);
                    } catch (Exception ex) { AppJournal.Write("Auction", "Saving files in base err:" + ex.ToString(), true); errStatus = true; }

                    // Save pdf files in base
                    try {
                        docOrderReq.fileName = docOrderReq.fileName.Replace(".xlsx", ".pdf");
                        archiveManager.SaveFile(docOrderReq, (int)Auction.fileslistid);
                        docCoverLetter.fileName = docCoverLetter.fileName.Replace(".docx", ".pdf");
                        archiveManager.SaveFile(docCoverLetter, (int)Auction.fileslistid);
                    } catch (Exception ex) { AppJournal.Write("Auction", "Saving files in base err:" + ex.ToString(), true); errStatus = true; }
                }

                // Change auction status                
                try {
                    Auction.statusid = 4;

                    if (Auction.customerid == 1) {
                        if (orderInfo != null) DataBaseClient.UpdateOrderStatus(orderInfo, 4, Auction.id);
                        else {
                            // Create order
                            try {
                                OrderEF orderItem = new OrderEF() {
                                    initiatorid = Auction.customerid,
                                    auctionid = Auction.id,
                                    statusid = 4,
                                    number = Auction.number,
                                    siteid = Auction.siteid,
                                    date = DateTime.Now,
                                    customerid = Auction.customerid
                                };

                                DataBaseClient.CreateOrder(orderItem);
                            } catch (Exception) { }
                        }
                    }

                    DataBaseClient.UpdateAuction(Auction);
                } catch (Exception ex) { AppJournal.Write("Auction", "Change auction status & save err:" + ex.ToString(), true); errStatus = true; }
                #endregion

                #region Caspy
                // Caspy
            } else if (Auction.siteid == 6) {
                string orderFileName = "";
                string techSpecFileName = "";
                string applicantsFileName = "";

                // Check for order exist
                if (orderInfo != null) {
                    // Get files from db
                    orderFileName = GetFileName((int)orderInfo.fileslistid, DocumentTypeEnum.OrderSource);
                    techSpecFileName = GetFileName((int)orderInfo.fileslistid, DocumentTypeEnum.TechSpecs);
                    applicantsFileName = GetFileName((int)orderInfo.fileslistid, DocumentTypeEnum.ApplicantsFromCustomer);
                } else {
                    // Get files from local storage
                    orderFileName = Service.GetFile("Выберите файл заявки(поручения)", "(*.docx) | *.docx").FullName;
                    techSpecFileName = Service.GetFile("Выберите файл тех. спецификации", "(*.xlsx) | *.xlsx").FullName;
                    applicantsFileName = Service.GetFile("Выберите файл списка участников", "(*.docx) | *.docx").FullName;
                }

                // Check getted files
                if (string.IsNullOrEmpty(orderFileName) || string.IsNullOrEmpty(techSpecFileName)) {
                    MessagesService.Show("Ошибка получения файлов заявки", "Нет файлов заявки");
                    //if (orderInfo != null) DataBaseClient.UpdateOrderStatus(orderInfo, 26);
                    return;
                }

                // Parse order file and tech spec
                var OrderParseResult = ProcuratoryWithTechSpecService.ParseKaspiProcuratory(orderFileName);

                if (OrderParseResult == null) {
                    MessagesService.Show("Ошибка обработки заявки", "Данные в заявке не корректны");
                    //if (orderInfo != null) DataBaseClient.UpdateOrderStatus(orderInfo, 26);
                    return;
                }

                Order CaspyOrder = OrderParseResult;
                CaspyOrder.Auction.Date = Auction.date;
                CaspyOrder.Organizer = Auction.broker.company.name;
                CaspyOrder.Auction.Lots[0].LotsExtended = new ObservableCollection<LotsExtended>(ProcuratoryWithTechSpecService.ParseKaspiTechSpec(techSpecFileName));

                if (CaspyOrder.Auction.Lots[0].LotsExtended == null || CaspyOrder.Auction.Lots[0].LotsExtended.Count < 1) {
                    MessagesService.Show("Ошибка обработки тех. спецификации", "Данные в тех. спецификации не корректны");
                    //if (orderInfo != null) DataBaseClient.UpdateOrderStatus(orderInfo, 26);
                    return;
                }

                // Prepare templates to fill order to exchange
                var docOrderReq = GetDocumentRequisites("Заявка №" + Auction.number.Replace("/", "_") + ".xlsx", DocumentTypeEnum.Order);
                var orderTemplateFileName = archiveManager.GetTemplate(docOrderReq, DocumentTemplateEnum.Order);

                if (string.IsNullOrEmpty(orderTemplateFileName)) {
                    MessagesService.Show("Ошибка создания шаблона заявки", "Не удалось получить шаблон заявки на биржу");
                    return;
                }

                // Check for file exist
                var orderDocument = DataBaseClient.ReadDocument((int)Auction.fileslistid, (int)DocumentTypeEnum.Order);

                if (orderDocument != null) DataBaseClient.DeleteDocument(orderDocument.id);

                archiveManager.SaveFile(docOrderReq, (int)Auction.fileslistid);

                // Fill templates with getted data
                if (!ProcuratoryWithTechSpecService.FillKaspiOrder(orderTemplateFileName, CaspyOrder, "Покупка")) {
                    MessagesService.Show("Ошибка заполнения заявки на биржу", "Произошла ошибка во время формирования заявки на биржу");
                    return;
                }

                // Save data in db
                // Prepare some parts
                CaspyOrder.Auction.Lots[0].auctionId = Auction.id;
                var lotUnit = dataManager.ReadUnits().FirstOrDefault(u => u.Description.ToLower() == CaspyOrder.Auction.Lots[0].Unit.ToLower() || u.Name.ToLower() == CaspyOrder.Auction.Lots[0].Unit.ToLower());
                CaspyOrder.Auction.Lots[0].UnitId = lotUnit == null ? 15 : lotUnit.Id;
                CaspyOrder.Auction.Lots[0].Quantity = CaspyOrder.Auction.Lots[0].Quantity == 0 ? 1 : CaspyOrder.Auction.Lots[0].Quantity;
                CaspyOrder.Auction.Lots[0].Price = CaspyOrder.Auction.Lots[0].Price == 0 ? CaspyOrder.Auction.Lots[0].LotsExtended.Sum(l => l.sum) : CaspyOrder.Auction.Lots[0].Price;
                CaspyOrder.Auction.Lots[0].Sum = CaspyOrder.Auction.Lots[0].Sum == 0 ? CaspyOrder.Auction.Lots[0].LotsExtended.Sum(l => l.sum) : CaspyOrder.Auction.Lots[0].Sum;

                // Check for lot exist
                var lot = dataManager.ReadLots(Auction.id);

                if (lot != null && lot.Count > 0) dataManager.DeleteLot(lot[0].Id);

                // Create lot info
                var lotId = dataManager.CreateLot(CaspyOrder.Auction.Lots[0]);

                if (lotId == null || lotId == 0) {
                    MessagesService.Show("Ошибка сохранения лота в базу", "Произошла ошибка во время сохранения лота в базу");
                    return;
                }

                CaspyOrder.Auction.Lots[0].Id = lotId;

                // Create lot extended info
                DataBaseClient.CreateLotsExtended(CaspyOrder, lotId);

                // Create order if not exist
                if (orderInfo == null) {
                    // Check for order of that auction
                    var orders = dataManager.GetOrders((int)Auction.id);

                    if (orders == null || orders.Count < 1) {
                        var orderId = dataManager.CreateOrder(new Order() { auctionId = (int)Auction.id, filesListId = 0, statusId = 6, Number = Auction.number, siteId = 6, Date = Auction.regulation.opendate, customerid = Auction.customerid });

                        if (orderId == null || orderId == 0) {
                            MessagesService.Show("Ошибка сохранения заявки в базу", "Произошла ошибка во время сохранения заявки в базу");
                            return;
                        }

                        orderInfo = DataBaseClient.ReadOrder(orderId);
                    }

                    orderInfo = DataBaseClient.ReadOrder(orders[0].id);
                }

                // Change order status                                
                DataBaseClient.UpdateOrderStatus(orderInfo, 6, Auction.id);
                UpdateLotList();
            }
            #endregion

            if (!errStatus) {
                OpenFolder();

                if (!createOnce) {
                    LotList.Clear();
                    DefaultParameters(null, true, true);
                }
            } else MessagesService.Show("Ошибка", "Произошла ошибка во время формирования заявки");
        }


        public ICommand FormateSupplierOrderCmd { get { return new DelegateCommand(FormateSupplierOrder); } }
        private void FormateSupplierOrder()
        {
            AppJournal.Write("Auction", "Formate supplier order", true);

            if (SelectedSupplierOrder != null) {
                try {
                    errStatus = false;

                    if (Auction.siteid != 5) FillOrder(1);

                    // KazETS
                    if (Auction.siteid == 5) {
                        // Check procuratory for minimal price
                        if (DataBaseClient.ReadProcuratory((int)SelectedSupplierOrder.supplierid, LotList[0].id) != null) {
                            FormateKazETSSupplierOrder();

                            if (SelectedSupplierOrder.contract.brokerid == 2 && Auction.customerid == 2) // For Karazhira auto creation alternative
                            {
                                SupplierOrderEF supplierOrder = new SupplierOrderEF() {
                                    supplierid = 3,
                                    auctionid = Auction.id,
                                    contractid = 1990,
                                    date = DateTime.Now,
                                    statusid = 5,
                                    fileListId = DataBaseClient.CreateFileList(new FilesListEF() { description = "Заявки поставщика - ТОО Ак Алтын Ко" })
                                };

                                int supplierOrderId = 0;

                                try {
                                    var sOrders = DataBaseClient.ReadSupplierOrders(Auction.id);

                                    if (sOrders != null && sOrders.Where(s => s.supplierid == 3).Count() < 1) supplierOrderId = DataBaseClient.CreateSupplierOrder(supplierOrder);
                                    else if (sOrders != null && sOrders.Where(s => s.supplierid == 3).Count() > 0) supplierOrderId = sOrders.FirstOrDefault(s => s.supplierid == 3).id;
                                } catch (Exception ex) {
                                    AppJournal.Write("Auction", "Create supplier order for alternative err:" + ex.ToString(), true);
                                    errStatus = true;
                                    errMsg = "Ошибка создания заявки на участие для альтернативы.";
                                    supplierOrderId = 0;
                                }

                                // Create procuratory for supplierOrder
                                // Get minimal price between start and end price
                                decimal minPrice = LotList[0].sum - (LotList[0].sum - DataBaseClient.ReadProcuratory((int)SelectedSupplierOrder.supplierid, LotList[0].id).minimalprice) / 2;

                                if (supplierOrderId != 0) {
                                    ProcuratoryEF procuratory = new ProcuratoryEF() {
                                        supplierid = 3,
                                        auctionid = Auction.id,
                                        lotid = LotList[0].id,
                                        minimalprice = minPrice
                                    };

                                    int procuratoryId = 0;

                                    try {
                                        var sProcuratory = DataBaseClient.ReadProcuratories(Auction.id);

                                        if (sProcuratory != null && sProcuratory.Where(s => s.supplierid == 3).Count() < 1) procuratoryId = DataBaseClient.CreateProcuratory(procuratory);
                                        else if (sProcuratory != null && sProcuratory.Where(s => s.supplierid == 3).Count() > 0) procuratoryId = sProcuratory.FirstOrDefault(s => s.supplierid == 3).id;
                                    } catch (Exception ex) {
                                        AppJournal.Write("Auction", "Create procuratory for alternative err:" + ex.ToString(), true);
                                        errStatus = true;
                                        errMsg = "Ошибка создания поручения для альтернативы.";
                                        procuratoryId = 0;
                                    }

                                    if (procuratoryId != 0) {
                                        UpdateSupplierOrdersList();

                                        if (SupplierOrdersList.Count > 1) {
                                            SelectedSupplierOrder = SupplierOrdersList[1];

                                            // Formate alternative
                                            FormateKazETSSupplierOrder(true);
                                        }
                                    }
                                }
                            }
                        } else {
                            errMsg = "Нет поручения для взятия итоговой суммы";
                            errStatus = true;
                        }
                    } else if (Auction.siteid == 4 && !Order.Auction.Lots[0].Number.ToLower().Contains("0g")) { // ETS lot error
                        errStatus = true;
                        errMsg = "Лоту не присвоен код";
                    } else if (Auction.siteid != 4 && Order.Auction.Procuratories[0].MinimalPrice == 0) { // UTB procuratory error
                        errStatus = true;
                        errMsg = "Нулевое значение поручения" + Order.Auction.Procuratories[0].MinimalPrice + " | " + Order.Auction.Lots[0].Sum;
                        // ETS & UTB
                    } else if (Auction.siteid == 4 || Auction.siteid == 1) {
                        var docSupplierOrderReq = GetDocumentRequisites("Заявка на участие от " + SelectedSupplierOrder.supplier.company.name.Replace("\"", "'").Replace("Товарищество с ограниченной ответственностью", "ТОО") + " №" + (Auction.siteid == 4 ? Auction.number.Length > 4 ? Auction.number.Substring(Auction.number.Length - 4) : Auction.number : Auction.number).Replace("/", "_") + ".docx", DocumentTypeEnum.SupplierOrderSource);

                        // Templates
                        string supplierOrderTemplateFile = "";

                        if (Auction.siteid == 4) supplierOrderTemplateFile = archiveManager.GetTemplate(docSupplierOrderReq, DocumentTemplateEnum.SupplierOrder);
                        else supplierOrderTemplateFile = archiveManager.GetTemplate(docSupplierOrderReq, SelectedSupplierOrder.contract.brokerid == 2 ? DocumentTemplateEnum.SupplierOrderKorund : DocumentTemplateEnum.SupplierOrderAkAltyn);

                        if (Auction.siteid == 4) {
                            if (Order.Auction.Lots == null) Order.Auction.Lots = new ObservableCollection<Lot>();

                            Order.Auction.Lots.Clear();

                            if (LotList != null && LotList.Count == 1) Order.Auction.Lots.Add(new Lot() { Number = LotList[0].number });
                            else if (SelectedSupplierOrder.lots != null && SelectedSupplierOrder.lots.Count > 1) {
                                foreach (var item in SelectedSupplierOrder.lots) {
                                    Order.Auction.Lots.Add(new Lot() { Number = item.number });
                                }
                            } else Order.Auction.Lots.Add(new Lot() { Number = "" });
                        }

                        try {
                            SupplierOrderService.CreateSupplierOrder(Order, supplierOrderTemplateFile);
                        } catch (Exception ex) { AppJournal.Write("Auction", "Formate supplier order error :" + ex.ToString(), true); }

                        if (SelectedSupplierOrder.fileListId != null && SelectedSupplierOrder.fileList.documents != null && SelectedSupplierOrder.fileList.documents.Where(f => f.documenttypeid == 7).Count() == 0) archiveManager.SaveFile(docSupplierOrderReq, (int)SelectedSupplierOrder.fileListId);
                        // Caspy
                    } else if (Auction.siteid == 6) {
                        // Check for needed data
                        // Prepare info
                        Order caspyOrder = new Order();
                        caspyOrder.Organizer = SelectedSupplierOrder.contract.broker.name;
                        caspyOrder.Initiator = SelectedSupplierOrder.supplier.company.name;
                        caspyOrder.Auction = dataManager.GetAuction(Auction.id);
                        caspyOrder.Auction.Lots = new ObservableCollection<Lot>(dataManager.ReadLots(Auction.id));
                        caspyOrder.Auction.Lots[0].LotsExtended = new ObservableCollection<LotsExtended>(dataManager.ReadLotExtended(caspyOrder.Auction.Lots[0].Id));

                        // Prepare template
                        var docSupplierOrderReq = GetDocumentRequisites("Заявка на участие №" + Auction.number.Replace("/", "_") + ".xlsx", DocumentTypeEnum.SupplierOrder);
                        var supplierOrderTemplateFileName = archiveManager.GetTemplate(docSupplierOrderReq, DocumentTemplateEnum.Order);

                        if (string.IsNullOrEmpty(supplierOrderTemplateFileName)) {
                            MessagesService.Show("Ошибка создания шаблона заявки на участие", "Не удалось получить шаблон заявки на участие");
                            return;
                        }

                        // Check for file exist
                        var supplierOrderDocument = DataBaseClient.ReadDocument((int)SelectedSupplierOrder.fileListId, (int)DocumentTypeEnum.SupplierOrder);

                        if (supplierOrderDocument != null) DataBaseClient.DeleteDocument(supplierOrderDocument.id);

                        archiveManager.SaveFile(docSupplierOrderReq, (int)Auction.fileslistid);

                        // Fill template with data                        
                        if (!ProcuratoryWithTechSpecService.FillKaspiOrder(supplierOrderTemplateFileName, caspyOrder, "Продажа")) {
                            MessagesService.Show("Ошибка заполнения заявки на биржу", "Произошла ошибка во время формирования заявки на биржу");
                            return;
                        }
                    }
                } catch (Exception ex) {
                    errStatus = true;
                    AppJournal.Write("Auction", "Formate supplier order error :" + ex.ToString(), true);
                }

                if (!errStatus) OpenFolder();
                else MessagesService.Show("Ошибка", "Произошла ошибка во время формирования заявки\n" + errMsg);
            } else MessagesService.Show("Ошибка", "Не выбран участник для формирования");
        }


        private void FormateKazETSSupplierOrder(bool isAlternative = false)
        {
            AppJournal.Write("Auction", "Formate KazETS supplierOrder", true);

            // Get templates & copy templates to auction folder
            // Fill document requisites
            var docSupOrderReq = GetDocumentRequisites((isAlternative == true ? "A1." : "") + "Заявка на участие №" + Auction.number.Replace("/", "_") + " от " + ConvertToShortName(SelectedSupplierOrder.supplier.company.name) + ".xlsx", DocumentTypeEnum.SupplierOrderSource);
            var docCoverLetter = GetDocumentRequisites((isAlternative == true ? "A2." : "") + "Сопроводительное письмо №" + Auction.number.Replace("/", "_") + " от " + ConvertToShortName(SelectedSupplierOrder.supplier.company.name) + ".xlsx", DocumentTypeEnum.CoverLetter);
            var docTechSpec = GetDocumentRequisites((isAlternative == true ? "A3." : "") + "Спецификация №" + Auction.number.Replace("/", "_") + " от " + ConvertToShortName(SelectedSupplierOrder.supplier.company.name) + ".docx", DocumentTypeEnum.TechSpecs);

            // Copy templates to auction folder
            string supOrderFile = "";
            string supCoverFile = "";
            string supTechFile = "";

            try {
                if (SelectedSupplierOrder.contract.brokerid == 2) {
                    supOrderFile = archiveManager.GetTemplate(docSupOrderReq, DocumentTemplateEnum.SupplierOrderKorund);
                    supCoverFile = archiveManager.GetTemplate(docCoverLetter, DocumentTemplateEnum.CoverLetterKorund);
                } else {
                    supOrderFile = archiveManager.GetTemplate(docSupOrderReq, DocumentTemplateEnum.SupplierOrderAkAltyn);
                    supCoverFile = archiveManager.GetTemplate(docCoverLetter, DocumentTemplateEnum.CoverLetterAkAltyn);
                }

                supTechFile = archiveManager.GetTemplate(docTechSpec, DocumentTemplateEnum.Specification);
            } catch (Exception ex) { AppJournal.Write("Auction", "Get template err:" + ex.ToString(), true); errStatus = true; }

            // Convert EF data to BO                
            try {
                Order = ConvertEFtoBO(Order, OrderPartEnum.Auction); // Update order with auction info                
                Order = ConvertEFtoBO(Order, OrderPartEnum.Lot); // Update order with lot info
                Order = ConvertEFtoBO(Order, OrderPartEnum.SupplierOrder, 1); // Update order with supplier info
                Order = ConvertEFtoBO(Order, OrderPartEnum.Procuratory); // Update order with procuratory info
            } catch (Exception ex) { AppJournal.Write("Auction", "Convert EF to BO err:" + ex.ToString(), true); errStatus = true; }

            // Fill templates
            try {
                SupplierOrderService.CreateSupplierOrder(Order, supOrderFile, supCoverFile, supTechFile, false);
            } catch (Exception ex) { AppJournal.Write("Auction", "Formate supplier order err:" + ex.ToString(), true); errStatus = true; }

            // Check files exist
            if ((SelectedSupplierOrder.contract.brokerid == 2 && DataBaseClient.ReadDocument((int)SelectedSupplierOrder.fileListId, (int)DocumentTypeEnum.SupplierOrderSource) == null) || (SelectedSupplierOrder.contract.brokerid == 4 && DataBaseClient.ReadDocuments((int)SelectedSupplierOrder.fileListId, (int)DocumentTypeEnum.SupplierOrderSource).Count < 1)) {
                // Save files in base
                try {
                    archiveManager.SaveFile(docSupOrderReq, (int)SelectedSupplierOrder.fileListId);
                    archiveManager.SaveFile(docCoverLetter, (int)SelectedSupplierOrder.fileListId);
                    archiveManager.SaveFile(docTechSpec, (int)SelectedSupplierOrder.fileListId);
                } catch (Exception ex) { AppJournal.Write("Auction", "Saving files in base err:" + ex.ToString(), true); errStatus = true; }
            }
        }


        public ICommand AttachSupplierOrderCmd { get { return new DelegateCommand(AttachSupplierOrder); } }
        private void AttachSupplierOrder()
        {
            AppJournal.Write("Auction", "Attach supplier order file", true);

            try {
                sourceFileName = Service.GetFile("Выберите заявку на участие от поставщика", "(*.pdf) | *.pdf").FullName;
            } catch (Exception ex) { AppJournal.Write("Auction", "Choose supplier order file error :" + ex.ToString(), true); }

            if (!string.IsNullOrEmpty(sourceFileName)) {
                newFileName = FolderExplorer.GetAuctionPath(Auction.siteid == 4 ? MarketPlaceEnum.ETS : MarketPlaceEnum.UTB, Auction.date.ToShortDateString(), Auction.number);
                newFileName += "Заявка на участие от поставщика " + SelectedSupplierOrder.supplier.company.name.Replace("\"", "'") + ".pdf";

                if (Service.CopyFile(sourceFileName, newFileName, true)) {
                    var docSupplierOrder = GetDocumentRequisites(newFileName.Substring(newFileName.LastIndexOf("\\") + 1), DocumentTypeEnum.SupplierOrder);

                    try {
                        archiveManager.SaveFile(docSupplierOrder, (int)SelectedSupplierOrder.fileListId);
                        //archiveManager.PutFile()

                        SelectedSupplierOrder.statusid = 17;

                        DataBaseClient.UpdateSupplierOrder(SelectedSupplierOrder);
                    } catch (Exception ex) { AppJournal.Write("Auction", "Save supplier order file in db error :" + ex.ToString(), true); }

                    MessagesService.Show("АРХИВ", "Файл скопирован в архив.");
                } else MessagesService.Show("ОШИБКА", "Проверьте доступ к архиву.");
            }
        }


        private void CreateApplicants()
        {
            AppJournal.Write("Auction", "Formate applicants", true);

            try {
                if (SupplierOrdersList.Count > 0) {
                    if (LotList.Count == 1) {
                        #region Applicants for 1 lot
                        FillOrder(2);

                        var docApplicants = GetDocumentRequisites("Список претендентов на аукцион №" + Auction.number.Replace("/", "_") + ".docx", DocumentTypeEnum.Applicants);
                        var docApplicantsForCustomer = Auction.customerid == 1 ? GetDocumentRequisites("Список претендентов для заказчика на аукцион №" + Auction.number.Replace("/", "_") + ".docx", DocumentTypeEnum.ApplicantsForCustomer) : null;

                        string applicantsFileName = archiveManager.GetTemplate(docApplicants, DocumentTemplateEnum.Applicants);
                        string applicantsCustomerFileName = "";

                        if (docApplicantsForCustomer != null) applicantsCustomerFileName = archiveManager.GetTemplate(docApplicantsForCustomer, DocumentTemplateEnum.ApplicantsForCustomer);

                        try {
                            ApplicantsService.CreateApplicants(Order, notAllowedApplicants, docApplicants, applicantsFileName);

                            if (docApplicantsForCustomer != null) ApplicantsService.CreateApplicants(Order, notAllowedApplicants, docApplicantsForCustomer, applicantsCustomerFileName, true);

                            try {
                                DataBaseClient.CreateApplicants(Order, LotList[0].id);

                                if (Auction.fileslistid != null) {
                                    var applicantsFile = DataBaseClient.ReadDocument((int)Auction.fileslistid, (int)DocumentTypeEnum.Applicants);
                                    var applicantsCustomerFile = DataBaseClient.ReadDocument((int)Auction.fileslistid, (int)DocumentTypeEnum.ApplicantsForCustomer);

                                    if (applicantsFile == null) archiveManager.SaveFile(docApplicants, (int)Auction.fileslistid);
                                    if (applicantsCustomerFile == null) archiveManager.SaveFile(docApplicantsForCustomer, (int)Auction.fileslistid);
                                }

                                errStatus = false;
                            } catch (Exception ex) { AppJournal.Write("Auction", "Create applicants in db error :" + ex.ToString(), true); }
                        } catch (Exception ex) { AppJournal.Write("Auction", "Formate applicants error :" + ex.ToString(), true); }
                        #endregion
                    } else {
                        // Applicants for many lots
                        foreach (var item in LotList) {
                            // Convert EF to BO
                            // Auction
                            Order.Auction.Number = Auction.number;
                            Order.Auction.SiteId = 4;
                            Order.Auction.Id = Auction.id;

                            // Lots
                            Order.Auction.Lots.Clear();
                            Order.Auction.Lots.Add(new Lot() { Number = item.number, Id = item.id });

                            // SupplierOrder
                            Order.Auction.SupplierOrders.Clear();

                            var supplierOrdersList = DataBaseClient.ReadSupplierOrders(Auction.id, 16);

                            if (supplierOrdersList != null && supplierOrdersList.Count > 0) {
                                foreach (var subItem in supplierOrdersList) {

                                    if (subItem.lots != null && subItem.lots.Count(s => s.id == item.id) > 0) {
                                        var supplierJournals = DataBaseClient.ReadSuppliersJournals((int)subItem.supplierid);
                                        string supplierCode = "";

                                        if (supplierJournals != null && supplierJournals.Count > 0 && subItem.contract != null) {
                                            var supplierJournal = supplierJournals.FirstOrDefault(s => s.brokerid == subItem.contract.brokerid);

                                            if (supplierJournal != null) supplierCode = supplierJournal.code;
                                        }

                                        var brokerJournal = DataBaseClient.GetBrokerCodeC01(subItem.contract == null ? 1 : (int)subItem.contract.brokerid);
                                        string brokerCode = "";
                                        string brokerName = "";

                                        if (brokerJournal != null && subItem.contract != null) {
                                            brokerCode = brokerJournal.code;
                                            brokerName = subItem.contract.broker.name;
                                        }

                                        Order.Auction.SupplierOrders.Add(new SupplierOrder() {
                                            Code = supplierCode,
                                            Name = subItem.supplier.company.name,
                                            BrokerCode = brokerCode,
                                            BrokerName = brokerName,
                                            BIN = subItem.supplier.company.bin,
                                            BankName = subItem.contract == null ? "" : subItem.contract.bank == null ? "" : subItem.contract.bank.name,
                                            BIK = subItem.contract == null ? "" : subItem.contract.bank == null ? "" : subItem.contract.bank.company.bik,
                                            Kbe = subItem.supplier.company.kbe.ToString(),
                                            IIK = subItem.supplier.company.iik,
                                            Address = subItem.supplier.company.addresslegal,
                                            Id = subItem.id
                                        });
                                    };
                                }
                            };

                            if (Order.Auction.SupplierOrders.Count > 0) {
                                // Get template
                                string fName = "Список претендентов на аукцион №" + Auction.number.Replace("/", "_") + " по лоту №" + item.number;
                                string customerFName = "Список претендентов для заказчика на аукцион №" + Auction.number.Replace("/", "_") + " по лоту №" + item.number;

                                var docApplicants = GetDocumentRequisites(fName + ".docx", DocumentTypeEnum.Applicants);
                                var docApplicantsForCustomer = Auction.customerid == 1 ? GetDocumentRequisites(customerFName + ".docx", DocumentTypeEnum.ApplicantsForCustomer) : null;

                                fName = archiveManager.GetTemplate(docApplicants, DocumentTemplateEnum.Applicants);

                                if (docApplicantsForCustomer != null) customerFName = archiveManager.GetTemplate(docApplicantsForCustomer, DocumentTemplateEnum.ApplicantsForCustomer);

                                // Formate document
                                try {
                                    ApplicantsService.CreateApplicants(Order, new List<SupplierOrder>(), docApplicants, fName);

                                    if (docApplicantsForCustomer != null) ApplicantsService.CreateApplicants(Order, new List<SupplierOrder>(), docApplicantsForCustomer, customerFName, true);

                                    // Save records in db
                                    try {
                                        DataBaseClient.CreateApplicants(Order, item.id);

                                        if (Auction.fileslistid != null) {
                                            var applicantsFile = DataBaseClient.ReadDocument((int)Auction.fileslistid, (int)DocumentTypeEnum.Applicants, fName);
                                            var applicantsCustomerFile = DataBaseClient.ReadDocument((int)Auction.fileslistid, (int)DocumentTypeEnum.ApplicantsForCustomer, customerFName);

                                            if (applicantsFile == null) archiveManager.SaveFile(docApplicants, (int)Auction.fileslistid);
                                            if (applicantsCustomerFile == null) archiveManager.SaveFile(docApplicantsForCustomer, (int)Auction.fileslistid);
                                        }

                                        errStatus = false;
                                    } catch (Exception ex) { AppJournal.Write("Auction", "Create applicants in db error :" + ex.ToString(), true); }
                                } catch (Exception ex) { AppJournal.Write("Auction", "Formate applicants error :" + ex.ToString(), true); }
                            }
                        }
                        //
                    }
                } else {
                    errStatus = true;
                    errMsg = "Количество участников равно 0.";
                }
            } catch (Exception ex) {
                errStatus = true;
                errMsg = ex.ToString();
                AppJournal.Write("Auction", "Formate applicants error :" + ex.ToString(), true);
            }

            // Create invoice GO if not exist
            if (!errStatus && Auction.siteid != 4 && Auction.siteid != 5 && Auction.customerid == 2) {
                if (!FolderExplorer.CheckInvoice(MarketPlaceEnum.UTB, Auction.date.ToShortDateString(), Auction.number, "Счет ГО для*.pdf")) {
                    SelectedSupplierOrder = SupplierOrdersList[0].supplier.company.name.ToUpper().Contains("АК АЛТЫН") ? SupplierOrdersList[1] : SupplierOrdersList[0];
                    CreateInvoice(true, false);
                } else OpenFolder();

            } else if (!errStatus) OpenFolder();
            else MessagesService.Show("Ошибка", "Произошла ошибка во время формирования списка претендентов");
        }


        private void ExcludeFromMailDelivery()
        {
            if (LotList != null && LotList.Count > 0) {
                foreach (var item in LotList) {
                    var mdInfo = dataManager.GetMailDeliveryException(item.description, Auction.regulation.opendate, Auction.customerid == 1 ? "ТОО 'Востокцветмет'" : Auction.customer.company.name);

                    if (mdInfo == null) dataManager.AddMailDeliveryException(item.description, Auction.regulation.opendate, Auction.customerid == 1 ? "ТОО 'Востокцветмет'" : Auction.customer.company.name);
                }
            } else MessagesService.Show("Исключение из рассылки", "Нельзя исключить из-за отсутствия лотов");
        }


        private double GetPercent(decimal sum, int contractId)
        {
            var RatesList = DataBaseClient.ReadRatesList(contractId).FirstOrDefault(r => r.siteid == Auction.siteid);

            if (RatesList != null) {
                var Rates = DataBaseClient.ReadRates(RatesList.id);

                if (Rates != null && Rates.Count > 0) {
                    var rateId = Rates.OrderByDescending(r => r.transaction).FirstOrDefault(r => r.transaction < sum);

                    if (rateId != null) return Convert.ToDouble(Rates.FirstOrDefault(r => r.id == rateId.id).percent);
                    else return Convert.ToDouble(Rates.FirstOrDefault(r => r.id == Rates.OrderBy(ra => ra.transaction).FirstOrDefault().id).percent);
                } else return 0;
            } else return 0;
        }


        public ICommand InvoiceGOCmd { get { return new DelegateCommand(() => CreateInvoice(true)); } }
        public ICommand InvoiceCmd { get { return new DelegateCommand(() => CreateInvoice(false)); } }
        public ICommand InvoicePlayCmd { get { return new DelegateCommand(() => CreateInvoice(false, true, true)); } }

        private ContractEF contract;

        private async void CreateInvoice(bool isWarranty, bool isSingle = true, bool onlyPlay = false)
        {
            AppJournal.Write("Auction", "Create invoice", true);

            string result = "";

            if (isSingle) { // TODO Find best solution for this realisation
                if (SelectedSupplierOrder.contract.brokerid != 3) {
                    var response = FolderExplorer.CheckFiles((MarketPlaceEnum)Auction.siteid, Auction.date.ToShortDateString(), Auction.number,
                        "Счет для компании *" + SelectedSupplierOrder.supplier.company.name.Replace("\"", "'").Replace("Товарищество с ограниченной ответственностью", "ТОО") + "*");

                    if (response != null && response.Length > 0) {
                        bool answer = await MessagesService.AskDialog("Счет существует", "Желаете его переформировать?");

                        if (!answer) return;
                    }

                    FillOrder(1);

                    try {
                        if (Order.Auction.SupplierOrders[0].BrokerName.ToLower().Contains("корунд"))
                            contract = DataBaseClient.ReadContract(3); // Korund
                        else if (Order.Auction.SupplierOrders[0].BrokerName.ToLower().Contains("ак алтын"))
                            contract = DataBaseClient.ReadContract(1990); // Ak Altin
                        else if (Order.Auction.SupplierOrders[0].BrokerName.ToLower().Contains("альта и"))
                            contract = DataBaseClient.ReadContract(2); // Alta i K

                        Order.Auction.SupplierOrders[0].BrokerBankName = contract.bank.name;
                        Order.Auction.SupplierOrders[0].BrokerBIK = contract.bank.company.bik;
                        Order.Auction.SupplierOrders[0].CurrencyCode = SelectedSupplierOrder.contract.currency.code;

                        // UTB (only Karazhira)
                        if ((Auction.siteid == 1 || Auction.siteid == 2) && (Auction.customerid == 2)) {
                            var percent = GetPercent(Order.Auction.Procuratories[0].MinimalPrice, (int)SelectedSupplierOrder.contractid);
                            Order.Auction.InvoicePercent = percent == 0 ? 1 : percent;
                            Order.Auction.SupplierOrders[0].MinimalPrice = Order.Auction.Procuratories[0].MinimalPrice;

                            result = InvoiceService.CreateInvoice(isWarranty, Order);
                            // ETS & UTB (except Karazhira UTB)
                        } else {
                            double percent = 0;
                            bool manyLots = false;

                            if (Auction.siteid == 5 && (Auction.customerid == 2 || Auction.customerid == 11 || Auction.customerid == 12)) {
                                Order.Auction.SupplierOrders[0].MinimalPrice = ProcuratoriesList[0].minimalprice;

                                if (Order.Auction.SupplierOrders[0].MinimalPrice < 10000000) percent = 1;
                                else if (Order.Auction.SupplierOrders[0].MinimalPrice >= 10000000 && Order.Auction.SupplierOrders[0].MinimalPrice < 75000000) percent = 0.6;
                                else percent = 0.4;

                                if (percent != 0) {
                                    Order.Auction.InvoicePercent = percent;
                                    result = InvoiceService.CreateInvoice(isWarranty, Order, "", onlyPlay);
                                } else result = "Msg";

                            } else {
                                Order.Auction.Lots.Clear();

                                if (SelectedSupplierOrder.lots != null && SelectedSupplierOrder.lots.Count > 0) {

                                    foreach (var item in SelectedSupplierOrder.lots) {
                                        manyLots = true;
                                        decimal curSum = 0;

                                        if (isWarranty) curSum = item.sum;
                                        else {
                                            var procuratory = DataBaseClient.ReadProcuratory((int)SelectedSupplierOrder.supplierid, item.id);

                                            if (procuratory != null) curSum = procuratory.minimalprice;
                                            else curSum = 0;
                                        }

                                        Order.Auction.Lots.Add(new Lot() { Sum = curSum, Name = item.description });
                                        Order.Auction.SupplierOrders[0].MinimalPrice = onlyPlay ? 0 : curSum;

                                        percent = GetPercent(curSum, (int)SelectedSupplierOrder.contractid);

                                        if (percent != 0) {
                                            Order.Auction.InvoicePercent = percent;
                                            result = InvoiceService.CreateInvoice(isWarranty, Order, item.number, onlyPlay);
                                        } else result = "Msg";
                                    }
                                } else {
                                    Order.Auction.Lots.Add(new Lot() { Sum = LotList[0].sum, Name = LotList[0].description });
                                    Order.Auction.SupplierOrders[0].MinimalPrice = onlyPlay ? 0 : LotList[0].sum;

                                    percent = GetPercent(LotList[0].sum, (int)SelectedSupplierOrder.contractid);

                                    if (percent != 0) {
                                        Order.Auction.InvoicePercent = percent;
                                        result = InvoiceService.CreateInvoice(isWarranty, Order, LotList[0].number, onlyPlay);
                                    } else result = "Msg";
                                }
                            }
                        }
                    } catch (Exception ex) {
                        result = ex.ToString();
                        AppJournal.Write("Auction", "Create invoice error :" + ex.ToString(), true);
                    }
                } else result = "Err: Для брокера Альтаир-Нур 1С недоступна.";
                // For Karazhira (when formate applicants & reports)
            } else {
                FillOrder(1);

                Order.Auction.InvoicePercent = LotList[0].warranty == 0 ? 1 : LotList[0].warranty;

                try {
                    Order.Auction.SupplierOrders[0].MinimalPrice = DataBaseClient.ReadProcuratories(Auction.id).Where(x => x.supplierid == SelectedSupplierOrder.supplierid).First().minimalprice;

                    if (Order.Auction.SupplierOrders[0].BrokerName.ToLower().Contains("корунд"))
                        contract = DataBaseClient.ReadContract(3); // Korund
                    else if (Order.Auction.SupplierOrders[0].BrokerName.ToLower().Contains("ак алтын"))
                        contract = DataBaseClient.ReadContract(1990); // Ak Altin
                    else if (Order.Auction.SupplierOrders[0].BrokerName.ToLower().Contains("альта и"))
                        contract = DataBaseClient.ReadContract(2); // Alta i K

                    Order.Auction.SupplierOrders[0].BrokerBankName = contract.bank.name;
                    Order.Auction.SupplierOrders[0].BrokerBIK = contract.bank.company.bik;
                    Order.Auction.SupplierOrders[0].CurrencyCode = SelectedSupplierOrder.contract.currency.code;

                    result = InvoiceService.CreateInvoice(isWarranty, Order);
                } catch (Exception ex) { AppJournal.Write("Auction", "Create invoice error :" + ex.ToString(), true); }
            }

            if (result != null && result.Contains("Err")) {
                MessagesService.Show("ОШИБКА", result.Contains("Договора") ? "Договор не передан в 1С" : result);
                AppJournal.Write("Auction", "Create invoice error :" + result, true);
            } else if (result != null && result.Contains("Msg")) MessagesService.Show("Счет не создан", "Нет тарифных данных");
            else OpenFolder();
        }


        private void ProtocolTemplate()
        {
            AppJournal.Write("Auction", "Formate protocol template", true);

            try {
                FillOrder(4);

                if (Order.Auction.Procuratories.Count() < 2) {
                    errStatus = true;
                    errMsg = "Сформировано меньше двух заявок на участие";
                } else {
                    ProtocolsService.CreateProtocols(Order);
                    errStatus = false;
                }

            } catch (Exception ex) {
                errStatus = true;
                errMsg = ex.ToString();
                AppJournal.Write("Auction", "Formate protocol template error :" + ex.ToString(), true);
            }

            if (!errStatus) OpenFolder();
            else MessagesService.Show("Ошибка", "Произошла ошибка во время формирования шаблона протокола");
        }


        public ICommand AttachProtocolCmd { get { return new DelegateCommand(() => AttachProtocol(1)); } }
        public ICommand AttachProtocolScanCmd { get { return new DelegateCommand(() => AttachProtocol(2)); } }

        private void AttachProtocol(int type)
        {
            if (Auction.siteid == 5) {
                AppJournal.Write("Auction", "Attach protocol", true);

                try {
                    sourceFileName = Service.GetFile("Выберите протокол от биржи", "(*.*) | *.*").FullName;
                } catch (Exception) { }

                if (!string.IsNullOrEmpty(sourceFileName)) {
                    newFileName = FolderExplorer.GetAuctionPath(Auction.siteid == 5 ? MarketPlaceEnum.KazETS : MarketPlaceEnum.UTB, Auction.date.ToShortDateString(), Auction.number);
                    newFileName += "Протокол от биржи - " + sourceFileName.Substring(sourceFileName.LastIndexOf("\\") + 1);

                    if (!Service.CopyFile(sourceFileName, newFileName, true)) MessagesService.Show("ОШИБКА", "Проверьте доступ к архиву.");
                    else {
                        var docProtocol = GetDocumentRequisites(sourceFileName.Substring(sourceFileName.LastIndexOf("\\") + 1), type == 1 ? DocumentTypeEnum.ProtocolSource : DocumentTypeEnum.Protocol, DocumentSectionEnum.Auction);
                        archiveManager.SaveFile(docProtocol, (int)Auction.fileslistid);
                    }
                }
            } else MessagesService.Show("Оповещение", "Данный функционал для биржи КазЭТС");
        }


        private async void CreateReports()
        {
            AppJournal.Write("Auction", "Formate reports", true);

            if (Auction.siteid != 4 && SupplierOrdersList.Count > 1) { // UTB & KazETS reports
                string protocolNumber = "";

                protocolNumber = await MessagesService.GetInput("ФОРМИРОВАНИЕ ОТЧЕТОВ", "Введите номер протокола");

                if (Auction.siteid != 5) FillOrder(5);

                if (string.IsNullOrEmpty(protocolNumber)) protocolNumber = "";

                string clientReportFile = "";
                string supplierReportFile = "";
                string supplierName = "";

                var procuratoriesItemsData = DataBaseClient.ReadProcuratories(Auction.id);

                if (procuratoriesItemsData == null) {
                    MessagesService.Show("ФОРМИРОВАНИЕ ОТЧЕТОВ", "Нет поручений");
                    return;
                }

                var supOrders = DataBaseClient.ReadSupplierOrders(Auction.id);
                supOrders = supOrders.Where(s => s.statusid != 16).ToList();
                List<ProcuratoryEF> procuratoriesItems = new List<ProcuratoryEF>();

                foreach (var item in supOrders) {
                    var pI = procuratoriesItemsData.Where(p => p.supplierid == item.supplierid);
                    procuratoriesItems.AddRange(pI);
                }

                var pMin = procuratoriesItems.Min(p => p.minimalprice);
                int pWinProc = procuratoriesItems[0].minimalprice == pMin ? 0 : 1;
                int pWinSup = SupplierOrdersList[0].supplierid == procuratoriesItems[pWinProc].supplierid ? 0 : 1;

                try {
                    supplierName = ConvertToShortName(SupplierOrdersList[pWinSup].supplier.company.name);
                } catch { }

                var docClientReportReq = GetDocumentRequisites("ОЗ №" + Auction.number.Replace("/", "_") + ".docx", DocumentTypeEnum.CustomerReport);
                var docSupplierReportReq = GetDocumentRequisites("ОП №" + Auction.number.Replace("/", "_") + " для " + supplierName + ".docx", DocumentTypeEnum.SupplierReport);

                if (Auction.siteid == 5) {
                    // Get template
                    try {
                        clientReportFile = archiveManager.GetTemplate(docClientReportReq, DocumentTemplateEnum.CustomerReport);
                        supplierReportFile = archiveManager.GetTemplate(docSupplierReportReq, (SupplierOrdersList[pWinSup].contract.brokerid == 2 ? DocumentTemplateEnum.SupplierReportKorund : DocumentTemplateEnum.SupplierReportAkAltyn));
                    } catch (Exception ex) { AppJournal.Write("Auction", "Get template err:" + ex.ToString(), true); errStatus = true; }

                    // Convert EF to BO
                    try {
                        Order = ConvertEFtoBO(Order, OrderPartEnum.Auction); // Update order with auction info                
                        Order = ConvertEFtoBO(Order, OrderPartEnum.Lot); // Update order with lot info
                        Order = ConvertEFtoBO(Order, OrderPartEnum.SupplierOrder, 2); // Update order with supplier info
                        Order = ConvertEFtoBO(Order, OrderPartEnum.Procuratory, 2); // Update order with procuratory info
                    } catch (Exception ex) { AppJournal.Write("Auction", "Convert EF to BO err:" + ex.ToString(), true); errStatus = true; }
                }

                Order.Auction.ProtocolNumber = protocolNumber;
                Order.Auction.Customer = Auction.customer.company.name + " (БИН - " + Auction.customer.company.bin + ")";

                try {
                    sourceFileName = Service.GetFile("Выберите протокол от биржи", "(*.*) | *.*").FullName;
                } catch (Exception) { }

                if (!string.IsNullOrEmpty(sourceFileName)) {
                    newFileName = FolderExplorer.GetAuctionPath(Auction.siteid == 5 ? MarketPlaceEnum.KazETS : MarketPlaceEnum.UTB, Auction.date.ToShortDateString(), Auction.number);
                    newFileName += "Протокол от биржи" + sourceFileName.Substring(sourceFileName.LastIndexOf("."), sourceFileName.Length - sourceFileName.LastIndexOf("."));

                    if (!Service.CopyFile(sourceFileName, newFileName, true)) MessagesService.Show("ОШИБКА", "Проверьте доступ к архиву.");
                    else {
                        DocumentRequisite docProtocolReq = new DocumentRequisite() {
                            fileName = newFileName.Substring(newFileName.LastIndexOf("\\") + 1),
                            date = Auction.date,
                            market = Auction.siteid == 4 ? MarketPlaceEnum.ETS : Auction.siteid == 5 ? MarketPlaceEnum.KazETS : MarketPlaceEnum.UTB,
                            number = Auction.number,
                            section = DocumentSectionEnum.Auction,
                            type = DocumentTypeEnum.Protocol
                        };

                        if (DataBaseClient.ReadDocument((int)Auction.fileslistid, (int)DocumentTypeEnum.Protocol) == null) {
                            try {
                                archiveManager.SaveFile(docProtocolReq, (int)Auction.fileslistid);
                            } catch (Exception ex) { AppJournal.Write("Auction", "Saving protocol file in base err:" + ex.ToString(), true); }
                        }
                    }
                } else MessagesService.Show("Формирование отчетов", "Не забудьте прикрепить скан протокола");

                try {
                    if (Auction.siteid != 5) ReportService.CreateReports(Order); // UTB
                    else if (Auction.siteid == 5) ReportService.CreateReports(Order, clientReportFile, supplierReportFile); // KazETS

                    // Supplier order status to lose
                    if (SupplierOrdersList != null && SupplierOrdersList.Count > 0) {
                        foreach (var item in SupplierOrdersList) {
                            item.statusid = 24;
                            try {
                                DataBaseClient.UpdateSupplierOrder(item);
                            } catch { }
                        }
                    }

                    // FinalReport
                    var supplier = SupplierOrdersList[pWinSup];
                    //var supplier = SupplierOrdersList.FirstOrDefault(s => s.contract.brokerid == 2 && s.statusid != 16);

                    if (LotList != null && supplier != null) {
                        // Supplier order status to winner
                        supplier.statusid = 23;

                        try {
                            DataBaseClient.UpdateSupplierOrder(supplier);
                        } catch { }

                        // Continium with final report
                        var finalReport = DataBaseClient.ReadFinalReport(LotList[0].auctionid, LotList[0].id);
                        var lastPrice = DataBaseClient.ReadProcuratory((int)supplier.supplierid, LotList[0].id);

                        if (finalReport != null) DataBaseClient.DeleteFinalReport(finalReport);

                        FinalReportEF finalReportItem = new FinalReportEF() {
                            auctionId = LotList[0].auctionid,
                            dealNumber = protocolNumber,
                            supplierId = (int)supplier.supplierid,
                            lotId = LotList[0].id,
                            finalPriceOffer = lastPrice != null ? lastPrice.minimalprice : 0,
                            brokerId = 2
                        };

                        DataBaseClient.CreateFinalReport(finalReportItem);
                    }
                    //

                    Auction.statusid = 2;

                    DataBaseClient.UpdateAuction(Auction);
                    UpdateView(Auction.id);

                    errStatus = false;

                    if (Auction.siteid == 5) {
                        // Check files exist
                        if (Auction.fileslistid == null) {
                            FilesListEF fileList = new FilesListEF() { description = "Файлы аукциона " + Auction.number };
                            Auction.fileslistid = DataBaseClient.CreateFileList(fileList);
                            DataBaseClient.UpdateAuction(Auction);
                        }

                        if (DataBaseClient.ReadDocument((int)Auction.fileslistid, (int)DocumentTypeEnum.CustomerReport) == null) {
                            // Save files in base
                            try {
                                archiveManager.SaveFile(docClientReportReq, (int)Auction.fileslistid);
                                archiveManager.SaveFile(docSupplierReportReq, (int)Auction.fileslistid);
                            } catch (Exception ex) { AppJournal.Write("Auction", "Saving report files in base err:" + ex.ToString(), true); }
                        }
                    }
                } catch (Exception ex) {
                    errStatus = true;
                    errMsg = ex.ToString();
                    AppJournal.Write("Auction", "Formate reports error :" + ex.ToString(), true);
                }

                if (!errStatus && Auction.customerid == 2 && Auction.siteid != 5) {
                    if (!FolderExplorer.CheckInvoice(MarketPlaceEnum.UTB, Auction.date.ToShortDateString(), Auction.number, "Счет для*.pdf")) {
                        SelectedSupplierOrder = SupplierOrdersList[0].supplier.company.name.ToUpper().Contains("АК АЛТЫН") ? SupplierOrdersList[1] : SupplierOrdersList[0];
                        CreateInvoice(false, false);
                    } else OpenFolder();
                } else if (!errStatus) OpenFolder();
                else MessagesService.Show("Ошибка", "Произошла ошибка во время формирования отчетов");
            } else MessagesService.Show("Отчеты", "Кол-во поставщиков меньше двух. (Не для ЕТС биржи)");

        }


        public void OpenFolder()
        {
            AppJournal.Write("Auction", "Open folder for check", true);
            FolderExplorer.OpenAuctionFolder((MarketPlaceEnum)Auction.siteid, Auction.date.ToShortDateString(), Auction.number);
        }


        private void UpdateView(int id)
        {
            AppJournal.Write("Auction", "Update auction view", true);

            Auction = DataBaseClient.ReadAuction(id);
            FormTitle = "Просмотр/редактирование аукциона";
            DefaultParameters(Auction, true);
        }


        private void Cancel() { Workspace.This.Panels.Remove(Workspace.This.ActiveDocument); }


        public ICommand CreateLotCmd { get { return new DelegateCommand(() => LotFormShow(1)); } }
        public ICommand UpdateLotCmd { get { return new DelegateCommand(() => LotFormShow(2)); } }

        private void LotFormShow(int crudMode)
        {
            AppJournal.Write("Auction", "Open lot - " + (crudMode == 1 ? "New" : SelectedLot.number), true);

            if (Auction.id != 0) {
                if (crudMode == 1) SelectedLot = null;

                var lotViewModel = new LotViewModel(Auction.id, this, SelectedLot) { Description = "Лот №" + (crudMode == 2 ? SelectedLot.number : "1") };

                var lotView = new LotView();
                lotViewModel.View = lotView;

                Workspace.This.Panels.Add(lotViewModel);
                Workspace.This.ActiveDocument = lotViewModel;
            }
        }


        public ICommand UpdateLotViewCmd { get { return new DelegateCommand(UpdateLotList); } }

        public void UpdateLotList()
        {
            AppJournal.Write("Auction", "Update lot view", true);

            try {
                LotList = new ObservableCollection<LotEF>(DataBaseClient.ReadLots(Auction.id));
            } catch (Exception ex) { AppJournal.Write("Auction", "Get lots from db error :" + ex.ToString(), true); }
        }


        public ICommand DeleteLotCmd { get { return new DelegateCommand(DeleteLot); } }

        private void DeleteLot()
        {
            if (SelectedLot != null) {
                try {
                    DataBaseClient.DeleteLot(SelectedLot.id);
                } catch (Exception ex) { AppJournal.Write("Auction", "Delete lot from db error :" + ex.ToString(), true); }

                UpdateLotList();
            }
        }


        public ICommand FormateTechSpecCmd { get { return new DelegateCommand(() => FormateTechSpec(1)); } }
        public ICommand FormateTechSpecWithSPCmd => new DelegateCommand(() => FormateTechSpec(2));
        private void FormateTechSpec(int type)
        {
            AppJournal.Write("Auction", "Formate tech spec", true);

            if (type == 1 && SelectedLot == null) {
                MessagesService.Show("Формирование тех. спец", "Не выбран лот");
                return;
            } else if (type == 2 && SelectedProcuratory == null) {
                MessagesService.Show("Формирование тех. спец", "Не выбрано поручение");
                return;
            }

            // Check for selected lot
            //if (SelectedLot != null && ((Auction.siteid == 4 && Auction.customerid == 1) || (Auction.siteid == 5 && Auction.customerid != 2))) {
            // Get & check lotExtended
            var lotExtended = DataBaseClient.ReadLotsExtended(type == 1 ? SelectedLot.id : (int)SelectedProcuratory.lotid);

            if (lotExtended != null && lotExtended.Count > 0) {
                // Get templates & files path
                var docTechSpec = GetDocumentRequisites("Тех спецификация по лоту " + (type == 1 ? SelectedLot.number : SelectedProcuratory.lot.number) + ".xlsx", DocumentTypeEnum.TechSpecs, DocumentSectionEnum.Auction);
                var docTechSpecTemplateFile = archiveManager.GetTemplate(docTechSpec, DocumentTemplateEnum.TechSpec);

                // Convert EF to BO
                List<LotsExtended> lotsExtendedList = new List<LotsExtended>();

                foreach (var item in lotExtended) {
                    try {
                        lotsExtendedList.Add(new LotsExtended() {
                            serialnumber = item.serialnumber,
                            name = item.name == null ? "" : item.name,
                            unit = item.unit == null ? "" : item.unit,
                            quantity = item.quantity,
                            price = item.price,
                            sum = item.sum,
                            country = item.country == null ? "" : item.country,
                            techspec = item.techspec == null ? "" : item.techspec,
                            terms = item.terms == null ? "" : item.terms,
                            paymentterm = item.paymentterm == null ? "" : item.paymentterm,
                            dks = item.dks == null ? 0 : (int)item.dks,
                            contractnumber = item.contractnumber == null ? "" : item.contractnumber,
                            endprice = item.endprice == null ? 0 : (decimal)item.endprice,
                            endsum = item.endsum == null ? 0 : (decimal)item.endsum
                        });
                    } catch (Exception ex) { AppJournal.Write("Auction", "Fill lotEx err:" + ex.ToString(), true); }
                }

                // Create document
                if (TechSpecService.CreateDocument(docTechSpecTemplateFile, lotsExtendedList, (type == 1 ? SelectedLot.number : SelectedProcuratory.lot.number), type == 1 ? 0 : SelectedProcuratory.minimalprice, type == 1 ? 0 : SelectedProcuratory.lot.sum, true) && lotsExtendedList.Count > 0) {
                    // Check for exist record in base about file
                    if ((type == 1 ? SelectedLot.filelistid : SelectedProcuratory.lot.filelistid) == null) {
                        if (type == 1) SelectedLot.filelistid = DataBaseClient.CreateFileList(new FilesListEF() { description = "Файлы лота" });
                        else SelectedProcuratory.lot.filelistid = DataBaseClient.CreateFileList(new FilesListEF() { description = "Файлы лота" });

                        DataBaseClient.UpdateLot(type == 1 ? SelectedLot : SelectedProcuratory.lot);
                    }

                    var techSpecFiles = DataBaseClient.ReadDocuments((type == 1 ? (int)SelectedLot.filelistid : (int)SelectedProcuratory.lot.filelistid), (int)DocumentTypeEnum.TechSpecs);

                    if (techSpecFiles != null && techSpecFiles.Where(t => t.name.ToLower().Contains((type == 1 ? SelectedLot.number : SelectedProcuratory.lot.number).ToLower())).Count() < 1) {
                        // Save files in base
                        try {
                            archiveManager.SaveFile(docTechSpec, (type == 1 ? (int)SelectedLot.filelistid : (int)SelectedProcuratory.lot.filelistid));
                        } catch (Exception ex) { AppJournal.Write("Auction", "Saving tech spec file in base err:" + ex.ToString(), true); }
                    }

                    OpenFolder();
                } else MessagesService.Show("Формирование тех. спецификации", "Ошибка формирования документа.");
            } else MessagesService.Show("Формирование тех. спецификации", "Для этого лота нет тех. спецификации.");
            //} else MessagesService.Show("Формирование тех. спецификации", "Не выбран лот или биржа не ЕТС.");
        }


        public ICommand ParseTechSpecCmd { get { return new DelegateCommand(ParseTechSpec); } }

        private void ParseTechSpec()
        {
            AppJournal.Write("Auction", "Parse tech spec", true);

            if (SelectedLot != null && (Auction.siteid == 4 || (Auction.siteid == 5 && Auction.customerid != 2))) {
                // Get file with tech spec from supplier
                string tsFileName = "";

                try {
                    tsFileName = Service.GetFile("Выберите тех. спецификацию от поставщика", "(*.xlsx) | *.xlsx").FullName;
                } catch (Exception ex) { AppJournal.Write("Auction", "Choose supplier tech spec file error :" + ex.ToString(), true); }

                if (!string.IsNullOrEmpty(tsFileName)) {
                    // Get lotEx data from db
                    var lotEx = DataBaseClient.ReadLotsExtended(SelectedLot.id);

                    if (lotEx != null && lotEx.Count > 0) {
                        // Make LotExtended BO
                        List<LotsExtended> lotsExtendedList = new List<LotsExtended>();

                        foreach (var item in lotEx) {
                            try {
                                lotsExtendedList.Add(new LotsExtended() {
                                    serialnumber = item.serialnumber,
                                    name = item.name == null ? "" : item.name,
                                    unit = item.unit == null ? "" : item.unit,
                                    quantity = item.quantity,
                                    price = item.price,
                                    sum = item.sum,
                                    country = item.country == null ? "" : item.country,
                                    techspec = item.techspec == null ? "" : item.techspec,
                                    terms = item.terms == null ? "" : item.terms,
                                    paymentterm = item.paymentterm == null ? "" : item.paymentterm,
                                    dks = item.dks == null ? 0 : (int)item.dks,
                                    contractnumber = item.contractnumber == null ? "" : item.contractnumber,
                                    endprice = item.endprice == null ? 0 : (decimal)item.endprice,
                                    endsum = item.endsum == null ? 0 : (decimal)item.endsum
                                });
                            } catch (Exception ex) { AppJournal.Write("Auction", "Fill lotEx err:" + ex.ToString(), true); }
                        }

                        // Parse file
                        List<LotsExtended> lotExResult = new List<LotsExtended>();

                        if (TechSpecService.ParseDocument(tsFileName, lotsExtendedList, out lotExResult)) {
                            // Get final report data
                            var finalReport = DataBaseClient.ReadFinalReport(Auction.id, SelectedLot.id);

                            if (finalReport != null) {
                                if (lotExResult.Sum(l => l.endsum) == finalReport.finalPriceOffer) {
                                    int rCount = 0;

                                    foreach (var item in lotEx) {
                                        item.endprice = lotExResult[rCount].endprice;
                                        item.endsum = lotExResult[rCount].endsum;

                                        rCount++;
                                    }

                                    try {
                                        DataBaseClient.UpdateLotsExtended(lotEx);
                                    } catch (Exception ex) { AppJournal.Write("Auction", "Update lotEx in db after parse tech spec err: " + ex.ToString(), true); }
                                    MessagesService.Show("Обработка тех. спецификации", "Данные успешно внесены.");
                                } else MessagesService.Show("Обработка тех. спецификации", "Указанные суммы тех. спецификации не совпадают с выигрышной.");
                            } else MessagesService.Show("Обработка тех. спецификации", "Нет выигрышной суммы.");
                        } else MessagesService.Show("Обработка тех. спецификации", "Нет первоначальные данные не соответствуют предоставленным.");
                    } else MessagesService.Show("Обработка тех. спецификации", "Нет первоначальных данных по тех. спецификации лота.");
                }
            } else MessagesService.Show("Обработка тех. спецификации", "Не выбран лот или биржа не ЕТС.");
        }



        public ICommand GetTechSpecCmd => new DelegateCommand(GetTechSpec);
        private void GetTechSpec()
        {
            if (SelectedLot == null) {
                MessagesService.Show("Изъятие технической спецификации", "Не выбран лот");
                return;
            }

            string fileName = "";

            try {
                fileName = Service.GetFile("Выберите заявку с тех. спецификацией", "(*.docx; *.doc) | *.docx; *.doc").FullName;
            } catch (Exception ex) { AppJournal.Write("Auction", "Choose order with tech spec file error :" + ex.ToString(), true); return; }

            if (!string.IsNullOrEmpty(fileName)) {
                // Parse file
                List<LotsExtended> lotsExtended = new List<LotsExtended>();

                try {
                    lotsExtended = TechSpecService.ParseOrderWithTS(fileName, SelectedLot.id);
                } catch (Exception ex) { MessagesService.Show("Парсинг заявки", "Произошла ошибка во время изъятия тех. спец из заявки"); return; }

                // Check exist lotEx in base
                if (lotsExtended == null) {
                    MessagesService.Show("Парсинг заявки", "Данные по тех. спецификациям не найдены");
                    return;
                }

                var lotEx = DataBaseClient.ReadLotsExtended(SelectedLot.id);

                Order tOrder = new Order();
                Auction tAuction = new Auction();
                tOrder.Auction = tAuction;
                tOrder.Auction.Lots.Add(new Lot() { Id = SelectedLot.id, LotsExtended = new ObservableCollection<LotsExtended>(lotsExtended) });

                // Put in base
                try {
                    if (lotEx == null || lotEx.Count < 1) DataBaseClient.CreateLotsExtended(tOrder, SelectedLot.id);
                    else DataBaseClient.UpdateLotsExtended(tOrder, SelectedLot.id);
                } catch { MessagesService.Show("Парсинг заявки", "Произошла ошибка во время занесения в базу"); return; }

                // Show message about success
                MessagesService.Show("Парсинг заявки", "Техническая спецификация по выбранному лоту занесена в систему");
            }
        }


        public ICommand CreateSupplierOrderCmd { get { return new DelegateCommand(() => SupplierOrderFormShow(1)); } }
        public ICommand UpdateSupplierOrderCmd { get { return new DelegateCommand(() => SupplierOrderFormShow(2)); } }

        private void SupplierOrderFormShow(int crudMode)
        {
            AppJournal.Write("Auction", "Open supplier order - " + (crudMode == 1 ? "New" : SelectedSupplierOrder.supplier.company.name), true);

            if (Auction.id != 0) {
                SupplierOrderViewModel supplierOrderViewModel;

                if (crudMode == 1) supplierOrderViewModel = new SupplierOrderViewModel(Auction.id, this) { Description = "Заявка на участие к " + Auction.number };
                else supplierOrderViewModel = new SupplierOrderViewModel(Auction.id, this, SelectedSupplierOrder) { Description = "Заявка на участие к " + Auction.number };

                var supplierOrderView = new SupplierOrderView();
                supplierOrderViewModel.View = supplierOrderView;

                Workspace.This.Panels.Add(supplierOrderViewModel);
                Workspace.This.ActiveDocument = supplierOrderViewModel;
            }
        }


        public ICommand UpdateSupplierOrderViewCmd { get { return new DelegateCommand(UpdateSupplierOrdersList); } }

        public void UpdateSupplierOrdersList()
        {
            AppJournal.Write("Auction", "Update supplier orders list", true);

            try {
                SupplierOrdersList = new ObservableCollection<SupplierOrderEF>(dataManager.GetSupplierOrders(Auction.id, 16));
            } catch (Exception ex) { AppJournal.Write("Auction", "Get supplier orders from db error :" + ex.ToString(), true); }
        }


        public ICommand DeniedSupplierOrderCmd { get { return new DelegateCommand(() => ChangeSupplierOrderStatus(16)); } }
        public ICommand AcceptSupplierOrderCmd { get { return new DelegateCommand(() => ChangeSupplierOrderStatus(15)); } }

        private async void ChangeSupplierOrderStatus(int status)
        {
            AppJournal.Write("Auction", "Change supplier order status", true);

            if (SelectedSupplierOrder != null) {
                try {
                    var comment = status == 16 ? await MessagesService.GetInput("Причина не допуска", "Впишите причину не допуска участника") : "";

                    if (comment != null && status == 16) SelectedSupplierOrder.comments = comment;

                    SelectedSupplierOrder.statusid = status;
                    dataManager.UpdateStatusOnSupplierOrder(SelectedSupplierOrder.id, status);

                    if (status == 16) {
                        if (ProcuratoriesList != null && ProcuratoriesList.Count > 0) {
                            foreach (var item in ProcuratoriesList) {
                                DataBaseClient.DeleteProcuratory(item.id);
                            }
                        }
                    }
                } catch (Exception ex) { AppJournal.Write("Auction", "Change supplier order status in db error :" + ex.ToString(), true); }

                try {
                    UpdateSupplierOrdersList();
                    UpdateProcuratoriesList();
                } catch (Exception) { }
            } else MessagesService.Show("Изменение статуса заявки на участие", "Заявка не выбрана");
        }


        public ICommand CreateProcuratoryCmd { get { return new DelegateCommand(() => ProcuratoryFormShow(1)); } }
        public ICommand UpdateProcuratoryCmd { get { return new DelegateCommand(() => ProcuratoryFormShow(2)); } }

        private async void ProcuratoryFormShow(int crudMode)
        {
            AppJournal.Write("Auction", "Open procuratory view -" + (crudMode == 1 ? "New" : SelectedProcuratory.supplier.company.name), true);

            if (Auction.id != 0) {
                if (SelectedSupplierOrder != null && SelectedSupplierOrder.id > 0) {
                    if (crudMode == 1 && SelectedLot != null && SelectedLot.id > 0) {
                        int procuratoryCount = DataBaseClient.GetProcuratoriesCount((int)SelectedSupplierOrder.supplierid, SelectedLot.id);

                        if (procuratoryCount == 0) {
                            var supplierProcuratories = DataBaseClient.ReadProcuratories(SelectedSupplierOrder.id, Auction.id);
                            int fId = 0;

                            if (supplierProcuratories != null && supplierProcuratories.Count > 0) {
                                foreach (var item in supplierProcuratories) {
                                    if (item.filelistid != null) {
                                        fId = (int)item.filelistid;
                                        break;
                                    }
                                }

                                if (fId == 0) fId = DataBaseClient.CreateFileList(new FilesListEF() { description = "Поручение от " + SelectedSupplierOrder.supplier.company.name });
                            }

                            string minimal = await MessagesService.GetInput("Создание поручения", "Введите минимальную сумму");

                            if (!string.IsNullOrEmpty(minimal)) {
                                ProcuratoryEF procuratoryItem = new ProcuratoryEF() {
                                    supplierid = SelectedSupplierOrder.supplierid,
                                    auctionid = Auction.id,
                                    lotid = SelectedLot.id,
                                    minimalprice = Convert.ToDecimal(minimal.Replace(".", ",")),
                                    filelistid = fId
                                };

                                try {
                                    DataBaseClient.CreateProcuratory(procuratoryItem);
                                    UpdateProcuratoriesList();
                                } catch (Exception ex) {
                                    MessagesService.Show("Создание поручения", "Произошла ошибка при создании");
                                    AppJournal.Write("Auction", "Create procuratory in db error :" + ex.ToString(), true);
                                }
                            }
                        } else MessagesService.Show("Создание поручения", "Создание не возможно, так как поручение существует");
                    } else if (crudMode == 2) {
                        if (SelectedProcuratory != null) {
                            string minimal = await MessagesService.GetInput("Обновление поручения", "Введите минимальную сумму");

                            try {
                                if (Convert.ToDecimal(minimal.Replace(".", ",")) > 0) {
                                    DataBaseClient.UpdateProcuratory(SelectedProcuratory, Convert.ToDecimal(minimal.Replace(".", ",")));
                                    UpdateProcuratoriesList();
                                }
                            } catch (Exception ex) {
                                MessagesService.Show("Обновление поручения", "Произошла ошибка при обновлении");
                                AppJournal.Write("Auction", "Update procuratory in db error :" + ex.ToString(), true);
                            }
                        } else MessagesService.Show("Обновление поручения", "Поручение не выбрано");
                    } else MessagesService.Show("Создание поручения", "Необходимо выбрать лот");
                } else MessagesService.Show("Работа с поручением", "Необходимо выбрать заявку на участие");
            } else MessagesService.Show("Работа с поручением", "Необходимо сохранить аукцион");
        }


        public ICommand UpdateProcuratoryViewCmd { get { return new DelegateCommand(UpdateProcuratoriesList); } }

        public void UpdateProcuratoriesList()
        {
            AppJournal.Write("Auction", "Update procuratories view", true);

            try {
                ProcuratoriesList = new ObservableCollection<ProcuratoryEF>(DataBaseClient.ReadProcuratories(SelectedSupplierOrder == null ? 0 : (int)SelectedSupplierOrder.supplierid, Auction.id));
            } catch (Exception ex) { AppJournal.Write("Auction", "Get procuratories from db error :" + ex.ToString(), true); }
        }


        public ICommand FormateProcuratoryCmd { get { return new DelegateCommand(FormateProcuratory); } }

        private void FormateProcuratory()
        {
            AppJournal.Write("Auction", "Formate procuratory", true);

            if (SelectedProcuratory != null && SelectedProcuratory.id > 0 && Auction.siteid == 4) {
                try {
                    //#if DEBUG
                    // New version of formating
                    Order orderData = new Order();

                    // Fill auction info in order
                    orderData.Date = Auction.regulation.opendate;
                    orderData.Auction = new Auction();
                    orderData.Auction.Date = Auction.date;
                    orderData.Auction.Number = Auction.number;
                    orderData.customerid = Auction.customerid;

                    // Fill supplier order info in order
                    SupplierOrder supplierOrderData = new SupplierOrder();
                    supplierOrderData.BrokerName = SelectedSupplierOrder.contract.broker.name;
                    supplierOrderData.BrokerCode = SelectedSupplierOrder.contract.broker.brokersJournal.code;
                    supplierOrderData.Name = SelectedSupplierOrder.supplier.company.name;

                    var supCode = SelectedSupplierOrder.supplier.supplierJournals.FirstOrDefault(s => s.brokerid == SelectedSupplierOrder.contract.brokerid);

                    if (supCode != null) supplierOrderData.Code = SelectedSupplierOrder.supplier.supplierJournals.FirstOrDefault(s => s.brokerid == SelectedSupplierOrder.contract.brokerid).code;
                    else supplierOrderData.Code = "";

                    supplierOrderData.Trader = SelectedSupplierOrder.contract.broker.company.director;
                    supplierOrderData.lots = new List<Lot>();

                    // Fill lots info in order
                    foreach (var item in SelectedSupplierOrder.lots) {
                        Lot lotItem = new Lot();

                        lotItem.Id = lotItem.Id;
                        lotItem.Number = item.number;
                        lotItem.Sum = item.sum;

                        //if (Auction.customerid == 1) {
                        var lEx = DataBaseClient.ReadLotsExtended(item.id);

                        if (lEx != null && lEx.Count > 0) {
                            lotItem.LotsExtended = new ObservableCollection<LotsExtended>();

                            foreach (var subItem in lEx) {
                                LotsExtended lotExt = new LotsExtended();

                                lotExt.serialnumber = subItem.serialnumber;
                                lotExt.name = subItem.name;
                                lotExt.unit = subItem.unit;
                                lotExt.quantity = subItem.quantity;
                                lotExt.price = subItem.price;
                                lotExt.sum = subItem.sum;
                                lotExt.country = subItem.country;
                                lotExt.techspec = subItem.techspec;
                                lotExt.terms = subItem.terms;
                                lotExt.paymentterm = subItem.paymentterm;
                                lotExt.dks = (int)subItem.dks;
                                lotExt.contractnumber = subItem.contractnumber;
                                lotExt.endprice = subItem.endprice != null ? (decimal)subItem.endprice : 0;
                                lotExt.endsum = subItem.endsum != null ? (decimal)subItem.endsum : 0;

                                lotItem.LotsExtended.Add(lotExt);
                            }
                        }
                        //}

                        supplierOrderData.lots.Add(lotItem);
                    }

                    orderData.Auction.SupplierOrders = new ObservableCollection<SupplierOrder>();
                    orderData.Auction.SupplierOrders.Add(supplierOrderData);

                    // Get template files
                    var docProcuratory = GetDocumentRequisites("Поручение на сделку от " + SelectedSupplierOrder.supplier.company.name.Replace("\"", "'").Replace("Товарищество с ограниченной ответственностью", "ТОО") + " №" + (Auction.siteid == 4 ? Auction.number.Length > 4 ? Auction.number.Substring(Auction.number.Length - 4) : Auction.number : Auction.number).Replace("/", "_") + ".xlsx", DocumentTypeEnum.ProcuratorySource);

                    string procuratoryFileName = archiveManager.GetTemplate(docProcuratory, DocumentTemplateEnum.Procuratory);

                    ProcuratoriesService.FormateProcuratory(procuratoryFileName, orderData);

                    // Save files in db
                    int fId = 0;

                    if (SelectedProcuratory.filelistid == null) fId = DataBaseClient.CreateFileList(new FilesListEF() { description = "Поручения от " + SelectedSupplierOrder.supplier.company.name });
                    else fId = (int)SelectedProcuratory.filelistid;

                    var pFile = DataBaseClient.ReadDocument(fId, (int)DocumentTypeEnum.ProcuratorySource);

                    if (pFile != null) DataBaseClient.DeleteDocument(pFile.id);

                    try {
                        archiveManager.SaveFile(docProcuratory, fId);
                    } catch (Exception ex) { AppJournal.Write("Auction", "Save procuratory file in db error :" + ex.ToString(), true); }

                    OpenFolder();
                    return;
                    //#endif

                    #region OldVersion to Word
                    // Old version
                    //FillOrder(1, (int)SelectedProcuratory.lotid);

                    //                    int[] tableNumbers = new int[ProcuratoriesList.Count];

                    //                    if (LotList.Count > 1)
                    //                    {
                    //                        int lCount = 1, pCount = 1;

                    //                        foreach (var item in LotList)
                    //                        {
                    //                            foreach (var subItem in ProcuratoriesList)
                    //                            {
                    //                                if (subItem.lotid == item.id)
                    //                                {
                    //                                    tableNumbers[pCount - 1] = lCount + 1;
                    //                                    pCount++;
                    //                                }
                    //                            }

                    //                            lCount++;
                    //                        }
                    //                    }
                    //                    else if (Auction.customerid == 14092) tableNumbers = null;
                    //                    else tableNumbers[0] = 2;

                    //                    if (ProcuratoriesList.Count > 1)
                    //                    {
                    //                        Order.Auction.Lots.Clear();

                    //                        foreach (var item in ProcuratoriesList)
                    //                        {
                    //                            try
                    //                            {
                    //                                Order.Auction.Lots.Add(new Lot()
                    //                                {
                    //                                    Id = item.lot.id,
                    //                                    Number = item.lot.number,
                    //                                    Name = item.lot.description,
                    //                                    Unit = item.lot.unit.name,
                    //                                    Quantity = item.lot.amount,
                    //                                    Price = item.lot.price,
                    //                                    Sum = item.lot.price * item.lot.amount,
                    //                                    PaymentTerm = item.lot.paymentterm,
                    //                                    DeliveryPlace = item.lot.deliveryplace,
                    //                                    DeliveryTime = item.lot.deliverytime,
                    //                                    Step = (decimal)item.lot.step,
                    //                                    Warranty = (decimal)item.lot.warranty,
                    //                                    LocalContent = item.lot.localcontent
                    //                                });
                    //                            }
                    //                            catch (Exception) { }
                    //                        }
                    //                    }

                    //#if !DEBUG
                    //                    // Get template files
                    //                    var docProcuratory = GetDocumentRequisites("Поручение на сделку от " + SelectedSupplierOrder.supplier.company.name.Replace("\"", "'").Replace("Товарищество с ограниченной ответственностью", "ТОО") + " №" + (Auction.siteid == 4 ? Auction.number.Length > 4 ? Auction.number.Substring(Auction.number.Length - 4) : Auction.number : Auction.number).Replace("/", "_") + ".docx", DocumentTypeEnum.ProcuratorySource);

                    //                    string procuratoryFileName = archiveManager.GetTemplate(docProcuratory, DocumentTemplateEnum.Procuratory);
                    //#endif

                    //                    // Trader name from director company
                    //                    if (SelectedSupplierOrder != null && SelectedSupplierOrder.contract != null && SelectedSupplierOrder.contract.broker != null) Order.Auction.Trader = SelectedSupplierOrder.contract.broker.company.director;
                    //                    else Order.Auction.Trader = "";

                    //                    // Formate file
                    //                    ProcuratoriesService.CreateProcuratory(Order, procuratoryFileName, tableNumbers == null ? null : tableNumbers);

                    //                    // Save files in db
                    //                    var pFile = DataBaseClient.ReadDocument((int)SelectedSupplierOrder.fileListId, (int)DocumentTypeEnum.ProcuratorySource);

                    //                    if (pFile != null) DataBaseClient.DeleteDocument(pFile.id);

                    //                    try
                    //                    {
                    //                        archiveManager.SaveFile(docProcuratory, (int)SelectedSupplierOrder.fileListId);
                    //                    }
                    //                    catch (Exception ex) { AppJournal.Write("Auction", "Save procuratory file in db error :" + ex.ToString(), true); }
                    #endregion
                } catch (Exception ex) {
                    errStatus = true;
                    errMsg = ex.ToString();
                    AppJournal.Write("Auction", "Formate procuratory error :" + ex.ToString(), true);
                }

                if (!errStatus) OpenFolder();
                else MessagesService.Show("Формирование поручения", "Произошла ошибка во время формирования поручения");
            } else MessagesService.Show("Формирование поручения", "Не выбрано поручение или ошиблись биржей");
        }


        public ICommand AttachProcuratoryScanCmd { get { return new DelegateCommand(() => AttachProcuratory(true)); } }
        public ICommand AttachProcuratoryExcelCmd { get { return new DelegateCommand(() => AttachProcuratory(false)); } }

        private void AttachProcuratory(bool isScan)
        {
            AppJournal.Write("Auction", "Attach procuratory", true);

            if (SelectedSupplierOrder != null) {
                if (Auction.siteid == 4) {
                    try {
                        sourceFileName = Service.GetFile("Выберите поручение от поставщика", isScan ? "(*.*) | *.*" : "(*.xlsx) | *.xlsx").FullName;
                    } catch (Exception) { }

                    if (!string.IsNullOrEmpty(sourceFileName)) {
                        newFileName = FolderExplorer.GetAuctionPath(MarketPlaceEnum.ETS, Auction.date.ToShortDateString(), Auction.number);
                        newFileName += "Поручение от поставщика " + SelectedSupplierOrder.supplier.company.name.Replace("\"", "'") + sourceFileName.Substring(sourceFileName.LastIndexOf("."), sourceFileName.Length - sourceFileName.LastIndexOf("."));

                        if (!Service.CopyFile(sourceFileName, newFileName, true)) MessagesService.Show("ОШИБКА", "Проверьте доступ к архиву.");
                        else {
                            var docProcuratory = GetDocumentRequisites(sourceFileName.Substring(sourceFileName.LastIndexOf("\\") + 1), isScan ? DocumentTypeEnum.Procuratory : DocumentTypeEnum.ProcuratorySource, DocumentSectionEnum.Auction);
                            if (docProcuratory != null) {
                                int fId = 0;

                                if (SelectedProcuratory.filelistid == null) {
                                    fId = DataBaseClient.CreateFileList(new FilesListEF() { description = "Поручения от " + SelectedSupplierOrder.supplier.company.name });
                                    SelectedProcuratory.filelistid = fId;

                                    DataBaseClient.UpdateProcuratory(SelectedProcuratory, SelectedProcuratory.minimalprice);
                                } else fId = (int)SelectedProcuratory.filelistid;

                                docProcuratory.filesListId = fId;
                                var pFile = DataBaseClient.ReadDocument(fId, isScan ? (int)DocumentTypeEnum.Procuratory : (int)DocumentTypeEnum.ProcuratorySource);

                                if (pFile != null) DataBaseClient.DeleteDocument(pFile.id);

                                archiveManager.SaveFile(docProcuratory, fId);

                                MessagesService.Show("Прикрепление поручения", "Поручение прикреплено к архиву");
                            }
                        }
                    }
                } else MessagesService.Show("Сообщение", "Для этой биржи функция не доступна");
            } else MessagesService.Show("Сообщение", "Не выбран поставщик");
        }


        public ICommand DeleteProcuratoryCmd { get { return new DelegateCommand(DeleteProcuratory); } }

        private void DeleteProcuratory()
        {
            AppJournal.Write("Auction", "Delete procuratory", true);

            if (SelectedProcuratory != null) {
                try {
                    DataBaseClient.DeleteProcuratory(SelectedProcuratory.id);
                    UpdateProcuratoriesList();
                    MessagesService.Show("Удаление поручения", "Поручение удалено");
                } catch (Exception ex) {
                    MessagesService.Show("Удаление поручения", "Произошла ошибка при удалении");
                    AppJournal.Write("Auction", "Delete procuratory from db error :" + ex.ToString(), true);
                }
            }
        }

        public ICommand MoneyTransferCmd { get { return new DelegateCommand(MoneyTransfer); } }
        private void MoneyTransfer()
        {
            // Log service
            AppJournal.Write("MoneyTransfer", "Create money transfer documents", true);

            // Check for selected supplier & lot
            if (SelectedSupplierOrder != null && SelectedLot != null && Auction.siteid == 4) {
                // Fill order with needed data
                Order = ConvertEFtoBO(Order, OrderPartEnum.Auction);
                Order = ConvertEFtoBO(Order, OrderPartEnum.Lot, 1);
                Order = ConvertEFtoBO(Order, OrderPartEnum.SupplierOrder, 1);

                // Get templates & copy to auction directory
                var docMoneyTransferReq = GetDocumentRequisites("Заявление в КЦ №" + Auction.number.Replace("/", "_") + " (c " + Order.Auction.SupplierOrders[0].BrokerCode + " на " + Order.Auction.SupplierOrders[0].Code + " по лоту " + SelectedLot.number + ").docx", DocumentTypeEnum.MoneyTransfer);
                var moneyTransferTemplateFile = archiveManager.GetTemplate(docMoneyTransferReq, DocumentTemplateEnum.MoneyTransfer);

                // Get transaction sum
                var transactionSum = SelectedLot.sum / 100 * Convert.ToDecimal(0.1);

                // Formate money transfer document & open folder with new document
                if (MoneyTransferService.CreateDocument(Order, moneyTransferTemplateFile, transactionSum)) {
                    OpenFolder();

                    // Put records in base
                    ClearingCountingEF clearingCounting = new ClearingCountingEF() {
                        brokerid = (int)SelectedSupplierOrder.contract.brokerid,
                        fromsupplierid = DataBaseClient.ReadSupplier(SelectedSupplierOrder.contract.broker.companyId).id,
                        tosupplierid = (int)SelectedSupplierOrder.supplierid,
                        lotid = SelectedLot.id,
                        transaction = transactionSum,
                        createdate = DateTime.Now,
                        statusid = 9
                    };

                    if (DataBaseClient.CreateClearingCounting(clearingCounting) == 0) {
                        MessagesService.Show("Заявление об изменении денежных средств", "Произошла ошибка при занесении данных в базу");
                        AppJournal.Write("MoneyTransfer", "Fault when create record in base", true);
                    }
                } else MessagesService.Show("Заявление об изменении денежных средств", "Произошла ошибка при формировании заявления");

            } else {
                MessagesService.Show("Заявление об изменении денежных средств", "Не выбран поставщик или лот.\nФункционал для биржи ЕТС.");
                AppJournal.Write("MoneyTransfer", "Fault because not selected supplier & lot. Maybe market not ETS.", true);
            }
        }

        #region Reglaments functional
        private void ReglamentControl(DateTime orderDate)
        {
            Order.Date = orderDate;

            Order.PropertyChanged += Order_PropertyChanged;
            Order.Auction.PropertyChanged += Auction_PropertyChanged;

            ProcessingDate = GetShiftedDay(Order.Date, Order.Date.Hour > (Auction.siteid == 4 ? 12 : 17) ? 1 : 0);
            Auction.regulation.applydate = ProcessingDate;
        }


        private void Auction_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("ApplicantsDeadline")) {
                Order.Auction.ExchangeProvisionDeadline = GetShiftedDay(Order.Auction.ApplicantsDeadline, 3);
                Auction.regulation.provisiondeadline = Order.Auction.ExchangeProvisionDeadline;
            }

            if (e.PropertyName.Equals("ExchangeProvisionDeadline")) {
                Order.Auction.Date = GetShiftedDay(Order.Auction.ExchangeProvisionDeadline, 1);
                Auction.date = Order.Auction.Date;
            }
        }


        private void Order_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Date")) {
                ProcessingDate = GetShiftedDay(Order.Date, Order.Date.Hour > 17 ? 1 : 0);
                Auction.regulation.applydate = ProcessingDate;
                Auction.regulation.opendate = Order.Date;
            }

            if (e.PropertyName.Equals("Deadline")) {
                Order.Auction.ApplicantsDeadline = GetShiftedDay(Order.Deadline, 1);
                Auction.regulation.applicantsdeadline = Order.Auction.ApplicantsDeadline;
            }
        }


        public DateTime processingDate;
        public DateTime ProcessingDate {
            get { return processingDate; }
            set {
                if (value != processingDate) {
                    processingDate = value;
                    RaisePropertyChangedEvent("ProcessingDate");
                    Order.Deadline = GetShiftedDay(ProcessingDate, 3);
                    Auction.regulation.applydeadline = Order.Deadline;
                }
            }
        }


        private DateTime GetShiftedDay(DateTime curDate, int nDates)
        {
            if (nDates <= 0) return curDate;
            else {
                var nextDate = curDate;
                nextDate = curDate.AddDays(1);

                if (nextDate.DayOfWeek == DayOfWeek.Saturday && nextDate != new DateTime(2017, 7, 1)) nextDate = nextDate.AddDays(2);
                else if (nextDate.DayOfWeek == DayOfWeek.Sunday) nextDate = nextDate.AddDays(1);

                while (Globals.Holydays.Count(h => h.Value.Year == nextDate.Year && h.Value.Month == nextDate.Month && h.Value.Day == nextDate.Day) > 0) nextDate = nextDate.AddDays(1);

                return GetShiftedDay(nextDate, nDates - 1);
            }
        }
        #endregion

        #region Work with qualifications
        public ICommand AddQualification { get { return new DelegateCommand(() => QualificationFunc(1)); } }
        public ICommand UpdateQualification { get { return new DelegateCommand(() => QualificationFunc(2)); } }
        public ICommand DeleteQualification { get { return new DelegateCommand(() => QualificationFunc(3)); } }

        private void QualificationFunc(int mode)
        {
            switch (mode) {
                case 1: // Add
                    QualificationEF qualification = new QualificationEF() {
                        auctionId = Auction.id,
                        qualification_dictionary_id = SelectedQualificationsDictionary.id,
                        note = QualificationText,
                        file = WithFile
                    };

                    try {
                        DataBaseClient.CreateQualification(qualification);
                    } catch { };
                    break;
                case 2: // Update
                    if (SelectedQualification != null) {
                        try {
                            SelectedQualification.qualification_dictionary_id = SelectedQualificationsDictionary.id;
                            SelectedQualification.note = QualificationText;
                            SelectedQualification.file = WithFile;

                            DataBaseClient.UpdateQualification(SelectedQualification);
                        } catch { };
                    } else MessagesService.Show("Изменение квалификационного требования", "Не выбрано требование");
                    break;
                case 3: // Delete
                    if (SelectedQualification != null) {
                        try {
                            DataBaseClient.DeleteQualification(SelectedQualification.id);
                        } catch { };
                    } else MessagesService.Show("Удаление квалификационного требования", "Не выбрано требование");
                    break;
            }

            QualificationsLst = new List<QualificationEF>(DataBaseClient.ReadQualifications(Auction.id));
        }
        #endregion

        #region Utilits
        protected override List<CommandViewModel> CreateCommands()
        {
            throw new NotImplementedException();
        }


        private Order ConvertEFtoBO(Order orderItem, OrderPartEnum orderPartEnum, int quantityType = 0)
        {
            // Fill choose & order parts
            switch (orderPartEnum) {
                case OrderPartEnum.Auction:
                    // Check auction exist
                    if (orderItem.Auction == null) orderItem.Auction = new Auction();

                    // Fill data
                    orderItem.customerid = Auction.customerid;
                    orderItem.Auction.Date = Auction.date;
                    orderItem.Auction.Site = Auction.site.name;
                    orderItem.Auction.Number = Auction.number;
                    orderItem.Date = Auction.regulation.opendate;
                    orderItem.Auction.Type = Auction.type.name;
                    orderItem.Auction.Customer = Auction.customer.company.name;
                    orderItem.Auction.SiteId = Auction.siteid;
                    break;
                case OrderPartEnum.Lot:
                    // Check lot exist
                    if (orderItem.Auction.Lots == null) orderItem.Auction.Lots = new ObservableCollection<Lot>();

                    // Clear exist lots
                    orderItem.Auction.Lots.Clear();

                    // Choose filling type
                    foreach (var item in (quantityType == 0 ? LotList.Where(ll => ll.id == LotList[0].id) : quantityType == 2 ? LotList : LotList.Where(ll => ll.id == SelectedLot.id))) {
                        orderItem.Auction.Lots.Add(new Lot() {
                            Name = item.description,
                            Number = item.number,
                            CodeTRFEA = "",
                            StartPrice = item.sum.ToString(),
                            Quantity = item.amount,
                            Unit = item.unit != null ? item.unit.description : "ед.",
                            UnitId = item.unit != null ? item.unitid : 0,
                            Sum = item.sum,
                            Price = item.price,
                            Step = Convert.ToDecimal(item.step),
                            DeliveryPlace = item.deliveryplace,
                            DeliveryTime = item.deliverytime,
                            PaymentTerm = item.paymentterm,
                            Warranty = Convert.ToDecimal(item.warranty),
                            LocalContent = item.localcontent
                        });
                    }
                    break;
                case OrderPartEnum.SupplierOrder:
                    // Check supplier order exist
                    if (orderItem.Auction.SupplierOrders == null) orderItem.Auction.SupplierOrders = new ObservableCollection<SupplierOrder>();

                    // Clear exist supplier orders
                    orderItem.Auction.SupplierOrders.Clear();

                    // Choose filling type
                    foreach (var item in (quantityType == 1 ? SupplierOrdersList.Where(ll => ll.id == SelectedSupplierOrder.id) : SupplierOrdersList)) {
                        orderItem.Auction.SupplierOrders.Add(new SupplierOrder() {
                            SupplierId = (int)item.supplierid,
                            Name = item.supplier.company.name,
                            Address = item.supplier.company.addresslegal,
                            BIN = item.supplier.company.bin,
                            IIK = item.supplier.company.iik,
                            BankName = item.contract != null ? item.contract.bank != null ? item.contract.bank.name : "" : "",
                            BIK = item.contract != null ? item.contract.bank != null ? item.contract.bank.company.bik : "" : "",
                            Phones = item.supplier.company.telephone,
                            Code = Auction.siteid == 4 ? DataBaseClient.ReadSuppliersJournals((int)item.supplierid, (int)item.contract.brokerid)[0].code : "",
                            BrokerName = item.contract.broker.name,
                            BrokerCode = Auction.siteid == 4 ? DataBaseClient.ReadSuppliersJournals((int)DataBaseClient.ReadSuppliers().FirstOrDefault(s => s.companyid == item.contract.broker.companyId).id, (int)item.contract.brokerid)[0].code : "",
                            BrokerBIN = item.contract.broker.company.bin,
                            BrokerAddress = item.contract.broker.company.addresslegal,
                            BrokerPhones = item.contract.broker.company.telephone
                        });
                    }
                    break;
                case OrderPartEnum.Procuratory:
                    // Check procuratory exist
                    if (orderItem.Auction.Procuratories == null) orderItem.Auction.Procuratories = new ObservableCollection<Procuratory>();

                    // Clear exist procuratory
                    orderItem.Auction.Procuratories.Clear();

                    // Choose filling type
                    foreach (var item in (quantityType == 0 ? ProcuratoriesList.Where(ll => ll.id == ProcuratoriesList[0].id) : DataBaseClient.ReadProcuratories(Auction.id))) {
                        orderItem.Auction.Procuratories.Add(new Procuratory() {
                            MinimalPrice = item.minimalprice,
                            SupplierId = (int)item.supplierid
                        });
                    }
                    break;
            }

            return orderItem;
        }

        private static string ConvertToShortName(string fullName)
        {
            if (fullName.ToUpper().Contains("ТОВАРИЩЕСТВО С ОГРАНИЧЕННОЙ ОТВЕТСТВЕННОСТЬЮ")) fullName = fullName.ToUpper().Replace("ТОВАРИЩЕСТВО С ОГРАНИЧЕННОЙ ОТВЕТСТВЕННОСТЬЮ", "ТОО");
            else if (fullName.ToUpper().Contains("АКЦИОНЕРНОЕ ОБЩЕСТВО")) fullName = fullName.ToUpper().Replace("АКЦИОНЕРНОЕ ОБЩЕСТВО", "АО");
            else if (fullName.ToUpper().Contains("ИНДИВИДУАЛЬНЫЙ ПРЕДПРИНИМАТЕЛЬ")) fullName = fullName.ToUpper().Replace("ИНДИВИДУАЛЬНЫЙ ПРЕДПРИНИМАТЕЛЬ", "ИП");
            else if (fullName.ToUpper().Contains("ОБЩЕСТВО С ОГРАНИЧЕННОЙ ОТВЕТСТВЕННОСТЬЮ")) fullName = fullName.ToUpper().Replace("ОБЩЕСТВО С ОГРАНИЧЕННОЙ ОТВЕТСТВЕННОСТЬЮ", "ООО");

            return fullName.Replace("\"", "'");
        }
        #endregion

        #endregion

        #region Bindings
        private AuctionEF _auction;
        public AuctionEF Auction {
            get { return _auction; }
            set { _auction = value; RaisePropertyChangedEvent("Auction"); }
        }

        private Order _order;
        public Order Order {
            get { if (_order == null) _order = new Order(); return _order; }
            set { if (value != _order) { _order = value; RaisePropertyChangedEvent("Order"); } }
        }

        private String _formTitle;
        public String FormTitle {
            get { return _formTitle; }
            set { _formTitle = value; RaisePropertyChangedEvent("FormTitle"); }
        }

        private List<SectionEF> _sectionsList;
        public List<SectionEF> SectionList {
            get { return _sectionsList; }
            set { _sectionsList = value; RaisePropertyChangedEvent("SectionList"); }
        }

        private SectionEF _selectedSection;
        public SectionEF SelectedSection {
            get { return _selectedSection; }
            set {
                _selectedSection = value;
                Auction.sectionid = value.id;
                RaisePropertyChangedEvent("SelectedSection");
            }
        }

        private List<TypeEF> _typeList;
        public List<TypeEF> TypeList {
            get { return _typeList; }
            set { _typeList = value; RaisePropertyChangedEvent("TypeList"); }
        }

        private TypeEF _selectedType;
        public TypeEF SelectedType {
            get { return _selectedType; }
            set {
                _selectedType = value;
                Auction.typeid = value.id;
                RaisePropertyChangedEvent("SelectedType");
            }
        }

        private List<StatusEF> _statusList;
        public List<StatusEF> StatusList {
            get { return _statusList; }
            set { _statusList = value; RaisePropertyChangedEvent("StatusList"); }
        }

        private StatusEF _selectedStatus;
        public StatusEF SelectedStatus {
            get { return _selectedStatus; }
            set {
                _selectedStatus = value;
                Auction.statusid = value.id;
                RaisePropertyChangedEvent("SelectedStatus");
            }
        }

        private List<SiteEF> _sourceList;
        public List<SiteEF> SourceList {
            get { return _sourceList; }
            set { _sourceList = value; RaisePropertyChangedEvent("SourceList"); }
        }

        private SiteEF _selectedSource;
        public SiteEF SelectedSource {
            get { return _selectedSource; }
            set {
                _selectedSource = value;
                Auction.siteid = value.id;
                RaisePropertyChangedEvent("SelectedSource");
            }
        }

        private List<CustomerEF> _customerList;
        public List<CustomerEF> CustomerList {
            get { return _customerList; }
            set { _customerList = value; RaisePropertyChangedEvent("CustomerList"); }
        }

        private CustomerEF _selectedCustomer;
        public CustomerEF SelectedCustomer {
            get { return _selectedCustomer; }
            set {
                _selectedCustomer = value;
                Auction.customerid = value.id;
                RaisePropertyChangedEvent("SelectedCustomer");
            }
        }

        private List<BrokerEF> _brokerList;
        public List<BrokerEF> BrokerList {
            get { return _brokerList; }
            set { _brokerList = value; RaisePropertyChangedEvent("BrokerList"); }
        }

        private BrokerEF _selectedBroker;
        public BrokerEF SelectedBroker {
            get { return _selectedBroker; }
            set {
                _selectedBroker = value;
                Auction.brokerid = value.id;
                RaisePropertyChangedEvent("SelectedBroker");
            }
        }

        private List<TraderEF> _traderList;
        public List<TraderEF> TraderList {
            get { return _traderList; }
            set { _traderList = value; RaisePropertyChangedEvent("TraderList"); }
        }

        private TraderEF _selectedTrader;
        public TraderEF SelectedTrader {
            get { return _selectedTrader; }
            set {
                _selectedTrader = value;
                Auction.traderid = value.id;
                RaisePropertyChangedEvent("SelectedTrader");
            }
        }

        private ObservableCollection<LotEF> _lotList;
        public ObservableCollection<LotEF> LotList {
            get { return _lotList; }
            set { _lotList = value; RaisePropertyChangedEvent("LotList"); }
        }

        private LotEF _selectedLot;
        public LotEF SelectedLot {
            get { return _selectedLot; }
            set { _selectedLot = value; RaisePropertyChangedEvent("SelectedLot"); }
        }

        private ObservableCollection<SupplierOrderEF> _supplierOrdersList;
        public ObservableCollection<SupplierOrderEF> SupplierOrdersList {
            get { return _supplierOrdersList; }
            set { _supplierOrdersList = value; RaisePropertyChangedEvent("SupplierOrdersList"); }
        }

        private SupplierOrderEF _selectedSupplierOrder;
        public SupplierOrderEF SelectedSupplierOrder {
            get { return _selectedSupplierOrder; }
            set {
                _selectedSupplierOrder = value;

                if (value != null) {
                    try {
                        var supplierFile = DataBaseClient.ReadDocuments((int)value.fileListId, 8);

                        if (supplierFile != null && supplierFile.Count > 0) SOInfoTxt = "Имеется скан от заказчика";
                        else SOInfoTxt = "Нет скана от заказчика";
                    } catch { SOInfoTxt = "Нет скана от заказчика"; }
                }

                ProcuratoriesList = new ObservableCollection<ProcuratoryEF>(DataBaseClient.ReadProcuratories((int)value.supplierid, Auction.id));
                RaisePropertyChangedEvent("SelectedSupplierOrder");
            }
        }

        private ObservableCollection<ProcuratoryEF> _procuratoriesList;
        public ObservableCollection<ProcuratoryEF> ProcuratoriesList {
            get { return _procuratoriesList; }
            set { _procuratoriesList = value; RaisePropertyChangedEvent("ProcuratoriesList"); }
        }

        private ProcuratoryEF _selectedProcuratory;
        public ProcuratoryEF SelectedProcuratory {
            get { return _selectedProcuratory; }
            set { _selectedProcuratory = value; RaisePropertyChangedEvent("SelectedProcuratory"); }
        }

        private ObservableCollection<CommandViewModel> _commandsSpc;
        public ObservableCollection<CommandViewModel> CommandsSpc {
            get { return _commandsSpc; }
            set { _commandsSpc = value; RaisePropertyChangedEvent("CommandsSpc"); }
        }

        private string _sOInfoTxt;
        public string SOInfoTxt {
            get { return _sOInfoTxt; }
            set { _sOInfoTxt = value; RaisePropertyChangedEvent("SOInfoTxt"); }
        }

        private List<QualificationDictionary> _qualificationsDictionaryLst;
        public List<QualificationDictionary> QualificationsDictionaryLst {
            get { return _qualificationsDictionaryLst; }
            set { _qualificationsDictionaryLst = value; RaisePropertyChangedEvent("QualificationsDictionaryLst"); }
        }

        private QualificationDictionary _selectedQualificationsDictionary;
        public QualificationDictionary SelectedQualificationsDictionary {
            get { return _selectedQualificationsDictionary; }
            set {
                _selectedQualificationsDictionary = value;

                if (value != null) {
                    QualificationText = value.description;
                    WithFile = false;
                }

                RaisePropertyChangedEvent("SelectedQualificationsDictionary");
            }
        }

        private List<QualificationEF> _qualificationsLst;
        public List<QualificationEF> QualificationsLst {
            get { return _qualificationsLst; }
            set { _qualificationsLst = value; RaisePropertyChangedEvent("QualificationsLst"); }
        }

        private QualificationEF _selectedQualification;
        public QualificationEF SelectedQualification {
            get { return _selectedQualification; }
            set {
                _selectedQualification = value;

                if (value != null) {
                    SelectedQualificationsDictionary = QualificationsDictionaryLst.FirstOrDefault(qd => qd.id == value.qualification_dictionary_id);
                    QualificationText = value.note;
                    WithFile = value.file;
                }

                RaisePropertyChangedEvent("SelectedQualification");
            }
        }

        private string _qualificationText;
        public string QualificationText {
            get { return _qualificationText; }
            set { _qualificationText = value; RaisePropertyChangedEvent("QualificationText"); }
        }

        private bool _withFile;
        public bool WithFile {
            get { return _withFile; }
            set { _withFile = value; RaisePropertyChangedEvent("WithFile"); }
        }

        private System.Windows.Visibility _reqDocsVis;
        public System.Windows.Visibility ReqDocsVis {
            get { return _reqDocsVis; }
            set { _reqDocsVis = value; RaisePropertyChangedEvent("ReqDocsVis"); }
        }
        #endregion
    }
}