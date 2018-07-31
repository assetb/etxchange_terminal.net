using System.Collections.Generic;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using AltaTransport;
using AltaMySqlDB.model.tables;
using System.Windows.Input;
using AltaDock.vm;
using System;
using AltaArchive.view;
using DocumentFormation;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AltaLog;
using AltaBO;
using AltaBO.specifics;
using AltaArchiveApp;
using AltaMySqlDB.service;
using AltaMySqlDB.model;
using AltaArchive.Services;
using System.Threading;
using AltaMySqlDB.model.views;

namespace AltaArchive.vm
{
    public class ClearingCenterViewModel : BaseViewModel
    {
        #region Variables
        private IDataManager dataManager = new EntityContext();
        private ArchiveManager archiveManager;
        private string path = "";
        private string templateFile = "";
        private string templatePath = @"\\192.168.11.5\Archive\Templates\ETS\";
        private List<CompaniesWithContractView> SuppliersListStorage;
        #endregion

        #region Methods
        public ClearingCenterViewModel()
        {
            //DefaultParametrs();
        }

        private void DefaultParametrs()
        {
            archiveManager = new ArchiveManager(dataManager);

            SuppliersListStorage = dataManager.GetSuppliersWithContract();
            BrokersList = DataBaseClient.ReadBrokers();
            SelectedBroker = BrokersList[0];
            FromDate = DateTime.Now.AddDays(-14);
            ToDate = DateTime.Now;
            StatusesList = DataBaseClient.ReadStatuses().Where(s => s.id > 8).ToList();
            SelectedStatus = StatusesList[0];

            FormateDate = DateTime.Now;
        }

        public ICommand ApplyCmd { get { return new DelegateCommand(() => Apply(0)); } }
        public ICommand ApplyByOneCmd { get { return new DelegateCommand(() => Apply(1)); } }
        private void Apply(int mode)
        {
            if (mode == 0) ClearingCountingsList = DataBaseClient.ReadClearingCountings(FromDate, ToDate, SelectedStatus.id, SelectedBroker.id);
            else ClearingCountingsList = DataBaseClient.ReadClearingCountings(FromDate, ToDate, SelectedStatus.id, SelectedBroker.id, SelectedSupplier.companyId);
        }

        public ICommand CreateBackMoneyTransferCmd { get { return new DelegateCommand(CreateBackMoneyTransfer); } }
        private void CreateBackMoneyTransfer()
        {
            // Log service
            AppJournal.Write("BackMoneyTransfer", "Create back money transfer documents", true);

            // Check for selected transaction
            if (SelectedClearingCount != null && SelectedClearingCount.statusid != 10)
            {
                // Get templates & copy to auction directory
                var fromCount = DataBaseClient.ReadSuppliersJournals(SelectedClearingCount.tosupplierid, SelectedClearingCount.brokerid);
                var toCount = DataBaseClient.ReadSuppliersJournals(SelectedClearingCount.fromsupplierid, SelectedClearingCount.brokerid);

                DocumentRequisite docMoneyTransferReq = new DocumentRequisite()
                {
                    fileName = "Заявление на возврат из КЦ №" + SelectedClearingCount.lot.auction.number.Replace("/", "_") + " (c " + fromCount[0].code + " на " + toCount[0].code + ").docx",
                    market = MarketPlaceEnum.ETS,
                    date = SelectedClearingCount.lot.auction.date,
                    number = SelectedClearingCount.lot.auction.number
                };

                var moneyTransferTemplateFile = archiveManager.GetTemplate(docMoneyTransferReq, DocumentTemplateEnum.MoneyTransfer);

                // Fill order with needed data
                Order order = new Order();
                order.Auction = new Auction();
                order.Auction.SupplierOrders = new System.Collections.ObjectModel.ObservableCollection<SupplierOrder>();

                order.Auction.SupplierOrders.Add(new SupplierOrder()
                {
                    BrokerName = SelectedClearingCount.broker.name,
                    BrokerCode = fromCount[0].code,
                    Code = toCount[0].code
                });

                order.Auction.Lots = new System.Collections.ObjectModel.ObservableCollection<Lot>();

                order.Auction.Lots.Add(new Lot()
                {
                    Number = SelectedClearingCount.lot.number
                });

                // Formate money transfer document & open folder with new document
                if (MoneyTransferService.CreateDocument(order, moneyTransferTemplateFile, SelectedClearingCount.transaction))
                {
                    FolderExplorer.OpenAuctionFolder(MarketPlaceEnum.ETS, SelectedClearingCount.lot.auction.date.ToShortDateString(), SelectedClearingCount.lot.auction.number);

                    // Put records in base
                    ClearingCountingEF clearingCounting = new ClearingCountingEF()
                    {
                        brokerid = SelectedClearingCount.brokerid,
                        fromsupplierid = fromCount[0].supplierid,
                        tosupplierid = toCount[0].supplierid,
                        lotid = SelectedClearingCount.lotid,
                        transaction = SelectedClearingCount.transaction,
                        createdate = DateTime.Now,
                        statusid = 10
                    };

                    if (DataBaseClient.CreateClearingCounting(clearingCounting) == 0)
                    {
                        MessagesService.Show("Заявление об изменении денежных средств", "Произошла ошибка при занесении данных в базу");
                        AppJournal.Write("MoneyTransfer", "Fault when create record in base", true);
                    }
                    else DataBaseClient.UpdateClearingCountStatus(toCount[0].supplierid, fromCount[0].supplierid, SelectedClearingCount.lotid);

                    Apply(0);
                }
                else MessagesService.Show("Заявление об изменении денежных средств", "Произошла ошибка при формировании заявления");
            }
            else
            {
                MessagesService.Show("Заявление о возврате денежных средств", "Не выбрана транзакция или статус ОПЛАЧЕНО.");
                AppJournal.Write("BackMoneyTransfer", "Fault because not selected transaction or status payed.", true);
            }
        }

