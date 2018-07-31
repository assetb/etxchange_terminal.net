using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonalCabinetSupplier.Areas.Search.Models
{
    public class SearchModel<Paraments, Object>
    {
        public Paraments paramets { get; set; }
        public List<Object> objects { get; set; }
    }
}