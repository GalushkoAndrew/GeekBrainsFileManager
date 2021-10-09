using System;
using System.IO;

namespace GeekBrains.Learn.FileManager
{
    /// <summary>
    /// Управляет логированием
    /// </summary>
    public class Logger
    {
        public static void Log(string text)
        {
            string path = "errors/log_exception.txt";

            var folderErrors = Path.GetDirectoryName(path);
            if (!Directory.Exists(folderErrors))
            {
                Directory.CreateDirectory(folderErrors);
            }

            using (StreamWriter sw = File.AppendText("errors/log_exception.txt"))
            {
                sw.WriteLine(DateTime.Now.ToString() + ", " + text);
            }
        }
    }
}
