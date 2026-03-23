using QaaS.Common.Assertions.CommonAssertionsConfigs.Delay;
using QaaS.Common.Assertions.CommonAssertionsConfigs.Hermetic;
using QaaS.Common.Assertions.Delay;
using QaaS.Common.Assertions.Hermetic;
using QaaS.Common.Generators.ConfigurationObjects.FromExternalSourceConfigurations;
using QaaS.Common.Generators.FromExternalSourceGenerators;
using QaaS.Framework.Policies;
using QaaS.Framework.Policies.ConfigurationObjects;
using QaaS.Framework.Protocols.ConfigurationObjects.RabbitMq;
using QaaS.Framework.SDK;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.Serialization;
using QaaS.Runner;
using QaaS.Runner.Assertions.ConfigurationObjects;
using QaaS.Runner.Sessions.Actions.Consumers.Builders;
using QaaS.Runner.Sessions.Actions.Publishers.Builders;
using QaaS.Runner.Sessions.Session.Builders;

var runner = Bootstrap.New(args);
var executionBuilder = runner.ExecutionBuilders.Single();

var dataSource = new DataSourceBuilder()
    .Named("FromFileSystemTestData")
    .HookNamed(nameof(FromFileSystem))
    .Configure(new FromFileSystemConfig
    {
        DataArrangeOrder = DataArrangeOrder.AsciiAsc,
        FileSystem = new FileSystemConfig
        {
            Path = Path.Combine(AppContext.BaseDirectory, "TestData")
        }
    });

var rabbitMqConfiguration = new BaseRabbitMqConfig
{
    Host = "127.0.0.1",
    Username = "admin",
    Password = "admin",
    VirtualHost = "/",
    Port = 5672
};

var publisher = new PublisherBuilder()
    .Named("Publisher")
    .AddDataSource("FromFileSystemTestData")
    .AddPolicy(new PolicyBuilder().Configure(new LoadBalancePolicyConfig
    {
        Rate = 50
    }))
    .Configure(new RabbitMqSenderConfig
    {
        Host = rabbitMqConfiguration.Host,
        Username = rabbitMqConfiguration.Username,
        Password = rabbitMqConfiguration.Password,
        Port = rabbitMqConfiguration.Port,
        ExchangeName = "input",
        RoutingKey = "/"
    });

var consumer = new ConsumerBuilder()
    .Named("Consumer")
    .WithTimeout(5000)
    .Configure(new RabbitMqReaderConfig
    {
        Host = rabbitMqConfiguration.Host,
        Username = rabbitMqConfiguration.Username,
        Password = rabbitMqConfiguration.Password,
        Port = rabbitMqConfiguration.Port,
        ExchangeName = "output",
        RoutingKey = "/"
    })
    .WithDeserializer(new DeserializeConfig
    {
        Deserializer = SerializationType.Json
    });

var session = new SessionBuilder()
    .Named("RabbitMqExchangeWithFromFileSystemTestData")
    .AddPublisher(publisher)
    .AddConsumer(consumer);

var hermeticAssertion = new AssertionBuilder
    {
        AssertionInstance = null!,
        Reporter = null!
    }
    .Named("HermeticByInputOutputPercentage")
    .HookNamed(nameof(HermeticByInputOutputPercentage))
    .AddSessionName(session.Name!)
    .Configure(new HermeticByInputOutputPercentageConfiguration
    {
        OutputNames = [consumer.Name!],
        InputNames = [publisher.Name!],
        ExpectedPercentage = 100
    });

var delayAssertion = new AssertionBuilder
    {
        AssertionInstance = null!,
        Reporter = null!
    }
    .Named("DelayByChunks")
    .HookNamed(nameof(DelayByChunks))
    .AddSessionName(session.Name!)
    .Configure(new DelayByChunksConfiguration
    {
        Output = new Chunk
        {
            Name = consumer.Name!,
            ChunkSize = 1
        },
        Input = new Chunk
        {
            Name = publisher.Name!,
            ChunkSize = 1
        },
        MaximumDelayMs = 5000
    });

executionBuilder
    .WithMetadata(new MetaDataConfig
    {
        Team = "Smoke",
        System = "DummyApp"
    })
    .AddDataSource(dataSource)
    .AddSession(session)
    .AddAssertion(hermeticAssertion)
    .AddAssertion(delayAssertion);

runner.Run();
