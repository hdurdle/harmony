using agsXMPP.Xml.xpnet;
using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;

namespace Harmony
{
    class Options
    {
        [Option('i', "ip", Required = true,
            HelpText = "IP Address of Harmony Hub.")]
        public string IpAddress { get; set; }

        [Option('u', "user", Required = true,
            HelpText = "Logitech Username")]
        public string Username { get; set; }

        [Option('p', "pass", Required = true,
            HelpText = "Logitech Password")]
        public string Password { get; set; }

        [Option('d', "device", Required = false,
            HelpText = "Device ID")]
        public string DeviceId { get; set; }

        [Option('c', "command", Required = false,
            HelpText = "Command to send to device")]
        public string Command { get; set; }

        [Option('s', "start", Required = false,
            HelpText = "Activity ID to start")]
        public string ActivityId { get; set; }

        [Option('l', "list", Required = false,
            HelpText = "List devices or activities")]
        public string ListType { get; set; }

        [Option('g', "get", Required = false,
            HelpText = "Get Current Activity")]
        public bool GetActivity { get; set; }

        [Option('t', "turnoff", Required = false,
            HelpText = "Turn system off")]
        public bool TurnOff { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            const int harmonyPort = 5222;

            var options = new Options();
            if (Parser.Default.ParseArguments(args, options))
            {
                Console.WriteLine();

                string ipAddress = options.IpAddress;
                string username = options.Username;
                string password = options.Password;

                string deviceId = options.DeviceId;
                string activityId = options.ActivityId;

                Dns.GetHostEntry(ipAddress);

                string sessionToken = LoginToLogitech(username, password, ipAddress, harmonyPort);

                var client = new ConfigClient(ipAddress, harmonyPort, sessionToken);
                client.GetConfig();

                while (string.IsNullOrEmpty(client.Config)) { }

                var harmonyConfig = new JavaScriptSerializer().Deserialize<HarmonyConfigResult>(client.Config);

                Console.WriteLine("Activities:");
                harmonyConfig.activity.Sort();
                foreach (var activity in harmonyConfig.activity)
                {
                    Console.WriteLine(" {0}:{1}", activity.id, activity.label);
                }

                ActivityClient activityClient = null;

                Console.WriteLine();
                Console.WriteLine("Devices:");
                harmonyConfig.device.Sort();
                foreach (var device in harmonyConfig.device)
                {
                    Console.WriteLine(" {0}:{1}", device.id, device.label);
                }

                if (!string.IsNullOrEmpty(options.DeviceId) && !string.IsNullOrEmpty(options.Command))
                {
                    if (null == activityClient) activityClient = new ActivityClient(ipAddress, harmonyPort, sessionToken);
                    //activityClient.PressButton("14766260", "Mute");
                    activityClient.PressButton(options.DeviceId, options.Command);
                }

                if (!string.IsNullOrEmpty(deviceId) && string.IsNullOrEmpty(options.Command))
                {
                    // just list device control options
                    foreach (var device in harmonyConfig.device.Where(device => device.id == deviceId))
                    {
                        foreach (Dictionary<string, object> controlGroup in device.controlGroup)
                        {
                            foreach (var o in controlGroup.Where(o => o.Key == "name"))
                            {
                                Console.WriteLine("{0}:{1}", o.Key, o.Value);
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(activityId))
                {
                    if (null == activityClient) activityClient = new ActivityClient(ipAddress, harmonyPort, sessionToken);
                    activityClient.StartActivity(activityId);
                }

                if (options.GetActivity)
                {
                    if (null == activityClient) activityClient = new ActivityClient(ipAddress, harmonyPort, sessionToken);
                    activityClient.GetCurrentActivity();
                    // now wait for it to be populated
                    while (string.IsNullOrEmpty(activityClient.CurrentActivity)) { }
                    Console.WriteLine("Current Activity: {0}", harmonyConfig.ActivityNameFromId(activityClient.CurrentActivity));
                }

                if (options.TurnOff)
                {
                    if (null == activityClient) activityClient = new ActivityClient(ipAddress, harmonyPort, sessionToken);
                    activityClient.TurnOff();
                }

            } // option parsing
        }

        public static string LoginToLogitech(string email, string password, string ipAddress, int harmonyPort)
        {
            string token = HarmonyLogin.Login(email, password);
            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("Could not get token from Logitech server.");
            }

            var authentication = new HarmonyAuthenticationClient(ipAddress, harmonyPort, token);

            string sessionToken = authentication.SwapAuthToken(token);
            if (string.IsNullOrEmpty(sessionToken))
            {
                throw new Exception("Could not swap token on Harmony Hub.");
            }

            return sessionToken;
        }
    }
}
