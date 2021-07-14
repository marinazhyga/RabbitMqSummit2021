using MassTransit;
using RabbitMqSummit2021.MessageContracts;
using System;
using System.Threading.Tasks;

namespace RabbitMqSummit2021.Consumers
{
    public class RemoveTransactionConsumer : IConsumer<IRemoveTransaction>
    {
        public async Task Consume(ConsumeContext<IRemoveTransaction> context)
        {
            Console.WriteLine("Consumed value: {0}", context.Message.Value);
        }
    }
}
