using System.IO;

namespace GeekBrains.Learn.FileManager.Repository
{
    /// <summary>
    /// Directory repository
    /// </summary>
    public class DirectoryRepository : IDirectoryRepository
    {
        /// <inheritdoc/>
        public bool Delete(string folderName, bool recursive)
        {
            try
            {
                Directory.Delete(folderName, recursive: true);
            }
            catch (System.Exception)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public bool Exists(string folderName)
        {
            return Directory.Exists(folderName);
        }

        /// <inheritdoc/>
        public DirectoryInfo GetDirectoryInfo(string folderName)
        {
            return new DirectoryInfo(folderName);
        }
    }
}
