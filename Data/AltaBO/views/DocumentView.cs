using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaBO.views
{
    public class DocumentView
    {
        public int id { get; set; }
        public string name { get; set; }
        public int? siteId { get; set; }
        public string extension { get; set; }
        public int documentTypeId { get; set; }
        public string documentType { get; set; }
        public int? filesListId { get; set; }
        public string number { get; set; }
        public DateTime date { get; set; }
        public int? fileSectionId { get; set; }
        public string description { get; set; }
    }
}
