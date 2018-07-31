using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using AltaBO;
using AltaBO.views;
using AltaTradingSystemApp.Services;

namespace AltaTradingSystemUI.VM
{
    public class SupplierOrderDetailsVM : BaseViewModel
    {
        #region Variables
        private List<Supplier> suppliersListStorage;
        #endregion

        #region Methods
        public SupplierOrderDetailsVM(SupplierOrderView supplierOrder)
        {
            SupplierOrder = supplierOrder;

            Init();
        }


        private void Init()
        {
            // Load default lists
            suppliersListStorage = DictionariesService.ReadSuppliers();
            BrokersList = DictionariesService.ReadBrokers();

            // Load supplier order
            UpdateSupplierOrderView(SupplierOrder.id);
        }


        public void UpdateSupplierOrderView(int supplierOrderId)
        {
            if (supplierOrderId == 0) {
                SupplierOrder = new SupplierOrderView() {
                    id = 0
                };

                SelectedSupplier = new Supplier();
                SelectedBroker = BrokersList[0];
                SelectedContract = new Contract();
                SelectedRate = new RatesList();
            } else {
                SupplierOrder = SupplierOrderService.ReadSupplierOrder(supplierOrderId);
                SelectedSupplier = suppliersListStorage.FirstOrDefault(s => s.Id == SupplierOrder.supplierId);
                SearchCompany = SelectedSupplier.Name;

                if (SupplierOrder.brokerId != null) SelectedBroker = BrokersList.FirstOrDefault(s => s.Id == SupplierOrder.brokerId);
                else SelectedBroker = null;

                if (SupplierOrder.brokerId != null) {
                    ContractsList = DictionariesService.ReadContracts(SelectedSupplier.companyId, SelectedBroker.Id);
                    SelectedContract = ContractsList.FirstOrDefault(c => c.id == SupplierOrder.contractId);
                } else ContractsList = new List<Contract>();                
            }
        }
        #endregion

        #region Bindings
        private SupplierOrderView _supplierOrder;
        public SupplierOrderView SupplierOrder {
            get { return _supplierOrder; }
            set { _supplierOrder = value; RaisePropertyChangedEvent("SupplierOrder"); }
        }


        private List<Supplier> _suppliersList;
        public List<Supplier> SuppliersList {
            get { return _suppliersList; }
            set { _suppliersList = value; RaisePropertyChangedEvent("SuppliersList"); }
        }


        private Supplier _selectedSupplier;
        public Supplier SelectedSupplier {
            get { return _selectedSupplier; }
            set {
                _selectedSupplier = value;

                if (value != null) SupplierOrder.supplierId = value.Id;
                if (value != null && SelectedBroker != null) ContractsList = DictionariesService.ReadContracts(value.companyId, SelectedBroker.Id);

                RaisePropertyChangedEvent("SelectedSupplier");
            }
        }


        private List<Broker> _brokersList;
        public List<Broker> BrokersList {
            get { return _brokersList; }
            set { _brokersList = value; RaisePropertyChangedEvent("BrokersList"); }
        }


        private Broker _selectedBroker;
        public Broker SelectedBroker {
            get { return _selectedBroker; }
            set {
                _selectedBroker = value;

                if (value != null) SupplierOrder.brokerId = value.Id;
                if (value != null && SelectedSupplier != null) ContractsList = DictionariesService.ReadContracts(SelectedSupplier.companyId, value.Id);

                RaisePropertyChangedEvent("SelectedBroker");
            }
        }


        private List<Contract> _contractsList;
        public List<Contract> ContractsList {
            get { return _contractsList; }
            set {
                _contractsList = value;

                if (value != null && value.Count > 0) SelectedContract = value[0];
                else SelectedContract = new Contract();

                RaisePropertyChangedEvent("ContractsList");
            }
        }


        private Contract _selectedContract;
        public Contract SelectedContract {
            get { return _selectedContract; }
            set {
                _selectedContract = value;

                if (value != null && value.id != 0) RatesList = DictionariesService.ReadRatesLists(value.id);
                else RatesList = new List<AltaBO.RatesList>();

                RaisePropertyChangedEvent("SelectedContract");
            }
        }


        private List<RatesList> _ratesList;
        public List<RatesList> RatesList {
            get { return _ratesList; }
            set {
                _ratesList = value;

                if (value != null && value.Count > 0) SelectedRate = RatesList[0];
                else SelectedRate = new AltaBO.RatesList();

                RaisePropertyChangedEvent("RatesList");
            }
        }


        private RatesList _selectedRate;
        public RatesList SelectedRate {
            get { return _selectedRate; }
            set { _selectedRate = value; RaisePropertyChangedEvent("SelectedRate"); }
        }


        private string _searchCompany;
        public string SearchCompany {
            get { return _searchCompany; }
            set {
                _searchCompany = value;

                if (value.Length > 2) SuppliersList = suppliersListStorage.Where(s => !string.IsNullOrEmpty(s.Name) && s.Name.ToLower().Contains(value.ToLower())).ToList();

                RaisePropertyChangedEvent("SearchCompany");
            }
        }


        private List<LotsList> _lotsList;
        public List<LotsList> LotsList {
            get { return _lotsList; }
            set { _lotsList = value; RaisePropertyChangedEvent("LotsList"); }
        }
        #endregion
    }


    public class LotsList : Lot
    {
        public bool inplay { get; set; } = false;
    }
}