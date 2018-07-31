using PersonalCabinetSupplier.Areas.Auctions.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace PersonalCabinetSupplier.Areas.Auctions.Controllers
{
    public class AuctionController : Controller
    {
        Message message;
        public void Init()
        {
            message = new Message();

            var orders = new List<Order>();

            orders.Add(new Order() { Date = System.DateTime.Now, Number = "test1" });
            orders.Add(new Order() { Date = System.DateTime.Now, Number = "test2" });
            orders.Add(new Order() { Date = System.DateTime.Now, Number = "test3" });
            orders.Add(new Order() { Date = System.DateTime.Now, Number = "test4" });
            orders.Add(new Order() { Date = System.DateTime.Now, Number = "test5" });
            orders.Add(new Order() { Date = System.DateTime.Now, Number = "test6" });

            
            message.orders = orders;
            message.searchValues = new List<string>() {
                "По номеру"
            };
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(new Message());
        }

        [HttpPost]
        public ActionResult Orders(Message message)
        {
            Init();
            return Json(this.message);
        }

        [HttpPost]
        public ActionResult Search(Message message) {
            return Json(this.message);
        }
    }
}
