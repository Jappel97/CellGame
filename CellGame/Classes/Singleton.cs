using CellGame.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace CellGame.Classes
{
    /*
     * This class contains instances of objects that should be universally accessible to a user
     * or pulled from the database once and used thereafter without having to go back to the database.
     */
    public class Singleton
    {
        public static Scenario currentScenario { get; set; }
        public static int selectedAnswer { get; set; }
        public static string userId { get; set; }
        public static string userName { get; set; }
        public static StudentsList studentList { get; set; }
        public static AnswersToGrade answersToGrade { get; set; }
        public static StudentGrades studentGrades { get; set; }
        public static ScenarioToGrade scenarioToGrade { get; set; }
        public static EditScenarioModel editScenario { get; set; }
        public static Student studentToEdit { get; set; }
        public static Dictionary<String, String> roles { get; set; }
        public static bool testing { get; set; }
        /// <summary>
        /// used to populate the roles select list when changing a users role
        /// </summary>
        public static SelectList RoleSelectList
        {
            get
            {
                return Singleton.roles == null ? new SelectList(new List<String>()) : new SelectList(Singleton.roles, "Key", "Value");
            }
        }
        //for error screen
        public static string errorCode { get; set; }
        public static string errorMsg { get; set; }
        public static bool isError { get; set; }
        public static string warningMsg { get; set; }
        public static bool isWarning { get; set; }


        /// <summary>
        /// This method is used to write a log named errorLog.txt in the root of the application on the server that is running it.
        /// We use this so that the user does not get the stack trace but they get an error code so they can let their professor or someone
        ///    know what the error is 
        /// </summary>
        /// <param name="errorCode"> error code that is displayed to the user</param>
        /// <param name="msg"> error message from the error</param>
        /// <param name="trace">stack trace from the error</param>
        public static void writeErrorToFile(string errorCode, string msg, string trace)
        {
            try
            {
                using (StreamWriter writeError = new StreamWriter(System.Web.Hosting.HostingEnvironment.MapPath("~/Logs/errorLog.txt"), true))
                {
                    writeError.WriteLine(DateTime.Now + " ERROR: Code: " + errorCode + " Message: " + msg); // Write the file.
                    writeError.WriteLine(DateTime.Now + " ERROR: Stack Trace: ");
                    writeError.WriteLine(trace); // Write the file.
                    writeError.WriteLine("");
                }
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "SIN01";
                //Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                Console.Write("" + Singleton.errorCode + "\n" + ex.Message + "\n" + ex.StackTrace);
            }
        }
    }
}