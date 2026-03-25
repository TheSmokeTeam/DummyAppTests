# DummyAppTests

Sample QaaS Runner project for the YAML quick start.

## What It Does

- loads one request payload from `DummyAppTests/TestData/input.json`
- publishes the payload to RabbitMQ `input` and consumes it from RabbitMQ `output`
- validates hermeticity and delay with `HermeticByInputOutputPercentage` and `DelayByChunks`

## Run

```bash
dotnet restore
cd DummyAppTests
dotnet run -- run test.qaas.yaml
```

The sample expects a local RabbitMQ broker on `127.0.0.1:5672` and a component that relays the published message from `input` to `output`, the same way the quick-start CI smoke test does.
