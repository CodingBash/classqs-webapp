using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Uchat.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

		public enum UserTypes { Teacher, Aid, Student };

		public UserTypes UserType { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

		//public System.Data.Entity.DbSet<Uchat.Models.ApplicationUser> ApplicationUsers { get; set; }

		public System.Data.Entity.DbSet<Uchat.Models.Course> Courses { get; set; }
		
		public System.Data.Entity.DbSet<Uchat.Models.Enrollment> Enrollments { get; set; }

		public System.Data.Entity.DbSet<Uchat.Models.Session> Sessions { get; set; }

		public System.Data.Entity.DbSet<Uchat.Models.Question> Questions { get; set; }

		public System.Data.Entity.DbSet<Uchat.Models.Answer> Answers { get; set; }

		public System.Data.Entity.DbSet<Uchat.Models.QuestionLike> QuestionLikes { get; set; }

		public System.Data.Entity.DbSet<Uchat.Models.QuizTemplateQuestion> QuizTemplateQuestions { get; set; }

		public System.Data.Entity.DbSet<Uchat.Models.QuizTemplateAnswer> QuizTemplateAnswers { get; set; }

		public System.Data.Entity.DbSet<Uchat.Models.QuizQuestion> QuizQuestions { get; set; }

		public System.Data.Entity.DbSet<Uchat.Models.QuizChoice> QuizChoices { get; set; }

		public System.Data.Entity.DbSet<Uchat.Models.QuizAnswer> QuizAnswers { get; set; }
    }
}