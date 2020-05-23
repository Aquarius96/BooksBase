using BooksBase.Shared;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace BooksBase.Infrastructure
{
    public class QueueService : IQueueService
    {
        private readonly string _connectionString;
        private CloudStorageAccount StorageAccount => CloudStorageAccount.Parse(_connectionString);
        private CloudQueueClient QueueClient => StorageAccount.CreateCloudQueueClient();

        public QueueService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AzureStorageConnection");
        }

        public async Task AddMessageAsync(string queueName, string message, CancellationToken cancellationToken = default)
        {
            var queue = await GetQueueAsync(queueName, cancellationToken);
            var queueMessage = new CloudQueueMessage(message);
            await queue.AddMessageAsync(queueMessage, cancellationToken);
        }

        private async Task<CloudQueue> GetQueueAsync(string name, CancellationToken cancellationToken = default)
        {
            var queue = QueueClient.GetQueueReference(name);
            await queue.CreateIfNotExistsAsync(cancellationToken);

            return queue;
        }
    }
}
