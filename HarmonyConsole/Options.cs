using CommandLine;
using CommandLine.Text;

namespace HarmonyConsole
{
    /// <summary>
    /// This is used to parse the commandline arguments
    /// </summary>
    internal class Options
    {
        [Option('s', "start", Required = false, HelpText = "Activity ID to start")]
        public string ActivityId { get; set; }

        [Option('c', "command", Required = false, HelpText = "Command to send to device")]
        public string Command { get; set; }

        [Option('d', "device", Required = false, HelpText = "Device ID")]
        public string DeviceId { get; set; }

        [Option('g', "get", Required = false, HelpText = "Get Current Activity")]
        public bool GetActivity { get; set; }

        [Option('i', "ip", Required = true, HelpText = "IP Address of Harmony Hub.")]
        public string IpAddress { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [Option('l', "list", Required = false, HelpText = "List devices or activities")]
        public string ListType { get; set; }

        [Option('p', "pass", Required = true, HelpText = "Logitech Password")]
        public string Password { get; set; }

        [Option('o', "off", Required = false, HelpText = "Turn entire system off")]
        public bool TurnOff { get; set; }

        [Option('u', "user", Required = true, HelpText = "Logitech Username")]
        public string Username { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}