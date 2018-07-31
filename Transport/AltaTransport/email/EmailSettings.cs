namespace AltaTransport
{
    public class EmailSettings
    {
        public string Server { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        //public int Port { get; set; }
        public SecurityEnum Security { get; set; }
        public string Folder { get; set; }
    }
}
