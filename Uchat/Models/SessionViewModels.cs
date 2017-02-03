using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Uchat.Models
{
	public class NewSessionViewModel
	{
		public int CourseID { get; set; }
		public string CourseName { get; set; }

		[Required]
		[Display(Name = "Description")]
		public string Name { get; set; }
	}

	public class ListSessionViewModel
	{
		public int CourseID { get; set; }
		public string CourseName { get; set; }
		public bool IsTeacher { get; set; }
		public IEnumerable<Session> Sessions { get; set; }
	}

	public class EditSessionViewModel
	{
		public int CourseID { get; set; }
		public int ID { get; set; }

		[Required]
		[Display(Name = "Description")]
		public string Name { get; set; }
		public bool Ended { get; set; }
	}
}