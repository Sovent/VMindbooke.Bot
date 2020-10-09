using System.Collections.Generic;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RestSharp;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace VMindbooke.SDK
{
    public class JsonRestClient
    {
        private const int RetryCount = 5;
        private readonly RetryPolicy<IRestResponse> _retryPolicy;
        private readonly RestClient _restClient;
        private readonly Logger _logger;

        private IRestResponse SendRequest<TBody>(Method method, string resource, TBody body,
            ICollection<KeyValuePair<string, string>> queryParams = null,
            ICollection<KeyValuePair<string, string>> headers = null)
        {
            RestRequest request = new RestRequest(resource, method);
            if (body != null) request.AddParameter("application/json",
                JsonConvert.SerializeObject(body), ParameterType.RequestBody);
            if (headers != null) request.AddHeaders(headers);
            if (queryParams != null)
                foreach (KeyValuePair<string, string> param in queryParams)
                    if (param.Value != null) request.AddQueryParameter(param.Key, param.Value);
            
            _logger.Information($"Requesting {request.Resource}");
            return _retryPolicy != null
                ? _retryPolicy.Execute(() => _restClient.Execute(request))
                : _restClient.Execute(request);
        }

        private IRestResponse SendRequest(Method method, string resource,
            ICollection<KeyValuePair<string, string>> queryParams = null,
            ICollection<KeyValuePair<string, string>> headers = null) =>
            SendRequest<object>(method, resource, null, queryParams, headers);

        private TResult ParseResponse<TResult>(IRestResponse response) =>
            JsonConvert.DeserializeObject<TResult>(response.Content);

        public JsonRestClient(string baseUrl)
        {
            _restClient = new RestClient(baseUrl);
            _retryPolicy = Policy<IRestResponse>
                .HandleResult(response => !response.IsSuccessful)
                .Retry(RetryCount, (result, i) =>
                    _logger.Information($"Retried request {i} times"));

            _logger = new LoggerConfiguration()
                .WriteTo.File("vmbsdk.log", LogEventLevel.Information)
                .WriteTo.Console()
                .CreateLogger();
        }

        public void MakeRequest(Method method, string resource,
            ICollection<KeyValuePair<string, string>> queryParams = null,
            ICollection<KeyValuePair<string, string>> headers = null) =>
            SendRequest(method, resource, queryParams, headers);

        public void MakeRequest<TBody>(Method method, string resource, TBody body,
            ICollection<KeyValuePair<string, string>> queryParams = null,
            ICollection<KeyValuePair<string, string>> headers = null) =>
            SendRequest<TBody>(method, resource, body, queryParams, headers);

        public TResult MakeRequest<TResult>(Method method, string resource,
            ICollection<KeyValuePair<string, string>> queryParams = null,
            ICollection<KeyValuePair<string, string>> headers = null) =>
            ParseResponse<TResult>(SendRequest(method, resource, queryParams, headers));

        public TResult MakeRequest<TResult, TBody>(Method method, string resource, TBody body,
            ICollection<KeyValuePair<string, string>> queryParams = null,
            ICollection<KeyValuePair<string, string>> headers = null) =>
            ParseResponse<TResult>(SendRequest<TBody>(method, resource, body, queryParams, headers));
    }
}
