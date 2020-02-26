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
        private readonly HttpTools _httpTools;
        private readonly Logger _logger;

        public F5ToolsClient(string server)
        {
            _httpTools = new HttpTools();
            _logger = new Logger();

            _httpTools.SetBaseAddress($"https://{server}:443/mgmt/");
        }

        private void Log(string msg) => _logger.Log(msg, "F5ToolsClient");

        public async Task<bool> Login(string username, string password)
        {
            Log("Loggin in...");

            var success = await _httpTools.SendJson(
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

                     return true;
                 })
                .ConfigureAwait(false);

            if (success)
                Log("Login was successfull.");
            else
                Log("Login wasn't successfull.");

            return success;
        }

        public async Task<bool> EnablePoolMember(string poolName, string poolMemberName)
        {
            Log($"Enabling pool member: {poolMemberName} in pool: {poolName}...");

            var success = await _httpTools.SendJson(
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
                        Log($"Pool member: {poolMemberName} in Pool: {poolName} is now enabled.");
                    return done;
                },
                $"X-F5-Auth-Token:{_token}").ConfigureAwait(false);

            if (success)
                Log("Enabling was successfull.");
            else
                Log("Enabling wasn't successfull.");

            return success;
        }

        public async Task<bool> DisablePoolMember(string poolName, string poolMemberName)
        {
            Log($"Disabling pool member: {poolMemberName} in pool: {poolName}...");

            var success = await _httpTools.SendJson(
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
                    Log($"Pool member: {poolMemberName} in Pool: {poolName} is now disabled.");
                return done;
            },
            $"X-F5-Auth-Token:{_token}").ConfigureAwait(false);

            if (success)
                Log("Disabling was successfull.");
            else
                Log("Disabling wasn't successfull.");
            
            return success;
        }
    }
}