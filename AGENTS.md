# AGENTS.md — DummyAppTests

Guidance for AI agents working in this repository.

## What this repo is

The **reference QaaS.Runner testbed**: a complete YAML-defined test that publishes `TestData/input.json` to RabbitMQ (`dummy-app-tests-input`), consumes from `dummy-app-tests-output`, and asserts hermeticity (`HermeticByInputOutputPercentage` = 100%) plus latency (`DelayByChunks`, chunk size 1). It exercises the whole ecosystem — Runner, Framework, Common.Generators, Common.Assertions — and is the end-to-end canary for upstream changes.

## Layout

| Path | Purpose |
|---|---|
| `DummyAppTests/test.qaas.yaml` | full test: MetaData / DataSources / Sessions (RabbitMQ publisher+consumer) / Assertions |
| `DummyAppTests/TestData/input.json` | the input payload (1 record) |
| `DummyAppTests/Program.cs` | Runner bootstrap |
| `NuGet.config` | feed configuration |

## Run

```powershell
# prerequisite: RabbitMQ on 127.0.0.1:5672 (admin/admin) with the exchanges wired input→output
dotnet restore
dotnet run --project DummyAppTests -- run test.qaas.yaml
```

## Critical gotchas

- **Requires live RabbitMQ** (127.0.0.1:5672, admin/admin) and something bridging `dummy-app-tests-input` → `dummy-app-tests-output` (the system under test). Without it the consumer times out (5000ms) and assertions fail — that's an environment failure, not a code bug.
- YAML section names (MetaData/Variables/Storages/DataSources/Sessions/Assertions/Links) and hook names (`HermeticByInputOutputPercentage`, `DelayByChunks`, `Generator: FromFileSystem`, `Deserializer: Json`) are exact contracts with discovered classes.
- Consumes latest public QaaS packages from nuget.org — breaking upstream releases surface here first; pin versions to bisect.
- Local checkouts may sit on the `yaml_configuration` branch — verify which branch you're comparing against.
- Publisher uses a LoadBalance policy (Rate: 50) — changing it alters delay-assertion expectations.

## Process

Testbed discipline: keep the scenario minimal and readable — it doubles as documentation. Validate any QaaS package bump by running the full scenario against local RabbitMQ. Conventional commits.
