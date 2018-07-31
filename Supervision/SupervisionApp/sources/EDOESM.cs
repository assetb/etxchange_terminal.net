using SupervisionModel;
using AltaLog;
using AltaTransport;
using System;

namespace SupervisionApp
{
    /// <summary>
    /// Class to monitor a appearance of New Report
    /// </summary>
    public class NewReportESM:EventsSourceMonitorBase
    {
        public NewReportESM(MonitorBO monitorArgs) : base(monitorArgs) { }

        public event EventHandler<NewReportEventArg> NewReportEvent;

        public override void Start()
        {
            RunTimer();
        }

        protected override void Execute()
        {
            AppJournal.Write(GetType().Name + ":" + System.Reflection.MethodBase.GetCurrentMethod(),"Started...");

            var reportNo = 0;
            foreach(var report in ReportTransport.GetNew()) {
                reportNo++;

                AppJournal.Write(GetType().Name + ":" + System.Reflection.MethodBase.GetCurrentMethod()," report NO:" + reportNo);

                var eventArg = new NewReportEventArg { ReportDocument = report, ReportNo = reportNo};
                NewReportEvent?.Invoke(this, eventArg);
            }
        }


    }
}
