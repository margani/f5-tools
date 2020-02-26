using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace F5Tools
{
    internal static class Program
    {
        public class Options
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
        }

        private static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args).MapResult(RunOptionsAndReturnExitCode, HandleParseError);

            Console.WriteLine($"Exit program, Code: {result}");
        }

        private static int RunOptionsAndReturnExitCode(Options options)
        {
            var exitCode = 0;

            var client = new F5ToolsClient(options.F5Server);

            var loginSuccessful = client.Login(options.Username, options.Password).Result;
            if (!loginSuccessful)
                return -1;

            _ = client.DisablePoolMember(options.PoolName, options.PoolMemberName).Result;

            _ = client.EnablePoolMember(options.PoolName, options.PoolMemberName).Result;

            Console.WriteLine("Finished the tasks");

            return exitCode;
        }

        private static int HandleParseError(IEnumerable<Error> errs)
        {
            var result = -2;
            Console.WriteLine("App arguments parsing errors {0}", errs.Count());
            if (errs.Any(x => x is HelpRequestedError || x is VersionRequestedError))
                result = -1;
            return result;
        }
    }
}
