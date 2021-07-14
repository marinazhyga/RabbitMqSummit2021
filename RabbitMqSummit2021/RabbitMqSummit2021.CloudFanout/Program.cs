using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using RabbitMqSummit2021.MessageContracts;

namespace RabbitMqSummit2021.CloudFanout
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri("rabbitmq://localhost/"), hst =>
                {
                    hst.Username("guest");
                    hst.Password("guest");
                });                
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

                    //var sendToUri = new Uri($"{RabbitMqConsts.RabbitMqUri}{RabbitMqConsts.RegisterDemandServiceQueue}");
                    var sendToUri = new Uri($"rabbitmq://localhost/edge{edgeId}-add-transaction");
                    var endPoint = await busControl.GetSendEndpoint(sendToUri);
                    await endPoint.Send<IAddTransaction>(new { Value = "IAddTransaction" });

                    sendToUri = new Uri($"rabbitmq://localhost/edge{edgeId}-remove-transaction");
                    endPoint = await busControl.GetSendEndpoint(sendToUri);
                    await endPoint.Send<IRemoveTransaction>(new { Value = "IRemoveTransaction" });

                    sendToUri = new Uri($"rabbitmq://localhost/edge{edgeId}-finalize-transaction");
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
