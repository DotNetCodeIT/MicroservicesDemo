using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BackendWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private IConfiguration _configuration;
        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);


                //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
    //CloudConfigurationManager.GetSetting("StorageConnectionString"));

                var connectionString = Environment.GetEnvironmentVariable("StorageConnectionString") ?? _configuration.GetConnectionString("StorageConnectionString"); 
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

                // Create the queue client.
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

                // Retrieve a reference to a container.
                CloudQueue queue = queueClient.GetQueueReference(Environment.GetEnvironmentVariable("ImageWorkerQueueName") ?? "imagesworker");

                // Create the queue if it doesn't already exist
                queue.CreateIfNotExists();

                    // Get the next message
                    CloudQueueMessage retrievedMessage = queue.GetMessage();

                if (retrievedMessage != null) { 
                
                    _logger.LogInformation($"Worker received: {retrievedMessage.AsString}");

                    try
                    {
                        ImageWorkerMessage msg = JsonConvert.DeserializeObject<ImageWorkerMessage>(retrievedMessage.AsString);

                        using (WebClient client = new WebClient())
                        {
                            client.DownloadFileAsync(new Uri(msg.Url), Path.Combine("storage", "images", msg.Id.ToString() + ".png"));
                        }


                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation($"Error received: {ex.Message} - Stack: {ex.StackTrace}");

                    }

                    //Process the message in less than 30 seconds, and then delete the message
                    queue.DeleteMessage(retrievedMessage);


                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }

    public class ImageWorkerMessage
    {
        public int Id { get; set; }
        public string Url { get; set; }
    }
}
