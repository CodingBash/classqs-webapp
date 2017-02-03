using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Uchat.Models;

namespace Uchat.Controllers
{
	[Authorize]
	public class AnswersController : Controller
	{
		private ApplicationDbContext db = new ApplicationDbContext();

		public UserManager<ApplicationUser> UserManager { get; private set; }

		public AnswersController()
			: this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
		{
		}

		public AnswersController(UserManager<ApplicationUser> userManager)
		{
			UserManager = userManager;
		}

		//GET: /Answer/QuestionIndex
		public ActionResult QuestionIndex(int sessionId)
		{
			return RedirectToAction("Index", "Questions", new { sessionId = sessionId });
		}

		// GET: /Answer/View/5
		public ActionResult View(int? answerId)
		{
			if (answerId == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Answer answer = db.Answers.Find(answerId);
			if (answer == null)
			{
				return HttpNotFound();
			}

			db.Entry(answer).Reference(q => q.Question).Load();
			db.Entry(answer).Reference(a => a.Answerer).Load();
			return View(answer);
		}

		//GET: /Answer/Create/5
		public ActionResult Create(int questionId)
		{
			ApplicationUser user = UserManager.FindById(User.Identity.GetUserId());
			Question question = db.Questions.Find(questionId);

			if (user.UserType == ApplicationUser.UserTypes.Student)
			{
				return RedirectToAction("Index", "Questions", new { sessionId = question.SessionID });
			}

			if (db.Answers.Any(a => a.AnswererID == user.Id && a.QuestionID == questionId))
			{
				return RedirectToAction("Edit", new { questionId = questionId });
			}

			AddQuestionAnswerViewModel view = new AddQuestionAnswerViewModel()
			{
				AnswererID = user.Id,
				QuestionID = questionId,
				SessionID = question.SessionID,
				QuestionText = question.Text
			};

			return View(view);
		}

		//POST: /Answer/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(AddQuestionAnswerViewModel view)
		{
			if (ModelState.IsValid)
			{
				Answer answer = new Answer()
				{
					QuestionID = view.QuestionID,
					AnswererID = view.AnswererID,
					Text = view.Text
				};
				db.Answers.Add(answer);
				db.SaveChanges();

				return RedirectToAction("Index", "Questions", new { sessionId = view.SessionID });
			}

			return View(view);
		}

		//GET: /Answer/Edit/5
		public ActionResult Edit(int questionId)
		{
			ApplicationUser user = UserManager.FindById(User.Identity.GetUserId());
			Question question = db.Questions.Find(questionId);
			//db.Entry(question).Collection(q => q.Answers).Query().Where(a => a.AnswererID.Equals(user.Id));

			if (user.UserType == ApplicationUser.UserTypes.Student)
			{
				return RedirectToAction("Index", "Questions", new { sessionId = question.SessionID });
			}

			//I removed this code and instead rely on the answers.Count() below to take me to Create.
			//if (!db.Answers.Any(a => a.AnswererID == user.Id && a.QuestionID == questionId))
			//{
			//	return RedirectToAction("Create", new { questionId = questionId });
			//}

			var answers = from a in db.Answers
						  where a.AnswererID == user.Id && a.QuestionID == questionId
						  select a;
			if (answers.Count() == 0)
			{
				return RedirectToAction("Create", new { questionId = questionId });
			}
			Answer answer = answers.First();

			EditQuestionAnswerViewModel view = new EditQuestionAnswerViewModel()
			{
				ID = answer.ID,
				QuestionID = answer.QuestionID,
				SessionID = question.SessionID,
				AnswererID = user.Id,
				QuestionText = question.Text,
				Text = answer.Text
			};

			return View(view);
		}

		//POST: /Answer/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EditQuestionAnswerViewModel view)
		{
			if (ModelState.IsValid)
			{
				Answer answer = new Answer()
				{
					ID = view.ID,
					QuestionID = view.QuestionID,
					AnswererID = view.AnswererID,
					Text = view.Text
				};
				db.Entry(answer).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index", "Questions", new { sessionId = view.SessionID });
			}

			return View(view);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
