namespace DocumentFormation.vm
{
    public class InvoiceViewModel_old //: PanelViewModelBase
    {
        /*private OurCompaniesConsts ourCompaniesConsts;
        private List<JournalC01Company> journalC01Alta;
        private List<JournalC01Company> journalC01Altk;
        private List<JournalC01Company> journalC01Kord;
        private List<JournalC01Company> journalC01Akal;


        public InvoiceViewModel_old()
        {
            ourCompaniesConsts = new OurCompaniesConsts();
            BrokerInfo = ourCompaniesConsts.GetListOurCompanies();

            JournalC01CompaniesService journalC01CompaniesService = new JournalC01CompaniesService();
            journalC01Kord = new List<JournalC01Company>(journalC01CompaniesService.GetCompanies("KORD"));
            journalC01Alta = new List<JournalC01Company>(journalC01CompaniesService.GetCompanies("ALTA"));
            journalC01Altk = new List<JournalC01Company>(journalC01CompaniesService.GetCompanies("ALTK"));
            journalC01Akal = new List<JournalC01Company>(journalC01CompaniesService.GetCompanies("AKAL"));
            journalC01CompaniesService.CloseService();
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


        private string auctionNumber;
        public string AuctionNumber {
            get { return auctionNumber; }
            set { auctionNumber = value; RaisePropertyChangedEvent("AuctionNumber"); }
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
                journalC01CompanyInfo = value; RaisePropertyChangedEvent("JournalC01CompanyInfo");
            }
        }


        private string invoiceSum;
        public string InvoiceSum {
            get { return invoiceSum; }
            set { invoiceSum = value; RaisePropertyChangedEvent("InvoiceSum"); }
        }


        protected override List<CommandViewModel> CreateCommands()
        {
            return new List<CommandViewModel> {
                new CommandViewModel("Счет на оплату ГО", new DelegateCommand(p=>OnWarrantyInvoiceCreate())),
                new CommandViewModel("Счет на оплату", new DelegateCommand(p=>OnInvoiceCreate()))
            };
        }


        private Invoice invoice;
        private void OnWarrantyInvoiceCreate()
        {
            GetInvoiceInfo();

            invoice.payCode = "859";

            InvoiceService invoiceService = new InvoiceService();
            invoiceService.CreateInvoice(invoice, 1);
        }


        private void OnInvoiceCreate()
        {
            GetInvoiceInfo();

            invoice.payCode = "171";

            InvoiceService invoiceService = new InvoiceService();
            invoiceService.CreateInvoice(invoice, 2);
        }


        private void GetInvoiceInfo()
        {
            invoice = new Invoice();
            invoice.brokerCode = SelBrokerName.code;
            invoice.brokerBIN = SelBrokerName.bin;

            switch (SelBrokerName.code.ToUpper()) {
                case "ALTA":
                    invoice.brokerId = "33452";
                    break;
                case "ALTK":
                    invoice.brokerId = "142";
                    break;
                case "KORD":
                    invoice.brokerId = "99";
                    break;
                case "AKAL":
                    invoice.brokerId = "311962";
                    break;
            }

            invoice.invoiceDate = DateTime.Now.ToString("dd.MM.yyyy");
            invoice.buyerBIN = JournalC01CompanyInfo.bin;
            invoice.invoiceName = "Гарантийное обеспечение за участие в аукционе";
            invoice.invoiceSum = InvoiceSum;
            invoice.auctionNumber = AuctionNumber;
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
                }
                searchSupplier = value;
                RaisePropertyChangedEvent("SearchSupplier");
            }
        }*/
    }
}
