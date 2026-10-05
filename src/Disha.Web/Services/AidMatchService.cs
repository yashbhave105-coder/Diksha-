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
    public class AidMatchService : IAidMatchService
    {
        private readonly DishaDbContext _context;

        public AidMatchService(DishaDbContext context)
        {
            _context = context;
        }

        public async Task<List<FinancialAid>> GetMatchingAidAsync(User user)
        {
            var allAid = await _context.FinancialAids
                .Where(a => a.IsActive)
                .ToListAsync();

            var matched = new List<FinancialAid>();

            foreach (var aid in allAid)
            {
                bool stateMatch = true;
                bool classMatch = true;

                if (!string.IsNullOrWhiteSpace(aid.ApplicableStatesJson))
                {
                    try
                    {
                        var states = JsonSerializer.Deserialize<List<string>>(aid.ApplicableStatesJson);
                        if (states != null && states.Any())
                        {
                            stateMatch = string.IsNullOrWhiteSpace(user.State) ||
                                         states.Any(s => s.Equals(user.State, StringComparison.OrdinalIgnoreCase));
                        }
                    }
                    catch { }
                }

                if (!string.IsNullOrWhiteSpace(aid.ApplicableClassesJson))
                {
                    try
                    {
                        var classes = JsonSerializer.Deserialize<List<string>>(aid.ApplicableClassesJson);
                        if (classes != null && classes.Any())
                        {
                            classMatch = string.IsNullOrWhiteSpace(user.EducationLevel) ||
                                         classes.Any(c => c.Equals(user.EducationLevel, StringComparison.OrdinalIgnoreCase));
                        }
                    }
                    catch { }
                }

                if (stateMatch && classMatch)
                {
                    matched.Add(aid);
                }
            }

            return matched;
        }

        public async Task<FinancialAid?> GetUrgentAidAsync(User user)
        {
            var matched = await GetMatchingAidAsync(user);
            // Return first aid item that has an upcoming deadline or high priority title
            return matched.FirstOrDefault(a => a.Title.Contains("Grant", StringComparison.OrdinalIgnoreCase) || a.Title.Contains("Laptop", StringComparison.OrdinalIgnoreCase)) ?? matched.FirstOrDefault();
        }
    }
}
