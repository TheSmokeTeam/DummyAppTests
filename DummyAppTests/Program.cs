var runner = QaaS.Runner.Bootstrap.New(args);
var configurator = new DummyAppTests.RunnerExecutionBuilderConfigurator();

foreach (var executionBuilder in runner.ExecutionBuilders)
{
    configurator.Configure(executionBuilder);
}

runner.Run();
