using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;

namespace VMindbooke.SDK
{
    public class JsonRestClient
    {
        private readonly RestClient _restClient;
        
        private IRestResponse SendRequest<TBody>(Method method, string resource,
            TBody body, ICollection<KeyValuePair<string, string>> headers = null)
        {
            RestRequest request = new RestRequest(resource, method);
            if (body != null) request.AddJsonBody(JsonConvert.SerializeObject(body));
            if (headers != null) request.AddHeaders(headers);
            return _restClient.Execute(request);
        }

        private IRestResponse SendRequest(Method method, string resource,
            ICollection<KeyValuePair<string, string>> headers = null) =>
            SendRequest<object>(method, resource, null, headers);

        private TResult ParseResponse<TResult>(IRestResponse response) =>
            JsonConvert.DeserializeObject<TResult>(response.Content);

        public JsonRestClient(string baseUrl)
        {
            _restClient = new RestClient(baseUrl);
        }

        public void MakeJsonRequest(Method method, string resource,
            ICollection<KeyValuePair<string, string>> headers = null) =>
            SendRequest(method, resource, headers);

        public void MakeJsonRequest<TBody>(Method method, string resource,
            TBody body, ICollection<KeyValuePair<string, string>> headers = null) =>
            SendRequest<TBody>(method, resource, body, headers);

        public TResult MakeJsonRequest<TResult>(Method method, string resource,
            ICollection<KeyValuePair<string, string>> headers = null) =>
            ParseResponse<TResult>(SendRequest(method, resource, headers));

        public TResult MakeJsonRequest<TResult, TBody>(Method method, string resource,
            TBody body, ICollection<KeyValuePair<string, string>> headers = null) =>
            ParseResponse<TResult>(SendRequest<TBody>(method, resource, body, headers));
    }
}
