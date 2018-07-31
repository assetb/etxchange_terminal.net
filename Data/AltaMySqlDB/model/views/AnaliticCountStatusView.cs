using AltaMySqlDB.model.tables;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.views
{
    public class AnaliticCountStatusView
    {
        public int Status { get; set; }
        public int Count { get; set; }
        public StatusEF StatusEf { get; set; }
    }
}
