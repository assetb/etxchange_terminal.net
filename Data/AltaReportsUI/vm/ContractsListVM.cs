using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using AltaBO.reports;
using System.Windows.Input;
using AltaReportsApp.services;

namespace AltaReportsUI.vm
{
    public class ContractsListVM : BaseViewModel
    {
        #region Methods
        public ContractsListVM()
        {
            Init();
        }


        private void Init()
        {
            FromDate = DateTime.Now.AddMonths(-3);
            ToDate = DateTime.Now;
        }

        public ICommand ApplyCmd => new DelegateCommand(Apply);
        private void Apply()
        {
            if (string.IsNullOrEmpty(SearchQuery)) SearchQuery = "";

            ContractsList = ReportManager.ReadContractsReport(FromDate, ToDate, SearchQuery);
        }
        #endregion

        #region Bindings
        private List<ContractsReportView> _contractsList;
        public List<ContractsReportView> ContractsList {
            get { return _contractsList; }
            set { _contractsList = value; RaisePropertyChangedEvent("ContractsList"); }
        }


        private ContractsReportView _selectedContracts;
        public ContractsReportView SelectedContract {
            get { return _selectedContracts; }
            set { _selectedContracts = value; RaisePropertyChangedEvent("SelectedContract"); }
        }


        private DateTime _fromDate;
        public DateTime FromDate {
            get { return _fromDate; }
            set { _fromDate = value;RaisePropertyChangedEvent("FromDate"); }
        }


        private DateTime _toDate;
        public DateTime ToDate {
            get { return _toDate; }
            set { _toDate = value;RaisePropertyChangedEvent("ToDate"); }
        }


        private string _searchQuery;
        public string SearchQuery {
            get { return _searchQuery; }
            set { _searchQuery = value;RaisePropertyChangedEvent("SearchQuery"); }
        }
        #endregion
    }
}
