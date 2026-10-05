using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Disha.Web.Data;
using Disha.Web.Models.Entities;
using Disha.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Disha.Web.Controllers
{
    [Authorize]
    public class QuizController : Controller
    {
        private readonly DishaDbContext _context;

        public QuizController(DishaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var questions = await _context.QuizQuestions
                .Where(q => q.IsActive)
                .OrderBy(q => q.OrderIndex)
                .Include(q => q.Options)
                .ToListAsync();

            var vm = new QuizViewModel
            {
                Questions = questions,
                CurrentQuestionIndex = 0
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit([FromForm] Dictionary<int, int> answers)
        {
            int userId = GetCurrentUserId();
            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            if (answers == null || !answers.Any())
            {
                return RedirectToAction("Index");
            }

            // Clear previous responses for user
            var existingResponses = await _context.QuizResponses
                .Where(r => r.UserId == userId)
                .ToListAsync();
            
            if (existingResponses.Any())
            {
                _context.QuizResponses.RemoveRange(existingResponses);
            }

            foreach (var (questionId, optionId) in answers)
            {
                _context.QuizResponses.Add(new QuizResponse
                {
                    UserId = userId,
                    QuestionId = questionId,
                    OptionId = optionId,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();

            TempData["QuizJustCompleted"] = true;
            return RedirectToAction("Index", "Dashboard");
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claim, out int id) ? id : 0;
        }
    }
}
