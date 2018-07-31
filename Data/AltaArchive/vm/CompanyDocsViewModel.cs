using System.Collections.Generic;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using AltaTransport;
using AltaMySqlDB.model.tables;
using System.Windows.Input;
using AltaDock.vm;
using System;
using System.Linq;
using AltaLog;
using System.Collections.ObjectModel;
using System.Windows;

namespace AltaArchive.vm {
    public class CompanyDocsViewModel : BaseViewModel {
        #region Variables
        #endregion

        #region Methods
        public CompanyDocsViewModel() {
            DefaultParametrs();
        }


        private void DefaultParametrs() {
            DocDetailsVis = Visibility.Collapsed;
            IncludeVis = Visibility.Collapsed;
            ExcludeVis = Visibility.Collapsed;

            DateStart = DateTime.Now.AddDays(-60);
            DateEnd = DateTime.Now;

            BrokersList = DataBaseClient.ReadBrokers().Where(b => b.id != 3).OrderByDescending(b => b.id).ToList();

            foreach(var item in BrokersList) {
                item.name = item.name.Replace("Товарищество с ограниченной ответственностью", "ТОО");
            }

            SelectedBroker = BrokersList[0];
            SelectedDefBroker = BrokersList[0];
            DocTypesList = DataBaseClient.ReadDocumentTypes(31, 42, 21);

            DocTypesList.Insert(9,DocTypesList.FirstOrDefault(d=>d.id==21));
            DocTypesList.RemoveAt(0);

            SelectedDocType = DocTypesList[0];
            DocQuantity = 1;

            UpdateCompaniesList();
        }


        public ICommand ApplyCmd { get { return new DelegateCommand(() => UpdateCompaniesList(FilterTxt)); } }
        private void UpdateCompaniesList(string filterText = "") {
            AppJournal.Write("Post|Companies", "Update list by filter", true);

            try {
                if(string.IsNullOrEmpty(FilterTxt)) CompaniesList = DataBaseClient.ReadCompanies();
                else CompaniesList = DataBaseClient.ReadCompanies(filterText);
            } catch(Exception ex) { AppJournal.Write("Post|Companies", "Get companies by filter from db error :" + ex.ToString(), true); }
        }

        public ICommand ApplyDocsCmd { get { return new DelegateCommand(() => UpdateDocsList()); } }
        private void UpdateDocsList() {
            AppJournal.Write("Post|CompanyDocs", "Update list by filter", true);

            if(SelectedCompany != null && SelectedCompany.id != 0) {
                try {
                    OtherDocsList = new ObservableCollection<OtherDocsEF>(DataBaseClient.ReadOtherDocs(SelectedCompany.id, DateStart, DateEnd));
                } catch(Exception ex) { AppJournal.Write("Post|CompanyDocs", "Get company docs by filter from db error :" + ex.ToString(), true); }
            } else {
                MessagesService.Show("Оповещение", "Не выбрана компания");
            }
        }


