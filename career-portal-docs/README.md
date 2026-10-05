# Disha — IT Career Guidance Portal — Planning Docs

This folder contains everything needed to hand off to Google Antigravity (or build manually):

1. **PRD.md** — product requirements: problem, personas, features, user journey, UI/UX rules.
2. **TRD.md** — technical requirements: architecture decisions (auth, data access, AI mentor strategy), database schema, folder structure, local setup.
3. **MASTER_PROMPT.md** — paste this directly into Antigravity's Agent Manager to build the whole app in one go.

## How to use this folder
1. Open Google Antigravity.
2. Start a new Agent Manager task.
3. Copy the full contents of `MASTER_PROMPT.md` into the task prompt.
4. Keep `PRD.md` and `TRD.md` in your repo root once the agent scaffolds the project — they're referenced inside the master prompt.
5. Before running: install XAMPP and start MySQL, and get free API keys from console.groq.com and Google AI Studio (Gemini).

Stack: HTML/CSS/JS/Bootstrap · ASP.NET Core MVC · MySQL (XAMPP) · Groq + Gemini AI Mentor. Runs 100% locally except the AI Mentor's two API calls.
