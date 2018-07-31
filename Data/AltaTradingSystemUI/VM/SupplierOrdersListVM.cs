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
using System.Windows;
using System.Windows.Input;
using AltaDock.vm;

namespace AltaTradingSystemUI.VM
{
    public class SupplierOrdersListVM : BaseViewModel
    {
        #region Variables
        private Auction auction;
        #endregion

        #region Methods
        public SupplierOrdersListVM(Auction auction)
        {
            this.auction = auction;

            Init();
        }


        private void Init()
        {
            SupplierOrderDetailsVis = Visibility.Collapsed;
            SupplierOrderDetailsVM = new SupplierOrderDetailsVM(new SupplierOrderView() { id = 0 });
            ProcuratoriesListVM = new ProcuratoriesListVM(auction);

            if (auction.Id == 0) {
            } else {
                SupplierOrdersList = SupplierOrderService.ReadSupplierOrders(auction.Id);
            }

        }


        public ICommand UpdateSupplierOrderCmd => new DelegateCommand(UpdateSupplierOrder);
        private void UpdateSupplierOrder()
        {
            if (SelectedSupplierOrder == null) {
                MessagesService.Show("Обновление заявки на участие", "Заявка на участие не выбрана");
                return;
            }

            SupplierOrderDetailsVM.UpdateSupplierOrderView(SelectedSupplierOrder.id);
            SupplierOrderDetailsVis = Visibility.Visible;
        }
        #endregion

        #region Bindings
        private List<SupplierOrderView> _supplierOrdersList;
        public List<SupplierOrderView> SupplierOrdersList {
            get { return _supplierOrdersList; }
            set { _supplierOrdersList = value; RaisePropertyChangedEvent("SupplierOrdersList"); }
        }


        private SupplierOrderView _selectedSupplierOrder;
        public SupplierOrderView SelectedSupplierOrder {
            get { return _selectedSupplierOrder; }
            set {
                _selectedSupplierOrder = value;

                if (value != null) ProcuratoriesListVM.UpdateListView(value.auctionId, value.supplierId);
                if (value != null && SupplierOrderDetailsVis == Visibility.Visible) UpdateSupplierOrder();

                RaisePropertyChangedEvent("SelectedSupplierOrder");
            }
        }


        private SupplierOrderDetailsVM _supplierOrderDetailsVM;
        public SupplierOrderDetailsVM SupplierOrderDetailsVM {
            get { return _supplierOrderDetailsVM; }
            set { _supplierOrderDetailsVM = value; RaisePropertyChangedEvent("SupplierOrderDetailsVM"); }
        }


        private Visibility _supplierOrderDetailsVis;
        public Visibility SupplierOrderDetailsVis {
            get { return _supplierOrderDetailsVis; }
            set { _supplierOrderDetailsVis = value; RaisePropertyChangedEvent("SupplierOrderDetailsVis"); }
        }


        private ProcuratoriesListVM _procuratoriesListVM;
        public ProcuratoriesListVM ProcuratoriesListVM {
            get { return _procuratoriesListVM; }
            set { _procuratoriesListVM = value;RaisePropertyChangedEvent("ProcuratoriesListVM"); }
        }
        #endregion
    }
}
