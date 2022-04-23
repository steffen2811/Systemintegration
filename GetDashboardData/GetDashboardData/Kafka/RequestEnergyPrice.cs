using Confluent.Kafka;
using System.Net;
using System.Text;

namespace GetDashboardData.Kafka
{
    public class RequestEnergyPrice
    {
        private static ProducerConfig config = new ProducerConfig
        {
            BootstrapServers = "localhost:9092",
            ClientId = Dns.GetHostName(),
        };

        public static void KafkaRequestEnergyPrice()
        {
            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                var headers = new Headers();
                var message_id = Guid.NewGuid().ToString("N");
                var timestamp = DateTime.Now.Ticks;

                ManageCorrelationIdentifiers.AddCorrelationIdentifier(message_id, timestamp);

                headers.Add("reply-to", Encoding.ASCII.GetBytes("ReplyPrice"));
                headers.Add("message-id", Encoding.ASCII.GetBytes(message_id));
                headers.Add("timestamp", BitConverter.GetBytes(timestamp));
                producer.ProduceAsync("RequestPrice", new Message<Null, string> { Headers = headers });
                producer.Flush();
            }
        }
    }
}
