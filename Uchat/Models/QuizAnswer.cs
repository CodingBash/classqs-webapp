namespace Uchat.Models
{
	public class QuizAnswer
	{
		public int ID { get; set; }
		public int QuestionID { get; set; }
		public string Text { get; set; }
		public string AnswererID { get; set; }
		public bool IsCorrect { get; set; }
		public int SessionID { get; set; }

		public QuizQuestion Question { get; set; }
		public ApplicationUser Answerer { get; set; }
		public Session Session { get; set; }
	}
}