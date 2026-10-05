# Master Prompt for Google Antigravity
Paste everything below the line into a new Antigravity Agent Manager task. Keep `PRD.md` and `TRD.md` in the repo root for reference — this prompt is self-contained but they have more product/architecture rationale if the agent (or you) need it mid-build.

---

## PROJECT BRIEF: "Disha" — IT Career Guidance Portal for Underprivileged Students

Build a full-stack local web app called **Disha** — a career guidance portal focused **exclusively on IT careers** (not commerce, not healthcare, not general careers) for underprivileged students in India. The visual design must closely follow the reference dashboard screenshot supplied by the user: a dark navy hero header with a greeting and progress ring, stat cards, "Recommended Debt-Free Career Tracks" cards with salary/demand badges, a horizontal milestone stepper, and sidebar cards for financial aid and a mentor chat.

**Runs 100% locally.** Do not deploy anywhere. Database is **MySQL via XAMPP** (not any cloud DB). Authentication is **fully custom, local, no Google/third-party login**. The only external network calls this app makes are to the Groq and Gemini APIs for the AI Mentor chatbot.

### Tech stack (fixed)
- **Frontend:** Razor views (server-rendered), Bootstrap 5, vanilla JS (only for the mentor chat widget), Bootstrap Icons.
- **Backend:** ASP.NET Core MVC, .NET 8.
- **Database:** MySQL 8 running locally via XAMPP, accessed through EF Core + `Pomelo.EntityFrameworkCore.MySql`.
- **Auth:** Custom `users` table, `Microsoft.AspNetCore.Identity.PasswordHasher<TUser>` for hashing, ASP.NET Core Cookie Authentication for sessions. No external identity provider of any kind.
- **AI Mentor:** Groq API (primary, OpenAI-compatible chat completions endpoint) with automatic fallback to Gemini API (Generative Language API) on failure.

---

### 1. Authentication — build exactly this way

