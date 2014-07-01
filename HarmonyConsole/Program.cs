using System.IO;
using CommandLine;
using CommandLine.Text;
using HarmonyHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;

namespace HarmonyConsole
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

        [Option('o', "off", Required = false,
            HelpText = "Turn entire system off")]
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

                string sessionToken;

                if (File.Exists("SessionToken"))
                {
                    sessionToken = File.ReadAllText("SessionToken");
                    Console.WriteLine("Reusing token: {0}", sessionToken);
                }
                else
                {
                    sessionToken = LoginToLogitech(username, password, ipAddress, harmonyPort);
                }

                // do we need to grab the config first?
                HarmonyConfigResult harmonyConfig = null;

                HarmonyClient client = null;

                if (!string.IsNullOrEmpty(deviceId) || options.GetActivity || !string.IsNullOrEmpty(options.ListType))
                {
                    client = new HarmonyClient(ipAddress, harmonyPort, sessionToken);
                    client.GetConfig();

                    while (string.IsNullOrEmpty(client.Config)) { }
                    File.WriteAllText("HubConfig", client.Config);
                    harmonyConfig = new JavaScriptSerializer().Deserialize<HarmonyConfigResult>(client.Config);

                }

                if (!string.IsNullOrEmpty(deviceId) && !string.IsNullOrEmpty(options.Command))
                {
                    if (null == client) client = new HarmonyClient(ipAddress, harmonyPort, sessionToken);
                    //activityClient.PressButton("14766260", "Mute");
                    client.PressButton(deviceId, options.Command);
                }

                if (null != harmonyConfig && !string.IsNullOrEmpty(deviceId) && string.IsNullOrEmpty(options.Command))
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
                    if (null == client) client = new HarmonyClient(ipAddress, harmonyPort, sessionToken);
                    client.StartActivity(activityId);
                }

                if (null != harmonyConfig && options.GetActivity)
                {
                    client.GetCurrentActivity();
                    // now wait for it to be populated
                    while (string.IsNullOrEmpty(client.CurrentActivity)) { }
                    Console.WriteLine("Current Activity: {0}", harmonyConfig.ActivityNameFromId(client.CurrentActivity));
                }

                if (options.TurnOff)
                {
                    if (null == client) client = new HarmonyClient(ipAddress, harmonyPort, sessionToken);
                    client.TurnOff();
                }

                if (null != harmonyConfig && !string.IsNullOrEmpty(options.ListType))
                {
                    if (!options.ListType.Equals("d") && !options.ListType.Equals("a")) return;

                    if (options.ListType.Equals("a"))
                    {
                        Console.WriteLine("Activities:");
                        harmonyConfig.activity.Sort();
                        foreach (var activity in harmonyConfig.activity)
                        {
                            Console.WriteLine(" {0}:{1}", activity.id, activity.label);
                        }
                    }

                    if (options.ListType.Equals("d"))
                    {
                        Console.WriteLine("Devices:");
                        harmonyConfig.device.Sort();
                        foreach (var device in harmonyConfig.device)
                        {
                            Console.WriteLine(" {0}:{1}", device.id, device.label);
                        }
                    }
                }

            } // option parsing
        }

        public static string LoginToLogitech(string email, string password, string ipAddress, int harmonyPort)
        {
            string userAuthToken = HarmonyLogin.GetUserAuthToken(email, password);
            if (string.IsNullOrEmpty(userAuthToken))
            {
                throw new Exception("Could not get token from Logitech server.");
            }

            File.WriteAllText("UserAuthToken", userAuthToken);

            var authentication = new HarmonyAuthenticationClient(ipAddress, harmonyPort);

            string sessionToken = authentication.SwapAuthToken(userAuthToken);
            if (string.IsNullOrEmpty(sessionToken))
            {
                throw new Exception("Could not swap token on Harmony Hub.");
            }

            File.WriteAllText("SessionToken", sessionToken);

            Console.WriteLine("Date Time : {0}", DateTime.Now);
            Console.WriteLine("User Token: {0}", userAuthToken);
            Console.WriteLine("Sess Token: {0}", sessionToken);

            return sessionToken;
        }
    }
}
