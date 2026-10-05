using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Disha.Web.Data;
using Disha.Web.Models.Entities;
using Disha.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Disha.Web.Controllers
{
    [Authorize]
    public class CareersController : Controller
    {
        private readonly DishaDbContext _context;
        private readonly IBookmarkService _bookmarkService;
        private readonly IAidMatchService _aidMatchService;

        public CareersController(DishaDbContext context, IBookmarkService bookmarkService, IAidMatchService aidMatchService)
        {
            _context = context;
            _bookmarkService = bookmarkService;
            _aidMatchService = aidMatchService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? tag = null)
        {
            var query = _context.Careers
                .Where(c => c.IsActive)
                .Include(c => c.Tags)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tag))
            {
                string tagLower = tag.Trim().ToLower();
                query = query.Where(c => c.Tags.Any(t => t.Tag.ToLower() == tagLower));
            }

            var careers = await query.ToListAsync();
            ViewData["SelectedTag"] = tag;

            return View(careers);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            int userId = GetCurrentUserId();

            var career = await _context.Careers
                .Include(c => c.Tags)
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (career == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(userId);
            var matchingAid = user != null ? await _aidMatchService.GetMatchingAidAsync(user) : new List<FinancialAid>();
            bool isBookmarked = await _bookmarkService.IsBookmarkedAsync(userId, "career", id);

            ViewData["IsBookmarked"] = isBookmarked;
            ViewData["MatchingAid"] = matchingAid;

            return View(career);
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claim, out int id) ? id : 0;
        }
    }
}
