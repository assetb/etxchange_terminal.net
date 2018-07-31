namespace AltaTransport
{
    public class DesktopClient
    {
        public static void StartListening(string pipeName, DelegateMessage eventHandler)
        {
            var server = new PipeServer();
            server.PipeData += eventHandler;
            server.Listen(pipeName);
        }

        private static PipeClient client;
        public static void Send(string pipeName, object sendObject)
        {
            if(client == null) client = new PipeClient();
            client?.Send(sendObject, pipeName);
        }
    }
}
