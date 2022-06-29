using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TestDemo.Models;

namespace QuizApplicationMVC5.Controllers
{
    [Authorize]
    public class TestController : Controller
    {

        public OnlineTestEntities dbContext = new OnlineTestEntities();

        // GET: Quizz
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetUser(UserVM user)
        {
            UserVM userConnected = dbContext.AspNetUsers.Where(u => u.UserName == user.UserName)
                                         .Select(u => new UserVM
                                         {
                                             UserID = u.Id,
                                             UserName = u.UserName
                                         }).FirstOrDefault();

            if (userConnected != null)
            {
                Session["UserConnected"] = userConnected;
                return RedirectToAction("SelectTest");
            }
            else
            {
                ViewBag.Msg = "Sorry : user is not found !!";
                return View();
            }

        }

        [HttpGet]
        public ActionResult SelectTest()
        {
            TestVM quiz = new TestVM();
            quiz.ListOfTest = dbContext.Tests.Select(q => new SelectListItem
            {
                Text = q.TestName,
                Value = q.TestID.ToString()

            }).ToList();

            return View(quiz);
        }

        [HttpPost]
        public ActionResult SelectTest(TestVM test)
        {
            TestVM testSelected = dbContext.Tests.Where(q => q.TestID == test.TestID).Select(q => new TestVM
            {
                TestID = q.TestID,
                TestName = q.TestName,

            }).FirstOrDefault();

            if (testSelected != null)
            {
                Session["SelectedTest"] = testSelected;

                return RedirectToAction("QuizTest");
            }

            return View();
        }

        [HttpGet]
        public ActionResult QuizTest()
        {
            TestVM quizSelected = Session["SelectedTest"] as TestVM;
            IQueryable<QuestionVM> questions = null;

            if (quizSelected != null)
            {
                questions = dbContext.Questions.Where(q => q.Test.TestID == quizSelected.TestID)
                   .Select(q => new QuestionVM
                   {
                       QuestionID = q.QuestionID,
                       QuestionText = q.QuestionText,
                       Choices = q.Choices.Select(c => new ChoiceVM
                       {
                           ChoiceID = c.ChoiceID,
                           ChoiceText = c.ChoiceText
                       }).ToList()

                   }).AsQueryable();


            }

            return View(questions);
        }

        [HttpPost]
        public ActionResult QuizTest(List<TestAnswersVM> resultQuiz)
        {
            List<TestAnswersVM> finalResultQuiz = new List<TestAnswersVM>();

            foreach (TestAnswersVM answser in resultQuiz)
            {
                TestAnswersVM result = dbContext.Answers.Where(a => a.QuestionID == answser.QuestionID).Select(a => new TestAnswersVM
                {
                    QuestionID = a.QuestionID.Value,
                    AnswerQ = a.AnswerText,
                    isCorrect = (answser.AnswerQ.ToLower().Equals(a.AnswerText.ToLower()))

                }).FirstOrDefault();

                finalResultQuiz.Add(result);
            }

            return Json(new { result = finalResultQuiz }, JsonRequestBehavior.AllowGet);
        }


    }
}