using System.ComponentModel.DataAnnotations;
using OnlineCourseSystem.Models;

namespace OnlineCourseSystem.ViewModels
{
    // Account ViewModels
    public class RegisterViewModel
    {
        [Required] public string FullName { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required, MinLength(6)] public string Password { get; set; } = string.Empty;
        [Compare("Password")] public string ConfirmPassword { get; set; } = string.Empty;
        [Required] public UserRole Role { get; set; } = UserRole.Student;
        public string? Bio { get; set; }
    }

    public class LoginViewModel
    {
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }

    // Course ViewModels
    public class CourseCreateViewModel
    {
        [Required, MaxLength(200)] public string Title { get; set; } = string.Empty;
        [Required] public string Description { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        [Required] public string Category { get; set; } = string.Empty;
        public string Level { get; set; } = "Beginner";
        [Range(0, 9999)] public decimal Price { get; set; }
        public bool IsPublished { get; set; }
    }

    public class CourseEditViewModel : CourseCreateViewModel
    {
        public int Id { get; set; }
    }

    public class CourseDetailsViewModel
    {
        public Course Course { get; set; } = null!;
        public bool IsEnrolled { get; set; }
        public bool IsFavorite { get; set; }
        public Enrollment? UserEnrollment { get; set; }
        public Rating? UserRating { get; set; }
        public string? NewComment { get; set; }
        public int NewRating { get; set; }
        public string? NewReview { get; set; }
    }

    public class CourseListViewModel
    {
        public IEnumerable<Course> Courses { get; set; } = new List<Course>();
        public string? SearchTerm { get; set; }
        public string? Category { get; set; }
        public string? Level { get; set; }
        public IEnumerable<string> Categories { get; set; } = new List<string>();
    }

    // Dashboard ViewModels
    public class StudentDashboardViewModel
    {
        public ApplicationUser User { get; set; } = null!;
        public IEnumerable<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public IEnumerable<Course> FavoriteCourses { get; set; } = new List<Course>();
        public int CompletedCourses { get; set; }
        public int InProgressCourses { get; set; }
    }

    public class InstructorDashboardViewModel
    {
        public ApplicationUser User { get; set; } = null!;
        public IEnumerable<Course> Courses { get; set; } = new List<Course>();
        public int TotalStudents { get; set; }
        public int TotalCourses { get; set; }
        public double AverageRating { get; set; }
    }

    // Lesson ViewModels
    public class LessonCreateViewModel
    {
        [Required] public string Title { get; set; } = string.Empty;
        [Required] public string Description { get; set; } = string.Empty;
        public string? VideoUrl { get; set; }
        public int Order { get; set; }
        public int DurationMinutes { get; set; }
        public int CourseId { get; set; }
    }

    public class LessonEditViewModel : LessonCreateViewModel
    {
        public int Id { get; set; }
    }
}
