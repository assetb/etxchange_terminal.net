using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using AltaBO;
using AltaTradingSystemApp.Services;

namespace AltaTradingSystemUI.VM
{
    public class ProcuratoriesListVM : BaseViewModel
    {
        #region Methods
        public ProcuratoriesListVM(Auction auction) {
            Init(auction);
        }


        private void Init(Auction auction)
        {
            ProcuratoryDetailsVM = new ProcuratoryDetailsVM(auction);
        }


        public void UpdateListView(int auctionId, int supplierId)
        {
            ProcuratoriesList = SupplierOrderService.ReadProcuratories(auctionId, supplierId);
        }
        #endregion

        #region Bindings
        private List<Procuratory> _procuratoriesList;
        public List<Procuratory> ProcuratoriesList {
            get { return _procuratoriesList; }
            set { _procuratoriesList = value; RaisePropertyChangedEvent("ProcuratoriesList"); }
        }


        private Procuratory _selectedProcuratory;
        public Procuratory SelectedProcuratory {
            get { return _selectedProcuratory; }
            set {
                _selectedProcuratory = value;

                if (value != null) ProcuratoryDetailsVM.UpdateProcuratory(value.Id);

                RaisePropertyChangedEvent("SelectedProcuratory");
            }
        }


        private ProcuratoryDetailsVM _procuratoryDetailsVM;
        public ProcuratoryDetailsVM ProcuratoryDetailsVM {
            get { return _procuratoryDetailsVM; }
            set { _procuratoryDetailsVM = value; RaisePropertyChangedEvent("ProcuratoryDetailsVM"); }
        }
        #endregion
    }
}
