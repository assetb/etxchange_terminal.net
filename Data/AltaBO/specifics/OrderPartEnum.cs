using altaik.baseapp.ext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaBO.specifics {
    public enum OrderPartEnum {
        [Name("Аукцион")]
        Auction = 1,

        [Name("Лот")]
        Lot = 2,

        [Name("Заявка на участие")]
        SupplierOrder = 3,

        [Name("Поручение")]
        Procuratory = 4,
    }
}
