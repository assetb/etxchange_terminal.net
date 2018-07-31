using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaBO
{
    public class EmployesConst
    {
        public List<Employe> Employes = new List<Employe>() { new Employe() {
            name = "Алия (aliyabroker@gmail.com)",
            fullName = "Алтынбек А.Б.",
            email = "aliyabroker@gmail.com",
            pass = "qhesllbpvlwnyrtu"
        },
        new Employe() {
            name = "Жадыра (zhadyrabroker@gmail.com)",
            fullName = "Майгаранова Ж.А.",
            email = "zhadyrabroker@gmail.com",
            pass = "kcdabfxzvrvypdln"
        },
        new Employe() {
            name = "Айнагуль ()",
            fullName = "Борамбаева А.С.",
            email = "",
            pass = ""
        },
            new Employe() {
            name = "Айнура (ainurbroker@gmail.com)",
            fullName = "Нурбосынова А.Д.",
            email = "ainurbroker@gmail.com",
            pass = "cbetafghbswptxmh"
        },
            new Employe() {
            name = "Игорь (igormbroker@gmail.com)",
            fullName = "Милошенко И.А.",
            email = "igormbroker@gmail.com",
            pass = "nbaoyfotzlccbpkt"
        },
            new Employe() {
            name = "Мунира (munirabroker@gmail.com)",
            fullName = "Азимова М.Д.",
            email = "munirabroker@gmail.com",
            pass = "fqydcuantypacydh"
        },
            new Employe() {
            name = "Альта и К (altkbroker@gmail.com)",
            fullName = "Альта и К",
            email = "altkbroker@gmail.com",
            pass = "ablrzaieneayisqa"
        },
            new Employe() {
            name = "Альтаир Нур (altairnurbroker@gmail.com)",
            fullName = "Альтаир Нур",
            email = "altairnurbroker@gmail.com",
            pass = "cozvmqqlicgdqmyq"
        },
        };

        public List<Employe> GetListOfEmployes()
        {
            return new List<Employe>(Employes);
        }
    }
}
