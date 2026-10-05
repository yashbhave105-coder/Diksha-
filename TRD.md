# Technical Requirements Document (TRD)
## Disha — Online Career Guidance Portal for Underprivileged Students (IT Track)

**Version:** 2.0
**Stack:** ASP.NET Core MVC (.NET 8) · Bootstrap 5 · MySQL (via XAMPP) · Groq + Gemini (AI Mentor)
**Run mode:** 100% local. XAMPP's MySQL service + `dotnet run`. No cloud database, no external identity provider.

---

## 1. Decision — Authentication Approach (no Google, no external IdP)

Three approaches considered:

### Approach 1 — Full ASP.NET Core Identity + Pomelo MySQL provider
Battle-tested, handles hashing/lockout/roles out of the box. **Drawback:** brings a lot of generated scaffolding (its own migration history, `AspNetUsers`/`AspNetRoles`/`AspNetUserClaims`... tables, `UserManager`/`RoleManager`/`SignInManager` wiring) that's heavier than this project needs, and fighting its default Razor Class Library UI to match a custom navy/mint design adds friction.

### Approach 2 — Fully custom Users table + hand-rolled password hashing
Full control, minimal tables. **Drawback:** rolling your own crypto is a real security risk if done carelessly (e.g., unsalted SHA-256).

### Approach 3 — Custom Users table + ASP.NET Core's built-in `PasswordHasher<TUser>` + Cookie Authentication middleware — **SELECTED**
Uses the **exact same secure hashing implementation** ASP.NET Core Identity uses internally (`Microsoft.AspNetCore.Identity.PasswordHasher<TUser>` — PBKDF2 with a random salt per password, configurable iteration count), but without pulling in the rest of the Identity framework. Combined with `Microsoft.AspNetCore.Authentication.Cookies` for session management, this is:
- **Secure** (no hand-rolled crypto — you're using Microsoft's own hasher class directly),
- **Simple to trace end-to-end** (one `Users` table, one `AccountController`, no hidden framework magic),
- **Easy to extend** with a `Role` column (`student` / `admin`) checked via a simple `[Authorize(Roles = "Admin")]` claim, without needing `RoleManager`.

**This is the selected approach.** Flow: Register → hash password with `PasswordHasher<User>().HashPassword()` → store in `users.password_hash` → Login → `VerifyHashedPassword()` → on success, build `ClaimsPrincipal` → `HttpContext.SignInAsync()` issues an `HttpOnly`, `Secure`, `SameSite=Lax` auth cookie → every subsequent page is a normal `[Authorize]`-protected MVC action.

---

## 2. Decision — Data Access Approach

### Approach 1 — Entity Framework Core + `Pomelo.EntityFrameworkCore.MySql` — **SELECTED**
Code-first entities, LINQ queries, automatic parameterization (SQL-injection-safe by default), `dotnet ef migrations` generates the schema directly against your local XAMPP MySQL instance. Best fit for a learner-maintainable, clean-architecture project — matches the pattern already used in this project's earlier Supabase/Postgres version, so the mental model transfers.

### Approach 2 — Dapper (micro-ORM) + hand-written SQL
Faster/lighter, more control over exact queries — but means hand-writing and maintaining SQL for every operation, more error-prone for a project of this size, and loses LINQ-based composability for the recommendation-scoring queries.

