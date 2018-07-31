using System.Collections.ObjectModel;

namespace AltaBO
{
    public class WaitingList
    {
        public string orderNumber { get; set; }
        public string sourceNumber { get; set; }
        public string orderDate { get; set; }
        public string lotCode { get; set; }
        public ObservableCollection<WaitingListTable> waitingListTable { get; set; }
    }
}
