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

        [Option('s', "start", Required = false,
            HelpText = "Activity ID to start")]
        public string ActivityId { get; set; }

        [Option('g', "get", Required = false,
            HelpText = "Get Current Activity")]
        public bool GetActivity { get; set; }

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

                    // show fixit commands for each activity
                    //foreach (var fixitCommand in activity.fixit)
                    //{
                    //    Console.WriteLine("   {0}:{1}:{2}", fixitCommand.Key, fixitCommand.Value.Input, fixitCommand.Value.Power);
                    //}
                }

                Console.WriteLine();
                Console.WriteLine("Devices:");
                harmonyConfig.device.Sort();
                foreach (var device in harmonyConfig.device)
                {
                    Console.WriteLine(" {0}:{1}", device.id, device.label);
                }

                if (!string.IsNullOrEmpty(deviceId))
                {
                    foreach (var device in harmonyConfig.device.Where(device => device.id == deviceId))
                    {
                        foreach (Dictionary<string, object> controlGroup in device.controlGroup)
                        {
                            foreach (var o in controlGroup)
                            {
                                if (o.Key == "name") Console.WriteLine("{0}:{1}", o.Key, o.Value);
                            }

                        }
                    }
                }

                if (!string.IsNullOrEmpty(activityId))
                {
                    var activityClient = new ActivityClient(ipAddress, harmonyPort, sessionToken);
                    activityClient.StartActivity(activityId);
                }

                if (options.GetActivity)
                {
                    var activityClient = new ActivityClient(ipAddress, harmonyPort, sessionToken);
                    activityClient.GetCurrentActivity();
                    // now wait for it to be populated
                    while (string.IsNullOrEmpty(activityClient.CurrentActivity)) { }
                    Console.WriteLine("Current Activity: {0}", harmonyConfig.ActivityNameFromId(activityClient.CurrentActivity));
                }

            } // option parsing
        }

        public static string LoginToLogitech(string email, string password, string ipAddress, int harmonyPort)
        {
            var authentication = new HarmonyAuthentication();

            string token = authentication.Login(email, password);
            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("Could not get token from Logitech server.");
            }

            string sessionToken = authentication.SwapAuthToken(ipAddress, harmonyPort, token);
            if (string.IsNullOrEmpty(sessionToken))
            {
                throw new Exception("Could not swap token on Harmony Hub.");
            }

            return sessionToken;
        }
    }
}
