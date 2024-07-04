using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SORS.Data.Models
{
    public class StationReport
    {
        public int StationId { get; set; }
        public DateTime TimeStamp { get; set; }
        public int Value { get; set; }
        [Key]
        public Guid Id { get; set; }
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
		public string Token { get; set; }
	}
}




