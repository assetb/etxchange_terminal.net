using AltaBO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaReportsApp.services
{
    public static class DictionariesService
    {
        public static List<Broker> ReadBrokers()
        {
            return ReportManager.dbManager.ReadBrokers();
        }
    }
}
