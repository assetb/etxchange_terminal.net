using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using AltaBO.reports;
using System.Windows.Input;
using AltaReportsApp.services;
using AltaTransport;
using AltaArchive.Services;
using AltaDock.vm;
using DocumentFormation;
using AltaBO;

namespace AltaReportsUI.vm
{
    public class DealNumberVM : BaseViewModel
    {
        #region Variables
        private List<DealNumberInfo> DealNumbersListStorage = new List<DealNumberInfo>();
        #endregion

        #region Methods
        public DealNumberVM()
        {
            Init();
        }


        private void Init()
        {
            FromDate = DateTime.Now.AddMonths(-6);
            ToDate = DateTime.Now;
            BrokersList = DictionariesService.ReadBrokers();
            SelectedBroker = BrokersList[0];
            RecordsCount = "0";
        }


        public ICommand ApplyCmd => new DelegateCommand(() => Apply(0));
        public ICommand ApplyBrokerCmd => new DelegateCommand(() => Apply(1));
        private void Apply(int type)
        {
            if (DealNumbersListStorage.Count < 1) DealNumbersListStorage = ReportManager.ReadDealNumbersInfo();

            if (string.IsNullOrEmpty(DealNumberTxt)) DealNumberTxt = "";

            DealNumbersList = DealNumbersListStorage.Where(d => d.dealNumber.ToLower().Contains(DealNumberTxt.ToLower()) && d.auctionDate >= FromDate && d.auctionDate <= ToDate &&
            (type == 1 ? d.brokerId == SelectedBroker.Id : true)).ToList();
        }


        public ICommand CalculateCmd => new DelegateCommand(Calculate);
        private void Calculate()
        {
            foreach (var item in DealNumbersList) {
                var percent = ReportManager.GetRatePercent(item.supplierId, item.auctionId, item.exchangeId, item.finalPriceOffer, item.brokerId);

                if (percent == null || percent == 0) item.debt = 0;
                else {
                    item.debt = item.finalPriceOffer / 100 * percent;

                    if (item.exchangeId == 4) item.debt += item.finalPriceOffer < 1000000 ? 500 : 5000;
                }
            }

            DealNumbersList = DealNumbersList;
        }


        public ICommand ShowAuctionCmd => new DelegateCommand(ShowAuction);
        private void ShowAuction()
        {
            if (SelectedDealNumber == null) return;


        }


        public ICommand SaveInExcelCmd => new DelegateCommand(SaveInExcel);
        private void SaveInExcel()
        {
            if (DealNumbersList.Count > 0) {
                // Variables info
                string path = "";
                string templateFile = "";
                string templatePath = @"\\192.168.11.5\Archive\Templates\ForAll\FinalReport.xlsx";

                // Get path to save template
                try {
                    path = Service.GetDirectory().FullName;
                } catch { path = ""; }

                if (!string.IsNullOrEmpty(path)) {
                    // Get template file
                    templateFile = path + "\\Финальный отчет с " + FromDate.ToShortDateString() + " по " + ToDate.ToShortDateString() + ".xlsx";

                    if (Service.CopyFile(templatePath, templateFile, true)) {

                        // Fill template file with info
                        try {
                            FinalReportService.FormateDocument(templateFile, DealNumbersList);

                            // Open folder with file   
                            FolderExplorer.OpenFolder(path + "\\");
                        } catch (Exception ex) { MessagesService.Show("Оповещение", "Произошла ошибка во время формирования отчета\n" + ex.ToString()); }
                    } else MessagesService.Show("Оповещение", "Произошла ошибка во время копирования шаблона");
                }
            } else MessagesService.Show("Оповещение", "Нет данных для формирования");
        }
        #endregion

        #region Bindings
        private List<DealNumberInfo> _dealNumbersList;
        public List<DealNumberInfo> DealNumbersList {
            get { return _dealNumbersList; }
            set {
                _dealNumbersList = value;

                if (value != null) RecordsCount = value.Count.ToString();

                RaisePropertyChangedEvent("DealNumbersList");
            }
        }


        private DealNumberInfo _selectedDealNumber;
        public DealNumberInfo SelectedDealNumber {
            get { return _selectedDealNumber; }
            set { _selectedDealNumber = value; RaisePropertyChangedEvent("SelectedDealNumber"); }
        }


        private string _dealNumberTxt;
        public string DealNumberTxt {
            get { return _dealNumberTxt; }
            set { _dealNumberTxt = value; RaisePropertyChangedEvent("DealNumberTxt"); }
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


        private string _recordsCount;
        public string RecordsCount {
            get { return _recordsCount; }
            set { _recordsCount = value;RaisePropertyChangedEvent("RecordsCount"); }
        }
        #endregion
    }
}
