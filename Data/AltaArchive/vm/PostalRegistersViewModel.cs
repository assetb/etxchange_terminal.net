using System.Collections.Generic;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using AltaTransport;
using AltaMySqlDB.model.tables;
using AltaMySqlDB.model.views;
using System.Windows.Input;
using AltaDock.vm;
using System;
using DocumentFormation;
using System.Linq;
using AltaBO;
using AltaArchive.Services;

namespace AltaArchive.vm
{
    public class PostalRegistersViewModel : BaseViewModel
    {
        #region Variables
        private string path = "";
        private string templateFile = "";
        private string templatePath = @"\\192.168.11.5\Archive\Templates\ForAll\";
        #endregion

        #region Methods
        public PostalRegistersViewModel()
        {
            DefaultParametrs();
        }

        private void DefaultParametrs()
        {
            StartDate = DateTime.Now.AddDays(-60);
            EndDate = DateTime.Now;

            UpdateListServ();
        }

        public ICommand ApplyListServCmd { get { return new DelegateCommand(UpdateListServ); } }
        private void UpdateListServ()
        {
            if (string.IsNullOrEmpty(ListServNumber)) ListServ = DataBaseClient.ReadListServ(StartDate, EndDate);
            else ListServ = DataBaseClient.ReadListServ(StartDate, EndDate, Convert.ToInt32(ListServNumber));
        }

        public ICommand PlusStatusCmd { get { return new DelegateCommand(() => ChangeStatus(1)); } }
        public ICommand MinusStatusCmd { get { return new DelegateCommand(() => ChangeStatus(2)); } }
        private void ChangeStatus(int mode)
        {
            if (SelectedListServ != null)
            {
                switch (mode)
                {
                    case 1: // Plus status
                        if (SelectedListServ.statusid != 14)
                        {
                            DataBaseClient.UpdateListServ(SelectedListServ.id, 14, DateTime.Now);
                        }
                        else MessagesService.Show("Оповещение", "У реестра наивысшый статус");
                        break;
                    case 2: // Minus status
                        if (SelectedListServ.statusid != 12)
                        {
                            DataBaseClient.UpdateListServ(SelectedListServ.id, 12);
                        }
                        else MessagesService.Show("Оповещение", "У реестра нижайщий статус");
                        break;
                }

                UpdateListServ();
            }
            else MessagesService.Show("Оповещение", "Не выбран почтовый реестр");
        }

        public ICommand DeleteEnvelopCmd { get { return new DelegateCommand(DeleteEnvelop); } }
        private void DeleteEnvelop()
        {
            // Check for selected and for listserv status
            if (SelectedEnvelop != null && SelectedListServ != null && SelectedListServ.statusid != 14)
            {
                // Delete all envelop content with change otherdocs status
                if (EnvelopContentList != null && EnvelopContentList.Count > 0)
                {
                    foreach (var item in EnvelopContentList)
                    {
                        // Change otherdocs status & delete envelop content
                        DataBaseClient.DeleteEnvelopContent(item.otherdocid);
                    }
                }

                // Delete envelop
                DataBaseClient.DeleteEnvelop(SelectedEnvelop.id);

                // Update views
                UpdateEnvelopList();
            }
            else MessagesService.Show("Оповещение", "Не выполненно одно из условий:\n1. Не выбран конверт\n2. Реестр в статусе отправлен");
        }

        public ICommand SetCodeCmd { get { return new DelegateCommand(SetCode); } }
        private async void SetCode()
        {
            // Check for selected 
            if (SelectedEnvelop != null)
            {
                // Get code from user
                string code = await MessagesService.GetInput("Установка ШПИ", "Введите код");

                if (!string.IsNullOrEmpty(code))
                {
                    // Update record in base & update view
                    if (DataBaseClient.UpdateEnvelop(SelectedEnvelop.id, code)) EnvelopsList = DataBaseClient.ReadEnvelopsList(SelectedListServ.id);
                    else MessagesService.Show("Оповещение", "Произошла ошибка при присвоении ШПИ");
                }
            }
            else MessagesService.Show("Оповещение", "Не выбран конверт");
        }

        private void UpdateEnvelopList()
        {
            EnvelopsList = DataBaseClient.ReadEnvelopsList(SelectedListServ.id);
        }

        private void UpdateEnvelopContent()
        {
            EnvelopContentList = DataBaseClient.ReadEnvelopContentList(SelectedEnvelop.id);
        }

        public ICommand ExcludeDocCmd { get { return new DelegateCommand(ExcludeDoc); } }
        private void ExcludeDoc()
        {
            int selOthDoc = SelectedEnvelopContent.otherdocid;

            var listServStatus = DataBaseClient.ReadEnvelopContentList();

            if (listServStatus != null)
            {
                if (listServStatus.FirstOrDefault(l => l.otherdocid == selOthDoc).listserv.statusid != 14)
                {
                    if (!DataBaseClient.DeleteEnvelopContent(selOthDoc)) MessagesService.Show("Оповещение", "Произошла ошибка при исключении документа из реестра.");
                    else
                    {
                        try
                        {
                            UpdateEnvelopContent();
                        }
                        catch { }
                    }
                }
                else MessagesService.Show("Оповещение", "Конверт в статусе отправлен.");
            }
        }

        public ICommand DeleteDocCmd { get { return new DelegateCommand(DeleteDoc); } }
        private void DeleteDoc()
        {
            if (SelectedEnvelopContent != null)
            {
                try
                {
                    DataBaseClient.DeleteEnvelopContent(SelectedEnvelopContent.otherdocid);
                    DataBaseClient.DeleteOtherDoc(SelectedEnvelopContent.otherdocid);
                    UpdateEnvelopContent();
                    UpdateEnvelopList();
                }
                catch { MessagesService.Show("Оповещение", "Ошибка при удалении записи из базы"); }
            }
            else MessagesService.Show("Оповещение", "Не выбран документ");
        }

