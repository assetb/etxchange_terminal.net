using AltaBO;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;

namespace AltaTransport
{
    public class EmailSender
    {
        public static void Send(string senderMail, string senderPass, string recipient, string subject, string content, ObservableCollection<AttachedFiles> attach = null)
        {
            var checkError = "";

            try {
                var mailMsg = new MailMessage();

                mailMsg.To.Add(new MailAddress(recipient));
                mailMsg.Subject = subject;
                mailMsg.Body = content;

                if (attach != null) {
                    foreach (var item in attach) {
                        var att = (AttachedFiles)item;
                        mailMsg.Attachments.Add(new System.Net.Mail.Attachment(att.fullName));
                    }
                }

                var smtpClient = new SmtpClient {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false
                };


                mailMsg.From = new MailAddress(senderMail);
                smtpClient.Credentials = new NetworkCredential(senderMail, senderPass);

                smtpClient.Send(mailMsg);
            } catch (Exception ex) {
                Debug.Write("Mail.Send: " + ex.Message);
                checkError = ex.Message;
            }

            if (checkError == "") Debug.Write("Письмо отправлено");
        }
    }
}
