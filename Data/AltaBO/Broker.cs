using System.Collections.Generic;

namespace AltaBO
{
    public class Broker
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Code { get; set; }
        public string Requisites { get; set; }

        public string Index { get; set; }
        public string Address { get; set; }
        public string Phones { get; set; }

        public Broker() { }

        public Broker(string code, string requisites)
        {
            Code = code;
            Requisites = requisites;
        }
    }
}
