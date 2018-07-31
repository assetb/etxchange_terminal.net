using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AltaTransport.calendars
{
    public class ZimbraCalendar : ICalendar
    {
        const string PROTOCOL = "https";
        const string BASE_ADDRESS = "mx.altatender.kz";
        const string SENDER_MAIL = "adm.assist@altatender.kz";
        const string TITLE_ORGANIZER = "Помощник";
        const string PROFILE = "adm.assist";
        const string SENDER_PASS = "ikalta7&IKALTA&7";

        protected String GetAppointment(string attendee, DateTime start, string subject, string description, string uuid = null, string location = null, double duration = 30)
        {

            uuid = (!String.IsNullOrEmpty(uuid)) ? uuid : (Guid.NewGuid().ToString() + "altatender.kz");

            String appointment =
                "BEGIN:VCALENDAR" + System.Environment.NewLine +
                "PRODID:-//Zimbra//Service Calendar//EN" + System.Environment.NewLine +
                "VERSION:2.0" + System.Environment.NewLine +
                "METHOD:REQUEST" + System.Environment.NewLine +
                "BEGIN:VEVENT" + System.Environment.NewLine +
                string.Format("ATTENDEE:MAILTO:{0}", attendee) + System.Environment.NewLine +
                string.Format("UID:{0}altatender.kz", uuid) + System.Environment.NewLine +
                string.Format("DESCRIPTION:{0}", description) + System.Environment.NewLine +
                string.Format("LOCATION: {0}", String.IsNullOrEmpty(location) ? "" : location) + System.Environment.NewLine +
                string.Format("CLASS:PUBLIC") + System.Environment.NewLine +
                string.Format("DTSTART:{0:yyyyMMddTHHmmssZ}", start) + System.Environment.NewLine +
                string.Format("DTSTAMP:{0:yyyyMMddTHHmmssZ}", DateTime.UtcNow) + System.Environment.NewLine +
                string.Format("DTEND:{0:yyyyMMddTHHmmssZ}", start.AddMinutes(duration)) + System.Environment.NewLine +
                string.Format("X-ALT-DESC;FMTTYPE=text/html:{0}", description) + System.Environment.NewLine +
                string.Format("SUMMARY:{0}", subject) + System.Environment.NewLine +
                string.Format("ORGANIZER;CN={0}:MAILTO:{1}", TITLE_ORGANIZER, SENDER_MAIL) + System.Environment.NewLine +
                "END:VEVENT" + System.Environment.NewLine +
                "END:VCALENDAR";

            return appointment;
        }

        protected Boolean SendAppointment(string appointment)
        {

            var form = new MultipartFormDataContent();
            form.Add(new StringContent(appointment, Encoding.UTF8, "text/calendar"), "file", "Calendar");

            HttpClient httpClient = new HttpClient();
            ServicePointManager.ServerCertificateValidationCallback = delegate (
            Object obj, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors errors)
            {
                return (true);
            };
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(
                    ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", SENDER_MAIL, SENDER_PASS))));

            httpClient.BaseAddress = new Uri(PROTOCOL + "://" + BASE_ADDRESS);

            var responce = httpClient.PostAsync("/home/" + PROFILE + "/calendar?auth=ba&fmt=ics", form).Result;
            return responce.IsSuccessStatusCode;
        }

        protected bool SendNotifyToRecipient(string recipient, string subject, string description, string appointment)
        {

            System.Net.Mime.ContentType contentType = new System.Net.Mime.ContentType("text/calendar");
            contentType.Parameters.Add("method", "REQUEST");
            contentType.Parameters.Add("name", "invate.ics");

            AlternateView calendarView = AlternateView.CreateAlternateViewFromString(appointment.ToString(), contentType);
            MailMessage message = new MailMessage();
            message.From = new MailAddress(SENDER_MAIL, TITLE_ORGANIZER);
            message.To.Add(new MailAddress(recipient));
            message.Subject = subject;
            message.Body = description;
            message.AlternateViews.Add(calendarView);

            SmtpClient smtpClient = new SmtpClient();

            smtpClient.Host = BASE_ADDRESS;
            smtpClient.Port = 25;
            smtpClient.EnableSsl = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(SENDER_MAIL, SENDER_PASS);

            smtpClient.Send(message);

            return true;
        }

        public bool CreateAppointment(string recipient, DateTime start, string subject, string description, double duration, string location, string urlAction = "", string eventId = "")
        {

            var messageBody = description + (!String.IsNullOrEmpty(urlAction) ? System.Environment.NewLine + "Перейти к событию: " + urlAction : "");
            var appointment = GetAppointment(recipient, start, subject, messageBody, eventId, location, duration);


            if (!SendAppointment(appointment))
            {
                return false;
            }

            if (!SendNotifyToRecipient(recipient, subject, messageBody, appointment))
            {
                return false;
            }

            return true;

        }
    }
}
