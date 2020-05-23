using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace CoreFunctions
{
    public static class EmailSender
    {
        [FunctionName("EmailSender")]
        public static async Task Run([QueueTrigger("%QueueName%")]string myQueueItem, ILogger log)
        {
            string apiKey = Environment.GetEnvironmentVariable("SendGridApiKey");
            string senderEmail = Environment.GetEnvironmentVariable("SenderEmail");
            string senderName = Environment.GetEnvironmentVariable("SenderName");

            var client = new SendGridClient(apiKey);
            var data = JsonConvert.DeserializeObject<MessageData>(myQueueItem);

            var from = new EmailAddress(senderEmail, senderName);
            var to = new EmailAddress(data.To.Email, data.To.Name);
            var message = MailHelper.CreateSingleTemplateEmail(from, to, data.TemplateId, data.Data);
            await client.SendEmailAsync(message);
            log.LogInformation($"Email to {to} has been sent.");
        }

        public class MessageData
        {
            public string TemplateId { get; set; }
            public dynamic Data { get; set; }
            public RecipientData To { get; set; }
        }

        public class RecipientData
        {
            public string Name { get; set; }
            public string Email { get; set; }
        }
    }
}
