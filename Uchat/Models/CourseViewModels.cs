using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Uchat.Models
{
	public class AddCourseViewModel
	{
		[Required]
		[Display(Name = "Course Name")]
		public string Name { get; set; }
	}

	public class ListCourseViewModel
	{
		public int ID { get; set; }
		public string Name { get; set; }
	}

	public class EditCourseViewModel
	{
		[Required]
		[Display(Name = "Course Name")]
		public string Name { get; set; }

		public int ID { get; set; }
	}
}