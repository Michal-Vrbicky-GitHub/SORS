using SORS.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SORS.Services
{
    public interface IMeasureService
    {
        Task<IEnumerable<StationReport>> GetCreditsAsync();
        Task AddCreditAsync(StationReport report);
    }
}
