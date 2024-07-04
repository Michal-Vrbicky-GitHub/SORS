using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SORS.Data;
using SORS.Data.Models;
using SORS.Pages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SORS.Pages
{
    public class ReportsModel : PageModel
    {
        private readonly  /*AppDbContextReports*/ApplicationDbContext _context;

        public ReportsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<StationReportViewModel> Reports { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime? FromDate { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime? ToDate { get; set; }
        /*
        public async Task<IActionResult> OnGetAsync()
        {
            Reports = await (from report in _context.Report
                             join station in _context.Stations on report.StationId equals station.StationID
                             select new StationReportViewModel
                             {
                                 StationId = report.StationId,
                                 StationName = station.Name,
                                 TimeStamp = report.TimeStamp,
                                 Value = report.Value,
                                 LvlMin = station.LvlMin,
                                 LvlMax = station.LvlMax,
                                 SortOrder = report.Value > station.LvlMax ? 1 :
                                             report.Value < station.LvlMin ? 2 : 3
                             })
                .OrderBy(r => r.SortOrder)
                .ThenByDescending(r => r.SortOrder == 1 ? r.Value : 0)
                .ThenBy(r => r.SortOrder == 2 ? r.Value : 0)
                .ThenByDescending(r => r.SortOrder == 3 ? r.TimeStamp : default(DateTime?))
                .ToListAsync();


            return Page();
        }*/
        public async Task<IActionResult> OnGetAsync()
        {
            var query = from report in _context.Report
                        join station in _context.Stations on report.StationId equals station.StationID
                        select new StationReportViewModel
                        {
                            StationId = report.StationId,
                            StationName = station.Name,
                            TimeStamp = report.TimeStamp,
                            Value = report.Value,
                            LvlMin = station.LvlMin,
                            LvlMax = station.LvlMax,
                            SortOrder = report.Value > station.LvlMax ? 1 :
                                        report.Value < station.LvlMin ? 2 : 3
                        };

            if (FromDate.HasValue)
            {
                query = query.Where(r => r.TimeStamp >= FromDate.Value);
            }

            if (ToDate.HasValue)
            {
                query = query.Where(r => r.TimeStamp <= ToDate.Value);
            }

            Reports = await query
                .OrderBy(r => r.SortOrder)
                .ThenByDescending(r => r.SortOrder == 1 ? r.Value : 0)
                .ThenBy(r => r.SortOrder == 2 ? r.Value : 0)
                .ThenByDescending(r => r.SortOrder == 3 ? r.TimeStamp : default(DateTime?))
                .ToListAsync();

            return Page();
        }
    }
}

