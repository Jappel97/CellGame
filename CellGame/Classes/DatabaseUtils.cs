using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using CellGame.Classes;
using CellGame.Models;
using System.Linq;
using System.Web;
using System.Web.SessionState;

public static class DatabaseUtils
{

    /*
     * This class contains references to all the database interactions. The actual functions are separated into 
     * classes for scenarios, questions, answers, users, and students. The database connection is made in this class
     * and passed on to the other classes. 
     * When connecting to the database, the string "Constringname" needs to reference the database connection
     * defined in the Web.config file.
     */

    private static readonly Configuration _rootWebConfig = WebConfigurationManager.OpenWebConfiguration("/CellGame");
    private const string Constringname = "CellGameJTB";

    private static readonly ConnectionStringSettings conString = _rootWebConfig.ConnectionStrings.ConnectionStrings[Constringname];

    private static readonly SqlConnection connection = new SqlConnection(conString.ConnectionString);

    public static void setUserNameId(string email)
    {
        UserDatabaseUtils.setUserNameId(email, connection);
    }

    public static void setNewUserName(string email)
    {
        UserDatabaseUtils.setNewUserName(email, connection);
    }

    public static int addScenario(Scenario scenario)
    {
        return ScenarioDatabaseUtils.addScenario(scenario, connection);
    }

    public static int getScenarioUsage(int scenarioID)
    {
        return ScenarioDatabaseUtils.getScenarioUsage(scenarioID, connection);
    }

    public static void makeScenarioInactive(int scenarioID)
    {
        ScenarioDatabaseUtils.makeScenarioInactive(scenarioID, connection);
    }

    public static void deleteScenario(int scenarioID)
    {
        ScenarioDatabaseUtils.deleteScenario(scenarioID, connection);
    }

    public static void deleteGrade(int stgID)
    {
        ScenarioDatabaseUtils.deleteGrade(stgID, connection);
    }

    public static Scenario getScenario(int scenarioId)
    {
        return ScenarioDatabaseUtils.getScenario(scenarioId, connection);
    }

    public static void editScenario(Scenario scenario)
    {
        ScenarioDatabaseUtils.editScenario(scenario, connection);
    }

    public static void approveScenario(int scenarioId)
    {
        ScenarioDatabaseUtils.approveScenario(scenarioId, connection);

    }

    public static Scenarios getScenarios(bool approved)
    {
        return ScenarioDatabaseUtils.getScenarios(approved, connection);
    }

    public static void getQuestionsAndAnswers(Scenario scenario)
    {
        ScenarioDatabaseUtils.getQuestionsAndAnswers(scenario, connection);
    }

    public static void submitAnswers()
    {
        var newScenarioToGrade =
            insertScenarioToGrade((string)HttpContext.Current.Session["userId"], ((Scenario)HttpContext.Current.Session["currentScenario"]).scenarioID, null);
        foreach (var ua in ((AnswersToGrade)HttpContext.Current.Session["answersToGrade"]).answers)
            insertStudentAnswer((string)HttpContext.Current.Session["userId"], ((Scenario)HttpContext.Current.Session["currentScenario"]).scenarioID, newScenarioToGrade,
                ua.Question, ua.Answer, ua.Comment);
    }


    private static void insertStudentAnswer(string studentId, int scenarioId, int scenarioToGrade, int scenarioQuestion,
        int scenarioAnswer, string studentComment)
    {
        StudentDatabaseUtils.insertStudentAnswer(studentId, scenarioId, scenarioToGrade, scenarioQuestion, scenarioAnswer, studentComment, connection);
    }

    private static int insertScenarioToGrade(string studentId, int scenarioId, int? grade)
    {
        return ScenarioDatabaseUtils.insertScenarioToGrade(studentId, scenarioId, grade, connection);
    }

    public static StudentGrades getStudentGrades(string studentId)
    {
        return StudentDatabaseUtils.getStudentGrades(studentId, connection);
    }

    public static ScenariosToGradeList getScenariosToGrade()
    {
        return ScenarioDatabaseUtils.getScenariosToGrade(connection);
    }

    public static ScenarioToGrade getScenarioToGrade(int stgID, int stgScenarioID, string stgStudentID)
    {
        return ScenarioDatabaseUtils.getScenarioToGrade(stgID, stgScenarioID, stgStudentID, connection);
    }

    public static void addGradeForScenario(int stgId, int grade, string gradeComments)
    {
        ScenarioDatabaseUtils.addGradeForScenario(stgId, grade, gradeComments, connection);
    }

    public static void getStudents()
    {
        StudentDatabaseUtils.getStudents(connection);
    }

    public static void changeFirstQuestion(Question question)
    {
        QuestionDatabaseUtils.changeFirstQuestion(question, connection);
    }

    public static int addQuestion(Question question)
    {
        return QuestionDatabaseUtils.addQuestion(question, connection);
    }

    public static void editQuestion(Question question)
    {
        QuestionDatabaseUtils.editQuestion(question, connection);
    }

    public static int getQuestionUsage(int questionId)
    {
        return QuestionDatabaseUtils.getQuestionUsage(questionId, connection);
    }

    public static void makeQuestionActive(int questionId)
    {
        QuestionDatabaseUtils.makeQuestionActive(questionId, connection);
    }

    public static void makeQuestionInactive(int questionId)
    {
        QuestionDatabaseUtils.makeQuestionInactive(questionId, connection);
    }

    public static bool deleteQuestion(int questionId)
    {
        return QuestionDatabaseUtils.deleteQuestion(questionId, connection);
    }

    public static int addAnswer(Answer answer)
    {
        return AnswerDatabaseUtils.addAnswer(answer, connection);
    }

    public static void editAnswer(Answer answer)
    {
        AnswerDatabaseUtils.editAnswer(answer, connection);
    }

    public static int getAnswerUsage(int answerID)
    {
       return AnswerDatabaseUtils.getAnswerUsage(answerID, connection);
    }

    public static bool makeAnswerActive(int answerID)
    {
        return AnswerDatabaseUtils.makeAnswerActive(answerID, connection);
    }

    public static void makeAnswerInactive(int answerID)
    {
        AnswerDatabaseUtils.makeAnswerInactive(answerID, connection);
    }

    public static bool deleteAnswer(int answerID)
    {
        return AnswerDatabaseUtils.deleteAnswer(answerID, connection);
    }

    public static void changeUserRole(String roleId, String userId)
    {
        UserDatabaseUtils.changeUserRole(roleId, userId, connection);
    }

    public static Scenarios getAdminScenariosToEdit()
    {
        return UserDatabaseUtils.getAdminScenariosToEdit(connection);
    }

    public static Scenarios getStudentScenariosToEdit()
    {
        return UserDatabaseUtils.getStudentScenariosToEdit(connection);
    }

    public static void addAnswerReference(int answerID, QuestionReference questionReference)
    {
        AnswerDatabaseUtils.addAnswerReference(answerID, questionReference, connection);
    }

    public static void deleteAnswerReference(int answerID, int questionReferenceID)
    {
        AnswerDatabaseUtils.deleteAnswerReference(answerID, questionReferenceID, connection);
    }

    public static void editAnswerReference(int answerID, QuestionReference newReference)
    {
        AnswerDatabaseUtils.editAnswerReference(answerID, newReference, connection);
    }

    public static void getUserRoles()
    {
        UserDatabaseUtils.getUserRoles(connection);
    }

    public static void makeScenarioActive(int scenarioId)
    {
        ScenarioDatabaseUtils.makeScenarioActive(scenarioId, connection);
    }
}