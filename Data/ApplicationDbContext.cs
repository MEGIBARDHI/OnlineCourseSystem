using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineCourseSystem.Models;

namespace OnlineCourseSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<LessonProgress> LessonProgresses { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<FavoriteCourse> FavoriteCourses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Course → Instructor
            builder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(u => u.TaughtCourses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Enrollment → Student
            builder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Enrollment → Course
            builder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique enrollment per student per course
            builder.Entity<Enrollment>()
                .HasIndex(e => new { e.StudentId, e.CourseId })
                .IsUnique();

            // Comment → User
            builder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Rating unique per user per course
            builder.Entity<Rating>()
                .HasIndex(r => new { r.UserId, r.CourseId })
                .IsUnique();

            builder.Entity<Rating>()
                .HasOne(r => r.User)
                .WithMany(u => u.Ratings)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // FavoriteCourse unique
            builder.Entity<FavoriteCourse>()
                .HasIndex(f => new { f.UserId, f.CourseId })
                .IsUnique();

            builder.Entity<FavoriteCourse>()
                .HasOne(f => f.User)
                .WithMany(u => u.FavoriteCourses)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Course price precision
            builder.Entity<Course>()
                .Property(c => c.Price)
                .HasColumnType("decimal(10,2)");

            builder.Entity<LessonProgress>()
    .HasOne(lp => lp.Enrollment)
    .WithMany(e => e.LessonProgresses)
    .HasForeignKey(lp => lp.EnrollmentId)
    .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<LessonProgress>()
    .HasOne(lp => lp.Lesson)
    .WithMany(l => l.Progresses)
    .HasForeignKey(lp => lp.LessonId)
    .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
