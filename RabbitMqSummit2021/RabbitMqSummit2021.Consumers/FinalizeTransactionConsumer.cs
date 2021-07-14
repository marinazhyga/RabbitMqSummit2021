using MassTransit;
using RabbitMqSummit2021.MessageContracts;
using System;
using System.Threading.Tasks;

namespace RabbitMqSummit2021.Consumers
{
    public class FinalizeTransactionConsumer : IConsumer<IFinalizeTransaction>
    {
        public async Task Consume(ConsumeContext<IFinalizeTransaction> context)
        {
            Console.WriteLine("Consumed value: {0}", context.Message.Value);
        }
    }
}
