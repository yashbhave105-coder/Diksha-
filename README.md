# Disha — IT Career Guidance Portal for Underprivileged Students

**Disha** is a full-stack, 100% local web application designed to guide underprivileged students in India toward lucrative, debt-free IT careers. The platform features personalized IT career recommendation scoring, matching financial aid & laptop subsidies, an interactive assessment quiz, a horizontal milestone roadmap stepper, and a 24/7 AI Mentor with Groq primary and Gemini automatic fallback.

---

## 🌟 Tech Stack

- **Framework**: ASP.NET Core 8 MVC (.NET 8 SDK)
- **Frontend**: Razor Views, Bootstrap 5, Bootstrap Icons, Custom CSS Theme (`site.css`), Vanilla JS
- **Database**: MySQL 8 (via local XAMPP), accessed via Entity Framework Core + `Pomelo.EntityFrameworkCore.MySql`
- **Authentication**: Custom local cookie authentication (`PasswordHasher<User>`, `HttpContext.SignInAsync`, sliding expiration)
- **AI Mentor**: Groq API (primary with Llama 3) + Gemini API (secondary fallback) with automatic failover and 10s timeouts

---

## 🛠️ Prerequisites

1. **.NET 8 SDK** installed (`dotnet --version`)
2. **XAMPP** with MySQL/MariaDB service running on `localhost:3306` (or `127.0.0.1:3306`)
3. **Groq / Gemini API Keys** (Optional for AI Mentor live responses; static fallback is built-in if keys are unconfigured)

---

## 🚀 Setup & Execution Instructions

### Step 1: Start XAMPP MySQL Service
Ensure MySQL is running on `127.0.0.1:3306` via XAMPP Control Panel or command line:
```powershell
wmic process call create "C:\xampp\mysql\bin\mysqld.exe --defaults-file=C:\xampp\mysql\bin\my.ini --standalone"
```

### Step 2: Initialize Database Schema & Seed Data (Fallback SQL included)
You can run the provided `schema.sql` and `seed.sql` scripts directly:
```powershell
& 'C:\xampp\mysql\bin\mysql.exe' -u root -e "SOURCE schema.sql; SOURCE seed.sql;"
```
*Note: EF Core's `DbSeeder` will also automatically initialize and seed the database on app startup if empty.*

### Step 3: Configure User Secrets (Optional for AI Mentor)
Set your Groq and Gemini API keys via .NET User Secrets:
```powershell
cd src/Disha.Web
dotnet user-secrets set "AiMentor:GroqApiKey" "YOUR_GROQ_API_KEY"
dotnet user-secrets set "AiMentor:GeminiApiKey" "YOUR_GEMINI_API_KEY"
```

### Step 4: Run the Application
Navigate to the web project directory and launch:
```powershell
cd src/Disha.Web
& 'C:\Users\HP\.dotnet\dotnet.exe' run
```
Open your web browser at: `http://localhost:5000` or `https://localhost:5001`.

---

## 📂 Project Structure

```
projectfolder/
├── PRD.md                        # Product Requirements Document
├── TRD.md                        # Technical Requirements Document
├── README.md                     # Setup and local execution instructions
├── schema.sql                    # Standalone raw MySQL table creation DDL
├── seed.sql                      # Seed data script (12 IT tracks, 10 aid schemes, 12 quiz Qs)
└── src/
    └── Disha.Web/
        ├── Controllers/          # Account, Onboarding, Dashboard, Quiz, Careers, FinancialAid, Mentor, etc.
        ├── Data/                 # DishaDbContext.cs and DbSeeder.cs
        ├── Models/
        │   ├── Entities/         # User, Career, CareerTag, QuizQuestion, QuizOption, FinancialAid, etc.
        │   └── ViewModels/       # RegisterViewModel, LoginViewModel, DashboardViewModel, QuizViewModel, etc.
        ├── Services/
        │   ├── RecommendationService.cs   # Rule-based career scoring engine
        │   ├── AidMatchService.cs         # Financial aid & laptop grant filter
        │   ├── BookmarkService.cs         # Saved resource manager
        │   └── AiMentor/                  # Groq & Gemini AI Providers with fallback logic
        ├── Views/                # Razor view templates matching reference dashboard design
        ├── wwwroot/              # CSS design system (site.css), JS widgets (mentor-chat.js, checklist.js)
        ├── appsettings.json      # Configuration settings
        └── Program.cs            # ASP.NET Core middleware, EF Core MySQL setup & Cookie Auth
```

---

## 🔍 Verification Checklist

- [x] **Authentication**: Custom registration & login using `PasswordHasher<User>` and Cookie Auth.
- [x] **Database**: 10 MySQL tables initialized with 12 IT career tracks and 10 financial aid programs.
- [x] **Recommendation Engine**: Tag-weighted scoring mapping quiz choices to IT career matches.
- [x] **AI Mentor**: Groq Llama 3 primary request with 10s timeout and automatic fallback to Gemini 1.5 Flash.
- [x] **UI/UX Aesthetics**: Deep navy hero header (`#0B1330`), progress ring card, stat cards, recommended IT track cards, horizontal milestone stepper, urgency badges, and AJAX mentor preview card.
