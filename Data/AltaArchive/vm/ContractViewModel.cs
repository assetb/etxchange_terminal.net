using System;
using System.Collections.Generic;
using System.Linq;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using AltaDock.vm;
using AltaMySqlDB.model.tables;
using AltaTransport;
using System.Windows.Input;
using AltaArchive.view;
using AltaBO;
using DocumentFormation;
using AltaTransport._1c.InvoicePaiment;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using AltaLog;
using AltaArchive.Services;
using AltaArchiveApp;
using AltaMySqlDB.model;
using AltaMySqlDB.service;
using AltaBO.specifics;

namespace AltaArchive.vm
{
    public class ContractViewModel : PanelViewModelBase
    {
        #region Variables   
        private int companyId;
        private CompanyViewModel companyViewModel;
        private IDataManager dataManager = new EntityContext();
        private ArchiveManager archiveManager;
        #endregion


        #region Methods
        public ContractViewModel(int companyId, CompanyViewModel companyViewModel, ContractEF contractInfo = null)
        {
            this.companyViewModel = companyViewModel;
            this.companyId = companyId;

            if (contractInfo != null) {
                FormTitle = "Просмотр/редактирование договора";
                Contract = contractInfo;
            } else FormTitle = "Создание договора";

            DefaultParametrs(Contract);
        }


        private void DefaultParametrs(ContractEF contract = null, bool refresh = false)
        {
            if (!refresh) {
                archiveManager = new ArchiveManager(dataManager);
                BanksList = DataBaseClient.ReadBanks();
                ContractTypesList = DataBaseClient.ReadContractTypes();
                CurrenciesList = DataBaseClient.ReadCurrencies();
                BrokersList = DataBaseClient.ReadBrokers();
                AuthorsList = DataBaseClient.ReadTraders();

                RangeList = new List<string>();
                RangeList.Add("От 1 ");
                RangeList.Add("От 20.000 тсч.");
                RangeList.Add("От 1.000.000 млн.");
                RangeList.Add("От 5.000.000 млн.");
                RangeList.Add("От 10.000.000 млн.");
                RangeList.Add("От 15.000.000 млн.");
                RangeList.Add("От 25.000.000 млн.");
                RangeList.Add("От 40.000.000 млн.");
                RangeList.Add("От 50.000.000 млн.");
                RangeList.Add("От 75.000.000 млн.");
                RangeList.Add("От 100.000.000 млн.");
                RangeList.Add("От 1.000.000.000 млрд.");
                SelectedRange = RangeList[0];

                ScanTypesList = new List<string>();
                ScanTypesList.Add("Копия");
                ScanTypesList.Add("Оригинал");
            }

            if (contract == null) {
                Contract = new ContractEF();
                Contract.companyid = companyId;
                Contract.agreementdate = DateTime.Now;
                SelectedContractType = ContractTypesList[0];
                SelectedCurrency = CurrenciesList[0];
                SelectedScanType = ScanTypesList[0];
            } else {
                try {
                    if (contract.bankid != null) {
                        SelectedBank = BanksList.Where(x => x.id == contract.bankid).FirstOrDefault();
                        SearchTxt = SelectedBank.name;
                    }

                    SelectedContractType = ContractTypesList.Where(x => x.id == (contract.contracttypeid == null ? 1 : contract.contracttypeid)).FirstOrDefault();
                    SelectedCurrency = CurrenciesList.Where(x => x.id == contract.currencyid).FirstOrDefault();
                    SelectedBroker = BrokersList.Where(x => x.id == contract.brokerid).FirstOrDefault();
                    RatesList = new ObservableCollection<RatesListEF>(DataBaseClient.ReadRatesList(Contract.id));
                    SelectedAuthor = AuthorsList.FirstOrDefault(a => a.id == (contract.authorid == null ? 1 : contract.authorid));
                    SelectedScanType = ScanTypesList[(contract.scantype == null ? 0 : (int)contract.scantype)];
                } catch (Exception) { }
            }
        }


