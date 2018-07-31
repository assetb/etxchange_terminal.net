using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaBO
{
    public class ConficurationsBroker
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string user { get; set; }
        public string pass { get; set; }
        public int? brokerId { get; set; }
    }
}
