
using Kafka_producer_energy_price;
namespace Kafka_producer_energy_price
{
    class main
    {
        static async Task Main()
        {
            await EventProcessor.HandleEvents();
        }
    }
}