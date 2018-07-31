using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaBO
{
    public class Regulation
    {
        public int id { get; set; }
        public DateTime openDate { get; set; }
        public DateTime closeDate { get; set; }
        public DateTime applyDate { get; set; }
        public DateTime applyDeadLine { get; set; }
        public DateTime applicantsDeadLine { get; set; }
        public DateTime provisionDeadLine { get; set; }
    }
}
