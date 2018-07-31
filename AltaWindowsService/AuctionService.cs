using SupervisionApp;
using System.ServiceProcess;

namespace AltaWindowsService
{
    internal partial class AuctionService : ServiceBase
    {
        private SupervisionProxy _eventsProxy;


        public AuctionService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _eventsProxy = new SupervisionProxy();
            _eventsProxy.Start();
        }

        protected override void OnStop()
        {
            _eventsProxy.Close();
        }

        public void Start()
        {
            OnStart(new string[0]);
        }


        public bool IsStoped()
        {
            return _eventsProxy.IsClosed();
        }
    }
}
