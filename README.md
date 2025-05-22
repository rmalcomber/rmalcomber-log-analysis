# LivenerTechTest - Log Analysis API

A .NET web API application for parsing and analyzing log files.

## Project Overview

This application provides an API for analyzing log files with the following features:

- Parsing log entries from a specified log file
- Aggregating log data including error counts and user activity
- Exposing the aggregated data through a REST API

## Prerequisites

- .NET 9.0 SDK or later
- A log file for analysis (default: `webrtc_studio.log` in the application root directory)

## Project Structure

- **LivenerTechTest**: Main application project
  - **parsers**: Contains classes for parsing log files
  - **providers**: Contains data provider implementations
  - **types**: Contains data models and DTOs
  - **interfaces**: Contains interface definitions

- **Tests**: Test project with unit tests for the application components

## Running the Application

### From Command Line

1. Navigate to the project directory:
   ```bash
   cd /path/to/LivenerTechTest
   ```

2. Run the application:
   ```bash
   cd LivenerTechTest
   dotnet run
   ```

3. Or specify the launch profile explicitly:
   ```bash
   cd LivenerTechTest
   dotnet run --launch-profile http
   ```

The application will start and listen on http://localhost:5217.

### Configuration

The application uses a log file specified in the `appsettings.json` file. By default, it's set to `webrtc_studio.log` in the application root directory.

To use a different log file, modify the `FileDataStore` setting in `appsettings.json`:

```json
{
  "FileDataStore": "path/to/your/logfile.log"
}
```

## API Endpoints

### Log Analysis

- **URL**: `/loganalysis`
- **Method**: GET
- **Description**: Returns aggregated log data including unique user count, user activity, and error statistics
- **Response Format**:
  ```json
  {
    "uniqueUsers": 3,
    "userActivity": [
      {
        "userId": "user123",
        "event": "INFO",
        "timestamp": "May 21, 2023"
      },
      ...
    ],
    "errors": {
      "ERROR": 2,
      "CRITICAL": 1,
      "WARNING": 1
    }
  }
  ```

## Running Tests

To run the unit tests:

```bash
cd /path/to/LivenerTechTest
dotnet test
```
