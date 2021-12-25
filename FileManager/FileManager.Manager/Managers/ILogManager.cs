namespace GeekBrains.Learn.FileManager.Manager
{
    /// <summary>
    /// Log manager interface
    /// </summary>
    public interface ILogManager : IManager
    {
        /// <summary>
        /// Log message
        /// </summary>
        /// <param name="text">Message text</param>
        void Log(string text);
    }
}
