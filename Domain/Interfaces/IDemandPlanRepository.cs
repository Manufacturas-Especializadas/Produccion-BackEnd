using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IDemandPlanRepository
    {
        Task<int> AddAsync(DemandPlan demandPlan);

        Task<IEnumerable<DemandPlan>> GetByDateAsync(DateTime productionDate);

        Task AddRangeAsync(IEnumerable<DemandPlan> demandPlans);
    }
}