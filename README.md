# 🎓 KursAL — Sistem për Menaxhimin e Kurseve Online

Aplikacion web i plotë i ndërtuar me **ASP.NET Core 8 MVC**, **C#**, **SQL Server** dhe **Razor Views**.

---

## 📁 Struktura e Projektit

```
OnlineCourseSystem/
├── Controllers/
│   ├── AccountController.cs       # Login, Register, Profile
│   ├── CourseController.cs        # CRUD kurset, komente, vlerësime, favorite
│   ├── EnrollmentController.cs    # Regjistrim, mësim, progres, certifikata
│   ├── HomeController.cs          # Kryefaqja, Dashboard (student/instruktor)
│   └── LessonController.cs        # CRUD leksionet
├── Models/
│   ├── ApplicationUser.cs         # Modeli i përdoruesit (Identity)
│   └── Course.cs                  # Course, Lesson, Enrollment, LessonProgress,
│                                  # Comment, Rating, FavoriteCourse
├── ViewModels/
│   └── ViewModels.cs              # Të gjitha ViewModel-et
├── Data/
│   ├── ApplicationDbContext.cs    # EF Core DbContext me FK relationships
│   └── DatabaseSetup.sql          # Udhëzime për databazën
├── Views/
│   ├── Shared/
│   │   ├── _Layout.cshtml         # Layout kryesor me navbar dhe footer
│   │   └── _CourseCard.cshtml     # Partial view për kartat e kurseve
│   ├── Home/
│   │   ├── Index.cshtml           # Kryefaqja me hero, kategori, kurse
│   │   ├── StudentDashboard.cshtml    # Dashboard për studentët
│   │   └── InstructorDashboard.cshtml # Dashboard për instruktorët
│   ├── Account/
│   │   ├── Login.cshtml           # Faqja e hyrjes
│   │   ├── Register.cshtml        # Regjistrimi me zgjedhje roli
│   │   └── Profile.cshtml         # Profili i përdoruesit
│   ├── Course/
│   │   ├── Index.cshtml           # Lista e kurseve me filtrim
│   │   ├── Details.cshtml         # Detajet e kursit, leksionet, komente
│   │   ├── Create.cshtml          # Formulari i krijimit
│   │   └── Edit.cshtml            # Formulari i modifikimit
│   ├── Lesson/
│   │   ├── Create.cshtml          # Shto leksion
│   │   └── Edit.cshtml            # Modifiko leksion
│   └── Enrollment/
│       ├── Learn.cshtml           # Faqja e mësimit me player dhe progres
│       └── Certificate.cshtml     # Certifikata e printuar
├── wwwroot/
│   ├── css/site.css              # Design system i plotë (dark theme)
│   └── js/site.js                # JavaScript për interaktivitet
├── Program.cs                    # Konfigurimi i aplikacionit + seed data
├── appsettings.json              # Connection string
└── OnlineCourseSystem.csproj     # Paketet NuGet
```

---

## 🚀 Si të Ekzekutoni

### Kërkesat
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server ose SQL Server LocalDB (vjen me Visual Studio)
- Visual Studio 2022 ose VS Code

### Hapat

**1. Klononi projektin**
```bash
git clone <repo-url>
cd OnlineCourseSystem
```

**2. Konfiguroni Connection String** në `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=OnlineCourseSystemDb;Trusted_Connection=True;"
  }
}
```

Nëse keni SQL Server të instaluar:
```
Server=localhost;Database=OnlineCourseSystemDb;User Id=sa;Password=YourPass;
```

**3. Ekzekutoni migracionet** (opsionale — aplikacioni gjithashtu krijon DB automatikisht):
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**4. Startoni aplikacionin**
```bash
dotnet run
```

Hapni `https://localhost:5001` ose `http://localhost:5000`.

---

## 👤 Llogaritë Demo

| Roli | Email | Fjalëkalimi |
|------|-------|-------------|
| Admin | admin@kurs.al | Admin123! |
| Instruktor | instruktor@kurs.al | Instruktor123! |

---

## ✅ Funksionalitetet

### Autentikimi & Autorizimi
- ✅ Regjistrim me zgjedhje roli (Student / Instruktor)
- ✅ Login / Logout me ASP.NET Core Identity
- ✅ Menaxhimi i roleve me `[Authorize(Roles = "...")]`
- ✅ Cookie-based authentication

### Menaxhimi i Kurseve
- ✅ CRUD i plotë (Create, Read, Update, Delete)
- ✅ Lista me filtrim sipas: kërkimit, kategorisë, nivelit
- ✅ Faqe detajesh me të gjitha informacionet
- ✅ Toggle publikim (draft / i publikuar)

### Regjistrimi në Kurse
- ✅ Studentët regjistrohen me një klik
- ✅ Dashboard "Kurset e mia" me progres
- ✅ Ndjekja e progresit leksion pas leksioni (% completion)

### Leksionet
- ✅ Instruktorët shtojnë / modifikojnë / fshijnë leksione
- ✅ Embed video YouTube ose link i jashtëm
- ✅ Renditje dhe kohëzgjatje
- ✅ Shënim si "e përfunduar" për studentët

### Komente & Vlerësime
- ✅ Komente nga studentët e regjistruar
- ✅ Sistem vlerësimi me yje (1–5)
- ✅ Llogaritje e mesatares dhe shfaqja e rishikimeve

### Certifikata
- ✅ Lëshohet automatikisht pas 100% progresit
- ✅ Faqe e printuar me dizajn profesional

### Bonus
- ✅ Kërkim kursesh
- ✅ Filtrim sipas kategorisë dhe nivelit
- ✅ Kurse të preferuara (favorites)
- ✅ Certifikata e printuar

---

## 🗄️ Struktura e Databazës

| Tabela | Përshkrimi |
|--------|-----------|
| `AspNetUsers` | Përdoruesit (Identity + fushat custom) |
| `AspNetRoles` | Rolet (Admin, Instructor, Student) |
| `Courses` | Kurset me FK tek Instructor |
| `Lessons` | Leksionet me FK tek Course |
| `Enrollments` | Regjistrimet Student ↔ Course (unique) |
| `LessonProgresses` | Progresi i leksioneve per enrollment |
| `Comments` | Komentet User ↔ Course |
| `Ratings` | Vlerësimet User ↔ Course (unique) |
| `FavoriteCourses` | Të preferuarat User ↔ Course (unique) |

---

## 🎨 Dizajni

- **Tema**: Dark mode me ngjyra teal/emerald
- **Font**: Syne (headings) + Inter (body)
- **Responsive**: Grid adaptive për mobile/tablet/desktop
- **Animacione**: CSS transitions dhe floating cards

---

## 📦 Paketat NuGet

```xml
Microsoft.AspNetCore.Identity.EntityFrameworkCore  8.0.0
Microsoft.EntityFrameworkCore.SqlServer            8.0.0
Microsoft.EntityFrameworkCore.Tools                8.0.0
Microsoft.AspNetCore.Identity.UI                   8.0.0
```
