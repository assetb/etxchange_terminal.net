using System.Collections.Generic;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using AltaMySqlDB.model.tables;
using AltaTransport;

namespace DocumentFormation.vm {
    public class InvoiceViewModel : PanelViewModelBase {
        public InvoiceViewModel() {
            AuctionsList = DataBaseClient.ReadAuctionsForUtb(4);
        }


        protected override List<CommandViewModel> CreateCommands() {
            return new List<CommandViewModel> {
                new CommandViewModel("Сформировать счет", new DelegateCommand(p=>CreateInvoice()))
            };
        }


        private void CreateInvoice() {
            //InvoiceService.CreateInvoice(WarrantyCB, SelectedApplicant);
        }


        #region Bindings
        private List<AuctionEF> _auctionsList;
        public List<AuctionEF> AuctionsList {
            get { return _auctionsList; }
            set { _auctionsList = value; RaisePropertyChangedEvent("AuctionsList"); }
        }


        private AuctionEF _selectedAuction;
        public AuctionEF SelectedAuction {
            get { return _selectedAuction; }
            set { _selectedAuction = value; ApplicantsList = DataBaseClient.ReadApplicants(SelectedAuction.id); RaisePropertyChangedEvent("SelectedAuction"); }
        }


        private List<ApplicantEF> _applicantsList;
        public List<ApplicantEF> ApplicantsList {
            get { return _applicantsList; }
            set { _applicantsList = value; RaisePropertyChangedEvent("ApplicantsList"); }
        }


        private ApplicantEF _selectedApplicants;
        public ApplicantEF SelectedApplicant {
            get { return _selectedApplicants; }
            set { _selectedApplicants = value; RaisePropertyChangedEvent("SelectedApplicant"); }
        }


        private bool _warrantyCB;
        public bool WarrantyCB {
            get { return _warrantyCB; }
            set { _warrantyCB = value; RaisePropertyChangedEvent("WarrantyCB"); }
        }
        #endregion
    }
}
