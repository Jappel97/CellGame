using System.Collections.Generic;
using CellGame.Models;
using System.Web.Mvc;
using System;
using CellGame.Classes;
using System.Linq;

public class ScenariosController : Controller
{
    /// <summary>
    /// approved scenarios are the ones that are playable
    /// </summary>
    /// <returns>returns list of approved scenarios</returns>
    [Authorize]
    public ActionResult Scenarios()
    {
        if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            Scenarios scenarios = DatabaseUtils.getScenarios(approved: true);
            CheckForError();
            return View(scenarios);
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON01";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    /// editable scenarios
    ///     Students -> can only edit their own that they have created
    ///     Admin, Professor, TA -> can edit any scenario
    /// </summary>
    /// <returns>returns a list of editable scenarios</returns>
    [HttpGet]
    [Authorize]
    public ActionResult EditableScenarios()
    {
        if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            Scenarios scenarios;
            if (User.IsInRole("Admin") || User.IsInRole("Professor") || User.IsInRole("TA"))
                scenarios = DatabaseUtils.getAdminScenariosToEdit();
            else
                scenarios = DatabaseUtils.getStudentScenariosToEdit();

            CheckForError();
            return View("EditableScenarios", scenarios);
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON02";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    /// creating a new scenario
    /// </summary>
    /// <returns>view to enter information for a new scenario</returns>
    [Authorize]
    [HttpGet]
    public ActionResult AddScenario()
    {
        if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            Scenario scenario = new Scenario();
            CheckForError();
            return PartialView("_PartialAddScenario", scenario);
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON03";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    /// allows the user to edit the main scenario information NOT QUESTIONS AND ANSWERS
    /// </summary>
    /// <param name="scenarioId">scenario id the user wants to edit</param>
    /// <returns>scenario information to edit</returns>
    [Authorize]
    [HttpGet]
    public ActionResult EditScenario(int scenarioId)
    {
        if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            Scenario scenario = DatabaseUtils.getScenario(scenarioId);
            CheckForError();
            return PartialView("_PartialEditScenario", scenario);
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON04";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    /// creates the scenario based off of user input
    /// </summary>
    /// <returns>takes them to edit questions and answers for the scenario just created</returns>
    [Authorize]
    [HttpPost]
    public ActionResult CreateScenario()
    {
        if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            Scenario scenario = new Scenario();
            if (ModelState.IsValid)
            {
                scenario.scenarioName = Request["scenarioName"];
                scenario.description = Request["description"];
                scenario.scenarioID = Convert.ToInt32(Request["scenarioID"]);
                scenario.cellFunction = Request["cellFunction"];
                scenario.cellShapeAndFeatures = Request["cellShapeAndFeatures"];
                scenario.cellLifespan = Request["cellLifespan"];
                scenario.cellNutrition = Request["cellNutrition"];
            }
            scenario.scenarioID = DatabaseUtils.addScenario(scenario);
            CheckForError();
            return this.EditScenarioQuestionsAndAnswers(scenario.scenarioID);
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON05";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    /// updates the scenario information not QUESTIONS AND ANSWERS
    /// </summary>
    /// <returns>to the editable scenarios page, a list of scenarios you can edit</returns>
    [Authorize]
    [HttpPost]
    public ActionResult UpdateScenario()
    {
        if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            Scenario scenario = new Scenario();
            if (ModelState.IsValid)
            {
                scenario.scenarioName = Request["scenarioName"];
                scenario.description = Request["description"];
                scenario.scenarioID = Convert.ToInt32(Request["scenarioID"]);
                scenario.cellFunction = Request["cellFunction"];
                scenario.cellShapeAndFeatures = Request["cellShapeAndFeatures"];
                scenario.cellLifespan = Request["cellLifespan"];
                scenario.cellNutrition = Request["cellNutrition"];
            }
            DatabaseUtils.editScenario(scenario);
            CheckForError();
            return this.EditableScenarios();
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON07";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    /// makes a scenario active
    /// meaning it can be used in a scenario
    /// </summary>
    /// <param name="questionId">question id you want to activate</param>
    /// <returns></returns>
    [Authorize]
    [HttpGet]
    public ActionResult ActivateScenario(int scenarioId)
    {
        if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            DatabaseUtils.makeScenarioActive(scenarioId);
            CheckForError();
            return EditableScenarios();
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON14";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    ///<summary>
    ///Makes a scenario inactive, meaning it cannot be seen from the play menu
    /// </summary>
    /// <param name="scenarioId">Scenario id you want to make inactive</param>
    /// <returns></returns>
    [Authorize]
    [HttpGet]
    public ActionResult InactivateScenario(int scenarioId)
    {
        if(String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            DatabaseUtils.makeScenarioInactive(scenarioId);
            CheckForError();
            return EditableScenarios();
        }
        catch(Exception ex)
        {
            Singleton.errorCode = "SENCON14";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }


    /// <summary>
    /// allow user to edit questions and answers
    /// makes a call to get the scenario then to get the questions and answers for the scenario
    /// </summary>
    /// <param name="scenarioId">scenario id in database that the user wants to update</param>
    /// <returns></returns>
    [Authorize]
    [HttpGet]
    public ActionResult EditScenarioQuestionsAndAnswers(int scenarioId)
    {
        /*if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }*/
        try
        {
            if (!Singleton.isError)
            {
                Singleton.errorMsg = "";
            }
            else
            {
                Singleton.isError = false;
            }
            if (!Singleton.isWarning)
            {
                Singleton.warningMsg = "";
            }
            else
            {
                Singleton.isWarning = false;
            }
            Scenario scenario = DatabaseUtils.getScenario(scenarioId);
            getEditQuestionsAndAnswers(scenario);
            CheckForError();
            return View("EditScenario", ((EditScenarioModel)Session["editScenario"]));
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON08";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    /// gets the questions and answers for a scenario
    /// this function sets ((EditScenarioModel)Session["editScenario"])
    /// </summary>
    /// <param name="scenario">scenario object that you want to get the scenario for</param>
    private void getEditQuestionsAndAnswers(Scenario scenario)
    {
        try
        {
            DatabaseUtils.getQuestionsAndAnswers(scenario);
            Dictionary<int, Answer> answerList = new Dictionary<int, Answer>();

            Question end = new Question();
            end.questionId = 0;
            end.questionTitle = "End Scenario";
            end.questionDescription = "End Scenario";
            end.isActive = true;
            scenario.questions.Add(end.questionId, end);
            scenario.questionsActive.Add(end.questionId, end);

            if (scenario.questions == null)
            {
                scenario.questions = new Dictionary<int, Question>();
            }
            foreach (KeyValuePair<int, Question> question in scenario.questions)
            {
                if (question.Value.answerList == null)
                {
                    question.Value.answerList = new Dictionary<int, Answer>();
                }
                else
                {
                    foreach (KeyValuePair<int, Answer> answer in question.Value.answerList)
                    {
                        if (!answerList.ContainsKey(answer.Key))
                        {
                            answerList.Add(answer.Key, answer.Value);
                        }
                    }
                }
            }

            Session["editScenario"] = new EditScenarioModel
            {
                ScenarioId = scenario.scenarioID,
                ScenarioName = scenario.scenarioName,
                answers = answerList,
                questions = scenario.questions,
                startingQuestion = scenario.currentQuestion,
                activeQuestions = scenario.questionsActive
            };
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON09";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    /// brings up the partial view to add a question
    /// </summary>
    /// <returns>view to partial add question</returns>
    [Authorize]
    [HttpGet]
    public ActionResult AddQuestion()
    {
        if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            Question question = new Question();
            CheckForError();
            return PartialView("_PartialAddQuestion", question);
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON10";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    /// brings up partial view to edit a question
    /// </summary>
    /// <param name="questionId">question id the user wants to edit</param>
    /// <returns></returns>
    [Authorize]
    [HttpGet]
    public ActionResult EditQuestion(int questionId)
    {
        if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            Question question = ((EditScenarioModel)Session["editScenario"]).questions[questionId];
            if (((EditScenarioModel)Session["editScenario"]).startingQuestion == question.questionId)
            {
                question.isFirstQuestion = true;
            }
            else
            {
                question.isFirstQuestion = false;
            }
            ModelState.Clear();
            CheckForError();
            return PartialView("_PartialEditQuestion", question);
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON11";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    /// forces the user to confirm they want to delete a question so they don't by accident
    /// if they try to delete a question that has a user answer (some one has answered it in play scenario) it makes it inactive
    /// </summary>
    /// <param name="questionId">question id they want to delete</param>
    /// <returns></returns>
    [Authorize]
    [HttpGet]
    public ActionResult ConfirmDeleteQuestion(int questionId)
    {
        if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            Question question = ((EditScenarioModel)Session["editScenario"]).questions[questionId];
            CheckForError();
            return PartialView("_PartialDeleteQuestion", question);
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON12";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    /// acutually deletes the question after confirmation or makes inactive
    /// </summary>
    /// <returns>returns the user to the edit scenario screen</returns>
    [Authorize]
    [HttpPost]
    public ActionResult DeleteQuestion()
    {
        if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            int questionId = Convert.ToInt32(Request["questionId"]);
            bool validDelete = DatabaseUtils.deleteQuestion(questionId);
            if (!validDelete)
            {
                DatabaseUtils.makeQuestionInactive(questionId);
            }

            getEditQuestionsAndAnswers(((Scenario)Session["currentScenario"]));
            CheckForError();
            return View("EditScenario", ((EditScenarioModel)Session["editScenario"]));
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON13";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    /// makes a question active
    /// meaning it can be used in a scenario
    /// </summary>
    /// <param name="questionId">question id you want to activate</param>
    /// <returns></returns>
    [Authorize]
    [HttpGet]
    public ActionResult ActivateQuestion(int questionId)
    {
        if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            DatabaseUtils.makeQuestionActive(questionId);
            getEditQuestionsAndAnswers(((Scenario)Session["currentScenario"]));
            CheckForError();
            return View("EditScenario", ((EditScenarioModel)Session["editScenario"]));
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON14";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    /// make a question inactive, it is not able to be used in play scenario
    /// </summary>
    /// <param name="questionId">question id you want to make inactive</param>
    /// <returns></returns>
    [Authorize]
    public ActionResult InactivateQuestion(int questionId)
    {
        if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            DatabaseUtils.makeQuestionInactive(questionId);
            CheckForError();
            return View("EditScenario", ((EditScenarioModel)Session["editScenario"]));
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON15";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    /// deletes a scenario if there are no users that have played it otherwise it is inactive
    /// </summary>
    /// <param name="scenarioID">scenario id you want to try to delete</param>
    /// <returns>view of the editable scenarios</returns>
    [Authorize]
    public ActionResult ConfirmDeleteScenario(int scenarioID)
    {
        if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            Session["currentScenario"] = DatabaseUtils.getScenario(scenarioID);
            CheckForError();
            return PartialView("_PartialDeleteScenario", ((Scenario)Session["currentScenario"]));
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON16";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    /// brings up partial view to confirm you want to delete the scenario
    /// </summary>
    /// <returns>brings up partial view to confirm you want to delete the scenario</returns>
    [Authorize]
    [HttpPost]
    public ActionResult DeleteScenario()
    {
        if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            int scenarioID = Convert.ToInt32(Request["scenarioID"]);

            int count = DatabaseUtils.getScenarioUsage(scenarioID);
            if (count > 0)
            {
                DatabaseUtils.makeScenarioInactive(scenarioID);
                Singleton.isWarning = true;
                Singleton.warningMsg =
                    "This scenario is referenced by current grades and cannot be deleted. It has been inactivated. ";
            }
            else
            {
                DatabaseUtils.deleteScenario(scenarioID);
            }
            return this.EditableScenarios();
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON17";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    /// saves the main question information
    /// </summary>
    /// <returns>to the scenario you are editing</returns>
    [Authorize]
    [HttpPost]
    public ActionResult SaveQuestion()
    {
        if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            int questionId = Convert.ToInt32(Request["questionId"]);
            string questionTitle = Request["questionTitle"];
            string questionDescription = Request["questionDescription"];
            string questionPictureURL = Request["questionPicture"];
            bool isFirstQuestion = checkBoxToBool(Request["isFirstQuestion"]);

            if (questionId != 0 && ((EditScenarioModel)Session["editScenario"]).questions.ContainsKey(questionId))
            {
                ((EditScenarioModel)Session["editScenario"]).questions[questionId].questionTitle = questionTitle;
                ((EditScenarioModel)Session["editScenario"]).questions[questionId].questionDescription = questionDescription;
                ((EditScenarioModel)Session["editScenario"]).questions[questionId].questionPicture = questionPictureURL;
                DatabaseUtils.editQuestion(((EditScenarioModel)Session["editScenario"]).questions[questionId]);
            }
            else
            {
                //add question to database then add id and question to ((EditScenarioModel)Session["editScenario"]).questions
                if (((EditScenarioModel)Session["editScenario"]).questions.Count == 1)
                {
                    isFirstQuestion = true;
                }
                Question newQuestion = new Question();
                newQuestion.questionTitle = questionTitle;
                newQuestion.questionDescription = questionDescription;
                newQuestion.questionScenario = ((EditScenarioModel)Session["editScenario"]).ScenarioId;
                newQuestion.questionPicture = questionPictureURL;
                newQuestion.answerList = new Dictionary<int, Answer>();
                newQuestion.questionId = DatabaseUtils.addQuestion(newQuestion);
                newQuestion.isActive = true;
                questionId = newQuestion.questionId;
                ((EditScenarioModel)Session["editScenario"]).questions.Add(newQuestion.questionId, newQuestion);
            }

            if (isFirstQuestion)
            {
                DatabaseUtils.changeFirstQuestion(((EditScenarioModel)Session["editScenario"]).questions[questionId]);
            }
            Scenario currScenario = new Scenario();
            currScenario.scenarioID = ((EditScenarioModel)Session["editScenario"]).ScenarioId;
            Session["currentScenario"] = DatabaseUtils.getScenario(((EditScenarioModel)Session["editScenario"]).ScenarioId);
            getEditQuestionsAndAnswers(((Scenario)Session["currentScenario"]));
            saveQuestionImage(questionPictureURL, ((EditScenarioModel)Session["editScenario"]).ScenarioName, questionTitle);
            CheckForError();
            return View("EditScenario", ((EditScenarioModel)Session["editScenario"]));
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON18";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    /// add an answer to the database
    /// </summary>
    /// <param name="id">question id that the answer is for</param>
    /// <returns>the user to the scenario they are editing</returns>
    [Authorize]
    [HttpGet]
    public ActionResult AddAnswer(int id)
    {
        if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            Singleton.errorMsg = "";
            Singleton.warningMsg = "";
            ((EditScenarioModel)Session["editScenario"]).newAnswer = new Answer();
            ((EditScenarioModel)Session["editScenario"]).newAnswer.answerForQuestion = id;
            ((EditScenarioModel)Session["editScenario"]).newAnswer.questionReferences = new List<QuestionReference>();
            CheckForError();
            return PartialView("_PartialAddAnswer", ((EditScenarioModel)Session["editScenario"]));
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON19";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="answerID">answer id which is being edited</param>
    /// <returns>the view in which the answer you want to edit</returns>
    [Authorize]
    [HttpGet]
    public ActionResult EditAnswer(int answerID)
    {
        if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            ((EditScenarioModel)Session["editScenario"]).newAnswer = ((EditScenarioModel)Session["editScenario"]).answers[answerID];
            CheckForError();
            return View("_PartialEditAnswer", ((EditScenarioModel)Session["editScenario"]));
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON20";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    /// save the answer information
    /// </summary>
    /// <param name="saveMethod">this is used to know if it needs to save an answer reference(a question the answer references) or the answer
    /// or cancel, which means it doesn't save</param>
    /// <returns>to the edit scenario page</returns>
    [Authorize]
    [HttpPost]
    public ActionResult SaveAnswer(string saveMethod)
    {
        if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            Singleton.errorMsg = "";
            Singleton.warningMsg = "";
            int answerID = Convert.ToInt32(Request["newAnswer.answerID"]);
            string answerText = Request["newAnswer.answerText"];
            int answerForQuestion = Convert.ToInt32(Request["newAnswer.answerForQuestion"]);
            bool requiresComment = checkBoxToBool(Request["newAnswer.requiresComment"]);

            Answer newAnswer = new Answer();

            if (answerID != 0 && ((EditScenarioModel)Session["editScenario"]).answers.ContainsKey(answerID))
            {
                ((EditScenarioModel)Session["editScenario"]).answers[answerID].answerText = answerText;
                ((EditScenarioModel)Session["editScenario"]).answers[answerID].answerForQuestion = answerForQuestion;
                ((EditScenarioModel)Session["editScenario"]).answers[answerID].requiresComment = requiresComment;
                DatabaseUtils.editAnswer(((EditScenarioModel)Session["editScenario"]).answers[answerID]);
            }
            else
            {
                //add answer to database then add id and answer to ((EditScenarioModel)Session["editScenario"]).answers
                newAnswer.answerText = answerText;
                newAnswer.answerForQuestion = answerForQuestion;
                newAnswer.requiresComment = requiresComment;
                newAnswer.answerID = DatabaseUtils.addAnswer(newAnswer);
                ((EditScenarioModel)Session["editScenario"]).answers.Add(newAnswer.answerID, newAnswer);
            }

            if (saveMethod == "AddAnswer")
            {
                return AddQuestionReference(newAnswer.answerID);
            }
            else if (saveMethod == "EditAnswer")
            {
                return EditQuestionReference(0, newAnswer.answerID);
            }
            CheckForError();
            return this.EditScenarioQuestionsAndAnswers(((EditScenarioModel)Session["editScenario"]).ScenarioId);
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON21";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    /// answer references are where a answer points to the next question
    /// </summary>
    /// <param name="answerID">answer id where you want to add the reference</param>
    /// <returns>takes you to a page where you can add a question reference to the answer</returns>
    [Authorize]
    [HttpGet]
    public ActionResult AddQuestionReference(int answerID)
    {
        if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            Session["selectedAnswer"] = answerID;
            ((EditScenarioModel)Session["editScenario"]).newAnswer = ((EditScenarioModel)Session["editScenario"]).answers[answerID];
            QuestionReference newReference = new QuestionReference();
            newReference.questionReference = 0;
            newReference.questionReferenceProbability = 100;
            ((EditScenarioModel)Session["editScenario"]).newQuestionReference = newReference;
            CheckForError();
            return View("_PartialAddAnswerReferences", ((EditScenarioModel)Session["editScenario"]));
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON22";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    /// allow the user to edit a reference of an answer to change probability or the question it points to
    /// </summary>
    /// <param name="referenceId">reference you want to change</param>
    /// <param name="answerID">the answer the reference belongs to</param>
    /// <returns></returns>
    [Authorize]
    [HttpGet]
    public ActionResult EditQuestionReference(int referenceId, int answerID)
    {
        if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            ((EditScenarioModel)Session["editScenario"]).newQuestionReference = new QuestionReference();
            ((EditScenarioModel)Session["editScenario"]).newQuestionReference.questionReference = ((EditScenarioModel)Session["editScenario"]).newAnswer.questionReferences[referenceId].questionReference;
            ((EditScenarioModel)Session["editScenario"]).newQuestionReference.questionReferenceProbability = ((EditScenarioModel)Session["editScenario"]).newAnswer.questionReferences[referenceId].questionReferenceProbability;
            ((EditScenarioModel)Session["editScenario"]).newQuestionReference.prevQuestionReference = ((EditScenarioModel)Session["editScenario"]).newQuestionReference.questionReference;

            Session["selectedAnswer"] = answerID;
            ((EditScenarioModel)Session["editScenario"]).editReferenceId = referenceId;
            ((EditScenarioModel)Session["editScenario"]).newAnswer = ((EditScenarioModel)Session["editScenario"]).answers[answerID];
            CheckForError();
            return View("_PartialEditAnswerReferences", ((EditScenarioModel)Session["editScenario"]));
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON23";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    /// save the question reference
    /// </summary>
    /// <param name="saveType">this is used to know if the answer reference is a save or cancel</param>
    /// <param name="isNew">if it is a new reference</param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    public ActionResult SubmitQuestionReference(string saveType, bool isNew) //isNew (reference)
    {
        if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            int answerID = (int)Session["selectedAnswer"];
            if (!saveType.Equals("Cancel"))
            {
                QuestionReference newReference = new QuestionReference();
                newReference.prevQuestionReference = Convert.ToInt32(Request["newQuestionReference.prevQuestionReference"]);
                newReference.questionReference = Convert.ToInt32(Request["newQuestionReference.questionReference"]);
                newReference.questionReferenceProbability = Convert.ToInt32(Request["newQuestionReference.questionReferenceProbability"]);

                ((EditScenarioModel)Session["editScenario"]).currentQuestionReferencesInAnswer = new List<int>();

                if (((EditScenarioModel)Session["editScenario"]).newAnswer.questionReferences == null)
                    ((EditScenarioModel)Session["editScenario"]).newAnswer.questionReferences = new List<QuestionReference>();

                foreach (QuestionReference reference in ((EditScenarioModel)Session["editScenario"]).newAnswer.questionReferences)
                {
                    ((EditScenarioModel)Session["editScenario"]).currentQuestionReferencesInAnswer.Add(reference.questionReference);
                }

                if (isNew)
                {
                    if (((EditScenarioModel)Session["editScenario"]).currentQuestionReferencesInAnswer.Contains(newReference.questionReference))
                    {
                        Console.Out.WriteLine("DIRTY ADD");
                        ((EditScenarioModel)Session["editScenario"]).errorMsg = "That question is already referenced by this answer. Please select a different question.";
                        return View("_PartialAddAnswerReferences", ((EditScenarioModel)Session["editScenario"]));
                    }
                    else
                    {
                        Console.Out.WriteLine("CLEAN ADD");
                        ((EditScenarioModel)Session["editScenario"]).errorMsg = "";
                        DatabaseUtils.addAnswerReference(answerID, newReference);
                    }
                }
                else
                {
                    ((EditScenarioModel)Session["editScenario"]).newAnswer.questionReferences[((EditScenarioModel)Session["editScenario"]).editReferenceId].questionReference = newReference.questionReference;
                    ((EditScenarioModel)Session["editScenario"]).newAnswer.questionReferences[((EditScenarioModel)Session["editScenario"]).editReferenceId].questionReferenceProbability = newReference.questionReferenceProbability;
                    int count = 0;
                    foreach (QuestionReference qr in ((EditScenarioModel)Session["editScenario"]).newAnswer.questionReferences)
                    {
                        if (qr.questionReference.Equals(newReference.questionReference))
                        {
                            count++;
                        }
                    }
                    if (count > 1)
                    {
                        Console.Out.WriteLine("DIRTY UPDATE");
                        ((EditScenarioModel)Session["editScenario"]).errorMsg = "An answer already references that question";
                        return View("_PartialEditAnswerReferences", ((EditScenarioModel)Session["editScenario"]));
                    }
                    else
                    {
                        Console.Out.WriteLine("CLEAN UPDATE");
                        ((EditScenarioModel)Session["editScenario"]).errorMsg = "";
                        DatabaseUtils.editAnswerReference(answerID, newReference);
                    }
                }
                Session["currentScenario"] = DatabaseUtils.getScenario(((EditScenarioModel)Session["editScenario"]).ScenarioId);
                //sets the ((EditScenarioModel)Session["editScenario"])
            }
            getEditQuestionsAndAnswers(((Scenario)Session["currentScenario"]));
            CheckForError();
            return EditAnswer(answerID);
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON24";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    /// delete a question reference if an answer has multiple or you just want to remove it
    /// </summary>
    /// <param name="answerID">answer the reference is attached to</param>
    /// <param name="questionReferenceID">reference id in the database</param>
    /// <returns></returns>
    [Authorize]
    [HttpGet]
    public ActionResult DeleteQuestionReference(int answerID, int questionReferenceID)
    {
        if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            //call database function to delete question references
            DatabaseUtils.deleteAnswerReference(answerID, ((EditScenarioModel)Session["editScenario"]).newAnswer.questionReferences[questionReferenceID].questionReference);

            Scenario currScenario = new Scenario();
            currScenario.scenarioID = ((EditScenarioModel)Session["editScenario"]).ScenarioId;
            Session["currentScenario"] = DatabaseUtils.getScenario(((EditScenarioModel)Session["editScenario"]).ScenarioId);

            //sets the ((EditScenarioModel)Session["editScenario"])
            getEditQuestionsAndAnswers(((Scenario)Session["currentScenario"]));

            return EditAnswer(answerID);
            // return PartialView("_PartialAddAnswer", ((EditScenarioModel)Session["editScenario"]));
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON25";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    /// makes the user confirm the delete of an answer before it happens.
    /// if an answer has been answered in play scenario it is made inactive
    /// </summary>
    /// <param name="answerID">answer you want to delete</param>
    /// <returns>view to the edit scenario page</returns>
    [Authorize]
    [HttpGet]
    public ActionResult ConfirmDeleteAnswer(int answerID)
    {
        if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            Answer answer = ((EditScenarioModel)Session["editScenario"]).answers[answerID];
            CheckForError();
            return PartialView("_PartialDeleteAnswer", answer);
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON26";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    /// actually deletes the answer
    /// </summary>
    /// <returns>to the edit scenario screen</returns>
    [Authorize]
    [HttpPost]
    public ActionResult DeleteAnswer()
    {
        if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            int answerID = Convert.ToInt32(Request["answerID"]);
            if (!DatabaseUtils.deleteAnswer(answerID))
            {
                DatabaseUtils.makeAnswerInactive(answerID);
            }

            return EditScenarioQuestionsAndAnswers(((EditScenarioModel)Session["editScenario"]).ScenarioId);
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON27";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    /// makes an inactive answer active
    /// </summary>
    /// <param name="answerID">answer to be made active</param>
    /// <returns>the updated edit scenario with the answer no being active</returns>
    [Authorize]
    [HttpGet]
    public ActionResult ActivateAnswer(int answerID)
    {
        if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            DatabaseUtils.makeAnswerActive(answerID);
            getEditQuestionsAndAnswers(((Scenario)Session["currentScenario"]));
            CheckForError();
            return View("EditScenario", ((EditScenarioModel)Session["editScenario"]));
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON28";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    /// deactivates an answer, it won't be able to be used in a scenario in play scenario
    /// </summary>
    /// <param name="answerID">answer you want to make inactive</param>
    /// <returns>to the edit scenario page with answer now being inactive</returns>
    [Authorize]
    [HttpPost]
    public ActionResult InactivateAnswer(int answerID)
    {
        if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            DatabaseUtils.makeAnswerInactive(answerID);
            CheckForError();
            return View("EditScenario", ((EditScenarioModel)Session["editScenario"]));
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON29";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    /// takes the value of a checkbox and converts it too a boolean
    /// </summary>
    /// <param name="cbVal">@HTML.checkboxfor value that is passed in from the ui</param>
    /// <returns>boolean if the checkbox is checked or not</returns>
    private static bool checkBoxToBool(string cbVal)
    {
        try
        {
            if (string.Compare(cbVal, "false") == 0)
                return false;
            if (string.Compare(cbVal, "true,false") == 0)
                return true;
            else
                throw new ArgumentNullException(cbVal);
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON30";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    /// returns a list of edited scenarios for a professor to approve
    /// </summary>
    /// <returns></returns>
    [Authorize(Roles = "Admin, Professor, TA")]
    public ActionResult ScenariosToApprove()
    {
        if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            Scenarios scenarios = DatabaseUtils.getScenarios(approved: false);
            return View(scenarios);
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON31";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    /// when a non student role approves a scenario it can now be played in play scenario
    /// </summary>
    /// <param name="scenarioID">scenario id they want to approve</param>
    /// <returns>to the approve scenarios page</returns>
    [Authorize(Roles = "Admin, Professor, TA")]
    public ActionResult ApproveScenario(int scenarioID)
    {
        if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            Session["currentScenario"] = DatabaseUtils.getScenario(scenarioID);
            DatabaseUtils.getQuestionsAndAnswers(((Scenario)Session["currentScenario"]));
            return View(((Scenario)Session["currentScenario"]));
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON32";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    /// <summary>
    /// gets a list of scenarios that need to be approved by non students (Prof,TA,admin)
    /// once a scenario has previously been approved
    /// </summary>
    /// <param name="scenarioID">scenario they want to approve</param>
    /// <returns>to the list of scenarios needing to be approved</returns>
    [HttpPost]
    [Authorize(Roles = "Admin, Professor, TA")]
    public ActionResult ScenariosToApprove(int scenarioID)
    {
        if (String.IsNullOrEmpty((string)Session["userId"]))
        {
            return RedirectToAction("Login", "Account");
        }
        try
        {
            scenarioID = Convert.ToInt32(Request["scenarioID"]);
            DatabaseUtils.approveScenario(scenarioID);
            Scenarios scenarios = DatabaseUtils.getScenarios(approved: false);
            return View(scenarios);
        }
        catch (Exception ex)
        {
            Singleton.errorCode = "SENCON32";
            Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
            throw ex;
        }
    }

    private void CheckForError()
    {
        if (!Singleton.isError)
        {
            Singleton.errorMsg = "";
        }
        else
        {
            Singleton.isError = false;
        }
        if (!Singleton.isWarning)
        {
            Singleton.warningMsg = "";
        }
        else
        {
            Singleton.isWarning = false;
        }
    }

    /// <summary>
    /// Stores the image associated with a question in a local file in order to avoid embarassment from links dying
    /// CURRENT WIP
    /// </summary>
    /// <param name="imageURL">The URL of the image associated with the question</param>
    /// <param name="scenarioName">The scenario in which the question is stored, for organization purposes</param>
    /// <param name="questionName">ditto but for the question</param>
    private void saveQuestionImage(string imageURL, string scenarioName, string questionName)
    {
        System.Drawing.Image img = storeImage(imageURL);
        string path = @"C:\CellGame\WebsiteFiles\Images\" + scenarioName;
        if(!System.IO.Directory.Exists(path))
        {
            System.IO.Directory.CreateDirectory(path);
        }
        path = path + "\\images.txt";
        if(!System.IO.File.Exists(path))
        {
            System.IO.File.Create(path);
        }
        if(!imgCompare(img, path))
        {
            string hash = getHash(new System.Drawing.Bitmap(img));
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
            {
                file.WriteLine(hash);
            }
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                questionName = questionName.Replace(c, '-');
            }
            path = @"C:\CellGame\WebsiteFiles\Images\" + scenarioName;
            img.Save(path + "\\" + questionName + ".png");
        }
    }

    /// <summary>
    /// a sub-function of saveQuestionImage
    /// </summary>
    /// <param name="imageURL"></param>
    /// <returns></returns>
    private System.Drawing.Image storeImage(string imageURL)
    {
        System.Drawing.Image img = null;

        try
        {
            System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(imageURL);
            //webRequest.AllowReadStreamBuffering = true;
            webRequest.Timeout = 30000;

            System.Net.WebResponse webResponse = webRequest.GetResponse();

            System.IO.Stream stream = webResponse.GetResponseStream();

            img = System.Drawing.Image.FromStream(stream);

            webResponse.Close();
        }
        catch(Exception ex)
        {
            return null;
        }
        return img;
    }

    /// <summary>
    /// Checks if a image has already been stored for that question
    /// </summary>
    /// <param name="image"></param>
    /// <param name="scenarioPath"></param>
    /// <returns></returns>
    private bool imgCompare(System.Drawing.Image image, string scenarioPath)
    {
        System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(image);
        string str = getHash(bmp);
        List<string> used = System.IO.File.ReadLines(scenarioPath).ToList();
        for(int i = 0; i < used.Count; i++)
        {
            if(used.ElementAt<string>(i).Equals(str))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// used to compare images in imgComapre
    /// </summary>
    /// <param name="bmpSource"></param>
    /// <returns></returns>
    private string getHash(System.Drawing.Bitmap bmpSource)
    {
        string hash = "";

        System.Drawing.Bitmap bmpMin = new System.Drawing.Bitmap(bmpSource, new System.Drawing.Size(24, 24));
        for(int i = 0; i < bmpMin.Height; i++)
        {
            for(int j = 0; j < bmpMin.Width; j++)
            {
                if(bmpMin.GetPixel(j, i).GetBrightness() < 0.5f)
                {
                    hash = hash + "1";
                }
                else
                {
                    hash = hash + "0";
                }
            }
        }
        return hash;
    }
}
