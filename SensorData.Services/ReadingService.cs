using SensorData.Core.Repositories;
using SensorData.Core.Services;

namespace SensorData.Services
{
    public class ReadingService : IReadingService
    {
        private readonly IReadingRepository _repository;

        public ReadingService(IReadingRepository repository)
        {
            _repository = repository;
        }
    }
}