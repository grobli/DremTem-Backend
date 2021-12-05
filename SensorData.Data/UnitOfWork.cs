using System.Threading;
using System.Threading.Tasks;
using SensorData.Core;
using SensorData.Core.Repositories;
using SensorData.Data.Repositories;

namespace SensorData.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ISensorDataContext _context;
        private ReadingRepository _readingRepository;


        public IReadingRepository Readings => _readingRepository ??= new ReadingRepository(_context);

        public UnitOfWork(ISensorDataContext context)
        {
            _context = context;
        }

        public async Task<int> CommitAsync(CancellationToken token = default)
        {
            return await _context.SaveChangesAsync(token);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}