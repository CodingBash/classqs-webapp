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
	public class CoursesController : Controller
	{
		private ApplicationDbContext db = new ApplicationDbContext();

		public UserManager<ApplicationUser> UserManager { get; private set; }

		public CoursesController()
			: this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
		{
		}

		public CoursesController(UserManager<ApplicationUser> userManager)
		{
			UserManager = userManager;
		}

		// GET: /Course/
		public ActionResult Index()
		{
			ApplicationUser user = UserManager.FindById(User.Identity.GetUserId());
			if (user == null)
			{
				return HttpNotFound();
			}
			if (user.UserType == ApplicationUser.UserTypes.Teacher)
			{
				var courses = from c in db.Courses
							  where c.TeacherID == user.Id
							  select new ListCourseViewModel()
							  {
								  ID = c.ID,
								  Name = c.Name
							  };
				return View(courses.ToList());
			}
			else
			{
				return RedirectToAction("Index", "Enrollments");
			}
		}

		// GET: /Course/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: /Course/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(AddCourseViewModel view)
		{
			ApplicationUser user = UserManager.FindById(User.Identity.GetUserId());

			if (ModelState.IsValid)
			{
				Course course = new Course();
				course.Name = view.Name;
				course.TeacherID = user.Id;
				db.Courses.Add(course);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(view);
		}

		// GET: /Course/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Course course = db.Courses.Find(id);
			if (course == null)
			{
				return HttpNotFound();
			}
			EditCourseViewModel view = new EditCourseViewModel()
			{
				ID = course.ID,
				Name = course.Name
			};
			return View(view);
		}

		// POST: /Course/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "ID,Name,TeacherID")] EditCourseViewModel view)
		{
			if (ModelState.IsValid)
			{
				Course course = new Course();
				course.ID = view.ID;
				course.Name = view.Name;
				course.TeacherID = User.Identity.GetUserId();
				db.Entry(course).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(view);
		}

		// GET: /Course/SessionList
		public ActionResult SessionList(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			return RedirectToAction("Index", "Sessions", new { courseID = id });
		}

		// GET: /Course/NewSession
		public ActionResult NewSession(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			return RedirectToAction("Create", "Sessions", new { courseID = id });
		}

		//GET: /Course/QuizQuestions
		public ActionResult QuizTemplates(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			return RedirectToAction("Index", "QuizTemplateQuestions", new { courseId = id });
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
