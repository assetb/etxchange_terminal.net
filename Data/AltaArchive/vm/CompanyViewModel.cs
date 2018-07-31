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
using AltaTransport._1c.InvoicePaiment;
using System.Threading.Tasks;
using AltaArchive.Services;
using AltaBO;
using DocumentFormation;
using AltaLog;
using AltaArchiveApp;
using AltaMySqlDB.service;
using AltaMySqlDB.model;
using System.IO;
using BridgeApp;

namespace AltaArchive.vm
{
    public class CompanyViewModel : PanelViewModelBase
    {
        #region Variables   
        private IDataManager dataManager = new EntityContext();
        private ArchiveManager archiveManager;
        #endregion

        #region Methods
        public CompanyViewModel(CompanyEF companyInfo = null)
        {
            if (companyInfo != null)
            {
                Company = companyInfo;
                FormTitle = "Просмотр/редактирование компании";
            }
            else FormTitle = "Создание компании";

            DefaultParametrs(companyInfo);
        }


        private void DefaultParametrs(CompanyEF company = null, bool refresh = false)
        {
            IikColor = System.Windows.Media.Brushes.Black;

            if (!refresh)
            {
                archiveManager = new ArchiveManager(dataManager);
                CountriesList = DataBaseClient.ReadCountries();
            }

            if (company == null)
            {
                Company = new CompanyEF();
                Company.createdate = DateTime.Now;
                IsSupplier = true;
            }
            else
            {
                try
                {
                    if (DataBaseClient.ReadSupplier(Company.id) != null) IsSupplier = true;
                    if (DataBaseClient.ReadCustomer(Company.id) != null) IsCustomer = true;

                    SelectedCountry = CountriesList.Where(x => x.id == company.countryid).FirstOrDefault();
                    ContractsList = DataBaseClient.ReadContracts(Company.id);
                    C01List = DataBaseClient.ReadSuppliersJournals(DataBaseClient.GetSupplierId(Company.id));
                    SearchSupplier = Company.name;

                    if (Company.iik == null) return;

                    if (Company.iik.Length < 20) IikColor = System.Windows.Media.Brushes.Red;
                    else IikColor = System.Windows.Media.Brushes.Black;

                }
                catch (Exception ex)
                {
                    MessagesService.Show("ОШИБКА", "Ошибка загрузки данных для страницы");
                    AppJournal.Write("Company", "Get company info from db error :" + ex.ToString(), true);
                }
            }
        }


        protected override List<CommandViewModel> CreateCommands()
        {
            return new List<CommandViewModel> {
                new CommandViewModel("Сохранить", new DelegateCommand(Save)),
                new CommandViewModel("Отмена", new DelegateCommand(Cancel)),
                new CommandViewModel("Сформировать анкету", new DelegateCommand(FormateProfile)),
                new CommandViewModel("Взять данные из 1С", new DelegateCommand(GetCompanyFrom1C)),
                new CommandViewModel("Передать в 1С", new DelegateCommand(PutCompanyTo1C)),
                new CommandViewModel("Открыть папку", new DelegateCommand(OpenFolder)),
            };
        }


        private void Save()
        {
            AppJournal.Write("Company", "Save", true);

            try
            {
                if (!string.IsNullOrEmpty(Company.bin)) Company.bin = Company.bin.Replace(" ", "");
                if (!string.IsNullOrEmpty(Company.iik)) Company.iik = Company.iik.Replace(" ", "");

                if (Company.id != 0)
                {
                    DataBaseClient.UpdateCompany(Company);
                    MessagesService.Show("Обновление компании", "Компания обновлена");
                }
                else
                {
                    if (CheckForSame())
                    {
                        UpdateView(DataBaseClient.CreateCompany(Company));
                        MessagesService.Show("Создание компании", "Компания создана");
                    }
                    else MessagesService.Show("Создание компании", "Компания существует");
                }

                CheckBelongs();
            }
            catch (Exception ex)
            {
                MessagesService.Show("Ошибка при сохранении", "Произошла ошибка при сохранении");
                AppJournal.Write("Company", "Save company in db error :" + ex.ToString(), true);
            }
        }


