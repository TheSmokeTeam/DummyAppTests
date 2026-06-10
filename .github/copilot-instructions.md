# Copilot instructions — DummyAppTests

Read `AGENTS.md` at the repo root first — it explains the end-to-end RabbitMQ test scenario and its environment prerequisites.

Essentials:
- Run: `cd DummyAppTests && dotnet run -- run test.qaas.yaml` — REQUIRES RabbitMQ at 127.0.0.1:5672 (admin/admin) with input→output exchanges bridged.
- Consumer timeout 5000ms: failures without live RabbitMQ are environmental, not code bugs.
- `test.qaas.yaml` sections and hook names (HermeticByInputOutputPercentage, DelayByChunks, FromFileSystem) are exact contracts with discovered classes.
- Consumes latest public QaaS packages; pin versions to bisect upstream breaks.
- Reference consumer: validate Runner/Framework changes against this scenario.
