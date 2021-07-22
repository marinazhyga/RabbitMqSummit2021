using MassTransit;
using RabbitMQ.Client;
using RabbitMqSummit2021.Common;
using RabbitMqSummit2021.Consumers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMqSummit2021.EdgeDirect
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

            var rmqSettings = SettingsExtension.Load();
         
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri($"{rmqSettings.Url}"), hst =>
                {
                    hst.Username(rmqSettings.Username);
                    hst.Password(rmqSettings.Password);
                });

                var endpoint = $"edge{edgeId}-outbound";

                cfg.ReceiveEndpoint($"edge{edgeId}-add-transaction", x =>
                {
                    x.Consumer<AddTransactionConsumer>();

                    x.Bind(endpoint, s =>
                    {
                        s.RoutingKey = $"edge{edgeId}-add-transaction";
                        s.ExchangeType = ExchangeType.Direct;
                    });
                });

                cfg.ReceiveEndpoint($"edge{edgeId}-remove-transaction", x =>
                {
                    x.Consumer<RemoveTransactionConsumer>();

                    x.Bind(endpoint, s =>
                    {
                        s.RoutingKey = $"edge{edgeId}-remove-transaction";
                        s.ExchangeType = ExchangeType.Direct;
                    });
                });

                cfg.ReceiveEndpoint($"edge{edgeId}-finalize-transaction", x =>
                {
                    x.Consumer<FinalizeTransactionConsumer>();

                    x.Bind(endpoint, s =>
                    {
                        s.RoutingKey = $"edge{edgeId}-finalize-transaction";
                        s.ExchangeType = ExchangeType.Direct;
                    });
                });                
            });

            await busControl.StartAsync();
            Console.WriteLine($"Edge direct {edgeId} started... Press enter to exit");
            Console.ReadLine();
            await busControl.StopAsync();
        }
    }
}
