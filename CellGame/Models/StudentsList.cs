using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CellGame.Models
{
    public class StudentsList
    {
        public List<String> roleNames { get; set; }
        public List<String> roleKeys { get; set; }
        public List<Student> studentsList { get; set; }
    }
}