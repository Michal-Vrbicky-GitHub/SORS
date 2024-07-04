using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SORS.Data;
using SORS.Data.Models;
using SORS.Pages.Stations;

namespace SORS.Pages.Stations
{
    public class EditModel : PageModel
    {
        private readonly SORS.Data.ApplicationDbContext _context;

        public EditModel(SORS.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Station Station { get; set; } = default!;
        [BindProperty]
        public List<string> AlertEmails { get; set; } = new List<string>();
		public string[] ErrorrMSGs { get; set; } = new string[3];

		public async Task<IActionResult> OnGetAsync(int? id)
        {
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
            AlertEmails = Station.AlertEmails.Select(e => e.Email).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            bool dataValidationFailure = false;
            if (!ModelState.IsValid)
            {
                dataValidationFailure = true;
            }
			if (Station.LvlMax <= Station.LvlMin){
				ErrorrMSGs[0] = "LvlMin must be lower than LvlMax.";
				ErrorrMSGs[1] = "LvlMax must be greater than LvlMin.";
                dataValidationFailure = true;
            }
			if (Station.AlertDelay <= 0){
				ErrorrMSGs[2] = "AlertDelay must be a positive number.";
                dataValidationFailure = true;
            }
			

            var existingAlertEmails = _context.AlertEmails.Where(e => e.StationID == Station.StationID);
            _context.AlertEmails.RemoveRange(existingAlertEmails);
            AlertEmails.Clear();
            var alertEmails = Request.Form["alertEmail"];/*
            //int index = 0;
            bool nonvalidEmail = false;
            foreach (var email in alertEmails)
            {
                //if (index==alertEmails.Count()-1)
                if (email == null  ||  email == "")
                    break;
                AlertEmails.Add(email);
                if (!string.IsNullOrEmpty(email) && !CreateModel.IsValidEmail(email))
                    ModelState.AddModelError("AlertEmails", "One or more email addresses are invalid.");
                //index++;
            }
            if (nonvalidEmail)
                return Page();*/
            foreach (var email in alertEmails)
            {
                if (!string.IsNullOrEmpty(email))
                {
                    AlertEmails.Add(email);
                    if (!CreateModel.IsValidEmail(email))
                    {
                        ModelState.AddModelError("AlertEmails", "One or more email addresses are invalid.");
                        dataValidationFailure = true;
                    }
                }
            }
            if (dataValidationFailure)
                return Page();

            AlertEmails.Clear();
            foreach (var email in alertEmails)
            {
                if (!string.IsNullOrEmpty(email))
                {
                    if (!CreateModel.IsValidEmail(email))
                    {
                        ModelState.AddModelError("AlertEmail", "Invalid email address format.");
                        return Page();
                    }
                    if (!AlertEmails.Contains(email))
                    {
                        var alertEmail = new AlertEmail
                        {
                            Email = email,
                            StationID = Station.StationID
                        };
                        _context.AlertEmails.Add(alertEmail);
                        AlertEmails.Add(email);
                    }
                }
            }

            _context.Attach(Station).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StationExists(Station.StationID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private bool StationExists(int id)
        {
            return _context.Stations.Any(e => e.StationID == id);
        }
    }
}
