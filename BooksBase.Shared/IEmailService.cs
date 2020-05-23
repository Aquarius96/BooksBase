using System.Threading;
using System.Threading.Tasks;

namespace BooksBase.Shared
{
    public interface IEmailService : IService
    {
        Task SendAsync(string templateName, RecipientData to, dynamic data, CancellationToken cancellationToken = default);
    }
}
