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
    public class IndexModel : PageModel
    {
        private readonly SORS.Data.ApplicationDbContext _context;

        public IndexModel(SORS.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Station> Station { get;set; } = default!;

		[BindProperty(SupportsGet = true)]
		public string? SearchString { get; set; }

		public string CurrentFilter { get; set; }

		public async Task OnGetAsync()
        {
            if (_context.Stations != null)
            {
				var stations = from s in _context.Stations
							   select s;

				if (!string.IsNullOrEmpty(SearchString))
				{
					stations = stations.Where(s => s.Name.Contains(SearchString));
				}

				CurrentFilter = SearchString;

				Station = await stations.ToListAsync();
            }
        }
    }
}
