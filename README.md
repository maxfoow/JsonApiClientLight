# JsonApiClientLight
A simple asynchronous library for requesting Api in Json. Compatible with most of project types (Dnx, .Net, ...)

## Feature

* Support standard http method GET, POST, PUT, DELETE with playload or url parameters in Json.
* Uploading file
* Configurable retry policy
* Automaticly deserialize json with [Newtonsoft.Json]
* All overridable methods for a full customisation

## Installation
Coming soon nugget package

## Usage

Create a JsonApiClientLight class and start using methods. There are two types of method, the standards which return HttpResponseMessage and the generics ones for automatic deserialization.
For all methods you need to fill url of target and some other parameters. The GET and DELETE method the parameters are turn into [query string] while POST and PUT serialize object in content of request.

The cancellation token can be use to cancel a request or stopped current retries.

### Standard

```c#
  Task<HttpResponseMessage> GetAsync(string url, object parameters, CancellationToken? token = null);
  Task<HttpResponseMessage> PostAsync(string url, object data, CancellationToken? token = null);
  Task<HttpResponseMessage> PutAsync(string url, object data, CancellationToken? token = null);
  Task<HttpResponseMessage> DeleteAsync(string url, object parameters, CancellationToken? token = null);
```

### Auto deserialization

```c#
  Task<T> GetAsync<T>(string url, object parameters, CancellationToken? token = null);
  Task<T> PostAsync<T>(string url, object data, CancellationToken? token = null);
  Task<T> PutAsync<T>(string url, object data, CancellationToken? token = null);
  Task<T> DeleteAsync<T>(string url, object parameters, CancellationToken? token = null);
```

### Upload file
```c#
	Task<HttpResponseMessage> UploadFile(string url, Stream file, string filename = null, HttpMethod httpMethod = null, CancellationToken? token = null);
	Task<T> UploadFile<T>(string url, Stream file, string filename = null, HttpMethod httpMethod = null, CancellationToken? token = null);
```

### Retry policy
The default number for retry is 3 with a delay of 2 second * tentive number².
The retry's number and delay can be change by:

```c#
var apiClient = new JsonApiClientLight();
apiClient.RetryPolicy = 5;
apiClient.Delay = TimeSpan.FromSeconds(1);
```

The interval calcul is overridable.
```c#
public override TimeSpan Next(int currentRetry) {
  return TimeSpan.FromSeconds(Delay.TotalSeconds * Math.Pow(currentRetry, 2));
}
```

### Headers

For adding headers to every request
```c#
var apiClient = new JsonApiClientLight();
apiClient.Headers.Add("Authorization", "Bearer xxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
```

   [Newtonsoft.Json]: <http://www.newtonsoft.com/json>
   [query string]: <https://en.wikipedia.org/wiki/Query_string>  
