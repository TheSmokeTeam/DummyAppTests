using System.Reflection;

Directory.SetCurrentDirectory(AppContext.BaseDirectory);
Assembly.Load("QaaS.Common.Assertions");
Assembly.Load("QaaS.Common.Generators");
QaaS.Runner.Bootstrap.New(args).Run();
