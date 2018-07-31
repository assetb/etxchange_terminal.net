using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using AltaBO.reports;
using AltaReportsApp.services;
using System.Windows.Input;

namespace AltaReportsUI.vm
{
    public class DebtorDetailsVM : PanelViewModelBase
    {
        #region Variables
        private DebtorsList debtorsList;
        #endregion

        #region Methods
        public DebtorDetailsVM(DebtorsList debtorsList)
        {
            Init(debtorsList);
        }


        private void Init(DebtorsList debtorsList)
        {
            this.debtorsList = debtorsList;

            DebtorName = debtorsList.companyName.ToUpper();
            DebtorTelephones = debtorsList.telephones;
            DebtorAddress = debtorsList.address;

            IsPay = false;
        }


        private void UpdateDebtorDetailsView()
        {
            DebtorDetailsList = CalculatingService.GetDebtorDetails(debtorsList, IsPay ? 10 : 9);
        }


        public ICommand DebtPlusCmd => new DelegateCommand(() => ChangeDebt(true));
        public ICommand DebtMinusCmd => new DelegateCommand(() => ChangeDebt(false));
        private void ChangeDebt(bool isDebtor)
        {
            if (SelectedDebtorDetails != null) {
                ReportManager.UpdateDebtorStatus(SelectedDebtorDetails, isDebtor);
                UpdateDebtorDetailsView();
            }
        }


        protected override List<CommandViewModel> CreateCommands()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Bindings
        private List<DebtorDetails> _debtorDetailsList;
        public List<DebtorDetails> DebtorDetailsList {
            get { return _debtorDetailsList; }
            set { _debtorDetailsList = value; RaisePropertyChangedEvent("DebtorDetailsList"); }
        }


        private DebtorDetails _selectedDebtorDetails;
        public DebtorDetails SelectedDebtorDetails {
            get { return _selectedDebtorDetails; }
            set { _selectedDebtorDetails = value; RaisePropertyChangedEvent("SelectedDebtorDetails"); }
        }


        private string _debtorName;
        public string DebtorName {
            get { return _debtorName; }
            set { _debtorName = value; RaisePropertyChangedEvent("DebtorName"); }
        }


        private string _debtorTelephones;
        public string DebtorTelephones {
            get { return _debtorTelephones; }
            set { _debtorTelephones = value; RaisePropertyChangedEvent("DebtorTelephones"); }
        }


        private string _debtorAddress;
        public string DebtorAddress {
            get { return _debtorAddress; }
            set { _debtorAddress = value; RaisePropertyChangedEvent("DebtorAddress"); }
        }


        private bool _isPay;
        public bool IsPay {
            get { return _isPay; }
            set {
                _isPay = value;

                UpdateDebtorDetailsView();
                RaisePropertyChangedEvent("IsPay");
            }
        }
        #endregion
    }
}
