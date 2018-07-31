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
    public class LotDetailsVM : BaseViewModel
    {
        #region Methods
        public LotDetailsVM(Lot lot)
        {
            Lot = lot;

            Init();
        }


        private void Init()
        {
            UnitsList = DictionariesService.ReadUnits();

            UpdateLotView(Lot.Id);
        }


        public void UpdateLotView(int lotId)
        {
            if (lotId == 0) {
                Lot = new Lot() {
                    Id = 0,
                    Name = "",
                    Quantity = 0,
                    Price = 0,
                    Sum = 0,
                    DeliveryPlace = "Согласно договору",
                    DeliveryTime = "Согласно договору",
                    PaymentTerm = "Согласно договору",
                    Step = 0,
                    Warranty = 0,
                    LocalContent = 0,
                    Dks = 0,
                    ContractNumber=""                    
                };

                SelectedUnit = UnitsList[0];
            } else {
                Lot = LotService.ReadLot(lotId);
                SelectedUnit = UnitsList.FirstOrDefault(u => u.Id == Lot.UnitId);
            }
        }
        #endregion

        #region Bindings
        private Lot _lot;
        public Lot Lot {
            get { return _lot; }
            set { _lot = value; RaisePropertyChangedEvent("Lot"); }
        }


        private List<Unit> _unitsList;
        public List<Unit> UnitsList {
            get { return _unitsList; }
            set { _unitsList = value; RaisePropertyChangedEvent("UnitsList"); }
        }


        private Unit _selectedUnit;
        public Unit SelectedUnit {
            get { return _selectedUnit; }
            set {
                _selectedUnit = value;

                if (value != null) Lot.UnitId = value.Id;

                RaisePropertyChangedEvent("SelectedUnit");
            }
        }
        #endregion
    }
}
