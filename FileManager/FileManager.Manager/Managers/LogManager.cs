using System;
using System.IO;
using System.Text;
using GeekBrains.Learn.FileManager.Repository;
using GeekBrains.Learn.FileManager.Shared;

namespace GeekBrains.Learn.FileManager.Manager
{
    /// <summary>
    /// Log manager
    /// </summary>
    public class LogManager : ILogManager
    {
        private readonly IFileRepository _fileRepository;

        /// <summary>
        /// ctor
        /// </summary>
        public LogManager()
        {
            _fileRepository = new FileRepository();
        }

        /// <summary>
        /// Соединяет массив строк в одну строку
        /// </summary>
        public static string ArraytoString(string[] paramArray, bool isAddSpaceDelimiter = false)
        {
            StringBuilder sb = new();
            string delimiter = isAddSpaceDelimiter ? " " : "";

            foreach (var item in paramArray)
            {
                sb.Append(item ?? "");
                sb.Append(delimiter);
            }

            return sb.ToString();
        }

        /// <inheritdoc/>
        public void Log(string text)
        {
            string path = Constants.LogFileName;

            var folderErrors = Path.GetDirectoryName(path);

            try
            {
                if (!Directory.Exists(folderErrors))
                {
                    Directory.CreateDirectory(folderErrors);
                }

                _fileRepository.Update(path, DateTime.Now.ToString() + ", " + text);
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
