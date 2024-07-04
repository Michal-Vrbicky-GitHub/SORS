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
    public class LatestReportsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public LatestReportsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<StationReportViewModel> Reports { get; set; }
        public int TotalReports { get; set; }

        public async Task<IActionResult> OnGetAsync(int pageSize = 10)
        {
            TotalReports = await _context.Report.CountAsync();

            Reports = await (from report in _context.Report
                             join station in _context.Stations on report.StationId equals station.StationID
                             orderby report.TimeStamp descending
                             select new StationReportViewModel
                             {
                                 StationId = report.StationId,
                                 StationName = station.Name,
                                 TimeStamp = report.TimeStamp,
                                 Value = report.Value,
                                 LvlMin = station.LvlMin,
                                 LvlMax = station.LvlMax
                             })
                             .Take(pageSize)
                             .ToListAsync();

            return Page();
        }
    }
    /*
    public class StationReportViewModel
    {
        public int StationId { get; set; }
        public string StationName { get; set; }
        public DateTime TimeStamp { get; set; }
        public int Value { get; set; }
        public int LvlMin { get; set; }
        public int LvlMax { get; set; }
    }*/
}
