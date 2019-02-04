using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CellGame.Models
{
    /*
     * This class contains the template for a scenario. It contains:
     *      scenarioID - the ID attributed to this scenario in the database
     *      scenarioName - the name for this scenario
     *      description - the description for this scenario
     *      madeBy - the ID for the user who created the scenario
     *      questions - a dictionary of all the questions contained in this scenario
     *      currentQuestion - the current question the user is answering while playing the scenario
     *      isActive - a boolean value indicating whether the scenario is active
     */
    [Serializable]
    public class Scenario
    {
        public int scenarioID { get; set; }
        [DataType(DataType.Text)]
        [MaxLength(15)]
        public string scenarioName { get; set; }
        [DataType(DataType.MultilineText)]
        [MaxLength(1000)]
        public string description { get; set; }
        public string madeBy { get; set; }
        public Dictionary<int, Question> questions { get; set; }
        public int currentQuestion { get; set; }
        [DataType(DataType.MultilineText)]
        [MaxLength(1000)]
        public string cellFunction { get; set; }
        [DataType(DataType.MultilineText)]
        [MaxLength(1000)]
        public string cellShapeAndFeatures { get; set; }
        [DataType(DataType.MultilineText)]
        [MaxLength(1000)]
        public string cellLifespan { get; set; }
        [DataType(DataType.MultilineText)]
        [MaxLength(1000)]
        public string cellNutrition { get; set; }
        [DefaultValue(true)]
        public bool isActive { get; set; }
        public Dictionary<int, Question> questionsActive { get; set; }
        public Dictionary<int, Answer> answersInactive { get; set; }
    }
}