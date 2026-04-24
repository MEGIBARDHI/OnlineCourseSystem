using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCourseSystem.Data;
using OnlineCourseSystem.Models;
using OnlineCourseSystem.ViewModels;

namespace OnlineCourseSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var featuredCourses = await _context.Courses
                .Where(c => c.IsPublished)
                .Include(c => c.Instructor)
                .Include(c => c.Ratings)
                .Include(c => c.Enrollments)
                .OrderByDescending(c => c.Enrollments.Count)
                .Take(6)
                .ToListAsync();

            var categories = await _context.Courses
                .Where(c => c.IsPublished)
                .Select(c => c.Category)
                .Distinct()
                .ToListAsync();

            ViewBag.FeaturedCourses = featuredCourses;
            ViewBag.Categories = categories;
            ViewBag.TotalCourses = await _context.Courses.CountAsync(c => c.IsPublished);
            ViewBag.TotalStudents = await _context.Users.CountAsync();
            ViewBag.TotalInstructors = (await _userManager.GetUsersInRoleAsync("Instructor")).Count;

            return View();
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (user.Role == UserRole.Instructor || await _userManager.IsInRoleAsync(user, "Instructor"))
            {
                var courses = await _context.Courses
                    .Where(c => c.InstructorId == user.Id)
                    .Include(c => c.Enrollments)
                    .Include(c => c.Ratings)
                    .Include(c => c.Lessons)
                    .ToListAsync();

                var vm = new InstructorDashboardViewModel
                {
                    User = user,
                    Courses = courses,
                    TotalCourses = courses.Count,
                    TotalStudents = courses.Sum(c => c.Enrollments.Count),
                    AverageRating = courses.Any() && courses.SelectMany(c => c.Ratings).Any()
                        ? courses.SelectMany(c => c.Ratings).Average(r => r.Stars)
                        : 0
                };
                return View("InstructorDashboard", vm);
            }
            else
            {
                var enrollments = await _context.Enrollments
                    .Where(e => e.StudentId == user.Id)
                    .Include(e => e.Course)
                        .ThenInclude(c => c!.Lessons)
                    .Include(e => e.Course)
                        .ThenInclude(c => c!.Instructor)
                    .Include(e => e.LessonProgresses)
                    .ToListAsync();

                var favorites = await _context.FavoriteCourses
                    .Where(f => f.UserId == user.Id)
                    .Include(f => f.Course)
                        .ThenInclude(c => c!.Instructor)
                    .Select(f => f.Course!)
                    .ToListAsync();

                var vm = new StudentDashboardViewModel
                {
                    User = user,
                    Enrollments = enrollments,
                    FavoriteCourses = favorites,
                    CompletedCourses = enrollments.Count(e => e.CompletedAt != null),
                    InProgressCourses = enrollments.Count(e => e.CompletedAt == null)
                };
                return View("StudentDashboard", vm);
            }
        }
    }
}
