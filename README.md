# Tradier WebSocket Client in .NET 8.0

## Description
This project is a .NET 8.0 application designed to connect to the Tradier WebSocket API. It demonstrates how to establish a WebSocket connection, send requests, and process incoming messages from the Tradier streaming API.

## Prerequisites
- .NET 8.0 SDK
- An IDE such as Visual Studio 2022 or Visual Studio Code
- An active account with Tradier and an API token

## Installation
1. Clone the repository to your local machine:
   ```bash
   git clone https://github.com/yoshishima/TradierStream
   ```
2. Open the solution file in Visual Studio or your preferred IDE.
3. Restore NuGet packages (if not done automatically):
   ```bash
   dotnet restore
   ```

## Configuration
Before running the application, configure the necessary settings:
1. Open appsettings.json or any other configuration file you are using.
2. Replace the placeholder for the Tradier API token with your actual token:
```json
  "UserSettings": {
    "BearerToken": "xxxxxxxx"
  }
```

## Usage
Run the application from your IDE or use the following command in the terminal:****
```bash
dotnet run
```

The application will connect to the Tradier WebSocket API and start receiving streaming data based on the predefined settings.

## Features
- Establishes a secure WebSocket connection to Tradier.
- Sends requests with custom parameters.
- Handles incoming streaming data.
- Demonstrates async/await patterns for efficient network communication.

## Contributing
Contributions to the project are welcome. Please follow the standard fork-branch-pull-request workflow.

## License
This project is licensed under the GPL3 License.

## Acknowledgments
Tradier API documentation: Tradier Documentation
.NET documentation: Microsoft .NET Docs
