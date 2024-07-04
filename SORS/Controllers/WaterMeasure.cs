using System.ComponentModel.DataAnnotations;
namespace SORS.Controllers
{
    public class WaterMeasure
    {
        public int IdRiver { get; set; }
        public int IdStation { get; set; }
        public DateTime TimeOfMeasure { get; set; }
        public int Value { get; set; }
        public Guid N { get; set; }
        public string? Summary { get; set; }
    }
}