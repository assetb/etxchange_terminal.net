using System.Collections.Generic;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using AltaTransport;
using AltaMySqlDB.model.tables;
using System.Windows.Input;
using AltaDock.vm;
using System;
using AltaArchive.view;
using DocumentFormation;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AltaLog;
using AltaBO.specifics;
using AltaMySqlDB.service;
using AltaMySqlDB.model;

namespace AltaArchive.vm
{
    public class CompanyAndGoods
    {
        public string companyName { get; set; }
        public int count { get; set; }
        public string result { get; set; }
    }

    public class GoodSearchingViewModel : BaseViewModel
    {
        #region Variables
        private IDataManager dataManager = new EntityContext();
        //private CompanyEF selCompany;
        #endregion

        #region Methods
        public GoodSearchingViewModel()
        {
            DefaultParametrs();
        }


        private void DefaultParametrs()
        {
        }


        public ICommand SearchCmd { get { return new DelegateCommand(Search); } }

        private void Search()
        {
            if (!string.IsNullOrEmpty(SearchTxt))
            {
                CompanyGoodsList = dataManager.GetCompaniesWithProduct(SearchTxt);

                if (CompanyGoodsList != null && CompanyGoodsList.Count > 0)
                {
                    CompanyAndGoods = new List<vm.CompanyAndGoods>();
                    List<ProductCompanyEF> productCompany = new List<ProductCompanyEF>();

                    foreach (var item in CompanyGoodsList)
                    {
                        if (item.company != null) productCompany.Add(item);
                    }

                    CompanyGoodsList = productCompany;

                    var goods = CompanyGoodsList.GroupBy(c => c.company.name);

                    foreach (IGrouping<string, ProductCompanyEF> g in goods)
                    {
                        CompanyAndGoods cag = new vm.CompanyAndGoods();
                        cag.companyName = g.Key;

                        foreach (var t in g) cag.result += t.name + ", ";

                        CompanyAndGoods.Add(cag);
                    }
                }
            }
        }


        public ICommand OpenCompanyCmd { get { return new DelegateCommand(OpenCompany); } }

        private void OpenCompany()
        {
            SelectedCompany = CompanyGoodsList.FirstOrDefault(cgl => cgl.company.name == SelectedCompanyAndGoods.companyName);

            var companyViewModel = new CompanyViewModel(SelectedCompany.company) { Description = "Компания " + SelectedCompany.company.name.Replace("Товарищество с ограниченной ответственностью", "ТОО") };

            var companyView = new CompanyView();
            companyViewModel.View = companyView;

            Workspace.This.Panels.Add(companyViewModel);
            Workspace.This.ActiveDocument = companyViewModel;
        }
        #endregion

        #region Bindings
        private string _searchTxt;
        public string SearchTxt
        {
            get { return _searchTxt; }
            set { _searchTxt = value; RaisePropertyChangedEvent("SearchTxt"); }
        }


        private List<ProductCompanyEF> _companyGoodsList;
        public List<ProductCompanyEF> CompanyGoodsList
        {
            get { return _companyGoodsList; }
            set { _companyGoodsList = value; RaisePropertyChangedEvent("CompanyGoodsList"); }
        }


        private ProductCompanyEF _selectedCompany;
        public ProductCompanyEF SelectedCompany
        {
            get { return _selectedCompany; }
            set
            {
                _selectedCompany = value;

                //if(value != null) selCompany = DataBaseClient.ReadCompany(value.companyId);

                RaisePropertyChangedEvent("SelectedCompany");
            }
        }


        private List<CompanyAndGoods> _companyAndGoods;
        public List<CompanyAndGoods> CompanyAndGoods
        {
            get { return _companyAndGoods; }
            set { _companyAndGoods = value; RaisePropertyChangedEvent("CompanyAndGoods"); }
        }

        private CompanyAndGoods _selectedCompanyAndGoods;
        public CompanyAndGoods SelectedCompanyAndGoods
        {
            get { return _selectedCompanyAndGoods; }
            set { _selectedCompanyAndGoods = value; RaisePropertyChangedEvent("SelectedCompanyAndGoods"); }
        }
        #endregion
    }
}