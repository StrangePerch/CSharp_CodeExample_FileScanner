using System;
using System.IO;
using System.Linq;

namespace FilesScanner
{
    public interface ICommand
    {
        public string GetName();
        public void Run(FileInfo[] files);
    }

    public class CountCommand : ICommand
    {
        public string GetName()
        {
            return "COUNT";
        }

        public void Run(FileInfo[] files)
        {
            Console.WriteLine(files.Length);
        }
    }

    public class SizeCommand : ICommand
    {
        public string GetName()
        {
            return "SIZE";
        }

        public void Run(FileInfo[] files)
        {
            var size = files.Sum(file => file.Length);
            switch (size)
            {
                case < 10000:
                    Console.WriteLine($"{size}B");
                    break;
                case < 10000000:
                    Console.WriteLine($"{size / 1024}KB");
                    break;
                case < 10000000000:
                    Console.WriteLine($"{size / Math.Pow(1024, 2)}MB");
                    break;
                case < 10000000000000:
                    Console.WriteLine($"{size / Math.Pow(1024, 3)}GB");
                    break;
            }
        }
    }

    public class PathCommand : ICommand
    {
        public string GetName()
        {
            return "PATH";
        }

        public void Run(FileInfo[] files)
        {
            var counter = 0;
            foreach (var file in files)
            {
                Console.WriteLine(file);
                counter++;
                if (counter <= 100) continue;
                Console.WriteLine($"100..{files.Length}");
                break;
            }
        }
    }
}