1. `users` table: `id, username, email, password_hash, full_name, role ('student'|'admin'), education_level, prior_computer_exposure, state, language_pref, created_at`.
2. `AccountController.Register(RegisterViewModel)`: validate uniqueness of username/email, hash the password with `new PasswordHasher<User>().HashPassword(user, plainPassword)`, insert the row, sign the user in immediately (don't require a separate login step after registering), redirect to `/Onboarding`.
3. `AccountController.Login(LoginViewModel)`: look up by username or email, verify with `PasswordHasher<User>().VerifyHashedPassword(user, storedHash, submittedPassword)`, on success build a `ClaimsPrincipal` (claims: user id, username, role) and call `HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal)`. On failure, show a friendly "username or password doesn't match" message (never reveal which field was wrong).
4. Configure cookie auth in `Program.cs`: `HttpOnly = true`, `SecurePolicy = CookieSecurePolicy.Always` (relax to `SameAsRequest` only if needed for local http testing), `SameSite = SameSiteMode.Lax`, sliding expiration.
5. `[Authorize]` on every controller except `HomeController`, `AccountController`'s Register/Login actions.
6. `AccountController.Logout()`: `HttpContext.SignOutAsync(...)`, redirect home.
7. `[ValidateAntiForgeryToken]` on every POST action across the whole app.

### 2. Database schema (MySQL — create via EF Core migrations AND provide the raw SQL as a fallback script)

```sql
CREATE TABLE users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    full_name VARCHAR(150),
    role ENUM('student','admin') NOT NULL DEFAULT 'student',
    education_level VARCHAR(50),
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
    roadmap JSON NOT NULL,
    avg_salary_range VARCHAR(100),
    duration_to_start VARCHAR(100),
    cost_estimate VARCHAR(100),
    demand_badge VARCHAR(50),
    icon VARCHAR(50),
    is_active BOOLEAN DEFAULT TRUE
);

CREATE TABLE career_tags (
    id INT AUTO_INCREMENT PRIMARY KEY,
    career_id INT NOT NULL,
    tag VARCHAR(30) NOT NULL,
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
    tag_weights JSON NOT NULL,
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
    applicable_states JSON,
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
    provider_used VARCHAR(20),
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

### 3. Recommendation logic (rule-based, no ML)

Implement `IRecommendationService`:
- Sum `tag_weights` (JSON) of every quiz option a student chose into a per-tag score dictionary (tags: `frontend, backend, data, security, support, design, cloud, qa`).
- Each career has 1–3 rows in `career_tags`. Score a career = sum of the student's score for each of its tags.
- Return top 3–5 careers sorted by score descending.
- Build a one-line "why this fits you" string from the student's top 1–2 scoring tags in plain language (e.g., "You enjoy visual design and problem-solving").

Implement `IAidMatchService`: filter `financial_aid` where `applicable_states` is null/empty or contains the student's state, same for `applicable_classes`; always include aid with no restrictions regardless of profile.

### 4. AI Mentor — build exactly this way

- `Services/AiMentor/IAiMentorService.cs` with `Task<string> SendMessageAsync(int userId, string studentMessage)`.
- `GroqMentorProvider`: `HttpClient` POST to `https://api.groq.com/openai/v1/chat/completions` with header `Authorization: Bearer {GroqApiKey}`, body includes the system prompt (below) as the `system` message, the last ~10 messages from `mentor_chat_messages` for context, and the new user message. Use a fast Groq-hosted Llama model.
- `GeminiMentorProvider`: `HttpClient` POST to the Gemini `generateContent` endpoint with the API key, same system prompt + conversation history translated into Gemini's `contents` format.
- `AiMentorService.SendMessageAsync`: try Groq first (10s timeout). On any exception or non-success response, log it and try Gemini (10s timeout). If both fail, return a static friendly fallback string — **never** show a raw exception, stack trace, or HTTP error to the student. Persist both the student's message and the AI's reply (with `provider_used`) to `mentor_chat_messages`.
- **System prompt** (store in `appsettings.json` under `AiMentor:SystemPrompt`, not hardcoded):
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
- `MentorController`: a full-page chat view (`/Mentor`) AND a compact chat preview card usable on the Dashboard. The full page shows the entire message history (scrollable, newest at bottom, user messages right-aligned, AI messages left-aligned with a small "AI Mentor" label — never implies a human). Sending a message is a small AJAX POST (`wwwroot/js/mentor-chat.js`) that appends the reply without a full page reload.
- API keys (`AiMentor:GroqApiKey`, `AiMentor:GeminiApiKey`) come from `dotnet user-secrets` — never hardcoded, never committed.

### 5. Pages / Controllers to build (all fully working, not stubs)

1. **Home** (`/`) — public landing page, IT-focused headline, Sign Up / Log In buttons.
2. **Account/Register**, **Account/Login**, **Account/Logout** — per Section 1.
3. **Onboarding** (`/Onboarding`, 3 short steps: education level & prior computer exposure / state & language / done) — persists to `users`.
4. **Dashboard** (`/Dashboard`) — **build to closely match the reference screenshot**:
   - Dark navy hero card: "Welcome back, {Name}!" + short encouraging line + a progress ring/percentage showing overall journey completion (assessment done, career matched, roadmap started, etc. — weight these steps to produce a % below the hero).
   - Row of 3–4 stat cards below the hero (e.g., quiz status, number of career matches, saved resources/certificates count, an urgent financial-aid deadline if one exists).
   - "Recommended Debt-Free IT Career Tracks" section — cards per matched career: icon, title, demand badge, salary prominent in the mint accent color, 3 quick-fact chips (time to start / cost / demand), tag pills, dark "View Roadmap" button.
   - Sidebar: Financial Aid urgency card (only shown if a real upcoming deadline exists in the data), Weekly Action Checklist (simple checkbox list from `checklist_items`), AI Mentor preview card (last message + "Continue chat" link to `/Mentor`).
   - Bottom full-width mint banner — reusable component, e.g. "Free laptops & data assistance — see what's available near you" linking to Financial Aid.
5. **Quiz** (`/Quiz`) — one question per screen, progress bar, large tap-target answer buttons, submits all answers at the end, redirects to Dashboard with results freshly computed.
6. **Careers/Index** — all active IT careers, filterable by tag (quick-match tiles: Frontend, Backend, Data, Security, Support, Design, Cloud, QA).
7. **Careers/Details/{id}** — full description, roadmap rendered as the **horizontal milestone stepper** (checked steps, one highlighted "current" step, greyed/locked future steps — matching the reference exactly), salary/cost/duration facts, matched Financial Aid panel, Bookmark button.
8. **FinancialAid/Index** — filterable list (state/class), eligibility in plain language, "Verified as of [date]" tag, Apply/Claim button, Bookmark button.
9. **Mentor/Index** — full AI Mentor chat page per Section 4.
10. **Dashboard bookmarks section** — saved careers + aid items, pulled via `IBookmarkService`.

### 6. Design system — match the reference screenshot

- Palette: deep navy hero/header (`#0B1330`-ish), mint/teal accent (`#1DB88E`-ish) for primary buttons, badges, and progress fills, soft off-white page background (`#F5F7FB`), white cards with 12–16px rounded corners and soft shadows.
- Build this as a `wwwroot/css/site.css` layer on top of Bootstrap 5 — override CSS variables (`--bs-primary`, etc.) rather than fighting Bootstrap's defaults with `!important` everywhere.
- Career/aid cards, stat cards, and the milestone stepper should visually match the screenshot's card style, badge pills, and iconography as closely as feasible with Bootstrap Icons + simple inline SVGs.
- Every interactive element has a minimum 44px tap target; base font-size ≥16px; WCAG AA contrast (don't put mint text on white — use it on navy or as a solid button fill).
- Fully responsive; confirm correct rendering at 360px width in addition to desktop, matching the reference's card-stacking behavior on smaller screens.

### 7. Seed data (required before demo — IT tracks only)

- **≥12 IT career tracks only** — e.g., IT Support Technician, QA/Software Tester, Frontend Developer, Backend Developer, Full-Stack Developer, Data Analyst, Cybersecurity Analyst (entry-level), Cloud Support Associate, UI/UX Designer, Network Administrator, Mobile App Developer, Database Administrator. Do **not** include non-IT careers (no nursing, no commerce, no general business roles) — this app is IT-only per the user's requirement. Each with a realistic multi-stage `roadmap` JSON, salary range, cost estimate, and duration.
- **≥10 financial aid / assistance entries** — mix of free IT certification programs, device/laptop assistance schemes, connectivity subsidies, and general scholarships that also cover IT diploma/certificate costs. Realistic eligibility text and (placeholder, clearly-commented) apply URLs.
- **12–15 quiz questions**, 3–4 options each, `tag_weights` using the fixed tag vocabulary: `frontend, backend, data, security, support, design, cloud, qa`.

### 8. Non-functional requirements

- `[Authorize]` everywhere except Home/Register/Login; `[ValidateAntiForgeryToken]` on every POST.
- All EF Core queries parameterized; no raw SQL string concatenation anywhere.
- Config via `appsettings.json` + `dotnet user-secrets` (MySQL connection string, Groq key, Gemini key) — never hardcoded, never committed populated.
- Layered structure: `Controllers/`, `Models/Entities/`, `Models/ViewModels/`, `Services/` (interface + implementation pairs, including `Services/AiMentor/`), `Data/`, `Views/`, `wwwroot/`. Controllers stay thin; business/scoring/AI logic lives in services.

### 9. What to do when you're done building

1. Confirm XAMPP's MySQL service is running, then run the app locally and, using your browser subagent, walk the full flow yourself: Home → Register → Onboarding → Dashboard (empty state) → Quiz → Dashboard (populated, progress ring updated) → Career Detail (milestone stepper) → Financial Aid → bookmark something → back to Dashboard confirms it saved → open AI Mentor, send a real message, confirm a real reply comes back (test that the Groq→Gemini fallback path doesn't crash if you temporarily use an invalid Groq key).
2. Fix anything that errors or looks visually broken at both desktop and 360px mobile width, and compare side-by-side against the reference screenshot for the Dashboard specifically.
3. Write a `README.md` at the repo root: prerequisites (.NET 8 SDK, XAMPP, free Groq + Gemini API keys), full local setup steps (start XAMPP MySQL → create DB → run schema/migrations → seed data → set user-secrets → `dotnet run`), and a short project structure overview.
4. Leave `PRD.md` and `TRD.md` in the repo root, unmodified, as living reference docs.

Build the complete project now — all controllers, views, services, entities, seed data, and the README — not a partial skeleton.
