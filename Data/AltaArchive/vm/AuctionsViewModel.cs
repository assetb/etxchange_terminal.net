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
using AltaBO.specifics;
using AltaMySqlDB.service;
using AltaMySqlDB.model;
using System.IO;
using AltaArchive.Services;

namespace AltaArchive.vm
{
    public class AuctionsViewModel : BaseViewModel
    {
        #region Variables
        private IDataManager dataManager = new EntityContext();
        private const string IP_PATH = @"\\192.168.11.5\Archive\";
        #endregion

        #region Methods
        public AuctionsViewModel()
        {
            Init();
        }


        private void Init()
        {
            FromDate = DateTime.Now.AddDays(-14);
            ToDate = DateTime.Now.AddDays(14);
            StatusesList = DataBaseClient.ReadStatuses().Where(s => s.id < 5).ToList();
            SelectedStatus = StatusesList[3];
            TradersList = DataBaseClient.ReadTraders();//.Where(t => t.id < 10).ToList();
            SelectedTrader = TradersList[0];
            SitesList = DataBaseClient.ReadSites().Where(s => s.id == 1 || s.id > 3).ToList();
            SelectedSite = SitesList.FirstOrDefault(s => s.id == 4);
        }


        private void CheckForEllapsedAuctions()
        {
            if (AuctionsList.Where(a => a.date > DateTime.Now && a.statusid == 4).Count() > 0) MessagesService.Show("Оповещение", "Есть не закрытые аукционы");
        }


        public void UpdateAuctionsList()
        {
            AuctionsList = DataBaseClient.ReadAuctions();
            CheckForEllapsedAuctions();
        }


        public ICommand ApplyCmd { get { return new DelegateCommand(() => UpdateAuctionsList(FromDate, ToDate, SelectedSite.id, SelectedStatus.id, SelectedTrader.id, string.IsNullOrEmpty(SearchText) ? null : SearchText)); } }
        public ICommand ApplyAllStatusesCmd { get { return new DelegateCommand(() => UpdateAuctionsList(FromDate, ToDate, SelectedSite.id, 0, SelectedTrader.id, string.IsNullOrEmpty(SearchText) ? null : SearchText)); } }
        private void UpdateAuctionsList(DateTime fromDate, DateTime toDate, int site, int statusId, int traderId, string searchText = null)
        {
            AppJournal.Write("Auctions List", "Update get auctions and check new orders from base", true);

            try {
                if (string.IsNullOrEmpty(searchText)) AuctionsList = DataBaseClient.ReadAuctions(fromDate.AddDays(-1), toDate, site, statusId, traderId);
                else AuctionsList = DataBaseClient.ReadAuctions(fromDate.AddDays(-1), toDate, site, statusId, traderId, searchText);
            } catch (Exception ex) { AppJournal.Write("Auctions List", "Get auctions list from base error: " + ex.ToString(), true); }

            try {
                NewOrders = DataBaseClient.GetOrders(site, 1);
            } catch (Exception ex) { AppJournal.Write("Orders List", "Get new orders list from error: " + ex.ToString(), true); }

            if (NewOrders.Count > 0) NewOrdersVis = System.Windows.Visibility.Visible;
            else NewOrdersVis = System.Windows.Visibility.Collapsed;
        }


        public ICommand ShowHideOrdersCmd { get { return new DelegateCommand(ShowHideOrders); } }
        private void ShowHideOrders()
        {
            if (NewOrdersVis == System.Windows.Visibility.Visible) NewOrdersVis = System.Windows.Visibility.Collapsed;
            else NewOrdersVis = System.Windows.Visibility.Visible;
        }


        public ICommand CreateFromOrderCmd { get { return new DelegateCommand(CreateFromOrder); } }
        private void CreateFromOrder()
        {
            AuctionFormShow(1, SelectedNewOrder);
        }


