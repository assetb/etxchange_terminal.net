using AltaLog;
using AltaTransport.model;
using System;
using System.Collections.Generic;
using System.IO;

namespace AltaTransport
{
    /// <summary>
    /// It is used for checking if new orders exist and to return new orders.
    /// New order are recieved from email.
    /// Current version uses EmailImapClient to access to email.
    /// </summary>
    public static class OrderTransport
    {
        //public static EmailSettings Settings { get; } = new EmailSettings() {
        //    Folder = "INBOX",
        //    Server = "imap.gmail.com",
        //    User = "andreyzenin2014@gmail.com",
        //    Pass = "mdybcuzamplwxjjl",
        //    Security = SecurityEnum.SSL
        //};

        public static EmailSettings Settings { get; } = new EmailSettings() {
            Folder = "INBOX",
            Server = "imap.gmail.com",
            User = "asetbn@gmail.com",
            Pass = "kazakM$N",
            Security = SecurityEnum.SSL
        };

        private static EmailImapClient _imap;

        public static bool HasNew()
        {
            if (_imap == null) _imap = new EmailImapClient(Settings);

            return _imap.HasNewMessages();
        }

        public static List<OrderDocument> GetNew()
        {
            if (_imap == null) return null;

            var orders = new List<OrderDocument>();

            var messageHeads = _imap.GetNewMessageHeads();
            if (messageHeads == null) return null;
            foreach(var head in messageHeads) {
                var message = _imap.GetMessage(head.UID);
                if (message?.Attachments == null) continue;
                var fileNames = new List<string>();
                foreach (var attach in message.Attachments) {
                    if (attach == null) continue;
                    var attachFileName = FileArchiveTransport.GetCustomerOrderPath() + attach.Text;
                    try {
                        File.WriteAllBytes(attachFileName, attach.Body);
                        fileNames.Add(attachFileName);
                    } catch(Exception ex) {
                        //Debug.WriteLine("NewOrderTransport.GetNew: " + ex.Message);
                        AppJournal.Write("NewOrderTransport.GetNew: " + ex.Message);
                    }
                }
                orders.Add(new OrderDocument() { OrderFileNames = fileNames });
            }
            return orders;
        }
    }
}
