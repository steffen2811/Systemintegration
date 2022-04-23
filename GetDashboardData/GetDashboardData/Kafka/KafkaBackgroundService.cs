using Confluent.Kafka;
using GetDashboardData.Kafka;
using GetDashboardData.Models;
using System.Text;
using System.Text.Json;

namespace GetDashboardData.backend
{

    public class KafkaBackgroundService : BackgroundService
    {
        private readonly string topic = "ReplyPrice";
        private readonly string groupId = "Group";
        private readonly string bootstrapServers = "localhost:9092";
        private static EnergyPrice? energyPrice;

        protected override async Task ExecuteAsync(CancellationToken stopToken)
        {
            await Task.Yield();

            var config = new ConsumerConfig
            {
                GroupId = groupId,
                BootstrapServers = bootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            try
            {
                using (var consumerBuilder = new ConsumerBuilder
                <Ignore, string>(config).Build())
                {
                    consumerBuilder.Subscribe(topic);
                    var cancelToken = new CancellationTokenSource();

                    try
                    {
                        while (!stopToken.IsCancellationRequested)
                        {
                            var consumer = consumerBuilder.Consume(cancelToken.Token);
                            var correl_id = Encoding.ASCII.GetString(consumer.Message.Headers.GetLastBytes("correlation-identifier"));

                            if (ManageCorrelationIdentifiers.CheckCorrelationIdentifier(correl_id))
                            {
                                TimedBackgroundService.SetLastHourUpdateKafka(int.Parse(DateTime.Now.ToString("HH")));

                                energyPrice = JsonSerializer.Deserialize<EnergyPrice>(consumer.Message.Value);
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        consumerBuilder.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public static EnergyPrice GetEnergyPrice()
        {
            if (energyPrice == null)
                return new EnergyPrice(0, null);
            else
                return energyPrice;
        }
    }
}