using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Uchat.Models;

namespace Uchat.Controllers
{
    public class QuizTemplateAnswersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: QuizTemplateAnswers
        public ActionResult Index(int questionId)
        {
			QuizTemplateQuestion question = db.QuizTemplateQuestions.Find(questionId);
			db.Entry(question).Collection(q => q.Answers).Load();
			return View(question);
        }

        // GET: QuizTemplateAnswers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuizTemplateAnswer quizTemplateAnswer = db.QuizTemplateAnswers.Find(id);
            if (quizTemplateAnswer == null)
            {
                return HttpNotFound();
            }
            return View(quizTemplateAnswer);
        }

        // GET: QuizTemplateAnswers/Create
        public ActionResult Create(int questionId)
        {
			QuizTemplateAnswer answer = new QuizTemplateAnswer()
			{
				QuestionID = questionId
			};
             
			return View(answer);
        }

        // POST: QuizTemplateAnswers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,QuestionID,Text,IsCorrect")] QuizTemplateAnswer quizTemplateAnswer)
        {
            if (ModelState.IsValid)
            {
                db.QuizTemplateAnswers.Add(quizTemplateAnswer);
                db.SaveChanges();
				return RedirectToAction("Index", new { questionId = quizTemplateAnswer.QuestionID });
            }

            return View(quizTemplateAnswer);
        }

        // GET: QuizTemplateAnswers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuizTemplateAnswer quizTemplateAnswer = db.QuizTemplateAnswers.Find(id);
            if (quizTemplateAnswer == null)
            {
                return HttpNotFound();
            }
            return View(quizTemplateAnswer);
        }

        // POST: QuizTemplateAnswers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,QuestionID,Text,IsCorrect")] QuizTemplateAnswer quizTemplateAnswer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(quizTemplateAnswer).State = EntityState.Modified;
                db.SaveChanges();
				return RedirectToAction("Index", new { questionId = quizTemplateAnswer.QuestionID });
            }
            ViewBag.QuestionID = new SelectList(db.QuizTemplateQuestions, "ID", "Text", quizTemplateAnswer.QuestionID);
            return View(quizTemplateAnswer);
        }

        // GET: QuizTemplateAnswers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuizTemplateAnswer quizTemplateAnswer = db.QuizTemplateAnswers.Find(id);
            if (quizTemplateAnswer == null)
            {
                return HttpNotFound();
            }
            return View(quizTemplateAnswer);
        }

        // POST: QuizTemplateAnswers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            QuizTemplateAnswer quizTemplateAnswer = db.QuizTemplateAnswers.Find(id);
            db.QuizTemplateAnswers.Remove(quizTemplateAnswer);
			int qid = quizTemplateAnswer.QuestionID;
            db.SaveChanges();
			return RedirectToAction("Index", new { questionId = qid });
        }

		// GET: QuizTemplateAnswers/TemplateList/5
		public ActionResult TemplateList(int courseId)
		{
			return RedirectToAction("Index", "QuizTemplateQuestions", new { courseId = courseId });
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
