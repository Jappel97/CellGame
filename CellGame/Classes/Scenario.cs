using System;
using System.Collections.Generic;

namespace CellGame.Classes
{
    [Serializable]
    public class Scenario
    {
        public int ScenarioId { get; set; }
        public string ScenarioName { get; set; }
        public int ScenarioNumber { get; set; }
        public string Description { get; set; }
        public int CurrentQuestion { get; set; }
        public Dictionary<int, Question> Questions { get; set; }
    }
}