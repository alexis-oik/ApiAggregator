API Aggregator Documentation
================================

Overview
--------

The API Aggregator is a .NET-core application designed to aggregate data from multiple external APIs, including weather information, news articles, and Spotify playlists. This documentation provides an overview of the key components, services, and endpoints of the application.

Table of Contents
-----------------

1. Architecture
2. Endpoints
	* Aggregate Endpoint
	* Statistics Endpoint
3. Services
	* News API Service
	* OpenWeatherMap Service
	* Spotify Service
4. Error Handling
5. Statistics
6. Middleware
7. Models
8. Configuration

Architecture
------------

The API Aggregator is structured around a modular architecture, where each feature is encapsulated in its own namespace. The application uses dependency injection to manage service lifetimes and configurations, allowing for easy testing and maintenance.

Endpoints
---------

### Aggregate Endpoint

* Path: `/aggregate`
* Method: `GET`
* Parameters:
	+ `sortBy` (optional): Specifies the sorting criteria for news articles (e.g., "date", "source").
	+ `filterBy` (optional): Specifies the source to filter news articles.
* Response:
	+ `200 OK`: Returns an aggregated response containing weather data, news articles, and Spotify playlists.
	+ `503 Service Unavailable`: Returned if all services fail to provide data.
* Example: `GET /aggregate?sortBy=date&filterBy=BBC`

### Statistics Endpoint

* Path: `/statistics`
* Method: `GET`
* Response:
	+ `200 OK`: Returns a dictionary of statistics for tracked APIs.
	+ `204 No Content`: Returned if no statistics are available.
* Example: `GET /statistics`

Services
--------

### News API Service

* Interface: `INewsApiService`
* Method: `Task<Result<NewsApiResponse>> GetNewsAsync(string query)`: Fetches news articles based on the provided query.
* Error Handling: Utilizes `NewsApiErrors` for error responses.

### OpenWeatherMap Service

* Interface: `IOpenWeatherMapService`
* Method: `Task<Result<OpenWeatherMapResponse>> GetWeatherForecastAsync(string city)`: Fetches weather forecasts for the specified city.
* Error Handling: Utilizes `OpenWeatherMapErrors` for error responses.

### Spotify Service

* Interface: `ISpotifyService`
* Methods:
	+ `Task<Result<string>> GetSpotifyTokenAsync()`: Retrieves an access token for Spotify API.
	+ `Task<Result<SpotifyPlaylistResponse>> GetSpotifyPlaylistAsync()`: Fetches a Spotify playlist.
* Error Handling: Utilizes `SpotifyErrors` for error responses.

Error Handling
-------------

The application uses a centralized error handling mechanism through the `GlobalExceptionHandler`. This handler logs exceptions and returns a standardized error response preventing the exposure of sensitive data.

The application also uses a Result pattern for error handling, which encapsulates the success or failure of operations. This pattern allows for a consistent way to return results from service methods, including error information when operations fail.

Result Pattern
--------------

* `Result` Class: Represents the outcome of an operation, indicating whether it was successful and providing an optional error.
* `Result<T>` Class: Extends the `Result` class to include a data payload, allowing methods to return both success status and data.

Statistics
----------

The application tracks API usage statistics through the `StatisticService`. It records metrics such as total requests, fast requests, average requests, slow requests, and average response time.

Middleware
----------

### TimingHttpHandler

The `TimingHttpHandler` middleware tracks the time taken for each API request. It updates the statistics for the corresponding API.

Configuration
-------------

The application configuration is managed through the `IConfiguration` interface, allowing for easy access to API keys and URLs.


Conclusion
----------

This documentation provides an overview of the API Aggregator's architecture, endpoints, services, error handling, statistics tracking, middleware, and models. For further details, refer to the source code and swaggerUI.