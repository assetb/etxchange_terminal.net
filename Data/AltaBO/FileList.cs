using System.Collections.Generic;

namespace AltaBO
{
    public class FileList
    {
        public int id { get; set; }
        public string description { get; set; }

        public List<DocumentRequisite> files { get; set; }
    }
}
