using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Disha.Web.Data;
using Disha.Web.Models.Entities;
using Disha.Web.Models.ViewModels;
using Disha.Web.Services;
using Disha.Web.Services.AiMentor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Disha.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly DishaDbContext _context;
        private readonly IRecommendationService _recommendationService;
        private readonly IAidMatchService _aidMatchService;
        private readonly IBookmarkService _bookmarkService;
        private readonly IAiMentorService _aiMentorService;

        public DashboardController(
            DishaDbContext context,
            IRecommendationService recommendationService,
            IAidMatchService aidMatchService,
            IBookmarkService bookmarkService,
            IAiMentorService aiMentorService)
        {
            _context = context;
            _recommendationService = recommendationService;
            _aidMatchService = aidMatchService;
            _bookmarkService = bookmarkService;
            _aiMentorService = aiMentorService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int userId = GetCurrentUserId();
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Check if user completed quiz
            bool quizCompleted = await _context.QuizResponses.AnyAsync(r => r.UserId == userId);

            // Calculate journey progress percentage
            int percentage = 25; // 25% for registering
            if (!string.IsNullOrWhiteSpace(user.State) && !string.IsNullOrWhiteSpace(user.EducationLevel))
            {
                percentage += 25; // Onboarding completed
            }
            if (quizCompleted)
            {
                percentage += 25; // Quiz completed
            }
            int savedCount = await _bookmarkService.GetBookmarkCountAsync(userId);
            if (savedCount > 0)
            {
                percentage += 25; // Saved resources / roadmap started
            }

            // Get recommendations
            var recommended = await _recommendationService.GetRecommendedCareersAsync(userId, 5);

            // Urgent aid
            var urgentAid = await _aidMatchService.GetUrgentAidAsync(user);
            var matchedAidList = await _aidMatchService.GetMatchingAidAsync(user);

            // Ensure checklist items exist for user
            var checklist = await _context.ChecklistItems
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.Id)
                .ToListAsync();

            if (!checklist.Any())
            {
                checklist = new List<ChecklistItem>
                {
                    new ChecklistItem { UserId = userId, ItemText = "Apply for laptop subsidy grant", IsDone = false },
                    new ChecklistItem { UserId = userId, ItemText = "Finish IT Support & HTML/CSS Basics module", IsDone = true },
                    new ChecklistItem { UserId = userId, ItemText = "Book 1-on-1 session with AI Mentor", IsDone = false }
                };
                _context.ChecklistItems.AddRange(checklist);
                await _context.SaveChangesAsync();
            }

            // Get recent mentor chat messages
            var recentMessages = await _aiMentorService.GetChatHistoryAsync(userId, 5);

            var vm = new DashboardViewModel
            {
                Student = user,
                JourneyCompletionPercentage = percentage,
                QuizCompleted = quizCompleted,
                MatchedCareersCount = recommended.Count,
                SavedResourcesCount = savedCount,
                UrgentAid = urgentAid,
                RecommendedCareers = recommended,
                ChecklistItems = checklist,
                RecentMentorMessages = recentMessages,
                MatchedAidList = matchedAidList
            };

            return View(vm);
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claim, out int id) ? id : 0;
        }
    }
}
