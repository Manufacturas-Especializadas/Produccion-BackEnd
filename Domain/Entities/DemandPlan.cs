namespace Domain.Entities
{
    public class DemandPlan
    {
        public int Id { get; set; }

        public string PartNumber { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public string LineName { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string ShopOrder { get; set; } = string.Empty;

        public DateTime ProductionDate { get; set; }

        public DateTime UploadDate { get; set; }
    }
}