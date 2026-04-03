using QaaS.Framework.SDK;
using QaaS.Framework.SDK.Extensions;
using QaaS.Runner;

var bootstrapArguments = args.Length > 0 ? args : ["run", "test.qaas.yaml"];
var runner = Bootstrap.New(bootstrapArguments);
runner.ExecutionBuilders.AsSingle().WithMetadata(new MetaDataConfig
{
    Team = "Smoke",
    System = "DummyApp"
});
runner.Run();
