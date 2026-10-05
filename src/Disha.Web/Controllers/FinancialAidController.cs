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
    public class FinancialAidController : Controller
    {
        private readonly DishaDbContext _context;
        private readonly IAidMatchService _aidMatchService;
        private readonly IBookmarkService _bookmarkService;

        public FinancialAidController(DishaDbContext context, IAidMatchService aidMatchService, IBookmarkService bookmarkService)
        {
            _context = context;
            _aidMatchService = aidMatchService;
            _bookmarkService = bookmarkService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? stateFilter = null, string? classFilter = null)
        {
            int userId = GetCurrentUserId();
            var user = await _context.Users.FindAsync(userId);

            var allAid = await _context.FinancialAids
                .Where(a => a.IsActive)
                .ToListAsync();

            var bookmarkedIds = await _context.Bookmarks
                .Where(b => b.UserId == userId && b.ItemType == "aid")
                .Select(b => b.ItemId)
                .ToListAsync();

            ViewData["BookmarkedIds"] = bookmarkedIds;
            ViewData["User"] = user;

            return View(allAid);
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claim, out int id) ? id : 0;
        }
    }
}
