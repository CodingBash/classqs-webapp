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
	public class EnrollmentsController : Controller
	{
		private ApplicationDbContext db = new ApplicationDbContext();

		public UserManager<ApplicationUser> UserManager { get; private set; }

		public EnrollmentsController()
			: this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
		{
		}

		public EnrollmentsController(UserManager<ApplicationUser> userManager)
		{
			UserManager = userManager;
		}

		// GET: /Enrollment/
		public ActionResult Index()
		{
			//var enrollments = db.Enrollments.Include(e => e.Course);
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
				var enrollments = from e in db.Enrollments.Include(e => e.Course)
								  where e.StudentID == user.Id
								  select new ListEnrollmentViewModel()
								  {
									  ID = e.ID,
									  Course = e.Course.Name,
									  Teacher = e.Course.Teacher.Email
								  };
				return View(enrollments.ToList());
			}
		}

		// GET: /Enrollment/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Enrollment enrollment = db.Enrollments.Find(id);
			if (enrollment == null)
			{
				return HttpNotFound();
			}
			return RedirectToAction("Index", "Sessions", new { courseID = enrollment.CourseID });
			//return View(enrollment);
		}

		// GET: /Enrollment/Add
		public ActionResult Add()
		{
			//ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name");
			//return View();

			var courses = from c in db.Courses.Include(a => a.Teacher)
						  select c;

			return View(courses);
		}

		// POST: /Enrollment/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public ActionResult Create([Bind(Include="ID,CourseID,StudentID")] Enrollment enrollment)
		//{
		//	if (ModelState.IsValid)
		//	{
		//		db.Enrollments.Add(enrollment);
		//		db.SaveChanges();
		//		return RedirectToAction("Index");
		//	}

		//	ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", enrollment.CourseID);
		//	return View(enrollment);
		//}

		// GET: /Enrollment/Enroll/5
		public ActionResult Enroll(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Course course = db.Courses.Find(id);
			Enrollment enrollment = new Enrollment()
			{
				CourseID = course.ID,
				StudentID = User.Identity.GetUserId()
			};

			db.Enrollments.Add(enrollment);
			db.SaveChanges();

			return RedirectToAction("Index");
		}

		// GET: /Enrollment/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Enrollment enrollment = db.Enrollments.Find(id);
			db.Enrollments.Remove(enrollment);
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
