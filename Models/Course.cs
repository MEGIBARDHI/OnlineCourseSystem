namespace OnlineCourseSystem.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Level { get; set; } = "Beginner";
        public decimal Price { get; set; }
        public bool IsPublished { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string InstructorId { get; set; } = string.Empty;

        // Navigation
        public ApplicationUser? Instructor { get; set; }
        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<FavoriteCourse> FavoriteCourses { get; set; } = new List<FavoriteCourse>();

        // Computed
        public double AverageRating => Ratings.Any() ? Ratings.Average(r => r.Stars) : 0;
        public int TotalStudents => Enrollments.Count;
        public int TotalLessons => Lessons.Count;
    }

    public class Lesson
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? VideoUrl { get; set; }
        public string? VideoFile { get; set; }
        public int Order { get; set; }
        public int DurationMinutes { get; set; }
        public int CourseId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Course? Course { get; set; }
        public ICollection<LessonProgress> Progresses { get; set; } = new List<LessonProgress>();
    }

    public class Enrollment
    {
        public int Id { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public bool CertificateIssued { get; set; } = false;

        // Navigation
        public ApplicationUser? Student { get; set; }
        public Course? Course { get; set; }
        public ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();

        // Computed
        public double CompletionPercentage
        {
            get
            {
                if (Course == null || !Course.Lessons.Any()) return 0;
                int completed = LessonProgresses.Count(lp => lp.IsCompleted);
                return Math.Round((double)completed / Course.Lessons.Count * 100, 1);
            }
        }
    }

    public class LessonProgress
    {
        public int Id { get; set; }
        public int EnrollmentId { get; set; }
        public int LessonId { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime? CompletedAt { get; set; }

        // Navigation
        public Enrollment? Enrollment { get; set; }
        public Lesson? Lesson { get; set; }
    }

    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ApplicationUser? User { get; set; }
        public Course? Course { get; set; }
    }

    public class Rating
    {
        public int Id { get; set; }
        public int Stars { get; set; }
        public string? Review { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ApplicationUser? User { get; set; }
        public Course? Course { get; set; }
    }

    public class FavoriteCourse
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ApplicationUser? User { get; set; }
        public Course? Course { get; set; }
    }
}
