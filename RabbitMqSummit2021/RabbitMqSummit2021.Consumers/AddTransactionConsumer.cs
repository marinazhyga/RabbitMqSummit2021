using MassTransit;
using RabbitMqSummit2021.MessageContracts;
using System;
using System.Threading.Tasks;

namespace RabbitMqSummit2021.Consumers
{
    public class AddTransactionConsumer : IConsumer<IAddTransaction>
    {
        public async Task Consume(ConsumeContext<IAddTransaction> context)
        {
            Console.WriteLine("Consumed value: {0}", context.Message.Value);
        }
    }
}
