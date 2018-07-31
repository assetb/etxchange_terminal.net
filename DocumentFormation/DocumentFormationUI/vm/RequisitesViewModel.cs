using System;
using altaik.baseapp.vm;

namespace DocumentFormation.vm
{
    public class RequisitesViewModel : BaseViewModel
    {
        public RequisitesViewModel()
        {
            Number = "1";
            Date = DateTime.Now;
        }

        public string number;
        public string Number {
            get { return number; }
            set { if (value != number) { number = value; RaisePropertyChangedEvent("Number"); } }
        }

        public DateTime date;
        public DateTime Date {
            get { return date; }
            set { if (value != date) { date = value; RaisePropertyChangedEvent("Date"); } }
        }

        public string Title {
            get {
                return Number + " от " + RoundedDate(Date);
            }
        }

        private string RoundedDate(DateTime dateTime)
        {
            string s;

            s = dateTime.ToString().Substring(dateTime.ToString().LastIndexOf(":") - 2);

            return dateTime.ToString().Replace(s, "00:00");
        }
    }
}
