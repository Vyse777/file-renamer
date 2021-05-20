using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace FileRenamer
{
    internal class Program
    {
        // Renames directories that are in the format MM-DD-YYYY into YYYY-MM-DD format
        private static void Main(string[] args)
        {
            var directoryNames = Directory.GetDirectories(Environment.CurrentDirectory).Select(x => new DirectoryInfo(x).Name);

            foreach (var directory in directoryNames)
            {
                var directoryNameParts = directory.Split("-");
                var newDirectoryName = $"{directoryNameParts[2]}-{directoryNameParts[0]}-{directoryNameParts[1]}";
                TryRenameDirectory(directory, newDirectoryName);
            }
        }

        private static void TryRenameDirectory(string oldFilename, string newFilename)
        {
            try
            {
                Directory.Move(oldFilename, newFilename);
            }
            catch (IOException e)
            {
                // If any error occurs log it to a newly created file.
                File.WriteAllText(Environment.CurrentDirectory + "\\ErrorWhenAutoRenaming.txt",
                    $"An error occurred when trying to rename \"{oldFilename}\" to \"{newFilename}\" + \\n + {e.Message}");
            }
        }
    }
}