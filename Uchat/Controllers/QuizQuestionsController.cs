using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Uchat.Models;

namespace Uchat.Controllers
{
	[Authorize]
    public class QuizQuestionsController : Controller
    {
		private ApplicationDbContext db = new ApplicationDbContext();

		public UserManager<ApplicationUser> UserManager { get; private set; }

		public QuizQuestionsController()
			: this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
		{
		}

		public QuizQuestionsController(UserManager<ApplicationUser> userManager)
		{
			UserManager = userManager;
		}

        // GET: QuizQuestions
        public ActionResult Index(int sessionId)
        {
			// This will simply return control to the Session index page.
			return RedirectToAction("Index", "Questions", new { sessionId = sessionId });
        }

        // GET: QuizQuestions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuizQuestion quizQuestion = db.QuizQuestions.Find(id);
            if (quizQuestion == null)
            {
                return HttpNotFound();
            }
            return View(quizQuestion);
        }

        // GET: QuizQuestions/Create
        public ActionResult Create(int sessionId)
        {
			SelectQuizQuestionsViewModel view = new SelectQuizQuestionsViewModel();
			view.Session = db.Sessions.Find(sessionId);
			db.Entry(view.Session).Reference(c => c.Course).Load();
			db.Entry(view.Session.Course).Collection(q => q.QuizTemplateQuestions).Load();

			return View(view);
        }

        // POST: QuizQuestions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SelectQuizQuestionsViewModel view)
        {
            if (ModelState.IsValid)
            {
				foreach (var q in view.SelectedQuestions)
				{
					//Get the selected quiz template.
					QuizTemplateQuestion temp = db.QuizTemplateQuestions.Find(Convert.ToInt32(q));
					db.Entry(temp).Collection(a => a.Answers).Load();

					//Copy question template to new quiz question object and save to database.
					QuizQuestion quizQuestion = new QuizQuestion()
					{
						Text = temp.Text,
						SessionID = view.Session.ID
					};
					db.QuizQuestions.Add(quizQuestion);
					db.SaveChanges();
					
					//Copy template answers to new choice objects and save each to database.
					foreach (var a in temp.Answers)
					{
						QuizChoice quizChoice = new QuizChoice()
						{
							Text = a.Text,
							QuestionID = quizQuestion.ID,
							IsCorrect = a.IsCorrect
						};

						db.QuizChoices.Add(quizChoice);
					}
					db.SaveChanges();
				}

				return RedirectToAction("Index", "Questions", new { sessionId = view.Session.ID });
			}

            return View(view);
        }

        // GET: QuizQuestions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuizQuestion quizQuestion = db.QuizQuestions.Find(id);
            if (quizQuestion == null)
            {
                return HttpNotFound();
            }
            ViewBag.SessionID = new SelectList(db.Sessions, "ID", "Name", quizQuestion.SessionID);
            return View(quizQuestion);
        }

        // POST: QuizQuestions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Text,SessionID")] QuizQuestion quizQuestion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(quizQuestion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SessionID = new SelectList(db.Sessions, "ID", "Name", quizQuestion.SessionID);
            return View(quizQuestion);
        }

        // GET: QuizQuestions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuizQuestion quizQuestion = db.QuizQuestions.Find(id);
            if (quizQuestion == null)
            {
                return HttpNotFound();
            }
            return View(quizQuestion);
        }

        // POST: QuizQuestions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            QuizQuestion quizQuestion = db.QuizQuestions.Find(id);
            db.QuizQuestions.Remove(quizQuestion);
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
