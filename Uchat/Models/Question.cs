using System.Collections.Generic;
namespace Uchat.Models
{
	public class Question
	{
		public int ID { get; set; }
		public string Text { get; set; }
		public int SessionID { get; set; }
		public string StudentID { get; set; }

		public Session Session { get; set; }
		public ApplicationUser Student { get; set; }
		public ICollection<Answer> Answers { get; set; }
		public ICollection<QuestionLike> Likes { get; set; }
	}
}