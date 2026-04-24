using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineCourseSystem.Data;
using OnlineCourseSystem.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
});

var app = builder.Build();

// Seed roles and admin
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    db.Database.Migrate();

    string[] roles = { "Admin", "Instructor", "Student" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // Seed admin
    if (await userManager.FindByEmailAsync("admin@kurs.al") == null)
    {
        var admin = new ApplicationUser
        {
            UserName = "admin@kurs.al",
            Email = "admin@kurs.al",
            FullName = "Administrator",
            Role = UserRole.Admin,
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(admin, "Admin123!");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(admin, "Admin");
    }

    // Seed demo instructor
    if (await userManager.FindByEmailAsync("instruktor@kurs.al") == null)
    {
        var instructor = new ApplicationUser
        {
            UserName = "instruktor@kurs.al",
            Email = "instruktor@kurs.al",
            FullName = "Arben Kelmendi",
            Bio = "Ekspert i teknologjisë informative me 10 vjet eksperiencë.",
            Role = UserRole.Instructor,
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(instructor, "Instruktor123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(instructor, "Instructor");

            // Seed demo courses
            if (!db.Courses.Any())
            {
                var courses = new[]
                {
                    new Course
                    {
                        Title = "C# dhe .NET 8 - Nga Fillimi",
                        Description = "Mëso programimin me C# dhe .NET 8 nga zero. Kursi mbulon bazat e OOP, LINQ, Entity Framework dhe ASP.NET Core.",
                        Category = "Programim",
                        Level = "Fillestare",
                        Price = 0,
                        IsPublished = true,
                        InstructorId = instructor.Id,
                        ThumbnailUrl = "https://images.unsplash.com/photo-1555066931-4365d14bab8c?w=400",
                        CreatedAt = DateTime.UtcNow.AddDays(-30)
                    },
                    new Course
                    {
                        Title = "Web Design Modern me CSS dhe JavaScript",
                        Description = "Krijo faqe web moderne dhe responsive duke përdorur CSS Grid, Flexbox, animacione dhe JavaScript ES6+.",
                        Category = "Design",
                        Level = "Mesatare",
                        Price = 29.99m,
                        IsPublished = true,
                        InstructorId = instructor.Id,
                        ThumbnailUrl = "https://images.unsplash.com/photo-1547658719-da2b51169166?w=400",
                        CreatedAt = DateTime.UtcNow.AddDays(-20)
                    },
                    new Course
                    {
                        Title = "Bazat e Inteligjencës Artificiale",
                        Description = "Hyrje në Machine Learning, Neural Networks dhe AI me Python dhe TensorFlow.",
                        Category = "AI & ML",
                        Level = "Avancuar",
                        Price = 49.99m,
                        IsPublished = true,
                        InstructorId = instructor.Id,
                        ThumbnailUrl = "https://images.unsplash.com/photo-1677442135703-1787eea5ce01?w=400",
                        CreatedAt = DateTime.UtcNow.AddDays(-10)
                    }
                };

                db.Courses.AddRange(courses);
                await db.SaveChangesAsync();

                // Add lessons to first course
                var firstCourse = courses[0];
                db.Lessons.AddRange(
                    new Lesson { Title = "Hyrje në C#", Description = "Çfarë është C# dhe pse ta mësoni", VideoUrl = "https://www.youtube.com/embed/GhQdlIFylQ8", Order = 1, DurationMinutes = 15, CourseId = firstCourse.Id },
                    new Lesson { Title = "Tipat e të dhënave", Description = "int, string, bool dhe tipat bazë", VideoUrl = "https://www.youtube.com/embed/GhQdlIFylQ8", Order = 2, DurationMinutes = 20, CourseId = firstCourse.Id },
                    new Lesson { Title = "Kushtet dhe Sythe", Description = "if/else, for, while loops", VideoUrl = "https://www.youtube.com/embed/GhQdlIFylQ8", Order = 3, DurationMinutes = 25, CourseId = firstCourse.Id }
                );
                await db.SaveChangesAsync();
            }
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
