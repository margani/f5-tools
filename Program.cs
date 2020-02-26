using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace F5Tools
{
    internal static class Program
    {
        private static readonly Logger _logger;

        static Program()
        {
            _logger = new Logger();
        }

        private static void Log(string msg) => _logger.Log(msg, "Program");

        private static void Main(string[] args) => 
            Parser.Default.ParseArguments<AppOptions>(args).MapResult(RunOptionsAndReturnExitCode, HandleParseError);

        private static int RunOptionsAndReturnExitCode(AppOptions options)
        {
            var exitCode = 0;

            _ = new LBCheck(options).Check().Result;

            return exitCode;
        }

        private static int HandleParseError(IEnumerable<Error> errs)
        {
            var result = -2;

            Log($"App arguments parsing errors {errs.Count()}");
            if (errs.Any(x => x is HelpRequestedError || x is VersionRequestedError))
                result = -1;

            return result;
        }
    }
}
