using System.Collections.Generic;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using DocumentFormation.view;
using AltaDock.vm;
using AltaTransport.vm;
using AltaTransport.view;
using System.Windows.Input;

namespace DocumentFormation.vm
{
    public class DFCommandsViewModel : PanelViewModelBase
    {
        protected override List<CommandViewModel> CreateCommands()
        {
            return new List<CommandViewModel> {
                new CommandViewModel("Формирование заявок", new DelegateCommand(p=>OnOrderDocumentFormation())),
                new CommandViewModel("Журнал С01", new DelegateCommand(p=>OnJournalC01())),
                new CommandViewModel("Заявка на участие (ЕТС)", new DelegateCommand(p=>OnEntryOrder())),
                new CommandViewModel("Заявка на участие (УТБ)", new DelegateCommand(p=>OnSupplierOrderUTB())),
                new CommandViewModel("Список претендентов (ЕТС)", new DelegateCommand(p=>OnWaitingList())),
                new CommandViewModel("СП, ШП, ОТ(УТБ)", new DelegateCommand(p=>OnApplicants())),
                new CommandViewModel("Отправка по ЭДО", new DelegateCommand(p=>OnSendByADO())),
                new CommandViewModel("Отправка по Эл. почте", new DelegateCommand(p=>OnSendByEmail())),
                new CommandViewModel("Поручение на сделку", new DelegateCommand(p=>OnInstructionAttachToDeal())),
                //new CommandViewModel("Счет на оплату", new DelegateCommand(p=>OnWarrantyInvoice())),
                new CommandViewModel("Формирование отчетов", new DelegateCommand(p=>OnReportDocumentFormation())),
                new CommandViewModel("Заявление об учете", new DelegateCommand(p=>OnFreeWarrantyProvision())),
                new CommandViewModel("Акт о несостоявшемся", new DelegateCommand(p=>OnFailedAct())),
                new CommandViewModel("Настройки", new DelegateCommand(p=>OnSettings()))
            };
        }


        #region UTB
        public ICommand OrderUTB { get { return new DelegateCommand(OrderUTBView); } }
        private static void OrderUTBView()
        {
            var orderUTBViewModel = new OrderUTBViewModel { Description = "Формирование заявок на биржу" };
            var orderUTBView = new OrderUTBView();
            orderUTBViewModel.View = orderUTBView;

            Workspace.This.Panels.Add(orderUTBViewModel);
            Workspace.This.ActiveDocument = orderUTBViewModel;
        }


        public ICommand SupplierOrderUTB { get { return new DelegateCommand(OnSupplierOrderUTB); } }
        private static void OnSupplierOrderUTB()
        {
            var supplierOrderViewModel = new SupplierOrderViewModel { Description = "Заявка на участие" };
            var supplierOrderView = new SupplierOrderView();
            supplierOrderViewModel.View = supplierOrderView;

            Workspace.This.Panels.Add(supplierOrderViewModel);
            Workspace.This.ActiveDocument = supplierOrderViewModel;
        }


        public ICommand ApplicantsUTB { get { return new DelegateCommand(OnApplicants); } }
        private static void OnApplicants()
        {
            var applicantsViewModel = new ApplicantsViewModel { Description = "Список участников" };
            var applicantsView = new ApplicantsView();
            applicantsViewModel.View = applicantsView;

            Workspace.This.Panels.Add(applicantsViewModel);
            Workspace.This.ActiveDocument = applicantsViewModel;
        }
        #endregion


        #region ForAll
        public ICommand Invoice { get { return new DelegateCommand(CreateInvoice); } }
        private static void CreateInvoice() {
            var invoiceViewModel = new InvoiceViewModel { Description = "Счет на оплату" };
            var invoiceView = new InvoiceView();
            invoiceViewModel.View = invoiceView;

            Workspace.This.Panels.Add(invoiceViewModel);
            Workspace.This.ActiveDocument = invoiceViewModel;
        }
        #endregion