        public ICommand FormateTransferCmd { get { return new DelegateCommand(FormateTransfer); } }
        private void FormateTransfer()
        {
            // Check for auctions in selected date
            var procuratories = DataBaseClient.ReadProcuratories(FormateDate);

            var supplierOrders = DataBaseClient.ReadSupplierOrders().Where(s => s.auction.siteid == 4 && s.auction.date == FormateDate && s.supplierid != 3 && s.supplierid != 27 && s.supplierid != 354 && s.supplierid != 384 && s.supplierid != 385).ToList();

            if (procuratories != null && procuratories.Count() > 0)
            {
                // Set folder for files
                try
                {
                    path = Service.GetDirectory().FullName;
                }
                catch { path = ""; }

                if (!string.IsNullOrEmpty(path))
                {
                    for (var iBroker = 1; iBroker <= 4; iBroker++)
                    {
                        // Get template & copy them
                        if (supplierOrders.Where(s => s.contract.brokerid == iBroker).Count() > 0)
                        {
                            templateFile = path + "\\" + "Перевод в КЦ по ТОО " + (iBroker == 1 ? "'Альта и К'" : iBroker == 2 ? "'Корунд-777'" : iBroker == 3 ? "'Альтаир-Нур'" : "'Ак Алтын Ко'") + " для даты аукциона " + FormateDate.ToShortDateString() + ".docx";

                            if (Service.CopyFile(templatePath + "MoneyTransfer.docx", templateFile, true))
                            {
                                // Convert EF to BO
                                List<MoneyTransferList> moneyTransferList = new List<MoneyTransferList>();

                                var supplierJournal = DataBaseClient.ReadSuppliersJournals();
                                List<ClearingCountingEF> clearingCountingLst = new List<ClearingCountingEF>();

                                foreach (var item in supplierOrders.Where(s => s.contract.brokerid == iBroker))
                                {
                                    foreach (var subItem in procuratories.Where(p => p.supplierid == item.supplierid && p.auctionid == item.auctionid))
                                    {
                                        try
                                        {
                                            moneyTransferList.Add(new MoneyTransferList()
                                            {
                                                fromCompany = iBroker == 1 ? "altaik" : iBroker == 2 ? "korund" : iBroker == 3 ? "alta008" : "akaltko",
                                                toCompany = supplierJournal.FirstOrDefault(s => s.supplierid == item.supplierid && s.brokerid == iBroker).code,
                                                lotNumber = subItem.lot.number,
                                                sum = (subItem.lot.sum / 100 * Convert.ToDecimal(0.1)).ToString()
                                            });

                                            //
                                            clearingCountingLst.Add(new ClearingCountingEF()
                                            {
                                                brokerid = iBroker,
                                                fromsupplierid = iBroker == 1 ? 354 : iBroker == 2 ? 354 : iBroker == 3 ? 27 : 384,
                                                tosupplierid = (int)subItem.supplierid,
                                                lotid = (int)subItem.lotid,
                                                transaction = subItem.lot.sum / 100 * Convert.ToDecimal(0.1),
                                                createdate = DateTime.Now,
                                                statusid = 9
                                            });

                                            // Put records in base                                            
                                            /*if(DataBaseClient.CreateClearingCounting(clearingCounting) == 0) {
                                                MessagesService.Show("Заявление об изменении денежных средств", "Произошла ошибка при занесении данных в базу");
                                                AppJournal.Write("MoneyTransfer", "Fault when create record in base", true);
                                            }*/

                                        }
                                        catch { }
                                    }
                                }

                                // Save into data base
                                foreach (var item in clearingCountingLst)
                                {
                                    // Check for exist
                                    var searchItem = DataBaseClient.ReadClearingCounting(item.brokerid, item.fromsupplierid, item.tosupplierid, item.lotid);

                                    // Save if not or remove old and create new record
                                    if (searchItem != null)
                                    { // Update
                                        DataBaseClient.UpdateClearingCounting(item.id, item.transaction);
                                    }
                                    else
                                    { // New
                                        if (DataBaseClient.CreateClearingCounting(item) == 0)
                                        {
                                            MessagesService.Show("Заявление об изменении денежных средств", "Произошла ошибка при занесении данных в базу");
                                            AppJournal.Write("MoneyTransfer", "Fault when create record in base", true);
                                        }
                                    }
                                }
                                //

                                // Formate file
                                MoneyTransferService.CreateDocument(templateFile, (iBroker == 1 ? "ТОО 'Альта и К'" : iBroker == 2 ? "ТОО 'Корунд-777'" : iBroker == 3 ? "ТОО 'Альтаир-Нур'" : "ТОО 'Ак Алтын Ко'"), moneyTransferList);
                            }
                        }
                    }
                }

                // Open folder
                FolderExplorer.OpenFolder(path + "\\");

                /*// Get templates & copy to auction directory
                var docMoneyTransferReq = GetDocumentRequisites("Заявление в КЦ №" + Auction.number.Replace("/", "_") + " (c " + Order.Auction.SupplierOrders[0].BrokerCode + " на " + Order.Auction.SupplierOrders[0].Code + ").docx", DocumentTypeEnum.MoneyTransfer);
                var moneyTransferTemplateFile = archiveManager.GetTemplate(docMoneyTransferReq, DocumentTemplateEnum.MoneyTransfer);

                // Get transaction sum
                var transactionSum = Order.Auction.Lots[0].Sum / 100 * Convert.ToDecimal(0.1);

                // Formate money transfer document & open folder with new document
                if(MoneyTransferService.CreateDocument(Order, moneyTransferTemplateFile, transactionSum)) {
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

                    if(DataBaseClient.CreateClearingCounting(clearingCounting) == 0) {
                        MessagesService.Show("Заявление об изменении денежных средств", "Произошла ошибка при занесении данных в базу");
                        AppJournal.Write("MoneyTransfer", "Fault when create record in base", true);
                    }
                } else MessagesService.Show("Заявление об изменении денежных средств", "Произошла ошибка при формировании заявления");*/
                //
            }
            else MessagesService.Show("Оповещение", "На эту дату нет аукционов или не созданы поручения");
        }
        #endregion