**Selected: EF Core + Pomelo.** Dapper is a reasonable fast-follow if performance ever becomes a real constraint (it won't, at this scale).

---

## 3. Decision — AI Mentor Provider Strategy (Groq + Gemini together)

You asked for both a Groq and a Gemini API key to be used — here's how to make that meaningful rather than arbitrary:

### Approach 1 — Let the student pick a provider
Adds a settings toggle "Fast mode (Groq) / Detailed mode (Gemini)." More UI surface for very little real benefit to a student who just wants an answer.

### Approach 2 — Primary + automatic fallback — **SELECTED**
- **Groq is the primary provider** (Llama-family models via Groq's OpenAI-compatible endpoint) — very low latency, which matters for a chat UI a student is actively waiting on.
- If the Groq call fails (network error, rate limit, non-2xx response), the service **automatically retries the same message against Gemini** before giving up — the student never sees which provider answered, just gets a response.
- If both fail, show a friendly local fallback message ("I'm having trouble connecting right now — try again in a moment, or check the Financial Aid page for device/connectivity help.") — never a raw error or stack trace.

**Architecture:**
```
IAiMentorService
 ├─ GroqMentorProvider   (calls https://api.groq.com/openai/v1/chat/completions)
 └─ GeminiMentorProvider (calls Generative Language API generateContent endpoint)

AiMentorService : IAiMentorService
 - SendMessageAsync(userId, message):
     1. Load last N messages from mentor_chat_messages for context
     2. Try GroqMentorProvider.GetReplyAsync(history, message)
     3. On failure -> try GeminiMentorProvider.GetReplyAsync(history, message)
     4. On both failure -> return static fallback string
     5. Persist both the user message and the AI reply to mentor_chat_messages
```

### System prompt (used for both providers identically)
```
You are the AI Mentor inside "Disha," a career guidance app for underprivileged
students exploring IT careers in India. Your student may be a first-generation
learner with limited resources. Rules:
- Use simple, warm, encouraging, jargon-free language. Explain any technical
  term the first time you use it.
- Only discuss IT career paths, learning resources, realistic timelines, study
  tips, and encouragement. Do not give financial, legal, or medical advice.
- Always be honest that you are an AI assistant, not a human.
- If the student's message suggests personal distress unrelated to careers,
  gently encourage them to talk to a trusted adult, teacher, or counsellor
  rather than trying to handle it yourself.
- Prefer short, concrete answers with a clear next step over long essays.
```
This system prompt is a **hard requirement**, not optional flavor text — implement it as configuration (`appsettings.json` → `AiMentor:SystemPrompt`), not hardcoded inline, so it can be tuned without a rebuild.

---

## 4. High-Level Architecture Diagram

```
┌──────────────────────────────┐
│  Browser (Bootstrap UI)      │
│  Server-rendered Razor views │
│  Plain HTML forms — no SPA   │
│  framework, no external SDKs │
└──────────────┬────────────────┘
               │ HTTPS (localhost)
               ▼
┌──────────────────────────────┐
│  ASP.NET Core MVC (.NET 8)   │
│  - AccountController         │       ┌────────────────────────┐
│    (custom cookie auth)      │       │  Groq API (chat)       │
│  - Quiz / Careers / Aid      │──────▶│  (primary, low-latency)│
│  - MentorController  ────────┼──────▶└────────────────────────┘
│  - Services/ (business logic)│                  │ on failure
└──────────────┬────────────────┘                  ▼
               │ EF Core (Pomelo)          ┌────────────────────────┐
               ▼                           │  Gemini API (chat)     │
┌──────────────────────────────┐           │  (fallback)            │
│  MySQL (via XAMPP, local)    │           └────────────────────────┘
│  users, careers, career_tags,│
│  quiz_*, financial_aid,      │
│  bookmarks, mentor_chat_*    │
└──────────────────────────────┘
```

**Everything except the two AI Mentor API calls is 100% local** — the app, the database, and auth all run on your machine with no internet dependency.

---

## 5. Tech Stack Detail

| Layer | Technology |
|---|---|
| Backend framework | ASP.NET Core MVC, .NET 8 (LTS) |
| ORM | Entity Framework Core + `Pomelo.EntityFrameworkCore.MySql` |
| Database | MySQL 8, run locally via **XAMPP** |
| Auth | Custom `Users` table + `Microsoft.AspNetCore.Identity.PasswordHasher<TUser>` + Cookie Authentication |
| Frontend | Razor Views, Bootstrap 5, vanilla JS (chat widget only), Bootstrap Icons |
| AI Mentor | Groq API (primary) + Gemini API (fallback), plain `HttpClient` calls (no SDK needed) |
| Config/secrets | `dotnet user-secrets` for local dev (API keys, DB connection string) |
| Session | ASP.NET Core cookie authentication (`HttpOnly`, `Secure`, `SameSite=Lax`) |

---

## 6. Database Schema (MySQL / XAMPP)

```sql
CREATE TABLE users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    full_name VARCHAR(150),
    role ENUM('student','admin') NOT NULL DEFAULT 'student',
    education_level VARCHAR(50),          -- e.g. 'Class 10', 'Class 12', 'Diploma'
    prior_computer_exposure ENUM('none','some','a_lot') DEFAULT 'none',
    state VARCHAR(100),
    language_pref VARCHAR(10) DEFAULT 'en',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE careers (
    id INT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(150) NOT NULL,
    summary VARCHAR(300) NOT NULL,
    description TEXT NOT NULL,
    roadmap JSON NOT NULL,                -- [{ "stage": "Learn HTML/CSS", "detail": "...", "duration": "4 weeks" }, ...]
    avg_salary_range VARCHAR(100),
    duration_to_start VARCHAR(100),       -- e.g. "3-6 months self-paced"
    cost_estimate VARCHAR(100),           -- e.g. "Free - Rs. 5,000"
    demand_badge VARCHAR(50),             -- 'High Demand' | 'Beginner Friendly' | 'Fast-Track'
    icon VARCHAR(50),
    is_active BOOLEAN DEFAULT TRUE
);

CREATE TABLE career_tags (
    id INT AUTO_INCREMENT PRIMARY KEY,
    career_id INT NOT NULL,
    tag VARCHAR(30) NOT NULL,             -- 'frontend','backend','data','security','support','design','cloud','qa'
    FOREIGN KEY (career_id) REFERENCES careers(id) ON DELETE CASCADE
);

CREATE TABLE quiz_questions (
    id INT AUTO_INCREMENT PRIMARY KEY,
    question_text VARCHAR(300) NOT NULL,
    order_index INT NOT NULL,
    is_active BOOLEAN DEFAULT TRUE
);

CREATE TABLE quiz_options (
    id INT AUTO_INCREMENT PRIMARY KEY,
    question_id INT NOT NULL,
    option_text VARCHAR(200) NOT NULL,
    tag_weights JSON NOT NULL,            -- { "frontend": 2, "design": 1 }
    FOREIGN KEY (question_id) REFERENCES quiz_questions(id) ON DELETE CASCADE
);

CREATE TABLE quiz_responses (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    question_id INT NOT NULL,
    option_id INT NOT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (question_id) REFERENCES quiz_questions(id),
    FOREIGN KEY (option_id) REFERENCES quiz_options(id)
);

CREATE TABLE financial_aid (
    id INT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(200) NOT NULL,
    provider VARCHAR(150) NOT NULL,
    description TEXT NOT NULL,
    eligibility TEXT NOT NULL,
    amount VARCHAR(100),
    apply_url VARCHAR(300),
    applicable_states JSON,               -- ["Maharashtra","Karnataka"] or NULL = all-India
    applicable_classes JSON,
    last_verified_date DATE,
    is_active BOOLEAN DEFAULT TRUE
);

CREATE TABLE bookmarks (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    item_type ENUM('career','aid') NOT NULL,
    item_id INT NOT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    UNIQUE KEY uq_bookmark (user_id, item_type, item_id),
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

CREATE TABLE mentor_chat_messages (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    sender ENUM('user','ai') NOT NULL,
    message TEXT NOT NULL,
    provider_used VARCHAR(20),            -- 'groq' | 'gemini' | NULL for user messages
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

CREATE TABLE checklist_items (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    item_text VARCHAR(200) NOT NULL,
    is_done BOOLEAN DEFAULT FALSE,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);
```

**Recommendation scoring (rule-based, unchanged philosophy — no ML needed):**
1. Each quiz option carries `tag_weights` (JSON) against IT sub-domain tags.
2. Sum a student's chosen option weights into a per-tag score vector.
3. Each career has 1–3 rows in `career_tags`. Score a career = sum of the student's score for each of its tags.
4. Return top 3–5 careers by score; use `demand_badge`/`is_active` as tie-breakers.

---

## 7. Project Folder Structure

```
CareerGuidancePortal/
├── CareerGuidancePortal.sln
├── src/
│   └── CareerGuidancePortal.Web/
│       ├── Controllers/
│       │   ├── HomeController.cs
│       │   ├── AccountController.cs        (Register, Login, Logout)
│       │   ├── OnboardingController.cs
│       │   ├── QuizController.cs
│       │   ├── DashboardController.cs
│       │   ├── CareersController.cs
│       │   ├── FinancialAidController.cs
│       │   └── MentorController.cs         (chat page + AJAX message endpoint)
│       │
│       ├── Models/
│       │   ├── Entities/                   (EF Core entities matching schema)
│       │   ├── ViewModels/
│       │   └── DTOs/
│       │
│       ├── Data/
│       │   ├── ApplicationDbContext.cs
│       │   └── Seed/SeedData.cs            (seed careers, aid, quiz)
│       │
│       ├── Services/
│       │   ├── IPasswordAuthService.cs / PasswordAuthService.cs
│       │   ├── IRecommendationService.cs / RecommendationService.cs
│       │   ├── IAidMatchService.cs / AidMatchService.cs
│       │   ├── IBookmarkService.cs / BookmarkService.cs
│       │   └── AiMentor/
│       │       ├── IAiMentorService.cs / AiMentorService.cs
│       │       ├── GroqMentorProvider.cs
│       │       └── GeminiMentorProvider.cs
│       │
│       ├── Views/
│       │   ├── Shared/_Layout.cshtml, _Navbar.cshtml
│       │   ├── Home/, Account/, Onboarding/, Quiz/, Dashboard/,
│       │   │   Careers/, FinancialAid/, Mentor/
│       │
│       ├── wwwroot/
│       │   ├── css/site.css                 (navy/mint theme, matches reference design)
│       │   ├── js/mentor-chat.js             (AJAX chat widget)
│       │   └── img/
│       │
│       ├── appsettings.json
│       ├── appsettings.Development.json      (gitignored)
│       └── Program.cs
│
└── README.md
```

---

## 8. Configuration & Secrets (local dev)

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:MySql" "Server=localhost;Port=3306;Database=career_guidance_portal;Uid=root;Pwd=;"
dotnet user-secrets set "AiMentor:GroqApiKey" "<your-groq-api-key>"
dotnet user-secrets set "AiMentor:GeminiApiKey" "<your-gemini-api-key>"
```
- Default XAMPP MySQL has an empty root password — adjust `Pwd=` if you've set one.
- `AiMentor:SystemPrompt` can live in `appsettings.json` (not secret) since it contains no keys — see Section 3.
- Never commit a populated `appsettings.Development.json` or hardcode API keys in `Program.cs` or any controller.

---

## 9. Non-Functional Requirements

- **Security:** `PasswordHasher<TUser>` (PBKDF2, salted) for all passwords; `[ValidateAntiForgeryToken]` on every POST; parameterized EF Core queries everywhere (no raw string-concatenated SQL); `HttpOnly`/`Secure`/`SameSite=Lax` auth cookie; `[Authorize]` on all pages except Home/Register/Login; `[Authorize(Roles="Admin")]` reserved for future admin tooling.
- **AI Mentor resilience:** Groq→Gemini fallback (Section 3); never surface a raw exception or API error text to the student; timeout each provider call (e.g., 10s) so the chat UI never hangs indefinitely.
- **Performance:** Bootstrap via CDN or local static copy, minimal custom JS, server-rendered pages for fast first paint on low-end devices.
- **Accessibility:** semantic HTML, `alt` text on icons/badges, min 16px body font, WCAG AA contrast (the navy/mint palette from the reference design comfortably meets this if mint text isn't used on white — use it on navy or as button fills instead).
- **Responsiveness:** mobile-first Bootstrap grid, tested down to 360px width.
- **Local-only run:** XAMPP's MySQL service must be running (via the XAMPP Control Panel) before `dotnet run`; README documents this explicitly as step 1.

---

## 10. Local Setup Checklist (for README)

1. Install XAMPP, start the **MySQL** service from the XAMPP Control Panel (Apache isn't required — the .NET app has its own server).
2. Open phpMyAdmin (`http://localhost/phpmyadmin`) or run the CLI, create database `career_guidance_portal`.
3. Run the schema SQL (Section 6) against that database, or apply EF Core migrations (`dotnet ef database update`) pointed at the local connection string.
4. Run the seed script/method to populate careers, financial aid, and quiz questions.
5. Get a **free Groq API key** (console.groq.com) and a **free Gemini API key** (Google AI Studio), set them via `dotnet user-secrets` (Section 8).
6. `dotnet restore && dotnet run`.
7. Open `https://localhost:5001`, register a student account, complete onboarding, take the quiz, chat with the AI Mentor.
