using System;

namespace AltaBO
{
    [Serializable()]
    public class PathSettings
    {
        public string RootPath { get; set; }
        public string OrdersPath { get; set; }
        public string EDOPath { get; set; }
        public string EDOReportsPath { get; set; }
        public string JournalC01Path { get; set; }
        public string EntryOrdersPath { get; set; }
        public string TemplatesPath { get; set; }
    }
}
