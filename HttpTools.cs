using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace F5Tools
{
    public class HttpTools
    {
        private readonly HttpClient _client;

        public HttpTools()
        {
            var clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            };

            _client = new HttpClient(clientHandler);
        }

        public HttpTools SetBaseAddress(string baseUri)
        {
            _client.BaseAddress = new Uri(baseUri);
            return this;
        }

        public async Task<HttpResponseMessage> GetResponseHeadersAsync(string requestUri) =>
            await _client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

        public async Task<T> SendJson<T>(HttpMethod method, string requestUri,
            string json = null,
            Func<dynamic, T> resultFunc = null,
            params string[] headers)
        {
            var message = new HttpRequestMessage(method, requestUri)
            {
                Content = string.IsNullOrWhiteSpace(json) ? null : new StringContent(json, Encoding.UTF8, "application/json"),
            };

            foreach (var header in headers)
            {
                var indexOfColon = header.IndexOf(':');
                var headerName = header.Substring(0, indexOfColon);

                indexOfColon++; // skip the colon

                var headerValue = header[indexOfColon..];

                message.Headers.Add(headerName, headerValue);
            }

            var response = await _client.SendAsync(message).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var responseContentString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (resultFunc != null)
                return resultFunc(JsonConvert.DeserializeObject(responseContentString));

            return default;
        }
    }
}
