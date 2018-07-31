using KazMineralsCabinet.Models;
using System.Collections.Generic;

namespace KazMineralsCabinet.utils
{
    public class DataManagerTest
    {
        List<OrderForReport> orderForReport;
        List<Status> statuses;
        public const int STATUS_OK = 2;
        public const int STATUS_ERROR = 3;

        public DataManagerTest()
        {

            statuses = new List<Status>
            {
               new Status { id = STATUS_OK, name= "Состоялся"},
               new Status { id = STATUS_ERROR, name= "Не состоялся"}
            };

            orderForReport = new List<OrderForReport>()
            {
                new OrderForReport { number = "101/14-11", sum = 200000, sumFinal = 160000, statusId = statuses.Find(s => s.id == STATUS_OK).id, status = statuses.Find(s => s.id == STATUS_OK).name},
                new OrderForReport { number = "102/14-11", sum = 100000, sumFinal = 80000, statusId = statuses.Find(s => s.id == STATUS_OK).id, status = statuses.Find(s => s.id == STATUS_OK).name},
                new OrderForReport { number = "103/14-11", sum = 12200000, sumFinal = 10111950, statusId = statuses.Find(s => s.id == STATUS_ERROR).id, status = statuses.Find(s => s.id == STATUS_ERROR).name},
                new OrderForReport { number = "104/14-11", sum = 5000, sumFinal = 3000, statusId = statuses.Find(s => s.id == STATUS_OK).id, status = statuses.Find(s => s.id == STATUS_OK).name},
                new OrderForReport { number = "105/14-11", sum = 400000, sumFinal = 310000, statusId = statuses.Find(s => s.id == STATUS_OK).id, status = statuses.Find(s => s.id == STATUS_OK).name},
                new OrderForReport { number = "106/14-11", sum = 200000, sumFinal = 160000, statusId = statuses.Find(s => s.id == STATUS_OK).id, status = statuses.Find(s => s.id == STATUS_OK).name}
            };

        }

        public List<OrderForReport> GetOrders()
        {
            return orderForReport;
        }

        public OrderForReport GetOrder(int id)
        {
            return orderForReport.Find(o => o.id == id);
        }

        public OrderForReport GetOrder(string number)
        {
            return orderForReport.Find(o => o.number == number);
        }
    }
}