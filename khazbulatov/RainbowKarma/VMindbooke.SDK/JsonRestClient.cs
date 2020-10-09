using System.Collections.Generic;
using Polly;
using Polly.Retry;
using RestSharp;
using Serilog;

namespace VMindbooke.SDK
{
    public class JsonRestClient
    {
        private const int RetryCount = 5;
        private readonly RestClient _restClient;

        private IRestRequest PrepareRequest<TBody>(Method method, string resource, TBody body,
            ICollection<KeyValuePair<string, string>> queryParams = null,
            ICollection<KeyValuePair<string, string>> headers = null)
        {
            RestRequest request = new RestRequest(resource, method);
            if (body != null) request.AddJsonBody(body);
            if (headers != null) request.AddHeaders(headers);
            if (queryParams != null)
                foreach (KeyValuePair<string, string> param in queryParams)
                    if (param.Value != null) request.AddQueryParameter(param.Key, param.Value);
            return request;
        }

        private IRestRequest PrepareRequest(Method method, string resource,
            ICollection<KeyValuePair<string, string>> queryParams = null,
            ICollection<KeyValuePair<string, string>> headers = null) => 
            PrepareRequest<object>(method, resource, null, queryParams, headers);

        private TResult ExecuteRequest<TResult>(IRestRequest request)
        {
            RetryPolicy<IRestResponse<TResult>> retryPolicy = Policy<IRestResponse<TResult>>
                .HandleResult(response => !response.IsSuccessful)
                .Retry(RetryCount, (result, i) =>
                    Log.Information($"Retried request {i} times"));
            
            Log.Information($"Requesting {request.Method} {request.Resource} "
                            + $"{string.Join("&", request.Parameters)}");
            return retryPolicy.Execute(() => _restClient.Execute<TResult>(request)).Data;
        }

        private void ExecuteRequest(IRestRequest request)
        {
            RetryPolicy<IRestResponse> retryPolicy = Policy<IRestResponse>
                .HandleResult(response => !response.IsSuccessful)
                .Retry(RetryCount, (result, i) =>
                    Log.Information($"Retried request {i} times"));
            
            Log.Information($"Requesting {request.Method} {request.Resource} "
                            + $"{string.Join("&", request.Parameters)}");
            retryPolicy.Execute(() => _restClient.Execute(request));
        }

        public JsonRestClient(string baseUrl) => _restClient = new RestClient(baseUrl);

        public void MakeRequest(Method method, string resource,
            ICollection<KeyValuePair<string, string>> queryParams = null,
            ICollection<KeyValuePair<string, string>> headers = null) =>
            ExecuteRequest(PrepareRequest(method, resource, queryParams, headers));

        public void MakeRequest<TBody>(Method method, string resource, TBody body,
            ICollection<KeyValuePair<string, string>> queryParams = null,
            ICollection<KeyValuePair<string, string>> headers = null) =>
            ExecuteRequest(PrepareRequest<TBody>(method, resource, body, queryParams, headers));

        public TResult MakeRequest<TResult>(Method method, string resource,
            ICollection<KeyValuePair<string, string>> queryParams = null,
            ICollection<KeyValuePair<string, string>> headers = null) =>
            ExecuteRequest<TResult>(PrepareRequest(method, resource, queryParams, headers));

        public TResult MakeRequest<TResult, TBody>(Method method, string resource, TBody body,
            ICollection<KeyValuePair<string, string>> queryParams = null,
            ICollection<KeyValuePair<string, string>> headers = null) =>
            ExecuteRequest<TResult>(PrepareRequest<TBody>(method, resource, body, queryParams, headers));
    }
}
