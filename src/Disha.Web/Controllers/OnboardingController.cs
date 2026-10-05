using System.Security.Claims;
using System.Threading.Tasks;
using Disha.Web.Data;
using Disha.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Disha.Web.Controllers
{
    [Authorize]
    public class OnboardingController : Controller
    {
        private readonly DishaDbContext _context;

        public OnboardingController(DishaDbContext context)
        {
            _context = context;
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

            var model = new OnboardingViewModel
            {
                EducationLevel = user.EducationLevel ?? "Class 12 Pass",
                PriorComputerExposure = user.PriorComputerExposure ?? "none",
                State = user.State ?? "Maharashtra",
                LanguagePref = user.LanguagePref ?? "en"
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(OnboardingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int userId = GetCurrentUserId();
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.EducationLevel = model.EducationLevel;
                user.PriorComputerExposure = model.PriorComputerExposure;
                user.State = model.State;
                user.LanguagePref = model.LanguagePref;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Quiz");
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claim, out int id) ? id : 0;
        }
    }
}
