using System;
using System.Collections.Generic;

namespace CellGame.Classes
{
    [Serializable]
    public class Question
    {
        public string QuestionTitle { get; set; }
        public int QuestionScenario { get; set; }
        public string QuestionDescription { get; set; }
        public int QuestionNumInScenario { get; set; }
        public Dictionary<int, Answer> AnswerList { get; set; }
        public int SelectedAnswer { get; set; }
    }
}