        public ICommand CreateAuctionCmd { get { return new DelegateCommand(() => AuctionFormShow(1)); } }
        public ICommand ReadAuctionCmd { get { return new DelegateCommand(() => AuctionFormShow(2)); } }
        public ICommand UpdateAuctionCmd { get { return new DelegateCommand(() => AuctionFormShow(3)); } }
        private void AuctionFormShow(int crudMode, OrderEF selectedOrder = null)
        {
            AppJournal.Write("Auctions List", "Open auction - " + (crudMode == 1 ? "new" : SelectedAuction.number), true);

            if (crudMode == 1) SelectedAuction = null;

            var auctionViewModel = new AuctionViewModel(SelectedAuction, selectedOrder, (MarketPlaceEnum)SelectedSite.id) { Description = "Аукцион № " + (crudMode == 2 ? SelectedAuction.number : ""), DockLocation = altaik.baseapp.helper.PanelDockLocationEnum.DetailsDocument };
            var auctionView = new AuctionView();
            auctionViewModel.View = auctionView;

            Workspace.This.Panels.Add(auctionViewModel);
            Workspace.This.ActiveDocument = auctionViewModel;
        }


        public ICommand DeleteAuctionCmd => new DelegateCommand(DeleteAuction);
        private void DeleteAuction()
        {
            if (SelectedAuction != null) {
                try {
                    DataBaseClient.DeleteAuction(SelectedAuction.id);
                } catch (Exception) { MessagesService.Show("ОШИБКА", "Произошла ошибка во время удаления"); }

                UpdateAuctionsList(FromDate, ToDate, SelectedSite.id, SelectedStatus.id, SelectedTrader.id);
            }
        }


        #region Notifications
        public ICommand MessagesCmd { get { return new DelegateCommand(ShowMessages); } }
        private void ShowMessages()
        {
            if (SelectedSite.id == 4) {
                var messages = dataManager.GetNotifications(0, 0, 3); // About new supplier orders

                messages.AddRange(dataManager.GetNotifications(0, 0, 4)); // Procuratory scan from supplier
                messages.AddRange(dataManager.GetNotifications(0, 0, 7)); // Reminder to supplier about filling tech spec

                string messagesList = "";

                if (messages != null && messages.Count > 0) {
                    foreach (var item in messages) {
                        switch (item.eventId) {
                            case 3:
                                messagesList += item.description + " по аукциону №" + dataManager.GetAuction(item.auctionId).Number + "\n";
                                break;
                            case 4:
                                messagesList += item.description + " " + dataManager.GetSupplier(item.supplierId).Name + " по аукциону №" + dataManager.GetAuction(item.auctionId).Number + "\n";
                                break;
                            case 7:
                                messagesList += item.description + " по аукциону №" + dataManager.GetAuction(item.auctionId).Number + "\n" + " у победителя " + dataManager.GetSupplier(item.supplierId).Name;
                                break;
                        }
                    }

                    MessagesService.Show("Список сообщений.", messagesList);
                }
            } else MessagesService.Show("Оповещения для биржи ЕТС", "Данный функционал пока не доступен для иных бирж.");

            CheckMessagesCount();
        }


        private void CheckMessagesCount()
        {
            /*var messages = dataManager.GetNotifications(0, 0, 3); // About new supplier orders
            messages.AddRange(dataManager.GetNotifications(0, 0, 4));
            messages.AddRange(dataManager.GetNotifications(0, 0, 7));

            MessagesTxt = string.Format("Сообщений ({0})", (messages == null ? "0" : messages.Count.ToString()));*/
        }
        #endregion

        #region ETS Reports
        public ICommand FormateETSReportsCusCmd { get { return new DelegateCommand(() => FormateETSReports(1)); } }
        public ICommand FormateETSReportsSupCmd { get { return new DelegateCommand(() => FormateETSReports(2)); } }
        private void FormateETSReports(int type)
        {
            AppJournal.Write("Auctions List", "ETS reports formating", true);

            string sourceFileName = "";
            bool errStatus = false;

            try {
                sourceFileName = Service.GetFile("Выберите файл отчета", "(IPO_RPT*.xml) | *.xml").FullName;
            } catch (Exception ex) { AppJournal.Write("ETS Reports formating", "error :" + ex.ToString(), true); }

            if (!string.IsNullOrEmpty(sourceFileName)) {
                try {
                    ReportBP.MakeNewReport(sourceFileName, type);
                    errStatus = false;
                } catch (Exception ex) {
                    errStatus = true;
                    AppJournal.Write("ETS Reports formating", "error :" + ex.ToString(), true);
                }

                if (errStatus) MessagesService.Show("Формирование отчетов", "При формировании произошла ошибка");
                else MessagesService.Show("Формирование отчетов", "Отчеты сформированы");
            }
        }


