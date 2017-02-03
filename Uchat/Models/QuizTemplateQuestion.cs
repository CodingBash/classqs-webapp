using System.Collections.Generic;

namespace Uchat.Models
{
	public class QuizTemplateQuestion
	{
		public int ID { get; set; }
		public string Text { get; set; }
		public int CourseID { get; set; }
		//Add a Tag field to allow for categorization of questions allowing easier lookup, such as by topic.

		public Course Course { get; set; }
		public ICollection<QuizTemplateAnswer> Answers { get; set; }
	}
}