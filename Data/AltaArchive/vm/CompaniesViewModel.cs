using System;
using System.Collections.Generic;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using AltaTransport;
using System.Windows.Input;
using AltaMySqlDB.model.tables;
using AltaDock.vm;
using AltaArchive.view;
using System.Threading.Tasks;
using AltaLog;

namespace AltaArchive.vm {
    internal class CompaniesViewModel : BaseViewModel {
        #region Variables
        #endregion


        #region Methods
        public CompaniesViewModel() {
            SetDefaultParametrs();
        }


        private void SetDefaultParametrs() {
            UpdateCompaniesList();
        }


        public void UpdateCompaniesList() {
            AppJournal.Write("Companies", "Update list", true);

            try {
                CompaniesList = DataBaseClient.ReadCompanies();
            } catch(Exception ex) { AppJournal.Write("Companies", "Get companies from db error :" + ex.ToString(), true); }
        }

        public ICommand ApplyCmd { get { return new DelegateCommand(() => UpdateCompaniesList(FilterTxt, FilterBinTxt)); } }
        private void UpdateCompaniesList(string filterText, string filterBinText = null) {
            AppJournal.Write("Companies", "Update list by filter", true);

            try {
                CompaniesList = DataBaseClient.ReadCompanies(filterText, filterBinText);
                if(string.IsNullOrEmpty(FilterTxt) && string.IsNullOrEmpty(FilterBinTxt)) CompaniesList = DataBaseClient.ReadCompanies();
                else if(!string.IsNullOrEmpty(FilterTxt) && string.IsNullOrEmpty(FilterBinTxt)) CompaniesList = DataBaseClient.ReadCompanies(FilterTxt, null);
                else if(string.IsNullOrEmpty(FilterTxt) && !string.IsNullOrEmpty(FilterBinTxt)) CompaniesList = DataBaseClient.ReadCompanies(null, FilterBinTxt);
                else if(!string.IsNullOrEmpty(FilterTxt) && !string.IsNullOrEmpty(FilterBinTxt)) CompaniesList = DataBaseClient.ReadCompanies(FilterTxt, FilterBinTxt);
            } catch(Exception ex) { AppJournal.Write("Companies", "Get companies by filter from db error :" + ex.ToString(), true); }
        }


        public ICommand CreateCompanyCmd { get { return new DelegateCommand(() => CompanyFormShow(1)); } }
        public ICommand ReadCompanyCmd { get { return new DelegateCommand(() => CompanyFormShow(2)); } }
        public ICommand UpdateCompanyCmd { get { return new DelegateCommand(() => CompanyFormShow(3)); } }


        private void CompanyFormShow(int crudMode) {
            AppJournal.Write("Companies", "Open company - " + (crudMode == 1 ? "New" : SelectedCompany.name), true);

            if(crudMode == 1) SelectedCompany = null;

            var companyViewModel = new CompanyViewModel(SelectedCompany) { Description = "Компания " + (crudMode == 1 ? "" : SelectedCompany.name.Replace("Товарищество с ограниченной ответственностью", "ТОО")) };

            var companyView = new CompanyView();
            companyViewModel.View = companyView;

            Workspace.This.Panels.Add(companyViewModel);
            Workspace.This.ActiveDocument = companyViewModel;
        }


        public ICommand DeleteCompanyCmd => new DelegateCommand(DeleteCompany);

        private void DeleteCompany() {
            AppJournal.Write("Companies", "Delete company", true);

            if(SelectedCompany != null) {
                try {
                    DataBaseClient.DeleteCompany(SelectedCompany.id);
                } catch(Exception ex) {
                    MessagesService.Show("ОШИБКА", "Ошибка во время удаление компании");
                    AppJournal.Write("Companies", "Delete company from db error :" + ex.ToString(), true);
                }

                UpdateCompaniesList(FilterTxt);
            }
        }


        public ICommand SearchCompanyCmd => new DelegateCommand(SearchCompany);

        private string companyBIN, companyName;

        private async void SearchCompany() {
            AppJournal.Write("Companies", "Search company in 1C", true);

            companyBIN = await MessagesService.GetInput("ПОИСК В 1С", "Введите БИН компании");
            string baseType = await MessagesService.GetInput("ПОИСК В 1С", "Выберите цифру базы где искать: (1 - для Альта и К, 2 - для Корунд-777, 3 - для Ак Алтын Ко)");

            if(baseType == "1" || baseType == "2" || baseType == "3") {

                if(!string.IsNullOrEmpty(companyBIN)) {
                    //progressDialog = await MessagesService.ShowProgress();
                    //progressDialog.SetIndeterminate();

                    //await Task.Run(() => {
                        try {
                            companyName = _1CTransport.SearchCompany(companyBIN, Convert.ToInt32(baseType)).FullName;
                        } catch(Exception ex) {
                            companyName = "";
                            AppJournal.Write("Companies", "Search comapny in 1C error :" + ex.ToString(), true);
                        }
                    //});

                    //await progressDialog.CloseAsync();

                    if(!string.IsNullOrEmpty(companyName)) await MessagesService.ShowDialog("РЕЗУЛЬТАТ ПОИСКА В БАЗЕ " + baseType == "1" ? "АЛЬТА И К" : baseType == "2" ? "КОРУНД-777" : "АК АЛТЫН КО", "Название компании по введенному БИНу " + companyName);
                    else await MessagesService.ShowDialog("РЕЗУЛЬТАТ ПОИСКА В БАЗЕ " + baseType == "1" ? "АЛЬТА И К" : baseType == "2" ? "КОРУНД-777" : "АК АЛТЫН КО", "Результатов нет");
                }
            } else MessagesService.Show("ПОИСК В 1С", "Введена не корректная цифра базы 1С");
        }
        #endregion


        #region Bindings
        private List<CompanyEF> _companiesList;
        public List<CompanyEF> CompaniesList {
            get { return _companiesList; }
            set { _companiesList = value; RaisePropertyChangedEvent("CompaniesList"); }
        }


        private CompanyEF _selectedCompany;
        public CompanyEF SelectedCompany {
            get { return _selectedCompany; }
            set { _selectedCompany = value; RaisePropertyChangedEvent("SelectedCompany"); }
        }


        private string _filterTxt;
        public string FilterTxt {
            get { return _filterTxt; }
            set { _filterTxt = value; RaisePropertyChangedEvent("FilterTxt"); }
        }


        private string _filterBinTxt;
        public string FilterBinTxt {
            get { return _filterBinTxt; }
            set { _filterBinTxt = value; RaisePropertyChangedEvent("FilterBinTxt"); }
        }
        #endregion
    }
}
