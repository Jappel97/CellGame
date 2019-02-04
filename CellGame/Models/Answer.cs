using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CellGame.Models
{
    /*
     * This class defines an Answer. An answer contains:
     *      answerID - the ID number for referencing in a database
     *      answerFor
     *      
     *      
     *      - the ID for the question this answer belongs to
     *      answerText - the text for the answer
     *      nextQuestion - the ID for the question that this answer will take the user to. This is selected randomly from the list of question references
     *      questionReferences - a list of question references
     *      answerComment - any comments the user makes on the answer when selected during the scenario
     *      isActive - a boolean value indicating whether this answer is active
     */
    [Serializable]
    public class Answer
    {
        public int answerID { get; set; }
        public int answerForQuestion { get; set; }
        public string answerText { get; set; }
        public int nextQuestion { get; set; }
        public List<QuestionReference> questionReferences { get; set; }
        [DataType(DataType.MultilineText)]
        [MaxLength(500)]
        public string answerComment { get; set; }
        public bool isActive { get; set; }
        public bool requiresComment { get; set; }
    }
}