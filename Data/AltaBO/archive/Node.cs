using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaBO.archive {
    public class Node {
        public int Id { get; set; }
        public int Parent_id { get; set; }
        public int Level_id { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
    }
}
