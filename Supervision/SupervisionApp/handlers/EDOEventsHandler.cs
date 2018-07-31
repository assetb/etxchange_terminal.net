using AltaWindowsNotification;
using System.Diagnostics;
using DocumentFormation;
using SupervisionModel;

namespace SupervisionApp
{
    /// <summary>
    /// Class to handle new report event appearance
    /// </summary>
    public class NewReportEH
    {

        public void NewReportEventHandler(object sender, NewReportEventArg e)
        {
            var toast = new ToastNotification();
            toast.Show("Пришел новый отчет с ЭДО.", NewReportReaction,e);
        }


        private static void NewReportReaction(object p)
        {
            var e = (NewReportEventArg)p;
            var reportCount = ReportBP.MakeNewReports(e.ReportDocument);

            AltaLog.AppJournal.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, "Reports Count " + reportCount);

            Process.Start("explorer", ReportBP.GetReportsPath());
        }

    }
}
