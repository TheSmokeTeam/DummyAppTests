# DummyAppTests

Sample QaaS Runner project for the YAML quick start.

## What It Does

- loads one request payload from `DummyAppTests/TestData/input.json`
- publishes the payload to RabbitMQ `input` and consumes it from RabbitMQ `output`
- validates hermeticity and delay with `HermeticByInputOutputPercentage` and `DelayByChunks`

## Run

```bash
dotnet restore
dotnet run --project DummyAppTests/DummyAppTests.csproj -- run test.qaas.yaml
```
