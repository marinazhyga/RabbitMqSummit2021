using MassTransit;
using RabbitMQ.Client;
using RabbitMqSummit2021.Common;
using RabbitMqSummit2021.MessageContracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMqSummit2021.CloudDirect
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var rmqSettings = SettingsExtension.Load();

            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri($"{rmqSettings.Url}"), hst =>
                {
                    hst.Username(rmqSettings.Username);
                    hst.Password(rmqSettings.Password);
                });               

                cfg.Send<IAddTransaction>(x =>
                {
                    x.UseRoutingKeyFormatter(context => context.Message.Value);
                });
                cfg.Publish<IAddTransaction>(x => x.ExchangeType = ExchangeType.Direct);

                cfg.Send<IRemoveTransaction>(x =>
                {
                    x.UseRoutingKeyFormatter(context => context.Message.Value);
                });
                cfg.Publish<IRemoveTransaction>(x => x.ExchangeType = ExchangeType.Direct);

                cfg.Send<IFinalizeTransaction>(x =>
                {
                    x.UseRoutingKeyFormatter(context => context.Message.Value);
                });
                cfg.Publish<IFinalizeTransaction>(x => x.ExchangeType = ExchangeType.Direct);

            });

            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            await busControl.StartAsync(source.Token);

            try
            {
                while (true)
                {
                    string value = await Task.Run(() =>
                    {
                        Console.WriteLine("Press enter or type quit to exit");
                        Console.Write("> ");
                        return Console.ReadLine();
                    });

                    if ("quit".Equals(value, StringComparison.OrdinalIgnoreCase))
                        break;

                    var random = new Random();
                    var edgeId = random.Next(1, 3);

                    Console.WriteLine($"Send messages to Edge {edgeId}");

                    var endpoint = $"edge{edgeId}-outbound";

                    var sendToUri = new Uri($"{rmqSettings.Url}/edge{edgeId}-add-transaction");
                    var endPoint = await busControl.GetSendEndpoint(sendToUri);
                    await endPoint.Send<IAddTransaction>(new { Value = $"edge{edgeId}-add-transaction" });

                    sendToUri = new Uri($"{rmqSettings.Url}/edge{edgeId}-remove-transaction");
                    endPoint = await busControl.GetSendEndpoint(sendToUri);
                    await endPoint.Send<IRemoveTransaction>(new { Value = $"edge{edgeId}-remove-transaction" });

                    sendToUri = new Uri($"{rmqSettings.Url}/edge{edgeId}-finalize-transaction");
                    endPoint = await busControl.GetSendEndpoint(sendToUri);
                    await endPoint.Send<IFinalizeTransaction>(new { Value = $"edge{edgeId}-finalize-transaction" });
                }
            }
            finally
            {
                await busControl.StopAsync();
            }
        }
    }
}
