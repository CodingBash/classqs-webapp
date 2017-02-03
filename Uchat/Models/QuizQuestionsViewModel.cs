using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Uchat.Models
{
	public class SelectQuizQuestionsViewModel
	{
		public Session Session { get; set; }

		public IEnumerable<string> SelectedQuestions { get; set; }
	}

}