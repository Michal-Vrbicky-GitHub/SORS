using Microsoft.AspNetCore.Identity;

namespace SORS.Data.Models
{
	public class ApplicationUserToken : IdentityUserToken<string>
	{
		public DateTime? ExpiryDate { get; set; }
	}
}