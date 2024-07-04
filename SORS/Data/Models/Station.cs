using System.ComponentModel.DataAnnotations;

namespace SORS.Data.Models
{
	public class Station
	{	[Key]
		public int StationID { get; set; }
		[Range(1, 100, ErrorMessage = "LvlMin must be between 1 and 100.")]
		public int LvlMin { get; set; } = 30;
		[Range(1, 100, ErrorMessage = "LvlMax must be between 1 and 100.")]
		public int LvlMax { get; set; } = 71;
		//[Range(0, int.MaxValue, ErrorMessage = "AlertDelay must be a positive number.")]
		public int AlertDelay { get; set; } = 240;
		public string? Name { get; set; }
        public ICollection<AlertEmail>? AlertEmails { get; set; }
		public string Token { get; set; } = "ChangeToRealToken";
		public DateTime? TokenExpiry { get; set; } = DateTime.Now.AddMonths(1);//DateTime.UtcNow malfunctioning
		public bool SentNoReporFromStationForAlertDelayTimeAlert { get; set; } = false;

        public Station()
		{	// Default values
			LvlMin = 30;     
			LvlMax = 71;     
			AlertDelay = 240;
		}

        /*public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (LvlMax <= LvlMin)
            {
                yield return new ValidationResult("LvlMax musí být větší než LvlMin.", new[] { nameof(LvlMax) });
            }
            if (AlertDelay <= 0)
            {
                yield return new ValidationResult("AlertDelay must be a positive number.", new[] { nameof(AlertDelay) });
            }
        }*/
    }
}