        private bool AllFieldsSet()
        {
            if (string.IsNullOrEmpty(Company.name) || string.IsNullOrEmpty(Company.bin) || Company.countryid == null || Company.kbe == null ||
                string.IsNullOrEmpty(Company.addressactual) || string.IsNullOrEmpty(Company.addresslegal) || string.IsNullOrEmpty(Company.telephone) ||
                string.IsNullOrEmpty(Company.iik)) return false;
            else return true;
        }


        private bool CheckForSame()
        {
            if (!DataBaseClient.CheckDuplicateCompany(Company)) return true;
            else return false;
        }


        private void UpdateView(int id)
        {
            AppJournal.Write("Company", "Update view", true);

            try
            {
                Company = DataBaseClient.ReadCompany(id);
            }
            catch (Exception ex) { AppJournal.Write("Company", "Get company from db error :" + ex.ToString(), true); }

            FormTitle = "Просмотр/редактирование компании";
            DefaultParametrs(Company, true);
        }


        private void Cancel() { Workspace.This.Panels.Remove(Workspace.This.ActiveDocument); }


        private void FormateProfile()
        {
            bool err = false;

            // Check faults
            if (Company.govregdate == null) Company.govregdate = DateTime.Now;
            if (Company.govregnumber == null) Company.govregnumber = "";

            // Copy template
            var companyFolder = archiveManager.ipPath + "\\Archive\\Companies\\" + Company.bin;
            var templateFileName = archiveManager.ipPath + "\\Archive\\Templates\\KazETS\\CompanyProfile.xlsx";

            if (!Directory.Exists(companyFolder)) Directory.CreateDirectory(companyFolder);

            try
            {
                File.Copy(templateFileName, companyFolder + "\\Анкета компании " + ConvertToShortName(Company.name).Replace("\"", "'") + ".xlsx", true);
            }
            catch (Exception ex)
            {
                err = true;
                AppJournal.Write("Template copy", "Err: " + ex.ToString(), true);
            }

            if (!err)
            {
                var fileName = companyFolder + "\\Анкета компании " + ConvertToShortName(Company.name).Replace("\"", "'") + ".xlsx";

                // Get bank name for company from contract if exist
                string bankName = "";
                string bankBik = "";
                Contract contractInfo = new Contract() { agreementdate = null, number = "" };

                if (SelectedContract == null && ContractsList != null && ContractsList.Count > 0) SelectedContract = ContractsList[0];

                if (SelectedContract != null)
                {
                    bankName = SelectedContract.bank != null ? SelectedContract.bank.name : "";
                    bankBik = SelectedContract.bank != null ? SelectedContract.bank.company.bik : "";
                    contractInfo.agreementdate = SelectedContract.agreementdate;
                    contractInfo.number = SelectedContract.number;
                }

                // Convert EF to BO data
                Company companyInfo = new Company()
                {
                    name = Company.name,
                    bin = Company.bin,
                    govregnumber = Company.govregnumber == null ? "" : Company.govregnumber,
                    govregdate = (DateTime)Company.govregdate == null ? DateTime.Now : (DateTime)Company.govregdate,
                    addressActual = Company.addressactual,
                    addressLegal = Company.addresslegal,
                    directorPowers = Company.directorpowers,
                    director = Company.director,
                    bank = bankName,
                    bik = bankBik,
                    iik = Company.iik
                };

                // Formate template
                CompanyProfileService.CreateDocument(companyInfo, contractInfo, fileName);

                // Open folder
                OpenFolder();
            }
            else MessagesService.Show("Формирование анкеты", "Ошибка при формировании");
        }


        private Clients Client;

