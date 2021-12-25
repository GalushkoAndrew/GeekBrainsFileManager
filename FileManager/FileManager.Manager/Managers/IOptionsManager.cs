using GeekBrains.Learn.FileManager.Domain;

namespace GeekBrains.Learn.FileManager.Manager
{
    /// <summary>
    /// Options manager interface
    /// </summary>
    public interface IOptionsManager : IManager
    {
        /// <summary>
        /// Returns path to work folder
        /// </summary>
        public string GetWorkPath();

        /// <summary>
        /// Save path to work folder
        /// </summary>
        /// <param name="path">Work path</param>
        /// <returns>True if success</returns>
        public bool SetWorkPath(string path);

        /// <summary>
        /// Save options
        /// </summary>
        /// <param name="options">Options</param>
        /// <returns>True if success</returns>
        public bool SetOptions(IOptions options);

        /// <summary>
        /// Returns options
        /// </summary>
        public IOptions GetOptions();
    }
}
