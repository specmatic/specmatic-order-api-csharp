# Specmatic Sample Client Application

* [Specmatic Website](https://specmatic.io)
* [Specmatic Documentation](https://docs.specmatic.io)

![HTML client talks to client API which talks to backend api](specmatic-sample-architecture.svg)

BFF = Backend For Frontend, the API invoked by the HTTP calls in the client HTML page (Website UI).

This project contains the product API, which is used by a small ecommerce client application.

Here is the [contract](https://github.com/specmatic/specmatic-order-contracts/blob/main/io/specmatic/examples/store/openapi/api_order_v3.yaml) governing the interaction of the client with the product API.

The architecture diagram was created using the amazing free online SVG editor at [Vectr](https://vectr.com).

## Tech
1. .NET core 10
2. Specmatic
3. Docker Desktop

### Prerequisites

1. Dotnet SDK
2. Docker Desktop

### How to run the application?

1. CD into the project directory : `cd specmatic-order-api-csharp`
2. Build the project using : `dotnet build`
3. Run the application using : `dotnet run`



### How to test the application?
1. Using `dotnet test`

2. Run the test inside container
```docker run --rm \
  -v "$(pwd)/specmatic-order-api-csharp.test/specmatic.yaml:/usr/src/app/specmatic.yaml" \
  -v "$(pwd)/build/reports/specmatic:/usr/src/app/build/reports/specmatic" \
  specmatic/specmatic test \
  --host=host.docker.internal \
  --port=8090
```