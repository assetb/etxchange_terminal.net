using AltaBO;
using System.Collections.Generic;
using AltaBO.specifics;

namespace AltaTransport.model
{
    public class OrderDocument:IAltaBO
    {
        public List<string> OrderFileNames;
        public CustomersEnum Customer;
    }
}
