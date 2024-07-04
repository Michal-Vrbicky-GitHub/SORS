using System.Collections.Generic;
using System.Threading.Tasks;
using SORS.Data.Models;

namespace SORS.Services
{
    public interface IStationsService
    {
        Task<IEnumerable<Station>> GetCreditsAsync();
        Task AddCreditAsync(Station report);
    }
}
