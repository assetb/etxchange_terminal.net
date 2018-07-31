using AltaArchive.view;
using AltaArchive.vm;
using AltaArchiveUI.service;
using AltaArchiveUI.view;
using AltaArchiveUI.vm;
using AltaDock.vm;
using altaik.baseapp.vm.command;
using DocumentFormation.view;
using DocumentFormation.vm;
using System.Windows.Input;
using altaik.baseapp.vm;
using System.Windows;
using AltaReportsUI.vm;
using AltaReportsUI.view;

namespace HostApp.vm {
    public class ComplexWindowViewModel : BaseViewModel {
        #region Variables
        #endregion


        #region Methods
        public ComplexWindowViewModel() { }


        public ICommand TradingSystemCmd => new DelegateCommand(TradingSystemShow);
        private void TradingSystemShow() {
            MainViewModel mainViewModel = new MainViewModel();
            mainViewModel.Description = "Брокерская система";
            var mainView = new MainView();
            mainViewModel.View = mainView;

            Workspace.This.Panels.Add(mainViewModel);
            Workspace.This.ActiveDocument = mainViewModel;
        }


        public ICommand PostCmd => new DelegateCommand(PostShow);
        private void PostShow() {
            PostViewModel postViewModel = new PostViewModel();
            postViewModel.Description = "Почта";
            var postView = new PostView();
            postViewModel.View = postView;

            Workspace.This.Panels.Add(postViewModel);
            Workspace.This.ActiveDocument = postViewModel;
        }


        public ICommand ArchivCmd => new DelegateCommand(()=>ArchivShow(PresentationEnum.Archive));
        public ICommand ExchangeCmd => new DelegateCommand(()=>ArchivShow(PresentationEnum.Exchange));

        private void ArchivShow(PresentationEnum present) {
            PresentTreeVM archiveViewModel = new PresentTreeVM(present);
            archiveViewModel.Description = archiveViewModel.DisplayName;
            var archiveView = new PresentTreeView();
            archiveViewModel.View = archiveView;

            Workspace.This.Panels.Add(archiveViewModel);
            Workspace.This.ActiveDocument = archiveViewModel;
        }


        public ICommand ReportsCmd => new DelegateCommand(ReportsShow);
        private void ReportsShow() {
            // Old
            /*ReportsViewModel reportsViewModel = new ReportsViewModel();
            reportsViewModel.Description = "Отчеты";
            var reportsView = new AltaArchive.view.ReportsView();
            reportsViewModel.View = reportsView;

            Workspace.This.Panels.Add(reportsViewModel);
            Workspace.This.ActiveDocument = reportsViewModel;*/

            // New solid project
            ReportsVM reportsVM = new ReportsVM();
            reportsVM.Description = "Отчеты";
            var reportsView = new AltaReportsUI.view.ReportsView();
            reportsVM.View = reportsView;

            Workspace.This.Panels.Add(reportsVM);
            Workspace.This.ActiveDocument = reportsVM;
        }
        #endregion

        #region Bindings
        #endregion
    }
}
