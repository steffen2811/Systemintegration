using Confluent.Kafka;
using Kafka_producer_energy_price.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

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

        public static async Task HandleEvents()
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
                    long? timestamp;
                    double? price;

                    try
                    {
                        correl_id = Encoding.ASCII.GetString(consumeResult.Message.Headers.GetLastBytes("message-id"));
                        reply_to = Encoding.ASCII.GetString(consumeResult.Message.Headers.GetLastBytes("reply-to"));
                        timestamp = BitConverter.ToInt64(consumeResult.Message.Headers.GetLastBytes("timestamp"));
                    }
                    catch (Exception ex)
                    {
                        // Problem with header. Forward to invalid message channel.
                        Producer.KafkaInvalidMessage(Encoding.ASCII.GetString(consumeResult.Message.Headers.GetLastBytes("message-id")));
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                        continue;
                    }

                    try
                    {
                        DateTime timestampDate = new DateTime((long)timestamp);
                        if (timestampDate < DateTime.Now.AddMinutes(-1))
                        {
                            // Message is older than 1 minut. Skip.
                            continue;
                        }

                        if (json == null)
                        {
                            json = await PriceDownloader.GetTodaysPricesJsonAsync();
                            JsonCache.Put(json);
                        }

                        PriceParser parser = new(json);

                        DateTime thisHour = DateOnly.FromDateTime(DateTime.Now).ToDateTime(new(DateTime.Now.Hour, 00));
                        price = parser.GetWestPrice(thisHour);
                        var data = JsonSerializer.Serialize(new EnergyPrice { energyPrice = price, updated = DateTime.Now });
                        Producer.KafkaProducer(data, reply_to, correl_id);
                    }
                    catch (Exception ex)
                    {
                        // Application error
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
                    
                }

                consumer.Close();
            }
        }
    }
}