        #region ETS
        public ICommand OrderETS { get { return new DelegateCommand(OnOrderDocumentFormation); } }
        private static void OnOrderDocumentFormation()
        {
            var etsViewModel = new ETSMainViewModel {Description = "Формирование заявок"};
            var etsView = new ETSMainView();
            etsViewModel.View = etsView;

            Workspace.This.Panels.Add(etsViewModel);
        }


        public ICommand JournalC01 { get { return new DelegateCommand(OnJournalC01); } }
        private static void OnJournalC01()
        {
            var journalC01ViewModel = new JournalC01ViewModel {Description = "Журнал C01"};
            var journalC01View = new JournalC01();
            journalC01ViewModel.View = journalC01View;

            Workspace.This.Panels.Add(journalC01ViewModel);
        }


        public ICommand SupplierOrderETS { get { return new DelegateCommand(OnEntryOrder); } }
        private static void OnEntryOrder()
        {
            var entryOrderViewModel = new EntryOrderViewModel {Description = "Заявка на участие"};
            var entryOrderView = new EntryOrderView();
            entryOrderViewModel.View = entryOrderView;

            Workspace.This.Panels.Add(entryOrderViewModel);
        }


        public ICommand ApplicantsETS { get { return new DelegateCommand(OnWaitingList); } }
        private static void OnWaitingList()
        {
            var waitingListViewModel = new WaitingListViewModel {Description = "Список претендентов"};
            var waitingListView = new WaitingListView();
            waitingListViewModel.View = waitingListView;

            Workspace.This.Panels.Add(waitingListViewModel);
        }

        
        public ICommand EmailSending { get { return new DelegateCommand(OnSendByEmail); } }
        private static void OnSendByEmail()
        {
            var emailSendViewModel = new EmailSendViewModel {Description = "Отправка по эл. почте"};
            var emailSendView = new EmailSendView();
            emailSendViewModel.View = emailSendView;

            Workspace.This.Panels.Add(emailSendViewModel);
        }


        public ICommand ProcuratoryETS { get { return new DelegateCommand(OnInstructionAttachToDeal); } }
        private static void OnInstructionAttachToDeal() {
            var instructionAttachToDealViewModel = new InstructionAttachToDealViewModel {Description = "Поручение и приложение к сделке"};
            var instructionAttachToDealView = new InstructionAttachToDealView();
            instructionAttachToDealViewModel.View = instructionAttachToDealView;

            Workspace.This.Panels.Add(instructionAttachToDealViewModel);
        }


        public ICommand ReportsETS { get { return new DelegateCommand(OnReportDocumentFormation); } }
        private static void OnReportDocumentFormation() {
            var reportViewModel = new ReportViewModel {Description = "Отчеты"};
            var reportView = new ReportView();
            reportViewModel.View = reportView;

            Workspace.This.Panels.Add(reportViewModel);
        }

        
        public ICommand ActETS { get { return new DelegateCommand(OnFailedAct); } }
        private static void OnFailedAct()
        {
            var failedActViewModel = new FailedActViewModel {Description = "Акт о несостоявшемся торге"};
            var failedActView = new FailedActView();
            failedActViewModel.View = failedActView;

            Workspace.This.Panels.Add(failedActViewModel);
        }


        private static void OnSettings()
        {
            var settingsViewModel = new SettingsViewModel {Description = "Настройки"};
            var settingsView = new SettingsView();
            settingsViewModel.View = settingsView;

            Workspace.This.Panels.Add(settingsViewModel);
        }


        private static void OnFreeWarrantyProvision()
        {
            var moneyTransferViewModel = new MoneyTransferViewModel { Description = "Заявление об изменении учета денег" };
            var moneyTransferView = new MoneyTransferView();
            moneyTransferViewModel.View = moneyTransferView;

            Workspace.This.Panels.Add(moneyTransferViewModel);
        }


        private static void OnSendByADO() { }
        #endregion
    }
}
