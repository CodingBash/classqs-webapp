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
    public class QuizTemplateQuestionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

		public UserManager<ApplicationUser> UserManager { get; private set; }

		public QuizTemplateQuestionsController()
			: this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
		{
		}

		public QuizTemplateQuestionsController(UserManager<ApplicationUser> userManager)
		{
			UserManager = userManager;
		}

        // GET: QuizTemplateQuestions
        public ActionResult Index(int? courseId)
        {
			ApplicationUser user = UserManager.FindById(User.Identity.GetUserId());
			if (user == null)
			{
				return HttpNotFound();
			}
			if (user.UserType == ApplicationUser.UserTypes.Teacher)
			{
				Course course = db.Courses.Find(courseId);
				var questions = from q in db.QuizTemplateQuestions.Include(a => a.Answers)
								where q.CourseID == courseId
								select q;

				ListQuizTemplateQuestionViewModel view = new ListQuizTemplateQuestionViewModel()
				{
					Course = course,
					Questions = questions
				};

				return View(view);
			}
			else
			{
				return RedirectToAction("Index", "Enrollments");
			}
        }

        // GET: QuizTemplateQuestions/Details/5
        public ActionResult Details(int? id)
        {
			return RedirectToAction("Index", "QuizTemplateAnswers", new { questionId = id });
			//if (id == null)
			//{
			//	return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			//}
			//QuizTemplateQuestion quizTemplateQuestion = db.QuizTemplateQuestions.Find(id);
			//if (quizTemplateQuestion == null)
			//{
			//	return HttpNotFound();
			//}
			//quizTemplateQuestion.Course = db.Courses.Find(quizTemplateQuestion.CourseID);
			//return View(quizTemplateQuestion);
        }

        // GET: QuizTemplateQuestions/Create
        public ActionResult Create(int? courseId)
        {
			if (courseId == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Course course = db.Courses.Find(courseId);
			AddQuizTemplateQuestionViewModel view = new AddQuizTemplateQuestionViewModel
			{
				Course = course
			};
			return View(view);
        }

        // POST: QuizTemplateQuestions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AddQuizTemplateQuestionViewModel view)
        {
            if (ModelState.IsValid)
            {
				QuizTemplateQuestion quizTemplateQuestion = new QuizTemplateQuestion()
				{
					CourseID = view.CourseId,
					Text = view.Text
				}; 
				db.QuizTemplateQuestions.Add(quizTemplateQuestion);
                db.SaveChanges();
				return RedirectToAction("Index", new { courseId = view.CourseId });
            }

            return View(view);
        }

        // GET: QuizTemplateQuestions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuizTemplateQuestion quizTemplateQuestion = db.QuizTemplateQuestions.Find(id);
            if (quizTemplateQuestion == null)
            {
                return HttpNotFound();
            }
			db.Entry(quizTemplateQuestion).Reference(c => c.Course).Load();
			return View(quizTemplateQuestion);
        }

        // POST: QuizTemplateQuestions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Text,CourseID")] QuizTemplateQuestion quizTemplateQuestion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(quizTemplateQuestion).State = EntityState.Modified;
                db.SaveChanges();
				return RedirectToAction("Index", new { courseId = quizTemplateQuestion.CourseID });
            }
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", quizTemplateQuestion.CourseID);
            return View(quizTemplateQuestion);
        }

        // GET: QuizTemplateQuestions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuizTemplateQuestion quizTemplateQuestion = db.QuizTemplateQuestions.Find(id);
            if (quizTemplateQuestion == null)
            {
                return HttpNotFound();
            }
            return View(quizTemplateQuestion);
        }

        // POST: QuizTemplateQuestions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            QuizTemplateQuestion quizTemplateQuestion = db.QuizTemplateQuestions.Find(id);
			quizTemplateQuestion.Answers.ToList().ForEach(q => db.QuizTemplateAnswers.Remove(q));
            db.QuizTemplateQuestions.Remove(quizTemplateQuestion);
            db.SaveChanges();
            return RedirectToAction("Index");
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
