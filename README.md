# DummyAppTests

Sample QaaS Runner project for the code quick start.

## What It Does

- builds the execution directly in `DummyAppTests/Program.cs`
- loads one request payload from `DummyAppTests/TestData/input.json`
- publishes the payload to RabbitMQ `input` and consumes it from RabbitMQ `output`
- validates hermeticity and delay with `HermeticByInputOutputPercentage` and `DelayByChunks`

## Run

```bash
dotnet restore
dotnet run --project DummyAppTests/DummyAppTests.csproj
```
