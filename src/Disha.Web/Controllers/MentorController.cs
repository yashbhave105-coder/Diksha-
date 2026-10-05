using System.Security.Claims;
using System.Threading.Tasks;
using Disha.Web.Services.AiMentor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Disha.Web.Controllers
{
    public class MentorMessageRequest
    {
        public string Message { get; set; } = string.Empty;
    }

    [Authorize]
    public class MentorController : Controller
    {
        private readonly IAiMentorService _aiMentorService;

        public MentorController(IAiMentorService aiMentorService)
        {
            _aiMentorService = aiMentorService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int userId = GetCurrentUserId();
            var messages = await _aiMentorService.GetChatHistoryAsync(userId, 50);
            return View(messages);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage([FromBody] MentorMessageRequest request)
        {
            int userId = GetCurrentUserId();
            if (userId == 0 || request == null || string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new { success = false, message = "Invalid message content." });
            }

            string reply = await _aiMentorService.SendMessageAsync(userId, request.Message);

            return Json(new
            {
                success = true,
                userMessage = request.Message,
                reply = reply
            });
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claim, out int id) ? id : 0;
        }
    }
}
