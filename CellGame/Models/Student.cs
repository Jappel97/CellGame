using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CellGame.Models
{
    [Serializable]
    public class Student
    {
        public String id { get; set; }
        public String name { get; set; }
        public string role { get; set; }
    }
}