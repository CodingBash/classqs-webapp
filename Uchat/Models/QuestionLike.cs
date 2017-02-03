namespace Uchat.Models
{
	public class QuestionLike
	{
		public int ID { get; set; }
		public int QuestionID { get; set; }
		public string LikerID { get; set; }

		public Question Question { get; set; }
		public ApplicationUser Liker { get; set; }
	}
}