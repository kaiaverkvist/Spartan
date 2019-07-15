using System;
using Spartan.Core;

namespace Spartan.Dev
{
    class Program
    {
        static void Main(string[] args)
        {
            SpartanConfiguration config = new SpartanConfiguration();
            SpartanServer server = new SpartanServer(config, port: 8084);

            RequestLogger logger = new RequestLogger(server.HttpHandler);

            server.Run();

            // Print out valid prefixes:
            string validPrefixes = "";
            foreach (string prefix in server.Listener.Prefixes)
            {
                validPrefixes = validPrefixes + prefix + "\n";
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Valid prefixes:\n{validPrefixes}");

            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Running Spartan.Dev...");
            Console.ReadKey();
            server.Stop();
        }
    }
}
