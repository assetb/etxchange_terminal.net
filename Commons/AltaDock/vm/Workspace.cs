using System;
using System.Linq;
using altaik.baseapp.vm;
using System.Collections.ObjectModel;
using altaik.baseapp.vm.command;
using System.Windows.Input;

namespace AltaDock.vm
{
    public class Workspace: BaseViewModel
    {
        protected Workspace() { DisplayName = "TTTTTTTTTTTTT"; }

        public static Workspace This { get; } = new Workspace();

        private ObservableCollection<PanelViewModelBase> _panels = new ObservableCollection<PanelViewModelBase>();
        public ObservableCollection<PanelViewModelBase> Panels => _panels ?? (_panels = new ObservableCollection<PanelViewModelBase>());

        #region OpenCommand

        private DelegateCommand _openCmd;
        public ICommand OpenCmd { get { return _openCmd ?? (_openCmd = new DelegateCommand(OnOpen, (p) => CanOpen())); } }

        private static bool CanOpen() { return true; }

        private void OnOpen(object parameter)
        {
            var panel = parameter as PanelViewModelBase;
            if (panel != null) Open(panel);
        }

        public PanelViewModelBase Open(PanelViewModelBase vm)
        {
            var panelViewModelBase = _panels.FirstOrDefault(p => p == vm);
            if (panelViewModelBase == null)
            {
                panelViewModelBase = vm;
                _panels.Add(panelViewModelBase);
            }

            ActiveDocument = panelViewModelBase;
            return panelViewModelBase;
        }

        #endregion

        #region NewCommand

        private DelegateCommand _newCommand;
        public ICommand NewCommand => _newCommand ?? (_newCommand = new DelegateCommand(OnNew, CanNew));

        private static bool CanNew(object parameter)
        {
            return true;
        }

        private void OnNew(object parameter)
        {
            //_panels.Add(new PanelViewModelBase());
            ActiveDocument = _panels.Last();
        }

        #endregion

        #region ActiveDocument

        private PanelViewModelBase _activeDocument;
        public PanelViewModelBase ActiveDocument
        {
            get { return _activeDocument; }
            set
            {
                if (_activeDocument != value)
                {
                    _activeDocument = value;
                    RaisePropertyChangedEvent("ActiveDocument");
                    ActiveDocumentChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler ActiveDocumentChanged;

        #endregion


        internal void Close(PanelViewModelBase panel)
        {
            _panels.Remove(panel);
        }

    }
}
