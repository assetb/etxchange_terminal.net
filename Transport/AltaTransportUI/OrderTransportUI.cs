using System.IO;

namespace AltaTransport
{
    public class OrderTransportUI
    {
        public static FileInfo GetVostokOrder()
        {
            return Service.GetFile("Выберите заявку от заказчика Восток", "Excel|*.xlsx;*.xls");
        }


        public static FileInfo GetTreatyDraft()
        {
            return Service.GetFile("Выберите проект договора", "Word|*.docx;*.doc");
        }


        public static FileInfo GetInkayOrder()
        {
            return Service.GetFile("Выберите заявку от заказчика", "Word|*.docx;*.doc");
        }


        public static DirectoryInfo GetETSOrderDirectory()
        {
            return Service.GetDirectory();
        }

    }
}
