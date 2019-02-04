using CellGame.Classes;
using CellGame.Models;
using System;
using System.Web.Mvc;

namespace CellGame.Controllers
{
    [Authorize]
    public class GradesController : Controller
    {
        /// <summary>
        /// gets a list of an individuals grades
        /// </summary>
        /// <returns>view of the list of their grades</returns>
        public ActionResult Grades()
        {
            if (String.IsNullOrEmpty((string)Session["userId"]))
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                Session["studentGrades"] = DatabaseUtils.getStudentGrades((string)Session["userId"]);
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "GRACON01";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            return View((StudentGrades)Session["studentGrades"]);
        }

        public ActionResult PendingGrades()
        {
            return View(DatabaseUtils.getScenariosToGrade());
        }
        
    }
}