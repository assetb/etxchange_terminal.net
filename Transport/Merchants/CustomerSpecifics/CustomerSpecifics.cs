using AltaTransport.model;

namespace MerchantBP
{
    public class CustomerSpecifics
    {
        public static bool IsOrder(string fileName)
        {
            return fileName.ToLower().Contains("заявка");
        }


        public static bool IsVostokOrder(string fileName)
        {
            return IsOrder(fileName) && (fileName.EndsWith("xlsx") || fileName.EndsWith("xls"));
        }


        public static bool IsVostokOrder(OrderDocument orderDocument)
        {
            return IsVostokOrder(orderDocument.OrderFileNames[0]);
        }


        public static bool IsInkayOrder(string fileName)
        {
            return IsOrder(fileName) && (fileName.EndsWith("docx") || fileName.EndsWith("doc"));
        }


        public static bool IsInkayOrder(OrderDocument orderDocument)
        {
            return IsInkayOrder(orderDocument.OrderFileNames[0]);
        }

    }
}