        protected override List<CommandViewModel> CreateCommands()
        {
            return new List<CommandViewModel> {
                new CommandViewModel("Сохранить", new DelegateCommand(Save)),
                new CommandViewModel("Отмена", new DelegateCommand(Cancel)),
                new CommandViewModel("Прикрепить договор",new DelegateCommand(AttachContract)),
                new CommandViewModel("Сформировать договор",new DelegateCommand(FormateContract)),
                new CommandViewModel("Взять данные из 1С", new DelegateCommand(GetContractFrom1C)),
                new CommandViewModel("Передать данные в 1С", new DelegateCommand(PutContractTo1C))
            };
        }


        private void Save()
        {
            AppJournal.Write("Contract", "Save", true);

            try {
                if (Contract.id != 0) {
                    if (AllFieldsSet()) {
                        DataBaseClient.UpdateContract(Contract);
                        MessagesService.Show("Обновление договора", "Договор обновлен");
                    } else MessagesService.Show("Обновление договора", "Не все поля заполненны");
                } else {
                    if (AllFieldsSet() && CheckForSame()) {
                        UpdateView(DataBaseClient.CreateContract(Contract));
                        MessagesService.Show("Создание договора", "Договор создан");
                    } else MessagesService.Show("Обновление договора", "Не все поля заполненны или договор существует");
                }

                companyViewModel.UpdateContractList();
                MessagesService.Show("Сохранение договора", "Не забудьте прикрепить скан договора");
            } catch (Exception ex) {
                MessagesService.Show("ОШИБКА", "Ошибка при сохранении компании");
                AppJournal.Write("Contract", "Save contract in db error :" + ex.ToString(), true);
            }
        }


        private bool AllFieldsSet()
        {
            if (Contract.agreementdate == null || Contract.bankid == null || Contract.brokerid == null || Contract.contracttypeid == null || Contract.currencyid == null) return false;
            else return true;
        }


        private bool CheckForSame()
        {
            if (!DataBaseClient.CheckDuplicateContract(Contract)) return true;
            else return false;
        }


        private void UpdateView(int id)
        {
            AppJournal.Write("Contract", "Update view", true);

            try {
                Contract = DataBaseClient.ReadContract(id);
                FormTitle = "Просмотр/редактирование договора";
                DefaultParametrs(Contract, true);
            } catch (Exception ex) {
                MessagesService.Show("Ошибка", "При обновлении вида произошла ошибка");
                AppJournal.Write("Contract", "Get contract from db error :" + ex.ToString(), true);
            }
        }


        private void Cancel() { Workspace.This.Panels.Remove(Workspace.This.ActiveDocument); }


        private BankAccounts BankAccounts;
        private Contracts Contracts;


        private void AttachContract()
        {
            string sourceFileName = "";
            string newFileName = "";

            if (Contract.id != 0) {
                try {
                    sourceFileName = Service.GetFile("Выберите файл договора", "(*.*) | *.*").FullName;
                } catch (Exception) { }

                if (!string.IsNullOrEmpty(sourceFileName)) {
                    newFileName = FolderExplorer.GetCompanyPath(Contract.company.bin);
                    newFileName += "Договор " + Contract.number + sourceFileName.Substring(sourceFileName.LastIndexOf("."));

                    if (Service.CopyFile(sourceFileName, newFileName, true)) {
                        try {
                            DocumentRequisite docReq = new DocumentRequisite() {
                                date = DateTime.Now,
                                fileName = newFileName.Substring(newFileName.LastIndexOf("\\") + 1),
                                number = Contract.company.bin,
                                section = AltaBO.specifics.DocumentSectionEnum.Company,
                                type = AltaBO.specifics.DocumentTypeEnum.Contract
                            };

                            DataBaseClient.UpdateContractFile(Contract.id, archiveManager.SaveFile(docReq));
                            MessagesService.Show("АРХИВ", "Файл скопирован в архив.");
                        } catch (Exception ex) {
                            MessagesService.Show("АРХИВ", "При занесении файла в базу произошла ошибка");
                            AppJournal.Write("Company", "Put contract into db error :" + ex.ToString(), true);
                        }
                    } else MessagesService.Show("ОШИБКА", "Проверьте доступ к архиву.");
                }
            } else MessagesService.Show("Прикрепление договора", "Договор не сохранен");
        }


