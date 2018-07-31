using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using altaik.baseapp.ext;

namespace AltaArchiveUI.service {
    public enum PresentationEnum {
        [Name("Архивное представление")]
        Archive = 1,
        [Name("Биржевое представление")]
        Exchange = 2
    }
}