        public ICommand CreateDocCmd { get { return new DelegateCommand(() => ActionsWithDoc(1)); } }
        public ICommand EditDocCmd { get { return new DelegateCommand(() => ActionsWithDoc(2)); } }
        public ICommand DeleteDocCmd { get { return new DelegateCommand(() => ActionsWithDoc(3)); } }
        public ICommand IncludeDocCmd { get { return new DelegateCommand(() => ActionsWithDoc(4)); } }
        public ICommand ExcludeDocCmd { get { return new DelegateCommand(() => ActionsWithDoc(5)); } }
        private void ActionsWithDoc(int mode) {
            switch(mode) {
                case 1: // Create
                    if(SelectedCompany != null) {
                        // Set default params                        
                        SelectedOtherDoc = null;
                        DocCompanyName = SelectedCompany.name;
                        SelectedBroker = BrokersList.FirstOrDefault(bl => bl.id == SelectedDefBroker.id);
                        DocCreateDate = DateTime.Now;
                        SelectedDocType = DocTypesList[0];
                        DocNumber = "";
                        DocInPost = false;
                        DocQuantity = 1;

                        // Show detials form
                        IncludeVis = Visibility.Visible;
                        ExcludeVis = Visibility.Collapsed;
                        DocDetailsVis = Visibility.Visible;
                    } else MessagesService.Show("Оповещение", "Не выбрана компания");
                    break;
                case 2: // Edit
                    // Fill form
                    DocCompanyName = SelectedOtherDoc.company.name;
                    SelectedBroker = BrokersList.FirstOrDefault(b => b.id == SelectedOtherDoc.brokerid);
                    DocCreateDate = SelectedOtherDoc.createdate;
                    SelectedDocType = DocTypesList.FirstOrDefault(d => d.id == SelectedOtherDoc.documenttypeid);
                    DocNumber = SelectedOtherDoc.number;
                    DocInPost = SelectedOtherDoc.inpost;
                    DocQuantity = SelectedOtherDoc.quantity;

                    if(DocInPost) {
                        IncludeVis = Visibility.Collapsed;
                        ExcludeVis = Visibility.Visible;
                    } else {
                        IncludeVis = Visibility.Visible;
                        ExcludeVis = Visibility.Collapsed;
                    }

                    DocDetailsVis = Visibility.Visible;
                    break;
                case 3: // Delete
                    if(SelectedOtherDoc != null) {
                        try {
                            DataBaseClient.DeleteOtherDoc(SelectedOtherDoc.id);
                        } catch { MessagesService.Show("Оповещение", "Ошибка при удалении записи из базы"); }
                    } else MessagesService.Show("Оповещение", "Не выбран документ");
                    break;
                case 4: // Include
                    // Save otherDoc if need
                    if(SelectedOtherDoc == null) SaveDoc();

                    // Search opened registral
                    var listServ = DataBaseClient.ReadListServ(SelectedOtherDoc.brokerid, 12);
                    int listServId = 0;
                    var listServNumber = DataBaseClient.ReadListServ();

                    if(listServ == null) {
                        // Create list serv with status opened
                        ListServEF listServItem = new ListServEF() {
                            brokerid = SelectedOtherDoc.brokerid,
                            createdate = DateTime.Now,
                            number = listServNumber.Count > 0 ? listServNumber.Max(l => l.number) + 1 : 1,
                            statusid = 12
                        };

                        listServId = DataBaseClient.CreateListServ(listServItem);
                    } else listServId = listServ.id;

                    // Search company in envelop
                    var envelop = DataBaseClient.ReadEnvelop(listServId, SelectedOtherDoc.companyid);
                    int envelopId = 0;

                    if(envelop == null) {
                        // Create envelop
                        EnvelopEF envelopItem = new EnvelopEF() {
                            listservid = listServId,
                            companyid = SelectedOtherDoc.companyid,
                        };

                        envelopId = DataBaseClient.CreateEnvelop(envelopItem);
                    } else envelopId = envelop.id;

                    // Create envelop content
                    EnvelopContentEF envelopContent = new EnvelopContentEF() {
                        listservid = listServId,
                        envelopid = envelopId,
                        otherdocid = SelectedOtherDoc.id
                    };

                    DataBaseClient.CreateEnvelopContent(envelopContent);

                    // Change status inpost
                    SelectedOtherDoc.inpost = true;
                    SelectedOtherDoc.listservnumber = DataBaseClient.ReadListServ(listServId).number;

                    DataBaseClient.UpdateOtherDoc(SelectedOtherDoc);

                    // Update view
                    try {
                        OtherDocsList = new ObservableCollection<OtherDocsEF>(DataBaseClient.ReadOtherDocs(SelectedCompany.id, DateStart, DateEnd));
                        SelectedOtherDoc = null;
                        DocDetailsVis = Visibility.Collapsed;
                    } catch { }
                    break;
                case 5: // Exclude
                    int selOthDoc = SelectedOtherDoc.id;

                    var listServStatus = DataBaseClient.ReadEnvelopContentList();

                    if(listServStatus != null) {
                        if(listServStatus.FirstOrDefault(l => l.otherdocid == selOthDoc).listserv.statusid != 14) {
                            if(!DataBaseClient.DeleteEnvelopContent(selOthDoc)) MessagesService.Show("Оповещение", "Произошла ошибка при исключении документа из реестра.");
                            else {
                                try {
                                    OtherDocsList = new ObservableCollection<OtherDocsEF>(DataBaseClient.ReadOtherDocs(SelectedCompany.id, DateStart, DateEnd));
                                    SelectedOtherDoc = OtherDocsList.FirstOrDefault(o => o.id == selOthDoc);
                                    DocDetailsVis = Visibility.Collapsed;
                                } catch { }
                            }
                        } else MessagesService.Show("Оповещение", "Конверт в статусе отправлен.");
                    }
                    break;
            }
        }


