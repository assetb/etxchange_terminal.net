using System.Collections.Generic;

namespace AltaTransport
{
    public class Email
    {
        public long UID { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Date { get; set; }
        public string Size { get; set; }
        public string TextBody { get; set; }
        public List<Attachment> Attachments { get; set; }

        public Email()
        {
            Attachments = new List<Attachment>();
        }

    }
}