        public ICommand GetETSReportsCmd => new DelegateCommand(GetETSReports);
        private async void GetETSReports()
        {
            int daysBefore = 0;

            try {
                daysBefore = Convert.ToInt32(await MessagesService.GetInput("Сегодняшняя дата " + DateTime.Now.ToShortDateString(), "Введите 0, если нужны отчеты за сегодня, 1 отчеты за вчера, 2 за позавчера и так далее...и нажмите ENTER"));
            } catch { return; }

            DateTime curDate = DateTime.Now.AddDays(-daysBefore);
            List<string> brokersList = new List<string>() { "KORD", "ALTK", "ALTA", "AKAL" };
            string message = "";

            if (!Directory.Exists("C:\\Temp")) Directory.CreateDirectory("C:\\Temp");

            foreach (var broker in brokersList) {
                string fileName = "IPO_RPT_2~104~" + broker + "~" + curDate.ToString("yyyy.MM.dd") + "_*.xml";

                string[] fileNames = Directory.GetFiles("\\\\EDI_" + broker + "\\edi\\in", fileName);

                if (fileNames.Length > 0 && File.Exists(fileNames[0])) {
                    int fIndex = 1;

                    foreach (var file in fileNames) {
                        File.Copy(file, "C:\\Temp\\Отчет с биржи по брокеру " + broker + " за дату " + curDate.ToShortDateString() + " (" + fIndex + ")" + ".xml", true);

                        fIndex++;
                    }
                    message += "Найден отчет по брокеру " + broker + "\n";
                }
            }

            message += "Более отчетов не найдено";

            await MessagesService.AskDialog("Сводка по поиску", message);
            Process.Start("C:\\Temp");
        }
        #endregion

        #region Merge scans from AisAlta
        //// 26052017 merge scans from AisAlta
        //public ICommand MergeScansCmd => new DelegateCommand(MergeScans);
        //private void MergeScans() {
        //    // Get files list
        //    DirectoryInfo dirInfo = new DirectoryInfo(@"e:\Temp\ContractsScans\");
        //    FileInfo[] filesInfo = dirInfo.GetFiles();
        //    int fileCount = filesInfo.Count();
        //    int fileCheck = -1;
        //    int filesAdded = 0;

        //    // Check each
        //    foreach(var item in filesInfo) {
        //        fileCheck++;

        //        // Parse file
        //        string companyBin = item.Name.Substring(0, item.Name.IndexOf("@"));
        //        string brokerId = item.Name.Substring(item.Name.IndexOf("@") + 1, 1);
        //        string contractNumber = item.Name.Substring(item.Name.LastIndexOf("@") + 1);

        //        contractNumber = contractNumber.Substring(0, contractNumber.IndexOf("."));

        //        // Check company exist by bin
        //        var company = DataBaseClient.ReadCompany(companyBin);

        //        if(company != null) {
        //            // Check contracts in company
        //            var contracts = DataBaseClient.ReadContracts(company.id);

        //            if(contracts != null && contracts.Count > 0) {
        //                // Check broker & number in founded contracts
        //                var contract = contracts.FirstOrDefault(c => c.brokerid == Convert.ToInt32(brokerId) && c.number == contractNumber);

        //                if(contract != null) {
        //                    // Check document in contract
        //                    if(contract.document == null || contract.documentId == 0) {
        //                        // Create document
        //                        DocumentEF document = new DocumentEF() {
        //                            name = "Договор " + ServiceFunctions.CompanyRenamer(company.name) + " №" + contractNumber,
        //                            siteid = 0,
        //                            extension = item.Extension.Substring(1),
        //                            documenttypeid = 21,
        //                            number = companyBin,
        //                            date = DateTime.Now,
        //                            filesectionid = 1,
        //                            description = "Мигрировано из Аис-Альта"
        //                        };

        //                        int docId = DataBaseClient.CreateDocument(document);

        //                        if(docId > 0) {
        //                            // Update contract with documentId
        //                            contract.documentId = docId;

