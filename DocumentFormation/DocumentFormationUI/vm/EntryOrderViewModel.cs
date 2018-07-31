using System;
using System.Collections.Generic;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using AltaBO;
using System.Windows.Input;
using System.Collections.ObjectModel;
using AltaTransport;
using System.Diagnostics;

namespace DocumentFormation.vm
{
    public class EntryOrderViewModel : PanelViewModelBase
    {
        private OurCompaniesConsts ourCompaniesConsts;
        private List<JournalC01Company> journalC01Alta;
        private List<JournalC01Company> journalC01Altk;
        private List<JournalC01Company> journalC01Kord;
        private List<JournalC01Company> journalC01Akal;

        public EntryOrderViewModel()
        {
            ourCompaniesConsts = new OurCompaniesConsts();
            ClientBrokers = ourCompaniesConsts.GetListOurCompanies();

            //JournalC01CompaniesService journalC01CompaniesService = new JournalC01CompaniesService();
            journalC01Kord = new List<JournalC01Company>(JournalC01CompaniesService.GetCompanies("KORD"));
            journalC01Alta = new List<JournalC01Company>(JournalC01CompaniesService.GetCompanies("ALTA"));
            journalC01Altk = new List<JournalC01Company>(JournalC01CompaniesService.GetCompanies("ALTK"));
            journalC01Akal = new List<JournalC01Company>(JournalC01CompaniesService.GetCompanies("AKAL"));
            //journalC01CompaniesService.CloseService();

            FromDate = DateTime.Now;
            LotNumber = "0G";
        }


        public ICommand AddDocCmd {
            get { return new DelegateCommand(AddDoc); }
        }
        private void AddDoc()
        {
            ReqDocList.Add(new RequestedDoc { name = ReqDocTxt });
            ReqDocTxt = "";
        }


        public ICommand DelDocCmd {
            get { return new DelegateCommand(DelDoc); }
        }
        private void DelDoc()
        {
            ReqDocList.Remove(ReqDoc);
        }


        public ICommand EditDocCmd {
            get { return new DelegateCommand(EditDoc); }
        }
        private void EditDoc()
        {
            foreach (var item in ReqDocList) {
                if (item.name.Equals(tempText)) {
                    item.name = ReqDocTxt;
                }
            }
        }


        public ICommand CleanDocCmd {
            get { return new DelegateCommand(CleanDoc); }
        }
        private void CleanDoc()
        {
            ReqDocTxt = "";
        }


        protected override List<CommandViewModel> CreateCommands()
        {
            return new List<CommandViewModel> {
                new CommandViewModel("Сформировать", new DelegateCommand(p=>OnMakeEntryOrder()))
            };
        }


        private void OnMakeEntryOrder()
        {
            // Where need to save new entry order
            var wherePath = Service.GetDirectory();
            if (wherePath != null) {

                // Put data to model
                var entryOrder = new EntryOrder();
                entryOrder.fromDate = FromDate.ToShortDateString();
                //entryOrder.brokerClient = ClientBroker.name;
                entryOrder.lotNumber = LotNumber;
                entryOrder.memberCode = ClientBroker.code;
                entryOrder.fullMemberName = ClientBroker.name;
                entryOrder.clientCode = JournalC01CompanyInfo.code;
                entryOrder.clientFullName = "";
                entryOrder.clientAddress = "";
                entryOrder.clientBIN = JournalC01CompanyInfo.bin;
                entryOrder.clientPhones = "";
                entryOrder.clientBankIIK = "";
                entryOrder.clientBankBIK = "";
                entryOrder.clientBankName = "";
                entryOrder.requestedDocs = new List<RequestedDoc>(ReqDocList);

                // Give model and path to service
                var entryOrderService = new EntryOrderService(entryOrder, wherePath.FullName);
                entryOrderService.CopyTemplateToEndPath();

                // Fill and save template
                entryOrderService.FillFileAndSave();

                // Close and show folder
                Process.Start("explorer", wherePath.FullName);
            }
        }


        private DateTime fromDate;
        public DateTime FromDate {
            get { return fromDate; }
            set { fromDate = value; RaisePropertyChangedEvent("FromDate"); }
        }


        private List<OurCompany> clientBrokers;
        public List<OurCompany> ClientBrokers {
            get { return clientBrokers; }
            set { clientBrokers = value; RaisePropertyChangedEvent("ClientBrokers"); }
        }


        private OurCompany clientBroker;
        public OurCompany ClientBroker {
            get { return clientBroker; }
            set {
                if (clientBroker != value) {
                    clientBroker = value;

                    switch (clientBroker.code.ToUpper()) {
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
                    RaisePropertyChangedEvent("ClientBroker");
                }
            }
        }


        private string lotNumber;
        public string LotNumber {
            get { return lotNumber; }
            set { lotNumber = value; RaisePropertyChangedEvent("LotNumber"); }
        }


        private string reqDocTxt;
        public string ReqDocTxt {
            get { return reqDocTxt; }
            set { reqDocTxt = value; RaisePropertyChangedEvent("ReqDocTxt"); }
        }


        private ObservableCollection<RequestedDoc> reqDocList = new ObservableCollection<RequestedDoc>();
        public ObservableCollection<RequestedDoc> ReqDocList {
            get { return reqDocList; }
            set { reqDocList = value; RaisePropertyChangedEvent("ReqDocList"); }
        }


        private string tempText = "";

        private RequestedDoc reqDoc;
        public RequestedDoc ReqDoc {
            get { return reqDoc; }
            set { reqDoc = value; ReqDocTxt = value.name; tempText = ReqDocTxt; RaisePropertyChangedEvent("ReqDoc"); }
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
