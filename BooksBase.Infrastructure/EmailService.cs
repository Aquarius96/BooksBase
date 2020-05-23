using BooksBase.Shared;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BooksBase.Infrastructure
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly IQueueService _queueService;

        public EmailService(IOptions<EmailSettings> options,
            IQueueService queueService)
        {
            _emailSettings = options.Value;
            _queueService = queueService;
        }

        public async Task SendAsync(string templateName, RecipientData to, dynamic data, CancellationToken cancellationToken = default)
        {
            var templateId = _emailSettings.Templates.First(t => t.Name == templateName).Id;

            var messageData = new EmailMessageData
            {
                TemplateId = templateId,
                Data = data,
                To = to
            };

            var message = JsonConvert.SerializeObject(messageData);
            await _queueService.AddMessageAsync(_emailSettings.QueueName, message, cancellationToken);
        }
    }
}
