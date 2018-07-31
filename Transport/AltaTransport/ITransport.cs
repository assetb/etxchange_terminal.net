using System.Collections.Generic;
using AltaBO;

namespace AltaTransport
{
    public interface ITransport
    {
        bool HasNew();
        List<IAltaBO> GetNew();
    }
}
