using AltaBO.archive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaArchiveApp.models {
    public class PresentTree {
        public List<PresentTree> Child { get; set; }
        public Node Node { get; set; }
    }
}
