using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using AltaBO;
using AltaTradingSystemApp.Services;
using System.Windows;
using System.Windows.Input;
using AltaDock.vm;

namespace AltaTradingSystemUI.VM
{
    public class LotsListVM : BaseViewModel
    {
        #region Variables
        private Auction auction;
        #endregion

        #region Methods
        public LotsListVM(Auction auction)
        {
            this.auction = auction;

            Init();
        }


        private void Init()
        {
            LotDetailsVis = Visibility.Collapsed;
            LotDetailsVM = new LotDetailsVM(new Lot() { Id = 0 });

            if (auction.Id == 0) {
            } else {
                LotsList = LotService.ReadLots(auction.Id);
            }
        }


        public ICommand CreateLotCmd => new DelegateCommand(CreateLot);
        private void CreateLot()
        {
            if (auction.Id == 0) {
                MessagesService.Show("Создание лота", "Сначала необходимо сохранить аукцион");
                return;
            }

            SelectedLot = new Lot() { Id = 0 };
            UpdateLot();
        }


        public ICommand UpdateLotCmd => new DelegateCommand(UpdateLot);
        private void UpdateLot()
        {
            if (SelectedLot == null) {
                MessagesService.Show("Обновление лота", "Лот не выбран");
                return;
            }

            LotDetailsVM.UpdateLotView(SelectedLot.Id);
            LotDetailsVis = Visibility.Visible;
        }


        public ICommand DeleteLotCmd => new DelegateCommand(DeleteLot);
        private void DeleteLot()
        {
            if (SelectedLot == null || SelectedLot.Id == 0) {
                MessagesService.Show("Удаление лота", "Лот не выбран или не существует");
                return;
            }

            LotService.DeleteLot(SelectedLot.Id);
            Init();
        }


        public ICommand SaveLotCmd => new DelegateCommand(SaveLot);
        private void SaveLot()
        {
            SelectedLot = LotDetailsVM.Lot;
            SelectedLot.auctionId = auction.Id;

            // Check for filled fields
            if (string.IsNullOrEmpty(SelectedLot.Number) || string.IsNullOrEmpty(SelectedLot.Name) || SelectedLot.Quantity == 0 || SelectedLot.Price == 0) {
                MessagesService.Show("Сохранение лота", "Не все поля заполненны");
                return;
            }

            if (SelectedLot.Id == 0) {
                int newLotId = LotService.CreateLot(SelectedLot);
            } else {
                LotService.UpdateLot(SelectedLot);
            }

            Init();
        }


        public ICommand CancelLotCmd => new DelegateCommand(CancelLot);
        private void CancelLot()
        {
            LotDetailsVis = Visibility.Collapsed;
        }
        #endregion

        #region Bindings
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

                if (value != null && value.Id != 0) TechSpecList = LotService.ReadLotExtended(value.Id);
                if (LotDetailsVis == Visibility.Visible && value != null) UpdateLot();

                RaisePropertyChangedEvent("SelectedLot");
            }
        }


        private LotDetailsVM _lotDetailsVM;
        public LotDetailsVM LotDetailsVM {
            get { return _lotDetailsVM; }
            set { _lotDetailsVM = value; RaisePropertyChangedEvent("LotDetailsVM"); }
        }


        private Visibility _lotDetailsVis;
        public Visibility LotDetailsVis {
            get { return _lotDetailsVis; }
            set { _lotDetailsVis = value; RaisePropertyChangedEvent("LotDetailsVis"); }
        }


        private List<LotsExtended> _techSpecList;
        public List<LotsExtended> TechSpecList {
            get { return _techSpecList; }
            set { _techSpecList = value; RaisePropertyChangedEvent("TechSpecList"); }
        }
        #endregion
    }
}
