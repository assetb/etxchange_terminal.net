using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using altaik.baseapp.vm;
using AltaBO.archive;
using System.Collections.ObjectModel;
using AltaArchiveApp.Services;

namespace AltaArchiveUI.vm {
    public class NodeVM : BaseViewModel {
        private Node node;
        public Node Node { get { return node; } set { if(value != node) { node = value; RaisePropertyChangedEvent("Node"); } } }


        private ObservableCollection<NodeVM> children;
        public ObservableCollection<NodeVM> Children { get { return children; } set { if(value != children) { children = value; RaisePropertyChangedEvent("Children"); } } }


        public NodeVM(Node node, bool hasDocuments) {
            this.Node = node;
            this.hasDocuments = hasDocuments;
        }


        private bool isSelected;
        public bool IsSelected { get { return isSelected; } set { if(value != isSelected) { isSelected = value; RaisePropertyChangedEvent("IsSelected"); } } }


        private bool hasDocuments;
        public bool HasDocuments { get { return hasDocuments; } set { if(value != hasDocuments) { hasDocuments = value; RaisePropertyChangedEvent("HasDocuments"); } } }


        private void SelectedEventHandler() {
            if(IsSelected) { }
        }


        private bool _isLastLevel;
        public bool IsLastLevel {
            get { return _isLastLevel; }
            set { _isLastLevel = value; RaisePropertyChangedEvent("IsLastLevel"); }
        }


        private NodeVM _parent;
        public NodeVM Parent {
            get { return _parent; }
            set { _parent = value; RaisePropertyChangedEvent("Parent"); }
        }
    }
}
