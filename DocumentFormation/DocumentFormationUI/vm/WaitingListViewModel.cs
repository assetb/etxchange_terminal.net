using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using AltaBO;
using System.Collections.ObjectModel;
using System.Data;
using AltaTransport;
using System.Text.RegularExpressions;

namespace DocumentFormation.vm
{
    internal class WaitingListViewModel : PanelViewModelBase
    {
        private OurCompaniesConsts ourCompaniesConsts;
        private WaitingList waitingList;
        private List<JournalC01Company> journalC01Alta;
        private List<JournalC01Company> journalC01Altk;
        private List<JournalC01Company> journalC01Kord;
        private List<JournalC01Company> journalC01Akal;
        private MSSQLTransport msSQLTransport;
        private DataSet dataSet;

        public WaitingListViewModel()
        {
            ourCompaniesConsts = new OurCompaniesConsts();
            BrokerInfo = ourCompaniesConsts.GetListOurCompanies();

            //JournalC01CompaniesService journalC01CompaniesService = new JournalC01CompaniesService();
            journalC01Kord = new List<JournalC01Company>(JournalC01CompaniesService.GetCompanies("KORD"));
            journalC01Alta = new List<JournalC01Company>(JournalC01CompaniesService.GetCompanies("ALTA"));
            journalC01Altk = new List<JournalC01Company>(JournalC01CompaniesService.GetCompanies("ALTK"));
            journalC01Akal = new List<JournalC01Company>(JournalC01CompaniesService.GetCompanies("AKAL"));
            //journalC01CompaniesService.CloseService();

            waitingList = new WaitingList();

            OrderDate = DateTime.Now;
            LotCode = "0G";
        }


        private string orderNumber;
        public string OrderNumber {
            get { return orderNumber; }
            set { orderNumber = value; RaisePropertyChangedEvent("OrderNumber"); }
        }


        private string sourceNumber;
        public string SourceNumber {
            get { return sourceNumber; }
            set { sourceNumber = value; RaisePropertyChangedEvent("SourceNumber"); }
        }


        private DateTime orderDate;
        public DateTime OrderDate {
            get { return orderDate; }
            set { orderDate = value; RaisePropertyChangedEvent("OrderDate"); }
        }


        private string lotCode;
        public string LotCode {
            get { return lotCode; }
            set { lotCode = value; RaisePropertyChangedEvent("LotCode"); }
        }


        private List<JournalC01Company> journalC01Companies;
        public List<JournalC01Company> JournalC01Companies {
            get { return journalC01Companies; }
            set { journalC01Companies = value; RaisePropertyChangedEvent("JournalC01Companies"); }
        }


        private JournalC01Company journalC01CompanyInfo;
        public JournalC01Company JournalC01CompanyInfo {
            get { return journalC01CompanyInfo; }
            set { journalC01CompanyInfo = value; SupplierCode = value.code; RaisePropertyChangedEvent("JournalC01CompanyInfo"); }
        }


        private string sName;

        private string searchSupplier;
        public string SearchSupplier {
            get { return searchSupplier; }
            set {
                sName = string.Empty;
                sName = JournalC01Companies.Find(x => x.name.ToLower().Contains(value.ToLower())).name;

                if (!string.IsNullOrEmpty(sName)) {
                    JournalC01CompanyInfo = JournalC01Companies.Find(x => x.name.ToLower().Contains(value.ToLower()));
                    SupplierName = JournalC01CompanyInfo.name;
                    SupplierCode = JournalC01CompanyInfo.code;
                }
                searchSupplier = value;
                RaisePropertyChangedEvent("SearchSupplier");
            }
        }


        private string supplierCode;
        public string SupplierCode {
            get { return supplierCode; }
            set { supplierCode = value; RaisePropertyChangedEvent("SupplierCode"); }
        }


        private string supplierName;
        public string SupplierName {
            get { return supplierName; }
            set { supplierName = value; RaisePropertyChangedEvent("SupplierName"); }
        }
        

        private string clientName;
        public string ClientName {
            get { return clientName; }
            set { clientName = value; RaisePropertyChangedEvent("ClientName"); }
        }


        private string clientCode;
        public string ClientCode {
            get { return clientCode; }
            set { clientCode = value; RaisePropertyChangedEvent("ClientCode"); }
        }


        private List<OurCompany> brokerInfo;
        public List<OurCompany> BrokerInfo {
            get { return brokerInfo; }
            set { brokerInfo = value; RaisePropertyChangedEvent("BrokerInfo"); }
        }


