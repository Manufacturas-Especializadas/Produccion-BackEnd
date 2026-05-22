namespace Application.Dtos
{
    public class DemandPlanDto
    {
        public string LineName { get; set; } = string.Empty;

        public string ShopOrder { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string PartNumber { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public DateTime ProductionDate { get; set; }
    }
}