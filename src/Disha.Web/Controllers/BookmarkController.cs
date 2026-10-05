using System.Security.Claims;
using System.Threading.Tasks;
using Disha.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Disha.Web.Controllers
{
    public class ToggleBookmarkRequest
    {
        public string ItemType { get; set; } = "career";
        public int ItemId { get; set; }
    }

    [Authorize]
    public class BookmarkController : Controller
    {
        private readonly IBookmarkService _bookmarkService;

        public BookmarkController(IBookmarkService bookmarkService)
        {
            _bookmarkService = bookmarkService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle([FromBody] ToggleBookmarkRequest request)
        {
            int userId = GetCurrentUserId();
            if (userId == 0 || request == null)
            {
                return BadRequest(new { success = false });
            }

            bool isBookmarked = await _bookmarkService.ToggleBookmarkAsync(userId, request.ItemType, request.ItemId);
            int newCount = await _bookmarkService.GetBookmarkCountAsync(userId);

            return Json(new { success = true, isBookmarked = isBookmarked, totalCount = newCount });
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claim, out int id) ? id : 0;
        }
    }
}
