using System;
using System.Collections.Generic;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using AltaBO;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace DocumentFormation.vm
{
    public class MoneyTransferViewModel : PanelViewModelBase
    {
        private OurCompaniesConsts ourCompaniesConsts;
        private List<JournalC01Company> journalC01Alta;
        private List<JournalC01Company> journalC01Altk;
        private List<JournalC01Company> journalC01Kord;
        private List<JournalC01Company> journalC01Akal;


        public MoneyTransferViewModel()
        {
            ourCompaniesConsts = new OurCompaniesConsts();
            BrokerInfo = ourCompaniesConsts.GetListOurCompanies();

            //JournalC01CompaniesService journalC01CompaniesService = new JournalC01CompaniesService();
            journalC01Kord = new List<JournalC01Company>(JournalC01CompaniesService.GetCompanies("KORD"));
            journalC01Alta = new List<JournalC01Company>(JournalC01CompaniesService.GetCompanies("ALTA"));
            journalC01Altk = new List<JournalC01Company>(JournalC01CompaniesService.GetCompanies("ALTK"));
            journalC01Akal = new List<JournalC01Company>(JournalC01CompaniesService.GetCompanies("AKAL"));
            //journalC01CompaniesService.CloseService();
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


        private List<JournalC01Company> journalC01Companies;
        public List<JournalC01Company> JournalC01Companies {
            get { return journalC01Companies; }
            set { journalC01Companies = value; RaisePropertyChangedEvent("JournalC01Companies"); }
        }


        private JournalC01Company journalC01CompanyInfo;
        public JournalC01Company JournalC01CompanyInfo {
            get { return journalC01CompanyInfo; }
            set {
                journalC01CompanyInfo = value; SupplierCode = value.code; SupplierName = value.name; RaisePropertyChangedEvent("JournalC01CompanyInfo");
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


        private string lotNumber;
        public string LotNumber {
            get { return lotNumber; }
            set { lotNumber = value; RaisePropertyChangedEvent("LotNumber"); }
        }


        private string sum;
        public string Sum {
            get { return sum; }
            set { sum = value; RaisePropertyChangedEvent("Sum"); }
        }


        public ICommand AddCmd {
            get { return new DelegateCommand(AddRecord); }
        }

        private decimal dSum;

        private void AddRecord()
        {
            dSum = Convert.ToDecimal(sum) / 100 * Convert.ToDecimal(0.1);
            MoneyTransferList.Add(new MoneyTransferList {
                brokerName = SelBrokerName.name,
                fromCompany = SelBrokerName.c01,
                toCompany = SupplierCode,
                lotNumber = LotNumber,
                sum = Convert.ToString(Math.Round(dSum, 2))
            });
        }


        public ICommand DeleteCmd {
            get { return new DelegateCommand(DeleteRecord); }
        }
        private void DeleteRecord()
        {
            MoneyTransferList.Remove(SelMoneyTransferList);
        }


        private ObservableCollection<MoneyTransferList> moneyTransferList = new ObservableCollection<MoneyTransferList>();
        public ObservableCollection<MoneyTransferList> MoneyTransferList {
            get { return moneyTransferList; }
            set { moneyTransferList = value; RaisePropertyChangedEvent("MoneyTransferList"); }
        }


        private MoneyTransferList selMoneyTransferList;
        public MoneyTransferList SelMoneyTransferList {
            get { return selMoneyTransferList; }
            set { selMoneyTransferList = value; RaisePropertyChangedEvent("SelMoneyTransferList"); }
        }


        protected override List<CommandViewModel> CreateCommands()
        {
            return new List<CommandViewModel> {
                new CommandViewModel("Сформировать заявление", new DelegateCommand(p=>OnFormingDoc())),
                new CommandViewModel("Изменить заявление", new DelegateCommand(p=>OnChangeDoc()))
            };
        }


        private void OnFormingDoc()
        {
            var moneyTransferService = new MoneyTransferService_old();
            moneyTransferService.StartFill(MoneyTransferList);
        }


        private void OnChangeDoc()
        {
            var moneyTransferService = new MoneyTransferService_old();
            moneyTransferService.ChangeMoneyDirection();
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
    }
}
