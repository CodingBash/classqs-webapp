using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace Uchat.Models
{
	public class QuizChoiceViewModel
	{
		public int ID { get; set; }
		public string Text { get; set; }
	}

	public class AskQuizQuestionViewModel
	{
		public int CurrentQuestionNum { get; set; }
		public QuizQuestion Question { get; set; }
	}

	public class EditQuizAnswerViewModel
	{
		public int SessionId { get; set; }
		public int CurrentQuestionNum { get; set; }
		public int TotalQuestions { get; set; }
		public string QuestionText { get; set; }
		public int QuestionId { get; set; }
		public IEnumerable<QuizChoiceViewModel> QuestionChoices { get; set; }

		[Display(Name = "Your Answer")]
		public string StudentAnswer { get; set; }
	}
}