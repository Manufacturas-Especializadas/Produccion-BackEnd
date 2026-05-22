using Application.Dtos;

namespace Application.Interfaces
{
    public interface IExcelParserService
    {
        IEnumerable<DemandPlanDto> ParseDemandExcel(Stream fileStream);
    }
}