        private async void GetCompanyFrom1C()
        {
            AppJournal.Write("Company", "Get company data from 1C", true);

            string companyBIN = await MessagesService.GetInput("ПОИСК В 1С", "Введите БИН компании");

            //MahApps.Metro.Controls.Dialogs.ProgressDialogController progressDialog = await MessagesService.ShowProgress();
            //progressDialog.SetIndeterminate();

            //await Task.Run(() => {
            if (!string.IsNullOrEmpty(companyBIN))
            {
                Client = _1CTransport.SearchCompany(companyBIN, 1);

                if (Client != null)
                {
                }
                else
                {
                    Client = _1CTransport.SearchCompany(companyBIN, 2);

                    if (Client != null)
                    {
                    }
                    else
                    {
                        Client = _1CTransport.SearchCompany(companyBIN, 3);

                        if (Client != null) { }
                    }
                }

                if (Client != null)
                {
                    Company.name = Client.FullName;
                    Company.comments = Client.Comments;

                    SelectedCountry = CountriesList.Where(x => x.code_1c == Convert.ToInt32(Client.CountryCode)).FirstOrDefault();

                    Company.bin = Client.IIN;
                    Company.kbe = string.IsNullOrEmpty(Client.KBE) ? 0 : Convert.ToInt32(Client.KBE);
                    Company.addresslegal = Client.LegalAddress;
                    Company.telephone = Client.PhoneNumber;
                    Company.addressactual = Client.АctualАddress;

                    Company = Company;
                }
            }
            //});

            //await progressDialog.CloseAsync();

            if (Client == null) MessagesService.Show("РЕЗУЛЬТАТ ПОИСКА В БАЗАХ", "Результатов нет");
        }


        private async void PutCompanyTo1C()
        {
            AppJournal.Write("Company", "Create company in 1C", true);

            string baseType = await MessagesService.GetInput("ЗАНЕСЕНИЕ В 1С", "Выберите цифру базы: (1 - для Альта и К, 2 - для Корунд-777, 3 - для Ак Алтын Ко)");

            //MahApps.Metro.Controls.Dialogs.ProgressDialogController progressDialog = await MessagesService.ShowProgress();
            //progressDialog.SetIndeterminate();

            string result = "";

            //await Task.Run(() => {

            Clients clients = new Clients();

            if (baseType == "1" || baseType == "2" || baseType == "3")
            {
                clients = _1CTransport.SearchCompany(Company.bin, Convert.ToInt32(baseType));
                if (clients == null)
                {
                    clients = new Clients();
                    clients.Comments = ".NET Автоматизация";
                    clients.CountryCode = SelectedCountry.code_1c.ToString();
                    clients.FullName = Company.name;
                    clients.CtlgName = ConvertToShortName(Company.name);
                    clients.IIN = Company.bin.PadLeft(12, '0');
                    clients.KBE = Company.kbe.ToString();
                    clients.LegalAddress = Company.addresslegal;
                    clients.PhoneNumber = Company.telephone;
                    clients.АctualАddress = Company.addressactual;
                    clients.ParentCode = "000000002";
                    clients.LegalNaturaPerson = "ЮрЛицо";
                    clients.CtlgCode = "";

                    result = _1CTransport.CreateCompany(Convert.ToInt32(baseType), clients);
                }
                else result = "exist";
            }
            //});

            //await progressDialog.CloseAsync();

            if (result.Contains("ok")) MessagesService.Show("РЕЗУЛЬТАТ ЗАНЕСЕНИЯ В 1С", "Компания создана успешно");
            else if (result.Contains("exist")) MessagesService.Show("РЕЗУЛЬТАТ ЗАНЕСЕНИЯ В 1С", "Компания с таким БИНом уже существует в 1С");
            else
            {
                MessagesService.Show("РЕЗУЛЬТАТ ЗАНЕСЕНИЯ В 1С", "При создании компании возникла ошибка");
                AppJournal.Write("Company", "Put company in 1c error :" + result, true);
            }
        }


