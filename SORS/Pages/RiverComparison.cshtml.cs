using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SORS.Data;
using SORS.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SORS.Pages
{
    public class RiverComparisonModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public RiverComparisonModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<string> StationNames { get; set; } = new List<string>();
        public List<int> MinValues { get; set; } = new List<int>();
        public List<int> MaxValues { get; set; } = new List<int>();
        public List<int> LatestValues { get; set; } = new List<int>();

        public async Task OnGetAsync()
        {
            var stations = await _context.Stations.ToListAsync();

            var reports = await _context.Report
                .ToListAsync();

            var groupedReports = reports
                .Join(stations, r => r.StationId, s => s.StationID, (r, s) => new { r.Value, r.TimeStamp, s.Name })
                .GroupBy(rs => rs.Name)
                .Select(g => new
                {
                    StationName = g.Key,
                    MinValue = g.Min(rs => rs.Value),
                    MaxValue = g.Max(rs => rs.Value),
                    LatestValue = g.OrderByDescending(rs => rs.TimeStamp).First().Value
                })
                .ToList();

            foreach (var group in groupedReports)
            {
                StationNames.Add(group.StationName);
                MinValues.Add(group.MinValue);
                MaxValues.Add(group.MaxValue);
                LatestValues.Add(group.LatestValue);
            }
        }

        public async Task<IActionResult> OnGetJsonAsync()
        {
            var stations = await _context.Stations.ToListAsync();

            var reports = await _context.Report
                .ToListAsync();

            var groupedReports = reports
                .Join(stations, r => r.StationId, s => s.StationID, (r, s) => new { r.Value, r.TimeStamp, s.Name })
                .GroupBy(rs => rs.Name)
                .Select(g => new
                {
                    StationName = g.Key,
                    MinValue = g.Min(rs => rs.Value),
                    MaxValue = g.Max(rs => rs.Value),
                    LatestValue = g.OrderByDescending(rs => rs.TimeStamp).First().Value
                })
                .ToList();

            var stationNames = new List<string>();
            var minValues = new List<int>();
            var maxValues = new List<int>();
            var latestValues = new List<int>();

            foreach (var group in groupedReports)
            {
                stationNames.Add(group.StationName);
                minValues.Add(group.MinValue);
                maxValues.Add(group.MaxValue);
                latestValues.Add(group.LatestValue);
            }

            return new JsonResult(new { stationNames, minValues, latestValues, maxValues });
        }
    }
}
