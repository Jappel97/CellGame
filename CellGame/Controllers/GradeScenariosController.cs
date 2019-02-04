using CellGame.Classes;
using CellGame.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CellGame.Controllers
{
    [Authorize(Roles = "Admin, Professor, TA")]
    public class GradeScenariosController : Controller
    {
        /// <summary>
        /// this method goes and grabs all of the scenarios that need to be graded (the ones which users have played and successfully completed)
        /// </summary>
        /// <returns>View->GradeScenario->Index with a model of ScenariosToGradeList</returns>
        public ActionResult Index()
        {
            if (String.IsNullOrEmpty((string)Session["userId"]))
            {
                return RedirectToAction("Login", "Account");
            }
            ScenariosToGradeList stgList;
            try
            {
                stgList = DatabaseUtils.getScenariosToGrade();
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "GRASEN01";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            return View(stgList);
        }

        /// <summary>
        /// this function is called when grading a scenario and a grade is entered and submitted.
        /// We assume they want to select another to grade so we bring them back to the list of scenarios completed but not yet graded
        /// </summary>
        /// <param name="stg">PARAM is not used it is there so we can have the same method name but one as a GET and the other as a POST</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(ScenariosToGrade stg)
        {
            if (String.IsNullOrEmpty((string)Session["userId"]))
            {
                return RedirectToAction("Login", "Account");
            }
            ScenariosToGradeList stgList;
            try
            {
                ((ScenarioToGrade)Session["scenarioToGrade"]).stgID = Convert.ToInt32(Request["stgId"]);
                ((ScenarioToGrade)Session["scenarioToGrade"]).stgGrade = Convert.ToInt32(Request["stgGrade"]);
                ((ScenarioToGrade)Session["scenarioToGrade"]).stgComments = Request["stgComments"];

                DatabaseUtils.addGradeForScenario(((ScenarioToGrade)Session["scenarioToGrade"]).stgID, ((ScenarioToGrade)Session["scenarioToGrade"]).stgGrade, ((ScenarioToGrade)Session["scenarioToGrade"]).stgComments);
                //Call database command passing it Singleton.scenarioToGrade
                stgList = DatabaseUtils.getScenariosToGrade();
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "GRASEN02";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            return View(stgList);
        }

        /// <summary>
        /// GET Request
        /// This is called from the Index page of GradeScenario when they click on an ungraded scenario from the scenario list
        /// it takes them to the scenario to view the questions, answers, and comments
        /// </summary>
        /// <param name="stgID">id of the scenario that needs to be graded (this table id contains the questions, answers, and comments of the 
        /// user who completed the scenario)</param>
        /// <param name="stgScenarioID">scenario id that needs to be graded</param>
        /// <param name="stgStudentID">student id of the user who created the scenario</param>
        /// <returns></returns>
        public ActionResult GradeScenario(int stgID, int stgScenarioID, string stgStudentID)
        {
            if (String.IsNullOrEmpty((string)Session["userId"]))
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                Session["scenarioToGrade"] = DatabaseUtils.getScenarioToGrade(stgID, stgScenarioID, stgStudentID);
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "GRASEN03";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            return View(((ScenarioToGrade)Session["scenarioToGrade"]));
        }

        /*
        /// <summary>
        /// Deletes a grade - CURRENT WIP
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult DeleteGrade()
        {
            if (String.IsNullOrEmpty((string)Session["userId"]))
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                int stgID = Convert.ToInt32(Request["STG_ID"]);
                DatabaseUtils.deleteGrade(stgID);
                return this.Index();
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "GRASEN04";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// Confirms that the person wants to delete the grade - CURRENT WIP
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult ConfirmDeleteGrade(int stgID)
        {
            try
            {
                return PartialView("_PartialDeleteGrade");
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "GRASEN05";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        */
    }
}