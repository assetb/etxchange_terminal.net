using System;
using System.Collections.Generic;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using AltaBO;
using AltaTransport;
using System.Diagnostics;
using AltaTransport.specifics;

namespace DocumentFormation.vm
{
    internal class InstructionAttachToDealViewModel : PanelViewModelBase
    {
        private OurCompaniesConsts ourCompaniesConsts;
        private List<JournalC01Company> journalC01Alta;
        private List<JournalC01Company> journalC01Altk;
        private List<JournalC01Company> journalC01Kord;
        private List<JournalC01Company> journalC01Akal;
        private StockDealInfo stockDealInfo;


        public InstructionAttachToDealViewModel()
        {
            LotCode = "0G";
            ourCompaniesConsts = new OurCompaniesConsts();
            MemberCodes = ourCompaniesConsts.GetListOurCompanies();

            //JournalC01CompaniesService journalC01CompaniesService = new JournalC01CompaniesService();
            journalC01Kord = new List<JournalC01Company>(JournalC01CompaniesService.GetCompanies("KORD"));
            journalC01Alta = new List<JournalC01Company>(JournalC01CompaniesService.GetCompanies("ALTA"));
            journalC01Altk = new List<JournalC01Company>(JournalC01CompaniesService.GetCompanies("ALTK"));
            journalC01Akal = new List<JournalC01Company>(JournalC01CompaniesService.GetCompanies("AKAL"));
            //journalC01CompaniesService.CloseService();

            Traders = ConstantData.Traders;
            Trader = Traders[0];

            DealRegDate = DateTime.Now;
            ComDateOut = DealRegDate;
            ComDateIn = Convert.ToDateTime(ComDateOut.ToShortDateString() + " 11:00");
        }


        protected override List<CommandViewModel> CreateCommands()
        {
            return new List<CommandViewModel> {
                new CommandViewModel("Сформировать", new DelegateCommand(p=>OnMakeInstructionAttachToDeal()))
            };
        }


        private void OnMakeInstructionAttachToDeal()
        {
            // Fill class with data
            stockDealInfo = new StockDealInfo();
            stockDealInfo.DealRegDate = DealRegDate;
            stockDealInfo.LotCode = LotCode;
            stockDealInfo.MemberName = MemberCode.name;
            stockDealInfo.MemberCode = MemberCode.code;
            stockDealInfo.SupplierName = SupplierName;
            stockDealInfo.SupplierCode = SupplierCode;
            stockDealInfo.Employe = Trader.fullName;
            stockDealInfo.ComDateIn = ComDateIn;
            stockDealInfo.ComDateOut = ComDateOut;

            // Path to copy new documents
            var wherePath = Service.GetDirectory();
            if (wherePath != null) {

                var instructionAttachToDealService = new InstructionAttachToDealService(wherePath.FullName, stockDealInfo);

                // Copy template documents
                instructionAttachToDealService.CopyTemplates();

                // Get info for attach to stock deal from sources and paste
                instructionAttachToDealService.GetInfoForAttachDocument();

                // Get info for commission to deal from sources and paste
                instructionAttachToDealService.GetInfoForCommissionDocument();

                // Open folder with new documents
                Process.Start("explorer", wherePath.FullName);
            }
        }


        private List<OurCompany> memberCodes;
        public List<OurCompany> MemberCodes {
            get { return memberCodes; }
            set { memberCodes = value; RaisePropertyChangedEvent("MemberCodes"); }
        }


        private OurCompany memberCode;
        public OurCompany MemberCode {
            get { return memberCode; }
            set {
                if (memberCode != value) {
                    memberCode = value;

                    switch (memberCode.code.ToUpper()) {
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

                    RaisePropertyChangedEvent("MemberCode");
                }
            }
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
                journalC01CompanyInfo = value; SupplierCode = value.code; RaisePropertyChangedEvent("JournalC01CompanyInfo");
            }
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


        private List<Trader> traders;
        public List<Trader> Traders {
            get { return traders; }
            set { traders = value; RaisePropertyChangedEvent("Traders"); }
        }


        private Trader trader;
        public Trader Trader {
            get { return trader; }
            set { trader = value; RaisePropertyChangedEvent("Trader"); }
        }


        private DateTime dealRegDate;
        public DateTime DealRegDate {
            get { return dealRegDate; }
            set { dealRegDate = value; RaisePropertyChangedEvent("DealRegDate"); }
        }


        private string lotCode;
        public string LotCode {
            get { return lotCode; }
            set { lotCode = value; RaisePropertyChangedEvent("LotCode"); }
        }


        private DateTime comDateOut;
        public DateTime ComDateOut {
            get { return comDateOut; }
            set { comDateOut = value; RaisePropertyChangedEvent("ComDateOut"); }
        }


        private DateTime comDateIn;
        public DateTime ComDateIn {
            get { return comDateIn; }
            set { comDateIn = value; RaisePropertyChangedEvent("ComDateIn"); }
        }
    }
}
