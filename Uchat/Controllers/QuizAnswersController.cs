using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Uchat.Models;

namespace Uchat.Controllers
{
	[Authorize]
    public class QuizAnswersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
		private ICollection<QuizQuestion> quizQuestions = new List<QuizQuestion>();

		public UserManager<ApplicationUser> UserManager { get; private set; }

		public QuizAnswersController()
			: this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
		{
		}

		public QuizAnswersController(UserManager<ApplicationUser> userManager)
		{
			UserManager = userManager;
		}

		// GET: QuizAnswers
        public ActionResult Index(int sessionId, int questionId = 0, int direction = 1)
        {
			AskQuizQuestionViewModel newQuestion = new AskQuizQuestionViewModel();

			if (questionId == 0)
			{
				newQuestion = GetFirstUnansweredQuestion(sessionId);
			}
			else
			{
				newQuestion = GetNextQuestion(sessionId, questionId, direction);
			}

			//use newQuestion to populate view for display.
			//ERROR Something about coming through here the second time is causing a context error with QuizQuestion.
			db.Entry(newQuestion.Question).Collection(q => q.Choices).Load();

			EditQuizAnswerViewModel view = new EditQuizAnswerViewModel()
			{
				SessionId = sessionId,
				QuestionId = newQuestion.Question.ID,
				CurrentQuestionNum = newQuestion.CurrentQuestionNum,
				TotalQuestions = quizQuestions.Count(),
				QuestionText = newQuestion.Question.Text,
			};

			//Now see if the user has already supplied an answer.
			string userId = User.Identity.GetUserId();
			QuizAnswer quizAnswer = db.QuizAnswers.SingleOrDefault(ans => ans.QuestionID == view.QuestionId && ans.AnswererID == userId);
			if (quizAnswer != null)
			{
				view.StudentAnswer = quizAnswer.Text;
			}

			//Using the stripped down QuizChoiceViewModel to remove the "IsCorrect" flag from the data
			//sent to the viewe page.
			ICollection<QuizChoiceViewModel> quizChoices = new List<QuizChoiceViewModel>();
			foreach (var choice in newQuestion.Question.Choices)
			{
				QuizChoiceViewModel newChoice = new QuizChoiceViewModel()
				{
					ID = choice.ID,
					Text = choice.Text
				};

				quizChoices.Add(newChoice);
			}
			view.QuestionChoices = quizChoices;

            return View(view);
        }

		[HttpPost]
		public ActionResult Index(string submitButton, EditQuizAnswerViewModel view)
		{
			if (ModelState.IsValid && view.StudentAnswer != null)
			{
				//Need to check for an existing answer first. If it exists, we update instead of add.
				string userId = User.Identity.GetUserId();
				QuizAnswer quizAnswer = db.QuizAnswers.SingleOrDefault(ans => ans.QuestionID == view.QuestionId && ans.AnswererID == userId);

				if (quizAnswer == null)
				{
					quizAnswer = new QuizAnswer()
					{
						SessionID = view.SessionId,
						QuestionID = view.QuestionId,
						Text = view.StudentAnswer,
						AnswererID = User.Identity.GetUserId(),
						IsCorrect = false
					};
					db.QuizAnswers.Add(quizAnswer);
				}
				else
				{
					//No need to update if there is no change.
					if (quizAnswer.Text != view.StudentAnswer)
					{
						quizAnswer.Text = view.StudentAnswer;
						db.Entry(quizAnswer).State = EntityState.Modified;
					}
				}
				db.SaveChanges();
			}

			switch (submitButton)
			{
				case "Next":
					return RedirectToAction("Index", new { sessionId = view.SessionId, questionId = view.QuestionId, direction = 1 });
					
				case "Prev":
					return RedirectToAction("Index", new { sessionId = view.SessionId, questionId = view.QuestionId, direction = -1 });

				case "Finish":
					return RedirectToAction("Index", "Questions", new { sessionId = view.SessionId });

				default:
					goto case "Next";
			}
		}

