using System.Security.Claims;
using System.Threading.Tasks;
using Disha.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Disha.Web.Controllers
{
    public class ToggleChecklistRequest
    {
        public int Id { get; set; }
    }

    [Authorize]
    public class ChecklistController : Controller
    {
        private readonly DishaDbContext _context;

        public ChecklistController(DishaDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleItem([FromBody] ToggleChecklistRequest request)
        {
            int userId = GetCurrentUserId();
            var item = await _context.ChecklistItems.FindAsync(request.Id);

            if (item == null || item.UserId != userId)
            {
                return NotFound(new { success = false });
            }

            item.IsDone = !item.IsDone;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isDone = item.IsDone });
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claim, out int id) ? id : 0;
        }
    }
}
