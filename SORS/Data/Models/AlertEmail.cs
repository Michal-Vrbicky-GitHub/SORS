using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SORS.Data.Models
{
    public class AlertEmail
    {
        [Key]
        public int AlertEmailID { get; set; }

		//[Required, EmailAddress]
        [Required]
		[EmailAddress(ErrorMessage = "Invalid email address")]
		public string Email { get; set; } = string.Empty;

        public int StationID { get; set; }
        public Station Station { get; set; }
    }
}
