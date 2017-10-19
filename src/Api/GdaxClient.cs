using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Boukenken.Gdax
{
    public abstract class GdaxClient
    {
        private readonly RequestAuthenticator _authenticator;
        private readonly HttpClient _httpClient;

        public GdaxClient(string baseUrl, RequestAuthenticator authenticator)
        {
            _authenticator = authenticator;
            _httpClient = new HttpClient();
            BaseUri = new Uri(baseUrl, UriKind.Absolute);
        }

        public Uri BaseUri { get; }

        public async Task<ApiResponse<TResponse>> GetResponseAsync<TResponse>(ApiRequest request)
        {
            var httpResponse = await GetResponseAsync(request);
            var contentBody = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            return new ApiResponse<TResponse>(
                httpResponse.Headers.ToArray(),
                httpResponse.StatusCode,
                contentBody,
                Deserialize<TResponse>(contentBody)
            );
        }

        public async Task<ApiResponse<TResponse>> GetResponseArrayAsync<TResponse>(ApiRequest request)
        {
            var httpResponse = await GetResponseAsync(request);
            var contentBody = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            return new ApiResponse<TResponse>(
                httpResponse.Headers.ToArray(),
                httpResponse.StatusCode,
                contentBody,
                DeserializeArray<TResponse>(contentBody)
            );
        }

        public async Task<HttpResponseMessage> GetResponseAsync(ApiRequest request)
        {
            var httpRequest = BuildRequestMessagee(request);
            httpRequest.Headers.Add("User-Agent", "GdaxClient (+https://sefbkn.github.io/)");
            var response = await _httpClient.SendAsync(httpRequest).ConfigureAwait(false);
            return response;
        }

        private HttpRequestMessage BuildRequestMessagee(ApiRequest request)
        {
            var requestMessage = new HttpRequestMessage {
                RequestUri = new Uri(BaseUri, request.RequestUrl),
                Method = request.HttpMethod,
                Content = String.IsNullOrEmpty(request.RequestBody) ? null : new StringContent(request.RequestBody, Encoding.UTF8, "application/json")
            };

            var token = _authenticator.GetAuthenticationToken(request);
            SetHttpRequestHeaders(requestMessage, token);

            return requestMessage;
        }

        private void SetHttpRequestHeaders(HttpRequestMessage requestMessage, AuthenticationToken token)
        {
            requestMessage.Headers.Add("CB-ACCESS-KEY", token.Key);
            requestMessage.Headers.Add("CB-ACCESS-SIGN", token.Signature);
            requestMessage.Headers.Add("CB-ACCESS-TIMESTAMP", token.Timestamp);
            requestMessage.Headers.Add("CB-ACCESS-PASSPHRASE", token.Passphrase);
        }

        protected virtual string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        protected virtual T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        protected virtual T DeserializeArray<T>(string array)
        {
            //var splitJArray = JArray.Parse(array);
            //List<object> arrayL = new List<object>();
            //foreach(JObject j in splitJArray.Children<>)
            //{

            //}
            //return arrayL.Cast<T>();
            return default(T);
            //return (T)Convert.ChangeType(array, typeof(T));
        }
    }
}