using QaaS.Runner;

var bootstrapArguments = args.Length > 0 ? args : ["run", "test.qaas.yaml"];
var runner = Bootstrap.New(bootstrapArguments);
runner.Run();
