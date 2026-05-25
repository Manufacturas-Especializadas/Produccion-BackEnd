using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IDemandPlanRepository
    {
        Task<int> AddAsync(DemandPlan demandPlan);

        Task<IEnumerable<DemandPlan>> GetByDateAsync(DateTime productionDate);

        Task<IEnumerable<DemandPlan>> GetDemandByDateAndLineAsync(DateTime date, string lineName);

        Task<IEnumerable<string>> GetAvailableLinesByDateAsync(DateTime date);

        Task AddRangeAsync(IEnumerable<DemandPlan> demandPlans);
    }
}