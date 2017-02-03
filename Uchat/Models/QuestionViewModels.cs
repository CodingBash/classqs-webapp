using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Uchat.Models
{
	public class ListQuestionViewModel
	{
		public int SessionID { get; set; }
		public string SessionName { get; set; }
		public bool SessionEnded { get; set; }
		public int CourseID { get; set; }
		public string CourseName { get; set; }
		public string UserID { get; set; }
		public int UnansweredQuizQuestions { get; set; }
		public IEnumerable<Question> Questions { get; set; }

		private ApplicationUser.UserTypes userType;
		public ApplicationUser.UserTypes UserType
		{
			get
			{
				return userType;
			}
			set
			{
				userType = value;
				switch (userType)
				{
					case ApplicationUser.UserTypes.Teacher:
						IsTeacher = true;
						IsAid = false;
						IsStudent = false;
						break;
					case ApplicationUser.UserTypes.Aid:
						IsTeacher = false;
						IsAid = true;
						IsStudent = false;
						break;
					case ApplicationUser.UserTypes.Student:
						IsTeacher = false;
						IsAid = false;
						IsStudent = true;
						break;
					default:
						IsTeacher = false;
						IsAid = false;
						IsStudent = false;
						break;
				}
			}
		}
		public bool IsTeacher { get; internal set; }
		public bool IsAid { get; internal set; }
		public bool IsStudent { get; internal set; }
	}

	public class NewQuestionViewModel
	{
		[Required]
		[Display(Name = "Question")]
		public string Text { get; set; }

		public int SessionID { get; set; }
	}

	public class AddQuestionAnswerViewModel
	{
		[Required]
		[Display(Name = "Answer")]
		public string Text { get; set; }

		public int QuestionID { get; set; }
		public int SessionID { get; set; }
		public string AnswererID { get; set; }
		public string QuestionText { get; set; }
	}

	public class EditQuestionAnswerViewModel
	{
		[Required]
		[Display(Name = "Answer")]
		public string Text { get; set; }

		public int ID { get; set; }
		public int QuestionID { get; set; }
		public int SessionID { get; set; }
		public string AnswererID { get; set; }
		public string QuestionText { get; set; }
	}

}