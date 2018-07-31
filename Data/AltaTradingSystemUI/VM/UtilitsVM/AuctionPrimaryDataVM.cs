using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using altaik.baseapp.vm;
using AltaBO;
using AltaTradingSystemApp.Services;

namespace AltaTradingSystemUI.VM.UtilitsVM
{
    public class AuctionPrimaryDataVM : BaseViewModel
    {
        #region Variables
        #endregion

        #region Methods
        public AuctionPrimaryDataVM(Auction auction)
        {
            Auction = auction;

            Init();
        }


        private void Init()
        {
            SectionsList = DictionariesService.ReadSections();
            TypesList = DictionariesService.ReadTypes();
            StatusesList = DictionariesService.ReadStatuses().Where(s => s.Id < 5).ToList();
            SourcesList = DictionariesService.ReadSites();
            CustomersList = DictionariesService.ReadCustomers();
            BrokersList = DictionariesService.ReadBrokers();
            TradersList = DictionariesService.ReadTraders().OrderBy(t => t.id).ToList();

            Init(Auction.Id == 0 ? true : false);
        }


        private void Init(bool isDefault)
        {
            if (isDefault) {
                SelectedSection = SectionsList[0];
                SelectedType = TypesList[0];
                SelectedStatus = StatusesList[0];
                SelectedSource = SourcesList[0];
                SelectedCustomer = CustomersList[0];
                SelectedBroker = BrokersList[0];
                SelectedTrader = TradersList[0];
            } else {
                SelectedSection = SectionsList.First(s => s.id == Auction.sectionId);
                SelectedType = TypesList.First(t => t.id == Auction.typeId);
                SelectedStatus = StatusesList.First(s => s.Id == Auction.StatusId);
                SelectedSource = SourcesList.First(s => s.id == Auction.SiteId);
                SelectedCustomer = CustomersList.First(c => c.id == Auction.CustomerId);
                SelectedBroker = BrokersList.First(b => b.Id == Auction.BrokerId);
                SelectedTrader = TradersList.First(t => t.id == Auction.TraderId);
            }
        }
        #endregion

        #region Bindings
        private Auction _auction;
        public Auction Auction {
            get { return _auction; }
            set { _auction = value; RaisePropertyChangedEvent("Auction"); }
        }


        private List<Section> _sectionsList;
        public List<Section> SectionsList {
            get { return _sectionsList; }
            set { _sectionsList = value; RaisePropertyChangedEvent("SectionsList"); }
        }


        private Section _selectedSection;
        public Section SelectedSection {
            get { return _selectedSection; }
            set { _selectedSection = value; Auction.sectionId = value.id; RaisePropertyChangedEvent("SelectedSection"); }
        }


        private List<AuctionType> _typesList;
        public List<AuctionType> TypesList {
            get { return _typesList; }
            set { _typesList = value; RaisePropertyChangedEvent("TypesList"); }
        }


        private AuctionType _selectedType;
        public AuctionType SelectedType {
            get { return _selectedType; }
            set { _selectedType = value; Auction.typeId = value.id; RaisePropertyChangedEvent("SelectedType"); }
        }


        private List<Status> _statusesList;
        public List<Status> StatusesList {
            get { return _statusesList; }
            set { _statusesList = value; RaisePropertyChangedEvent("StatusesList"); }
        }


        private Status _selectedStatus;
        public Status SelectedStatus {
            get { return _selectedStatus; }
            set { _selectedStatus = value; Auction.StatusId = value.Id; RaisePropertyChangedEvent("SelectedStatus"); }
        }


        private List<Site> _sourcesList;
        public List<Site> SourcesList {
            get { return _sourcesList; }
            set { _sourcesList = value; RaisePropertyChangedEvent("SourcesList"); }
        }


        private Site _selectedSource;
        public Site SelectedSource {
            get { return _selectedSource; }
            set { _selectedSource = value; Auction.SiteId = value.id; RaisePropertyChangedEvent("SelectedSource"); }
        }


        private List<Customer> _customersList;
        public List<Customer> CustomersList {
            get { return _customersList; }
            set { _customersList = value; RaisePropertyChangedEvent("CustomersList"); }
        }


        private Customer _selectedCustomer;
        public Customer SelectedCustomer {
            get { return _selectedCustomer; }
            set { _selectedCustomer = value; Auction.CustomerId = value.id; RaisePropertyChangedEvent("SelectedCustomer"); }
        }


        private List<Broker> _brokersList;
        public List<Broker> BrokersList {
            get { return _brokersList; }
            set { _brokersList = value; RaisePropertyChangedEvent("BrokersList"); }
        }


        private Broker _selectedBroker;
        public Broker SelectedBroker {
            get { return _selectedBroker; }
            set { _selectedBroker = value; Auction.BrokerId = value.Id; RaisePropertyChangedEvent("SelectedBroker"); }
        }


        private List<Trader> _tradersList;
        public List<Trader> TradersList {
            get { return _tradersList; }
            set { _tradersList = value; RaisePropertyChangedEvent("TradersList"); }
        }


        private Trader _selectedTrader;
        public Trader SelectedTrader {
            get { return _selectedTrader; }
            set { _selectedTrader = value; Auction.TraderId = value.id; RaisePropertyChangedEvent("SelectedTrader"); }
        }
        #endregion
    }
}
