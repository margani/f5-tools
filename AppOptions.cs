using CommandLine;

namespace F5Tools
{
    public class AppOptions
    {
        [Option("server", Required = true, HelpText = "Set F5 server name or IP address")]
        public string F5Server { get; set; }

        [Option("username", Required = true, HelpText = "Set username")]
        public string Username { get; set; }

        [Option("password", Required = true, HelpText = "Set password")]
        public string Password { get; set; }

        [Option("pool", Required = true, HelpText = "Set pool name")]
        public string PoolName { get; set; }

        [Option("pool-member", Required = true, HelpText = "Set pool member name")]
        public string PoolMemberName { get; set; }

        [Option("url-to-check", Required = true, HelpText = "Set the url we want to check intervally")]
        public string UrlToCheck { get; set; }
    }
}
