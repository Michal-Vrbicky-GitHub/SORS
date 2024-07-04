using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SORS.Data;
using SORS.Data.Models;

namespace SORS.Pages.Stations
{
    //[Authorize]
    public class CreateModel : PageModel
	{
		private readonly SORS.Data.ApplicationDbContext _context;
		public List<string> BadAlertEmails { get; set; } = new List<string>();//if a non-valid email is posted, the emails are loaded here so they can be preserved on the page
		public string[] ErrorrMSGs { get; set; } = new string[3];
		public CreateModel(SORS.Data.ApplicationDbContext context)
		{
			_context = context;
		}
		/*
        public IActionResult OnGet()
        {
            return Page();
        }*/
        public void OnGet()
		{
			Station = new Station(); 
			LoadPreviousEmails();
		}

		[BindProperty]
		public Station Station { get; set; } = default!;
		[BindProperty]
		public List<string> AlertEmails { get; set; } = new List<string>();


		// To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
		public async Task<IActionResult> OnPostAsync()
		{
			bool dataValidationFailure = false;
		  if (!ModelState.IsValid || _context.Stations == null || Station == null)
			{
				dataValidationFailure = true;//return Page(); after saving all content on page
			}
            if (Station.LvlMax <= Station.LvlMin){
				string errMsg = "LvlMax must be greater than LvlMin.";
                ModelState.AddModelError(nameof(Station.LvlMax), errMsg);
                ModelState.AddModelError(nameof(Station.LvlMin), errMsg);
				ErrorrMSGs[0] = ErrorrMSGs[1] = errMsg;//ModelError magically not displaying
				dataValidationFailure = true;
			}
            if (Station.AlertDelay <= 0) {
				string error = "AlertDelay must be a positive number.";
				ErrorrMSGs[2] = error;
				ModelState.AddModelError("AlertDelay", error);
                dataValidationFailure = true;
            }

            var alertEmails = Request.Form["alertEmail"];//.ToArray();
            List<string> invalidEmails = new List<string>();
            foreach (var email in alertEmails)
            {
                if (!string.IsNullOrEmpty(email) && !IsValidEmail(email))
                    invalidEmails.Add(email);
				else
					if(!AlertEmails.Contains(email))
						AlertEmails.Add(email);
			}

            if (invalidEmails.Count > 0)
            {
                foreach (var email in invalidEmails)
                    BadAlertEmails.Add(email);
                ModelState.AddModelError("AlertEmail", "One or more email addresses are invalid.");
                dataValidationFailure = true;
            }
			if (dataValidationFailure)
				return Page();
            _context.Stations.Add(Station);
			await _context.SaveChangesAsync();

			// emails
			AlertEmails.Clear();
            foreach (var email in alertEmails)
            {
                if (!string.IsNullOrEmpty(email))
                {
					if (!IsValidEmail(email))
					{
						ModelState.AddModelError("AlertEmail", "Invalid email address format.");
						foreach (var emailToPage in alertEmails){
							BadAlertEmails.Add(emailToPage);
						}
                        return Page();
                        return RedirectToPage("./Create?mails="/**/);
					}
					if (!AlertEmails.Contains(email)) { 
						var alertEmail = new AlertEmail{
							Email = email,
							StationID = Station.StationID
						};
						//if(!_context.AlertEmails.Contains(alertEmail))
						_context.AlertEmails.Add(alertEmail);
						AlertEmails.Add(email);
                    }
                }
            }
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
		}
		public static bool IsValidEmail(string email)
		{
			try
			{
				var addr = new System.Net.Mail.MailAddress(email);
				return addr.Address == email;
			}
			catch
			{
				return false;
			}
		}

		private void LoadPreviousEmails(){

		}
	}
}
