using System.IO;

namespace Mercury.Helpers;

public static class DirectoryHelper
{
    public static void CopyAll(string sourceDir, string destDir, bool overwrite = true)
    {
        if (sourceDir.ToLower() == destDir.ToLower()
            || string.IsNullOrWhiteSpace(sourceDir)
            || string.IsNullOrWhiteSpace(destDir))
            return;
        
        if (!Directory.Exists(destDir))
            Directory.CreateDirectory(destDir);
        
        /* Copy Files in sourceDir */
        foreach (string file in Directory.GetFiles(sourceDir))
            File.Copy(file, Path.Combine(destDir, Path.GetFileName(file)), overwrite);
        
        /* Copy subdirectory and its files with recursion */
        foreach (string dir in Directory.GetDirectories(sourceDir))
        {
            CopyAll(dir, Path.Combine(destDir, Path.GetFileName(dir)), overwrite);
        }
    }
}