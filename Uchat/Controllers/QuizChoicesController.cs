using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Uchat.Models;

namespace Uchat.Controllers
{
    public class QuizChoicesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: QuizChoices
        public ActionResult Index()
        {
            var quizChoices = db.QuizChoices.Include(q => q.Question);
            return View(quizChoices.ToList());
        }

        // GET: QuizChoices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuizChoice quizChoice = db.QuizChoices.Find(id);
            if (quizChoice == null)
            {
                return HttpNotFound();
            }
            return View(quizChoice);
        }

        // GET: QuizChoices/Create
        public ActionResult Create()
        {
            ViewBag.QuestionID = new SelectList(db.QuizQuestions, "ID", "Text");
            return View();
        }

        // POST: QuizChoices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,QuestionID,Text,IsCorrect")] QuizChoice quizChoice)
        {
            if (ModelState.IsValid)
            {
                db.QuizChoices.Add(quizChoice);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.QuestionID = new SelectList(db.QuizQuestions, "ID", "Text", quizChoice.QuestionID);
            return View(quizChoice);
        }

        // GET: QuizChoices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuizChoice quizChoice = db.QuizChoices.Find(id);
            if (quizChoice == null)
            {
                return HttpNotFound();
            }
            ViewBag.QuestionID = new SelectList(db.QuizQuestions, "ID", "Text", quizChoice.QuestionID);
            return View(quizChoice);
        }

        // POST: QuizChoices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,QuestionID,Text,IsCorrect")] QuizChoice quizChoice)
        {
            if (ModelState.IsValid)
            {
                db.Entry(quizChoice).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.QuestionID = new SelectList(db.QuizQuestions, "ID", "Text", quizChoice.QuestionID);
            return View(quizChoice);
        }

        // GET: QuizChoices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuizChoice quizChoice = db.QuizChoices.Find(id);
            if (quizChoice == null)
            {
                return HttpNotFound();
            }
            return View(quizChoice);
        }

        // POST: QuizChoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            QuizChoice quizChoice = db.QuizChoices.Find(id);
            db.QuizChoices.Remove(quizChoice);
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