        private string ConvertToShortName(string fullName)
        {
            if (fullName.ToUpper().Contains("ТОВАРИЩЕСТВО С ОГРАНИЧЕННОЙ ОТВЕТСТВЕННОСТЬЮ")) fullName = fullName.ToUpper().Replace("ТОВАРИЩЕСТВО С ОГРАНИЧЕННОЙ ОТВЕТСТВЕННОСТЬЮ", "ТОО");
            else if (fullName.ToUpper().Contains("АКЦИОНЕРНОЕ ОБЩЕСТВО")) fullName = fullName.ToUpper().Replace("АКЦИОНЕРНОЕ ОБЩЕСТВО", "АО");
            else if (fullName.ToUpper().Contains("ИНДИВИДУАЛЬНЫЙ ПРЕДПРИНИМАТЕЛЬ")) fullName = fullName.ToUpper().Replace("ИНДИВИДУАЛЬНЫЙ ПРЕДПРИНИМАТЕЛЬ", "ИП");
            else if (fullName.ToUpper().Contains("ОБЩЕСТВО С ОГРАНИЧЕННОЙ ОТВЕТСТВЕННОСТЬЮ")) fullName = fullName.ToUpper().Replace("ОБЩЕСТВО С ОГРАНИЧЕННОЙ ОТВЕТСТВЕННОСТЬЮ", "ООО");

            return fullName;
        }


        public ICommand CreateContractCmd { get { return new DelegateCommand(() => ContractFormShow(1)); } }
        public ICommand UpdateContractCmd { get { return new DelegateCommand(() => ContractFormShow(2)); } }

        private void ContractFormShow(int crudMode)
        {
            AppJournal.Write("Company", "Open contract - " + (crudMode == 1 ? "New" : SelectedContract.number), true);

            if (Company.id != 0)
            {
                if (crudMode == 1) SelectedContract = null;

                ContractViewModel contractViewModel = new ContractViewModel(Company.id, this, SelectedContract) { Description = "Договора №" + (crudMode == 1 ? "" : SelectedContract.number) };

                ContractView contractView = new ContractView();
                contractViewModel.View = contractView;

                Workspace.This.Panels.Add(contractViewModel);
                Workspace.This.ActiveDocument = contractViewModel;
            }
            else MessagesService.Show("ОПОВЕЩЕНИЕ", "Ваша компания не сохранена");
        }

        public ICommand UpdateContractViewCmd { get { return new DelegateCommand(UpdateContractList); } }

        public void UpdateContractList()
        {
            AppJournal.Write("Company", "Update contracts list", true);

            try
            {
                ContractsList = DataBaseClient.ReadContracts(Company.id);
            }
            catch (Exception ex) { AppJournal.Write("Company", "Get contracts list from db error :" + ex.ToString(), true); }
        }

        public ICommand DeleteContractCmd { get { return new DelegateCommand(DeleteContract); } }

        private void DeleteContract()
        {
            AppJournal.Write("Company", "Delete contract", true);

            if (SelectedContract != null)
            {
                try
                {
                    DataBaseClient.DeleteContract(SelectedContract.id);
                }
                catch (Exception ex) { AppJournal.Write("Company", "Delete contract from db error :" + ex.ToString(), true); }

                UpdateContractList();
            }
        }


        public ICommand AttachContractCmd { get { return new DelegateCommand(AttachContract); } }

        private string sourceFileName, newFileName;

