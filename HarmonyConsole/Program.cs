using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using CommandLine;
using HarmonyHub;

namespace HarmonyConsole
{
    public class Program
    {
        private const int HarmonyPort = 5222;

        public static void Main(string[] args)
        {
            Task.Run(async () => await MainAsync(args)).Wait();
        }

        public static async Task MainAsync(string[] args)
        {
            var options = new Options();
            if (!Parser.Default.ParseArguments(args, options))
            {
                return;
            }
            Console.WriteLine();

            string ipAddress = options.IpAddress;
            string username = options.Username;
            string password = options.Password;

            string deviceId = options.DeviceId;
            string activityId = options.ActivityId;

            string sessionToken;

            if (File.Exists("SessionToken"))
            {
                sessionToken = File.ReadAllText("SessionToken");
                Console.WriteLine("Reusing token: {0}", sessionToken);
            }
            else
            {
                sessionToken = await HarmonyLogin.LoginToLogitechAsync(username, password, ipAddress, HarmonyPort).ConfigureAwait(false);
            }

            // do we need to grab the config first?
            HarmonyConfigResult harmonyConfig = null;

            HarmonyClient client = null;

            if (!string.IsNullOrEmpty(deviceId) || options.GetActivity || !string.IsNullOrEmpty(options.ListType))
            {
                client = new HarmonyClient(ipAddress, HarmonyPort, sessionToken);
                await client.LoginAsync().ConfigureAwait(false);
                var config = await client.GetConfigAsync().ConfigureAwait(false);
                File.WriteAllText("HubConfig", config);
                harmonyConfig = new JavaScriptSerializer().Deserialize<HarmonyConfigResult>(config);
            }

            if (!string.IsNullOrEmpty(deviceId) && !string.IsNullOrEmpty(options.Command))
            {
                if (null == client)
                {
                    client = new HarmonyClient(ipAddress, HarmonyPort, sessionToken);
                    await client.LoginAsync().ConfigureAwait(false);
                }
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
                            Console.WriteLine($"{o.Key}:{o.Value}");
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(activityId))
            {
                if (null == client)
                {
                    client = new HarmonyClient(ipAddress, HarmonyPort, sessionToken);
                    await client.LoginAsync().ConfigureAwait(false);
                }
                await client.StartActivityAsync(activityId).ConfigureAwait(false);
            }

            if (null != harmonyConfig && options.GetActivity)
            {
                var currentActivity = await client.GetCurrentActivityAsync().ConfigureAwait(false);
                Console.WriteLine("Current Activity: {0}", harmonyConfig.ActivityNameFromId(currentActivity));
            }

            if (options.TurnOff)
            {
                if (null == client)
                {
                    client = new HarmonyClient(ipAddress, HarmonyPort, sessionToken);
                    await client.LoginAsync().ConfigureAwait(false);
                }
                await client.TurnOffAsync().ConfigureAwait(false);
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
                        Console.WriteLine($" {device.id}:{device.label}");
                    }
                }
            }
        }
    }
}