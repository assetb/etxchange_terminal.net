using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using altaik.baseapp.vm;
using AltaMySqlDB.service;
using AltaMySqlDB.model;
using AltaBO.archive;
using AltaArchiveApp.models;

namespace AltaArchiveUI.vm {
    public class MainVM : BaseAppViewModel {

        private IDataManager dm = new EntityContext();

        public MainVM() {

        }


        private PresentTree _archivePresentation;
        public PresentTree ArchivePresentation { get { return _archivePresentation; } set { if(value != _archivePresentation) { _archivePresentation = value; RaisePropertyChangedEvent("ArchivePresentation"); }} }


        protected override List<CommandViewModel> CreateCommands() {
            throw new NotImplementedException();
        }
    }
}
