
namespace Domain.Entities
{
    public class DowntimeCode
    {
        public int Id { get; set; }

        public string Area { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public ICollection<HourlyProductionRecord> HourlyProductionRecords { get; set; } = new List<HourlyProductionRecord>();

    }
}