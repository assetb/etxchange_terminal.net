using System;
using AltaTransport.model;

namespace SupervisionModel
{
    public class NewReportEventArg:EventArgs
    {
        public ReportDocument ReportDocument { get; set; }
        public int ReportNo { get; set; }
    }
}
