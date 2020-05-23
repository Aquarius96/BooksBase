using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BooksBase.CoreFunctions
{
    public static class EmailSender
    {
        [FunctionName("EmailSender")]
        public static async Task Run([QueueTrigger("%QueueName%")]string myQueueItem, ILogger log)
        {
            string apiKey = "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";
            string senderEmail = Environment.GetEnvironmentVariable("SenderEmail");
            string senderName = Environment.GetEnvironmentVariable("SenderName");

            var client = new SendGridClient(apiKey);
            var data = JsonConvert.DeserializeObject<MessageData>(myQueueItem);

            var from = new EmailAddress(senderEmail, senderName);
            var to = new EmailAddress(data.To.Email, data.To.Name);
            var message = MailHelper.CreateSingleTemplateEmail(from, to, "d-0bcaa395547b43198ae46499f3661ee2", data.Data);
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
