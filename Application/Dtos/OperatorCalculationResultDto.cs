
namespace Application.Dtos
{
    public class OperatorCalculationResultDto
    {
        public string PartNumber { get; set; } = string.Empty;

        public string Process { get; set; } = string.Empty;

        public decimal PiecesPerHour { get; set; }

        public decimal RequiredHours { get; set; }

        public decimal RequiredOperators { get; set; }
    }
}