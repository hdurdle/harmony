using System;
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

            HarmonyClient client;
            if (File.Exists("SessionToken"))
            {
                var sessionToken = File.ReadAllText("SessionToken");
                Console.WriteLine("Reusing token: {0}", sessionToken);
                client = HarmonyClient.Create(options.IpAddress, sessionToken);
            }
            else
            {
                client = await HarmonyClient.Create(options.IpAddress, options.Username, options.Password);
                File.WriteAllText("SessionToken", client.Token);
            }

            using (client)
            {
                string deviceId = options.DeviceId;
                string activityId = options.ActivityId;
                // do we need to grab the config first?
                Config harmonyConfig = null;
                if (!string.IsNullOrEmpty(deviceId) || options.GetActivity || !string.IsNullOrEmpty(options.ListType))
                {
                    harmonyConfig = await client.GetConfigAsync();
                }

                if (!string.IsNullOrEmpty(deviceId) && !string.IsNullOrEmpty(options.Command))
                {
                    await client.PressButtonAsync(deviceId, options.Command);
                }

                if (null != harmonyConfig && !string.IsNullOrEmpty(deviceId) && string.IsNullOrEmpty(options.Command))
                {
                    // just list device control options
                    foreach (var device in harmonyConfig.Devices.Where(device => device.Id == deviceId))
                    {
                        foreach (ControlGroup controlGroup in device.ControlGroups)
                        {
                            foreach (Function f in controlGroup.Functions)
                            {
                                Console.WriteLine(f.ToString());
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(activityId))
                {
                    await client.StartActivityAsync(activityId);
                }

                if (null != harmonyConfig && options.GetActivity)
                {
                    var currentActivity = await client.GetCurrentActivityAsync();
                    Console.WriteLine("Current Activity: {0}", harmonyConfig.ActivityNameFromId(currentActivity));
                }

                if (options.TurnOff)
                {
                    await client.TurnOffAsync();
                }

                if (null != harmonyConfig && !string.IsNullOrEmpty(options.ListType))
                {
                    if (!options.ListType.Equals("d") && !options.ListType.Equals("a")) return;

                    if (options.ListType.Equals("a"))
                    {
                        Console.WriteLine("Activities:");
                        foreach (var activity in harmonyConfig.Activities.OrderBy(x => x.ActivityOrder))
                        {
                            Console.WriteLine(" {0}:{1}", activity.Id, activity.Label);
                        }
                    }

                    if (options.ListType.Equals("d"))
                    {
                        Console.WriteLine("Devices:");
                        foreach (var device in harmonyConfig.Devices.OrderBy(x => x.Label))
                        {
                            Console.WriteLine(device.ToString());
                        }
                    }
                }
            }
			Console.WriteLine("Ready");
			Console.ReadLine();
        }
    }
}