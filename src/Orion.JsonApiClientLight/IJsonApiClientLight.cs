using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Orion.JsonApiClientLight {
	public interface IJsonApiClientLight {
		Dictionary<string, string> Headers { get; }
		Task<HttpResponseMessage> GetAsync( string url,  object parameters, CancellationToken? token = null);
		Task<HttpResponseMessage> PostAsync( string url,  object data, CancellationToken? token = null);
		Task<HttpResponseMessage> PutAsync( string url,  object data, CancellationToken? token = null);
		Task<HttpResponseMessage> DeleteAsync( string url,  object data, CancellationToken? token = null);
		Task<T> GetAsync<T>( string url,  object parameters, CancellationToken? token = null);
		Task<T> PostAsync<T>( string url,  object data, CancellationToken? token = null);
		Task<T> PutAsync<T>( string url,  object data, CancellationToken? token = null);
		Task<T> DeleteAsync<T>( string url,  object data, CancellationToken? token = null);
	}
}