		//Get the next question in the quiz. Might repurpose this to go both directions based on parameters.
		public AskQuizQuestionViewModel GetNextQuestion(int sessionId, int questionId, int direction)
		{
			ICollection<QuizQuestion> sortedQuestions = new List<QuizQuestion>();
			QuizQuestion newQuestion = new QuizQuestion();
			bool thisQuestion = false;
			int questionCtr = 0;

			quizQuestions = (from q in db.QuizQuestions
							 where q.SessionID == sessionId
							 select q).ToList();

			//If 1, then go forward. Any other value will mean go backward, but I will use -1.
			if (direction == 1)
			{
				sortedQuestions = quizQuestions.OrderBy(q => q.ID).ToList();
			}
			else
			{
				sortedQuestions = quizQuestions.OrderByDescending(q => q.ID).ToList();
				questionCtr = sortedQuestions.Count() + 1;
			}

			foreach (QuizQuestion question in sortedQuestions)
			{
				newQuestion = question;
				if (direction == 1) { questionCtr += 1; } else { questionCtr -= 1; }
				
				if (thisQuestion)
				{
					break;
				}
				if (questionId == question.ID)
				{
					thisQuestion = true;
				}
			}

			return new AskQuizQuestionViewModel()
			{
				CurrentQuestionNum = questionCtr,
				Question = newQuestion
			};
		}

		//This will look for the first unanswered question on the quiz and start it there.
		//This assumes that they are starting the quiz from the original class Question Controller,
		//and will actually re-load the questions from the database.
		public AskQuizQuestionViewModel GetFirstUnansweredQuestion(int sessionId)
		{
			QuizQuestion newQuestion = new QuizQuestion();
			int questionCtr = 0;

			quizQuestions = (from q in db.QuizQuestions
							where q.SessionID == sessionId
							select q).ToList();

			foreach (QuizQuestion question in quizQuestions.OrderBy(q => q.ID))
			{
				newQuestion = question;
				questionCtr += 1;
				string userId = User.Identity.GetUserId();

				if (db.QuizAnswers.Any(q => q.AnswererID == userId && q.QuestionID == question.ID))
				{
					continue;
				}
				else
				{
					break;
				}
			}

			return new AskQuizQuestionViewModel()
			{
				CurrentQuestionNum = questionCtr,
				Question = newQuestion
			};
		}

		public ActionResult QuestionIndex(int sessionId)
		{
			return RedirectToAction("Index", "Questions", new { sessionId = sessionId });
		}

        // GET: QuizAnswers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuizAnswer quizAnswer = db.QuizAnswers.Find(id);
            if (quizAnswer == null)
            {
                return HttpNotFound();
            }
            return View(quizAnswer);
        }

        // GET: QuizAnswers/Create
        public ActionResult Create()
        {
            ViewBag.AnswererID = new SelectList(db.Users, "Id", "Email");
            ViewBag.QuestionID = new SelectList(db.QuizQuestions, "ID", "Text");
            return View();
        }

        // POST: QuizAnswers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,QuestionID,Text,AnswererID,IsCorrect")] QuizAnswer quizAnswer)
        {
            if (ModelState.IsValid)
            {
                db.QuizAnswers.Add(quizAnswer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AnswererID = new SelectList(db.Users, "Id", "Email", quizAnswer.AnswererID);
            ViewBag.QuestionID = new SelectList(db.QuizQuestions, "ID", "Text", quizAnswer.QuestionID);
            return View(quizAnswer);
        }

        // GET: QuizAnswers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuizAnswer quizAnswer = db.QuizAnswers.Find(id);
            if (quizAnswer == null)
            {
                return HttpNotFound();
            }
            ViewBag.AnswererID = new SelectList(db.Users, "Id", "Email", quizAnswer.AnswererID);
            ViewBag.QuestionID = new SelectList(db.QuizQuestions, "ID", "Text", quizAnswer.QuestionID);
            return View(quizAnswer);
        }

        // POST: QuizAnswers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,QuestionID,Text,AnswererID,IsCorrect")] QuizAnswer quizAnswer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(quizAnswer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AnswererID = new SelectList(db.Users, "Id", "Email", quizAnswer.AnswererID);
            ViewBag.QuestionID = new SelectList(db.QuizQuestions, "ID", "Text", quizAnswer.QuestionID);
            return View(quizAnswer);
        }

        // GET: QuizAnswers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuizAnswer quizAnswer = db.QuizAnswers.Find(id);
            if (quizAnswer == null)
            {
                return HttpNotFound();
            }
            return View(quizAnswer);
        }

        // POST: QuizAnswers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            QuizAnswer quizAnswer = db.QuizAnswers.Find(id);
            db.QuizAnswers.Remove(quizAnswer);
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
