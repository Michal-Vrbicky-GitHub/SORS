namespace SORS.Pages
{
    public class StationReportViewModel
    {
        public int StationId { get; set; }
        public string StationName { get; set; }
        public DateTime TimeStamp { get; set; }
        public double Value { get; set; }
        public double LvlMin { get; set; }
        public double LvlMax { get; set; }
        public int SortOrder { get; set; }
    }
}
