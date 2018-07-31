using AltaBO.archive;
using altaik.baseapp.vm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaArchiveUI.vm
{
    public class DetailsInfoVM : BaseAppViewModel
    {
        #region Variables
        #endregion

        #region Methods
        public DetailsInfoVM() { }


        public void ShowDocument(Document document)
        {
            Document = document;
        }


        protected override List<CommandViewModel> CreateCommands()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Bindings
        private Document _document;
        public Document Document {
            get { return _document; }
            set { _document = value; RaisePropertyChangedEvent("Document"); }
        }
        #endregion
    }
}
