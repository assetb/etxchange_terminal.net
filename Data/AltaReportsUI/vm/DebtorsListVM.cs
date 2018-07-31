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
using AltaReportsUI.view;
using AltaDock.vm;

namespace AltaReportsUI.vm
{
    public class DebtorsListVM : BaseViewModel
    {
        #region Variables
        #endregion

        #region Methods
        public DebtorsListVM()
        {
            Init();
        }


        private void Init()
        {
            DebtorsList = ReportManager.ReadDebtorsList();
        }


        public ICommand OpenDebtorCmd => new DelegateCommand(OpenDebtor);
        private void OpenDebtor()
        {
            var debtorDetailsVM = new DebtorDetailsVM(SelectedDebtor) { Description = string.Format("Дебитор: {0}", SelectedDebtor.companyName) };
            var debtorDetailsView = new DebtorDetailsView();
            debtorDetailsVM.View = debtorDetailsView;

            Workspace.This.Panels.Add(debtorDetailsVM);
            Workspace.This.ActiveDocument = debtorDetailsVM;
        }


        public ICommand SearchCmd => new DelegateCommand(Search);
        private void Search()
        {
            if (string.IsNullOrEmpty(SearchCompany)) SearchCompany = "";
            if (string.IsNullOrEmpty(SearchCustomer)) SearchCustomer = "";

            DebtorsList = ReportManager.ReadDebtorsList(SearchCustomer, SearchCompany);            
        }
        #endregion

        #region Bindings
        private List<DebtorsList> _debtorsList;
        public List<DebtorsList> DebtorsList {
            get { return _debtorsList; }
            set { _debtorsList = value; RaisePropertyChangedEvent("DebtorsList"); }
        }


        private DebtorsList _selectedDebtor;
        public DebtorsList SelectedDebtor {
            get { return _selectedDebtor; }
            set { _selectedDebtor = value; RaisePropertyChangedEvent("SelectedDebtor"); }
        }


        private string _searchCompany;
        public string SearchCompany {
            get { return _searchCompany; }
            set { _searchCompany = value; RaisePropertyChangedEvent("SearchCompany"); }
        }


        private string _searchCustomer;
        public string SearchCustomer {
            get { return _searchCustomer; }
            set { _searchCustomer = value; RaisePropertyChangedEvent("SearchCustomer"); }
        }
        #endregion
    }
}
