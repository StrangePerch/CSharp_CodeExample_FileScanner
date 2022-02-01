using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FilesScanner
{
    public interface IFileScanner
    {
        public FileInfo[] GetFiles();
        public FileInfo[] GetFilesFullPath();
    }

    public class Scanner : IFileScanner
    {
        private readonly string _path;
        private readonly List<FileInfo> _files = new();

        private readonly List<FileInfo> _fullPathFiles = new();

        private readonly object _filesLocker = new();
        private readonly object _fullPathFilesLocker = new();

        public Scanner(string path)
        {
            _path = path;
        }

        public FileInfo[] GetFiles()
        {
            lock (_filesLocker)
            {
                return _files.ToArray();
            }
        }

        public FileInfo[] GetFilesFullPath()
        {
            lock (_fullPathFilesLocker)
            {
                return _fullPathFiles.ToArray();
            }
        }

        public void Start()
        {
            try
            {
                var dir = new DirectoryInfo(_path);
                if (!dir.Exists) throw new DirectoryNotFoundException();

                var subDirs = dir.GetDirectories();
                var files = dir.GetFiles();
                PushFiles(files);
                var tasks = subDirs.Select(ScanDirectoryAsync).ToArray();
                Task.WaitAll(tasks);
                PushFilesFullPath(files);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("No access to provided directory");
                Console.WriteLine(e);
            }
        }

        private void PushFiles(IEnumerable<FileInfo> files)
        {
            lock (_filesLocker)
            {
                var toAdd = files.Where(file => file.Extension != "").ToArray();
                _files.AddRange(toAdd);
            }
        }

        private void PushFilesFullPath(IEnumerable<FileInfo> files)
        {
            lock (_fullPathFilesLocker)
            {
                var toAdd = files.Where(file => file.Extension != "").ToArray();
                _fullPathFiles.AddRange(toAdd);
            }
        }

        private async Task ScanDirectoryAsync(DirectoryInfo dir)
        {
            await Task.Run(async () =>
            {
                try
                {
                    var subDirs = dir.GetDirectories();
                    var files = dir.GetFiles();
                    PushFiles(files);
                    foreach (var subDir in subDirs) await ScanDirectoryAsync(subDir);
                    PushFilesFullPath(files);
                }
                catch (UnauthorizedAccessException)
                {
                }
            });
        }
    }
}