using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class HourlyProductionRepository : IHourlyProductionRepository
    {
        private readonly ApplicationDbContext _context;

        public HourlyProductionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddAsync(HourlyProductionRecord record)
        {
            await _context.HourlyProductionRecords.AddAsync(record);

            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<HourlyProductionRecord>> GetByShiftAndLineAsync(DateTime date, int shift, int line)
        {
            return await _context.HourlyProductionRecords
                        .Include(r => r.DowntimeCode)
                        .Where(r => r.Date.Date == date.Date && r.Shift == shift && r.Line == line)
                        .OrderBy(r => r.Hour)
                        .ToListAsync();
        }
    }
}