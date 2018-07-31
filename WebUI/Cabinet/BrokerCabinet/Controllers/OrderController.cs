using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BrokerCabinet.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult OrderInfo(int? id)
        {
            if (id == null) return View();
            else
            {
                ViewBag.currentId = id;
                return View("OrderInfo");
            }
        }
    }
}