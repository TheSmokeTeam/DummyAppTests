# DummyAppTests

Sample QaaS Runner project for the YAML quick start.

## What It Does

- loads one request payload from `DummyAppTests/Requests/request.json`
- sends `GET http://127.0.0.1:8080/data`
- asserts that the response status is `200` using `HttpStatus`
- uses the latest public `QaaS.Runner`, `QaaS.Common.Assertions`, and `QaaS.Common.Generators` packages from `nuget.org`

## Run

Start the matching `DummyAppMock` sample first, then run:

```bash
dotnet restore
dotnet run --project DummyAppTests/DummyAppTests.csproj -- run test.qaas.yaml
```
