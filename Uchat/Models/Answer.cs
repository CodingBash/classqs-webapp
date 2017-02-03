namespace Uchat.Models
{
	public class Answer
	{
		public int ID { get; set; }
		public int QuestionID { get; set; }
		public string AnswererID { get; set; }
		public string Text { get; set; }

		public Question Question { get; set; }
		public ApplicationUser Answerer { get; set; }
	}
}