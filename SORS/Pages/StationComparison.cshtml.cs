using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SORS.Data;
using SORS.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SORS.Pages
{
    public class StationComparisonModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public StationComparisonModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<string> StationNames { get; set; } = new List<string>();
        public List<int> StationValues { get; set; } = new List<int>();

        public async Task OnGetAsync()
        {
            var stations = await _context.Stations.ToListAsync();

            foreach (var station in stations)
            {
                var latestReport = await _context.Report
                    .Where(r => r.StationId == station.StationID)
                    .OrderByDescending(r => r.TimeStamp)
                    .FirstOrDefaultAsync();

                if (latestReport != null)
                {
                    if(station.Name == null)
                        StationNames.Add(station.StationID.ToString());
                    else
                        StationNames.Add(station.StationID + "–" + station.Name);
                    StationValues.Add(latestReport.Value); 
                }
            }
        }
    }
}
