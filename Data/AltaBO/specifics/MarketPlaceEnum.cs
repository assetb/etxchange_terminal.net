using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using altaik.baseapp.ext;

namespace AltaBO.specifics {
    public enum MarketPlaceEnum {
        [Name("Все")]
        ForAll = 0,

        [Name("УТБ")]
        UTB = 1,

        [Name("ЕТС")]
        ETS = 4,

        [Name("КазЭТС")]
        KazETS = 5,

        [Name("Каспи")]
        Caspy = 6
    }
}
