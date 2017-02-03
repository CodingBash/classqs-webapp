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
	public class SessionsController : Controller
	{
		private ApplicationDbContext db = new ApplicationDbContext();

		public UserManager<ApplicationUser> UserManager { get; private set; }

		public SessionsController()
			: this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
		{
		}

		public SessionsController(UserManager<ApplicationUser> userManager)
		{
			UserManager = userManager;
		}

		// GET: /Session/
		public ActionResult Index(int? courseID)
		{
			if (courseID == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			ApplicationUser user = UserManager.FindById(User.Identity.GetUserId());
			var sessions = from s in db.Sessions.Include(s => s.Course)
						   where s.CourseID == courseID
						   select s;
			Course course = db.Courses.Find(courseID);
			ListSessionViewModel view = new ListSessionViewModel()
			{
				CourseID = course.ID,
				CourseName = course.Name,
				IsTeacher = (user.UserType == ApplicationUser.UserTypes.Teacher),
				Sessions = sessions
			};
			return View(view);
		}

		// GET: /Session/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Session session = db.Sessions.Find(id);
			if (session == null)
			{
				return HttpNotFound();
			}
			return RedirectToAction("Index", "Questions", new { sessionId = id });
		}

		// GET: /Session/Create
		public ActionResult Create(int? courseID)
		{
			ApplicationUser user = UserManager.FindById(User.Identity.GetUserId());
			if (user == null)
			{
				return HttpNotFound();
			}
			if (courseID == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Course course = db.Courses.Find(courseID);
			if (user.UserType != ApplicationUser.UserTypes.Teacher)
			{
				return RedirectToAction("Index", "Sessions", new { courseID = courseID });
			}
			NewSessionViewModel view = new NewSessionViewModel();
			view.CourseID = (int)courseID;
			view.CourseName = course.Name;

			return View(view);
		}

		// POST: /Session/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(NewSessionViewModel view)
		{
			if (ModelState.IsValid)
			{
				var oldSessions = from s in db.Sessions.Include(s => s.Course)
								  where (s.CourseID == view.CourseID) && (s.Ended == false)
								  select s;
				foreach (Session s in oldSessions)
				{
					s.Ended = true;
					db.Entry(s).State = EntityState.Modified;
				}

				Session session = new Session()
				{
					CourseID = view.CourseID,
					Name = view.Name,
					DateStarted = System.DateTime.Now,
					Ended = false
				};
				db.Sessions.Add(session);
				db.SaveChanges();
				// Want to change the default below to redirect to Question list for session?
				return RedirectToAction("Index", "Questions", new { sessionID = session.ID });
			}

			return View(view);
		}

		// GET: /Session/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Session session = db.Sessions.Find(id);
			db.Entry(session).Reference(c => c.Course).Load();
			if (session == null)
			{
				return HttpNotFound();
			}
			EditSessionViewModel view = new EditSessionViewModel()
			{
				CourseID = session.Course.ID,
				ID = (int)id,
				Name = session.Name,
				Ended = session.Ended
			};
			return View(view);
		}

		// POST: /Session/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EditSessionViewModel view)
		{
			Session session = db.Sessions.Find(view.ID);
			if (ModelState.IsValid)
			{
				session.Name = view.Name;
				session.Ended = view.Ended;
				db.Entry(session).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index", new { courseID = session.CourseID });
			}
			return View(view);
		}

		// GET: /Session/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Session session = db.Sessions.Find(id);
			if (session == null)
			{
				return HttpNotFound();
			}
			return View(session);
		}

		// POST: /Session/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Session session = db.Sessions.Find(id);
			db.Sessions.Remove(session);
			db.SaveChanges();
			return RedirectToAction("Index");
		}

		// GET: /Session/CourseIndex
		public ActionResult CourseIndex()
		{
			ApplicationUser user = UserManager.FindById(User.Identity.GetUserId());
			if (user == null)
			{
				return HttpNotFound();
			}
			if (user.UserType == ApplicationUser.UserTypes.Teacher)
			{
				return RedirectToAction("Index", "Courses");
			}
			else
			{
				return RedirectToAction("Index", "Enrollments");
			}
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
