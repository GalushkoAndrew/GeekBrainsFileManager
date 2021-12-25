using System.IO;

namespace GeekBrains.Learn.FileManager.Repository
{
    /// <summary>
    /// File repository interface, CRUD
    /// </summary>
    public interface IFileRepository : IRepository
    {
        /// <summary>
        /// Create file
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>True if success</returns>
        public bool Create(string fileName);

        /// <summary>
        /// Read file
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>Returns the contents of the file as a string</returns>
        public string Read(string fileName);

        /// <summary>
        /// Update file
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="text">Contents of the file</param>
        /// <param name="isAppend">Append or rewrite file</param>
        /// <returns>True if success</returns>
        public bool Update(string fileName, string text, bool isAppend = true);

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>True if success</returns>
        public bool Delete(string fileName);

        /// <summary>
        /// Determines whether the given path refers to an existing directory on disk
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>True if exists</returns>
        public bool Exists(string fileName);

        /// <summary>
        /// Copies an existing file to a new file
        /// </summary>
        /// <param name="sourceFileName">The file to copy</param>
        /// <param name="destFileName">The name of the destination file</param>
        bool Copy(string sourceFileName, string destFileName);

        /// <summary>
        /// Gets file info
        /// </summary>
        /// <param name="fileName">File name</param>
        FileInfo GetFileInfo(string fileName);
    }
}
