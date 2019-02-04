using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CellGame.Models
{
    [Serializable]
    public class StudentGrades
    {
        public string studentName { get; set; }
        public List<StudentGrade> studentGrades { get; set; }
    }
}