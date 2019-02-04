using System;

namespace CellGame.Classes
{
    [Serializable]
    public class Grade
    {
        public string ScenarioName { get; set; }
        public int GradeTotal { get; set; }
        public int GradeMax { get; set; }
        public string GradeComments { get; set; }
    }
}