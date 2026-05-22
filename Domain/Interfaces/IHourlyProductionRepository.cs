using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IHourlyProductionRepository
    {
        Task<int> AddAsync(HourlyProductionRecord record);

        Task<IEnumerable<HourlyProductionRecord>> GetByShiftAndLineAsync(DateTime date, int shift, int line);
    }
}