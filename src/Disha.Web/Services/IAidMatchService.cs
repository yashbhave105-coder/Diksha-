using System.Collections.Generic;
using System.Threading.Tasks;
using Disha.Web.Models.Entities;

namespace Disha.Web.Services
{
    public interface IAidMatchService
    {
        Task<List<FinancialAid>> GetMatchingAidAsync(User user);
        Task<FinancialAid?> GetUrgentAidAsync(User user);
    }
}
