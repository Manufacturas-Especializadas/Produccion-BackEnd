using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IMasterIndustrialRepository
    {
        Task<MasterIndustrial?> GetByPartNumberAsync(string partNumber);
    }
}