using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;

namespace Shared.Services.GrpcClientServices
{
    public interface IGrpcClientProvider<TClient> where TClient : ClientBase
    {
        (GrpcChannel channel, string serviceId) NextChannel { get; }
        (TClient client, string serviceId) NextClient { get; }

        Task RefreshChannels(CancellationToken token = default);
        TClient GetClientById(string id);
    }
}