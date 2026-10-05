using System.Collections.Generic;
using System.Threading.Tasks;
using Disha.Web.Models.Entities;

namespace Disha.Web.Services
{
    public class RecommendedCareerResult
    {
        public Career Career { get; set; } = null!;
        public int MatchScore { get; set; }
        public int MatchPercentage { get; set; }
        public string FitReason { get; set; } = string.Empty;
    }

    public interface IRecommendationService
    {
        Task<List<RecommendedCareerResult>> GetRecommendedCareersAsync(int userId, int limit = 5);
        Task<Dictionary<string, int>> GetUserTagScoresAsync(int userId);
    }
}
