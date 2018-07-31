using AltaLog;
using System.Threading;

namespace AltaWindowsService
{
    public static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        public static void Main() {
            AppJournal.HasWinJournal = true;
            // For battle
            //ServiceBase[] ServicesToRun;
            //ServicesToRun = new ServiceBase[]
            //{
            //    new AuctionService()
            //};
            //ServiceBase.Run(ServicesToRun);

            // For test
            var service = new AuctionService();
            service.Start();
            //Thread.Sleep(750000);
            while (!service.IsStoped()) Thread.Sleep(1000);
        }
    }
}
