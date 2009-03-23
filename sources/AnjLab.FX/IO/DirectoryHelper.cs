using System.IO;
using AnjLab.FX.Sys;

namespace AnjLab.FX.IO
{
    public class DirectoryHelper
    {
        public static void CopyDirectory(string sourceDir, string copyPath)
        {
            Guard.IsTrue(Directory.Exists(sourceDir));

            if (!Directory.Exists(copyPath))
                Directory.CreateDirectory(copyPath);

            foreach (string file in Directory.GetFiles(sourceDir))
                File.Copy(file, Path.Combine(copyPath, Path.GetFileName(file)));

            foreach (string dir in Directory.GetDirectories(sourceDir))
                CopyDirectory(dir, Path.Combine(copyPath, Path.GetDirectoryName(dir)));
        }
    }
}
