# DummyAppTests

Sample QaaS Runner project for the code quick start.

## What It Does

- runs the code-defined execution in `DummyAppTests/Program.cs` when no program arguments are passed
- runs the checked-in `DummyAppTests/test.qaas.yaml` when you pass `run`
- renders the code-defined execution as YAML when you pass `template` without a file path
- loads one request payload from `DummyAppTests/TestData/input.json`
- publishes the payload to RabbitMQ `input` and consumes it from RabbitMQ `output`
- validates hermeticity and delay with `HermeticByInputOutputPercentage` and `DelayByChunks`

## Run

```bash
dotnet restore
cd DummyAppTests
dotnet run
```

The sample expects a local RabbitMQ broker on `127.0.0.1:5672` and a component that relays the published message from `input` to `output`, the same way the quick-start CI smoke test does.

## YAML Path

```bash
dotnet run -- run test.qaas.yaml
```

## Template Check

```bash
dotnet run -- template
```
