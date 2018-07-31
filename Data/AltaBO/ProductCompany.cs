using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaBO
{
    public class ProductCompany : Product
    {
        public string description { get; set; }
        public int? fileId { get; set; }
    }
}
