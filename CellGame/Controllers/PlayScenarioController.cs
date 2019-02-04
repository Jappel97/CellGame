using CellGame.Classes;
using CellGame.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Web;

namespace CellGame.Controllers
{
    [Authorize]
    public class PlayScenarioController : Controller
    {
        /// <summary>
        /// this is called when they initially start a scenario and after every answer and comment unless it is the end of the scenario
        /// </summary>
        /// <param name="scenarioID">scenario identifier for the scenario they are playing</param>
        /// <param name="nextQuestion">the next question in the scenario</param>
        /// <param name="newGame">if they just started a new game of if they are continuing a game (T/F)</param>
        /// <param name="wasAnswered">if a question was answered (T/F)</param>
        /// <returns></returns>
        public ActionResult playScenario(int scenarioID, int nextQuestion, bool newGame, bool wasAnswered)
        {
            if (String.IsNullOrEmpty((string)Session["userId"]))
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                if (newGame)
                {
                    ModelState.Clear();
                    Session["currentScenario"] = new Scenario();
                    Session["answersToGrade"] = new AnswersToGrade();
                    LoadScenario(scenarioID, nextQuestion);
                    ((Scenario)Session["currentScenario"]).currentQuestion = nextQuestion;
                }
                else
                {
                    if (wasAnswered)
                    {
                        addAnswerToList();
                        ((Scenario)Session["currentScenario"]).currentQuestion = this.nextQuestion(((Scenario)Session["currentScenario"]).questions[((Scenario)Session["currentScenario"]).currentQuestion].selectedAnswer, ((Scenario)Session["currentScenario"]).currentQuestion);
                    }
                    if (nextQuestion == 0)
                    {
                        return RedirectToAction("ScenarioComplete", "PlayScenario");
                    }
                }
                
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "PLYSENCON01";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            return View("PlayScenario", ((Scenario)Session["currentScenario"]));
        }

        public ActionResult playScenario2(int scenarioID, int nextQuestion, int selectedAnswer, bool newGame, bool wasAnswered)
        {
            try
            {
                int currQuestion = ((Scenario)Session["currentScenario"]).currentQuestion;
                ((Scenario)Session["currentScenario"]).questions[currQuestion].selectedAnswer = selectedAnswer;
                return this.playScenario(
                            ((Scenario)Session["currentScenario"]).scenarioID,
                            ((Scenario)Session["currentScenario"]).questions[((Scenario)Session["currentScenario"]).currentQuestion].answerList[selectedAnswer].nextQuestion,
                            false,
                            true
                        );
            }
            catch(Exception ex)
            {
                Singleton.errorCode = "PLYSENCON02";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                HttpCookie erck = new HttpCookie("PlayError");
                erck.Values.Add("Code", Singleton.errorCode);
                erck.Values.Add("Message", ex.Message);
                erck.Values.Add("Trace", ex.StackTrace);
                erck.Expires = DateTime.Now.AddDays(2);
                Response.Cookies.Add(erck);
                return RedirectToAction("ScenarioError", "Error");
                throw ex;
            }
        }

        /// <summary>
        /// This method is called from the student comment UI
        /// It is used to determine if we are done playing or if there are more questions left to answer
        /// If the scenario ends it calls scenario complete
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PlayScenario()
        {
            if (String.IsNullOrEmpty((string)Session["userId"]))
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                if (ModelState.IsValid)
                {

                    int currQuestion = ((Scenario)Session["currentScenario"]).currentQuestion;
                    int selectedAnswer = Convert.ToInt32(Request["selectedAnswer"]);
                    ((Scenario)Session["currentScenario"]).questions[currQuestion].selectedAnswer = selectedAnswer;
                    ((Scenario)Session["currentScenario"]).questions[currQuestion].answerList[selectedAnswer].answerComment = Request["answerComment"];

                    if (((Scenario)Session["currentScenario"]).questions[currQuestion].answerList[selectedAnswer].nextQuestion == 0)
                    {
                        addAnswerToList();
                        return RedirectToAction("ScenarioComplete", "PlayScenario");
                    }
                    return this.playScenario(
                        ((Scenario)Session["currentScenario"]).scenarioID,
                        ((Scenario)Session["currentScenario"]).questions[((Scenario)Session["currentScenario"]).currentQuestion].answerList[selectedAnswer].nextQuestion,
                        false,
                        true
                    );
                }
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "PLYSENCON02";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                HttpCookie erck = new HttpCookie("PlayError");
                erck.Values.Add("Code", Singleton.errorCode);
                erck.Values.Add("Message", ex.Message);
                erck.Values.Add("Trace", ex.StackTrace);
                erck.Expires = DateTime.Now.AddDays(2);
                Response.Cookies.Add(erck);
                return RedirectToAction("ScenarioError", "Error");
                throw ex;
            }
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// this is used to display the page when a user selects an answer and they are prompted to comment why they chose thier answer
        /// </summary>
        /// <param name="currentQuestion">question identifier that they answerd</param>
        /// <param name="selectedAnswer">the answer identifier which they chose</param>
        /// <returns></returns>
        public ActionResult StudentComment(int currentQuestion, int selectedAnswer)
        {
            if (String.IsNullOrEmpty((string)Session["userId"]))
            {
                return RedirectToAction("Login", "Account");
            }
            ((Scenario)Session["currentScenario"]).currentQuestion = currentQuestion;
            ((Scenario)Session["currentScenario"]).questions[currentQuestion].selectedAnswer = selectedAnswer;
            return View(((Scenario)Session["currentScenario"]));
        }

