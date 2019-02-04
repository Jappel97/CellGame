using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CellGame.Models
{
    [Serializable]
    public class ScenariosToGrade
    {
        public int stgID { get; set; }
        public int stgScenarioID { get; set; }
        public String stgScenarioName { get; set; }
        public String stgStudentID { get; set; }
        public String stgStudentName { get; set; }
        public bool stgIsGraded { get; set; }
        public int stgGrade { get; set; }
    }
}