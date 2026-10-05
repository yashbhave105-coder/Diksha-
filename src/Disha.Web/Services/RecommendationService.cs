using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Disha.Web.Data;
using Disha.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Disha.Web.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly DishaDbContext _context;

        public RecommendationService(DishaDbContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string, int>> GetUserTagScoresAsync(int userId)
        {
            var responses = await _context.QuizResponses
                .Where(r => r.UserId == userId)
                .Include(r => r.Option)
                .ToListAsync();

            var tagScores = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                { "frontend", 0 },
                { "backend", 0 },
                { "data", 0 },
                { "security", 0 },
                { "support", 0 },
                { "design", 0 },
                { "cloud", 0 },
                { "qa", 0 }
            };

            foreach (var resp in responses)
            {
                if (resp.Option != null && !string.IsNullOrWhiteSpace(resp.Option.TagWeightsJson))
                {
                    try
                    {
                        var weights = JsonSerializer.Deserialize<Dictionary<string, int>>(resp.Option.TagWeightsJson);
                        if (weights != null)
                        {
                            foreach (var (tag, weight) in weights)
                            {
                                if (tagScores.ContainsKey(tag))
                                {
                                    tagScores[tag] += weight;
                                }
                                else
                                {
                                    tagScores[tag] = weight;
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Ignore JSON parsing errors gracefully
                    }
                }
            }

            return tagScores;
        }

        public async Task<List<RecommendedCareerResult>> GetRecommendedCareersAsync(int userId, int limit = 5)
        {
            var tagScores = await GetUserTagScoresAsync(userId);
            var careers = await _context.Careers
                .Where(c => c.IsActive)
                .Include(c => c.Tags)
                .ToListAsync();

            int maxPossibleScore = tagScores.Values.Sum();
            if (maxPossibleScore == 0) maxPossibleScore = 1;

            var results = new List<RecommendedCareerResult>();

            foreach (var career in careers)
            {
                int score = 0;
                var matchedTags = new List<string>();

                foreach (var tag in career.Tags)
                {
                    if (tagScores.TryGetValue(tag.Tag, out int tagScore) && tagScore > 0)
                    {
                        score += tagScore;
                        matchedTags.Add(tag.Tag);
                    }
                }

                int matchPercentage = Math.Min(98, Math.Max(45, (int)Math.Round((double)score / Math.Max(1, maxPossibleScore) * 100) + 40));

                string fitReason = BuildFitReason(matchedTags, tagScores);

                results.Add(new RecommendedCareerResult
                {
                    Career = career,
                    MatchScore = score,
                    MatchPercentage = matchPercentage,
                    FitReason = fitReason
                });
            }

            return results
                .OrderByDescending(r => r.MatchScore)
                .ThenByDescending(r => r.MatchPercentage)
                .Take(limit)
                .ToList();
        }

        private static string BuildFitReason(List<string> matchedTags, Dictionary<string, int> tagScores)
        {
            var topTags = tagScores.OrderByDescending(kv => kv.Value).Take(2).Select(kv => kv.Key).ToList();

            if (topTags.Contains("frontend") && topTags.Contains("design"))
                return "You enjoy visual web design, interactive interfaces, and intuitive user experiences.";
            if (topTags.Contains("backend") || topTags.Contains("cloud"))
                return "You excel at logical problem solving, server architectures, and API integrations.";
            if (topTags.Contains("data"))
                return "You have a natural affinity for analyzing numbers, pattern recognition, and statistics.";
            if (topTags.Contains("security") || topTags.Contains("support"))
                return "You possess strong hands-on troubleshooting abilities and protective IT instincts.";
            if (topTags.Contains("qa"))
                return "You demonstrate meticulous attention to detail and software testing precision.";

            return "Matches your interests in problem solving, computer technology, and practical IT skills.";
        }
    }
}
