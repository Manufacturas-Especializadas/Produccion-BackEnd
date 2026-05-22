
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class DowntimeCodeRepository : IDowntimeCodeRepository
    {
        private readonly ApplicationDbContext _context;

        public DowntimeCodeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DowntimeCode>> GetAllAsync()
        {
            return await _context.DowntimeCodes.ToListAsync();
        }

        public async Task<DowntimeCode?> GetByIdAsync(int id)
        {
            return await _context.DowntimeCodes.FindAsync(id);
        }
    }
}