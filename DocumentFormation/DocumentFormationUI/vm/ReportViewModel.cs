using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using System.Collections.Generic;
using System.Windows;
using System.Diagnostics;
using AltaLog;

namespace DocumentFormation.vm
{
    internal class ReportViewModel : PanelViewModelBase
    {
        protected override List<CommandViewModel> CreateCommands()
        {
            return new List<CommandViewModel> {
                new CommandViewModel("Сформировать отчеты", new DelegateCommand(p=>OnReport()))
            };
        }


        private static void OnReport() {
            var reportCount = ReportBP.MakeNewReports();
            if(reportCount == 0) MessageBox.Show("Отчеты не найдены");

            AppJournal.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, "Reports Count = " + reportCount);
            //Debug.Write("ReportViewModel: Reports Count " + reportCount);

            Process.Start("explorer", ReportBP.GetReportsPath());
        }
    }
}
