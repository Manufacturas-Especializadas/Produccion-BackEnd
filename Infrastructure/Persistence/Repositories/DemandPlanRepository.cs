using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class DemandPlanRepository : IDemandPlanRepository
    {
        private readonly ApplicationDbContext _context;

        public DemandPlanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddAsync(DemandPlan demandPlan)
        {
            await _context.DemandPlans.AddAsync(demandPlan);

            return await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<DemandPlan> demandPlans)
        {
            await _context.DemandPlans.AddRangeAsync(demandPlans);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<DemandPlan>> GetByDateAsync(DateTime productionDate)
        {
            return await _context.DemandPlans
                    .Where(d => d.ProductionDate.Date == productionDate.Date)
                    .ToListAsync();
        }
    }
}