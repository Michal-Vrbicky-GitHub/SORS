using SORS.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SORS.Data;
using SORS.Data.Models;

namespace SORS.Services
{
    public class StationsService : IStationsService
    {
        private readonly ApplicationDbContext _dbContext;

        public StationsService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Station>> GetCreditsAsync()
        {
            return await _dbContext.Stations.ToListAsync();
        }

        public async Task AddCreditAsync(Station station)
        {
            _dbContext.Stations.Add(station);
            await _dbContext.SaveChangesAsync();
        }
    }
}

