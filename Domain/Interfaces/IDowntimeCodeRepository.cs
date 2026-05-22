using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IDowntimeCodeRepository
    {
        Task<IEnumerable<DowntimeCode>> GetAllAsync();

        Task<DowntimeCode?> GetByIdAsync(int id);
    }
}