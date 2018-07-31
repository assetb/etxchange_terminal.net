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
    public class ProcuratoryDetailsVM : BaseViewModel
    {
        #region Methods
        public ProcuratoryDetailsVM(Auction auction) {
            Init(auction);
        }


        private void Init(Auction auction)
        {
            Procuratory = new Procuratory();

            if (auction.Id == 0) LotsList = new List<Lot>();
            else LotsList = LotService.ReadLots(auction.Id);
        }


        public void UpdateProcuratory(int procuratoryId)
        {
            Procuratory = SupplierOrderService.ReadProcuratory(procuratoryId);
        }
        #endregion

        #region Bindings
        private Procuratory _procuratory;
        public Procuratory Procuratory {
            get { return _procuratory; }
            set { _procuratory = value;RaisePropertyChangedEvent("Procuratory"); }
        }


        private List<Lot> _lotsList;
        public List<Lot> LotsList {
            get { return _lotsList; }
            set { _lotsList = value; RaisePropertyChangedEvent("LotsList"); }
        }


        private Lot _selectedLot;
        public Lot SelectedLot {
            get { return _selectedLot; }
            set {
                _selectedLot = value;

                if (value != null) Procuratory.lotId = value.Id;

                RaisePropertyChangedEvent("SelectedLot");
            }
        }
        #endregion
    }
}
