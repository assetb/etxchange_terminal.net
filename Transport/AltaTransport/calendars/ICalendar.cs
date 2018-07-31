using System;

namespace AltaTransport.calendars
{
    public interface ICalendar
    {
        bool CreateAppointment(string recipient, DateTime start, string subject, string description, double duration, string location, string urlAction = "", string eventId = "");
    }
}
