using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kafka_producer_energy_price
{
    internal class EventProcessor
    {
        static ConsumerConfig config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "Group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        public static async void HandleEvents()
        {
            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe("RequestPrice");

                while (true)
                {
                    var consumeResult = consumer.Consume();

                    string? json = JsonCache.Get();
                    string? correl_id;
                    string? reply_to;

                    try
                    {
                        correl_id = Encoding.ASCII.GetString(consumeResult.Message.Headers.GetLastBytes("message-id"));
                        reply_to = Encoding.ASCII.GetString(consumeResult.Message.Headers.GetLastBytes("reply-to"));
                    } catch
                    {
                        correl_id = "1";
                        reply_to = "ReplyPrice";
                    }

                    if (json == null)
                    {
                        json = await PriceDownloader.GetTodaysPricesJsonAsync();
                        JsonCache.Put(json);
                    }

                    PriceParser parser = new(json);

                    DateTime thisHour = DateOnly.FromDateTime(DateTime.Now).ToDateTime(new(DateTime.Now.Hour, 00));
                    var price = parser.GetWestPrice(thisHour);
                    Producer.KafkaProducer(price.GetValueOrDefault(), reply_to, correl_id);
                }

                consumer.Close();
            }
        }
    }
}
