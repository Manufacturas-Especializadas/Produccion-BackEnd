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

        public async Task<IEnumerable<string>> GetAvailableLinesAsync(DateTime date)
        {
            return await _demandPlanRepository.GetAvailableLinesByDateAsync(date);
        }

        public async Task<IEnumerable<OperatorCalculationResultDto>> ProcessDemandAndCalculateOperatorsAsync(IEnumerable<DemandPlanDto> demandPlans, string lineName)
        {
            TimeZoneInfo mexicoTimezone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)");

            DateTime nowInMexico = TimeZoneInfo.ConvertTime(DateTime.UtcNow, mexicoTimezone);

            var entitiesToSave = demandPlans.Select(dto => new DemandPlan
            {
                PartNumber = dto.PartNumber,
                Quantity = dto.Quantity,
                ShopOrder = dto.ShopOrder ?? string.Empty,
                Model = dto.Model ?? string.Empty,
                Description = dto.Description ?? string.Empty,
                LineName = !string.IsNullOrEmpty(lineName) ? lineName : dto.LineName,
                ProductionDate = dto.ProductionDate,
                UploadDate = nowInMexico
            });

            if (entitiesToSave.Any())
            {
                await _demandPlanRepository.AddRangeAsync(entitiesToSave);
            }

            var results = new List<OperatorCalculationResultDto>();

            foreach(var demand in demandPlans)
            {
                var childComponents = await _masterRepository.GetComponentsByParentAsync(demand.PartNumber);

                var validChildren = childComponents?.Where(c => c.TCiclo.HasValue && c.TCiclo.Value > 0).ToList();

                if (validChildren != null && validChildren.Any())
                {
                    decimal totalCycleTimeRaw = validChildren.Sum(c => c.TCiclo!.Value);
                    decimal totalCycleTime = Math.Round(totalCycleTimeRaw);

                    var bottleneckComponent = validChildren.OrderByDescending(c => c.TCiclo!.Value).First();

                    decimal piecesPerHour = bottleneckComponent.PzsHr.HasValue
                            ? bottleneckComponent.PzsHr.Value
                            : (3600m / bottleneckComponent.TCiclo!.Value);

                    decimal requiredHours = (demand.Quantity * totalCycleTime) / 3600m;
                    decimal requiredOperators = requiredHours / ShiftHours;

                    results.Add(new OperatorCalculationResultDto
                    {
                        PartNumber = demand.PartNumber,
                        Process = bottleneckComponent.Operation ?? "Ensamble",
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
                        Process = "Faltan T.Ciclo / Sin hijos",
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