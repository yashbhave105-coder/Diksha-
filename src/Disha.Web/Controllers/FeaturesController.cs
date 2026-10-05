using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Disha.Web.Data;
using Disha.Web.Services.AiMentor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Disha.Web.Controllers
{
    public class MockInterviewAnswerRequest
    {
        public int CareerId { get; set; }
        public string RoleTitle { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
    }

    public class CommunityStory
    {
        public string Name { get; set; } = string.Empty;
        public string Hometown { get; set; } = string.Empty;
        public string Background { get; set; } = string.Empty;
        public string CurrentRole { get; set; } = string.Empty;
        public string CompanyType { get; set; } = string.Empty;
        public string StartingPackage { get; set; } = string.Empty;
        public string Quote { get; set; } = string.Empty;
        public string Strategy { get; set; } = string.Empty;
        public List<string> FreeResourcesUsed { get; set; } = new();
    }

    public class TechHubLocation
    {
        public string City { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // e.g. "Public Digital Library", "Free NGO Computer Lab"
        public string Address { get; set; } = string.Empty;
        public string Facilities { get; set; } = string.Empty;
        public string Timings { get; set; } = string.Empty;
        public string Cost { get; set; } = "100% Free";
    }

    public class FeaturesController : Controller
    {
        private readonly DishaDbContext _context;
        private readonly IAiMentorService _aiMentorService;

        public FeaturesController(DishaDbContext context, IAiMentorService aiMentorService)
        {
            _context = context;
            _aiMentorService = aiMentorService;
        }

        // 1. Day In The Life Simulator
        [HttpGet]
        public async Task<IActionResult> DayInLife(int? careerId)
        {
            var careers = await _context.Careers.Where(c => c.IsActive).ToListAsync();
            var selectedCareer = careerId.HasValue 
                ? careers.FirstOrDefault(c => c.Id == careerId.Value) 
                : careers.FirstOrDefault();

            ViewBag.Careers = careers;
            return View(selectedCareer);
        }

        // 2. Interactive RPG Skill Tree
        [HttpGet]
        public async Task<IActionResult> SkillTree(int? careerId)
        {
            var careers = await _context.Careers.Where(c => c.IsActive).ToListAsync();
            var selectedCareer = careerId.HasValue
                ? careers.FirstOrDefault(c => c.Id == careerId.Value)
                : careers.FirstOrDefault();

            ViewBag.Careers = careers;
            return View(selectedCareer);
        }

        // 5. First-Gen Journey Wall
        [HttpGet]
        public IActionResult Journeys()
        {
            var stories = new List<CommunityStory>
            {
                new CommunityStory
                {
                    Name = "Rahul Verma",
                    Hometown = "Bhopal, MP",
                    Background = "First in family to attend college (Government Polytechnic Diploma in Electrical). No home Wi-Fi, shared a single Android phone with father.",
                    CurrentRole = "Junior Frontend Developer",
                    CompanyType = "Tech Services Firm (Pune)",
                    StartingPackage = "₹4.8 LPA",
                    Quote = "I thought coding was only for elite IIT students with MacBooks. When I built my first HTML button on a cyber café computer, everything changed.",
                    Strategy = "Studied FreeCodeCamp on mobile during 45-minute bus commute. Practiced typing on cyber cafe desktop on weekends.",
                    FreeResourcesUsed = new List<string> { "FreeCodeCamp Responsive Web Design", "Namaste JavaScript by Akshay Saini (YouTube)", "GitHub Pages (Free Hosting)" }
                },
                new CommunityStory
                {
                    Name = "Pooja Shankar",
                    Hometown = "Solapur, Maharashtra",
                    Background = "B.Com graduate from rural college. Worked in local data entry for ₹7,000/month to support family before learning QA Automation.",
                    CurrentRole = "QA Automation Engineer",
                    CompanyType = "FinTech Startup (Bangalore - Remote)",
                    StartingPackage = "₹5.5 LPA",
                    Quote = "You don't need a Computer Science degree to get into software. Manual testing and basic Python logic gave me my career breakthrough.",
                    Strategy = "Spent 2 hours every evening after work taking free Postman and Selenium tutorials.",
                    FreeResourcesUsed = new List<string> { "Ministry of Electronics Free IT Skills Portal", "Software Testing Help free guides", "Postman Student Expert Program" }
                },
                new CommunityStory
                {
                    Name = "Arjun Patel",
                    Hometown = "Vadodara, Gujarat",
                    Background = "Son of a small grocery shop owner. Learned Linux and Cloud troubleshooting using AWS and GCP Free Tier.",
                    CurrentRole = "Cloud Operations Associate",
                    CompanyType = "Global IT Consultancy",
                    StartingPackage = "₹6.0 LPA",
                    Quote = "Cloud providers offer generous 1-year free tiers. You can learn enterprise infrastructure without spending a single rupee.",
                    Strategy = "Created disposable free-tier cloud servers to practice terminal commands and setup web servers.",
                    FreeResourcesUsed = new List<string> { "AWS Skill Builder Free Tier", "Linux Journey (linuxjourney.com)", "freeCodeCamp DevOps & Docker Handbook" }
                }
            };

            return View(stories);
        }

        // 7. AI Mock Interview Practice
        [HttpGet]
        public async Task<IActionResult> MockInterview(int? careerId)
        {
            var careers = await _context.Careers.Where(c => c.IsActive).ToListAsync();
            var selectedCareer = careerId.HasValue
                ? careers.FirstOrDefault(c => c.Id == careerId.Value)
                : careers.FirstOrDefault();

            ViewBag.Careers = careers;
            return View(selectedCareer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitInterviewAnswer([FromBody] MockInterviewAnswerRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Answer))
            {
                return BadRequest(new { success = false, message = "Please provide an answer." });
            }

            int userId = GetCurrentUserId();
            string prompt = $"The student is practicing for a beginner-level job interview for the role of '{req.RoleTitle}'.\n" +
                            $"Question asked: \"{req.Question}\"\n" +
                            $"Student's answer: \"{req.Answer}\"\n\n" +
                            $"Please evaluate the answer in a warm, encouraging manner:\n" +
                            $"1. Highlight what was good in their answer.\n" +
                            $"2. Give 1 or 2 concrete suggestions to make it stronger, more structured, or clearer.\n" +
                            $"3. Provide an example of how an ideal fresher could answer it concisely.\n" +
                            $"Keep the total response under 180 words, positive and actionable.";

            string feedback = await _aiMentorService.SendMessageAsync(userId > 0 ? userId : 1, prompt);

            return Json(new { success = true, feedback = feedback });
        }

        // 8. 1-Click Fresher Portfolio & Resume Builder
        [HttpGet]
        public async Task<IActionResult> ResumeBuilder()
        {
            int userId = GetCurrentUserId();
            var user = userId > 0 ? await _context.Users.FindAsync(userId) : null;
            var careers = await _context.Careers.Where(c => c.IsActive).ToListAsync();
            ViewBag.User = user;
            ViewBag.Careers = careers;
            return View();
        }

        // 9. Daily Code Bite Micro-Quiz
        [HttpGet]
        public IActionResult DailyCodeBite()
        {
            return View();
        }

        // 10. Local Free IT Hubs & Computer Centers Directory
        [HttpGet]
        public IActionResult TechHubs(string? city = null)
        {
            var hubs = new List<TechHubLocation>
            {
                new TechHubLocation { City = "Mumbai", Name = "National Digital Library Resource Center", Type = "Free Digital Lab", Address = "Near Fort Campus, CST", Facilities = "Free Wi-Fi, 30 Desktop PCs, Coding Tutorials", Timings = "8:00 AM - 8:00 PM" },
                new TechHubLocation { City = "Mumbai", Name = "Yuva Parivartan Community Tech Center", Type = "NGO Skill Lab", Address = "Bandra East, Mumbai", Facilities = "Free Basic Computer & Typing Coaching", Timings = "9:30 AM - 5:30 PM" },
                new TechHubLocation { City = "Delhi", Name = "Delhi Public Library Digital Wing", Type = "Public Digital Library", Address = "Opposite Old Delhi Railway Station, Delhi", Facilities = "High-speed broadband, quiet study desk, Linux terminals", Timings = "9:00 AM - 7:00 PM" },
                new TechHubLocation { City = "Delhi", Name = "NIIT Foundation Digital Center", Type = "CSR Lab", Address = "Sangam Vihar, New Delhi", Facilities = "Free IT Literacy, Web Dev basics, mentor assistance", Timings = "10:00 AM - 5:00 PM" },
                new TechHubLocation { City = "Bangalore", Name = "State Central Library Cyber Section", Type = "Public Library", Address = "Cubbon Park, Bengaluru", Facilities = "Free internet browsing terminal, access to digital journals", Timings = "8:30 AM - 7:30 PM" },
                new TechHubLocation { City = "Bangalore", Name = "Dream a Dream Skill Development Center", Type = "Youth Tech Center", Address = "Lakkasandra, Wilson Garden, Bengaluru", Facilities = "Free coding bootcamps, resume review, laptop lending library", Timings = "9:00 AM - 6:00 PM" },
                new TechHubLocation { City = "Hyderabad", Name = "City Central Library Digital Learning Center", Type = "Public Library", Address = "Chikkadpally, Hyderabad", Facilities = "Free desktops, high-speed Wi-Fi, IT exam reference materials", Timings = "8:00 AM - 8:00 PM" },
                new TechHubLocation { City = "Pune", Name = "Pune University Free Public Reading Room & Web Lab", Type = "Academic Lab", Address = "Ganeshkhind, Pune", Facilities = "Computer terminals, open-source programming tools", Timings = "24/7 Access" },
                new TechHubLocation { City = "Bhopal", Name = "Swami Vivekananda Central Library Digital Unit", Type = "Public Digital Lab", Address = "GTB Complex, TT Nagar, Bhopal", Facilities = "Free computer terminals, competitive IT prep books", Timings = "9:00 AM - 7:00 PM" }
            };

            if (!string.IsNullOrWhiteSpace(city))
            {
                hubs = hubs.Where(h => h.City.Equals(city, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ViewBag.SelectedCity = city;
            return View(hubs);
        }

        // 11. AI Project Idea Generator & Budget Cost Calculator
        [HttpGet]
        public async Task<IActionResult> ProjectGenerator()
        {
            var careers = await _context.Careers.Where(c => c.IsActive).ToListAsync();
            ViewBag.Careers = careers;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateProjectIdea([FromBody] ProjectIdeaRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Track))
            {
                return BadRequest(new { success = false, message = "Please select a career track." });
            }

            int userId = GetCurrentUserId();
            string prompt = $"The student is learning the IT track: '{req.Track}'.\n" +
                            $"Their local interests or community context: '{req.Interest ?? "Local community problem"}'\n" +
                            $"Their device limitation: '{req.DeviceConstraint ?? "Low-spec laptop or mobile with 1.5GB daily data"}'\n\n" +
                            $"Provide 1 unique, highly realistic, beginner-friendly portfolio project idea that solves a real Indian community problem.\n" +
                            $"Respond strictly in this structured format:\n" +
                            $"**Project Title:** <Name>\n" +
                            $"**The Problem:** <1-2 sentences on what real problem it solves>\n" +
                            $"**Recommended Tech Stack:** <100% free open-source tools>\n" +
                            $"**Step-by-Step Implementation:** <3 bullet points>\n" +
                            $"**Hosting & Cost:** Explain how to deploy it for ₹0 using free tiers (GitHub Pages/Render/Vercel).\n" +
                            $"Keep the total response under 180 words, crisp, and motivating.";

            string idea = await _aiMentorService.SendMessageAsync(userId > 0 ? userId : 1, prompt);

            return Json(new { success = true, result = idea });
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claim, out int id) ? id : 0;
        }
    }

    public class ProjectIdeaRequest
    {
        public string Track { get; set; } = string.Empty;
        public string? Interest { get; set; }
        public string? DeviceConstraint { get; set; }
    }
}
