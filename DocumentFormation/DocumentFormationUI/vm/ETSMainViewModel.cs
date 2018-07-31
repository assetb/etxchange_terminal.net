using System.Collections.Generic;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using DocumentFormation.view;
using AltaTransport;
using AltaBO;
using AltaDock.vm;
using System.Diagnostics;
using AltaBO.specifics;

namespace DocumentFormation.vm {
    public class ETSMainViewModel : PanelViewModelBase {
        private string orderFile;
        private string treatyFile;
        private CustomersEnum customerType = CustomersEnum.Vostok;


        public ETSMainViewModel() {
            BtnIsEnable = true;
        }


        public void SetOrderFile(string fileOrder) {
            orderFile = fileOrder;
        }

        public void SetContractFile(string contractFile) {
            treatyFile = contractFile;
        }

        public void SetCustomerType(CustomersEnum customersEnumType) {
            customerType = customersEnumType;
        }


        private InputOrderViewModel datesVM;
        public InputOrderViewModel DatesVM {
            get { return datesVM ?? (datesVM = new InputOrderViewModel()); }
            set { if(value != null && value != datesVM) { datesVM = value; RaisePropertyChangedEvent("DatesVM"); } }
        }


        private RequisitesViewModel requisitesVM;
        public RequisitesViewModel RequisitesVM {
            get { return requisitesVM ?? (requisitesVM = new RequisitesViewModel()); }
            set { if(value != null && value != requisitesVM) { requisitesVM = value; DatesVM.Order.Title = RequisitesVM.Title; RaisePropertyChangedEvent("RequisitesVM"); } }
        }


        public Order Order {
            get { return DatesVM.Order; }
            set { if(value != null) DatesVM.Order = value; }
        }


        private bool btnIsEnable;
        public bool BtnIsEnable {
            get { return btnIsEnable; }
            set { btnIsEnable = value; RaisePropertyChangedEvent("BtnIsEnable"); }
        }


        protected override List<CommandViewModel> CreateCommands() {
            var commandViewModel = new List<CommandViewModel>();

            if(orderFile == null) {
                commandViewModel.Add(new CommandViewModel("Создать заявку Восток", new DelegateCommand(p => OnCreateOrder(CustomersEnum.Vostok))));
                commandViewModel.Add(new CommandViewModel("Создать заявку Инкай", new DelegateCommand(p => OnCreateOrder(CustomersEnum.Inkay))));
            } else {
                commandViewModel.Add(customerType == CustomersEnum.Vostok
                    ? new CommandViewModel("Подтвердить создание Восток",
                        new DelegateCommand(p => OnCreateOrder(CustomersEnum.Vostok)))
                    : new CommandViewModel("Подтвердить создание Инкай",
                        new DelegateCommand(p => OnCreateOrder(CustomersEnum.Inkay))));
            }

            //commandViewModel.Add(new CommandViewModel("Журнал С01", new DelegateCommand(p => OnJournalC01())));
            //commandViewModel.Add(new CommandViewModel("Отправка по ЭДО", new DelegateCommand(p => OnADOSend())));

            return commandViewModel;
        }


        private void OnCreateOrder(CustomersEnum customerEnumType) {
            BtnIsEnable = false;
            var wherePath = OrderTransportUI.GetETSOrderDirectory().FullName;

            if(orderFile == null) {
                orderFile = customerEnumType == CustomersEnum.Vostok ? OrderTransportUI.GetVostokOrder().FullName : customerEnumType == CustomersEnum.Inkay ? OrderTransportUI.GetInkayOrder().FullName : null;
                if(string.IsNullOrEmpty(orderFile)) return;
            }

            if(customerEnumType == CustomersEnum.Vostok) {
                if(treatyFile == null) {
                    treatyFile = OrderTransportUI.GetTreatyDraft().FullName;
                    if(string.IsNullOrEmpty(treatyFile)) return;
                }

                Order.Title = RequisitesVM.Title;
            }

            Order = OrderBP.GenerateOrder(customerEnumType, orderFile, treatyFile, Order, wherePath, "");

            DesktopClient.Send("EventsPipe", Order);

            BtnIsEnable = true;

            Process.Start("explorer", wherePath);
        }


        private void OnJournalC01() {
            var journalC01ViewModel = new JournalC01ViewModel { Description = "Журнал C01" };
            var journalC01View = new JournalC01();
            journalC01ViewModel.View = journalC01View;

            Workspace.This.Panels.Add(journalC01ViewModel);
        }


        private void OnADOSend() { }
    }
}
