using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class MasterIndustrialRepository : IMasterIndustrialRepository
    {
        private readonly ApplicationDbContext _context;

        public MasterIndustrialRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MasterIndustrial?> GetByPartNumberAsync(string partNumber)
        {
            return await _context.MasterIndustrials
                        .FirstOrDefaultAsync(m => m.ParentPartNumber == partNumber);
        }
    }
}