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
    public class AuctionsResultVM : BaseViewModel
    {
        #region Variables
        #endregion

        #region Methods
        public AuctionsResultVM() { }


        public ICommand UpdateCmd => new DelegateCommand(Update);
        private void Update()
        {
            AuctionsResultList = ReportManager.ReadAuctionsResult();
        }
        #endregion

        #region Bindings
        private List<AuctionResult> _auctionsResultList;
        public List<AuctionResult> AuctionsResultList {
            get { return _auctionsResultList; }
            set { _auctionsResultList = value; RaisePropertyChangedEvent("AuctionsResultList"); }
        }
        #endregion
    }
}
