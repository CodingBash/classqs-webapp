using System.Collections.Generic;

namespace Uchat.Models
{
	public class Course
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string TeacherID { get; set; }

		public ApplicationUser Teacher { get; set; }
		public ICollection<Session> Sessions { get; set; }
		public ICollection<QuizTemplateQuestion> QuizTemplateQuestions { get; set; }
	}
}