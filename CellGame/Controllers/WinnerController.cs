using CellGame.Classes;
using System.Web.Mvc;

namespace CellGame.Controllers
{
    public class WinnerController : Controller
    {
        // GET: Winner
        public ActionResult Winner()
        {
            DatabaseUtils.submitAnswers();
            return View();
        }
    }
}