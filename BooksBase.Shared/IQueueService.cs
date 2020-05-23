using System.Threading;
using System.Threading.Tasks;

namespace BooksBase.Shared
{
    public interface IQueueService : IService
    {
        Task AddMessageAsync(string queueName, string message, CancellationToken cancellationToken = default);
    }
}
