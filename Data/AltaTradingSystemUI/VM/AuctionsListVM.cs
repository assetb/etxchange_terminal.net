using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using AltaBO;
using AltaTradingSystemApp.Services;
using System.Windows.Input;
using AltaTradingSystemUI.View;
using AltaDock.vm;

namespace AltaTradingSystemUI.VM
{
    public class AuctionsListVM : BaseViewModel
    {
        #region Variables
        #endregion

        #region Methods
        public AuctionsListVM()
        {
            Init();
        }


        private void Init()
        {
            // Default parameters
            FromDate = DateTime.Now.AddDays(-10);
            ToDate = DateTime.Now.AddDays(10);
            StatusesList = DictionariesService.ReadStatuses().Where(s => s.Id < 5).ToList();
            SelectedStatus = StatusesList[3];

            // Load orders
            OrdersList = AuctionService.ReadOrders(1);

            // Load auctions
            UpdateAuctionsList();
        }


        public ICommand CreateAuctionCmd => new DelegateCommand(() => OpenAuction(true));
        public ICommand UpdateAuctionCmd => new DelegateCommand(() => OpenAuction(false));
        private void OpenAuction(bool isNew)
        {
            if (SelectedAuction == null && !isNew) return;

            var auctionDetailsVM = new AuctionDetailsVM(isNew ? null : SelectedAuction) { Description = isNew ? "Новый" : SelectedAuction.Number };
            var auctionDetailsView = new AuctionDetailsView();
            auctionDetailsVM.View = auctionDetailsView;

            Workspace.This.Panels.Add(auctionDetailsVM);
            Workspace.This.ActiveDocument = auctionDetailsVM;
        }


        public ICommand ApplyCmd => new DelegateCommand(UpdateAuctionsList);
        private void UpdateAuctionsList()
        {
            AuctionsList = AuctionService.ReadAuctions(FromDate, ToDate, SelectedStatus.Id);
        }

        #endregion

        #region Bindings
        private List<Order> _ordersList;
        public List<Order> OrdersList {
            get { return _ordersList; }
            set { _ordersList = value; RaisePropertyChangedEvent("OrdersList"); }
        }


        private Order _selectedOrder;
        public Order SelectedOrder {
            get { return _selectedOrder; }
            set { _selectedOrder = value; RaisePropertyChangedEvent("SelectedOrder"); }
        }


        private List<Auction> _auctionsList;
        public List<Auction> AuctionsList {
            get { return _auctionsList; }
            set { _auctionsList = value; RaisePropertyChangedEvent("AuctionsList"); }
        }


        private Auction _selectedAuction;
        public Auction SelectedAuction {
            get { return _selectedAuction; }
            set { _selectedAuction = value; RaisePropertyChangedEvent("SelectedAuction"); }
        }


        private DateTime _fromDate;
        public DateTime FromDate {
            get { return _fromDate; }
            set { _fromDate = value; RaisePropertyChangedEvent("FromDate"); }
        }


        private DateTime _toDate;
        public DateTime ToDate {
            get { return _toDate; }
            set { _toDate = value; RaisePropertyChangedEvent("ToDate"); }
        }


        private List<Status> _statusesList;
        public List<Status> StatusesList {
            get { return _statusesList; }
            set { _statusesList = value; RaisePropertyChangedEvent("StatusesList"); }
        }


        private Status _selectedStatus;
        public Status SelectedStatus {
            get { return _selectedStatus; }
            set { _selectedStatus = value; RaisePropertyChangedEvent("SelectedStatus"); }
        }
        #endregion
    }
}
