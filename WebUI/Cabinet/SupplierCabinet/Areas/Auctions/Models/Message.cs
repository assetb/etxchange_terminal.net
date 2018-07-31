using System.Collections.Generic;

namespace PersonalCabinetSupplier.Areas.Auctions.Models
{

    public class Message
    {
        public List<string> searchValues { get; set; }
        public string searchSelectedValue { get; set; }
        public string searchText { get; set; }

        public List<Order> orders { get; set; }
    }
}