        //                            DataBaseClient.UpdateContractFile(contract.id, docId);

        //                            // Create folders
        //                            string companyDir = @"\\192.168.11.5\Archive\Companies\" + companyBin;

        //                            if(!Directory.Exists(companyDir)) Directory.CreateDirectory(companyDir);

        //                            // Put file in created folder
        //                            string newFileName = companyDir + @"\" + document.name + item.Extension;

        //                            File.Copy(item.FullName, newFileName, true);

        //                            // Put merged file on merge directory
        //                            File.Move(item.FullName, dirInfo.FullName + @"\Merged\" + item.Name + item.Extension);

        //                            filesAdded++;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    MessagesService.Show("OPERATION DONE", string.Format("Всего файлов {0}, проверено {1}, добавлено {2}", fileCount, fileCheck, filesAdded));
        //}
        #endregion

        #endregion

        #region Bindings
        private string _statusTxt;
        public string StatusTxt {
            get { return _statusTxt; }
            set { _statusTxt = value; RaisePropertyChangedEvent("StatusTxt"); }
        }


        private List<AuctionEF> _auctionsList;
        public List<AuctionEF> AuctionsList {
            get { return _auctionsList; }
            set { _auctionsList = value; RaisePropertyChangedEvent("AuctionsList"); }
        }


        private AuctionEF _selectedAuction;
        public AuctionEF SelectedAuction {
            get { return _selectedAuction; }
            set {
                _selectedAuction = value;

                RaisePropertyChangedEvent("SelectedAuction");
            }
        }


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


        private System.Windows.Visibility _newOrdersVis;
        public System.Windows.Visibility NewOrdersVis {
            get { return _newOrdersVis; }
            set { _newOrdersVis = value; RaisePropertyChangedEvent("NewOrdersVis"); }
        }


        private List<OrderEF> _newOrders;
        public List<OrderEF> NewOrders {
            get { return _newOrders; }
            set { _newOrders = value; RaisePropertyChangedEvent("NewOrders"); }
        }


        private OrderEF _selectedNewOrder;
        public OrderEF SelectedNewOrder {
            get { return _selectedNewOrder; }
            set { _selectedNewOrder = value; RaisePropertyChangedEvent("SelectedNewOrder"); }
        }


        private string _searchText;
        public string SearchText {
            get { return _searchText; }
            set { _searchText = value; RaisePropertyChangedEvent("SearchText"); }
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


        private List<TraderEF> _tradersList;
        public List<TraderEF> TradersList {
            get { return _tradersList; }
            set { _tradersList = value; RaisePropertyChangedEvent("TradersList"); }
        }


        private TraderEF _selectedTrader;
        public TraderEF SelectedTrader {
            get { return _selectedTrader; }
            set { _selectedTrader = value; RaisePropertyChangedEvent("SelectedTrader"); }
        }


        private string _dGInfoTxt;
        public string DGInfoTxt {
            get { return _dGInfoTxt; }
            set { _dGInfoTxt = value; RaisePropertyChangedEvent("DGInfoTxt"); }
        }


        private string _messagesTxt;
        public string MessagesTxt {
            get { return _messagesTxt; }
            set { _messagesTxt = value; RaisePropertyChangedEvent("MessagesTxt"); }
        }


        private List<SiteEF> _sitesList;
        public List<SiteEF> SitesList {
            get { return _sitesList; }
            set { _sitesList = value; RaisePropertyChangedEvent("SitesList"); }
        }


        private SiteEF _selectedSite;
        public SiteEF SelectedSite {
            get { return _selectedSite; }
            set {
                _selectedSite = value;

                if (value != null) UpdateAuctionsList(FromDate, ToDate, value.id, SelectedStatus.id, SelectedTrader.id);
                if (value.id == 5) {
                    var newAuctions = DataBaseClient.ReadAuctions(FromDate, ToDate, value.id, 1, 1);

                    if (newAuctions != null && newAuctions.Count > 0) MessagesService.Show("Новые аукционы", "Есть новые аукционы. Кол-во - " + newAuctions.Count.ToString() + ". Не забудьте проверить.");
                }

                RaisePropertyChangedEvent("SelectedSite");
            }
        }
        #endregion
    }
}