using CellGame.Classes;
using CellGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CellGame.Controllers
{
    public class StudentsController : Controller
    {
        // GET: Students
        /// <summary>
        /// this method can only be reached by Admins and Professors and TAs
        /// </summary>
        /// <returns>a list of all the users who use the app</returns>
        [Authorize(Roles = "Admin, Professor, TA")]
        public ActionResult Index()
        {
            if (String.IsNullOrEmpty((string)Session["userId"]))
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                Session["studentList"] = new StudentsList();
                DatabaseUtils.getStudents();
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "STUCON01";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            return View(Session["studentList"]);
        }

        /// <summary>
        /// this method is used to show what a student got on each of their completed scenarios (THIS IS THE TA,PROFESSOR,ADMIN VIEW)
        /// </summary>
        /// <param name="id">user id</param>
        /// <param name="name">student name</param>
        /// <returns>a list of the students grades</returns>
        [Authorize(Roles = "Admin, Professor, TA")]
        public ActionResult Grades(string id, string name)
        {
            if (String.IsNullOrEmpty((string)Session["userId"]))
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                Session["studentGrades"] = DatabaseUtils.getStudentGrades(id);
                ((StudentGrades)Session["studentGrades"]).studentName = name;
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "STUCON02";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            return View(((StudentGrades)Session["studentGrades"]));
        }

        /// <summary>
        /// This method takes you to a screen to modify user roles (Student/TA/Professor/Admin)
        /// </summary>
        /// <param name="i">user id</param>
        /// <returns>a student (you can edit the users role in this screen)</returns>
        [Authorize(Roles = "Admin")]
        public ActionResult Role(int i)
        {
            if (String.IsNullOrEmpty((string)Session["userId"]))
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                ViewBag.RolesList = Singleton.RoleSelectList;
                Session["studentToEdit"] = ((StudentsList)Session["studentList"]).studentsList[i];
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "STUCON03";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            return View(((Student)Session["studentToEdit"]));
        }

        /// <summary>
        /// This page takes you to the intial index page of Students, but it saves the new user role you selected for a user
        /// </summary>
        /// <param name="role">the role you want to change to</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin, Professor, TA")]
        public ActionResult Index(String role)
        {
            if (String.IsNullOrEmpty((string)Session["userId"]))
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                String newRole = Request["role"];
                DatabaseUtils.changeUserRole(newRole, ((Student)Session["studentToEdit"]).id);

                Session["studentList"] = new StudentsList();
                DatabaseUtils.getStudents();
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "STUCON04";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            return View(Session["studentList"]);
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
        public ActionResult EditGrade(int stgID, int stgScenarioID, string stgStudentID)
        {
            if (String.IsNullOrEmpty((string)Session["userId"]))
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                Session["scenarioToGrade"] = null;
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
        /// <summary>
        /// this function is called when grading a scenario and a grade is entered and submitted.
        /// We assume they want to select another to grade so we bring them back to the list of scenarios completed but not yet graded
        /// </summary>
        /// <param name="stg">PARAM is not used it is there so we can have the same method name but one as a GET and the other as a POST</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Grades(ScenariosToGrade stg)
        {
            if (String.IsNullOrEmpty((string)Session["userId"]))
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                ((ScenarioToGrade)Session["scenarioToGrade"]).stgID = Convert.ToInt32(Request["stgId"]);
                ((ScenarioToGrade)Session["scenarioToGrade"]).stgGrade = Convert.ToInt32(Request["stgGrade"]);
                ((ScenarioToGrade)Session["scenarioToGrade"]).stgComments = Request["stgComments"];

                DatabaseUtils.addGradeForScenario(((ScenarioToGrade)Session["scenarioToGrade"]).stgID, ((ScenarioToGrade)Session["scenarioToGrade"]).stgGrade, ((ScenarioToGrade)Session["scenarioToGrade"]).stgComments);
                String name = ((StudentGrades)Session["studentGrades"]).studentName;
                Session["studentGrades"] = DatabaseUtils.getStudentGrades(((ScenarioToGrade)Session["scenarioToGrade"]).stgStudentID);
                ((StudentGrades)Session["studentGrades"]).studentName = name;
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "STUCON02";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            return View(((StudentGrades)Session["studentGrades"]));
        }
    }
}