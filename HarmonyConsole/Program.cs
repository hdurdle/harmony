using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using HarmonyHub;
using HarmonyHub.Entities;

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

            string sessionToken;

            if (File.Exists("SessionToken"))
            {
                sessionToken = File.ReadAllText("SessionToken");
                Console.WriteLine("Reusing token: {0}", sessionToken);
            }
            else
            {
                sessionToken = await HarmonyLogin.LoginToLogitechAsync(options.Username, options.Password, options.IpAddress, HarmonyPort);
            }

            // do we need to grab the config first?
            HarmonyConfigResult harmonyConfig = null;

            HarmonyClient client = null;

            string deviceId = options.DeviceId;
            string activityId = options.ActivityId;
            if (!string.IsNullOrEmpty(deviceId) || options.GetActivity || !string.IsNullOrEmpty(options.ListType))
            {
                client = new HarmonyClient(options.IpAddress, HarmonyPort, sessionToken);
                harmonyConfig = await client.GetConfigAsync();
            }

            if (!string.IsNullOrEmpty(deviceId) && !string.IsNullOrEmpty(options.Command))
            {
                if (null == client)
                {
                    client = new HarmonyClient(options.IpAddress, HarmonyPort, sessionToken);
                }
                await client.PressButtonAsync(deviceId, options.Command);
            }

            if (null != harmonyConfig && !string.IsNullOrEmpty(deviceId) && string.IsNullOrEmpty(options.Command))
            {
                // just list device control options
                foreach (var device in harmonyConfig.Device.Where(device => device.Id == deviceId))
                {
                    foreach (Dictionary<string, object> controlGroup in device.ControlGroup)
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
                    client = new HarmonyClient(options.IpAddress, HarmonyPort, sessionToken);
                }
                await client.StartActivityAsync(activityId);
            }

            if (null != harmonyConfig && options.GetActivity)
            {
                var currentActivity = await client.GetCurrentActivityAsync();
                Console.WriteLine("Current Activity: {0}", harmonyConfig.ActivityNameFromId(currentActivity));
            }

            if (options.TurnOff)
            {
                if (null == client)
                {
                    client = new HarmonyClient(options.IpAddress, HarmonyPort, sessionToken);
                }
                await client.TurnOffAsync();
            }

            if (null != harmonyConfig && !string.IsNullOrEmpty(options.ListType))
            {
                if (!options.ListType.Equals("d") && !options.ListType.Equals("a")) return;

                if (options.ListType.Equals("a"))
                {
                    Console.WriteLine("Activities:");
                    foreach (var activity in harmonyConfig.Activity.OrderBy(x => x.ActivityOrder))
                    {
                        Console.WriteLine(" {0}:{1}", activity.Id, activity.Label);
                    }
                }

                if (options.ListType.Equals("d"))
                {
                    Console.WriteLine("Devices:");
                    foreach (var device in harmonyConfig.Device.OrderBy(x => x.Label))
                    {
                        Console.WriteLine($" {device.Id}:{device.Label}");
                    }
                }
            }
        }
    }
}