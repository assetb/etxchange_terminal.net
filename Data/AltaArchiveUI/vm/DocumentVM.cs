using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using altaik.baseapp.vm;
using AltaBO.archive;
using System.Windows.Media;

namespace AltaArchiveUI.vm {
    public class DocumentVM:BaseViewModel {

        public DocumentVM(Document doc) {
            this.document = doc;            
        }

        private Document document;
        public Document Document { get { return document; } set { if(value!=document) { document = value; RaisePropertyChangedEvent("Document"); } } }

        private ImageSource image;
        public ImageSource Image { get { return image; } set { if(value != image) { image = value; RaisePropertyChangedEvent("Image"); } } }
    }
}
