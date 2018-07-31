using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using System.Windows.Input;
using AltaBO;
using AltaTransport;
using AltaDock.vm;
using AltaMySqlDB.model.tables;
using AltaMySqlDB.model.views;
using DocumentFormation;
using AltaArchive.Services;

namespace AltaArchive.vm {
    public class ReconcilationsReportViewModel : BaseViewModel {
        #region Variables
        #endregion

        #region Methods
        public ReconcilationsReportViewModel() {
            DefaultParametrs();
        }

        private void DefaultParametrs() {
            IsDropDown = false;
            BrokersList = DataBaseClient.ReadBrokers();

            StartDate = DateTime.Now.AddYears(-1);
            EndDate = DateTime.Now;
        }

        public ICommand ApplyCmd { get { return new DelegateCommand(ApplyQuery); } }
        private void ApplyQuery() {
            if(SelectedBroker.id != 3) {
                var supplierContract = DataBaseClient.ReadContracts(SelectedSupplier.companyId, SelectedBroker.id);

                if(supplierContract != null) {
                    try {
                        ReconcilationsList = _1CTransport.GetReconcilation(SelectedBroker.id == 4 ? 3 : SelectedBroker.id, SelectedSupplier.companyBin, SelectedSupplier.contractNumber, StartDate, EndDate);
                    } catch(Exception) { }
                } else MessagesService.Show("Оповещение", "Нет договора между брокером и поставщиком");
            } else MessagesService.Show("Оповещение", "По этому брокеру 1С недоступна");
        }

        public ICommand FormateActCmd { get { return new DelegateCommand(FormateAct); } }
        private void FormateAct() {
            if(ReconcilationsList.Count > 0) {
                // Variables info
                string path = "";
                string templateFile = "";
                string templatePath = @"\\192.168.11.5\Archive\Templates\ForAll\ReconcilationAct.docx";

                // Get path to save template
                try {
                    path = Service.GetDirectory().FullName;
                } catch { path = ""; }

                if(!string.IsNullOrEmpty(path)) {
                    // Get template file
                    templateFile = path + "\\Акт сверки для компании " + SelectedSupplier.companyName.Replace("\"", "'") + " за " + EndDate.ToShortDateString() + ".docx";

                    if(Service.CopyFile(templatePath, templateFile, true)) {
                        // Convert info
                        Broker broker = new Broker() {
                            Name = SelectedBroker.name,
                            Requisites = SelectedBroker.company.bin
                        };

                        Supplier supplier = new Supplier() {
                            Name = SelectedSupplier.companyName,
                            BIN = SelectedSupplier.companyBin
                        };

                        // Fill template file with info
                        try {
                            ReconcilationActService.FormateAct(StartDate, EndDate, broker, supplier, ReconcilationsList, templateFile);

                            // Open folder with file   
                            FolderExplorer.OpenFolder(path + "\\");
                        } catch(Exception ex) { MessagesService.Show("Оповещение", "Произошла ошибка во время формирования акта\n" + ex.ToString()); }
                    } else MessagesService.Show("Оповещение", "Произошла ошибка во время копирования шаблона");
                }
            } else MessagesService.Show("Оповещение", "Нет данных для формирования");
        }
        #endregion

        #region Bindings
        private DateTime _startDate;
        public DateTime StartDate {
            get { return _startDate; }
            set { _startDate = value; RaisePropertyChangedEvent("StartDate"); }
        }

        private DateTime _endDate;
        public DateTime EndDate {
            get { return _endDate; }
            set { _endDate = value; RaisePropertyChangedEvent("EndDate"); }
        }

        private List<ReconcilationReport> _reconcilationsList;
        public List<ReconcilationReport> ReconcilationsList {
            get { return _reconcilationsList; }
            set {
                _reconcilationsList = value;

                if(value != null) {
                    FullCredit = value.Sum(v => v.credit);
                    FullDebet = value.Sum(v => v.debit);
                    FullDifference = Math.Abs(FullCredit - FullDebet) + ((FullCredit - FullDebet) < 0 ? " в пользу Брокера" : " в пользу Поставщика");
                }

                RaisePropertyChangedEvent("ReconcilationsList");
            }
        }

        private List<BrokerEF> _brokersList;
        public List<BrokerEF> BrokersList {
            get { return _brokersList; }
            set {
                _brokersList = value;
                RaisePropertyChangedEvent("BrokersList");
            }
        }

        private BrokerEF _selectedBroker;
        public BrokerEF SelectedBroker {
            get { return _selectedBroker; }
            set {
                _selectedBroker = value;
                SuppliersList = DataBaseClient.GetSuppliersWithContract(/*value.id*/);
                SelectedSupplier = SuppliersList[0];
                RaisePropertyChangedEvent("SelectedBroker");
            }
        }

        private string _fullDebt;
        public string FullDebt {
            get { return _fullDebt; }
            set { _fullDebt = value; RaisePropertyChangedEvent("FullDebt"); }
        }

        private List<CompaniesWithContractView> _suppliersList;
        public List<CompaniesWithContractView> SuppliersList {
            get { return _suppliersList; }
            set { _suppliersList = value; RaisePropertyChangedEvent("SuppliersList"); }
        }

        private CompaniesWithContractView _selectedSupplier;
        public CompaniesWithContractView SelectedSupplier {
            get { return _selectedSupplier; }
            set { _selectedSupplier = value; RaisePropertyChangedEvent("SelectedSupplier"); }
        }

        private string _searchTxt;
        public string SearchTxt {
            get { return _searchTxt; }
            set {
                _searchTxt = value;
                if(value.Length > 1) {
                    SuppliersList = DataBaseClient.GetSuppliersWithContract(/*SelectedBroker.id*/).Where(s => s.companyName.ToLower().Contains(value.ToLower())).ToList();
                    if(SuppliersList.Count > 0 && SuppliersList.Count < 10) IsDropDown = true;
                    else IsDropDown = false;
                }
                RaisePropertyChangedEvent("SearchTxt");
            }
        }

        private bool _isDropDown;
        public bool IsDropDown {
            get { return _isDropDown; }
            set { _isDropDown = value; RaisePropertyChangedEvent("IsDropDown"); }
        }

        private decimal _fullCredit;
        public decimal FullCredit {
            get { return _fullCredit; }
            set { _fullCredit = value; RaisePropertyChangedEvent("FullCredit"); }
        }

        private decimal _fullDebet;
        public decimal FullDebet {
            get { return _fullDebet; }
            set { _fullDebet = value; RaisePropertyChangedEvent("FullDebet"); }
        }

        private string _fullDifference;
        public string FullDifference {
            get { return _fullDifference; }
            set { _fullDifference = value; RaisePropertyChangedEvent("FullDifference"); }
        }
        #endregion
    }
}