        private void AttachContract()
        {
            AppJournal.Write("Company", "Attach contract", true);

            if (SelectedContract != null && SelectedContract.id > 0)
            {
                try
                {
                    sourceFileName = Service.GetFile("Выберите файл договора", "(*.*) | *.*").FullName;
                }
                catch (Exception) { }

                if (!string.IsNullOrEmpty(sourceFileName))
                {
                    newFileName = FolderExplorer.GetCompanyPath(Company.bin);
                    newFileName += "Договор " + SelectedContract.number.Replace("/", "_").Replace("\\", "_") + " с " + ServiceFunctions.CompanyRenamer(Company.name) + sourceFileName.Substring(sourceFileName.LastIndexOf("."));

                    if (Service.CopyFile(sourceFileName, newFileName, true))
                    {
                        try
                        {
                            DocumentRequisite docReq = new DocumentRequisite()
                            {
                                date = DateTime.Now,
                                fileName = newFileName.Substring(newFileName.LastIndexOf("\\") + 1),
                                number = Company.bin,
                                section = AltaBO.specifics.DocumentSectionEnum.Company,
                                type = AltaBO.specifics.DocumentTypeEnum.Contract
                            };

                            DataBaseClient.UpdateContractFile(SelectedContract.id, archiveManager.SaveFile(docReq));
                            MessagesService.Show("АРХИВ", "Файл скопирован в архив.");
                            UpdateContractList();
                        }
                        catch (Exception ex)
                        {
                            MessagesService.Show("АРХИВ", "При занесении файла в базу произошла ошибка");
                            AppJournal.Write("Company", "Put contract into db error :" + ex.ToString(), true);
                        }
                    }
                    else MessagesService.Show("ОШИБКА", "Проверьте доступ к архиву.");
                }
            }
            else MessagesService.Show("Работа с договором", "Не выбран договор");
        }


        private void CheckBelongs()
        {
            if (!IsSupplier && DataBaseClient.ReadSupplier(Company.id) != null)
                DataBaseClient.DeleteSupplier(Company.id);
            else if (IsSupplier && DataBaseClient.ReadSupplier(Company.id) == null)
                DataBaseClient.CreateSupplier(Company.id);

            if (!IsCustomer && DataBaseClient.ReadCustomer(Company.id) != null)
                DataBaseClient.DeleteCustomer(Company.id);
            else if (IsCustomer && DataBaseClient.ReadCustomer(Company.id) == null)
                DataBaseClient.CreateCustomer(Company.id);
        }


        public ICommand CreateC01Cmd { get { return new DelegateCommand(() => C01Func(1)); } }
        public ICommand UpdateC01Cmd { get { return new DelegateCommand(() => C01Func(2)); } }

        private async void C01Func(int crudMode)
        {
            AppJournal.Write("Company", "Open C01 - " + (crudMode == 1 ? "New" : SelectedC01.code), true);

            if (Company.id != 0)
            {
                if (crudMode == 1)
                {
                    if (SelectedContract != null && SelectedContract.id > 0 && DataBaseClient.ReadSupplier(Company.id) != null)
                    {
                        if (DataBaseClient.ReadSuppliersJournals(DataBaseClient.ReadSupplier(Company.id).id, (int)SelectedContract.brokerid).Count() < 1)
                        {
                            string companyCode = await MessagesService.GetInput("Создание С01", "Введите код клиента:");
                            string serialNumber = await MessagesService.GetInput("Создание С01", "Введите номер (последним был - " + DataBaseClient.ReadSuppliersJournals((int)SelectedContract.brokerid, true).Max(s => s.serialnumber) + "):");

                            try
                            {
                                SuppliersJournalEF suppliersJournal = new SuppliersJournalEF()
                                {
                                    code = companyCode,
                                    brokerid = (int)SelectedContract.brokerid,
                                    regdate = DateTime.Now,
                                    supplierid = DataBaseClient.GetSupplierId(Company.id),
                                    serialnumber = Convert.ToInt32(serialNumber)
                                };

                                DataBaseClient.CreateSuppliersJournal(suppliersJournal);
                                JournalC01Service.CreateRecordInJournal(JournalC01Service.FillC01(SelectedContract.broker.company.bin, companyCode, Company.bin, Company.name, suppliersJournal.serialnumber.ToString()));
                                UpdateC01List();

                                OpenFolder();
                            }
                            catch (Exception ex)
                            {
                                MessagesService.Show("Создание С01", "При создании С01 произошла ошибка");
                                AppJournal.Write("Company", "Create c01 in db error :" + ex.ToString(), true);
                            }
                        }
                        else MessagesService.Show("Создание С01", "По выбраному договору запись С01 существует");
                    }
                    else MessagesService.Show("Создание С01", "Не выбран договор или компания не является поставщиком");
                }
                else
                {
                    if (SelectedC01 != null && SelectedC01.id > 0)
                    {
                        string companyCode = await MessagesService.GetInput("Обновление С01", "Введите новый код клиента:");

                        if (!string.IsNullOrEmpty(companyCode))
                        {
                            try
                            {
                                SelectedC01.code = companyCode;
                                DataBaseClient.UpdateSuppliersJournal(SelectedC01);
                                JournalC01Service.CreateRecordInJournal(JournalC01Service.FillC01(SelectedC01.broker.company.bin, SelectedC01.code, SelectedC01.supplier.company.bin, SelectedC01.supplier.company.name, SelectedC01.serialnumber.ToString()));
                            }
                            catch (Exception ex)
                            {
                                MessagesService.Show("Обновление С01", "При обновлении произошла ошибка");
                                AppJournal.Write("Company", "Update c01 in db error :" + ex.ToString(), true);
                            }
                        }
                    }
                    else MessagesService.Show("Обновление С01", "Не выбрана запись С01");
                }
            }
            else MessagesService.Show("Работа с С01", "Ваша компания не сохранена");
        }