        public ICommand SaveDocCmd { get { return new DelegateCommand(SaveDoc); } }
        private void SaveDoc() {
            if(SelectedOtherDoc == null || SelectedOtherDoc.id == 0) { // Create
                                                                       // Fill 
                OtherDocsEF otherDoc = new OtherDocsEF() {
                    companyid = DataBaseClient.ReadCompanies().FirstOrDefault(c => c.name == DocCompanyName).id,
                    brokerid = SelectedBroker.id,
                    createdate = DocCreateDate,
                    documenttypeid = SelectedDocType.id,
                    number = DocNumber,
                    inpost = false,
                    quantity = DocQuantity
                };

                // Save
                int otherDocId = 0;

                try {
                    otherDocId = DataBaseClient.CreateOtherDoc(otherDoc);
                    IncludeVis = Visibility.Visible;

                    // Auto creation CertificateOfComplition when add InvoiceFacture
                    if(SelectedDocType.id == 31) {
                        OtherDocsEF cCDoc = new OtherDocsEF() {
                            companyid = otherDoc.companyid,
                            brokerid = SelectedBroker.id,
                            createdate = DocCreateDate,
                            documenttypeid = 32,
                            number = DocNumber,
                            inpost = false,
                            quantity = 2
                        };

                        DataBaseClient.CreateOtherDoc(cCDoc);
                    }
                } catch { MessagesService.Show("Оповещение", "Произошла ошибка во время сохранения в базу данных"); }

                // Update view
                UpdateOtherDocsList(otherDoc.companyid);
                SelectedOtherDoc = DataBaseClient.ReadOtherDoc(otherDocId);
            } else { // Update
                // Fill fields
                SelectedOtherDoc.brokerid = SelectedBroker.id;
                SelectedOtherDoc.createdate = DocCreateDate;
                SelectedOtherDoc.documenttypeid = SelectedDocType.id;
                SelectedOtherDoc.number = DocNumber;
                SelectedOtherDoc.quantity = DocQuantity;

                // Save changes
                try {
                    DataBaseClient.UpdateOtherDoc(SelectedOtherDoc);
                } catch { MessagesService.Show("Оповещение", "Произошла ошибка во время сохранения в базу данных"); }

                // Update view
                UpdateOtherDocsList(SelectedOtherDoc.companyid);
            }

            DocDetailsVis = Visibility.Collapsed;
        }

        private void UpdateOtherDocsList(int companyId) {
            OtherDocsList = new ObservableCollection<OtherDocsEF>(DataBaseClient.ReadOtherDocs(companyId, DateStart, DateEnd));
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
            set {
                _selectedCompany = value;
                DocDetailsVis = Visibility.Collapsed;
                UpdateOtherDocsList(value.id);
                RaisePropertyChangedEvent("SelectedCompany");
            }
        }

        private string _filterTxt;
        public string FilterTxt {
            get { return _filterTxt; }
            set { _filterTxt = value; RaisePropertyChangedEvent("FilterTxt"); }
        }

        private ObservableCollection<OtherDocsEF> _otherDocsList;
        public ObservableCollection<OtherDocsEF> OtherDocsList {
            get { return _otherDocsList; }
            set { _otherDocsList = value; RaisePropertyChangedEvent("OtherDocsList"); }
        }

