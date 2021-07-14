using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Configuration;
using RabbitMqSummit2021.Common;
using RabbitMqSummit2021.MessageContracts;

namespace RabbitMqSummit2021.CloudFanout
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
            });

            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            await busControl.StartAsync(source.Token);

            try
            {
                while (true)
                {
                    string edgeId = await Task.Run(() =>
                    {
                        Console.WriteLine("Press enter or type quit to exit");
                        Console.Write("> ");
                        return Console.ReadLine();
                    });

                    if ("quit".Equals(edgeId, StringComparison.OrdinalIgnoreCase))
                        break;                    

                    Console.WriteLine($"Send messages to Edge {edgeId}");

                    var sendToUri = new Uri($"{rmqSettings.Url}/edge{edgeId}-add-transaction");
                    var endPoint = await busControl.GetSendEndpoint(sendToUri);
                    await endPoint.Send<IAddTransaction>(new { Value = "IAddTransaction" });

                    sendToUri = new Uri($"{rmqSettings.Url}/edge{edgeId}-remove-transaction");
                    endPoint = await busControl.GetSendEndpoint(sendToUri);
                    await endPoint.Send<IRemoveTransaction>(new { Value = "IRemoveTransaction" });

                    sendToUri = new Uri($"{rmqSettings.Url}/edge{edgeId}-finalize-transaction");
                    endPoint = await busControl.GetSendEndpoint(sendToUri);
                    await endPoint.Send<IFinalizeTransaction>(new { Value = "IFinalizeTransaction" });
                }
            }
            finally
            {
                await busControl.StopAsync();
            }
        }
    }
}
