using Application.Dtos;

namespace Application.Interfaces
{
    public interface IDemandCalculationService
    {
        Task<IEnumerable<OperatorCalculationResultDto>> ProcessDemandAndCalculateOperatorsAsync(IEnumerable<DemandPlanDto> demandPlans, string lineName);

        Task<IEnumerable<OperatorCalculationResultDto>> GetHistoricalCalculationAsync(DateTime date, string lineName);

        Task<IEnumerable<string>> GetAvailableLinesAsync(DateTime date);
    }
}