using System.Collections.Generic;
using System.Threading.Tasks;
using Disha.Web.Models.Entities;

namespace Disha.Web.Services.AiMentor
{
    public interface IAiMentorService
    {
        Task<string> SendMessageAsync(int userId, string studentMessage);
        Task<List<MentorChatMessage>> GetChatHistoryAsync(int userId, int limit = 20);
    }
}
