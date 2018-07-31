using AltaDock.vm;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using AltaMySqlDB.model.tables;
using AltaTransport;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using AltaBO.specifics;

namespace AltaArchive.vm {
    #region Example
    public class Tree : BaseViewModel {
        readonly List<Tree> _node = new List<Tree>();
        public IList<Tree> Node {
            get { return _node; }
        }

        public string Name { get; set; }
        public bool IsBottom { get; set; } = false;

        private bool _isSelected;
        public bool IsSelected {
            get { return _isSelected; }
            set {
                _isSelected = value;

                if(value && IsBottom) MessagesService.Show("Selected branch", "Node is..." + Name);

                RaisePropertyChangedEvent("IsSelected");
            }
        }
    }
    #endregion

    public class ArchiveViewModel : PanelViewModelBase {
        #region Variables        
        private string[] Monthes = new string[] { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" };
        #endregion

        #region Methods
        public ArchiveViewModel() {
            //DefaultParams();            
            BuildTree();
        }


        private void BuildTree() {
            TreeList = GetTreeData();
        }


        private List<Tree> GetTreeData() {
            var documents = new List<DocumentEF>(DataBaseClient.ReadDocuments());

            List<Tree> treeInfo = new List<Tree>();

            foreach(var itemYear in documents.GroupBy(d => d.date.Year).Select(g => new { Year = g.Key })) {
                var branchYear = new Tree { Name = itemYear.Year.ToString() };

                foreach(var itemSite in documents.Where(d => d.date.Year == itemYear.Year && d.site != null).GroupBy(d => d.site.name).Select(g => new { Site = g.Key })) {
                    var branchSite = new Tree { Name = itemSite.Site };

                    foreach(var itemMonth in documents.Where(d => d.date.Year == itemYear.Year && d.site != null && d.site.name == itemSite.Site).GroupBy(d => d.date.Month).Select(g => new { Month = g.Key })) {
                        var branchMonth = new Tree { Name = Monthes[itemMonth.Month - 1] };

                        foreach(var itemNumber in documents.Where(d => d.date.Year == itemYear.Year && d.site != null && d.site.name == itemSite.Site && d.date.Month == itemMonth.Month).GroupBy(d => d.number).Select(g => new { Number = g.Key })) {
                            var branchNumber = new Tree { Name = itemNumber.Number };

                            foreach(var itemDocument in documents.Where(d => d.date.Year == itemYear.Year && d.site != null && d.site.name == itemSite.Site && d.date.Month == itemMonth.Month && d.number == itemNumber.Number).GroupBy(d => d.name).Select(g => new { Name = g.Key })) {
                                var branchDocument = new Tree { Name = itemDocument.Name, IsBottom = true };

                                branchNumber.Node.Add(branchDocument);
                            }

                            branchMonth.Node.Add(branchNumber);
                        }

                        branchSite.Node.Add(branchMonth);
                    }

                    branchYear.Node.Add(branchSite);
                }

                treeInfo.Add(branchYear);
            }

            return treeInfo;
        }


        private void DefaultParams() {
            DocTypesList = DataBaseClient.ReadDocumentTypes();
        }


        protected override List<CommandViewModel> CreateCommands() {
            throw new NotImplementedException();
        }
        #endregion

        #region Bindings
        private List<Tree> _treeList;
        public List<Tree> TreeList {
            get { return _treeList; }
            set { _treeList = value; RaisePropertyChangedEvent("TreeList"); }
        }

        private List<DocumentTypeEF> _docTypesList;
        public List<DocumentTypeEF> DocTypesList {
            get { return _docTypesList; }
            set { _docTypesList = value; RaisePropertyChangedEvent("DocTypesList"); }
        }

        private DocumentTypeEF _selectedDocType;
        public DocumentTypeEF SelectedDocType {
            get { return _selectedDocType; }
            set { _selectedDocType = value; RaisePropertyChangedEvent("SelectedDocType"); }
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
        #endregion
    }
}
