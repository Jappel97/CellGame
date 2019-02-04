using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CellGame.Models
{
    [Serializable]
    public class StudentGrade
    {
        public int stgId { get; set; }
        public string studentId { get; set; }
        public int scenarioId { get; set; }
        public string scenarioName { get; set; }
        public int grade { get; set; }
        public string gradeComments { get; set; }
    }
}