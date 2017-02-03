using System;
using System.Collections.Generic;
namespace Uchat.Models
{
	public class Session
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public DateTime DateStarted { get; set; } 
		public int CourseID { get; set; }
		public bool Ended { get; set; }

		public Course Course { get; set; }
		public ICollection<Question> Questions { get; set; }
		public ICollection<QuizQuestion> QuizQuestions { get; set; }
	}
}