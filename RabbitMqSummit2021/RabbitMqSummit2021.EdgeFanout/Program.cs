using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Configuration;
using RabbitMqSummit2021.Consumers;

namespace RabbitMqSummit2021.EdgeFanout
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            string edgeId = string.Empty;

            if (args != null && args.Any())
            {
                edgeId = args[0];
            }
            else
            {
                Console.WriteLine($"Please specify edgeId as argument");
            }

            var builder = new ConfigurationBuilder()
              .AddJsonFile($"appsettings.json", true, true);

            var config = builder.Build();

            var username = config["RabbitMq:Username"];
            var password = config["RabbitMq:Password"];
            var url = config["RabbitMq:Url"];

            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri($"{url}"), hst =>
                {
                    hst.Username(username);
                    hst.Password(password);
                });

                cfg.ReceiveEndpoint($"edge{edgeId}-add-transaction", e =>
                {
                    e.Consumer<AddTransactionConsumer>();
                });

                cfg.ReceiveEndpoint($"edge{edgeId}-remove-transaction", e =>
                {
                    e.Consumer<RemoveTransactionConsumer>();
                });

                cfg.ReceiveEndpoint($"edge{edgeId}-finalize-transaction", e =>
                {
                    e.Consumer<FinalizeTransactionConsumer>();
                });
            });

            await busControl.StartAsync();
            Console.WriteLine($"Edge {edgeId} started... Press enter to exit");
            Console.ReadLine();
            await busControl.StopAsync();            
        }
    }
}
