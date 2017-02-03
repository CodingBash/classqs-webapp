namespace Uchat.Models
{
	public class Enrollment
	{
		public int ID { get; set; }
		public int CourseID { get; set; }
		public string StudentID { get; set; }

		public Course Course { get; set; }
		public ApplicationUser Student { get; set; }
	}
}