using System;
using System.ComponentModel.DataAnnotations;

namespace CellGame.Classes
{
    [Serializable]
    public class Answer
    {
        public string AnswerText { get; set; }
        public int NextQuestion { get; set; }
        public int AnswerNumInQuestion { get; set; }
        [DataType(DataType.MultilineText)]
        [MaxLength(500)]
        public string AnswerComment { get; set; }
    }
}