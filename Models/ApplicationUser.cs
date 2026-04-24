using Microsoft.AspNetCore.Identity;

namespace OnlineCourseSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; }
        public string? Bio { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public UserRole Role { get; set; } = UserRole.Student;

        // Navigation
        public ICollection<Course> TaughtCourses { get; set; } = new List<Course>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<FavoriteCourse> FavoriteCourses { get; set; } = new List<FavoriteCourse>();
    }

    public enum UserRole { Student, Instructor, Admin }
}
