# DummyAppTests

Sample QaaS Runner project for the code quick start.

## What It Does

- keeps `DummyAppTests/Program.cs` as the whole code configuration
- keeps an empty `DummyAppTests/test.qaas.yaml` only so `Bootstrap.New(...)` can expose one execution builder
- loads one request payload from `DummyAppTests/TestData/input.json`
- publishes the payload to RabbitMQ `dummy-app-tests-input` and consumes it from `dummy-app-tests-output`
- validates hermeticity and delay with `HermeticByInputOutputPercentage` and `DelayByChunks`

## Run

```bash
dotnet restore
cd DummyAppTests
dotnet run
```

The sample expects a local RabbitMQ broker on `127.0.0.1:5672` and a component that relays the published message from `input` to `output`, the same way the quick-start CI smoke test does.
