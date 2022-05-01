﻿using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Kafka_producer_energy_price
{
    internal class Producer
    {
        private static ProducerConfig config = new ProducerConfig
        {
            BootstrapServers = "localhost:9092",
            ClientId = Dns.GetHostName(),
        };

        public static void KafkaProducer(string data, string reply_to, string correl_id)
        {
            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                var headers = new Headers();
                headers.Add("correlation-identifier", Encoding.ASCII.GetBytes(correl_id) );
                headers.Add("message-id", Encoding.ASCII.GetBytes(Guid.NewGuid().ToString("N")));
                headers.Add("timestamp", BitConverter.GetBytes(DateTime.Now.Ticks));
                producer.ProduceAsync(reply_to, new Message<Null, string> { Value = data, Headers = headers });
                producer.Flush();
            }
        }
        public static void KafkaInvalidMessage(string message_id)
        {
            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                var headers = new Headers();
                headers.Add("message-id", Encoding.ASCII.GetBytes(message_id));
                headers.Add("timestamp", BitConverter.GetBytes(DateTime.Now.Ticks));
                producer.ProduceAsync("InvalidPriceRequests", new Message<Null, string> { Headers = headers });
                producer.Flush();
            }
        }
    }
}
