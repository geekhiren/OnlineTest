using System;
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
                NumberOfQuestion = test.NumberOfQuestion

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

                   }).AsQueryable().Take(quizSelected.NumberOfQuestion);
            }
            return View(questions);
        }

        [HttpPost]
        public ActionResult QuizTest(List<TestAnswersVM> resultQuiz)
        {
            List<TestAnswersVM> ResultQuiz = new List<TestAnswersVM>();

            List<TestAnswersVM> finalResultQuiz = new List<TestAnswersVM>();

            foreach (TestAnswersVM answser in resultQuiz)
            {
                TestAnswersVM result = dbContext.Answers.Where(a => a.QuestionID == answser.QuestionID).Select(a => new TestAnswersVM
                {
                    QuestionID = a.QuestionID.Value,
                    AnswerQ = a.AnswerText,
                    QuestionText = a.Question.QuestionText,
                    isCorrect = (answser.AnswerQ.ToLower().Equals(a.AnswerText.ToLower()))

                }).FirstOrDefault();

                ResultQuiz.Add(result);
            }
            foreach (var item in ResultQuiz)
            {
                int a = 0;
                if (item.isCorrect == true)
                {
                    TestAnswersVM result = new TestAnswersVM()
                    {
                        AnswerQ = item.AnswerQ,
                        isCorrect = item.isCorrect,
                        QuestionID = item.QuestionID,
                        QuestionText = item.QuestionText,
                        TotalMark = 1
                    };
                    finalResultQuiz.Add(result);
                }
                else
                {
                    TestAnswersVM result = new TestAnswersVM()
                    {
                        AnswerQ = item.AnswerQ,
                        isCorrect = item.isCorrect,
                        QuestionID = item.QuestionID,
                        QuestionText = item.QuestionText,
                        TotalMark = 0
                    };
                    finalResultQuiz.Add(result);
                }
            }
            UserVM userConnected = Session["UserConnected"] as UserVM;
            Mark mark = new Mark()
            {
                TotalQA = finalResultQuiz.Count(),
                TotalMark = finalResultQuiz.Sum(x => x.TotalMark),
                Username = userConnected.UserName
            };

            if (mark != null)
            {
                dbContext.Marks.Add(mark);
                dbContext.SaveChangesAsync();
            }

            return Json(new { result = finalResultQuiz }, JsonRequestBehavior.AllowGet);
        }


    }
}