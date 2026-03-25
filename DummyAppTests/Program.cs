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

if (ShouldUseCodeConfiguration(args, out var codeExecutionMode))
{
    if (codeExecutionMode == CodeExecutionMode.Template)
    {
        RenderCodeTemplate();
        return;
    }

    var runner = Bootstrap.New(BuildCodeBootstrapArguments());
    if (runner.ExecutionBuilders.Count > 0)
        ConfigureExecution(runner.ExecutionBuilders.Single());

    runner.Run();
    return;
}

Bootstrap.New(args).Run();

return;

static bool ShouldUseCodeConfiguration(string[] args, out CodeExecutionMode codeExecutionMode)
{
    if (args.Any(IsHelpOrVersionOption))
    {
        codeExecutionMode = default;
        return false;
    }

    if (args.Length == 0)
    {
        codeExecutionMode = CodeExecutionMode.Run;
        return true;
    }

    if (args[0].Equals("template", StringComparison.OrdinalIgnoreCase) && !HasExplicitTemplateConfigurationPath(args))
    {
        codeExecutionMode = CodeExecutionMode.Template;
        return true;
    }

    codeExecutionMode = default;
    return false;
}

static string[] BuildCodeBootstrapArguments()
{
    return ["run", EnsureCodeBootstrapFile()];
}

static string EnsureCodeBootstrapFile()
{
    var path = Path.Combine(AppContext.BaseDirectory, "code-bootstrap.qaas.yaml");
    if (!File.Exists(path))
        File.WriteAllText(path, string.Empty);

    return path;
}

static void RenderCodeTemplate()
{
    Console.WriteLine(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "test.qaas.yaml")));
}

static bool HasExplicitTemplateConfigurationPath(IReadOnlyList<string> args)
{
    return args.Count > 1 && !args[1].StartsWith("-", StringComparison.Ordinal);
}

static bool IsHelpOrVersionOption(string argument)
{
    return argument.Equals("--help", StringComparison.OrdinalIgnoreCase) ||
           argument.Equals("-h", StringComparison.OrdinalIgnoreCase) ||
           argument.Equals("--version", StringComparison.OrdinalIgnoreCase);
}

static void ConfigureExecution(ExecutionBuilder executionBuilder)
{
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
}

enum CodeExecutionMode
{
    Run,
    Template
}