        private OurCompany selBrokerName;
        public OurCompany SelBrokerName {
            get { return selBrokerName; }
            set {
                if (selBrokerName != value) {
                    selBrokerName = value;

                    switch (selBrokerName.code.ToUpper()) {
                        case "ALTA":
                            JournalC01Companies = journalC01Alta;
                            break;
                        case "ALTK":
                            JournalC01Companies = journalC01Altk;
                            break;
                        case "KORD":
                            JournalC01Companies = journalC01Kord;
                            break;
                        case "AKAL":
                            JournalC01Companies = journalC01Akal;
                            break;
                    }
                    BrokerCode = value.code;
                    BrokerName = value.name;
                    RaisePropertyChangedEvent("SelBrokerName");
                }
            }
        }


        private string brokerName;
        public string BrokerName {
            get { return brokerName; }
            set { brokerName = value; RaisePropertyChangedEvent("BrokerName"); }
        }


        private string brokerCode;
        public string BrokerCode {
            get { return brokerCode; }
            set { brokerCode = value; RaisePropertyChangedEvent("BrokerCode"); }
        }


        public ICommand AddCmd {
            get { return new DelegateCommand(AddRecord); }
        }
        private void AddRecord()
        {
            msSQLTransport = new MSSQLTransport();
            var regex = new Regex(@"[\d*]{1,}");

            string req = "", loc = "";
            var cBIN = JournalC01CompanyInfo.bin.Replace(" ", "");
            long bankId;

            cBIN = regex.Match(cBIN).ToString();            

            if(!string.IsNullOrEmpty(cBIN)) dataSet = msSQLTransport.Execute("select top 1 a.* from altair.dbo.companiesView a where a.bin like '%" + cBIN + "%'");

            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count>0) {
                loc = dataSet.Tables[0].Rows[0].Field<string>("addrJur");
                bankId = dataSet.Tables[0].Rows[0].Field<long>("bankId");
                req = "БИН " + cBIN + "\n";
                req += "БИК " + dataSet.Tables[0].Rows[0].Field<string>("bik") + "\n";
                req += "Кбе " + dataSet.Tables[0].Rows[0].Field<string>("kbe") + "\n";
                req += "ИИК " + dataSet.Tables[0].Rows[0].Field<string>("iik") + "\n";

                dataSet = msSQLTransport.Execute("select a.* from altair.dbo.banksView a where a.id='" + bankId + "'");
                if (dataSet != null && dataSet.Tables.Count > 0) req += "Банк " + dataSet.Tables[0].Rows[0].Field<string>("bankName") + "\n";
            }else {
                req = "Данных в базе не обнаружено";               
            }

            WaitingListTable.Add(new WaitingListTable {
                company = "Код клиента: " + SupplierCode + "\n" + SupplierName + "\n Код брокера: " + BrokerCode + "\n" + BrokerName,
                bankReq = req,
                location = loc
            });
        }


        public ICommand DeleteCmd {
            get { return new DelegateCommand(DeleteRecord); }
        }
        private void DeleteRecord()
        {
            WaitingListTable.Remove(SelWaitingListTable);
        }


        private ObservableCollection<WaitingListTable> waitingListTable = new ObservableCollection<WaitingListTable>();
        public ObservableCollection<WaitingListTable> WaitingListTable {
            get { return waitingListTable; }
            set { waitingListTable = value; RaisePropertyChangedEvent("WaitingListTable"); }
        }


        private WaitingListTable selWaitingListTable;
        public WaitingListTable SelWaitingListTable {
            get { return selWaitingListTable; }
            set { selWaitingListTable = value; RaisePropertyChangedEvent("SelWaitingListTable"); }
        }


        protected override List<CommandViewModel> CreateCommands()
        {
            return new List<CommandViewModel> {
                new CommandViewModel("Сформировать список", new DelegateCommand(p=>OnWaitingListForClient()))
            };
        }


        private void OnWaitingListForClient()
        {
            waitingList.lotCode = LotCode;
            waitingList.orderDate = OrderDate.ToShortDateString();
            waitingList.orderNumber = OrderNumber;
            waitingList.sourceNumber = SourceNumber;
            waitingList.waitingListTable = WaitingListTable;

            var waitingListService = new WaitingListService(waitingList);
            waitingListService.StartFillList();
        }
    }
}
