
namespace Domain.Entities
{
    public class HourlyProductionRecord
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int Shift { get; set; }

        public int Line { get; set; }

        public int Hour { get; set; }

        public string PartNumber { get; set; } = string.Empty;

        public int ActualPieces { get; set; }

        public string EmployeeNumber { get; set; } = string.Empty;

        public int? DowntimeCodeId { get; set; }

        public DowntimeCode? DowntimeCode { get; set; }
    }
}