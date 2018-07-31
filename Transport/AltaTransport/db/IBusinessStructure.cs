using AltaBO;
using System.Collections.Generic;

namespace AltaTransport.db
{
    public interface IBusinessStructure
    {
        List<Product> FindProducts(string searchString, int? offset = null, int? limit = null);
    }
}
