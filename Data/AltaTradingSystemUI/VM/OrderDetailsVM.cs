using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using AltaBO;
using AltaBO.views;
using AltaTradingSystemApp.Services;
using System.Windows.Input;
using AltaDock.vm;
using AltaArchiveApp;

namespace AltaTradingSystemUI.VM
{
    public class OrderDetailsVM : BaseViewModel
    {
        #region Variables
        private Auction auction;
        #endregion

        #region Methods
        public OrderDetailsVM(Auction auction, Order order)
        {
            this.auction = auction;
            Order = order;

            Init();
        }


        private void Init()
        {
            TypesList = DictionariesService.ReadDocumentTypes();

            if (auction.Id != 0) FilesList = DocumentService.ReadDocuments(auction.FilesListId);
            if (Order.id == 0) Order = AuctionService.ReadOrder(auction.Id);
            if (Order != null) {
                if ((Order.id != null || Order.id != 0) && (Order.filesListId != null || Order.filesListId != 0)) CustomerFilesList = DocumentService.ReadDocuments(Order.filesListId);
            }
        }


        public ICommand DownloadFileCmd => new DelegateCommand(DownloadFile);
        private void DownloadFile()
        {
            if (SelectedCustomerFile == null && SelectedFile == null) {
                MessagesService.Show("Скачивание документа", "Не выбран документ для скачивания");
                return;
            }

            DocumentView documentView = SelectedCustomerFile == null ? SelectedFile : SelectedCustomerFile;
            var path = DocumentService.GetNewDocumentPath("Укажите путь и файл для сохранения", "(*." + documentView.extension + ") | *." + documentView.extension, documentView.name);

            if (path == null) return;

            if (DocumentService.DownloadDocument(documentView, path)) DocumentService.OpenFolder(path.Substring(0, path.LastIndexOf("\\")));
            else MessagesService.Show("Скачивание документа", "Произошла ошибка при скачивании документа");
        }


        public ICommand UploadFileCmd => new DelegateCommand(UploadFile);
        private void UploadFile()
        {
            if (auction.Id == 0) {
                MessagesService.Show("Загрузка файла", "Необходимо сохранить аукцион");
                return;
            }

            var path = DocumentService.GetDocumentPath("Укажите файл для загрузки", "(*.*) | *.*");

            if (path == null) return;

            DocumentService.UploadDocument(SelectedType, path, auction);
            Init();
        }


        public ICommand DeleteFileCmd => new DelegateCommand(DeleteFile);
        private void DeleteFile()
        {
            if (SelectedCustomerFile == null && SelectedFile == null) {
                MessagesService.Show("Удаление документа", "Не выбран документ для удаление");
                return;
            }

            DocumentView documentView = SelectedCustomerFile == null ? SelectedFile : SelectedCustomerFile;

            DocumentService.DeleteDocument(documentView.id);
            Init();
        }
        #endregion

        #region Bindings
        private Order _order;
        public Order Order {
            get { return _order; }
            set { _order = value; RaisePropertyChangedEvent("Order"); }
        }


        private List<DocumentView> _customerFilesList;
        public List<DocumentView> CustomerFilesList {
            get { return _customerFilesList; }
            set { _customerFilesList = value; RaisePropertyChangedEvent("CustomerFilesList"); }
        }


        private DocumentView _selectedCustomerFile;
        public DocumentView SelectedCustomerFile {
            get { return _selectedCustomerFile; }
            set {
                _selectedCustomerFile = value;

                if (value != null && SelectedFile != null) SelectedFile = null;

                RaisePropertyChangedEvent("SelectedCustomerFile");
            }
        }


        private List<DocumentView> _FilesList;
        public List<DocumentView> FilesList {
            get { return _FilesList; }
            set { _FilesList = value; RaisePropertyChangedEvent("FilesList"); }
        }


        private DocumentView _selectedFile;
        public DocumentView SelectedFile {
            get { return _selectedFile; }
            set {
                _selectedFile = value;

                if (value != null && SelectedCustomerFile != null) SelectedCustomerFile = null;

                RaisePropertyChangedEvent("SelectedFile");
            }
        }


        private List<DocumentType> _typesList;
        public List<DocumentType> TypesList {
            get { return _typesList; }
            set { _typesList = value; RaisePropertyChangedEvent("TypesList"); }
        }


        private DocumentType _selectedType;
        public DocumentType SelectedType {
            get { return _selectedType; }
            set { _selectedType = value; RaisePropertyChangedEvent("SelectedType"); }
        }
        #endregion
    }
}
