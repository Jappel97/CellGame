using CellGame.Classes;
using System.Web.Mvc;
using System;

namespace CellGame.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// this is the main page when you open the website
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (String.IsNullOrEmpty((string)Session["userId"]))
            {
                return RedirectToAction("IndexLogin", "Home");
            }
            return View();
        }

        /// <summary>
        /// takes you to the how to play page
        /// </summary>
        /// <returns></returns>
        public ActionResult HowToPlay()
        {
            return View();
        }

        /// <summary>
        /// Used for when the website is first opened. Presents the login front and center
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexLogin()
        {
            string test = (string)Session["userId"];

            if (!String.IsNullOrEmpty((string)Session["userId"]))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}