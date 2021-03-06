﻿using System.Web.Mvc;

namespace PolymetalCabinet.Controllers
{
    public class AuctionsController : Controller
    {

        public AuctionsController()
        {
        }

        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return View();
            }
            else
            {
                return View("Auction", id);
            }
        }

        [HttpGet]
        public ActionResult History()
        {
            return View();
        }

        public ActionResult New()
        {
            return View();
        }
    }
}
