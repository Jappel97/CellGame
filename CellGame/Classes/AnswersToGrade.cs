using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CellGame.Classes
{
    /*
     * This class contains the definition for a list of user answers. This list is added to as the user
     * plays through a scenario and is submitted to the database upon completion of the scenario.
     */
    public class AnswersToGrade
    {
        public List<UserAnswer> answers { get; set; }

        public AnswersToGrade()
        {
            this.answers = new List<UserAnswer>();
        }
    }
}