using CellGame.Classes;
using System.Web.Mvc;

namespace CellGame.Controllers
{
    public class LoserController : Controller
    {
        // GET: Loser
        public ActionResult Loser()
        {
            DatabaseUtils.submitAnswers();
            return View();
        }
    }
}