        private void FormateContract()
        {
            // Prepare data
            // Get contract data
            var cContract = dataManager.ReadContracts(companyId, SelectedBroker != null ? SelectedBroker.id : 0);

            if (cContract == null || cContract.Count < 1) {
                MessagesService.Show("Формирование договора", "Нет данных договора");
                return;
            }

            Contract caspyContract = cContract.FirstOrDefault(c => c.number.ToLower() == Contract.number);

            // Get company data
            var cCompany = dataManager.GetCompany(companyId);

            if (cCompany == null) {
                MessagesService.Show("Формирование договора", "Нет данных компании");
                return;
            }

            Company caspyCompany = cCompany;
            caspyCompany.bank = Contract.bank != null ? Contract.bank.name : "";
            caspyCompany.bik = Contract.bank != null ? Contract.bank.company.bik : "";

            // Get company broker data
            var cBroker = dataManager.GetCompany(SelectedBroker.companyId);

            if (cBroker == null) {
                MessagesService.Show("Формирование договора", "Нет данных компании брокера");
                return;
            }

            Company caspyBroker = cBroker;

            var bContracts = dataManager.ReadContracts(companyId, SelectedBroker.id);

            if (bContracts == null || bContracts.Count < 1) {
                MessagesService.Show("Формирование договора", "Нет данных договора брокера");
                return;
            }

            var bContract = DataBaseClient.ReadContract(bContracts[0].id);

            if (bContract == null) {
                MessagesService.Show("Формирование договора", "Нет данных договора брокера");
                return;
            }

            caspyBroker.bank = bContract.bank != null ? bContract.bank.name : "";
            caspyBroker.bik = bContract.bank != null ? bContract.bank.company.bik : "";

            // Copy templates to end folder
            DocumentRequisite docContractReq = new DocumentRequisite() {
                date = DateTime.Now,
                fileName = "Договор " + Contract.number + ".docx",
                number = Contract.company.bin,
                section = DocumentSectionEnum.Company,
                type = DocumentTypeEnum.Contract,
                market = MarketPlaceEnum.Caspy
            };

            var contractFileName = archiveManager.GetTemplate(docContractReq, DocumentTemplateEnum.Contract);

            if (string.IsNullOrEmpty(contractFileName)) {
                MessagesService.Show("Формирование договора", "Ошибка копирования шаблона договора");
                return;
            }

            string newFileName = FolderExplorer.GetCompanyPath(Contract.company.bin);
            newFileName += docContractReq.fileName;

            if (Service.CopyFile(contractFileName, newFileName, true)) Service.DeleteFile(contractFileName);
            else {
                MessagesService.Show("Формирование договора", "Ошибка копирования шаблона договора");
                return;
            }

            if (string.IsNullOrEmpty(newFileName)) {
                MessagesService.Show("Формирование договора", "Ошибка копирования шаблона договора");
                return;
            }

            // Fill contract template with prepared data
            if (!ContractService.FillKaspiContract(newFileName, caspyContract, caspyCompany, caspyBroker)) {
                MessagesService.Show("Формирование договора", "Произошла ошибка при формировании документа");
                return;
            }

            DataBaseClient.UpdateContractFile(Contract.id, archiveManager.SaveFile(docContractReq));
        }


