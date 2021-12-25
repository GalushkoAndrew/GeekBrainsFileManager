using System;
using System.IO;

namespace GeekBrains.Learn.FileManager.Repository
{
    /// <summary>
    /// File repository
    /// </summary>
    public class FileRepository : IFileRepository
    {
        /// <inheritdoc/>
        public bool Copy(string sourceFileName, string destFileName)
        {
            try
            {
                File.Copy(sourceFileName, destFileName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public bool Create(string fileName)
        {
            try
            {
                File.Create(fileName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public bool Delete(string fileName)
        {
            try
            {
                File.Delete(fileName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public bool Exists(string fileName)
        {
            try
            {
                return File.Exists(fileName);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public FileInfo GetFileInfo(string fileName)
        {
            try
            {
                return new FileInfo(fileName);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public string Read(string fileName)
        {
            try
            {
                return File.ReadAllText(fileName);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public bool Update(string fileName, string text, bool isAppend = true)
        {
            try
            {
                if (!isAppend)
                {
                    File.WriteAllText(fileName, text);
                    return true;
                }

                using StreamWriter sw = File.AppendText(fileName);
                sw.WriteLine(text);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