        private OtherDocsEF _selectedOtherDoc;
        public OtherDocsEF SelectedOtherDoc {
            get { return _selectedOtherDoc; }
            set {
                _selectedOtherDoc = value;

                if(value != null) ActionsWithDoc(2);

                RaisePropertyChangedEvent("SelectedOtherDoc");
            }
        }

        private DateTime _dateStart;
        public DateTime DateStart {
            get { return _dateStart; }
            set { _dateStart = value; RaisePropertyChangedEvent("DateStart"); }
        }

        private DateTime _dateEnd;
        public DateTime DateEnd {
            get { return _dateEnd; }
            set { _dateEnd = value; RaisePropertyChangedEvent("DateEnd"); }
        }

        private Visibility _docDetailsVis;
        public Visibility DocDetailsVis {
            get { return _docDetailsVis; }
            set { _docDetailsVis = value; RaisePropertyChangedEvent("DocDetailsVis"); }
        }

        private string _docDetailHeader;
        public string DocDetailHeader {
            get { return _docDetailHeader; }
            set { _docDetailHeader = value; RaisePropertyChangedEvent("DocDetailHeader"); }
        }

        private string _docCompanyName;
        public string DocCompanyName {
            get { return _docCompanyName; }
            set { _docCompanyName = value; RaisePropertyChangedEvent("DocCompanyName"); }
        }

        private List<BrokerEF> _brokersList;
        public List<BrokerEF> BrokersList {
            get { return _brokersList; }
            set { _brokersList = value; RaisePropertyChangedEvent("BrokersList"); }
        }

        private BrokerEF _selectedBroker;
        public BrokerEF SelectedBroker {
            get { return _selectedBroker; }
            set { _selectedBroker = value; RaisePropertyChangedEvent("SelectedBroker"); }
        }

        private BrokerEF _selectedDefBroker;
        public BrokerEF SelectedDefBroker {
            get { return _selectedDefBroker; }
            set { _selectedDefBroker = value; RaisePropertyChangedEvent("SelectedDefBroker"); }
        }

        private DateTime _docCreateDate;
        public DateTime DocCreateDate {
            get { return _docCreateDate; }
            set { _docCreateDate = value; RaisePropertyChangedEvent("DocCreateDate"); }
        }

        private List<DocumentTypeEF> _docTypesList;
        public List<DocumentTypeEF> DocTypesList {
            get { return _docTypesList; }
            set { _docTypesList = value; RaisePropertyChangedEvent("DocTypesList"); }
        }

        private DocumentTypeEF _selectedDocType;
        public DocumentTypeEF SelectedDocType {
            get { return _selectedDocType; }
            set {
                _selectedDocType = value;

                if(value != null && value.id == 32 && SelectedOtherDoc == null) DocQuantity = 2;
                else if(value != null && SelectedOtherDoc == null) DocQuantity = 1;

                RaisePropertyChangedEvent("SelectedDocType");
            }
        }

        private string _docNumber;
        public string DocNumber {
            get { return _docNumber; }
            set { _docNumber = value; RaisePropertyChangedEvent("DocNumber"); }
        }

        private bool _docInPost;
        public bool DocInPost {
            get { return _docInPost; }
            set { _docInPost = value; RaisePropertyChangedEvent("DocInPost"); }
        }

        private Visibility _inculdeVis;
        public Visibility IncludeVis {
            get { return _inculdeVis; }
            set { _inculdeVis = value; RaisePropertyChangedEvent("IncludeVis"); }
        }

        private Visibility _excludeVis;
        public Visibility ExcludeVis {
            get { return _excludeVis; }
            set { _excludeVis = value; RaisePropertyChangedEvent("ExcludeVis"); }
        }

        private int _docQuantity;
        public int DocQuantity {
            get { return _docQuantity; }
            set { _docQuantity = value; RaisePropertyChangedEvent("DocQuantity"); }
        }
        #endregion
    }
}