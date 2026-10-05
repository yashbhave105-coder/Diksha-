# Product Requirements Document (PRD)
## Disha — Online Career Guidance Portal for Underprivileged Students (IT Track)

**Version:** 2.0 (revised — IT-only scope, local MySQL, AI mentor)
**Owner:** Rudra
**Status:** Draft for Build

---

## 1. Problem Statement

Underprivileged students (Class 9–12 / early college, first-generation learners, low-income households) who show interest or aptitude in computers have **no structured, free guidance** into IT careers specifically. Generic career counselling — if available at all — treats "IT" as one vague box ("do coding"), when in reality there are a dozen realistic, different-skill, different-cost entry paths into IT (support technician, QA tester, frontend developer, data analyst, cybersecurity, etc.), many of which don't require a 4-year degree.

**This version of Disha narrows scope on purpose:** rather than a broad "any career" portal, it is a **focused IT career on-ramp** — one domain, done well, with a working AI mentor a student can actually talk to. This mirrors the reference dashboard you shared (progress ring, debt-free career tracks, milestone roadmap, mentor card) but every recommended path is an IT path.

---

## 2. Goals & Non-Goals

### Goals (MVP)
1. Onboard a student and immediately show a **personalized IT-track dashboard** (styled like the reference: dark hero, progress %, stat cards).
2. A short **IT-interest quiz** (not a generic aptitude test) that places the student into 1–2 IT sub-domains (frontend, backend, data, security, support/hardware, design, cloud, QA).
3. Recommend **3–5 realistic, debt-free/low-cost IT career tracks** per student, each with salary range, time-to-start, and a **step-by-step milestone roadmap**.
4. Surface **IT-relevant financial aid** — scholarships, free certification programs (e.g., free digital-skills courses), device/connectivity assistance — since "no laptop" or "no internet" is often the actual blocker, not lack of interest.
5. A **built-in AI Mentor chatbot** (Groq + Gemini backed) a student can ask anything to, in plain language, available 24/7 — replacing the need for a live human mentor in v1.
6. **Simple local authentication** — username/password, no external identity provider, no Google account required (many target students don't have one, or share a family Google account they don't want to mix with this).
7. **Fully local**: MySQL via XAMPP, ASP.NET Core MVC app on localhost, AI mentor calls out to Groq/Gemini APIs (the only outbound network calls) — everything else runs offline-capable on a laptop.

### Non-Goals (v1)
- No non-IT career tracks (commerce, healthcare, arts, etc.) — explicitly excluded per scope.
- No human mentor Q&A board in v1 (AI mentor replaces it; the schema still allows adding human mentors later).
- No payments, no live hosting/deployment.
- No mobile app — responsive web only.
- No admin CMS UI — content seeded via SQL/seed scripts.

---

## 3. Target Users & Personas

### Persona 1 — "Priya", the Student (primary)
- Class 9–12, interested in "computers" but doesn't know what that actually means as a job.
- Shared Android phone or a low-end school/cyber-café PC, patchy internet.
- Needs plain language: not "full-stack" or "DevOps" without a one-line explanation the first time it appears.

### Persona 2 — "Admin" (you, via seed scripts / MySQL Workbench / phpMyAdmin from XAMPP)
- Manages the master list of IT careers, scholarships, and quiz questions directly in the database for v1.

*(Note: the "mentor" persona from earlier scope is now the AI Mentor — see Section 6 — so no separate human-mentor persona in this version.)*

---

## 4. Core Features (MVP Scope)

| # | Feature | Priority |
|---|---------|----------|
| 1 | Local sign-up / login (username or email + password) | P0 |
| 2 | Onboarding — class/education level, prior exposure to computers (none/some/a lot), state (for financial-aid matching), language preference | P0 |
| 3 | IT-Interest Quiz (10–15 questions, plain language, IT-sub-domain scoring) | P0 |
| 4 | Dashboard (reference-style): progress ring, stat cards, quick-match category tiles | P0 |
| 5 | Recommended IT Career Tracks (3–5 cards: salary, time-to-start, cost, demand badge, "View Roadmap") | P0 |
| 6 | Career Detail + Milestone Roadmap (visual stepper: Assessment → Matched → Learning → Job-ready) | P0 |
| 7 | Financial Aid / Device & Connectivity Assistance panel (filtered by student profile) | P0 |
| 8 | **AI Mentor Chatbot** (Groq primary / Gemini fallback), persistent per-student chat history | P0 |
| 9 | Bookmarks (careers + aid programs saved to dashboard) | P1 |
| 10 | Weekly Action Checklist (small nudges: "Complete your assessment," "Explore 1 new career") | P1 |
| 11 | Resource Library (free IT courses/certifications, filtered by matched sub-domain) | P1 |

---

## 5. User Journey

