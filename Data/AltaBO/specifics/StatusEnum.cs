using altaik.baseapp.ext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaBO.specifics
{
    public enum StatusEnum
    {
        [Name("Новый (-ая)")]
        New = 1,

        [Name("Допущен")]
        Admitted = 15,

        [Name("Не допущен")]
        NotAllowed = 16,

        [Name("Подтвержден (-ая)")]
        Confirmed = 17,

        [Name("Заявленная")]
        Declared = 22,

        [Name("Победил")]
        Won = 23,

        [Name("Проиграл")]
        Lost = 24,

        [Name("Сделка заключена")]
        TheTransactionIsConcluded = 25
    }
}
