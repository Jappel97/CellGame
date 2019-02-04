using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CellGame.Classes;

namespace CellGame.Controllers
{
    [Authorize(Roles = "Admin, Professor, TA")]
    public class ScenariosToGradeController : Controller
    {
        // GET: ScenariosToGrade
        public ActionResult Index()
        {
            if (String.IsNullOrEmpty((string)Session["userId"]))
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
    }
}