        private async void GetContractFrom1C()
        {
            AppJournal.Write("Contract", "Get contract from 1c", true);

            CompanyEF Company = DataBaseClient.ReadCompany(companyId);
            string contractNum = await MessagesService.GetInput("ПОИСК В 1С", "Введите номер договора");

            if (!string.IsNullOrEmpty(contractNum)) {
                try {
                    Contracts = _1CTransport.SearchContract(Company.bin, contractNum, 1);

                    if (Contracts != null) {
                        SelectedBroker = BrokersList.Where(x => x.id == 1).First();
                        BankAccounts = _1CTransport.SearchBankAccount(Company.iik, Company.bin, 1);
                    } else {
                        Contracts = _1CTransport.SearchContract(Company.bin, contractNum, 2);

                        if (Contracts != null) {
                            SelectedBroker = BrokersList.Where(x => x.id == 2).First();
                            BankAccounts = _1CTransport.SearchBankAccount(Company.iik, Company.bin, 2);
                        } else {
                            Contracts = _1CTransport.SearchContract(Company.bin, contractNum, 3);

                            if (Contracts != null) {
                                SelectedBroker = BrokersList.Where(x => x.id == 4).First();
                                BankAccounts = _1CTransport.SearchBankAccount(Company.iik, Company.bin, 3);
                            }
                        }
                    }

                    if (Contracts != null) {
                        Contract.agreementdate = Contracts.ContracDate;

                        Contract.number = Contracts.ContractNumber;

                        if (BankAccounts != null) SelectedBank = BanksList.Where(x => x.company.bik == BankAccounts.BankBIК).First();

                        if (Contracts.ContractType.ToUpper().Contains("ПРОЧЕЕ") || Contracts.ContractType.ToUpper().Contains("ПОКУПАТЕЛЕМ"))
                            SelectedContractType = ContractTypesList.Where(x => x.id == 1).First();
                        else SelectedContractType = ContractTypesList.Where(x => x.id == 2).First();

                        SelectedCurrency = CurrenciesList.Where(x => x.code == Contracts.СontractСurrency).First();

                        Contract = Contract;
                    }
                } catch (Exception ex) {
                    Contracts = null;
                    AppJournal.Write("Contract", "Get contract from 1c error :" + ex.ToString(), true);
                }
            }

            if (Contracts == null) MessagesService.Show("РЕЗУЛЬТАТ ПОИСКА В БАЗАХ", "Результатов нет");
        }


        private void PutContractTo1C()
        {
            AppJournal.Write("Contract", "Put contract to 1c", true);

            if (AllFieldsSet()) {
                string result = "";
                int baseType = 2;
                CompanyEF company = DataBaseClient.ReadCompany(companyId);
                Contracts contract = new Contracts();
                string companyBin = Contract.company.bin;

                if (_1CTransport.SearchCompany(companyBin, baseType) == null) companyBin = companyBin.PadLeft(12, '0');

                contract = _1CTransport.SearchContract(companyBin, Contract.number, baseType = (int)Contract.brokerid == 4 ? 3 : (int)Contract.brokerid);

                if (contract == null) {
                    contract = new Contracts();

                    contract.CtlgName = "Договор №" + Contract.number + " от " + Contract.agreementdate;
                    contract.CtlgCode = "";
                    contract.ClientIIN = companyBin;
                    contract.ContractNumber = Contract.number.Replace("с", "c");
                    contract.ContracDate = Contract.agreementdate;
                    contract.ContractType = "СПокупателем";
                    contract.Settlements = "";
                    contract.StartDate = null;
                    contract.TerminationDate = null;
                    contract.Comments = ".NET Автоматизация";
                    contract.СontractСurrency = Contract.currency.code;

                    result = _1CTransport.CreateContract(baseType, contract);
                } else result = "exist";

                BankAccounts bankAccount = new BankAccounts();
                bankAccount = _1CTransport.SearchBankAccount(company.iik, companyBin, baseType = Contract.brokerid == 4 ? 3 : (int)Contract.brokerid);

                if (bankAccount == null) _1CTransport.CreateBankAccount(company.iik, companyBin, Contract.currency.code, baseType = Contract.brokerid == 4 ? 3 : (int)Contract.brokerid, Contract.bank.company.bik);

                if (result.Contains("ok")) MessagesService.Show("РЕЗУЛЬТАТ ЗАНЕСЕНИЯ В 1С", "Договор создан успешно");
                else if (result.Contains("exist")) MessagesService.Show("РЕЗУЛЬТАТ ЗАНЕСЕНИЯ В 1С", "Договор с таким номером уже существует в 1С");
                else {
                    MessagesService.Show("РЕЗУЛЬТАТ ЗАНЕСЕНИЯ В 1С", "При создании договора возникла ошибка");
                    AppJournal.Write("Contract", "Put contract to 1c error :" + result, true);
                }
            } else MessagesService.Show("РЕЗУЛЬТАТ ЗАНЕСЕНИЯ В 1С", "Не все поля заполненны");
        }


