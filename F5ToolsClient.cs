using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace F5Tools
{
    public class F5ToolsClient
    {
        private string _token;
        private readonly HttpClient _client;

        public F5ToolsClient(string server)
        {
            var clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            };

            _client = new HttpClient(clientHandler)
            {
                BaseAddress = new Uri($"https://{server}:443/mgmt/")
            };
        }

        public async Task<bool> Login(string username, string password)
        {
            Console.WriteLine("Loggin in...");

            var success = await SendJson(
                 HttpMethod.Post,
                 "shared/authn/login",
                 JsonConvert.SerializeObject(new
                 {
                     username,
                     password,
                     loginProviderName = "tmos"
                 }),
                 response =>
                 {
                     _token = response.token.token;

                     Console.WriteLine($"Token: {_token}");

                     return true;
                 })
                .ConfigureAwait(false);

            if (success)
                Console.WriteLine("Login was successfull.");
            else
                Console.WriteLine("Login wasn't successfull.");

            Console.WriteLine();
            return success;
        }

        public async Task<bool> EnablePoolMember(string poolName, string poolMemberName)
        {
            Console.WriteLine($"Enabling pool member: {poolMemberName} in pool: {poolName}...");

            var success = await SendJson(
                HttpMethod.Put,
                $"tm/ltm/pool/~Common~{poolName}/members/~Common~{poolMemberName}",
                JsonConvert.SerializeObject(new
                {
                    session = "user-enabled"
                }),
                response =>
                {
                    var done = response.session == "monitor-enabled";
                    if (done)
                        Console.WriteLine($"Pool member: {poolMemberName} in Pool: {poolName} is now enabled.");
                    return done;
                },
                $"X-F5-Auth-Token:{_token}").ConfigureAwait(false);

            if (success)
                Console.WriteLine("Enabling was successfull.");
            else
                Console.WriteLine("Enabling wasn't successfull.");

            Console.WriteLine();
            return success;
        }

        public async Task<bool> DisablePoolMember(string poolName, string poolMemberName)
        {
            Console.WriteLine($"Disabling pool member: {poolMemberName} in pool: {poolName}...");

            var success = await SendJson(
            HttpMethod.Put,
            $"tm/ltm/pool/~Common~{poolName}/members/~Common~{poolMemberName}",
            JsonConvert.SerializeObject(new
            {
                session = "user-disabled"
            }),
            response =>
            {
                var done = response.session == "user-disabled";
                if (done)
                    Console.WriteLine($"Pool member: {poolMemberName} in Pool: {poolName} is now disabled.");
                return done;
            },
            $"X-F5-Auth-Token:{_token}").ConfigureAwait(false);

            if (success)
                Console.WriteLine("Disabling was successfull.");
            else
                Console.WriteLine("Disabling wasn't successfull.");

            Console.WriteLine();
            return success;
        }

        private async Task<T> SendJson<T>(HttpMethod method, string requestUri,
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