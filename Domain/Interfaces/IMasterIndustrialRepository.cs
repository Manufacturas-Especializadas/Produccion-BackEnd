using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IMasterIndustrialRepository
    {
        Task<IEnumerable<MasterIndustrial>> GetComponentsByParentAsync(string parentPartNumber);

        Task<MasterIndustrial?> GetByPartNumberAsync(string partNumber);
    }
}