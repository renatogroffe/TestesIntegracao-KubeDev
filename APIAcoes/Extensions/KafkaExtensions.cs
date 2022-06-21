using Confluent.Kafka;

namespace APIAcoes.Extensions;

public static class KafkaExtensions
{
    public static IProducer<Null, string> CreateProducer(
        IConfiguration configuration)
    {
        if (!String.IsNullOrWhiteSpace(configuration["ApacheKafka:Password"]))
            return new ProducerBuilder<Null, string>(
                new ProducerConfig()
                {
                    BootstrapServers = configuration["ApacheKafka:Broker"],
                    SecurityProtocol = SecurityProtocol.SaslSsl,
                    SaslMechanism = SaslMechanism.Plain,
                    SaslUsername = configuration["ApacheKafka:Username"],
                    SaslPassword = configuration["ApacheKafka:Password"]
                }).Build();
        else
            return new ProducerBuilder<Null, string>(
                new ProducerConfig()
                {
                    BootstrapServers = configuration["ApacheKafka:Broker"]
                }).Build();
    }
}