        #region Bindings
        private DateTime _fromDate;
        public DateTime FromDate {
            get { return _fromDate; }
            set { _fromDate = value; RaisePropertyChangedEvent("FromDate"); }
        }

        private DateTime _toDate;
        public DateTime ToDate {
            get { return _toDate; }
            set { _toDate = value; RaisePropertyChangedEvent("ToDate"); }
        }

        private List<ClearingCountingEF> _clearingCountingsList;
        public List<ClearingCountingEF> ClearingCountingsList {
            get { return _clearingCountingsList; }
            set { _clearingCountingsList = value; RaisePropertyChangedEvent("ClearingCountingsList"); }
        }

        private ClearingCountingEF _selectedClearingCount;
        public ClearingCountingEF SelectedClearingCount {
            get { return _selectedClearingCount; }
            set { _selectedClearingCount = value; RaisePropertyChangedEvent("SelectedClearingCounting"); }
        }

        private List<StatusEF> _statusesList;
        public List<StatusEF> StatusesList {
            get { return _statusesList; }
            set { _statusesList = value; RaisePropertyChangedEvent("StatusesList"); }
        }

        private StatusEF _selectedStatus;
        public StatusEF SelectedStatus {
            get { return _selectedStatus; }
            set { _selectedStatus = value; RaisePropertyChangedEvent("SelectedStatus"); }
        }

        private List<BrokerEF> _brokersList;
        public List<BrokerEF> BrokersList {
            get { return _brokersList; }
            set {
                _brokersList = value;
                RaisePropertyChangedEvent("BrokersList");
            }
        }

        private BrokerEF _selectedBroker;
        public BrokerEF SelectedBroker {
            get { return _selectedBroker; }
            set {
                _selectedBroker = value;
                SuppliersList = SuppliersListStorage.Where(s => s.brokerId == value.id).ToList();
                SelectedSupplier = SuppliersList[0];
                RaisePropertyChangedEvent("SelectedBroker");
            }
        }

        private List<CompaniesWithContractView> _suppliersList;
        public List<CompaniesWithContractView> SuppliersList {
            get { return _suppliersList; }
            set { _suppliersList = value; RaisePropertyChangedEvent("SuppliersList"); }
        }

        private CompaniesWithContractView _selectedSupplier;
        public CompaniesWithContractView SelectedSupplier {
            get { return _selectedSupplier; }
            set { _selectedSupplier = value; RaisePropertyChangedEvent("SelectedSupplier"); }
        }

        private DateTime _formateDate;
        public DateTime FormateDate {
            get { return _formateDate; }
            set { _formateDate = value; RaisePropertyChangedEvent("FormateDate"); }
        }
        #endregion
    }
}