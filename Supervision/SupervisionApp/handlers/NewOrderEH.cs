using AltaWindowsNotification;
using SupervisionModel;
using System.Diagnostics;
using AltaTransport;
using AltaBO.specifics;
using altaik.baseapp.ext;

namespace SupervisionApp
{
    /// <summary>
    /// Class to handle new orderDocument event appearance
    /// </summary>
    public class NewOrderEH
    {

        public void NewOrderEventHandler(object sender, NewOrderEventArgs e)
        {
            var toast = new ToastNotification();
            toast.Show("Пришла заявка от " +  e.OrderDocument.Customer.GetName(), NewOrderReaction,e);
        }

        private static void NewOrderReaction(object p)
        {
            var info = new ProcessStartInfo {FileName = AppTransport.GetHostAppFileName()};
            var e = (NewOrderEventArgs)p;
            info.Arguments = BusinessObjectsEnum.Order.GetName() + " " +  e.OrderDocument.Customer.GetName() + string.Join(" ", e.OrderDocument.OrderFileNames);
            Process.Start(info);
        }


    }
}
