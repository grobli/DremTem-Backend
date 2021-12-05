using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SensorData.Core.Models;

namespace SensorData.Core
{
    public interface ISensorDataContext : IDisposable
    {
        DbSet<Reading> Readings { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}