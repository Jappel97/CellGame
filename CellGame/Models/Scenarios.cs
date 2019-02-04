using System;
using System.Collections.Generic;
using CellGame.Classes;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CellGame.Models
{
    [Serializable]
    public class Scenarios
    {
        public List<Scenario> ScenarioList { get; set; }
        public StudentGrades StudentGrades { get; set; }
        public int newScenarioIndex { get; set; }
    }
}