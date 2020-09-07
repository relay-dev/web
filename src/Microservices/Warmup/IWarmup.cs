using System.Threading;
using System.Threading.Tasks;

namespace Microservices.Warmup
{
    public interface IWarmup
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}
