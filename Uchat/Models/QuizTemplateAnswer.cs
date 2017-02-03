namespace Uchat.Models
{
	public class QuizTemplateAnswer
	{
		public int ID { get; set; }
		public int QuestionID { get; set; }
		public string Text { get; set; }
		public bool IsCorrect { get; set; }

		public QuizTemplateQuestion Question { get; set; }
	}
}