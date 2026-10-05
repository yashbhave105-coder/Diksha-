using System.Collections.Generic;
using Disha.Web.Models.Entities;
using Disha.Web.Services;

namespace Disha.Web.Models.ViewModels
{
    public class DashboardViewModel
    {
        public User Student { get; set; } = null!;
        public int JourneyCompletionPercentage { get; set; }
        public bool QuizCompleted { get; set; }
        public int MatchedCareersCount { get; set; }
        public int SavedResourcesCount { get; set; }
        public FinancialAid? UrgentAid { get; set; }
        public List<RecommendedCareerResult> RecommendedCareers { get; set; } = new();
        public List<ChecklistItem> ChecklistItems { get; set; } = new();
        public List<MentorChatMessage> RecentMentorMessages { get; set; } = new();
        public List<FinancialAid> MatchedAidList { get; set; } = new();
    }
}
