using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Uchat.Models
{
	public class ListQuizTemplateQuestionViewModel
	{
		public Course Course { get; set; }
		public IEnumerable<QuizTemplateQuestion> Questions { get; set; }
	}

	public class AddQuizTemplateQuestionViewModel
	{
		[Required]
		[Display(Name = "Question")]
		public string Text { get; set; }

		[Required]
		public int CourseId { get; set; }

		public Course Course { get; set; }

	}
}