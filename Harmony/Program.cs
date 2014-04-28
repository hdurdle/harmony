using System;
using System.Net;
using System.Web.Script.Serialization;
using CommandLine;
using CommandLine.Text;

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

            string ipAddress;
            string username;
            string password;

            var options = new Options();
            if (Parser.Default.ParseArguments(args, options))
            {
                ipAddress = options.IpAddress;
                username = options.Username;
                password = options.Password;
            }
            else
            {
                return;
            }

            Dns.GetHostEntry(ipAddress);

            string sessionToken = LoginToLogitech(username, password, ipAddress, harmonyPort);

            var client = new ConfigClient(ipAddress, harmonyPort, sessionToken);
            client.GetConfig();

            // now wait for it to be populated
            while (string.IsNullOrEmpty(client.Config))
            { }

            var harmonyConfig = new JavaScriptSerializer().Deserialize<HarmonyConfigResult>(client.Config);

            Console.WriteLine("Activities:");
            foreach (var activity in harmonyConfig.activity)
            {
                Console.WriteLine(" {0}:{1}", activity.id, activity.label);
            }

            Console.WriteLine("Devices:");
            foreach (var device in harmonyConfig.device)
            {
                Console.WriteLine(" {0}:{1}", device.id, device.label);
            }

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
