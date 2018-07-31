using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltaBO;

namespace AltaBO.reports
{
    public class ContractsReportView:Contract
    {
        public string brokerName { get; set; }
        public string companyName { get; set; }
        public string authorName { get; set; }
        public int documentAttach { get; set; }
        public string scanTypeName { get; set; }
    }
}