        public ICommand UpdateC01ViewCmd { get { return new DelegateCommand(UpdateC01List); } }

        private void UpdateC01List()
        {
            AppJournal.Write("Company", "Update c01 list", true);

            try
            {
                C01List = DataBaseClient.ReadSuppliersJournals(DataBaseClient.GetSupplierId(Company.id));
            }
            catch (Exception ex) { AppJournal.Write("Company", "Get c01 from db error :" + ex.ToString(), true); }
        }

        public ICommand DeleteC01Cmd { get { return new DelegateCommand(DeleteC01); } }

        private void DeleteC01()
        {
            AppJournal.Write("Company", "Delete c01", true);

            if (SelectedC01 != null)
            {
                try
                {
                    DataBaseClient.DeleteSuppliersJournal(SelectedC01.id);
                    MessagesService.Show("Удаление из журнала С01", "Запись удалена");
                }
                catch (Exception ex)
                {
                    MessagesService.Show("Удаление из журнала С01", "Произошла ошибка при удалении");
                    AppJournal.Write("Company", "Delete c01 from db error :" + ex.ToString(), true);
                }

                UpdateC01List();
            }
        }


        private void OpenFolder()
        {
            if (Company != null && Company.id > 0) FolderExplorer.OpenCompanyFolder(Company.bin);
            else MessagesService.Show("Открытие папки", "Компании не существует или не сохранена");
        }


        public ICommand BridgeToSCCmd { get { return new DelegateCommand(BridgeToSC); } }
        private void BridgeToSC()
        {
            SupplierBridge.OpenSupplierCabinet(Company.id);
        }


        public ICommand SetArchiveNumberCmd { get { return new DelegateCommand(SetArchiveNumber); } }
        private void SetArchiveNumber()
        {
            if (SelectedContract != null)
            {
                if (SelectedContract.document != null)
                {
                    var archiveNumberViewModel = new ArchiveNumberViewModel(SelectedContract.document, (int)SelectedContract.brokerid) { Description = "Архивный номер" };

                    var archiveNumberView = new ArchiveNumberView();
                    archiveNumberViewModel.View = archiveNumberView;

                    Workspace.This.Panels.Add(archiveNumberViewModel);
                    Workspace.This.ActiveDocument = archiveNumberViewModel;
                }
                else MessagesService.Show("Работа с архивным номером", "У договора нет прикрепленного скана");
            }
            else MessagesService.Show("Работа с архивным номером", "Не выбран договор");
        }
        #endregion

        #region Bindings
        private CompanyEF _company;
        public CompanyEF Company {
            get { return _company; }
            set { _company = value; RaisePropertyChangedEvent("Company"); }
        }


        private string sName;

