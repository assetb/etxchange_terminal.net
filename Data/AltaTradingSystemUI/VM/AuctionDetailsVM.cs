using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using AltaBO;
using AltaTradingSystemUI.VM.UtilitsVM;
using System.Windows.Input;
using AltaDock.vm;
using AltaTradingSystemApp.Services;

namespace AltaTradingSystemUI.VM
{
    public class AuctionDetailsVM : PanelViewModelBase
    {
        #region Variables
        private Order order;
        #endregion

        #region Methods
        public AuctionDetailsVM(Auction auction = null, Order order = null)
        {
            if (auction != null) Auction = auction;
            else Auction = new Auction() { Id = 0, Number = "Новый", Date = DateTime.Now, Site = "", Customer = "" };

            if (order != null) this.order = order;
            else this.order = new Order() { id = 0, Number = "Новый", Date = DateTime.Now };

            Init();
        }


        private void Init()
        {
            OrderDetailsVM = new OrderDetailsVM(Auction, order);
            DatesRegulationVM = new DatesRegulationVM(Auction);
            AuctionPrimaryDataVM = new AuctionPrimaryDataVM(Auction);
            LotsListVM = new LotsListVM(Auction);
            SupplierOrdersListVM = new SupplierOrdersListVM(Auction);
        }


        public ICommand SaveCmd => new DelegateCommand(Save);
        private void Save()
        {
            Auction = AuctionPrimaryDataVM.Auction;
            Auction.Date = DatesRegulationVM.Order.Auction.Date;

            if (Auction.Id == 0) {
                if (string.IsNullOrEmpty(Auction.Number)) {
                    MessagesService.Show("Сохранение аукциона", "Аукцион не может быть сохранен, так как не имеет номера");
                    return;
                }

                Auction.OwnerId = 1;
                Auction.signStatusId = 1;

                try {
                    Auction.RegulationId = DatesRegulationVM.CreateRegulation();
                } catch { MessagesService.Show("Сохранение аукциона", "Произошла ошибка при сохранении дат"); return; }

                try {
                    Auction.FilesListId = DocumentService.CreateFilesList("Файлы аукциона №" + Auction.Number);
                } catch { MessagesService.Show("Сохранение аукциона", "Произошла ошибка во время занесения данных аукциона"); return; }

                try {
                    Auction.Id = AuctionService.CreateAuction(Auction);
                } catch { MessagesService.Show("Сохранение аукциона", "Произошла ошибка при сохранении аукциона"); return; }

                if (OrderDetailsVM.Order.id != 0) {
                    // Update order
                } else {
                    // Create order
                    order.customerid = Auction.CustomerId;
                    order.auctionId = Auction.Id;
                    order.statusId = 4;
                    order.Number = Auction.Number;
                    order.siteId = Auction.SiteId;
                    order.Date = DatesRegulationVM.Order.Date;

                    try {
                        order.filesListId = DocumentService.CreateFilesList("Файлы заявки №" + order.Number);
                    } catch { MessagesService.Show("Сохранение аукциона", "Произошла ошибка во время занесения данных заявки"); return; }

                    try {
                        order.id = AuctionService.CreateOrder(order);
                    } catch { MessagesService.Show("Сохранение аукциона", "Произошла ошибка при создании заявки"); return; }
                }

                Init();
            } else {
                try {
                    DatesRegulationVM.UpdateRegulation(Auction.RegulationId);
                } catch { MessagesService.Show("Сохранение аукциона", "Произошла ошибка при сохранении дат"); return; }

                try {
                    AuctionService.UpdateAuction(Auction);
                } catch { MessagesService.Show("Сохранение аукциона", "Произошла ошибка при сохранении аукциона"); return; }
            }

            MessagesService.Show("Сохранение аукциона", "Аукциона успешно сохранен");
        }


        public ICommand CancelCmd => new DelegateCommand(Cancel);
        private void Cancel()
        {
            Workspace.This.Panels.Remove(Workspace.This.ActiveDocument);
        }


        protected override List<CommandViewModel> CreateCommands()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Bindings
        private Auction _auction;
        public Auction Auction {
            get { return _auction; }
            set { _auction = value; RaisePropertyChangedEvent("Auction"); }
        }


        private DatesRegulationVM _datesRegulationVM;
        public DatesRegulationVM DatesRegulationVM {
            get { return _datesRegulationVM; }
            set { _datesRegulationVM = value; RaisePropertyChangedEvent("DatesRegulationVM"); }
        }


        private AuctionPrimaryDataVM _auctionPrimaryDataVM;
        public AuctionPrimaryDataVM AuctionPrimaryDataVM {
            get { return _auctionPrimaryDataVM; }
            set { _auctionPrimaryDataVM = value; RaisePropertyChangedEvent("AuctionPrimaryDataVM"); }
        }


        private LotsListVM _lotsListVM;
        public LotsListVM LotsListVM {
            get { return _lotsListVM; }
            set { _lotsListVM = value; RaisePropertyChangedEvent("LotsListVM"); }
        }


        private SupplierOrdersListVM _supplierOrdersListVM;
        public SupplierOrdersListVM SupplierOrdersListVM {
            get { return _supplierOrdersListVM; }
            set { _supplierOrdersListVM = value; RaisePropertyChangedEvent("SupplierOrdersListVM"); }
        }


        private OrderDetailsVM _orderDetailsVM;
        public OrderDetailsVM OrderDetailsVM {
            get { return _orderDetailsVM; }
            set { _orderDetailsVM = value; RaisePropertyChangedEvent("OrderDetailsVM"); }
        }
        #endregion
    }
}