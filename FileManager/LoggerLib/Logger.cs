namespace GeekBrains.Learn.FileManager.LoggerLib
{
    /// <summary>
    /// Logger base class
    /// </summary>
    public abstract class Logger : ILogger
    {
        /// <summary>
        /// Sends message
        /// </summary>
        public abstract void SendLine(string value);
    }
}