        /// <summary>
        /// when they get the the last question in a scenario and they answer it (with a comment) it will ask them if they want to play another
        /// scenario after it submits their answers into the Scenarios to be graded table
        /// </summary>
        /// <returns></returns>
        public ActionResult ScenarioComplete()
        {
            if (String.IsNullOrEmpty((string)Session["userId"]))
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                if(Session["testing"] != null)
                {
                    if((bool)Session["testing"])
                    {
                        Session["testing"] = false;
                        return RedirectToAction("EditableScenarios", "Scenarios");
                    }
                    else
                    {
                        DatabaseUtils.submitAnswers();
                    }
                }
                else
                {
                    DatabaseUtils.submitAnswers();
                }
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "PLYSENCON03";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            return View(((Scenario)Session["currentScenario"]));
        }

        /// <summary>
        /// Similar to play scenario, but does not save the grade.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TestScenario()
        {
            Session["testing"] = true;
            return this.PlayScenario();
        }

        /// <summary>
        /// Similar to play scenario, but does not save the grade.
        /// </summary>
        /// <param name="scenarioID"></param>
        /// <param name="nextQuestion"></param>
        /// <param name="newGame"></param>
        /// <param name="wasAnswered"></param>
        /// <returns></returns>
        public ActionResult testScenario(int scenarioID, int nextQuestion, bool newGame, bool wasAnswered)
        {
            Session["bool"] = true;
            return this.playScenario(scenarioID, nextQuestion, newGame, wasAnswered);
        }

        /// <summary>
        /// as they are playing a scenario we use this to capture the question they answered, their answer, and their comment to the answer
        /// </summary>
        private void addAnswerToList()
        {
            try
            {
                
                int currentQuestion = ((Scenario)Session["currentScenario"]).currentQuestion;
                //if it is not = 0 an answer has already been selected for that question
                if (((Scenario)Session["currentScenario"]).questions[currentQuestion].selectedAnswer != 0)
                {
                    int currentAnswer = ((Scenario)Session["currentScenario"]).questions[currentQuestion].selectedAnswer;
                    string comment = "";
                    if (!String.IsNullOrEmpty(((Scenario)Session["currentScenario"]).questions[currentQuestion]
                        .answerList[currentAnswer].answerComment))
                    {
                        comment = ((Scenario)Session["currentScenario"]).questions[currentQuestion].answerList[currentAnswer]
                            .answerComment;
                    }
                    int answerId = ((Scenario)Session["currentScenario"]).questions[currentQuestion].answerList[currentAnswer]
                        .answerID;
                    UserAnswer userAnswer = new UserAnswer(currentQuestion, answerId, comment);
                    ((AnswersToGrade)Session["answersToGrade"]).answers.Add(userAnswer);
                }
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "PLYSENCON04";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            
        }

        /// <summary>
        /// this happens when a scenario is loaded for the first time (they push play on the scenarios page)
        /// </summary>
        /// <param name="scenarioID">the scenario identify they want to play</param>
        /// <param name="nextQuestion">the starting question of the scenario</param>
        private void LoadScenario(int scenarioID, int nextQuestion)
        {
            try
            {
                Session["currentScenario"] = DatabaseUtils.getScenario(scenarioID);
                DatabaseUtils.getQuestionsAndAnswers(((Scenario)Session["currentScenario"]));
                Session["answersToGrade"] = new AnswersToGrade();
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "PLYSENCON05";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
        }

        private int nextQuestion(int currentAnswer, int currentQuestion)
        {
            //For number of question references off of a given answer A in question Q
            //Singleton.currentScenario.questions[Q].answerList[A].questionReferences.Count;
            //To get the question itself:
            //Singleton.currentScenario.questions[Singleton.currentScenario.questions[Q1].answerList[A].questionReferences[Q2].questionReference];

            //in the current answer, we check all the questions it references
            //In each question, we calculate it's position on the d100 table.
            //Compare that position with our random number
            //if the number is within the threshhold for that entry, then we return the index of that entry.
            Random rand = new Random();
            int R = rand.Next() % 100;
            int myCount = ((Scenario)Session["currentScenario"]).questions[currentQuestion].answerList[currentAnswer].questionReferences.Count;
            List<int> thresh = new List<int>(myCount);
            for (int i = 0; i < myCount; i++)
            {
                thresh.Add(0);
                for (int j = 0; j <= i; j++)
                {
                    thresh[i] += ((Scenario)Session["currentScenario"]).questions[currentQuestion].answerList[currentAnswer].questionReferences[j].questionReferenceProbability;
                }
                if(i == 0)
                {
                    if(0 <= R && R < thresh[i])
                    {
                        return ((Scenario)Session["currentScenario"]).questions[currentQuestion].answerList[currentAnswer].questionReferences[i].questionReference;
                    }
                }

                else
                {
                    if(thresh[i-1] <= R && R < thresh[i])
                    {
                        return ((Scenario)Session["currentScenario"]).questions[currentQuestion].answerList[currentAnswer].questionReferences[i].questionReference;
                    }
                }
            }
            return 0;
        }
        

    }
}