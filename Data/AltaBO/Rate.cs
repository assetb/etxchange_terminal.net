using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaBO
{
    public class Rate
    {
        public int id { get; set; }
        public decimal transaction { get; set; }
        public decimal percent { get; set; }
        public string description { get; set; }
        public int ratesListId { get; set; }
    }
}
