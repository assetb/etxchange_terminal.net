using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using System.Windows.Input;
using AltaBO;
using AltaTransport;
using AltaDock.vm;
using AltaMySqlDB.model.tables;
using AltaMySqlDB.model.views;

namespace AltaArchive.vm {
    public class DebtorsReportViewModel : BaseViewModel {
        #region Variables
        #endregion

        #region Methods
        public DebtorsReportViewModel() {
            DefaultParametrs();
        }

        private void DefaultParametrs() {
            IsDropDown = false;
            BrokersList = DataBaseClient.ReadBrokers();

            StartDate = DateTime.Now.AddYears(-1);
            EndDate = DateTime.Now;
        }

        public ICommand ApplyCmd { get { return new DelegateCommand(() => ApplyQuery(0)); } }
        public ICommand ApplySelectedCmd { get { return new DelegateCommand(() => ApplyQuery(1)); } }

        private void ApplyQuery(int mode) {
            if(SelectedBroker.id != 3) {
                List<DebtorReport> debtorReport = new List<DebtorReport>();
                string[] clientBins = new string[] { };

                if(mode == 0) clientBins = DataBaseClient.GetSuppliersBins(SelectedBroker.id);

                try {
                    if(mode == 0) debtorReport = _1CTransport.GetDebtors(SelectedBroker.id == 4 ? 3 : SelectedBroker.id, StartDate, EndDate, clientBins);
                    else debtorReport = _1CTransport.GetDebtors(SelectedBroker.id == 4 ? 3 : SelectedBroker.id, StartDate, EndDate, SelectedSupplier.companyBin);
                } catch(Exception) { }

                FullDebt = "0";

                // Fill from database
                foreach(var item in debtorReport) {
                    try {
                        item.clientName = DataBaseClient.GetCompanyName(item.clientBIN);
                        FullDebt = (Convert.ToDecimal(FullDebt) + item.debit).ToString();
                    } catch(Exception) { }
                }

                DebtorsList = debtorReport.OrderByDescending(d => d.result).ToList();
            } else MessagesService.Show("Оповещение", "По этому брокеру 1С недоступна");
        }
        #endregion

        #region Bindings
        private DateTime _startDate;
        public DateTime StartDate {
            get { return _startDate; }
            set { _startDate = value; RaisePropertyChangedEvent("StartDate"); }
        }

        private DateTime _endDate;
        public DateTime EndDate {
            get { return _endDate; }
            set { _endDate = value; RaisePropertyChangedEvent("EndDate"); }
        }

        private List<DebtorReport> _debtorsList;
        public List<DebtorReport> DebtorsList {
            get { return _debtorsList; }
            set { _debtorsList = value; RaisePropertyChangedEvent("DebtorsList"); }
        }

        private List<BrokerEF> _brokersList;
        public List<BrokerEF> BrokersList {
            get { return _brokersList; }
            set {
                _brokersList = value;
                SelectedBroker = value[1];
                RaisePropertyChangedEvent("BrokersList");
            }
        }

        private BrokerEF _selectedBroker;
        public BrokerEF SelectedBroker {
            get { return _selectedBroker; }
            set {
                _selectedBroker = value;
                SuppliersList = DataBaseClient.GetSuppliersWithContract(/*value.id*/);
                SelectedSupplier = SuppliersList[0];
                RaisePropertyChangedEvent("SelectedBroker");
            }
        }

        private string _fullDebt;
        public string FullDebt {
            get { return _fullDebt; }
            set { _fullDebt = value; RaisePropertyChangedEvent("FullDebt"); }
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

        private string _searchTxt;
        public string SearchTxt {
            get { return _searchTxt; }
            set {
                _searchTxt = value;
                if(value.Length > 2) {
                    SuppliersList = DataBaseClient.GetSuppliersWithContract(/*SelectedBroker.id*/).Where(s => s.companyName.ToLower().Contains(value.ToLower())).ToList();
                    if(SuppliersList.Count > 0 && SuppliersList.Count < 10) IsDropDown = true;
                }
                RaisePropertyChangedEvent("SearchTxt");
            }
        }

        private bool _isDropDown;
        public bool IsDropDown {
            get { return _isDropDown; }
            set { _isDropDown = value; RaisePropertyChangedEvent("IsDropDown"); }
        }
        #endregion
    }
}
