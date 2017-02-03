namespace Uchat.Models
{
	public class QuizChoice
	{
		public int ID { get; set; }
		public int QuestionID { get; set; }
		public string Text { get; set; }
		public bool IsCorrect { get; set; }

		public QuizQuestion Question { get; set; }
	}
}