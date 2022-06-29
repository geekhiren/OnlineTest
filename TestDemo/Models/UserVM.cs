using System.Collections.Generic;
using System.Web.Mvc;

namespace TestDemo.Models
{
    public class UserVM
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
    }

    public class TestVM
    {
        public int TestID { get; set; }
        public string TestName { get; set; }
        public int NumberOfQuestion { get; set; }
        public List<SelectListItem> ListOfTest { get; set; }

    }

    public class QuestionVM
    {
        public int QuestionID { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public string Anwser { get; set; }
        public  ICollection<ChoiceVM> Choices { get; set; }
    }

    public class ChoiceVM
    {
        public int ChoiceID { get; set; }
        public string ChoiceText { get; set; }
    }

    public class TestAnswersVM
    {
        public int QuestionID { get; set; }
        public string QuestionText { get; set; }
        public string AnswerQ { get; set; }
        public bool isCorrect { get; set; }
        public int TotalMark { get; set; }




    }
}