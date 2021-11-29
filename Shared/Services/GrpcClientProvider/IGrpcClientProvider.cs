using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace Shared.Services.GrpcClientProvider
{
    public interface IGrpcClientProvider<TClient> where TClient : ClientBase
    {
        Task<TClient> GetRandomClientAsync(CancellationToken cancellationToken = default);

        Task<TClient> GetClientByIdAsync(string id, CancellationToken cancellationToken = default);

        //    Task<TClient> GetClientWithUriAsync(Uri address, CancellationToken cancellationToken = default);
    }
}