        public ICommand CreateRatesListCmd { get { return new DelegateCommand(() => CRUDRatesList(1)); } }
        public ICommand UpdateRatesListCmd { get { return new DelegateCommand(() => CRUDRatesList(2)); } }
        public ICommand DeleteRatesListCmd { get { return new DelegateCommand(() => CRUDRatesList(3)); } }
        private async void CRUDRatesList(int type)
        {
            AppJournal.Write("Contract", "Open rates list - " + (type == 1 ? "New" : SelectedRatesList.name), true);

            string market = "";
            int marketType = 0;

            if (type != 3) {
                market = await MessagesService.GetInput("Выберите биржу", "ЕТС - 1 / УТБ - 2 / КазЭТС - 3 (Введите цифру):");

                if (!string.IsNullOrEmpty(market)) {
                    try {
                        marketType = Convert.ToInt32(market);
                    } catch (Exception) { }

                    if (marketType == 1) {
                        marketType = 4;
                        market = "ETS";
                    } else if (marketType == 2) {
                        marketType = 1;
                        market = "Астана";
                    } else if (marketType == 3) {
                        marketType = 5;
                        market = "КазЭТС";
                    } else marketType = 0;
                }
            }

            switch (type) {
                case 1: // Create
                    if (marketType != 0) {
                        // Fill model
                        var ratesList = new RatesListEF() {
                            contractid = Contract.id,
                            name = market,
                            siteid = marketType
                        };

                        // Save changes
                        if (DataBaseClient.CreateRatesList(ratesList) == 0) {
                            MessagesService.Show("Создание тарифной сетки", "Во время создания произошла ошибка");
                            AppJournal.Write("Contract", "Create rates list error", true);
                        }
                    }
                    break;
                case 2: // Update
                    if (SelectedRatesList != null && marketType != 0) {
                        // Get new market
                        if (DataBaseClient.UpdateRatesList(SelectedRatesList.id, market, marketType)) MessagesService.Show("Обновление тарифной сетки", "Обновление прошло успешно");
                        else {
                            MessagesService.Show("Обновление тарифной сетки", "Во время обновления произошла ошибка");
                            AppJournal.Write("Contract", "Update rates list error", true);
                        }
                    } else MessagesService.Show("Удаление тарифной сетки", "Не выбрана тарифная сетка");
                    break;
                case 3: // Delete
                    if (SelectedRatesList != null) {
                        if (DataBaseClient.DeleteRatesList(SelectedRatesList.id)) MessagesService.Show("Удаление тарифной сетки", "Удаление прошло успешно");
                        else {
                            MessagesService.Show("Удаление тарифной сетки", "Во время удаления произошла ошибка");
                            AppJournal.Write("Contract", "Delete rates list error", true);
                        }
                    } else MessagesService.Show("Удаление тарифной сетки", "Не выбрана тарифная сетка");
                    break;
            }

            UpdateRatesListView();
        }

        public ICommand UpdateRatesListViewCmd { get { return new DelegateCommand(() => UpdateRatesListView()); } }
        private void UpdateRatesListView()
        {
            AppJournal.Write("Contract", "Update rates list view", true);

            try {
                RatesList = new ObservableCollection<RatesListEF>(DataBaseClient.ReadRatesList(Contract.id));
            } catch (Exception ex) { AppJournal.Write("Contract", "Get rates list from db error :" + ex.ToString(), true); }
        }

        public ICommand AddRangeCmd { get { return new DelegateCommand(() => CRUDRate(1, SelectedRange)); } }
        public ICommand CreateRateCmd { get { return new DelegateCommand(() => CRUDRate(1)); } }
        public ICommand UpdateRateCmd { get { return new DelegateCommand(() => CRUDRate(2)); } }
        public ICommand UpdateRangeCmd { get { return new DelegateCommand(() => CRUDRate(2, SelectedRange)); } }
        public ICommand DeleteRateCmd { get { return new DelegateCommand(() => CRUDRate(3)); } }