        public ICommand PrintEnvelopCmd { get { return new DelegateCommand(() => Print(1)); } }
        public ICommand PrintNotificationCmd { get { return new DelegateCommand(() => Print(2)); } }
        public ICommand PrintRegisterCmd { get { return new DelegateCommand(() => Print(3)); } }
        private void Print(int mode)
        {
            // Check for selected and for count
            if (SelectedListServ != null && SelectedListServ.envelopcount > 1)
            {
                // Get path to save template
                try
                {
                    path = Service.GetDirectory().FullName;
                }
                catch { path = ""; }

                if (!string.IsNullOrEmpty(path))
                {
                    // Get template file
                    templateFile = path + (mode == 1 ? "\\Конверты №" : mode == 2 ? "\\Уведомления №" : "\\Почтовый реестр №") + SelectedListServ.number + ".xlsx";

                    if (Service.CopyFile(templatePath + (mode == 1 ? "Envelop.xlsx" : mode == 2 ? "Notification.xlsx" : "Registral.xlsx"), templateFile, true))
                    {
                        // Convert info
                        List<PostRegister> postRegister = new List<PostRegister>();

                        foreach (var item in EnvelopsList)
                        {
                            postRegister.Add(new PostRegister()
                            {
                                name = item.company == null ? "" : item.company,
                                index = item.index == null ? "" : item.index,
                                address = item.address == null ? "" : item.address,
                                phones = item.phones == null ? "" : item.phones,
                                code = item.code == null ? "" : item.code
                            });
                        }

                        var brokerItem = DataBaseClient.ReadBrokers().FirstOrDefault(b => b.id == SelectedListServ.brokerid);

                        if (brokerItem != null)
                        {
                            Broker broker = new Broker();

                            broker.Name = SelectedListServ.broker == null ? "" : SelectedListServ.broker;
                            broker.Index = brokerItem.company.postcode == null ? "" : brokerItem.company.postcode;
                            broker.Address = brokerItem.company.addressactual == null ? "" : brokerItem.company.addressactual;
                            broker.Phones = brokerItem.company.telephone == null ? "" : brokerItem.company.telephone;

                            // Formate document
                            try
                            {
                                PostRegisterService.CreateDocument(postRegister, templateFile, mode, broker);
                                FolderExplorer.OpenFolder(path + "\\");
                            }
                            catch (Exception) { MessagesService.Show("Оповещение", "во время формирования произошла ошибка"); }
                        }
                        else MessagesService.Show("ОШИБКА", "Не найдены данные брокера");
                    }
                    else MessagesService.Show("Оповещение", "Произошла ошибка во время копирования файла");
                }
            }
            else MessagesService.Show("Оповещение", "Не выбран конверт или он пуст");
        }
        #endregion

        #region Bindings
        private DateTime _startDate;
        public DateTime StartDate {
            get { return _startDate; }
            set { _startDate = value; RaisePropertyChangedEvent("StartDate"); }
        }

        private DateTime _endDate;
        public DateTime EndDate {
            get { return _endDate; }
            set { _endDate = value; RaisePropertyChangedEvent("EndDate"); }
        }

        private List<ListServView> _listServ;
        public List<ListServView> ListServ {
            get { return _listServ; }
            set { _listServ = value; RaisePropertyChangedEvent("ListServ"); }
        }

        private ListServView _selectedListServ;
        public ListServView SelectedListServ {
            get { return _selectedListServ; }
            set {
                _selectedListServ = value;
                EnvelopsList = DataBaseClient.ReadEnvelopsList(value.id);
                RaisePropertyChangedEvent("SelectedListServ");
            }
        }

        private List<EnvelopsView> _envelopsList;
        public List<EnvelopsView> EnvelopsList {
            get { return _envelopsList; }
            set {
                _envelopsList = value;

                if (value.Count > 0) EnvelopContentList = DataBaseClient.ReadEnvelopContentList(value[0].id);
                EnvelopsCount = "Конверты (" + value.Count.ToString() + ")";

                RaisePropertyChangedEvent("EnvelopsList");
            }
        }

        private EnvelopsView _selectedEnvelop;
        public EnvelopsView SelectedEnvelop {
            get { return _selectedEnvelop; }
            set {
                _selectedEnvelop = value;
                UpdateEnvelopContent();
                RaisePropertyChangedEvent("SelectedEnvelop");
            }
        }

        private List<EnvelopContentEF> _envelopContentList;
        public List<EnvelopContentEF> EnvelopContentList {
            get { return _envelopContentList; }
            set { _envelopContentList = value; RaisePropertyChangedEvent("EnvelopContentList"); }
        }

        private EnvelopContentEF _selectedEnvelopContent;
        public EnvelopContentEF SelectedEnvelopContent {
            get { return _selectedEnvelopContent; }
            set { _selectedEnvelopContent = value; RaisePropertyChangedEvent("SelectedEnvelopContent"); }
        }

        private string _listServNumber;
        public string ListServNumber {
            get { return _listServNumber; }
            set { _listServNumber = value; RaisePropertyChangedEvent("ListServNumber"); }
        }

        private string _envelopsCount;
        public string EnvelopsCount {
            get { return _envelopsCount; }
            set { _envelopsCount = value; RaisePropertyChangedEvent("EnvelopsCount"); }
        }
        #endregion
    }
}