using System.Collections.Generic;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;

namespace DocumentFormation.vm
{
    public class MasterPageViewModel:PanelViewModelBase
    {
        #region Variables
        #endregion


        #region Commands
        protected override List<CommandViewModel> CreateCommands()
        {
            return new List<CommandViewModel> {
                new CommandViewModel("Command", new DelegateCommand(p=>Command()))
            };
        }


        private void Command() { }
        #endregion


        #region Binding items
        #endregion
    }
}