1. **Landing page** → warm headline, "this is for students exploring IT/computer careers," Sign Up / Log In.
2. **Sign Up** — username, email, password (with clear strength hint, not a scary red error wall), confirm.
3. **Onboarding** (3 short screens) — class/level → comfort with computers so far → state + language.
4. **Dashboard, first visit** — empty/progress state: "Welcome, Priya! Let's find your IT path — take the 5-minute assessment." Progress ring at 0%.
5. **IT-Interest Quiz** — one question per screen, big tap targets, progress bar, plain language ("Do you enjoy making things look good on screen?" rather than "Do you have design aptitude?").
6. **Results → Dashboard updates** — progress ring jumps (e.g., "Assessment complete — 25%"), **Recommended Debt-Free IT Career Tracks** section populates with 3–5 matched cards (e.g., Frontend Developer, IT Support Technician, Data Analyst), each with salary, time-to-start, and a **View Roadmap** button.
7. **Career Detail page** — full plain-language explanation, visual milestone stepper (Complete Assessment ✓ → Matched with Path ✓ → Start Roadmap (current) → Land a Role (locked)), a **Financial Aid available for you** panel, Bookmark button.
8. **Financial Aid page** — list of scholarships/free-course/device-assistance programs, filtered by state/profile, each with a clear "Claim / Apply" button and eligibility in plain language.
9. **AI Mentor** — a persistent chat entry point (sidebar card on dashboard + full-page chat view). Student types a question ("Can I become a developer without going to college?") and gets an encouraging, plain-language, IT-career-specific answer in seconds. Chat history is saved so it's there next visit.
10. **Return visit** — dashboard shows updated progress %, saved bookmarks, and picks the mentor chat up where it left off.

---

## 6. AI Mentor — Product Behaviour (not just tech)

This replaces the human "Ask a Mentor" board from the general-scope version. Requirements at the product level:
- Tone: warm, encouraging, plain-language, **never condescending**, answers scoped to IT career guidance for underprivileged students (education paths, free resources, realistic timelines, encouragement) — not general-purpose chit-chat, and never financial/legal/medical advice.
- Every AI response should nudge toward action where relevant ("You could start with [free resource] this week") rather than just being informational.
- Must clearly indicate it's an AI assistant (not a real person) — this is both an honesty requirement and reduces the chance a student treats it as a substitute for real crisis support.
- If a student's message signals something outside scope (e.g., personal distress unrelated to careers), the mentor should gently redirect to a trusted adult/appropriate resource rather than attempt to handle it — this behaviour is a hard requirement in the system prompt (see TRD/Master Prompt), not optional polish.

---

## 7. UI/UX Principles (from your reference design + student-first constraints)

Carrying forward the visual language of the screenshot you shared:
1. **Dark navy hero header** with a warm greeting ("Welcome back, [Name]!"), short encouraging subtext, and a **progress ring/percentage** ("68% through your Software track" style — here, "X% through your IT journey").
2. **Stat cards row** directly under the hero: e.g., "Assessment status," "Career matches found," "Financial aid deadline," "Certificates/resources saved."
3. **Quick-match category tiles** (icons) for IT sub-domains — Frontend, Data, Security, Support/Hardware — as a fast way to browse beyond the quiz.
4. **Career track cards**: icon, title, a "High Demand" / "Beginner Friendly" style badge, salary figure prominent in teal/green, 3 quick-fact chips (time to start, cost, demand), tag pills (Remote-Friendly, Fast-Track, No-Degree-Needed), dark "View Roadmap" button.
5. **Milestone stepper** — horizontal step indicator with checkmarks for completed steps, a highlighted current step, and a locked/greyed future step — exactly like the reference's 4-step plan.
6. **Sidebar cards**: Financial Aid urgency card (red/urgent accent used sparingly, only for real deadlines), Weekly Action Checklist, AI Mentor chat preview card.
7. **Bottom awareness banner** — a full-width, mint-green callout (as in the reference's "Reliable High-Speed Internet is a Human Right" banner) — reused here for something like "Free laptops & data — see what's available near you."
8. Keep the general student-first rules from v1: plain language everywhere, one primary action per screen, 44px+ tap targets, encouraging never-judgmental tone, fast on slow networks, high contrast, mobile-first down to 360px.

### Visual Direction
- Palette: deep navy/indigo (`#0B1330`-ish) hero and header, mint/teal accent (`#1DB88E`-ish) for primary actions and positive badges, soft off-white background (`#F5F7FB`), white cards with soft shadow and 12–16px rounded corners — matching the reference screenshot closely.
- Bootstrap 5 as the component base, with a custom `site.css` theme layer overriding Bootstrap's default blue with this navy/mint palette.

---

## 8. Success Metrics

- % of students who complete the IT-interest quiz after signup (target >70%).
- % of students who send at least 1 message to the AI Mentor.
- % of students who bookmark at least 1 career or aid program.
- Average time from signup to first career match (< 6 minutes).
- Qualitative: can a student use the whole flow with zero explanation from you?

---

## 9. Content Requirements (seed before demo)

- **≥12 IT career tracks** — e.g., IT Support Technician, QA/Software Tester, Frontend Developer, Backend Developer, Full-Stack Developer, Data Analyst, Cybersecurity Analyst (entry), Cloud Support Associate, UI/UX Designer, Network Administrator, Mobile App Developer, Database Administrator — each with a realistic multi-stage roadmap, salary range, and cost/time-to-start.
- **≥10 financial aid / assistance programs** relevant to IT students specifically — free certification programs (e.g., Google/NASSCOM-style digital skill programs), device/laptop assistance schemes, connectivity subsidies, general education scholarships that also cover IT diploma/certificate costs.
- **12–15 quiz questions** mapped to IT sub-domain tags (frontend, backend, data, security, support, design, cloud, qa).

---

## 10. Assumptions & Constraints

- Runs **fully locally** — XAMPP's MySQL service + the ASP.NET Core app on localhost. The **only** external network calls are to the Groq/Gemini APIs for the AI Mentor (everything else works even without internet, once XAMPP is running).
- No third-party identity provider — auth is entirely self-hosted against the local MySQL database.
- Single language (English) for MVP; UI text centralized so Hindi can be added later.
- Scope is intentionally IT-only; the schema is generic enough (`category` field on careers) that other departments could be added later without a redesign, but v1 seed data and UI copy are 100% IT-focused.
