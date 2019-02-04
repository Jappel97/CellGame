using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CellGame.Models
{
    [Serializable]
    public class StudentAnswer
    {
        public String questionTitle { get; set; }
        public String questionDescription { get; set; }
        public String answerText { get; set; }
        public String answerComments { get; set; }
    }
}