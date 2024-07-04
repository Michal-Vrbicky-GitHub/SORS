using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SORS.Data;
using SORS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SORS.Pages
{
    public class StationGraphModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public StationGraphModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int StationId { get; set; }

        public List<DateTime> Timestamps { get; set; } = new List<DateTime>();
        public List<int> StationValues { get; set; } = new List<int>();
        public string ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            if (StationId <= 0)
            {
                ErrorMessage = "Please enter a valid Station ID.";
                return;
            }

            var reports = await _context.Report
                .Where(r => r.StationId == StationId)
                .OrderBy(r => r.TimeStamp)
                .ToListAsync();

            if (!reports.Any())
            {
                ErrorMessage = "No reports found for this station.";
                return;
            }

            foreach (var report in reports)
            {
                Timestamps.Add(report.TimeStamp);
                StationValues.Add(report.Value);
            }
        }
    }
}
