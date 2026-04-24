using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCourseSystem.Data;
using OnlineCourseSystem.Models;
using OnlineCourseSystem.ViewModels;

namespace OnlineCourseSystem.Controllers
{
    [Authorize]
    public class EnrollmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EnrollmentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int courseId)
        {
            var user = await _userManager.GetUserAsync(User);
            var existing = await _context.Enrollments
                .AnyAsync(e => e.StudentId == user!.Id && e.CourseId == courseId);

            if (!existing)
            {
                _context.Enrollments.Add(new Enrollment
                {
                    StudentId = user!.Id,
                    CourseId = courseId,
                    EnrolledAt = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
                TempData["Success"] = "U regjistruat me sukses në kurs!";
            }

            return RedirectToAction("Details", "Course", new { id = courseId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteLesson(int enrollmentId, int lessonId)
        {
            var user = await _userManager.GetUserAsync(User);
            var enrollment = await _context.Enrollments
                .Include(e => e.LessonProgresses)
                .Include(e => e.Course)
                    .ThenInclude(c => c!.Lessons)
                .FirstOrDefaultAsync(e => e.Id == enrollmentId && e.StudentId == user!.Id);

            if (enrollment == null) return NotFound();

            var progress = enrollment.LessonProgresses.FirstOrDefault(lp => lp.LessonId == lessonId);
            if (progress == null)
            {
                _context.LessonProgresses.Add(new LessonProgress
                {
                    EnrollmentId = enrollmentId,
                    LessonId = lessonId,
                    IsCompleted = true,
                    CompletedAt = DateTime.UtcNow
                });
            }
            else
            {
                progress.IsCompleted = !progress.IsCompleted;
                progress.CompletedAt = progress.IsCompleted ? DateTime.UtcNow : null;
            }

            // Check if all lessons completed
            await _context.SaveChangesAsync();

            var updatedEnrollment = await _context.Enrollments
                .Include(e => e.LessonProgresses)
                .Include(e => e.Course)
                    .ThenInclude(c => c!.Lessons)
                .FirstOrDefaultAsync(e => e.Id == enrollmentId);

            if (updatedEnrollment?.Course != null &&
                updatedEnrollment.LessonProgresses.Count(lp => lp.IsCompleted) == updatedEnrollment.Course.Lessons.Count &&
                updatedEnrollment.Course.Lessons.Any())
            {
                updatedEnrollment.CompletedAt = DateTime.UtcNow;
                updatedEnrollment.CertificateIssued = true;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Urime! Keni përfunduar kursin dhe certifikata juaj është gati!";
            }

            return RedirectToAction("Learn", new { id = enrollmentId, lessonId });
        }

        public async Task<IActionResult> Learn(int id, int? lessonId)
        {
            var user = await _userManager.GetUserAsync(User);
            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                    .ThenInclude(c => c!.Lessons.OrderBy(l => l.Order))
                .Include(e => e.LessonProgresses)
                .FirstOrDefaultAsync(e => e.Id == id && e.StudentId == user!.Id);

            if (enrollment == null) return NotFound();

            var currentLesson = lessonId.HasValue
                ? enrollment.Course!.Lessons.FirstOrDefault(l => l.Id == lessonId)
                : enrollment.Course!.Lessons.FirstOrDefault();

            ViewBag.CurrentLesson = currentLesson;
            return View(enrollment);
        }

        public async Task<IActionResult> Certificate(int enrollmentId)
        {
            var user = await _userManager.GetUserAsync(User);
            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                    .ThenInclude(c => c!.Instructor)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(e => e.Id == enrollmentId && e.StudentId == user!.Id);

            if (enrollment == null || !enrollment.CertificateIssued) return NotFound();
            return View(enrollment);
        }
    }

    [Authorize(Roles = "Instructor,Admin")]
    public class LessonController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LessonController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Create(int courseId)
        {
            ViewBag.CourseId = courseId;
            return View(new LessonCreateViewModel { CourseId = courseId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LessonCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            var course = await _context.Courses.FindAsync(model.CourseId);

            if (course == null || (course.InstructorId != user!.Id && !User.IsInRole("Admin")))
                return Forbid();

            _context.Lessons.Add(new Lesson
            {
                Title = model.Title,
                Description = model.Description,
                VideoUrl = model.VideoUrl,
                Order = model.Order,
                DurationMinutes = model.DurationMinutes,
                CourseId = model.CourseId,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            TempData["Success"] = "Leksioni u shtua me sukses!";
            return RedirectToAction("Details", "Course", new { id = model.CourseId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null) return NotFound();

            return View(new LessonEditViewModel
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Description = lesson.Description,
                VideoUrl = lesson.VideoUrl,
                Order = lesson.Order,
                DurationMinutes = lesson.DurationMinutes,
                CourseId = lesson.CourseId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LessonEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var lesson = await _context.Lessons.FindAsync(model.Id);
            if (lesson == null) return NotFound();

            lesson.Title = model.Title;
            lesson.Description = model.Description;
            lesson.VideoUrl = model.VideoUrl;
            lesson.Order = model.Order;
            lesson.DurationMinutes = model.DurationMinutes;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Leksioni u përditësua!";
            return RedirectToAction("Details", "Course", new { id = lesson.CourseId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null) return NotFound();

            var courseId = lesson.CourseId;
            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Leksioni u fshi!";
            return RedirectToAction("Details", "Course", new { id = courseId });
        }
    }
}
