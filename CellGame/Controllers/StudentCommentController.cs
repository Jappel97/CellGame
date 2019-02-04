using CellGame.Classes;
using CellGame.Models;
using System.Web.Mvc;

namespace CellGame.Controllers
{
    public class StudentCommentController : Controller
    {
        // GET: StudentComment
        public ActionResult StudentComment(int currentQuestion, int selectedAnswer)
        {
            Singleton.currentScenario.questions[currentQuestion].selectedAnswer = selectedAnswer;
            if (Singleton.currentScenario.questions[currentQuestion].answerList[Singleton.currentScenario.questions[currentQuestion].selectedAnswer].nextQuestion == 0)
            {
                return RedirectToAction("Winner", "PlayScenario");
            }
            else if (Singleton.currentScenario.questions[currentQuestion].answerList[selectedAnswer].nextQuestion == -1)
            {
                return RedirectToAction("Loser", "PlayScenario");
            }
            else
            {
                return View(Singleton.currentScenario);
            }
        }
    }
}