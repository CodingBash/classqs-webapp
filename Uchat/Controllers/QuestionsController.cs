using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Uchat.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Uchat.Controllers
{
	[Authorize]
	public class QuestionsController : Controller
	{
		private ApplicationDbContext db = new ApplicationDbContext();

		public UserManager<ApplicationUser> UserManager { get; private set; }

		public QuestionsController()
			: this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
		{
		}

		public QuestionsController(UserManager<ApplicationUser> userManager)
		{
			UserManager = userManager;
		}

		// GET: /Question/
		public ActionResult Index(int? sessionId)
		{
			if (sessionId == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			ApplicationUser user = UserManager.FindById(User.Identity.GetUserId());

			Session session = db.Sessions.Find(sessionId);
			db.Entry(session).Reference(c => c.Course).Load();
			db.Entry(session).Collection(q => q.QuizQuestions).Load();

			var questions = from q in db.Questions.Include(s => s.Likes).Include(s => s.Answers).Include(s => s.Student)
							where q.SessionID == (int)sessionId
							select q;

			var quizAnswers = from a in db.QuizAnswers
							  where a.SessionID == sessionId && a.AnswererID == user.Id
							  select a;

			foreach (Question q in questions)
			{
				//q.LikeCount = q.Likes.Count();
				foreach (Answer a in q.Answers)
				{
					a.Answerer = UserManager.FindById(a.AnswererID);
					if (a.Text.Length > 40)
					{
						a.Text = a.Text.Substring(0, 40) + "...";
					}
					if (a.Answerer.UserType == ApplicationUser.UserTypes.Teacher)
					{
						a.Text = "Prof: " + a.Text;
					}
					else
					{
						a.Text = "Aid: " + a.Text;
					}
				}
			}

			ListQuestionViewModel view = new ListQuestionViewModel()
			{
				SessionID = session.ID,
				SessionName = session.Name,
				SessionEnded = session.Ended,
				CourseID = session.CourseID,
				CourseName = session.Course.Name,
				UserType = user.UserType,
				UserID = user.Id,
				UnansweredQuizQuestions = session.QuizQuestions.Count() - quizAnswers.Count(),
				Questions = questions.ToList().OrderByDescending(s => s.Likes.Count())
			};

			return View(view);
		}

		// GET: /Question/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Question question = db.Questions.Find(id);
			if (question == null)
			{
				return HttpNotFound();
			}
			return View(question);
		}

		// GET: /Question/Create
		public ActionResult Create(int? sessionId)
		{
			if (sessionId == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			NewQuestionViewModel view = new NewQuestionViewModel()
			{
				SessionID = (int)sessionId
			};
			return View(view);
		}

		public ActionResult TakeQuiz(int sessionId)
		{
			return RedirectToAction("Index", "QuizAnswers", new { sessionId = sessionId });
		}

		// POST: /Question/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(NewQuestionViewModel view)
		{
			if (ModelState.IsValid)
			{
				Question question = new Question()
				{
					Text = view.Text,
					SessionID = view.SessionID,
					StudentID = User.Identity.GetUserId()
				};
				db.Questions.Add(question);
				db.SaveChanges();
				return RedirectToAction("Index", new { sessionId = view.SessionID });
			}

			return View(view);
		}

		// GET: /Question/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Question question = db.Questions.Find(id);
			if (question == null)
			{
				return HttpNotFound();
			}
			return View(question);
		}

		// POST: /Question/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "ID,Text,SessionID,StudentID")] Question question)
		{
			if (ModelState.IsValid)
			{
				db.Entry(question).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(question);
		}

		// GET: /Question/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Question question = db.Questions.Find(id);
			if (question == null)
			{
				return HttpNotFound();
			}
			return View(question);
		}

		// POST: /Question/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Question question = db.Questions.Find(id);
			db.Questions.Remove(question);
			db.SaveChanges();
			return RedirectToAction("Index");
		}

		// GET: /Question/Like/5
		public ActionResult Like(int questionId)
		{
			var userId = User.Identity.GetUserId();
			if (!db.QuestionLikes.Any(q => q.LikerID == userId && q.QuestionID == questionId))
			{
				QuestionLike newlike = new QuestionLike()
				{
					QuestionID = questionId,
					LikerID = userId
				};
				db.QuestionLikes.Add(newlike);
				db.SaveChanges();
			}

			Question question = db.Questions.Find(questionId);
			return RedirectToAction("Index", new { sessionId = question.SessionID });
		}

		//GET: /Question/Answer/5
		public ActionResult Answer(int questionId)
		{
			var userId = User.Identity.GetUserId();
			if (db.Answers.Any(a => a.AnswererID == userId && a.QuestionID == questionId))
			{
				return RedirectToAction("Create", "Answers", new { questionId = questionId });
			}
			else
			{
				return RedirectToAction("Edit", "Answers", new { questionId = questionId });
			}
		}

		//GET: /Question/ViewAnswer/5
		public ActionResult ViewAnswer(int answerId)
		{
			return RedirectToAction("View", "Answers", new { answerId = answerId });
		}

		// GET: /Question/SessionIndex
		public ActionResult SessionIndex(int courseId)
		{
			return RedirectToAction("Index", "Sessions", new { courseId = courseId });
		}

		// GET: /Question/Quiz
		public ActionResult Quiz(int sessionId)
		{
			return RedirectToAction("Create", "QuizQuestions", new { sessionId = sessionId });                                                                                                                                                                                                                                                                                                                     
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
