using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CellGame.Classes
{
    /*
     * This class contains the structure for an answer. The answer consists of the question the answer is for,
     * the actual answer selected, and any comments the student has.
     */
    public class UserAnswer
    {
        public int Question { get; set; }
        public int Answer { get; set; }
        public string Comment { get; set; }

        public UserAnswer(int question, int answer, string comment)
        {
            this.Question = question;
            this.Answer = answer;
            this.Comment = comment;
        }
    }
}