        private void CRUDRate(int type, string range = null)
        {
            AppJournal.Write("Contract", "Open rate - " + (type == 1 ? "New" : SelectedRate.id.ToString()), true);

            string transactionTxt, procentTxt;
            decimal transaction = 0, procent = 0;
            bool error = false;

            if (SelectedRatesList != null) {
                if (type != 3) {
                    /*if(range == null) {
                        transactionTxt = await MessagesService.GetInput("Ввод диапазона сумм", "Введите суммы от и до через тире (-):");
                        procentTxt = await MessagesService.GetInput("Ввод процента", "Введите процент для заданного диапазона:");
                    } else {*/
                    transactionTxt = range.Substring(2, range.LastIndexOf(" ") - range.IndexOf(" ")).Replace(" ", "").Replace(".", "");
                    procentTxt = Percent.ToString();
                    //}

                    if (!string.IsNullOrEmpty(transactionTxt) && !string.IsNullOrEmpty(procentTxt)) {
                        try {
                            transactionTxt = transactionTxt.Replace("-", "-");
                            procentTxt = procentTxt.Replace(".", ",");
                            transaction = Convert.ToDecimal(string.IsNullOrEmpty(range) ? transactionTxt.Substring(0, transactionTxt.IndexOf("-")) : transactionTxt);
                            procent = Convert.ToDecimal(procentTxt);
                        } catch (Exception) { error = true; }
                    } else error = true;
                }

                switch (type) {
                    case 1: // Create
                        if (!error) {
                            // Fill model
                            var rate = new RateEF() {
                                transaction = transaction,
                                percent = procent,
                                rateslistid = SelectedRatesList.id
                            };

                            // Save changes
                            if (DataBaseClient.CreateRate(rate) == 0) MessagesService.Show("Создание тарифных данных", "Во время создания произошла ошибка");
                            else AppJournal.Write("Contract", "Create rate error", true);
                        }
                        break;
                    case 2: // Update
                        if (!error && SelectedRate != null) {
                            if (!DataBaseClient.UpdateRate(SelectedRate.id, transaction, procent)) MessagesService.Show("Обновление тарифных данных", "Во время обновления произошла ошибка");
                        } else {
                            MessagesService.Show("Обновление тарифных данных", "При обновлении произошла ошибка.\nВозможно не были выбраны данные");
                            AppJournal.Write("Contract", "Update rate error", true);
                        }
                        break;
                    case 3: // Delete
                        if (SelectedRate != null) {
                            if (!DataBaseClient.DeleteRate(SelectedRate.id)) {
                                MessagesService.Show("Удаление тарифных данных", "Во время удаления произошла ошибка");
                                AppJournal.Write("Contract", "Delete rate error", true);
                            }
                        } else MessagesService.Show("Удаление тарифных данных", "Не выбраны тарифные данные");
                        break;
                }

                UpdateRateView();
            } else MessagesService.Show("Тарифные данные", "Не выбрана тарифная сетка");
        }

        public ICommand UpdateRateViewCmd { get { return new DelegateCommand(() => UpdateRateView()); } }
        private void UpdateRateView()
        {
            AppJournal.Write("Contract", "Update rate list view", true);

            try {
                Rates = new ObservableCollection<RateEF>(DataBaseClient.ReadRates(SelectedRatesList.id).OrderBy(o => o.transaction));
            } catch (Exception ex) { AppJournal.Write("Contract", "Get rates from db error :" + ex.ToString(), true); }
        }
        #endregion


        #region Bindings
        private ContractEF _contract;
        public ContractEF Contract {
            get { return _contract; }
            set { _contract = value; RaisePropertyChangedEvent("Contract"); }
        }


        private String _formTitle;
        public String FormTitle {
            get { return _formTitle; }
            set { _formTitle = value; RaisePropertyChangedEvent("FormTitle"); }
        }


        private List<BankEF> _banksList;
        public List<BankEF> BanksList {
            get { return _banksList; }
            set { _banksList = value; RaisePropertyChangedEvent("BanksList"); }
        }


        private BankEF _selectedBank;
        public BankEF SelectedBank {
            get { return _selectedBank; }
            set { _selectedBank = value; Contract.bankid = value.id; RaisePropertyChangedEvent("SelectedBank"); }
        }


        private List<ContractTypeEF> _contractTypesList;
        public List<ContractTypeEF> ContractTypesList {
            get { return _contractTypesList; }
            set { _contractTypesList = value; RaisePropertyChangedEvent("ContractTypesList"); }
        }


        private ContractTypeEF _selectedContractType;
        public ContractTypeEF SelectedContractType {
            get { return _selectedContractType; }
            set { _selectedContractType = value; Contract.contracttypeid = value.id; RaisePropertyChangedEvent("SelectedContractType"); }
        }


