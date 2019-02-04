using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CellGame.Models
{
    /*
     * This class defines the model for a scenario that is being edited. It contains:
     *      ScenarioID - the ID for this scenario as defined when it is submitted to the database
     *      ScenarioName - the title for this scenario
     *      startingQuestion - the first question the user is presented with when selecting this scenario
     *      editReferenceId - in the list of question references for a specific answer, the index of the selected reference
     *      answers - a dictionary of all the answers contained by the scenario
     *      newAnswer - the template for any new answer added to a scenario
     *      selectedAnswer - the ID for an answer selected for editing
     *      newQuestionReference - the template for a new question reference that is being added to an answer
     *      currentQuestionReferencesInAnswer - a list of questions that a specific answer references. Used to ensure each answer only references a question once
     *      questions - a dictionary of questions contained in the scenario
     *      QuestionSelectList - a SelectList of all the questions in this scenario. Used to select a question reference for answers
     *      errorMsg - an error message that will be presented to the user upon error
     *      loopCurrAnswer - Used to pass the current answer of a for each loop between views, esp. for building a tree
     *      loopCurrQuestion - See loopCurrAnswer, but for questions
     */

    public class EditScenarioModel
    {
        public int ScenarioId { get; set; }
        [DataType(DataType.Text)]
        [MaxLength(15)]
        public string ScenarioName { get; set; }
        public int startingQuestion { get; set; }
        public int editReferenceId { get; set; }
        public Dictionary<int, Answer> answers { get; set; }
        public Answer newAnswer { get; set; }
        public int selectedAnswer { get; set; }
        public QuestionReference newQuestionReference { get; set; }
        public List<int> currentQuestionReferencesInAnswer { get; set; }
        public Dictionary<int, Question> questions { get; set; }
        public Dictionary<int, Question> activeQuestions { get; set; }
        public SelectList QuestionsSelectList
        {
            get
            {
                return activeQuestions == null ? new SelectList(new List<Question>()) : new SelectList(activeQuestions, "Key", "Value.questionTitle");
            }
        }
        public string errorMsg { get; set; }
        public string warningMsg { get; set; }
        public KeyValuePair<int, Answer> loopCurrAnswer { get; set; }
        public KeyValuePair<int, Question> loopCurrQuestion { get; set; }
    }
}