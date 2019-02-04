using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CellGame.Models
{
    [Serializable]
    public class ScenarioToGrade
    {
        public int stgID { get; set; }
        public int stgScenarioID { get; set; }
        public String stgScenarioName { get; set; }
        public String stgStudentID { get; set; }
        public int stgGrade { get; set; }
        public String stgComments { get; set; }
        public List<StudentAnswer> stgAnswersToGrade { get; set; }
    }
}