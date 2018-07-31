using SupervisionModel;
using System;

namespace SupervisionApp
{
    public class ApplicantESM : EventsSourceMonitorBase
    {
        public ApplicantESM(MonitorBO monitorArgs) : base(monitorArgs){}


        //public event EventHandler WarrantyProvisionPaidEvent;
        //public event EventHandler DebtsPaidEvent;
        //public event EventHandler AccountFundEnded;


        public override void Start()
        {
        }

        protected override void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
