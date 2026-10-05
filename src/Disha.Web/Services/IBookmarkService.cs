using System.Collections.Generic;
using System.Threading.Tasks;
using Disha.Web.Models.Entities;

namespace Disha.Web.Services
{
    public interface IBookmarkService
    {
        Task<bool> ToggleBookmarkAsync(int userId, string itemType, int itemId);
        Task<bool> IsBookmarkedAsync(int userId, string itemType, int itemId);
        Task<List<Career>> GetBookmarkedCareersAsync(int userId);
        Task<List<FinancialAid>> GetBookmarkedAidAsync(int userId);
        Task<int> GetBookmarkCountAsync(int userId);
    }
}
