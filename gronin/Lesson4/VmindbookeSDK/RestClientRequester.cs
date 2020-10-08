using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;

namespace VmindbookeSDK
{
    public class RestClientRequester
    {
        private readonly RestClient _restClient;
        
        public RestClientRequester(string baseUrl)
        {
            _restClient = new RestClient(baseUrl);
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
            return _restClient.Execute(request);
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
