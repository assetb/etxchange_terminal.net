using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AltaBO;

namespace BrokerCabinet.Controllers {
    public class AuctionsController : Controller {
        [HttpGet]
        public ActionResult Index(int? id) {
            if(id == null) return View();
            else return View("Auction", id);
        }

        [HttpGet]
        public ActionResult History(int? id) {
            if(id == null) return View();
            else return View("Auction", id);
        }

        [HttpGet]
        public ActionResult Order(int? id)
        {
            if (id == null) return View();
            else return View(id);
        }  

        //FIX
        [HttpGet]
        public ActionResult NewAuction()
        {
              
            return View();
        }  
    }
}