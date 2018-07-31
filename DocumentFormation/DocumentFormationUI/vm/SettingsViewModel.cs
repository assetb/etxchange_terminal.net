using System.Collections.Generic;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using AltaBO;
using System.IO;
using AltaTransport;
using System.Windows.Input;

namespace DocumentFormation.vm
{
    public class SettingsViewModel : PanelViewModelBase
    {
        private DirectoryInfo tmpPath;

        public SettingsViewModel()
        {
            FillSettings();
        }


        private void FillSettings()
        {
            PathSettings = FileArchiveTransport.LoadConfiguration();
        }


        protected override List<CommandViewModel> CreateCommands()
        {
            return new List<CommandViewModel> {
                new CommandViewModel("Сохранить", new DelegateCommand(p=>OnSaveConfiguration())),
            };
        }


        private void OnSaveConfiguration()
        {
            //FileArchiveTransport.SaveConfiguration(pathSettings);
        }


        private PathSettings pathSettings;
        public PathSettings PathSettings {
            get { return pathSettings; }
            set { pathSettings = value; FileArchiveTransport.ReloadConfig(value); RaisePropertyChangedEvent("PathSettings"); }
        }


        public ICommand RootPathCmd { get { return new DelegateCommand(() => SetPath("root")); } }
        public ICommand OrdersPathCmd { get { return new DelegateCommand(() => SetPath("orders")); } }
        public ICommand EDOPathCmd { get { return new DelegateCommand(() => SetPath("edo")); } }
        public ICommand EDOReportsPathCmd { get { return new DelegateCommand(() => SetPath("edoReports")); } }
        public ICommand JournalC01PathCmd { get { return new DelegateCommand(() => SetPath("journalC01")); } }
        public ICommand EntryOrdersPathCmd { get { return new DelegateCommand(() => SetPath("entryOrders")); } }
        public ICommand TemplatesPathCmd { get { return new DelegateCommand(() => SetPath("templates")); } }


        private void SetPath(string way)
        {
            tmpPath = Service.GetDirectory();

            if (tmpPath != null) {                
                switch (way) {
                    case "root": PathSettings.RootPath = tmpPath.FullName; break;
                    case "orders": PathSettings.OrdersPath = tmpPath.FullName; break;
                    case "edo": PathSettings.EDOPath = tmpPath.FullName; break;
                    case "edoReports": PathSettings.EDOReportsPath = tmpPath.FullName; break;
                    case "journalC01": PathSettings.JournalC01Path = tmpPath.FullName; break;
                    case "entryOrders": PathSettings.EntryOrdersPath = tmpPath.FullName; break;
                    case "templates": PathSettings.TemplatesPath = tmpPath.FullName; break;
                }
                RaisePropertyChangedEvent("PathSettings");
            }
        }
    }
}
