using System.Collections.Generic;
using Disha.Web.Models.Entities;

namespace Disha.Web.Models.ViewModels
{
    public class QuizSubmitModel
    {
        public Dictionary<int, int> Answers { get; set; } = new(); // QuestionId -> OptionId
    }

    public class QuizViewModel
    {
        public List<QuizQuestion> Questions { get; set; } = new();
        public int CurrentQuestionIndex { get; set; }
        public int TotalQuestions => Questions.Count;
    }
}
