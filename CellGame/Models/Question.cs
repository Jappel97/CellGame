using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CellGame.Models
{
    /*
     * This class contains the template for a question. It contains:
     *      questionId - the ID used in the database to reference this question
     *      questionScenario - the scenario to which this question belongs
     *      questionTitle - the title for the question
     *      questionDescription - the main body for the question text
     *      questionPicture - a URL indicating which picture should be displayed with this question, if any
     *      isFirstQuestion - a boolean value indicating whether this question is the first question the user is presented with when playing this scenario
     *      answerList - a dictionary of answers for this question
     *      selectedAnswer - the ID for the answer the user selected when presented with this question. Used for recording the user's answers
     *      isActive - a boolean value indicating whether this question is active.
     */
    [Serializable]
    public class Question
    {
        public int questionId { get; set; }
        public int questionScenario { get; set; }
        public string questionTitle { get; set; }
        [DataType(DataType.MultilineText)]
        [MaxLength(1000)]
        public string questionDescription { get; set; }
        [DataType(DataType.ImageUrl)]
        [MaxLength(500)]
        [DefaultValue("")]
        public string questionPicture { get; set; }
        public bool isFirstQuestion { get; set; }
        public Dictionary<int, Answer> answerList { get; set; }
        public int selectedAnswer { get; set; }
        public bool isActive { get; set; }
    }
}