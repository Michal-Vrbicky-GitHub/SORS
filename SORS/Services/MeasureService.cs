using SORS.Data.Models;
using SORS.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SORS.Data;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using SORS.Data;
using SORS.Services;

namespace SORS.Services
{
    public class MeasureService : IMeasureService
    {
        private readonly ApplicationDbContext _dbContext;//AppDbContextReports

        public MeasureService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<StationReport>> GetCreditsAsync()
        {
            return await _dbContext.Report.ToListAsync();
        }

        public async Task AddCreditAsync(StationReport report)
        {
            _dbContext.Report.Add(report);
            await _dbContext.SaveChangesAsync();
        }
    }
}
