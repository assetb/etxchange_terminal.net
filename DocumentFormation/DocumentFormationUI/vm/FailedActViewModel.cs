using System.Collections.Generic;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;


namespace DocumentFormation.vm
{
    public class FailedActViewModel : PanelViewModelBase
    {
        public FailedActViewModel() { }


        protected override List<CommandViewModel> CreateCommands()
        {
            return new List<CommandViewModel> {
                new CommandViewModel("Сформировать акт", new DelegateCommand(p=>OnActFormation()))
            };
        }


        private void OnActFormation()
        {
            var failedActService = new FailedActService();
            failedActService.StartFormation();
        }
    }
}