        private List<CurrencyEF> _currenciesList;
        public List<CurrencyEF> CurrenciesList {
            get { return _currenciesList; }
            set { _currenciesList = value; RaisePropertyChangedEvent("CurrenciesList"); }
        }


        private CurrencyEF _selectedCurrency;
        public CurrencyEF SelectedCurrency {
            get { return _selectedCurrency; }
            set { _selectedCurrency = value; Contract.currencyid = value.id; RaisePropertyChangedEvent("SelectedCurrency"); }
        }


        private List<BrokerEF> _brokersList;
        public List<BrokerEF> BrokersList {
            get { return _brokersList; }
            set { _brokersList = value; RaisePropertyChangedEvent("BrokersList"); }
        }


        private BrokerEF _selectedBroker;
        public BrokerEF SelectedBroker {
            get { return _selectedBroker; }
            set { _selectedBroker = value; Contract.brokerid = value.id; RaisePropertyChangedEvent("SelectedBroker"); }
        }


        private ObservableCollection<RatesListEF> _ratesList;
        public ObservableCollection<RatesListEF> RatesList {
            get { return _ratesList; }
            set { _ratesList = value; RaisePropertyChangedEvent("RatesList"); }
        }


        private RatesListEF _selectedRatesList;
        public RatesListEF SelectedRatesList {
            get { return _selectedRatesList; }
            set {
                _selectedRatesList = value;
                Rates = new ObservableCollection<RateEF>(DataBaseClient.ReadRates(value.id).OrderBy(o => o.transaction));
                RaisePropertyChangedEvent("SelectedRatesList");
            }
        }


        private ObservableCollection<RateEF> _rates;
        public ObservableCollection<RateEF> Rates {
            get { return _rates; }
            set { _rates = value; RaisePropertyChangedEvent("Rates"); }
        }


        private RateEF _selectedRate;
        public RateEF SelectedRate {
            get { return _selectedRate; }
            set { _selectedRate = value; RaisePropertyChangedEvent("SelectedRate"); }
        }


        private List<string> _rangeList;
        public List<string> RangeList {
            get { return _rangeList; }
            set { _rangeList = value; RaisePropertyChangedEvent("RangeList"); }
        }

        private string _selectedRange;
        public string SelectedRange {
            get { return _selectedRange; }
            set { _selectedRange = value; RaisePropertyChangedEvent("SelectedRange"); }
        }

        private decimal _percent;
        public decimal Percent {
            get { return _percent; }
            set { _percent = value; RaisePropertyChangedEvent("Percent"); }
        }

        private string _searchTxt;
        public string SearchTxt {
            get { return _searchTxt; }
            set {
                _searchTxt = value;

                if (value.Length > 1) {
                    BanksList = DataBaseClient.ReadBanks().Where(s => s.name.ToLower().Contains(value.ToLower())).ToList();

                    if (BanksList.Count > 0 && BanksList.Count < 10) IsDropDown = true;
                }

                RaisePropertyChangedEvent("SearchTxt");
            }
        }


        private bool _isDropDown;
        public bool IsDropDown {
            get { return _isDropDown; }
            set { _isDropDown = value; RaisePropertyChangedEvent("IsDropDown"); }
        }


        private List<TraderEF> _authorsList;
        public List<TraderEF> AuthorsList {
            get { return _authorsList; }
            set { _authorsList = value; RaisePropertyChangedEvent("AuthorsList"); }
        }


        private TraderEF _selectedAuthor;
        public TraderEF SelectedAuthor {
            get { return _selectedAuthor; }
            set {
                _selectedAuthor = value;

                if (value != null) Contract.authorid = SelectedAuthor.id;

                RaisePropertyChangedEvent("SelectedAuthor");
            }
        }


        private List<string> _scanTypesList;
        public List<string> ScanTypesList {
            get { return _scanTypesList; }
            set { _scanTypesList = value; RaisePropertyChangedEvent("ScanTypesList"); }
        }


        private string _selectedScanType;
        public string SelectedScanType {
            get { return _selectedScanType; }
            set {
                _selectedScanType = value;

                if (value != null) Contract.scantype = value == ScanTypesList[0] ? 0 : 1;

                RaisePropertyChangedEvent("SelectedScanType");
            }
        }
        #endregion
    }
}