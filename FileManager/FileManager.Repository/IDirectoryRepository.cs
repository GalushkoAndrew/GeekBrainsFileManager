using System.IO;

namespace GeekBrains.Learn.FileManager.Repository
{
    /// <summary>
    /// Directory repository interface, CRUD
    /// </summary>
    public interface IDirectoryRepository : IRepository
    {
        /// <summary>
        /// Determines whether the given path refers to an existing directory on disk
        /// </summary>
        /// <param name="folderName">Folder name</param>
        /// <returns>True if exists</returns>
        bool Exists(string folderName);

        /// <summary>
        /// Delete directory
        /// </summary>
        /// <param name="folderName">folder name</param>
        /// <param name="recursive">true to remove directories, subdirectories, and files in path; otherwise, false</param>
        bool Delete(string folderName, bool recursive);

        /// <summary>
        /// Gets directory info
        /// </summary>
        /// <param name="folderName">folder name</param>
        DirectoryInfo GetDirectoryInfo(string folderName);
    }
}
