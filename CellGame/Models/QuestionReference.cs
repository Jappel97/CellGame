using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CellGame.Models
{
    /*
     * A question reference consists of:
     *      questionReference - the ID for the question that this answer refers to
     *      questionReferenceProbability - the probability that this question is selected as the one this answer refers to
     */
    [Serializable]
    public class QuestionReference
    {
        public int prevQuestionReference { get; set; }
        public int questionReference { get; set; }
        public int questionReferenceProbability { get; set; }
    }
}