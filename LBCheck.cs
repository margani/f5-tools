using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace F5Tools
{
    public class LBCheck
    {
        private readonly F5ToolsClient f5Client;
        private readonly HttpTools _httpTools;
        private readonly AppOptions _options;
        private readonly Logger _logger;

        public LBCheck(AppOptions options)
        {
            _options = options;
            f5Client = new F5ToolsClient(_options.F5Server);
            _httpTools = new HttpTools();
            _logger = new Logger();
        }

        private void Log(string msg) => _logger.Log(msg, "LBCheck");

        public async Task<bool> Check()
        {
            Log("Checking");

            bool isDown = await IsDown(_options.UrlToCheck).ConfigureAwait(false);
            bool enabled = false,
                disabled = false,
                loginSuccessful = false;

            if (isDown)
            {
                loginSuccessful = f5Client.Login(_options.Username, _options.Password).Result;

                if (loginSuccessful)
                {
                    disabled = f5Client.DisablePoolMember(_options.PoolName, _options.PoolMemberName).Result;

                    enabled = f5Client.EnablePoolMember(_options.PoolName, _options.PoolMemberName).Result;
                }
            }

            Log("Finished" +
                (isDown ? $", Url: {_options.UrlToCheck} is down" : $", Url: {_options.UrlToCheck} is up, no need to reset the pool.") +
                (loginSuccessful ? ", Logged in" : isDown ? ", Couldn't login" : "") +
                (disabled ? ", Disabled the pool member" : isDown ? ", Couldn't disable the pool member" : "") +
                (enabled ? ", Enabled the pool member" : isDown ? ", Couldn't enable the pool member" : ""));

            return true;
        }

        private async Task<bool> IsDown(string urlToCheck)
        {
            var response = await _httpTools.GetResponseHeadersAsync(urlToCheck).ConfigureAwait(false);

            return !response.IsSuccessStatusCode;
        }
    }
}
