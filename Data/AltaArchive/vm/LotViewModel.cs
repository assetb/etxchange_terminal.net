using System;
using System.Collections.Generic;
using System.Linq;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using AltaDock.vm;
using AltaMySqlDB.model.tables;
using AltaTransport;
using AltaLog;

namespace AltaArchive.vm {
    public class LotViewModel : PanelViewModelBase {
        #region Variables  
        private int _auctionId;
        private AuctionViewModel auctionViewModel;
        #endregion


        #region Methods
        public LotViewModel(int auctionId, AuctionViewModel auctionViewModel, LotEF lotInfo = null) {
            this.auctionViewModel = auctionViewModel;
            _auctionId = auctionId;

            if(lotInfo != null) {
                FormTitle = "Просмотр/редактирование лота";
                Lot = lotInfo;
            } else FormTitle = "Создание лота";

            DefaultParametrs(Lot);
        }


        private void DefaultParametrs(LotEF lot = null, bool refresh = false) {
            if(!refresh) {
                UnitList = DataBaseClient.ReadUnits();
            }

            if(lot == null) {
                Lot = new LotEF();
                Lot.auctionid = _auctionId;
                Lot.number = DataBaseClient.ReadAuction(_auctionId).siteid == 4 ? "0G" : "";
                Lot.deliveryplace = "Согласно договору";
                Lot.deliverytime = "Согласно договору";
                Lot.paymentterm = "Согласно договору";
                Lot.step = 1;
                Lot.warranty = 0.1;
                SelectedUnit = UnitList[0];
            } else {
                try {
                    SelectedUnit = UnitList.FirstOrDefault(x => x.id == lot.unitid);
                } catch(Exception) {
                    SelectedUnit = UnitList[0];
                }

                Quantity = Lot.amount;
                Price = Lot.price;
                Sum = Lot.sum;
                Lot = Lot;
            }
        }


        protected override List<CommandViewModel> CreateCommands() {
            return new List<CommandViewModel> {
                new CommandViewModel("Сохранить", new DelegateCommand(Save)),
                new CommandViewModel("Отмена", new DelegateCommand(Cancel))
            };
        }


        private void Save() {
            AppJournal.Write("Lot", "Save", true);

            try {
                if(Lot.filelistid == null) Lot.filelistid = DataBaseClient.CreateFileList(new FilesListEF() { description = "Файлы лота" });

                if(Lot.id != 0) {
                    DataBaseClient.UpdateLot(Lot);
                } else {
                    UpdateView(DataBaseClient.CreateLot(Lot));
                }

                auctionViewModel.UpdateLotList();
                MessagesService.Show("Сохранение лота", "Лот успешно сохранен");
                Workspace.This.Panels.Remove(Workspace.This.ActiveDocument);
            } catch(Exception ex) {
                MessagesService.Show("Сохранение лота", "Произошла ошибка во время сохранения");
                AppJournal.Write("Lot", "Saving error :" + ex.ToString(), true);
            }
        }


        private void UpdateView(int id) {
            AppJournal.Write("Lot", "Update view", true);

            try {
                Lot = DataBaseClient.ReadLot(id);
            } catch(Exception ex) { AppJournal.Write("Lot", "Get lot from db error :" + ex.ToString(), true); }

            FormTitle = "Просмотр/редактирование лота";
            DefaultParametrs(Lot, true);
        }


        private void Cancel() {
            Workspace.This.Panels.Remove(Workspace.This.ActiveDocument);
        }
        #endregion


        #region Bindings
        private string _formTitle;
        public string FormTitle {
            get { return _formTitle; }
            set { _formTitle = value; RaisePropertyChangedEvent("FormTitle"); }
        }


        private List<UnitEF> _unitList;
        public List<UnitEF> UnitList {
            get { return _unitList; }
            set { _unitList = value; RaisePropertyChangedEvent("UnitList"); }
        }


        private UnitEF _selectedUnit;
        public UnitEF SelectedUnit {
            get { return _selectedUnit; }
            set { _selectedUnit = value; Lot.unitid = value.id; RaisePropertyChangedEvent("SelectedUnit"); }
        }


        private LotEF _lot;
        public LotEF Lot {
            get { return _lot; }
            set { _lot = value; RaisePropertyChangedEvent("Lot"); }
        }


        private decimal _quantity;
        public decimal Quantity {
            get { return _quantity; }
            set {
                _quantity = value;
                Sum = value * Price;
                Lot.amount = value;
                RaisePropertyChangedEvent("Quantity");
            }
        }


        private decimal _price;
        public decimal Price {
            get { return _price; }
            set {
                _price = value;
                Sum = value * Quantity;
                Lot.price = value;
                RaisePropertyChangedEvent("Price");
            }
        }


        private decimal _sum;
        public decimal Sum {
            get { return _sum; }
            set { _sum = value; Lot.sum = value; RaisePropertyChangedEvent("Sum"); }
        }
        #endregion
    }
}