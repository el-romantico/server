using Rituals.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rituals.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            GameRoom.DropAllPlayers();
            ViewBag.Title = "Home Page";

            return View();
        }
    }
}
