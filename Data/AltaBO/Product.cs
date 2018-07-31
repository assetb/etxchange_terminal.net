using AltaBO.specifics;

namespace AltaBO
{
    public class Product
    {
        public int id { get; set; }
        public string name { get; set; }
        public int unitId { get; set; }
        public string utin { get; set; }

        public string code { get; set; }
        public ClassificationsProductsEnum classification { get; set; } 
    }
}
