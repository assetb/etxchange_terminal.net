using System.Windows;
using AltaDock.vm;
using DocumentFormation.vm;
using DocumentFormation.view;
using altaik.baseapp.ext;
using System;
using AltaBO.specifics;
using System.Linq;
using System.Diagnostics;
using altaik.baseapp.vm;
using AltaArchive.view;
using AltaArchive.vm;
using AltaTradingSystemUI.View;
using AltaTradingSystemUI.VM;


namespace HostApp
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Archive
            var archiveVM = OpenArchive();

            if (archiveVM != null) Workspace.This.Panels.Add(archiveVM);

            // Payda
            //var tradingSystemVM = OpenTradingSystem();

            //if (tradingSystemVM != null) Workspace.This.Panels.Add(tradingSystemVM);

            #region Old
            // Old
            //var args = Environment.GetCommandLineArgs();

            //if (args.Length > 1) {
            //    if (args[1] == BusinessObjectsEnum.Order.GetName()) {
            //        //Order formation

            //        var etsMainVM = ReleasedETSMainViewModel(args);

            //        if(etsMainVM != null) Workspace.This.Panels.Add(etsMainVM);

            //    } else if (args[1] == BusinessObjectsEnum.Report.GetName()) {
            //        // Reports formating

            //    } else if (args[1] == BusinessObjectsEnum.Warranty.GetName()) {
            //        // Free warranty
            //    }else if (args[1] == BusinessObjectsEnum.Archive.GetName()) {
            //        var archiveVM = OpenArchive();

            //        if (archiveVM != null) Workspace.This.Panels.Add(archiveVM);
            //    }
            //} else {
            //    var commandsViewModel = new DFCommandsViewModel();
            //    commandsViewModel.Description = "Панель команд";
            //    var commandsView = new CommandsView();
            //    commandsViewModel.View = commandsView;

            //    Workspace.This.Panels.Add(commandsViewModel);
            //}
            #endregion

            var complexWindow = new ComplexWindow();
            complexWindow.DataContext = Workspace.This;
            complexWindow.ShowDialog();
        }


        private PanelViewModelBase ReleasedETSMainViewModel(string[] args)
        {
            if (args == null) return null;

            var etsViewModel = new ETSMainViewModel();
            if (args.Length > 2) {
                if (args[2] == CustomersEnum.Vostok.GetName()) {
                    etsViewModel.SetCustomerType(CustomersEnum.Vostok);
                } else if (args[2] == CustomersEnum.Inkay.GetName()) {
                    etsViewModel.SetCustomerType(CustomersEnum.Inkay);
                }
            }

            string orderFile = null;
            try {
                orderFile = args.Skip(2).First(p => p.Contains("заявка"));
            } catch (Exception ex) {
                Debug.WriteLine(GetType().Name + ": " + ex.Message);
            }
            if (orderFile != null) etsViewModel.SetOrderFile(orderFile);

            string treatyFile = null;
            try {
                treatyFile = args.Skip(2).First(p => p.Contains("догов"));
            } catch (Exception ex) {
                Debug.WriteLine(GetType().Name + ": " + ex.Message);
            }
            if (treatyFile != null && treatyFile.Length > 0) etsViewModel.SetContractFile(treatyFile);

            etsViewModel.Description = "Формирование заявок";
            var etsView = new ETSMainView();
            etsViewModel.View = etsView;

            return etsViewModel;
        }


        private PanelViewModelBase OpenArchive()
        {
            var mainViewModel = new MainViewModel();
            mainViewModel.Description = "Учетная система";
            var mainView = new AltaArchive.view.MainView();
            mainViewModel.View = mainView;

            return mainViewModel;
        }


        private PanelViewModelBase OpenTradingSystem()
        {
            var mainVM = new MainVM();
            mainVM.Description = "Брокерская система";
            var mainView = new AltaTradingSystemUI.View.MainView();
            mainVM.View = mainView;

            return mainVM;
        }
    }
}