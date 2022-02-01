using System;
using System.IO;

namespace FilesScanner
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string path;
            if (args.Length < 1)
            {
                Console.Write("No path found in arguments");
                Console.Write("Path: ");
                path = Console.ReadLine();
            }
            else
            {
                path = args[0];
            }

            var scanner = new Scanner(path);
            var commandHandler = new CommandHandler(scanner,
                new ICommand[]
                {
                    new CountCommand(),
                    new PathCommand(),
                    new SizeCommand()
                });
            var task = commandHandler.StartAsync();
            try
            {
                scanner.Start();
                Console.WriteLine("Scan finished successfully");
                task.Wait();
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Directory not found");
            }
        }
    }
}