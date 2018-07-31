using System.Collections.Generic;

namespace AltaBO
{
    public class SupplierWithProduct : Supplier
    {
        public Supplier supplier { get; set; }
        public List<Product> products { get; set; }
    }
}