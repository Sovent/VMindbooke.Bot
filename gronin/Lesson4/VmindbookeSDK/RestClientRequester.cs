using System.Collections.Generic;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RestSharp;
using Serilog;
using Serilog.Core;

namespace VmindbookeSDK
{
    public class RestClientRequester
    {
        private readonly RestClient _restClient;
        private readonly RetryPolicy<IRestResponse> _retryPolicy;

        public RestClientRequester(string baseUrl)
        {
            _restClient = new RestClient(baseUrl);
            _retryPolicy = Policy.HandleResult<IRestResponse>
                    (r => !r.IsSuccessful)
                .Retry(10, (result, span) => Log.Logger.Information("Retry"));
        }
        
        public IRestResponse SendRequest<TBody>(
            Method method,
            string resource,
            TBody body,
            ICollection<KeyValuePair<string, string>> queryParameters = null,
            ICollection<KeyValuePair<string, string>> headers = null)
        {
            RestRequest request = new RestRequest(resource, method);
            
            if (body != null) 
                request.AddJsonBody(body);
            if (headers != null) 
                request.AddHeaders(headers);
            if (queryParameters != null)
            {
                foreach (var param in queryParameters)
                    if (param.Value != null) 
                        request.AddQueryParameter(param.Key, param.Value);
            }
            
            var response = _retryPolicy.Execute(()=> _restClient.Execute(request));
            
            return response;
        }

        public IRestResponse SendRequest(
            Method method,
            string resource,
            ICollection<KeyValuePair<string, string>> queryParams = null,
            ICollection<KeyValuePair<string, string>> headers = null) =>
            SendRequest<object>(method, resource, null, queryParams, headers);

        private TResult ParseResponse<TResult>(IRestResponse response) =>
            JsonConvert.DeserializeObject<TResult>(response.Content);
        
        public TResult SendRequestParseResult<TResult>(Method method, string resource,
            ICollection<KeyValuePair<string, string>> queryParams = null,
            ICollection<KeyValuePair<string, string>> headers = null) =>
            ParseResponse<TResult>(SendRequest(method, resource, queryParams, headers));

        public TResult SendRequestParseResult<TResult, TBody>(Method method, string resource, TBody body,
            ICollection<KeyValuePair<string, string>> queryParams = null,
            ICollection<KeyValuePair<string, string>> headers = null) =>
            ParseResponse<TResult>(SendRequest<TBody>(method, resource, body, queryParams, headers));
    }
}
