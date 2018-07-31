using System;
using System.Collections.Generic;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using AltaBO;
using AltaTransport;

namespace DocumentFormation.vm
{
    public class JournalC01ViewModel : PanelViewModelBase
    {
        private OurCompaniesConsts ourCompaniesConsts;
        public string msg;

        public JournalC01ViewModel()
        {
            ourCompaniesConsts = new OurCompaniesConsts();
            OurCompanies = ourCompaniesConsts.GetListOurCompanies();
            DateTxt = DateTime.Now.ToShortDateString();
        }


        protected override List<CommandViewModel> CreateCommands()
        {
            return new List<CommandViewModel> {
                new CommandViewModel("Записать в журнал и сформировать форму", new DelegateCommand(p=>OnWriteInJournal())),
            };
        }


        private void OnWriteInJournal()
        {
            var formC01Service = new FormC01Service(FileArchiveTransport.GetJournalC01FileName());
            var formC01 = new FormC01();

            formC01.code = OurCompany.code;
            formC01.bin = BIN;
            formC01.date = Convert.ToDateTime(DateTxt);
            formC01.name = OurCompany.name;
            formC01.broker = OurCompany;

            var wherePath = Service.GetDirectory();
            if (wherePath != null) {
                formC01 = formC01Service.InsertCompany(formC01);
                formC01Service.CreateTemplate(formC01, wherePath.FullName);
            }
        }


        private List<OurCompany> ourCompanies;
        public List<OurCompany> OurCompanies {
            get { return ourCompanies; }
            set { ourCompanies = value; RaisePropertyChangedEvent("OurCompanies"); }
        }


        private OurCompany ourCompany;
        public OurCompany OurCompany {
            get { return ourCompany; }
            set { ourCompany = value; RaisePropertyChangedEvent("OurCompany"); }
        }


        private string dateTxt;
        public string DateTxt {
            get { return dateTxt; }
            set { dateTxt = value; RaisePropertyChangedEvent("DateTxt"); }
        }


        private string code;
        public string Code {
            get { return code; }
            set { code = value; RaisePropertyChangedEvent("Code"); }
        }


        private string companyName;
        public string CompanyName {
            get { return companyName; }
            set { companyName = value; RaisePropertyChangedEvent("CompanyName"); }
        }


        private string bin;
        public string BIN {
            get { return bin; }
            set { bin = value; RaisePropertyChangedEvent("BIN"); }
        }
    }
}
