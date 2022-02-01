using System;
using System.Linq;
using System.Threading.Tasks;

namespace FilesScanner
{
    public class CommandHandler
    {
        private readonly ICommand[] _commands;
        private readonly IFileScanner _scanner;

        public CommandHandler(IFileScanner scanner, ICommand[] commands)
        {
            _scanner = scanner;
            _commands = commands;
        }

        public async Task StartAsync()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    var input = Console.ReadLine()?.Split(" ");
                    if (!CheckInput(input)) continue;
                    var name = input[0];
                    var extension = input.Length > 1 ? input[1] : ".*";
                    if (!Invoke(name, extension)) Console.WriteLine("Command not found");
                }
            });
        }

        private bool CheckInput(string[] input)
        {
            if (input == null)
            {
                Console.WriteLine("No command found");
                return false;
            }

            if (input.Length is < 1 or > 2)
            {
                Console.WriteLine("Wrong amount of arguments");
                return false;
            }

            return true;
        }

        private bool Invoke(string commandName, string extension)
        {
            foreach (var command in _commands)
            {
                if (commandName.ToUpper() != command.GetName()) continue;

                var files = commandName == "PATH" ? _scanner.GetFilesFullPath() : _scanner.GetFiles();
                var selection = extension == ".*"
                    ? files.Where(file => file != null).ToArray()
                    : files.Where(file => file != null
                                          && file.Extension == extension).ToArray();
                command.Run(selection);
                return true;
            }

            return false;
        }
    }
}