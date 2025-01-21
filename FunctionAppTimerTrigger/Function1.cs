using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using FunctionAppTimerTrigger.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FunctionAppTimerTrigger
{
    public class Function1
    {
        private readonly string _connectionString;
        public Function1(IConfiguration configuration)
        {
            _connectionString = configuration["ServiceBusConnectionString"];
        }


        //The function is configured to run every 1 minute
        [FunctionName("Function1")]
        public void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            AddMessageAsync(new Message {
                Content = $"Message sent at: {DateTime.Now}"
            });
        }

        public async Task AddMessageAsync(Message message)
        {
            //instantiating the message queue as a sender
            await using var client = new ServiceBusClient(_connectionString);
            ServiceBusSender sender = client.CreateSender("mynewqueue");

            //sending a message to the queue    
            ServiceBusMessage serviceBusMessage = new ServiceBusMessage(message.Content);
            await sender.SendMessageAsync(serviceBusMessage);
        }
    }
}
