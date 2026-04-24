using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCourseSystem.Data;
using OnlineCourseSystem.Models;
using OnlineCourseSystem.ViewModels;

namespace OnlineCourseSystem.Controllers
{
    public class CourseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CourseController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Course
        public async Task<IActionResult> Index(string? search, string? category, string? level)
        {
            var query = _context.Courses
                .Where(c => c.IsPublished)
                .Include(c => c.Instructor)
                .Include(c => c.Ratings)
                .Include(c => c.Enrollments)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(c => c.Title.Contains(search) || c.Description.Contains(search));

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(c => c.Category == category);

            if (!string.IsNullOrWhiteSpace(level))
                query = query.Where(c => c.Level == level);

            var courses = await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
            var categories = await _context.Courses.Where(c => c.IsPublished)
                .Select(c => c.Category).Distinct().ToListAsync();

            return View(new CourseListViewModel
            {
                Courses = courses,
                SearchTerm = search,
                Category = category,
                Level = level,
                Categories = categories
            });
        }

        // GET: /Course/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Lessons.OrderBy(l => l.Order))
                .Include(c => c.Enrollments)
                .Include(c => c.Comments)
                    .ThenInclude(cm => cm.User)
                .Include(c => c.Ratings)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            Enrollment? enrollment = null;
            Rating? userRating = null;
            bool isFavorite = false;

            if (userId != null)
            {
                enrollment = await _context.Enrollments
                    .Include(e => e.LessonProgresses)
                    .Include(e => e.Course)
                        .ThenInclude(c => c!.Lessons)
                    .FirstOrDefaultAsync(e => e.StudentId == userId && e.CourseId == id);

                userRating = await _context.Ratings
                    .FirstOrDefaultAsync(r => r.UserId == userId && r.CourseId == id);

                isFavorite = await _context.FavoriteCourses
                    .AnyAsync(f => f.UserId == userId && f.CourseId == id);
            }

            return View(new CourseDetailsViewModel
            {
                Course = course,
                IsEnrolled = enrollment != null,
                UserEnrollment = enrollment,
                UserRating = userRating,
                IsFavorite = isFavorite
            });
        }

        // GET: /Course/Create
        [Authorize(Roles = "Instructor,Admin")]
        public IActionResult Create() => View();

        // POST: /Course/Create
        [HttpPost]
        [Authorize(Roles = "Instructor,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            var course = new Course
            {
                Title = model.Title,
                Description = model.Description,
                ThumbnailUrl = model.ThumbnailUrl,
                Category = model.Category,
                Level = model.Level,
                Price = model.Price,
                IsPublished = model.IsPublished,
                InstructorId = user!.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Kursi u krijua me sukses!";
            return RedirectToAction("Details", new { id = course.Id });
        }

        // GET: /Course/Edit/5
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var course = await _context.Courses.FindAsync(id);

            if (course == null) return NotFound();
            if (course.InstructorId != user!.Id && !User.IsInRole("Admin")) return Forbid();

            return View(new CourseEditViewModel
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                ThumbnailUrl = course.ThumbnailUrl,
                Category = course.Category,
                Level = course.Level,
                Price = course.Price,
                IsPublished = course.IsPublished
            });
        }

        // POST: /Course/Edit/5
        [HttpPost]
        [Authorize(Roles = "Instructor,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CourseEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var course = await _context.Courses.FindAsync(model.Id);
            if (course == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (course.InstructorId != user!.Id && !User.IsInRole("Admin")) return Forbid();

            course.Title = model.Title;
            course.Description = model.Description;
            course.ThumbnailUrl = model.ThumbnailUrl;
            course.Category = model.Category;
            course.Level = model.Level;
            course.Price = model.Price;
            course.IsPublished = model.IsPublished;
            course.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Kursi u përditësua me sukses!";
            return RedirectToAction("Details", new { id = course.Id });
        }

        // POST: /Course/Delete/5
        [HttpPost]
        [Authorize(Roles = "Instructor,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (course.InstructorId != user!.Id && !User.IsInRole("Admin")) return Forbid();

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Kursi u fshi me sukses!";
            return RedirectToAction("Index");
        }

        // POST: /Course/AddComment
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int courseId, string content)
        {
            var user = await _userManager.GetUserAsync(User);
            _context.Comments.Add(new Comment
            {
                CourseId = courseId,
                UserId = user!.Id,
                Content = content,
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = courseId });
        }

        // POST: /Course/AddRating
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRating(int courseId, int stars, string? review)
        {
            var user = await _userManager.GetUserAsync(User);
            var existing = await _context.Ratings
                .FirstOrDefaultAsync(r => r.UserId == user!.Id && r.CourseId == courseId);

            if (existing != null)
            {
                existing.Stars = stars;
                existing.Review = review;
            }
            else
            {
                _context.Ratings.Add(new Rating
                {
                    CourseId = courseId,
                    UserId = user!.Id,
                    Stars = stars,
                    Review = review,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = courseId });
        }

        // POST: /Course/ToggleFavorite
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavorite(int courseId)
        {
            var user = await _userManager.GetUserAsync(User);
            var fav = await _context.FavoriteCourses
                .FirstOrDefaultAsync(f => f.UserId == user!.Id && f.CourseId == courseId);

            if (fav != null)
                _context.FavoriteCourses.Remove(fav);
            else
                _context.FavoriteCourses.Add(new FavoriteCourse
                {
                    UserId = user!.Id,
                    CourseId = courseId,
                    AddedAt = DateTime.UtcNow
                });

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = courseId });
        }
    }
}
