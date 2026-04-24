-- =============================================
-- KursAL - Online Course Management System
-- Database Setup Script (alternative to EF migrations)
-- Run this ONLY if not using EF Core migrations
-- =============================================

-- For EF Core migrations, use these commands in Package Manager Console:
-- Add-Migration InitialCreate
-- Update-Database

-- Or via CLI:
-- dotnet ef migrations add InitialCreate
-- dotnet ef database update

-- The application uses DbContext.Database.EnsureCreated() which auto-creates tables.
-- Connection string in appsettings.json:
-- "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=OnlineCourseSystemDb;Trusted_Connection=True;"
