using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class DemandCalculationService : IDemandCalculationService
    {
        private readonly IMasterIndustrialRepository _masterRepository;
        private readonly IDemandPlanRepository _demandPlanRepository;

        private const decimal ShiftHours = 9.3m;

        public DemandCalculationService(
            IMasterIndustrialRepository masterRepository,
            IDemandPlanRepository demandPlanRepository)
        {
            _masterRepository = masterRepository;
            _demandPlanRepository = demandPlanRepository;
        }

        public async Task<IEnumerable<OperatorCalculationResultDto>> ProcessDemandAndCalculateOperatorsAsync(IEnumerable<DemandPlanDto> demandPlans, string lineName)
        {

            var entitiesToSave = demandPlans.Select(dto => new DemandPlan
            {
                PartNumber = dto.PartNumber,
                Quantity = dto.Quantity,
                ShopOrder = dto.ShopOrder ?? string.Empty,
                Model = dto.Model ?? string.Empty,
                Description = dto.Description ?? string.Empty,
                LineName = !string.IsNullOrEmpty(lineName) ? lineName : dto.LineName,
                ProductionDate = dto.ProductionDate,
                UploadDate = DateTime.Now,
            });

            if (entitiesToSave.Any())
            {
                await _demandPlanRepository.AddRangeAsync(entitiesToSave);
            }

            var results = new List<OperatorCalculationResultDto>();

            foreach(var demand in demandPlans)
            {
                var masterData = await _masterRepository.GetByPartNumberAsync(demand.PartNumber);

                if(masterData != null && masterData.TCiclo.HasValue && masterData.TCiclo.Value > 0)
                {
                    decimal cycleTimeSeconds = masterData.TCiclo.Value;
                    decimal requiredHours = (demand.Quantity * cycleTimeSeconds) / 3600m;

                    decimal requiredOperators = requiredHours / ShiftHours;

                    decimal piecesPerHour = masterData.PzsHr.HasValue
                            ? masterData.PzsHr.Value
                            : (3600m / cycleTimeSeconds);

                    results.Add(new OperatorCalculationResultDto
                    {
                        PartNumber = demand.PartNumber,
                        Process = masterData.Operation ?? "N/A",
                        PiecesPerHour = Math.Round(piecesPerHour, 2),
                        RequiredHours = Math.Round(requiredHours, 2),
                        RequiredOperators = Math.Round(requiredOperators, 2),
                    });
                }
                else
                {
                    results.Add(new OperatorCalculationResultDto
                    {
                        PartNumber = demand.PartNumber,
                        Process = "Not Found / Missing Cycle Time",
                        PiecesPerHour = 0,
                        RequiredHours = 0,
                        RequiredOperators = 0
                    });
                }
            }

            return results;
        }
    }
}