        private string searchSupplier;
        public string SearchSupplier {
            get { return searchSupplier; }
            set {
                sName = string.Empty;

                if (value.Length < 4)
                {
                    if (value.ToLower().Contains("тоо")) sName = "Товарищество с ограниченной ответственностью ";
                    else if (value.ToLower().Contains("ооо")) sName = "Общество с ограниченной ответственностью ";
                    else if (value.ToLower().Contains("ао")) sName = "Акционерное общество ";
                    else if (value.ToLower().Contains("ип")) sName = "Индивидуальный предприниматель ";
                    else sName = "";
                }

                if (!string.IsNullOrEmpty(sName)) value = sName;

                searchSupplier = value;
                Company.name = value;
                RaisePropertyChangedEvent("SearchSupplier");
            }
        }


        private String _formTitle;
        public String FormTitle {
            get { return _formTitle; }
            set { _formTitle = value; RaisePropertyChangedEvent("FormTitle"); }
        }


        private List<ContractEF> _contractsList;
        public List<ContractEF> ContractsList {
            get { return _contractsList; }
            set { _contractsList = value; RaisePropertyChangedEvent("ContractsList"); }
        }


        private ContractEF _selectedContract;
        public ContractEF SelectedContract {
            get { return _selectedContract; }
            set {
                _selectedContract = value;

                if (value != null)
                {
                    var ratesList = DataBaseClient.ReadRatesList(value.id);

                    if (ratesList != null && ratesList.Count > 0)
                    {
                        RatesListTxt = "Перечень тарифных сеток: ";

                        foreach (var item in ratesList)
                        {
                            RatesListTxt += item.name + "; ";
                        }
                    }
                    else RatesListTxt = "Тарифные сетки отсутствуют";

                    if (SelectedContract.document != null && SelectedContract.document.archiveNumber != null)
                    {
                        var aNum = SelectedContract.document.archiveNumber;

                        ArchiveNumberTxt = "Архивный номер: " + aNum.Year.ToString() + "\\" + aNum.Broker.shortname + "\\" + aNum.Case.Name + "\\" + aNum.Volume.Name + "\\" + aNum.DocumentNumber;
                    }
                    else ArchiveNumberTxt = "Архивный номер не присвоен";
                }

                RaisePropertyChangedEvent("SelectedContract");
            }
        }


        private List<CountryEF> _countriesList;
        public List<CountryEF> CountriesList {
            get { return _countriesList; }
            set { _countriesList = value; RaisePropertyChangedEvent("CountriesList"); }
        }


        private CountryEF _selectedCountry;
        public CountryEF SelectedCountry {
            get { return _selectedCountry; }
            set {
                _selectedCountry = value;
                if (value != null) Company.countryid = value.id;
                RaisePropertyChangedEvent("SelectedCountry");
            }
        }


        private bool _isCustomer;
        public bool IsCustomer {
            get { return _isCustomer; }
            set { _isCustomer = value; RaisePropertyChangedEvent("IsCustomer"); }
        }


        private bool _isSupplier;
        public bool IsSupplier {
            get { return _isSupplier; }
            set { _isSupplier = value; RaisePropertyChangedEvent("IsSupplier"); }
        }


        private List<SuppliersJournalEF> _c01List;
        public List<SuppliersJournalEF> C01List {
            get { return _c01List; }
            set { _c01List = value; RaisePropertyChangedEvent("C01List"); }
        }


        private SuppliersJournalEF _selectedC01;
        public SuppliersJournalEF SelectedC01 {
            get { return _selectedC01; }
            set { _selectedC01 = value; RaisePropertyChangedEvent("SelectedC01"); }
        }


        private System.Windows.Media.Brush _iikcolor;
        public System.Windows.Media.Brush IikColor {
            get { return _iikcolor; }
            set { _iikcolor = value; RaisePropertyChangedEvent("IikColor"); }
        }


        private string _ratesListTxt;
        public string RatesListTxt {
            get { return _ratesListTxt; }
            set { _ratesListTxt = value; RaisePropertyChangedEvent("RatesListTxt"); }
        }


        private string _archiveNumberTxt;
        public string ArchiveNumberTxt {
            get { return _archiveNumberTxt; }
            set { _archiveNumberTxt = value; RaisePropertyChangedEvent("ArchiveNumberTxt"); }
        }
        #endregion
    }
}
