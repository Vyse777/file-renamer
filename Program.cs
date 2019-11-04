using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace FileRenamer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var filenames = Directory.GetFiles(Environment.CurrentDirectory).Select(x => Path.GetFileName(x)).Where(filename => filename.ToLowerInvariant().Substring(filename.Length - 4).Equals(".mkv"));
            foreach (var filename in filenames)
            {
                var filenameParts = filename.Split('.');
                if (filenameParts.Length == 2) return; // Do nothing if the file has already been renamed

                for (int i = 0; i < filenameParts.Length; i++)
                {
                    var val = filenameParts[i];

                    // Find the year in the filename
                    if (DateTime.TryParseExact(val, "yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime year))
                    {
                        if (i == 0) continue; // This covers a really rare edge case when the year is in the title as the first word. Example: 2001.A.Space.Odyssey.1968.1080p....mkv

                        var newFilenameParts = filenameParts.Take(i);
                        var formattedYear = $"({filenameParts[i]})";

                        // Rename the file and move on to the next mkv.
                        TryRenameFile(filename, string.Join(" ", newFilenameParts.Append(formattedYear)), ".mkv", true);
                        break;
                    }
                }
            }
        }

        private static void TryRenameFile(string oldFilename, string newFilename, string fileTypePostfix, bool placeFileInNameMatchingDir)
        {
            var filePath = newFilename;
            if (placeFileInNameMatchingDir)
            {
                Directory.CreateDirectory(filePath);
                filePath = filePath + "\\" + newFilename;
            }

            try
            {
                File.Move(oldFilename, filePath + fileTypePostfix);
            }
            catch (IOException e)
            {
                // TODO: Devise a renaming strategy for if the file exists at the current directory.
                // For now, since the user initiated this in the directory via the Windows Explorer Context, we can output the exception to a file named accordingly.
                File.WriteAllText(Environment.CurrentDirectory + "\\ErrorWhenAutoRenaming.txt",
                    $"An error occurred when trying to rename \"{oldFilename}\" to \"{newFilename}\" A file with that name might already exist");
            }
        }
    }
}