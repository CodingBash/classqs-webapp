using System.Collections.Generic;

namespace Uchat.Models
{
	public class QuizQuestion
	{
		public int ID { get; set; }
		public string Text { get; set; }
		public int SessionID { get; set; }

		public Session Session { get; set; }
		public ICollection<QuizChoice> Choices { get; set; }
	}
}