using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SORS.Data;
using SORS.Data.Models;

namespace SORS.Pages.Stations
{
    public class DetailsModel : PageModel
    {
        private readonly SORS.Data.ApplicationDbContext _context;

        public DetailsModel(SORS.Data.ApplicationDbContext context)
        {
            _context = context;
        }

      public Station Station { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {/*
            var station = await _context.Stations.FirstOrDefaultAsync(m => m.StationID == id);
            if (station == null){return NotFound();}else{Station = station;}return Page();*/
            if (id == null || _context.Stations == null)
            {
                return NotFound();
            }

            var station = await _context.Stations
                .Include(s => s.AlertEmails)
                .FirstOrDefaultAsync(m => m.StationID == id);

            if (station == null)
            {
                return NotFound();
            }

            Station = station;
            return Page();
        }
    }
}
